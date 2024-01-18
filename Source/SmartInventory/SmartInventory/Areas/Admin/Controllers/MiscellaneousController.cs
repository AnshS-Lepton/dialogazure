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
using System.Net;
using System.Configuration;
using System.Web.Http.Results;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class MiscellaneousController : Controller
    {
        private readonly BLVendorSpecification blVendorSpecification;
        public MiscellaneousController()
        {
            blVendorSpecification = new BLVendorSpecification();
        }
        //public ActionResult DynamicTheme()
        //{
        //    DynamicTheme obj = new DynamicTheme();
        //    obj.themelist = new BLDynamicTheme().getThemes();
        //    return View(obj);
        //}
        //public ActionResult SaveTheme(int themeId)
        //{
        //    var mainFile = @"~/Content/css/DynamicTheme/color_stylesheet.css";
        //    int id = themeId;
        //    var mainPath=Path.Combine(Server.MapPath(mainFile));
        //    DynamicTheme css = new DynamicTheme();
        //    css = new BLDynamicTheme().savetheme(id);
        //    String file = css.css_file_content;
        //    System.IO.File.WriteAllText(mainPath, file);
        //    return Json("test");
        //}
        public ActionResult EmailSettings()
        {
            EmailSettingsModel _objEmailSettingsModel = new EmailSettingsModel();
            EmailSettingsVM objMessage = new EmailSettingsVM();
            PageMessage objMsg = new PageMessage();
            List<EmailSettingsModel> lstEmailSettingsModel = new List<EmailSettingsModel>();
            _objEmailSettingsModel = new BLMisc().getEmailSettings();
            string DecodePassword = Utility.MiscHelper.DecodeTo64(_objEmailSettingsModel.email_password);
            _objEmailSettingsModel.email_password = DecodePassword;

            objMessage.lstEmailSettingsModel = _objEmailSettingsModel;
            objMessage.pageMsg = objMsg;
            return View(objMessage);
        }

        public ActionResult SaveEmailSettings(EmailSettingsVM _objEmailSettingsVM)
        {
            EmailSettingsVM objMessage = new EmailSettingsVM();
            EmailSettingsModel _objEmailSettingsModel = new EmailSettingsModel();
            PageMessage objMsg = new PageMessage();
            string EncodePassword = Utility.MiscHelper.EncodeTo64(_objEmailSettingsVM.lstEmailSettingsModel.email_password);
            int user_id = Convert.ToInt32(Session["user_id"]);
            Session["id"] = _objEmailSettingsVM.lstEmailSettingsModel.id;
            int value = Convert.ToInt32(Session["id"]);
            _objEmailSettingsModel = new BLMisc().SaveEmailSettings(_objEmailSettingsVM.lstEmailSettingsModel, EncodePassword, user_id);
            _objEmailSettingsModel = new BLMisc().getEmailSettings();
            if (value == _objEmailSettingsModel.id)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Email settings updated successfully!";
            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Email settings saved successfully!";
            }
            _objEmailSettingsModel = new BLMisc().getEmailSettings();
            objMessage.lstEmailSettingsModel = _objEmailSettingsModel;
            objMessage.pageMsg = objMsg;
            return View("EmailSettings", objMessage);
        }
        #region ChangeNetworkCode
        public ActionResult ChangeNetworkCode(ChangeNetworkCode _objParam)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            _objParam.lstLayers = new BLLayer().GetChangeNetworkInfoLayers();
            if (objLgnUsrDtl.is_admin_rights_enabled)
            {
                var lstLayers = new BLLayer().GetReportLayers(objLgnUsrDtl.role_id, "ENTITY");
                _objParam.lstLayers = _objParam.lstLayers.Where(p => lstLayers.Any(p2 => p2.layer_id == p.layer_id)).ToList();
            }
            //if (!String.IsNullOrEmpty(_objParam.layer_name) && !String.IsNullOrEmpty(_objParam.old_network_id) && !String.IsNullOrEmpty(_objParam.new_network_id))
            //{
            //    _objParam.created_by = Convert.ToInt32(Session["user_id"]);
            //    _objParam.dbMsg = new BLMisc().SaveChangeNetworkCode(_objParam);
            //    _objParam.pageMsg.status = _objParam.dbMsg.status == true ? "OK" : "FAILED";
            //    _objParam.pageMsg.message = _objParam.dbMsg.message;
            //}
            return View("ChangeNetworkCode", _objParam);
        }
        public JsonResult getNewNetworkcode(string etype, string entity_network_id)
        {
            var newnetworkcode = new BLMisc().getNewNetworkcode(etype, entity_network_id);
            return Json(newnetworkcode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveChangeNewNetworkcode(string etype, string old_network_id, string new_network_id, string remarks)
        {
            ChangeNetworkCode _objParam = new ChangeNetworkCode();
            _objParam.created_by = Convert.ToInt32(Session["user_id"]);
            _objParam.layer_name = etype; _objParam.old_network_id = old_network_id; _objParam.new_network_id = new_network_id; _objParam.remarks = remarks;
            var result = new BLMisc().SaveChangeNetworkCode(_objParam);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getNetworkIdDependency(string etype, string old_network_id)
        {
            var result = new BLMisc().getNetworkIdDependency(etype, old_network_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ViewUtilizationSetting
        public ActionResult ViewUtilizationSetting(ViewEntityUtilizationSettings objVwUtil, int page = 0, string sort = "", string sortdir = "")
        {
            BindUtilizationViewDropdown(ref objVwUtil);
            objVwUtil.objEntityUtiliSettingsFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objVwUtil.objEntityUtiliSettingsFilter.currentPage = page == 0 ? 1 : page;
            objVwUtil.objEntityUtiliSettingsFilter.sort = sort;
            objVwUtil.objEntityUtiliSettingsFilter.orderBy = sortdir;
            objVwUtil.lstViwUtilizationSetting = new BLMisc().GetUtilizationSettingsList(objVwUtil.objEntityUtiliSettingsFilter);
            objVwUtil.objEntityUtiliSettingsFilter.totalRecord = objVwUtil.lstViwUtilizationSetting.Count > 0 ? objVwUtil.lstViwUtilizationSetting[0].totalRecords : 0;
            Session["viewUtilFilters"] = objVwUtil.objEntityUtiliSettingsFilter;
            return View("_ViewUtilizationSetting", objVwUtil);
        }
        public void BindUtilizationViewDropdown(ref ViewEntityUtilizationSettings objVwUtil)
        {
            //Bind Layers..   
            objVwUtil.lstLayers = new BLLayer().GetLayerDetailsForUtilization();
            //Bind Regions..
            objVwUtil.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (objVwUtil.objEntityUtiliSettingsFilter.region_id > 0)
            {
                objVwUtil.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { userId = Convert.ToInt32(Session["user_id"]), regionIds = objVwUtil.objEntityUtiliSettingsFilter.region_id.ToString() });
            }
        }
        [HttpGet]
        public void ExportUtilizationDetails()
        {
            if (Session["viewUtilFilters"] != null)
            {
                ViewEntityUtilizationSettingsFilter objViewFilter = (ViewEntityUtilizationSettingsFilter)Session["viewUtilFilters"];
                List<ViwUtilizationSetting> lstViewUtilDetails = new List<ViwUtilizationSetting>();
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstViewUtilDetails = new BLMisc().GetUtilizationSettingsList(objViewFilter);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<ViwUtilizationSetting>(lstViewUtilDetails);
                dtReport.TableName = "Entity_Utlilization_Details";
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("SYSTEM_ID");
                dtReport.Columns.Remove("REGION_ID");
                dtReport.Columns.Remove("PROVINCE_ID");
                dtReport.Columns.Remove("LAYER_ID");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_ON");

                dtReport.Columns["layer_name"].SetOrdinal(0);
                dtReport.Columns["network_status"].SetOrdinal(1);
                dtReport.Columns["Region"].SetOrdinal(2);
                dtReport.Columns["Province"].SetOrdinal(3);
                dtReport.Columns["utilization_range_Low"].SetOrdinal(4);
                dtReport.Columns["utilization_range_Moderate"].SetOrdinal(5);
                dtReport.Columns["utilization_range_High"].SetOrdinal(6);
                dtReport.Columns["utilization_range_Over"].SetOrdinal(7);
                dtReport.Columns["created_by_text"].SetOrdinal(8);
                dtReport.Columns["created on"].SetOrdinal(9);
                dtReport.Columns["modified_by_text"].SetOrdinal(10);
                dtReport.Columns["modified on"].SetOrdinal(11);

                dtReport.Columns["layer_name"].ColumnName = "Entity Type";
                dtReport.Columns["network_status"].ColumnName = "Network Status";
                dtReport.Columns["Region"].ColumnName = "Region";
                dtReport.Columns["Province"].ColumnName = "Province";
                dtReport.Columns["utilization_range_Low"].ColumnName = "Low(%)";
                dtReport.Columns["utilization_range_Moderate"].ColumnName = "Moderate(%)";
                dtReport.Columns["utilization_range_High"].ColumnName = "High(%)";
                dtReport.Columns["utilization_range_Over"].ColumnName = "Over(%)";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["created on"].ColumnName = "Created On";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                dtReport.Columns["modified on"].ColumnName = "Modified On";
                var filename = "UtilizationDetails";
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

        [HttpPost]
        public JsonResult DeleteUtilizationSettingsById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BLMisc().DeleteUtilizationSettingDetailById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Entity Utilization deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Entity Utilization!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Entity Utilization not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindProviceByRegionId(ProvinceIn objProvinceIn)
        {
            objProvinceIn.userId = Convert.ToInt32(Session["user_id"]);
            var objResp = new BLLayer().GetProvinceByRegionId(objProvinceIn);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        #endregion

        #region AddNewEntityUtilization
        public ActionResult AddEntityUtilization(int system_id)
        {
            AddNewEntityUtilization objutil = system_id > 0 ? new BLMiscellaneous().GetUtilizationDetailByID(system_id) : new AddNewEntityUtilization();
            BindUtilizationSaveDropdown(ref objutil);
            return PartialView("_AddUtilizationSetting", objutil);
        }
        public ActionResult SaveEntityUtilization(AddNewEntityUtilization _objaddnewutil)
        {
            try
            {
                ModelState.Clear();
                PageMessage objMsg = new PageMessage();
                _objaddnewutil.user_id = Convert.ToInt32(Session["user_id"]);
                var isNew = _objaddnewutil.system_id > 0 ? false : true;

                if (_objaddnewutil.system_id == 0)
                {
                    var objUtilizationExist = new BLMiscellaneous().ChkUtilizationExist(_objaddnewutil);
                    if (objUtilizationExist != null)
                    {
                        objMsg.status = ResponseStatus.FAILED.ToString();
                        objMsg.message = "Entity Utilization Setting already exist!";
                        _objaddnewutil.pageMsg = objMsg;
                        BindUtilizationSaveDropdown(ref _objaddnewutil);
                        return PartialView("_AddUtilizationSetting", _objaddnewutil);
                    }
                }
                // save utilization detail
                _objaddnewutil = new BLMiscellaneous().SaveEntityUtilizationDetails(_objaddnewutil);
                if (_objaddnewutil.system_id > 0)
                {
                    if (isNew)
                    {
                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.isNewEntity = isNew;
                        objMsg.message = "Entity Utilization detail saved successfully!";
                    }
                    else
                    {
                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.message = "Entity Utilization detail updated successfully!";
                    }
                    _objaddnewutil = new AddNewEntityUtilization();
                }
                else
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "Error while saving utilization detail!";
                }
                _objaddnewutil.pageMsg = objMsg;
                BindUtilizationSaveDropdown(ref _objaddnewutil);
                return PartialView("_AddUtilizationSetting", _objaddnewutil);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void BindUtilizationSaveDropdown(ref AddNewEntityUtilization objUtil)
        {
            //Bind Layers..   
            objUtil.lstLayers = new BLLayer().GetLayerDetailsForUtilization();
            objUtil.lstUtilizationReportLayers = objUtil.lstLayers.Where(m => m.layer_name == "Cable").ToList();
            //Bind Regions..
            objUtil.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (objUtil.region_id > 0)
            {
                objUtil.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { userId = Convert.ToInt32(Session["user_id"]), regionIds = objUtil.region_id.ToString() });
            }
        }
        #endregion

        #region DropDown_Master

        [HttpPost]
        public JsonResult getDropMasterDetailsByLayerId(int Layer_Id, bool IsFilter = false)
        {
            var result = blVendorSpecification.GetDropDownListbyLayerId(Layer_Id, IsFilter);
            var list = result.DistinctBy(x => x.dropdown_type);
            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getPrentDetailsByLayerId(int Layer_Id, string Mapping_DrpType)
        {
            List<ParentDropdownMasterMapping> objDropdownMasterMapping = new List<ParentDropdownMasterMapping>();
            dropdown_master objdropdownMaster = blVendorSpecification.GetParentDropDownListbyId(Mapping_DrpType);
            if (objdropdownMaster.parent_id > 0)
            {
                var reslt = blVendorSpecification.GetDropDownParentListbyId(objdropdownMaster.parent_id);
                objDropdownMasterMapping = blVendorSpecification.GetPrentDetailsByLayerId(Layer_Id, reslt.dropdown_type);
            }
            else
            {
                objDropdownMasterMapping.Add(new ParentDropdownMasterMapping
                {
                    dropdown_mapping_key = 0,
                    dropdown_mapping_value = "#",
                    dropdown_value = "#"
                });
            }
            return Json(objDropdownMasterMapping, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDropdownMasterRowCount(string layer_name, string fieldname, string value)
        {

            var result = blVendorSpecification.GetDropdownMasterRowCount(0, layer_name, fieldname, value);

            return Json(result[0], JsonRequestBehavior.AllowGet);
        }


        public ActionResult DropDownMaster(ViewEntityDropdownMasterSettings objDropdownMasterSettings, int page = 0, string sort = "", string sortdir = "", string msg = "", string status = "")
        {
            if (sort != "" || page != 0)
            {
                objDropdownMasterSettings.objdrpFilter = (ViewEntityDropdownMasterSettingsFilter)Session["viewDropdownFilters"];
                objDropdownMasterSettings.layer_id = objDropdownMasterSettings.objdrpFilter.layer_id;
                objDropdownMasterSettings.dropdown_type = objDropdownMasterSettings.objdrpFilter.dropdown_type;
                objDropdownMasterSettings.Value = objDropdownMasterSettings.objdrpFilter.dropdown_value == null ? objDropdownMasterSettings.objdrpFilter.dropdown_value :
                    objDropdownMasterSettings.objdrpFilter.dropdown_value.Trim();

            }
            else
            {
                objDropdownMasterSettings.objdrpFilter.id = 0;
                objDropdownMasterSettings.objdrpFilter.layer_id = objDropdownMasterSettings.layer_id;
                objDropdownMasterSettings.objdrpFilter.dropdown_type = objDropdownMasterSettings.dropdown_type;
                objDropdownMasterSettings.objdrpFilter.dropdown_value = objDropdownMasterSettings.Value == null ? objDropdownMasterSettings.Value :
                    objDropdownMasterSettings.Value.Trim();

            }

            objDropdownMasterSettings.lstLayers = new BLLayer().GetAllLayerDetail().Where(x => x.isvisible == true).ToList();
            objDropdownMasterSettings.objdrpFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objDropdownMasterSettings.objdrpFilter.currentPage = page == 0 ? 1 : page;
            objDropdownMasterSettings.objdrpFilter.sort = sort;
            objDropdownMasterSettings.objdrpFilter.orderBy = sortdir;
            objDropdownMasterSettings.lstDropdownTypes = objDropdownMasterSettings.layer_id > 0 ? blVendorSpecification.GetDropDownListbyLayerId(objDropdownMasterSettings.layer_id, false).DistinctBy(x => x.dropdown_type).ToList() : new List<dropdown_master>();
            objDropdownMasterSettings.lstViewDropdownMasterSetting = blVendorSpecification.GetDropdownMasterSettingsList(objDropdownMasterSettings.objdrpFilter);
            objDropdownMasterSettings.objdrpFilter.totalRecord = objDropdownMasterSettings.lstViewDropdownMasterSetting.Count > 0 ?
                objDropdownMasterSettings.lstViewDropdownMasterSetting[0].totalRecords : 0;
            if (!string.IsNullOrWhiteSpace(msg))
            {
                objDropdownMasterSettings.pageMsg = new PageMessage() { message = msg, status = status };
            }
            Session["viewDropdownFilters"] = objDropdownMasterSettings.objdrpFilter;
            return View(objDropdownMasterSettings);
        }

        [HttpPost]
        public JsonResult DeleteDropdownMaster(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();

            var result = blVendorSpecification.DeleteDropdownMaster(id);
            if (result[0] == 1)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Record has been successfully deleted!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "No record found";
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDropdownDetails(int layer_id, string fieldname, string Value)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var checkRecord = blVendorSpecification.GetDropDownListbyDropdowndetails(layer_id, fieldname, Value.Trim());
            if (checkRecord > 0)
            {
                objResp.message = "Record already exists!";
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.result = checkRecord.ToString();
            }
            else
            {
                objResp.status = ResponseStatus.OK.ToString();

            }
            return Json(objResp, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SaveDropdownMaster(ViewEntityDropdownMasterSettings _objdrpSetting)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            if (_objdrpSetting.id != 0)
            {
                var checkRecord = blVendorSpecification.GetDropDownListbyDropdowndetails(_objdrpSetting.layer_id, _objdrpSetting.dropdown_type, _objdrpSetting.Value);
                if (checkRecord == 0 || _objdrpSetting.OldValue == _objdrpSetting.Value)
                {
                    int UserId = Convert.ToInt32(Session["user_id"]);
                    var finalresult = blVendorSpecification.UpdateDropdownMaster
                       (_objdrpSetting.id, _objdrpSetting.layer_id, _objdrpSetting.dropdown_type, _objdrpSetting.OldValue, _objdrpSetting.Value,
                       _objdrpSetting.IsVisible, UserId);
                    if (finalresult[0] == 1)
                    {
                        objMsg.message = "Dropdown Master updated successfully!";
                        objMsg.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        objMsg.message = "Record not found";
                        objMsg.status = ResponseStatus.FAILED.ToString();

                    }
                }
            }
            else
            {
                var newDropdownMaster = new dropdown_master();
                newDropdownMaster.created_by = Convert.ToInt32(Session["user_id"]);
                newDropdownMaster.layer_id = _objdrpSetting.layer_id;
                newDropdownMaster.dropdown_key = _objdrpSetting.Value.Trim();
                newDropdownMaster.dropdown_type = _objdrpSetting.dropdown_type;
                newDropdownMaster.dropdown_value = _objdrpSetting.Value.Trim();
                newDropdownMaster.dropdown_status = _objdrpSetting.IsVisible;
                newDropdownMaster.db_column_name = _objdrpSetting.dropdown_type;
                newDropdownMaster.is_action_allowed = true;
                newDropdownMaster.is_active = true;
                newDropdownMaster.parent_value = _objdrpSetting.parent_value;
                newDropdownMaster.parent_id = _objdrpSetting.dropdown_mapping_key;
                var NewRecord = blVendorSpecification.SaveDropDownMasterdetails(newDropdownMaster);
                if (NewRecord == 1)
                {
                    objMsg.message = "Dropdown Master saved successfully!";
                    objMsg.status = ResponseStatus.OK.ToString();
                }
                else if (NewRecord == -1)
                {
                    objMsg.message = "Record already exists!";
                    objMsg.status = ResponseStatus.OK.ToString();

                }

            }
            _objdrpSetting.dropdown_type = " ";
            _objdrpSetting.Value = "";
            _objdrpSetting.OldValue = "";
            _objdrpSetting.id = 0;
            _objdrpSetting.layer_id = 0;
            //DropDownMaster(_objdrpSetting);
            _objdrpSetting.pageMsg = objMsg;
            // return Json(objMsg, JsonRequestBehavior.AllowGet);
            //we can also set the msg here after calling obj
            return PartialView("_AddDropdownMaster", _objdrpSetting);
            //return PartialView("DropdownMaster", _objdrpSetting);
        }

        public ActionResult AddEntityDropdownMaster(int id)
        {
            var objdropdownmasterSetting = new ViewEntityDropdownMasterSettings();
            var objdropdownmasterMapping = new DropdownMasterMapping();
            objdropdownmasterSetting.lstLayers = new BLLayer().GetAllDropdownLayerDetail().ToList();
            if (id > 0)
            {
                dropdown_master objdropdownMaster = blVendorSpecification.GetDropDownListbyId(id);
                objdropdownmasterSetting.layer_id = objdropdownMaster.layer_id;
                objdropdownmasterSetting.dropdown_type = objdropdownMaster.dropdown_type;
                objdropdownmasterSetting.Value = objdropdownMaster.dropdown_value;
                objdropdownmasterSetting.IsVisible = objdropdownMaster.dropdown_status;
                objdropdownmasterSetting.OldValue = objdropdownMaster.dropdown_value;
                objdropdownmasterMapping = blVendorSpecification.GetMappingDetailsbyId(objdropdownMaster.parent_id);
                objdropdownmasterSetting.dropdown_mapping_key = objdropdownmasterMapping.parent_mapping_id;
                objdropdownmasterSetting.parent_value = objdropdownMaster.parent_value;
                var reslt = blVendorSpecification.GetDropDownParentListbyId(objdropdownMaster.parent_id);
                if (reslt != null)
                {
                    objdropdownmasterSetting.dropdown_parent_value = reslt.dropdown_type;
                    objdropdownmasterSetting.objDropdownMasterMapping = blVendorSpecification.GetPrentDetailsByLayerId(objdropdownMaster.layer_id, reslt.dropdown_type);
                }
                objdropdownmasterSetting.id = objdropdownMaster.id;
                objdropdownmasterSetting.lstLayers.Where(x => x.layer_id == objdropdownMaster.layer_id);
            }
            else
            {
                objdropdownmasterSetting.IsVisible = true;
            }

            return PartialView("_AddDropdownMaster", objdropdownmasterSetting);
        }


        [HttpGet]
        public void ExportDropdownMasterDetails()
        {
            if (Session["viewDropdownFilters"] != null)
            {
                ViewEntityDropdownMasterSettingsFilter objdropFilter = (ViewEntityDropdownMasterSettingsFilter)Session["viewDropdownFilters"];
                List<ViewDropDownMasterSetting> lstViewdropDetails = new List<ViewDropDownMasterSetting>();
                objdropFilter.currentPage = 0;
                objdropFilter.pageSize = 0;
                lstViewdropDetails = blVendorSpecification.GetDropdownMasterSettingsList(objdropFilter);

                DataTable dtReport = new DataTable();

                dtReport = MiscHelper.ListToDataTable<ViewDropDownMasterSetting>(lstViewdropDetails);
                dtReport.TableName = "DropdownMaster_Details";
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.DefaultView.Sort = "S_NO asc";
                dtReport = dtReport.DefaultView.ToTable();
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("DROPDOWN_STATUS");
                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("ISVISIBLE");
                dtReport.Columns.Remove("LAYER_ID");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");

                dtReport.Columns.Remove("is_action_allowed");
                dtReport.Columns["S_NO"].ColumnName = "Sr.No";
                dtReport.Columns["layer_name"].ColumnName = "Entity Name";
                dtReport.Columns["DROPDOWN_TYPE"].ColumnName = "Dropdown Type";
                dtReport.Columns["DROPDOWN_VALUE"].ColumnName = "Value";
                dtReport.Columns["STATUS"].ColumnName = "Status";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";

                var filename = "DropdownDetails";
                ExportDataTableToExcel(dtReport, "Export_" + filename + "_" +
                    DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        #endregion
        #region Application Logs
        public ActionResult ViewErrorLog(ViewErrorLogFilter objErrorLogFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            if (sort != "" || page != 0)
            {
                objErrorLogFilter = (ViewErrorLogFilter)Session["gridAttr"];
            }
            objErrorLogFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objErrorLogFilter.currentPage = page == 0 ? 1 : page;
            objErrorLogFilter.sort = sort;
            objErrorLogFilter.orderBy = sortdir;
            Session["viewErrorLog"] = objErrorLogFilter;
            objErrorLogFilter.listErrorLog = new BLErrorLog().GetErrorLogDetails(objErrorLogFilter);
            objErrorLogFilter.totalRecord = objErrorLogFilter.listErrorLog != null && objErrorLogFilter.listErrorLog.Count > 0 ? objErrorLogFilter.listErrorLog[0].totalRecords : 0;
            Session["gridAttr"] = objErrorLogFilter;
            return View("_ViewErrorLog", objErrorLogFilter);
        }
        public ActionResult ReadMore(int queryId)
        {
            ViewErrorLogFilter objErrorLogFilter = new ViewErrorLogFilter();
            objErrorLogFilter.objViewMore = new BLErrorLog().getFullText(queryId);
            objErrorLogFilter.logtype = "WebErrorLog";
            return PartialView("_ReadMore", objErrorLogFilter);
        }

        public void DownloadErrorLogs()
        {
            if (Session["viewErrorLog"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewErrorLogFilter objErrorLogFilter = new ViewErrorLogFilter();
                objErrorLogFilter = (ViewErrorLogFilter)Session["viewErrorLog"];
                List<AccessoriesMapping> lstViewUserDetails = new List<AccessoriesMapping>();
                objErrorLogFilter.currentPage = 0;
                objErrorLogFilter.pageSize = 0;
                objErrorLogFilter.listErrorLog = new BLErrorLog().GetErrorLogDetails(objErrorLogFilter);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<ErrorLog>(objErrorLogFilter.listErrorLog);
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns["CREATED_ON"].ColumnName = "Created On";
                dtReport.Columns["USER_NAME"].ColumnName = "User Name";
                dtReport.Columns["CLIENT_IP"].ColumnName = "Client IP";
                dtReport.Columns["SERVER_IP"].ColumnName = "Server IP";
                dtReport.Columns["BROWSER_NAME"].ColumnName = "Browser Name";
                dtReport.Columns["STATUS_CODE"].ColumnName = "Status Code";
                dtReport.Columns["browser_version"].ColumnName = "Browser Version";
                dtReport.Columns["CONTROLLER_NAME"].ColumnName = "Controller Name";
                dtReport.Columns["ACTION_NAME"].ColumnName = "Action Name";
                dtReport.Columns["ERR_MESSAGE"].ColumnName = "Error Message";
                dtReport.Columns["ERR_LABEL"].ColumnName = "Error Label";
                dtReport.Columns["ERR_TYPE"].ColumnName = "Error Type";
                dtReport.Columns["ERR_DESCRIPTION"].ColumnName = "Error Description";
                dtReport.Columns["STACK_TRACE"].ColumnName = "Stack Trace";

                var filename = "ErrorLogList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        private void ExportAccessoriesData(DataTable dtReport, string fileName)
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



        public ActionResult ViewErrorApiLog(ViewErrorLogFilter objErrorLogFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            if (sort != "" || page != 0)
            {
                objErrorLogFilter = (ViewErrorLogFilter)Session["gridAttr"];
            }
            objErrorLogFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objErrorLogFilter.currentPage = page == 0 ? 1 : page;
            objErrorLogFilter.sort = sort;
            objErrorLogFilter.orderBy = sortdir;
            Session["viewErrorApiLog"] = objErrorLogFilter;
            objErrorLogFilter.listApiErrorLog = new BLErrorLog().GetApiErrorLogDetails(objErrorLogFilter);
            objErrorLogFilter.totalRecord = objErrorLogFilter.listApiErrorLog != null && objErrorLogFilter.listApiErrorLog.Count > 0 ? objErrorLogFilter.listApiErrorLog[0].totalRecords : 0;

            Session["gridAttr"] = objErrorLogFilter;
            return View("_ViewErrorApiLog", objErrorLogFilter);
        }

        public void DownloadErrorApiLogs()
        {
            if (Session["viewErrorApiLog"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewErrorLogFilter objErrorLogFilter = new ViewErrorLogFilter();
                objErrorLogFilter = (ViewErrorLogFilter)Session["viewErrorApiLog"];
                List<AccessoriesMapping> lstViewUserDetails = new List<AccessoriesMapping>();
                objErrorLogFilter.currentPage = 0;
                objErrorLogFilter.pageSize = 0;
                objErrorLogFilter.listApiErrorLog = new BLErrorLog().GetApiErrorLogDetails(objErrorLogFilter);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<ApiErrorLog>(objErrorLogFilter.listApiErrorLog);
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns["created_on"].ColumnName = "Created On";
                dtReport.Columns["USER_NAME"].ColumnName = "User Name";
                dtReport.Columns["controller_name"].ColumnName = "Controller Name";
                dtReport.Columns["action_name"].ColumnName = "Action Name";
                dtReport.Columns["err_message"].ColumnName = "Error Message";
                dtReport.Columns["REQUEST_DATA"].ColumnName = "Request Data";
                dtReport.Columns["err_label"].ColumnName = "Error Label";
                dtReport.Columns["err_type"].ColumnName = "Error Type";
                dtReport.Columns["err_description"].ColumnName = "Error Description";
                dtReport.Columns["stack_trace"].ColumnName = "Stack Trace";

                var filename = "ApiErrorLogList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public ActionResult ReadMoreApi(int queryId)
        {
            ViewErrorLogFilter objErrorLogFilter = new ViewErrorLogFilter();
            objErrorLogFilter.objApiViewMore = new BLErrorLog().getApiFullText(queryId);
            objErrorLogFilter.logtype = "ApiErrorLog";
            return PartialView("_ReadMore", objErrorLogFilter);
        }
       
        
        public ActionResult ViewApiRequestLog(ViewErrorLogFilter objErrorLogFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            if (sort != "" || page != 0)
            {
                objErrorLogFilter = (ViewErrorLogFilter)Session["gridAttr"];
            }
            objErrorLogFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objErrorLogFilter.currentPage = page == 0 ? 1 : page;
            objErrorLogFilter.sort = sort;
            objErrorLogFilter.orderBy = sortdir;
            Session["viewApiRequestLog"] = objErrorLogFilter;
            objErrorLogFilter.listApiRequestLog = new BLAPIRequestLog().GetApiRequestLogDetails(objErrorLogFilter);
            objErrorLogFilter.totalRecord = objErrorLogFilter.listApiRequestLog != null && objErrorLogFilter.listApiRequestLog.Count > 0 ? objErrorLogFilter.listApiRequestLog[0].totalRecords : 0;
            Session["gridAttr"] = objErrorLogFilter;
            return View("_ViewApiRequestLog", objErrorLogFilter);
        }

        public void DownloadApiRequestLog()
        {
            if (Session["viewApiRequestLog"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewErrorLogFilter objErrorLogFilter = new ViewErrorLogFilter();
                objErrorLogFilter = (ViewErrorLogFilter)Session["viewApiRequestLog"];
                List<AccessoriesMapping> lstViewUserDetails = new List<AccessoriesMapping>();
                objErrorLogFilter.currentPage = 0;
                objErrorLogFilter.pageSize = 0;
                objErrorLogFilter.listApiRequestLog = new BLAPIRequestLog().GetApiRequestLogDetails(objErrorLogFilter);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<APIRequestLog>(objErrorLogFilter.listApiRequestLog);
                dtReport.Columns.Remove("log_id");
                dtReport.Columns["created_on"].ColumnName = "Created On";
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns["SOURCE"].ColumnName = "Source";
                dtReport.Columns["USER_NAME"].ColumnName = "User Name";
                dtReport.Columns["controller_name"].ColumnName = "Controller Name";
                dtReport.Columns["action_name"].ColumnName = "Action Name";
                dtReport.Columns["OS_NAME"].ColumnName = "OS Name";
                dtReport.Columns["OS_VERSION"].ColumnName = "OS Version";
                dtReport.Columns["REQUEST"].ColumnName = "Request";
                dtReport.Columns["HEADER_ATTRIBUTE"].ColumnName = "Header Attribute";


                var filename = "ApiRequestLogList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public ActionResult ReadMoreApiRequest(int queryId)
        {
            ViewErrorLogFilter objErrorLogFilter = new ViewErrorLogFilter();
            objErrorLogFilter.objApiRequestViewMore = new BLAPIRequestLog().getApiFullText(queryId);
            objErrorLogFilter.logtype = "ApiRequestLog";
            return PartialView("_ReadMore", objErrorLogFilter);
        }

        public ActionResult ViewGisApiLog(ViewErrorLogFilter objErrorLogFilter, int page = 0, string sort = "", string sortdir = "")
        {
            if (sort != "" || page != 0)
            {
                objErrorLogFilter = (ViewErrorLogFilter)Session["gridAttr"];
            }
            objErrorLogFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objErrorLogFilter.currentPage = page == 0 ? 1 : page;
            objErrorLogFilter.sort = sort;
            objErrorLogFilter.orderBy = sortdir;
            Session["viewGisApiLog"] = objErrorLogFilter;
            objErrorLogFilter.listGisApiLog = new BLAPIRequestLog().GetGisApiLogDetails(objErrorLogFilter);
            objErrorLogFilter.totalRecord = objErrorLogFilter.listGisApiLog != null && objErrorLogFilter.listGisApiLog.Count > 0 ? objErrorLogFilter.listGisApiLog[0].totalRecords : 0;
            Session["gridAttr"] = objErrorLogFilter;
            return View("_ViewGisApiLog", objErrorLogFilter);
        }

        public void DownloadGisApiLog()
        {
            if (Session["viewGisApiLog"] != null)
            {
                ViewErrorLogFilter objErrorLogFilter = new ViewErrorLogFilter();
                objErrorLogFilter = (ViewErrorLogFilter)Session["viewGisApiLog"];
                List<AccessoriesMapping> lstViewUserDetails = new List<AccessoriesMapping>();
                objErrorLogFilter.currentPage = 0;
                objErrorLogFilter.pageSize = 0;
                objErrorLogFilter.listGisApiLog = new BLAPIRequestLog().GetGisApiLogDetails(objErrorLogFilter);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<GisApiLog>(objErrorLogFilter.listGisApiLog);
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("user_id"); 
                dtReport.Columns.Remove("entity_type"); 
                dtReport.Columns.Remove("system_id"); 
                dtReport.Columns["api_url"].ColumnName = "Api Url";
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns["request"].ColumnName = "Request";
                dtReport.Columns["response"].ColumnName = "response";
                dtReport.Columns["request_time"].ColumnName = "Request Time";
                dtReport.Columns["response_time"].ColumnName = "Response Time";
                dtReport.Columns["api_type"].ColumnName = "Api Type";
                dtReport.Columns["gdb_version"].ColumnName = "Gdb Version";
                dtReport.Columns["gis_design_id"].ColumnName = "gis design Id";
                var filename = "GisApiLogList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public ActionResult ReadMoreGisApiLog(int id)
        {
            ViewErrorLogFilter objErrorLogFilter = new ViewErrorLogFilter();
            objErrorLogFilter.objGisApiViewMore = new BLAPIRequest().getGisApiLogDetailById(id);
            objErrorLogFilter.logtype = "GisApiLog";
            return PartialView("_ReadMore", objErrorLogFilter);
        }
        #endregion

        #region Help & FAQ,s
        public ActionResult ViewFaq(ViewFaqFilter objFaqFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objFaqFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objFaqFilter.currentPage = page == 0 ? 1 : page;
            objFaqFilter.sort = sort;
            objFaqFilter.orderBy = sortdir;
            Session["viewFaq"] = objFaqFilter;
            objFaqFilter.listFaq = new BLHelp().GetFaqDetails(objFaqFilter);
            objFaqFilter.totalRecord = objFaqFilter.listFaq != null && objFaqFilter.listFaq.Count > 0 ? objFaqFilter.listFaq[0].totalRecords : 0;
            return View("_ViewFaq", objFaqFilter);
        }
        public void DownloadFaq()
        {
            if (Session["viewFaq"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewFaqFilter objFaqFilter = new ViewFaqFilter();
                objFaqFilter = (ViewFaqFilter)Session["viewFaq"];
                objFaqFilter.currentPage = 0;
                objFaqFilter.pageSize = 0;
                objFaqFilter.listFaq = new BLHelp().GetFaqDetails(objFaqFilter);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<FAQMaster>(objFaqFilter.listFaq);
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("LSTCATEGORIES");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns["category"].ColumnName = "Category";
                dtReport.Columns["question"].ColumnName = "Question";
                dtReport.Columns["answer"].ColumnName = "Answer";
                dtReport.Columns["created_on"].ColumnName = "Created On";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_on"].ColumnName = "Modified On";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                var filename = "FaqList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public ActionResult AddNewFaq(int id)
        {
            FAQMaster objFaq = new FAQMaster();
            objFaq = id > 0 ? new BLHelp().GetFaqById(id) : new FAQMaster();
            return PartialView("_AddFaq", objFaq);
        }
        public ActionResult SaveFaq(FAQMaster objFaqMst)
        {
            ModelState.Clear();
            if (TryValidateModel(objFaqMst))
            {
                var isNew = objFaqMst.id > 0 ? false : true;
                objFaqMst = new BLHelp().SaveFaq(objFaqMst, Convert.ToInt32(Session["user_id"]));
                objFaqMst.pageMsg.status = ResponseStatus.OK.ToString();
                if (isNew)
                    objFaqMst.pageMsg.message = "FAQ has been saved successfully!";
                else
                    objFaqMst.pageMsg.message = "FAQ has been updated successfully!";

            }
            return Json(objFaqMst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewFaqUserManual(ViewFaqFilter objFaqFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objFaqFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objFaqFilter.currentPage = page == 0 ? 1 : page;
            objFaqFilter.sort = sort;
            objFaqFilter.orderBy = sortdir;
            Session["ViewFaqUserManual"] = objFaqFilter;
            objFaqFilter.listFaqUserManual = new BLHelp().GetFaqUserManualDetails(objFaqFilter);
            foreach (var item in objFaqFilter.listFaqUserManual)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            objFaqFilter.totalRecord = objFaqFilter.listFaqUserManual != null && objFaqFilter.listFaqUserManual.Count > 0 ? objFaqFilter.listFaqUserManual[0].totalRecords : 0;
            return View("_ViewFaqUserManual", objFaqFilter);
        }
        public static String BytesToString(long byteCount)
        {
            string[] suf = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0 " + suf[1];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }
        public void DownloadFaqUserManual()
        {
            if (Session["ViewFaqUserManual"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewFaqFilter objFaqFilter = new ViewFaqFilter();
                objFaqFilter = (ViewFaqFilter)Session["ViewFaqUserManual"];
                objFaqFilter.currentPage = 0;
                objFaqFilter.pageSize = 0;
                objFaqFilter.listFaqUserManual = new BLHelp().GetFaqUserManualDetails(objFaqFilter);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<FAQ_UserManual>(objFaqFilter.listFaqUserManual);
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("DISPLAY_NAME");
                dtReport.Columns.Remove("FILE_SIZE_CONVERTED");
                dtReport.Columns["category"].ColumnName = "Category";
                dtReport.Columns["file_name"].ColumnName = "File Name";
                dtReport.Columns["file_size"].ColumnName = "File Size";
                dtReport.Columns["file_extension"].ColumnName = "File Extension";
                dtReport.Columns["file_url"].ColumnName = "File Url";
                dtReport.Columns["created_on"].ColumnName = "Created On";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_on"].ColumnName = "Modified On";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                var filename = "FaqUserManualList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        public ActionResult AddNewFaqUserManual(int id)
        {
            FAQ_UserManual objFaqUM = new FAQ_UserManual();
            objFaqUM = id > 0 ? new BLHelp().GetUserManualById(id) : new FAQ_UserManual();
            return PartialView("_AddFaqUserManual", objFaqUM);
        }
        public ActionResult UploadUserManual(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    var category = collection["category"];
                    var id = collection["id"];
                    var featureName = collection["feature_name"];
                    var attachmentType = "Document";
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string FileName = file.FileName;
                        string strfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp();
                        string strNewfilename = strfilename + Path.GetExtension(FileName);
                        string strFilePath = "";
                        if (!string.IsNullOrEmpty(featureName))
                        {
                            strFilePath = UploadfileOnFTP(category, file, attachmentType, strNewfilename, featureName);
                        }
                        else
                        {
                            strFilePath = UploadfileOnFTP(category, file, attachmentType, strNewfilename);
                        }

                        // get User Detail..
                        User objUser = (User)(Session["userDetail"]);
                        FAQ_UserManual objAttachment = new FAQ_UserManual();
                        objAttachment.id = Convert.ToInt32(id);
                        objAttachment.category = category;
                        objAttachment.file_name = strfilename;
                        objAttachment.display_name = strfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_url = strFilePath + strNewfilename;
                        objAttachment.created_by = objUser.user_id;
                        objAttachment.file_size = file.ContentLength;

                        //Save Image on FTP and related detail in database..
                        var savefile = new BLHelp().SaveUserManual(objAttachment);
                    }
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_154;
                    jResp.status = StatusCodes.OK.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadUserManual()", "Miscellaneous", ex);
                    jResp.message = "Error in uploading image!";
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = "No files selected.";
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }
        static string UploadfileOnFTP(string sCategory, HttpPostedFileBase postedFile, string sUploadType, string newfilename, string featureType = null)
        {
            try
            {
                string strFTPFilePath = "";
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {
                    // Create Directory if not exists and get Final FTP path to save file..
                    strFTPFilePath = CreateNestedDirectoryOnFTP(strFTPPath, strFTPUserName, strFTPPassWord, featureType, sCategory);

                    //Prepare FTP Request..
                    FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(strFTPFilePath + newfilename);
                    ftpReq.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                    ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpReq.UseBinary = true;

                    //Save file temporarily on local path..
                    string savepath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "//Attachments/";
                    postedFile.SaveAs(savepath + @"\" + newfilename);
                    byte[] b = System.IO.File.ReadAllBytes(@"" + savepath + "/" + newfilename);
                    ftpReq.ContentLength = b.Length;
                    using (Stream s = ftpReq.GetRequestStream())
                    {
                        s.Write(b, 0, b.Length);
                    }

                    try
                    {
                        ftpReq.GetResponse();
                    }
                    catch { throw; }
                    finally
                    {
                        //Delete from local path.. 
                        System.IO.File.Delete(@"" + savepath + "/" + newfilename);
                    }
                }
                return strFTPFilePath.Replace(strFTPPath, ""); // return file path
            }
            catch { throw; }
        }
        private static bool isValidFTPConnection(string ftpUrl, string strUserName, string strPassWord)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(strUserName, strPassWord);
                request.GetResponse();
            }
            catch (WebException ex) { throw new Exception("Unable to connect to FTP Server", ex); }
            return true;
        }

        //Code by Nihal Singh on 7th feb 2019
        //CreateNestedDirectoryOnFTP is create the Directory on FTP
        private static string CreateNestedDirectoryOnFTP(string strFTPPath, string strUserName, string strPassWord, params string[] directories)
        {
            try
            {
                FtpWebRequest reqFTP;
                string strFTPFilePath = strFTPPath;
                foreach (string directory in directories)
                {
                    if (!string.IsNullOrEmpty(directory) && directory.Trim() != "")
                    {
                        strFTPFilePath += directory + "/";
                        try
                        {
                            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(strFTPFilePath));
                            reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                            reqFTP.UseBinary = true;
                            reqFTP.Credentials = new NetworkCredential(strUserName, strPassWord);
                            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                            Stream ftpStream = response.GetResponseStream();
                            ftpStream.Close();
                            response.Close();
                        }
                        catch (WebException ex)
                        {
                            FtpWebResponse response = (FtpWebResponse)ex.Response;
                            //Directory already exists
                            if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable) { response.Close(); }
                            //Error in creating new directory on FTP..
                            else { throw new Exception("Error in creating directory/sub-directory!", ex); }
                        }
                    }
                }
                return strFTPFilePath;
            }
            catch { throw; }
        }

        public JsonResult DeleteFaqById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLHelp().DeleteFaqById(id);
            if (result > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Faq deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Faq could not deleted!";
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteUserManualById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLHelp().DeleteUserManualById(id);
            if (result > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "User Manual deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "User Manual could not deleted!";
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FaqReadMore(int queryId)
        {
            ReadMore objMore = new ReadMore();
            objMore.querytext = new BLHelp().getFullText(queryId);
            return PartialView("_ReadMoreFaq", objMore);
        }
        #endregion
       
        #region Termination Point Management
        public ActionResult ViewTerminationPoint(ViewTPMaster objTPMaster, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objTPMaster.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objTPMaster.currentPage = page == 0 ? 1 : page;
            objTPMaster.sort = sort;
            objTPMaster.orderBy = sortdir;
            Session["viewTerminationPoint"] = objTPMaster;
            objTPMaster.listTPM = new BLMisc().GetTPDetails(objTPMaster);
            objTPMaster.totalRecord = objTPMaster.listTPM != null && objTPMaster.listTPM.Count > 0 ? objTPMaster.listTPM[0].totalRecords : 0;
            return View("_ViewTerminationPoint", objTPMaster);
        }
        public void DownloadViewTerminationPoint()
        {
            if (Session["viewTerminationPoint"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewTPMaster objTPMaster = new ViewTPMaster();
                objTPMaster = (ViewTPMaster)Session["viewTerminationPoint"];
                objTPMaster.currentPage = 0;
                objTPMaster.pageSize = 0;
                objTPMaster.listTPM = new BLMisc().GetTPDetails(objTPMaster);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<TerminationPointMaster>(objTPMaster.listTPM);

                dtReport.Columns.Add("IS ISP TP", typeof(string));
                dtReport.Columns.Add("IS OSP TP", typeof(string));
                dtReport.Columns.Add("Status", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["IS ISP TP"] = Convert.ToBoolean(dtReport.Rows[i]["is_isp_tp"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["IS OSP TP"] = Convert.ToBoolean(dtReport.Rows[i]["is_osp_tp"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_enabled"]) == true ? "Active" : "InActive";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("LAYER_ID");
                dtReport.Columns.Remove("TP_LAYER_ID");
                dtReport.Columns.Remove("LSTLINELAYERDETAILS");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("LSTTPLAYERDETAILS");
                dtReport.Columns.Remove("LSTTPLAYER");
                dtReport.Columns.Remove("is_isp_tp");
                dtReport.Columns.Remove("is_osp_tp");
                dtReport.Columns.Remove("is_enabled");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");

                dtReport.Columns["layer_name"].SetOrdinal(0);
                dtReport.Columns["tp_layer_name"].SetOrdinal(1);
                dtReport.Columns["IS ISP TP"].SetOrdinal(2);
                dtReport.Columns["IS OSP TP"].SetOrdinal(3);
                dtReport.Columns["Status"].SetOrdinal(4);
                dtReport.Columns["Created On"].SetOrdinal(5);
                dtReport.Columns["created_by_text"].SetOrdinal(6);
                dtReport.Columns["Modified On"].SetOrdinal(7);
                dtReport.Columns["modified_by_text"].SetOrdinal(8);

                dtReport.Columns["layer_name"].ColumnName = "Entity Name";
                dtReport.Columns["tp_layer_name"].ColumnName = "TP Entity Name";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";

                var filename = "Termination Points";
                dtReport.TableName = "Termination Points";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        public ActionResult AddNewTerminationPoint(int id)
        {

            TerminationPointMaster objTerminationPoint = new TerminationPointMaster();

            objTerminationPoint = id > 0 ? new BLMisc().GetTerminationPointById(id) : new TerminationPointMaster();
            if (objTerminationPoint.id == 0)
                objTerminationPoint.is_enabled = true;
            BindTerminationPointDropdown(objTerminationPoint);
            return PartialView("_AddTerminationPoint", objTerminationPoint);
        }
        private void BindTerminationPointDropdown(dynamic obj)
        {
            string flag = "";
            if (obj.id > 0)
                flag = "Edit";

            var lstLayer = new BLLayer().GetLayerDetails();
            obj.lstLineLayerDetails = lstLayer.Where(m => m.geom_type == "Line" && m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            //obj.lstTPLayerDetails = lstLayer.Where(m => m.is_tp_layer == true && m.isvisible == true).OrderBy(m => m.layer_name).ToList();

            obj.lstTPLayerDetails = GetTPDropdownList(obj.layer_id, flag); ;
        }

        public List<TPDropdown> GetTPDropdownList(int layer_id = 0, string flag = "")
        {
            return new BLMisc().TPDropdownList(layer_id, flag);
        }
        public JsonResult BindTPDropdownList(int layer_id)
        {
            var objResp = GetTPDropdownList(layer_id, "");
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        public JsonResult SaveTerminationPoint(TerminationPointMaster objTPMaster)
        {
            ModelState.Clear();
            var user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();

            objTPMaster.created_by = user_id;
            var status = new BLMisc().SaveTerminationPoint(objTPMaster);
            //}
            if (status == "Update")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Termination Point updated successfully!";
            }
            else if (status == "Save")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Termination Point saved successfully!";
            }
            else if (status == "Failed")
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Entry already exists !";
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = status;
            }
            objTPMaster.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTerminationPointById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLMisc().DeleteTerminationPointById(id);
            if (result == "DELETE")
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Termination Point deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = result;
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion
       
        #region DU Management
        public ActionResult ViewTemplateColumn(ViewTemplateColumn objTemplateColumn, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objTemplateColumn.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objTemplateColumn.currentPage = page == 0 ? 1 : page;
            objTemplateColumn.sort = sort;
            objTemplateColumn.orderBy = sortdir;
            Session["viewTemplateColumn"] = objTemplateColumn;
            objTemplateColumn.listTemplateColumn = new BLMisc().GetTemplateColumn(objTemplateColumn);
            objTemplateColumn.totalRecord = objTemplateColumn.listTemplateColumn != null && objTemplateColumn.listTemplateColumn.Count > 0 ? objTemplateColumn.listTemplateColumn[0].totalRecords : 0;
            return View("_ViewTemplateColumn", objTemplateColumn);
        }
        public void DownloadViewTemplateColumn()
        {
            if (Session["viewTemplateColumn"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewTemplateColumn objTemplateColumn = new ViewTemplateColumn();
                objTemplateColumn = (ViewTemplateColumn)Session["viewTemplateColumn"];
                objTemplateColumn.currentPage = 0;
                objTemplateColumn.pageSize = 0;
                objTemplateColumn.listTemplateColumn = new BLMisc().GetTemplateColumn(objTemplateColumn);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<TemplateColumn>(objTemplateColumn.listTemplateColumn);

                dtReport.Columns.Add("Is Dropdown Base", typeof(string));
                dtReport.Columns.Add("IS Kml Attribute", typeof(string));
                dtReport.Columns.Add("Is Null Allowed", typeof(string));
                dtReport.Columns.Add("IS Excel Attribute", typeof(string));
                dtReport.Columns.Add("Is Mandatory", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Is Dropdown Base"] = Convert.ToBoolean(dtReport.Rows[i]["is_dropdown"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["IS Kml Attribute"] = Convert.ToBoolean(dtReport.Rows[i]["is_kml_attribute"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["Is Null Allowed"] = Convert.ToBoolean(dtReport.Rows[i]["is_nullable"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["IS Excel Attribute"] = Convert.ToBoolean(dtReport.Rows[i]["is_excel_attribute"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["Is Mandatory"] = Convert.ToBoolean(dtReport.Rows[i]["is_mandatory"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("LAYER_ID");
                dtReport.Columns.Remove("is_mandatory");
                dtReport.Columns.Remove("LSTLAYERDETAILS");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("lstTemplateColumnDropdown");
                dtReport.Columns.Remove("is_dropdown");
                dtReport.Columns.Remove("is_kml_attribute");
                dtReport.Columns.Remove("is_nullable");
                dtReport.Columns.Remove("is_excel_attribute");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");
                dtReport.Columns.Remove("column_sequence");

                dtReport.Columns["layer_name"].SetOrdinal(0);
                dtReport.Columns["template_column_name"].SetOrdinal(1);
                dtReport.Columns["display_column_data_type"].SetOrdinal(2);
                dtReport.Columns["db_column_name"].SetOrdinal(3);
                dtReport.Columns["db_column_data_type"].SetOrdinal(4);
                dtReport.Columns["example_value"].SetOrdinal(5);
                dtReport.Columns["max_length"].SetOrdinal(6);
                dtReport.Columns["Is Mandatory"].SetOrdinal(7);
                dtReport.Columns["Is Null Allowed"].SetOrdinal(8);
                dtReport.Columns["Is Dropdown Base"].SetOrdinal(9);
                dtReport.Columns["description"].SetOrdinal(10);
                dtReport.Columns["IS Kml Attribute"].SetOrdinal(11);
                dtReport.Columns["IS Excel Attribute"].SetOrdinal(12);
                dtReport.Columns["created_by_text"].SetOrdinal(13);
                dtReport.Columns["Created On"].SetOrdinal(14);
                dtReport.Columns["modified_by_text"].SetOrdinal(15);
                dtReport.Columns["Modified On"].SetOrdinal(16);

                dtReport.Columns["layer_name"].ColumnName = "Entity Name";
                dtReport.Columns["db_column_name"].ColumnName = "Db Column Name";
                dtReport.Columns["template_column_name"].ColumnName = "Display Column Name";
                dtReport.Columns["db_column_data_type"].ColumnName = "Db Column Data Type";
                dtReport.Columns["display_column_data_type"].ColumnName = "Display Column Data Type";
                dtReport.Columns["description"].ColumnName = "Description";
                dtReport.Columns["example_value"].ColumnName = "Example Value";
                dtReport.Columns["max_length"].ColumnName = "Max Length";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                var filename = "Uploadter Template Columns ";
                dtReport.TableName = "Template Columns";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        public ActionResult AddNewTemplateColumn(int id)
        {
            TemplateColumn objTemplateColumn = new TemplateColumn();
            objTemplateColumn = id > 0 ? new BLMisc().GetTemplateColumnById(id) : new TemplateColumn();

            BindTemplateColumnDropdown(objTemplateColumn);
            return PartialView("_AddTemplateColumn", objTemplateColumn);
        }
        private void BindTemplateColumnDropdown(dynamic obj)
        {
            var flag = "Edit";
            var lstLayer = new BLLayer().GetLayerDetails();
            obj.lstLayerDetails = lstLayer.Where(m => m.isvisible == true && m.is_data_upload_enabled == true).OrderBy(m => m.layer_name).ToList();
            obj.lstTemplateColumnDropdown = new BLMisc().GetTemplateColumnDropdown(obj.layer_id, flag);
        }
        public List<TemplateColumnDropdown> GetDb_Column_name(int layer_id = 0)
        {

            TemplateColumn objTemplateColumn = new TemplateColumn();
            var flag = "";
            return objTemplateColumn.lstTemplateColumnDropdown = new BLMisc().GetTemplateColumnDropdown(layer_id, flag);
        }
        public JsonResult BindDb_Column_name(int layer_id)
        {
            var objResp = GetDb_Column_name(layer_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        public JsonResult SaveTemplateColumn(TemplateColumn objTemplateColumn)
        {
            ModelState.Clear();
            var user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();
            if (objTemplateColumn.db_column_data_type == "character varying")
                objTemplateColumn.display_column_data_type = "Alphanumeric";
            else
                objTemplateColumn.display_column_data_type = "Decimal/Float";
            objTemplateColumn.created_by = user_id;
            var status = new BLMisc().SaveTemplateColumn(objTemplateColumn);
            //}
            if (status == "Update")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Template Column has been updated successfully!";
            }
            else if (status == "Save")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Template Column has been saved successfully!";
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Error has been saved in  Template Column  !";
            }

            objTemplateColumn.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTemplateColumnById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLMisc().DeleteTemplateColumnById(id);
            if (result == "DELETE")
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Template Column has been successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = result;
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Port Status Master
        public ActionResult ViewPortStatus(LogicalViewVM objPortStatusMaster, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objPortStatusMaster.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objPortStatusMaster.currentPage = page == 0 ? 1 : page;
            objPortStatusMaster.sort = sort;
            objPortStatusMaster.orderBy = sortdir;
            Session["viewPortStatus"] = objPortStatusMaster;
            objPortStatusMaster.listPortStatus = new BLMisc().GetportStatus(objPortStatusMaster);
            objPortStatusMaster.totalRecord = objPortStatusMaster.listPortStatus != null && objPortStatusMaster.listPortStatus.Count > 0 ? objPortStatusMaster.listPortStatus[0].totalRecords : 0;
            return View("_ViewPortStatus", objPortStatusMaster);
        }
        public ActionResult AddPortStatus(int id)
        {
            portStatusMaster objportStatus = new portStatusMaster();
            objportStatus = id > 0 ? new BLMisc().GetPortStatusById(id) : new portStatusMaster();
            if (id == 0)
                objportStatus.is_active = true;
            return PartialView("_AddPortStatus", objportStatus);
        }
        public JsonResult SavePortStatus(portStatusMaster objPortStatus)
        {
            ModelState.Clear();
            var user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();

            objPortStatus.created_by = user_id;
            var status = new BLMisc().SavePortStatus(objPortStatus);
            //}
            if (status == "Update")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Port Status has been updated successfully!";
            }
            else if (status == "Save")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Port Status has been saved successfully!";
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Error has been saved in  Port Status  !";
            }

            objPortStatus.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePortStatusById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLMisc().DeletePortStatusById(id);
            if (result.status == true)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = result.message; ;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = result.message;
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public void DownloadViewPortStatus()
        {
            if (Session["viewPortStatus"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                LogicalViewVM objportStatus = new LogicalViewVM();
                objportStatus = (LogicalViewVM)Session["viewPortStatus"];
                objportStatus.currentPage = 0;
                objportStatus.pageSize = 0;
                objportStatus.listPortStatus = new BLMisc().GetportStatus(objportStatus);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<portStatusMaster>(objportStatus.listPortStatus);

                dtReport.Columns.Add("iS Active", typeof(string));
                dtReport.Columns.Add("iS Manual", typeof(string));
                dtReport.Columns.Add("iS Splicing", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["iS Active"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["iS Manual"] = Convert.ToBoolean(dtReport.Rows[i]["is_manual_status"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["iS Splicing"] = Convert.ToBoolean(dtReport.Rows[i]["is_splicing_enabled"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("system_id");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("is_active");
                dtReport.Columns.Remove("is_manual_status");
                dtReport.Columns.Remove("is_splicing_enabled");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");

                dtReport.Columns["status"].SetOrdinal(0);
                dtReport.Columns["color_code"].SetOrdinal(1);
                dtReport.Columns["iS Active"].SetOrdinal(2);
                dtReport.Columns["iS Manual"].SetOrdinal(3);
                dtReport.Columns["iS Splicing"].SetOrdinal(4);
                dtReport.Columns["Created On"].SetOrdinal(5);
                dtReport.Columns["created_by_text"].SetOrdinal(6);
                dtReport.Columns["Modified On"].SetOrdinal(7);
                dtReport.Columns["modified_by_text"].SetOrdinal(8);

                dtReport.Columns["status"].ColumnName = "Status";
                dtReport.Columns["color_code"].ColumnName = "Color Code";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";

                var filename = "Port Status";
                dtReport.TableName = "Port Status";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public JsonResult CheckColorCode(string ColorCode, int system_id)
        {
            var result = new BLMisc().CheckColorCode(ColorCode, system_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Association Mapping
        public ActionResult AssociationMapping(ViewAEMaster objAEaster, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objAEaster.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objAEaster.currentPage = page == 0 ? 1 : page;
            objAEaster.sort = sort;
            objAEaster.orderBy = sortdir;
            Session["viewAccessoriesMapping"] = objAEaster;
            objAEaster.listAEM = new BLMisc().GetAMDetails(objAEaster);
            objAEaster.totalRecord = objAEaster.listAEM != null && objAEaster.listAEM.Count > 0 ? objAEaster.listAEM[0].totalRecords : 0;
            return View("_ViewAccessoriesMapping", objAEaster);
        }
        public void DownloadAssociationMapping()
        {
            if (Session["viewAccessoriesMapping"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewAEMaster objAEaster = new ViewAEMaster();
                objAEaster = (ViewAEMaster)Session["viewAccessoriesMapping"];
                objAEaster.currentPage = 0;
                objAEaster.pageSize = 0;
                objAEaster.listAEM = new BLMisc().GetAMDetails(objAEaster);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<AssociateEntityMaster>(objAEaster.listAEM);

               
                dtReport.Columns.Add("Status", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                   
                    dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_enabled"]) == true ? "Active" : "InActive";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("system_id");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("LAYER_ID");
                dtReport.Columns.Remove("ASSOCIATE_LAYER_ID");
                dtReport.Columns.Remove("LSTSELAYERDETAILS");
                dtReport.Columns.Remove("is_enabled");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");
                dtReport.Columns.Remove("PAGEMSG");

                dtReport.Columns["layer_name"].SetOrdinal(0);
                dtReport.Columns["associate_layer_name"].SetOrdinal(1);
                dtReport.Columns["LAYER_SUBTYPE"].SetOrdinal(2);
                dtReport.Columns["Status"].SetOrdinal(3);
                dtReport.Columns["Created On"].SetOrdinal(4);
                dtReport.Columns["created_by_text"].SetOrdinal(5);
                dtReport.Columns["Modified On"].SetOrdinal(6);
                dtReport.Columns["modified_by_text"].SetOrdinal(7);

                dtReport.Columns["layer_name"].ColumnName = "Entity Name";
                dtReport.Columns["associate_layer_name"].ColumnName = "Associate Entity Name";
                dtReport.Columns["LAYER_SUBTYPE"].ColumnName = "Layer Subtype";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";

                var filename = "Associate Entity";
                dtReport.TableName = "Associate Entity";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        public ActionResult AddNewAccessoriesMapping(int id)
        {

            AssociateEntityMaster objAssociateEntityMaster = new AssociateEntityMaster();
            objAssociateEntityMaster.is_enabled = true;
            BindAssociateEntityDropdown(objAssociateEntityMaster);
            return PartialView("_AddAssociationMapping", objAssociateEntityMaster);
        }
        private void BindAssociateEntityDropdown(dynamic obj)
        {
           

            var lstLayer = new BLLayer().GetLayerDetails();
            obj.lstSELayerDetails = lstLayer.Where(m => (m.geom_type == "Line" || m.geom_type == "Point") && m.isvisible == true).OrderBy(m => m.layer_title).ToList();
        }
        public JsonResult SaveAssociateEntity(AssociateEntityMaster objAssociateEntity)
        {
            ModelState.Clear();
            var user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();

            objAssociateEntity.created_by = user_id;
            var status = new BLMisc().SaveAssociateEntity(objAssociateEntity);
            //}
            
             if (status == "Save")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Accessories Entity saved successfully!";
            }
            else if (status == "Failed")
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Entry already Associated !";
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = status;
            }
            objAssociateEntity.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAssociateEntityById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLMisc().DeleteAssociateEntityById(id);
            if (result == "DELETE")
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Associate Entity deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = result;
            }



               return Json(objResp, JsonRequestBehavior.AllowGet);
            }
        #endregion

        #region Layer Icon Mapping
        public ActionResult LayerIconMapping(ViewLayerIcon objLayerIcon, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            objLayerIcon.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objLayerIcon.currentPage = page == 0 ? 1 : page;
            objLayerIcon.sort = sort;
            objLayerIcon.orderBy = sortdir;
            Session["ViewLayerIcon"] = objLayerIcon;
            objLayerIcon.listLayerIcon = new BLMisc().GetLayerIcom(objLayerIcon);
            objLayerIcon.totalRecord = objLayerIcon.listLayerIcon != null && objLayerIcon.listLayerIcon.Count > 0 ? objLayerIcon.listLayerIcon[0].totalRecords : 0;
            return View("_ViewLayerIcon", objLayerIcon);
        }
        public ActionResult UploadNewLayerIcon(int id)
        {
            LayerIconMapping objLayerIcon = new LayerIconMapping();
           // objLayerIcon = id > 0 ? BLISPModelInfo.Instance.GetModleImageById(id) : new ISPModelImageMaster();
            if (id == 0)
                objLayerIcon.status = true;
            var PortList = BLISPModelInfo.Instance.GetModelImage();
            //objLayerIcon.lstTypeModel = PortList.Where(m => m.is_model_image_allowed == true).OrderBy(m => m.key).ToList();
            BindLayerIconDropdown(objLayerIcon);
            return PartialView("AddLayerIcon", objLayerIcon);
        }

        private void BindLayerIconDropdown(dynamic obj)
        {
            var lstLayer = new BLLayer().GetLayerDetails();
            obj.lstlayerDetails = lstLayer.Where(m =>m.isvisible == true && m.is_dynamic_enabled==true ).OrderBy(m => m.layer_title).ToList();
        }

        public ActionResult UploadLayerIcon(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();
            try
            {
                
                var DefaultPortDimension = ApplicationSettings.DefaultPortDimension;
                var layer_name = "";
                var layer_id = 0;
                var network_status = "";
                var network_status_text = "";
                var Category = "";
                var id = "";
                if (collection["layer_id"] != null)
                {
                     layer_id = Int32.Parse((collection["layer_id"]));
                     network_status = collection["network_status"];
                    network_status_text = collection["network_status_text"];
                    Category = collection["Category"];
                    id = collection["id"];
                    var featureName = collection["feature_name"];
                }
                
                var lstLayer = new BLLayer().GetLayerDetails();
                if (layer_id == 0)
                {
                    var lstlayerdetail = lstLayer.Where(m => m.layer_name == EntityType.LandBase.ToString() && m.isvisible == true).OrderBy(m => m.layer_title).ToList();
                    layer_id = lstlayerdetail.Select(m => m.layer_id).FirstOrDefault();
                    layer_name = EntityType.LandBase.ToString();


                }
                else
                {
                    var lstlayerdetails = lstLayer.Where(m => m.layer_id == layer_id && m.isvisible == true).OrderBy(m => m.layer_title).ToList();
                    layer_name = lstlayerdetails.Select(m => m.layer_name).FirstOrDefault();
                }
                var status = false;
                if (collection["status"] == "true")
                    status = true;
                var isvirtual = false;
                if (collection["isvirtual"] == "true")
                    isvirtual = true;
                if (network_status == "")
                {
                    network_status = "O";
                    network_status_text = "Other"; 
                }
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string FileName = file.FileName;
                        string strNewfilename = Path.GetFileNameWithoutExtension(layer_name.ToLower())  + Path.GetExtension(FileName);
                        if(Category!="")
                        {
                            strNewfilename = Category + "_" + strNewfilename;
                        }
                        if (isvirtual == true)
                            strNewfilename = "V_" + strNewfilename;
                        var urlpath = Path.Combine(ApplicationSettings.MapDirPath + "icons/" + network_status_text, strNewfilename);
                        file.SaveAs(urlpath);
                        // get User Detail..
                        User objUser = (User)(Session["userDetail"]);
                        LayerIconMapping objLayerIcon = new LayerIconMapping();
                        //objAttachment.id = Convert.ToInt32(id);
                        objLayerIcon.layer_id = Convert.ToInt32(layer_id);
                        objLayerIcon.icon_name = strNewfilename;
                        objLayerIcon.network_status = network_status;
                        objLayerIcon.status = status;
                        objLayerIcon.category = Category;
                        objLayerIcon.icon_path = "icons/" + network_status_text+"/"+ strNewfilename ;
                        objLayerIcon.created_by = objUser.user_id;
                        objLayerIcon.is_virtual = isvirtual;
                        var savefile = new BLMisc().saveLayerIcon(objLayerIcon);
                    }
                    jResp.message = "Layer Icon has been uploaded successfully!";
                    jResp.status = StatusCodes.OK.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (layer_id != 0 && Convert.ToInt32(id) != 0)
                    {
                        User objUser = (User)(Session["userDetail"]);
                        LayerIconMapping objLayerIcon = new LayerIconMapping();
                        objLayerIcon.id = Convert.ToInt32(id);
                        objLayerIcon.layer_id = Convert.ToInt32(layer_id);
                        //objLayerIcon.image_data = image_data;
                        objLayerIcon.status = status;
                        objLayerIcon.created_by = objUser.user_id;
                       // var savefile = BLISPModelInfo.Instance.SaveModleImage(objAttachment);
                        jResp.message = "Model Image has been updated successfully!";
                        jResp.status = StatusCodes.OK.ToString();
                    }
                    else
                    {
                        jResp.message = "No files selected.";
                        jResp.status = StatusCodes.INVALID_INPUTS.ToString();

                    }
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadModleImage()", "Equipment", ex);
                jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_243;
                jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
                //Error Logging...
            }
        }
        //public dropdown_master GetDropDown_byLayer_id(int layer_id = 0)
        //{
        //    return new BLMisc().GetDropDown_byLayer_id(layer_id);
        //}
        public JsonResult BindDropDown_byLayer_id(int layer_id)
        {

            var objResp = new BLMisc().GetEntityTypeDropdownList(layer_id).ToList();
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        #endregion

        #region Ticket Type Master
        public ActionResult ViewTicketType(ViewTicketType objTicketType, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objTicketType.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objTicketType.currentPage = page == 0 ? 1 : page;
            objTicketType.sort = sort;
            objTicketType.orderBy = sortdir;
            Session["viewTicketType"] = objTicketType;
            objTicketType.listTTM = new BLTicketType().GetTicketTypeDetails(objTicketType);
            objTicketType.totalRecord = objTicketType.listTTM != null && objTicketType.listTTM.Count > 0 ? objTicketType.listTTM[0].totalRecords : 0;
            return View("_ViewTicketType", objTicketType);
        }
        public void DownloadViewTicketType()
        {
            if (Session["viewTicketType"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewTicketType objTicketType = new ViewTicketType();
                objTicketType = (ViewTicketType)Session["viewTicketType"];
                objTicketType.currentPage = 0;
                objTicketType.pageSize = 0;
                objTicketType.listTTM = new BLTicketType().GetTicketTypeDetails(objTicketType) ;

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<TicketTypeMaster>(objTicketType.listTTM);

               
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
             
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("ICON_CONTENT");
                dtReport.Columns.Remove("ICON_CLASS");

                dtReport.Columns["ticket_type"].SetOrdinal(0);
                dtReport.Columns["description"].SetOrdinal(1);
                dtReport.Columns["color_code"].SetOrdinal(2);
                dtReport.Columns["module"].SetOrdinal(3);
                dtReport.Columns["Created On"].SetOrdinal(4);
                dtReport.Columns["created_by_text"].SetOrdinal(5);
                dtReport.Columns["Modified On"].SetOrdinal(6);
                dtReport.Columns["modified_by_text"].SetOrdinal(7);

                dtReport.Columns["ticket_type"].ColumnName = "Ticket Type";
                dtReport.Columns["description"].ColumnName = "Description";
                dtReport.Columns["color_code"].ColumnName = "Color Code";
                dtReport.Columns["module"].ColumnName = "Module";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";

                var filename = "Ticlet Type";
                dtReport.TableName = "Ticlet Typ";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        public ActionResult AddNewTicketType(int id)
        {

            TicketTypeMaster objTicketType = new TicketTypeMaster();

            objTicketType = id > 0 ? new BLTicketType().DATicketType(id) : new TicketTypeMaster();
           // BindTerminationPointDropdown(objTicketType);
            return PartialView("_AddTicketType", objTicketType);
        }

        public JsonResult SaveTicketType(TicketTypeMaster objTicketType)
        {
            ModelState.Clear();
            var user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();

            objTicketType.created_by = user_id;
            var status = new BLTicketType().SaveTicketType(objTicketType);
            //}
            if (status == "Update")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Ticket Type updated successfully!";
            }
            else if (status == "Save")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Ticket Type saved successfully!";
            }
            else if (status == "Failed")
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Entry already exists !";
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = status;
            }
            objTicketType.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTicketTypeById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLTicketType().DeleteTicketTypeById(id);
            if (result == "DELETE")
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Ticket Type deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = result;
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult ViewUserActivityLog(UserActivityLog objUserActivityLogFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            if (sort != "" || page != 0)
            {
                objUserActivityLogFilter.objGridAttributes = (CommonGridAttributes)Session["gridAttr"];
            }

            objUserActivityLogFilter.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objUserActivityLogFilter.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objUserActivityLogFilter.objGridAttributes.sort = sort;
            objUserActivityLogFilter.objGridAttributes.orderBy = sortdir;
            Session["viewUserActivityLog"] = objUserActivityLogFilter;
            objUserActivityLogFilter.listUserActivityLog = new BLUserActivityLog().GetUserActivityLogDetails(objUserActivityLogFilter.objGridAttributes);
            objUserActivityLogFilter.objGridAttributes.totalRecord = objUserActivityLogFilter.listUserActivityLog != null && objUserActivityLogFilter.listUserActivityLog.Count > 0 ? objUserActivityLogFilter.listUserActivityLog[0].totalRecords : 0;
            Session["gridAttr"] = objUserActivityLogFilter.objGridAttributes;
            return View("_ViewUserActivityLog", objUserActivityLogFilter);
        }

        
        public void DownloadUserActivityLogs()
        {
            if (Session["viewUserActivityLog"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                UserActivityLog objUserActivityLog = new UserActivityLog();
                objUserActivityLog = (UserActivityLog)Session["viewUserActivityLog"];
                List<AccessoriesMapping> lstViewUserDetails = new List<AccessoriesMapping>();
                objUserActivityLog.objGridAttributes.currentPage = 0;
                objUserActivityLog.objGridAttributes.pageSize = 0;
                objUserActivityLog.listUserActivityLog = new BLUserActivityLog().GetUserActivityLogDetails(objUserActivityLog.objGridAttributes);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<global::Models.UserActivityLog>(objUserActivityLog.listUserActivityLog);
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("OBJGRIDATTRIBUTES");
                dtReport.Columns.Remove("LISTUSERACTIVITYLOG");
                dtReport.Columns["USER_NAME"].ColumnName = "User Name";
                dtReport.Columns["ACTION_NAME"].ColumnName = "Action Name";
                dtReport.Columns["CLIENT_IP"].ColumnName = "Client IP";
                dtReport.Columns["SERVER_IP"].ColumnName = "Server IP";
                dtReport.Columns["SOURCE"].ColumnName = "Source";
                dtReport.Columns["ACTION_ON"].ColumnName = "Created On";

                var filename = "UserActivityLogList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public ActionResult ViewUserActivityLogSettings(ViewUserActivityLogSettings objViewUserActivityLogSettings, int page = 0, string sort = "", string sortdir = "")
        {
            if (sort != "" || page != 0)
            {
                objViewUserActivityLogSettings.objGridAttributes = (CommonGridAttributes)Session["viewUserActivityLogSettings"];
            }
            objViewUserActivityLogSettings.lstSearchBy = GetUserActivitySearchByColumns();
            objViewUserActivityLogSettings.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewUserActivityLogSettings.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewUserActivityLogSettings.objGridAttributes.sort = sort;
            objViewUserActivityLogSettings.objGridAttributes.orderBy = sortdir;
            objViewUserActivityLogSettings.lstlogs = new BLUserActivityLog().GetUserActivityLogsSettings(objViewUserActivityLogSettings.objGridAttributes);
            objViewUserActivityLogSettings.objGridAttributes.totalRecord = objViewUserActivityLogSettings.lstlogs != null && objViewUserActivityLogSettings.lstlogs.Count > 0 ? objViewUserActivityLogSettings.lstlogs[0].totalRecords : 0;
            Session["viewUserActivityLogSettings"] = objViewUserActivityLogSettings.objGridAttributes;
            return View("_UserActivityLogSettings", objViewUserActivityLogSettings);
        }

        public List<KeyValueDropDown> GetUserActivitySearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Controller Name", value = "controller_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Action Name", value = "action_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Description", value = "description" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Source", value = "source" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Project Name", value = "project_name" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }
        public ActionResult EditLogStatus(int id, bool status)
        {
            var Result = false;
            PageMessage objPM = new PageMessage();
            Result = new BLUserActivityLog().EditLogStatus(id, status);
            if (Result)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "Log Status Updated Successfully.";
                ViewBag.message = objPM.message;
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = "Failed to Update Log Status";
                ViewBag.message = objPM.message;
            }
            return Json(objPM, JsonRequestBehavior.AllowGet);
        }
        public void DownloadUserActivityLogsSettings()
        {
            if (Session["viewUserActivityLogSettings"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                ViewUserActivityLogSettings objUserActivityLogFilter = new ViewUserActivityLogSettings();
                CommonGridAttributes objViewFilter = (CommonGridAttributes)Session["viewUserActivityLogSettings"];
                objUserActivityLogFilter.lstlogs = new BLUserActivityLog().GetUserActivityLogsSettings(objViewFilter);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<global::Models.UserActivityLogSettingsPage>(objUserActivityLogFilter.lstlogs);
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns["controller_name"].ColumnName = "controller Name";
                dtReport.Columns["ACTION_NAME"].ColumnName = "Action Name";
                dtReport.Columns["description"].ColumnName = "Description";
                dtReport.Columns["project_name"].ColumnName = "Project Name";
                dtReport.Columns["SOURCE"].ColumnName = "Source";
                dtReport.Columns["created_on"].ColumnName = "Created On";

                var filename = "UserActivityLogSettingsList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        public ActionResult ViewAutoCodificationLog(AutoCodificationLog objAutoCodificationLogFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            if (sort != "" || page != 0)
            {
                objAutoCodificationLogFilter.objGridAttributes = (CommonGridAttributes)Session["gridAttr"];
            }

            objAutoCodificationLogFilter.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objAutoCodificationLogFilter.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objAutoCodificationLogFilter.objGridAttributes.sort = sort;
            objAutoCodificationLogFilter.objGridAttributes.orderBy = sortdir;
            Session["ViewAutoCodificationLog"] = objAutoCodificationLogFilter;
            objAutoCodificationLogFilter.listCodificationLog = new BLAutoCodificationLog().GetAutoCodificationLogDetails(objAutoCodificationLogFilter.objGridAttributes);
            objAutoCodificationLogFilter.objGridAttributes.totalRecord = objAutoCodificationLogFilter.listCodificationLog != null && objAutoCodificationLogFilter.listCodificationLog.Count > 0 ? objAutoCodificationLogFilter.listCodificationLog[0].totalRecords : 0;
            Session["gridAttr"] = objAutoCodificationLogFilter.objGridAttributes;
            return View("_ViewAutoCodificationLog", objAutoCodificationLogFilter);
        }
        public void DownloadAutoCodificationLogs()
        {
            if (Session["ViewAutoCodificationLog"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                AutoCodificationLog objAutoCodificationLog = new AutoCodificationLog();
                objAutoCodificationLog = (AutoCodificationLog)Session["ViewAutoCodificationLog"];
                //List<AccessoriesMapping> lstViewUserDetails = new List<AccessoriesMapping>();
                objAutoCodificationLog.objGridAttributes.currentPage = 0;
                objAutoCodificationLog.objGridAttributes.pageSize = 0;
                objAutoCodificationLog.listCodificationLog = new BLAutoCodificationLog().GetAutoCodificationLogDetails(objAutoCodificationLog.objGridAttributes);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<global::Models.AutoCodificationLog>(objAutoCodificationLog.listCodificationLog);
                //dtReport.Columns.Remove("id");
                //dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("OBJGRIDATTRIBUTES");
                dtReport.Columns.Remove("listCodificationLog");
                dtReport.Columns["SYSTEM_ID"].ColumnName = "System ID";
                dtReport.Columns["ENTITY_TYPE"].ColumnName = "Entity Type";
                dtReport.Columns["ACTION_BY"].ColumnName = "Action By";
                dtReport.Columns["ACTION_TYPE"].ColumnName = "Action Type";
                dtReport.Columns["ACTION_ON"].ColumnName = "Action On";
                //dtReport.Columns["IS_VALID"].ColumnName = "Is Valid";

                var filename = "AutoCodificationLogList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public ActionResult ViewEntityDeleteLog(EntityDeleteLog objEntityDeleteLogFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            if (sort != "" || page != 0)
            {
                objEntityDeleteLogFilter.objGridAttributes = (CommonGridAttributes)Session["gridAttr"];
            }

            objEntityDeleteLogFilter.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objEntityDeleteLogFilter.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objEntityDeleteLogFilter.objGridAttributes.sort = sort;
            objEntityDeleteLogFilter.objGridAttributes.orderBy = sortdir;
            Session["ViewEntityDeleteLog"] = objEntityDeleteLogFilter;
            objEntityDeleteLogFilter.listEntityDeleteLog = new BLEntityDeleteLog().GetEntityDeleteLogDetails(objEntityDeleteLogFilter.objGridAttributes);
            objEntityDeleteLogFilter.objGridAttributes.totalRecord = objEntityDeleteLogFilter.listEntityDeleteLog != null && objEntityDeleteLogFilter.listEntityDeleteLog.Count > 0 ? objEntityDeleteLogFilter.listEntityDeleteLog[0].totalRecords : 0;
            Session["gridAttr"] = objEntityDeleteLogFilter.objGridAttributes;
            return View("_ViewEntityDeleteLog", objEntityDeleteLogFilter);
        }
        public void DownloadEntityDeleteLogs()
        {
            if (Session["ViewEntityDeleteLog"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                EntityDeleteLog objEntityDeleteLog = new EntityDeleteLog();
                objEntityDeleteLog = (EntityDeleteLog)Session["ViewEntityDeleteLog"];
                //List<AccessoriesMapping> lstViewUserDetails = new List<AccessoriesMapping>();
                objEntityDeleteLog.objGridAttributes.currentPage = 0;
                objEntityDeleteLog.objGridAttributes.pageSize = 0;
                objEntityDeleteLog.listEntityDeleteLog = new BLEntityDeleteLog().GetEntityDeleteLogDetails(objEntityDeleteLog.objGridAttributes);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<global::Models.EntityDeleteLog>(objEntityDeleteLog.listEntityDeleteLog);
                //dtReport.Columns.Remove("id");
                //dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("OBJGRIDATTRIBUTES");
                dtReport.Columns.Remove("listEntityDeleteLog");
                dtReport.Columns["SYSTEM_ID"].ColumnName = "System ID";
                dtReport.Columns["ENTITY_TYPE"].ColumnName = "Entity Type";
                dtReport.Columns["ACTION_BY"].ColumnName = "Action By";
                dtReport.Columns["ACTION_DATE"].ColumnName = "Action Date";
                //dtReport.Columns["IS_VALID"].ColumnName = "Is Valid";

                var filename = "EntityDeleteLogList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
    }
}



