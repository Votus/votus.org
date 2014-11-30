using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using Votus.Core.Domain.Ideas;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.ViewManagers
{
    public class TasksByIdeaViewManagerTests
    {
        #region Test Setup

        private const           int     ValidVersion   = 1;
        private const           string  ValidTaskTitle = "Valid Title";
        private static readonly Guid    ValidIdeaId    = Guid.NewGuid();
        private static readonly Guid    ValidTaskId    = Guid.NewGuid();

        readonly TaskAddedToIdeaEvent ValidTaskAddedToIdeaEvent = new TaskAddedToIdeaEvent {
            TaskId        = ValidTaskId,
            EventSourceId = ValidIdeaId, 
            Version       = ValidVersion
        };

        private readonly IKeyValueRepository            _fakeViewRepo;
        private readonly TasksByIdeaViewManager           _viewManager;
        private readonly IVersioningRepository<Task>    _fakeTaskRepo;

        public 
        TasksByIdeaViewManagerTests()
        {
            _viewManager = new TasksByIdeaViewManager {
                ViewRepository = _fakeViewRepo = A.Fake<IKeyValueRepository>(),
                TaskRepository = _fakeTaskRepo = A.Fake<IVersioningRepository<Task>>()
            };

            A.CallTo(() =>
                _fakeViewRepo.GetAsync<IEnumerable<GoalViewModel>>(A<string>.Ignored)
            ).ReturnsCompletedTask(Enumerable.Empty<GoalViewModel>());

            A.CallTo(() =>
                _fakeTaskRepo.GetAsync<Task>(A<Guid>.Ignored)
            ).ReturnsCompletedTask(CreateTask());
        }

        #endregion

        [Fact]
        public
        async System.Threading.Tasks.Task
        HandleAsync_CachedViewExists_CachedViewIsUpdated()
        {
            // Arrange
            var existingView = new List<TaskViewModel> { CreateTaskViewModel() };

            A.CallTo(() =>
                _fakeViewRepo.GetAsync<List<TaskViewModel>>(A<string>.Ignored)
            ).ReturnsCompletedTask(existingView);

            // Act
            await _viewManager.HandleAsync(ValidTaskAddedToIdeaEvent);

            // Assert
            A.CallTo(() =>
                _fakeViewRepo.SetAsync(
                    A<string>.That.Not.IsNull(),
                    A<List<TaskViewModel>>.That.Matches(list => list.Count() == 2)
                )
            ).MustHaveHappened();
        }

        [Fact]
        public
        async System.Threading.Tasks.Task
        HandleAsync_CachedViewDoesntExist_CachedViewIsSet()
        {
            // Arrange
            A.CallTo(() => 
                _fakeViewRepo.GetAsync<IEnumerable<TaskViewModel>>(A<string>.Ignored)
            ).ReturnsCompletedTask(null);

            // Act
            await _viewManager.HandleAsync(ValidTaskAddedToIdeaEvent);

            // Assert
            A.CallTo(() =>
                _fakeViewRepo.SetAsync(
                    A<string>.That.Not.IsNull(), 
                    A<List<TaskViewModel>>.That.Not.IsNull()
                )
            ).MustHaveHappened();
        }

        [Fact]
        public
        async System.Threading.Tasks.Task
        HandleAsync_ValidGoalAddedToIdeaEvent_AddedToCachedList()
        {
            // Act
            await _viewManager.HandleAsync(ValidTaskAddedToIdeaEvent);

            // Assert
            A.CallTo(() =>
                _fakeViewRepo.SetAsync(
                    A<string>.That.Not.IsNull(), 
                    A<List<TaskViewModel>>.That.Matches(list => list.Any(m => m.Id == ValidTaskAddedToIdeaEvent.TaskId))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public
        async System.Threading.Tasks.Task
        HandleAsync_TaskIdExists_TaskTitleAddedToCachedList()
        {
            // Arrange
            var task = CreateTask(title: "A cool task!");

            A.CallTo(() => 
                _fakeTaskRepo.GetAsync<Task>(A<Guid>.Ignored)
            ).ReturnsCompletedTask(task);

            // Act
            await _viewManager.HandleAsync(ValidTaskAddedToIdeaEvent);

            // Assert
            A.CallTo(() =>
                _fakeViewRepo.SetAsync(
                    A<string>.That.Not.IsNull(), 
                    A<List<TaskViewModel>>.That.Matches(list => list.Any(m => m.Title == task.Title))
                )
            ).MustHaveHappened();
        }

        static 
        TaskViewModel 
        CreateTaskViewModel()
        {
            return new TaskViewModel {
                Id    = Guid.NewGuid(),
                Title = ValidTaskTitle
            };
        }

        private 
        static 
        Task
        CreateTask(
            string title = ValidTaskTitle)
        {
            return new Task(
                ValidTaskId, 
                ValidIdeaId, 
                title
            );
        }
    }
}
