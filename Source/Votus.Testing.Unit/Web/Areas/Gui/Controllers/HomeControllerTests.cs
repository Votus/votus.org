using System;
using Votus.Core.Domain.Ideas;
using Votus.Web.Areas.Gui.Controllers;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Gui.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void ShowHomepage_Always_SetsCreateIdeaCommandId()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.ShowHomepage().Model as CreateIdeaCommand;

            // Assert
            Assert.True(result.NewIdeaId != Guid.Empty);
        }
    }
}
