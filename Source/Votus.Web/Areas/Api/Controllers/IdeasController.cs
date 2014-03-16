using System;
using Ninject;
using System.Threading.Tasks;
using System.Web.Http;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;

namespace Votus.Web.Areas.Api.Controllers
{
    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName)]
    public class IdeasController : ApiController
    {
        [Inject] public QueueManager            CommandDispatcher           { get; set; }
        [Inject] public IKeyValueRepository     ViewCache                   { get; set; }
        [Inject] public IPartitionedRepository  IdeasByTimeDescendingCache  { get; set; }

        [Route("ideas")]
        public
        Task<PagedResult<IdeaViewModel>>
        GetIdeasAsync(
            string  nextPageToken = null,
            string  excludeTag    = null,
            int     itemsPerPage  = 10)
        {
            // TODO: Translate null return values to HTTP 404 responses...

            return IdeasByTimeDescendingCache
                .GetWherePagedAsync<IdeaViewModel>(
                    wherePredicate: idea => idea.Tag != excludeTag,
                    nextPageToken:  nextPageToken,
                    maxPerPage:     itemsPerPage
                );
        }

        [Route("ideas/{ideaId}")]
        public 
        Task<IdeaViewModel>
        GetIdeaAsync(
            Guid ideaId)
        {
            return ViewCache.GetAsync<IdeaViewModel>(
                IdeaByIdViewManager.GetViewKey(ideaId)
            );
        }
    }
}