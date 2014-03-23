using System.Net;
using TechTalk.SpecFlow;
using Votus.Core.Infrastructure.Web;
using Xunit;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    [Binding]
    class ApiSteps : BaseSteps
    {
        [Given(@"the API User requests a resource that does not exist")]
        public void GivenTheAPIUserRequestsAResourceThatDoesNotExist()
        {
            try
            {
                VotusApiClient.HttpClient.Get<object>("/api/ideas/7d6845da-e8cd-48d3-a99a-1b829bf7034b");
            }
            catch (RequestFailedException ex)
            {
                ContextSet(ex);
            }
        }

        [Then(@"the API returns a Not Found response")]
        public void ThenTheAPIReturnsANotFoundResponse()
        {
            var exception = ContextGet<RequestFailedException>();

            Assert.Equal(
                HttpStatusCode.NotFound, 
                exception.Response.StatusCode
            );
        }
    }
}
