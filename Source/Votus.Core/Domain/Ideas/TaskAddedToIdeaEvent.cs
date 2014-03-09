using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Domain.Ideas
{
    public class TaskAddedToIdeaEvent : AggregateRootEvent
    {
        public Guid TaskId { get; set; }
    }
}
