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
using System.Dynamic;
using static iTextSharp.text.pdf.AcroFields;
using BusinessLogics;
using Models.WFM;
using DataUploader;
using System.Windows.Input;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using NPOI.SS.Formula.Functions;
using iTextSharp.text.pdf.parser;

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
        public ActionResult ViewItemVendorCost(ViewItemVendorCost objViewItemVendorCost, int page = 0, string sort = "", string sortdir = "")
        {
            CommonGridAttr objGridAttributes=new CommonGridAttr();
            BindSearchBy(objViewItemVendorCost);
            if (sort != "" || page != 0)
            {
                objViewItemVendorCost.objGridAttributes = (CommonGridAttr)Session["ViewItemVendorCost"];
            }
            objViewItemVendorCost.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewItemVendorCost.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewItemVendorCost.objGridAttributes.sort = sort;
            objViewItemVendorCost.objGridAttributes.orderBy = sortdir;
            objViewItemVendorCost.lstItem= new BLVendorSpecification().ItemVendorCost(objViewItemVendorCost.objGridAttributes).ToList();
            
            var users = objViewItemVendorCost.lstItem
                .Select(x => new { x.user_id, x.user_name })
                .Distinct()
                .OrderBy(x => x.user_id) // Ensure ordering is based on user_name
                .ToList();
         
                
            Session["ViewItemVendorCost"] = objViewItemVendorCost.objGridAttributes;
            // Transform data dynamically
            var transformedData = objViewItemVendorCost.lstItem
                    .GroupBy(x => new { x.code, x.specification, x.category_reference, x.unit_measurement,x.layer_id })
                    .Select(g =>
                    {
                        dynamic row = new ExpandoObject();
                        var dict = (IDictionary<string, object>)row;

                        // Fixed properties
                        dict["code"] = g.Key.code;
                        dict["specification"] = g.Key.specification;
                        dict["entity_type"] = g.Key.category_reference;
                        dict["uom"] = g.Key.unit_measurement;
                        dict["layer_id"] = g.Key.layer_id;


                        // Dynamically add user columns
                        foreach (var user in users)
                        {
                            var costValue = g.FirstOrDefault(x => x.user_id == user.user_id)?.cost_per_unit.ToString() ?? "";
                            //dict[$"User_{user.user_name}"] = costValue+"/"+user.user_id;
                            dict[$"User_{user.user_name+"/"+user.user_id}"] = costValue;
                        }

                        return row;
                    })
                .ToList();

            ViewBag.transformedData = transformedData;
            
           
            objViewItemVendorCost.objGridAttributes.totalRecord = objViewItemVendorCost.lstItem.Select(a => a.totalRecord).FirstOrDefault();
            return View("ViewItemVendorCost", objViewItemVendorCost);
        }
        public IList<KeyValueDropDown> BindSearchBy(ViewItemVendorCost objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Item Code", value = "code" });
            items.Add(new KeyValueDropDown { key = "Specification", value = "specification" });
            items.Add(new KeyValueDropDown { key = "Entity Type", value = "category_reference" });
            items.Add(new KeyValueDropDown { key = "UOM", value = "unit_measurement" });
            return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();

        }
        public ActionResult AddItemVendorCost(VendorSpecificationMaster objVendorSpecification)
        {
            BLVendorSpecification blVendorSpec = new BLVendorSpecification();

            //VendorSpecificationMaster objVendorSpecification = id != 0 ? new BLVendorSpecification().GetVendorSpeicificationDetailsByID(id) : new VendorSpecificationMaster();
            BindItemVendorSpecDropDowns(objVendorSpecification);
            objVendorSpecification.key = objVendorSpecification.layer_id;
            objVendorSpecification.code = objVendorSpecification.code;
            return View("AddItemVendorCost", objVendorSpecification);
        }

        public void BindItemVendorSpecDropDowns(VendorSpecificationMaster objVendorSpecification)
        {
            objVendorSpecification.lstUserDetails=new BLUser().GetPartnerUser().ToList();
            BLVendorSpecification blVendorSpec = new BLVendorSpecification();
            
            objVendorSpecification.listItemCategory = blVendorSpec.GetVendorItemCategory();
            objVendorSpecification.listItemCode = blVendorSpec.GetItemVendorCode(objVendorSpecification.layer_id, objVendorSpecification.specification);
            objVendorSpecification.lstItemSpec = blVendorSpec.GetItemSpec(objVendorSpecification.layer_id);
            objVendorSpecification.unit_measurement =  blVendorSpec.GetUOM(objVendorSpecification.layer_id, objVendorSpecification.specification, objVendorSpecification.code);


        }
        
        public ActionResult BindSpecificationBylayerId(VendorSpecificationMaster objVendorSpecification)
        {
            BLVendorSpecification blVendorSpec = new BLVendorSpecification();
            objVendorSpecification.lstItemSpec = blVendorSpec.GetItemSpec(objVendorSpecification.layer_id);
            return Json(new { Data = objVendorSpecification, JsonRequestBehavior.AllowGet });
        }
        public ActionResult BindItemCodeBySpecification_layerId(VendorSpecificationMaster objVendorSpecification)
        {
            BLVendorSpecification blVendorSpec = new BLVendorSpecification();
            objVendorSpecification.listItemCode = blVendorSpec.GetItemVendorCode(objVendorSpecification.layer_id, objVendorSpecification.specification);
            return Json(new { Data = objVendorSpecification, JsonRequestBehavior.AllowGet });
        }
        public ActionResult getUOMByItemCode(VendorSpecificationMaster objVendorSpecification)
        {
            BLVendorSpecification blVendorSpec = new BLVendorSpecification();
            objVendorSpecification.unit_measurement = blVendorSpec.GetUOM(objVendorSpecification.layer_id, objVendorSpecification.specification, objVendorSpecification.code);
            return Json(new { Data = objVendorSpecification, JsonRequestBehavior.AllowGet });
        }
        [HttpPost]
        public JsonResult SaveItemVendorCostDetails(VendorSpecificationMaster objSpecificationMaster, int layer_id=0)
        {
            ModelState.Clear();
            ItemVendorCostMaster objivcm = new ItemVendorCostMaster();
            objivcm.layer_id = objSpecificationMaster.key;
            objivcm.specification = objSpecificationMaster.specification;
            objivcm.item_code = objSpecificationMaster.code;
            objivcm.uom = objSpecificationMaster.unit_measurement;
            objivcm.user_id = objSpecificationMaster.user_id;
            objivcm.item_cost = objSpecificationMaster.cost_per_unit;

            PageMessage objMsg = new PageMessage();
            if (objivcm.layer_id != 0 && objivcm.specification != null)
            {
                var status = new BLVendorSpecification().SaveItemVendorCostDetails(objivcm, Convert.ToInt32(Session["user_id"]));
                if (status == "Save")
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Item VendorCost Saved successfully!";
                }
                else if (status == "Update")
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "Item VendorCost Updated successfully!";
                }
                else if (status == "Failed")
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "Unable to update Item Vendor Cost as it is already been mapped!";
                }
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Mandatory fields required";
            }
            objivcm.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public void DownloadItemVendorCostDetails()
        {
            try
            {

                try
                {
                    ViewItemVendorCost objViewItemVendorCost = new ViewItemVendorCost();
                    CommonGridAttr objGridAttributes = new CommonGridAttr();
                    int page = 0;
                    objViewItemVendorCost.objGridAttributes.pageSize = 0;// ApplicationSettings.ViewAdminDashboardGridPageSize;
                    objViewItemVendorCost.objGridAttributes.currentPage = 0;// page == 0 ? 1 : page;
                    objViewItemVendorCost.objGridAttributes.totalRecord = 0;
                    objViewItemVendorCost.lstItem=new BLVendorSpecification().ItemVendorCost(objViewItemVendorCost.objGridAttributes).ToList();

                     var users = objViewItemVendorCost.lstItem
                    .Select(x => new { x.user_id, x.user_name })
                    .Distinct()
                   // .OrderBy(x => x.user_id) // Ensure ordering is based on user_name
                    .ToList();
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("Sheet-1");
                
                    var currR = 0;
                    IRow currRow = null;
                   
                    currRow = sheet.CreateRow(0);
                    var finalData = new List<string> { "Item Code", "Specification", "Entity Type" };
                    finalData.AddRange(users.Select(u => Convert.ToString(u.user_name)).Distinct());
                    finalData.Add("UOM");
                    currR = currRow.RowNum;
                    AddHeader(workbook, sheet, finalData);
                    
                    workbook = DataTableToExcelVendorCostDetails(objViewItemVendorCost.lstItem, workbook, "xlsx", sheet, currRow, users.Count);
                    using (var exportData = new MemoryStream())
                    {
                        Response.Clear();
                       
                            
                            workbook.Write(exportData);
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "ItemVendorCost" + ".xlsx"));
                            Response.BinaryWrite(exportData.ToArray());
                            Response.End();
                        
                    }  
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("DownloadSiteReport()", "Report", ex);
                    throw ex;
                }
            }

            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadSiteReport()", "Report", ex);
                throw ex;
            }
        }
        public static void AddHeader(IWorkbook workbook, ISheet sheet, List<string> users)
        {
            
            ICellStyle headerStyle = workbook.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center;
            IFont headerFont = workbook.CreateFont();
            headerFont.Boldweight = (short)FontBoldWeight.Bold;
            headerFont.FontName = "Arial";
            headerFont.FontHeightInPoints = 10;
            headerStyle.SetFont(headerFont);
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;
            headerStyle.BottomBorderColor = IndexedColors.Black.Index;
            headerStyle.TopBorderColor = IndexedColors.Black.Index;
            headerStyle.LeftBorderColor = IndexedColors.Black.Index;
            headerStyle.RightBorderColor = IndexedColors.Black.Index;

            IRow row = sheet.CreateRow(0);
            NPOIExcelHelper.CreateCustomCellFiberAllocation(row, 0, "", headerStyle, true, false);
            var rn3 = row.RowNum;
            row.HeightInPoints = 20;
            var ct = users.Count;
            for (int i = 0; i <= users.Count-1; i++)
            {
                int columnIndex = i;
                var cell = row.CreateCell(columnIndex);
                cell.SetCellValue(users[i % ct]);
                cell.CellStyle = headerStyle;
            }
            




        }
       
        public static IWorkbook DataTableToExcelVendorCostDetails(List<VendorSpecificationMaster> filteredData, IWorkbook workbook, string extension, ISheet sheet, IRow currRow,int usercount)//arvind
        {

            var currR = 0;
         
            currR = currRow.RowNum;
            currRow = sheet.CreateRow(currR+1);
            currR = currRow.RowNum;
            //-----------------------------------loops through data--------------------------------------------------------------------------------        
            var cellstyle = NPOIExcelHelper.getCellStyle(workbook);
            int col = 0;int colctn = 0;
            foreach (var (item1, index) in filteredData.GroupBy(m => m.user_id).Select(group => group.Key).Select((item, index) => (item, index)).ToList())
            {
                if (colctn == 1) { col = col + 1; colctn = 0; }
                foreach (var item in filteredData.Where(m => m.user_id == item1 && m.user_id!=0))
                {
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 0, item.code, cellstyle, true, false);
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 1, item.specification, cellstyle, true, false);
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 2, item.category_reference, cellstyle, true, false);
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, col +3, string.IsNullOrEmpty(Convert.ToString(item.cost_per_unit)) ? "0" : Convert.ToString(item.cost_per_unit), cellstyle, true, false);
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, usercount + 3, item.unit_measurement, cellstyle, true, false);
                    currRow = sheet.CreateRow(currR + 1);
                    currR = currRow.RowNum;
                    
                    colctn = 1;
                }

            }
            for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
            {
                for (int i = 0; i <= filteredData.Count(); i++)
                {
                    sheet.AutoSizeColumn(i);
                }
            }
            return workbook;

        }
    }
}