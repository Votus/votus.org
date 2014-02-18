using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Infrastructure.EventSourcing;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.EventSourcing
{
    public class EventPublisherTests
    {
        private readonly IEventBus      _fakeEventBus;
        private readonly EventPublisher _eventPublisher;

        public EventPublisherTests()
        {
            _fakeEventBus   = A.Fake<IEventBus>();
            _eventPublisher = new EventPublisher {
                EventBus = _fakeEventBus
            };
        }

        [Fact]
        public
        async Task 
        PublishNewEventsAsync_AggregateRootHasUnpublishedEvents_EventsArePublished()
        {
            // Arrange
            var sampleAggregateRoot = new SampleAggregateRoot();

            sampleAggregateRoot.CreateEvent();
            sampleAggregateRoot.CreateEvent();

            // Act
            await _eventPublisher.PublishNewEventsAsync(sampleAggregateRoot);

            // Assert
            A.CallTo(() => 
                _fakeEventBus.PublishAsync(A<IEnumerable<AggregateRootEvent>>.That.Matches(events => events.Count() == 2))
            ).MustHaveHappened();
        }

        #region Helper Classes

        class SampleAggregateRoot : AggregateRoot
        {
            public void CreateEvent()
            {
                ApplyEvent(new SampleAggregateRootEvent());
            }
        }

        class SampleAggregateRootEvent : AggregateRootEvent
        {
            
        }

        #endregion
    }
}
