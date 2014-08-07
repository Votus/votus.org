using Ninject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Task = System.Threading.Tasks.Task;

namespace Votus.Web.Areas.Api.Controllers
{
    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName)]
    public class TasksController : ApiController
    {
        [Inject] public IKeyValueRepository ViewCache           { get; set; }
        [Inject] public QueueManager        CommandDispatcher   { get; set; }

        [Route("ideas/{ideaId}/tasks")]
        public
        Task<HashSet<TaskViewModel>>
        GetTasksByIdeaIdAsync(
            Guid ideaId)
        {  
            return ViewCache.GetAsync<HashSet<TaskViewModel>>(
                TasksByIdeaViewManager.GetViewKey(ideaId)
            );
        }

        [Route("tasks/{taskId}")]
        public
        Task<TaskViewModel> 
        GetTaskById(
            Guid taskId)
        {
            return ViewCache.GetAsync<TaskViewModel>(
                TaskByIdViewManager.GetViewKey(taskId)
            );
        }

        [HttpPost, Route("tasks/{taskId}/completed-votes")]
        public
        Task
        VoteTaskCompleted(
            Guid taskId)
        {
            // TODO: Return a Location header where the client can check the status of the command.

            return CommandDispatcher.SendAsync(
                commandId:  Guid.NewGuid(),
                command:    new VoteTaskCompletedCommand {
                                TaskId  = taskId,
                                VoterId = "255.255.255.255" // TODO: Make dynamic
                            }
            );
        }
    }
}