using FakeItEasy;
using System;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Collections;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Controllers;
using Votus.Web.Areas.Api.Models;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class GoalsControllerTests
    {
        private readonly Guid ValidIdeaId = Guid.NewGuid();

        private readonly QueueManager           _fakeCommandDispatcher;
        private readonly GoalsController        _goalsController;
        private readonly IKeyValueRepository    _fakeViewCache;

        public GoalsControllerTests()
        {
            _fakeViewCache         = A.Fake<IKeyValueRepository>();
            _fakeCommandDispatcher = A.Fake<QueueManager>();
            
            _goalsController = new GoalsController {
                ViewCache         = _fakeViewCache,
                CommandDispatcher = _fakeCommandDispatcher
            };
        }

        [Fact]
        public
        async Task
        GetGoalsAsync_ViewCacheContainsIdeaGoals_ReturnsCachedGoals()
        {
            // Arrange
            var expectedGoals = new ConsistentHashSet<GoalViewModel> {
                new GoalViewModel { Id    = Guid.NewGuid(), Title ="Goal 1" },
                new GoalViewModel { Id    = Guid.NewGuid(), Title ="Goal 2" }
            };

            A.CallTo(() => 
                _fakeViewCache.GetAsync<ConsistentHashSet<GoalViewModel>>(A<string>.Ignored)
             ).ReturnsCompletedTask(expectedGoals);

            // Act
            var returnedGoals = await _goalsController.GetGoalsAsync(ValidIdeaId);

            // Assert
            Assert.True(returnedGoals.Contains(expectedGoals));
        }
    }
}
