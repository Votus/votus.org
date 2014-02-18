﻿using TechTalk.SpecFlow;
using Votus.Testing.Integration.Acceptance.Pages;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    [Binding]
    class NavigationSteps : BaseSteps
    {
        [Given(@"the Voter is on the Homepage")]
        public void GivenTheVoterIsOnTheHomepage()
        {
            ContextSet(Browser.NavigateToPage<HomePage>());
        }
    }
}
