using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Votus.Core.Infrastructure.Caching;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Domain
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

    public class TaskCreatedEvent : AggregateRootEvent
    {
        public string   Title           { get; set; }
        public Guid     InitialIdeaId   { get; set; }
    }

    public class TaskVotedCompleteEvent : AggregateRootEvent
    {
        // TODO: Should not be needed, remove when the IdeaTasksViewManager stops caching full Task views.
        public Guid IdeaId { get; set; }
        public string VoterId { get; set; }
    }

    [OncePer("You have already voted that this task is completed.")]
    public class VoteTaskCompletedCommand
    {
        [Required] public Guid      TaskId   { get; set; } // TODO: Validate that TaskId exists
        [Required] public string    VoterId  { get; set; }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", TaskId, VoterId);
        }
    }

    // TODO: Find a better location for this class.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OncePerAttribute : ValidationAttribute
    {
        //[Inject] Couldn't get property injection to work.
        public ICache ValueHashCodeRepository { get; set; }

        public 
        OncePerAttribute(
            string errorMessage,
            ICache valueHashCodeRepository) : base(errorMessage)
        {
            ValueHashCodeRepository = valueHashCodeRepository;
        }

        public OncePerAttribute(
            string errorMessage) : this(
                errorMessage, 
                DependencyResolver.Current.GetService<ICache>()) // TODO: Remove once property injection is working
        {
        }

        public 
        override 
        bool 
        IsValid(
            object value)
        {
            return !ValueExists(value);
        }

        private 
        bool 
        ValueExists(
            object value)
        {
            var key = string.Format(
                "{0}:{1}", 
                value.GetType(), 
                value.GetHashCode()
            );

            // This call is intentionally synchronous; expecting to hit a fast local in memory cache most of the time.
            return ValueHashCodeRepository.Contains(key);
        }
    }
}
