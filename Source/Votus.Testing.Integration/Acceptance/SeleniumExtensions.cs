using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Web;
using OpenQA.Selenium.Support.UI;
using Votus.Testing.Integration.Acceptance.Pages;

namespace Votus.Testing.Integration.Acceptance
{
    static class SeleniumExtensions
    {
        public 
        static
        TPage 
        NavigateToPage<TPage>(
            this
            IWebDriver driver)
                where TPage : BasePage, new()
        {
            return driver.NavigateToPage<TPage>(
                excludeTag: string.Empty // Clear the excludeTag filter by default so "votus-test" items are visible.
            );
        }

        public 
        static
        TPage 
        NavigateToPage<TPage>(
            this 
            IWebDriver  driver,
            string      excludeTag)
                where TPage: BasePage, new()
        {
            // Instantiate the page
            // TODO: Use Ninject to do this?
            var page = new TPage {
                Browser = driver
            };

            // Assemble the target URL
            var destinationUri = driver.GetDestinationUri(
                page,
                excludeTag
            );

            // Attempt to navigate to the pages URL
            driver
                .Navigate()
                .GoToUrl(destinationUri);

            // Initialize the elements in the page class.
            PageFactory.InitElements(
                driver,
                page
            );

            // Return the page.
            return page;
        }

        public
        static
        Uri
        GetDestinationUri(
            this
            IWebDriver  driver, 
            BasePage    page,
            string      excludeTag) // TODO: Pass in generic query string parameters instead of this specific one.
        {
            var baseUrl = new Uri(driver.Url).GetLeftPart(UriPartial.Authority);
            var baseUri = new Uri(baseUrl);

            // Create the correct type of collection.
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            
            if (excludeTag != HomePage.NoExcludeTag)
                queryParams["excludeTag"] = excludeTag;

            var queryString = string.Empty;

            if (queryParams.Count > 0)
                queryString = "?" + queryParams;

            var fullUri = new Uri(
                baseUri:        baseUri,
                relativeUri:    new Uri(page.PageRelativePath + queryString, UriKind.Relative)
            );

            return fullUri;
        }
        
        public
        static
        string
        GetAttributeValue(
            this
            IWebElement element,
            string      attributeName
            )
        {
            return element.GetAttribute(attributeName);
        }

        public 
        static 
        string 
        GetElementText(
            this
            IWebDriver  driver,
            By          by)
        {
            try
            {
                return driver.FindElement(@by).Text;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error occurred getting element text for " + by,
                    ex
                );
            }
        }

        public
        static
        IWebElement
        GetElementByClass(
            this
            IWebElement element,
            object      @class)
        {
            return element.FindElement(By.ClassName(@class.ToString()));
        }

        public
        static
        IWebElement
        GetElementByClass(
            this
            IWebDriver driver,
            object     @class)
        {
            return driver.FindElement(By.ClassName(@class.ToString()));
        }

        public
        static
        IWebElement
        GetElementById(
            this
            IWebDriver  driver,
            object      id)
        {
            try
            {
                return driver.FindElement(By.Id(id.ToString()));
            }
            catch (NoSuchElementException exception)
            {
                throw new NoSuchElementException(
                    string.Format("The ID '{0}' could not be found.", id), 
                    exception
                );
            }
        }

        public 
        static 
        T 
        GetSubElementText<T>(
            this
            IWebElement element,
            By         by)
        {
            var text = element.FindElement(by).Text;

            // Need some special handling for some types, such as guids...
            if (typeof(T) == typeof(Guid))
                return (T)(object)Guid.Parse(text);

            return (T)Convert.ChangeType(text, typeof(T));            
        }

        public 
        static 
        T 
        GetAttributeValue<T>(
            this 
            IWebElement element,
            string      attributeName) 
                where T: struct
        {
            var attributeStringValue = element.GetAttribute(attributeName);

            // Need some special handling for some types, such as guids...
            if (typeof (T) == typeof(Guid))
                return (T)(object)Guid.Parse(attributeStringValue);

            return (T)Convert.ChangeType(attributeStringValue, typeof(T));
        }

        public
            static
            void
            WaitUntil<T>(
                this 
                IWebDriver          driver, 
                Func<IWebDriver, T> condition,
                int                 waitForSeconds = 5)
        {
            new WebDriverWait(
                driver,
                TimeSpan.FromSeconds(waitForSeconds)
            ).Until(condition);
        }
    }
}