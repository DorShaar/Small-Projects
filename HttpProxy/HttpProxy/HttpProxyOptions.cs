namespace HttpProxy;

public class HttpProxyOptions
{
	public required ushort Port { get; init; }
    
	public required string RawIpAddress { get; init; }

	public required bool DecryptSsl { get; init; }
}