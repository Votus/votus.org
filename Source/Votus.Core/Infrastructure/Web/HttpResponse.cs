using System;
using System.IO;
using System.Net;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.Web
{
    public class HttpResponse
    {
        public TimeSpan         Duration    { get; set; }
        public string           Body        { get; set; }
        public HttpWebResponse  RawResponse { get; set; }
        
        public HttpStatusCode   StatusCode          { get { return RawResponse.StatusCode; } }
        public string           StatusDescription   { get { return RawResponse.StatusDescription; } }
        
        public
        HttpResponse(WebException webException, TimeSpan duration)
            : this(webException.Response, duration)
        {
        }

        public 
        HttpResponse(WebResponse response, TimeSpan duration)
            : this(response, GetResponseBody(response), duration)
        {
            
        }

        public
        HttpResponse(WebResponse response, string responseBody, TimeSpan duration)
        {
            Duration    = duration;
            Body        = responseBody;
            RawResponse = (HttpWebResponse)response;
        }

        private 
        static 
        string 
        GetResponseBody(
            WebResponse response)
        {
            using (var responseStream = response.GetResponseStream())
                if (responseStream != null) return new StreamReader(responseStream).ReadToEnd();
        
            return null;
        }

        public override string ToString()
        {
            return string.Format(
                "\r\n" +
                "HTTP RESPONSE =====================================\r\n" + 
                "{0} {1} {2}\r\n" +
                "{3}\r\n" +
                "Duration: {4}\r\n" +
                "================================================", 
                (int)StatusCode, 
                StatusCode, 
                StatusDescription,
                Body,
                Duration
            );
        }
    }

    public class HttpResponse<TPayload> : HttpResponse
    {
        public HttpResponse(ISerializer serializer, HttpResponse response)
            : base(response.RawResponse, response.Body, response.Duration)
        {
            Payload = serializer.Deserialize<TPayload>(Body);
        }

        public TPayload Payload { get; set; }
    }
}
