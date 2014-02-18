using System;

namespace Votus.Core.Infrastructure.Web
{
    public class RequestFailedException : Exception
    {
        public 
        HttpResponse
        Response { get; set; }
        
        public 
        RequestFailedException(HttpResponse response)
            : base(GetErrorMessage(response))
        {
            Response = response;
        }
        
        private 
        static 
        string 
        GetErrorMessage(HttpResponse response)
        {
            return string.Format(
                "{0} {1}: {2}",
                (int)response.StatusCode,
                response.StatusCode,
                response.StatusDescription);
        }
    }
}
