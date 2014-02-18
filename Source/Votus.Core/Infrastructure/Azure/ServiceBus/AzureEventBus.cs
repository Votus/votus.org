using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Messaging;
using Votus.Core.Infrastructure.Serialization;
using RetryPolicy = Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    public class AzureEventBus : IEventBus
    {
        #region Constants, Variables & Properties

        private const string DefaultEventTopicName   = "AggregateRootEvents";
        private const string DefaultSubscriptionName = "AzureEventBus";

        private readonly TopicClient         _topicClient;
        private readonly SubscriptionClient  _subscriptionClient;
        
        [Inject] public ILog        Log         { get; set; }
        [Inject] public ISerializer Serializer  { get; set; }
        
        public RetryPolicy      RetryPolicy { get; private set; }
        public HandlerManager   Handlers    { get; private set; }

        #endregion

        #region Constructors

        public AzureEventBus()
        {
            Handlers = new HandlerManager();
        }

        public
        AzureEventBus(
            string connectionString)
            : this()
        {
            _subscriptionClient = SubscriptionClient.CreateFromConnectionString(
                connectionString:   connectionString,
                topicPath:          DefaultEventTopicName,
                name:               DefaultSubscriptionName
            );

            _subscriptionClient.PrefetchCount                  = 25;
            _subscriptionClient.MessagingFactory.PrefetchCount = 25;

            _topicClient = TopicClient.CreateFromConnectionString(
                connectionString:   connectionString,
                path:               DefaultEventTopicName
            );

            RetryPolicy = new RetryPolicy(
                new ServiceBusTopicErrorDetectionStrategy(
                    NamespaceManager.CreateFromConnectionString(connectionString),
                    DefaultEventTopicName,
                    DefaultSubscriptionName
                ),
                new ExponentialBackoff()
            );
        }

        #endregion

        #region IEventBus Members

        public 
        void
        Subscribe<TEvent>(
            Func<TEvent, Task> handlerAsync)
        {
            Handlers.Add(handlerAsync);
        }

        public
        void
        BeginProcessingEvents()
        {
            Task.Run(() =>
                RetryPolicy.ExecuteAction(() =>
                    // Executing this in a new Task so it can 
                    // retry without blocking the website from starting
                    _subscriptionClient.OnMessageAsync(HandleMessageAsync)
                )
            );
        }

        public 
        async Task 
        PublishAsync(IEnumerable<AggregateRootEvent> events)
        {
            foreach (var message in events.Select(ConvertToBrokeredMessage))
                await _topicClient.SendAsync(message);
        }

        #endregion

        #region Methods

        private
        async Task
        HandleMessageAsync(
            BrokeredMessage message)
        {
            var stopwatch = Stopwatch.StartNew();

            var envelope     = message.GetBody<DynamicMessageEnvelope>();
            var payloadType  = Handlers.GetTypeForName(envelope.PayloadType);
            var handlerAsync = Handlers.Get(envelope.PayloadType);
            var payload      = Serializer.Deserialize(envelope.Payload, payloadType);

            await handlerAsync(payload);

            Log.Verbose(
                "Processed {0} event message {1} in {2}ms",
                envelope.PayloadType,
                message.MessageId,
                stopwatch.ElapsedMilliseconds
            );
        }

        public 
        BrokeredMessage 
        ConvertToBrokeredMessage(
            AggregateRootEvent aggregateRootEvent)
        {
            return new BrokeredMessage(
                ConvertToEnvelope(aggregateRootEvent)
            );
        }

        public 
        DynamicMessageEnvelope 
        ConvertToEnvelope(
            AggregateRootEvent aggregateRootEvent)
        {
            var payloadTypeName = aggregateRootEvent.GetType().Name;
            var payload         = Serializer.Serialize(aggregateRootEvent);

            return new DynamicMessageEnvelope {
                Payload     = payload,
                PayloadType = payloadTypeName
            };
        }

        #endregion
    }
}