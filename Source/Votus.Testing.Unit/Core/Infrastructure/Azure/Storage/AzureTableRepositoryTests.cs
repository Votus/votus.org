using Votus.Core.Infrastructure.Azure.Storage;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Azure.Storage
{
    public class AzureTableRepositoryTests
    {
        [Fact]
        public
        void
        AssembleNextPageControllerToken_SkipIs0_SkipIsOmittedFromToken()
        {
            // Arrange
            const int skip         = 0;
            const string repoToken = "token";
            const char delimiter   = ':';

            // Act
            var actual = AzureTableRepository.AssembleNextPageControllerToken(
                repoToken,
                skip,
                delimiter
            );

            // Assert
            Assert.Equal(repoToken, actual);
        }
    }
}
