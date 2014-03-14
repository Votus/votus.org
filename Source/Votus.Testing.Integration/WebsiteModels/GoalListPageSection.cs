using System;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class GoalListPageSection : BasePageSection
    {
        public
        GoalListPageSection(
            IWebDriver  browser,
            IWebElement goalListPageElement)
            : base(browser, goalListPageElement)
        {
        }

        public GoalPageSection this[Guid goalId]
        {
            get { return ConvertToModel(goalId); }
        }

        public
        IEnumerable<GoalPageSection>
        GetGoalsList()
        {
            return PageSectionElement
                .FindElements(By.ClassName("Goal"))
                .Select(ConvertToModel);
        }

        private
        GoalPageSection
        ConvertToModel(
            Guid goalId)
        {
            return new GoalPageSection(
                Browser,
                PageSectionElement.GetElementById(goalId)
            );
        }

        private
        GoalPageSection
        ConvertToModel(
            IWebElement goalElement)
        {
            return new GoalPageSection(
                Browser,
                goalElement
            );
        }

        public 
        GoalPageSection
        SubmitGoal(
            string goalTitle)
        {
            // Get the id that will be used for the new goal
            var newGoalId = PageSectionElement
                .GetElementByClass("NewGoalId")
                .GetAttributeValue<Guid>("value");

            // Input the goal title
            SendKeysToNewGoalTitle(goalTitle);

            var newGoalButton = PageSectionElement.GetElementByClass("NewGoalButton");

            if (!newGoalButton.Displayed)
                throw new Exception(GetCurrentErrorMessage());

            PageSectionElement
                .GetElementByClass("NewGoalButton")
                .Click();

            return this[newGoalId];
        }

        public
        string
        GetCurrentErrorMessage()
        {
            return PageSectionElement.GetSubElementTextAs<string>(By.ClassName("field-validation-error"));
        }

        public 
        void
        SendKeysToNewGoalTitle(
            string newGoalTitle)
        {
            PageSectionElement
                .GetElementByClass("NewGoalTitle")
                .SendKeys(newGoalTitle);
        }
    }
}
