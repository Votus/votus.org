﻿using System;
using FakeItEasy;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Caching;
using Votus.Core.Infrastructure.Data;
using Xunit;

namespace Votus.Testing.Unit.Core.Domain
{
    public class TasksTests
    {
        private readonly ICache                         _fakeVoteTaskCompletedCommandRepo;
        private readonly IVersioningRepository<Task>    _fakeRepository;
        private readonly Tasks                          _tasksManager;

        public 
        TasksTests()
        {
            _fakeRepository                   = A.Fake<IVersioningRepository<Task>>();
            _fakeVoteTaskCompletedCommandRepo = A.Fake<ICache>();

            _tasksManager = new Tasks {
                TaskRepository  = _fakeRepository,
                Cache           = _fakeVoteTaskCompletedCommandRepo
            };
        }
        
        [Fact]
        public
        async System.Threading.Tasks.Task 
        HandleAsync_CreateTaskCommand_TaskIsPersisted()
        {
            // Arrange
            var createTaskCommand = new CreateTaskCommand {
                NewTaskId = Guid.NewGuid()
            };
    
            // Act
            await _tasksManager.HandleAsync(createTaskCommand);

            // Assert
            A.CallTo(() => 
                _fakeRepository.SaveAsync(
                    A<Task>.That.Matches(persistedTask => persistedTask.Id == createTaskCommand.NewTaskId), 
                    A<int>.Ignored
                )
            ).MustHaveHappened();
        }

        [Fact]
        public
        async System.Threading.Tasks.Task
        HandleAsync_VoteTaskCompletedCommand_TaskIsSaved()
        {
            // Arrange
            var task                     = new Task                     { Id     = Guid.NewGuid() };
            var voteTaskCompletedCommand = new VoteTaskCompletedCommand { TaskId = task.Id        };

            A.CallTo(() => 
                _fakeRepository.GetAsync<Task>(voteTaskCompletedCommand.TaskId)
            ).ReturnsCompletedTask(task);

            // Act
            await _tasksManager.HandleAsync(voteTaskCompletedCommand);

            // Assert
            A.CallTo(() =>
                _fakeRepository.SaveAsync(
                    A<Task>.That.Matches(persistedTask => persistedTask.Id == task.Id),
                    A<int>.Ignored
                )
            ).MustHaveHappened();
        }

        [Fact]
        public
        async System.Threading.Tasks.Task
        HandleAsync_VoteTaskCompletedCommand_OriginalVersionIsPassedToRepo()
        {
            // Arrange
            const int originalVersion = 4;
            var task = new Task { Version = originalVersion };
            
            A.CallTo(() => 
                _fakeRepository.GetAsync<Task>(A<Guid>.Ignored)
            ).ReturnsCompletedTask(task);

            // Act
            await _tasksManager.HandleAsync(new VoteTaskCompletedCommand());

            // Assert
            A.CallTo(() => 
                _fakeRepository.SaveAsync(A<Task>.Ignored, originalVersion)
            ).MustHaveHappened();
        }
    }
}