using StackExchange.Redis;

namespace Redis_Usages;

public static class SortedSets
{
	private static readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("localhost");
	private static readonly IDatabase _db = _redis.GetDatabase();
	private static readonly string _leaderboardKey = "game:leaderboard";

	public static async Task Run()
	{
		await _db.SortedSetAddAsync(_leaderboardKey, "Alice", 1500);
		await _db.SortedSetAddAsync(_leaderboardKey, "Bob", 1800);
		await _db.SortedSetAddAsync(_leaderboardKey, "Charlie", 1200);
		await _db.SortedSetAddAsync(_leaderboardKey, "Dave", 2000);

		Console.WriteLine("Leaderboard initialized!");

		// Get Alice's current score
		double? aliceScore = await _db.SortedSetScoreAsync(_leaderboardKey, "Alice");
		Console.WriteLine($"Alice's Score: {aliceScore}");

		// Increase Alice's score by 200
		await _db.SortedSetIncrementAsync(_leaderboardKey, "Alice", 200);
		Console.WriteLine($"Alice's New Score: {await _db.SortedSetScoreAsync(_leaderboardKey, "Alice")}");

		// Get Top 3 players
		Console.WriteLine("\n🏆 Top 3 Players:");
		SortedSetEntry[] topPlayers = await _db.SortedSetRangeByRankWithScoresAsync(_leaderboardKey, 0, 2, Order.Descending);
		foreach (SortedSetEntry player in topPlayers)
		{
			Console.WriteLine($"{player.Element}: {player.Score}");
		}

		// Get Alice's rank
		long? aliceRank = await _db.SortedSetRankAsync(_leaderboardKey, "Alice", Order.Descending);
		Console.WriteLine($"\nAlice's Rank: {aliceRank + 1}"); // Rank is 0-based
	}
}