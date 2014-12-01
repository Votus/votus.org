using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using Votus.Core.Domain.Goals;
using Votus.Core.Domain.Ideas;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class GoalsByIdeaViewManager
    {
        public const string IdeaCachedViewKeyPattern = "ideas/{0}/goals.json";

        [Inject] public IKeyValueRepository         ViewRepository { get; set; }
        [Inject] public IVersioningRepository<Goal> GoalRepository { get; set; }

        public
        async Task
        HandleAsync(
            GoalAddedToIdeaEvent goalAddedToIdeaEvent)
        {
            var cacheKey = GetViewKey(goalAddedToIdeaEvent.EventSourceId);

            // Get existing cached view data, if any.
            var cachedView = await ViewRepository.GetAsync<List<GoalViewModel>>(cacheKey) ??
                new List<GoalViewModel>();

            // No need to add it if it already exists in the list...
            if (cachedView.Any(goalViewModel => goalViewModel.Id == goalAddedToIdeaEvent.GoalId)) return;

            // Get goal...
            // TODO: Maybe get it from the GoalAddedToIdeaEvent?
            var goal = await GoalRepository.GetAsync<Goal>(
                goalAddedToIdeaEvent.GoalId
            );

            // Add the goal to the view data.
            cachedView.Add(
                new GoalViewModel {
                    Id    = goal.Id,
                    Title = goal.Title
                }
            );

            // Update the cache.
            await ViewRepository.SetAsync(
                cacheKey,
                cachedView
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