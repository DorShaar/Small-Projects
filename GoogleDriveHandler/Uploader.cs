using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;
using GoogleDriveHandler.Models;
using GoogleDriveHandler.Notifications;

namespace GoogleDriveHandler
{
    internal class Uploader : IDisposable
    {
        private readonly HttpClient mHttpClient = new();
        private readonly GoogleDriveCredentialsHandler mCredentialsHandler;
        private readonly Reader mGoogleDriveReader;
        private readonly INotifier mNotifier;

        public Uploader(GoogleDriveCredentialsHandler credentialsHandler,
                        Reader googleDriveReader,
                        INotifier notifier)
        {
            mCredentialsHandler = credentialsHandler;
            mGoogleDriveReader = googleDriveReader;
            mNotifier = notifier;
        }

        public async Task Upload(string filePath, string destinationDirectory, CancellationToken cancellationToken)
        {
            string uploadId = Guid.NewGuid().ToString();
            bool hasAccessToken = await ValidateAccessToken(uploadId, cancellationToken).ConfigureAwait(false);
            if (!hasAccessToken)
            {
                return;
            }
            
            string? destinationDirectoryId = await mGoogleDriveReader.GetDirectoryId(destinationDirectory, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(destinationDirectoryId))
            {
                await mNotifier.Notify(uploadId, $"Could not find destination directory {destinationDirectory}", cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            using MultipartContent multipartContent = await PrepareMultipartContent(filePath, destinationDirectoryId).ConfigureAwait(false);
            using HttpResponseMessage httpResponse = await PerformUpload(multipartContent, cancellationToken).ConfigureAwait(false);
            await mNotifier.NotifyFromHttpResponse(uploadId, httpResponse, cancellationToken)
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            mHttpClient.Dispose();
        }

        private async Task<bool> ValidateAccessToken(string uploadId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(mCredentialsHandler.AccessToken))
            {
                await mCredentialsHandler.RefreshAccessToken(uploadId, cancellationToken).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(mCredentialsHandler.AccessToken))
                {
                    await mNotifier.Notify(uploadId, "Access token is empty after fetching token", cancellationToken)
                        .ConfigureAwait(false);
                    return false;
                }
            }

            return true;
        }
        
        private static async Task<MultipartContent> PrepareMultipartContent(string filePath, string parentDirectoryId)
        {
            StringContent metadataContent = createMetadataStringContent(filePath, parentDirectoryId);
            ByteArrayContent mediaContent = await createMediaContent(filePath).ConfigureAwait(false);

            return new MultipartContent("related", Guid.NewGuid().ToString())
            {
                metadataContent,
                mediaContent
            };
        }

        private static string getFileName(string filePath)
        {
            const string dateTimeFormat = "yyyy-MM-dd_HH-mm-ss";
            string fileName = Path.GetFileName(filePath);
            return $"{fileName}_{DateTime.Now.ToString(dateTimeFormat)}";
        }

        private static StringContent createMetadataStringContent(string filePath, string parentDirectoryId)
        {
            string uploadFileName = getFileName(filePath);

            UploadFileModel uploadFileModel = new()
            {
                FileName = uploadFileName,
                Parents = [parentDirectoryId]
            };

            string metadata = JsonConvert.SerializeObject(uploadFileModel);

            StringContent metadataStringContent = new(metadata,
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            return metadataStringContent;
        }

        private static async Task<ByteArrayContent> createMediaContent(string filePath)
        {
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
            return new ByteArrayContent(fileBytes);
        }

        private async Task<HttpResponseMessage> PerformUpload(MultipartContent multipartContent, CancellationToken cancellationToken)
        {
            const string uploadUrl = "https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart";
            using HttpRequestMessage requestMessage = new(HttpMethod.Post, uploadUrl);
            requestMessage.Content = multipartContent;

            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", mCredentialsHandler.AccessToken);

            return await mHttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        }
    }
}
