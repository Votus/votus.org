using System;

namespace Votus.Core.Infrastructure.Messaging.Eventing
{
    /// <summary>
    /// The base class for all events.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// Gets or sets an id that uniquely identifies the source of the event.
        /// </summary>
        public Guid EventSourceId { get; set; }

        /// <summary>
        /// Gets or sets the date/time when the event occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        protected 
        Event()
        {
            Timestamp = DateTimeOffset.UtcNow;
        }

        protected 
        bool 
        Equals(
            Event other)
        {
            return EventSourceId.Equals(other.EventSourceId) && Timestamp.Equals(other.Timestamp);
        }

        public 
        override 
        int 
        GetHashCode()
        {
            unchecked
            {
                return (EventSourceId.GetHashCode()*397) ^ Timestamp.GetHashCode();
            }
        }

        public 
        override 
        bool 
        Equals(
            object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((Event) obj);
        }
    }
}
