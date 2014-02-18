using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Votus.Web.Areas.Api
{
    /// <summary>
    /// Applied from http://codepaste.net/4w6c6i - by Glenn Block
    /// </summary>
    public class WebApiHashCachingDelegatingHandler : DelegatingHandler
    {
        private static readonly EntityTagHeaderValue DefaultETag = new EntityTagHeaderValue("\"0\"");

        protected 
        override
        Task<HttpResponseMessage> 
        SendAsync(
            HttpRequestMessage  request, 
            CancellationToken   cancellationToken)
        {
            var oldETags = request.Headers.IfNoneMatch;

            return base.SendAsync(request, cancellationToken).ContinueWith(task => {
                var httpResponse      = task.Result;
                var resultContentETag = GetETag(httpResponse.Content);

                if (oldETags.Any(etag => etag.Equals(resultContentETag)))
                    return new HttpResponseMessage(HttpStatusCode.NotModified);

                httpResponse.Headers.ETag = resultContentETag;

                return httpResponse;
            });
        }

        public static EntityTagHeaderValue GetETag(HttpContent content)
        {
            var objectContent = (ObjectContent) content;

            return objectContent == null || objectContent.Value == null ?
                DefaultETag : new EntityTagHeaderValue("\"" + objectContent.Value.GetHashCode() + "\"");
        }
    }
}