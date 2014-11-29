using TechTalk.SpecFlow;
using Votus.Testing.Integration.WebsiteModels;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    [Binding]
    class NavigationSteps : BaseSteps
    {
        [When(@"a Voter navigates to the Homepage")]
        [When(@"the Voter navigates to the Homepage")]
        [Given(@"the Voter navigates to the Homepage")]
        public void GivenTheVoterIsOnTheHomepage()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
        }

        [When(@"a Voter navigates to the Homepage \(no exclude tag\)")]
        public void WhenAVoterNavigatesToTheHomepageNoExcludeTag()
        {
            ContextSet(Browser.NavigateToPage<HomePage>(excludeTag: HomePage.NoExcludeTag));
        }

        [Given(@"I navigate to the Admin Dashboard")]
        public void GivenINavigateToTheAdminDashboard()
        {
            ContextSet(Browser.NavigateToPage<AdminConsolePage>());
        }
    }
}
