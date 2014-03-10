using OpenQA.Selenium;

namespace Votus.Testing.Integration.WebsiteModels
{
    abstract class BasePageSection
    {
        public IWebDriver Browser { get; set; }
    }
}
