using Ninject;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Votus.Core.Infrastructure.Collections;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;

namespace Votus.Web.Areas.Api.Controllers
{
    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName)]
    public class GoalsController : ApiController
    {
        [Inject] public QueueManager        CommandDispatcher   { get; set; }
        [Inject] public IKeyValueRepository ViewCache           { get; set; }

        [Route("ideas/{ideaId}/goals")]
        public
        Task<ConsistentHashSet<GoalViewModel>>
        GetGoalsAsync(Guid ideaId)
        {
            return ViewCache.GetAsync<ConsistentHashSet<GoalViewModel>>(
                GoalsByIdeaViewManager.GetViewKey(ideaId)
            );
        }
    }
}