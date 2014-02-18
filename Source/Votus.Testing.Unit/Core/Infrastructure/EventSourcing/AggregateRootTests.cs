using System;
using Votus.Core.Infrastructure.EventSourcing;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.EventSourcing
{
    public class AggregateRootTests
    {
        [Fact]
        public 
        void 
        ApplyEvent_RootIsCreated_IdIsSet()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var newRoot = new SampleAggregateRoot(id);

            // Assert
            Assert.Equal(id, newRoot.Id);
        }

        [Fact]
        public
        void 
        LoadHistory_ValidEvents_EventsAreLoaded()
        {
            // Arrange
            var id      = Guid.NewGuid();
            var ar      = new SampleAggregateRoot();
            var history = new[] {new SampleAggregateRootCreated(id)};

            // Act
            ar.LoadHistory(history);
            
            // Assert
            Assert.Equal(id, ar.Id);
        }

        [Fact]
        public 
        void 
        ApplyEvent_Always_UpdatesVersion()
        {
            // Arrange
            var ar          = new SampleAggregateRoot{ Version = 1 };
            var sampleEvent = new SampleAggregateRootUpdated{ Version = 2 };

            // Act
            ar.ApplyEvent(sampleEvent);

            // Assert
            Assert.Equal(2, ar.Version);
        }

        #region Helper Classes

        class SampleAggregateRoot : AggregateRoot
        {
            public SampleAggregateRoot(){ }

            public SampleAggregateRoot(Guid id)
            {
                ApplyEvent(new SampleAggregateRootCreated(id));
            }

            private void Apply(SampleAggregateRootCreated createdEvent)
            {
                Id = createdEvent.EventSourceId;
            }
        }

        class SampleAggregateRootCreated : AggregateRootEvent
        {
            public SampleAggregateRootCreated(Guid id)
            {
                EventSourceId = id;
            }
        }

        class SampleAggregateRootUpdated : AggregateRootEvent
        {
            
        }

        #endregion
    }
}
