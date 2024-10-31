namespace TCPServerApp.Settings
{
    public class TcpServerSettings
    {
        public required string ServerAddress { get; init; }
        public required int Port { get; init; }
    }
}