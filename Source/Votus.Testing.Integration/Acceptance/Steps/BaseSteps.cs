using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Votus.Testing.Integration.ApiClients.Votus;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    abstract class BaseSteps
    {
        protected IWebDriver        Browser         { get; set; }
        protected VotusApiClient    VotusApiClient  { get; set; }

        protected BaseSteps()
        {
            Browser        = SharedResources.Get<IWebDriver>();
            VotusApiClient = SharedResources.Get<VotusApiClient>();

            ContextSet(this);
        }

        protected T ContextGet<T>()
        {
            return ScenarioContext.Current.Get<T>();
        }

        protected void ContextSet<T>(T value)
        {
            var key = typeof (T) == typeof (BaseSteps) ?
                GetType().FullName : typeof(T).FullName;

            ScenarioContext.Current.Set(value, key);
        }
    }
}