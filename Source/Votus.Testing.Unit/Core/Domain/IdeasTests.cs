using System;
using System.Linq;
using FakeItEasy;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Data;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Unit.Core.Domain
{
    public class IdeasTests
    {
        private readonly Guid ValidGoalId = Guid.NewGuid();
        private readonly Guid ValidTaskId = Guid.NewGuid();

        private readonly Ideas                _ideasManager;
        private readonly IVersioningRepository<Idea> _fakeRepository;

        public IdeasTests()
        {
            _fakeRepository = A.Fake<IVersioningRepository<Idea>>();
            _ideasManager   = new Ideas {
                Repository = _fakeRepository
            };

            A.CallTo(() =>
                _fakeRepository.SaveAsync(A<Idea>.Ignored, A<int>.Ignored)
            ).ReturnsCompletedTask();
        }

        [Fact]
        public async Task HandleAsync_CreateIdeaCommand_IdeaIsPersisted()
        {
            // Arrange
            var command = new CreateIdeaCommand();

            // Act
            await _ideasManager.HandleAsync(command);

            // Assert
            A.CallTo(() => 
                _fakeRepository.SaveAsync(A<Idea>.That.Not.IsNull(), A<int>.Ignored)
            ).MustHaveHappened();
        }

        [Fact]
        public
        async Task
        HandleAsync_GoalCreatedEvent_IdeaSavedWithGoal()
        {
            // Arrange
            var existingIdea     = new Idea();
            var goalCreatedEvent = new GoalCreatedEvent { EventSourceId = ValidGoalId };

            A.CallTo(() =>
                _fakeRepository.GetAsync<Idea>(A<Guid>.Ignored)
            ).ReturnsCompletedTask(existingIdea);

            // Act
            await _ideasManager.HandleAsync(goalCreatedEvent);

            // Assert
            A.CallTo(() =>
                _fakeRepository.SaveAsync(A<Idea>.That.Matches(idea => idea.Goals.Contains(ValidGoalId)), A<int>.Ignored)
            ).MustHaveHappened();
        }

        [Fact]
        public
        async Task
        HandleAsync_TaskCreatedEvent_IdeaSavedWithTask()
        {
            // Arrange
            var existingIdea     = new Idea();
            var taskCreatedEvent = new TaskCreatedEvent { EventSourceId = ValidTaskId };

            A.CallTo(() =>
                _fakeRepository.GetAsync<Idea>(A<Guid>.Ignored)
            ).ReturnsCompletedTask(existingIdea);

            // Act
            await _ideasManager.HandleAsync(taskCreatedEvent);

            // Assert
            A.CallTo(() =>
                _fakeRepository.SaveAsync(A<Idea>.That.Matches(idea => idea.Tasks.Contains(ValidTaskId)), A<int>.Ignored)
            ).MustHaveHappened();
        }

        [Fact]
        public
        async Task
        HandleAsync_GoalCreatedEvent_OriginalIdeaVersionPassedAsExpected()
        {
            // Arrange
            var idea = new Idea {Version = 5};
            var goalCreatedEvent = new GoalCreatedEvent();

            A.CallTo(() =>
                _fakeRepository.GetAsync<Idea>(A<Guid>.Ignored)
            ).ReturnsCompletedTask(idea);

            // Act
            await _ideasManager.HandleAsync(goalCreatedEvent);

            // Assert
            A.CallTo(() => 
                _fakeRepository.SaveAsync(A<Idea>.Ignored, 5)
            ).MustHaveHappened();
        }
    }
}
