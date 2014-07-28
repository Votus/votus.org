using System;
using System.Collections.Generic;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Domain.Tasks
{
    public class Task : AggregateRoot
    {
        public string       Title           { get; set; }
        public Guid         InitialIdeaId   { get; set; }
        public ISet<string> CompletedVotes  { get; set; }

        public Task()
        {
            CompletedVotes = new HashSet<string>();
        }

        public 
        Task(
            Guid    id,
            Guid    initialIdeaId,
            string  title) : this()
        {
            ApplyEvent(new TaskCreatedEvent {
                EventSourceId = id, // TODO: ApplyEvent could set this
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
            CompletedVotes.Add(taskVotedCompleteEvent.VoterId);
        }

        public 
        void
        VoteCompleted(
            string voterId)
        {
            ApplyEvent(new TaskVotedCompleteEvent {
                EventSourceId = Id,             // TODO: ApplyEvent could set this
                IdeaId        = InitialIdeaId,
                VoterId       = voterId
            });
        }
    }
}
