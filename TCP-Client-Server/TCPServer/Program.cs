using Microsoft.Extensions.DependencyInjection;
using TCPServerApp.Infra.Ioc;
using TCPServerApp.Infra.Server;

namespace TCPServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using TCPServerServiceProvider serviceProvider = new TCPServerServiceProvider();

            TCPServer server = serviceProvider.GetRequiredService<TCPServer>();

            server.Start().Wait();
        }
    }
}