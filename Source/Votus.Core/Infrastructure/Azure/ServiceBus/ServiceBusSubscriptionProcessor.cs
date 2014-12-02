using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Ninject;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Logging;
using RetryPolicy = Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    public class ServiceBusSubscriptionProcessor<TEvent> : Votus.Core.Infrastructure.EventSourcing.IEventProcessor
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
                GetSubscriptionName(typeof(TEvent).Name, asyncEventHandler))
        {
            Handler = asyncEventHandler;
        }

        public
        static
        string 
        GetSubscriptionName(
            string              eventName,
            Func<TEvent, Task>  asyncEventHandler)
        {
            var handlerName = asyncEventHandler
                .Method
                .DeclaringType
                .Name;

            var name = string.Format(
                "{0}-{1}", 
                handlerName, 
                eventName
            );

            if (name.Length > 50)
                name = name.Substring(0, 50);

            return name;
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

            var eventName = typeof (TEvent).Name;

            try
            {
                var payload = message.GetBody<TEvent>();
                await Handler(payload);
            }
            catch (Exception ex)
            {
                Log.Error(
                    "{0}: Error processing {1} event message {2} in {3}ms: {4}",
                    Handler.Method.DeclaringType.Name,
                    eventName,
                    message.MessageId,
                    stopwatch.ElapsedMilliseconds,
                    ex
                );

                throw;
            }

            Log.Verbose(
                "{0}: Processed {1} event message {2} in {3}ms",
                Handler.Method.DeclaringType.Name,
                eventName,
                message.MessageId,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}
