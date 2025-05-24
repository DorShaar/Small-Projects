using Confluent.Kafka;

namespace Kafka_Usages;

public interface IMessagesProducerFactory
{
	IProducer<string, string> CreateProducer(KafkaConnectionInfo connectionConfig);
}