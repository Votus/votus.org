using Microsoft.ServiceBus.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    public class AzureEventBus : IEventBus
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
        Task 
        PublishAsync(
            IEnumerable<AggregateRootEvent> events)
        {
            // Convert the events to Azure Service Bus message type
            var brokeredMessages = events.Select(@event => 
                new BrokeredMessage(@event) {
                    Label = @event.GetType().Name
                }
            );

            // Send all the messages at once
            return _topicClient.SendBatchAsync(brokeredMessages);
        }

        #endregion
    }
}