using System.Web.Optimization;
namespace SmartInventory
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            #region Login

            bundles.Add(new StyleBundle("~/Logincss")

                        .Include("~/Content/css/login.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/loginDefault.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/Roots/root_DarkRed.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/login-responsive.css", new CssRewriteUrlTransformWrapper())
                       
                       );

            bundles.Add(new ScriptBundle("~/bundles/LoginScripts").Include(
                        "~/Content/js/login.js"
                        ));

            #endregion

            #region OSP
            bundles.Add(new ScriptBundle("~/bundles/Scripts").Include(
                        "~/Content/js/jquery-3.7.1.js",
                        "~/Content/js/jquery-ui.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Content/js/bootstrap/bootstrap.js",
                        "~/Content/js/bootstrap/bootstrap-slider.js",
                         "~/Content/js/googlewms.min.js",
                        "~/Content/js/Main.js",
                        "~/Content/js/distanceWidget.js",
                         "~/Content/js/SpliceTray.js",
                        "~/Content/js/jquery.cookie.js",
                        "~/Content/js/jquery.treeview.js",
                        "~/Content/js/Utility.js",
                         "~/Scripts/APIHandler.js",
                        "~/Content/js/ModalPopUp.js",
                        "~/Content/js/mapCore.js",
                        "~/Content/js/Alert.js",
                        "~/Content/js/keydragzoom_packed.js",
                        "~/Content/js/chosen.jquery.min.js",
                        "~/Content/js/gOverlays.js",
                        "~/Content/js/Calendar/calendar.js",
                        "~/Content/js/Calendar/calendar-en.js",
                        "~/Content/js/Calendar/calendar-setup.js",
                        "~/Content/js/InputMask/dx.all.min.js",
                        //"~/Content/js/InputMask/dx.all.js",
                        "~/Content/js/survey.js",
                        "~/Content/js/oms.min.js",
                        // "~/Content/js/OSPSplice.js",
                        "~/Content/js/Splicing.js",
                        "~/Content/js/Splice/Js-Plumb-1.4.1.min.js",
                        "~/Content/js/jquery.orbit-1.2.3.min.js",
                        "~/Content/js/LightBox/js/lightbox.min.js",
                        //"~/Content/js/ColorPicker/docs.js",
                        "~/Content/js/ColorPicker/spectrum.js",
                         //  "~/Content/js/room-view/container.js",
                         //"~/Content/js/room-view/room-manager.js",
                         // "~/Content/js/room-view/connection-builder.js",
                         "~/Content/js/ExternalDataUploader/ZipFile.complete.js",
                           "~/Content/js/FormChangescheck.js",
                          "~/Content/js/gmaps-large-polygon.js",
                           "~/Content/js/FiberCutTracing.js",
                             "~/Content/js/inputpicker/popper.min.js",
                           "~/Content/js/inputpicker/jquery.inputpicker.js",
                           "~/Content/js/jquery.serializejson.js", //js for additional attributes
                           "~/Areas/Admin/Content/js/expressionBuilder.js" , // for expression builder
                           "~/Content/js/FiberLink.js"

                        ));

            bundles.Add(new StyleBundle("~/css")
                        .Include("~/Content/css/color_stylesheet.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/jquery-ui.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/bootstrap.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/main.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/tab-scroll.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/main-responsive.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/Alert.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/font-awesome.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/jquery.treeview.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/chosen.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/js/Calendar/calendar.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/mapManager.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/js/InputMask/dx.common.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/js/InputMask/dx.light.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/js/Splice/splice-style.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/FileUpload.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/js/LightBox/css/lightbox.min.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/icostyle.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/js/ColorPicker/spectrum.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/Splicing.css", new CssRewriteUrlTransformWrapper())
                        .Include("~/Content/css/animate.min.css", new CssRewriteUrlTransformWrapper())
                    //.Include("~/Content/css/Equipment/equipment.css", new CssRewriteUrlTransformWrapper())
                    );



            #endregion

            #region ISP
            bundles.Add(new ScriptBundle("~/bundles/ISP/Scripts").Include(
                          "~/Content/js/jquery-3.7.1.js",
                           "~/Content/js/jquery-ui.min.js",
                           "~/Content/js/bootstrap/bootstrap.js",
                           "~/Scripts/jquery.unobtrusive-ajax.min.js",
                           "~/Scripts/jquery.validate.min.js",
                           "~/Scripts/jquery.validate.unobtrusive.min.js",
                           "~/Content/js/Splicing.js",
                           "~/Content/js/ISP.js",
                            "~/Content/js/jquery.treeview.js",
                           "~/Content/js/Utility.js",
                           "~/Content/js/Alert.js",
                            "~/Content/js/chosen.jquery.min.js",
                           "~/Content/js/ModalPopUp.js",
                            "~/Content/js/InputMask/dx.all.min.js",
                           "~/Content/js/Calendar/calendar.js",
                      "~/Content/js/Calendar/calendar-en.js",
                      "~/Content/js/Calendar/calendar-setup.js",
                       "~/Content/js/ColorPicker/spectrum.js",
                    "~/Content/js/room-view/container.js",
                        "~/Content/js/room-view/room-manager.js"
                          //"~/Content/js/room-view/ConnectionBuilder.js"
                          //"~/Content/js/room-view/connection-builder.js"
                          ));

            bundles.Add(new ScriptBundle("~/ISPSCRIPTS").Include(
                            "~/Content/js/jquery-3.7.1.js",
                            "~/Content/js/jquery-ui.min.js",
                            "~/Content/js/bootstrap/bootstrap.js",
                            "~/Scripts/jquery.unobtrusive-ajax.min.js",
                            "~/Scripts/jquery.validate.min.js",
                            "~/Scripts/jquery.validate.unobtrusive.min.js",
                            "~/Content/js/d3.v5.min.js", // library to draw vertical SVG cables
                            "~/Content/js/d3.custom.js",// custom d3 methods..  
                            "~/Content/js/ISP.js",
                            "~/Content/js/jquery.treeview.js",
                            "~/Content/js/Utility.js",
                            "~/Content/js/Alert.js",
                            "~/Content/js/chosen.jquery.min.js",
                            "~/Content/js/ModalPopUp.js",
                            "~/Content/js/Calendar/calendar.js",
                            "~/Content/js/Calendar/calendar-en.js",
                            "~/Content/js/Calendar/calendar-setup.js",
                            "~/Content/js/InputMask/dx.all.min.js",
                            "~/Content/js/ColorPicker/spectrum.js",
                            "~/Content/js/Splicing.js",
                              "~/Content/js/SpliceTray.js",
                             "~/Content/js/Splice/Js-Plumb-1.4.1.min.js",
                            "~/Content/js/ColorPicker/spectrum.js",
                            "~/Content/js/jquery.orbit-1.2.3.min.js",
                            "~/Content/js/LightBox/js/lightbox.min.js",
                             "~/Content/js/room-view/container.js",
                        "~/Content/js/room-view/room-manager.js",
                          //"~/Content/js/room-view/connection-builder.js"
                          "~/Content/js/room-view/ConnectionBuilder.js",
                          "~/Areas/Admin/Content/js/equipment-workspace/workspace.js",
                            "~/Areas/Admin/Content/js/equipment-workspace/equipment-builder.js",
                            "~/Content/js/room-view/EquipmentEditor.js",
                            "~/Content/js/jquery.serializejson.js" //js for additional attributes

                          ));

            bundles.Add(new StyleBundle("~/ISPCSS")
                            .Include("~/Content/css/bootstrap.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/main.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/Alert.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/font-awesome.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/jquery.treeview.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/chosen.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/js/Calendar/calendar.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/ISP.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/ISPEntity.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/bread_crumb.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/icomoon.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/js/InputMask/dx.common.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/js/InputMask/dx.light.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/icostyle.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/js/ColorPicker/spectrum.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/Splicing.css", new CssRewriteUrlTransformWrapper())
                            .Include("~/Content/css/color_stylesheet.css", new CssRewriteUrlTransformWrapper())
                           );

            bundles.Add(new StyleBundle("~/ISPCSSNEW")
                .Include("~/Content/css/bootstrap.css", new CssRewriteUrlTransformWrapper())
                  .Include("~/Content/css/Alert.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/chosen.css", new CssRewriteUrlTransformWrapper())
                 .Include("~/Content/css/ISP.css", new CssRewriteUrlTransformWrapper())
                  .Include("~/Content/css/ISP-responsive.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/icostyle.css", new CssRewriteUrlTransformWrapper())
                 .Include("~/Content/css/chosen.css", new CssRewriteUrlTransformWrapper())
                 .Include("~/Content/js/Calendar/calendar.css", new CssRewriteUrlTransformWrapper())
                 .Include("~/Content/css/jquery.treeview.css", new CssRewriteUrlTransformWrapper())
                 .Include("~/Content/css/font-awesome.css", new CssRewriteUrlTransformWrapper())
                 .Include("~/Content/js/InputMask/dx.common.css", new CssRewriteUrlTransformWrapper())
                  .Include("~/Content/js/InputMask/dx.light.css", new CssRewriteUrlTransformWrapper())
                   .Include("~/Content/js/ColorPicker/spectrum.css", new CssRewriteUrlTransformWrapper())
                    .Include("~/Content/css/Splicing.css", new CssRewriteUrlTransformWrapper())
                   .Include("~/Content/js/LightBox/css/lightbox.min.css", new CssRewriteUrlTransformWrapper())
                    .Include("~/Content/css/tab-scroll.css", new CssRewriteUrlTransformWrapper())
                    .Include("~/Content/css/color_stylesheet.css", new CssRewriteUrlTransformWrapper())
                 // .Include("~/Content/css/Equipment/equipment.css", new CssRewriteUrlTransformWrapper())
                 //.Include("~/Content/css/Equipment/equipmentFonts.css", new CssRewriteUrlTransformWrapper())
                 //.Include("~/Content/css/jquery-ui.css", new CssRewriteUrlTransformWrapper())
                 );

            bundles.Add(new ScriptBundle("~/VizNetworkScripts").Include(
              "~/Content/js/html2canvas.js",
               "~/Content/js/jspdf.min.js",
                "~/Content/js/vis_4_18_1.min.js",
                 "~/Content/js/jquery-1.9.1.min.js",
                  "~/Content/js/Utility.js"
            ));
            bundles.Add(new StyleBundle("~/VizNetworkCSS")
                  .Include("~/Content/css/visDiagram.css", new CssRewriteUrlTransformWrapper())
                   );

            #endregion

            #region endToEndSchematic

            bundles.Add(new ScriptBundle("~/bundles/endToEndSchematic/Scripts").Include(
            "~/Content/js/Utility.js",
            "~/Content/js/SchematicView/d3.v3.min.js",
            "~/Content/js/SchematicView/dndTree.js",
            "~/Content/js/DivToImage.js",
            "~/Content/js/jspdf.min.js"
            ));
            #endregion

            #region Barcode

            bundles.Add(new ScriptBundle("~/bundles/Barcode/Scripts").Include(
            "~/Content/js/DivToImage.js",
            "~/Content/js/jspdf.min.js"
            ));
            #endregion

            #region KMLZ Uploder
            bundles.Add(new ScriptBundle("~/bundles/ExternalDataUploader/Scripts").Include(
                  "~/Content/js/ExternalDataUploader/jsgeoxml3.js",
                  "~/Content/js/ExternalDataUploader/geoxml3_gxParse_kmz.js",
                  "~/Content/js/ExternalDataUploader/ExternalDataUploader.js"
                ));
            #endregion

            #region network plan 

            bundles.Add(new ScriptBundle("~/bundles/NetworkPlanning/Scripts").Include(
                "~/Content/js/NetworkPlanning/NetworkPlanning.js",
                "~/Content/js/inputpicker/jquery.inputpicker1.js",
                 "~/Content/js/NetworkPlanning/distanceWidget.js"
              ));

            bundles.Add(new StyleBundle("~/bundles/NetworkPlanning/css").Include(
                  "~/Content/css/NetworkPlanning/NetworkPlanning.css",
                  "~/Content/css/inputpicker/jquery.inputpicker1.css"
            ));

            #endregion

            #region DataUploader JS
            //bundles.Add(new ScriptBundle("~/bundles/Scripts").Include(
            //     "~/Content/js/datauploader.js"
            //   ));
            #endregion

            #region DynamicControl JS
            bundles.Add(new ScriptBundle("~/bundles/AdditionalAttributes/Scripts").Include(
                 "~/Content/js/DynamicForms/dynamic.js",
                 "~/Content/js/DynamicForms/AddRemoveDropDown.js",
                 "~/Content/js/DynamicForms/ControlEvents.js",
                 "~/Content/js/DynamicForms/ControlGenerator.js",
                 "~/Content/js/DynamicForms/ControlInitializer.js",
                 "~/Content/js/DynamicForms/HTMLRenderer.js",
                  "~/Content/js/DynamicForms/RenderExistingControls.js",
                  "~/Content/js/DynamicForms/ControlValidator.js",
                  "~/Content/js/DynamicForms/DeletePopupScript.js"
               ));
            #endregion
            
            BundleTable.EnableOptimizations = true;
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