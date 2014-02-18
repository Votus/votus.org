using Ninject;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Votus.Core.Infrastructure.Messaging;
using Votus.Core.Infrastructure.Queuing;

namespace Votus.Web.Areas.Api.Controllers
{
    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName)]
    public class CommandsController : ApiController
    {
        [Inject]
        public QueueManager CommandDispatcher { get; set; }

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