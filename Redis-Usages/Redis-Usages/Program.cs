// See https://aka.ms/new-console-template for more information

using Redis_Usages;

Console.WriteLine("Hello, World!");

// await Hashes.Run().ConfigureAwait(false);
// await SortedSets.Run().ConfigureAwait(false);
await Streams.Run().ConfigureAwait(false);