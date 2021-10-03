using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TCPServerApp.App.Requests;
using TCPServerApp.Config;

namespace TCPServerApp.Infra.Server
{
    public class TCPServer
    {
        private const int BufferSize = 4096;

        private readonly IRequestHandler mServerRequestHandler;
        private readonly IOptionsMonitor<ServerOptions> mServerOptions;
        private readonly TcpListener mTCPListener;
        private readonly ILogger<TCPServer> mLogger;

        public TCPServer(IRequestHandler serverRequestHandler,
            IOptionsMonitor<ServerOptions> optionsMonitor,
            ILogger<TCPServer> logger)
        {
            mServerRequestHandler = serverRequestHandler ?? throw new ArgumentNullException(nameof(serverRequestHandler));
            mServerOptions = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));

            mTCPListener = new TcpListener(
                IPAddress.Parse(optionsMonitor.CurrentValue.Address),
                optionsMonitor.CurrentValue.Port);
        }

        public async Task Start()
        {
            try
            {
                mLogger.LogInformation($"Starting server, listening on port {mServerOptions.CurrentValue.Port}");
                mTCPListener.Start();

                using TcpClient client = mTCPListener.AcceptTcpClient();
                mLogger.LogInformation("Connected with a client");

                NetworkStream networkStream = client.GetStream();

                byte[] bytesBuffer = new byte[BufferSize];

                int bytesRead = 0;

                while ((bytesRead = await networkStream.ReadAsync(bytesBuffer, 0, bytesBuffer.Length).ConfigureAwait(false)) != 0)
                {
                    mLogger.LogDebug($"Recieved {bytesRead} bytes");

                    byte[] requestResult = mServerRequestHandler.ProcessRequest(bytesBuffer, bytesRead);

                    await networkStream.WriteAsync(requestResult, 0, requestResult.Length).ConfigureAwait(false);

                    mLogger.LogDebug($"Sent {requestResult.Length} bytes");
                }
            }
            catch (SocketException e)
            {
                mLogger.LogError(e, "Error occurred");
            }
            finally
            {
                mTCPListener.Stop();
            }
        }
    }
}