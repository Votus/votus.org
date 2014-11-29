using System;
using System.Threading.Tasks;
using System.Web.Http;
using Ninject;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Infrastructure.Queuing;

namespace Votus.Web.Areas.Api.Controllers
{
    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName + "/event-store")]
    public class EventStoreController : ApiController
    {
        [Inject] public QueueManager CommandBus { get; set; }

        [HttpPost]
        [Route("republish-events")]
        public 
        Task 
        RepublishEventsAsync()
        {
            return CommandBus.SendAsync(
                Guid.NewGuid(), 
                new RepublishAllEventsCommand()
            );
        }
    }
}