using Amazon.S3;
using Amazon.S3.Transfer;

namespace S3Client;

internal class S3Uploader
{
    private readonly IAmazonS3? _s3Client;
    private readonly string _bucketName;

    public S3Uploader(IAmazonS3 s3Client, string bucketName)
    {
        _s3Client = s3Client;
        _bucketName = bucketName;
    }

    public async Task Upload(string filePath)
    {
        try
        {
            await UploadFileAsync(filePath);
            Console.WriteLine($"File '{filePath}' uploaded successfully!");
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown encountered on server. Message:'{0}'", e.Message);
        }
    }

    private async Task UploadFileAsync(string filePath)
    {
        var fileTransferUtility = new TransferUtility(_s3Client);

        // Create the S3 key with "backups/" prefix
        string key = "backups/" + Path.GetFileName(filePath);

        await fileTransferUtility.UploadAsync(filePath, _bucketName, key);
    }
}
