namespace TCPClient.Settings;

public class TcpClientSettings
{
	public required string ServerAddress { get; init; }

	public required int Port { get; init; }
}