using System.Threading.Tasks;
using Votus.Core.Infrastructure.Azure.ServiceBus;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Azure.ServiceBus
{
    public class ServiceBusSubscriptionProcessorTests
    {
        [Fact]
        public 
        void 
        GetSubscriptionName_EventNameAndHandlerAreValid_ReturnsSubscriptionName()
        {
            // Arrange
            const string SampleEventName = "SampleEvent";

            // Act
            var actual = ServiceBusSubscriptionProcessor<SampleEvent>.GetSubscriptionName(
                SampleEventName, 
                new SampleHandler().HandleAsync
            );

            // Assert
            Assert.Equal("SampleHandler-SampleEvent", actual);
        }

        [Fact]
        public 
        void 
        GetSubscriptionName_SubscriptionNameLengthGreaterThanMax_ReturnsTruncatedSubscriptionName()
        {
            // Arrange
            const string SampleEventName = "SampleEventThatIsTooLongAndMustBeTruncated";

            // Act
            var actual = ServiceBusSubscriptionProcessor<SampleEvent>.GetSubscriptionName(
                SampleEventName, 
                new SampleHandler().HandleAsync
            );

            // Assert
            Assert.Equal("SampleHandler-SampleEventThatIsTooLongAndMustBeTru", actual);
        }

        #region Helper Classes

        class SampleHandler
        {
            public Task HandleAsync(SampleEvent sampleEvent)
            {
                return Task.Run(() => { });
            }
        }

        abstract class SampleEvent
        {

        }

        #endregion
    }
}
