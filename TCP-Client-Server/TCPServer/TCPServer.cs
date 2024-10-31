using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.TcpCommunication;
using TCPClientApp.Models;
using TCPServerApp.Settings;

namespace TCPServerApp.Infra.Server
{
    public class TCPServer
    {
        private readonly IOptionsMonitor<TcpServerSettings> mServerOptions;
        private readonly TcpListener mTCPListener;
        private readonly ILogger<TCPServer> mLogger;

        public TCPServer(IOptionsMonitor<TcpServerSettings> optionsMonitor, ILogger<TCPServer> logger)
        {
            mServerOptions = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            mTCPListener = new TcpListener(IPAddress.Parse(optionsMonitor.CurrentValue.ServerAddress), optionsMonitor.CurrentValue.Port);
            mLogger.LogInformation($"Creating TCP server which listens to message on {optionsMonitor.CurrentValue.ServerAddress}:{optionsMonitor.CurrentValue.Port}");
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            mLogger.LogInformation($"Starting tcp server, listening on port {mServerOptions.CurrentValue.Port}");
            mTCPListener.Start();
            TcpClient? client = null;

            try
            {
                while(true)
                {
                    try
                    {
                        if (client is null || client.Connected is false)
                        {
                            client = await mTCPListener.AcceptTcpClientAsync(cancellationToken);
                            mLogger.LogInformation("Connected with a client");
                        }
                        else
                        {
                            mLogger.LogInformation("Waiting for messages from the same client");
                        }

                        NetworkStream networkStream = client.GetStream();
                
                        TcpMessage tcpMessage = await TcpMessageReceiver.ReceiveServerResponseMessage(networkStream, mLogger).ConfigureAwait(false);

                        TcpMessage tcpResponseMessage = createResponseMessage(tcpMessage);
                    
                        await TcpMessageSender.SendMessage(tcpResponseMessage, networkStream, mLogger, cancellationToken).ConfigureAwait(false);
                    }
                    catch (SocketException e)
                    {
                        mLogger.LogError(e, "Error occurred");
                    }
                }
            }
            finally
            {
                client?.Dispose();
                mTCPListener.Stop();
            }
        }

        private static TcpMessage createResponseMessage(TcpMessage tcpMessage)
        {
            string receivedData = Encoding.UTF8.GetString(tcpMessage.RawData);
            string resultData = $"Hi {receivedData}";
            
            return new TcpMessage
            {
                CorrelationId = tcpMessage.CorrelationId,
                RequestId = Guid.NewGuid().ToString(),
                RawData = Encoding.UTF8.GetBytes(resultData)
            };
        }
    }
}