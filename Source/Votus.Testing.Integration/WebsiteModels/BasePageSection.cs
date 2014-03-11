using OpenQA.Selenium;

namespace Votus.Testing.Integration.WebsiteModels
{
    abstract class BasePageSection
    {
        protected IWebElement PageSectionElement { get; private set; }

        protected BasePageSection(
            IWebElement pageSectionElement)
        {
            PageSectionElement = pageSectionElement;
        }
    }
}