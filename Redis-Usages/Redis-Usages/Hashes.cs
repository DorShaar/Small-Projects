using StackExchange.Redis;

namespace Redis_Usages;

public static class Hashes
{
	private static readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("localhost");
	private static readonly IDatabase _db = _redis.GetDatabase();

	public static async Task Run()
	{
		const string userKey = "user:1001";  // Unique Redis key for the user

		// Add user details using a Redis Hash
		await _db.HashSetAsync(userKey,
		[
			new HashEntry("name", "Alice"),
			new HashEntry("age", "30"),
			new HashEntry("city", "New York")
		]).ConfigureAwait(false);

		Console.WriteLine("User added to Redis!");

		// Retrieve user details
		string? name = await _db.HashGetAsync(userKey, "name").ConfigureAwait(false);
		string? age = await _db.HashGetAsync(userKey, "age").ConfigureAwait(false);
		string? city = await _db.HashGetAsync(userKey, "city").ConfigureAwait(false);

		Console.WriteLine($"User Details: Name={name}, Age={age}, City={city}");

		// Update user's age
		await _db.HashIncrementAsync(userKey, "age", value: 1).ConfigureAwait(false);
		Console.WriteLine($"Updated Age: {await _db.HashGetAsync(userKey, "age").ConfigureAwait(false)}");

		// Get all fields in the hash
		HashEntry[] userEntries = await _db.HashGetAllAsync(userKey).ConfigureAwait(false);
		Console.WriteLine("User Info from Redis:");
		foreach (HashEntry entry in userEntries)
		{
			Console.WriteLine($"{entry.Name}: {entry.Value}");
		}
	}
}