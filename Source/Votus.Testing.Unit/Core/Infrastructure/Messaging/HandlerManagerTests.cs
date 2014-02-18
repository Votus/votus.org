using System.Threading.Tasks;
using Votus.Core.Infrastructure.Messaging;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Messaging
{
    public class HandlerManagerTests
    {
        private readonly HandlerManager _handlerManager;

        public HandlerManagerTests()
        {
            _handlerManager = new HandlerManager();
        }

        [Fact]
        public
        void
        GetTypeForName_TypeIsRegistered_ReturnsType()
        {
            // Arrange
            var messageName = typeof(SampleMessage).Name;

            _handlerManager.Add<SampleMessage>(command =>
                Task.Run(() => { })
            );

            // Act
            var actual = _handlerManager.GetTypeForName(messageName);

            // Assert
            Assert.Equal(typeof(SampleMessage), actual);
        }

        [Fact]
        public
        void
        GetTypeForName_TypeIsNotRegistered_ThrowsUnknownMessageTypeException()
        {
            // Arrange
            const string InvalidTypeName = "invalid-name";

            // Act
            // Assert
            Assert.Throws<UnknownMessageTypeException>(() => _handlerManager.GetTypeForName(InvalidTypeName));
        }

        class SampleMessage { }
    }
}
