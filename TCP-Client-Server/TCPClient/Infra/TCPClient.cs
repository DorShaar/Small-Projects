using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClientApp.Infra
{
    public class TCPClient : IDisposable
    {
        private const int BufferSize = 4096;
        private readonly TcpClient mTCPClient;

        public TCPClient()
        {
            mTCPClient = new TcpClient("127.0.0.1", 13000);
        }

        public async Task Run()
        {
            Console.WriteLine("Type a request");
            string userInput = Console.ReadLine();

            while (!string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
            {
                byte[] requestData = Encoding.ASCII.GetBytes(userInput);

                NetworkStream networkStream = mTCPClient.GetStream();

                networkStream.Write(requestData, 0, requestData.Length);

                byte[] responseData = new byte[BufferSize];

                int bytesRead = networkStream.Read(responseData, 0, responseData.Length);
                string responseString = Encoding.ASCII.GetString(responseData, 0, bytesRead);

                byte[] decodedResponseBytes = Convert.FromBase64String(responseString);
                string decodedResponseMessage = Encoding.ASCII.GetString(decodedResponseBytes);
                Console.WriteLine($"Received: {decodedResponseMessage}");

                Console.WriteLine("Type a request");
                userInput = Console.ReadLine();
            }
        }

        public void Dispose()
        {
            mTCPClient.Dispose();
        }
    }
}