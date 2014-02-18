﻿using Ninject;
using System;
using System.Collections.Generic;
using Votus.Core.Ideas;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Tasks;
using Votus.Web.Areas.Api.Models;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class IdeaTasksViewManager
    {
        public const string IdeaCachedViewKeyPattern = "ideas/{0}/tasks.json";

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

            // Get task...
            var task = await TaskRepository.GetAsync<Task>(
                taskAddedToIdeaEvent.TaskId
            );

            // Add the task to the view data.
            cachedView.Add(
                new TaskViewModel {
                    Id    = task.Id,
                    Title = task.Title
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