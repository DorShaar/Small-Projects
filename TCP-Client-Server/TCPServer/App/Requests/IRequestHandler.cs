namespace TCPServerApp.App.Requests
{
    public interface IRequestHandler
    {
        byte[] ProcessRequest(byte[] requestContent, int actualSize);
    }
}