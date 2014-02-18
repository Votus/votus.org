using System.Threading.Tasks;
using Ninject;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Goals
{
    public class GoalsManager
    {
        [Inject]
        public IVersioningRepository<Goal> Repository { get; set; }

        public 
        Task 
        HandleAsync(
            CreateGoalCommand command)
        {
            var goal = new Goal(
                command.NewGoalId,
                command.IdeaId,
                command.NewGoalTitle
            );

            return Repository.SaveAsync(goal);
        }
    }
}
