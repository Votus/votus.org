using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Tasks
{
    public class TaskVotedCompleteEvent : AggregateRootEvent
    {
        // TODO: Should not be needed, remove when the IdeaTasksViewManager stops caching full Task views.
        public Guid IdeaId { get; set; }
    }
}
