using Ninject;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Tasks;
using Votus.Web.Areas.Api.Models;
using Task = System.Threading.Tasks.Task;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class TasksByTaskIdViewManager
    {
        [Inject]
        public IKeyValueRepository Repository { get; set; }

        public 
        Task 
        HandleAsync(
            TaskCreatedEvent taskCreatedEvent)
        {
            return Repository.SetAsync(
                taskCreatedEvent.EventSourceId,
                new TaskViewModel {
                    Id    = taskCreatedEvent.EventSourceId, 
                    Title = taskCreatedEvent.Title
                }
            );
        }
    }
}