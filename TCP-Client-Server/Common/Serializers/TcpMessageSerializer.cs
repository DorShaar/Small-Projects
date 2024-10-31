using System.Text;
using Newtonsoft.Json;
using TCPClientApp.Models;

namespace Common.Serializers;

public static class TcpMessageSerializer
{
	public static byte[] Serialize(TcpMessage tcpMessage)
	{
		string serializedTcpMessage = JsonConvert.SerializeObject(tcpMessage, Formatting.None);
		return Encoding.UTF8.GetBytes(serializedTcpMessage);
	}
	
	public static TcpMessage Deserialize(byte[] rawTcpMessage)
	{
		string serializedTcpMessage = Encoding.UTF8.GetString(rawTcpMessage);
		TcpMessage? tcpMessage = JsonConvert.DeserializeObject<TcpMessage>(serializedTcpMessage);

		if (tcpMessage is null)
		{
			throw new NullReferenceException("Failed to deserialize tcp message");
		}

		return tcpMessage;
	}
}