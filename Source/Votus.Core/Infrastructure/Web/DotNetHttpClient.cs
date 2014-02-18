using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Net;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.Web
{
    public class DotNetHttpClient : IHttpClient
    {
        public Uri BaseUri { get; set; }

        [Inject] public ILog        Log         { get; set; }
        [Inject] public ISerializer Serializer  { get; set; }

        public 
        HttpResponse<T>
        Get<T>(
            string relativeUrl)
        {
            var request  = CreateRequest(HttpVerbs.Get, relativeUrl);
            var response = ExecuteRequest(request);

            return new HttpResponse<T>(Serializer, response);
        }

        public
        HttpResponse
        Put(
            string relativeUrl, 
            object body)
        {
            var request  = CreateRequest(HttpVerbs.Put, relativeUrl, body);
            var response = ExecuteRequest(request);

            return response;
        }

        public 
        Task<HttpResponse>
        PutAsync(
            string relativeUrl, 
            object body)
        {
            var request = CreateRequest(
                HttpVerbs.Put, 
                relativeUrl,
                body
            );

            return ExecuteRequestAsync(request);
        }

        private 
        WebRequest 
        CreateRequest(
            HttpVerbs   httpVerb, 
            string      relativeUrl,
            object      body        = null)
        {
            var request    = WebRequest.Create(new Uri(BaseUri, relativeUrl));

            request.Method      = httpVerb.ToString();
            request.ContentType = "application/json";

            var serializedBody = string.Empty;

            if (body != null)
            {
                serializedBody = Serializer.Serialize(body);

                using (var requestStream = request.GetRequestStream())
                using (var writer        = new StreamWriter(requestStream))
                    writer.Write(serializedBody);
            }

            Log.Verbose(request.ToTraceString(serializedBody));

            return request;
        }

        private
        async Task<HttpResponse>
        ExecuteRequestAsync(
            WebRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var rawResponse = await request.GetResponseAsync();

                stopwatch.Stop();

                var response = new HttpResponse(
                    rawResponse, 
                    stopwatch.Elapsed
                );

                Log.Verbose(response);

                return response;
            }
            catch (WebException webException)
            {
                stopwatch.Stop();
                
                // TODO: Load the error response asynchronously
                var response = new HttpResponse(webException, stopwatch.Elapsed);

                Log.Error(response);

                throw new RequestFailedException(response);
            }
        }

        private
        HttpResponse
        ExecuteRequest(
            WebRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var rawResponse = request.GetResponse();
                
                stopwatch.Stop();

                var response = new HttpResponse(rawResponse, stopwatch.Elapsed);

                Log.Info(response);

                return response;
            }
            catch (WebException webException)
            {
                stopwatch.Stop();
                
                var response = new HttpResponse(webException, stopwatch.Elapsed);

                Log.Error(response);

                throw new RequestFailedException(response);
            }
        }
    }
}
