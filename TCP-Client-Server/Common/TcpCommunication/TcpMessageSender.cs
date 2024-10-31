using System.Net;
using System.Net.Sockets;
using Common.Serializers;
using Microsoft.Extensions.Logging;
using TCPClientApp.Models;

namespace Common.TcpCommunication;

public static class TcpMessageSender
{
	public static async Task SendMessage(TcpMessage tcpMessage,
										 NetworkStream networkStream,
										 ILogger logger,
										 CancellationToken iCancellationToken)

	{
		byte[] rawTcpMessage = TcpMessageSerializer.Serialize(tcpMessage);

		await using MemoryStream memoryStream = new(rawTcpMessage);

		// Write header.
		logger.LogInformation($"Sending header size: {memoryStream.Length}");
		byte[] sizeBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(memoryStream.Length));
		await networkStream.WriteAsync(sizeBuffer.AsMemory(0, Consts.HeaderSizeBytes), iCancellationToken).ConfigureAwait(false);

		// Write body. 
		logger.LogInformation("Sending body content");
		await memoryStream.CopyToAsync(networkStream, iCancellationToken).ConfigureAwait(false);

		await networkStream.FlushAsync(iCancellationToken).ConfigureAwait(false);
	}
}