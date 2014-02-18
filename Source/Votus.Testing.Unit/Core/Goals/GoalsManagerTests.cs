using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Goals;
using Votus.Core.Infrastructure.Data;
using Xunit;

namespace Votus.Testing.Unit.Core.Goals
{
    public class GoalsManagerTests
    {
        private readonly GoalsManager                _goalsManager;
        private readonly IVersioningRepository<Goal> _fakeRepository;

        public GoalsManagerTests()
        {
            _fakeRepository = A.Fake<IVersioningRepository<Goal>>();
            _goalsManager   = new GoalsManager {
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
