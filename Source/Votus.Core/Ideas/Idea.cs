using System;
using System.Collections.Generic;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Ideas
{
    public class Idea : AggregateRoot
    {
        private readonly List<Guid> _goalIds = new List<Guid>();
        private readonly List<Guid> _taskIds = new List<Guid>();

        public string                       Title   { get; set; }
        public string                       Tag     { get; set; }

        public IReadOnlyCollection<Guid> Goals { get { return _goalIds.AsReadOnly(); } }
        public IReadOnlyCollection<Guid> Tasks { get { return _taskIds.AsReadOnly(); } }

        public Idea() {}

        public 
        Idea(
            Guid    id, 
            string  title,
            string  tag = null)
        {
            ApplyEvent(new IdeaCreatedEvent {
                EventSourceId = id,
                Version       = 1,
                Title         = title,
                Tag           = tag
            });
        }

        public 
        void 
        AddGoal(
            Guid goalId)
        {
            ApplyEvent(new GoalAddedToIdeaEvent {
                EventSourceId = Id,
                GoalId        = goalId
            });
        }

        public 
        void 
        AddTask(
            Guid taskId)
        {
            ApplyEvent(new TaskAddedToIdeaEvent {
                EventSourceId = Id,
                TaskId        = taskId
            });            
        }

        public 
        void 
        Apply(
            IdeaCreatedEvent ideaCreatedEvent)
        {
            Id    = ideaCreatedEvent.EventSourceId;
            Title = ideaCreatedEvent.Title;
            Tag   = ideaCreatedEvent.Tag;
        }

        public
        void
        Apply(
            GoalAddedToIdeaEvent goalAddedToIdeaEvent)
        {
            _goalIds.Add(goalAddedToIdeaEvent.GoalId);
        }

        public
        void
        Apply(
            TaskAddedToIdeaEvent taskAddedToIdeaEvent)
        {
            _taskIds.Add(taskAddedToIdeaEvent.TaskId);
        }
    }
}
