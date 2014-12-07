using System.Web.Http;
using FakeItEasy;
using System;
using System.Threading.Tasks;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using WebApi.OutputCache.Core.Cache;
using WebApi.OutputCache.V2;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Unit.Web.Areas.Api.ViewManagers
{
    public class IdeasByTimeDescendingViewManagerTests
    {
        private readonly IPartitionedRepository             _fakeIdeasRepo;
        private readonly IdeasByTimeDescendingViewManager   _manager;
        private readonly IApiOutputCache                    _fakeCache;

        public IdeasByTimeDescendingViewManagerTests()
        {
            _fakeCache     = A.Fake<IApiOutputCache>();
            _fakeIdeasRepo = A.Fake<IPartitionedRepository>();
            
            _manager       = new IdeasByTimeDescendingViewManager {
                OutputCache     = _fakeCache,
                IdeasRepository = _fakeIdeasRepo,
                CacheConfig     = new CacheOutputConfiguration(new HttpConfiguration())
            };
        }

        [Fact]
        public 
        async Task 
        HandleAsync_ValidNewIdea_IsSavedToTopOfList()
        {
            // Arrange
            var newIdea = GetIdeaCreatedEvent();

            // Act
            await _manager.HandleAsync(newIdea);

            // Assert
            A.CallTo(() => 
                _fakeIdeasRepo.InsertAsync(
                    A<object>.Ignored,
                    A<object>.Ignored,
                    A<IdeaViewModel>
                        .That
                        .Matches(idea => idea.Id == newIdea.EventSourceId)
                )
            ).MustHaveHappened();
        }

        [Fact]
        public 
        async Task 
        HandleAsync_IdeaSaved_CacheIsInvalidated()
        {
            // Arrange
            var newIdea = GetIdeaCreatedEvent();

            // Act
            await _manager.HandleAsync(newIdea);

            // Assert
            A.CallTo(() => 
                _fakeCache.RemoveStartsWith(A<string>.Ignored)
            ).MustHaveHappened();
        }

        #region Helper Methods

        static IdeaCreatedEvent GetIdeaCreatedEvent()
        {
            return new IdeaCreatedEvent {
                EventSourceId = Guid.NewGuid(),
                Title         = "Test title",
                Version       = 1
            };
        }

        #endregion
    }
}
