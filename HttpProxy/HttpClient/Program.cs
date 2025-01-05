// See https://aka.ms/new-console-template for more information

using HttpClientWithProxy;

Console.WriteLine("Hello, World!");

HttpClientWithWebProxy httpClient = new();
await httpClient.Run("https://www.google.com", CancellationToken.None).ConfigureAwait(false);
await httpClient.Run("https://www.ynet.co.il", CancellationToken.None).ConfigureAwait(false);