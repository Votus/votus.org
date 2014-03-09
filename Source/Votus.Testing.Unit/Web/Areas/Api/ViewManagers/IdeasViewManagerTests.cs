using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Domain.Ideas;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.ViewManagers
{
    public class IdeasViewManagerTests
    {
        private readonly IPartitionedRepository _fakeIdeasRepo;
        private readonly IdeasViewManager       _manager;

        public IdeasViewManagerTests()
        {
            _fakeIdeasRepo = A.Fake<IPartitionedRepository>();
            _manager       = new IdeasViewManager {
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
