using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Kafka_Usages;

public class MessagesProducer : IMessagesProducer, IDisposable
{
	private readonly IMessagesProducerFactory _messagesProducerFactory;
	private readonly KafkaConnectionInfo _connectionConfig;
	private IProducer<string, string>? _producer;
	private readonly SemaphoreSlim _publishMessageSemaphore = new(1, 1);
	private readonly ILogger<MessagesProducer> _logger;

	public MessagesProducer(IMessagesProducerFactory messagesProducerFactory,
							KafkaConnectionInfo connectionInfo,
							ILogger<MessagesProducer> logger)
	{
		_messagesProducerFactory = messagesProducerFactory;
		_connectionConfig = connectionInfo;
		_logger = logger;
	}

	public void Dispose()
	{
		_logger.LogDebug($"Disposing {nameof(MessagesProducer)}");
		_producer?.Flush();
		_producer?.Dispose();
		_publishMessageSemaphore.Dispose();
	}

	public async Task<bool> PublishMessage(string topicName, Message<string, string> message, CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topicName);
		TimeSpan waitForNewTransactionTimeout = TimeSpan.FromSeconds(30);
		bool isTransactionStarted = false;

		_producer ??= _messagesProducerFactory.CreateProducer(_connectionConfig);
		
		try
		{
			_logger.LogDebug("Waiting for transaction to be available");
			await _publishMessageSemaphore.WaitAsync(waitForNewTransactionTimeout, cancellationToken).ConfigureAwait(false);

			_logger.LogInformation("Starting transaction");
			_producer.BeginTransaction();
			isTransactionStarted = true;

			_logger.LogInformation("Producing transaction");
			await _producer.ProduceAsync(topicName, message, cancellationToken).ConfigureAwait(false);

			_logger.LogInformation("Commiting transaction");
			_producer.CommitTransaction(_connectionConfig.TransactionOperationTimeout);
			_logger.LogInformation("Commited transaction");

			return true;
		}
		catch (Exception ex)
		{
			if (ex is ProduceException<string, string>)
			{
				_logger.LogError("Failed to produce message, {ex}", ex);
			}
			else if (ex is KafkaException)
			{
				_logger.LogError("Failed to publish message, {ex}", ex);
			}
			else
			{
				_logger.LogError("Unexcepted error while publishing message, {ex}", ex);
			}

			if (isTransactionStarted)
			{
				_ = await tryAbortTransactionSafely();
			}

			return false;
		}
		finally
		{
			_publishMessageSemaphore.Release();
			_logger.LogDebug("Released transaction lock");
		}
	}
	
	private async Task<bool> tryAbortTransactionSafely()
	{
		try
		{
			ArgumentNullException.ThrowIfNull(_producer);
			
			_logger.LogInformation("Aborting transaction");
			_producer.AbortTransaction();
			_logger.LogInformation("Transaction aborted");
			return true;
		}
		catch (KafkaException ex)
		{
			_logger.LogError("AbortTransaction failed, {ex}", ex);
			await disposeAndRecreateProducer();
			return false;
		}
	}
	
	private async Task disposeAndRecreateProducer()
	{
		ArgumentNullException.ThrowIfNull(_producer);
		
		_logger.LogError("Disposing and recreating producer");
		_producer.Dispose();

		await Task.Delay(TimeSpan.FromSeconds(1));

		_producer = _messagesProducerFactory.CreateProducer(_connectionConfig);
	}
}