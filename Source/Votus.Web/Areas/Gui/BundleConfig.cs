using System.Web.Optimization;

namespace Votus.Web.Areas.Gui
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(
                new ScriptBundle("~/bundles/modernizr")
                    .Include("~/Areas/Gui/Scripts/modernizr-*")
            );

            bundles.Add(
                new ScriptBundle("~/bundles/knockoutjs")
                    .Include("~/Areas/Gui/Scripts/knockout-*")
            );

            bundles.Add(
                new ScriptBundle("~/bundles/jquery")
                    .Include("~/Areas/Gui/Scripts/jquery-{version}.js")
                    .Include("~/Areas/Gui/Scripts/jquery.url.js")
                    .Include("~/Areas/Gui/Scripts/jquery.validate.min.js")
                    .Include("~/Areas/Gui/Scripts/jquery.validate.unobtrusive.min.js")
                    .Include("~/Areas/Gui/Scripts/jquery.autosize.input.js")
                    .Include("~/Areas/Gui/Scripts/jquery.inputhints.min.js")
            );

            bundles.Add(
                new ScriptBundle("~/bundles/votus")
                    .Include("~/Areas/Gui/Scripts/main.js")
                    .Include("~/Areas/Gui/Scripts/votus.utilities.js")
                    .Include("~/Areas/Gui/Scripts/votus.api.client.js")
            );

            bundles.Add(
                new StyleBundle("~/Content/css")
                    .Include("~/Areas/Gui/Content/Site.css")
            );

            BundleTable.EnableOptimizations = true;
        }
    }
}