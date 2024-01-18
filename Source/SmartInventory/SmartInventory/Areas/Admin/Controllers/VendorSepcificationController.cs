using Models;
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
using System.Configuration;
using System.Net;
using Ionic.Zip;


namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class VendorSepcificationController : Controller
    {
        public ActionResult AddVendorSpecification(int id = 0, int layerId = 0, int accessoriesId = 0)
        {
            VendorSpecificationMaster objVendorSpecification = id != 0 ? new BLVendorSpecification().GetVendorSpeicificationDetailsByID(id) : new VendorSpecificationMaster();

            //BIND VENDOR SEPCIFICATION DROPDOWNS...
            BindVendorSpecDropDowns(objVendorSpecification, layerId, accessoriesId);
            return View("VendorSpecification", objVendorSpecification);
        }

        public void BindVendorSpecDropDowns(VendorSpecificationMaster objVendorSpecification, int layerId = 0, int accessories_id = 0)
        {
            BLVendorSpecification blVendorSpec = new BLVendorSpecification();
            //var entityType = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_id == objVendorSpecification.layer_id).FirstOrDefault().layer_name : null;
            layerId = objVendorSpecification.id > 0 ? objVendorSpecification.layer_id : layerId;
            accessories_id = objVendorSpecification.accessories_id > 0 ? objVendorSpecification.accessories_id : accessories_id;
            objVendorSpecification.lstAllVendor = blVendorSpec.GetAllVendorList();
            objVendorSpecification.listItemType = blVendorSpec.GetAllItemTypeList();
            objVendorSpecification.listUnitMeasurement = blVendorSpec.GetAllUnitMeasurement();
            objVendorSpecification.lstLayerDetails = new BLLayer().GetVendorSpecLayers();
            objVendorSpecification.categories = blVendorSpec.GetVendorCategory();
            objVendorSpecification.listItemMasterCode = blVendorSpec.GetItemMasterCode(layerId);
            objVendorSpecification.formInputSettings = new BLFormInputSettings().getformInputSettings();
            if (layerId > 0)
            {

                //set input_unit_type
                if (objVendorSpecification.categories.Where(m => m.layer_id == layerId).Count() > 0)
                {
                    objVendorSpecification.unit_input_type = objVendorSpecification.categories.Where(m => m.layer_id == layerId).FirstOrDefault().unit_input_type;
                    objVendorSpecification.layer_id = layerId;
                }
                objVendorSpecification.layer_name = ApplicationSettings.listLayerDetails.Where(x => x.layer_id == layerId).FirstOrDefault().layer_name;
                objVendorSpecification.is_specification_allowed = new BLLayer().GetSpecificationAllowed(objVendorSpecification.layer_name).ToString();
                if (objVendorSpecification.is_specification_allowed == "True")
                {
                    objVendorSpecification.lstSpecifyType = blVendorSpec.GetSpecifyTypeList(layerId, objVendorSpecification.layer_name + "_types");
                }
                var objDDLMicroduct = new BLMisc().GetDropDownList(EntityType.Microduct.ToString());
                var objDDLSplitter = new BLMisc().GetDropDownList(EntityType.Splitter.ToString());
                //var objDDLCable = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
                objVendorSpecification.lstNoOfWays = objDDLMicroduct.Where(x => x.dropdown_type == DropDownType.Number_of_Ways.ToString()).ToList();
                objVendorSpecification.lstSplitterType = objDDLSplitter.Where(x => x.dropdown_type == DropDownType.Splitter_Type.ToString()).ToList();
                // objVendorSpecification.lstCableCategory = objDDLCable.Where(x => x.dropdown_type == DropDownType.Cable_Category.ToString()).ToList();
            }
            if (!string.IsNullOrEmpty(objVendorSpecification.unit_input_type) && objVendorSpecification.layer_id > 0 && objVendorSpecification.unit_input_type.ToLower() == "iopddl")
            {
                var ddType = "";
                if (objVendorSpecification.layer_name == EntityType.Splitter.ToString())//splitter
                    ddType = "Splitter_Ratio";
                else if (objVendorSpecification.layer_name == EntityType.ONT.ToString())//ONT
                    ddType = "Ont_Port_Ratio";
                else if (objVendorSpecification.layer_name == EntityType.Cable.ToString())//CABLE
                    ddType = "Fiber_Count";

                // get entity type by layer_id...
                var entityType = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_id == objVendorSpecification.layer_id).FirstOrDefault().layer_name : null;
                objVendorSpecification.lstIOPDetails = new BLVendorSpecification().BindIOPDetails(entityType, ddType).ToList();

            }
            if (accessories_id > 0)
            {
                objVendorSpecification.accessories_id = accessories_id;
            }
        }



        public JsonResult BindSplitterPortRatio(VendorSpecificationMaster objVendorSpecification, string splitterType)
        {
            objVendorSpecification.lstIOPDetails = new BLMisc().GetPortRatio(splitterType);
            return Json(objVendorSpecification.lstIOPDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveVendorSpecification(VendorSpecificationMaster objSaveVendorSpecification)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            objSaveVendorSpecification.user_id = Convert.ToInt32(Session["user_id"]);

            var isNew = objSaveVendorSpecification.id > 0 ? false : true;

            int IsVendorSpecificationDetailsExists = new BLVendorSpecification().ChkVendorSpecifiationDetailExist(objSaveVendorSpecification);

            if (IsVendorSpecificationDetailsExists > 0)
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Item Code already exists for selected entity and vendor !";

                objSaveVendorSpecification.pageMsg = objMsg;
                BindVendorSpecDropDowns(objSaveVendorSpecification);
                return View("VendorSpecification", objSaveVendorSpecification);
            }

            if (!string.IsNullOrEmpty(objSaveVendorSpecification.iop_value))
            {
                if (objSaveVendorSpecification.layer_id == 23 || objSaveVendorSpecification.layer_id == 9)//SPLITTER && ONT
                {
                    var arrPorts = objSaveVendorSpecification.iop_value.Split(':');
                    objSaveVendorSpecification.no_of_input_port = Convert.ToInt32(arrPorts[0]);
                    objSaveVendorSpecification.no_of_output_port = Convert.ToInt32(arrPorts[1]);
                    objSaveVendorSpecification.unit = "Port";
                    //objSaveVendorSpecification.specify_type=
                }
                else
                {
                    var arrValues = objSaveVendorSpecification.iop_value.Split(' ');
                    objSaveVendorSpecification.other = arrValues[0];
                    objSaveVendorSpecification.unit = arrValues[1];
                }
            }

            if (!string.IsNullOrWhiteSpace(objSaveVendorSpecification.unit_input_type) && objSaveVendorSpecification.unit_input_type.ToLower() == "port")
            {
                objSaveVendorSpecification.iop_value = objSaveVendorSpecification.no_of_port + " Port";
                objSaveVendorSpecification.unit = "Port";
            }
            objSaveVendorSpecification.category_reference = objSaveVendorSpecification.layer_name;

            List<KeyValueDropDown> objListOfOldCost = new BLVendorSpecification().GetVendorOldCost(objSaveVendorSpecification.id == 0 ? 0 : objSaveVendorSpecification.id);
            List<KeyValueDropDown> objCatgegoryReference = new BLVendorSpecification().GetCatgegoryReference(objSaveVendorSpecification.id == 0 ? 0 : objSaveVendorSpecification.id);
            objSaveVendorSpecification = new BLVendorSpecification().SaveVendorSpecification(objSaveVendorSpecification);

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Vendor Specification Detail Saved successfully!";

            }
            else
            {
                if (objMsg.message == "" || objMsg.message == null)
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Vendor Specification Detail Updated successfully!";
                    bool isCostPerUnitChanges = false;
                    bool isServiceCostPerUnit = false;
                    string existingCostPerUnit = "";
                    string existingSericeCostPerunit = "";
                    #region 
                    if (objListOfOldCost != null && objListOfOldCost.Count > 0)
                    {
                        foreach (KeyValueDropDown obj in objListOfOldCost)
                        {
                            if (obj.key.ToLower() == "cost_per_unit")
                            {
                                isCostPerUnitChanges = Convert.ToDecimal(obj.value) != objSaveVendorSpecification.cost_per_unit ? true : false;
                                existingCostPerUnit = obj.value.ToString();
                            }
                            if (obj.key.ToLower() == "service_cost_per_unit")
                            {
                                isServiceCostPerUnit = Convert.ToDouble(obj.value) != objSaveVendorSpecification.service_cost_per_unit ? true : false;
                                existingSericeCostPerunit = obj.value.ToString();
                            }
                        }
                    }
                    if (isCostPerUnitChanges == true || isServiceCostPerUnit == true)
                    {
                        DataTable dtPriceChangeList = new DataTable();
                        dtPriceChangeList.Columns.Add("EntityName");
                        dtPriceChangeList.Columns.Add("EntitySpecification");
                        dtPriceChangeList.Columns.Add("ExistingCostPerUnit");
                        dtPriceChangeList.Columns.Add("NewCostPerUnit");
                        dtPriceChangeList.Columns.Add("ExistingServiceCostPerUnit");
                        dtPriceChangeList.Columns.Add("NewServiceCostPerUnit");
                        DataRow dtRow = dtPriceChangeList.NewRow();
                        dtRow["EntityName"] = objCatgegoryReference[0].value;
                        dtRow["EntitySpecification"] = objSaveVendorSpecification.specification;
                        dtRow["ExistingCostPerUnit"] = existingCostPerUnit;
                        dtRow["NewCostPerUnit"] = objSaveVendorSpecification.cost_per_unit.ToString();
                        dtRow["ExistingServiceCostPerUnit"] = existingSericeCostPerunit;
                        dtRow["NewServiceCostPerUnit"] = objSaveVendorSpecification.service_cost_per_unit.ToString();
                        dtPriceChangeList.Rows.Add(dtRow);
                        dtPriceChangeList.TableName = "PriceChange";
                        List<string> filePath = new List<string>();
                        var filename = "PriceChangeReport" + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "_" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
                        string filePaths = Server.MapPath("~/uploads/temp/");
                        filePath.Add(EmailAttachmentFilePath(dtPriceChangeList, filePaths + filename));
                        Dictionary<string, string> objPriceDict = new Dictionary<string, string>();
                        objPriceDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
                        //objPriceDict.Add("EntityName", objSaveVendorSpecification.category_reference);
                        //objPriceDict.Add("EntitySpecification", objSaveVendorSpecification.specification);
                        //objPriceDict.Add("ExistingCostPerUnit",existingCostPerUnit);
                        //objPriceDict.Add("NewCostPerUnit",objSaveVendorSpecification.cost_per_unit.ToString());
                        //objPriceDict.Add("ExistingServiceCostPerUnit",existingSericeCostPerunit);
                        //objPriceDict.Add("NewServiceCostPerUnit", objSaveVendorSpecification.service_cost_per_unit.ToString());
                        //List<string> objFileList = new List<string>();
                        //objFileList.Add(VendorSpecificationReportForEmail());
                        BLUser objBLuser = new BLUser();
                        List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EmailEventList.PriceChange.ToString());
                        System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objPriceDict, null, filePath, "", EmailEventList.PriceChange.ToString()));
                        //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objPriceDict, null, filePath);
                    }
                    #endregion
                }
                else
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = objSaveVendorSpecification.pageMsg.message;
                }
            }


            objSaveVendorSpecification.pageMsg = objMsg;

            //BIND VENDOR SEPCIFICATION DROPDOWNS...
            BindVendorSpecDropDowns(objSaveVendorSpecification);

            return View("VendorSpecification", objSaveVendorSpecification);

        }
        public string EmailAttachmentFilePath(DataTable dt, string filepath)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {

                    dt.TableName = "PriceChangeReport";
                    string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(file);

                    // return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                }
            }
            catch (Exception ex)
            {

            }
            return filepath;
        }

        public ActionResult ViewVendorSpecification(ViewVendorSpecificationDetailsList model, int page = 0, string sort = "", string sortdir = "")
        {
            //BIND SERACH BY DROPDOWN.. STATIC VALUES
            BindSearchBy(model);
            //BIND VENDOR SPECIFICATION LIST WITH PAGING
            model.viewVendorSpecificationDetail.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            model.viewVendorSpecificationDetail.currentPage = page == 0 ? 1 : page;
            model.viewVendorSpecificationDetail.sort = sort;
            model.viewVendorSpecificationDetail.orderBy = sortdir;
            model.VendorDetailSpecificationList = new BLVendorSpecification().GetVendorSpecificationDetailsList(model);
            model.viewVendorSpecificationDetail.totalRecord = model.VendorDetailSpecificationList != null && model.VendorDetailSpecificationList.Count > 0 ? model.VendorDetailSpecificationList[0].totalRecords : 0;

            Session["viewVendorSpecificationDetailFilter"] = model.viewVendorSpecificationDetail;
            return View("ViewVendorSpecification", model);

        }

        public IList<KeyValueDropDown> BindSearchBy(TemplateForDropDownVendorSpec objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Entity Type", value = "entity_type" });
            items.Add(new KeyValueDropDown { key = "Sub Category 1", value = "subcategory_1" });
            items.Add(new KeyValueDropDown { key = "Sub Category 2", value = "subcategory_2" });
            items.Add(new KeyValueDropDown { key = "Sub Category 3", value = "subcategory_3" });
            items.Add(new KeyValueDropDown { key = "Specification", value = "specification" });
            items.Add(new KeyValueDropDown { key = "Item Code", value = "code" });
            items.Add(new KeyValueDropDown { key = "Vendor Name", value = "vendor_name" });
            return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();
        }
        public JsonResult BindIOPDetail(string enType)
        {
            string ddtype = enType.ToLower() == "splitter" ? "splitter_ratio" : "";
            var objResp = new BLVendorSpecification().BindIOPDetails(enType, ddtype);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }



        // Validation Pending if vendor-specification details being used furture then it should not be delete (Not decided yet)
        [HttpPost]
        public JsonResult DeleteVendorSpecificationById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                DbMessage objDBMessage = new BLVendorSpecification().validateSpecification(id);
                if (!objDBMessage.status)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = objDBMessage.message;
                    return Json(objResp, JsonRequestBehavior.AllowGet);
                }
                var output = new BLVendorSpecification().DeleteVendorSpecificationById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Vendor Specification detail is deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Vendor Specification!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Vendor Specification not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSpecificationServicebyId(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                new BLVendorSpecification().DeleteSpecificationServiceById(id);
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Specification Service detail is deleted successfully!";

            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Something went wrong while deleting Specification Services!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        //public PartialViewResult AddAttributeDetail(dropdown_master objddmaster_Detail)
        //{
        //    //if (objddmaster_Detail.id != 0)
        //    if (!string.IsNullOrEmpty(objddmaster_Detail.dropdown_value))
        //    {
        //        objddmaster_Detail = new BLVendorSpecification().getAttributeDetailsById(objddmaster_Detail);

        //        return PartialView("_AddNewAttributeValue", objddmaster_Detail);

        //    }
        //    return PartialView("_AddNewAttributeValue", objddmaster_Detail);
        //}


        //[HttpPost]
        //public ActionResult SaveAttributeDetail(dropdown_master objddmaster_Detail)
        //{
        //    ModelState.Clear();
        //    PageMessage objMsg = new PageMessage();

        //    objddmaster_Detail.created_by = Convert.ToInt32(Session["user_id"]);

        //    //var isNew = objddmaster_Detail.dropdown_value != "" ? false : true;

        //    //var isNew = objddmaster_Detail.id != 0 ? false : true;

        //    if (objddmaster_Detail.entity_name.ToLower() == "splitter")
        //    {

        //        objddmaster_Detail.dropdown_value = objddmaster_Detail.inputport + ":" + objddmaster_Detail.outputport;
        //        objddmaster_Detail.dropdown_key = objddmaster_Detail.dropdown_value;
        //        objddmaster_Detail.dropdown_type = "Splitter_Ratio";

        //    }


        //    //if (isNew)
        //    //{
        //    int IsValueUnique = new BLVendorSpecification().ChkAttributeDetailExist(objddmaster_Detail);
        //    if (IsValueUnique > 0)
        //    {
        //        objMsg.status = ResponseStatus.FAILED.ToString();
        //        objMsg.message = "Input value already exists for selected entity";
        //        objddmaster_Detail.pageMsg = objMsg;

        //        return PartialView("_AddNewAttributeValue", objddmaster_Detail);
        //    }

        //    //}

        //    var output = new BLVendorSpecification().SaveAttributeDetails(objddmaster_Detail);


        //    if (output == 0)
        //    {
        //        objMsg.status = ResponseStatus.OK.ToString();
        //        objMsg.isNewEntity = true;
        //        objMsg.message = "Detail Saved successfully!";
        //        objddmaster_Detail.inputport = 0;
        //        objddmaster_Detail.outputport = 0;
        //    }

        //    else
        //    {
        //        objMsg.status = ResponseStatus.FAILED.ToString();
        //        //objMsg.message = "Detail Updated successfully!";
        //        objMsg.message = "Detail Not Saved successfully!";

        //    }


        //    //if (isNew)
        //    //{
        //    //    objMsg.status = ResponseStatus.OK.ToString();
        //    //    objMsg.isNewEntity = isNew;
        //    //    objMsg.message = "Detail Saved successfully!";
        //    //    objddmaster_Detail.inputport = 0;
        //    //    objddmaster_Detail.outputport = 0;

        //    //}
        //    //else
        //    //{
        //    //    objMsg.status = ResponseStatus.OK.ToString();
        //    //    //objMsg.message = "Detail Updated successfully!";
        //    //    objMsg.message = "Detail Saved successfully!";
        //    //    objddmaster_Detail.inputport = 0;
        //    //    objddmaster_Detail.outputport = 0;
        //    //}

        //    objddmaster_Detail.pageMsg = objMsg;

        //    return PartialView("_AddNewAttributeValue", objddmaster_Detail);
        //}

        #region VendorSpecificationHistory
        public ActionResult GetVendorSpecificationHistory(int systemId = 0, string eType = "", int page = 0, string sort = "", string sortdir = "")
        {
            if (page <= 0)
            {
                Session["viewVendorSpecificationHistory"] = null;
            }
            ViewAuditMasterModel objAudit = new ViewAuditMasterModel();
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.systemid = systemId;
            objAudit.objFilterAttributes.entityType = eType;

            if (Session["viewVendorSpecificationHistory"] != null)
            {
                var sessionValues = (FilterHistoryAttr)Session["viewVendorSpecificationHistory"];
                objAudit.objFilterAttributes.systemid = sessionValues.systemid;
                objAudit.objFilterAttributes.entityType = sessionValues.entityType;
            }
            List<Dictionary<string, string>> lstReportData = new BLVendorSpecification().GetVendorSpecificationHistoryDetailById(objAudit.objFilterAttributes);
            foreach (Dictionary<string, string> dic in lstReportData)
            {
                var obj = (IDictionary<string, object>)new System.Dynamic.ExpandoObject();
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                objAudit.lstData.Add(obj);
            }
            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;
            Session["viewVendorSpecificationHistory"] = objAudit.objFilterAttributes;
            return PartialView("ViewVendorSpecificationHistory", objAudit);
        }

        public void DownloadVendorSpecificationHistory()
        {
            if (Session["viewVendorSpecificationHistory"] != null)
            {
                FilterHistoryAttr objViewFilter = (FilterHistoryAttr)Session["viewVendorSpecificationHistory"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                List<Dictionary<string, string>> lstReportData = new BLVendorSpecification().GetVendorSpecificationHistoryDetailById(objViewFilter);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData);
                dtReport.TableName = "Vendor_Specification_History";
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }

                }
                if (dtReport.Rows.Count > 0)
                {
                    dtReport.Columns.Add("Status", typeof(string));

                    for (int i = 0; i < dtReport.Rows.Count; i++)
                    {
                        dtReport.Rows[i]["created_on"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                        dtReport.Rows[i]["modified_on"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                        dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Active" : "InActive";
                    }
                    dtReport.Columns.Remove("is_active");

                    dtReport.Columns["entity_type"].ColumnName = "Entity Type";
                    dtReport.Columns["specification"].ColumnName = "Specification";
                    dtReport.Columns["vendor_name"].ColumnName = "Vendor Name";
                    dtReport.Columns["code"].ColumnName = "Item Code";
                    //dtReport.Columns["cost_per_unit"].ColumnName = "Cost Per Unit/Mtr (Rs.)";
                    dtReport.Columns["cost_per_unit"].ColumnName = String.Format("Cost Per Unit/Mtr({0})", ApplicationSettings.Currency);
                    dtReport.Columns["iop_value"].ColumnName = "Port/Fiber Info";
                    dtReport.Columns["item_type"].ColumnName = "Item Type";
                    dtReport.Columns["unit_measurement"].ColumnName = "Unit Measurement";
                    dtReport.Columns["is_arfs"].ColumnName = "is A - RFS Item";
                    dtReport.Columns["is_brfs"].ColumnName = "is B - RFS Item";
                    dtReport.Columns["is_crfs"].ColumnName = "is C - RFS Item";

                    dtReport.Columns["subcategory_1"].ColumnName = "Sub Category 1";
                    dtReport.Columns["subcategory_2"].ColumnName = "Sub Category 2";
                    dtReport.Columns["subcategory_3"].ColumnName = "Sub Category 3";
                    //dtReport.Columns["is_active"].ColumnName = "Status";
                    dtReport.Columns["action"].ColumnName = "Action";
                    dtReport.Columns["created_by_text"].ColumnName = "Created By";
                    dtReport.Columns["created_on"].ColumnName = "Created On";
                    dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                    dtReport.Columns["modified_on"].ColumnName = "Modified On";
                    //dtReport.Columns["service_cost_per_unit"].ColumnName = "Service Cost Per Unit/Mtr (Rs.)";
                    dtReport.Columns["service_cost_per_unit"].ColumnName = String.Format("Service Cost Per Unit/Mtr({0})", ApplicationSettings.Currency);
                    dtReport.Columns["Status"].SetOrdinal(7);

                    DataTable dtCloned = dtReport.Clone();
                    dtCloned.Columns["is A - RFS Item"].DataType = typeof(string);
                    dtCloned.Columns["is B - RFS Item"].DataType = typeof(string);
                    dtCloned.Columns["is C - RFS Item"].DataType = typeof(string);


                    foreach (DataRow row in dtReport.Rows)
                    {
                        dtCloned.ImportRow(row);
                    }
                    for (int i = 0; i < dtCloned.Rows.Count; i++)
                    {
                        if (dtCloned.Rows[i]["is A - RFS Item"].ToString() == "True")
                        {
                            dtCloned.Rows[i]["is A - RFS Item"] = "Yes";
                        }
                        else
                        {
                            dtCloned.Rows[i]["is A - RFS Item"] = "No";
                        }
                        if (dtCloned.Rows[i]["is B - RFS Item"].ToString() == "True")
                        {
                            dtCloned.Rows[i]["is B - RFS Item"] = "Yes";
                        }
                        else
                        {
                            dtCloned.Rows[i]["is B - RFS Item"] = "No";
                        }
                        if (dtCloned.Rows[i]["is C - RFS Item"].ToString() == "True")
                        {
                            dtCloned.Rows[i]["is C - RFS Item"] = "Yes";
                        }
                        else
                        {
                            dtCloned.Rows[i]["is C - RFS Item"] = "No";
                        }

                    }

                    ExportVendorSpecificationData(dtCloned, "" + dtCloned.TableName.ToString() + "_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }
        private void ExportVendorSpecificationData(DataTable dtReport, string fileName)
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
        #region VendorSpecificationDetailsDownload
        [HttpGet]
        public void DownloadVendorSpecificationDetail()
        {
            if (Session["viewVendorSpecificationDetailFilter"] != null)
            {
                ViewVendorSpecificationDetailsList model = new ViewVendorSpecificationDetailsList();
                model.viewVendorSpecificationDetail = (ViewVendorSpecficationDetail)Session["viewVendorSpecificationDetailFilter"];
                model.viewVendorSpecificationDetail.pageSize = 0;
                model.viewVendorSpecificationDetail.currentPage = 0;

                model.VendorDetailSpecificationList = new BLVendorSpecification().GetVendorSpecificationDetailsList(model);
                model.viewVendorSpecificationDetail.totalRecord = model.VendorDetailSpecificationList != null && model.VendorDetailSpecificationList.Count > 0 ? model.VendorDetailSpecificationList[0].totalRecords : 0;

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<ViewVendorSpecificationList>(model.VendorDetailSpecificationList);
                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("VENDOR_ID");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Add("Status", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));

                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];

                    dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Active" : "InActive";
                }
                dtReport.Columns.Remove("is_active");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");

                dtReport.Columns["entity_type"].SetOrdinal(0);
                dtReport.Columns["specification"].SetOrdinal(1);
                dtReport.Columns["vendor_name"].SetOrdinal(2);
                dtReport.Columns["code"].SetOrdinal(3);
                dtReport.Columns["cost_per_unit"].SetOrdinal(4);
                dtReport.Columns["iop_value"].SetOrdinal(5);
                dtReport.Columns["subcategory_1"].SetOrdinal(6);
                dtReport.Columns["subcategory_2"].SetOrdinal(7);
                dtReport.Columns["subcategory_3"].SetOrdinal(8);

                dtReport.Columns["item_type"].SetOrdinal(9);
                dtReport.Columns["unit_measurement"].SetOrdinal(10);
                dtReport.Columns["is_arfs"].SetOrdinal(11);
                dtReport.Columns["is_brfs"].SetOrdinal(12);
                dtReport.Columns["is_crfs"].SetOrdinal(13);

                dtReport.Columns["Status"].SetOrdinal(14);
                dtReport.Columns["created_by_text"].SetOrdinal(15);
                dtReport.Columns["Created On"].SetOrdinal(16);
                dtReport.Columns["modified_by_text"].SetOrdinal(17);
                dtReport.Columns["Modified On"].SetOrdinal(18);


                dtReport.Columns["entity_type"].ColumnName = "Entity Type";
                dtReport.Columns["specification"].ColumnName = "Specification";
                dtReport.Columns["vendor_name"].ColumnName = "Vendor Name";
                dtReport.Columns["code"].ColumnName = "Item Code";
                dtReport.Columns["cost_per_unit"].ColumnName = string.Format("Cost Per Unit/Mtr({0})", ApplicationSettings.Currency);
                dtReport.Columns["iop_value"].ColumnName = "Port/Fiber Info";
                dtReport.Columns["subcategory_1"].ColumnName = "Sub Category 1";
                dtReport.Columns["subcategory_2"].ColumnName = "Sub Category 2";
                dtReport.Columns["subcategory_3"].ColumnName = "Sub Category 3";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                dtReport.Columns["item_type"].ColumnName = "item type";
                dtReport.Columns["unit_measurement"].ColumnName = "unit measurement";
                dtReport.Columns["is_arfs"].ColumnName = "is A - RFS Item";
                dtReport.Columns["is_brfs"].ColumnName = "is B - RFS Item";
                dtReport.Columns["is_crfs"].ColumnName = "is C - RFS Item";

                //dtReport.Columns["service_cost_per_unit"].ColumnName = "Service Cost Per Unit/Mtr (Rs.)";
                dtReport.Columns["service_cost_per_unit"].ColumnName = string.Format("Service Cost Per Unit/Mtr({0})", ApplicationSettings.Currency);

                DataTable dtCloned = dtReport.Clone();
                dtCloned.Columns["is A - RFS Item"].DataType = typeof(string);
                dtCloned.Columns["is B - RFS Item"].DataType = typeof(string);
                dtCloned.Columns["is C - RFS Item"].DataType = typeof(string);


                foreach (DataRow row in dtReport.Rows)
                {
                    dtCloned.ImportRow(row);
                }
                for (int i = 0; i < dtCloned.Rows.Count; i++)
                {
                    if (dtCloned.Rows[i]["is A - RFS Item"].ToString() == "True")
                    {
                        dtCloned.Rows[i]["is A - RFS Item"] = "Yes";
                    }
                    else
                    {
                        dtCloned.Rows[i]["is A - RFS Item"] = "No";
                    }
                    if (dtCloned.Rows[i]["is B - RFS Item"].ToString() == "True")
                    {
                        dtCloned.Rows[i]["is B - RFS Item"] = "Yes";
                    }
                    else
                    {
                        dtCloned.Rows[i]["is B - RFS Item"] = "No";
                    }
                    if (dtCloned.Rows[i]["is C - RFS Item"].ToString() == "True")
                    {
                        dtCloned.Rows[i]["is C - RFS Item"] = "Yes";
                    }
                    else
                    {
                        dtCloned.Rows[i]["is C - RFS Item"] = "No";
                    }

                }

                var filename = "VendorSpecification";
                ExportVendorSpecificationData(dtCloned, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        #endregion


        public JsonResult ValidateItemSpecificaton(int no_of_ports, string entityType, int vendor_id)
        {
            List<VendorSpecificationMaster> objResult = new List<VendorSpecificationMaster>();
            //var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == entityType.ToUpper()).FirstOrDefault() : null;
            objResult = new BLVendorSpecification().GetEntityTemplateDetails(no_of_ports, entityType, vendor_id);
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #region vendorSpecification upload
        public FileResult DownloadVendorSpecificationUploadTemplate(string FileName)
        {
            var file = "~//Content//Templates//Bulk//VendorSpecificationTemplate.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, FileName + ".xlsx");
        }
        [HttpPost]
        public ActionResult UploadVendorSpecificationData()
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
                    filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\VendorSpecifications\\"), fileName);
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
                            string strMappingFilePath = Server.MapPath("~\\Content\\Templates\\Bulk\\VendorSpecificationColumnMapping.xml");
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
                            BLVendorSpecification blVendorSpec = new BLVendorSpecification();

                            //delete DATA FROM TEMP TABLE ON THE BASIS OF UPLOADED_BY ID
                            blVendorSpec.DeleteTempVendorSpecificationData(userId);

                            List<TempVendorSpecificationMaster> lstVendorSpecification = new List<TempVendorSpecificationMaster>();

                            VendorSpecificationMaster objVendormaster = new VendorSpecificationMaster();
                            foreach (DataRow dr in dtExcelData.Rows)
                            {
                                TempVendorSpecificationMaster objVendorSpecification = new TempVendorSpecificationMaster();
                                string strErrorMsg = ValidateVendorSpecificationData(dr, ref objVendorSpecification, dicColumnMapping);

                                objVendorSpecification.created_by = userId;
                                objVendorSpecification.entity_type = Convert.ToString(dr[dicColumnMapping["entity_type"]]);
                                objVendorSpecification.specification = Convert.ToString(dr[dicColumnMapping["specification"]]);
                                objVendorSpecification.vendor = Convert.ToString(dr[dicColumnMapping["vendor"]]);
                                objVendorSpecification.code = Convert.ToString(dr[dicColumnMapping["code"]]);
                                objVendorSpecification.item_type = Convert.ToString(dr[dicColumnMapping["item_type"]]);
                                objVendorSpecification.is_arfs = Convert.ToString(dr[dicColumnMapping["item_type"]]).ToUpper() == "WCR" ? Convert.ToString(dr[dicColumnMapping["is_arfs"]]) : "FALSE";
                                objVendorSpecification.is_brfs = Convert.ToString(dr[dicColumnMapping["item_type"]]).ToUpper() == "WCR" ? Convert.ToString(dr[dicColumnMapping["is_brfs"]]) : "FALSE";
                                objVendorSpecification.is_crfs = Convert.ToString(dr[dicColumnMapping["item_type"]]).ToUpper() == "WCR" ? Convert.ToString(dr[dicColumnMapping["is_crfs"]]) : "FALSE";
                                objVendorSpecification.unit_measurement = Convert.ToString(dr[dicColumnMapping["unit_measurement"]]);
                                objVendorSpecification.cost_per_unit = Convert.ToString(dr[dicColumnMapping["cost_per_unit"]]);
                                objVendorSpecification.service_cost_per_unit = Convert.ToString(dr[dicColumnMapping["service_cost_per_unit"]]);
                                objVendorSpecification.no_of_port = Convert.ToString(dr[dicColumnMapping["no_of_port"]]);
                                objVendorSpecification.iop_value = Convert.ToString(dr[dicColumnMapping["iop_value"]]);
                                objVendorSpecification.length = Convert.ToString(dr[dicColumnMapping["length"]]);
                                objVendorSpecification.width = Convert.ToString(dr[dicColumnMapping["width"]]);
                                // objVendorSpecification.height = dr[dicColumnMapping["height"]].ToString();
                                objVendorSpecification.no_of_units = Convert.ToString(dr[dicColumnMapping["no_of_units"]]);
                                objVendorSpecification.border_width = Convert.ToString(dr[dicColumnMapping["border_width"]]);
                                objVendorSpecification.border_color = Convert.ToString(dr[dicColumnMapping["border_color"]]);
                                objVendorSpecification.subcategory_1 = Convert.ToString(dr[dicColumnMapping["subcategory_1"]]);
                                objVendorSpecification.subcategory_2 = Convert.ToString(dr[dicColumnMapping["subcategory_2"]]);
                                objVendorSpecification.subcategory_3 = Convert.ToString(dr[dicColumnMapping["subcategory_3"]]);
                                objVendorSpecification.is_active = Convert.ToString(dr[dicColumnMapping["is_active"]]);
                                objVendorSpecification.specify_type = Convert.ToString(dr[dicColumnMapping["specify_type"]]);
                                lstVendorSpecification.Add(objVendorSpecification);
                            }

                            if (lstVendorSpecification.Count > 0)
                            {
                                //SAVE DATA INTO TEMP VENDOR SPECIFICATION TABLE
                                blVendorSpec.BulkUploadVendorSpecification(lstVendorSpecification);
                                //VALIDATE AND UPLAOD VENDOR SPECIFICATION INTO MAIN TABLE.
                                dynamic result = "";
                                result = blVendorSpec.UploadVendorSpecificationForInsert(userId);
                                if (!result.status)
                                {
                                    // exit function if failed..
                                    return Json(new { strReturn = string.Format(result.message), msg = "error" }, JsonRequestBehavior.AllowGet);//"Error in uploading Buildings! <br> Error:" + result.message,
                                }

                            }
                            var getTotalUploadVendorSpecificationfailureAndSuccess = blVendorSpec.getTotalUploadBuildingfailureAndSuccess(userId);
                            var GetTotalCountOfSuccesAndFailure = "<table border='1' class='alertgrid'><thead><tr><td><b>Status</b></td><td><b>Count</b></td></tr></thead><tbody><tr><td>Success</td><td>" + getTotalUploadVendorSpecificationfailureAndSuccess.Item1 + "</td></tr><tr><td>failure</td><td>" + getTotalUploadVendorSpecificationfailureAndSuccess.Item2 + "</td></tr></tbody>";
                            strReturn = string.Format("Vendor Specification data processed successfully.", GetTotalCountOfSuccesAndFailure);
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
                ErrorLogHelper.WriteErrorLog("UploadVendorSpecificationData()", "VendorSpecification", ex);

            }
            catch (Exception ex)
            {
                msg = "error";
                strReturn = "Failed to upload Vendor Specification! <br> Error:" + ex.Message;
                ErrorLogHelper.WriteErrorLog("UploadVendorSpecificationData()", "VendorSpecification", ex);
            }
            return Json(new { strReturn = strReturn, msg = msg == "" ? "success" : msg }, JsonRequestBehavior.AllowGet);
        }
        public string ValidateVendorSpecificationData(DataRow dr, ref TempVendorSpecificationMaster objVendorSpecifation, Dictionary<string, string> dicColumnMapping)
        {

            double cost_per_unit, service_cost_per_unit;
            objVendorSpecifation.is_valid = true;
            string[] arrBooleanValues = { "YES", "NO" };
            try
            {

                if (dr[dicColumnMapping["item_type"]].ToString().ToUpper() == "WCR")
                {
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["is_brfs"]].ToString()))
                    {
                        objVendorSpecifation.is_valid = false;
                        objVendorSpecifation.error_msg = "Is B-RFS cannot be blank!";
                        objVendorSpecifation.is_brfs = "FALSE";
                    }
                    else
                    {
                        if (!Array.Exists(arrBooleanValues, m => m == dr[dicColumnMapping["is_brfs"]].ToString().ToUpper()))
                        {
                            objVendorSpecifation.is_valid = false;
                            objVendorSpecifation.error_msg = "Invalid Is B-RFS Value!";
                        }

                    }

                    if (string.IsNullOrEmpty(dr[dicColumnMapping["is_arfs"]].ToString()))
                    {
                        objVendorSpecifation.is_valid = false;
                        objVendorSpecifation.error_msg = "Is A-RFS cannot be blank!";// "Latitude can not be blank!";
                        objVendorSpecifation.is_arfs = "FALSE";
                    }
                    else
                    {
                        if (!Array.Exists(arrBooleanValues, m => m == dr[dicColumnMapping["is_arfs"]].ToString().ToUpper()))
                        {
                            objVendorSpecifation.is_valid = false;
                            objVendorSpecifation.error_msg = "Invalid Is A-RFS Value!";
                        }

                    }
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["is_crfs"]].ToString()))
                    {
                        objVendorSpecifation.is_valid = false;
                        objVendorSpecifation.error_msg = "Is C-RFS cannot be blank!";// "Latitude can not be blank!";
                        objVendorSpecifation.is_crfs = "FALSE";
                    }
                    else
                    {
                        if (!Array.Exists(arrBooleanValues, m => m == dr[dicColumnMapping["is_crfs"]].ToString().ToUpper()))
                        {

                            objVendorSpecifation.is_valid = false;
                            objVendorSpecifation.error_msg = "Invalid Is C-RFS Value!";
                        }

                    }
                }
                if (string.IsNullOrEmpty(dr[dicColumnMapping["cost_per_unit"]].ToString()))
                {
                    objVendorSpecifation.is_valid = false;
                    objVendorSpecifation.error_msg = "Cost per Unit/Mtr cannot be blank!";// "Latitude can not be blank!";
                }
                else
                {
                    if (!double.TryParse(dr[dicColumnMapping["cost_per_unit"]].ToString(), out cost_per_unit))
                    {
                        objVendorSpecifation.is_valid = false;
                        objVendorSpecifation.error_msg = "Invalid Cost per Unit/Mtr Value!";
                    }

                }

                if (string.IsNullOrEmpty(dr[dicColumnMapping["service_cost_per_unit"]].ToString()))
                {
                    objVendorSpecifation.is_valid = false;
                    objVendorSpecifation.error_msg = "Service Cost per Unit/Mtr cannot be blank!";// "Latitude can not be blank!";
                }
                else
                {
                    if (!double.TryParse(dr[dicColumnMapping["service_cost_per_unit"]].ToString(), out service_cost_per_unit))
                    {

                        objVendorSpecifation.is_valid = false;
                        objVendorSpecifation.error_msg = "Invalid Service Cost per Unit/Mtr Value!";
                    }
                }

                if (string.IsNullOrEmpty(dr[dicColumnMapping["is_active"]].ToString()))
                {
                    objVendorSpecifation.is_valid = false;
                    objVendorSpecifation.error_msg = "Is Active cannot be blank!";// "Latitude can not be blank!";
                }
                else
                {
                    // if (!string.Equals(dr[dicColumnMapping["is_active"]].ToString().ToUpper(), "YES") || !string.Equals(dr[dicColumnMapping["is_active"]].ToString().ToUpper(), "NO"))
                    // {
                    if (!Array.Exists(arrBooleanValues, m => m == dr[dicColumnMapping["is_active"]].ToString().ToUpper()))
                    {

                        objVendorSpecifation.is_valid = false;
                        objVendorSpecifation.error_msg = "Invalid Is Active Value!";
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        public void DownloadUploadVendorSpecificationLogs()
        {
            BLVendorSpecification BLVendorSpec = new BLVendorSpecification();
            DataTable dtlogs = new DataTable();
            dtlogs.Columns.Add("Entity", typeof(string));
            dtlogs.Columns.Add("Vendor", typeof(string));
            dtlogs.Columns.Add("Specification", typeof(string));
            dtlogs.Columns.Add("Item code", typeof(string));
            dtlogs.Columns.Add("Message", typeof(string));
            dtlogs.Columns.Add("Status", typeof(string));

            dtlogs.TableName = "VendorSpecificationLogs";
            int userId = Convert.ToInt32(Session["user_id"]);

            VendorSpecificationMaster VendorSpecificationMaster = new VendorSpecificationMaster();
            var status = "";
            using (var exportData = new MemoryStream())
            {
                Response.Clear(); ;
                List<TempVendorSpecificationMaster> BulkUploadLogs = BLVendorSpec.GetUploadVendorSpecificationLogs(userId);   //GetUploadBuildingLogs(userId);
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

                            dtlogs.Rows.Add(Convert.ToString(t.entity_type), Convert.ToString(t.vendor), Convert.ToString(t.specification), Convert.ToString(t.code), t.error_msg, status);
                        }
                    }
                    IWorkbook workbook = SmartInventory.Helper.NPOIExcelHelper.DataTableToExcel("xlsx", dtlogs);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", AppendTimeStamp("VendorSpecificationlogs.xlsx")));
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
        public ActionResult UploadVendorSpecification()
        {
            return View("_UploadVendorSpecification");

        }

        #endregion
        public ActionResult UploadVendorSpecificationImageDoc(int id)
        {
            UploadVendorSpecificationImageDocVM ImgDocVM = new UploadVendorSpecificationImageDocVM();
            ImgDocVM.specification_id = id;
            var lstspecifcationDocs = new BLAttachment().getSpecificationAttachments(id);
            List<UploadVendorSpecificationImageDoc> CopylstImages = new List<UploadVendorSpecificationImageDoc>();
            List<UploadVendorSpecificationImageDoc> CopylstDoc = new List<UploadVendorSpecificationImageDoc>();
            List<UploadVendorSpecificationImageDoc> CopylstLink = new List<UploadVendorSpecificationImageDoc>();
            CopylstImages = lstspecifcationDocs.Where(x => x.upload_type.ToUpper() == "IMAGE").ToList();
            CopylstDoc = lstspecifcationDocs.Where(x => x.upload_type.ToUpper() == "DOCUMENT").ToList();
            CopylstLink = lstspecifcationDocs.Where(x => x.upload_type.ToUpper() == "REFLINK").ToList();

            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

            if (CopylstImages.Count != 0)
            {
                foreach (var item in CopylstImages)
                {
                    var _imgSrc = "";
                    string imageUrl = string.Concat(FtpUrl, item.file_location, item.file_name);

                    WebClient request = new WebClient();
                    if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                        request.Credentials = new NetworkCredential(UserName, PassWord);

                    byte[] objdata = null;
                    objdata = request.DownloadData(imageUrl);
                    if (objdata != null && objdata.Length > 0)
                        _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));

                    ImgDocVM.lstImages.Add(new UploadVendorSpecificationImageDoc()
                    {
                        org_file_name = item.org_file_name,
                        ImgSrc = _imgSrc,
                        uploaded_by = item.uploaded_by,
                        id = item.id,
                        file_location = item.file_location,
                        file_name = item.file_name,
                        specification_id = item.specification_id,
                        file_size = item.file_size,
                        file_size_converted = MiscHelper.BytesToString(item.file_size),
                        created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                        uploaded_by_name = item.uploaded_by_name
                    });

                }
            }
            if (CopylstDoc.Count != 0)
            {
                foreach (var item in CopylstDoc)
                {
                    ImgDocVM.lstDocuments.Add(new UploadVendorSpecificationImageDoc()
                    {
                        id = item.id,
                        specification_id = item.specification_id,
                        file_name = item.file_name,
                        uploaded_by = item.uploaded_by,
                        org_file_name = item.org_file_name,
                        file_extension = item.file_extension,
                        file_location = item.file_location,
                        upload_type = item.upload_type,
                        file_size = item.file_size,
                        created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                        file_size_converted = MiscHelper.BytesToString(item.file_size),
                        uploaded_by_name = item.uploaded_by_name
                    });

                }
            }
            if (CopylstLink.Count != 0)
            {
                foreach (var item in CopylstLink)
                {
                    ImgDocVM.lstRefLinks.Add(new UploadVendorSpecificationImageDoc()
                    {
                        id = item.id,
                        specification_id = item.specification_id,
                        // EntityType = item.entity_type,
                        uploaded_by = item.uploaded_by,
                        created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                        org_file_name = item.org_file_name,
                        file_location = item.file_location,
                        upload_type = item.upload_type,
                        uploaded_by_name = item.uploaded_by_name
                    });
                }

            }

            return PartialView("_UploadFile", ImgDocVM);

        }

        [HttpPost]
        public ActionResult UploadDocument(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    var SpecificationId = collection["id"];
                    var featureName = collection["feature_name"];
                    var attachmentType = "Document";
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string FileName = file.FileName;
                        string strNewfilename = "specification_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strFilePath = "";

                        strFilePath = UploadfileOnFTP(featureName, SpecificationId, file, attachmentType, strNewfilename);


                        // get User Detail..
                        User objUser = (User)(Session["userDetail"]);
                        UploadVendorSpecificationImageDoc objAttachment = new UploadVendorSpecificationImageDoc();
                        objAttachment.specification_id = Convert.ToInt32(SpecificationId);
                        objAttachment.org_file_name = FileName;
                        objAttachment.file_name = strNewfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.upload_type = attachmentType;
                        objAttachment.uploaded_by = objUser.user_id;
                        objAttachment.file_size = Convert.ToInt32(file.ContentLength);
                        objAttachment.uploaded_on = DateTime.Now;
                        //Save Image on FTP and related detail in database..
                        var savefile = new BLAttachment().SaveSpecificationAttachment(objAttachment);
                    }
                    jResp.message = "File uploaded successfully";
                    jResp.status = StatusCodes.OK.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadDocument()", "VendorSpecification", ex);
                    jResp.message = "Error in uploading document!";
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
        static string UploadfileOnFTP(string FeatureName, string id, HttpPostedFileBase postedFile, string sUploadType, string newfilename)
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
                    strFTPFilePath = CreateNestedDirectoryOnFTP(strFTPPath, strFTPUserName, strFTPPassWord, FeatureName, id, sUploadType);

                    if (sUploadType.ToUpper() == "IMAGES")
                    {
                        string thumnailImageName = "Thumb_" + newfilename;
                        FtpWebRequest ftpThumbnailImage = (FtpWebRequest)WebRequest.Create(strFTPFilePath + thumnailImageName);
                        ftpThumbnailImage.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                        ftpThumbnailImage.Method = WebRequestMethods.Ftp.UploadFile;
                        ftpThumbnailImage.UseBinary = true;
                        // var image = System.Drawing.Image.FromStream(postedFile.InputStream);
                        System.Drawing.Bitmap bmThumb = new System.Drawing.Bitmap(postedFile.InputStream);
                        System.Drawing.Image bmp2 = bmThumb.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                        string saveThumnailPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                        bmp2.Save(saveThumnailPath + @"\" + thumnailImageName);
                        byte[] c = System.IO.File.ReadAllBytes(@"" + saveThumnailPath + "/" + thumnailImageName);
                        ftpThumbnailImage.ContentLength = c.Length;
                        using (Stream s = ftpThumbnailImage.GetRequestStream())
                        {
                            s.Write(c, 0, c.Length);
                        }

                        try
                        {
                            ftpThumbnailImage.GetResponse();
                        }
                        catch { throw; }
                        finally
                        {

                        }

                    }
                    FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(strFTPFilePath + newfilename);
                    ftpReq.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                    ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpReq.UseBinary = true;

                    //Save file temporarily on local path..
                    string savepath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
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
                        if (sUploadType.ToUpper() == "IMAGES")
                        {
                            System.IO.File.Delete(@"" + savepath + "/" + "Thumb_" + newfilename);
                        }
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


        [HttpPost]
        public ActionResult UploadImage(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    var SpecificationId = collection["id"];
                    var FeatureName = collection["feature_name"];
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string FileName = file.FileName;
                        // string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strNewfilename = "specification_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strFilePath = UploadfileOnFTP(FeatureName, Convert.ToString(SpecificationId), file, "Images", strNewfilename);
                        // get User Detail..
                        User objUser = (User)(Session["userDetail"]);
                        UploadVendorSpecificationImageDoc objAttachment = new UploadVendorSpecificationImageDoc();
                        objAttachment.specification_id = Convert.ToInt32(SpecificationId);
                        objAttachment.org_file_name = FileName;
                        objAttachment.file_name = strNewfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.upload_type = "Image";
                        objAttachment.uploaded_by = objUser.user_id;
                        objAttachment.file_size = (Convert.ToInt32(file.ContentLength));
                        objAttachment.uploaded_on = DateTime.Now;
                        //Save Image on FTP and related detail in database..
                        var savefile = new BLAttachment().SaveSpecificationAttachment(objAttachment);
                    }
                    jResp.message = "Image uploaded successfully!";
                    jResp.status = StatusCodes.OK.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadImage()", "Main", ex);
                    jResp.message = "Error in uploading image!";
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = "No file selected";
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult getSpecificationAttachments(int specification_Id)
        {
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

            var lstImages = new BLAttachment().getSpecificationAttachments(specification_Id);
            List<UploadVendorSpecificationImageDoc> lstImageResult = new List<UploadVendorSpecificationImageDoc>();

            foreach (var item in lstImages)
            {
                var _imgSrc = "";
                string imageUrl = string.Concat(FtpUrl, item.file_location, item.file_name);

                WebClient request = new WebClient();
                if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                    request.Credentials = new NetworkCredential(UserName, PassWord);

                byte[] objdata = null;
                objdata = request.DownloadData(imageUrl);
                if (objdata != null && objdata.Length > 0)
                    _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));

                lstImageResult.Add(new UploadVendorSpecificationImageDoc()
                {
                    org_file_name = item.org_file_name,
                    ImgSrc = _imgSrc,
                    uploaded_by = item.uploaded_by,
                    id = item.id,
                    file_location = item.file_location,
                    file_name = item.file_name,
                    specification_id = item.specification_id,
                    created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                    uploaded_by_name = item.uploaded_by_name
                });

            }
            return PartialView("_UploadFile", lstImageResult);
        }

        public JsonResult DeleteSpecificationImgDoc(int Id)
        {
            //PGDataAccess objPGDataAccess = new PGDataAccess();
            var objFileDetail = new BLAttachment().getSpecificationAttachmentsbyid(Id);
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                bool isDeletedFromDB = false;
                List<string> sImgDocPaths = new List<string>();

                if (objFileDetail != null && objFileDetail.id > 0)
                {
                    //Delete entry from database
                    isDeletedFromDB = new BLAttachment().DeleteFromSpecificationImgDoc(Id);

                    if (isDeletedFromDB && (objFileDetail.upload_type.ToUpper() == "IMAGE" || objFileDetail.upload_type.ToUpper() == "DOCUMENT"))
                    {
                        sImgDocPaths.Add(objFileDetail.file_location + objFileDetail.file_name);
                        if (objFileDetail.upload_type.ToUpper() == "IMAGE")
                        {
                            sImgDocPaths.Add(objFileDetail.file_location + "Thumb_" + objFileDetail.file_name);
                        }

                        foreach (string item in sImgDocPaths)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                DeleteFileFromFTP(item);
                            }

                        }
                    }

                }
                if (isDeletedFromDB)
                {
                    objResp.status = StatusCodes.OK.ToString();
                    objResp.message = objFileDetail.upload_type + " has been deleted Successfully";
                }
                else
                {
                    objResp.status = StatusCodes.FAILED.ToString();
                    objResp.message = "Failed to delete the " + objFileDetail.upload_type + "!";
                }

            }
            catch (Exception ex)
            {
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = ex.Message;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        static void DeleteFileFromFTP(string filePath)
        {
            try
            {
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {

                    System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(strFTPPath + @"\" + filePath);
                    //If you need to use network credentials
                    request.Credentials = new System.Net.NetworkCredential(strFTPUserName, strFTPPassWord);
                    //additionally, if you want to use the current user's network credentials, just use:
                    //System.Net.CredentialCache.DefaultNetworkCredentials
                    request.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                    System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();
                    response.Close();

                }
            }
            catch { throw; }
        }

        public PartialViewResult GetImageDetails(int specification_Id)
        {

            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
            UploadVendorSpecificationImageDocVM ImgDocVM = new UploadVendorSpecificationImageDocVM();
            ImgDocVM.specification_id = specification_Id;
            var lstImages = new BLAttachment().getSpecificationAttachments(specification_Id, "IMAGE");

            foreach (var item in lstImages)
            {
                var _imgSrc = "";
                string imageUrl = string.Concat(FtpUrl, item.file_location, item.file_name);

                WebClient request = new WebClient();
                if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                    request.Credentials = new NetworkCredential(UserName, PassWord);

                byte[] objdata = null;
                objdata = request.DownloadData(imageUrl);
                if (objdata != null && objdata.Length > 0)
                    _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));

                ImgDocVM.lstImages.Add(new UploadVendorSpecificationImageDoc()
                {
                    org_file_name = item.org_file_name,
                    ImgSrc = _imgSrc,
                    uploaded_by = item.uploaded_by,
                    id = item.id,
                    file_location = item.file_location,
                    file_name = item.file_name,
                    specification_id = item.specification_id,
                    file_size = item.file_size,
                    created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                    file_size_converted = MiscHelper.BytesToString(item.file_size),
                    uploaded_by_name = item.uploaded_by_name
                });

            }


            return PartialView("_uploadedImages", ImgDocVM);

        }

        public PartialViewResult GetDocumentDetails(int specification_Id)
        {

            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

            UploadVendorSpecificationImageDocVM ImgDocVM = new UploadVendorSpecificationImageDocVM();
            ImgDocVM.specification_id = specification_Id;
            var lstDocument = new BLAttachment().getSpecificationAttachments(specification_Id, "DOCUMENT");
            foreach (var item in lstDocument)
            {
                ImgDocVM.lstDocuments.Add(new UploadVendorSpecificationImageDoc()
                {
                    id = item.id,
                    specification_id = item.specification_id,
                    file_name = item.file_name,
                    uploaded_by = item.uploaded_by,
                    org_file_name = item.org_file_name,
                    file_extension = item.file_extension,
                    file_location = item.file_location,
                    upload_type = item.upload_type,
                    file_size = item.file_size,
                    file_size_converted = MiscHelper.BytesToString(item.file_size),
                    uploaded_by_name = item.uploaded_by_name,
                    created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString())
                });

            }

            return PartialView("_uploadedDocument", ImgDocVM);

        }

        public FileResult DownloadFileById(string json)
        {
            var id = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();

            try
            {
                UploadVendorSpecificationImageDoc data = new BLAttachment().getSpecificationAttachmentsbyid(Convert.ToInt32(id));
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);


                string fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                string FileName = data.file_location + "/" + data.file_name;
                string localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "/Attachments/" + data.file_name + "";


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



                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, data.org_file_name);
            }
            catch (Exception ex)
            {
                //context.Response.ContentType = "text/plain";
                //context.Response.Write(ex.Message);
            }
            finally
            {
                string FileAddress = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "/Attachments";
                System.IO.DirectoryInfo di = new DirectoryInfo(FileAddress);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            return null;

        }
        public FileResult DownloadFiles(string json, string type)
        {
            var specification_id = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var lstImageDoc = new BLAttachment().getSpecificationAttachments(Convert.ToInt32(specification_id), type);
            string zipName = string.Empty;
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    zip.AlternateEncodingUsage = Ionic.Zip.ZipOption.AsNecessary;
                    zip.AddDirectoryByName("Files");
                    foreach (var item in lstImageDoc)
                    {
                        string fullPath = FtpUrl + item.file_location + "/" + item.file_name;
                        string FileName = item.file_location + "/" + item.file_name;
                        string localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "/Attachments/" + item.file_name + "";

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
                        zip.AddFile(localPath, "Files");
                    }
                    zipName = String.Format("{0}{1}{2}{3}{4}.zip", "VendorSpecification_", type + "_", DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));


                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        zip.Save(memoryStream);
                        return File(memoryStream.ToArray(), "application/zip", zipName);
                    }
                    System.IO.File.Delete(zipName);
                }
            }
            catch (Exception ex)
            {
                //context.Response.ContentType = "text/plain";
                //context.Response.Write(ex.Message);
            }
            finally
            {
                string FileAddress = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "/Attachments";
                System.IO.DirectoryInfo di = new DirectoryInfo(FileAddress);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            return null;

        }

        public ActionResult UploadRefLink(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            try
            {
                var DisplayTxt = collection["refDisplayTxt"];
                var RefLink = collection["refLink"];

                var specification_id = collection["specification_id"];

                // get User Detail..
                User objUser = (User)(Session["userDetail"]);
                UploadVendorSpecificationImageDoc objAttachment = new UploadVendorSpecificationImageDoc();
                objAttachment.specification_id = Convert.ToInt32(specification_id);
                objAttachment.upload_type = "RefLink";
                //objAttachment.e = entity_type;
                //objAttachment.refDisplayTxt = DisplayTxt;
                //objAttachment.refLink = RefLink;
                objAttachment.org_file_name = DisplayTxt;
                objAttachment.file_location = RefLink;

                objAttachment.uploaded_by = objUser.user_id;
                objAttachment.uploaded_on = DateTime.Now;
                //Save Ref link details
                var savefile = new BLAttachment().SaveSpecificationAttachment(objAttachment);

                jResp.message = "Reference link uploaded successfully.";
                jResp.status = StatusCodes.OK.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadRefLink()", "VendorSpecification", ex);
                jResp.message = "Error in uploading reference link!";
                jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
                //Error Logging...
            }
        }

        public PartialViewResult getReferenceLink(int specification_id)
        {
            UploadVendorSpecificationImageDocVM ImgDocVM = new UploadVendorSpecificationImageDocVM();
            ImgDocVM.specification_id = specification_id;
            var lstReflinks = new BLAttachment().getSpecificationAttachments(specification_id, "REFLINK");

            foreach (var item in lstReflinks)
            {
                ImgDocVM.lstRefLinks.Add(new UploadVendorSpecificationImageDoc()
                {
                    id = item.id,
                    specification_id = item.specification_id,
                    uploaded_by = item.uploaded_by,
                    created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                    org_file_name = item.org_file_name,
                    file_location = item.file_location,
                    upload_type = item.upload_type,
                    uploaded_by_name = item.uploaded_by_name
                });
            }

            return PartialView("_uploadedRefLinks", ImgDocVM);

        }


        [HttpPost]
        public ActionResult CheckFileExist(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();
            var isExist = false;
            if (Request.Files.Count > 0)
            {
                try
                {

                    var Id = collection["id"];
                    var featureName = collection["feature_name"];
                    var documentType = collection["document_type"];
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string FileName = file.FileName;
                        var id = Convert.ToInt32(Id);
                        var doctype = "." + file.FileName.Split('.')[1];

                        isExist = new BLAttachment().CheckSpecificationFileExist(FileName, id, documentType, doctype);
                    }
                    if (isExist)
                    {
                        jResp.message = documentType + " with same name already exist. Do you still want to continue?";
                        jResp.status = StatusCodes.DUPLICATE_EXIST.ToString();
                    }
                    else
                    {
                        jResp.message = "";
                        jResp.status = StatusCodes.OK.ToString();
                    }

                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("CheckImageExist()", "VendorSpecification", ex);
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_243;
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_244;
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownlTextFormatFile(int specification_id, string DocType = "", string feature_name = "", int id = 0)
        {
            //var specification_id = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            var lstRefLink = new List<UploadVendorSpecificationImageDoc>();
            if (id == 0)
            {
                lstRefLink = new BLAttachment().getSpecificationAttachments(Convert.ToInt32(specification_id), DocType);
            }
            else
            {
                var othes = new BLAttachment().getSpecificationAttachmentsbyid(Convert.ToInt32(id));
                lstRefLink.Add(new UploadVendorSpecificationImageDoc()
                {

                    id = othes.id,
                    specification_id = othes.specification_id,
                    org_file_name = othes.org_file_name,
                    uploaded_by = othes.uploaded_by,
                    created_on = MiscHelper.FormatDateTime(othes.uploaded_on.ToString()),
                    file_location = othes.file_location,
                    upload_type = othes.upload_type,
                    uploaded_on = othes.uploaded_on,
                    uploaded_by_name = othes.uploaded_by_name
                });

            }


            for (int i = 0; i < lstRefLink.Count; i++)
            {
                tw.WriteLine("Display Text   :  " + lstRefLink[i].org_file_name);
                tw.WriteLine("Reference Link :  " + lstRefLink[i].file_location);
                tw.WriteLine("Uploaded By    :  " + lstRefLink[i].uploaded_by_name);
                tw.WriteLine("Uploaded On    :  " + MiscHelper.FormatDateTime(lstRefLink[i].uploaded_on.ToString()));
                if (lstRefLink.Count > 1 && i < lstRefLink.Count - 1)
                    tw.WriteLine("-----------------------------------------------------------------------------");
                tw.WriteLine("  ");
            }
            tw.Flush();
            tw.Close();
            string FName = String.Format("{0}{1}{2}{3}.txt", feature_name + "" + DocType, DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));
            return File(memoryStream.GetBuffer(), "text/plain", FName);
        }

        public IList<KeyValueDropDown> BindSearchByService(TemplateForDropDownVendorSpec objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Entity Type", value = "entity_type" });
            items.Add(new KeyValueDropDown { key = "Service Name", value = "service_name" });
            items.Add(new KeyValueDropDown { key = "Specification", value = "specification" });
            items.Add(new KeyValueDropDown { key = "Item Code", value = "code" });
            return objTemplateForDropDown.lstBindSearchByServices = items.OrderBy(m => m.key).ToList();
        }

        public ActionResult ViewSpecificationServices(ViewSpecificationServiceDetailsList model, int page = 0, string sort = "", string sortdir = "")
        {
            //BIND SERACH BY DROPDOWN.. STATIC VALUES
            BindSearchByService(model);
            //BIND VENDOR SPECIFICATION LIST WITH PAGING
            model.viewSpecificationServiceDetail.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            model.viewSpecificationServiceDetail.currentPage = page == 0 ? 1 : page;
            model.viewSpecificationServiceDetail.sort = sort;
            model.viewSpecificationServiceDetail.orderBy = sortdir;
            model.DetailSpecificationServiceList = new BLVendorSpecification().GetSpecificationServicesDetailsList(model);
            model.viewSpecificationServiceDetail.totalRecord = model.DetailSpecificationServiceList != null && model.DetailSpecificationServiceList.Count > 0 ? model.DetailSpecificationServiceList[0].totalRecords : 0;

            Session["viewSpecificationServiceDetailFilter"] = model.viewSpecificationServiceDetail;
            return View("ViewSpecificationServices", model);

        }

        public ActionResult AddSpecificationService(int id = 0, int layerId = 0)
        {
            ViewSpecificationServiceList objVendorSpecification = new ViewSpecificationServiceList();
            if (id != 0)
            {
                objVendorSpecification = new BLVendorSpecification().GetSpeicificationServiceDetailsByID(id);
                objVendorSpecification.id = id;
            }
            else objVendorSpecification.layer_id = layerId;
            //BIND VENDOR SEPCIFICATION DROPDOWNS...
            BindVendorSpecDropDowns2(objVendorSpecification);
            return View("SpecificationService", objVendorSpecification);
        }

        public ActionResult SaveSpecificationService(ViewSpecificationServiceList objSaveVendorSpecification)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            BLVendorSpecification blVendorSpec = new BLVendorSpecification();

            objSaveVendorSpecification.created_by = Convert.ToInt32(Session["user_id"]);

            var isNew = objSaveVendorSpecification.id > 0 ? false : true;

            try
            {
                new BLVendorSpecification().SaveSpecificationService(objSaveVendorSpecification);
            }
            catch { throw; }

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Service Saved successfully!";

            }
            else
            {
                if (objMsg.message == "" || objMsg.message == null)
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Service Updated successfully!";
                }
                else
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = objSaveVendorSpecification.pageMsg.message;
                }
            }


            objSaveVendorSpecification.pageMsg = objMsg;

            //BIND VENDOR SEPCIFICATION DROPDOWNS...
            BindVendorSpecDropDowns2(objSaveVendorSpecification);

            return View("SpecificationService", objSaveVendorSpecification);

        }
        public void BindVendorSpecDropDowns2(ViewSpecificationServiceList objVendorSpecification)
        {
            BLVendorSpecification blVendorSpec = new BLVendorSpecification();
            int layerId = objVendorSpecification.layer_id > 0 ? objVendorSpecification.layer_id : 0;
            objVendorSpecification.categories = blVendorSpec.GetVendorCategory();
            objVendorSpecification.lstspecification = blVendorSpec.GetAllSpecificationList(layerId);
            if (layerId > 0)
            {
                if (objVendorSpecification.categories.Where(m => m.layer_id == layerId).Count() > 0)
                {
                    objVendorSpecification.layer_name = ApplicationSettings.listLayerDetails.Where(x => x.layer_id == layerId).FirstOrDefault().layer_name;
                }
            }
            if (objVendorSpecification.item_template_id != 0)
            {
                objVendorSpecification.specification = Convert.ToString(objVendorSpecification.item_template_id);
            }
        }

    }
}