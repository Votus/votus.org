using Microsoft.ServiceBus;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    class ServiceBusQueueErrorDetectionStrategy : ServiceBusErrorDetectionStrategy
    {
        private readonly string _queueName;

        public ServiceBusQueueErrorDetectionStrategy(
            NamespaceManager namespaceManager,
            string           queueName) : base(namespaceManager)
        {
            _queueName = queueName;
        }

        protected 
        override
        bool
        ResolveError()
        {
            if (NamespaceManager.QueueExists(_queueName)) return false;
            
            NamespaceManager.CreateQueue(_queueName);

            return true;
        }
    }
}