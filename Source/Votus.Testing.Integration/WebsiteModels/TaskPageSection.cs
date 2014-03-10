using System;
using OpenQA.Selenium;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class TaskPageSection : BasePageSection
    {
        private readonly IWebElement _taskElement;

        public Guid Id { get; set; }
    
        public 
        TaskPageSection(
            IWebElement taskElement)
        {
            _taskElement = taskElement;
        }

        public string Title
        {
            get
            {
                return _taskElement.GetElementByClass("Title").Text;
            }
        }

        public int CompletedVoteCount
        {
            get
            {
                return _taskElement.GetSubElementText<int>(
                    By.ClassName("CompletedVoteCount")
                );
            }
        }
    }
}
