using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Ideas
{
    public class IdeaCreatedEvent : AggregateRootEvent
    {
        public string Title { get; set; }
        public string Tag   { get; set; }
    }
}
