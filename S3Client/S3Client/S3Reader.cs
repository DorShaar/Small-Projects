using Amazon.S3;
using Amazon.S3.Model;

namespace S3Client;

public class S3Reader
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Reader(IAmazonS3 s3Client, string bucketName)
    {
        _s3Client = s3Client;
        _bucketName = bucketName;
    }

    public async Task<byte[]> ReadFileAsync(string key)
    {
        try
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            using var response = await _s3Client.GetObjectAsync(getRequest);
            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            Console.WriteLine($"File '{key}' read from bucket '{_bucketName}'");
            return memoryStream.ToArray();
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"S3 error: {e.Message}");
            return [];
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unknown error: {e.Message}");
            return [];
        }
    }
}

