using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Infrastructure.Configuration;
using Votus.Core.Infrastructure.Data;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Configuration
{
    public class ConfigManagerTests
    {
        const string ValidSettingValue  = "setting-value";
        const string ValidSettingName   = "some-setting";

        private readonly IReadableRepository    _fakeReadableProvider;
        private readonly ConfigManager          _configManager;

        public ConfigManagerTests()
        {
            _fakeReadableProvider = A.Fake<IReadableRepository>();

            _configManager = new ConfigManager {
                Providers = new List<IReadableRepository> { _fakeReadableProvider }
            };

            A.CallTo(() =>
                _fakeReadableProvider.Get(A<string>.Ignored)
            ).Returns(ValidSettingValue);
        }

        [Fact]
        public void Get_SettingExists_ReturnsSettingValue()
        {
            // Act
            var actual = _configManager.Get(ValidSettingName);

            // Assert
            Assert.Equal(ValidSettingValue, actual);
        }

        [Fact]
        public void Get_SettingExistsInSecondProvider_ReturnsSettingValue()
        {
            // Arrange
            const string expectedValue = "new-setting-value";

            var fakeProvider2 = A.Fake<IReadableRepository>();

            _configManager.Providers.Add(fakeProvider2);

            A.CallTo(() => _fakeReadableProvider.Get(A<string>.Ignored)).Returns(null);
            A.CallTo(() => fakeProvider2.Get(A<string>.Ignored)).Returns(expectedValue);

            // Act
            var actualValue = _configManager.Get("new-setting");

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Get_SettingDoesntExist_ThrowsSettingNotFoundException()
        {
            // Arrange
            const string notFoundSettingName = "non-existent-setting";

            A.CallTo(() => 
                _fakeReadableProvider.Get(A<string>.Ignored))
            .Returns(null);

            // Act
            var act = new Assert.ThrowsDelegate(() => 
                _configManager.Get(notFoundSettingName)
            );

            // Assert
            Assert.Throws<SettingNotFoundException>(act);
        }

        [Fact]
        public void Get_SettingDoesntExist_ExceptionProvidesMissingSettingName()
        {
            // Arrange
            const string notFoundSettingName = "non-existent-setting";

            var message = string.Empty;

            A.CallTo(() =>
                _fakeReadableProvider.Get(A<string>.Ignored))
            .Returns(null);

            // Act
            try
            {
                _configManager.Get(notFoundSettingName);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            // Assert
            Assert.True(message.Contains(notFoundSettingName));
        }

        [Fact]
        public void Get_SettingDoesntExistAndDefaultIsSpecified_ReturnsDefaultValue()
        {
            // Arrange
            const string defaultValue        = "default-value";
            const string notFoundSettingName = "non-existent-setting";

            A.CallTo(() =>
                _fakeReadableProvider.Get(A<string>.Ignored))
            .Returns(null);

            // Act
            var actualValue = _configManager.Get(notFoundSettingName, defaultValue);

            // Assert
            Assert.Equal(defaultValue, actualValue);
        }

        [Fact]
        public void Set_WritableProviderExists_ValueIsWritten()
        {
            // Arrange
            var fakeWritableProvider = A.Fake<WritableProvider>();
            _configManager.Providers.Add(fakeWritableProvider);

            // Act
            _configManager.Set(ValidSettingName, ValidSettingValue);

            // Assert
            A.CallTo(() => 
                fakeWritableProvider.Set(ValidSettingName, ValidSettingValue)
            ).MustHaveHappened();
        }

        [Fact]
        public void Set_WritableProviderDoesntExist_ThrowsSaveSettingException()
        {
            Assert.Throws<SaveSettingException>(() => 
                _configManager.Set(ValidSettingName, ValidSettingValue)
            );
        }

        #region Helper Classes

        public class WritableProvider : IReadWriteRepository
        {
            public WritableProvider() { }

            public virtual string Get(string settingName)
            {
                throw new NotImplementedException();
            }
            public virtual void Set(string settingName, string value)
            {
                throw new NotImplementedException();
            }

            public Task InsertBatchAsync<TEntity, TKey>(Guid groupId, IEnumerable<TEntity> entities, Func<TEntity, TKey> keyLocator)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
