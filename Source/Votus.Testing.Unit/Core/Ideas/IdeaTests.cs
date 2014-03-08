using System;
using System.Linq;
using Votus.Core.Ideas;
using Xunit;

namespace Votus.Testing.Unit.Core.Ideas
{
    public class IdeaTests
    {
        private readonly Guid   ValidIdeaId     = Guid.NewGuid();
        private readonly Guid   ValidGoalId     = Guid.NewGuid();
        private readonly Guid   ValidTaskId     = Guid.NewGuid();
        private const string    ValidTitle      = "This is a valid idea title.";

        [Fact]
        public void Constructor_PassesValidation_AddsIdeaCreatedEventAsUncommittedEvent()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var idea = new Idea(id, ValidTitle);

            // Assert
            Assert.IsType<IdeaCreatedEvent>(idea.GetUncommittedEvents().Single());
        }

        [Fact]
        public void Constructor_PassesValidation_IdeaCreatedEventIsVersion1()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var idea = new Idea(id, ValidTitle);

            // Assert
            Assert.Equal(1, idea.GetUncommittedEvents().Single().Version);
        }

        [Fact]
        public void Apply_IdeaCreatedEvent_SetsAggregateRootId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var idea = new Idea(id, ValidTitle);

            // Assert
            Assert.Equal(id, idea.Id);
        }

        [Fact]
        public void Apply_IdeaCreatedEvent_SetsTitle()
        {
            // Arrange
            const string title = "Some title";

            // Act
            var idea = new Idea(ValidIdeaId, title);

            // Assert
            Assert.Equal(title, idea.Title);
        }

        [Fact]
        public 
        void
        AddGoal_ValidGoalId_AddsGoalAddedToIdeaEvent()
        {
            // Arrange
            var idea = new Idea();

            // Act
            idea.AddGoal(ValidGoalId);

            // Assert
            Assert.IsType<GoalAddedToIdeaEvent>(idea.GetUncommittedEvents().Single());
        }

        [Fact]
        public 
        void
        AddTask_ValidTaskId_AddsTaskAddedToIdeaEvent()
        {
            // Arrange
            var idea = new Idea();

            // Act
            idea.AddTask(ValidTaskId);

            // Assert
            Assert.IsType<TaskAddedToIdeaEvent>(idea.GetUncommittedEvents().Single());
        }

        [Fact]
        public
        void
        Apply_GoalAddedToIdeaEvent_AddsGoalIdToGoals()
        {
            // Arrange
            var idea                 = new Idea();
            var goalAddedToIdeaEvent = new GoalAddedToIdeaEvent { GoalId = ValidGoalId };
            
            // Act
            idea.Apply(goalAddedToIdeaEvent);

            // Assert
            Assert.Contains(ValidGoalId, idea.Goals);
        }

        [Fact]
        public
        void
        Apply_TaskAddedToIdeaEvent_AddsTaskIdToGoals()
        {
            // Arrange
            var idea = new Idea();
            var taskAddedToIdeaEvent = new TaskAddedToIdeaEvent { TaskId = ValidTaskId };

            // Act
            idea.Apply(taskAddedToIdeaEvent);

            // Assert
            Assert.Contains(ValidTaskId, idea.Tasks);
        }
    }
}
