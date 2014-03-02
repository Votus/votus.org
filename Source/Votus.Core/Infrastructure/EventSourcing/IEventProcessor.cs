using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.EventSourcing
{
    public interface IEventProcessor
    {
        Task ProcessEventsAsync();
    }
}
