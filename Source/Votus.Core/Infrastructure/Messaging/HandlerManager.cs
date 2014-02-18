using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Reflection;

namespace Votus.Core.Infrastructure.Messaging
{
    /// <summary>
    /// The HandlerManager assists in mapping keys to handlers while maintaining strong-typing via generics.
    /// </summary>
    public class HandlerManager
    {
        private
        readonly 
        IDictionary<Type, Func<object, Task>> 
        _handlers = new Dictionary<Type, Func<object, Task>>();

        /// <summary>
        /// Register an asynchronous handler.
        /// </summary>
        /// <typeparam name="TMessage">The message type the handler is for.</typeparam>
        /// <param name="handlerAsync">The handler for instances of the specified type.</param>
        public 
        void 
        Add<TMessage>(
            Func<TMessage, Task> handlerAsync)
        {
            var adjustedDelegate = DelegateAdjuster.CastArgument<object, TMessage>(x =>
                handlerAsync(x)
            );

            _handlers.Add(
                typeof(TMessage),
                adjustedDelegate
            );
        }

        /// <summary>
        /// Gets the registered Type based on the Type's Name.
        /// </summary>
        /// <param name="messageName">Specifies the name of the message type.</param>
        /// <returns>Returns the Type of the specified message type name.</returns>
        public
        Type
        GetTypeForName(
            string messageName)
        {
            var types = _handlers.Keys
                .Where(type => type.Name == messageName)
                .ToList();

            if (!types.Any()) throw new UnknownMessageTypeException(messageName);

            // Attempt to find a matching handler key.
            return types.Single();
        }

        /// <summary>
        /// Gets the handler for the specified message type name.
        /// </summary>
        /// <param name="messageTypeName">Specifies the name of the message type.</param>
        /// <returns>Returns the handler for the specified message type.</returns>
        public
        Func<object, Task> 
        Get(
            string messageTypeName)
        {
            return _handlers[GetTypeForName(messageTypeName)];
        }
    }
}
