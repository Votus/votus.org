using System.Web.Mvc;
using System.Web.Optimization;
using Votus.Web.Areas.Gui;

namespace Votus.Web5.Areas.Gui
{
    public class GuiAreaRegistration : AreaRegistration 
    {
        public const string AreaRegistrationName = "Gui";

        public override string AreaName
        {
            get { return AreaRegistrationName; }
        }
        
        public override void RegisterArea(AreaRegistrationContext context)
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            context.Routes.MapMvcAttributeRoutes();
        }
    }
}