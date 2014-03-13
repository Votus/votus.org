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

        private readonly IWebDriver _browser;

        [FindsBy] IWebElement LoadNextIdeasButton = null;
        [FindsBy] IWebElement SubmitNewIdeaButton = null;
        [FindsBy] IWebElement NewIdeaId           = null;
        [FindsBy] IWebElement NewIdeaTitle        = null;
        [FindsBy] IWebElement NewIdeaTag          = null;

        #endregion

        #region Properties

        public IdeaPageSection this[Guid ideaId] { get { return ConvertToModel(ideaId); } }

        #endregion

        #region Constructors

        public IdeaListPageSection(
            IWebDriver  browser)
            : base(browser.GetElementById("Ideas"))
        {
            _browser = browser;
            PageFactory.InitElements(_browser, this);
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
            return _browser.GetElementText(By.ClassName("field-validation-error"));
        }

        private 
        IdeaPageSection 
        ConvertToModel(
            IWebElement ideaElement)
        {
            return new IdeaPageSection(ideaElement);
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
        bool 
        LoadNextPage()
        {
            if (!LoadNextIdeasButton.Displayed) return false;
                
            LoadNextIdeasButton.Click();
                
            // TODO: Detect errors, throw exception...
                
            // The button becomes either:
            // Enabled:       If the results came back and there is another page or,
            // Not Displayed: If the results came back and there is not another page
            _browser.WaitUntil(driver => 
                LoadNextIdeasButton.Enabled || LoadNextIdeasButton.Displayed == false
            );
            
            return LoadNextIdeasButton.Displayed;
        }
    }
}
