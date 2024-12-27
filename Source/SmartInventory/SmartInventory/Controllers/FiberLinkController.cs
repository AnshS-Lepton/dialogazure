
using BusinessLogics;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class FiberLinkController : Controller
    {
        // GET: FiberLink 
        public ActionResult ShowFiberLinkDetails(FiberLinkFilter objFiberLinkFilter, int page = 0, string sort = "", string sortdir = "")
        {
            var objUser = (User)Session["userDetail"];
            objFiberLinkFilter.pageSize = 10;
            objFiberLinkFilter.userid = objUser.user_id;
            objFiberLinkFilter.currentPage = page == 0 ? 1 : page;
            objFiberLinkFilter.sort = sort;
            objFiberLinkFilter.orderBy = sortdir;
            var jsonSerialiser = new JavaScriptSerializer();
            var lstFiberLinkStatus = jsonSerialiser.Serialize(objFiberLinkFilter.lstFiberLinkStatus);
            objFiberLinkFilter.objFiberLink.user_role_id = objUser.role_id;

            List<Dictionary<string, string>> lstFiberLinkDetails = new BLFiberLink().getFiberLinkDetails(objUser.user_id, objFiberLinkFilter);

            string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
            foreach (Dictionary<string, string> dic in lstFiberLinkDetails)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                //string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                objFiberLinkFilter.lstFiberLinkDetails.Add(obj);
            }
            objFiberLinkFilter.lstFiberLinkDetails = BLConvertMLanguage.MultilingualConvert(objFiberLinkFilter.lstFiberLinkDetails, arrIgnoreColumns);

            objFiberLinkFilter.lstFiberLinkStatus = new BLFiberLink().getfiberLinkStatusCounts(objFiberLinkFilter, objUser.user_id);

            var lstColumnMapping = new BLFiberLinkColumns().getFiberLinkColumns();
            objFiberLinkFilter.lstSearchByColumns = lstColumnMapping;
            objFiberLinkFilter.objFiberLink.lstFiberLinkColumnsMapping = lstColumnMapping.Select(x => x.column_name).ToList();

            objFiberLinkFilter.totalRecord = lstFiberLinkDetails.Count > 0 ? Convert.ToInt32(lstFiberLinkDetails[0].FirstOrDefault().Value) : 0;
            objFiberLinkFilter.lstFiberLinkColumnsMapping = objFiberLinkFilter.objFiberLink.lstFiberLinkColumnsMapping;
            Session["viewFiberLinkDashboard"] = objFiberLinkFilter;
            BindFiberLinkDropDown(objFiberLinkFilter.objFiberLink);
            return PartialView("_ViewFiberLink", objFiberLinkFilter);
        }

        public void BindFiberLinkDropDown(FiberLink objFiberLink)
        {
            objFiberLink.lstStartPointType = new BLLayer().GetStartEndPointType();
            objFiberLink.lstEndPointType = objFiberLink.lstStartPointType;
            var lstColumnMapping = new BLFiberLinkColumns().getFiberLinkColumns();
            objFiberLink.lstFiberLinkColumnsMapping = lstColumnMapping.Select(x => x.column_name).ToList();
            // Add Fiber link Type (NT Requirement 13-Apr-21)
            objFiberLink.lstLinkType = new BLMisc().GetDropDownList("", DropDownType.FiberLinkType.ToString());
            objFiberLink.lstPrefixType = new BLMisc().GetDropDownList("", DropDownType.FiberLinkPrefix.ToString());

        }

        public PartialViewResult AddFiberLink(int system_id = 0)
        {
            FiberLink objFiberLink = new FiberLink();
            int user_id = Convert.ToInt32(Session["user_id"]);
            if (system_id > 0)
            {
                User objUser = new BLUser().GetUserDetailByID(user_id);

                objFiberLink = new BLFiberLink().GetFiberLinkById(system_id);
                objFiberLink.gis_length = Math.Round(objFiberLink.gis_length, 3);
                objFiberLink.total_route_length = Math.Round(objFiberLink.total_route_length, 3);
                objFiberLink.lstFiberLinkAttachments = new BLAttachment().getAttachmentDetails(system_id, "", "Document", "");
                foreach (var item in objFiberLink.lstFiberLinkAttachments)
                {
                    item.file_size_converted = BytesToString(item.file_size);

                }
                if (objUser != null)
                {
                    objFiberLink.user_role_id = objUser.role_id;
                }

            }
            BindFiberLinkDropDown(objFiberLink);
            return PartialView("_CreateFiberLink", objFiberLink);
        }



        public ActionResult SaveFiberLink(FiberLink objFiberLink)
        {
            ModelState.Clear();
            if (TryValidateModel(objFiberLink))
            {
                // GENERATE LINK NETWORK ID
                if (objFiberLink.system_id == 0)
                {
                    var FLNetworkId = new BLFiberLink().GetFiberLinkNetworkId();
                    objFiberLink.network_id = FLNetworkId.network_id;
                }
                var result = new BLFiberLink().SaveFiberLink(objFiberLink, Convert.ToInt32(Session["user_id"]));
                if (result != null && string.IsNullOrEmpty(result.pageMsg.message))
                {
                    if (objFiberLink.pageMsg.isNewEntity)
                    {
                        objFiberLink.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_422;
                        objFiberLink.pageMsg.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        objFiberLink.pageMsg.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_029;
                        objFiberLink.pageMsg.status = ResponseStatus.OK.ToString();
                    }
                }
                else
                {
                    objFiberLink.pageMsg.status = ResponseStatus.FAILED.ToString();
                    objFiberLink.pageMsg.message = Resources.Resources.SI_GBL_GBL_NET_FRM_115;
                }
            }
            else
            {
                objFiberLink.pageMsg.status = ResponseStatus.FAILED.ToString();
                objFiberLink.pageMsg.message = getFirstErrorFromModelState();
            }
            return Json(objFiberLink, JsonRequestBehavior.AllowGet);
        }

        //Mayank 
        public FileResult DownloadUploadFiberTemplate(string FileName)
        {
            var file = "~//Content//Templates//Bulk//FiberData.xlsx";
            string contentType = "";
            try
            {
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            catch (Exception ex)
            {
                //DACommon.WriteAdminErrorLogDB("DownloadTemplate", "DownloadTemplate[Maker]", ex);
            }
            return File(file, contentType, FileName + ".xlsx");
        }

        [HttpPost]
        public ActionResult UploadFiberLink()
        {
            string strReturn = "";
            string msg = "";
            dynamic result = "";
            //table for data 
            DataTable dtExcelData = new DataTable();
            try
            {
                if (Request != null)
                {
                    int userId = Convert.ToInt32(Session["user_id"]);
                    var objfile = Request.Files[0];
                    var fileName = AppendTimeStamp(Request.Files[0].FileName);
                    var filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\Fiber\\"), fileName);
                    objfile.SaveAs(filepath);
                    bool isHeaderFound = false;
                    DataTable dataTable = NPOIExcelHelper.ExcelToTable(filepath, out isHeaderFound);
                    if (!isHeaderFound)
                    {
                        return Json(new { strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_323, msg = "error" }, JsonRequestBehavior.AllowGet);
                    }
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        dtExcelData = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
                    }
                    if (dtExcelData.Rows.Count > 0)
                    {

                        if (dtExcelData.Rows.Count <= ApplicationSettings.BulkFiberUploadMaxCount)
                        {
                            string ErrorMsg = "";
                            if (ErrorMsg != "")
                                return Json(new { strReturn = ErrorMsg, msg = "error" }, JsonRequestBehavior.AllowGet);
                            if (ErrorMsg == "")
                            {

                                //ADD COLUMN TO DTEXCEL.. (IS_VALID)
                                DataColumn dcIsValid = new DataColumn("IS_VALID", typeof(int));
                                dtExcelData.Columns.Add(dcIsValid);

                                //ADD COLUMN TO DTEXCEL.. (ERROR_MSG)
                                DataColumn dcErrorMsg = new DataColumn("ERROR_MSG", typeof(string));
                                dcErrorMsg.MaxLength = 200;
                                dtExcelData.Columns.Add(dcErrorMsg);

                            }
                            BLTempFiberLink.Instance.DeleteTempFiberData(userId);

                            List<TempFiberLink> lstFiberLink = new List<TempFiberLink>();

                            foreach (DataRow dr in dtExcelData.Rows)
                            {


                                TempFiberLink objTempFiberLink = new TempFiberLink();
                                

                                objTempFiberLink.link_id = dr["link_id"].ToString();
                                objTempFiberLink.link_name = dr["link_name"].ToString();
                                objTempFiberLink.link_type = dr["link_type"].ToString();
                                objTempFiberLink.main_link_type = dr["main_link_type"].ToString();
                                objTempFiberLink.main_link_id = dr["main_link_id"].ToString();
                                objTempFiberLink.redundant_link_id = dr["redundant_link_id"].ToString();
                                objTempFiberLink.redundant_link_type = dr["redundant_link_type"].ToString();
                                objTempFiberLink.end_point_location = dr["end_point_location"].ToString();
                                //objTempFiberLink.end_point_network_id = dr["end_point_network_id"].ToString();
                                objTempFiberLink.end_point_type = dr["end_point_type"].ToString();
                                //objTempFiberLink.start_point_network_id = dr["start_point_network_id"].ToString();
                                objTempFiberLink.each_lmc_length = dr["each_lmc_length"].ToString();
                                objTempFiberLink.start_point_location = dr["start_point_location"].ToString();
                                objTempFiberLink.start_point_type = dr["start_point_type"].ToString();
                                objTempFiberLink.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                                objTempFiberLink.created_on = DateTimeHelper.Now;
                                objTempFiberLink.handover_date = DateTimeHelper.Now;
                                objTempFiberLink.hoto_signoff_date = DateTimeHelper.Now;
                                objTempFiberLink.fiber_link_status = "Free";
                                //Get Network Id


                                string strErrorMsg = ValidateFiberData(dr, ref objTempFiberLink);
                                lstFiberLink.Add(objTempFiberLink);
                               

                            }

                            if (lstFiberLink.Count > 0)
                            {
                                for(int i =0;i< lstFiberLink.Count; i++)
                                {
                                    var FLNetworkId = new BLFiberLink().GetFiberLinkNetworkId();
                                    lstFiberLink[i].network_id = FLNetworkId.network_id.ToString();
                                    List<TempFiberLink> lst =new List<TempFiberLink> { lstFiberLink[i] };   
                                    BLTempFiberLink.Instance.BulkUploadTempFiber(lst);
                                    result = BLTempFiberLink.Instance.UploadFiber(userId, lstFiberLink[i].network_id.ToString());                               
                                }                           
                                if (!result.status)
                                {
                                    // exit function if failed..
                                    return Json(new { strReturn = String.Format("Error in uploading Fiber!", result.message), msg = "error" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            var getTotalUploadFiberfailureAndSuccess = BLTempFiberLink.Instance.getTotalUploadFiberfailureAndSuccess(userId);
                            var GetTotalCountOfSuccesAndFailure = "<table border='1' class='alertgrid'><thead><tr><td><b>Status</b></td><td><b>Count</b></td></tr></thead><tbody><tr><td>Success</td><td>" + getTotalUploadFiberfailureAndSuccess.Item1 + "</td></tr><tr><td>failure</td><td>" + getTotalUploadFiberfailureAndSuccess.Item2 + "</td></tr></tbody></table>";
                            var message = "Fiber Link data saved successfully.";
                            strReturn = String.Format(message + GetTotalCountOfSuccesAndFailure);
                        }

                        else
                        {
                            // exit function with max record error...
                            return Json(new { strReturn = String.Format("Maximum {0} Fiber can be uploaded at a time!", ApplicationSettings.BulkFiberUploadMaxCount), msg = "error" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        // exit function with no record...
                        return Json(new { strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_327, msg = "error" }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (NPOI.POIFS.FileSystem.NotOLE2FileException ex)
            {
                msg = "error";
                strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_328;
                ErrorLogHelper.WriteErrorLog("UploadTicketData()", "Library", ex);

            }
            catch (Exception ex)
            {
                msg = "error";
                strReturn = String.Format("Failed to upload Tickets!" + Environment.NewLine + ex.Message);
                ErrorLogHelper.WriteErrorLog("UploadTicketData()", "Library", ex);
            }
            return Json(new { strReturn = strReturn, msg = msg == "" ? "success" : msg }, JsonRequestBehavior.AllowGet);
        }
        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(
            Path.GetFileNameWithoutExtension(fileName),
            DateTimeHelper.Now.ToString("yyyyMMddHHmmssfff"),
            Path.GetExtension(fileName)
            );

        }
        public string ValidateFiberData(DataRow dr, ref TempFiberLink objTempFiberLink)
        {

            objTempFiberLink.is_valid = true;
            Regex nonNumericRegex = new Regex(@"\.");
            try
            {
                if (objTempFiberLink.link_type == "Main Link".ToString())
                {

                    if (string.IsNullOrWhiteSpace(dr["link_id"].ToString()))
                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "link_id Can Not Be Blank! ";
                    }
                    if (string.IsNullOrWhiteSpace(dr["redundant_link_type"].ToString()))
                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "redundant_link_type Can Not Be Blank! ";
                    }
                    if (string.IsNullOrWhiteSpace(dr["redundant_link_id"].ToString()))
                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "redundant_link_id Can Not Be Blank!";
                    }
                    if (string.IsNullOrWhiteSpace(dr["link_name"].ToString()))
                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "link_name Can Not Be Blank!";
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(dr["link_id"].ToString()))
                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "link_id Can Not Be Blank!";
                    }
                    if (string.IsNullOrWhiteSpace(dr["main_link_type"].ToString()))
                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "main_link_type Not Be Blank!";
                    }
                    if (string.IsNullOrWhiteSpace(dr["main_link_id"].ToString()))

                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "main_link_id Not Be Blank!";
                    }
                    if (string.IsNullOrWhiteSpace(dr["link_name"].ToString()))
                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "link_name Not Be Blank!";
                    }

                }
                
                if ((dr["link_id"].ToString()) !=""  || !string.IsNullOrWhiteSpace(dr["link_id"].ToString()))
                {
                    string link_id = dr["link_id"].ToString();
                    var lstLinkId = new BLFiberLink().checkDuplicaketLinkId(link_id);
                    if (lstLinkId.Count > 0)
                    {
                        objTempFiberLink.is_valid = false;
                        objTempFiberLink.error_msg = "Duplicate link_id  is found !";
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }
        public void DownloadUploadFiberLogs()
        {

            DataTable dtlogs = new DataTable();
            dtlogs.Columns.Add("link_id", typeof(string));
            dtlogs.Columns.Add("link_name", typeof(string));
            dtlogs.Columns.Add("link_type", typeof(string));
            dtlogs.Columns.Add("main_link_type", typeof(string));
            dtlogs.Columns.Add("main_link_id", typeof(string));
            dtlogs.Columns.Add("redundant_link_id", typeof(string));
            dtlogs.Columns.Add("redundant_link_type", typeof(string));
            dtlogs.Columns.Add("Error Msg", typeof(string));
            dtlogs.TableName = "FiberLogs";
            int userId = Convert.ToInt32(Session["user_id"]);
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                List<TempFiberLink> BulkUploadLogs = BLTempFiberLink.Instance.GetUploadFiberLogs(userId);

                if (BulkUploadLogs.Count() > 0)
                {
                    foreach (var l in BulkUploadLogs)
                    {
                        {
                            dtlogs.Rows.Add(l.link_id.ToString(), l.link_name.ToString(), l.link_type.ToString(), l.main_link_type.ToString(), l.main_link_id.ToString(), l.redundant_link_id.ToString(), l.redundant_link_type.ToString(), l.error_msg);
                        }
                    }

                    IWorkbook workbook = SmartInventory.Helper.NPOIExcelHelper.DataTableToExcel("xlsx", dtlogs);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", AppendTimeStamp("Fiberlogs.xlsx")));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }
        
        public JsonResult deleteFiberLinkById(int system_id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {


                var result = new BLFiberLink().deleteFiberLinkById(system_id);
                if (result > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_116;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_117;
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_117;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult validateLinkIdByText(string searchText, string columnName)
        {
            JsonResponse<Dictionary<string, string>> objResp = new JsonResponse<Dictionary<string, string>>();
            try
            {


                var fiberLinkDetails = new BLMisc().getFiberLinkDetails(searchText, columnName);
                if (fiberLinkDetails.Count > 0)
                {
                    objResp.result = fiberLinkDetails;
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
                }

            }
            catch (Exception)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_118;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetFiberLinkCustomer(FiberLinkCustomerFilter objFiberLinkCustomerFilter, int LinkId = 0, int page = 0, string sort = "", string sortdir = "")
        {
            int _LinkId = 0;
            if (LinkId > 0)
            {
                Session["link_id"] = LinkId;
                _LinkId = Convert.ToInt32(Session["link_id"]);
            }
            else
            {
                _LinkId = Convert.ToInt32(Session["link_id"]);
            }
            if (_LinkId > 0)
            {
                int user_id = Convert.ToInt32(Session["user_id"]);
                objFiberLinkCustomerFilter.pageSize = 10;
                objFiberLinkCustomerFilter.userid = user_id;
                objFiberLinkCustomerFilter.currentPage = page == 0 ? 1 : page;
                objFiberLinkCustomerFilter.sort = sort;
                objFiberLinkCustomerFilter.orderBy = sortdir;
                objFiberLinkCustomerFilter.userid = user_id;
                objFiberLinkCustomerFilter.link_system_id = _LinkId;
                List<Dictionary<string, string>> lstFiberLinkDetails = new BLFiberLink().getAssociationCustomer(objFiberLinkCustomerFilter);

                foreach (Dictionary<string, string> dic in lstFiberLinkDetails)
                {
                    var obj = (IDictionary<string, object>)new ExpandoObject();
                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                    foreach (var col in dic)
                    {
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                        {
                            //obj.Add(col.Key, col.Value);
                            if (col.Key.ToUpper() == "CAN_ID")
                            {
                                obj.Add("Can Id", col.Value);
                            }
                            else if (col.Key.ToUpper() == "CUSTOMER_NAME")
                            {
                                // obj.Add("Customer Name", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_GBL_GBL_GBL_089, col.Value);
                            }
                            else if (col.Key.ToUpper() == "CUSTOMER_TYPE")
                            {
                                //obj.Add("Customer Type", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_CUS_NET_GBL_001, col.Value);
                            }
                            else if (col.Key.ToUpper() == "LMC_TYPE")
                            {
                                //obj.Add("LMC Type", col.Value);
                                obj.Add("LMC Type", col.Value);
                            }
                            else if (col.Key.ToUpper() == "ADDRESS")
                            {
                                //obj.Add("Address", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_GBL_GBL_GBL_008, col.Value);
                            }
                            else if (col.Key.ToUpper() == "REGION_NAME")
                            {
                                //obj.Add("Region Name", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_GBL_GBL_GBL_002, col.Value);
                            }
                            else if (col.Key.ToUpper() == "PROVINCE_NAME")
                            {
                                //obj.Add("Province Name", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_GBL_GBL_GBL_003, col.Value);
                            }
                            else if (col.Key.ToUpper() == "CREATED_ON")
                            {
                                //obj.Add("Created On", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_GBL_GBL_GBL_055, col.Value);
                            }
                            else if (col.Key.ToUpper() == "CREATED_BY")
                            {
                                //obj.Add("Created By", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_GBL_GBL_GBL_056, col.Value);
                            }
                            else if (col.Key.ToUpper() == "EMAIL_ID")
                            {
                                //obj.Add("Created By", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_STR_NET_FRM_055, col.Value);
                            }
                            else if (col.Key.ToUpper() == "MOBILE_NO")
                            {
                                //obj.Add("Created By", col.Value);
                                obj.Add(Resources.Resources.SI_OSP_STR_NET_FRM_053, col.Value);
                            }
                            else if (col.Key.ToUpper() == "PHONE_NO")
                            {
                                //obj.Add("Created By", col.Value);
                                obj.Add("Phone Number", col.Value);
                            }

                        }
                    }
                    objFiberLinkCustomerFilter.lstFiberLinkCustomer.Add(obj);
                }

                objFiberLinkCustomerFilter.totalRecord = lstFiberLinkDetails.Count > 0 ? Convert.ToInt32(lstFiberLinkDetails[0].FirstOrDefault().Value) : 0;
            }
            return PartialView("_AssociateCustomer", objFiberLinkCustomerFilter);
        }
        public ActionResult AssociateCustomer(FiberLink objFiberLink)
        {
            ModelState.Clear();
            if (TryValidateModel(objFiberLink))
            {

                var result = new BLFiberLink().SaveFiberLink(objFiberLink, Convert.ToInt32(Session["user_id"]));
                if (string.IsNullOrEmpty(result.pageMsg.message))
                {
                    if (objFiberLink.pageMsg.isNewEntity)
                    {
                        objFiberLink.pageMsg.message = Resources.Resources.SI_GBL_GBL_NET_FRM_119;
                        objFiberLink.pageMsg.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        objFiberLink.pageMsg.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_029;
                        objFiberLink.pageMsg.status = ResponseStatus.OK.ToString();
                    }
                }
                else
                {
                    objFiberLink.pageMsg.status = ResponseStatus.FAILED.ToString();
                    objFiberLink.pageMsg.message = Resources.Resources.SI_GBL_GBL_NET_FRM_120;
                }
            }
            else
            {
                objFiberLink.pageMsg.status = ResponseStatus.FAILED.ToString();
                objFiberLink.pageMsg.message = Resources.Resources.SI_GBL_GBL_NET_FRM_120;
            }
            return Json(objFiberLink, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetStartEndPointNetworkId(string SearchText)
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
                            lstSearchResult = objBLSearch.GetSearchEntityResult(arrSrchText[0], arrSrchText[1], usrDetail.user_id, "network_id").Take(5).ToList();
                        }
                        else if (arrSrchText[1].Length >= ApplicationSettings.EntitySearchLength)
                        {
                            lstSearchResult = objBLSearch.GetSearchEntityResult(arrSrchText[0], arrSrchText[1], usrDetail.user_id, "network_id").Take(5).ToList();
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
        public JsonResult validateFiberLinkPointNetworkId(string networkId, string columnName)
        {
            JsonResponse<Dictionary<string, string>> objResp = new JsonResponse<Dictionary<string, string>>();
            var fiberLinkDetails = new BLNetworkStatus().isNetworkIdExist(networkId, columnName, Convert.ToInt32(Session["user_id"]));
            if (fiberLinkDetails)
            {
                return Json(new { strReturn = "", msg = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { strReturn = "", msg = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        public void ExportFiberLinkById(string ReportType, int SystemId)
        {
            if (!string.IsNullOrEmpty(ReportType) && SystemId > 0)
            {
                if (ReportType == "EXCEL")
                {
                    FiberLinkFilter objFiberLinkFilter = (FiberLinkFilter)Session["viewFiberLinkDashboard"];
                    objFiberLinkFilter.currentPage = 0;
                    objFiberLinkFilter.pageSize = 0;
                    objFiberLinkFilter.system_id = SystemId;
                    FiberLinkExcelExport(objFiberLinkFilter);
                }
            }
        }
        public void ExportFiberLink()
        {
            if (Session["viewFiberLinkDashboard"] != null)
            {
                FiberLinkFilter objFiberLinkFilter = (FiberLinkFilter)Session["viewFiberLinkDashboard"];
                objFiberLinkFilter.currentPage = 0;
                objFiberLinkFilter.pageSize = 0;
                objFiberLinkFilter.system_id = 0;
                FiberLinkExcelExport(objFiberLinkFilter);
            }
        }

        public void ExportFiberLinkCustomer()
        {
            FiberLinkCustomerFilter objFiberLinkCustomerFilter = new FiberLinkCustomerFilter();
            int LinkId = Convert.ToInt32(Session["link_id"]);
            DataSet ds = new DataSet();
            if (LinkId > 0)
            {
                int user_id = Convert.ToInt32(Session["user_id"]);
                objFiberLinkCustomerFilter.userid = user_id;
                objFiberLinkCustomerFilter.link_system_id = LinkId;
                List<Dictionary<string, string>> lstFiberLinkDetails = new BLFiberLink().getAssociationCustomer(objFiberLinkCustomerFilter);
                foreach (Dictionary<string, string> dic in lstFiberLinkDetails)
                {
                    var obj = (IDictionary<string, object>)new ExpandoObject();
                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                    foreach (var col in dic)
                    {
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                        {
                            obj.Add(col.Key, col.Value);
                        }

                        objFiberLinkCustomerFilter.lstFiberLinkCustomer.Add(obj);
                    }
                }

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(lstFiberLinkDetails);
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("s_no")) { dtReport.Columns.Remove("s_no"); }
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("can_id")) { dtReport.Columns["can_id"].ColumnName = "CAN Id"; }
                    if (dtReport.Columns.Contains("customer_name")) { dtReport.Columns["customer_name"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_089; }
                    if (dtReport.Columns.Contains("customer_type")) { dtReport.Columns["customer_type"].ColumnName = Resources.Resources.SI_OSP_CUS_NET_GBL_001; }
                    if (dtReport.Columns.Contains("lmc_type")) { dtReport.Columns["lmc_type"].ColumnName = "LMC Type"; }
                    if (dtReport.Columns.Contains("address")) { dtReport.Columns["address"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_008; }
                    if (dtReport.Columns.Contains("region_name")) { dtReport.Columns["region_name"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_002; }
                    if (dtReport.Columns.Contains("province_name")) { dtReport.Columns["province_name"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_003; }
                    if (dtReport.Columns.Contains("created_on")) { dtReport.Columns["created_on"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_055; }
                    if (dtReport.Columns.Contains("created_by")) { dtReport.Columns["created_by"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_056; }
                    if (dtReport.Columns.Contains("email_id")) { dtReport.Columns["email_id"].ColumnName = Resources.Resources.SI_OSP_STR_NET_FRM_055; }
                    if (dtReport.Columns.Contains("mobile_no")) { dtReport.Columns["mobile_no"].ColumnName = Resources.Resources.SI_OSP_STR_NET_FRM_053; }
                    if (dtReport.Columns.Contains("phone_no")) { dtReport.Columns["phone_no"].ColumnName = "Phone Number"; }
                }

                ds.Tables.Add(dtReport);
                ds.Tables[0].TableName = "Fiber Link Customer";
            }

            var filename = "FiberLinkCustomer_report";
            ExportData(ds, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }
        public void FiberLinkExcelExport(FiberLinkFilter objFiberLinkFilter)
        {

            List<Dictionary<string, string>> lstFiberLinkDetails = new BLFiberLink().getFiberLinkDetails(Convert.ToInt32(Session["user_id"]), objFiberLinkFilter);

            lstFiberLinkDetails = BLConvertMLanguage.ExportMultilingualConvert(lstFiberLinkDetails);
            DataTable dtReport = new DataTable();
            dtReport = MiscHelper.GetDataTableFromDictionaries(lstFiberLinkDetails);
            dtReport.Columns.Remove("s_no");
            dtReport.Columns.Remove("system_id");
            dtReport.Columns.Remove("totalrecords");
            DataSet ds = new DataSet();
            ds.Tables.Add(dtReport);
            ds.Tables[0].TableName = "Fiber Link";


            //-------------Export Cable Info--------------
            var lstFiberLinkIds = objFiberLinkFilter.lstFiberLinkDetails.Select(x => x.system_id.ToString()).ToList();
            var strFiberLinkIds = String.Join(",", lstFiberLinkIds);
            List<Dictionary<string, string>> info = new BLFiberLink().getExportCableInfoByLinkSystemIds(strFiberLinkIds);
            if (objFiberLinkFilter.system_id == 0)
            {
                DataTable dtReport2 = new DataTable();
                dtReport2 = MiscHelper.GetDataTableFromDictionaries(info);
                if (dtReport2 != null && dtReport2.Rows.Count > 0)
                {
                    if (dtReport2.Columns.Contains("cable_network_id")) { dtReport2.Columns["cable_network_id"].ColumnName = "Cable Network Id"; }
                    if (dtReport2.Columns.Contains("cable_name")) { dtReport2.Columns["cable_name"].ColumnName = "Cable Name"; }
                    if (dtReport2.Columns.Contains("total_core")) { dtReport2.Columns["total_core"].ColumnName = "Total Core"; }
                    if (dtReport2.Columns.Contains("no_of_tube")) { dtReport2.Columns["no_of_tube"].ColumnName = "No. Of Tube"; }
                    if (dtReport2.Columns.Contains("a_location")) { dtReport2.Columns["a_location"].ColumnName = "A Location"; }
                    if (dtReport2.Columns.Contains("b_location")) { dtReport2.Columns["b_location"].ColumnName = "B Location"; }
                    if (dtReport2.Columns.Contains("fiber_number")) { dtReport2.Columns["fiber_number"].ColumnName = "Associated Fiber Number"; }
                    if (dtReport2.Columns.Contains("link_network_id")) { dtReport2.Columns["link_network_id"].ColumnName = "Link Network Id"; }
                    if (dtReport2.Columns.Contains("Link_id")) { dtReport2.Columns["Link_id"].ColumnName = "Link/Route Id"; }
                }
                ds.Tables.Add(dtReport2);
                ds.Tables[1].TableName = "Cable Information";
            }
            //------------------END;
            if (objFiberLinkFilter.system_id > 0)
            {
                DataTable dtReport1 = new DataTable();

                List<Dictionary<string, string>> data = new BLFiberLink().getExportCableInfoByLinkId(objFiberLinkFilter.system_id);
                if (data.Count > 0)
                {
                    foreach (Dictionary<string, string> dic in data)
                    {
                        var obj = (IDictionary<string, object>)new ExpandoObject();
                        foreach (var col in dic)
                        {
                            obj.Add(col.Key, col.Value);
                        }
                    }

                    dtReport1 = MiscHelper.GetDataTableFromDictionaries(data);

                    if (dtReport1 != null && dtReport1.Rows.Count > 0)
                    {
                        if (dtReport1.Columns.Contains("network_id")) { dtReport1.Columns["network_id"].ColumnName = "Network Id"; }
                        if (dtReport1.Columns.Contains("cable_name")) { dtReport1.Columns["cable_name"].ColumnName = "Cable Name"; }
                        if (dtReport1.Columns.Contains("total_core")) { dtReport1.Columns["total_core"].ColumnName = "Total Core"; }
                        if (dtReport1.Columns.Contains("no_of_tube")) { dtReport1.Columns["no_of_tube"].ColumnName = "No. Of Tube"; }
                        if (dtReport1.Columns.Contains("a_location")) { dtReport1.Columns["a_location"].ColumnName = "A Location"; }
                        if (dtReport1.Columns.Contains("b_location")) { dtReport1.Columns["b_location"].ColumnName = "B Location"; }
                        if (dtReport1.Columns.Contains("fiber_number")) { dtReport1.Columns["fiber_number"].ColumnName = "Associated Fiber Number"; }
                        if (dtReport1.Columns.Contains("cable_measured_length")) { dtReport1.Columns["cable_measured_length"].ColumnName = "Cable Measured Length"; }
                        if (dtReport1.Columns.Contains("cable_calculated_length")) { dtReport1.Columns["cable_calculated_length"].ColumnName = "Cable Calculated Length"; }
                    }
                    ds.Tables.Add(dtReport1);
                    ds.Tables[1].TableName = "Cable Information";
                }
            }

            var filename = "FiberLink_report";
            ExportData(ds, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyyHHmmss"));
        }
        private void ExportData(DataSet dsReport, string fileName, bool isDataContainBarcode = false)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DatasetToExcel("xlsx", dsReport, isDataContainBarcode);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }

        public JsonResult validateFiberlinkId(string linkId, string columnName)
        {
            FiberLink objfiberLink = new FiberLink();
            objfiberLink = new BLFiberLink().isFiberLinkIdExist(linkId, columnName, Convert.ToInt32(Session["user_id"]));
            return Json(new { objfiberLink }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssociateFiberLinkWithCable(int link_system_id, int cable_id, int fiber_no, string action)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var resObj = new BLMisc().AssociateFiberLinkWithCable(link_system_id, cable_id, fiber_no, action);
                if (resObj.status)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = resObj.message;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = resObj.message;
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_122;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult showFiberLinkOnMap(int linkSystemId)
        {
            JsonResponse<vmfiberLinkOnMap> objResp = new JsonResponse<vmfiberLinkOnMap>();
            try
            {
                objResp.result = new BLFiberLink().getFiberLinkElements(linkSystemId, Convert.ToInt32(Session["user_id"]));


                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {

                ErrorLogHelper.WriteErrorLog("showFiberLinkOnMap()", "ShowLinkOnMap", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Error in showFiberLinkOnMap()!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        public void ExportFiberLinkIntoKML(int SystemId, string ReportType)
        {
            JsonResponse<vmfiberLinkOnMap> objResp = new JsonResponse<vmfiberLinkOnMap>();
            Dictionary<string, string> LinkDetails = new Dictionary<string, string>();
            try
            {
                objResp.result = new BLFiberLink().getFiberLinkElements(SystemId, Convert.ToInt32(Session["user_id"]));
                LinkDetails = new BLFiberLink().getLinkInfoForKML(SystemId);
                List<Dictionary<string, string>> obj = new List<Dictionary<string, string>>();
                obj.Add(LinkDetails);
                DataSet ds = new DataSet();
                DataTable dtReport = new DataTable();
                DataTable dtReport1 = new DataTable();
                DataTable dtReport2 = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(obj);
                dtReport.TableName = "LinkDetails";
                dtReport1 = MiscHelper.ListToDataTable<cableInfo>(objResp.result.lstCableInfo);
                dtReport2 = MiscHelper.ListToDataTable<connectedElements>(objResp.result.lstConnectedElements);
                if (dtReport.Rows.Count > 0)
                    ds.Tables.Add(dtReport);
                if (dtReport1.Rows.Count > 0)
                    ds.Tables.Add(dtReport1);
                if (dtReport2.Rows.Count > 0)
                    ds.Tables.Add(dtReport2);
                KMLHelper.GetKmlForFiberLikEntities(ds, ApplicationSettings.DownloadTempPath);
            }
            catch (Exception ex)
            {

                ErrorLogHelper.WriteErrorLog("ExportFiberLinkIntoKML()", "Fiber Link Connection KML", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_125;
            }
        }
        public ActionResult GetAssociateLink(int cable_id, int fiber_no)
        {
            fiberLinkAssociation objfiberLinkAssociation = new fiberLinkAssociation();
            objfiberLinkAssociation = new BLFiberLink().getAssociatedLinkId(cable_id, fiber_no);
            return PartialView("_AssociateLink", objfiberLinkAssociation);
        }
        public string getFirstErrorFromModelState()
        {
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    if (error.ErrorMessage != "")
                        return error.ErrorMessage;
                }
            }
            return "";
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

        public void DownloadFiberLinkIntoKML()
        {
            JsonResponse<vmfiberLinkOnMap> objResp = new JsonResponse<vmfiberLinkOnMap>();
            FiberLinkFilter objFiberLinkFilter = (FiberLinkFilter)Session["viewFiberLinkDashboard"];
            try
            {
                //-- GET ALL FIBER LINK LIST
                var objUser = (User)Session["userDetail"];
                objFiberLinkFilter.pageSize = 0;
                objFiberLinkFilter.currentPage = 0;
                objFiberLinkFilter.lstAllFiberLinkDetails = new BLFiberLink().getAssociatedFiberLinkDetails(objUser.user_id, objFiberLinkFilter);
                KMLHelper.GetKMLForAllFiberLinkEntities(objFiberLinkFilter.lstAllFiberLinkDetails, ApplicationSettings.DownloadTempPath, Convert.ToInt32(Session["user_id"]));
            }
            catch (Exception ex)
            {

                ErrorLogHelper.WriteErrorLog("ExportFiberLinkIntoKML()", "Fiber Link Connection KML", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_125;
            }
        }
        public PartialViewResult CreateFiberLink(int system_id = 0,string link_id="")
        {
            FiberLink objFiberLink = new FiberLink();
            objFiberLink.link_id= link_id;
            objFiberLink.CreateFL = 1;           
            BindFiberLinkDropDown(objFiberLink);
            return PartialView("_CreateFiberLink", objFiberLink);
        }
        public void ExportFiberLinkInPDF()
        {

            if (Session["viewFiberLinkDashboard"] != null)
            {
                FiberLinkFilter objFiberLinkFilter = (FiberLinkFilter)Session["viewFiberLinkDashboard"];
                objFiberLinkFilter.currentPage = 0;
                objFiberLinkFilter.pageSize = 0;
                objFiberLinkFilter.system_id = 0;
                List<Dictionary<string, string>> lstFiberLinkDetails = new BLFiberLink().getFiberLinkDetails(Convert.ToInt32(Session["user_id"]), objFiberLinkFilter);

                lstFiberLinkDetails = BLConvertMLanguage.ExportMultilingualConvert(lstFiberLinkDetails);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(lstFiberLinkDetails);
                if (dtReport.Columns.Contains("s_no")) { dtReport.Columns.Remove("s_no"); }
                if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                if (dtReport.Columns.Contains("Start Point Type")) { dtReport.Columns.Remove("Start Point Type"); }
                if (dtReport.Columns.Contains("Start Point Network ID")) { dtReport.Columns.Remove("Start Point Network ID"); }
                if (dtReport.Columns.Contains("End Point Type")) { dtReport.Columns.Remove("End Point Type"); }
                if (dtReport.Columns.Contains("End Point Network ID")) { dtReport.Columns.Remove("End Point Network ID"); }
                if (dtReport.Columns.Contains("No. Of LMC")) { dtReport.Columns.Remove("No. Of LMC"); }
                if (dtReport.Columns.Contains("Each LMC Length(meter)")) { dtReport.Columns.Remove("Each LMC Length(meter)"); }
                if (dtReport.Columns.Contains("OTDR Distance(meter)")) { dtReport.Columns.Remove("OTDR Distance(meter)"); }
                if (dtReport.Columns.Contains("No. Of Pairs")) { dtReport.Columns.Remove("No. Of Pairs"); }
                if (dtReport.Columns.Contains("Tube And Core Details")) { dtReport.Columns.Remove("Tube And Core Details"); }
                if (dtReport.Columns.Contains("Existing Route Length(OTDR)")) { dtReport.Columns.Remove("Existing Route Length(OTDR)"); }
                if (dtReport.Columns.Contains("New Build Route Length(meter)")) { dtReport.Columns.Remove("New Build Route Length(meter)"); }
                if (dtReport.Columns.Contains("OTL Length(meter)")) { dtReport.Columns.Remove("OTL Length(meter)"); }
                if (dtReport.Columns.Contains("OTM Length(meter)")) { dtReport.Columns.Remove("OTM Length(meter)"); }
                if (dtReport.Columns.Contains("Any Row Portion")) { dtReport.Columns.Remove("Any Row Portion"); }
                if (dtReport.Columns.Contains("ROW Authority")) { dtReport.Columns.Remove("ROW Authority"); }
                if (dtReport.Columns.Contains("Total ROW Segments")) { dtReport.Columns.Remove("Total ROW Segments"); }
                if (dtReport.Columns.Contains("Total ROW Length(meter)")) { dtReport.Columns.Remove("Total ROW Length(meter)"); }
                if (dtReport.Columns.Contains("Total ROW Recurring Charges/Annum(Rs.)")) { dtReport.Columns.Remove("Total ROW Recurring Charges/Annum(Rs.)"); }
                if (dtReport.Columns.Contains("Handover Date")) { dtReport.Columns.Remove("Handover Date"); }
                if (dtReport.Columns.Contains("Hoto Signoff Date")) { dtReport.Columns.Remove("Hoto Signoff Date"); }
                if (dtReport.Columns.Contains("Modified By")) { dtReport.Columns.Remove("Modified By"); }
                if (dtReport.Columns.Contains("Modified On\t")) { dtReport.Columns.Remove("Modified On\t"); }
                if (dtReport.Columns.Contains("Service ID")) { dtReport.Columns.Remove("Service ID"); }

                DataSet ds = new DataSet();
                ds.Tables.Add(dtReport);
                ds.Tables[0].TableName = "Fiber Link";


                //-------------Export Cable Info--------------
                var lstFiberLinkIds = objFiberLinkFilter.lstFiberLinkDetails.Select(x => x.system_id.ToString()).ToList();
                var strFiberLinkIds = String.Join(",", lstFiberLinkIds);
                List<Dictionary<string, string>> info = new BLFiberLink().getExportCableInfoByLinkSystemIds(strFiberLinkIds);
                if (objFiberLinkFilter.system_id == 0)
                {
                    DataTable dtReport2 = new DataTable();
                    dtReport2 = MiscHelper.GetDataTableFromDictionaries(info);
                    if (dtReport2 != null && dtReport2.Rows.Count > 0)
                    {
                        if (dtReport2.Columns.Contains("cable_network_id")) { dtReport2.Columns["cable_network_id"].ColumnName = "Cable Network Id"; }
                        if (dtReport2.Columns.Contains("cable_name")) { dtReport2.Columns["cable_name"].ColumnName = "Cable Name"; }
                        if (dtReport2.Columns.Contains("total_core")) { dtReport2.Columns["total_core"].ColumnName = "Total Core"; }
                        if (dtReport2.Columns.Contains("no_of_tube")) { dtReport2.Columns["no_of_tube"].ColumnName = "No. Of Tube"; }
                        if (dtReport2.Columns.Contains("a_location")) { dtReport2.Columns["a_location"].ColumnName = "A Location"; }
                        if (dtReport2.Columns.Contains("b_location")) { dtReport2.Columns["b_location"].ColumnName = "B Location"; }
                        if (dtReport2.Columns.Contains("fiber_number")) { dtReport2.Columns["fiber_number"].ColumnName = "Associated Fiber Number"; }
                        if (dtReport2.Columns.Contains("link_network_id")) { dtReport2.Columns["link_network_id"].ColumnName = "Link Network Id"; }
                        if (dtReport2.Columns.Contains("Link_id")) { dtReport2.Columns["Link_id"].ColumnName = "Link/Route Id"; }
                    }
                    ds.Tables.Add(dtReport2);
                    ds.Tables[1].TableName = "Cable Information";
                }
                //------------------END;
                if (objFiberLinkFilter.system_id > 0)
                {
                    DataTable dtReport1 = new DataTable();

                    List<Dictionary<string, string>> data = new BLFiberLink().getExportCableInfoByLinkId(objFiberLinkFilter.system_id);
                    if (data.Count > 0)
                    {
                        foreach (Dictionary<string, string> dic in data)
                        {
                            var obj = (IDictionary<string, object>)new ExpandoObject();
                            foreach (var col in dic)
                            {
                                obj.Add(col.Key, col.Value);
                            }
                        }

                        dtReport1 = MiscHelper.GetDataTableFromDictionaries(data);

                        if (dtReport1 != null && dtReport1.Rows.Count > 0)
                        {
                            if (dtReport1.Columns.Contains("network_id")) { dtReport1.Columns["network_id"].ColumnName = "Network Id"; }
                            if (dtReport1.Columns.Contains("cable_name")) { dtReport1.Columns["cable_name"].ColumnName = "Cable Name"; }
                            if (dtReport1.Columns.Contains("total_core")) { dtReport1.Columns["total_core"].ColumnName = "Total Core"; }
                            if (dtReport1.Columns.Contains("no_of_tube")) { dtReport1.Columns["no_of_tube"].ColumnName = "No. Of Tube"; }
                            if (dtReport1.Columns.Contains("a_location")) { dtReport1.Columns["a_location"].ColumnName = "A Location"; }
                            if (dtReport1.Columns.Contains("b_location")) { dtReport1.Columns["b_location"].ColumnName = "B Location"; }
                            if (dtReport1.Columns.Contains("fiber_number")) { dtReport1.Columns["fiber_number"].ColumnName = "Associated Fiber Number"; }
                        }
                        ds.Tables.Add(dtReport1);
                        ds.Tables[1].TableName = "Cable Information";
                    }
                }

                GenerateToPDF(ds, "FiberLink_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"), Resources.Resources.SI_OSP_GBL_NET_RPT_127);

            }

        }
        //private void GenerateToPDF(DataSet ds, string Name, string title)
        //{
        //    iTextSharp.text.Font _font = new iTextSharp.text.Font(PDFHelper.GetFont(), 10, iTextSharp.text.Font.BOLD);
        //    iTextSharp.text.Font font = new iTextSharp.text.Font(PDFHelper.GetFont(), 6, iTextSharp.text.Font.NORMAL);
        //    Paragraph EntityHeading1 = new Paragraph(new Chunk("Fiber Link", _font));
        //    EntityHeading1.Alignment = Element.ALIGN_CENTER;
        //    EntityHeading1.SpacingAfter = 20;

        //    PdfPTable table = new PdfPTable(ds.Tables[0].Columns.Count);
        //    table.WidthPercentage = 100f;
        //    PdfPCell cell = new PdfPCell();
        //    foreach (DataColumn column in ds.Tables[0].Columns)
        //    {
        //        cell = new PdfPCell(new Phrase("" + column.ColumnName + "", font));
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.BackgroundColor = new BaseColor(0, 186, 138);
        //        cell.FixedHeight = 70f;
        //        table.AddCell(cell);
        //    }
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        int cnt = 0;
        //        foreach (DataColumn column in ds.Tables[0].Columns)
        //        {
        //            if (row[0] == Resources.Resources.SI_OSP_GBL_GBL_GBL_041)
        //            {
        //                cell = new PdfPCell(new Phrase("" + row[column] + "", font));
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.BackgroundColor = new BaseColor(0, 186, 138);
        //                cell.FixedHeight = 70f;
        //                table.AddCell(cell);
        //            }
        //            else
        //            {
        //                cell = new PdfPCell(new Phrase("" + row[column] + "", font));
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.FixedHeight = 70f;
        //                table.AddCell(cell);
        //            }
        //            cnt++;
        //        }
        //    }
        //    //Table 1
        //    PdfPTable table1 = new PdfPTable(ds.Tables[1].Columns.Count);
        //    table1.WidthPercentage = 100f;//90f;
        //    Paragraph EntityHeading = new Paragraph(new Chunk("Cable Information", _font));
        //    EntityHeading.Alignment = Element.ALIGN_CENTER;
        //    EntityHeading.SpacingAfter = 20;
        //    PdfPCell cell1 = new PdfPCell();
        //    foreach (DataColumn column in ds.Tables[1].Columns)
        //    {
        //            cell1 = new PdfPCell(new Phrase("" + column.ColumnName + "", font));
        //            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell1.BackgroundColor = new BaseColor(0, 186, 138);
        //            cell1.FixedHeight = 45f;
        //            table1.AddCell(cell1);
        //    }
        //    foreach (DataRow row in ds.Tables[1].Rows)
        //    {
        //        int cnt = 0;
        //        foreach (DataColumn column in ds.Tables[1].Columns)
        //        {
        //                if (row[0] == Resources.Resources.SI_OSP_GBL_GBL_GBL_041)
        //                {
        //                    cell1 = new PdfPCell(new Phrase("" + row[column] + "", font));
        //                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell1.BackgroundColor = new BaseColor(0, 186, 138);
        //                    cell1.FixedHeight = 25f;
        //                    table1.AddCell(cell1);
        //                }
        //                else
        //                {
        //                    cell1 = new PdfPCell(new Phrase("" + row[column] + "", font));
        //                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell1.FixedHeight = 35f;
        //                    table1.AddCell(cell1);
        //                }
        //                cnt++;
        //        }
        //    }
        //    Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 25f, 30f);
        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    writer.PageEvent = new PDFHelper.AllPdfPageEvents();
        //    pdfDoc.Open();
        //    pdfDoc.Add(EntityHeading1);
        //    pdfDoc.Add(table);
        //    pdfDoc.Add(EntityHeading);
        //    pdfDoc.Add(table1);
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + Name + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();
        //}
        private void GenerateToPDF(DataSet ds, string fileName, string title)
        {
            iTextSharp.text.Font _font = new iTextSharp.text.Font(PDFHelper.GetFont(), 10, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font font = new iTextSharp.text.Font(PDFHelper.GetFont(), 6, iTextSharp.text.Font.NORMAL);

            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 20f, 20f);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            writer.PageEvent = new PDFHelper.AllPdfPageEvents();

            pdfDoc.Open();

            // Add Title
            Paragraph titleParagraph = new Paragraph(new Chunk(title, _font));
            titleParagraph.Alignment = Element.ALIGN_CENTER;
            titleParagraph.SpacingAfter = 20;
            pdfDoc.Add(titleParagraph);
            PdfPCell cell = new PdfPCell();

            // Loop through each table in the DataSet
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable table = ds.Tables[i];
                if (table.Columns.Count > 0)
                {
                    // Add table title
                    Paragraph tableTitle = new Paragraph(new Chunk($"Table {i + 1}: {table.TableName}", _font));
                    tableTitle.Alignment = Element.ALIGN_CENTER;
                    tableTitle.SpacingAfter = 10;
                    pdfDoc.Add(tableTitle);

                    // Create PDF Table

                    PdfPTable pdfTable = new PdfPTable(table.Columns.Count);
                    pdfTable.WidthPercentage = 100f;

                    // Add column headers
                    foreach (DataColumn column in table.Columns)
                    {
                        cell = new PdfPCell(new Phrase(column.ColumnName, font));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.BackgroundColor = new BaseColor(0, 186, 138);
                        cell.FixedHeight = 25f;
                        pdfTable.AddCell(cell);
                    }

                    // Add rows
                    foreach (DataRow row in table.Rows)
                    {
                        int cnt = 0;
                        foreach (DataColumn column in ds.Tables[i].Columns)
                        {
                            if (row[0] == Resources.Resources.SI_OSP_GBL_GBL_GBL_041)
                            {
                                cell = new PdfPCell(new Phrase("" + row[column] + "", font));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.BackgroundColor = new BaseColor(0, 186, 138);
                                cell.FixedHeight = 25f;
                                pdfTable.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Phrase("" + row[column] + "", font));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.FixedHeight = 35f;
                                pdfTable.AddCell(cell);
                            }
                            cnt++;
                        }
                        //foreach (DataColumn column in table.Columns)
                        //{

                        //    PdfPCell cell = new PdfPCell(new Phrase(row[column].ToString(), font));
                        //    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        //    cell.FixedHeight = 25f;
                        //    pdfTable.AddCell(cell);
                        //}
                    }

                    // Add table to PDF document
                    pdfDoc.Add(pdfTable);
                    pdfDoc.Add(new Paragraph("\n")); // Add spacing between tables
                }
            }

            pdfDoc.Close();

            // Return PDF as response
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }


    }
}