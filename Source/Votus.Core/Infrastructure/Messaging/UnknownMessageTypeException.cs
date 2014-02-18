using System;

namespace Votus.Core.Infrastructure.Messaging
{
    public class UnknownMessageTypeException : Exception
    {
        public UnknownMessageTypeException(string unknownType)
            : base(string.Format("No handler is registered for message type '{0}'.", unknownType))
        {
        }
    }
}
