using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Votus.Core.Infrastructure.Net.Http;

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
            // Only convert nulls to 404 for GETs.
            if (context.Request.Method != HttpMethod.Get) return;
            
            TranslateResponse(context.Request, context.Response);
        }

        public 
        static
        void 
        TranslateResponse(
            HttpRequestMessage  request,
            HttpResponseMessage response
            )
        {
            if (response == null || response.HasContent()) return;

            var errorResponse = request.CreateErrorResponse(
                HttpStatusCode.NotFound, 
                new HttpError()
            );

            throw new HttpResponseException(errorResponse);
        }

        #endregion
    }
}
