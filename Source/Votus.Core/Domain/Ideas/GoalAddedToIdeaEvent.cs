using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Domain.Ideas
{
    public class GoalAddedToIdeaEvent : AggregateRootEvent
    {
        public Guid GoalId { get; set; }
    }
}
