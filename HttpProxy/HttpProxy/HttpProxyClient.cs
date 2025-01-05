using System.Net;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace HttpProxy;

/// <summary>
/// The next proxy client makes sure we route only to urls that contains the word "google" in it.
/// </summary>
public class HttpProxyClient : IDisposable
{
	private readonly HttpProxyOptions mOptions;
	private readonly HttpClient mHttpClient;

	public HttpProxyClient(HttpProxyOptions options)
	{
		mOptions = options ?? throw new ArgumentNullException(nameof(options));
		mHttpClient = new HttpClient();
	}

	public void Dispose()
	{
		mHttpClient.Dispose();
	}

	public async Task HandleRunHttpProxyRequest(CancellationToken cancellationToken)
	{
		Console.WriteLine($"Going to run http proxy on {mOptions.RawIpAddress}:{mOptions.Port}. Decrypt Ssl? {mOptions.DecryptSsl}");
		ExplicitProxyEndPoint explicitEndPoint = new(IPAddress.Parse(mOptions.RawIpAddress),
													 mOptions.Port,
													 mOptions.DecryptSsl);
		ProxyServer proxyServer = CreateProxyServer(explicitEndPoint, cancellationToken);

		Console.WriteLine("Starting the proxy server");
		proxyServer.Start();
		Console.WriteLine("Proxy server started, waiting for cancellation token");

		await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
	}

	private ProxyServer CreateProxyServer(ExplicitProxyEndPoint explicitEndPoint,
										  CancellationToken cancellationToken)
	{
		ProxyServer proxyServer = new(userTrustRootCertificate: false);
		proxyServer.CheckCertificateRevocation = X509RevocationMode.NoCheck;
		proxyServer.EnableHttp2 = false;
		proxyServer.AddEndPoint(explicitEndPoint);

		Console.WriteLine($"Registering to {nameof(proxyServer.BeforeRequest)}");
		proxyServer.BeforeRequest += (_,
									  iEventArgs) => OnHttpRequestAsync(iEventArgs, cancellationToken);

		return proxyServer;
	}

	private async Task OnHttpRequestAsync(SessionEventArgs sessionEventArgs,
										  CancellationToken cancellationToken)
	{
		if (IsValidRequest(sessionEventArgs.HttpClient.Request.Url) is false)
		{
			string errorMessage = $"Invalid request destination: {sessionEventArgs.HttpClient.Request.Method} {sessionEventArgs.HttpClient.Request.Url}";
			Console.WriteLine(errorMessage);

			sessionEventArgs.GenericResponse(errorMessage,
											 HttpStatusCode.Forbidden,
											 [],
											 closeServerConnection: true);
			return;
		}

		HeaderCollection? headers = null;
		try
		{
			TimeSpan requestTimeout = TimeSpan.FromSeconds(3);
			HttpResponseMessage httpResponse = await SendHttpRequestAsync(sessionEventArgs,
																		  requestTimeout,
																		  cancellationToken)
												   .ConfigureAwait(false);

			headers = collectResponseHeaders(httpResponse);

			byte[] content = await httpResponse.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
			Console.WriteLine($"Sending response to client from: {sessionEventArgs.HttpClient.Request.RequestUri}. Status code: {httpResponse.StatusCode}");
			sessionEventArgs.GenericResponse(content,
											 httpResponse.StatusCode,
											 headers,
											 closeServerConnection: true);
		}
		catch (HttpRequestException ex)
		{
			handleHttpRequestException(sessionEventArgs, ex, headers);
		}
		catch (Exception ex)
		{
			HandleGenericException(sessionEventArgs, ex, headers);
		}
	}

	private static bool IsValidRequest(string iUrl)
	{
		return iUrl.Contains("google");
	}

	private async Task<HttpResponseMessage> SendHttpRequestAsync(SessionEventArgs sessionEventArgs,
																 TimeSpan httpRequestTimeout,
																 CancellationToken cancellationToken)
	{
		HttpMethod httpMethod = new(sessionEventArgs.HttpClient.Request.Method);
		using CancellationTokenSource cancellationTokenSource = new();
		cancellationTokenSource.CancelAfter(httpRequestTimeout);

		Console.WriteLine($"Sending {httpMethod} request to '{sessionEventArgs.HttpClient.Request.Url}'");
		using HttpRequestMessage request = new(httpMethod, sessionEventArgs.HttpClient.Request.Url);
		await UpdateRequestContent(request, sessionEventArgs, cancellationToken).ConfigureAwait(false);

		return await mHttpClient.SendAsync(request,
										   HttpCompletionOption.ResponseHeadersRead,
										   cancellationToken).ConfigureAwait(false);
	}

	private static async Task UpdateRequestContent(HttpRequestMessage request,
												   SessionEventArgs sessionEventArgs,
												   CancellationToken cancellationToken)
	{
		string requestBodyContent = await sessionEventArgs.GetRequestBodyAsString(cancellationToken).ConfigureAwait(false);
		if (string.IsNullOrWhiteSpace(requestBodyContent))
		{
			return;
		}

		using StringContent content = new(requestBodyContent, Encoding.UTF8, MediaTypeNames.Application.Json);
		request.Content = content;
	}

	private static HeaderCollection collectResponseHeaders(HttpResponseMessage iHttpResponse)
	{
		HeaderCollection headers = [];

		// Copy response headers
		foreach (KeyValuePair<string, IEnumerable<string>> header in iHttpResponse.Headers)
		{
			headers.AddHeader(header.Key, string.Join(",", header.Value));
		}

		// Add content headers (e.g., Content-Type)
		foreach (KeyValuePair<string, IEnumerable<string>> header in iHttpResponse.Content.Headers)
		{
			headers.AddHeader(header.Key, string.Join(",", header.Value));
		}

		return headers;
	}

	private void handleHttpRequestException(SessionEventArgs iSessionEventArgs,
											HttpRequestException iException,
											HeaderCollection? iHeaders)
	{
		string errorMessage = $"Error while sending request to server. Status Code: {iException.StatusCode}. Message: {iException.Message}";
		HandleGenericException(iSessionEventArgs,
							   iException,
							   iHeaders,
							   errorMessage,
							   iHttpStatusCode: iException.StatusCode ?? HttpStatusCode.InternalServerError);
	}

	private static void HandleGenericException(SessionEventArgs iSessionEventArgs,
											   Exception iException,
											   HeaderCollection? iHeaders,
											   string? iErrorMessage = null,
											   HttpStatusCode iHttpStatusCode = HttpStatusCode.InternalServerError)

	{
		string errorMessage = string.IsNullOrWhiteSpace(iErrorMessage) ? $"Error while sending request to server. Error: {iException.Message}" : iErrorMessage;
		Console.WriteLine(errorMessage, iException);
		iSessionEventArgs.GenericResponse(errorMessage,
										  iHttpStatusCode,
										  iHeaders,
										  closeServerConnection: true);
	}
}