using System;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Tasks
{
    public class Task : AggregateRoot
    {
        public string   Title               { get; set; }
        public Guid     InitialIdeaId       { get; set; }
        public int      CompletedVoteCount  { get; set; }

        public Task() { }

        public 
        Task(
            Guid    id,
            Guid    initialIdeaId,
            string  title)
        {
            ApplyEvent(new TaskCreatedEvent {
                EventSourceId = id, // TODO: ApplyEvent could set this
                Version       = 1,
                InitialIdeaId = initialIdeaId,
                Title         = title
            });
        }

        public
        void 
        Apply(
            TaskCreatedEvent taskCreatedEvent)
        {
            Id            = taskCreatedEvent.EventSourceId;
            InitialIdeaId = taskCreatedEvent.InitialIdeaId;
            Title         = taskCreatedEvent.Title;
        }

        public
        void
        Apply(
            TaskVotedCompleteEvent taskVotedCompleteEvent)
        {
            CompletedVoteCount++;
        }

        public 
        void 
        VoteCompleted()
        {
            ApplyEvent(new TaskVotedCompleteEvent {
                EventSourceId = Id // TODO: ApplyEvent could set this
            });
        }
    }
}
