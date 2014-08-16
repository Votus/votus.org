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

        [When(@"a Voter submits a new Idea")]
        public void WhenAVoterSubmitsANewIdea()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
            ContextSet(Stopwatch.StartNew());
            ContextSet(ContextGet<HomePage>().Ideas.SubmitIdea());
        }

        [When(@"a Voter submits a new idea with a tag")]
        public 
        void
        WhenAVoterSubmitsANewIdeaWithATag()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
            ContextSet(ContextGet<HomePage>().Ideas.SubmitIdea(tag: ValidTag));
        }

        [Then(@"the idea appears in the Ideas list")]
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
            var command = new CreateIdeaCommand(newIdeaTag: VotusTestingTag);

            // Issue the command to create the test idea...
            VotusApiClient.Commands.Send(command.NewIdeaId, command);

            // Poll the first page of the list until the idea appears in it
            // TODO: Refactor polling logic to something simple and easy to re-use
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed.TotalSeconds < 10)
            {
                var result = VotusApiClient.Ideas.GetPage();
                var idea   = result.Page.SingleOrDefault(i => i.Id == command.NewIdeaId);

                if (idea != null)
                {
                    ContextSet(idea);
                    return;
                }

                Thread.Sleep(500);
            }

            throw new Exception(
                string.Format("Could not find Idea {0} in the Ideas List after 10 seconds.", command.NewIdeaId)
            );
        }
        
        [Then(@"ideas created for testing purposes do not appear")]
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

        [Then(@"ideas created for testing purposes appear")]
        public void ThenIdeasCreatedForTestingPurposesAppear()
        {
            var submittedIdea = ContextGet<HomePage>()
                .Ideas[ContextGet<Idea>().Id];

            Assert.Equal(VotusTestingTag, submittedIdea.Tag);
        }

        [Given(@"at least 1 idea exists")]
        public void GivenAtLeast1IdeaExists()
        {
            var command = new CreateIdeaCommand();

            // Issue the command to create the idea...
            VotusApiClient.Commands.Send(command.NewIdeaId, command);

            // Poll the API until the idea is available...
            ContextSet(VotusApiClient.Ideas.Get(command.NewIdeaId));
        }

        [Then(@"the Voter can view all ideas")]
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

        [When(@"a Voter submits a new idea with an invalid title")]
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

        [When(@"a Voter submits a new idea with title ""(.*)"" via API")]
        public void WhenAVoterSubmitsANewIdeaWithTitleViaAPI(string title)
        {
            try
            {
                var command = new CreateIdeaCommand(newIdeaTitle: title);

                VotusApiClient.Commands.Send(command.NewIdeaId, command);

                ContextSet(command);
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