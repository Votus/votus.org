using System;
using System.Collections.Generic;
using Votus.Core.Infrastructure.Reflection;

namespace Votus.Core.Infrastructure.EventSourcing
{
    public class AggregateRoot
    {
        #region Constants, Variables & Properties

        private
        readonly
        IList<AggregateRootEvent>
        _uncommittedEvents = new List<AggregateRootEvent>();

        public Guid Id      { get; set; }
        public int  Version { get; set; }

        #endregion

        #region Methods

        public
        IEnumerable<AggregateRootEvent>
        GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }

        public
        void
        LoadHistory(IEnumerable<AggregateRootEvent> history)
        {
            foreach (var aggregateRootEvent in history)
                ApplyEvent(aggregateRootEvent, false);
        }

        public
        void
        ApplyEvent(
            AggregateRootEvent  @event, 
            bool                isNew = true)
        {
            // Call the method on the inheriting class that handles this specific type of event.
            this.AsDynamic().Apply(@event);

            if (isNew) {
                _uncommittedEvents.Add(@event);
                Version++;
                @event.Version = Version;
            }
            else {
                Version = @event.Version;
            }
        }
        
        #endregion
    }
}
