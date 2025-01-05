using System.Net;
using System.Security.Authentication;

namespace HttpClientWithProxy;

public class HttpClientWithWebProxy : IDisposable
{
	private readonly HttpClient mHttpClient;

	public HttpClientWithWebProxy()
	{
		mHttpClient = new HttpClient(CrateHttpClientHandler());
	}
	
	public async Task Run(string iUri, CancellationToken cancellationToken)
	{
		HttpRequestMessage request = new()
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri(iUri, UriKind.Absolute),
		};
		
		HttpResponseMessage response = await mHttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
		string errorMessage = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
		Console.WriteLine($"Response for request to '{iUri}' Status: {response.StatusCode}");

		if (string.IsNullOrWhiteSpace(errorMessage) is false)
		{
			Console.WriteLine($"Error Message: {errorMessage}");
		}
	}

	public void Dispose()
	{
		mHttpClient.Dispose();
	}

	private static HttpClientHandler CrateHttpClientHandler()
	{
		IWebProxy proxy = new WebProxy("127.0.0.1", 55555);
		
		HttpClientHandler clientHandler = new() {
            AllowAutoRedirect = false, 
            ServerCertificateCustomValidationCallback = (iRequest, _1, _2, iErrors) => true,
            Proxy = proxy,
            CheckCertificateRevocationList = false,
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
            DefaultProxyCredentials = proxy.Credentials,
            UseCookies = true,
            PreAuthenticate = false,
            // Credentials = ...
            UseDefaultCredentials = false,
        };

        return clientHandler;
	}
}