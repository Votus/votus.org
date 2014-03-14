using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace Votus.Testing.Integration.WebsiteModels
{
    abstract class BasePageSection
    {
        protected IWebDriver    Browser             { get; set; }
        protected IWebElement   PageSectionElement  { get; private set; }

        protected BasePageSection(
            IWebDriver  browser,
            IWebElement pageSectionElement)
        {
            Browser            = browser;
            PageSectionElement = pageSectionElement;

            PageFactory.InitElements(Browser, this);
        }

        protected 
        void 
        WaitUntil(
            Func<IWebDriver, bool> condition)
        {
            new WebDriverWait(
                Browser, 
                TimeSpan.FromSeconds(10)
            ).Until(condition);
        }
    }
}