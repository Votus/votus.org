using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace Votus.Testing.Integration.WebsiteModels
{
    class AdminConsolePage : BasePage
    {
        [FindsBy] public IWebElement RepublishEventsButton = null;

        public AdminConsolePage() : base("/Admin")
        {
        }

        public void RepublishAllEvents()
        {
            RepublishEventsButton.Click();

            // TODO: Wait for result, throw exception if error
        }
    }
}
