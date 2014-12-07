using System;
using System.Threading.Tasks;
using Ninject;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Task = System.Threading.Tasks.Task;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class IdeaByIdViewManager
    {
        private const string IdeaCachedViewKeyPattern = "ideas/{0}.json";

        [Inject] public IKeyValueRepository ViewCache { get; set; }

        public 
        Task 
        HandleAsync(
            IdeaCreatedEvent ideaCreatedEvent)
        {
            return ViewCache.SetAsync(
                GetViewKey(ideaCreatedEvent.EventSourceId),
                new IdeaViewModel {
                    Id    = ideaCreatedEvent.EventSourceId,
                    Tag   = ideaCreatedEvent.Tag,
                    Title = ideaCreatedEvent.Title
                }
            );
        }

        public
        static 
        string 
        GetViewKey(
            Guid ideaId)
        {
            return string.Format(
                IdeaCachedViewKeyPattern, 
                ideaId
            );
        }
    }
}