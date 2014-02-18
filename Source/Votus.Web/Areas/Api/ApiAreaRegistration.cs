using System.Web.Http;
using System.Web.Mvc;
using Votus.Core.Infrastructure.Web.WebApi;

namespace Votus.Web.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public const string AreaRegistrationName = "Api";

        public override string AreaName
        {
            get
            {
                return AreaRegistrationName;
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            GlobalConfiguration.Configuration.Filters.Add(new ValidationExceptionFilter());
            GlobalConfiguration.Configuration.Filters.Add(new ValidateModelStateAttribute());

            GlobalConfiguration.Configuration.MessageHandlers.Add(new WebApiHashCachingDelegatingHandler());
        }
    }
}
