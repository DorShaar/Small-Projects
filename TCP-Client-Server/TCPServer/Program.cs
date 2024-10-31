using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TCPServerApp.Infra;
using TCPServerApp.Infra.Server;

namespace TCPServerApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using TCPServerServiceProvider serviceProvider = new();

            TCPServer server = serviceProvider.GetRequiredService<TCPServer>();

            await server.Run(CancellationToken.None).ConfigureAwait(false);
        }
    }
}