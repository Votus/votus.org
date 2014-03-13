using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using Votus.Testing.Integration.Acceptance;
using Votus.Testing.Integration.ApiClients.Votus.Models;

namespace Votus.Testing.Integration.WebsiteModels
{
    class HomePage : BasePage
    {
        #region Constants

        public const string NoExcludeTag     = null;
        public const string RelativePath     = "/";
        public const string InvalidGoalTitle = "InvalidTitle";
        public const string InvalidTaskTitle = "InvalidTitle";

        #endregion

        #region HTML Elements

        [FindsBy] public IWebElement Tags                     = null;
        [FindsBy] public IWebElement TagButtonVotusTest       = null;
        [FindsBy] public IWebElement SystemVersionInfo        = null;
        [FindsBy] public IWebElement EnvironmentName          = null;
        [FindsBy] public IWebElement TagFilterLoadingIdeasIcon= null;

        #endregion

        #region Properties

        private IdeaListPageSection _ideas;
        public IdeaListPageSection Ideas
        {
            get { return _ideas ?? (_ideas = new IdeaListPageSection(Browser)); }
        }

        #endregion

        #region Constructors

        public 
        HomePage() 
            : base(RelativePath)
        {
        }

        #endregion

        #region Page Methods

        public 
        void
        ShowTestData()
        {
            var excludedTags = Tags.FindElements(By.ClassName("TagExcluded"));

            if (!excludedTags.Contains(TagButtonVotusTest)) return;

            // De-select the "votus-testing" tag button 
            TagButtonVotusTest.Click();

            new WebDriverWait(Browser, TimeSpan.FromSeconds(15))
                .Until(browser => !TagFilterLoadingIdeasIcon.Displayed);
        }

        public 
        Version 
        GetSystemVersionNumber()
        {
            return new Version(SystemVersionInfo.Text.TrimStart('v'));
        }

        public 
        string 
        GetEnvironmentName()
        {
            return EnvironmentName.Text;
        }

        #endregion
    }
}