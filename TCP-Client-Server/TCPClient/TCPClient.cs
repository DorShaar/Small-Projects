using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.TcpCommunication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TCPClient.Settings;
using TCPClientApp.Models;

namespace TCPClientApp
{
    public class TCPClient : IDisposable
    {
        private readonly TcpClient mTcpClient;
        private readonly ILogger<TCPClient> mLogger;

        public TCPClient(IOptions<TcpClientSettings> tcpClientSettings, ILogger<TCPClient> logger)
        {
            mLogger = logger;
            
            mTcpClient = new TcpClient(tcpClientSettings.Value.ServerAddress, tcpClientSettings.Value.Port);
            mLogger.LogInformation($"Creating TCP client to communicate with {tcpClientSettings.Value.ServerAddress}:{tcpClientSettings.Value.Port}");
        }

        public async Task Run(CancellationToken iCancellationToken)
        {
            string? userInput = GetUserInput();

            while (ShouldFinishTcpClient(userInput) is false)
            {
                NetworkStream networkStream = mTcpClient.GetStream();
                
                TcpMessage tcpMessage = CreateTcpMessage(userInput);

                await TcpMessageSender.SendMessage(tcpMessage, networkStream, mLogger, iCancellationToken).ConfigureAwait(false);

                TcpMessage tcpResponseMessage = await TcpMessageReceiver.ReceiveServerResponseMessage(networkStream, mLogger).ConfigureAwait(false);

                ValidateResponseMessage(tcpResponseMessage, tcpMessage);

                PrintResponseMessage(tcpResponseMessage);

                userInput = GetUserInput();
            }
        }

        public void Dispose()
        {
            mTcpClient.Dispose();
        }

        private string? GetUserInput()
        {
            mLogger.LogInformation("Type a request");
            return Console.ReadLine();
        }

        private static bool ShouldFinishTcpClient([NotNullWhen(false)] string? userInput)
        {
            return string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(userInput);
        }

        private static TcpMessage CreateTcpMessage(string userInput)
        {
            string requestId = Guid.NewGuid().ToString();
            
            return new TcpMessage
            {
                CorrelationId = requestId, 
                RequestId = requestId,
                RawData = Encoding.UTF8.GetBytes(userInput)
            };
        }

        private static void ValidateResponseMessage(TcpMessage tcpResponseMessage, TcpMessage tcpMessage)
        {
            if (tcpResponseMessage.CorrelationId != tcpMessage.CorrelationId)
            {
                throw new Exception("Response is not valid since the correlation IDs are different");
            }
        }

        private void PrintResponseMessage(TcpMessage tcpResponseMessage)
        {
            mLogger.LogInformation($"CorrelationId: {tcpResponseMessage.CorrelationId}");
            mLogger.LogInformation($"RequestId: {tcpResponseMessage.RequestId}");
            
            string serializedTcpMessage = Encoding.UTF8.GetString(tcpResponseMessage.RawData);
            
            mLogger.LogInformation($"Returned data: {serializedTcpMessage}");
        }
    }
}