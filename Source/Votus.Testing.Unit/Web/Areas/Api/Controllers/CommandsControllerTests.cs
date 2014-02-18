using System;
using System.Threading.Tasks;
using FakeItEasy;
using Votus.Core.Infrastructure.Messaging;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Controllers;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class CommandsControllerTests
    {
        private readonly CommandsController _controller;

        private readonly Guid ValidCommandId = Guid.NewGuid();

        public CommandsControllerTests()
        {
            _controller = new CommandsController {
                CommandDispatcher = A.Fake<QueueManager>()
            };
        }

        [Fact]
        public
        async Task
        SendCommandAsync_CommandIsValid_CommandIsSent()
        {
            // Arrange
            var command = new CommandEnvelope();

            // Act
            await _controller.SendCommandAsync(ValidCommandId, command);

            // Assert
            A.CallTo(() => 
                _controller.CommandDispatcher.SendAsync(ValidCommandId, command)
            ).MustHaveHappened();
        }
    }
}
