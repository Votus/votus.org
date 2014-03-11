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
        #region Variables

        [FindsBy] 
        private IWebElement LoadNextIdeasButton = null;
        private IWebDriver _browser;

        #endregion

        #region Properties

        public IdeaPageSection this[Guid ideaId]{ get { return ConvertToModel(ideaId); } }

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
        IEnumerable<IdeaPageSection>
        GetIdeasList()
        {
            return PageSectionElement
                .FindElements(By.ClassName("Idea"))
                .Select(ConvertToModel);
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
