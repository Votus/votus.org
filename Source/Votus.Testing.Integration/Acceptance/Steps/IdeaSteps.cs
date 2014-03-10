using System.Diagnostics;
using System.Linq;
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

        [When(@"a Voter submits a new idea")]
        public 
        void 
        WhenAVoterSubmitsANewIdea()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
            ContextSet(Stopwatch.StartNew());
            ContextSet(ContextGet<HomePage>().SubmitIdea());
        }

        [When(@"a Voter submits a new idea with a tag")]
        public 
        void
        WhenAVoterSubmitsANewIdeaWithATag()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
            ContextSet(ContextGet<HomePage>().SubmitIdea(tag: ValidTag));
        }

        [Then(@"the idea appears in the Ideas list")]
        public void ThenTheIdeaAppearsInTheIdeasList()
        {
            var createdIdea = ContextGet<IdeaSection>();
            var idea        = ContextGet<HomePage>().Ideas[createdIdea.Id];

            Assert.Equal(createdIdea, idea);
        }

        [Given(@"a test idea exists")]
        public void GivenATestIdeaExists()
        {
            var command = new CreateIdeaCommand(tag: VotusTestingTag);

            // Issue the command to create the test idea...
            VotusApiClient.Commands.Send(command.NewIdeaId, command);

            // Poll the API until the idea is available...
            ContextSet(VotusApiClient.Ideas.Get(command.NewIdeaId));
        }

        [When(@"a Voter navigates to the Homepage")]
        public void WhenAVoterNavigatesToTheHomepage()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
        }

        [When(@"a Voter navigates to the Homepage \(no exclude tag\)")]
        public void WhenAVoterNavigatesToTheHomepageNoExcludeTag()
        {
            ContextSet(Browser.NavigateToPage<HomePage>(excludeTag: HomePage.NoExcludeTag));
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
            ContextGet<HomePage>().ShowTestData();
        }

        [Then(@"ideas created for testing purposes appear")]
        public void ThenIdeasCreatedForTestingPurposesAppear()
        {
            var submittedIdea = ContextGet<HomePage>()
                .Ideas
                .GetIdeaFromList(ContextGet<Idea>().Id);

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
        public void WhenAVoterSubmitsANewIdeaWithTitle()
        {
            ContextGet<HomePage>().SubmitInvalidIdea();
        }

        [Then(@"the error ""(.*)"" is displayed")]
        public void ThenTheErrorIsDisplayed(string errorMessage)
        {
            Assert.Equal(errorMessage, ContextGet<HomePage>().GetCurrentErrorMessage());
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

        [Then(@"the idea appears in the Ideas list within (.*) seconds")]
        public void ThenTheIdeaAppearsInTheIdeasListWithinSeconds(int seconds)
        {
            // Get the stopwatch that started when the Idea was submitted...
            var stopwatch = ContextGet<Stopwatch>();

            // This method will return once the idea appears in the list...
            ContextGet<HomePage>()
                .Ideas
                .GetIdeaFromList(ContextGet<IdeaSection>().Id);

            // Stop it now that we got the idea...
            stopwatch.Stop();
            
            // Verify it...
            Assert.InRange(stopwatch.Elapsed.TotalSeconds, 0, seconds);
        }
    }
}