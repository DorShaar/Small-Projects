using Newtonsoft.Json;

namespace GoogleDriveHandler.Models;

internal class FileInfoModel
{
	[JsonProperty("id")]
	public required string Id { get; init; }
	
	[JsonProperty("name")]
	public required string Name { get; init; }
	
	[JsonProperty("mimeType")]
	public required string MimeType { get; init; }
}