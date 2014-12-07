using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Domain
{
    public class TestEntity : AggregateRoot
    {
        public string TestProperty { get; set; }

        public 
        TestEntity(
            Guid    id, 
            string  testProperty)
        {
            ApplyEvent(new TestEntityCreatedEvent {
                EventSourceId = id,
                Version       = 1,
                TestProperty  = testProperty
            });

        }

        public 
        void 
        Apply(
            TestEntityCreatedEvent testEntityCreatedEvent)
        {
            Id           = testEntityCreatedEvent.EventSourceId;
            TestProperty = testEntityCreatedEvent.TestProperty;
        }
    }

    public class TestEntityCreatedEvent : AggregateRootEvent
    {
        public string TestProperty { get; set; }
    }
}
