using Votus.Core.Infrastructure.Messaging.Eventing;

namespace Votus.Core.Infrastructure.EventSourcing
{
    public abstract class AggregateRootEvent : Event
    {
        public int Version { get; set; }
    }
}
