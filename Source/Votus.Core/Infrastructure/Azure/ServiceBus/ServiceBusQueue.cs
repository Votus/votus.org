using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Queuing;
using RetryPolicy = Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    class ServiceBusQueue : IQueue
    {
        #region Variables & Properties

        private readonly List<QueueClient>          _queueClients;
        private Func<DynamicMessageEnvelope, Task>  _asyncHandler;

        [Inject]
        public ILog Log { get; set; }

        public RetryPolicy RetryPolicy { get; private set; }

        #endregion

        #region Constructors

        public
        ServiceBusQueue(
            string connectionString,
            string queueName,
            int    numberOfConcurrentClients = 2)
        {
            _queueClients = new List<QueueClient>();

            for (var i = 0; i < numberOfConcurrentClients; i++)
                _queueClients.Add(CreateQueueClient(connectionString, queueName));

            RetryPolicy = new RetryPolicy(
                new ServiceBusQueueErrorDetectionStrategy(
                    NamespaceManager.CreateFromConnectionString(connectionString),
                    queueName
                ),
                new ExponentialBackoff()
            );
        }

        private 
        static
        QueueClient 
        CreateQueueClient(
            string connectionString, 
            string queueName)
        {
            var client = QueueClient.CreateFromConnectionString(
                connectionString,
                queueName
            );

            client.PrefetchCount                  = 25;
            client.MessagingFactory.PrefetchCount = 25;

            return client;
        }

        #endregion

        #region IQueue Members

        public
        void
        BeginReceivingMessages(
            Func<DynamicMessageEnvelope, Task> asyncHandler)
        {
            _asyncHandler = asyncHandler;

            // Executing this in a new Task so it can 
            // retry without blocking the website from starting
            Task.Run(() => 
                RetryPolicy.ExecuteAction(() => {
                    foreach (var queueClient in _queueClients)
                        queueClient.OnMessageAsync(HandleMessageAsync);
                })
            );
        }

        private 
        async Task 
        HandleMessageAsync(
            BrokeredMessage message)
        {
            var stopwatch = Stopwatch.StartNew();

            Log.Verbose(
                "Starting to process {0} queue message {1}.",
                message.Label,
                message.MessageId
            );

            try
            {
                await _asyncHandler(ConvertToEnvelope(message));
            }
            catch (Exception exception)
            {
                Log.Error(
                    "Error processing {0} queue message {1} after {2}ms: {3}",
                    message.Label,
                    message.MessageId,
                    stopwatch.ElapsedMilliseconds,
                    exception
                );

                throw;
            }

            Log.Verbose(
                "Processed {0} queue message {1} in {2}ms",
                message.Label,
                message.MessageId,
                stopwatch.ElapsedMilliseconds
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

            return _queueClients
                .First()
                .SendAsync(brokeredMessage);
        }

        #endregion

        #region Methods

        private
        static
        DynamicMessageEnvelope
        ConvertToEnvelope(
            BrokeredMessage message)
        {
            return message.GetBody<DynamicMessageEnvelope>();
        }

        #endregion
    }
}
