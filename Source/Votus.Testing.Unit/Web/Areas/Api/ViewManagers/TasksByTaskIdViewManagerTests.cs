using System;
using FakeItEasy;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.ViewManagers
{
    public class TasksByTaskIdViewManagerTests
    {
        private readonly TasksByTaskIdViewManager   _viewManager;
        private readonly IKeyValueRepository        _fakeRepo;

        public TasksByTaskIdViewManagerTests()
        {
            _fakeRepo = A.Fake<IKeyValueRepository>();

            _viewManager = new TasksByTaskIdViewManager {
                ViewCache = _fakeRepo
            };
        }

        [Fact]
        public
        async System.Threading.Tasks.Task
        HandleAsync_TaskCreatedEvent_TaskViewModelSavedToRepo()
        {
            // Arrange
            var taskCreatedEvent = new TaskCreatedEvent {
                EventSourceId = Guid.NewGuid()
            };

            // Act
            await _viewManager.HandleAsync(taskCreatedEvent);

            // Assert
            A.CallTo(() => 
                _fakeRepo.SetAsync(
                    A<string>.Ignored,
                    A<TaskViewModel>.That.Matches(task => task.Id == taskCreatedEvent.EventSourceId)
                )
            ).MustHaveHappened();
        }

        [Fact]
        public 
        async System.Threading.Tasks.Task
        HandleAsync_TaskVotedCompleteEvent_CompletedVoteCountIsIncremented()
        {
            // Arrange
            var taskViewModel          = new TaskViewModel { CompletedVoteCount = 1 };
            var taskVotedCompleteEvent = new TaskVotedCompleteEvent();

            A.CallTo(() => 
                _fakeRepo.GetAsync<TaskViewModel>(A<object>.Ignored)
            ).ReturnsCompletedTask(taskViewModel);

            // Act
            await _viewManager.HandleAsync(taskVotedCompleteEvent);

            // Assert
            A.CallTo(() => 
                _fakeRepo.SetAsync(A<object>.Ignored, A<TaskViewModel>.That.Matches(t => t.CompletedVoteCount == 2))
            ).MustHaveHappened();
        }
    }
}
