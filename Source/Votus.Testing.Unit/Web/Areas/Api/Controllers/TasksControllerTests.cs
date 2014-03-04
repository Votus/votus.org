using FakeItEasy;
using System;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Collections;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Controllers;
using Votus.Web.Areas.Api.Models;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class TasksControllerTests
    {
        private readonly Guid ValidIdeaId = Guid.NewGuid();

        private readonly TasksController        _tasksController;
        private readonly IKeyValueRepository    _fakeViewCache;

        public TasksControllerTests()
        {
            _fakeViewCache = A.Fake<IKeyValueRepository>();
            
            _tasksController = new TasksController {
                ViewCache = _fakeViewCache
            };
        }

        [Fact]
        public
        async Task
        GetTasksByIdeaIdAsync_ViewCacheContainsIdeaTasks_ReturnsCachedTasks()
        {
            // Arrange
            var expectedTasks = new ConsistentHashSet<TaskViewModel> {
                new TaskViewModel { Id = Guid.NewGuid(), Title ="Task 1" },
                new TaskViewModel { Id = Guid.NewGuid(), Title ="Task 2" }
            };

            A.CallTo(() => 
                _fakeViewCache.GetAsync<ConsistentHashSet<TaskViewModel>>(A<string>.Ignored)
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
                _fakeViewCache.GetAsync<TaskViewModel>(expectedTask.Id)
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
    }
}
