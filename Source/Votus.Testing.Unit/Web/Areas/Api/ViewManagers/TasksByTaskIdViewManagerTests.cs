using System;
using FakeItEasy;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Tasks;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Xunit;
using Task = System.Threading.Tasks.Task;

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
        async Task 
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
                    taskCreatedEvent.EventSourceId,
                    A<TaskViewModel>.That.Matches(task => task.Id == taskCreatedEvent.EventSourceId)
                )
            ).MustHaveHappened();
        }
    }
}
