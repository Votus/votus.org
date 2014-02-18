using Ninject;
using System.Web.Http.Filters;
using Votus.Core.Infrastructure.Logging;

namespace Votus.Core.Infrastructure.Web.WebApi
{
    public class UnhandledExceptionLoggerFilter : ExceptionFilterAttribute 
    {
        [Inject]
        public ILog Log { get; set; }

        public 
        override
        void
        OnException(
            HttpActionExecutedContext actionExecutedContext)
        {
            Log.Error(actionExecutedContext.Exception);
        }
    }
}
