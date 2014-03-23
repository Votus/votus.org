using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Votus.Core.Infrastructure.Web.WebApi // TODO: Use similar namespace as ActionFilterAttribute
{
    public class NullTo404ActionFilter : ActionFilterAttribute
    {
        #region Overrides

        public 
        override 
        void 
        OnActionExecuted(
            HttpActionExecutedContext   actionExecutedContext)
        {
            TranslateHttpActionExecutedContext(actionExecutedContext);
            base.OnActionExecuted(actionExecutedContext);
        }

        public 
        override 
        Task 
        OnActionExecutedAsync(
            HttpActionExecutedContext   actionExecutedContext, 
            CancellationToken           cancellationToken)
        {
            TranslateHttpActionExecutedContext(actionExecutedContext);
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        #endregion

        #region Methods

        private 
        static 
        void 
        TranslateHttpActionExecutedContext(
            HttpActionExecutedContext   context)
        {
            if (context.Request.Method != HttpMethod.Get) return;

            var response = context.Response;
            if (response == null) return;

            object responseValue;
            response.TryGetContentValue(out responseValue);

            TranslateResponse(
                context.Response.RequestMessage, 
                responseValue
            );
        }

        public 
        static
        void 
        TranslateResponse(
            HttpRequestMessage          request,
            object                      actionReturnValue
            )
        {
            if (actionReturnValue != null) return;
            
            var response = request.CreateErrorResponse(
                HttpStatusCode.NotFound, 
                new HttpError()
            );

            throw new HttpResponseException(response);
        }

        #endregion
    }
}
