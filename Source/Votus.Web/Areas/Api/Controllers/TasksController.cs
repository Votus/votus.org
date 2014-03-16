using Ninject;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Votus.Core.Infrastructure.Collections;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;

namespace Votus.Web.Areas.Api.Controllers
{
    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName)]
    public class TasksController : ApiController
    {
        [Inject] public IKeyValueRepository ViewCache { get; set; }

        [Route("ideas/{ideaId}/tasks")]
        public
        Task<ConsistentHashSet<TaskViewModel>>
        GetTasksByIdeaIdAsync(Guid ideaId)
        {
            return ViewCache.GetAsync<ConsistentHashSet<TaskViewModel>>(
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
    }
}