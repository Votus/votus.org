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
            var task             = new Task();
            const string voterId = "voter-id";

            // Act
            task.VoteCompleted(voterId);

            // Assert
            Assert.IsType<TaskVotedCompleteEvent>(task.GetUncommittedEvents().Single());
        }

        [Fact]
        public
        void 
        Apply_TaskVotedCompleteEvent_IncrementsCompletedVoteCount()
        {
            // Arrange
            var task = new Task();

            // Act
            task.Apply(new TaskVotedCompleteEvent());

            // Assert
            Assert.Equal(1, task.CompletedVotes.Count);
        }
    }
}
