using System;
using Microsoft.WindowsAzure.Storage;
using Ninject;
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
        HandleAsync(
            IdeaCreatedEvent ideaCreatedEvent)
        {
            var newIdea = new IdeaViewModel {
                Id    = ideaCreatedEvent.EventSourceId, 
                Title = ideaCreatedEvent.Title,
                Tag   = ideaCreatedEvent.Tag
            };

            var partitionKey = (DateTimeOffset.MaxValue.Ticks - ideaCreatedEvent.Timestamp.Ticks).ToString("d19");

            try
            {
                await IdeasRepository.InsertAsync(
                    partitionKey:   partitionKey,
                    rowKey:         ideaCreatedEvent.EventSourceId,
                    entity:         newIdea
                );

            }
            catch (StorageException storageException) // TODO: Should not have tech specific exceptions here.
            {
                if (storageException.RequestInformation.ExtendedErrorInformation.ErrorCode != "EntityAlreadyExists")
                    throw;
            }

            // TODO: Abstract away the implementation details of this cache key stuff...
            var cacheKey = CacheConfig.MakeBaseCachekey((IdeasController c) => c.GetIdeasAsync(null, null, 0));
            OutputCache.RemoveStartsWith(cacheKey);
        }
    }
}