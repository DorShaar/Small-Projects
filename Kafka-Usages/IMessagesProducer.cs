using Confluent.Kafka;

namespace Kafka_Usages;

public interface IMessagesProducer
{
	Task<bool> PublishMessage(string topicName, Message<string, string> message, CancellationToken cancellationToken);
}