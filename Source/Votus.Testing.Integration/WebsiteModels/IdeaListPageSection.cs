using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class IdeaListPageSection : BasePageSection
    {
        #region Constants

        public const string VotusTestingTag  = "votus-test";
        public const string ValidIdeaTitle   = "Valid test idea";

        #endregion

        #region Variables

        [FindsBy] IWebElement Tags                      = null;
        [FindsBy] IWebElement TagButtonVotusTest        = null;
        [FindsBy] IWebElement LoadNextIdeasButton       = null;
        [FindsBy] IWebElement SubmitNewIdeaButton       = null;
        [FindsBy] IWebElement NewIdeaId                 = null;
        [FindsBy] IWebElement NewIdeaTitle              = null;
        [FindsBy] IWebElement NewIdeaTag                = null;
        [FindsBy] IWebElement TagFilterLoadingIdeasIcon = null;

        #endregion

        #region Properties

        public IdeaPageSection this[Guid ideaId] { get { return ConvertToModel(ideaId); } }

        #endregion

        #region Constructors

        public IdeaListPageSection(
            IWebDriver browser)
            : base(browser, browser.GetElementById("Ideas"))
        {
        }

        #endregion

        public
        IdeaPageSection
        SubmitIdea(
            string tag      = VotusTestingTag,
            string title    = ValidIdeaTitle)
        {
            // Get the id that will be used for the new idea.
            var id = NewIdeaId.GetAttributeValue<Guid>(attributeName: "value");

            // Append the test run id to the titles
            title  = string.Format("{0} {1}", title, SharedResources.TestId);

            // Enter the values in to the tag and title inputs.
            NewIdeaTag.SendKeys(tag);
            NewIdeaTitle.SendKeys(title);

            if (!SubmitNewIdeaButton.Displayed)
                throw new Exception(GetCurrentErrorMessage());

            SubmitNewIdeaButton.Click();
            return this[id];
        }

        public
        IEnumerable<IdeaPageSection>
        GetIdeasList()
        {
            return PageSectionElement
                .FindElements(By.ClassName("Idea"))
                .Select(ConvertToModel);
        }

        public
        string
        GetCurrentErrorMessage()
        {
            return Browser.GetElementText(By.ClassName("field-validation-error"));
        }

        private 
        IdeaPageSection 
        ConvertToModel(
            IWebElement ideaElement)
        {
            return new IdeaPageSection(Browser, ideaElement);
        }

        private 
        IdeaPageSection
        ConvertToModel(
            Guid ideaId)
        {
            return ConvertToModel(
                PageSectionElement.GetElementById(ideaId)
            );
        }

        public 
        IEnumerable<IdeaPageSection>
        GetAllDescending()
        {
            bool morePages;

            do
                morePages = LoadNextPage();
            while (morePages);

            return GetIdeasList();
        }

        public
        void
        ShowTestData()
        {
            var excludedTags = Tags.FindElements(By.ClassName("TagExcluded"));

            if (!excludedTags.Contains(TagButtonVotusTest)) return;

            // De-select the "votus-testing" tag button 
            TagButtonVotusTest.Click();

            WaitUntil(browser => 
                !TagFilterLoadingIdeasIcon.Displayed
            );
        }

        public 
        bool 
        LoadNextPage()
        {
            if (!LoadNextIdeasButton.Displayed) return false;
                
            LoadNextIdeasButton.Click();
                
            // TODO: Detect errors, throw exception...
                
            // The button becomes either:
            // Enabled:       If the results came back and there is another page or,
            // Not Displayed: If the results came back and there is not another page
            WaitUntil(driver => 
                LoadNextIdeasButton.Enabled || LoadNextIdeasButton.Displayed == false
            );
            
            return LoadNextIdeasButton.Displayed;
        }
    }
}
