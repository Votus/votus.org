using System.Linq;
using Votus.Core.Domain.Tasks;
using Xunit;

namespace Votus.Testing.Unit.Core.Domain.Tasks
{
    public class TaskTests
    {
        [Fact]
        public 
        void 
        VoteCompleted_PassesValidation_AddsTaskVotedCompleteEventAsUncommittedEvent()
        {
            // Arrange
            var task = new Task();

            // Act
            task.VoteCompleted();

            // Assert
            Assert.IsType<TaskVotedCompleteEvent>(task.GetUncommittedEvents().Single());
        }

        [Fact]
        public
        void 
        Apply_TaskVotedCompleteEvent_IncrementsCompletedVoteCount()
        {
            // Arrange
            var task = new Task{ CompletedVoteCount = 1 };

            // Act
            task.Apply(new TaskVotedCompleteEvent());

            // Assert
            Assert.Equal(2, task.CompletedVoteCount);
        }
    }
}
