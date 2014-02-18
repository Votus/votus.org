using System.Net;

namespace Votus.Core.Infrastructure.Net
{
    public static class Extensions
    {
        public 
        static 
        string
        ToTraceString(
            this 
            WebRequest  request,
            string      body)
        {
            return string.Format(
                "\r\n" +
                "HTTP REQUEST ======================================\r\n" +
                "{0} {1}\r\n" + 
                "{2}\r\n" + 
                "{3}\r\n" + 
                "================================================", 
                request.Method, 
                request.RequestUri,
                request.Headers,
                body
            );
        }
    }
}
