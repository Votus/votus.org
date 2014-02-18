
using OpenQA.Selenium;

namespace Votus.Testing.Integration.Acceptance.Pages
{
    abstract class BasePage
    {
        public IWebDriver   Browser             { get; set; }
        public string       PageRelativePath    { get; set; }

        protected BasePage(string relativePath)
        {
            PageRelativePath = relativePath;
        }
    }
}
