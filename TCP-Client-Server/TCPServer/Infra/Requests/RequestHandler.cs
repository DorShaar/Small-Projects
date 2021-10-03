using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using TCPServerApp.App.Requests;

namespace TCPServerApp.Infra.Requests
{
    public class RequestHandler : IRequestHandler
    {
        private const string ParsingFailedResponse= "Failed Parsing Request";
        private const string InvalidRequestResponse = "Invalid Request";
        private readonly ILogger<RequestHandler> mLogger;

        public RequestHandler(ILogger<RequestHandler> logger)
        {
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public byte[] ProcessRequest(byte[] requestContent, int actualSize)
        {
            try
            {
                string requestString = Encoding.ASCII.GetString(requestContent, 0, actualSize);

                switch(requestString.ToLower())
                {
                    case "hostname":
                        return PrepareResponeForBase64Bytes(Dns.GetHostName());
                    case "ip":
                        string ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
                        return PrepareResponeForBase64Bytes(ip);
                    default:
                        return PrepareResponeForBase64Bytes(InvalidRequestResponse);
                }
            }
            catch(Exception ex)
            {
                mLogger.LogError(ex, "Could not parse request content");
                return PrepareResponeForBase64Bytes(ParsingFailedResponse);
            }
        }

        private static byte[] PrepareResponeForBase64Bytes(string response)
        {
            byte[] responseAsBytes = Encoding.ASCII.GetBytes(response);
            string convertedBase64 = Convert.ToBase64String(responseAsBytes);
            return Encoding.ASCII.GetBytes(convertedBase64);
        }
    }
}