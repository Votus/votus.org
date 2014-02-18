using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.Web
{
    public interface IHttpClient
    {
        HttpResponse<T> Get<T>(string relativeUrl);

        HttpResponse        Put     (string relativeUrl, object body);
        Task<HttpResponse>  PutAsync(string relativeUrl, object body);
    }
}