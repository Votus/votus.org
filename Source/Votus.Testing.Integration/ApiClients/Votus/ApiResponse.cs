using System.Collections.Generic;
using System.IO;
using System.Net;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Testing.Integration.ApiClients.Votus
{
    class ApiResponse<TPayload>
    {
        public ApiError ApiError { get; set; }
        
        public 
        ApiResponse(
            WebException    webException, 
            ISerializer     serializer)
        {
            using (var responseStream = webException.Response.GetResponseStream())
            {
                var response = new StreamReader(responseStream).ReadToEnd();
                ApiError     = serializer.Deserialize<ApiError>(response);
            }
        }

        public ApiResponse(TPayload payload)
        {
            Payload = payload;
        }

        public TPayload Payload { get; set; }
    }

    class ApiError
    {
        public string                       Message             { get; set; }
        public string                       ExceptionMessage    { get; set; }
        public Dictionary<string, string[]> ModelState          { get; set; }

        public override string ToString()
        {
            return string.Format(
                "User Message:      {0}\r\n" + 
                "Exception Message: {1}", Message, ExceptionMessage
            );
        }
    }
}
