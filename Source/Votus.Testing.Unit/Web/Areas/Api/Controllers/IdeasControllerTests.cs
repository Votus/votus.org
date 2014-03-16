using System;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Controllers;
using Votus.Web.Areas.Api.Models;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class IdeasControllerTests
    {
        private readonly IdeasController        _ideasController;
        private readonly QueueManager           _fakeCommandDispatcher;
        private readonly IPartitionedRepository _fakeIdeasByTimeDescendingCache;
        private readonly IKeyValueRepository    _fakeViewCache;

        public
        IdeasControllerTests()
        {
            _fakeViewCache                  = A.Fake<IKeyValueRepository>();
            _fakeCommandDispatcher          = A.Fake<QueueManager>();
            _fakeIdeasByTimeDescendingCache = A.Fake<IPartitionedRepository>();
            
            _ideasController = new IdeasController {
                ViewCache                  = _fakeViewCache,
                CommandDispatcher          = _fakeCommandDispatcher,
                IdeasByTimeDescendingCache = _fakeIdeasByTimeDescendingCache
            };
        }

        [Fact]
        public 
        async Task 
        GetIdeaAsync_IdeaExists_ReturnsIdeaViewModel()
        {
            // Arrange
            var ideaId   = Guid.NewGuid();
            var expected = new IdeaViewModel {Id = ideaId};

            A.CallTo(() =>
                _fakeViewCache.GetAsync<IdeaViewModel>(A<object>.Ignored)
            ).ReturnsCompletedTask(expected);

            // Act
            var actual = await _ideasController.GetIdeaAsync(ideaId);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
