using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Votus.Web.Areas.Api;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Gui
{
    public class WebApiHashCachingDelegatingHandlerTests
    {
        #region Fields & Constructors
        
        private readonly EntityTagHeaderValue   ValidETag;
        private const    string                 EtagWildcard = "\"*\"";
        private readonly object                 ValidContent = new object();
        private readonly HttpRequestMessage     ValidRequest = new HttpRequestMessage(HttpMethod.Get, "http://www.somesite.com");

        private readonly HttpClient                         _client;
        private readonly WebApiHashCachingDelegatingHandler _cachingHandler;

        public WebApiHashCachingDelegatingHandlerTests()
        {
            ValidETag = new EntityTagHeaderValue("\"" + ValidContent.GetHashCode() + "\"");

            _cachingHandler = new WebApiHashCachingDelegatingHandler();
            _client         = new HttpClient(_cachingHandler);
        }

        #endregion

        [Fact]
        public 
        async Task
        SendAsync_ETagIsWildcard_InnerHandlerIsCalled()
        {
            // Arrange
            ValidRequest.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(EtagWildcard));

            _cachingHandler.InnerHandler = new TestHandler(
                (req, token) => Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK))
            );
            
            // Act
            var result = await _client.SendAsync(ValidRequest);

            // Assert
            Assert.Equal(result.StatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public 
        async Task
        SendAsync_ETagIsWildcard_ResponseContainsEtag()
        {
            // Arrange
            ValidRequest.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(EtagWildcard));

            _cachingHandler.InnerHandler = new TestHandler(
                (req, token) => Task.Factory.StartNew(
                    () => new HttpResponseMessage(HttpStatusCode.OK) {
                        Content = new ObjectContent(ValidContent.GetType(), ValidContent, new JsonMediaTypeFormatter())
                    }
                )
            );
            
            // Act
            var result = await _client.SendAsync(ValidRequest);

            // Assert
            Assert.Equal(
                ValidContent.GetHashCode().ToString(), 
                result.Headers.ETag.Tag.Trim('"')
            );
        }

        [Fact]
        public
        async Task
        SendAsync_ETagIsCurrent_Returns302NotModified()
        {
            // Arrange
            ValidRequest.Headers.IfNoneMatch.Add(
                new EntityTagHeaderValue("\"" + ValidContent.GetHashCode() + "\"")
            );

            _cachingHandler.InnerHandler = new TestHandler(
                (req, token) => Task.Factory.StartNew(
                    () => new HttpResponseMessage(HttpStatusCode.OK) {
                        Content = new ObjectContent(ValidContent.GetType(), ValidContent, new JsonMediaTypeFormatter())
                    }
                )
            );

            // Act
            var result = await _client.SendAsync(ValidRequest);

            // Assert
            Assert.Equal(
                HttpStatusCode.NotModified,
                result.StatusCode
            );
        }

        [Fact]
        public void GetETag_ContentValueIsNull_Returns0()
        {
            // Arrange
            var nullContent = new ObjectContent(
                ValidContent.GetType(),
                null, 
                new JsonMediaTypeFormatter()
            );

            // Act
            var actual = WebApiHashCachingDelegatingHandler.GetETag(nullContent);

            // Assert
            Assert.Equal("\"0\"", actual.Tag);
        }

        [Fact]
        public void GetETag_ContentValueIsNotNull_ReturnsValueHashCode()
        {
            // Arrange
            var content = new ObjectContent(
                ValidContent.GetType(),
                ValidContent,
                new JsonMediaTypeFormatter()
            );

            // Act
            var actual = WebApiHashCachingDelegatingHandler.GetETag(content);

            // Assert
            Assert.Equal(ValidETag, actual);
        }

        #region Helper Classes
        
        public class TestHandler : HttpMessageHandler
        {

            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

            public TestHandler()
            {
                _handlerFunc = (httpRequestMessage, cancellationToken) => Return200();
            }

            public TestHandler(Func<HttpRequestMessage,
                                   CancellationToken, Task<HttpResponseMessage>> handlerFunc)
            {
                _handlerFunc = handlerFunc;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _handlerFunc(request, cancellationToken);
            }

            public static Task<HttpResponseMessage> Return200()
            {
                return Task.Factory.StartNew(
                    () => new HttpResponseMessage(HttpStatusCode.OK));
            }
        }

        #endregion
    }
}
