using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;

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

        [FindsBy] public IWebElement SystemVersionInfo        = null;
        [FindsBy] public IWebElement EnvironmentName          = null;

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