using StackExchange.Redis;

namespace Redis_Usages;

public class Streams
{
    private static readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("localhost");
    private static readonly IDatabase _db = _redis.GetDatabase();
    
	public static async Task Run()
	{
        const string streamKey = "serviceB:requests";
        const string consumerGroup = "groupB";
        const string consumerName = "workerB";
        const int retryThresholdMs = 3_000;           // Time after which to retry failed requests

        await EnsureConsumerGroupExists(streamKey, consumerGroup);

        // Simulating adding requests
        await AddRequestAsync(streamKey, "request_1");
        await AddRequestAsync(streamKey, "request_2");

        while (true)
        {
            // Read new messages from the stream
            // '>' reads only new messages.
            StreamEntry[] entries = await _db.StreamReadGroupAsync(streamKey, consumerGroup, consumerName, ">", count: 10);
            if (entries.Length > 0)
            {
                await ProcessEntriesAsync(streamKey, consumerGroup, entries);
            }
            
            // Try to claim old pending messages before reading new ones
            StreamEntry[] claimedEntries = await ClaimPendingMessagesAsync(streamKey, consumerGroup, consumerName, retryThresholdMs);
            if (claimedEntries.Length <= 0)
            {
                Console.WriteLine("No requests to process.");
                break;
            }
            
            await ProcessEntriesAsync(streamKey, consumerGroup, claimedEntries);
        }
    }
    
    private static async Task ProcessEntriesAsync(string streamKey, string consumerGroup, StreamEntry[] entries)
    {
        foreach (StreamEntry entry in entries)
        {
            string? requestId = entry.Values[0].Value;
            
            ArgumentException.ThrowIfNullOrWhiteSpace(requestId);
            
            bool success = await MakeApiCallAsync(requestId);

            if (success)
            {
                await _db.StreamAcknowledgeAsync(streamKey, consumerGroup, entry.Id);
                Console.WriteLine($"✅ Processed {requestId} successfully.");
            }
            else
            {
                Console.WriteLine($"❌ {requestId} failed. Will retry later.");
                // Do NOT acknowledge failed messages so they stay pending.
            }
        }
    }

    private static async Task EnsureConsumerGroupExists(string streamKey, string consumerGroup)
    {
        try
        {
            await _db.StreamCreateConsumerGroupAsync(streamKey, consumerGroup, "0", createStream: true);
        }
        catch (RedisServerException e) when (e.Message.Contains("BUSYGROUP"))
        {
            Console.WriteLine("Consumer group already exists.");
        }
    }
    
    // Claims old pending messages that haven't been acknowledged
    private static async Task<StreamEntry[]> ClaimPendingMessagesAsync(string streamKey, string consumerGroup, string consumerName, int minIdleTimeMs)
    {
        // '-' - represents the smallest possible ID
        // '+' - represents the largest possible ID
        StreamPendingMessageInfo[] pending = await _db.StreamPendingMessagesAsync(streamKey, consumerGroup, count: 1, consumerName, "-", "+");
        RedisValue[] pendingMessageIds = pending.Select(p => p.MessageId).ToArray();

        if (pendingMessageIds.Length == 0)
        {
            return [];
        }
        
        StreamEntry[] claimedEntries = await _db.StreamClaimAsync(streamKey, consumerGroup, consumerName, minIdleTimeMs, pendingMessageIds);
        if (claimedEntries.Length > 0)
        {
            Console.WriteLine("🔄 Retrying old pending messages...");
        }
        
        return claimedEntries;
    }

    private static async Task AddRequestAsync(string streamKey, string requestId)
    {
        await _db.StreamAddAsync(streamKey, [new NameValueEntry("request", requestId)]);
        Console.WriteLine($"📌 Added {requestId} to stream.");
    }

    static async Task<bool> MakeApiCallAsync(string requestId)
    {
        await Task.Delay(500); // Simulate API call delay
        return new Random().Next(0, 2) == 0; // 50% success rate
    }
}