using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Unit.Core.Domain.Ideas
{
    public class IdeaTasksViewManagerTests
    {
        private readonly IKeyValueRepository    _fakeRepo;
        private readonly IdeaTasksViewManager   _ideaTasksViewManager;

        public 
        IdeaTasksViewManagerTests()
        {
            _fakeRepo             = A.Fake<IKeyValueRepository>();
            _ideaTasksViewManager = new IdeaTasksViewManager {ViewRepository = _fakeRepo};
        }

        [Fact]
        public 
        async Task 
        HandleAsync_TaskVotedCompleteEvent_CompletedVoteCountIsIncremented()
        {
            // Arrange
            var tasks = new List<TaskViewModel> {
                new TaskViewModel {
                    CompletedVoteCount = 1
                }
            };

            A.CallTo(() => 
                _fakeRepo.GetAsync<IEnumerable<TaskViewModel>>(A<object>.Ignored)
            ).ReturnsCompletedTask(tasks);

            // Act
            await _ideaTasksViewManager.HandleAsync(new TaskVotedCompleteEvent());

            // Assert
            A.CallTo(() =>
                _fakeRepo.SetAsync(
                    A<object>.Ignored, 
                    A<IEnumerable<TaskViewModel>>.That.Matches(t => t.First().CompletedVoteCount == 2))
            ).MustHaveHappened();
        }
    }
}
