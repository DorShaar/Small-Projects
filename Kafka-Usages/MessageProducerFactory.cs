using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Kafka_Usages;

public class MessagesProducerFactory(ILogger<MessagesProducerFactory> logger) : IMessagesProducerFactory
{
	/// <summary>
	/// Created the producer and initialize it for using transactions.
	/// </summary>
	/// <param name="connectionConfig"></param>
	/// <returns></returns>
	public IProducer<string, string> CreateProducer(KafkaConnectionInfo connectionConfig)
	{
		ProducerConfig producerConfig = createProducerConfig(connectionConfig);
		IProducer<string, string> producer = new ProducerBuilder<string, string>(producerConfig).Build();
		producer.InitTransactions(TimeSpan.FromSeconds(15));

		return producer;
	}
	
	private ProducerConfig createProducerConfig(KafkaConnectionInfo connectionConfig)
	{
		ClientConfig clientConfig = createClientConfig(connectionConfig);
		ProducerConfig producerConfig = new(clientConfig)
		{
			EnableIdempotence = true,
			TransactionalId = connectionConfig.TransactionalId ?? string.Empty,
			MessageTimeoutMs = 10000, // 10 seconds. Applies to produce requests only - it configures how long to wait for the broker to acknowledge the message.
			RequestTimeoutMs = 8000,  // 8 seconds. Applies to all kafka requests - it configures how long to wait for any broker response.
		};

		logger.LogInformation($"Created producer config with transactional id {producerConfig.TransactionalId}");

		return producerConfig;
	}

	private static ClientConfig createClientConfig(KafkaConnectionInfo connectionConfig)
	{
		return new ClientConfig
		{
			BootstrapServers = string.Join(",", connectionConfig.Servers),
			SecurityProtocol = connectionConfig.SecurityProtocol,
			SaslMechanism = connectionConfig.SaslMechanism,
			SaslUsername = connectionConfig.ApiKey,
			SaslPassword = connectionConfig.ApiSecret,
			EnableSslCertificateVerification = connectionConfig.EnableSslCertificateVerification
		};
	}
}