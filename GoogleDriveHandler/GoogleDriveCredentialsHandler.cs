using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using GoogleDriveHandler.Notifications;
using Newtonsoft.Json.Linq;

namespace GoogleDriveHandler
{
    internal class GoogleDriveCredentialsHandler
    {
        private readonly GoogleDriveCredentials mCredentials;

        private readonly HttpClient mHttpClient = new();

        private readonly INotifier mNotifier;

        public GoogleDriveCredentialsHandler(GoogleDriveCredentials credentials, INotifier notifier)
        {
            mCredentials = credentials;
            mNotifier = notifier;
        }

        public string? AccessToken => mCredentials.AccessToken;

        public async Task RefreshAccessToken(string uploadId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(mCredentials.RefreshToken))
            {
                string? authorizationCode = await GetAuthorizationCode(uploadId, cancellationToken).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(authorizationCode))
                {
                    await mNotifier.Notify(uploadId, "Authorization code user input is empty from", cancellationToken)
                                   .ConfigureAwait(false);
                    return;
                }
                
                await GetAccessToken(uploadId, authorizationCode, cancellationToken).ConfigureAwait(false);
            }

            if (string.IsNullOrWhiteSpace(mCredentials.RefreshToken))
            {
                await mNotifier.Notify(uploadId, "Refresh token is empty after fetching token", cancellationToken)
                        .ConfigureAwait(false);
                return;
            }

            Dictionary<string, string> formUrlMap = new()
            {
                {
                    "client_id", mCredentials.ClientId
                },
                {
                    "client_secret", mCredentials.ClientSecret
                },
                {
                    "grant_type", "refresh_token"
                },
                {
                    "refresh_token", mCredentials.RefreshToken
                }
            };

            using FormUrlEncodedContent formUrlEncodedContent = new(formUrlMap);
            HttpRequestMessage requestMessage = new(HttpMethod.Post, GoogleDriveCredentials.TokenUri);
            HttpResponseMessage responseMessage = await mHttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                string rawResponse = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                await UpdateAccessTokenFromFetchResponse(uploadId, rawResponse, cancellationToken)
                    .ConfigureAwait(false);
            }

            await mNotifier.NotifyFromHttpResponse(uploadId, responseMessage, cancellationToken).ConfigureAwait(false);
        }
        
        private async Task GetAccessToken(string uploadId, string authorizationCode, CancellationToken cancellationToken)
        {
            Dictionary<string, string> formUrlMap = new()
            {
                {
                    "code", authorizationCode
                },
                {
                    "client_id", mCredentials.ClientId
                },
                {
                    "client_secret", mCredentials.ClientSecret
                },
                {
                    "redirect_uri", GoogleDriveCredentials.RedirectUri
                },
                {
                    "grant_type", "authorization_code"
                },
            };

            using FormUrlEncodedContent formUrlEncodedContent = new(formUrlMap);

            HttpResponseMessage responseMessage = await mHttpClient.PostAsync(GoogleDriveCredentials.TokenUri,
                                                                              formUrlEncodedContent,
                                                                              cancellationToken);

            if (responseMessage.IsSuccessStatusCode)
            {
                string rawResponse = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                await UpdateAccessTokenFromFetchResponse(uploadId, rawResponse, cancellationToken)
                    .ConfigureAwait(false);
                await UpdateRefreshTokenFromFetchResponse(uploadId, rawResponse, cancellationToken)
                    .ConfigureAwait(false);
            }

            await mNotifier.NotifyFromHttpResponse(uploadId, responseMessage, cancellationToken).ConfigureAwait(false);
        }

        private async Task UpdateAccessTokenFromFetchResponse(string uploadId,
            string fetchResponseMessage,
            CancellationToken cancellationToken)
        {
            string? accessToken = JObject.Parse(fetchResponseMessage)["access_token"]?.ToString();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                await mNotifier.Notify(uploadId,
                    $"Empty access token from response '{fetchResponseMessage}'",
                    cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            await mNotifier.Notify(uploadId,
                    $"Access token updated",
                    cancellationToken)
                    .ConfigureAwait(false);
            mCredentials.AccessToken = accessToken;
        }

        private async Task UpdateRefreshTokenFromFetchResponse(string uploadId,
            string fetchResponseMessage,
            CancellationToken cancellationToken)
        {
            string? refreshToken = JObject.Parse(fetchResponseMessage)["refresh_token"]?.ToString();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await mNotifier.Notify(uploadId,
                    $"Empty refresh token from response '{fetchResponseMessage}'",
                    cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            await mNotifier.Notify(uploadId,
                    $"Refresh token updated",
                    cancellationToken)
                    .ConfigureAwait(false);
            mCredentials.RefreshToken = refreshToken;
        }
        
        private async Task<string?> GetAuthorizationCode(string uploadId, CancellationToken cancellationToken)
        {
            try
            {
                using HttpListener httpListener = new();
                httpListener.Prefixes.Add(GoogleDriveCredentials.RedirectUri.Trim('/') + '/');
                
                httpListener.Start();
                
                Console.WriteLine(mCredentials.AuthUrl);

                HttpListenerContext listenerContext = await httpListener.GetContextAsync().ConfigureAwait(false);

                await UpdateListenerResponse(listenerContext, cancellationToken);

                if (listenerContext.Request.Url is null)
                {
                    await mNotifier.Notify(uploadId, "Url of redirected page is empty", cancellationToken)
                             .ConfigureAwait(false);
                    return null;
                }
                
                string? authorizationCode = ExtractAuthorizationCodeFromUri(listenerContext.Request.Url.OriginalString);
                if (string.IsNullOrWhiteSpace(authorizationCode))
                {
                    await mNotifier.Notify(uploadId,
                                           $"Authorization code is empty. Failed to fetch it from uri {listenerContext.Request.Url.OriginalString}",
                                           cancellationToken)
                                   .ConfigureAwait(false);
                    return null;
                }

                return authorizationCode;
            }
            catch (Exception ex)
            {
                await mNotifier.Notify(uploadId,
                                       $"Failed to extract authorization code. Exception: {ex.Message}",
                                       cancellationToken)
                               .ConfigureAwait(false);
                return null;
            }
        }
        
        private static string? ExtractAuthorizationCodeFromUri(string redirectedUriWithAuthorizationCode)
        {
            const string codeQueryParameterValue = "code";

            Uri uri = new(redirectedUriWithAuthorizationCode, UriKind.Absolute);
            NameValueCollection queryParametersMap = HttpUtility.ParseQueryString(uri.Query);

            string? authorizationCode = queryParametersMap[codeQueryParameterValue];

            return string.IsNullOrWhiteSpace(authorizationCode) ? null : HttpUtility.UrlDecode(authorizationCode);
        }

        private static async Task UpdateListenerResponse(HttpListenerContext listenerContext, CancellationToken cancellationToken)
        {
            const string userMessage = "You can close the browser now";
            
            HttpListenerResponse listenerResponse = listenerContext.Response;
            listenerResponse.Headers.Set("Content-Type", "text/plain");

            byte[] userMessageBytes = Encoding.UTF8.GetBytes(userMessage);
            listenerResponse.ContentLength64 = userMessageBytes.Length;
            await using Stream outputStream = listenerResponse.OutputStream;
            await outputStream.WriteAsync(userMessageBytes, cancellationToken).ConfigureAwait(false);
        }
    }
}
