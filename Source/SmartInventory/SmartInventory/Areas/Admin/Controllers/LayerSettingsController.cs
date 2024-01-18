
using Models.Admin;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics.Admin;
using SmartInventory.Settings;
using Utility;
using BusinessLogics;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using System.Web.Helpers;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Models;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class LayerSettingsController : Controller
    {
        public ActionResult AddLayerGroups(int group_id = 0)
        {
            var validateGrpStatus = new BLLayerGroup().ValidateLayerGroupById(group_id);
            LayerGroupMaster objLayerGroup = new LayerGroupMaster();
            objLayerGroup = group_id != 0 ? new BLLayerGroup().GetLayerGroupDetailsByID(group_id) : new LayerGroupMaster();
            if (group_id == null || group_id == 0)
            {
                objLayerGroup.status = true;
            }
            objLayerGroup.chk_GrpStatus = validateGrpStatus;
            return View("AddGroups", objLayerGroup);
        }
        [HttpPost]
        public JsonResult SaveLayerGroupDetails(LayerGroupMaster objLyrGroup)
        {
            ModelState.Clear();
            objLyrGroup.user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();
            if (objLyrGroup.group_name != null && objLyrGroup.group_description != null)
            {
                var status = new BLLayerGroup().SaveLayerGroupDetails(objLyrGroup);
                if (status == "Save")
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Layer Group Saved successfully!";
                }
                else if (status == "Update")
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Layer Group Updated successfully!";
                }
                else if (status == "Failed")
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "Unable to update group as it is already been mapped!";
                }
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Mandatory fields required";
            }
            objLyrGroup.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewGroups(ViewLayerGroup objViewGroup, int page = 0, string sort = "", string sortdir = "")
        {
            BindSearchBy(objViewGroup);
            if (sort != "" || page != 0)
            {
                objViewGroup.objGridAttributes = (CommonGridAttributes)Session["viewLayerGroups"];
            }
            objViewGroup.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewGroup.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewGroup.objGridAttributes.sort = sort;
            objViewGroup.objGridAttributes.orderBy = sortdir;
            objViewGroup.lstLayerGroups = new BLLayerGroup().GetGroupList(objViewGroup.objGridAttributes);
            objViewGroup.objGridAttributes.totalRecord = objViewGroup.lstLayerGroups != null && objViewGroup.lstLayerGroups.Count > 0 ? objViewGroup.lstLayerGroups[0].totalRecords : 0;
            Session["viewLayerGroups"] = objViewGroup.objGridAttributes;
            return View("ViewGroups", objViewGroup);
        }
        public IList<KeyValueDropDown> BindSearchBy(ViewLayerGroup vwlyrgrp)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Group Name", value = "group_name" });
            items.Add(new KeyValueDropDown { key = "Group Description", value = "group_description" });
            return vwlyrgrp.lstBindSearchBy = items.OrderBy(m => m.key).ToList();
        }

        [HttpPost]
        public JsonResult DeleteLayerGroupsById(int group_id)
        {

            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var validateGroup = new BLLayerGroup().ValidateLayerGroupById(group_id);
                if (validateGroup == 1)
                {
                    var output = new BLLayerGroup().DeleteLayerGrouprById(group_id);
                    if (output > 0)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        objResp.message = "Layer Group Deleted successfully!";
                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = "Something went wrong while deleting Group!";
                    }
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Unable to delete group as it is already been mapped!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "User not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void DownloadViewGroupsDetail()
        {
            if (Session["viewLayerGroups"] != null)
            {
                CommonGridAttributes objGridAttributes = (CommonGridAttributes)Session["viewLayerGroups"];
                List<LayerGroupMaster> lstViewGroupDetails = new List<LayerGroupMaster>();
                objGridAttributes.currentPage = 0;
                objGridAttributes.pageSize = 0;
                lstViewGroupDetails = new BLLayerGroup().GetGroupList(objGridAttributes);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<LayerGroupMaster>(lstViewGroupDetails);
                dtReport.TableName = "View_Layer_Group_Details";
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                dtReport.Columns.Add("Groups Status", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                    dtReport.Rows[i]["Groups Status"] = Convert.ToBoolean(dtReport.Rows[i]["status"]) == true ? "Active" : "InActive";
                }
                dtReport.Columns.Remove("GROUP_ID");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_ON");
                dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("CHK_GRPSTATUS");
                dtReport.Columns.Remove("STATUS");
                dtReport.Columns["group_name"].SetOrdinal(0);
                dtReport.Columns["group_description"].SetOrdinal(1);
                dtReport.Columns["Groups Status"].SetOrdinal(2);
                dtReport.Columns["created_by_text"].SetOrdinal(3);
                dtReport.Columns["created on"].SetOrdinal(4);
                dtReport.Columns["modified_by_text"].SetOrdinal(5);
                dtReport.Columns["modified on"].SetOrdinal(6);

                dtReport.Columns["group_name"].ColumnName = "Group Name";
                dtReport.Columns["group_description"].ColumnName = "Group Description";
                dtReport.Columns["Groups Status"].ColumnName = "Group Status";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["created on"].ColumnName = "Created On";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                dtReport.Columns["modified on"].ColumnName = "Modified On";
                var filename = "ViewLayerGroupDetails";
                ExportDataTableToExcel(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        [HttpPost]
        public JsonResult CheckLayerSequence( int new_sequence,int layer_id)
        {

            var result = new BLLayerStyleMaster().CheckUpdateLayerSequence( new_sequence, layer_id, false);

            return Json(result[0], JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewLayerGroupsMapping(ViewLayerGroupMapping objLyrGrpMapping, int page = 0, string sort = "", string sortdir = "")
        {
            BindSearchByLyrGrpMapping(objLyrGrpMapping);

            objLyrGrpMapping.lstLayerGroups = new BLLayerGroup().GetLayerGrpDetails();
            objLyrGrpMapping.lstLayerGroupsMapping = new BLLayerGroup().GetLyrGrpMappingList();

            objLyrGrpMapping.objGridAttributes.totalRecord = objLyrGrpMapping.lstLayerGroupsMapping != null && objLyrGrpMapping.lstLayerGroupsMapping.Count > 0 ? objLyrGrpMapping.lstLayerGroupsMapping[0].totalRecords : 0;
            foreach (var item in objLyrGrpMapping.lstLayerGroupsMapping)
            {
                item.modified_on_formated = MiscHelper.FormatDateTime(item.modified_on.ToString());

            }
            Session["viewLayerGroupsMapping"] = objLyrGrpMapping.objGridAttributes;

            return View("_LayerGroupMappingColumns", objLyrGrpMapping);
        }

        public ActionResult LayerConfigurationSettings(int id = 0)
        {
            layerDetail objlayerdetail = new layerDetail();
            if (id != 0)
            {
                objlayerdetail = new BLLayerConfiguration().GetLayerConfigurationById(id);
            }
            objlayerdetail.lstzoom = new BLMapScales().GetZoomLevel();
            objlayerdetail.lstlayerdetails = new BLLayerConfiguration().GetLayerConfiguration();
            return View("LayerConfigurationSetting", objlayerdetail);
        }
        public ActionResult saveLayerConfigurationSettings(layerDetail objlayerdetails)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            //if (objlayerdetails.minzoomlevel < objlayerdetails.maxzoomlevel)
            //{
                if (objlayerdetails != null)
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Layer Configuration Updated Successfully";
                
                objlayerdetails = new BLLayerConfiguration().SaveLayerConfigurationById(objlayerdetails);
                }
            else
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
            }

            objlayerdetails.pageMsg = objMsg;
            objlayerdetails.lstzoom = new BLMapScales().GetZoomLevel();
            objlayerdetails.lstlayerdetails = new BLLayerConfiguration().GetLayerConfiguration();
            return Json(objlayerdetails, JsonRequestBehavior.AllowGet);
        }
        public IList<KeyValueDropDown> BindSearchByLyrGrpMapping(ViewLayerGroupMapping vwlyrgrpMpng)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Layer Name", value = "layer_name" });
            items.Add(new KeyValueDropDown { key = "Group Name", value = "group_name" });
            items.Add(new KeyValueDropDown { key = "Group Description", value = "group_description" });
            return vwlyrgrpMpng.lstBindSearchBy = items.OrderBy(m => m.key).ToList();
        }
        [HttpGet]
        public void DownloadViewGroupsMappingDetail()
        {
            if (Session["viewLayerGroupsMapping"] != null)
            {
                CommonGridAttributes objGridAttributes = (CommonGridAttributes)Session["viewLayerGroupsMapping"];
                List<LayerGroupMapping> lstViewGroupMpngDetails = new List<LayerGroupMapping>();
                objGridAttributes.currentPage = 0;
                objGridAttributes.pageSize = 0;
                lstViewGroupMpngDetails = new BLLayerGroup().GetLyrGrpMappingList();

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<LayerGroupMapping>(lstViewGroupMpngDetails);
                dtReport.TableName = "View_Layer_Group_Mapping_Details";
                // dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("GROUP_ID");
                dtReport.Columns.Remove("LAYER_ID");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("layer_name");
                dtReport.Columns.Remove("MAPPING_ID");
                dtReport.Columns.Remove("LAYER_SEQ");
                //dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("CREATED_BY_TEXT");
                dtReport.Columns.Remove("MODIFIED_ON");
                dtReport.Columns.Remove("MODIFIED_ON_FORMATED");
                dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("LSTGROUPDETAILS");
                dtReport.Columns.Remove("LSTLAYERGRPMAPPING");

                dtReport.Columns["LAYER_TITLE"].SetOrdinal(0);
                dtReport.Columns["group_name"].SetOrdinal(1);
                dtReport.Columns["group_description"].SetOrdinal(2);

                dtReport.Columns["modified_by_text"].SetOrdinal(3);
                dtReport.Columns["modified on"].SetOrdinal(4);

                dtReport.Columns["LAYER_TITLE"].ColumnName = "Layer Title";
                dtReport.Columns["group_name"].ColumnName = "Group Name";
                dtReport.Columns["group_description"].ColumnName = "Group Description";

                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                dtReport.Columns["modified on"].ColumnName = "Modified On";
                var filename = "ViewLayerGroupMappingDetails";
                ExportDataTableToExcel(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public ActionResult AddNewLayerGroupMapping(int mappingId)
        {
            LayerGroupMapping objLyrGrpMpng = new LayerGroupMapping();
            objLyrGrpMpng = mappingId > 0 ? new BLLayerGroup().GetGroupMappingById(mappingId) : new LayerGroupMapping();
            BindLayerGroupMappingDropdown(objLyrGrpMpng);
            return PartialView("AddLayerGroupMapping", objLyrGrpMpng);
        }
        private void BindLayerGroupMappingDropdown(dynamic obj)
        {
            var lstLayer = new BLLayer().GetLayerDetails();
            obj.lstLayerGrpMapping = lstLayer.Where(m => m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            var lstLyrGrpDetails = new BLLayerGroup().GetLayerGrpDetails();
            obj.lstGroupDetails = lstLyrGrpDetails.Where(m => m.status == true).OrderBy(m => m.group_name).ToList();
        }

        [HttpPost]
        public ActionResult SaveLayerGroupMapping(ViewLayerGroupMapping objLyrGroupMpng)
        {


            List<LayerGroupMapping> layers = objLyrGroupMpng.lstLayerGroupsMapping;
            ModelState.Clear();
            var user_id = Convert.ToInt32(Session["user_id"]);

            foreach (var item in layers)
            {
                item.user_id = Convert.ToInt32(Session["user_id"]);
                item.modified_by = Convert.ToInt32(Session["user_id"]);
                item.modified_on = DateTime.Now;
            }

            var layers1 = layers.Select(m => new { m.mapping_id, m.layer_id, m.group_id, m.layer_seq, m.modified_by, m.modified_on, m.user_id }).ToList();

            PageMessage objMsg = new PageMessage();
            var response = new BLLayerGroup().SaveLayerGroupMappingDetails(JsonConvert.SerializeObject(layers1));
            if (response.status == true)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = response.message;
            }

            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Failed to update Layer Group Mapping !";
            }

            objLyrGroupMpng.lstLayerGroups = new BLLayerGroup().GetLayerGrpDetails();
            objLyrGroupMpng.lstLayerGroupsMapping = new BLLayerGroup().GetLyrGrpMappingList();
            foreach (var item in objLyrGroupMpng.lstLayerGroupsMapping)
            {
                item.modified_on_formated = MiscHelper.FormatDateTime(item.modified_on.ToString());
            }
            objLyrGroupMpng.objGridAttributes.totalRecord = objLyrGroupMpng.lstLayerGroupsMapping != null && objLyrGroupMpng.lstLayerGroupsMapping.Count > 0 ? objLyrGroupMpng.lstLayerGroupsMapping[0].totalRecords : 0;
            objLyrGroupMpng.pageMsg = objMsg;

            return View("_LayerGroupMappingColumns", objLyrGroupMpng);

        }


        [HttpPost]
        public JsonResult SaveLayerGroupMappingByGroupId(LayerGroupMapping objLyrGroupMpng)
        {
            ModelState.Clear();
            objLyrGroupMpng.user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();
            var isNew = objLyrGroupMpng.mapping_id > 0 ? false : true;
            var status = new BLLayerGroup().SaveLayerGroupMappingDetailById(objLyrGroupMpng);
            if (status == "Update")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Layer Group Mapping Updated Successfully!";
            }
            else if (status == "Save")
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Layer Group Mapping Saved Successfully!";
            }
            else if (status == "Failed")
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Layer Group Mapping already exists !";
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = status;
            }
            objLyrGroupMpng.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public IList<KeyValueDropDown> BindLayerSettingSearchBy(TemplateForDropDownLayer objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Layer Name", value = "layer_name" });
            items.Add(new KeyValueDropDown { key = "Layer Title", value = "layer_title" });
            items.Add(new KeyValueDropDown { key = "Layer Table", value = "layer_table" });
            return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();
        }
        [HttpPost]
        public ActionResult ReadMoreLayer(int layer_id)
        {
            List<LayerDetailsColumn> lstLayerDetailsColumn = new List<LayerDetailsColumn>();
            lstLayerDetailsColumn = new BLLayerMaster().GetLayerDetailById(layer_id);

            return PartialView("_ReadMore", lstLayerDetailsColumn);

        }
        public ActionResult AddLayerDetailSettings(int layer_id)
        {
            List<LayerDetailsColumn> lstLayerDetailsColumn = new List<LayerDetailsColumn>();
            lstLayerDetailsColumn = new BLLayerMaster().GetLayerDetailById(layer_id);

            return View("_EditLayerDetail", lstLayerDetailsColumn);
        }
        [HttpPost]
        public ActionResult SaveLayerDetail(List<LayerDetailsColumn> lstLayerDetailsColumn)
        {

            //  var objLayerDetail = new List<layerDetail>();
            ModelState.Clear();
            lstLayerDetailsColumn = lstLayerDetailsColumn.Where(x => x.is_edit_allowed == true).ToList();
            foreach (var item in lstLayerDetailsColumn)
            {
                item.pageMsg = null;
                item.viewLayerDetails = null;
                if (item.data_type.ToUpper() == "CHARACTER")
                {
                    item.column_value = "'" + item.column_value + "'";
                }
            }
            var LayerDetailsColumn = new BLLayerMaster().SaveLayerSettings(JsonConvert.SerializeObject(lstLayerDetailsColumn), lstLayerDetailsColumn[0].layer_id);
            List<layerDetail> objLayerDetail = new List<layerDetail>();
            lstLayerDetailsColumn = new BLLayerMaster().GetLayerDetailById(lstLayerDetailsColumn[0].layer_id);

            ApplicationSettings.InitializeGlobalSettings();

            lstLayerDetailsColumn[0].pageMsg.status = ResponseStatus.OK.ToString();
            lstLayerDetailsColumn[0].pageMsg.message = LayerDetailsColumn.message;
            return View("_EditLayerDetail", lstLayerDetailsColumn);
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

        #region LayerDetailsDownload
        [HttpGet]
        public void DownloadLayerDetail()
        {
            if (Session["viewLayerDetailFilter"] != null)
            {
                ViewLayerSettingDetailList model = new ViewLayerSettingDetailList();
                model.viewLayerDetails = (ViewLayerDetails)Session["viewLayerDetailFilter"];
                model.viewLayerDetails.pageSize = 0;
                model.viewLayerDetails.currentPage = 0;
                List<Dictionary<string, string>> lstlayerDetails = new BLLayerMaster().GetLayerSettingDetails(model);
                model.viewLayerDetails.totalRecord = model.ViewLayerDetailList != null && model.ViewLayerDetailList.Count > 0 ? model.ViewLayerDetailList[0].totalRecords : 0;

                DataTable dtReport = new DataTable();

                //----------------------
                foreach (string column in lstlayerDetails[0].Keys)
                {
                    dtReport.Columns.Add(column);
                }
                foreach (Dictionary<string, string> dictionary in lstlayerDetails)
                {
                    DataRow dataRow = dtReport.NewRow();

                    foreach (string column in dictionary.Keys)
                    {
                        dataRow[column] = dictionary[column];
                    }

                    dtReport.Rows.Add(dataRow);
                }

                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("S_NO");
                dtReport.Columns.Remove("LAYER ID");

                var filename = "LayerDeatils";
                ExportDataTableToExcel(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        #endregion

        #region dynamic grid
        public ActionResult ShowlayerDetails(ViewLayerSettingDetailList model, int page = 0, string sort = "", string sortdir = "")
        {
            var objUser = (User)Session["userDetail"];
            model.viewLayerDetails.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            model.viewLayerDetails.currentPage = page == 0 ? 1 : page;
            model.viewLayerDetails.sort = sort;
            model.viewLayerDetails.orderBy = sortdir;
            var jsonSerialiser = new JavaScriptSerializer();
            var lstFiberLinkStatus = jsonSerialiser.Serialize(model.viewLayerDetails);


            List<Dictionary<string, string>> lstlayerDetails = new BLLayerMaster().GetLayerSettingDetails(model);

            string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
            foreach (Dictionary<string, string> dic in lstlayerDetails)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                model.lstlayerDetails.Add(obj);
            }

            BindLayerSettingSearchBy(model);
            model.viewLayerDetails.totalRecord = model.lstlayerDetails.Count > 0 ? Convert.ToInt32(lstlayerDetails[0].FirstOrDefault().Value) : 0;
            Session["viewLayerDetailFilter"] = model.viewLayerDetails;
            return View("_ViewLayerDetails", model);
        }
        #endregion

        #region Layer Style Master
        public IList<KeyValueDropDown> BindSearchBy(LayerStyleMaster vwlyrgrp)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Layer Name", value = "layer_name" });
            items.Add(new KeyValueDropDown { key = "Entity Category", value = "entity_category" });
            return vwlyrgrp.lstBindSearchBy = items.OrderBy(m => m.key).ToList();
        }
        
        public ActionResult ViewLayerStyleMaster(LayerStyleMaster objLayerStyleMaster, int page = 0, string sort = "", string sortdir = "", string Message = "")
        {
            BindSearchBy(objLayerStyleMaster);            
            if (sort != "" || page != 0)
            objLayerStyleMaster.objGridAttributes = (CommonGridAttributes)Session["ViewLayerStyleGroup"];            
            if ((string)Session["GridloadWithoutRefresh"] != null)
            {
               objLayerStyleMaster.objGridAttributes = (CommonGridAttributes)Session["ViewLayerStyleGroup"];
               page = objLayerStyleMaster.objGridAttributes.currentPage;
               Session.Remove("GridloadWithoutRefresh");
            }
            objLayerStyleMaster.objGridAttributes.pageSize = ApplicationSettings.ViewAdminLayerStyleGridPageSize;
            objLayerStyleMaster.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objLayerStyleMaster.objGridAttributes.sort = sort;
            objLayerStyleMaster.objGridAttributes.orderBy = sortdir;
            objLayerStyleMaster.lstLayerGroups = new BLLayerStyleMaster().GetLayerStyleMaster(objLayerStyleMaster.objGridAttributes);
            objLayerStyleMaster.objGridAttributes.totalRecord = objLayerStyleMaster.lstLayerGroups != null && objLayerStyleMaster.lstLayerGroups.Count > 0 ? objLayerStyleMaster.lstLayerGroups[0].totalRecords : 0;
            Session["ViewLayerStyleGroup"] = objLayerStyleMaster.objGridAttributes;
            return View("ViewLayerStyleMaster", objLayerStyleMaster);
        }

        public ActionResult AddLayerStyleMaster(int layer_id = 0, int id = 0, string layer_name =null)
        {
            List<LayerStyleMaster> objLayerStyleMaster = new List<LayerStyleMaster>();
            objLayerStyleMaster = new BLLayerStyleMaster().GetLayerStyleMasterDetailsByID(id);
            objLayerStyleMaster[0].max_layer_sequence = ApplicationSettings.listLayerDetails.Where(m=>m.isvisible=true).Count();
            objLayerStyleMaster[0].calegorylistbylayername = BLLabelSetting.Instance.GetCategorylistbylayername(layer_name);
            objLayerStyleMaster[0].LabelExpression = BLLabelSetting.Instance.GetLayerStyleColumn(layer_id);
            if(id > 0)
            {
                List<object> mixedList = JsonConvert.DeserializeObject<List<object>>(objLayerStyleMaster[0].label_expression);
                int i = 0;
                foreach (object item in mixedList)
                {
                    JObject jsonObject = JObject.Parse(item.ToString());
                    string type = jsonObject["type"].ToString();
                    if(i == 0)
                    {
                        objLayerStyleMaster[0].LableExp1 = jsonObject["Value"].ToString();
                    }
                    else if (i == 1)
                    {
                        objLayerStyleMaster[0].lstlblAttributes = jsonObject["Value"].ToString();
                    }
                    else if (i == 2)
                    {
                        objLayerStyleMaster[0].LableExp2 = jsonObject["Value"].ToString();
                    }
                    else if (i == 3)
                    {
                        objLayerStyleMaster[0].SeclblAttributes = jsonObject["Value"].ToString();
                    }
                    i++;
                }
            }
            
            return View("AddLayerStyleMaster", objLayerStyleMaster[0]);
        }
        [HttpPost]
        public JsonResult SaveLayerStyleMasterDetails(LayerStyleMaster objLyrStyMaster)
        {
            ModelState.Clear();
            var userdetails = (User)Session["userDetail"];
            objLyrStyMaster.user_id = userdetails.user_id;
            bool status = true;
            PageMessage objMsgst = new PageMessage();
            if (objLyrStyMaster.lstlblAttributes != null && objLyrStyMaster.label_color_hex != null)
            {
               List<object> mixedList = new List<object>
               {
                   new LableExp1 { type = "UD", Value = objLyrStyMaster.LableExp1 },
                   new LableExp2 { type = "Col", Value = objLyrStyMaster.lstlblAttributes },
                   new LableExp3 { type = "UD", Value = objLyrStyMaster.LableExp2 },
                   new LableExp4 { type = "Col", Value = objLyrStyMaster.SeclblAttributes },
               };
                objLyrStyMaster.label_expression = JsonConvert.SerializeObject(mixedList, Formatting.Indented);
                status = new BLLayerStyleMaster().SaveLayerStyleMasterDetails(objLyrStyMaster);
                if (status == true)
                {
                    objMsgst.status = ResponseStatus.OK.ToString();
                    objMsgst.message = "Layer Style Updated successfully!";
                }
                else
                {
                    objMsgst.status = ResponseStatus.FAILED.ToString();
                    objMsgst.message = "Unable to update Style as it is already been mapped!";
                }
            }
            else
            {
                objMsgst.status = ResponseStatus.FAILED.ToString();
                objMsgst.message = "Mandatory fields required";
            }
            objLyrStyMaster.pageMsg = objMsgst;
            return Json(objMsgst, JsonRequestBehavior.AllowGet);
        }


        #endregion

    }
}