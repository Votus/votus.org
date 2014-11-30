using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Messaging.Eventing;
using Votus.Core.Infrastructure.Serialization;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.EventSourcing
{
    public class EventStoreTests
    {
        private readonly ILog                   _fakeLog;
        private readonly EventStore             _eventStore;
        private readonly IEventBus              _fakeEventBus;
        private readonly IPartitionedRepository _fakeEventRepository;

        private readonly    Guid    ValidId                     = Guid.NewGuid();
        private const       int     FirstCreationEventVersion   = -1;

        public EventStoreTests()
        {
            _fakeLog             = A.Fake<ILog>();
            _fakeEventBus        = A.Fake<IEventBus>();
            _fakeEventRepository = A.Fake<IPartitionedRepository>();

            _eventStore = new EventStore {
                Log             = _fakeLog,
                EventBus        = _fakeEventBus,
                Serializer      = new NewtonsoftJsonSerializer(),
                EventRepository = _fakeEventRepository
            };
        }

        [Fact]
        public 
        async Task 
        SaveAsync_FirstCreationEvent_EventsArePersisted()
        {
           // Arrange
            var eventsToSave = new[] {
                CreateSampleEvent()
            };
 
            // Act
            await _eventStore.SaveAsync(
                ValidId, 
                eventsToSave, 
                FirstCreationEventVersion
            );

            // Assert
            A.CallTo(() => 
                _fakeEventRepository.InsertBatchAsync(
                    A<object>.Ignored,
                    A<Func<EventEnvelope, object>>.Ignored,
                    A<IEnumerable<EventEnvelope>>.That.Matches(events => events.Count() == eventsToSave.Length))
            ).MustHaveHappened();
        }

        [Fact]
        public
        async Task
        SaveAsync_EventsPersisted_EventsArePublished()
        {
            // Arrange
            var eventsToSave = new[] {
                CreateSampleEvent()
            };

            // Act
            await _eventStore.SaveAsync(
                ValidId, 
                eventsToSave, 
                FirstCreationEventVersion
            );

            // Assert
            A.CallTo(() => 
                _fakeEventBus.PublishAsync(A<IEnumerable<AggregateRootEvent>>.That.Matches(events => events.Count() == eventsToSave.Length))
            ).MustHaveHappened();
        }

        [Fact]
        public 
        async Task 
        SaveAsync_NoEventsToSave_InsertBatchAsyncNotCalled()
        {
            // Arrange
            var eventsToSave = Enumerable.Empty<AggregateRootEvent>();
  
            // Act
            await _eventStore.SaveAsync(ValidId, eventsToSave, 0);

            // Assert
            A.CallTo(() => 
                _fakeEventRepository.InsertBatchAsync(
                    A<object>.Ignored, 
                    A<Func<EventEnvelope, object>>.Ignored, 
                    A<IEnumerable<EventEnvelope>>.Ignored)
            ).MustNotHaveHappened();
        }

        [Fact]
        public
        async Task
        GetAsync_RepoReturnsEvents_EventsAreReturned()
        {
            // Arrange
            var expectedEvents = new List<EventEnvelope> {
                CreateEventEnvelope()
            };

            A.CallTo(() => 
                _fakeEventRepository.GetPartitionAsync<EventEnvelope>(ValidId)
            ).ReturnsCompletedTask(expectedEvents);

            // Act
            var actualEvents = await _eventStore.GetAsync(ValidId);

            // Assert
            Assert.Equal(expectedEvents.Count, actualEvents.Count());
        }

        [Fact]
        public 
        async Task 
        RepublishAllEventsAsync_EventsStoredOutOfOrder_EventsPublishedInOrder()
        {
            // Arrange
            var unsortedEvents = new[] {
                CreateEventEnvelope(CreateSampleEvent(timestamp: DateTimeOffset.MaxValue)),
                CreateEventEnvelope(CreateSampleEvent(timestamp: DateTimeOffset.MinValue))
            };

            var sortedEvents = new[] {
                CreateSampleEvent(timestamp: DateTimeOffset.MinValue),
                CreateSampleEvent(timestamp: DateTimeOffset.MaxValue)
            };

            A.CallTo(() => 
                _fakeEventRepository.GetAllAsync<EventEnvelope>()
            ).ReturnsCompletedTask(unsortedEvents);
            
            // Act
            await _eventStore.RepublishAllEventsAsync();

            // Assert
            A.CallTo(() =>
                _fakeEventBus.PublishAsync(
                    A<AggregateRootEvent[]>.That.IsSameSequenceAs(sortedEvents))
            ).MustHaveHappened();
        }

        [Fact]
        public 
        void 
        ConvertToEnvelope_EventIsValid_PayloadTypeContainsOnlyTypeName()
        {
            // Arrange
            var aggregateRootEvent = CreateSampleEvent();

            // Act
            var actual = _eventStore.ConvertToEnvelope(aggregateRootEvent);

            // Assert
            Assert.Equal("SampleEntityCreatedEvent", actual.PayloadType);
        }

        #region Helper Methods

        static 
        EventEnvelope 
        CreateEventEnvelope()
        {
            return CreateEventEnvelope(CreateSampleEvent());
        }

        static 
        SampleEntityCreatedEvent
        CreateSampleEvent(
            DateTimeOffset timestamp = default(DateTimeOffset))
        {
            return new SampleEntityCreatedEvent {
                Timestamp = timestamp
            };
        }

        static 
        EventEnvelope 
        CreateEventEnvelope(
            Event @event)
        {
            return new EventEnvelope {
                Payload     = string.Format(
                                "{{'Timestamp':'{0}'}}", 
                                @event.Timestamp.ToString("O")),
                PayloadType = @event.GetType().Name
            };
        }

        #endregion
    }

    public class SampleEntityCreatedEvent  : AggregateRootEvent { }
}
