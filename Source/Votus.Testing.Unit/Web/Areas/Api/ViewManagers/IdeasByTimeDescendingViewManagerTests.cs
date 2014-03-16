using FakeItEasy;
using System;
using System.Threading.Tasks;
using Votus.Core.Domain.Ideas;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.ViewManagers
{
    public class IdeasByTimeDescendingViewManagerTests
    {
        private readonly IPartitionedRepository             _fakeIdeasRepo;
        private readonly IdeasByTimeDescendingViewManager   _manager;

        public IdeasByTimeDescendingViewManagerTests()
        {
            _fakeIdeasRepo = A.Fake<IPartitionedRepository>();
            _manager       = new IdeasByTimeDescendingViewManager {
                IdeasRepository = _fakeIdeasRepo
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
