using System;
using OpenQA.Selenium;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class GoalPageSection : BasePageSection
    {
        public Guid     Id      { get { return PageSectionElement.GetAttributeValue<Guid>("Id"); } }
        public string   Title   { get { return PageSectionElement.GetElementByClass("Title").Text; } }

        public 
        GoalPageSection(
            IWebElement goalElement)
            : base(goalElement)
        {
        }
    }
}
