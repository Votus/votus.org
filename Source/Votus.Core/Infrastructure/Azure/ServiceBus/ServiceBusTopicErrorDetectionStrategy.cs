using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
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
        private readonly string _subscriptionFilter;

        #endregion

        #region Constructors

        public
        ServiceBusTopicErrorDetectionStrategy(
            NamespaceManager    namespaceManager,
            string              topicName,
            string              subscriptionName,
            string              subscriptionFilter
            ) : base(namespaceManager)
        {
            _topicName          = topicName;
            _subscriptionName   = subscriptionName;
            _subscriptionFilter = subscriptionFilter;
        }

        #endregion

        protected 
        override 
        bool 
        ResolveError()
        {
            var resolved = false;

            if (!NamespaceManager.TopicExists(_topicName)) {
                try
                {
                    NamespaceManager.CreateTopic(_topicName);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    // Do nothing...

                    // Sometimes a race condition can occur between threads / VMs
                    // when the topic doesn't exist all try to create it at once...
                }
                catch (MessagingException messagingException)
                {
                    var webException = messagingException.InnerException as WebException;

                    if (webException == null) 
                        throw;

                    if (((HttpWebResponse) webException.Response).StatusCode != HttpStatusCode.Conflict) 
                        throw;
                }

                resolved = true;
            }

            if (!NamespaceManager.SubscriptionExists(_topicName, _subscriptionName))
            {
                var subscriptionSqlFilter = new SqlFilter(_subscriptionFilter);

                subscriptionSqlFilter.Validate();


                try
                {
                    NamespaceManager.CreateSubscription(
                        _topicName,
                        _subscriptionName,
                        subscriptionSqlFilter
                    );
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    // Do nothing...

                    // Sometimes a race condition can occur between threads / VMs
                    // when the topic doesn't exist all try to create it at once...
                }

                resolved = true;
            }

            return resolved;
        }
    }
}