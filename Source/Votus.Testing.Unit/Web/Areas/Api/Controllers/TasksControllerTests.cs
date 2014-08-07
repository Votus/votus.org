using System.Collections.Generic;
using FakeItEasy;
using System;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.Collections;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Controllers;
using Votus.Web.Areas.Api.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class TasksControllerTests
    {
        private readonly Guid ValidIdeaId = Guid.NewGuid();

        private readonly TasksController        _tasksController;
        private readonly IKeyValueRepository    _fakeViewCache;
        private readonly QueueManager           _fakeQueueManager;

        public TasksControllerTests()
        {
            _fakeViewCache    = A.Fake<IKeyValueRepository>();
            _fakeQueueManager = A.Fake<QueueManager>();
            
            _tasksController = new TasksController {
                ViewCache         = _fakeViewCache,
                CommandDispatcher = _fakeQueueManager
            };
        }

        [Fact]
        public
        async Task
        GetTasksByIdeaIdAsync_ViewCacheContainsIdeaTasks_ReturnsCachedTasks()
        {
            // Arrange
            var expectedTasks = new HashSet<TaskViewModel> {
                new TaskViewModel { Id = Guid.NewGuid(), Title ="Task 1" },
                new TaskViewModel { Id = Guid.NewGuid(), Title ="Task 2" }
            };

            A.CallTo(() => 
                _fakeViewCache.GetAsync<HashSet<TaskViewModel>>(A<string>.Ignored)
             ).ReturnsCompletedTask(expectedTasks);

            // Act
            var returnedTasks = await _tasksController.GetTasksByIdeaIdAsync(ValidIdeaId);

            // Assert
            Assert.True(returnedTasks.Contains(expectedTasks));
        }

        [Fact]
        public
        async Task
        GetTaskByIdAsync_ViewCacheContainsTask_ReturnsCachedTask()
        {
            // Arrange
            var expectedTask = new TaskViewModel {
                Id = Guid.NewGuid()
            };

            A.CallTo(() => 
                _fakeViewCache.GetAsync<TaskViewModel>(A<string>.Ignored)
            ).ReturnsCompletedTask(expectedTask);

            // Act
            var actualTask = await _tasksController.GetTaskById(
                expectedTask.Id
            );

            // Assert
            Assert.Equal(
                expectedTask, 
                actualTask
            );
        }

        [Fact]
        public
        async Task
        VoteTaskCompleted_VoteIsValid_VoteTaskCompletedCommandIsSent()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            // Act
            await _tasksController.VoteTaskCompleted(
                taskId
            );

            // Assert
            A.CallTo(() =>
                _fakeQueueManager.SendAsync(
                    A<Guid>.Ignored,
                    A<VoteTaskCompletedCommand>.Ignored)
            ).MustHaveHappened();
        }
    }
}
