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
    }
}
