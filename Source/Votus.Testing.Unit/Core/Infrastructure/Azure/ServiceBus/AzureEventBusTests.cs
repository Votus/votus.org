using System;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Infrastructure.Azure.ServiceBus;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Infrastructure.Serialization;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Azure.ServiceBus
{
    public class AzureEventBusTests
    {
        #region Test Setup

        private readonly ISerializer    _fakeSerializer;
        private readonly AzureEventBus  _azureEventBus;

        public 
        AzureEventBusTests()
        {
            _azureEventBus = new AzureEventBus {
                Serializer = _fakeSerializer = A.Fake<ISerializer>()
            };
        }

        #endregion

        [Fact]
        public
        void
        Subscribe_DoesntExist_AddsToHandlers()
        {
            // Act
            _azureEventBus.Subscribe<SampleEvent>(HandlerAsync);

            // Assert
            Assert.NotNull(_azureEventBus.Handlers.Get(typeof(SampleEvent).Name));
        }
        
        [Fact]
        public
        void
        ConvertToBrokeredMessage_AggregateRootEvent_ReturnsBrokeredMessageWithEnvelope()
        {
            // Arrange
            var sampleEvent = new SampleEvent();

            // Act
            var brokeredMessage = _azureEventBus.ConvertToBrokeredMessage(sampleEvent);

            // Assert
            Assert.NotNull(brokeredMessage.GetBody<DynamicMessageEnvelope>());
        }

        [Fact]
        public
        void
        ConvertToEnvelope_EventIsValid_SetsPayloadType()
        {
            // Arrange
            var sampleEvent = new SampleEvent();

            // Act
            var actual = _azureEventBus.ConvertToEnvelope(sampleEvent).PayloadType;

            // Assert
            Assert.Equal(typeof(SampleEvent).Name, actual);
        }

        [Fact]
        public
        void
        ConvertToEnvelope_EventIsValid_SetsPayload()
        {
            // Arrange
            var sampleEvent = new SampleEvent();
            const string serializedSampleEvent = "{}";

            A.CallTo(() =>
                _fakeSerializer.Serialize(A<object>.Ignored)
            ).Returns(serializedSampleEvent);

            // Act
            var actual = _azureEventBus.ConvertToEnvelope(sampleEvent).Payload;

            // Assert
            Assert.Equal(serializedSampleEvent, actual);
        }

        #region Helper Classes & Methods
        
        private 
        static 
        Task 
        HandlerAsync(object o)
        {
            throw new NotImplementedException();
        }

        class SampleEvent : AggregateRootEvent
        { }

        #endregion
    }
}
