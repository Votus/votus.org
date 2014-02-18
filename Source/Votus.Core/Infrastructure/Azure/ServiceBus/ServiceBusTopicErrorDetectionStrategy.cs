using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.IdentityModel.Tokens;
using System.Net;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    /// <summary>
    /// This class can handle some "non-transient" errors as transient, 
    /// such as non-existent queues since they can be created on the fly when needed.
    /// </summary>
    public class ServiceBusTopicErrorDetectionStrategy : ServiceBusErrorDetectionStrategy
    {
        #region Variables

        private readonly string _topicName;
        private readonly string _subscriptionName;

        #endregion

        #region Constructors

        public
        ServiceBusTopicErrorDetectionStrategy(
            NamespaceManager    namespaceManager,
            string              topicName,
            string              subscriptionName
            ) : base(namespaceManager)
        {
            _topicName        = topicName;
            _subscriptionName = subscriptionName;
        }

        #endregion

        protected 
        override 
        bool 
        ResolveError()
        {
            var resolved = false;

            if (!NamespaceManager.TopicExists(_topicName)) {
                NamespaceManager.CreateTopic(_topicName);
                resolved = true;
            }

            if (!NamespaceManager.SubscriptionExists(_topicName, _subscriptionName)) {
                NamespaceManager.CreateSubscription(_topicName, _subscriptionName);
                resolved = true;
            }

            return resolved;
        }
    }
}
