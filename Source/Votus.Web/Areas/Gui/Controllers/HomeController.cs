using System;
using System.Web.Mvc;
using Votus.Core.Ideas;
using Votus.Web5.Areas.Gui;

namespace Votus.Web.Areas.Gui.Controllers
{
    [RouteArea(GuiAreaRegistration.AreaRegistrationName, AreaPrefix = "")]
    public class HomeController : Controller
    {
        [Route("")]
        public 
        ViewResult
        ShowHomepage()
        {
            return View(
                "Home",
                new CreateIdeaCommand { NewIdeaId = Guid.NewGuid() }
            );
        }
    }
}