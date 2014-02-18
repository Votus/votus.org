using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.EventSourcing
{
    public class EventStore
    {
        [Inject] public ISerializer             Serializer      { get; set; }
        [Inject] public IEventBus               EventBus        { get; set; }
        [Inject] public IPartitionedRepository  EventRepository { get; set; }

        public
        virtual
        async Task
        SaveAsync(
            Guid                            id,
            IEnumerable<AggregateRootEvent> events,
            int                             expectedVersion)
        {
            var eventArray  = events.ToArray();
            var nextVersion = expectedVersion + 1;

            // TODO: Might need to check the latest store to verify expectedVersion == latestVersion
            // Example: https://github.com/gregoryyoung/m-r/blob/master/SimpleCQRS/EventStore.cs

            await EventRepository.InsertBatchAsync(
               partitionKey:    id,
               rowKeyGetter:    e => nextVersion++,
               entities:        eventArray.Select(ConvertToEnvelope)
            );

            await EventBus.PublishAsync(eventArray);
        }

        private 
        EventEnvelope
        ConvertToEnvelope(
            AggregateRootEvent aggregateRootEvent)
        {
            return new EventEnvelope {
                PayloadType = aggregateRootEvent.GetType().FullName,
                Payload     = Serializer.Serialize(aggregateRootEvent)
            };
        }

        private
        AggregateRootEvent
        ConvertToEvent(
            EventEnvelope envelope)
        {
            return (AggregateRootEvent)Serializer.Deserialize(
                envelope.Payload, 
                envelope.PayloadType
            );
        }

        public 
        async Task
        RepublishAllEventsAsync()
        {
            var envelopes = await EventRepository.GetAllAsync<EventEnvelope>();

            var events = envelopes.Select(ConvertToEvent);

            await EventBus.PublishAsync(events);
        }

        public 
        virtual 
        async Task<IEnumerable<AggregateRootEvent>>
        GetAsync(Guid id)
        {
            var envelopes = await EventRepository
                .GetPartitionAsync<EventEnvelope>(id);

            return envelopes.Select(ConvertToEvent);
        }
    }

    public class EventEnvelope
    {
        public string PayloadType   { get; set; }
        public string Payload       { get; set; }
    }
}
