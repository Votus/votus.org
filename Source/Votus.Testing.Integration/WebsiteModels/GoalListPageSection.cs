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
            IWebElement goalListPageElement)
            : base(goalListPageElement)
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
        ConvertToModel(Guid goalId)
        {
            return new GoalPageSection(
                PageSectionElement.GetElementById(goalId)
            );
        }

        private
        static
        GoalPageSection
        ConvertToModel(
            IWebElement goalElement)
        {
            return new GoalPageSection(
                goalElement
            );
        }
    }
}
