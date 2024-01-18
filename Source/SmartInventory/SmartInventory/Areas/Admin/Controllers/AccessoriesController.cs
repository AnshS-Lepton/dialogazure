using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using SmartInventory.Settings;
using Utility;
using SmartInventory.Filters;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class AccessoriesController : Controller
    {
        // GET: Admin/Accessories
        public ActionResult ViewAccessories(AccessoriesViewModel objViewAccessories, int page = 0, string sort = "", string sortdir = "", int is_active=0)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objViewAccessories.objFilterAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewAccessories.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewAccessories.objFilterAttributes.sort = sort;
            objViewAccessories.objFilterAttributes.orderBy = sortdir;
            objViewAccessories.objFilterAttributes.status = is_active;
            Session["viewAccessories"] = objViewAccessories.objFilterAttributes;
            objViewAccessories.listDatas = new BLAccessories().GetAccessoriesList(objViewAccessories, objLgnUsrDtl.user_id);
            objViewAccessories.objFilterAttributes.totalRecord = objViewAccessories.listDatas != null && objViewAccessories.listDatas.Count > 0 ? objViewAccessories.listDatas[0].totalRecords : 0;

            return View("_ViewAccessories", objViewAccessories);
        }
         
        public ActionResult AddNewAccessories(int id)
        {
            AccessoriesMaster objAccessories = new AccessoriesMaster();
             objAccessories = id > 0 ? new BLAccessories().GetAccessoriesById(id) : new AccessoriesMaster();
            if (id == 0)
                objAccessories.is_active = true;
            return PartialView("_AddAccessories", objAccessories);
        }

        public ActionResult SaveAccessories(AccessoriesMaster objAccessoriesMst)
        {
            ModelState.Clear();
            if (TryValidateModel(objAccessoriesMst))
            {
                var isNew = objAccessoriesMst.id > 0 ? false : true;
                objAccessoriesMst = new BLAccessories().SaveAccessories(objAccessoriesMst, Convert.ToInt32(Session["user_id"]));
                objAccessoriesMst.pageMsg.status = ResponseStatus.OK.ToString();
                if (isNew)
                    objAccessoriesMst.pageMsg.message = "Accessories has been saved successfully!";
                else
                    objAccessoriesMst.pageMsg.message = "Accessories has been updated successfully!";
               
            }
            return Json(objAccessoriesMst, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteAccessories(int id, string AccName)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            string entity_type="";
            var status = new BLAccessories().verifyAccessories(id, entity_type, AccName);
            if(status.status)
            { 

            var result = new BLAccessories().DeleteAccessoriesById(id, Convert.ToInt32(Session["user_id"]));
            if (result > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Accessories deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Accessories could not deleted!";
            }
            
            }
            else
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = status.message;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAccessoriesMapping(AccessoriesViewModel objViewAccessories, int page = 0, string sort = "", string sortdir = "", int is_active = 0)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objViewAccessories.objFilterAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewAccessories.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewAccessories.objFilterAttributes.sort = sort;
            objViewAccessories.objFilterAttributes.orderBy = sortdir;
            objViewAccessories.objFilterAttributes.status = is_active;
            Session["viewAccessoriesMapping"] = objViewAccessories.objFilterAttributes;
            objViewAccessories.listMappingDatas = new BLAccessories().GetAccessoriesMappingList(objViewAccessories, objLgnUsrDtl.user_id);
            objViewAccessories.objFilterAttributes.totalRecord = objViewAccessories.listMappingDatas != null && objViewAccessories.listMappingDatas.Count > 0 ? objViewAccessories.listMappingDatas[0].totalRecords : 0;
            return View("_ViewAccessoriesMapping", objViewAccessories);
        }

        public ActionResult AccessoriesMapping(int id)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            AccessoriesMapping objAccessories = new AccessoriesMapping();
            objAccessories = id > 0 ? new BLAccessories().GetAccessoriesMappingById(id) : new AccessoriesMapping();
            if (id == 0)
            objAccessories.is_active = true;
            objAccessories.lstAccessoriesdropdown = new BLAccessories().GetAccessoriesDropdownList();

            objAccessories.lstLayers = new BLLayer().GetInfoLayers();
            return PartialView("_AccessoriesMapping", objAccessories);
        }

        [HttpPost]
        public JsonResult getSpecifyTypeByLayerId(int layer_id = 0)
        {
            AccessoriesMapping objAccessoriesMapping = new AccessoriesMapping();

            BLVendorSpecification blVendorSpec = new BLVendorSpecification();
            if (layer_id > 0)
            {
                objAccessoriesMapping.layer_name = ApplicationSettings.listLayerDetails.Where(x => x.layer_id == layer_id).FirstOrDefault().layer_name;
                objAccessoriesMapping.lstSpecifyType = blVendorSpec.GetSpecifyTypeList(layer_id, objAccessoriesMapping.layer_name + "_types");
            }
            return Json(objAccessoriesMapping.lstSpecifyType, JsonRequestBehavior.AllowGet);
        }
        public bool CheckDuplicateAccessories(AccessoriesMapping objAccessoriesMsts)
        {
            List<AccessoriesMapping> objAccessoriesList = new List<AccessoriesMapping>();
            AccessoriesMapping objAccessoriesExist = new AccessoriesMapping();
            objAccessoriesExist.accessories_id = objAccessoriesMsts.accessories_id;
            objAccessoriesExist.layer_id = objAccessoriesMsts.layer_id;
            objAccessoriesExist.max_quantity = objAccessoriesMsts.max_quantity;
            objAccessoriesExist.min_quantity = objAccessoriesMsts.min_quantity;
            objAccessoriesList = new BLAccessories().ChkDuplicateAccessoriesExist(objAccessoriesExist);
            if (objAccessoriesList.Count > 0)
            {
                    return true;
            }
                return false;
        }
        public ActionResult SaveAccessoriesMapping(AccessoriesMapping objAccessoriesMsts)
        {
            var isNew = objAccessoriesMsts.id > 0 ? false : true;
            if (isNew == false)
            {
                    objAccessoriesMsts = new BLAccessories().SaveAccessoriesMapping(objAccessoriesMsts, Convert.ToInt32(Session["user_id"]));
                    objAccessoriesMsts.pageMsg.status = ResponseStatus.OK.ToString();
                    objAccessoriesMsts.pageMsg.message = "Accessories Mapping has been updated successfully!"; 
            }
            else
            {
                var isDuplicate = CheckDuplicateAccessories(objAccessoriesMsts);
                if(isDuplicate==false)
                {
                    objAccessoriesMsts = new BLAccessories().SaveAccessoriesMapping(objAccessoriesMsts, Convert.ToInt32(Session["user_id"]));
                    objAccessoriesMsts.pageMsg.status = ResponseStatus.OK.ToString();
                    objAccessoriesMsts.pageMsg.message = "Accessories Mapping has been Saved successfully!";
                }
                else
                {
                    objAccessoriesMsts.pageMsg.status = ResponseStatus.ERROR.ToString();
                    objAccessoriesMsts.pageMsg.message = "Accessories Mapping already exist!";
                }
            }
            return Json(objAccessoriesMsts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAccessoriesMapping(int id,string entity_type)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            string AccName = "";
            var status = new BLAccessories().verifyAccessories(id, entity_type, AccName);
            if (status.status)
            {

                var result = new BLAccessories().DeleteAccessoriesMappingById(id, Convert.ToInt32(Session["user_id"]));
            if (result > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Accessories Mapping deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Accessories Mapping  could not deleted!";
            }
            }
            else
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = status.message;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        public void DownloadViewAccessories()
        {
            if (Session["viewAccessories"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                AccessoriesViewModel objViewFilter = new AccessoriesViewModel();
                objViewFilter.objFilterAttributes = (FilterAccessoriesAttr)Session["viewAccessories"];
                List<AccessoriesMaster> lstViewUserDetails = new List<AccessoriesMaster>();
                objViewFilter.objFilterAttributes.currentPage = 0;
                objViewFilter.objFilterAttributes.pageSize = 0;
                lstViewUserDetails = new BLAccessories().GetAccessoriesList(objViewFilter, objLgnUsrDtl.user_id); ;

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<AccessoriesMaster>(lstViewUserDetails);
                dtReport.Columns.Add("Status", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Active" : "InActive";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_ON");
                dtReport.Columns.Remove("IS_ACTIVE");
                dtReport.Columns.Remove("MIN_QUANTITY");
                dtReport.Columns.Remove("MAX_QUANTITY");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("CREATED_BY_USER");    
                dtReport.Columns.Remove("S_NO");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns["name"].SetOrdinal(0);
                dtReport.Columns["display_name"].SetOrdinal(1);
                dtReport.Columns["created_by_text"].SetOrdinal(2);
                dtReport.Columns["created on"].SetOrdinal(3);
                dtReport.Columns["modified_by_text"].SetOrdinal(4);
                dtReport.Columns["modified on"].SetOrdinal(5);
                dtReport.Columns["status"].SetOrdinal(6);
                dtReport.Columns["name"].ColumnName = "Name";
                dtReport.Columns["display_name"].ColumnName = "Display Name";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                var filename = "AccessoriesList";
                ExportAccessoriesData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public void DownloadViewAccessoriesMapping()
        {
            if (Session["viewAccessoriesMapping"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                AccessoriesViewModel objViewFilter = new AccessoriesViewModel();
                objViewFilter.objFilterAttributes = (FilterAccessoriesAttr)Session["viewAccessoriesMapping"];
                List<AccessoriesMapping> lstViewUserDetails = new List<AccessoriesMapping>();
                objViewFilter.objFilterAttributes.currentPage = 0;
                objViewFilter.objFilterAttributes.pageSize = 0;
                lstViewUserDetails = new BLAccessories().GetAccessoriesMappingList(objViewFilter, objLgnUsrDtl.user_id); ;

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<AccessoriesMapping>(lstViewUserDetails);
                dtReport.Columns.Add("Status", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Active" : "InActive";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");
                dtReport.Columns.Remove("is_active");
                dtReport.Columns.Remove("layer_id");
                dtReport.Columns.Remove("accessories_id");
                dtReport.Columns.Remove("totalrecords");
                dtReport.Columns.Remove("s_no");
                dtReport.Columns.Remove("pagemsg");
                dtReport.Columns.Remove("lstaccessoriesdropdown");
                dtReport.Columns.Remove("lstspecifytype");
                dtReport.Columns.Remove("lstlayers");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns["name"].SetOrdinal(1);
                dtReport.Columns["layer_name"].SetOrdinal(0);
                dtReport.Columns["min_quantity"].SetOrdinal(2);
                dtReport.Columns["max_quantity"].SetOrdinal(3);
                dtReport.Columns["CREATED_BY_TEXT"].SetOrdinal(4);
                dtReport.Columns["created on"].SetOrdinal(5);
                dtReport.Columns["MODIFIED_BY_TEXT"].SetOrdinal(6);
                dtReport.Columns["modified on"].SetOrdinal(7);
                dtReport.Columns["status"].SetOrdinal(8);
                ;
                dtReport.Columns["name"].ColumnName = "Name";
                dtReport.Columns["layer_name"].ColumnName = "Entity Type";
                dtReport.Columns["min_quantity"].ColumnName = "Min Quantity";
                dtReport.Columns["max_quantity"].ColumnName = "Max Quantity";
                dtReport.Columns["CREATED_BY_TEXT"].ColumnName = "Created By";
                dtReport.Columns["MODIFIED_BY_TEXT"].ColumnName = "Modified By";
                var filename = "AccessoriesMappingList";
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
    }
}