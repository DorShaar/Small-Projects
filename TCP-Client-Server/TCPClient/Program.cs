using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TCPClient.IOC;

namespace TCPClientApp
{
    public static class Program
    {
        public static async Task Main()
        {
            TcpClientServiceProvider clientServiceProvider = new();

            TCPClient tcpClient = clientServiceProvider.GetRequiredService<TCPClient>();
            await tcpClient.Run(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
