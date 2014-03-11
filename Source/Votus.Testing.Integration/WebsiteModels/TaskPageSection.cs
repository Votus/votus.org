using System;
using OpenQA.Selenium;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class TaskPageSection : BasePageSection
    {
        public Guid     Id                  { get { return PageSectionElement.GetAttributeValue<Guid>("Id"); } }
        public string   Title               { get { return PageSectionElement.GetElementByClass("Title").Text; } }
        public int      CompletedVoteCount  { get { return PageSectionElement.GetSubElementTextAs<int>(By.ClassName("CompletedVoteCount")); } }
    
        public 
        TaskPageSection(
            IWebElement taskElement)
            : base(taskElement)
        {
        }

        public
        void
        VoteCompleted()
        {
            PageSectionElement
                .GetElementByClass("VoteCompletedButton")
                .Click();

            // TODO: Wait for success/fail
        }
    }
}
