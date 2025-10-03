using Amazon.S3.Model;
using Amazon.S3;
using Amazon;

namespace S3Client;

internal class S3Handler
{
    private readonly S3Credentials _credentials;
    private readonly AmazonS3Client _s3Client;
    private readonly S3Reader _reader;
    private readonly S3Uploader _uploader;

    public S3Handler()
    {
        _credentials = new S3Credentials()
        {
            AccessKey = "",
            Secret = "",
            BuckeyName = "",
            BucketRegion = RegionEndpoint.EUWest1
        };

        _s3Client = new AmazonS3Client(_credentials.AccessKey,
            _credentials.Secret,
            _credentials.BucketRegion);

        _reader = new S3Reader(_s3Client, _credentials.BuckeyName);
        _uploader = new S3Uploader(_s3Client, _credentials.BuckeyName);
    }

    public async Task Handle()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            const string readKey = "c/file";

            byte[] file = await _reader.ReadFileAsync(readKey);
            if (file is null || file.Length == 0)
            {
                continue;
            }

            await _uploader.Upload(@"C:/some/path/to/file");
            await DeleteFileAsync(readKey);
        }
    }

    private async Task DeleteFileAsync(string key)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _credentials.BuckeyName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
            Console.WriteLine($"File '{key}' deleted from bucket '{_credentials.BuckeyName}'");
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"S3 error: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unknown error: {e.Message}");
            throw;
        }
    }
}