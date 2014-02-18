using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Votus.Core.Infrastructure.Web.WebApi
{
    public class ValidationExceptionFilter : ExceptionFilterAttribute
    {
        public 
        override 
        void 
        OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is ValidationException == false) return;

            var response = actionExecutedContext.Request.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                actionExecutedContext.Exception.Message
            );

            // Sends the response directly to the user bypassing any subsequent ExceptionFilters
            // from getting this exception, effectively "handling" the exception.
            // HttpResponseException is a special exception that doesn't get caught by any exception handlers / filters.
            throw new HttpResponseException(response);
        }
    }
}
