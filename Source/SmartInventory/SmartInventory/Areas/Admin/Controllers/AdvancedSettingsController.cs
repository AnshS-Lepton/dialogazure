using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics.Admin;
using BusinessLogics;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System.Threading.Tasks;
using System.Data;
using Utility;
using System.IO;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using System.Xml.Linq;
using System.Collections;
using System.Text;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using System.Text.RegularExpressions;
using static iTextSharp.tool.xml.html.table.TableRowElement;
using ZXing;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class AdvancedSettingsController : Controller
    {
        #region REPORT COLUMNS SETTINGS
        // GET: Admin/AdvancedSettings
        public ActionResult ReportColumnSettings(int id = 0)
        {

            vwLayerColumnSettings objVwLayerColumnSettings = new vwLayerColumnSettings();
            if (id != 0)
            {
                objVwLayerColumnSettings.lstLayerColumns = BLAdvancedSettings.Instance.GetLayerColumnSettings(id, LayerSettingType.Report.ToString());
                objVwLayerColumnSettings.layer_id = id;
            }
            objVwLayerColumnSettings.lstLayers = new BLLayer().GetLayerDetailsForReport();
            return View("_ReportColumnSettings", objVwLayerColumnSettings);
        }

        public ActionResult SaveReportColumnSettings(vwLayerColumnSettings objLayerColumnSettings)
        {
            ModelState.Clear();
            bool isValid = true;
            PageMessage objMsg = new PageMessage();
            int layerCount = 0;
            foreach (layerColumnSettings resourcekey in objLayerColumnSettings.lstLayerColumns)
            {
                string key = resourcekey.res_field_key;
                if (key != null)
                {
                    var chkResourceKey = new BLResources().chkResourceKeys(key);
                    if (chkResourceKey.Count == 0)
                    {
                        isValid = false;
                        objMsg.status = ResponseStatus.FAILED.ToString();
                        objMsg.message = "Some resources keys are not available!";
                        objLayerColumnSettings.pageMsg = objMsg;
                        objLayerColumnSettings.lstLayers = new BLLayer().GetLayerDetailsForReport();
                        objLayerColumnSettings.lstLayerColumns = BLAdvancedSettings.Instance.GetLayerColumnSettings(objLayerColumnSettings.layer_id, LayerSettingType.Report.ToString());
                        objLayerColumnSettings.lstLayerColumns[layerCount].res_field_key = key;
                        objLayerColumnSettings.lstLayerColumns[layerCount].display_name = resourcekey.display_name;
                        objLayerColumnSettings.lstLayerColumns[layerCount].is_active = resourcekey.is_active;
                        objLayerColumnSettings.lstLayerColumns[layerCount].is_kml_column_required = resourcekey.is_kml_column_required;
                        objLayerColumnSettings.lstLayerColumns[layerCount].id = resourcekey.id;
                        return View("_ReportColumnSettings", objLayerColumnSettings);
                    }
                }
                layerCount++;
            }
            objLayerColumnSettings.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            objLayerColumnSettings.settingType = LayerSettingType.Report.ToString();
            var status = BLAdvancedSettings.Instance.SaveLayerColumnSettings(objLayerColumnSettings);
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Report column settings updated successfully!";
            objLayerColumnSettings.pageMsg = objMsg;
            objLayerColumnSettings.lstLayers = new BLLayer().GetLayerDetailsForReport();
            objLayerColumnSettings.lstLayerColumns = BLAdvancedSettings.Instance.GetLayerColumnSettings(objLayerColumnSettings.layer_id, LayerSettingType.Report.ToString());
            return View("_ReportColumnSettings", objLayerColumnSettings);
        }
        #endregion

        #region HISTORY COLUMNS SETTINGS

        public ActionResult HistoryColumnSettings(int id = 0)
        {

            vwLayerColumnSettings objVwLayerColumnSettings = new vwLayerColumnSettings();
            if (id != 0)
            {
                objVwLayerColumnSettings.lstLayerColumns = BLAdvancedSettings.Instance.GetLayerColumnSettings(id, LayerSettingType.History.ToString());
                objVwLayerColumnSettings.layer_id = id;
            }
            objVwLayerColumnSettings.lstLayers = new BLLayer().GetLayerDetailsForHistory();
            return View("_HistoryColumnSettings", objVwLayerColumnSettings);
        }

        public ActionResult SaveHistoryColumnSettings(vwLayerColumnSettings objLayerColumnSettings)
        {
            ModelState.Clear();
            var isValid = true;
            PageMessage objMsg = new PageMessage();
            int layerCount = 0;
            foreach (layerColumnSettings resourcekey in objLayerColumnSettings.lstLayerColumns)
            {
                string key = resourcekey.res_field_key;
                if (key != null)
                {
                    var chkResourceKey = new BLResources().chkResourceKeys(key);
                    if (chkResourceKey.Count == 0)
                    {
                        isValid = false;
                        objMsg.status = ResponseStatus.FAILED.ToString();
                        objMsg.message = "Some resources keys are not available!";
                        objLayerColumnSettings.pageMsg = objMsg;
                        objLayerColumnSettings.lstLayers = new BLLayer().GetLayerDetailsForReport();
                        objLayerColumnSettings.lstLayerColumns = BLAdvancedSettings.Instance.GetLayerColumnSettings(objLayerColumnSettings.layer_id, LayerSettingType.History.ToString());
                        objLayerColumnSettings.lstLayerColumns[layerCount].res_field_key = key;
                        objLayerColumnSettings.lstLayerColumns[layerCount].display_name = resourcekey.display_name;
                        objLayerColumnSettings.lstLayerColumns[layerCount].is_active = resourcekey.is_active;
                        objLayerColumnSettings.lstLayerColumns[layerCount].id = resourcekey.id;
                        return View("_HistoryColumnSettings", objLayerColumnSettings);
                    }
                }
                layerCount++;
            }
            objLayerColumnSettings.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            objLayerColumnSettings.settingType = LayerSettingType.History.ToString();
            var status = BLAdvancedSettings.Instance.SaveLayerColumnSettings(objLayerColumnSettings);
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "History column settings updated successfully!";
            objLayerColumnSettings.pageMsg = objMsg;
            objLayerColumnSettings.lstLayers = new BLLayer().GetLayerDetailsForHistory();
            objLayerColumnSettings.lstLayerColumns = BLAdvancedSettings.Instance.GetLayerColumnSettings(objLayerColumnSettings.layer_id, LayerSettingType.History.ToString());
            return View("_HistoryColumnSettings", objLayerColumnSettings);
        }
        #endregion


        #region FORM INPUT SETTINGS

        public ActionResult FormInputSettings(string formname = "")
        {

            vwFormInputSettings objVWFormInputSettings = new vwFormInputSettings();
            if (!string.IsNullOrWhiteSpace(formname))
            {
                objVWFormInputSettings.lstFormInputSettings = new BLFormInputSettings().getformInputSettings(formname);
                objVWFormInputSettings.formName = formname;
            }
            objVWFormInputSettings.lstFormNames = new BLFormInputSettings().getDistinctFormNames();
            return View("_FormInputSettings", objVWFormInputSettings);
        }

        public ActionResult SaveFormInputSettings(vwFormInputSettings objFormInputSettings)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            objFormInputSettings.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var status = new BLFormInputSettings().SaveFormInputSettings(objFormInputSettings);
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Input settings updated successfully!";
            //update form input settings globally for all users...
            ApplicationSettings.formInputSettings = new BLFormInputSettings().getformInputSettings();
            objFormInputSettings.pageMsg = objMsg;
            objFormInputSettings.lstFormNames = new BLFormInputSettings().getDistinctFormNames();
            objFormInputSettings.lstFormInputSettings = new BLFormInputSettings().getformInputSettings(objFormInputSettings.formName);
            return View("_FormInputSettings", objFormInputSettings);
        }
        #endregion


        #region GLOBAL PARAMETER SETTINGS

        public ActionResult ViewGlobalSettings(VMGlobalSetting objVMGlobalSetting, string msg = "", string status = "", int page = 0, string sort = "", string sortdir = "")
        {
            try
            {
                if (sort != "" || page != 0)
                {
                    objVMGlobalSetting.objGridAttributes = (CommonGridAttributes)Session["gridAttr"];
                }
                objVMGlobalSetting.lstSearchBy = GetSearchByColumns();
                objVMGlobalSetting.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
                objVMGlobalSetting.objGridAttributes.currentPage = page == 0 ? 1 : page;
                objVMGlobalSetting.objGridAttributes.sort = sort;

                objVMGlobalSetting.objGridAttributes.orderBy = sortdir;
                objVMGlobalSetting.lstGlobalSettings = new BLGlobalSetting().GetAllGlobalSettings(objVMGlobalSetting.objGridAttributes, Convert.ToInt32(((User)Session["userDetail"]).user_id));

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    objVMGlobalSetting.pageMsg = new PageMessage() { message = msg, status = status };
                }
                objVMGlobalSetting.objGridAttributes.totalRecord = objVMGlobalSetting.lstGlobalSettings != null && objVMGlobalSetting.lstGlobalSettings.Count > 0 ? objVMGlobalSetting.lstGlobalSettings[0].totalRecords : 0;
                Session["gridAttr"] = objVMGlobalSetting.objGridAttributes;
                return View("_ViewGlobalSettings", objVMGlobalSetting);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult AddGlobalSetting(int id = 0)
        {
            GlobalSetting objGlobalSettings = id == 0 ? new GlobalSetting() : new BLGlobalSetting().GetGlobalSettingbyId(id);
            return View("_AddGlobalSettings", objGlobalSettings);
        }

        public ActionResult SaveGlobalSetting(GlobalSetting objGlobalSetting)
        {
            ModelState.Clear();
            if (objGlobalSetting.id != 0)
            {
                var status = new BLGlobalSetting().SaveGlobalSetting(objGlobalSetting, Convert.ToInt32(((User)Session["userDetail"]).user_id));
                //update Global Setting value..
                ApplicationSettings.InitializeGlobalSettings();
                objGlobalSetting = new BLGlobalSetting().GetGlobalSettingbyId(objGlobalSetting.id);

            }
            PageMessage pageMsg = new PageMessage();
            pageMsg.status = ResponseStatus.OK.ToString();
            pageMsg.message = "Selected Setting updated successfully!";
            objGlobalSetting.pageMsg = pageMsg;
            return Json(pageMsg, JsonRequestBehavior.AllowGet);
            //return View("_AddGlobalSettings", objGlobalSetting);
        }

        [HttpGet]
        public void DownloadViewglobalSettingsDetail()
        {
            if (Session["gridAttr"] != null)
            {
                CommonGridAttributes objGlobalSettingFilter = (CommonGridAttributes)Session["gridAttr"];
                List<GlobalSetting> lstGlobalSettingDetails = new List<GlobalSetting>();
                objGlobalSettingFilter.currentPage = 0;
                objGlobalSettingFilter.pageSize = 0;
                lstGlobalSettingDetails = new BLGlobalSetting().GetAllGlobalSettings(objGlobalSettingFilter, Convert.ToInt32(((User)Session["userDetail"]).user_id));

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable(lstGlobalSettingDetails);

                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("TYPE");
                dtReport.Columns.Remove("IS_EDIT_ALLOWED");
                dtReport.Columns.Remove("DATA_TYPE");
                dtReport.Columns.Remove("MIN_VALUE");
                dtReport.Columns.Remove("MAX_VALUE");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("MODIFIED_ON");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("TOTALRECORDS");

                dtReport.Columns["key"].SetOrdinal(0);
                dtReport.Columns["value"].SetOrdinal(1);
                dtReport.Columns["description"].SetOrdinal(2);
                dtReport.Columns["is_web_key"].SetOrdinal(3);

                dtReport.Columns["is_mobile_key"].SetOrdinal(4);

                dtReport.Columns["key"].ColumnName = "key";
                dtReport.Columns["value"].ColumnName = "value";
                dtReport.Columns["description"].ColumnName = "description";
                dtReport.Columns["is_web_key"].ColumnName = "is_web_key";
                dtReport.Columns["is_mobile_key"].ColumnName = "is_mobile_key";

                DataTable dtCloned = dtReport.Clone();
                dtCloned.Columns["is_web_key"].DataType = typeof(string);
                dtCloned.Columns["is_mobile_key"].DataType = typeof(string);

                foreach (DataRow row in dtReport.Rows)
                {
                    dtCloned.ImportRow(row);
                }
                for (int i = 0; i < dtCloned.Rows.Count; i++)
                {
                    if (dtCloned.Rows[i]["is_web_key"].ToString() == "True")
                    {
                        dtCloned.Rows[i]["is_web_key"] = "Yes";
                    }
                    else
                    {
                        dtCloned.Rows[i]["is_web_key"] = "No";
                    }
                    if (dtCloned.Rows[i]["is_mobile_key"].ToString() == "True")
                    {
                        dtCloned.Rows[i]["is_mobile_key"] = "Yes";
                    }
                    else
                    {
                        dtCloned.Rows[i]["is_mobile_key"] = "No";
                    }
                }
                var filename = "GlobalSettings";
                ExportAssignmentData(dtCloned, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        private void ExportAssignmentData(DataTable dtReport, string fileName)
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


        #endregion


        #region MODULE SETTINGS


        public ActionResult ModuleSettings(int update_Status = -1)
        {
            BLAdvancedSettings objBL = BLAdvancedSettings.Instance;
            PageMessage objMsg = new PageMessage();
            ModuleSettings settings = new ModuleSettings();
            settings.lstUserModule = objBL.GetModuleMasterList();
            settings.lstUserModule.ForEach(x => x.is_selected = x.is_active);

            if (update_Status == 1)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Module setting has been saved successfully!";
            }
            else if (update_Status == 0)
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while submitting module!!";
            }
            settings.pageMsg = objMsg;


            return View("_ModuleSettings", settings);
        }

        [HttpPost]
        public ActionResult SaveModuleSettings(ModuleSettings mapping)
        {
            var updateStatus = -1;
            try
            {
                int usrId = Convert.ToInt32(Session["user_id"]);

                var status = BLAdvancedSettings.Instance.SaveModuleSettings(mapping.lstUserModule, usrId);

                updateStatus = status ? 1 : 0;
            }
            catch (Exception)
            {
                updateStatus = 0;
                throw;
            }
            return RedirectToAction("ModuleSettings", new { update_Status = updateStatus });
        }

        #endregion

        #region LAYER ACTION SETTINGS
        public ActionResult LayerActionSettings(int id = 0, int update_Status = -1)
        {
            PageMessage objMsg = new PageMessage();
            LayerActionSettings layerActions = new LayerActionSettings();
            if (id != 0)
            {
                layerActions.actionDetails = BLAdvancedSettings.Instance.GetLayerActionSettings(id);
                layerActions.layer_id = id;
            }
            layerActions.layerDetails = new BLLayer().GetAllLayerDetail();


            if (update_Status == 1)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Actions setting has been saved successfully!";
            }
            else if (update_Status == 0)
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while submitting action settings!!";
            }
            layerActions.pageMsg = objMsg;

            return View("_LayerActionSettings", layerActions);
        }
        public ActionResult SaveLayerActionSettings(LayerActionSettings actionSettings)
        {
            var updateStatus = -1;
            try
            {
                int usrId = Convert.ToInt32(Session["user_id"]);

                var status = BLAdvancedSettings.Instance.SaveLayerActionSettings(actionSettings.actionDetails, actionSettings.layer_id, usrId);

                updateStatus = status ? 1 : 0;
            }
            catch (Exception)
            {
                updateStatus = 0;
                throw;
            }
            return RedirectToAction("LayerActionSettings", new { id = actionSettings.layer_id, update_Status = updateStatus });
        }

        #endregion
        #region ConnectionLabelSettings
        public ActionResult ViewConnectionLabelSettings(VwConnectionLabelSettings objConLablSetng, int page = 0, string sort = "", string sortdir = "")
        {
            objConLablSetng.objConnectionLabelFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objConLablSetng.objConnectionLabelFilter.currentPage = page == 0 ? 1 : page;
            objConLablSetng.objConnectionLabelFilter.sort = sort;
            objConLablSetng.objConnectionLabelFilter.orderBy = sortdir;
            objConLablSetng.lstSearchBy = GetRoleSearchByColumns();
            objConLablSetng.lstViwConnectionLabelSettings = new BLConnectionLabelSettings().GetConnectionLabelSettingsList(objConLablSetng.objConnectionLabelFilter);
            objConLablSetng.objConnectionLabelFilter.totalRecord = objConLablSetng.lstViwConnectionLabelSettings.Count > 0 ? objConLablSetng.lstViwConnectionLabelSettings[0].totalRecords : 0;
            return View("_ViewConnectionLabelSettings", objConLablSetng);
        }
        public List<KeyValueDropDown> GetRoleSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Entity Title", value = "entity_title" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Created By", value = "created_by_text" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Status", value = "status" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }
        public ActionResult EditConnectionLabelSettings(int id, string entityTitle)
        {
            ConnectionLabelSettings objConLblStngs = new BLConnectionLabelSettings().GetConnectionLabelSettingbyId(id);
            objConLblStngs.lstConnectionLabel = new BLConnectionLabelSettings().GetConnectionLabel(objConLblStngs.layer_id);
            objConLblStngs.entity_title = entityTitle;
            return PartialView("ConnectionLabelSettings", objConLblStngs);
        }
        public JsonResult SaveConnectionLabelSettings(ConnectionLabelSettings objModel)
        {
            PageMessage objMsg = new PageMessage();
            try
            {
                objModel.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                var result = new BLConnectionLabelSettings().SaveConnectionLabelInfo(objModel);

                if (result)
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Connection Label Settings updated successfully!";
                }
                else
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "Failed to update Connection Label Settings!!";
                }
            }
            catch
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while processing your request!!";
            }
            objModel.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SyncAllLabels(int id)
        {
            //    ViewBag.SyncOrAsync = "Asynchronous";
            //    string results = string.Empty;
            PageMessage objMsg = new PageMessage();
            ConnectionLabelSettings objConLblStngs = new BLConnectionLabelSettings().GetConnectionLabelSettingbyId(id);
            var result = new BLConnectionLabelSettings().SyncAllLabelColumn(objConLblStngs.layer_id);
            if (result)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Label Columns Synchronized successfully!";
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Failed to update Label Columns synchronization!!";
            }
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult ReadMore(string key)
        {
            GlobalSetting globalSettings = new BLGlobalSetting().getValueFullText(key);
            return PartialView("_ReadMore", globalSettings);
        }

        public List<KeyValueDropDown> GetSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "key", value = "key" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Description", value = "description" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Value", value = "value" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }

        public ActionResult VoiceCommandMaster(VoiceCommandMaster objVoiceCommandMaster, int page = 0, string sort = "", string sortdir = "")
        {
            BindSearchBy(objVoiceCommandMaster);
            objVoiceCommandMaster.lstVoiceCommand = new BLVoiceCommandMaster().getVoiceCommandMaster();
            VoiceCommandDetail(objVoiceCommandMaster, page, sort, sortdir);
            return View("_VoiceCommandMaster", objVoiceCommandMaster);
        }

        public IList<KeyValueDropDown> BindSearchBy(VoiceCommandMaster objSearchByDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Command Name", value = "actual_command_name" });
            items.Add(new KeyValueDropDown { key = "Command Pronounce", value = "command_pronounce" });
            return objSearchByDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();
        }

        public ActionResult SaveVoiceCommandMaster(SaveVoiceCommandMaster objVoiceCommandMaster)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            bool isNew = objVoiceCommandMaster.id > 0 ? false : true;

            objVoiceCommandMaster.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var status = new BLVoiceCommandMaster().SaveVoiceCommandMaster(objVoiceCommandMaster);
            objMsg.status = ResponseStatus.OK.ToString();
            VoiceCommandMaster voiceCommandMaster = new VoiceCommandMaster();
            if (isNew)
            {
                objMsg.message = "Voice Command Detail Saved successfully!";
            }
            else
            {
                objMsg.message = "Voice Command Detail Updated successfully!";
            }

            voiceCommandMaster.pageMsg = objMsg;
            return VoiceCommandMaster(voiceCommandMaster);
        }
        public void VoiceCommandDetail(VoiceCommandMaster model, int page = 0, string sort = "", string sortdir = "")
        {
            model.pageSize = 10; //ApplicationSettings.ViewAdminDashboardGridPageSize;
            model.currentPage = page == 0 ? 1 : page;
            model.sort = sort;
            model.orderBy = sortdir;
            Session["VoiceCommandDetail"] = model.VoiceCommandDetail = new BLVoiceCommandMaster().GetVoiceCommandDetail(model);
            model.totalrecords = model.VoiceCommandDetail != null && model.VoiceCommandDetail.Count > 0 ? model.VoiceCommandDetail[0].totalrecords : 0;
        }
        public ActionResult EditVoiceCommand(int id)
        {
            VoiceCommandMaster objVoiceCommand = new VoiceCommandMaster();
            objVoiceCommand.saveVoiceCommandData = new BLVoiceCommandMaster().GetVoiceCommandDetailsByID(id);
            objVoiceCommand.VoiceCommandDetail = (IList<SaveVoiceCommandMaster>)Session["VoiceCommandDetail"];
            //VoiceCommandDetail(objVoiceCommand);
            objVoiceCommand.command_id = objVoiceCommand.saveVoiceCommandData.command_id;
            objVoiceCommand.command_pronounce = objVoiceCommand.saveVoiceCommandData.command_pronounce;
            objVoiceCommand.created_on = objVoiceCommand.saveVoiceCommandData.created_on;
            objVoiceCommand.created_by = objVoiceCommand.saveVoiceCommandData.created_by;

            objVoiceCommand.lstVoiceCommand = new BLVoiceCommandMaster().getVoiceCommandMaster();
            BindSearchBy(objVoiceCommand);
            return View("_VoiceCommandMaster", objVoiceCommand);
        }

        #region VoiceCommandDetailsDownload
        [HttpGet]
        public void DownloadVoiceCommandDetail()
        {
            if (Session["VoiceCommandDetail"] != null)
            {
                VoiceCommandMaster model = new VoiceCommandMaster();

                model.VoiceCommandDetail = (IList<SaveVoiceCommandMaster>)Session["VoiceCommandDetail"];

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<SaveVoiceCommandMaster>(model.VoiceCommandDetail);
                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("COMMAND_ID");
                dtReport.Columns.Remove("LSTVOICECOMMAND");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_ON"); ;
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("PAGESIZE");
                dtReport.Columns.Remove("IS_VALID");
                dtReport.Columns.Remove("ERROR_MSG");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns["actual_command_name"].ColumnName = "Actual Command Name";
                dtReport.Columns["command_pronounce"].ColumnName = "Command Pronounce";
                var filename = "VoiceCommandDetail";
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
        #endregion

        public ActionResult UploadVoiceCommandSettings(VoiceCommandMaster objVoiceCommandMaster, int page = 0, string sort = "", string sortdir = "")
        {
            return View("_UploadVoiceCommandMaster");
        }


        #region VoiceCommandMapping upload
        public FileResult DownloadVoiceCommandMappingUploadTemplate(string FileName)
        {
            var file = "~//Content//Templates//Bulk//VoiceCommandMappingTemplate.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, FileName + ".xlsx");
        }
        [HttpPost]
        public ActionResult UploadVoiceCommandMappingData()
        {
            string strReturn = "";
            string msg = "";
            //table for data
            DataTable dtExcelData = new DataTable();
            try
            {
                if (Request != null)
                {
                    var fileName = string.Empty;
                    var filepath = string.Empty;
                    var dataFileName = string.Empty;
                    int userId = Convert.ToInt32(Session["user_id"]);
                    HttpFileCollectionBase files = Request.Files;
                    DataTable dataTable = new DataTable();
                    PageMessage objMsg = new PageMessage();
                    //Save uplaoded excel file on temp path
                    HttpPostedFileBase file = files[0];
                    fileName = AppendTimeStamp(Request.Files[0].FileName);
                    filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\VoiceCommandMapping\\"), fileName);
                    file.SaveAs(filepath);
                    if (Path.GetExtension(fileName).ToUpper() == ".XLS" || Path.GetExtension(fileName).ToUpper() == ".XLSX")
                    {
                        dataTable = NPOIExcelHelper.ExcelToTable(filepath, "");

                    }

                    // Remove blank rows...
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        dtExcelData = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
                    }

                    if (dtExcelData.Rows.Count > 0)
                    {
                        //get maximum upload count allowed at a time...
                        if (dtExcelData.Rows.Count <= 500)
                        {
                            string ErrorMsg = "";
                            // get column mapping...
                            string strMappingFilePath = Server.MapPath("~\\Content\\Templates\\Bulk\\VoiceCommandColumnMapping.xml");
                            Dictionary<string, string> dicColumnMapping = GetBulkUploadColumnMapping(strMappingFilePath);

                            // validate uploaded excel column with template mapping...
                            ErrorMsg = validateTemplateColumn(dicColumnMapping, dtExcelData);
                            if (ErrorMsg != "")
                                return Json(new { strReturn = ErrorMsg, msg = "error" }, JsonRequestBehavior.AllowGet);
                            if (ErrorMsg == "")
                            {
                                //ADD COLUMN TO DTEXCEL.. (UPLOADED_BY)
                                DataColumn dcUploadedBy = new DataColumn("UPLOADED_BY", typeof(int));
                                dcUploadedBy.DefaultValue = userId;
                                dtExcelData.Columns.Add(dcUploadedBy);

                                //ADD COLUMN TO DTEXCEL.. (IS_VALID)
                                DataColumn dcIsValid = new DataColumn("IS_VALID", typeof(int));
                                dtExcelData.Columns.Add(dcIsValid);

                                //ADD COLUMN TO DTEXCEL.. (ERROR_MSG)
                                DataColumn dcErrorMsg = new DataColumn("ERROR_MSG", typeof(string));
                                dcErrorMsg.MaxLength = 200;
                                dtExcelData.Columns.Add(dcErrorMsg);

                            }
                            BLVoiceCommandMaster blVoiceCommand = new BLVoiceCommandMaster();

                            //delete DATA FROM TEMP TABLE ON THE BASIS OF UPLOADED_BY ID
                            blVoiceCommand.DeleteTempVoiceCommandMappingData(userId);

                            List<TempSaveVoiceCommandMaster> lstVoiceCommandMapping = new List<TempSaveVoiceCommandMaster>();

                            foreach (DataRow dr in dtExcelData.Rows)
                            {
                                TempSaveVoiceCommandMaster objVoiceCommandMapping = new TempSaveVoiceCommandMaster();
                                string strErrorMsg = ValidateVoiceCommandMappingData(dr, ref objVoiceCommandMapping, dicColumnMapping);

                                objVoiceCommandMapping.created_by = userId;
                                objVoiceCommandMapping.actual_command_name = Convert.ToString(dr[dicColumnMapping["actual_command_name"]]);
                                objVoiceCommandMapping.command_pronounce = Convert.ToString(dr[dicColumnMapping["command_pronounce"]]);
                                objVoiceCommandMapping.created_on = DateTimeHelper.Now;
                                lstVoiceCommandMapping.Add(objVoiceCommandMapping);
                            }

                            if (lstVoiceCommandMapping.Count > 0)
                            {
                                //SAVE DATA INTO TEMP VOICE COMMAND MAPPING
                                blVoiceCommand.BulkUploadVoiceCommandMapping(lstVoiceCommandMapping);
                                //VALIDATE AND UPLAOD VOICE COMMAND MAPPING INTO MAIN TABLE.
                                dynamic result = "";
                                result = blVoiceCommand.UploadVoiceCommandMappingForInsert(userId);
                                if (!result.status)
                                {
                                    // exit function if failed..
                                    return Json(new { strReturn = string.Format(result.message), msg = "error" }, JsonRequestBehavior.AllowGet);//"Error in uploading Buildings! <br> Error:" + result.message,
                                }

                            }
                            var getTotalUploadVoiceCommandMappingfailureAndSuccess = blVoiceCommand.getTotalUploadBuildingfailureAndSuccess(userId);
                            var GetTotalCountOfSuccesAndFailure = "<table border='1' class='alertgrid'><thead><tr><td><b>Status</b></td><td><b>Count</b></td></tr></thead><tbody><tr><td>Success</td><td>" + getTotalUploadVoiceCommandMappingfailureAndSuccess.Item1 + "</td></tr><tr><td>failure</td><td>" + getTotalUploadVoiceCommandMappingfailureAndSuccess.Item2 + "</td></tr></tbody>";
                            strReturn = string.Format("Voice Command Mapping data processed successfully.", GetTotalCountOfSuccesAndFailure);
                        }
                        else
                        {
                            // exit function with max record error...
                            return Json(new { strReturn = "Maximum 500 specifications can be uploaded at a time!", msg = "error" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        // exit function with no record...
                        return Json(new { strReturn = "No record exists in selected file!", msg = "error" }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (NPOI.POIFS.FileSystem.NotOLE2FileException ex)
            {
                msg = "error";
                strReturn = "Selected file is either corrupted or invalid excel file!";
                ErrorLogHelper.WriteErrorLog("UploadVoiceCommandMappingData()", "AdvancedSettings", ex);

            }
            catch (Exception ex)
            {
                msg = "error";
                strReturn = "Failed to upload Voice Command Mapping! <br> Error:" + ex.Message;
                ErrorLogHelper.WriteErrorLog("UploadVoiceCommandMappingData()", "AdvancedSettings", ex);
            }
            return Json(new { strReturn = strReturn, msg = msg == "" ? "success" : msg }, JsonRequestBehavior.AllowGet);
        }
        public string ValidateVoiceCommandMappingData(DataRow dr, ref TempSaveVoiceCommandMaster objVoiceCommandMapping, Dictionary<string, string> dicColumnMapping)
        {

            objVoiceCommandMapping.is_valid = true;
            string[] arrBooleanValues = { "YES", "NO" };
            try
            {
                if (string.IsNullOrEmpty(dr[dicColumnMapping["actual_command_name"]].ToString()))
                {
                    objVoiceCommandMapping.is_valid = false;
                    objVoiceCommandMapping.error_msg = "Actual Command name cannot be blank!";// "Command name can not be blank!";
                }


                if (string.IsNullOrEmpty(dr[dicColumnMapping["command_pronounce"]].ToString()))
                {
                    objVoiceCommandMapping.is_valid = false;
                    objVoiceCommandMapping.error_msg = "Command pronounce cannot be blank!";// "Command pronounce can not be blank!";
                }


            }

            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        public void DownloadUploadVoiceCommandMappingLogs()
        {
            BLVoiceCommandMaster BLVoiceCommandMap = new BLVoiceCommandMaster();
            DataTable dtlogs = new DataTable();
            dtlogs.Columns.Add("Command Name", typeof(string));
            dtlogs.Columns.Add("Command Pronounce", typeof(string));
            dtlogs.Columns.Add("Message", typeof(string));
            dtlogs.Columns.Add("Status", typeof(string));

            dtlogs.TableName = "VoiceCommandMappingLogs";
            int userId = Convert.ToInt32(Session["user_id"]);

            SaveVoiceCommandMaster SaveVoiceCommandMaster = new SaveVoiceCommandMaster();
            var status = "";
            using (var exportData = new MemoryStream())
            {
                Response.Clear(); ;
                List<TempSaveVoiceCommandMaster> BulkUploadLogs = BLVoiceCommandMap.GetUploadVoiceCommandMappingLogs(userId);   //GetUploadBuildingLogs(userId);
                if (BulkUploadLogs.Count() > 0)
                {
                    if (BulkUploadLogs.Count() > 0)
                    {
                        foreach (var t in BulkUploadLogs)
                        {
                            if (t.is_valid == true)
                            {
                                status = "YES";
                            }
                            else
                            {
                                status = "NO";
                            }

                            dtlogs.Rows.Add(Convert.ToString(t.actual_command_name), Convert.ToString(t.command_pronounce), t.error_msg, status);
                        }
                    }
                    IWorkbook workbook = SmartInventory.Helper.NPOIExcelHelper.DataTableToExcel("xlsx", dtlogs);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", AppendTimeStamp("VoiceCommandMappinglogs.xlsx")));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(
            Path.GetFileNameWithoutExtension(fileName),
            DateTimeHelper.Now.ToString("yyyyMMddHHmmssfff"),
            Path.GetExtension(fileName)
            );

        }
        public Dictionary<string, string> GetBulkUploadColumnMapping(string filepath)
        {
            Dictionary<string, string> dicMapping = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(filepath);
            return dicMapping = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
                .Select(p => new
                {
                    DbColName = p.Element("DbColName").Value,
                    TemplateColName = p.Element("TemplateColName").Value
                })
                .ToDictionary(t => t.DbColName, t => t.TemplateColName);
        }

        public string validateTemplateColumn(Dictionary<string, string> dicColumnMapping, DataTable dt)
        {
            string[] arrColumns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower()).ToArray();
            foreach (var pair in dicColumnMapping)
            {
                // if column not found in template and return error..
                if (!arrColumns.Contains(pair.Value.ToLower()))
                    return "Selected file does not contain '" + pair.Value + "' column!";
            }
            return "";
        }
        public Dictionary<string, string> GetBulkUploadShapeColumnMapping(string filepath)
        {
            Dictionary<string, string> dicMapping = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(filepath);
            return dicMapping = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
                .Select(p => new
                {
                    ShapeColName = p.Element("ShapeColName").Value,
                    TemplateColName = p.Element("TemplateColName").Value
                })
                .ToDictionary(t => t.ShapeColName, t => t.TemplateColName);
        }
        public ActionResult UploadVoiceCommandMapping()
        {
            return View("_UploadVoiceCommandMaster");

        }
        #endregion

        public ActionResult CBOMFormula()
        {
            ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();
            objVoiceCommand.constructionohlogicmasterlist = new BLCBomLogic().GetAllCBOMLogic().Where(c => c.calculation_sub_type == "Formula" && c.is_active == true && c.is_default == true &&  c.is_formula_update_allowed == true).OrderBy(c => c.execution_sequence).ToList();

            return View("_CBOMFormulaDesign", objVoiceCommand);
        }
        public ActionResult GetVariables(string column_name, int fsa_id, int execution_sequence)
        {


            ViewBag.sequence = execution_sequence;
            ViewBag.column_name = column_name;
            // ConstructionOhLogicMaster objVoiceCommand1 = id > 0 ? new BLCBomLogic().GetCBOMLogic(id) : new ConstructionOhLogicMaster();
            //ConstructionOhLogicMaster objVoiceCommand1 = new BLCBomLogic().GetVariables();

            ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();
            ConstructionOhLogicMaster objVoiceCommandbycolumnname = new BLCBomLogic().GetCBOMLogic(column_name,0);
            objVoiceCommand.constructionohlogicmasterlist = new BLCBomLogic().GetVariables().Where(c => c.is_active).OrderBy(c => c.execution_sequence).ToList().DistinctBy(c => c.column_name).Take(objVoiceCommandbycolumnname.execution_sequence - 1).ToList();
            // new BLCBomLogic().GetAllCBOMLogic().OrderBy(c => c.execution_sequence).Take(objVoiceCommandbycolumnname.execution_sequence - 1).DistinctBy(c=>c.column_name).ToList();

            if (objVoiceCommandbycolumnname.oh_display_formula != "")
            {
                ViewBag.logic = objVoiceCommandbycolumnname.oh_display_formula;
                ViewBag.logic1 = objVoiceCommandbycolumnname.oh_logic;
                ViewBag.oh_expression_json = objVoiceCommandbycolumnname.oh_expression_json;
            }

            return PartialView("_CBOMVariable", objVoiceCommand);




        }

        [HttpPost]
        //commented by priyanka
        //public ActionResult savebomlogic(OverheadLogicsDTO overheadLogicsDTO)
        //{

        //    int Success = new BLCBomLogic().Savebomlogic(overheadLogicsDTO.DisplayFormula, 0, overheadLogicsDTO.OhLogic, overheadLogicsDTO.ColumnName, overheadLogicsDTO.OhExpressionJson);
        //    ConstructionOhLogicMaster objVoiceCommand = new BLCBomLogic().GetCBOMLogic(overheadLogicsDTO.ColumnName, 0);

        //    return Json(new
        //    {
        //        Message = Success == 1,
        //        formula = overheadLogicsDTO.DisplayFormula,
        //        id = objVoiceCommand.id,
        //        JsonRequestBehavior.AllowGet
        //    });

        //}
        //end


        public ActionResult savebomlogic(OverheadLogicsDTO overheadLogicsDTO)
        {

            int Success = new BLCBomLogic().Savebomlogic(overheadLogicsDTO.DisplayFormula,Convert.ToInt32(overheadLogicsDTO.FSAId), overheadLogicsDTO.OhLogic, overheadLogicsDTO.ColumnName, overheadLogicsDTO.OhExpressionJson);
            ConstructionOhLogicMaster objVoiceCommand = new BLCBomLogic().GetCBOMLogic(overheadLogicsDTO.ColumnName, Convert.ToInt32(overheadLogicsDTO.FSAId));

            return Json(new
            {
                Message = Success == 1,
                formula = overheadLogicsDTO.DisplayFormula,
                id = objVoiceCommand.id,
                JsonRequestBehavior.AllowGet
            });

        }
        //public ActionResult savebomlogic(string formula, String fsa_id, string formula1 ,string column_name)

        //{
        //    string message = null;
        //    ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();
        //   // objVoiceCommand.id = id;
        //    objVoiceCommand.oh_display_formula = formula;
        //    objVoiceCommand.oh_logic = formula1;

        //    var status = new BLCBomLogic().SaveCBOMLogic(objVoiceCommand);

        //    if (status == true)
        //    {
        //        message = "SUCCESS";
        //    }
        //    else
        //    {
        //        message = "Fail";
        //    }
        //    return Json(new
        //    {
        //        Message = message,
        //        formula = formula,
        //        formula1 = formula1,
        //        JsonRequestBehavior.AllowGet
        //    });
        //    //return Json(new { Message = message, formula = formula, formula1 = formula1, ex_sequence= execution_Sequence, id=id, JsonRequestBehavior.AllowGet });
        //}
        [HttpGet]
        public ActionResult Validatebomformula(string formula)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(formula);
            StringBuilder sb1 = new StringBuilder();
            ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();
            objVoiceCommand.constructionohlogicmasterlist = new BLCBomLogic().GetAllCBOMLogic().ToList();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            int i = 1;
            // objVoiceCommand.constructionohlogicmasterlist.Select(s => s.column_name).Distinct().OrderByDescending(c=>c.Length)
            foreach (var item in objVoiceCommand.constructionohlogicmasterlist.Select(s => s.label).Distinct().OrderByDescending(c => c.Length))
            {

                dict.Add(item, i);
                i = i + 1;
            }

            var sm = formula;
            foreach (KeyValuePair<string, int> replacement in dict)
            {

                sb.Replace(replacement.Key, replacement.Value.ToString());





            }
            string total = sb.ToString();

            return Json(new { total = total }, JsonRequestBehavior.AllowGet);
        }


    }
}