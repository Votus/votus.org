using System.Net;
using System.Net.Http;
using System.Web.Http;
using Votus.Core.Infrastructure.Web.WebApi;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Web.WebApi
{
    public class NullTo404ActionFilterTests
    {
        [Fact]
        public 
        void 
        TranslateResponse_ResponseContentIsNull_Throws404HttpResponseException()
        {
            // Arrange
            object responseContent = null;
            
            // Act
            var exception = Assert.Throws<HttpResponseException>(() =>
                NullTo404ActionFilter.TranslateResponse(new HttpRequestMessage(), responseContent)
            );

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, exception.Response.StatusCode);
        }

        [Fact]
        public
        void
        TranslateResponse_ResponseContentIsNotNull_ReturnsWithoutException()
        {
            // Arrange
            var responseContent = new object();

            // Act
            NullTo404ActionFilter.TranslateResponse(new HttpRequestMessage(),  responseContent);
        }
    }
}
