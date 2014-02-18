using System;
using System.Linq;
using Votus.Core.Goals;
using Xunit;

namespace Votus.Testing.Unit.Core.Goals
{
    public class GoalTests
    {
        private readonly    Guid    ValidId             = Guid.NewGuid();
        private readonly    Guid    ValidInitialIdeaId  = Guid.NewGuid();
        private const       string  ValidTitle          = "This is a valid title";

        [Fact]
        public 
        void 
        Constructor_PassesValidation_AddsIdeaCreatedEventAsUncommittedEvent()
        {
            // Act
            var goal = new Goal(
                ValidId, 
                ValidInitialIdeaId,
                ValidTitle
            );

            // Assert
            Assert.IsType<GoalCreatedEvent>(goal.GetUncommittedEvents().Single());
        }

        [Fact]
        public void Constructor_PassesValidation_GoalCreatedEventIsVersion1()
        {
            // Act
            var goal = new Goal(
                ValidId, 
                ValidInitialIdeaId, 
                ValidTitle
            );

            // Assert
            Assert.Equal(1, goal.GetUncommittedEvents().Single().Version);
        }

        [Fact]
        public void Apply_GoalCreatedEvent_SetsAggregateRootId()
        {
            // Act
            var goal = new Goal(
                ValidId,
                ValidInitialIdeaId,
                ValidTitle
            );

            // Assert
            Assert.Equal(ValidId, goal.Id);
        }

        [Fact]
        public void Apply_GoalCreatedEvent_SetsTitle()
        {
            // Act
            var goal = new Goal(
                ValidId,
                ValidInitialIdeaId,
                ValidTitle
            );

            // Assert
            Assert.Equal(ValidTitle, goal.Title);
        }

        [Fact]
        public void Apply_GoalCreatedEvent_SetsInitialIdeaId()
        {
            // Act
            var goal = new Goal(
                ValidId,
                ValidInitialIdeaId,
                ValidTitle
            );

            // Assert
            Assert.Equal(ValidInitialIdeaId, goal.InitialIdeaId);
        }
    }
}
