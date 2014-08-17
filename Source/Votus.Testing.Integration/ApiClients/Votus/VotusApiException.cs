using System;
using Votus.Core.Infrastructure.Web;

namespace Votus.Testing.Integration.ApiClients.Votus
{
    class VotusApiException : Exception
    {
        public 
        VotusApiException(
            RequestFailedException  requestFailedException)
            : base(requestFailedException.Response.Body)
        {
        }
    }
}
