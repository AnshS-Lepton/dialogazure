using BusinessLogics;
using Models;
using Models.WFM;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Utility;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class TicketManagerController : Controller
    {
        // GET: TicketManager
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowTicketManager(TicketManagerFilter objTicketManagerFilter, int page = 0, string sort = "", string sortdir = "")
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
           
            objTicketManagerFilter.pageSize = 10;
            objTicketManagerFilter.userid = user_id;
            objTicketManagerFilter.currentPage = page == 0 ? 1 : page;
            objTicketManagerFilter.sort = sort;
            objTicketManagerFilter.orderBy = sortdir;
            var jsonSerialiser = new JavaScriptSerializer();
            var lstTicketSatatus = jsonSerialiser.Serialize(objTicketManagerFilter.objticketstatus);
             var lstTicketType = jsonSerialiser.Serialize(objTicketManagerFilter.objTicketMaster.lstTicketTypeMaster);
            User  objUser = new BLUser().GetUserDetailByID(user_id);
            if (objUser!=null)
            {
                objTicketManagerFilter.objTicketMaster.user_role_id = objUser.role_id;
            }
            //objTicketManagerFilter.lstTicketMaster = new BLTicketManager().getTicketList(Convert.ToInt32(Session["user_id"]), objTicketManagerFilter, lstTicketSatatus, lstTicketType);
            objTicketManagerFilter.lstTicketMaster = new BLTicketManager().getTicketList(Convert.ToInt32(Session["user_id"]), objTicketManagerFilter);
            objTicketManagerFilter.totalRecord = objTicketManagerFilter != null && objTicketManagerFilter.lstTicketMaster.Count > 0 ? objTicketManagerFilter.lstTicketMaster[0].totalrecords : 0;
            //objTicketMasterVM.objTicketManagerFilter.totalRecord = (objTicketMasterVM.lstTicketMaster[0].totalrecords);
            objTicketManagerFilter.objticketstatus = new BLTicketManager().getTicketStatusCounts(Convert.ToInt32(Session["user_id"]));
            Session["viewTicketDashboardFilter"] = objTicketManagerFilter;
            BindTicketDropDown(objTicketManagerFilter.objTicketMaster); 
            return PartialView("_Index", objTicketManagerFilter);
        }

        public PartialViewResult AddTicket(int ticket_id = 0) 
        {
            TicketMaster objTicketMaster = new TicketMaster();
            int user_id = Convert.ToInt32(Session["user_id"]);
            if (ticket_id > 0)
            {
                User objUser = new BLUser().GetUserDetailByID(user_id);
               
                objTicketMaster = new BLTicketManager().GetTicketById(ticket_id);
                objTicketMaster.IsCustomerExist = new BLCustomer().IsCustomerCodeExists(objTicketMaster.can_id);
                var objBuilding = BLBuilding.Instance.GetBuildingByCode(objTicketMaster.building_code);
                if (objUser != null)
                {
                    objTicketMaster.user_role_id = objUser.role_id;
                }
                if (objBuilding.network_id!=null)
                {
                    objTicketMaster.IsBuildingExist = true;  
                }

            }
            BindTicketDropDown(objTicketMaster);
            return PartialView("_AddTicket", objTicketMaster);
        }
        public void BindTicketDropDown(TicketMaster objTicketMaster)
        {
            var ddlTicketType = new BLTicketManager().GetTicketType();
            ddlTicketType = ddlTicketType.Where(x => x.module.ToUpper() == "CUSTOMER").ToList();
            objTicketMaster.lstTicketTypeMaster.AddRange(ddlTicketType);

            var ddlRefType = new BLMisc().GetTicketDropdownList(DropDownType.Reference_Type.ToString());
            objTicketMaster.lstReferenceType = ddlRefType.Where(x => x.dropdown_type.ToUpper() == ("Reference_Type").ToUpper()).ToList();
            var UserDetails = new BLUser().GetUserByManagerId(Convert.ToInt32(Session["user_id"]));
            objTicketMaster.lstUserName.AddRange(UserDetails);
        }
        public ActionResult SaveTicket(TicketMaster objTicketMaster)
        {
            ModelState.Clear(); 
            if (TryValidateModel(objTicketMaster))
            {
                var isValidRFSType = new BLTicketManager().ValidateRfsType(objTicketMaster.bld_rfs_type);
                if (!isValidRFSType.status)
                {
                    objTicketMaster.pageMsg.message = BLConvertMLanguage.MultilingualMessageConvert(isValidRFSType.message) ;
                    objTicketMaster.pageMsg.status = ResponseStatus.VALIDATION_FAILED.ToString();
                }
                else
                {
                    var result = new BLTicketManager().SaveTicket(objTicketMaster, Convert.ToInt32(Session["user_id"]));
                    if (string.IsNullOrEmpty(result.pageMsg.message))
                    {
                        if (objTicketMaster.pageMsg.isNewEntity)
                        {
                            objTicketMaster.pageMsg.message =Resources.Resources.SI_OSP_GBL_NET_FRM_317;
                            objTicketMaster.pageMsg.status = ResponseStatus.OK.ToString();
                        }
                        else
                        {
                            objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_318;
                            objTicketMaster.pageMsg.status = ResponseStatus.OK.ToString();
                        }

                        //objTicketMaster = result;
                    }
                    else
                    {
                        objTicketMaster.pageMsg.status = ResponseStatus.FAILED.ToString();
                        objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_319;
                    }
                } 
               
            }
            else
            {
                objTicketMaster.pageMsg.status = ResponseStatus.FAILED.ToString();
                objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_319;
            }

            //TicketMaster objTicket = new TicketMaster();
            //BindTicketDropDown(objTicket);
            //return PartialView("_AddTicket", objTicketMaster);
            return Json(objTicketMaster, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerByCanId(string networkId)
        {
            Customer objCustomerInfo = new Customer();
            var objCustomer = new BLCustomer().GetCustomerByCanId(networkId);
            if (objCustomer != null)
            {
                return Json(objCustomer, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(objCustomerInfo, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetTicketDetailByCanId(string networkId) 
        {
            TicketMaster objTicketInfo = new TicketMaster();
            var objTicketMaster = new BLTicketManager().GetTicketDetailByCanId(networkId);
            if (objTicketMaster != null)
            {
                return Json(objTicketMaster, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(objTicketInfo, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBuildingByCode(string networkId)
        {
            BuildingMaster objBuildingMaster = new BuildingMaster();
            var objBuilding = BLBuilding.Instance.GetBuildingByCode(networkId);
            if (objBuilding != null)
            {
                return Json(objBuilding, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(objBuildingMaster, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult EditTicket(int ticket_id)
        {
            TicketMaster objTicketMaster = new TicketMaster();
            try
            {
                objTicketMaster = new BLTicketManager().GetTicketById(ticket_id);
                if (objTicketMaster != null)
                {
                    objTicketMaster.pageMsg.status = ResponseStatus.OK.ToString();
                    return Json(objTicketMaster, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    objTicketMaster.pageMsg.status = ResponseStatus.FAILED.ToString();
                    objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_GBL_149;
                }
            }
            catch (Exception ex)
            {
                objTicketMaster.pageMsg.status = ResponseStatus.ERROR.ToString();
                objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
            }
            return Json(objTicketMaster, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTicket(string ticket_ids) 
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
               

                var result = new BLTicketManager().DeleteTicketById(ticket_ids, Convert.ToInt32(Session["user_id"]));
                if (result)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_321;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_322;
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message =Resources.Resources.SI_OSP_GBL_NET_FRM_320;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public void ExportTickets()
        {
            if (Session["viewTicketDashboardFilter"] != null)
            {
                TicketManagerFilter objViewFilter = (TicketManagerFilter)Session["viewTicketDashboardFilter"];
                List<TicketMasterGrid> lstTicketMaster = new List<TicketMasterGrid>();
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstTicketMaster = new BLTicketManager().GetTicketDetails(Convert.ToInt32(Session["user_id"]),objViewFilter);
                DataTable dtReport = new DataTable();                
                dtReport = MiscHelper.ListToDataTable<TicketMasterGrid>(lstTicketMaster);
                dtReport.Columns.Add("COMPLETED ON", typeof(System.String));
                dtReport.Columns.Add("TARGET DATE", typeof(System.String));
                dtReport.Columns.Add("ASSIGNED_ON", typeof(System.String));
                
                foreach (DataRow dr in dtReport.Rows)
                {
                    dr["COMPLETED ON"] = MiscHelper.FormatDateTime((dr["completed_on"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                    dr["TARGET DATE"] = MiscHelper.FormatDate((dr["target_date"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                    dr["ASSIGNED_ON"]= MiscHelper.FormatDateTime((dr["assigned_date"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                  
                }
                 
                //dtReport.Columns.Remove("assigned_by");
                dtReport.Columns.Remove("ticket_description");
                dtReport.Columns.Remove("totalrecords");
                //dtReport.Columns.Remove("ticket_id");
                dtReport.Columns.Remove("assigned_date");
                dtReport.Columns.Remove("target_date");
                dtReport.Columns.Remove("completed_on");
                dtReport.Columns["COMPLETED ON"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_169;
                dtReport.Columns["TARGET DATE"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_170;
                dtReport.Columns["BLD_RFS_TYPE"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_171;
                //dtReport.Columns["TICKET_ID"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_172;
                dtReport.Columns["TICKET_TYPE"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_173;
                dtReport.Columns["REFERENCE_TYPE"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_174;
                dtReport.Columns["CAN_ID"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_175;
                dtReport.Columns["CUSTOMER_NAME"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_176;
                dtReport.Columns["ADDRESS"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_078;
                dtReport.Columns["TICKET_STATUS"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_177;
                dtReport.Columns["BUILDING_CODE"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_178;
                dtReport.Columns["ASSIGNED_TO"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_179;           
                dtReport.Columns["COMPLETED_BY"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_180;
                dtReport.Columns["ASSIGNED_ON"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_181;
                //string.Format("{0:DD-Mon-YYYY}", dtReport.Columns["ASSIGNED_DATE"]);
                var filename = "Ticket_report";
                dtReport.TableName = "Customer Tickets";
                ExportAssignmentData(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
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
        
        public ActionResult TicketFilter(TicketManagerFilter objTicketManagerFilter)
        {
            
            return PartialView("_TicketFilter", objTicketManagerFilter);
        }

        #region Ticket upload 
        public FileResult DownloadUploadTicketsTemplate(string FileName) 
        {
            var file = "~//Content//Templates//Bulk//TicketData.xlsx";
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
        public ActionResult UploadTicketData()
        {
            string strReturn = "";
            string msg = "";
           
            //table for data
            DataTable dtExcelData = new DataTable();
            try
            {

               

                if (Request != null)
                {
                    int userId = Convert.ToInt32(Session["user_id"]);
                    var objfile = Request.Files[0];
                    var fileName = AppendTimeStamp(Request.Files[0].FileName);
                    var filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\Tickets\\"), fileName);
                    objfile.SaveAs(filepath);
                    bool isHeaderFound = false;
                    //read uploaded excel files..
                    DataTable dataTable = NPOIExcelHelper.ExcelToTable(filepath, out isHeaderFound);
                    if (!isHeaderFound)
                    {
                        return Json(new { strReturn =Resources.Resources.SI_OSP_GBL_NET_FRM_323, msg = "error" }, JsonRequestBehavior.AllowGet);
                    }
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        dtExcelData = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
                    }
                    
                    if (dtExcelData.Rows.Count > 0)
                    {
                        //get maximum building upload count allowed at a time...
                        if (dtExcelData.Rows.Count <= ApplicationSettings.BulkTicketUploadMaxCount)
                        {
                            string ErrorMsg = "";
                            // get branch column mapping...
                            string strMappingFilePath = Server.MapPath("~\\Content\\Templates\\Bulk\\TicketTemplate.xml");
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
                             

                            //delete DATA FROM TEMP TABLE ON THE BASIS OF UPLOADED_BY ID
                            BLTempTicketMaster.Instance.DeleteTempTicketData(userId);


                            List<TempTicketMaster> lstTicketMaster = new List<TempTicketMaster>();
                             

                            foreach (DataRow dr in dtExcelData.Rows)
                            {


                                TempTicketMaster objTempTicketMaster  = new TempTicketMaster();

                                string strErrorMsg = ValidateTicketData(dr, ref objTempTicketMaster, dicColumnMapping);

                                objTempTicketMaster.uploaded_by = userId;
                               // var ticketTypeId= new BLTicketManager().GetTicketTypeByID(dr[dicColumnMapping["ticket_type"]].ToString());
                                //objTempTicketMaster.ticket_type_id = ticketTypeId;
                                objTempTicketMaster.ticket_type = dr[dicColumnMapping["ticket_type"]].ToString();
                                objTempTicketMaster.can_id = dr[dicColumnMapping["can_id"]].ToString();
                                objTempTicketMaster.reference_type = dr[dicColumnMapping["reference_type"]].ToString();
                                objTempTicketMaster.customer_name = dr[dicColumnMapping["customer_name"]].ToString();
                                objTempTicketMaster.address = dr[dicColumnMapping["Address"]].ToString();
                                objTempTicketMaster.building_code = dr[dicColumnMapping["building_code"]].ToString();
                                //objTempTicketMaster.bld_rfs_type = dr[dicColumnMapping["bld_rfs_type"]].ToString();
                                //var objUser=new BLUser().GetUserDetailByName(dr[dicColumnMapping["assigned_to"]].ToString());
                               // objTempTicketMaster.assigned_to = objUser.user_id;
                                objTempTicketMaster.assigned_to = dr[dicColumnMapping["assigned_to"]].ToString();
                                //objTempTicketMaster.target_date = MiscHelper.FormatDateTime(dr[dicColumnMapping["target_date"]].ToString());
                                objTempTicketMaster.target_date = dr[dicColumnMapping["target_date"]].ToString();
                                objTempTicketMaster.ticket_status = "Assigned";
                                objTempTicketMaster.created_by = userId;
                                objTempTicketMaster.created_on = DateTimeHelper.Now;
                                objTempTicketMaster.assigned_by = userId;
                                objTempTicketMaster.assigned_date = DateTimeHelper.Now;
                                lstTicketMaster.Add(objTempTicketMaster);

                            }
                            if (lstTicketMaster.Count > 0)
                            {
                                //SAVE DATA INTO TEMP BUILDING TABLE
                                BLTempTicketMaster.Instance.BulkUploadTempTicket(lstTicketMaster);
                                //VALIDATE AND UPLAOD BUILDING INTO MAIN TABLE.
                                dynamic result = "";

                                //var jsonSerializer = new JavaScriptSerializer();
                               // var jsonTicketlist = jsonSerializer.Serialize(lstTicketMaster);
                                result = BLTempTicketMaster.Instance.UploadTickets(userId);

                                if (!result.status)
                                {
                                    // exit function if failed..
                                    return Json(new { strReturn = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_324, result.message), msg = "error" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            var getTotalUploadTicketfailureAndSuccess = BLTempTicketMaster.Instance.getTotalUploadTicketfailureAndSuccess(userId); 
                            var GetTotalCountOfSuccesAndFailure = "<table border='1' class='alertgrid'><thead><tr><td><b>Status</b></td><td><b>Count</b></td></tr></thead><tbody><tr><td>Success</td><td>" + getTotalUploadTicketfailureAndSuccess.Item1 + "</td></tr><tr><td>failure</td><td>" + getTotalUploadTicketfailureAndSuccess.Item2 + "</td></tr></tbody>";
                            strReturn = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_325, GetTotalCountOfSuccesAndFailure);
                        }
                        else 
                        {
                            // exit function with max record error...
                            return Json(new { strReturn = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_326, ApplicationSettings.BulkTicketUploadMaxCount), msg = "error" }, JsonRequestBehavior.AllowGet);
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
                strReturn =Resources.Resources.SI_OSP_GBL_NET_FRM_328;
                ErrorLogHelper.WriteErrorLog("UploadTicketData()", "Library", ex);

            }
            catch (Exception ex)
            {
                msg = "error";
                strReturn = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_329, ex.Message);
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
                    return   string.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_331, pair.Value);
               // return Resources.Resources.SI_OSP_GBL_NET_FRM_331 +" "+ pair.Value + "' column!";
            }
            return "";
        }


        public string ValidateTicketData(DataRow dr, ref TempTicketMaster objTempTicketMaster, Dictionary<string, string> dicColumnMapping)
        {
             
            objTempTicketMaster.is_valid = true;
            Regex nonNumericRegex = new Regex(@"\.");
            try
            {
                if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["ticket_type"]].ToString()))
                {
                    objTempTicketMaster.is_valid = false;
                    objTempTicketMaster.error_msg = Resources.Resources.SI_OSP_GBL_NET_FRM_330;
                }
                if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["reference_type"]].ToString()))
                {
                    objTempTicketMaster.is_valid = false;
                    objTempTicketMaster.error_msg = Resources.Resources.SI_OSP_GBL_NET_FRM_332;
                }
                if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["can_id"]].ToString()))
                {
                    objTempTicketMaster.is_valid = false;
                    objTempTicketMaster.error_msg = Resources.Resources.SI_OSP_GBL_NET_FRM_333;
                }
                if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["customer_name"]].ToString()))
                {
                    objTempTicketMaster.is_valid = false;
                    objTempTicketMaster.error_msg = Resources.Resources.SI_OSP_GBL_NET_FRM_334;
                }
                if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["Address"]].ToString()))
                {
                    objTempTicketMaster.is_valid = false;
                    objTempTicketMaster.error_msg = Resources.Resources.SI_OSP_GBL_NET_FRM_335;
                }
                if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["building_code"]].ToString()))
                {
                    objTempTicketMaster.is_valid = false;
                    objTempTicketMaster.error_msg = Resources.Resources.SI_OSP_GBL_NET_FRM_336;
                }
                //if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["bld_rfs_type"]].ToString()))
                //{
                //    objTempTicketMaster.is_valid = false;
                //    objTempTicketMaster.error_msg = "RFS type can not be blank!";
                //}
                if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["assigned_to"]].ToString()))
                {
                    objTempTicketMaster.is_valid = false;
                    objTempTicketMaster.error_msg = Resources.Resources.SI_OSP_GBL_NET_FRM_337;
                }
                if (string.IsNullOrWhiteSpace(dr[dicColumnMapping["target_date"]].ToString()))
                {
                    objTempTicketMaster.is_valid = false;
                    objTempTicketMaster.error_msg = Resources.Resources.SI_OSP_GBL_NET_FRM_338;
                }
                 
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        public void DownloadUploadTicketLogs()
        {
            DataTable dtlogs = new DataTable();
            dtlogs.Columns.Add("Ticket Type", typeof(string));
            dtlogs.Columns.Add("Reference Type", typeof(string));
            dtlogs.Columns.Add("Can Id", typeof(string));
            dtlogs.Columns.Add("Customer Name", typeof(string));
            dtlogs.Columns.Add("Address", typeof(string));
            dtlogs.Columns.Add("Building Code", typeof(string)); 
            dtlogs.Columns.Add("Assigned To", typeof(string));
            dtlogs.Columns.Add("Target Date", typeof(string));
            dtlogs.Columns.Add("Error Msg", typeof(string));
            dtlogs.TableName = "TicketLogs";
            int userId = Convert.ToInt32(Session["user_id"]);
            using (var exportData = new MemoryStream()) 
            {
                Response.Clear();
                List<TempTicketMaster> BulkUploadLogs = BLTempTicketMaster.Instance.GetUploadTicketLogs(userId);

                if (BulkUploadLogs.Count() > 0)
                {
                    foreach (var t in BulkUploadLogs)
                    {
                        //if (string.IsNullOrWhiteSpace(t.address))
                        //{
                        //    t.address = "";
                        //    dtlogs.Rows.Add(t.ticket_type.ToString(), t.reference_type.ToString(), t.can_id.ToString(), t.customer_name.ToString(), t.address.ToString(), t.building_code.ToString(), t.assigned_to.ToString(), t.target_date.ToString(), t.error_msg);
                        //}
                        //else
                        {
                            dtlogs.Rows.Add(t.ticket_type.ToString(), t.reference_type.ToString(), t.can_id.ToString(), t.customer_name.ToString(), t.address.ToString(), t.building_code.ToString(), t.assigned_to.ToString(), t.target_date.ToString(), t.error_msg);
                        }
                    }

                    IWorkbook workbook = SmartInventory.Helper.NPOIExcelHelper.DataTableToExcel("xlsx", dtlogs);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", AppendTimeStamp("Ticketlogs.xlsx")));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }
        #endregion

        public ActionResult AddHPSMTicket()
        {
            TicketMaster objTicketMaster = new TicketMaster();
            //ModelState.Clear();
            User userDetail = ((User)Session["userDetail"]);
            objTicketMaster.contact_no = MiscHelper.Decrypt(userDetail.mobile_number).ToString();  //mobile number D/E

            objTicketMaster.customer_name = userDetail.user_name;
            objTicketMaster.ticket_reference = "JFP";
            objTicketMaster.lstTicketTypeMaster = new BLNetworkTicket().GetHPSMTicketType("HPSM");

            return PartialView("_HPSMTicket", objTicketMaster);
        }
        [HttpPost]
        public ActionResult SaveHPSMTicket(TicketMaster objHPSMTicket)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            List<HttpPostedFileBase> totalFiles = new List<HttpPostedFileBase>();
            string extension = string.Empty, fileName;
            HttpPostedFileBase file;
            int TicketAttachmentMaxSize = ApplicationSettings.TicketAttachmentMaxSize;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                file = Request.Files[i];
                if ((file.ContentLength / (1024 * 1024)) > TicketAttachmentMaxSize)
                {
                    return Json(new { Message = "Please select a file less than " + TicketAttachmentMaxSize + " MB", msg = ResponseStatus.ERROR.ToString() }, JsonRequestBehavior.AllowGet);
                }
                fileName = file.FileName;
                extension = System.IO.Path.GetExtension(fileName).Split('.')[1].ToLower();

                string[] RestrictedTicketAttachments = ApplicationSettings.RestrictedTicketAttachments.Split(',').Select(ext => ext.Trim()).ToArray();
                if (RestrictedTicketAttachments.Contains(extension))
                {
                    return Json(new { Message = ApplicationSettings.RestrictedTicketAttachments + " not acceptable!", msg = ResponseStatus.ERROR.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                objHPSMTicket.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                objHPSMTicket.can_id = "0";
                objHPSMTicket.bld_rfs_type = "N/A";
                objHPSMTicket.target_date = DateTimeHelper.Now;
                objHPSMTicket.assigned_to = "0";
                objHPSMTicket.reference_type = "N/A";
                objHPSMTicket.building_code = "N/A";
                objHPSMTicket.address = "N/A";
                objHPSMTicket.customer_name = Request.Form["txtCustomerName"];
                objHPSMTicket.contact_no = Request.Form["txtContactNo"];
                objHPSMTicket.ticket_reference = Request.Form["txtTicketReference"];
                objHPSMTicket.ticket_type_id = Convert.ToInt32(Request.Form["ddlTicketTypes"]);
                objHPSMTicket.ticket_description = Request.Form["txtDescription"];
                objHPSMTicket.created_on = DateTimeHelper.Now;
                objHPSMTicket.ticket_status = "Open";
                objHPSMTicket.ticket_status_id = 0;
                objHPSMTicket.pageMsg.isNewEntity = true;
                string UploadStatus = "";

                var status = new BLHPSMTicket().SaveHPSMTicket(objHPSMTicket);
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase postedFile = Request.Files[i];
                        totalFiles.Add(postedFile);
                    }
                    UploadStatus = UploadTicketFile(objHPSMTicket);
                }
                if (UploadStatus != "")
                {
                    objMsg.status = ResponseStatus.ERROR.ToString();
                    objMsg.message = UploadStatus;
                    objHPSMTicket.pageMsg = objMsg;
                    return Json(new { Message = objMsg.message, msg = objMsg.status }, JsonRequestBehavior.AllowGet);
                }
                objMsg.status = ResponseStatus.OK.ToString();

                objMsg.message = "Ticket Detail Saved successfully!";

                objHPSMTicket.pageMsg = objMsg;
                string mailsentmsg;
                string user_email = string.Empty;
                int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                objHPSMTicket.lstTicketTypeMaster = new BLNetworkTicket().GetHPSMTicketType("HPSM");
                string CatagoryName = objHPSMTicket.lstTicketTypeMaster.SingleOrDefault(cat => cat.id == objHPSMTicket.ticket_type_id).ticket_type;

                //Name//Mobile//Category//Business name
                //string subject = objHPSMTicket.customer_name + "/" + objHPSMTicket.contact_no + "/" + CatagoryName + "/" + objHPSMTicket.ticket_reference;

                //BIM//Business name//Category
                string subject = "BIM" + ":" + "JFP" + ":" + CatagoryName;

                string[] aarReceiver = ApplicationSettings.TicketReceiverMailId.Split(',');
                commonUtil.SendEmail(aarReceiver, subject, objHPSMTicket.ticket_description, totalFiles, out mailsentmsg, BLMisc.EmailSettingsModel, user_email);

            }
            catch (Exception ex)
            {
                string msg = "";
                if (objMsg.status == "OK")
                {
                    msg = objMsg.message + " But ";
                }
                ErrorLogHelper.WriteErrorLog("SaveHPSMTicket()", "TicketManager Controller", ex);
                objMsg.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objMsg.message = msg + ex.Message.ToString();
            }
            return Json(new { Message = objMsg.message, Status = objMsg.status }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public string UploadTicketFile(TicketMaster objHPSMTicket)
        {
            List<TicketAttachments> lstTicketAttachments = new List<TicketAttachments>();
            string fileName;
            List<HttpPostedFileBase> PostedFiles = new List<HttpPostedFileBase>();
            int TicketAttachmentMaxSize = ApplicationSettings.TicketAttachmentMaxSize;
            try
            {
                int user_id = Convert.ToInt32(Session["user_id"]);
               

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    PostedFiles.Add(Request.Files[i]);
                    
                }
                UploadfileOnFTP(objHPSMTicket.ticket_id, PostedFiles);
               
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    fileName = Request.Files[i].FileName;
                    TicketAttachments ticketAttachments = new TicketAttachments();
                    ticketAttachments.ticket_id = objHPSMTicket.ticket_id;
                    ticketAttachments.org_file_name = fileName;
                    ticketAttachments.file_name = fileName;
                    ticketAttachments.file_extension = System.IO.Path.GetExtension(fileName).Split('.')[1];
                    ticketAttachments.file_location = ConfigurationManager.AppSettings["FTPAttachment"] + objHPSMTicket.ticket_id + "/" + fileName;
                    ticketAttachments.file_size = Request.Files[i].ContentLength;
                    ticketAttachments.uploaded_by = user_id;
                    ticketAttachments.uploaded_on = DateTimeHelper.Now;
                    lstTicketAttachments.Add(ticketAttachments);
                }
                bool savefile = new BLHPSMTicket().SaveTicketAttachments(lstTicketAttachments, user_id);
                if (savefile)
                {
                    return "";
                }
                else
                {
                    return "Error";
                }

            }
            catch (NPOI.POIFS.FileSystem.NotOLE2FileException ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadTicketFile()", "TicketManager", ex);
                return "Selected file is either corrupted or invalid excel file!";
            }
            catch (Exception ex)
            {

                ErrorLogHelper.WriteErrorLog("UploadTicketFile()", "TicketManager", ex);
                return "Failed to upload HPSM Ticket! <br> Error:" + ex.Message;
            }
        }
        static void UploadfileOnFTP(int TicketId, List<HttpPostedFileBase> postedFiles)
        {
            try
            {
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {
                    // Create Directory if not exists and get Final FTP path to save file..

                    strFTPPath = CreateNestedDirectoryOnFTP(strFTPPath, strFTPUserName, strFTPPassWord, TicketId);
                    //Prepare FTP Request..
                    foreach (HttpPostedFileBase file in postedFiles)
                    {
                        string newfilename = file.FileName;
                        FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(strFTPPath + "/" + newfilename);
                        ftpReq.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                        ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                        ftpReq.UseBinary = true;

                        string savepath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);

                        file.SaveAs(savepath + @"\" + newfilename);
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
                    }
                }

               // return file path
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
        private static string CreateNestedDirectoryOnFTP(string strFTPPath, string strUserName, string strPassWord, int TicketId)
        {
            try
            {
                FtpWebRequest reqFTP;
                string strFTPFilePath = strFTPPath + TicketId;
                if (TicketId > 0)
                {

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
                return strFTPFilePath;
            }
            catch { throw; }
        }
    }
}