using Newtonsoft.Json;

namespace GoogleDriveHandler.Models
{
    internal class UploadFileModel
    {
        [JsonProperty("name")]
        public required string FileName { get; init; }

        [JsonProperty("parents")]
        public required string[] Parents { get; init; }
    }
}
