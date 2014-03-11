using System;
using OpenQA.Selenium;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class TaskPageSection : BasePageSection
    {
        public Guid Id { get; set; }
    
        public 
        TaskPageSection(
            IWebElement taskElement)
            : base(taskElement)
        {
        }

        public string Title
        {
            get
            {
                return PageSectionElement.GetElementByClass("Title").Text;
            }
        }

        public int CompletedVoteCount
        {
            get
            {
                return PageSectionElement.GetSubElementText<int>(
                    By.ClassName("CompletedVoteCount")
                );
            }
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
