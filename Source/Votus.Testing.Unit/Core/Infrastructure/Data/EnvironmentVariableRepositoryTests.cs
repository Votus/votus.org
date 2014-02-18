using System;
using Votus.Core.Infrastructure.Data;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Data
{
    public class EnvironmentVariableRepositoryTests : IReadableRepositoryTests
    {
        public EnvironmentVariableRepositoryTests()
            : base(new EnvironmentVariableRepository())
        {
        }

        protected override void SetValue(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value);
        }

        [Fact]
        public
        void
        Get_ValueSetInUserScope_ReturnsValue()
        {
            // Arrange
            const string settingName = "UserSetting";
            const string value       = "user-value";

            Environment.SetEnvironmentVariable(settingName, value, EnvironmentVariableTarget.User);

            // Act
            var actual = ReadableRepository.Get(settingName);

            // Assert
            Assert.Equal(value, actual);
        }

        [Fact]
        public
        void
        Get_ValueSetInProcessScope_ReturnsValue()
        {
            // Arrange
            const string settingName = "ProcessSetting";
            const string value       = "process-value";

            Environment.SetEnvironmentVariable(settingName, value, EnvironmentVariableTarget.Process);

            // Act
            var actual = ReadableRepository.Get(settingName);

            // Assert
            Assert.Equal(value, actual);
        }
    }
}
