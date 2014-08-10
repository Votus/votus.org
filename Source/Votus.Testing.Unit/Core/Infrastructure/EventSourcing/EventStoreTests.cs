using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Infrastructure.Serialization;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.EventSourcing
{
    public class EventStoreTests
    {
        private readonly EventStore             _eventStore;
        private readonly IEventBus              _fakeEventBus;
        private readonly ISerializer            _fakeSerializer;
        private readonly IPartitionedRepository _fakeEventRepository;

        private readonly  EventEnvelope ValidEventEnvelope      = new EventEnvelope {
                                                                    Payload     = "{}",
                                                                    PayloadType = "SampleEntityCreatedEvent"
                                                                };

        private readonly    Guid    ValidId                     = Guid.NewGuid();
        private const       int     FirstCreationEventVersion   = -1;

        public EventStoreTests()
        {
            _fakeEventBus        = A.Fake<IEventBus>();
            _fakeSerializer      = A.Fake<ISerializer>();
            _fakeEventRepository = A.Fake<IPartitionedRepository>();

            _eventStore = new EventStore {
                EventBus        = _fakeEventBus,
                Serializer      = _fakeSerializer,
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
                new SampleEntityCreatedEvent()
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
                new SampleEntityCreatedEvent()
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
        GetAsync_RepoReturnsEvents_EventsAreReturned()
        {
            // Arrange
            var expectedEvents = new List<EventEnvelope> {
                ValidEventEnvelope
            };

            A.CallTo(() => 
                _fakeEventRepository.GetPartitionAsync<EventEnvelope>(ValidId)
            ).ReturnsCompletedTask(expectedEvents);

            A.CallTo(() =>
                _fakeSerializer.Deserialize(A<string>.Ignored, A<Type>.Ignored)
            ).Returns(new SampleEntityCreatedEvent());

            // Act
            var actualEvents = await _eventStore.GetAsync(ValidId);

            // Assert
            Assert.Equal(expectedEvents.Count, actualEvents.Count());
        }

        [Fact]
        public 
        void 
        ConvertToEnvelope_EventIsValid_PayloadTypeContainsOnlyTypeName()
        {
            // Arrange
            var aggregateRootEvent = new SampleEntityCreatedEvent();

            // Act
            var actual = _eventStore.ConvertToEnvelope(aggregateRootEvent);

            // Assert
            Assert.Equal("SampleEntityCreatedEvent", actual.PayloadType);
        }

        [Fact]
        public
        void
        ConvertToEvent_PayloadTypeContainsOnlyTypeName_TypeUsedForDeserialization()
        {
            // Arrange
            var envelope = new EventEnvelope {
                PayloadType = "SampleEntityCreatedEvent"
            };

            var serializerCall = A.CallTo(() => 
                _fakeSerializer.Deserialize(
                    A<string>.Ignored, 
                    typeof(SampleEntityCreatedEvent))
            );

            serializerCall.Returns(new SampleEntityCreatedEvent());

            // Act
            _eventStore.ConvertToEvent(envelope);

            // Assert
            serializerCall.MustHaveHappened();
        }

        [Fact]
        public
        void
        ConvertToEvent_PayloadTypeContainsFullName_TypeUsedForDeserialization()
        {
            // Arrange
            var envelope = new EventEnvelope {
                PayloadType = "Some.Random.Namespace.SampleEntityCreatedEvent"
            };

            var serializerCall = A.CallTo(() => 
                _fakeSerializer.Deserialize(
                    A<string>.Ignored, 
                    typeof(SampleEntityCreatedEvent))
            );

            serializerCall.Returns(new SampleEntityCreatedEvent());

            // Act
            _eventStore.ConvertToEvent(envelope);

            // Assert
            serializerCall.MustHaveHappened();
        }
    }

    public class SampleEntityCreatedEvent  : AggregateRootEvent { }
}
