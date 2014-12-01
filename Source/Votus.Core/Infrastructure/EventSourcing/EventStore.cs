using System.Diagnostics;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.EventSourcing
{
    public class EventStore
    {
        private static readonly Dictionary<string, Type> _eventNameTypeMap;

        [Inject] public ILog                    Log             { get; set; }
        [Inject] public ISerializer             Serializer      { get; set; }
        [Inject] public IEventBus               EventBus        { get; set; }
        [Inject] public IPartitionedRepository  EventRepository { get; set; }

        static EventStore()
        {
            _eventNameTypeMap = CreateEventNameTypeMap();
        }

        private 
        static 
        Dictionary<string, Type> 
        CreateEventNameTypeMap()
        {
            var baseType = typeof (AggregateRootEvent);

            var votusAssemblies = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(assembly => 
                    assembly.FullName.Contains("Votus"));

            return votusAssemblies
                .Select(assembly => 
                    assembly
                        .GetTypes()
                        .Where(type => 
                            type.IsClass     && 
                            type.IsPublic    &&
                            !type.IsAbstract &&
                            type.IsSubclassOf(baseType)))
                .SelectMany(eventTypes => eventTypes)
                .ToDictionary(eventType => eventType.Name);
        }

        public
        virtual
        async Task
        SaveAsync(
            Guid                            id,
            IEnumerable<AggregateRootEvent> events,
            int                             expectedVersion)
        {
            var eventArray  = events.ToArray();

            if (eventArray.Length == 0) return;

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

        public
        EventEnvelope
        ConvertToEnvelope(
            AggregateRootEvent aggregateRootEvent)
        {
            return new EventEnvelope {
                PayloadType = aggregateRootEvent.GetType().Name,
                Payload     = Serializer.Serialize(aggregateRootEvent)
            };
        }

        public 
        AggregateRootEvent
        ConvertToEvent(
            EventEnvelope envelope)
        {
            var eventName = envelope
                .PayloadType
                .Split('.')
                .Last();

            return (AggregateRootEvent)Serializer.Deserialize(
                envelope.Payload, 
                _eventNameTypeMap[eventName]
            );
        }

        public 
        async Task
        RepublishAllEventsAsync()
        {
            Log.Info("Republishing all events from the Event Store...");

            var stopwatch = Stopwatch.StartNew();

            // TODO: Will need to implement this different when dealing with lots of events.

            var envelopes = await EventRepository.GetAllAsync<EventEnvelope>();

            var events = envelopes
                .Select(ConvertToEvent)
                .OrderBy(e => e.Timestamp)
                .ToArray();

            await EventBus.PublishAsync(events);

            Log.Info("Published {0} events in {1}ms.", events.Length, stopwatch.ElapsedMilliseconds);
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
        public string           PayloadType { get; set; }
        public string           Payload     { get; set; }
    }
}
