using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Domain.TestEntities
{
    public class TestEntityCreatedEvent : AggregateRootEvent
    {
        public string TestProperty { get; set; }
    }
}
