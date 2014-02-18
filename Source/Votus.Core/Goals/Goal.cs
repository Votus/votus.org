using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Goals
{
    public class Goal : AggregateRoot
    {
        public string   Title           { get; set; }
        public Guid     InitialIdeaId   { get; set; }

        public Goal() { }

        public 
        Goal(
            Guid    id, 
            Guid    initialIdeaId,
            string  title)
        {
            ApplyEvent(new GoalCreatedEvent {
                EventSourceId = id,
                Version       = 1,
                InitialIdeaId = initialIdeaId,
                Title         = title
            });
        }

        public 
        void 
        Apply(
            GoalCreatedEvent goalCreatedEvent)
        {
            Id            = goalCreatedEvent.EventSourceId;
            InitialIdeaId = goalCreatedEvent.InitialIdeaId;
            Title         = goalCreatedEvent.Title;
        }
    }
}
