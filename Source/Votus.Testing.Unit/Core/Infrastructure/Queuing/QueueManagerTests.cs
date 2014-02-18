using FakeItEasy;
using System;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Azure.ServiceBus;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Messaging;
using Votus.Core.Infrastructure.Queuing;
using Votus.Core.Infrastructure.Serialization;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Queuing
{
    public class QueueManagerTests
    {
        private readonly ILog           _fakeLog;
        private readonly IQueue         _fakeQueue;
        private readonly QueueManager   _queueManager;

        private readonly Guid ValidMessageId = Guid.NewGuid();

        public QueueManagerTests()
        {
            _queueManager = new QueueManager {
                Log        = _fakeLog   = A.Fake<ILog>(),
                Queue      = _fakeQueue = A.Fake<IQueue>(),
                Serializer = new NewtonsoftJsonSerializer()
            };
        }

        [Fact]
        public
        void 
        RegisterAsyncHandler_ValidHandler_IsRegistered()
        {
            // Arrange
            Func<SampleMessage, Task> handler = message => Task.Run(() => { });
            
            // Act
            _queueManager.RegisterAsyncHandler(handler);

            // Assert
            Assert.NotNull(_queueManager.Handlers.Get("SampleMessage"));
        }
        
        [Fact]
        public async Task SendAsync_Always_SavesCommandToQueue()
        {
            // Arrange
            var messageId     = Guid.NewGuid();
            var sampleMessage = new SampleMessage();

            // Act
            await _queueManager.SendAsync(messageId, sampleMessage);

            // Assert
            A.CallTo(() =>
                _fakeQueue.EnqueueAsync(messageId.ToString(), A<DynamicMessageEnvelope>.That.Not.IsNull())
            ).MustHaveHappened();
        }

        [Fact]
        public void GetMessageTypeName_Always_ReturnsTypeAsString()
        {
            // Arrange
            var message = new SampleMessage();

            // Act
            var messageTypeName = QueueManager.GetMessageTypeName(message);

            // Assert
            Assert.Equal(typeof(SampleMessage).Name, messageTypeName);
        }
        
        [Fact]
        public
        void
        LogExceptions_TaskContainsExceptions_ExceptionsAreLogged()
        {
            // Arrange
            var exception = new Exception("Testing!");
            
            // Act
            Task.Run(() => { throw exception; })
                .ContinueWith(_queueManager.LogExceptions)
                .Wait();

            // Assert
            A.CallTo(() => _fakeLog.Error(exception)).MustHaveHappened();
        }

        [Fact]
        public
        void
        LogExceptions_TaskDoesNotContainExceptions_NoExceptionsLogged()
        {
            // Act
            Task.Run(() => { })
                .ContinueWith(_queueManager.LogExceptions)
                .Wait();

            // Assert
            A.CallTo(() => 
                _fakeLog.Error(A<Exception>.Ignored)
            ).MustNotHaveHappened();
        }

        [Fact]
        public
        async Task
        SendAsync_CommandNameExists_SavesCommandToQueue()
        {
            // Arrange
            var commandEnvelope = new CommandEnvelope {
                Name    = typeof(SampleMessage).Name,
                Payload = "{SampleProperty: ''}"
            };

            _queueManager.RegisterAsyncHandler<SampleMessage>(command => 
                Task.Run(() => { })
            );

            // Act
            await _queueManager.SendAsync(
                ValidMessageId, 
                commandEnvelope
            );

            // Assert
            A.CallTo(() =>
                _fakeQueue.EnqueueAsync(A<string>.Ignored, A<DynamicMessageEnvelope>.That.Not.IsNull())
            ).MustHaveHappened();
        }

        [Fact]
        public
        void
        SendAsync_CommandEnvelopeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            CommandEnvelope nullEnvelope = null;

            // Assert
            Assert.Throws<ArgumentNullException>(() => 
                _queueManager.SendAsync(ValidMessageId, nullEnvelope)
            );
        }

        [Fact]
        public
        void
        SendAsync_CommandEnvelopeIsNull_ExceptionMessageContainsParameterName()
        {
            // Arrange
            CommandEnvelope nullEnvelope = null;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => 
                _queueManager.SendAsync(ValidMessageId, nullEnvelope)
            );

            // Assert
            Assert.Contains("commandEnvelope", exception.Message);
        }

        #region Helper Classes & Methods

        static 
        DynamicMessageEnvelope 
        GetMessageEnvelope()
        {
            return new DynamicMessageEnvelope {
                Payload        = string.Empty,
                PayloadType    = typeof(SampleMessage).Name,
            };
        }

        class SampleMessage
        {
            public string SampleProperty { get; set; }
        }

        #endregion
    }
}
