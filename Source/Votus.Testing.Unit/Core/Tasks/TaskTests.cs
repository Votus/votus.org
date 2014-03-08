using System.Linq;
using Votus.Core.Tasks;
using Xunit;

namespace Votus.Testing.Unit.Core.Tasks
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
    }
}
