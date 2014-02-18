using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Ideas
{
    public class GoalAddedToIdeaEvent : AggregateRootEvent
    {
        public Guid GoalId { get; set; }
    }
}
