using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using WebApi.OutputCache.Core.Cache;
using WebApi.OutputCache.V2;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Unit.Core.Domain.Ideas
{
    public class IdeaTasksViewManagerTests
    {
        private readonly IKeyValueRepository        _fakeRepo;
        private readonly CacheOutputConfiguration   _fakeConfig;
        private readonly IApiOutputCache            _fakeOutputCache;
        private readonly TasksByIdeaViewManager     _ideaTasksViewManager;

        public 
        IdeaTasksViewManagerTests()
        {
            _fakeRepo        = A.Fake<IKeyValueRepository>();
            _fakeConfig      = A.Fake<CacheOutputConfiguration>();
            _fakeOutputCache = A.Fake<IApiOutputCache>();

            _ideaTasksViewManager = new TasksByIdeaViewManager {
                ViewRepository = _fakeRepo,
                CacheConfig    = _fakeConfig,
                OutputCache    = _fakeOutputCache
            };
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
