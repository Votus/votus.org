using System;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Azure.ServiceBus;
using Votus.Core.Infrastructure.EventSourcing;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Azure.ServiceBus
{
    public class AzureEventBusTests
    {
        #region Test Setup

        private readonly AzureEventBus  _azureEventBus;

        public 
        AzureEventBusTests()
        {
            const string fakeConnectionString = 
                "Endpoint=sb://test.servicebus.windows.net/;SharedSecretIssuer=owner;SharedSecretValue=AAAAABBBBBCCCCCDDDDDEEEEEFFFFFGGGGGHHHHHIII=";

            _azureEventBus = new AzureEventBus(fakeConnectionString);
        }

        #endregion

        [Fact]
        public
        void
        Subscribe_DoesntExist_AddsToEventManagers()
        {
            // Act
            _azureEventBus.Subscribe<SampleEvent>(HandlerAsync);

            // Assert
            Assert.NotNull(_azureEventBus.EventManagers[typeof(SampleEvent)]);
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
