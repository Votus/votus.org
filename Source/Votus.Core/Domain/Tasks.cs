using System;
using System.ComponentModel.DataAnnotations;
using Ninject;
using Votus.Core.Infrastructure.Caching;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Domain
{
    public class Tasks
    {
        [Inject] public IVersioningRepository<Task>             TaskRepository  { get; set; }
        [Inject] public ICache                                  Cache           { get; set; } // TODO: Use a real repository.

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

        public
        async System.Threading.Tasks.Task
        HandleAsync(
            VoteTaskCompletedCommand voteTaskCompletedCommand)
        {
            var task = await TaskRepository.GetAsync<Task>(
                voteTaskCompletedCommand.TaskId
            );

            var originalVersion = task.Version;

            task.VoteCompleted(voteTaskCompletedCommand.VoterId);

            await TaskRepository.SaveAsync(task, originalVersion);

            // TODO: Unify key format with OncePerAttribute.

            var key = string.Format(
                "{0}:{1}", 
                typeof(VoteTaskCompletedCommand), 
                voteTaskCompletedCommand.GetHashCode()
            );

            Cache.Set(key, voteTaskCompletedCommand);
        }
    }

    public class CreateTaskCommand
    {
        public Guid     NewTaskId       { get; set; }
        public Guid     IdeaId          { get; set; }

        [Required(                                  ErrorMessage = "Please say a few words about your task")]
        [RegularExpression(@"^.*(\w+)\s+(\w+).*$",  ErrorMessage = "Please say a few words about your task")]
        public string   NewTaskTitle    { get; set; }
    }
}
