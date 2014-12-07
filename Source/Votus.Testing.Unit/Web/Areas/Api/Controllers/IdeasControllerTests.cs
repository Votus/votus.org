using System;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Controllers;
using Votus.Web.Areas.Api.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class IdeasControllerTests
    {
        private readonly    Guid    ValidIdeaId    = Guid.NewGuid();
        private const       string  ValidIdeaTitle = "Valid Idea Title";

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

        [Fact]
        public 
        async Task
        CreateIdea_IsValid_CreateIdeaCommandIsSent()
        {
            // Arrange
            var command = new CreateIdeaCommand {
                NewIdeaId = ValidIdeaId
            };

            // Act
            await _ideasController.CreateIdea(
                command
            );
            
            // Assert
            A.CallTo(() => 
                _fakeCommandDispatcher.SendAsync(
                    ValidIdeaId, 
                    command)
            ).MustHaveHappened();
        }
    }
}
