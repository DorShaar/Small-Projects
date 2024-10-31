using System.Net;
using System.Net.Sockets;
using Common.Serializers;
using Microsoft.Extensions.Logging;
using TCPClientApp.Models;

namespace Common.TcpCommunication;

public static class TcpMessageReceiver
{
	public static async Task<TcpMessage> ReceiveServerResponseMessage(NetworkStream networkStream, ILogger logger)
	{
		byte[] sizeBuffer = new byte[Consts.HeaderSizeBytes];
		_ = await networkStream.ReadAsync(sizeBuffer.AsMemory(0, Consts.HeaderSizeBytes)).ConfigureAwait(false);
		long bodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(sizeBuffer));

		logger.LogInformation($"Received tcp message size {bodyLength}");
            
		byte[] buffer = new byte[bodyLength];
		int readBytes = 0;
		while (readBytes < bodyLength)
		{
			long leftBytesToRead = bodyLength - readBytes;
			readBytes += await networkStream.ReadAsync(buffer.AsMemory(readBytes, (int)leftBytesToRead)).ConfigureAwait(false);
		}
		
		return TcpMessageSerializer.Deserialize(buffer);
	}
}