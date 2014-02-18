using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Votus.Testing.Integration.Acceptance
{
    [Binding]
    static class SeleniumHandlers
    {
        [AfterTestRun]
        static
        void
        AfterTestRun()
        {
            using (var driver = SharedResources.Get<IWebDriver>())
                driver.Close();
        }
    }
}
