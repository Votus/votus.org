using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Ninject;
using System;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Queuing;
using RetryPolicy = Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    class ServiceBusQueue : IQueue
    {
        #region Variables & Properties

        private readonly QueueClient _queueClient;

        [Inject]
        public ILog Log { get; set; }

        public RetryPolicy RetryPolicy { get; private set; }

        #endregion

        #region Constructors

        public
        ServiceBusQueue(
            string connectionString,
            string queueName)
        {
            _queueClient = QueueClient.CreateFromConnectionString(
                connectionString, 
                queueName
            );

            _queueClient.PrefetchCount = 25;

            RetryPolicy = new RetryPolicy(
                new ServiceBusQueueErrorDetectionStrategy(
                    NamespaceManager.CreateFromConnectionString(connectionString),
                    queueName
                ),
                new ExponentialBackoff()
            );
        }

        #endregion

        #region IQueue Members

        public
        void
        BeginReceivingMessages(
            Func<DynamicMessageEnvelope, Task> asyncHandler)
        {
            Task.Run(() => 
                RetryPolicy.ExecuteAction(() =>
                    // Executing this in a new Task so it can 
                    // retry without blocking the website from starting
                    _queueClient.OnMessageAsync(m => asyncHandler(ConvertToEnvelope(m)))
                )
            );
        }

        public
        Task
        EnqueueAsync(
            string  messageId, 
            object  message)
        {
            var brokeredMessage = new BrokeredMessage(message) {
                MessageId = messageId
            };

            return _queueClient.SendAsync(brokeredMessage);
        }

        #endregion

        #region Methods

        private
        static
        DynamicMessageEnvelope
        ConvertToEnvelope(BrokeredMessage message)
        {
            return message.GetBody<DynamicMessageEnvelope>();
        }

        #endregion
    }
}
