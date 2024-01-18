using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using SmartInventory.Settings;
using Utility;

namespace SmartInventory.Areas.Admin.Controllers
{
    public class OrthoImageLayerController : Controller
    {
        // GET: Admin/ArialView
        public ActionResult ViewOrthoImage(OrthoImageModel objViewOrthoImage, int page = 0, string sort = "", string sortdir = "", int is_active = 0)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            objViewOrthoImage.objFilterAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewOrthoImage.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewOrthoImage.objFilterAttributes.sort = sort;
            objViewOrthoImage.objFilterAttributes.orderBy = sortdir;
            objViewOrthoImage.objFilterAttributes.status = is_active;
            Session["ViewOrthoImage"] = objViewOrthoImage.objFilterAttributes;
            objViewOrthoImage.listDatas = new BLOrthoImageLayer().GetOrthoImageLayerList(objViewOrthoImage, objLgnUsrDtl.user_id);
            objViewOrthoImage.objFilterAttributes.totalRecord = objViewOrthoImage.listDatas != null && objViewOrthoImage.listDatas.Count > 0 ? objViewOrthoImage.listDatas[0].totalRecords : 0;
            return View("_ViewOrthoImage", objViewOrthoImage);
        }
        public ActionResult AddNewOrthoImage(int id)
        {
            OrthoImageMasterModel objOrthoImage = new OrthoImageMasterModel();
            objOrthoImage = id > 0 ? new BLOrthoImageLayer().GetOrthoImageById(id) : new OrthoImageMasterModel();
            if (id == 0)
                objOrthoImage.is_active = true;
            return PartialView("_AddOrthoImage", objOrthoImage);
        }
        public ActionResult SaveOrthoImage(OrthoImageMasterModel objOrthoImage)
        {
            ModelState.Clear();
            if (TryValidateModel(objOrthoImage))
            {
                var isNew = objOrthoImage.system_id > 0 ? false : true;
                objOrthoImage = new BLOrthoImageLayer().SaveOrthoImage(objOrthoImage, Convert.ToInt32(Session["user_id"]));
                objOrthoImage.pageMsg.status = ResponseStatus.OK.ToString();
                if (isNew)
                    objOrthoImage.pageMsg.message = "Ortho image has been saved successfully!";
                else
                    objOrthoImage.pageMsg.message = "Ortho image has been updated successfully!";

            }
            return Json(objOrthoImage, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteOrthoImageById(int id, string AccName)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLOrthoImageLayer().DeleteOrthoImageById(id, Convert.ToInt32(Session["user_id"]));
            if (result > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Ortho image deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Ortho image could not deleted!";
            }


            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public void DownloadViewOrthoImageLayer()
        {
            if (Session["ViewOrthoImage"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                OrthoImageModel objViewFilter = new OrthoImageModel();
                objViewFilter.objFilterAttributes = (FilterOrthoImageAttr)Session["ViewOrthoImage"];
                List<OrthoImageMasterModel> lstViewUserDetails = new List<OrthoImageMasterModel>();
                objViewFilter.objFilterAttributes.currentPage = 0;
                objViewFilter.objFilterAttributes.pageSize = 0;
                lstViewUserDetails = new BLOrthoImageLayer().GetOrthoImageLayerList(objViewFilter, objLgnUsrDtl.user_id);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<OrthoImageMasterModel>(lstViewUserDetails);
                dtReport.Columns.Add("Status", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Active" : "InActive";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("SYSTEM_ID");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_ON");
                dtReport.Columns.Remove("IS_ACTIVE");
                dtReport.Columns.Remove("TOTALRECORDS");
                

                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns["display_name"].SetOrdinal(0);
                dtReport.Columns["image_name"].SetOrdinal(1);
                dtReport.Columns["image_extension"].SetOrdinal(2);
                dtReport.Columns["created_by_text"].SetOrdinal(3);
                dtReport.Columns["created on"].SetOrdinal(4);
                dtReport.Columns["modified_by_text"].SetOrdinal(5);
                dtReport.Columns["modified on"].SetOrdinal(6);
                dtReport.Columns["status"].SetOrdinal(7);
                dtReport.Columns["image_name"].ColumnName = "Image Name";
                dtReport.Columns["image_extension"].ColumnName = "Image Extension";
                dtReport.Columns["display_name"].ColumnName = "Display Name";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                var filename = "OrthoImageList";
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