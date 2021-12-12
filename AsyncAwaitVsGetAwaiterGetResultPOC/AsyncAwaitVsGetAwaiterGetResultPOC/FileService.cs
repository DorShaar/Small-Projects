using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Threads
{
    internal class FileService
    {
        private const string cContent = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        private const int cBufferLength = 4096;

        private static readonly byte[] srBytes = Encoding.UTF8.GetBytes(cContent);

        public void WriteFile(string filePath, int fileSizeToReach)
        {
            System.Console.WriteLine($"Start writing to file {filePath}");

            using Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            while (stream.Length < fileSizeToReach)
            {
                stream.WriteAsync(srBytes, 0, srBytes.Length).GetAwaiter().GetResult();
            }
        }

        public async Task WriteFileAsync(string filePath, int fileSizeToReach)
        {
            System.Console.WriteLine($"Start writing async to file {filePath}");

            using Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            while (stream.Length < fileSizeToReach)
            {
                await stream.WriteAsync(srBytes, 0, srBytes.Length).ConfigureAwait(false);
            }
        }

        public void ReadFile(string filePath)
        {
            System.Console.WriteLine($"Start reading from file {filePath}");

            using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            byte[] readBytes = new byte[cBufferLength];
            int readBytesCount = 1;

            while (readBytesCount > 0)
            {
                readBytesCount = stream.ReadAsync(readBytes, 0, cBufferLength).GetAwaiter().GetResult();
                string convertedString = Encoding.UTF8.GetString(readBytes);
            }
        }

        public async Task ReadFileAsync(string filePath)
        {
            System.Console.WriteLine($"Start reading async from file {filePath}");

            using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            byte[] readBytes = new byte[cBufferLength];
            int readBytesCount = 1;

            while (readBytesCount > 0)
            {
                readBytesCount = await stream.ReadAsync(readBytes, 0, cBufferLength).ConfigureAwait(false);
                string convertedString = Encoding.UTF8.GetString(readBytes);
            }
        }
    }
}