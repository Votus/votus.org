using System.Collections.Generic;
using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.EventSourcing
{
    public interface IEventBus
    {
        Task PublishAsync(IEnumerable<AggregateRootEvent> events);
    }
}
