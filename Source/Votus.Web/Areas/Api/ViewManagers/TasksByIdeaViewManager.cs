using System.Linq;
using Ninject;
using System;
using System.Collections.Generic;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Controllers;
using Votus.Web.Areas.Api.Models;
using WebApi.OutputCache.Core.Cache;
using WebApi.OutputCache.V2;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class TasksByIdeaViewManager
    {
        public const string IdeaCachedViewKeyPattern = "ideas/{0}/tasks.json";

        [Inject] public IApiOutputCache             OutputCache    { get; set; }
        [Inject] public CacheOutputConfiguration    CacheConfig    { get; set; }
        [Inject] public IKeyValueRepository         ViewRepository { get; set; }
        [Inject] public IVersioningRepository<Task> TaskRepository { get; set; }

        public
        async System.Threading.Tasks.Task
        HandleAsync(
            TaskAddedToIdeaEvent taskAddedToIdeaEvent)
        {
            var cacheKey = GetViewKey(taskAddedToIdeaEvent.EventSourceId);

            // Get existing cached view data, if any.
            var cachedView = await ViewRepository.GetAsync<List<TaskViewModel>>(cacheKey) ??
                new List<TaskViewModel>();

            // No need to add it if it already exists in the list...
            if (cachedView.Any(taskViewModel => taskViewModel.Id == taskAddedToIdeaEvent.TaskId)) return;

            // Get task...
            var task = await TaskRepository.GetAsync<Task>(
                taskAddedToIdeaEvent.TaskId
            );

            // Add the task to the view data.
            cachedView.Add(
                new TaskViewModel {
                    Id                 = task.Id,
                    Title              = task.Title,
                    CompletedVoteCount = task.CompletedVotes.Count
                }
            );

            // Update the cache.
            await ViewRepository.SetAsync(
                cacheKey,
                cachedView
            );
        }

        public 
        async System.Threading.Tasks.Task 
        HandleAsync(
            TaskVotedCompleteEvent taskVotedCompleteEvent)
        {
            var cacheKey = GetViewKey(taskVotedCompleteEvent.IdeaId);
            var tasks    = await ViewRepository.GetAsync<IEnumerable<TaskViewModel>>(cacheKey);

            var task = tasks.Single(t => t.Id == taskVotedCompleteEvent.EventSourceId);

            task.CompletedVoteCount++;

            await ViewRepository.SetAsync(
                cacheKey, 
                tasks
            );

            // TODO: Abstract away the implementation details of this cache key stuff...
            cacheKey = CacheConfig.MakeBaseCachekey((TasksController c) => c.GetTasksByIdeaIdAsync(Guid.Empty));
            OutputCache.RemoveStartsWith(cacheKey);
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