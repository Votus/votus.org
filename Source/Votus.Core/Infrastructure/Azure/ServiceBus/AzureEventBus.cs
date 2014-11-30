using Microsoft.ServiceBus.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    public class AzureEventBus : IEventBus // TODO: Refactor, no such thing as the AzureEventBus 
    {
        #region Constants, Variables & Properties

        private readonly TopicClient _topicClient;

        #endregion

        #region Constructors

        public
        AzureEventBus(
            string connectionString,
            string topicPath)
        {
            _topicClient = TopicClient.CreateFromConnectionString(
                connectionString:   connectionString,
                path:               topicPath
            );
        }

        #endregion

        #region IEventBus Members

        public 
        async Task 
        PublishAsync(
            IEnumerable<AggregateRootEvent> events)
        {
            const int MaxEventsPerBatch = 50;

            var batchCount          = 1;
            var aggregateRootEvents = events.ToList();
            var eventsToSend        = aggregateRootEvents.Take(MaxEventsPerBatch).ToList();

            while (eventsToSend.Any())
            {
                var brokeredMessages = eventsToSend.Select(@event => 
                    new BrokeredMessage(@event) {
                        Label = @event.GetType().Name
                    }
                );

                await _topicClient.SendBatchAsync(brokeredMessages);

                eventsToSend = aggregateRootEvents
                    .Skip(MaxEventsPerBatch * batchCount)
                    .Take(MaxEventsPerBatch)
                    .ToList();

                batchCount++;
            }
        }

        #endregion
    }
}