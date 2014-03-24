using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
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
            var request  = new HttpRequestMessage();
            var response = new HttpResponseMessage();
            
            // Act
            var exception = Assert.Throws<HttpResponseException>(() =>
                NullTo404ActionFilter.TranslateResponse(request, response)
            );

            // Assert
            Assert.Equal(
                HttpStatusCode.NotFound, 
                exception.Response.StatusCode
            );
        }

        [Fact]
        public
        void
        TranslateResponse_ResponseContentIsNotNull_ReturnsWithoutException()
        {
            // Arrange
            var request  = new HttpRequestMessage();
            var response = new HttpResponseMessage {
                Content = new ObjectContent(typeof(object), new object(), new JsonMediaTypeFormatter())
            };

            // Act / Assert
            NullTo404ActionFilter.TranslateResponse(
                request,  
                response
            );
        }
    }
}
