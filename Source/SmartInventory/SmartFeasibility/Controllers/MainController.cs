using BusinessLogics;
using BusinessLogics.Feasibility;
using Models;
using Models.Feasibility;
using NPOI.SS.UserModel;
using SmartFeasibility.Filters;
using SmartFeasibility.Helper;
using SmartFeasibility.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utility;
using static Models.Feasibility.FeasibilityKMLData;

namespace SmartFeasibility.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult test(int id)
        {
            return View();
        }

        public ActionResult Index()
        {

            var usrDetail = (User)Session["userDetail"];
            if (usrDetail != null)
            {
                if (usrDetail.role_id == 1)
                {
                    return RedirectToAction("index", "UnAuthorized");
                }
                var usrId = usrDetail.user_id;
                var role_Id = usrDetail.role_id;
                if (!string.IsNullOrEmpty(Convert.ToString(usrId)))
                {
                    FeasibilityViewModel objMain = new FeasibilityViewModel();
                    objMain.userDetail = usrDetail;

                    objMain.lstCableTypes = new BLFeasibilityCableType().getFeasibilityCableTypesddl();
                    var demarcations = new BLFeasibilityDemarcationType().getFeasibilityDemarcationTypes();
                    objMain.layerView.NELayers = new FeasibilitySettingsBL().GetNEntityLayers();
                    if (demarcations != null)
                    {
                        foreach (FeasibilityDemarcationType demarcation in demarcations)
                        {
                            if (demarcation.name.Equals("inside", StringComparison.OrdinalIgnoreCase))
                            {
                                objMain.color_inside = demarcation.color;
                            }
                            else if (demarcation.name.Equals("inside_A", StringComparison.OrdinalIgnoreCase))
                            {
                                objMain.color_inside_A = demarcation.color;
                            }
                            else if (demarcation.name.Equals("inside_P", StringComparison.OrdinalIgnoreCase))
                            {
                                objMain.color_inside_P = demarcation.color;
                            }
                            else if (demarcation.name.Equals("outside", StringComparison.OrdinalIgnoreCase))
                            {
                                objMain.color_Outside = demarcation.color;
                            }
                            else if (demarcation.name.Equals("lmc_start", StringComparison.OrdinalIgnoreCase))
                            {
                                objMain.color_LMC_Start = demarcation.color;
                            }
                            else if (demarcation.name.Equals("lmc_end", StringComparison.OrdinalIgnoreCase))
                            {
                                objMain.color_LMC_End = demarcation.color;
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(Convert.ToString(usrId)))
                    {

                        BLLayer objBLLayer = new BLLayer();
                        objMain.layerView.lstNetworkLayers = objBLLayer.GetNetworkLayers(usrId, 0, role_Id);
                        objMain.layerView.lstRegionProvinceLayers = objBLLayer.GetRegionProvinceLayers(usrId);
                        objMain.layerView.userDetail = usrDetail;
                        objMain.layerView.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());
                        Session["ApplicableModuleListForFeasibility"] = objMain.layerView.lstUserModule;
                    }

                    return View(objMain);
                }
            }
            return RedirectToAction("index", "Login");
            //return View("Login/Index");
        }
        public JsonResult GetNetworkStatuses(string systemIds)
        {
            string cableStatuses = "";
            try
            {
                foreach (string id in systemIds.Split(','))
                {
                    CableMaster cable = new BLMisc().GetEntityDetailById<CableMaster>(Convert.ToInt32(id), EntityType.Cable);
                    cableStatuses += cable.network_status;
                }

                if (cableStatuses.Length > 0)
                    return Json(new { status = "success", cableStatuses = cableStatuses }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ErrorLogHelper.WriteErrorLog("GetNetworkStatuses", "MainController", e);
                return Json(new { status = "error", message = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult MapManager()
        {
            return PartialView("_MapManager");
        }

        public JsonResult getRoutes(string src, string desti, double sourceBuffer, double destiBuffer, int p_core_required)
        {
            try
            {
                //List <DirectionsDetail> res = new RouteAPIHelper().getRoutes(src, desti, sourceBuffer, destiBuffer);
                List<RoutingDetail> res = new BLFeasibilityRouting().getRoutingDirections(src, desti, Convert.ToInt32(sourceBuffer), Convert.ToInt32(destiBuffer), p_core_required);

                // update cable details
                foreach (RoutingDetail route in res)
                {
                    int availableCores = new BLCable().GetAvailableCores(Convert.ToInt32(route.edge_TargetID));
                    CableMaster cable = new BLMisc().GetEntityDetailById<CableMaster>(Convert.ToInt32(route.edge_TargetID), EntityType.Cable);
                    route.network_id = cable.network_id;
                    route.cable_name = cable.cable_name;
                    route.cable_status = cable.network_status;
                    route.available_cores = availableCores;
                    route.total_cores = cable.total_core;
                }

                return Json(new { status = "success", directions = res });
            }
            catch (Exception e)
            {
                ErrorLogHelper.WriteErrorLog("getRoutes", "MainController", e);
                return Json(new { status = "error", message = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetCableType()
        {
            FeasibilityCableType model = new FeasibilityCableType();
            return PartialView("_cabletype", model);
        }

        public JsonResult GetCablesDetails(string systemIds)
        {
            //string cableStatuses = "";
            ArrayList cableStatuses = new ArrayList();
            try
            {
                foreach (string id in systemIds.Split(','))
                {
                    int availableCores = new BLCable().GetAvailableCores(Convert.ToInt32(id));
                    CableMaster cable = new BLMisc().GetEntityDetailById<CableMaster>(Convert.ToInt32(id), EntityType.Cable);
                    // cableStatuses += cable.network_status;
                    cableStatuses.Add(new
                    {
                        cable_id = id,
                        network_id = cable.network_id,
                        cable_name = cable.cable_name,
                        cable_status = cable.network_status,
                        available_cores = availableCores,
                        total_cores = cable.total_core
                    });
                }

                if (cableStatuses.Count > 0)
                    return Json(new { status = "success", cableStatuses = cableStatuses }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ErrorLogHelper.WriteErrorLog("GetCablesDetails", "MainController", e);
                return Json(new { status = "error", message = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult SaveCableType(FeasibilityCableType objCableType)
        {
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;

            if (objCableType.cable_type_id == 0)
            {
                objCableType.IsExists = new BLFeasibilityCableType().CheckDisplayNameExist(0, objCableType.display_name);
            }
            if (!objCableType.IsExists)
            {
                objCableType = new BLFeasibilityCableType().saveFeasibilityCableTypes(objCableType, usrId);
            }

            return PartialView("_CableType", objCableType);
        }

        public JsonResult deleteCableType(int systemId)
        {
            PageMessage objPM = new PageMessage();
            var response = new BLFeasibilityCableType().deleteCableType(systemId);
            if (response)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = Resources.Resources.SF_GBL_GBL_NET_FRM_091;
            }
            return Json(new { Data = objPM, JsonRequestBehavior.AllowGet });
        }

        public JsonResult hasAccess(string moduleAbbr)
        {
            User user = (User)Session["userDetail"];

            bool hasAccess = ModuleAccessHelper.FeasibilityModuleAccess(user, moduleAbbr);

            PageMessage objPM = new PageMessage();
            if (hasAccess)
            {
                objPM.status = ResponseStatus.OK.ToString();
            }
            else
            {
                objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
            }

            return Json(new { Data = objPM, JsonRequestBehavior.AllowGet });
        }

        public ActionResult GetCableTypeLst(FeasibilityCableTypeViewModel model, int page = 0, string sort = "", string sortdir = "")
        {
            model.lstSearchBy = GetCableTypeSearchByColumns();
            model.objGridAttributes.pageSize = 5;
            model.objGridAttributes.currentPage = page == 0 ? 1 : page;
            model.objGridAttributes.sort = sort;
            model.objGridAttributes.orderBy = sortdir;
            model.lstCableTypes = new BLFeasibilityCableType().getFeasibilityCableTypes(model.objGridAttributes);
            model.objGridAttributes.totalRecord = model.lstCableTypes != null && model.lstCableTypes.Count > 0 ? model.lstCableTypes[0].totalRecords : 0;

            return PartialView("_CableTypeLst", model);
        }

        public ActionResult GetCableTypeSearchResult(string SearchText)
        {

            List<FeasibiltiyCablesearch> lstEquipment = new List<FeasibiltiyCablesearch>();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                lstEquipment = new BLFeasibilityCableType().GetSearchEquipmentResult(SearchText.ToUpper());
            }

            return Json(lstEquipment, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHistory(ViewHistoryMaster objAudit, int systemId = 0, string eType = "feasibility_cable_type", int page = 0, string sort = "", string sortdir = "")
        {

            objAudit.lstSearchBy = GetCableTypeSearchByColumns();
            objAudit.systemId = systemId;
            objAudit.eType = eType;
            objAudit.objFilterAttributes.pageSize = 10;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.systemid = systemId;
            objAudit.objFilterAttributes.entityType = eType;

            List<Dictionary<string, string>> lstReportData = new BLFeasibilityCableType().GetFeasibilityHistory(systemId, eType.ToUpper(), objAudit.objFilterAttributes);
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            foreach (Dictionary<string, string> dic in lstReportData)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();

                string[] arrIgnoreColumns = { "AUDIT_ID", "TOTALRECORDS", "CABLE_TYPE_ID", "S_NO" };

                foreach (var col in dic)
                {

                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        if (col.Key.ToUpper() == "SF_GBL_GBL_JQ_FRM_002")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_JQ_FRM_002, col.Value);
                        }
                        else if (col.Key.ToUpper() == "SF_GBL_GBL_NET_FRM_027")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_NET_FRM_027, col.Value);
                        }
                        else if (col.Key.ToUpper() == "SF_GBL_GBL_JQ_FRM_003")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_JQ_FRM_003 + " (" + ApplicationSettings.CustomerCurrency + ")", col.Value);
                        }
                        else if (col.Key.ToUpper() == "SF_GBL_GBL_JQ_FRM_004")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_JQ_FRM_004 + " (" + ApplicationSettings.CustomerCurrency + ")", col.Value);
                        }
                        else if (col.Key.ToUpper() == "SF_GBL_GBL_NET_FRM_036")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_NET_FRM_036, col.Value);
                        }
                        else if (col.Key.ToUpper() == "SF_GBL_GBL_JQ_FRM_009")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_JQ_FRM_009, col.Value);
                        }
                        else if (col.Key.ToUpper() == "SF_GBL_GBL_JQ_FRM_006")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_JQ_FRM_006, col.Value);
                        }
                        else if (col.Key.ToUpper() == "SF_GBL_GBL_JQ_FRM_007")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_JQ_FRM_007, col.Value);
                        }
                        else if (col.Key.ToUpper() == "SF_GBL_GBL_JQ_FRM_008")
                        {
                            obj.Add(Resources.Resources.SF_GBL_GBL_JQ_FRM_008, col.Value);
                        }




                    }

                }

                objAudit.lstData.Add(obj);
            }


            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;


            return PartialView("_FeasibilityHistory", objAudit);
        }

        public ActionResult GetMapLayer()
        {
            MainViewModel objMain = new MainViewModel();
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            if (!string.IsNullOrEmpty(Convert.ToString(usrId)))
            {

                BLLayer objBLLayer = new BLLayer();
                objMain.lstNetworkLayers = objBLLayer.GetNetworkLayers(usrId, 0);
                objMain.lstRegionProvinceLayers = objBLLayer.GetRegionProvinceLayers(usrId);
                objMain.userDetail = usrDetail;
            }
            return PartialView("_MapLayer", objMain);
        }

        public List<KeyValueDropDown> GetCableTypeSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Core", value = "cores" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Display Name", value = "display_name" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }
        public void ExportBomRecord(int systemId)
        {
            var listBomRecord = new BLFeasibilityCableType().getBomExportData<Dictionary<string, string>>(systemId);
            DataTable dtlogs = MiscHelper.GetDataTableFromDictionaries(listBomRecord);
            dtlogs.Columns["material_price_per_unit"].ColumnName = "Material Price Per Unit (" + ApplicationSettings.CustomerCurrency + ")";
            dtlogs.Columns["service_price_per_unit"].ColumnName = "Service Price Per Unit (" + ApplicationSettings.CustomerCurrency + ")";
            dtlogs.Columns["created_user"].ColumnName = "Created By";
            dtlogs.Columns["modified_user"].ColumnName = "Modified By";

            foreach (DataColumn col in dtlogs.Columns)
            {
                col.ColumnName = MiscHelper.ToCamelCase(col.ColumnName);
            }
            dtlogs.TableName = "BOM_Settings";
            ExportData(dtlogs, "BOM_Settings_" + DateTimeHelper.Now.ToString("ddMMyyyy") + DateTimeHelper.Now.ToString("HHmmss"));
        }
        public void ExportBomHistory(int systemId)
        {
            string eType = "feasibility_cable_type";
            ViewHistoryMaster objAudit = new ViewHistoryMaster();
            objAudit.lstSearchBy = GetCableTypeSearchByColumns();
            objAudit.systemId = systemId;
            objAudit.eType = eType;
            objAudit.objFilterAttributes.pageSize = 5;
            objAudit.objFilterAttributes.currentPage = 0;
            objAudit.objFilterAttributes.sort = "audit_id";
            objAudit.objFilterAttributes.orderBy = "desc";
            objAudit.objFilterAttributes.systemid = systemId;
            objAudit.objFilterAttributes.entityType = eType;

            List<Dictionary<string, string>> lstReportData = new BLFeasibilityCableType().GetFeasibilityHistory(systemId, eType.ToUpper(), objAudit.objFilterAttributes);
            List<Dictionary<string, string>> lstFinalData = new List<Dictionary<string, string>>();
            //  var listBomHistoryRecord = new BLFeasibilityCableType().getBomHistoryData<Dictionary<string, string>>(systemId);
            DataTable dtlogs = MiscHelper.GetDataTableFromDictionaries(lstReportData);

            string[] arrIgnoreColumns = { "totalrecords", "s_no" };
            foreach (var IgnoreColumn in arrIgnoreColumns)
            {
                if (dtlogs.Columns.Contains(IgnoreColumn))
                {
                    dtlogs.Columns.Remove(IgnoreColumn);
                }
            }
            string Display_Name = "";
            foreach (DataColumn col in dtlogs.Columns)
            {
                if (col.ColumnName.ToUpper() == "SF_GBL_GBL_JQ_FRM_002")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_002;
                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_NET_FRM_027")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_NET_FRM_027;

                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_JQ_FRM_003")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_003 + " (" + ApplicationSettings.CustomerCurrency + ")";
                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_JQ_FRM_004")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_004 + " (" + ApplicationSettings.CustomerCurrency + ")";
                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_NET_FRM_036")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_NET_FRM_036;
                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_JQ_FRM_009")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_009;
                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_JQ_FRM_006")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_006;
                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_JQ_FRM_007")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_007;
                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_JQ_FRM_008")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_008;
                }
                else if (col.ColumnName.ToUpper() == "SF_GBL_GBL_JQ_FRM_005")
                {
                    col.ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_005;
                }
            }
            dtlogs.TableName = "BOM_History_Report";
            ExportData(dtlogs, string.Format("BOM_History_[{0}]_timestamp", dtlogs.Rows[0][1].ToString()));
        }

        private void ExportData(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }

        public string GetMatchedAddresses(string addString)
        {
            var retTxt = "{\"status\":false,\"data\":\"\"}";
            try
            {
                var searchResult = new FeasibilitySettingsBL().GetMatchedAddressList(addString);

                //Return json type data of image name from database
                retTxt = "{\"status\":true,\"data\":[";
                if (searchResult.Rows.Count > 0)
                {
                    for (int i = 0; i < searchResult.Rows.Count; i++)
                    {
                        retTxt += "{\"" + "buidingCode" + "\":" + "\"" + cleanForJSON(searchResult.Rows[i][0].ToString()) + "\",";
                        retTxt += "\"" + "address" + "\":" + "\"" + cleanForJSON(searchResult.Rows[i][1].ToString()) + "\",";
                        retTxt += "\"" + "latlng" + "\":" + "\"" + searchResult.Rows[i][2] + "\"},";
                    }
                    retTxt = retTxt.Substring(0, retTxt.Length - 1);
                }
                retTxt += "]}";

            }
            catch (Exception ex)
            {
                retTxt = "{\"status\":false,\"data\":\"\"}";
            }
            return retTxt;

        }

        private string cleanForJSON(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        [HttpPost]
        public JsonResult GetNEntityDetails(string custLoc, string radius, string[] lyrsTbl, string[] lyrsName)
        {
            if (lyrsName == null || lyrsTbl == null)
                return Json(new { Status = false, Data = "Parameters values not defined." }, JsonRequestBehavior.AllowGet);

            var result = new FeasibilitySettingsBL().GetNEntityLyrsDetails(custLoc, radius, lyrsName, lyrsTbl);
            if (result != null && result.Count > 0)
                return Json(new { Status = true, Data = result }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Status = false, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public string GetBuildingsHere(string centerLatLngPoints, string rdsValue)
        {
           
            var result = new FeasibilitySettingsBL().GetBuildingsHere(centerLatLngPoints, rdsValue);
            return result;
        }

        public void SaveFeasibilityForFtth(string searchLoc, string locAdd, string locPoints)
        {
            BrowserHelper browserHelper = new BrowserHelper();
            var frmBrwsr = browserHelper.BrowserName + ": " + browserHelper.BrowserVersion;
            var frmMachine = IPHelper.GetHostName();
            var frmIpAdd = IPHelper.GetIPAddress();
            new FeasibilitySettingsBL().SaveFeasiblityHistory(searchLoc, locAdd, locPoints, frmBrwsr, frmMachine, frmIpAdd);
        }

        public PartialViewResult GetLegendManagerPartial()
        {
            return PartialView("~/Views/Main/_LegendManager.cshtml");
        }

        [HttpPost]
        public ActionResult GetEntitySearchResult(string SearchText)
        {
           var usrDetail = (User)Session["userDetail"];
            BLSearch objBLSearch = new BLSearch();
            List<SearchResult> lstSearchResult = new List<SearchResult>();
            var serchvalue = SearchText.TrimEnd();
            if (!string.IsNullOrWhiteSpace(serchvalue))
            {
                var arrSrchText = serchvalue.Split(new[] { ':' }, 2);
                if (arrSrchText.Length == 2)
                {
                    if (arrSrchText[1].Length > 0)
                    {
                        int searchTest = 0;
                        if (int.TryParse(arrSrchText[1], out searchTest))
                        {
                            lstSearchResult = objBLSearch.GetSearchEntityResult(arrSrchText[0], arrSrchText[1], usrDetail.user_id, "");
                        }
                        else if (arrSrchText[1].Length >= ApplicationSettings.EntitySearchLength)
                        {
                            lstSearchResult = objBLSearch.GetSearchEntityResult(arrSrchText[0], arrSrchText[1], usrDetail.user_id, "");
                        }
                    }
                }
                else
                {
                    lstSearchResult = objBLSearch.GetSearchEntityType(arrSrchText[0], usrDetail.role_id);
                }
            }
            return Json(new { geonames = lstSearchResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getGeometryDetail(GeomDetailIn objGeomDetailIn)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            var objGeometryDetail = new BLSearch().GetGeometryDetails(objGeomDetailIn);


            if (objGeometryDetail.geometry_extent != null)
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

    }
}
