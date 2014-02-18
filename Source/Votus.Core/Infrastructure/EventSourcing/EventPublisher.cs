using System.Threading.Tasks;
using Ninject;

namespace Votus.Core.Infrastructure.EventSourcing
{
    public class EventPublisher
    {
        [Inject]
        public IEventBus EventBus { get; set; }

        public
        virtual 
        async Task 
        PublishNewEventsAsync(AggregateRoot aggregateRoot)
        {
            await EventBus.PublishAsync(aggregateRoot.GetUncommittedEvents());
        }
    }
}
