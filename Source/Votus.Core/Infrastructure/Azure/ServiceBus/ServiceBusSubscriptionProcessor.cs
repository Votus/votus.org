using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Ninject;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Infrastructure.Logging;
using RetryPolicy = Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    public class ServiceBusSubscriptionProcessor<TEvent> : IEventProcessor
    {
        protected RetryPolicy         _retryPolicy;
        protected SubscriptionClient  _subscriptionClient;

        [Inject] public ILog Log { get; set; }

        public Func<TEvent, Task> Handler { get; set; }

        protected
        ServiceBusSubscriptionProcessor(
            string serviceBusConnectionString,
            string topicPath,
            string eventName,
            string subscriptionName)
        {
            _subscriptionClient = SubscriptionClient.CreateFromConnectionString(
                 connectionString:  serviceBusConnectionString,
                 topicPath:         topicPath,
                 name:              subscriptionName
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
                    topicName:          topicPath,
                    subscriptionName:   subscriptionName,
                    subscriptionFilter: subscriptionFilterByLabel
                ),
                new ExponentialBackoff()
            );
        }

        public ServiceBusSubscriptionProcessor(
            string              serviceBusConnectionString,
            string              topicPath,
            Func<TEvent, Task>  asyncEventHandler)
            : this(
                serviceBusConnectionString, 
                topicPath,
                typeof(TEvent).Name, 
                asyncEventHandler.Method.DeclaringType.Name)
        {
            Handler = asyncEventHandler;
        }

        public
        Task
        ProcessEventsAsync()
        {
            return Task.Run(() => 
                _retryPolicy.ExecuteAction(() =>
                    _subscriptionClient.OnMessageAsync(HandleMessageAsync)
                )
            );
        }

        private
        async Task
        HandleMessageAsync(
            BrokeredMessage message)
        {
            var stopwatch = Stopwatch.StartNew();

            var payload = message.GetBody<TEvent>();

            await Handler(payload);

            Log.Verbose(
                "Processed {0} event message {1} in {2}ms",
                typeof(TEvent).Name,
                message.MessageId,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}
