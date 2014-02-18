using System;
using Votus.Core.Infrastructure.Serialization;
using Votus.Core.Infrastructure.Web;

namespace Votus.Testing.Integration.ApiClients.Votus
{
    class VotusApiException : Exception
    {
        public 
        VotusApiException(
            ISerializer             serializer,
            RequestFailedException  requestFailedException)
            : base(GetExceptionMessage(serializer, requestFailedException), requestFailedException)
        {
        }

        private
        static 
        string 
        GetExceptionMessage(
            ISerializer             serializer,
            RequestFailedException  requestFailedException)
        {
            var serverErrorMessage = serializer.Deserialize<ApiError>(
                requestFailedException.Response.Body
            );

            var errorMessage = string.Format(
                "The server failed to process the request:\r\n{0}", 
                serverErrorMessage
            );

            return errorMessage;
        }
    }
}
