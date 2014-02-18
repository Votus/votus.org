using System.Reflection;
using Votus.Web;
using Xunit;

namespace Votus.Testing.Unit
{
    public class BuildTests
    {
        [Fact]
        public void VotusWeb_VersionNumber_MatchesTestProjectVersion()
        {
            // Arrange
            var testAssembly     = Assembly.GetExecutingAssembly();
            var votusWebAssembly = Assembly.GetAssembly(typeof(Global));

            // Assert
            Assert.Equal(
                testAssembly.GetName().Version, 
                votusWebAssembly.GetName().Version
            );
        }
    }
}
