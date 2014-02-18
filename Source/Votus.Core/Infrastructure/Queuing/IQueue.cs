using System;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Azure.ServiceBus;

namespace Votus.Core.Infrastructure.Queuing
{
    public interface IQueue
    {
        Task 
        EnqueueAsync(
            string messageId, 
            object message
        );

        void 
        BeginReceivingMessages(
            Func<DynamicMessageEnvelope, Task> asyncHandler
        );
    }
}