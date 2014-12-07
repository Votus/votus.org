using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TechTalk.SpecFlow;
using Votus.Testing.Integration.ApiClients.Votus;
using Votus.Testing.Integration.ApiClients.Votus.Models;
using Votus.Testing.Integration.WebsiteModels;
using Xunit;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    [Binding]
    class IdeaSteps : BaseSteps
    {
        private const string ValidTag        = VotusTestingTag;
        private const string VotusTestingTag = "votus-test";

        [When(@"a Voter submits an Idea")]
        public void WhenAVoterSubmitsAnIdea()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
            ContextSet(Stopwatch.StartNew());
            ContextSet(ContextGet<HomePage>().Ideas.SubmitIdea());
        }

        [When(@"a Voter submits an Idea with a Tag")]
        public 
        void
        WhenAVoterSubmitsANewIdeaWithATag()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
            ContextSet(ContextGet<HomePage>().Ideas.SubmitIdea(tag: ValidTag));
        }

        [Then(@"the Idea appears in the Ideas List")]
        public void ThenTheIdeaAppearsInTheIdeasList()
        {
            var createdIdea = ContextGet<IdeaPageSection>();
            var idea        = ContextGet<HomePage>().Ideas[createdIdea.Id];

            Assert.Equal(createdIdea, idea);
        }

        [Given(@"an Idea exists in the Ideas List")]
        [Given(@"a test Idea exists in the Ideas List")]
        public void GivenAnIdeaExistsInTheIdeasList()
        {
            // Submit the idea to the API.
            var ideaId = VotusApiClient.Ideas
                .Create(tag: VotusTestingTag);

            // Poll the first page of the Ideas List until the idea appears in it
            // TODO: Refactor polling logic to something simple and easy to re-use
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed.TotalSeconds < 10)
            {
                var result = VotusApiClient.Ideas.GetPage();
                var idea   = result.Page.SingleOrDefault(i => i.Id == ideaId);

                if (idea != null)
                {
                    ContextSet(idea);
                    return;
                }

                Thread.Sleep(500);
            }

            throw new Exception(
                string.Format("Could not find Idea {0} in the Ideas List after 10 seconds.", ideaId)
            );
        }
        
        [Then(@"Ideas created for testing purposes do not appear")]
        public void ThenIdeasCreatedForTestingPurposesDoNotAppear()
        {
            var homepageIdeas = ContextGet<HomePage>()
                .Ideas
                .GetIdeasList();

            Assert.False(homepageIdeas.Any(idea => idea.Tag == VotusTestingTag));
        }

        [When(@"the Voter removes test data filter")]
        public void WhenTheVoterRemovesTestDataFilter()
        {
            ContextGet<HomePage>()
                .Ideas
                .ShowTestData();
        }

        [Then(@"Ideas created for testing purposes appear")]
        public void ThenIdeasCreatedForTestingPurposesAppear()
        {
            var submittedIdea = ContextGet<HomePage>()
                .Ideas[ContextGet<Idea>().Id];

            Assert.Equal(VotusTestingTag, submittedIdea.Tag);
        }

        [Then(@"the Voter can view all Ideas")]
        public void ThenTheVoterCanViewAllIdeas()
        {
            // List all ideas from API
            var allApiIdeas = VotusApiClient
                .Ideas
                .GetAllDescending()
                .Select(idea => idea.Id);

            // Find all ideas in UI
            var allUiIdeas = ContextGet<HomePage>()
                .Ideas
                .GetAllDescending()
                .Select(idea => idea.Id);

            // Validate that the UI list has all the same items as the API list.
            Assert.Equal(allApiIdeas, allUiIdeas);
        }

        [When(@"a Voter submits an Idea with an invalid Title")]
        public void WhenAVoterSubmitsANewIdeaWithAnInvalidTitle()
        {
            var invalidTitle = string.Empty;

            try
            {
                ContextGet<HomePage>()
                    .Ideas
                    .SubmitIdea(title: invalidTitle);
            }
            catch (Exception ex)
            {
                ContextSet(ex);
            }
        }

        [Then(@"the error ""(.*)"" is displayed")]
        public void ThenTheErrorIsDisplayed(string expectedErrorMessage)
        {
            var actualErrorMessage = ContextGet<Exception>().Message;

            Assert.Equal(
                expectedErrorMessage, 
                actualErrorMessage
            );
        }

        [When(@"a Voter submits an Idea with Title ""(.*)"" via API")]
        public void WhenAVoterSubmitsAnIdeaWithTitleViaAPI(string title)
        {
            try
            {
                var ideaId = VotusApiClient.Ideas.Create(title: title);

                ContextSet(ideaId);
            }
            catch (VotusApiException votusApiException)
            {
                ContextSet(votusApiException);
            }
        }

        [Then(@"the error ""(.*)"" is returned")]
        public void ThenTheErrorIsReturned(string errorMessage)
        {
            Assert.Contains(errorMessage, ContextGet<VotusApiException>().Message);
        }

        [Then(@"the Idea appears within (.*) seconds")]
        public void ThenTheIdeaAppearsWithinSeconds(int seconds)
        {
            // Get the stopwatch that started when the Idea was submitted...
            var stopwatch = ContextGet<Stopwatch>();

            // This method will return once the idea appears in the list...
            var idea = ContextGet<HomePage>()
                .Ideas[ContextGet<IdeaPageSection>().Id];

            // Stop it now that we got the idea...
            stopwatch.Stop();
            
            // Verify it...
            Assert.InRange(stopwatch.Elapsed.TotalSeconds, 0, seconds);
        }
    }
}