using Confluent.Kafka;

namespace Kafka_Usages;

public class KafkaConnectionInfo
{
	public required IEnumerable<string> Servers { get; init; }

	public string ApiKey { get; init; } = "";

	public string ApiSecret { get; init; } = "";

	public SecurityProtocol SecurityProtocol { get; init; }

	public SaslMechanism? SaslMechanism { get; init; }

	public bool EnableSslCertificateVerification { get; init; } = true;
        
	public string? ConsumerGroupId { get; init; }
	
	/// <summary>
	/// Used in case kafka producer is used in transactions.
	/// </summary>
	public string? TransactionalId { get; init; } // Example: $"{producerName}-{Environment.MachineName}"

	public TimeSpan TransactionOperationTimeout { get; init; } = TimeSpan.FromSeconds(30);
}