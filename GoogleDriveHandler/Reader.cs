using GoogleDriveHandler.Models;
using GoogleDriveHandler.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoogleDriveHandler;

internal class Reader : IDisposable
{
	private const string FolderMimeType = "application/vnd.google-apps.folder";
	
	private readonly HttpClient mHttpClient = new();
	private readonly GoogleDriveCredentialsHandler mCredentialsHandler;
	private readonly INotifier mNotifier;

	public Reader(GoogleDriveCredentialsHandler credentialsHandler, INotifier notifier)
	{
		mCredentialsHandler = credentialsHandler;
		mNotifier = notifier;
	}
	
	/// <summary>
	/// Assumption: The directory name is unique in the root directory.
	/// </summary>
	public async Task<string?> GetDirectoryId(string directoryName, CancellationToken cancellationToken)
	{
		string query = $"name = '{directoryName}' and mimeType = '{FolderMimeType}' and 'root' in parents and trashed = false";
		HttpRequestMessage request = new(HttpMethod.Get,
										 $"https://www.googleapis.com/drive/v3/files?q={Uri.EscapeDataString(query)}&fields=files(id,name,mimeType)");

		request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", mCredentialsHandler.AccessToken);
		
		HttpResponseMessage response = await mHttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
		string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		if (JObject.Parse(content)["files"] is not JArray files)
		{
			return null;
		}
            
		FileInfoModel? destinationDirectoryInfo = JsonConvert.DeserializeObject<FileInfoModel>(files[0].ToString());
		return destinationDirectoryInfo?.Id;
	}

	public void Dispose()
	{
		mHttpClient.Dispose();
	}
}