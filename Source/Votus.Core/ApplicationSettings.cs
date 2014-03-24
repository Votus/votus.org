using Microsoft.WindowsAzure.Storage;
using Ninject;
using System;
using Votus.Core.Infrastructure.Configuration;

namespace Votus.Core
{
    public class ApplicationSettings
    {
        [Inject]
        public ConfigManager ConfigManager { get; set; }

        public const string EnvironmentNameConfigName = "Votus.Environment.Name";
        public string EnvironmentName
        {
            get { return ConfigManager.Get(settingName: EnvironmentNameConfigName); }
            set {        ConfigManager.Set(settingName: EnvironmentNameConfigName, value: value);}
        }

        public const string AzureDataCenterLocationConfigName = "Votus.Azure.DataCenter.Location";
        public string AzureDataCenterLocation
        {
            get { return ConfigManager.Get(AzureDataCenterLocationConfigName); }
            set { ConfigManager.Set(AzureDataCenterLocationConfigName, value); }
        }

        public string AzureWebsiteServiceName
        {
            get
            {
                return string.Format(
                    "{0}-votus-web-{1}", 
                    EnvironmentName, 
                    AzureDataCenterLocation.Replace(" ", "-")
                ).ToLower();
            }
        }

        public const string VotusWebsiteBaseUrlConfigName = "Votus.Website.BaseUrl";
        public string WebsiteBaseUrl
        {
            get 
            { 
                return ConfigManager.Get(
                    VotusWebsiteBaseUrlConfigName, 
                    defaultValue: string.Format("http://{0}.azurewebsites.net", AzureWebsiteServiceName)
                ); 
            }
            set { ConfigManager.Set(VotusWebsiteBaseUrlConfigName, value); }
        }

        public Uri WebsiteBaseUri
        {
            get { return new Uri(WebsiteBaseUrl); }
        }

        public string AppStorageAccountName
        {
            get { return string.Format("{0}votusapp{1}", EnvironmentName, AzureDataCenterLocation.Replace(" ", "")).ToLower(); }
        }

        public const string AppStorageAccountKeyConfigName = "Votus.App.Storage.AccountKey";
        public string AppStorageAccountKey
        {
            get { return ConfigManager.Get(AppStorageAccountKeyConfigName); }
            set { ConfigManager.Set(AppStorageAccountKeyConfigName, value); }
        }

        public string AppStorageConnectionString
        {
            get 
            {
                return string.Format(
                    "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};", 
                    AppStorageAccountName, 
                    AppStorageAccountKey
                );
            }
        }

        public CloudStorageAccount AppCloudStorageAccount
        {
            get { return CloudStorageAccount.Parse(AppStorageConnectionString); }
        }

        public string AzureServiceBusNamespace
        {
            get 
            {
                return string.Format(
                    "{0}-votus-bus-{1}", 
                    EnvironmentName, 
                    AzureDataCenterLocation.Replace(" ", "-")
                ).ToLower(); 
            }
        }

        public const string AzureServiceBusSecretValueConfigName = "Votus.Azure.ServiceBus.SecretValue";
        public string AzureServiceBusSecretValue
        {
            get { return ConfigManager.Get(AzureServiceBusSecretValueConfigName); }
            set { ConfigManager.Set(AzureServiceBusSecretValueConfigName, value); }
        }

        public string AzureServiceBusConnectionString
        {
            get
            {
                return string.Format(
                    "Endpoint=sb://{0}.servicebus.windows.net/;SharedSecretIssuer=owner;SharedSecretValue={1}",
                    AzureServiceBusNamespace,
                    AzureServiceBusSecretValue
                );
            }
        }

        public string AzureCachingServiceName
        {
            get
            {
                return string.Format(
                    "{0}votus{1}",
                    EnvironmentName,
                    AzureDataCenterLocation.Replace(" ", "")
                ).ToLower();
            }
        }

        public const string AzureCachingServiceAccountKeyConfigName = "Votus.Azure.Caching.AccountKey";
        public string AzureCachingServiceAccountKey
        {
            get { return ConfigManager.Get(AzureCachingServiceAccountKeyConfigName); }
            set { ConfigManager.Set(AzureCachingServiceAccountKeyConfigName, value); }            
        }
    }
}
