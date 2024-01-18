using BusinessLogics;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Models;
using Newtonsoft.Json;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;
using Utility.MapPrinter;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class PrintController : Controller
    {
        [OutputCache(CacheProfile = "CacheForOneDay")]
        //public ActionResult Index(int printHistoryID = 0, int mapCurrentZoom = 5, double mapCenterLat = 22.973407)
        //{
        //    PrintMap pmap = new PrintMap();
        //    pmap.mapCurrentZoom = mapCurrentZoom;
        //    pmap.mapCenterLat = mapCenterLat;
        //    if (printHistoryID > 0)
        //    {
        //        var jsonStr = new BLPrintLog().GetPrintExportLogJson(printHistoryID);
        //        pmap = JsonConvert.DeserializeObject<PrintMap>(jsonStr);
        //        pmap.printHistoryID = printHistoryID;
        //        System.Threading.Thread.Sleep(500);
        //    }
        //    pmap.ScaleLimit = MapReport.SCALELIMIT;
        //    // list of applicable modules..
        //    pmap.ApplicableModuleList = (List<string>)Session["ApplicableModuleList"];
        //    pmap.isFooterTemplateEnabled = pmap.ApplicableModuleList.Contains("PMA");

        //    BLPrintSavedTemplate objbl = new BLPrintSavedTemplate();
        //    var usrDetail = (User)Session["userDetail"];
        //    pmap.ddlSavedTemplate = objbl.GetPrintTemplateList(0, usrDetail.user_id);

        //    //pmap.ddlMapScale = objbl.GetMapScaleList(pmap.mapCurrentZoom, pmap.mapCenterLat);
        //    if (printHistoryID > 0)
        //    {
        //        Session["RePrtMapsModel"] = pmap;
        //    }

        //    return PartialView("_PrintMap", pmap);
        //}
        public ActionResult Index(PrintMap pmap)
        {
            if (pmap.printHistoryID > 0)
            {
                int hstid = pmap.printHistoryID;
                var jsonStr = new BLPrintLog().GetPrintExportLogJson(pmap.printHistoryID);
                pmap = JsonConvert.DeserializeObject<PrintMap>(jsonStr);
                pmap.printHistoryID = hstid;
                System.Threading.Thread.Sleep(500);
            }
            pmap.ScaleLimit = MapReport.SCALELIMIT;
            // list of applicable modules..
            pmap.ApplicableModuleList = (List<string>)Session["ApplicableModuleList"];
            pmap.isFooterTemplateEnabled = pmap.ApplicableModuleList.Contains("PMA");

            BLPrintSavedTemplate objbl = new BLPrintSavedTemplate();
            var usrDetail = (User)Session["userDetail"];
            pmap.ddlSavedTemplate = objbl.GetPrintTemplateList(0, usrDetail.user_id);

            pmap.ddlMapScale = objbl.GetMapScaleList(pmap.mapCurrentZoom, Convert.ToDouble(pmap.mapCenterLat));
            if (pmap.printHistoryID > 0)
            {
                Session["RePrtMapsModel"] = pmap;
            }
            //else
            //{
            //    pmap.printHistoryID = 0;
            //}
            //ModelState.Remove("printHistoryID");
            //ModelState.Clear();
            return PartialView("_PrintMap", pmap);
        }
        [OutputCache(CacheProfile = "CacheForOneDay")]
        public ActionResult MapPrint()
        {
            return PartialView("_MapPrint", new PrintMap());
        }
        public ActionResult PrintExportLog(PrintExportLogVM ObjPrintExportLogVM, int page = 0, string sort = "", string sortdir = "")
        {          
            var usrDetail = (User)Session["userDetail"];
            if (sort != "" || page != 0)
            {
                ObjPrintExportLogVM.objGridAttributes = (CommonGridAttributes)Session["printExportLog"];
            }
            var timeInteval = ApplicationSettings.PrintLogTimeInterval;
            ObjPrintExportLogVM.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            ObjPrintExportLogVM.objGridAttributes.currentPage = page == 0 ? 1 : page;
            ObjPrintExportLogVM.objGridAttributes.sort = sort;
            ObjPrintExportLogVM.objGridAttributes.orderBy = sortdir;
            ObjPrintExportLogVM.printLog = new BLPrintLog().GetPrintExportLogList(ObjPrintExportLogVM.objGridAttributes, usrDetail.user_id, timeInteval);
            ObjPrintExportLogVM.objGridAttributes.totalRecord = ObjPrintExportLogVM.printLog != null && ObjPrintExportLogVM.printLog.Count > 0 ? ObjPrintExportLogVM.printLog[0].totalRecords : 0;
            Session["printExportLog"] = ObjPrintExportLogVM.objGridAttributes;
            return PartialView("_PrintExportLog", ObjPrintExportLogVM);
        }


        public JsonResult RePrintMap(int ValidID)
        {
            var jsonStr = new BLPrintLog().GetPrintExportLogJson(ValidID);
            PrintMap model = JsonConvert.DeserializeObject<PrintMap>(jsonStr);
            int sheetCount = 0;
            var success = 0;
            var strMsg = "";
            try
            {
                // assign print map global and other helper values...
                var usrDetail = (User)Session["userDetail"];
                model.roleId = usrDetail.role_id;
                model.userId = usrDetail.user_id;
                model.ApplicableModuleList = (List<string>)Session["ApplicableModuleList"];
                //model.isFooterTemplateEnabled = model.ApplicableModuleList.Contains("PMA");
                model.isStaticMapEnabled = ApplicationSettings.isStaticMapEnabled == true && model.backgroundMap == true && model.mapType.ToLower() != "coordinate" ? true : false;
                model.AttachmentLocalPath = System.Web.HttpContext.Current.Server.MapPath(ApplicationSettings.AttachmentLocalPath);
                model.checkBox_ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Content/images/blank_checkbox.png");
                model.maxPDflimit = ApplicationSettings.MaxPDFWihoutThread;
                model.ftpPath = ApplicationSettings.FTPAttachment;
                model.ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                model.ftpPassword = ApplicationSettings.FTPPasswordAttachment;
                model.sheetCount = sheetCount;
                var pageSize = PDFHelper.GetPageSize(model.pageSize);
                pageSize = model.pageLayout == "Landscape" ? pageSize.Rotate() : pageSize;

                //// Set Created on For Print PDF every page
                model.CreatedOn = DateTimeHelper.Now;
                BLPrintSavedTemplate objbl = new BLPrintSavedTemplate();
                model.ddlSavedTemplate = objbl.GetPrintTemplateList(0, model.userId);

                //extend the print map object for selected geom with upper and lower bound values.
                model = GetPrintMapBySelectedArea(model);

                // prepare the map layer url based or layer filters.
                model.layerUrls = GetLayerURLs(model);
                model.staticMapImgUrl = model.isStaticMapEnabled ? GetStaicMapUrl(model) : string.Empty;
                // export map into pdf files.               
                success = ExportMap(model, pageSize);
            }
            catch (Exception ex)
            {
                success = 0;
                strMsg = String.Format(Resources.Resources.SI_OSP_PM_NET_FRM_021, ex.Message); //"Error while processing your request!<br><b>Error Message:</b>" + (ex.Message ?? "");
                //Write error log
                ErrorLogHelper.WriteErrorLog("PrintMap()", "Print", ex, model);
            }
            return Json(new { success = success, message = strMsg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ZoomHighlightPrintMap(int printHistoryID = 0)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            var objGeometryDetail = new BLPrintLog().GetPrintMapGeoms(printHistoryID);
            if (objGeometryDetail != null)
            {
                var extent = objGeometryDetail.geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
                string[] bounds = extent.Split(',');
                string[] southWest = bounds[0].Split(' ');
                string[] northEast = bounds[1].Split(' ');
                objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
                objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
                objResp.result = objGeometryDetail;
                objResp.status = ResponseStatus.OK.ToString();
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetEntityGeoms(double bufferValue, int enityID = 0)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            var objGeometryDetail = new BLPrintLog().GetEntityGeometry(enityID);
            if (objGeometryDetail != null)
            {
                var extent = objGeometryDetail.geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
                string[] bounds = extent.Split(',');
                string[] southWest = bounds[0].Split(' ');
                string[] northEast = bounds[1].Split(' ');
                objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
                objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
                objResp.result = objGeometryDetail;
                objResp.status = ResponseStatus.OK.ToString();
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult PrintMap(PrintMap model, int sheetCount)
        {
            if (model.selectedGeomType == "Circle")
            {
                model.mapSelectedGeom = new BLLayer().getCircleBoundary(model.radius, model.mapSelectedGeom);
            }
            var success = 0;
            var strMsg = "";
            try
            {
                PrintMap pMap = new PrintMap();
                if (model.printHistoryID > 0)
                {
                    pMap = (PrintMap)Session["RePrtMapsModel"];
                    sheetCount = pMap.sheetCount;
                }
                else
                {
                    pMap = model;
                }

                var usrDetail = (User)Session["userDetail"];
                pMap.roleId = usrDetail.role_id;
                pMap.userId = usrDetail.user_id;
                // model.ApplicableModuleList = (List<string>)Session["ApplicableModuleList"];
                pMap.isStaticMapEnabled = ApplicationSettings.isStaticMapEnabled == true && model.backgroundMap == true && model.mapType.ToLower() != "coordinate" ? true : false;
                pMap.AttachmentLocalPath = System.Web.HttpContext.Current.Server.MapPath(ApplicationSettings.AttachmentLocalPath);
                pMap.checkBox_ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Content/images/blank_checkbox.png");
                pMap.maxPDflimit = ApplicationSettings.MaxPDFWihoutThread;
                pMap.ftpPath = ApplicationSettings.FTPAttachment;
                pMap.ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                pMap.ftpPassword = ApplicationSettings.FTPPasswordAttachment;
                pMap.ApplicableModuleList = (List<string>)Session["ApplicableModuleList"];


                pMap.printMapAttr = model.printMapAttr;
                pMap.pageTitle = model.pageTitle;
                pMap.pageScale = model.pageScale;
                pMap.pageLayout = model.pageLayout;
                pMap.sheetCount = sheetCount;
                var pageSize = PDFHelper.GetPageSize(model.pageSize);
                pageSize = model.pageLayout == "Landscape" ? pageSize.Rotate() : pageSize;

                //// Set Created on For Print PDF every page
                pMap.CreatedOn = DateTimeHelper.Now;
                BLPrintSavedTemplate objbl = new BLPrintSavedTemplate();
                pMap.ddlSavedTemplate = objbl.GetPrintTemplateList(0, pMap.userId);

                //extend the print map object for selected geom with upper and lower bound values.

                if (model.printHistoryID > 0)
                {
                    PrintMap pmap = (PrintMap)Session["RePrtMapsModel"];
                    pMap.mapCanvasHeight = pmap.mapCanvasHeight;
                    pMap.mapCanvasWidth = pmap.mapCanvasWidth;
                    pMap.mapLeftLng = pmap.mapLeftLng;
                    pMap.mapRightLng = pmap.mapRightLng;
                    pMap.mapTopLat = pmap.mapTopLat;
                    pMap.mapBottomLat = pmap.mapBottomLat;
                }
                else
                {
                    pMap = GetPrintMapBySelectedArea(pMap);
                }
               

                // prepare the map layer url based or layer filters.
                pMap.layerUrls = GetLayerURLs(pMap);
                pMap.staticMapImgUrl = pMap.isStaticMapEnabled ? GetStaicMapUrl(pMap) : string.Empty;

                // export map into pdf files.               
                success = ExportMap(pMap, pageSize);
            }
            catch (Exception ex)
            {
                success = 0;
                strMsg = String.Format(Resources.Resources.SI_OSP_PM_NET_FRM_021, ex.Message); //"Error while processing your request!<br><b>Error Message:</b>" + (ex.Message ?? "");
                //Write error log
                ErrorLogHelper.WriteErrorLog("PrintMap()", "Print", ex, model);
            }
            return Json(new { success = success, message = strMsg }, JsonRequestBehavior.AllowGet);
        }
        public int ExportMap(PrintMap printMap, iTextSharp.text.Rectangle pageSize)
        {
           
            var updateStatus = 0;
            PrintExportLog printLog = new PrintExportLog();
            printLog.user_id = printMap.userId;
            printLog.page_title = printMap.pageTitle;
            printLog.start_on = DateTimeHelper.Now;
            //printMap.mapCanvasWidth = printMap.printScale == 2 ? printMap.mapCanvasWidth * 2 : printMap.mapCanvasWidth;
            //printMap.mapCanvasHeight = printMap.printScale == 2 ? printMap.mapCanvasHeight * 2 : printMap.mapCanvasHeight;
            MapReport mapReport = MapReport.GetMapReportContent(printMap);
            if (!string.IsNullOrEmpty(ApplicationSettings.ClientLogoImageBytesForWeb))
            {
                if (ApplicationSettings.ClientLogoImageBytesForWeb.Trim() != "")
                {
                    printMap.ClientLogoImageBytesForWeb = ApplicationSettings.ClientLogoImageBytesForWeb;

                }
            }
            MapPrintManager printManager = new MapPrintManager(printMap.mapCanvasWidth, printMap.mapCanvasHeight, (double)printMap.mapLeftLng, (double)printMap.mapRightLng, (double)printMap.mapTopLat, (double)printMap.mapBottomLat, mapReport.MapWidth, mapReport.MapWidth, printMap.mapCurrentZoom);
            //convert geom to list of MapPoints.
            string[] geomSplit = printMap.mapSelectedGeom.Split(',');
            printManager.SelectedGeoMapArea = new List<MapPoint>();
            foreach (var point in geomSplit)
            {
                string[] mark = point.Split(' ');
                double lng = Convert.ToDouble(mark[0]);
                double lat = Convert.ToDouble(mark[1]);
                printManager.SelectedGeoMapArea.Add(new MapPoint { X = lat, Y = lng });
            }
            printManager.StaticMapURL = ApplicationSettings.StaticMapURL;
            printManager.MapKey = ApplicationSettings.MapKey;
            printManager.LocalLayerUrls = printMap.layerUrls.Select(x => x.url).ToList();
            printManager.MapType = printMap.mapType;
            printManager.IsStaticMapEnabled = printMap.isStaticMapEnabled;
            printManager.StaticScaleMap = printMap.printScale;
            printManager.IsLayerMapClipEnabled = true;
            printManager.IsStaticMapClipEnabled = printMap.backgroundClipping;
            var tiles = printManager.CreateMapTiles(printMap.pageScale);
            List<ReportSheet> reportSheets = new List<ReportSheet>();
            printLog.page_count = 1;
            if (printMap.pageScale > 0)
            {
               
                reportSheets = printManager.CreateReportSheets(printMap.pageScale, mapReport);
            
                List<ReportSheet> newReportSheet = new List<ReportSheet>();
                var id = 0;
                
                foreach (var pdfSheet in reportSheets)
                {
                    var points = printManager.GetSheetClippingPoints(pdfSheet);
                    var clippingPoints = points.ToArray();
                    // var clippingPoints = GetSheetWiseClippingPoints(pdfSheet).ToArray();

                    if (printManager.findIsSheetContainsPolygon(points, pdfSheet))
                    {
                        id = id + 1;
                        pdfSheet.Revised_Id = id;
                        newReportSheet.Add(pdfSheet);
                    }
                }
               
                reportSheets = newReportSheet;
                printManager.MapSheetWithTiles(tiles, reportSheets);
                printLog.page_count = reportSheets.Count();
            }
            printLog.page_type = printMap.pageSize;
            printLog.export_request_params = JsonConvert.SerializeObject(printMap);
            printMap.isCropByScreen = true;
            var printLogObj = new BLPrintLog().SavePrintLog(printLog);

            ///// Save print map templates

            if (printMap.printMapAttr.IsSavedAsTemplate)
            {
                var savePrintTlt = SavedPrintTemplates(printMap);
            }

            //printManager.StartBrowser(printMap.remarks,AttachmentLocalPath);
            if (reportSheets.Count() <= ApplicationSettings.MaxPDFWihoutThread)
            {
               // reportSheets[0].Revised_Id = 1;
                Session["PrintExportOut"] = printManager.MergeImageTiles(tiles, reportSheets, pageSize, mapReport, printMap, printLogObj, false);
                updateStatus = 1;
            }
            else
            {
                
                new Thread(() =>
                {
                    printManager.MergeImageTiles(tiles, reportSheets, pageSize, mapReport, printMap, printLogObj, true);
                }).Start();
               
                updateStatus = -1;
            }
            
            printMap.IsVisiblePrintLegendEntityCount = ApplicationSettings.IsVisiblePrintLegendEntityCount;
            return updateStatus;
        }
     
        [HttpPost]
        public JsonResult PrintMapPDFCount(PrintMap printMap)
        {
          

            var pdfCount = 0.0;
            var status = false;
            var strMsg = string.Empty;
            try
            {
                printMap.isFooterTemplateEnabled = ((List<string>)Session["ApplicableModuleList"]).Contains("PMA");


            

                if (printMap.printHistoryID > 0) {

                    PrintMap pmap =(PrintMap)Session["RePrtMapsModel"];
                    printMap.mapCanvasHeight = pmap.mapCanvasHeight;
                    printMap.mapCanvasWidth = pmap.mapCanvasWidth;
                    printMap.mapLeftLng = pmap.mapLeftLng;
                    printMap.mapRightLng = pmap.mapRightLng;
                    printMap.mapTopLat = pmap.mapTopLat;
                    printMap.mapBottomLat = pmap.mapBottomLat;
                }
                else
                {
                   
                    printMap = GetPrintMapBySelectedArea(printMap);
                  
                }
                //if (printMap.printScale == 2)
                //{
                //    printMap.mapCanvasHeight = printMap.mapCanvasHeight * 2;
                //    printMap.mapCanvasWidth = printMap.mapCanvasWidth * 2;
                //}
                MapReport mapReport = MapReport.GetMapReportContent(printMap);
                MapPrintManager printManager = new MapPrintManager(printMap.mapCanvasWidth, printMap.mapCanvasHeight, (double)printMap.mapLeftLng, (double)printMap.mapRightLng, (double)printMap.mapTopLat, (double)printMap.mapBottomLat, mapReport.MapWidth, mapReport.MapWidth, printMap.mapCurrentZoom);
                printManager.StaticScaleMap = printMap.printScale;
                List<ReportSheet> reportSheets = new List<ReportSheet>();

                printManager.SelectedGeoMapArea = new List<MapPoint>();
                string[] geomSplit = printMap.mapSelectedGeom.Split(',');
                foreach (var point in geomSplit)
                {
                    string[] mark = point.Split(' ');
                    double lng = Convert.ToDouble(mark[0]);
                    double lat = Convert.ToDouble(mark[1]);
                    printManager.SelectedGeoMapArea.Add(new MapPoint { X = lat, Y = lng });
                }
                if (printMap.pageScale > 0)
                {
                 
                    reportSheets = printManager.CreateReportSheets(printMap.pageScale, mapReport);
                  
                }



              
                foreach (var pdfSheet in reportSheets)
                {


                    var points = printManager.GetSheetClippingPoints(pdfSheet);
                    var clippingPoints = points.ToArray();
                    // var clippingPoints = GetSheetWiseClippingPoints(pdfSheet).ToArray();

                    if (printManager.findIsSheetContainsPolygon(points, pdfSheet))
                    {
                        pdfCount = pdfCount + 1;
                    }
                }
              
                // pdfCount = printManager.PdfSheetCount(printMap.pageScale, mapReport);
                status = true;
            }
            catch (Exception ex)
            {
                pdfCount = -1;
                strMsg = "Error while calculating the total files count!<br><b>Error Message:</b>" + (ex.Message ?? "");
                //Write error log
                ErrorLogHelper.WriteErrorLog("PrintMapPDFCount()", "Print", ex, printMap);
              
            }
           
            return Json(new { success = status, message = strMsg, pdfCount = pdfCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckTemplateNameExist(string template_name)
        {
            BLPrintSavedTemplate BLPrint = new BLPrintSavedTemplate();
            bool status = false;
            var usrDetail = (User)Session["userDetail"];
            try
            {
                status =  BLPrint.CheckPrintTemplateName(template_name, usrDetail.user_id);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("CheckTemplateNameExist()", "Print", ex, template_name);
            }
            return Json(new { success = status}, JsonRequestBehavior.AllowGet);
        }


        public FileResult DownloadFileFromFTP(string filePath, string fileExtension, string pageType)
        {
            string localPath = string.Empty;
            try
            {
                string FtpUrl = ApplicationSettings.FTPAttachment;
                string UserName = ApplicationSettings.FTPUserNameAttachment;
                string PassWord = ApplicationSettings.FTPPasswordAttachment;

                string fullPath = FtpUrl + filePath;
                string fileNameFormatted = string.Format("Export_Map_{0}_{1}{2}", pageType, MiscHelper.getTimeStamp(), fileExtension);
                localPath = string.Concat(Server.MapPath(ApplicationSettings.AttachmentLocalPath), fileNameFormatted);

                var request = (FtpWebRequest)WebRequest.Create(fullPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(UserName, PassWord);
                request.UseBinary = true;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (FileStream fs = new FileStream(localPath, FileMode.Create))
                        {
                            byte[] buffer = new byte[102400];
                            int read = 0;

                            while (true)
                            {
                                read = responseStream.Read(buffer, 0, buffer.Length);

                                if (read == 0)
                                    break;

                                fs.Write(buffer, 0, read);
                            }
                            fs.Close();
                        }
                    }
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(localPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameFormatted);
            }
            catch (Exception ex)
            {
                //Write error log
                ErrorLogHelper.WriteErrorLog("DownloadFileFromFTP()", "Print", ex);
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    System.IO.File.Delete(localPath);
                }
            }
            return null;

        }
        public ActionResult DownloadPrintMap(string page_type)
        {
            PrintMapOut objPrintMapOut = (PrintMapOut)Session["PrintExportOut"];
            if (objPrintMapOut.fileBytes == null)
                return new EmptyResult();
            Session["PrintExportOut"] = null;
            string fileNameFormatted = string.Format("Export_Map_{0}_{1}{2}", page_type, MiscHelper.getTimeStamp(), objPrintMapOut.fileExtension);
            return File(objPrintMapOut.fileBytes, "application/octet-stream", fileNameFormatted);
        }
        private string GetStaicMapUrl(PrintMap model)
        {
            return string.Format("{0}&center={1},{2}&zoom={3}&size={4}x{5}&maptype={6}&key={7}",
                ApplicationSettings.StaticMapURL, model.mapCenterLat,
                model.mapCenterLng, model.mapCurrentZoom,
                model.mapCanvasWidth, model.mapCanvasHeight,
                model.mapType, ApplicationSettings.MapKey
                );
        }
        private string GetLayerURL(PrintMap model)
        {
            var layerUrl = model.layerURL;
            layerUrl = layerUrl.Replace(getBetween(layerUrl, "HEIGHT=", "&", true), "HEIGHT=" + model.mapCanvasHeight);
            layerUrl = layerUrl.Replace(getBetween(layerUrl, "WIDTH=", "&", true), "WIDTH=" + model.mapCanvasWidth);
            layerUrl = layerUrl.Replace(getBetween(layerUrl, "BBOX=", "&", true), "BBOX=" + model.mapCordinates);
            //   layerUrl = "http://192.168.1.106:8080/cgi-bin/mapserv.exe?MAP=D:/mapfiles/Spectra_mvc/NetworkEntitiesNoLabel.map&srs=EPSG:4326&version=1.0.0&REQUEST=GetMap&LAYERS=ARA,SVA,SBA,BLD,BLDC,BLDP,STR,ADB,BDB,CDB,SPL,POD,MOD,POL,TRE,MH,CBL,DCT,TRH,SC,CUS,ONT,WMT&regionFilter=%20region_id%20in%20(25)&provinceFilter=(region_id%20in%20(25)%20AND%20province_id%20in%20(31,32))&layerFilter=network_status%20in%20(%27D%27)&prjectspecificationFilter=1%20=%201&service=wms&BBOX=77.08061995208436,28.495264001423013,77.08913864791566,28.49845099175983&WIDTH=1588&HEIGHT=676&FORMAT=image/png&reqver=0";

            return layerUrl;
        }
        private string GetLegendUrl(PrintMap model)
        {

            return string.Format("{0}MAP={1}Legend.map&srs=EPSG:4326&version=1.0.0&REQUEST=GetLegendGraphic&LAYER=Network&service=wms&FORMAT=image/png&legend_options=layout:vertical",
                ApplicationSettings.MapServerURL, ApplicationSettings.MapDirPath);

        }
        public static string getBetween(string strSource, string strStart, string strEnd, bool includeStartText)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return string.Concat(strStart, strSource.Substring(Start, End - Start));
            }
            else
            {
                return "";
            }
        }
        public iTextSharp.text.Image OverlayImages(System.Drawing.Image bgImage, List<layerUrl> urls)
        {
            var layerImgHeightWidth = GetImageFromUrl(urls.FirstOrDefault().url);
            System.Drawing.Image layerImg = null;

            System.Drawing.Image imgResult = new System.Drawing.Bitmap(layerImgHeightWidth.Width, layerImgHeightWidth.Height);
            using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(imgResult))
            {
                if (bgImage != null)
                {
                    gr.DrawImage(bgImage, new System.Drawing.Point(0, 0));
                }

                foreach (var item in urls)
                {
                    layerImg = GetImageFromUrl(item.url);
                    gr.DrawImage(layerImg, new System.Drawing.Point(0, 0));
                }
            }
            //Save image temporaily. This is saved as System.Drawing.Image can not converted directly into Itextsharp image.
            //Commented by priyanka
            //var tempFilePath = System.IO.Path.Combine(Server.MapPath("~/Uploads/Temp/"), "Temp_Map_" + Utility.MiscHelper.getTimeStamp());
            //Commented end by priyanka
            var tempFilePath = System.IO.Path.Combine(Server.MapPath(Settings.ApplicationSettings.DownloadTempPath), "Temp_Map_" + Utility.MiscHelper.getTimeStamp());
            imgResult.Save(tempFilePath, System.Drawing.Imaging.ImageFormat.Png);
            return iTextSharp.text.Image.GetInstance(tempFilePath);
        }
        public System.Drawing.Image GetImageFromUrl(string url)
        {

            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath(ConfigurationManager.AppSettings["logFolderPath"].ToString() + "PrintLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
            {
                //sw.WriteLine("old:" + oldurl);
                sw.WriteLine("URL:" + url);
            }
            WebRequest req = WebRequest.Create(url);
            WebResponse response = req.GetResponse();
            Stream stream = response.GetResponseStream();
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            stream.Close();
            return img;
        }
        public static PrintMap GetPrintMapBySelectedArea(PrintMap printMap)
        {
           
            MapPrintManager printManager = new MapPrintManager(printMap.mapCanvasWidth, printMap.mapCanvasHeight, (double)printMap.mapLeftLng, (double)printMap.mapRightLng, (double)printMap.mapTopLat, (double)printMap.mapBottomLat, printMap.mapCanvasWidth, printMap.mapCanvasHeight, printMap.mapCurrentZoom);
            string[] geomSplit = printMap.mapSelectedGeom.Split(',');
            double maxLat = -360, minLat = 360, maxLong = -360, minLong = 360;

            foreach (var point in geomSplit)
            {
                string[] mark = point.Split(' ');
                double lng = Convert.ToDouble(mark[0]);
                double lat = Convert.ToDouble(mark[1]);
                if (lng < minLong) minLong = lng;
                if (lng > maxLong) maxLong = lng;
                if (lat < minLat) minLat = lat;
                if (lat > maxLat) maxLat = lat;
            }
            MapPoint leftTop = printManager.ConvertGeoToPixel(maxLat, minLong);
            MapPoint rightBottom = printManager.ConvertGeoToPixel(minLat, maxLong);
            printMap.mapCanvasHeight = (int)Math.Abs((Math.Abs(rightBottom.Y) - Math.Abs(leftTop.Y)));
            printMap.mapCanvasWidth = (int)Math.Abs((Math.Abs(rightBottom.X) - Math.Abs(leftTop.X)));
            printMap.mapLeftLng = minLong;
            printMap.mapRightLng = maxLong;
            printMap.mapTopLat = maxLat;
            printMap.mapBottomLat = minLat;
            return printMap;
        }
        private double GetScaleRatio(PrintMap model)
        {
            //string result = "";
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(model.mapLeftLng, model.mapTopLat, model.mapRightLng, model.mapTopLat);
            double ratio = distance / model.mapCanvasWidth;
            return ratio;
        }
        private double GetMapDistance(PrintMap model)
        {
            //string result = "";
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(model.mapLeftLng, model.mapTopLat, model.mapRightLng, model.mapTopLat);
            // double ratio = distance / model.mapCanvasWidth;
            return distance;
        }
        private List<layerUrl> GetLayerURLs(PrintMap model)
        {
            List<layerUrl> urls = new List<layerUrl>();

            layerUrl layerList = null;
            foreach (var layer in model.layerFilters)
            {
                if (layer.Name != null)
                {
                    layerList = new layerUrl();
                    string mapFilePath = layer.MapFilePath;
                    if (model.printScale == 2)
                    {
                        string[] path = mapFilePath.Split('.');
                        if (path.Length > 1)
                            mapFilePath = string.Format("{0}-Scale2.{1}", path[0], path[1]);
                    }
                    layerList.url = string.Format("{0}MAP={1}&srs=EPSG:4326&version=1.0.0&REQUEST=GetMap&LAYERS={2}{3}&service=wms&BBOX={4}&WIDTH={5}&HEIGHT={6}&FORMAT=image/png&reqver=0",
               ApplicationSettings.LocalMapServerURL, mapFilePath, layer.Name, addFilter(layer.filters), model.mapCordinates, model.mapCanvasWidth, model.mapCanvasHeight);
                    urls.Add(layerList);
                }

            }

            return urls;
        }
        private string addFilter(List<Models.Filters> filters)
        {
            string filterexp = "";

            foreach (var itme in filters)
            {
                filterexp += '&' + itme.Field + '=' + itme.value;
            }
            return filterexp;
        }

        public JsonResult GetSaveTemplateData(int p_id)
        {
            var strMsg = "";
            try
            {
                var usrDetail = (User)Session["userDetail"];
                BLPrintSavedTemplate blp = new BLPrintSavedTemplate();
                var listitm = blp.GetPrintTemplateList(p_id, usrDetail.user_id);
                return Json(new { status = "success", success = listitm }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                strMsg = "Something went wrong";
                // ErrorLogHelper.WriteErrorLog("Get_SaveTemplateData()", "Print", ex);
            }
            return Json(new { status = "fail", error = strMsg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult validateTemplateText(string searchText)
        {
            JsonResponse<Dictionary<string, string>> objResp = new JsonResponse<Dictionary<string, string>>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                BLPrintSavedTemplate blp = new BLPrintSavedTemplate();
                var IsExists = blp.ValidatePrintTemplate(searchText, usrDetail.user_id);
                if (IsExists.Count > 0)
                {
                    objResp.result = IsExists;
                    objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.OK.ToString();           
                }

            }
            catch (Exception)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult _validateTemplateByText(string _Txtname)
        {
            JsonResponse<Dictionary<string, string>> objResp = new JsonResponse<Dictionary<string, string>>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                BLPrintSavedTemplate blp = new BLPrintSavedTemplate();
                var valTxt = blp.ValidatePrintTemplate(_Txtname, usrDetail.user_id);
                if (valTxt.Count > 0)
                {
                    objResp.result = valTxt;
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
                }

            }
            catch (Exception)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        private int SavedPrintTemplates(PrintMap printMap)
        {
            var usrDetail = (User)Session["userDetail"];
            PrintSavedTemplate objt = new PrintSavedTemplate()
            {
                id = printMap.printMapAttr.templateId,
                template_name = printMap.printMapAttr.Template_name,
                user_id = usrDetail.user_id,
                job_id = printMap.printMapAttr.jobId,
                department = printMap.printMapAttr.department,
                plotted_by = printMap.printMapAttr.plottedBy,
                team = printMap.printMapAttr.team,
                drawn_by = printMap.printMapAttr.drawnBy,
                checked_by = printMap.printMapAttr.checkedBy,
                rechecked_by = printMap.printMapAttr.recheckedBy,
                approved_by = printMap.printMapAttr.approvedBy,
                x_document_index = printMap.printMapAttr.X_Document_Index,
                y_document_index = printMap.printMapAttr.Y_Document_Index,
                phase = printMap.printMapAttr.phase,
                plan = printMap.printMapAttr.plan,
                prov_dir = printMap.printMapAttr.provDir,
                fdc_no = printMap.printMapAttr.FDCNo,
                olt = printMap.printMapAttr.OLT

            };

            BLPrintSavedTemplate bls = new BLPrintSavedTemplate();
            int status = bls.SaveTemplate(objt);
            return status;
        }

        public string GetNetworkIdForJobID(int systemId, string entityType)
        {

            var NetworkId =new BLMisc().GetNetworkIdForJobID(systemId, entityType);
            return NetworkId;
        }
        public bool DeletePrintTemplateList(int templateId)
        {
            var usrDetail = (User)Session["userDetail"];
            bool response = new BLPrintSavedTemplate().DeleteTemplate(templateId, usrDetail.user_id);
            return response;
        }
        public JsonResult Deleteexportlog(int p_id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var result = new BLPrintLog().DeleteexportlogById(p_id, Convert.ToInt32(Session["user_id"]));
                if (result)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Current log deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting log!";
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Something went wrong while deleting log!";
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

    }
}