
using System.Web.Mvc;
using Votus.Web5.Areas.Gui;

namespace Votus.Web.Areas.Gui.Controllers
{
    [RouteArea(GuiAreaRegistration.AreaRegistrationName, AreaPrefix = "")]
    public class AdminController : Controller
    {
        [Route("Admin")]
        public 
        ViewResult
        ShowAdminPage()
        {
            return View("Index");
        }
    }
}