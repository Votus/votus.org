using System;
using Ninject;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Tasks;
using Votus.Web.Areas.Api.Models;
using Task = System.Threading.Tasks.Task;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class TasksByTaskIdViewManager
    {
        public const string TaskCachedViewKeyPattern = "tasks/{0}.json";

        [Inject] public IKeyValueRepository ViewCache { get; set; }

        public 
        Task 
        HandleAsync(
            TaskCreatedEvent taskCreatedEvent)
        {
            return ViewCache.SetAsync(
                GetViewKey(taskCreatedEvent.EventSourceId),
                new TaskViewModel {
                    Id    = taskCreatedEvent.EventSourceId, 
                    Title = taskCreatedEvent.Title
                }
            );
        }

        public 
        static 
        string 
        GetViewKey(
            Guid taskId)
        {
            return string.Format(
                TaskCachedViewKeyPattern, 
                taskId
            );
        }
    }
}