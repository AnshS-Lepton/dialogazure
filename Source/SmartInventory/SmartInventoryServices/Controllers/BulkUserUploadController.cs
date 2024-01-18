using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web;
using System.Web.Hosting;
using System.Text;
using System.Text.RegularExpressions;
using Utility;
using BusinessLogics;
using Models;
using Models.API;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System.Data;
using NPOI.XSSF.UserModel;

namespace SmartInventoryServices.Controllers
{
    [CustomAuthorization]
    [HandleException]
    [RoutePrefix("api/blkuserupload")]
    public class BulkUserUploadController : ApiController
    {
        [HttpPost]
        [Route("bulkusrupd")]
        public ApiResponse<BulkUserUploadSummary> BulkUserUploadFile()
        {
            var response = new ApiResponse<BulkUserUploadSummary>();
            BulkUserUploadSummary userUploadSummary = new BulkUserUploadSummary();
            try
            {
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    // Get the uploaded file from Files collection
                    var postedFile = HttpContext.Current.Request.Files["postedFile"];
                    int login_user_id = Convert.ToInt32(HttpContext.Current.Request.Form["usrid"]);
                    if (postedFile != null)
                    {
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            string filePath = string.Empty;
                            string path = HostingEnvironment.MapPath("~/Uploads/BulkUserUpload");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            try
                            {
                                ////save the excel file
                                string datefomat = DateTime.Now.ToString("ddMMyyyy_HHmmss");
                                string extension = Path.GetExtension(postedFile.FileName);
                                string filename = Path.GetFileNameWithoutExtension(postedFile.FileName) + "_" + datefomat + extension;
                                filePath = Path.Combine(path, filename);
                                postedFile.SaveAs(filePath);

                                //To retrive All the sheet's Name from the uploaded file
                                List<string> listOfSheets = new List<string>();
                                FileStream excelStream = new FileStream(filePath, FileMode.Open);

                                if (extension == ".xlsx")
                                {
                                    //XSSFWorkBook will read Excel format  
                                    XSSFWorkbook hssfwb = new XSSFWorkbook(excelStream);

                                    for (int i = 0; i < hssfwb.NumberOfSheets; i++)
                                    {
                                        string SheetName = hssfwb.GetSheetName(i).Trim().Replace(" ", "").ToUpper();

                                        if (!string.IsNullOrEmpty(SheetName))
                                        {
                                            listOfSheets.Add(SheetName);
                                        }
                                    }
                                }
                              
                                ////read all 5 sheets
                                bool isheader = true;
                                DataTable dtUserDetails = NPOIExcelHelper.ExcelToTable(filePath, "User Details", out isheader);
                                DataTable dtWebModule = NPOIExcelHelper.ExcelToTable(filePath, "Web Module", out isheader);
                                DataTable dtMobileModule = NPOIExcelHelper.ExcelToTable(filePath, "Mobile Module", out isheader);
                                DataTable dtAdminModule = NPOIExcelHelper.ExcelToTable(filePath, "Admin Module", out isheader);
                                DataTable dtWorkAreaDetails = NPOIExcelHelper.ExcelToTable(filePath, "Work Area Details", out isheader);
                                //ankit
                                DataTable dtServiceFacility = null;
                                DataTable dtJobOrder = null;
                                DataTable dtJobCategory = null;
                                if (listOfSheets.Contains("SERVICEFACILITY"))
                                {
                                    dtServiceFacility = NPOIExcelHelper.ExcelToTable(filePath, "Service Facility", out isheader);
                                }
                                if (listOfSheets.Contains("JOBORDER"))
                                {
                                    dtJobOrder = NPOIExcelHelper.ExcelToTable(filePath, "Job Order", out isheader);
                                }
                                if (listOfSheets.Contains("JOBCATEGORY"))
                                {
                                    dtJobCategory = NPOIExcelHelper.ExcelToTable(filePath, "Job Category", out isheader);
                                }
                                
                                //ankit

                                userUploadSummary.file_name = filename;
                                userUploadSummary.created_by = login_user_id;
                                userUploadSummary.status = "OK";
                                userUploadSummary.err_description = "";
                                userUploadSummary.total_record = dtUserDetails.Rows.Count;

                                userUploadSummary = new BLBulkUserUploadSummary().SaveBulkUserUploadSummary(userUploadSummary);

                                ////for Creating the User Object
                                if (dtUserDetails != null && dtUserDetails.Rows.Count > 0)
                                {
                                    List<BulkUserUpload> lstUser = dtUserDetails.AsEnumerable().Select(item => new BulkUserUpload
                                    {
                                        user_upload_id = userUploadSummary.id,
                                        user_name = Convert.ToString(item.Field<string>("User Name")).Trim(),
                                        name = Convert.ToString(item.Field<string>("Full Name")).Trim(),
                                        user_email = Convert.ToString(item.Field<string>("Email ID")).Trim(),
                                        password = Convert.ToString(item.Field<string>("Password")).Trim(),
                                        mobile_number = Convert.ToString(item.Field<string>("Mobile number")).Trim(),
                                        role_name = Convert.ToString(item.Field<string>("User Role")).Trim(),
                                        reporting_manager = Convert.ToString(item.Field<string>("Reporting Manager")).Trim(),
                                        application_access = Convert.ToString(item.Field<string>("Application Access")).ToUpper().Trim(),
                                        is_admin_rights_enabled = Convert.ToString(item.Field<string>("is Admin right Allowed")).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false,
                                        is_active = Convert.ToString(item.Field<string>("Is Active")).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false,
                                        created_by = login_user_id,
                                        user_img = "user.png",
                                        user_type = Convert.ToString(item.Field<string>("User Type")).Trim(),
                                        pan = Convert.ToString(item.Field<string>("PAN")).Trim(),
                                        prms_id = Convert.ToString(item.Field<string>("PRMS Id")).Trim(),
                                        vendor_id = Convert.ToString(item.Field<string>("Vendor Id")).Trim()
                                    }).ToList<BulkUserUpload>();

                                    ////Validation code goes here 
                                    lstUser = ValidateUserUploadedFile(lstUser, login_user_id);
                                    new BLBulkUserUpload().SaveBulkUserUpload(lstUser);
                                    //check for both type of user limit 
                                    BulkUserUploadLimit bulkUserUploadLimit = CheckBulkUploadUserLimit(userUploadSummary.id);
                                    if (bulkUserUploadLimit.status)
                                    {
                                        List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping = new BLBulkUserUploadModuleMapping().GetBulkUserUploadModuleMasterMapping();
                                        ////insert user module mapping details
                                        List<BulkUserUploadModuleMapping> lstUserUploadModuleMapping = new List<BulkUserUploadModuleMapping>();
                                        CreateUserUploadMobileModuleMapping(userUploadSummary.id, dtMobileModule, "Mobile", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
                                        CreateUserUploadWebModuleMapping(userUploadSummary.id, dtWebModule, "Web", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
                                        CreateUserUploadAdminModuleMapping(userUploadSummary.id, dtAdminModule, "Admin", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
                                        new BLBulkUserUploadModuleMapping().SaveBulkUserUploadModuleMapping(lstUserUploadModuleMapping);

                                        //ankit
                                        List<BulkUserUploadManagerMapping> lstBulkUserUploadManagermapping = CreateUserUploadManagerMapping(userUploadSummary.id, lstUser);
                                        new BLBulkUserUploadManagerMapping().SaveUserUploadManagerMapping(lstBulkUserUploadManagermapping);

                                        List<BulkUserUploadJoTypeMasterMapping> lstBulkUserUploadJoTypeMasterMapping = new BLBulkUserUploadJoTypeMapping().GetBulkUserUploadJoTypeMasterMapping();
                                        List<BulkUserUploadJoCategoryMasterMapping> lstBulkUserUploadJoCategoryMasterMapping = new BLBulkUserUploadJoCatehoryMapping().GetBulkUserUploadJoCategoryMasterMapping();
                                        List<BulkUserUploadServiceFacilityMasterMapping> lstBulkUserUploadServiceFacilityMasterMapping = new BLBulkUserUploadServiceFacilityMapping().GetBulkUserUploadServiceFacilityMasterMapping();

                                        List<BulkUserUploadJoTypeMapping> lstUserUploadJoTypeMapping = new List<BulkUserUploadJoTypeMapping>();
                                        List<BulkUserUploadJoCategoryMapping> lstUserUploadJoCategoryMapping = new List<BulkUserUploadJoCategoryMapping>();
                                        List<BulkUserUploadServiceFacilityMapping> lstUserUploadServiceFacilityMapping = new List<BulkUserUploadServiceFacilityMapping>();
                                        
                                        CreateUserUploadJoTypeMapping(userUploadSummary.id, dtJobOrder, "type", lstUserUploadJoTypeMapping, lstBulkUserUploadJoTypeMasterMapping);
                                        CreateUserUploadJoCategoryMapping(userUploadSummary.id, dtJobCategory, "category", lstUserUploadJoCategoryMapping, lstBulkUserUploadJoCategoryMasterMapping);
                                        CreateUserUploadServiceFacilitymapping(userUploadSummary.id, dtServiceFacility, "service", lstUserUploadServiceFacilityMapping, lstBulkUserUploadServiceFacilityMasterMapping);

                                        new BLBulkUserUploadJoTypeMapping().SaveBulkUserUploadJoTypeMapping(lstUserUploadJoTypeMapping);
                                        new BLBulkUserUploadJoCatehoryMapping().SaveBulkUserUploadJoCategoryMapping(lstUserUploadJoCategoryMapping);
                                        new BLBulkUserUploadServiceFacilityMapping().SaveBulkUserUploadServiceFacilityMapping(lstUserUploadServiceFacilityMapping);
                                        //ankit

                                        ////insert user work area details
                                        if (dtWorkAreaDetails != null && dtWorkAreaDetails.Rows.Count > 0)
                                        {
                                            if (dtWorkAreaDetails.Columns.Contains("Sub District Name") && dtWorkAreaDetails.Columns.Contains("Block Name"))
                                            {
                                                List<BulkUserUploadWorkAreaDetail> lstUserUploadWorkAreaDetail = dtWorkAreaDetails.AsEnumerable().Select(item => new BulkUserUploadWorkAreaDetail
                                                {
                                                    user_upload_id = userUploadSummary.id,
                                                    user_name = Convert.ToString(item.Field<string>("User Name")).Trim(),
                                                    country_name = Convert.ToString(item.Field<string>("Country Name")).Trim(),
                                                    region_name = Convert.ToString(item.Field<string>("Region Name")).Trim(),
                                                    province_name = Convert.ToString(item.Field<string>("Province Name")).Trim(),
                                                    sub_district_name = Convert.ToString(item.Field<string>("Sub District Name")).Trim(),
                                                    block_name = Convert.ToString(item.Field<string>("Block Name")).Trim(),
                                                }).ToList<BulkUserUploadWorkAreaDetail>();
                                                new BLBulkUserUploadWorkAreaDetail().SaveBulkUserUploadWorkAreaDetail(lstUserUploadWorkAreaDetail);
                                            }
                                            else {
                                                List<BulkUserUploadWorkAreaDetail> lstUserUploadWorkAreaDetail = dtWorkAreaDetails.AsEnumerable().Select(item => new BulkUserUploadWorkAreaDetail
                                                {
                                                    user_upload_id = userUploadSummary.id,
                                                    user_name = Convert.ToString(item.Field<string>("User Name")).Trim(),
                                                    country_name = Convert.ToString(item.Field<string>("Country Name")).Trim(),
                                                    region_name = Convert.ToString(item.Field<string>("Region Name")).Trim(),
                                                    province_name = Convert.ToString(item.Field<string>("Province Name")).Trim(),
                                                }).ToList<BulkUserUploadWorkAreaDetail>();
                                                new BLBulkUserUploadWorkAreaDetail().SaveBulkUserUploadWorkAreaDetail(lstUserUploadWorkAreaDetail);
                                            }
                                        }
                                        new BLBulkUserUpload().ProcessBulkUserUpload(userUploadSummary.id);
                                        response.error_message = "Success";
                                        response.status = StatusCodes.OK.ToString();
                                        response.results = userUploadSummary;
                                    }
                                }
                                else
                                {
                                    response.error_message = "User's not found in Excel";
                                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                    response.results = userUploadSummary;
                                }
                            }
                            catch (Exception ex)
                            {
                                // response.Message = StatusMessage.Failed.ToString();
                                response.error_message = "Error while file uploading.";
                                response.status = StatusCodes.INVALID_FILE.ToString();
                                response.results = userUploadSummary;
                                ErrorLogHelper logHelper = new ErrorLogHelper();
                                logHelper.ApiLogWriter("BulkUserUploadFile()", "BulkUserUpload Controller", "", ex);
                            }
                        }
                        else
                        {
                            //response.Message = StatusMessage.Failed.ToString();
                            response.error_message = "File size is zero.";
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.results = userUploadSummary;
                        }
                    }
                    else
                    {
                        response.error_message = "File not selected";
                        response.status = StatusCodes.INVALID_REQUEST.ToString();
                        response.results = userUploadSummary;
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("BulkUserUploadFile()", "BulkUserUpload Controller", "", ex);
                response.status = StatusCodes.FAILED.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        //public ApiResponse<BulkUserUploadSummary> BulkUserUploadFile()
        //{
        //    var response = new ApiResponse<BulkUserUploadSummary>();
        //    BulkUserUploadSummary userUploadSummary = new BulkUserUploadSummary();
        //    try
        //    {
        //        if (HttpContext.Current.Request.Files.AllKeys.Any())
        //        {
        //            // Get the uploaded file from Files collection
        //            var postedFile = HttpContext.Current.Request.Files["postedFile"];
        //            int login_user_id = Convert.ToInt32(HttpContext.Current.Request.Form["usrid"]);
        //            if (postedFile != null)
        //            {
        //                if (postedFile != null && postedFile.ContentLength > 0)
        //                {
        //                    string filePath = string.Empty;
        //                    string path = HostingEnvironment.MapPath("~/Uploads/BulkUserUpload");
        //                    if (!Directory.Exists(path))
        //                    {
        //                        Directory.CreateDirectory(path);
        //                    }
        //                    try
        //                    {
        //                        ////save the excel file
        //                        string datefomat = DateTime.Now.ToString("ddMMyyyy_HHmmss");
        //                        string extension = Path.GetExtension(postedFile.FileName);
        //                        string filename = Path.GetFileNameWithoutExtension(postedFile.FileName) + "_" + datefomat + extension;
        //                        filePath = Path.Combine(path, filename);
        //                        postedFile.SaveAs(filePath);

        //                        ////read all 5 sheets
        //                        bool isheader = true;
        //                        DataTable dtUserDetails = NPOIExcelHelper.ExcelToTable(filePath, "User Details", out isheader);
        //                        DataTable dtWebModule = NPOIExcelHelper.ExcelToTable(filePath, "Web Module", out isheader);
        //                        DataTable dtMobileModule = NPOIExcelHelper.ExcelToTable(filePath, "Mobile Module", out isheader);
        //                        DataTable dtAdminModule = NPOIExcelHelper.ExcelToTable(filePath, "Admin Module", out isheader);
        //                        DataTable dtWorkAreaDetails = NPOIExcelHelper.ExcelToTable(filePath, "Work Area Details", out isheader);
        //                        //ankit
        //                        DataTable dtServiceFacility = NPOIExcelHelper.ExcelToTable(filePath, "Service Facility", out isheader);
        //                        DataTable dtJobOrder = NPOIExcelHelper.ExcelToTable(filePath, "Job Order", out isheader);
        //                        DataTable dtJobCategory = NPOIExcelHelper.ExcelToTable(filePath, "Job Category", out isheader);
        //                        //ankit

        //                        userUploadSummary.file_name = filename;
        //                        userUploadSummary.created_by = login_user_id;
        //                        userUploadSummary.status = "OK";
        //                        userUploadSummary.err_description = "";
        //                        userUploadSummary.total_record = dtUserDetails.Rows.Count;

        //                        userUploadSummary = new BLBulkUserUploadSummary().SaveBulkUserUploadSummary(userUploadSummary);

        //                        ////for Creating the User Object
        //                        if (dtUserDetails != null && dtUserDetails.Rows.Count > 0)
        //                        {
        //                            List<BulkUserUpload> lstUser = dtUserDetails.AsEnumerable().Select(item => new BulkUserUpload
        //                            {
        //                                user_upload_id = userUploadSummary.id,
        //                                user_name = Convert.ToString(item.Field<string>("User Name")).Trim(),
        //                                name = Convert.ToString(item.Field<string>("Full Name")).Trim(),
        //                                user_email = Convert.ToString(item.Field<string>("Email ID")).Trim(),
        //                                password = Convert.ToString(item.Field<string>("Password")).Trim(),
        //                                mobile_number = Convert.ToString(item.Field<string>("Mobile number")).Trim(),
        //                                role_name = Convert.ToString(item.Field<string>("User Role")).Trim(),
        //                                reporting_manager = Convert.ToString(item.Field<string>("Reporting Manager")).Trim(),
        //                                application_access = Convert.ToString(item.Field<string>("Application Access")).ToUpper().Trim(),
        //                                is_admin_rights_enabled = Convert.ToString(item.Field<string>("is Admin right Allowed")).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false,
        //                                is_active = Convert.ToString(item.Field<string>("Is Active")).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false,
        //                                created_by = login_user_id,
        //                                user_img = "user.png",
        //                                user_type = Convert.ToString(item.Field<string>("User Type")).Trim(),
        //                                pan= Convert.ToString(item.Field<string>("PAN")).Trim(),
        //                                prms_id = Convert.ToString(item.Field<string>("PRMS Id")).Trim(),
        //                                vendor_id = Convert.ToString(item.Field<string>("Vendor Id")).Trim()

        //                            }).ToList<BulkUserUpload>();

        //                            ////Validation code goes here 
        //                            lstUser = ValidateUserUploadedFile(lstUser, login_user_id);
        //                            new BLBulkUserUpload().SaveBulkUserUpload(lstUser);
        //                            //check for both type of user limit 
        //                            BulkUserUploadLimit bulkUserUploadLimit = CheckBulkUploadUserLimit(userUploadSummary.id);
        //                            if (bulkUserUploadLimit.status)
        //                            {
        //                                List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping = new BLBulkUserUploadModuleMapping().GetBulkUserUploadModuleMasterMapping();
        //                                ////insert user module mapping details
        //                                List<BulkUserUploadModuleMapping> lstUserUploadModuleMapping = new List<BulkUserUploadModuleMapping>();
        //                                CreateUserUploadMobileModuleMapping(userUploadSummary.id, dtMobileModule, "Mobile", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
        //                                CreateUserUploadWebModuleMapping(userUploadSummary.id, dtWebModule, "Web", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
        //                                CreateUserUploadAdminModuleMapping(userUploadSummary.id, dtAdminModule, "Admin", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
        //                                new BLBulkUserUploadModuleMapping().SaveBulkUserUploadModuleMapping(lstUserUploadModuleMapping);

        //                                //ankit
        //                                List<BulkUserUploadManagerMapping> lstBulkUserUploadManagermapping = new List<BulkUserUploadManagerMapping>();

        //                                List<BulkUserUploadJoTypeMasterMapping> lstBulkUserUploadJoTypeMasterMapping = new BLBulkUserUploadJoTypeMapping().GetBulkUserUploadJoTypeMasterMapping();
        //                                List<BulkUserUploadJoCategoryMasterMapping> lstBulkUserUploadJoCategoryMasterMapping = new BLBulkUserUploadJoCatehoryMapping().GetBulkUserUploadJoCategoryMasterMapping();
        //                                List<BulkUserUploadServiceFacilityMasterMapping> lstBulkUserUploadServiceFacilityMasterMapping = new BLBulkUserUploadServiceFacilityMapping().GetBulkUserUploadServiceFacilityMasterMapping();

        //                                List<BulkUserUploadJoTypeMapping> lstUserUploadJoTypeMapping = new List<BulkUserUploadJoTypeMapping>();
        //                                List<BulkUserUploadJoCategoryMapping> lstUserUploadJoCategoryMapping = new List<BulkUserUploadJoCategoryMapping>();
        //                                List<BulkUserUploadServiceFacilityMapping> lstUserUploadServiceFacilityMapping = new List<BulkUserUploadServiceFacilityMapping>();

        //                                CreateUserUploadJoTypeMapping(userUploadSummary.id, dtJobOrder, "type", lstUserUploadJoTypeMapping, lstBulkUserUploadJoTypeMasterMapping);
        //                                CreateUserUploadJoCategoryMapping(userUploadSummary.id, dtJobCategory, "category", lstUserUploadJoCategoryMapping, lstBulkUserUploadJoCategoryMasterMapping);
        //                                CreateUserUploadServiceFacilitymapping(userUploadSummary.id, dtServiceFacility, "service", lstUserUploadServiceFacilityMapping, lstBulkUserUploadServiceFacilityMasterMapping);

        //                                new BLBulkUserUploadJoTypeMapping().SaveBulkUserUploadJoTypeMapping(lstUserUploadJoTypeMapping);
        //                                new BLBulkUserUploadJoCatehoryMapping().SaveBulkUserUploadJoCategoryMapping(lstUserUploadJoCategoryMapping);
        //                                new BLBulkUserUploadServiceFacilityMapping().SaveBulkUserUploadServiceFacilityMapping(lstUserUploadServiceFacilityMapping);
        //                                //ankit

        //                                ////insert user work area details
        //                                if (dtWorkAreaDetails != null && dtWorkAreaDetails.Rows.Count > 0)
        //                                {
        //                                    List<BulkUserUploadWorkAreaDetail> lstUserUploadWorkAreaDetail = dtWorkAreaDetails.AsEnumerable().Select(item => new BulkUserUploadWorkAreaDetail
        //                                    {
        //                                        user_upload_id = userUploadSummary.id,
        //                                        user_name = Convert.ToString(item.Field<string>("User Name")).Trim(),
        //                                        country_name = Convert.ToString(item.Field<string>("Country Name")).Trim(),
        //                                        region_name = Convert.ToString(item.Field<string>("Region Name")).Trim(),
        //                                        province_name = Convert.ToString(item.Field<string>("Province Name")).Trim(),
        //                                        sub_district_name = Convert.ToString(item.Field<string>("Sub District Name")).Trim(),
        //                                        block_name = Convert.ToString(item.Field<string>("Block Name")).Trim(),
        //                                    }).ToList<BulkUserUploadWorkAreaDetail>();
        //                                    new BLBulkUserUploadWorkAreaDetail().SaveBulkUserUploadWorkAreaDetail(lstUserUploadWorkAreaDetail);
        //                                }
        //                                new BLBulkUserUpload().ProcessBulkUserUpload(userUploadSummary.id);
        //                                response.error_message = "Success";
        //                                response.status = StatusCodes.OK.ToString();
        //                                response.results = userUploadSummary;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            response.error_message = "User's not found in Excel";
        //                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
        //                            response.results = userUploadSummary;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        // response.Message = StatusMessage.Failed.ToString();
        //                        response.error_message = "Error while file uploading.";
        //                        response.status = StatusCodes.INVALID_FILE.ToString();
        //                        response.results = userUploadSummary;
        //                    }
        //                }
        //                else
        //                {
        //                    //response.Message = StatusMessage.Failed.ToString();
        //                    response.error_message = "File size is zero.";
        //                    response.status = StatusCodes.INVALID_INPUTS.ToString();
        //                    response.results = userUploadSummary;
        //                }
        //            }
        //            else
        //            {
        //                response.error_message = "File not selected";
        //                response.status = StatusCodes.INVALID_REQUEST.ToString();
        //                response.results = userUploadSummary;
        //            }
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogHelper logHelper = new ErrorLogHelper();
        //        logHelper.ApiLogWriter("BulkUserUploadFile()", "BulkUserUpload Controller", "", ex);
        //        response.status = StatusCodes.FAILED.ToString();
        //        response.error_message = "Error While Processing  Request.";
        //    }
        //    return response;

        //}


        [HttpGet]
        [Route("bulkusrlog")]
        public HttpResponseMessage ExportBulkUserLog(HttpRequestMessage oHttpRequestMessage)
        {
            string id = string.Empty;
            string status = string.Empty;
            string jsonContent = "[{\"Message\":\"Something went wrong\"}]";
            var response = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
            string querystring = oHttpRequestMessage.RequestUri.Query;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                if (!string.IsNullOrEmpty(querystring))
                {
                    System.Collections.Specialized.NameValueCollection parameters = System.Web.HttpUtility.ParseQueryString(querystring);
                    if (parameters.AllKeys.Contains("id"))
                        id = parameters.Get("id");
                    if (parameters.AllKeys.Contains("status"))
                        status = parameters.Get("status");
                }
                Dictionary<string, string> lstReportColumn = new Dictionary<string, string>();
                lstReportColumn.Add("user_name", "User Name");
                lstReportColumn.Add("name", "Full Name");
                lstReportColumn.Add("user_email", "Email ID");
                lstReportColumn.Add("password", "Password");
                lstReportColumn.Add("mobile_number", "Mobile number");
                lstReportColumn.Add("role_name", "User Role");
                lstReportColumn.Add("reporting_manager", "Reporting Manager");
                lstReportColumn.Add("application_access", "Application Access");
                lstReportColumn.Add("is_admin_rights_enabled", "is Admin right Allowed");
                lstReportColumn.Add("is_active", "Is Active");
                lstReportColumn.Add("user_type", "User Type");
                lstReportColumn.Add("pan", "PAN");
                lstReportColumn.Add("prms_id", "PRMS Id");
                lstReportColumn.Add("vendor_id", "Vendor Id");
                lstReportColumn.Add("uploaded_by", "Upload By");
                lstReportColumn.Add("uploaded_date", "Upload Date");
                lstReportColumn.Add("err_message", "Error Message");
                lstReportColumn.Add("status", "Status");
               


                string[] BulkUserUpoladLogColName = lstReportColumn.Select(i => i.Key.ToString()).ToArray();
                DataTable dt1 = new BLBulkUserUpload().GetBulkUserUploadLog(Convert.ToInt32(id), status);
                if (dt1.Rows.Count > 0)
                {
                    DataView view = new DataView(dt1);
                    DataTable dt = view.ToTable(false, BulkUserUpoladLogColName);
                    foreach (var item in lstReportColumn)
                    {
                        dt.Columns[item.Key].ColumnName = item.Value;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        var filename = "Bulk_User_UploadLogs_" + status + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "_" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
                        string filepath = HttpContext.Current.Server.MapPath("~/uploads/temp/"); 

                        if (!Directory.Exists(filepath))
                        {
                            Directory.CreateDirectory(filepath);
                        }
                        filepath= filepath + filename;
                        string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                        result.Content = new ByteArrayContent(fileBytes.ToArray());
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        result.Content.Headers.ContentDisposition.FileName = filename;
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    }
                }
                else
                {
                    jsonContent = "[{\"Message\":\"File not Exists\"}]";
                    response = this.Request.CreateResponse(HttpStatusCode.NotFound);
                    response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ExportBulkUserLog()", "BulkUserUpload Controller", string.Empty, ex);
                response = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }
            return response;
        }

        #region private methods
        //        private List<BulkUserUpload> ValidateUserUploadedFile(List<BulkUserUpload> lstUploadUser, int login_user_id)
        //        {
        //            List<BulkUserUpload> lsttempUploadUser = new List<BulkUserUpload>();
        //            try
        //            {
        //                foreach (BulkUserUpload objUser in lstUploadUser)
        //                {
        //                    string err_message = string.Empty;
        //                    string reg_err_message = string.Empty;
        //                    ////Check user validation
        //                    if (!string.IsNullOrEmpty(objUser.user_name) && !string.IsNullOrEmpty(objUser.user_email))
        //                    {
        //                        Models.User obj = new Models.User();
        //                        obj.user_name = objUser.user_name;
        //                        obj.user_email = objUser.user_email;
        //                        var objUserExist = new BLUser().ChkUserExist(obj);
        //                        if ((objUserExist != null && objUser.user_id != objUserExist.user_id))
        //                        {
        //                            if (objUserExist.user_name.Trim().ToLower() == objUser.user_name.Trim().ToLower())
        //                            {
        //                                err_message = " User Name already exist!";
        //                            }
        //                            else if (objUserExist.user_email.Trim().ToLower() == objUser.user_email.Trim().ToLower())
        //                            {
        //                                err_message += " # Email Id already exist!";
        //                            }

        //                        }
        //                        //check for user name
        //                        if (!string.IsNullOrEmpty(objUser.user_name))
        //                        {
        //                            string reg = @"^[^<>,?;:'()!~%\-@#/*""\s]+$";
        //                            CheckMatch(reg, objUser.user_name, "user_name", out reg_err_message);
        //                            err_message += reg_err_message;
        //                        }
        //                        if (!string.IsNullOrEmpty(objUser.user_email))
        //                        {
        //                            string reg = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";
        //                            CheckMatch(reg, objUser.user_email, "user_email", out reg_err_message);
        //                            err_message += reg_err_message;

        //                        }

        //                    }
        //                    else
        //                    {
        //                        err_message += " # Either User name or User email is empty";
        //                    }

        //                    if (string.IsNullOrEmpty(objUser.name))
        //                    {
        //                        err_message += " # name is empty";
        //                    }
        //                    if (string.IsNullOrEmpty(objUser.password))
        //                    {
        //                        err_message += " # Password is empty";
        //                    }
        //                    if (!string.IsNullOrEmpty(objUser.password))
        //                    {
        //                        string reg = @"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$";
        //                        bool ismatch = CheckMatch(reg, objUser.password, "password", out reg_err_message);
        //                        err_message += reg_err_message;
        //                        if (ismatch)
        //                        {
        //                            objUser.user_password = Convert.ToString(Utility.MiscHelper.EncodeTo64(objUser.password));
        //                        }
        //                    }
        //                    if (string.IsNullOrEmpty(Convert.ToString(objUser.mobile_number)))
        //                    {
        //                        err_message += " # Mobile number  is empty";
        //                    }
        //                    if (!string.IsNullOrEmpty(Convert.ToString(objUser.mobile_number)))
        //                    {
        //                        string reg = @"^[0-9]{1,10}$";
        //                        bool ismatch = CheckMatch(reg, objUser.mobile_number.ToString(), "mobile_number", out reg_err_message);
        //                        err_message += reg_err_message;
        //                    }

        //                    if (string.IsNullOrEmpty(objUser.role_name))
        //                    {
        //                        err_message += " # Role name is empty";
        //                    }
        //                    if (string.IsNullOrEmpty(objUser.reporting_manager))
        //                    {
        //                        err_message += " # Reporting Manager is empty";
        //                    }
        //                    if (string.IsNullOrEmpty(objUser.application_access))
        //                    {
        //                        err_message += " # Application Access is empty";
        //                    }
        //                    if (string.IsNullOrEmpty(objUser.user_type))
        //                    {
        //                        err_message += " # User Type is empty";
        //                    }
        //                    if (!string.IsNullOrEmpty(objUser.pan))
        //                    {
        //                        if (objUser.pan.Length < 10)
        //                        {
        //                            err_message += " # Enter correct PAN";
        //                        }
        //                    }
        //                    if (!string.IsNullOrEmpty(objUser.application_access))
        //                    {
        //                        if (objUser.application_access.ToLower() != "mobile" && objUser.application_access.ToLower() != "web" && objUser.application_access.ToLower() != "both")
        //                        {
        //                            err_message += " # Application Access value should be mobile/web/both";
        //                        }
        //                    }

        //                    if (!string.IsNullOrEmpty(objUser.role_name))
        //                    {
        //                        objUser.role_id = new BLRoles().GetRoleByName(objUser.role_name).role_id;
        //                    }
        //                    if (!string.IsNullOrEmpty(objUser.reporting_manager))
        //                    {

        //                        objUser.manager_id = new BLUser().GetUserDetailByUserName(objUser.reporting_manager).user_id;

        //                        if (objUser.role_id > 0)
        //                        {
        //                            objUser.lstRM = GetReportingManagers(login_user_id, objUser.role_id);
        //                            if (objUser.manager_id > 0)
        //                            {
        //                                int reportingManagerExist = objUser.lstRM.Where(x => x.value == Convert.ToString(objUser.manager_id)).Count();
        //                                if (reportingManagerExist == 0)
        //                                {
        //                                    err_message += " # Reporting Manager not exist";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                err_message += " # Reporting User not exist";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            err_message += " # User role not exist";
        //                        }

        //                    }
        //                    objUser.err_message = err_message;

        //                    if (string.IsNullOrEmpty(objUser.err_message))
        //                    {
        //                        objUser.isvalid = true;
        //                    }
        //                    lsttempUploadUser.Add(objUser);
        //                }
        //            }
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //            catch (Exception ex)
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //            {
        //                throw;
        //            }
        //            return lsttempUploadUser;

        //        }
        private List<BulkUserUpload> ValidateUserUploadedFile(List<BulkUserUpload> lstUploadUser, int login_user_id)
        {
            List<BulkUserUpload> lsttempUploadUser = new List<BulkUserUpload>();
            try
            {
                GlobalSetting globalSetting = new BLGlobalSetting().getValueFullText("IsMultiManagerAllowed");
                foreach (BulkUserUpload objUser in lstUploadUser)
                {
                    string err_message = string.Empty;
                    string reg_err_message = string.Empty;
                    objUser.is_multi_manager_allowed = globalSetting.value == "1" ? true : false;
                    objUser.name = MiscHelper.Encrypt(objUser.name);
                    ////Check user validation
                    if (!string.IsNullOrEmpty(objUser.user_name) && !string.IsNullOrEmpty(objUser.user_email))
                    {
                        Models.User obj = new Models.User();
                        obj.user_name = objUser.user_name;
                        obj.user_email = objUser.user_email;                
                        var objUserExist = new BLUser().ChkUserExist(obj);
                        if (objUserExist != null)
                        {
                            objUserExist.user_email = MiscHelper.Decrypt(objUserExist.user_email);
                        }
                        if (objUserExist != null && objUser.user_id != objUserExist.user_id)
                        {
                            if (objUserExist.user_name.Trim().ToLower() == objUser.user_name.Trim().ToLower())
                            {
                                err_message = " User Name already exist!";
                            }
                            else if (objUserExist.user_email.Trim().ToLower() == objUser.user_email.Trim().ToLower())
                            {
                                err_message += " # Email Id already exist!";
                            }
                        }
                        //check for user name
                        if (!string.IsNullOrEmpty(objUser.user_name))
                        {
                            string reg = @"^[^<>,?;:'()!~%\@#/*""\s]+$";
                            CheckMatch(reg, objUser.user_name, "user_name", out reg_err_message);
                            err_message += reg_err_message;
                        }
                        if (!string.IsNullOrEmpty(objUser.user_email))
                        {
                            string reg = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";
                            bool ismatch = CheckMatch(reg, objUser.user_email, "user_email", out reg_err_message);
                            err_message += reg_err_message;
                            if (ismatch)
                            {
                                objUser.user_email = MiscHelper.Encrypt(objUser.user_email);
                            }
                        }
                    }
                    else
                    {
                        err_message += " # Either User name or User email is empty";
                    }

                    if (string.IsNullOrEmpty(objUser.name))
                    {
                        err_message += " # name is empty";
                    }
                    if (string.IsNullOrEmpty(objUser.password))
                    {
                        err_message += " # Password is empty";
                    }
                    if (!string.IsNullOrEmpty(objUser.password))
                    {
                        string reg = @"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$";
                        bool ismatch = CheckMatch(reg, objUser.password, "password", out reg_err_message);
                        err_message += reg_err_message;
                        if (ismatch)
                        {
                            objUser.user_password = Convert.ToString(Utility.MiscHelper.EncodeTo64(objUser.password));
                        }
                    }
                    if (string.IsNullOrEmpty(Convert.ToString(objUser.mobile_number)))
                    {
                        err_message += " # Mobile number  is empty";
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(objUser.mobile_number)))
                    {
                        string reg = @"^[0-9]{1,10}$";
                        bool ismatch = CheckMatch(reg, objUser.mobile_number.ToString(), "mobile_number", out reg_err_message);
                        if (ismatch)
                        {
                            objUser.mobile_number = MiscHelper.Encrypt(objUser.mobile_number);
                        }
                        err_message += reg_err_message;
                    }

                    if (string.IsNullOrEmpty(objUser.role_name))
                    {
                        err_message += " # Role name is empty";
                    }
                    if (string.IsNullOrEmpty(objUser.reporting_manager))
                    {
                        err_message += " # Reporting Manager is empty";
                    }
                    if (string.IsNullOrEmpty(objUser.application_access))
                    {
                        err_message += " # Application Access is empty";
                    }
                    if (string.IsNullOrEmpty(objUser.user_type))
                    {
                        err_message += " # User Type is empty";
                    }
                    if (!string.IsNullOrEmpty(objUser.pan))
                    {
                        if (objUser.pan.Length < 10)
                        {
                            err_message += " # Enter correct PAN";
                        }
                    }
                    if (!string.IsNullOrEmpty(objUser.application_access))
                    {
                        if (objUser.application_access.ToLower() != "mobile" && objUser.application_access.ToLower() != "web" && objUser.application_access.ToLower() != "both")
                        {
                            err_message += " # Application Access value should be mobile/web/both";
                        }
                    }

                    if (!string.IsNullOrEmpty(objUser.role_name))
                    {
                        objUser.role_id = new BLRoles().GetRoleByName(objUser.role_name).role_id;
                    }
                    if (!string.IsNullOrEmpty(objUser.reporting_manager))
                    {

                        //    objUser.manager_id = new BLUser().GetUserDetailByUserName(objUser.reporting_manager).user_id;
                        //    if (objUser.role_id > 0)
                        //    {
                        //        objUser.lstRM = GetReportingManagers(login_user_id, objUser.role_id);
                        //        if (objUser.manager_id > 0)
                        //        {
                        //            int reportingManagerExist = objUser.lstRM.Where(x => x.value == Convert.ToString(objUser.manager_id)).Count();
                        //            if (reportingManagerExist == 0)
                        //            {
                        //                err_message += " # Reporting Manager not exist";
                        //            }
                        //        }
                        //        else
                        //        {
                        //            err_message += " # Reporting User not exist";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        err_message += " # User role not exist";
                        //    }
                        List<int> manager_ids = new List<int>();
                        List<string> valid_manager_names = new List<string>();
                        List<string> invalid_manager_names = new List<string>();
                        string[] reporting_managers = objUser.reporting_manager.Split(',');
                        foreach (string str_manager in reporting_managers)
                        {
                            var manager_user = new BLUser().GetUserDetailByUserName(str_manager.Trim());
                            if (manager_user != null)
                            {
                                objUser.manager_id = manager_user.user_id;
                            }
                            else
                            {
                                objUser.manager_id = 0;
                            }
                            if (objUser.role_id > 0)
                            {
                                objUser.lstRM = GetReportingManagers(login_user_id, objUser.role_id);
                                if (objUser.manager_id > 0)
                                {
                                    int reportingManagerExist = objUser.lstRM.Where(x => x.value == Convert.ToString(objUser.manager_id)).Count();
                                    if (reportingManagerExist == 0)
                                    {
                                        invalid_manager_names.Add(str_manager);
                                        err_message += " # Reporting Manager not exist";
                                    }
                                    else
                                    {
                                        manager_ids.Add(objUser.manager_id);
                                        valid_manager_names.Add(str_manager);
                                    }
                                }
                                else
                                {
                                    invalid_manager_names.Add(str_manager);
                                    err_message += " # Reporting User not exist";
                                }
                            }
                            else
                            {
                                err_message += " # User role not exist";
                            }
                        }
                        objUser.invalid_reporting_manager = string.Join(",", invalid_manager_names.Select(n => n.ToString()).ToArray());
                        objUser.multi_manager_ids = string.Join(",", manager_ids.Select(n => n.ToString()).ToArray());
                        objUser.reporting_manager = string.Join(",", valid_manager_names.Select(n => n.ToString()).ToArray());
                    }
                    objUser.err_message = err_message;

                    if (string.IsNullOrEmpty(objUser.err_message))
                    {
                        objUser.isvalid = true;
                    }
                    lsttempUploadUser.Add(objUser);
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
            return lsttempUploadUser;

        }

        private bool CheckMatch(string regex, string input, string type, out string message)
        {
            bool isMatch = true;
            message = String.Empty;
            try
            {
                var reg = new Regex(regex);
                if (!reg.IsMatch(input))
                {
                    isMatch = false;
                    if (type == "password")
                    {
                        message = " # password should be : 1 lower and 1 upper case letter. 1 number and 1 special character. Minimum 8 character long!";
                    }
                    if (type == "user_name")
                    {
                        message = " # user_name accept : Only dot and underscore are allowed";
                    }
                    if (type == "mobile_number")
                    {
                        message = "# Mobile number allowed only ten digit ";
                    }
                    if (type == "user_email")
                    {
                        message = "- # Invalid user email!";
                    }

                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
            return isMatch;
        }

        //ankit
        //ankit
        private List<BulkUserUploadManagerMapping> CreateUserUploadManagerMapping(int userUpoadId, List<BulkUserUpload> lstUser)
        {
            List<BulkUserUploadManagerMapping> lstmappings = new List<BulkUserUploadManagerMapping>();
            try
            {
                if (lstUser.Count > 0)
                {
                    foreach (BulkUserUpload bulkUser in lstUser)
                    {
                        string[] manager_ids = bulkUser.multi_manager_ids.Split(',');
                        string[] manager_name = bulkUser.reporting_manager.Split(',');
                        for (int i = 0; i < manager_name.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(manager_name[i]))
                            {
                                BulkUserUploadManagerMapping mapping = new BulkUserUploadManagerMapping();
                                mapping.user_name = bulkUser.user_name;
                                mapping.manager_name = manager_name[i];
                                mapping.user_upload_id = userUpoadId;
                                mapping.manager_id = Convert.ToInt32(manager_ids[i]);
                                lstmappings.Add(mapping);
                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstmappings;
        }
        private List<BulkUserUploadJoTypeMapping> CreateUserUploadJoTypeMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadJoTypeMapping> lstUploadModuleMappings, List<BulkUserUploadJoTypeMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {
            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Select(x => x.jo_name).ToList<string>();
                if (lstUploadUserModule.Count > 0)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadJoTypeMapping bulkUserUploadModuleMapping = new BulkUserUploadJoTypeMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.jo_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }
        //        private List<BulkUserUploadJoTypeMapping> CreateUserUploadJoTypeMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadJoTypeMapping> lstUploadModuleMappings, List<BulkUserUploadJoTypeMasterMapping> lstBulkUserUploadModuleMasterMapping)
        //        {
        //            try
        //            {
        //                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Select(x => x.jo_name).ToList<string>();
        //                if (lstUploadUserModule.Count > 0)
        //                {
        //                    if (dt != null && dt.Rows.Count > 0)
        //                    {
        //                        for (int i = 0; i < dt.Rows.Count; i++)
        //                        {

        //                            foreach (string item in lstUploadUserModule)
        //                            {
        //                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
        //                                {
        //                                    if (dt.Columns.Contains(item))
        //                                    {
        //                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
        //                                        {
        //                                            BulkUserUploadJoTypeMapping bulkUserUploadModuleMapping = new BulkUserUploadJoTypeMapping();
        //                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
        //                                            bulkUserUploadModuleMapping.jo_name = item;
        //                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
        //                                            bulkUserUploadModuleMapping.module_type = moduleType;
        //                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
        //                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    break;
        //                                }

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //            catch (Exception ex)
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //            {
        //                throw ex;
        //            }
        //            return lstUploadModuleMappings;
        //        }
        //        //ankit
        //        private List<BulkUserUploadJoCategoryMapping> CreateUserUploadJoCategoryMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadJoCategoryMapping> lstUploadModuleMappings, List<BulkUserUploadJoCategoryMasterMapping> lstBulkUserUploadModuleMasterMapping)
        //        {
        //            try
        //            {
        //                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Select(x => x.category_name).ToList<string>();
        //                if (lstUploadUserModule.Count > 0)
        //                {
        //                    if (dt != null && dt.Rows.Count > 0)
        //                    {
        //                        for (int i = 0; i < dt.Rows.Count; i++)
        //                        {

        //                            foreach (string item in lstUploadUserModule)
        //                            {
        //                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
        //                                {
        //                                    if (dt.Columns.Contains(item))
        //                                    {
        //                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
        //                                        {
        //                                            BulkUserUploadJoCategoryMapping bulkUserUploadModuleMapping = new BulkUserUploadJoCategoryMapping();
        //                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
        //                                            bulkUserUploadModuleMapping.category_name = item;
        //                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
        //                                            bulkUserUploadModuleMapping.module_type = moduleType;
        //                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
        //                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    break;
        //                                }

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //            catch (Exception ex)
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //            {
        //                throw ex;
        //            }
        //            return lstUploadModuleMappings;
        //        }
        private List<BulkUserUploadJoCategoryMapping> CreateUserUploadJoCategoryMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadJoCategoryMapping> lstUploadModuleMappings, List<BulkUserUploadJoCategoryMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {
            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Select(x => x.category_name).ToList<string>();
                if (lstUploadUserModule.Count > 0)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadJoCategoryMapping bulkUserUploadModuleMapping = new BulkUserUploadJoCategoryMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.category_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }
        //ankit
        //        private List<BulkUserUploadServiceFacilityMapping> CreateUserUploadServiceFacilitymapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadServiceFacilityMapping> lstUploadModuleMappings, List<BulkUserUploadServiceFacilityMasterMapping> lstBulkUserUploadModuleMasterMapping)
        //        {
        //            try
        //            {
        //                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Select(x => x.service_facility_name).ToList<string>();
        //                if (lstUploadUserModule.Count > 0)
        //                {
        //                    if (dt != null && dt.Rows.Count > 0)
        //                    {
        //                        for (int i = 0; i < dt.Rows.Count; i++)
        //                        {

        //                            foreach (string item in lstUploadUserModule)
        //                            {
        //                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
        //                                {
        //                                    if (dt.Columns.Contains(item))
        //                                    {
        //                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
        //                                        {
        //                                            BulkUserUploadServiceFacilityMapping bulkUserUploadModuleMapping = new BulkUserUploadServiceFacilityMapping();
        //                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
        //                                            bulkUserUploadModuleMapping.service_facility_name = item;
        //                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
        //                                            bulkUserUploadModuleMapping.module_type = moduleType;
        //                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
        //                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    break;
        //                                }

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //            catch (Exception ex)
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //            {
        //                throw ex;
        //            }
        //            return lstUploadModuleMappings;
        //        }
        private List<BulkUserUploadServiceFacilityMapping> CreateUserUploadServiceFacilitymapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadServiceFacilityMapping> lstUploadModuleMappings, List<BulkUserUploadServiceFacilityMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {
            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Select(x => x.service_facility_name).ToList<string>();
                if (lstUploadUserModule.Count > 0)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadServiceFacilityMapping bulkUserUploadModuleMapping = new BulkUserUploadServiceFacilityMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.service_facility_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }
        //        private List<BulkUserUploadModuleMapping> CreateUserUploadMobileModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        //        {

        //            try
        //            {
        //                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "mobile").Select(x => x.module_name).ToList<string>();
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    if (lstUploadUserModule.Count > 0)
        //                    {
        //                        for (int i = 0; i < dt.Rows.Count - 1; i++)
        //                        {

        //                            foreach (string item in lstUploadUserModule)
        //                            {
        //                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
        //                                {
        //                                    if (dt.Columns.Contains(item))
        //                                    {
        //                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
        //                                        {
        //                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
        //                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
        //                                            bulkUserUploadModuleMapping.module_name = item;
        //                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
        //                                            bulkUserUploadModuleMapping.module_type = moduleType;
        //                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
        //                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    break;
        //                                }

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //            catch (Exception ex)
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //            {
        //                throw ex;
        //            }
        //            return lstUploadModuleMappings;
        //        }

        private List<BulkUserUploadModuleMapping> CreateUserUploadMobileModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {
            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "mobile").Select(x => x.module_name).ToList<string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (lstUploadUserModule.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.module_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }

        //        private List<BulkUserUploadModuleMapping> CreateUserUploadWebModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        //        {
        //            try
        //            {
        //                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "web").Select(x => x.module_name).ToList<string>();
        //                if (lstUploadUserModule.Count > 0)
        //                {
        //                    if (dt != null && dt.Rows.Count > 0)
        //                    {
        //                        for (int i = 0; i < dt.Rows.Count - 1; i++)
        //                        {

        //                            foreach (string item in lstUploadUserModule)
        //                            {
        //                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
        //                                {
        //                                    if (dt.Columns.Contains(item))
        //                                    {
        //                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
        //                                        {
        //                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
        //                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
        //                                            bulkUserUploadModuleMapping.module_name = item;
        //                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
        //                                            bulkUserUploadModuleMapping.module_type = moduleType;
        //                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
        //                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //            catch (Exception ex)
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //            {
        //                throw ex;
        //            }
        //            return lstUploadModuleMappings;
        //        }
        private List<BulkUserUploadModuleMapping> CreateUserUploadWebModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {
            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "web").Select(x => x.module_name).ToList<string>();
                if (lstUploadUserModule.Count > 0)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.module_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }
        private List<BulkUserUploadModuleMapping> CreateUserUploadAdminModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {
            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "admin").Select(x => x.module_name).ToList<string>();
                if (lstUploadUserModule.Count > 0)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.module_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }
        //        private List<BulkUserUploadModuleMapping> CreateUserUploadAdminModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        //        {
        //            try
        //            {
        //                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "admin").Select(x => x.module_name).ToList<string>();
        //                if (lstUploadUserModule.Count > 0)
        //                {
        //                    if (dt != null && dt.Rows.Count > 0)
        //                    {
        //                        for (int i = 0; i < dt.Rows.Count - 1; i++)
        //                        {

        //                            foreach (string item in lstUploadUserModule)
        //                            {
        //                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
        //                                {
        //                                    if (dt.Columns.Contains(item))
        //                                    {
        //                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
        //                                        {
        //                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
        //                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
        //                                            bulkUserUploadModuleMapping.module_name = item;
        //                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
        //                                            bulkUserUploadModuleMapping.module_type = moduleType;
        //                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
        //                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    break;
        //                                }

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //            catch (Exception ex)
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //            {
        //                throw ex;
        //            }
        //            return lstUploadModuleMappings;
        //        }

        private BulkUserUploadLimit CheckBulkUploadUserLimit(int userUploadId)
        {
            int WebUserMaxLimit = Convert.ToInt32(ApplicationConfig.AppConfig.WebUserMaxLimit);
            int MobileUserMaxLimit = Convert.ToInt32(ApplicationConfig.AppConfig.MobileUserMaxLimit);
            BulkUserUploadLimit bulkUserUploadLimit = new BulkUserUploadLimit();
            try
            {
                bulkUserUploadLimit = new BLBulkUserUploadSummary().BulkUploadUserCheckLimit(userUploadId, WebUserMaxLimit, MobileUserMaxLimit);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return bulkUserUploadLimit;
        }

        private List<KeyValueDropDown> GetReportingManagers(int login_id, int RoleId = 0)
        {
            //int user_id = Convert.ToInt32(Session["user_id"]);
            Models.User objUser = new Models.User();
            return objUser.lstRM = new BLUser().BindReportingManager(RoleId, login_id).OrderBy(m => m.key).ToList();
        }

        #endregion
    }
}
