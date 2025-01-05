// See https://aka.ms/new-console-template for more information

using HttpProxy;

Console.WriteLine("Hello, World!");

HttpProxyOptions options = new()
{
	Port = 55555,
	RawIpAddress = "127.0.0.1",
	DecryptSsl = true
};

HttpProxyClient client = new(options);
await client.HandleRunHttpProxyRequest(CancellationToken.None).ConfigureAwait(false);