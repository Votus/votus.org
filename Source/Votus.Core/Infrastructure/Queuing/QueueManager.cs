using Ninject;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Azure.ServiceBus;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Messaging;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.Queuing
{
    public class QueueManager
    {
        [Inject] public ILog        Log        { get; set; }
        [Inject] public IQueue      Queue      { get; set; }
        [Inject] public ISerializer Serializer { get; set; }

        public HandlerManager Handlers { get; set; }

        public 
        QueueManager()
        {
            Handlers = new HandlerManager();
        }

        public
        void 
        BeginProcessingMessages()
        {
            Queue.BeginReceivingMessages(
               asyncHandler: ProcessQueueMessageAsync
            );
        }

        public void LogExceptions(Task processingTask)
        {
            if (processingTask.Exception == null) return;

            foreach (var exception in processingTask.Exception.InnerExceptions)
                Log.Error(exception);
        }

        public 
        async Task
        ProcessQueueMessageAsync(
            DynamicMessageEnvelope message)
        {
            var stopwatch = Stopwatch.StartNew();

            // Get the handler for the message payload.
            var payloadType = message.PayloadType;
            var handler     = Handlers.Get(payloadType);

            // Parse the contents of the payload.
            var payload     = Serializer.Deserialize(message.Payload, Handlers.GetTypeForName(payloadType));

            // Pass the payload to the handler.
            await handler(payload);

            Log.Verbose(
                "Processed {0} queue message {1} in {2}ms",
                message.PayloadType,
                message.Id,
                stopwatch.ElapsedMilliseconds
            );
        }

        public
        void
        RegisterAsyncHandler<T>(
            Func<T, Task> handlerAsync)
        {
            Handlers.Add(handlerAsync);
        }

        public
        virtual
        Task 
        SendAsync(
            Guid            commandId, 
            CommandEnvelope commandEnvelope)
        {
            if (commandEnvelope == null) throw new ArgumentNullException("commandEnvelope");

            // Get type information based on command name
            var commandType = Handlers.GetTypeForName(commandEnvelope.Name);

            // Deserialize command to instance of the command type.
            var payload = Serializer.Deserialize(
                commandEnvelope.Payload.ToString(),
                commandType
            );

            // Make sure the command is valid before sending
            ValidateCommand(payload);

            // Send the command
            return SendAsync(commandId, payload);
        }

        private 
        static 
        void 
        ValidateCommand(object model)
        {
            var ctx = new ValidationContext(model);

            Validator.ValidateObject(
                model,
                ctx, 
                true
                );
        }



        public
        virtual
        async Task
        SendAsync<TCommand>(
            Guid        commandId, 
            TCommand    command)
        {
            var envelope = new DynamicMessageEnvelope {
                Id          = commandId,
                PayloadType = GetMessageTypeName(command),
                Payload     = Serializer.Serialize(command),
            };

            // Send to queue.
            await Queue.EnqueueAsync(commandId.ToString(), envelope);
        }

        public 
        static 
        string
        GetMessageTypeName(object command)
        {
            return command.GetType().Name;
        }
    }
}