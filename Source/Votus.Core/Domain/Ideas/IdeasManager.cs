using Ninject;
using Votus.Core.Domain.Goals;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.Data;
using Task = System.Threading.Tasks.Task;

namespace Votus.Core.Domain.Ideas
{
    /// <summary>
    /// Accepts commands to make changes to the state of idea data.
    /// </summary>
    public class IdeasManager
    {
        [Inject]
        public IVersioningRepository<Idea> Repository { get; set; }

        public 
        async Task 
        HandleAsync(
            CreateIdeaCommand createIdeaCommand)
        {
            var idea = new Idea(
                createIdeaCommand.NewIdeaId, 
                createIdeaCommand.NewIdeaTitle,
                createIdeaCommand.NewIdeaTag
            );

            await Repository.SaveAsync(idea);
        }

        public 
        async Task 
        HandleAsync(
            GoalCreatedEvent goalCreatedEvent)
        {
            // Get the existing idea.
            var idea = await Repository.GetAsync<Idea>(goalCreatedEvent.InitialIdeaId);

            // TODO: Manage the original version from within the AR base class.
            var originalVersion = idea.Version;

            // Add the goal to it.
            idea.AddGoal(goalCreatedEvent.EventSourceId);

            // Save the idea.
            await Repository.SaveAsync(idea, originalVersion);
        }

        public 
        async Task 
        HandleAsync(
            TaskCreatedEvent taskCreatedEvent)
        {
            // Get the existing idea.
            var idea = await Repository.GetAsync<Idea>(taskCreatedEvent.InitialIdeaId);

            // TODO: Manage the original version from within the AR base class.
            var originalVersion = idea.Version;

            // Add the task to it.
            idea.AddTask(taskCreatedEvent.EventSourceId);

            // Save the idea.
            await Repository.SaveAsync(idea, originalVersion);            
        }
    }
}
