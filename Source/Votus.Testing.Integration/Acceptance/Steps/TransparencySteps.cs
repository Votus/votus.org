using System;
using System.Reflection;
using TechTalk.SpecFlow;
using Votus.Core;
using Votus.Testing.Integration.Acceptance.Pages;
using Xunit;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    [Binding]
    class TransparencySteps : BaseSteps
    {
        [Then(@"the version number is visible")]
        public void ThenTheVersionNumberIsVisible()
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var serverVersion  = ContextGet<HomePage>().GetSystemVersionNumber();

            Assert.Equal(currentVersion, serverVersion);
        }

        [Then(@"the environment name is visible")]
        public void ThenTheEnvironmentNameIsVisible()
        {
            var settings = SharedResources.Get<ApplicationSettings>();
            var serverEnvName = ContextGet<HomePage>().GetEnvironmentName();

            Assert.Equal(
                settings.EnvironmentName, 
                serverEnvName, 
                StringComparer.CurrentCultureIgnoreCase
            );
        }
    }
}
