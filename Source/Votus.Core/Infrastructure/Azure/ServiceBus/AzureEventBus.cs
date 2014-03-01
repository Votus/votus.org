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
using Votus.Core.Infrastructure.Serialization;
using RetryPolicy = Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    public class AzureEventBus : IEventBus
    {
        #region Constants, Variables & Properties

        public const string AggregateRootEventTopicName = "AggregateRootEvents";

        private readonly string         _connectionString;
        private readonly TopicClient    _topicClient;

        public Dictionary<Type, EventManager> EventManagers { get; private set; }

        [Inject] public ILog        Log         { get; set; }
        [Inject] public ISerializer Serializer  { get; set; }
        
        #endregion

        #region Constructors

        public AzureEventBus()
        {
            EventManagers = new Dictionary<Type, EventManager>();
        }

        public
        AzureEventBus(
            string connectionString)
            : this()
        {
            _topicClient = TopicClient.CreateFromConnectionString(
                connectionString:   connectionString,
                path:               AggregateRootEventTopicName
            );

            _connectionString = connectionString;
        }

        #endregion

        #region IEventBus Members

        public 
        void
        Subscribe<TEvent>(
            Func<TEvent, Task> handlerAsync)
        {
            var eventManager = new EventManager<TEvent>(
                _connectionString, 
                handlerAsync
            ) { Log = Log };

            EventManagers.Add(
                typeof(TEvent), 
                eventManager
            );
        }

        public
        void
        BeginProcessingEvents()
        {
            foreach (var eventManager in EventManagers.Values)
            {
                var manager = eventManager;

                Task.Run(() => manager.BeginProcessingEvents());
            }
        }

        public 
        Task 
        PublishAsync(
            IEnumerable<AggregateRootEvent> events)
        {
            // Convert the events to Azure Service Bus message type
            var brokeredMessages = events.Select(@event => new BrokeredMessage(@event) { Label = @event.GetType().Name });

            // Send all the messages at once
            return _topicClient.SendBatchAsync(brokeredMessages);
        }

        #endregion
    }

    public abstract class EventManager
    {
        protected RetryPolicy         _retryPolicy;
        protected SubscriptionClient  _subscriptionClient;

        protected
        EventManager(
            string serviceBusConnectionString,
            string eventName,
            string eventHandlerName)
        {
            _subscriptionClient = SubscriptionClient.CreateFromConnectionString(
                 connectionString:  serviceBusConnectionString,
                 topicPath:         AzureEventBus.AggregateRootEventTopicName,
                 name:              eventHandlerName
             );

            _subscriptionClient.PrefetchCount                  = 25;
            _subscriptionClient.MessagingFactory.PrefetchCount = 25;

            var subscriptionFilterByLabel = string.Format(
                "sys.Label='{0}'", 
                eventName
            );

            _retryPolicy = new RetryPolicy(
                new ServiceBusTopicErrorDetectionStrategy(
                    namespaceManager:   NamespaceManager.CreateFromConnectionString(serviceBusConnectionString),
                    topicName:          AzureEventBus.AggregateRootEventTopicName,
                    subscriptionName:   eventHandlerName,
                    subscriptionFilter: subscriptionFilterByLabel
                ),
                new ExponentialBackoff()
            );
        }

        public
        abstract
        void
        BeginProcessingEvents();
    }

    public class EventManager<TEvent> : EventManager
    {
        private readonly Func<TEvent, Task> _asyncEventHandler;

        public ILog Log { get; set; }

        public EventManager(
            string              serviceBusConnectionString,
            Func<TEvent, Task>  asyncEventHandler)
            : base(
                serviceBusConnectionString, 
                typeof(TEvent).Name, 
                GetEventHandlerName(asyncEventHandler))
        {
            _asyncEventHandler = asyncEventHandler;
        }

        private 
        static 
        string 
        GetEventHandlerName(
            Func<TEvent, Task> asyncEventHandler)
        {
            var handlerName = asyncEventHandler
                .Method
                .DeclaringType
                .Name;

            return string.Format(
                "{0}-{1}", 
                handlerName, 
                typeof (TEvent).Name
            );
        }


        public
        override 
        void 
        BeginProcessingEvents()
        {
            _retryPolicy.ExecuteAction(() =>
                _subscriptionClient.OnMessageAsync(HandleMessageAsync)
            );
        }

        private
        async Task
        HandleMessageAsync(
            BrokeredMessage message)
        {
            var stopwatch = Stopwatch.StartNew();

            var payload = message.GetBody<TEvent>();

            await _asyncEventHandler(payload);

            Log.Verbose(
                "Processed {0} event message {1} in {2}ms",
                typeof(TEvent).Name,
                message.MessageId,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}