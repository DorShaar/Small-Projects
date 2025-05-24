using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Kafka_Usages;

public class MessagesProducer : IMessagesProducer
{
	private readonly IMessagesProducerFactory _MessagesProducerFactory;
	private readonly KafkaConnectionInfo _ConnectionConfig;
	private IProducer<string, string> _Producer;
	private readonly SemaphoreSlim _PublishMessageSemaphore = new(1, 1);
	private readonly ILogger<MessagesProducer> _Logger;

	public MessagesProducer(IMessagesProducerFactory messagesProducerFactory,
							KafkaConnectionInfo connectionInfo,
							ILogger<MessagesProducer> logger)
	{
		_MessagesProducerFactory = messagesProducerFactory;
		_ConnectionConfig = connectionInfo;
		_Logger = logger;
		_Producer = _MessagesProducerFactory.CreateProducer(connectionInfo);
	}

	public void Dispose()
	{
		_Logger.LogDebug($"Disposing {nameof(MessagesProducer)}");
		_Producer.Flush();
		_Producer.Dispose();
		_PublishMessageSemaphore.Dispose();
	}

	public void BeginTransaction()
	{
		_Producer.BeginTransaction();
	}

	public Task ProduceAsync(string topic,
							 Message<string, string> message,
							 CancellationToken cancellationToken)
	{
		return _Producer.ProduceAsync(topic, message, cancellationToken);
	}

	public void CommitTransaction(TimeSpan timeout)
	{
		_Producer.CommitTransaction(timeout);
	}

	public void AbortTransaction()
	{
		_Producer.AbortTransaction();
	}

	public async Task<bool> PublishMessage(string topicName, Message<string, string> message, CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topicName);
		TimeSpan waitForNewTransactionTimeout = TimeSpan.FromSeconds(30);
		bool isTransactionStarted = false;

		try
		{
			_Logger.LogDebug("Waiting for transaction to be available");
			await _PublishMessageSemaphore.WaitAsync(waitForNewTransactionTimeout, cancellationToken).ConfigureAwait(false);

			_Logger.LogInformation("Starting transaction");
			BeginTransaction();
			isTransactionStarted = true;

			_Logger.LogInformation("Producing transaction");
			await ProduceAsync(topicName, message, cancellationToken).ConfigureAwait(false);

			_Logger.LogInformation("Commiting transaction");
			CommitTransaction(_ConnectionConfig.TransactionOperationTimeout);
			_Logger.LogInformation("Commited transaction");

			return true;
		}
		catch (Exception ex)
		{
			if (ex is ProduceException<string, string>)
			{
				_Logger.LogError("Failed to produce message, {ex}", ex);
			}
			else if (ex is KafkaException)
			{
				_Logger.LogError("Failed to publish message, {ex}", ex);
			}
			else
			{
				_Logger.LogError("Unexcepted error while publishing message, {ex}", ex);
			}

			if (isTransactionStarted)
			{
				_ = await tryAbortTransactionSafely();
			}

			return false;
		}
		finally
		{
			_PublishMessageSemaphore.Release();
			_Logger.LogDebug("Released transaction lock");
		}
	}
	
	private async Task<bool> tryAbortTransactionSafely()
	{
		try
		{
			_Logger.LogInformation("Aborting transaction");
			AbortTransaction();
			_Logger.LogInformation("Transaction aborted");
			return true;
		}
		catch (KafkaException ex)
		{
			_Logger.LogError("AbortTransaction failed, {ex}", ex);
			await disposeAndRecreateProducer();
			return false;
		}
	}
	
	private async Task disposeAndRecreateProducer()
	{
		_Logger.LogError("Disposing and recreating producer");
		_Producer.Dispose();

		await Task.Delay(TimeSpan.FromSeconds(1));

		_Producer = _MessagesProducerFactory.CreateProducer(_ConnectionConfig);
	}
}