using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class OSPSettingsController : Controller
    {

        #region INFO COLUMNS SETTINGS
        // GET: Admin/AdvancedSettings
        public ActionResult InfoSettings(int id = 0)
        {

            vwLayerColumnSettings objVwLayerColumnSettings = new vwLayerColumnSettings();
            if (id != 0)
            {
                objVwLayerColumnSettings.lstLayerColumns = BLAdvancedSettings.Instance.GetLayerColumnSettings(id, LayerSettingType.Info.ToString());
                objVwLayerColumnSettings.layer_id = id;
            }
            objVwLayerColumnSettings.lstLayers = new BLLayer().GetInfoLayers();
            return View("InfoSettings", objVwLayerColumnSettings);
        }

        public ActionResult SaveInfoSettings(vwLayerColumnSettings objLayerColumnSettings)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            objLayerColumnSettings.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            objLayerColumnSettings.settingType = LayerSettingType.Info.ToString();
            var status = BLAdvancedSettings.Instance.SaveLayerColumnSettings(objLayerColumnSettings);
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Info settings updated successfully!";
            objLayerColumnSettings.pageMsg = objMsg;
            objLayerColumnSettings.lstLayers = new BLLayer().GetInfoLayers();
            objLayerColumnSettings.lstLayerColumns = BLAdvancedSettings.Instance.GetLayerColumnSettings(objLayerColumnSettings.layer_id, LayerSettingType.Info.ToString());
            return View("InfoSettings", objLayerColumnSettings);
        }
        #endregion
        #region SEARCH SETTINGS
        public ActionResult SearchSettings(int id = 0)
        {
            SearchSetting objSearchSetting = new SearchSetting();
            if (id != 0)
            {
                objSearchSetting.lstSearchAttributes = BLSearchSetting.Instance.GetLayerSearchAttributes(id);
                objSearchSetting.layer_id = id;
            }
            objSearchSetting.lstLayers = new BLLayer().GetOSPLayers();
            return View(objSearchSetting);


        }
        [HttpPost]
        public ActionResult SaveSearchSettings(SearchSetting objSearchSettings)
        {
            PageMessage objMsg = new PageMessage();

            // prepare comma seperated selected columns list
            if (objSearchSettings != null && objSearchSettings.lstSearchAttributes != null & objSearchSettings.lstSearchAttributes.Count > 0)
            {
                var arrSelectedColumns = objSearchSettings.lstSearchAttributes.Where(m => m.is_selected == true).Select(s => s.column_name).ToArray();
                if (arrSelectedColumns.Length > 0)
                {
                    objSearchSettings.search_columns = arrSelectedColumns != null ? string.Join(",", arrSelectedColumns) : "";
                    // update layer attributes in Search setting table...
                    objSearchSettings.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                    objSearchSettings = BLSearchSetting.Instance.SaveSearchSetting(objSearchSettings);
                    if (objSearchSettings != null)
                    {
                        //success
                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.message = "Search settings updated successfully!";
                    }
                    else
                    {
                        //failure
                        objMsg.status = ResponseStatus.FAILED.ToString();
                        objMsg.message = "Error in updating Search settings!";
                    }
                }
                else
                {
                    //failure
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "No attribute selected for entity!";
                }
            }
            else
            {
                //failure
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Error in updating Search settings!";
            }

            objSearchSettings.pageMsg = objMsg;
            objSearchSettings.lstLayers = new BLLayer().GetOSPLayers();

            return View("SearchSettings", objSearchSettings);
        }

        #endregion

        #region ZOOM SETTINGS
        public ActionResult Zoom()
        {
            List<layerDetail> objlayer = new BLLayer().GetAllOSPLayers();

            return View(objlayer);
        }



        [HttpPost]
        public ActionResult SaveLayerZoom(List<layerDetail> objlayer)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            objlayer = new BLLayer().SaveLayerDetails(objlayer);
            objMsg.status = ResponseStatus.OK.ToString();
            //objMsg.isNewEntity = isNew;
            objMsg.message = "layerDetail Saved successfully.";
            return View("Zoom", objlayer);
        }

        #endregion



        #region LABEL SETTINGS
        public ActionResult labelSettings(int id = 0)
        {
            LabelSetting objLabelSettings = new LabelSetting();
            BindAttribute(id);
            if (id != 0)
            {

                objLabelSettings.lstLabelAttributes = BLLabelSetting.Instance.GetLayerLabelAttributes(id);
                objLabelSettings.layer_id = id;
            }
            objLabelSettings.lstLayers = new BLLayer().GetOSPLayers();
            return View("LabelSettings", objLabelSettings);
        }



        public JsonResult BindAttributOnChange(int layer_id)
        {
            var objResp = BindAttribute(layer_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public IList<AttributeDetail> BindAttribute(int layer_id = 0)
        {
            LabelSetting objLabelSettings = new LabelSetting();
            return objLabelSettings.lstLabelAttributes = BLLabelSetting.Instance.GetLayerLabelAttributes(layer_id);
        }

        [HttpPost]
        public ActionResult SaveLabelSettings(LabelSetting objLabelSettings)
        {
            PageMessage objMsg = new PageMessage();
            List<Status> message = new List<Status>();


            // update layer attributes in info setting table...
            objLabelSettings.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            message = BLLabelSetting.Instance.UpdateLabelSettingView(Convert.ToString(objLabelSettings.layer_id), objLabelSettings.label_columns).ToList();
            if (message[0].status == "ok")
            {
                objLabelSettings = BLLabelSetting.Instance.SaveLabelSetting(objLabelSettings);

                if (objLabelSettings != null)
                {
                    //success

                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Label settings updated successfully!";
                }
                else
                {
                    //failure
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "Error in updating Label settings!";
                }


            }

            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = message[0].message;

            }


            objLabelSettings.pageMsg = objMsg;
            objLabelSettings.lstLayers = new BLLayer().GetOSPLayers();
            objLabelSettings.lstLabelAttributes = BindAttribute(objLabelSettings.layer_id).ToList();

            return View("LabelSettings", objLabelSettings);
        }

        #endregion

        #region TUBE COLOR SETTINGS

        public ActionResult TubeCoreColorSettings(string type = "")
        {
            ModelCableColorSettings objCableColor = new ModelCableColorSettings();
            List<CableColorSettings> lstCableColors = !string.IsNullOrEmpty(type) ? new BLTubeCoreColorSettings().GetTubeColorSettings(0, type) : new List<CableColorSettings>();
            objCableColor.lstCableColor = lstCableColors;
            objCableColor.type = type;
            return View("TubeColorSettings", objCableColor);
        }
        public JsonResult getTotalColorCount(string type)
        {
            var colors = new BLTubeColorSettings().getTotalColorCount(type);
            return Json(colors, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCableById(int colorid)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var objTube = new BLTubeColorSettings().DeleteCableCoreById(colorid);
                if (objTube == 1)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "successfully delete !!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Tube color !!";
                }


            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Something went wrong while process !!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveCableCoreColorSettings(ModelCableColorSettings objModel)
        {

            // ModelCableColorSettings objCableColor = new ModelCableColorSettings();
            PageMessage objMsg = new PageMessage();
            try
            {
                if (objModel.lstCableColor.Count > 0)
                {
                    objModel.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                    var result = new BLTubeColorSettings().SaveTubeCoreColorInfo(objModel);

                    if (result)
                    {
                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.message = "Colors Saved Successfully!";
                    }
                    else
                    {
                        objMsg.status = ResponseStatus.FAILED.ToString();
                        objMsg.message = "Failed to Update Colors!!";
                    }
                }
            }
            catch
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while processing your request!!";
            }
            objModel.pageMsg = objMsg;

            List<CableColorSettings> lstCableColors = new BLTubeCoreColorSettings().GetTubeColorSettings(0, objModel.type);
            objModel.lstCableColor = lstCableColors;
            return View("TubeColorSettings", objModel);
        }
        #endregion

        #region CableMapColorSettings
        public ActionResult ViewCableMapColorSettings(VwCableMapColorSettings objVwCablColor, int page = 0, string sort = "", string sortdir = "")
        {
            BindCableMapClrDropdown(objVwCablColor);
            objVwCablColor.objCableColorSettingsFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objVwCablColor.objCableColorSettingsFilter.currentPage = page == 0 ? 1 : page;
            objVwCablColor.objCableColorSettingsFilter.sort = sort;
            objVwCablColor.objCableColorSettingsFilter.orderBy = sortdir;
            objVwCablColor.lstViwCableColorSetting = new BLCableColorSettings().GetCableMapColorSettingsList(objVwCablColor.objCableColorSettingsFilter);
            objVwCablColor.objCableColorSettingsFilter.totalRecord = objVwCablColor.lstViwCableColorSetting.Count > 0 ? objVwCablColor.lstViwCableColorSetting[0].totalRecords : 0;
            Session["viewCableMapColorFilters"] = objVwCablColor.objCableColorSettingsFilter;
            return View("ViewCableMapColorSettings", objVwCablColor);
        }
        [HttpGet]
        public void ExportCableMapColorDetails()
        {
            if (Session["viewCableMapColorFilters"] != null)
            {
                VwCableMapColorSettingsFilter objViewFilter = (VwCableMapColorSettingsFilter)Session["viewCableMapColorFilters"];
                List<CableMapColorSettingVw> lstViewCableDetails = new List<CableMapColorSettingVw>();
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstViewCableDetails = new BLCableColorSettings().GetCableMapColorSettingsList(objViewFilter);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<CableMapColorSettingVw>(lstViewCableDetails);
                dtReport.TableName = "Cable_Color_Setting_Details";
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_ON");

                dtReport.Columns["cable_type"].SetOrdinal(0);
                dtReport.Columns["cable_category"].SetOrdinal(1);
                dtReport.Columns["fiber_count"].SetOrdinal(2);
                dtReport.Columns["color_code"].SetOrdinal(3);
                dtReport.Columns["created_by_text"].SetOrdinal(4);
                dtReport.Columns["created on"].SetOrdinal(5);
                dtReport.Columns["modified_by_text"].SetOrdinal(6);
                dtReport.Columns["modified on"].SetOrdinal(7);

                dtReport.Columns["cable_type"].ColumnName = "Cable Type";
                dtReport.Columns["cable_category"].ColumnName = "Cable Category";
                dtReport.Columns["fiber_count"].ColumnName = "Fiber Count";
                dtReport.Columns["color_code"].ColumnName = "Color Code";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["created on"].ColumnName = "Created On";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                dtReport.Columns["modified on"].ColumnName = "Modified On";
                var filename = "CableMapColorSettingDetails";
                ExportDataTableToExcel(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        private void ExportDataTableToExcel(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }
        public ActionResult CableMapColorSettings(int id)
        {
            CableMapColorSettings objCableClrStng = id > 0 ? new BLCableColorSettings().GetCablMapColorDetailByID(id) : new CableMapColorSettings();
            BindCableMapClrDropdown(objCableClrStng);
            return PartialView("CableMapColorSetting", objCableClrStng);
        }
        private void BindCableMapClrDropdown(dynamic obj)
        {
            var lstdllitem = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
            obj.lstFiberCount = lstdllitem.Where(x => x.dropdown_type == DropDownType.Fiber_Count.ToString()).ToList();
            obj.lstCableCategory = lstdllitem.Where(o => o.dropdown_type == DropDownType.Cable_Category.ToString()).ToList();
            obj.lstCableType = lstdllitem.Where(o => o.dropdown_type == DropDownType.Cable_Type.ToString()).ToList();
        }

        [HttpPost]
        public JsonResult DeleteCableMapColourSettingsById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BLCableColorSettings().DeleteCableColourSettingsById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Cable Map Color Setting deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Cable Map Color Setting!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Cable Map Color Setting not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveCableMapColorSettings(CableMapColorSettings _objaddnewcblcolr)
        {
            ModelState.Clear();
            var user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();
            if (_objaddnewcblcolr.lstfiberCounts == null)
            {
                _objaddnewcblcolr.lstfiberCounts = new BLMisc().GetDropDownList(EntityType.Cable.ToString()).Where(o => o.dropdown_type == "Fiber_Count").Select(s => s.dropdown_value).ToList();
            }
            if (_objaddnewcblcolr.lstCableCategorys == null)
            {
                _objaddnewcblcolr.lstCableCategorys = new BLMisc().GetDropDownList(EntityType.Cable.ToString()).Where(o => o.dropdown_type == "Cable_Category").Select(s => s.dropdown_key).ToList();
            }
            foreach (var strCategory in _objaddnewcblcolr.lstCableCategorys)
            {
                foreach (var fiberCount in _objaddnewcblcolr.lstfiberCounts)
                {
                    CableMapColorSettings obj1 = new CableMapColorSettings();
                    obj1.cable_type = _objaddnewcblcolr.cable_type;
                    obj1.cable_category = strCategory;
                    obj1.fiber_count = Convert.ToInt32(fiberCount);
                    obj1.color_code = _objaddnewcblcolr.color_code;
                    obj1.user_id = user_id;
                    var status = new BLCableColorSettings().SaveCableMapColorSettingDetails(obj1);
                }
            }
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Cable Map Color Settings updated successfully!";
            _objaddnewcblcolr.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}