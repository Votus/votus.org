using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    public class IdeaListPageSection
    {
        public IWebDriver Browser { get; set; }

        [FindsBy]
        public IWebElement LoadNextIdeasButton { get; set; }

        public
        IdeaPageSection
        GetIdeaFromList(
            Guid id)
        {
            return ConvertToModel(id);
        }

        public
        IEnumerable<IdeaPageSection>
        GetIdeasList()
        {
            return Browser
                .FindElements(By.CssSelector("#Ideas .Idea"))
                .Select(e => e.GetAttributeValue<Guid>("Id"))
                .Select(ConvertToModel);
        }

        private 
        IdeaPageSection
        ConvertToModel(
            Guid ideaId)
        {
            var ideaElement = Browser.GetElementById(ideaId);
            
            return new IdeaPageSection {
                Id      = ideaId,
                Title   = ideaElement.GetElementByClass("Title").Text,
                Tag     = ideaElement.GetElementByClass("Tag").Text,
                Browser = Browser
            };
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
            Browser.WaitUntil(driver => 
                LoadNextIdeasButton.Enabled || LoadNextIdeasButton.Displayed == false
            );

            return LoadNextIdeasButton.Displayed;
        }

        public IdeaPageSection this[Guid ideaId]
        {
            get
            {
                return GetIdeaFromList(ideaId);
            }
        }
    }
}
