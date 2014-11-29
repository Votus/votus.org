using System;
using System.Linq;
using System.Threading;
using TechTalk.SpecFlow;
using Votus.Testing.Integration.WebsiteModels;
using Xunit;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    [Binding]
    class EventStoreManagementSteps : BaseSteps
    {
        [Given(@"an event exists in the Event Store")]
        public void GivenAnEventExistsInTheEventStore()
        {
            // Create a new entity which will do so by adding a new event to the Event Store.
            ContextSet(
                VotusApiClient.TestEntities.Create()
            );

            // Wait for the view to be updated with the test info.
        }

        [When(@"I republish all events")]
        public void WhenIRepublishAllEvents()
        {
            ContextGet<AdminConsolePage>()
                .RepublishAllEvents();
        }

        [Then(@"the event is republished")]
        public void ThenTheEventIsRepublished()
        {
            var testEntityId    = ContextGet<Guid>();
            var testEvents      = VotusApiClient.TestEntities.GetRecent();

            var matches = testEvents.Where(@event => 
                @event.TestEntityId == testEntityId
            );

            Thread.Sleep(10000);

            Assert.Equal(2, matches.Count());
         }
    }
}
