using Confluent.Kafka;

namespace Kafka_Usages;

public interface IMessagesProducer
{
	void BeginTransaction();
	
	Task ProduceAsync(string topic, Message<string, string> message, CancellationToken cancellationToken);
	
	void CommitTransaction(TimeSpan timeout);
	
	void AbortTransaction();
}