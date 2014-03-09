using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Domain.Goals
{
    public class GoalCreatedEvent : AggregateRootEvent
    {
        public string   Title           { get; set; }
        public Guid     InitialIdeaId   { get; set; }
    }
}
