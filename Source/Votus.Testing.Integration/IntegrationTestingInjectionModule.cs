using Ninject;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Votus.Core;
using Votus.Core.Infrastructure.Web;
using Votus.Testing.Integration.ApiClients.Votus;

namespace Votus.Testing.Integration
{
    class IntegrationTestingInjectionModule : NinjectModule
    {
        public 
        override 
        void 
        Load()
        {
            LoadPrerequisites();

            var appSettings  = Kernel.Get<ApplicationSettings>();
            var votusBaseUri = appSettings.WebsiteBaseUri;

            ConfigureSelenium(votusBaseUri);
            ConfigureVotusApiClient(votusBaseUri);
        }

        private 
        void 
        LoadPrerequisites()
        {
            Kernel.Load(
                new[] {
                    new CoreInjectionModule()
                }
            );
        }

        private 
        void 
        ConfigureSelenium(
            Uri baseUri)
        {
            // Configure the Selenium dependencies
            Bind<IWebDriver>()
                .ToMethod(ctx =>
                {
                    var chromeDriver = new ChromeDriver
                    {
                        Url = baseUri.ToString()
                    };

                    chromeDriver
                        .Manage()
                        .Timeouts()
                        .ImplicitlyWait(TimeSpan.FromSeconds(10));

                    return chromeDriver;
                })
                .InSingletonScope();
        }

        private 
        void 
        ConfigureVotusApiClient(
            Uri votusBaseUri)
        {
            Bind<IHttpClient>()
                .ToMethod(ctx => new DotNetHttpClient { BaseUri = votusBaseUri })
                .InTransientScope();

            Bind<VotusApiClient>()
                .ToSelf()
                .InTransientScope();
        }
    }
}