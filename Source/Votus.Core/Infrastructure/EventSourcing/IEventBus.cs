using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.EventSourcing
{
    public interface IEventBus
    {
        void BeginProcessingEvents();
        void Subscribe<T>(Func<T, Task> handlerAsync);
        Task PublishAsync(IEnumerable<AggregateRootEvent> events);
    }
}
