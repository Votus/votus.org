using Ninject;
using System;
using System.Threading.Tasks;
using Votus.Core.Domain.Ideas;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Controllers;
using Votus.Web.Areas.Api.Models;
using WebApi.OutputCache.Core.Cache;
using WebApi.OutputCache.V2;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class IdeasByTimeDescendingViewManager
    {
        [Inject] public IPartitionedRepository      IdeasRepository { get; set; }
        [Inject] public IApiOutputCache             OutputCache     { get; set; }
        [Inject] public CacheOutputConfiguration    CacheConfig     { get; set; }

        public 
        async Task
        HandleAsync(IdeaCreatedEvent ideaCreatedEvent)
        {
            var newIdea = new IdeaViewModel {
                Id    = ideaCreatedEvent.EventSourceId, 
                Title = ideaCreatedEvent.Title,
                Tag   = ideaCreatedEvent.Tag
            };

            // TODO: Reconsider generating this key here...
            //       If the subsequent calls fail, like to the OutputCache, 
            //       then the message is retried resulting in the same idea appearing multiple times in the view.
            //       Maybe capture the key when the command is created instead...
            var partitionKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");

            await IdeasRepository.InsertAsync(
                partitionKey:   partitionKey,
                rowKey:         ideaCreatedEvent.EventSourceId,
                entity:         newIdea
            );

            // TODO: Abstract away the implementation details of this cache key stuff...
            var cacheKey = CacheConfig.MakeBaseCachekey((IdeasController c) => c.GetIdeasAsync(null, null, 0));
            OutputCache.RemoveStartsWith(cacheKey);
        }
    }
}