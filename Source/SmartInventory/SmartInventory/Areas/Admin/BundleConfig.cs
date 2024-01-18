using NPOI.SS.Formula.Functions;
using System.Web.Optimization;

namespace SmartInventory.Areas.Admin
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/AdminScripts").Include(
                              //"~/Areas/Admin/Content/js/jquery-1.9.1.js",
                              "~/Areas/Admin/Content/js/jquery-3.6.0.js",
                             "~/Areas/Admin/Content/js/bootstrap.bundle.min.js",
                            "~/Areas/Admin/Content/js/admin.js",
                             "~/Areas/Admin/Content/js/gChart.js",
                            "~/Areas/Admin/Content/js/chosen.jquery.js",
                            "~/Areas/Admin/Content/js/Alert.js",
                            "~/Areas/Admin/Content/js/jquery.validate.js",
                            "~/Areas/Admin/Content/js/jquery.validate.unobtrusive.js",
                            "~/Areas/Admin/Content/js/ModalPopUpAdmin.js",
                            "~/Areas/Admin/Content/js/jquery.unobtrusive-ajax.min.js",
                            "~/Areas/Admin/Content/js/Utility.js",
                            "~/Areas/Admin/Content/js/jquery-ui.js",
                              "~/Content/js/d3.v5.min.js",
                               "~/Areas/Admin/Content/js/equipment-workspace/equipment-view.js",
                             "~/Areas/Admin/Content/js/equipment-workspace/workspace.js",
                            "~/Areas/Admin/Content/js/equipment-workspace/equipment-builder.js",
                             "~/Areas/Admin/Content/js/InputMask/dx.all.min.js",
                             "~/Areas/Admin/Content/js/bs-stepper.min.js",
                             "~/Content/js/jquery.treeview.js",
                             "~/Content/js/Calendar/calendar.js",
                             "~/Content/js/Calendar/calendar-setup.js",
                              "~/Content/js/Calendar/calendar-en.js",
                              "~/Content/js/LightBox/js/lightbox.min.js",
                              "~/Areas/Admin/Content/js/expressionBuilder.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/Admincss")
                .Include("~/Content/css/color_stylesheet.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Areas/Admin/Content/css/bootstrap.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Areas/Admin/Content/css/font-awesome.min.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Areas/Admin/Content/css/admin.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Areas/Admin/Content/css/chosen.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Areas/Admin/Content/css/Alert.css", new CssRewriteUrlTransformWrapper())
                         .Include("~/Areas/Admin/Content/css/jquery-ui-RangeSilder.css", new CssRewriteUrlTransformWrapper())
                           .Include("~/Content/css/Equipment/equipment.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/Equipment/equipmentFonts.css", new CssRewriteUrlTransformWrapper())
                    .Include("~/Areas/Admin/Content/js/InputMask/dx.common.css", new CssRewriteUrlTransformWrapper())
                    .Include("~/Areas/Admin/Content/js/InputMask/dx.light.css", new CssRewriteUrlTransformWrapper())
                    .Include("~/Areas/Admin/Content/css/bs-stepper.min.css", new CssRewriteUrlTransformWrapper())
                     .Include("~/Content/css/jquery.treeview.css", new CssRewriteUrlTransformWrapper())
                     .Include("~/Content/js/Calendar/calendar.css", new CssRewriteUrlTransformWrapper())
                     .Include("~/Content/js/LightBox/css/lightbox.min.css", new CssRewriteUrlTransformWrapper())
                    );
            BundleTable.EnableOptimizations = true;
        }
    }
}
