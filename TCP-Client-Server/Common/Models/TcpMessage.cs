namespace TCPClientApp.Models;

public class TcpMessage
{
	public required string CorrelationId { get; init; }
	
	public required string RequestId { get; init; }
	
	public required byte[] RawData { get; init; }
}