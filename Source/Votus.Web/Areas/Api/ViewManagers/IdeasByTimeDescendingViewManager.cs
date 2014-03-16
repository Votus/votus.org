using Ninject;
using System;
using System.Threading.Tasks;
using Votus.Core.Domain.Ideas;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class IdeasByTimeDescendingViewManager
    {
        [Inject] public IPartitionedRepository IdeasRepository { get; set; }

        public 
        Task
        HandleAsync(IdeaCreatedEvent ideaCreatedEvent)
        {
            var newIdea = new IdeaViewModel {
                Id    = ideaCreatedEvent.EventSourceId, 
                Title = ideaCreatedEvent.Title,
                Tag   = ideaCreatedEvent.Tag
            };

            var partitionKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");

            return IdeasRepository.InsertAsync(
                partitionKey:   partitionKey,
                rowKey:         ideaCreatedEvent.EventSourceId,
                entity:         newIdea
            );
        }
    }
}