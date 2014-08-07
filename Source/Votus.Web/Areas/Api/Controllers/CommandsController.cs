using Ninject;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Votus.Core.Infrastructure.Messaging;
using Votus.Core.Infrastructure.Queuing;
using WebApi.OutputCache.V2;

namespace Votus.Web.Areas.Api.Controllers
{
    // TODO: Refactor this class away, have each controller to send commands instead.

    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName)]
    public class CommandsController : ApiController
    {
        [Inject]
        public QueueManager CommandDispatcher { get; set; }

        [CacheOutput(NoCache = true)]
        [HttpPut, Route("commands/{commandId}")]
        public 
        Task 
        SendCommandAsync(
            Guid            commandId,
            CommandEnvelope commandEnvelope)
        {
            // Send the command
            return CommandDispatcher.SendAsync(
                commandId, 
                commandEnvelope
            );
        }
    }
}