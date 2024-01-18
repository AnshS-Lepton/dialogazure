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
using NPOI.SS.UserModel;
using System.Data;
using System.IO;
using SmartInventory.Helper;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class VendorController : Controller
    {
        public ActionResult CreateVendor()
        {
            CreateVendor objCreateVendor = new CreateVendor();
            return View("CreateVendor", objCreateVendor);
        }

        [HttpPost]
        public ActionResult SaveVendor(CreateVendor objSaveVendor)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            objSaveVendor.user_id = Convert.ToInt32(Session["user_id"]);
            var isNew = objSaveVendor.id > 0 ? false : true;

            int IsVendorEmailExists = new BLVendor().ChkVendorEmailExist(objSaveVendor.email_id, objSaveVendor.id);

            if (IsVendorEmailExists > 0)
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "This email already exists";

                objSaveVendor.pageMsg = objMsg;

                return View("CreateVendor", objSaveVendor);
            }


            objSaveVendor = new BLVendor().SaveEntityVendorDetails(objSaveVendor);

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Vendor Detail Saved successfully!";

            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Vendor Detail Updated successfully!";
            }


            objSaveVendor.pageMsg = objMsg;

            return View("CreateVendor", objSaveVendor);

        }


        public ActionResult ViewVendor(ViewVendorDetailsList model, int page = 0, string sort = "", string sortdir = "")
        {
            BindSearchBy(model);
            if (sort != "" || page != 0)
            {
                model.viewVendorDetail = (ViewVendorDetail)Session["viewVendorDetails"];
            }

            model.viewVendorDetail.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            model.viewVendorDetail.currentPage = page == 0 ? 1 : page;

            model.viewVendorDetail.sort = sort;
            model.viewVendorDetail.orderBy = sortdir;

            model.VendorDetailList = new BLVendor().GetVendorDetailsList(model);

            model.viewVendorDetail.totalRecord = model.VendorDetailList.Count > 0 ? model.VendorDetailList[0].totalRecords : 0;
            Session["viewVendorDetails"] = model.viewVendorDetail;
            return View("ViewVendor", model);

        }
        [HttpGet]
        public void ExportVendorDetails()
        {
            ViewVendorDetailsList VenderDetailsList = new ViewVendorDetailsList();
            if (Session["viewVendorDetails"] != null)
            {

                VenderDetailsList.viewVendorDetail = (ViewVendorDetail)Session["viewVendorDetails"];
                IList<ViewVendorList> lstViewVendorDetails = new List<ViewVendorList>();
                VenderDetailsList.viewVendorDetail.currentPage = 0;
                VenderDetailsList.viewVendorDetail.pageSize = 0;
                lstViewVendorDetails = new BLVendor().GetVendorDetailsList(VenderDetailsList);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<ViewVendorList>(lstViewVendorDetails);
                dtReport.TableName = "View_Vendor_Details";
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("ID");
                //dtReport.Columns.Remove("IS_ACTIVE");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_ON");

                dtReport.Columns["name"].SetOrdinal(0);
                dtReport.Columns["type"].SetOrdinal(1);
                dtReport.Columns["address"].SetOrdinal(2);
                dtReport.Columns["contact"].SetOrdinal(3);
                dtReport.Columns["email_id"].SetOrdinal(4);
                dtReport.Columns["remarks"].SetOrdinal(5);
                dtReport.Columns["created_by_text"].SetOrdinal(6);
                dtReport.Columns["created on"].SetOrdinal(7);
                dtReport.Columns["modified_by_text"].SetOrdinal(8);
                dtReport.Columns["modified on"].SetOrdinal(9);

                dtReport.Columns["name"].ColumnName = "Name";
                dtReport.Columns["type"].ColumnName = "Type";
                dtReport.Columns["address"].ColumnName = "Address";
                dtReport.Columns["contact"].ColumnName = "Contact";
                dtReport.Columns["email_id"].ColumnName = "Email-Id";
                dtReport.Columns["remarks"].ColumnName = "Remarks";
                dtReport.Columns["is_active"].ColumnName = "Status";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["created on"].ColumnName = "Created On";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                dtReport.Columns["modified on"].ColumnName = "Modified On";
                var filename = "ViewVendorDetails";
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

        public ActionResult ViewVendorUpdateMode(int id)
        {
            CreateVendor objCreateVendor = new CreateVendor();
            objCreateVendor = new BLVendor().GetVendorDetailsByID(id);
            return View("CreateVendor", objCreateVendor);
        }


        public IList<KeyValueDropDown> BindSearchBy(TemplateForDropDownVendor objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Name", value = "name" });
            items.Add(new KeyValueDropDown { key = "Address", value = "address" });
            items.Add(new KeyValueDropDown { key = "Contact", value = "contact" });
            items.Add(new KeyValueDropDown { key = "Email Id", value = "email_id" });
            items.Add(new KeyValueDropDown { key = "Remarks", value = "remarks" });
            items.Add(new KeyValueDropDown { key = "Type", value = "type" });
            //items.Add(new KeyValueDropDown { key = "Status", value = "is_active" });

            return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();

        }


        // Validation Pending if vendor's details being used furture then it should not be delete (Not decided yet)
        [HttpPost]
        public JsonResult DeleteVendorDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                var IsVendorUsing = new BLVendorSpecification().GetVendorSpeicificationDetailsByID(0, id);

                if (IsVendorUsing != null)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Selected vendor can not be deleted as it is in use!";
                }

                else
                {

                    var output = new BLVendor().DeleteVendorById(id);
                    if (output > 0)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        objResp.message = "Vendor Detail Deleted successfully!";
                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = "Something went wrong while deleting Vendor!";
                    }
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Vendor not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        //    [HttpPost]
        //    public JsonResult IsEmailExists(CreateVendor objSaveVendor)
        //{
        //        if (objSaveVendor.email_id != "")
        //        {
        //            objSaveVendor = new BLVendor().ChkEmailExist(objSaveVendor.email_id);
        //        }

        //        if (objSaveVendor != null)
        //        {
        //            return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
        //        }

        //        return Json(new { Data = true }, JsonRequestBehavior.AllowGet);
        //    }

    }
}