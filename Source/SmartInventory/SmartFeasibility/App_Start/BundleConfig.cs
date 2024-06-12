using System.Web;
using System.Web.Optimization;

namespace SmartFeasibility
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            #region Login
            bundles.Add(new StyleBundle("~/Logincss")
                        .Include("~/Content/css/login.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/login-responsive.css", new CssRewriteUrlTransformWrapper())
                       );

            bundles.Add(new ScriptBundle("~/bundles/LoginScripts").Include(
                        "~/Content/js/login.js"
                        ));
            #endregion

            bundles.Add(new ScriptBundle("~/bundles/Scripts").Include(
                        "~/Content/js/jquery-3.7.1.js",
                        "~/Content/js/jquery-ui.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Content/js/bootstrap/bootstrap.js",
                        "~/Content/js/bootstrap/bootstrap-slider.js",
                        "~/Content/js/jquery.cookie.js",
                        "~/Content/js/jquery.treeview.js",
                           "~/Content/js/Utility.js",
                         "~/Content/js/ModalPopUpAdmin.js",
                         "~/Content/js/mapCore.js",
                         "~/Content/js/Utility.js",
                        "~/Content/js/main.js",
                           "~/Content/js/distanceWidget.js",
                        "~/Content/js/mapOverlays.js",
                      "~/Content/js/keydragzoom_packed.js",
                        "~/Content/js/bulkFeasibility.js",
                        "~/Content/js/feasibilityHistory.js",
                        "~/Content/js/Alert.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Content/js/Calendar/calendar.js",
                        "~/Content/js/Calendar/calendar-en.js",
                        "~/Content/js/Calendar/calendar-setup.js",
                         "~/Content/js/ftth-distanceWidget.js"
                         
                        ));

            bundles.Add(new StyleBundle("~/css")
                        .Include("~/Content/css/jquery-ui.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/bootstrap.css", new CssRewriteUrlTransformWrapper())
                          .Include("~/Content/css/main.css", new CssRewriteUrlTransformWrapper())
                          .Include("~/Content/css/feasibility.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/icoFeasimodule.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/Alert.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/fontawesome.min.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/jquery.treeview.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/jquery.dataTables.min.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/js/Calendar/calendar.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/mapManager.css", new CssRewriteUrlTransformWrapper())
                    );
        }
    }

    public class CssRewriteUrlTransformWrapper : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            return new CssRewriteUrlTransform().Process("~" + System.Web.VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
        }
    }
}
