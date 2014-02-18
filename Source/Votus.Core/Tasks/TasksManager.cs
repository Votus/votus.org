using Ninject;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Tasks
{
    public class TasksManager
    {
        [Inject]
        public IVersioningRepository<Task> TaskRepository { get; set; }

        public
        System.Threading.Tasks.Task 
        HandleAsync(
            CreateTaskCommand createTaskCommand)
        {
            var newTask = new Task(
                id:             createTaskCommand.NewTaskId,
                initialIdeaId:  createTaskCommand.IdeaId,
                title:          createTaskCommand.NewTaskTitle
            );

            return TaskRepository.SaveAsync(newTask);
        }
    }
}
