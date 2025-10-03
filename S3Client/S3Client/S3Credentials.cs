using Amazon;

namespace S3Client;

internal class S3Credentials
{
    public required string BuckeyName { get; init; }
    public required string AccessKey { get; init; }
    public required string Secret { get; init; }
    public required RegionEndpoint BucketRegion { get; init; }
}
