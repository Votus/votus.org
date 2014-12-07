using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Data;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Unit.Core.Domain
{
    public class GoalsTests
    {
        private readonly Goals                _goalsManager;
        private readonly IVersioningRepository<Goal> _fakeRepository;

        public GoalsTests()
        {
            _fakeRepository = A.Fake<IVersioningRepository<Goal>>();
            _goalsManager   = new Goals {
                Repository = _fakeRepository
            };

            A.CallTo(() =>
                _fakeRepository.SaveAsync(A<Goal>.Ignored, A<int>.Ignored)
            ).ReturnsCompletedTask();
        }

        [Fact]
        public 
        async 
        Task 
        HandleAsync_CreateGoalCommand_GoalIsPersisted()
        {
            // Arrange
            var command = new CreateGoalCommand();

            // Act
            await _goalsManager.HandleAsync(command);

            // Assert
            A.CallTo(() =>
                _fakeRepository.SaveAsync(A<Goal>.That.Not.IsNull(), A<int>.Ignored)
            ).MustHaveHappened();
        }
    }
}
