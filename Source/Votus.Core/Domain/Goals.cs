using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Ninject;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Domain
{
    public class Goals
    {
        [Inject]
        public IVersioningRepository<Goal> Repository { get; set; }

        public 
        System.Threading.Tasks.Task 
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

    public class CreateGoalCommand
    {
        public Guid NewGoalId   { get; set; }
        public Guid IdeaId      { get; set; }

        [Required(                                  ErrorMessage = "Please say a few words about your goal")]
        [RegularExpression(@"^.*(\w+)\s+(\w+).*$",  ErrorMessage = "Please say a few words about your goal")]
        public string NewGoalTitle { get; set; }
    }
}
