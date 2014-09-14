﻿using System;
using TechTalk.SpecFlow;
using Votus.Testing.Integration.ApiClients.Votus.Models;
using Votus.Testing.Integration.WebsiteModels;
using Xunit;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    [Binding]
    class GoalSteps : BaseSteps
    {
        private const string ValidGoalTitle   = "This is a valid goal";
        private const string InvalidGoalTitle = "invalidgoal";

        [When(@"a Voter submits a valid Goal to the Idea")]
        public void WhenAVoterSubmitsAValidGoalToTheIdea()
        {
            var idea = ContextGet<Idea>();

            var goal = ContextGet<HomePage>()
                .Ideas[idea.Id]
                .ShowGoalsDisplay()
                .SubmitGoal(ValidGoalTitle);

            ContextSet(goal);
        }
        
        [Then(@"the Goal appears under the Idea")]
        public void ThenTheGoalAppearsUnderTheIdea()
        {
            var idea    = ContextGet<Idea>();
            var newGoal = ContextGet<GoalPageSection>();

            var actualGoal = ContextGet<HomePage>()
                .Ideas[idea.Id]
                .Goals[newGoal.Id];

            Assert.NotNull(actualGoal);
        }

        [When(@"a Voter submits an invalid Goal to the Idea")]
        public void WhenAVoterSubmitsAnInvalidGoalToTheIdea()
        {
            var idea = ContextGet<Idea>();

            try
            {
                ContextGet<HomePage>()
                    .Ideas[idea.Id]
                    .ShowGoalsDisplay()
                    .SubmitGoal(InvalidGoalTitle);
            }
            catch (Exception ex)
            {
                ContextSet(ex);
            }
        }
    }
}
