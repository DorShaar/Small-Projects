using TCPClientApp.Infra;

namespace TCPClientApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using TCPClient tcpClient = new TCPClient();

            tcpClient.Run().Wait();
        }
    }
}
