using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.EventSourcing;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Data
{
    public class AggregateRootRepositoryTests
    {
        private readonly Guid ValidAggregateRootId = Guid.NewGuid();

        private readonly EventStore                 _fakeEventStore;
        private readonly AggregateRootRepository    _repository;

        public
        AggregateRootRepositoryTests()
        {
            _fakeEventStore = A.Fake<EventStore>();
            _repository     = new AggregateRootRepository {
                EventStore = _fakeEventStore
            };
        }

        [Fact]
        public
        async Task
        SaveAsync_EntityHasUncommittedEvents_SavesToEventStore()
        {
            // Arrange
            const int expectedVersion = 1;

            var aggregateRoot   = new SampleAggregateRoot {
                Id = Guid.NewGuid()
            };

            aggregateRoot.CreateEvent();

            // Act
            await _repository.SaveAsync(aggregateRoot, expectedVersion);

            // Assert
            A.CallTo(() => 
                _fakeEventStore.SaveAsync(
                    aggregateRoot.Id,
                    A<IEnumerable<AggregateRootEvent>>.That.Not.IsNull(),
                    expectedVersion)
            ).MustHaveHappened();
        }

        [Fact]
        public
        async Task
        GetAsync_AggregateRootHasHistory_HistoryIsLoaded()
        {
            // Arrange
            var events = new List<AggregateRootEvent> {
                new SampleAggregateRootCreated { EventSourceId = ValidAggregateRootId }
            };

            A.CallTo(() => 
                _fakeEventStore.GetAsync(A<Guid>.Ignored)
            ).ReturnsCompletedTask(events);

            // Act
            var actual = (await _repository.GetAsync<SampleAggregateRoot>(ValidAggregateRootId)).Id;

            // Assert
            Assert.Equal(ValidAggregateRootId, actual);
        }

        class SampleAggregateRoot : AggregateRoot 
        {
            public void CreateEvent()
            {
                ApplyEvent(new SampleAggregateRootModified());
            }

            void
            Apply(SampleAggregateRootCreated sampleAggregateRootCreated)
            {
                Id = sampleAggregateRootCreated.EventSourceId;
            }

            void
            Apply(SampleAggregateRootModified sampleAggregateRootModified)
            {
            }
        }

        class SampleAggregateRootCreated  : AggregateRootEvent { }
        class SampleAggregateRootModified : AggregateRootEvent { }
    }
}