using System;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Models;
using Votus.Web.Areas.Api.ViewManagers;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Unit.Web.Areas.Api.ViewManagers
{
    public class IdeaByIdViewManagerTests
    {
        private readonly IKeyValueRepository _fakeViewCache;
        private readonly IdeaByIdViewManager _viewManager;

        public IdeaByIdViewManagerTests()
        {
            _fakeViewCache = A.Fake<IKeyValueRepository>();

            _viewManager   = new IdeaByIdViewManager {
                ViewCache = _fakeViewCache
            };
        }

        [Fact]
        public 
        async Task 
        HandleAsync_IdeaCreatedEvent_IdeaIsAddedToViewCache()
        {
            // Arrange
            var ideaId = Guid.NewGuid();
            var ideaCreatedEvent = new IdeaCreatedEvent {EventSourceId = ideaId};

            // Act
            await _viewManager.HandleAsync(ideaCreatedEvent);

            // Assert
            A.CallTo(() => 
                _fakeViewCache.SetAsync(
                    A<object>.Ignored, 
                    A<IdeaViewModel>.That.Matches(model => model.Id == ideaId)
                )
            ).MustHaveHappened();
        }
    }
}
