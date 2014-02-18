using Ninject;
using System;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Infrastructure.Data
{
    public class AggregateRootRepository : IVersioningRepository<AggregateRoot>
    {
        [Inject]
        public EventStore EventStore { get; set; }

        public 
        async Task<T>
        GetAsync<T>(Guid id)
            where T: AggregateRoot, new()
        {
            // Create a new instance of the aggregate root.
            var ar = new T();

            // Get existing events for the AR.
            var events = await EventStore.GetAsync(id);

            // Load the events.
            ar.LoadHistory(events);

            // Create an instance of it and load the history.
            return ar;
        }

        public
        async Task
        SaveAsync(
            AggregateRoot   aggregateRoot, 
            int             expectedVersion)
        {
            await EventStore.SaveAsync(
                aggregateRoot.Id,
                aggregateRoot.GetUncommittedEvents(),
                expectedVersion
            );
        }
    }
}
