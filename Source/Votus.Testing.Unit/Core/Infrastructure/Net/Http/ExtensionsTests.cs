using System.Net.Http;
using System.Net.Http.Formatting;
using Votus.Core.Infrastructure.Net.Http;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Net.Http
{
    public class ExtensionsTests
    {
        [Fact]
        public 
        void 
        HasContent_ResponseMessageHasObjectContent_ReturnsTrue()
        {
            // Arrange
            var response = new HttpResponseMessage {
                Content = new ObjectContent(
                    typeof(object),
                    new object(),
                    new JsonMediaTypeFormatter()
                )
            };

            // Act
            var result = response.HasContent();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public
        void
        HasContent_ResponseMessageHasNullObjectContent_ReturnsFalse()
        {
            // Arrange
            var response = new HttpResponseMessage {
                Content = new ObjectContent(
                    typeof(object),
                    null,
                    new JsonMediaTypeFormatter()
                )
            };

            // Act
            var result = response.HasContent();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public
        void
        HasContent_ResponseMessageHasByteArrayContent_ReturnsTrue()
        {
            // Arrange
            var response = new HttpResponseMessage {
                Content = new ByteArrayContent(
                    new byte[]{1}
                )
            };

            // Act
            var result = response.HasContent();

            // Assert
            Assert.True(result);
        }
    }
}
