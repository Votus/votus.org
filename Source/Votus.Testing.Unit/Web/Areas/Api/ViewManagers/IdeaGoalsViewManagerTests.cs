using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Domain.Goals;
using Votus.Core.Domain.Ideas;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.ViewManagers
{
    public class IdeaGoalsViewManagerTests
    {
        #region Test Setup

        private const           int     ValidVersion   = 1;
        private const           string  ValidGoalTitle = "Valid Title";
        private static readonly Guid    ValidIdeaId    = Guid.NewGuid();
        private static readonly Guid    ValidGoalId    = Guid.NewGuid();

        readonly GoalAddedToIdeaEvent ValidGoalAddedToIdeaEvent = new GoalAddedToIdeaEvent {
            GoalId        = ValidGoalId,
            EventSourceId = ValidIdeaId, 
            Version       = ValidVersion
        };

        private readonly GoalViewModel ValidGoalViewModel = new GoalViewModel {
            Id    = ValidGoalId,
            Title = ValidGoalTitle
        };

        private readonly IKeyValueRepository            _fakeViewRepo;
        private readonly IdeaGoalsViewManager           _viewManager;
        private readonly IVersioningRepository<Goal>    _fakeGoalRepo;

        public IdeaGoalsViewManagerTests()
        {
            _viewManager = new IdeaGoalsViewManager {
                ViewRepository = _fakeViewRepo = A.Fake<IKeyValueRepository>(),
                GoalRepository = _fakeGoalRepo = A.Fake<IVersioningRepository<Goal>>()
            };

            A.CallTo(() =>
                _fakeViewRepo.GetAsync<IEnumerable<GoalViewModel>>(A<string>.Ignored)
            ).ReturnsCompletedTask(Enumerable.Empty<GoalViewModel>());

            A.CallTo(() =>
                _fakeGoalRepo.GetAsync<Goal>(A<Guid>.Ignored)
            ).ReturnsCompletedTask(GenerateGoal());
        }

        #endregion

        [Fact]
        public
        async Task
        HandleAsync_CachedViewExists_CachedViewIsUpdated()
        {
            // Arrange
            var existingView = new List<GoalViewModel> { ValidGoalViewModel };

            A.CallTo(() =>
                _fakeViewRepo.GetAsync<List<GoalViewModel>>(A<string>.Ignored)
            ).ReturnsCompletedTask(existingView);

            // Act
            await _viewManager.HandleAsync(ValidGoalAddedToIdeaEvent);

            // Assert
            A.CallTo(() =>
                _fakeViewRepo.SetAsync(
                    A<string>.That.Not.IsNull(),
                    A<List<GoalViewModel>>.That.Matches(list => list.Count() == 2)
                )
            ).MustHaveHappened();
        }

        [Fact]
        public
        async Task
        HandleAsync_CachedViewDoesntExist_CachedViewIsSet()
        {
            // Arrange
            A.CallTo(() => 
                _fakeViewRepo.GetAsync<IEnumerable<GoalViewModel>>(A<string>.Ignored)
            ).ReturnsCompletedTask(null);

            // Act
            await _viewManager.HandleAsync(ValidGoalAddedToIdeaEvent);

            // Assert
            A.CallTo(() =>
                _fakeViewRepo.SetAsync(
                    A<string>.That.Not.IsNull(), 
                    A<List<GoalViewModel>>.That.Not.IsNull()
                )
            ).MustHaveHappened();
        }

        [Fact]
        public
        async Task
        HandleAsync_ValidGoalAddedToIdeaEvent_AddedToCachedList()
        {
            // Act
            await _viewManager.HandleAsync(ValidGoalAddedToIdeaEvent);

            // Assert
            A.CallTo(() =>
                _fakeViewRepo.SetAsync(
                    A<string>.That.Not.IsNull(), 
                    A<List<GoalViewModel>>.That.Matches(list => list.Any(m => m.Id == ValidGoalAddedToIdeaEvent.GoalId))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public
        async Task
        HandleAsync_GoalIdExists_GoalTitleAddedToCachedList()
        {
            // Arrange
            var goal = GenerateGoal(title: "A cool goal!");

            A.CallTo(() => 
                _fakeGoalRepo.GetAsync<Goal>(A<Guid>.Ignored)
            ).ReturnsCompletedTask(goal);

            // Act
            await _viewManager.HandleAsync(ValidGoalAddedToIdeaEvent);

            // Assert
            A.CallTo(() =>
                _fakeViewRepo.SetAsync(
                    A<string>.That.Not.IsNull(), 
                    A<List<GoalViewModel>>.That.Matches(list => list.Any(m => m.Title == goal.Title))
                )
            ).MustHaveHappened();
        }

        private 
        static 
        Goal 
        GenerateGoal(
            string title = ValidGoalTitle)
        {
            return new Goal(
                ValidGoalId, 
                ValidIdeaId, 
                title
            );
        }
    }
}
