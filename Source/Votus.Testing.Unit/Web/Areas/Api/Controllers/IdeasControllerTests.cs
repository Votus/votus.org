using FakeItEasy;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Controllers;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class IdeasControllerTests
    {
        private readonly IdeasController        _ideasController;
        private readonly QueueManager           _fakeCommandDispatcher;
        private readonly IPartitionedRepository _fakeIdeasRepo;

        public IdeasControllerTests()
        {
            _fakeCommandDispatcher  = A.Fake<QueueManager>();
            _fakeIdeasRepo          = A.Fake<IPartitionedRepository>();

            _ideasController = new IdeasController {
                CommandDispatcher          = _fakeCommandDispatcher,
                IdeasByTimeDescendingCache = _fakeIdeasRepo
            };
        }
    }
}
