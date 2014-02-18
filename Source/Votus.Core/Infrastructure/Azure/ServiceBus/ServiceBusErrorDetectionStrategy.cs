using System;
using System.IdentityModel.Tokens;
using System.Net;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    public abstract class ServiceBusErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public NamespaceManager NamespaceManager { get; private set; }

        protected 
        ServiceBusErrorDetectionStrategy(
            NamespaceManager namespaceManager)
        {
            NamespaceManager = namespaceManager;
        }

        public 
        bool 
        IsTransient(
            Exception ex)
        {
            var messagingEntityNotFoundException = ex as MessagingEntityNotFoundException;
            if (messagingEntityNotFoundException != null)
                return MessagingEntityNotFoundExceptionIsTransient(messagingEntityNotFoundException);

            var unauthorizedAccessException = ex as UnauthorizedAccessException;
            if (unauthorizedAccessException != null)
                return UnauthorizedAccessExceptionIsTransient(unauthorizedAccessException);

            return false;
        }

        #region MessagingEntityNotFoundException Handlers

        private
            bool
            MessagingEntityNotFoundExceptionIsTransient(
            MessagingEntityNotFoundException messagingEntityNotFoundException)
        {
            // All MessagingEntityNotFoundException are resolvable...
            return ResolveError();
        }


        protected 
        abstract 
        bool
        ResolveError();

        #endregion

        #region UnauthorizedAccessException Handlers

        private
            static
            bool 
            UnauthorizedAccessExceptionIsTransient(
            UnauthorizedAccessException unauthorizedAccessException)
        {
            var exception = unauthorizedAccessException.InnerException as SecurityTokenException;

            return exception != null && SecurityTokenExceptionIsTransient(exception);
        }

        private
            static 
            bool 
            SecurityTokenExceptionIsTransient(
            SecurityTokenException securityTokenException)
        {
            var exception = securityTokenException.InnerException as WebException;

            return exception != null && WebExceptionIsTransient(exception);
        }

        private 
            static 
            bool 
            WebExceptionIsTransient(
            WebException webException)
        {
            return webException.Response == null;
        }

        #endregion
    }
}
