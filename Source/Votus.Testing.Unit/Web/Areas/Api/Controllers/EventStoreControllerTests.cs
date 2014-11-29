using System;
using FakeItEasy;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Controllers;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class EventStoreControllerTests
    {
        private readonly QueueManager            _fakeCommandBus;
        private readonly EventStoreController    _eventStoreController;

        public EventStoreControllerTests()
        {
            _fakeCommandBus = A.Fake<QueueManager>();
            
            _eventStoreController = new EventStoreController {
                CommandBus = _fakeCommandBus
            };
        }
        
        [Fact]
        public 
        async Task 
        RepublishEventsAsync_Always_SendsRepublishEventsCommand()
        {
            // Arrange
 
            // Act
            await _eventStoreController.RepublishEventsAsync();

            // Assert
            A.CallTo(() => 
                _fakeCommandBus.SendAsync(
                    A<Guid>.Ignored, 
                    A<RepublishAllEventsCommand>.That.Not.IsNull())
            ).MustHaveHappened();
        }
    }
}
