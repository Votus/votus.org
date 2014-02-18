using FakeItEasy;
using Votus.Core;
using Votus.Core.Infrastructure.Configuration;
using Xunit;

namespace Votus.Testing.Unit.Core
{
    public class ApplicationSettingsTests
    {
        #region Test Values & Setup

        const string ValidEnvironmentName         = "Local";
        const string ValidAzureDataCenterLocation = "West US";
        const string ValidWebsiteBaseUrl          = "http://local-votus-web-west-us.azurewebsites.net/";

        private readonly ConfigManager          _fakeConfigManager;
        private readonly ApplicationSettings    _applicationSettings;

        public ApplicationSettingsTests()
        {
            _fakeConfigManager   = A.Fake<ConfigManager>();
            _applicationSettings = new ApplicationSettings {
                ConfigManager = _fakeConfigManager
            };

            A.CallTo(() => _fakeConfigManager.Get("Votus.Environment.Name", A<string>.Ignored)).Returns(ValidEnvironmentName);
            A.CallTo(() => _fakeConfigManager.Get("Votus.Azure.DataCenter.Location")).Returns(ValidAzureDataCenterLocation);
        }

        #endregion

        [Fact]
        public void WebsiteBaseUri_StringWebsiteBaseUriIsValid_ReturnsStringUriAsUriType()
        {
            // Arrange
            A.CallTo(() => _fakeConfigManager.Get(ApplicationSettings.VotusWebsiteBaseUrlConfigName, A<string>.Ignored)).Returns(ValidWebsiteBaseUrl);

            // Act
            var actualValue = _applicationSettings.WebsiteBaseUri.ToString();

            // Assert
            Assert.Equal(ValidWebsiteBaseUrl, actualValue);
        }

        [Fact]
        public void AzureWebsiteServiceName_PartsAreMixedCase_ReturnsFormattedName()
        {
            // Arrange
            A.CallTo(() => _fakeConfigManager.Get(ApplicationSettings.EnvironmentNameConfigName)).Returns("ENV");
            A.CallTo(() => _fakeConfigManager.Get(ApplicationSettings.AzureDataCenterLocationConfigName)).Returns("LOC");

            // Act
            var actualValue = _applicationSettings.AzureWebsiteServiceName;

            // Assert
            Assert.Equal("env-votus-web-loc", actualValue);
        }
    }
}
