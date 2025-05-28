using BusinessLogics;
using BusinessLogics.Admin;
using Ionic.Zip;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Lepton.GISConvertor;
using Lepton.Utility;
using Models;
using Models.Admin;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Noding;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using ProjNet.CoordinateSystems;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using Utility;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;


namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]

    public class ReportController : Controller
    {
        //
        // GET: /Report/
        string ftpFolder = "ExportReportLog/";
        private readonly BLExportData blExportData;
        public ReportController()
        {
            blExportData = new BLExportData();
        }
        public ActionResult Index()
        {
            return View();
        }


        #region Export Entites Report

        private void GeneratePDF(Document docPDF, PdfWriter pdfWriter, PdfPTable pdfTable, string fileName)
        {
            docPDF.Add(pdfTable);
            pdfWriter.CloseStream = false;
            docPDF.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(docPDF);
            Response.End();
        }

        private void ExportData(DataTable dtReport, string fileName, bool isDataContainBarcode = false)
        {

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count >= 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport, isDataContainBarcode);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }
        private void ExportDataNew(DataTable dtReport, string fileName, string tempfileName)
        {

            using (var exportData = new MemoryStream())
            {
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport, false);
                    workbook.Write(exportData);
                    FileStream xfile = new FileStream(tempfileName, FileMode.Create, System.IO.FileAccess.Write);
                    workbook.Write(xfile);
                    xfile.Close();

                }

            }
        }
        private void ExportROWBudgetData(DataTable dtReport, string fileName, string roadName, string networkId)
        {

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.ROWBudgetDataTableToExcel("xlsx", dtReport, roadName, networkId);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
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

                    string tempfileName = fileName + ".xlsx";
                    //commented by priyanka
                    //string tempfilePath = Path.Combine(Server.MapPath("~/Uploads/Temp/ExportReportLog"), tempfileName);
                    //end commented by priyanka
                    string downloadTempPath = Settings.ApplicationSettings.DownloadTempPath;
                    string tempfilePath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"), tempfileName);

                    //upper updated by priyanka

                    FileStream xfile = new FileStream(tempfilePath, FileMode.Create, System.IO.FileAccess.Write);
                    workbook.Write(xfile);
                    xfile.Close();


                    string ftpFilePath = ApplicationSettings.FTPAttachment + "ExportReportLog/";

                    CommonUtility.FTPFileUpload(tempfilePath, tempfileName, ftpFilePath, ApplicationSettings.FTPUserNameAttachment, ApplicationSettings.FTPPasswordAttachment);


                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }

        public void StreamCSVToFTP(DataTable dataTable, string remoteFilePath)
        {
            //LogHelper.GetInstance.WriteDebugLog($"\n{DateTime.Now}: Inside StreamCSVToFTP. Table name: {dataTable.TableName}, file name: {remoteFilePath}");
            // Define FTP server details
            string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
            string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
            string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

            try
            {
                // Create a CSV content as a MemoryStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(memoryStream))
                    {
                        // Write the header row
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            writer.Write('"' + column.ColumnName + '"');
                            writer.Write(",");
                        }
                        writer.WriteLine(); // Add a newline after the header row

                        // Write the data rows
                        foreach (DataRow row in dataTable.Rows)
                        {
                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                writer.Write('"' + row[i].ToString() + '"');
                                writer.Write(",");
                            }
                            writer.WriteLine(); // Add a newline after each data row
                        }

                        // Flush the StreamWriter to ensure data is written immediately
                        writer.Flush();
                        //LogHelper.GetInstance.WriteDebugLog($"StreamWriter flushed");

                        // Reset the memory stream position to the beginning
                        memoryStream.Position = 0;

                        // Create an FTP request and upload the CSV content
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServer + remoteFilePath);
                        request.Method = WebRequestMethods.Ftp.UploadFile;
                        request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                        using (Stream ftpStream = request.GetRequestStream())
                        {
                            memoryStream.CopyTo(ftpStream);
                            //LogHelper.GetInstance.WriteDebugLog($"Copied to ftpStream");
                        }

                        // Get the response from the FTP server (optional)
                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                        string uploadStatus = response.StatusDescription;
                        //LogHelper.GetInstance.WriteDebugLog($"Response from FTP server: {uploadStatus}");

                        // Close the response and memory stream
                        response.Close();
                    }

                    memoryStream.Close();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.GetInstance.WriteDebugLog($"\n{DateTime.Now}: Exception - {ex.Message}");
                //LogHelper.GetInstance.WriteDebugLog($"{ex.StackTrace}");
                throw;
            }
        }

        public void StreamCSVInFolder(DataTable dataTable, string remoteFilePath)
        {
            try
            {
                string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                string completePath = Path.Combine(attachmentLocalPath, remoteFilePath);
                string directoryPath = System.IO.Path.Combine(Server.MapPath(completePath));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(directoryPath))
                    {
                        // Write the header row
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            writer.Write('"' + column.ColumnName + '"');
                            writer.Write(ApplicationSettings.CsvDelimiter);
                        }
                        writer.WriteLine(); // Add a newline after the header row

                        // Write the data rows
                        foreach (DataRow row in dataTable.Rows)
                        {
                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                writer.Write('"' + row[i].ToString() + '"');
                                writer.Write(ApplicationSettings.CsvDelimiter);
                            }
                            writer.WriteLine(); // Add a newline after each data row
                        }

                        // Flush the StreamWriter to ensure data is written immediately
                        writer.Flush();
                        // Reset the memory stream position to the beginning
                        memoryStream.Position = 0;

                    }

                    memoryStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void StreamNewCSVInFolder(DataTable dataTable, string remoteFilePath)
        {
            try
            {
                string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                string completePath = Path.Combine(attachmentLocalPath, remoteFilePath);
                string directoryPath = System.IO.Path.Combine(Server.MapPath(completePath));
                //LogHelper.GetInstance.WriteDebugLog($"{dataTable.TableName}.CSV file creation start on  : {DateTime.Now}");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(directoryPath))
                    {

                        // Write the data rows
                        foreach (DataRow row in dataTable.Rows)
                        {
                            writer.WriteLine('"' + row[0].ToString() + '"');

                        }
                        // Flush the StreamWriter to ensure data is written immediately
                        writer.Flush();
                        // Reset the memory stream position to the beginning
                        memoryStream.Position = 0;

                    }

                    memoryStream.Close();
                }
                //LogHelper.GetInstance.WriteDebugLog($"{dataTable.TableName}.CSV file creation completed on : {DateTime.Now}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// ExportData DataTable version
        /// </summary>
        private void ExportDataToFTP(DataTable dtReport, string parentDirectory, string fileName, bool isDataContainBarcode = false)
        {
            LogHelper.GetInstance.WriteDebugLog($"\n{DateTime.Now}: Inside ExportDataToFTP. DT name: {dtReport.TableName}, parent directory: {parentDirectory}, file name: {fileName}");
            //string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder + $"{parentDirectory}/";
            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

            //using (var exportData = new MemoryStream())
            //{
            Response.Clear();
            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport, isDataContainBarcode);
                //workbook.Write(exportData);

                //string tempfileName = fileName + ".xlsx";
                string path = $"~/Uploads/Temp/ExportReportLog/{parentDirectory}";
                string directoryPath = Path.Combine(Server.MapPath(path));
                string tempfilePath = Path.Combine(Server.MapPath(path), fileName);

                if (Directory.Exists(directoryPath).Equals(false))
                    Directory.CreateDirectory(directoryPath);

                //ftpfilePath = ftpfilePath + "ExportReportLog/";
                using (FileStream xfile = new FileStream(tempfilePath, FileMode.Create, System.IO.FileAccess.Write))
                {
                    workbook.Write(xfile);
                    LogHelper.GetInstance.WriteDebugLog($"{DateTime.Now}: Local file written");
                }

                CommonUtility.FTPFileUpload(tempfilePath, fileName, ftpFilePath, ftpUserName, ftpPwd);
                LogHelper.GetInstance.WriteDebugLog($"{DateTime.Now}: File uploaded to FTP");
                ///delete the file from local server
                System.IO.File.Delete(tempfilePath);
                LogHelper.GetInstance.WriteDebugLog($"{DateTime.Now}: Local file deleted");
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                //Response.BinaryWrite(exportData.ToArray());
                //Response.End();
            }

            //Response.End();
            //}
        }

        private void ExportData(DataSet dsReport, string fileName, string ftpfilePath, string ftpUserName, string ftpPassword, bool isDataContainBarcode = false)
        {

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    LogHelper.GetInstance.WriteDebugLog("\r\n");
                    LogHelper.GetInstance.WriteDebugLog($"Start Process to write the data in excel: {DateTime.Now}");
                    IWorkbook workbook = NPOIExcelHelper.DatasetToExcel("xlsx", dsReport, isDataContainBarcode);
                    LogHelper.GetInstance.WriteDebugLog($"End Process to write the data in excel: {DateTime.Now}");
                    LogHelper.GetInstance.WriteDebugLog("\r\n");
                    //workbook.Write(exportData);

                    //string tempfileName = fileName + ".xlsx";
                    //commented by priyanka
                    //string directoryPath = Path.Combine(Server.MapPath("~/Uploads/Temp/ExportReportLog"));
                    //string tempfilePath = Path.Combine(Server.MapPath("~/Uploads/Temp/ExportReportLog"), fileName);
                    //commented by priyanka
                    string downloadTempPath = Settings.ApplicationSettings.DownloadTempPath;
                    string directoryPath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"));
                    string tempfilePath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"), fileName);
                    if (Directory.Exists(directoryPath).Equals(false))
                        Directory.CreateDirectory(directoryPath);

                    //ftpfilePath = ftpfilePath + "ExportReportLog/";
                    using (FileStream xfile = new FileStream(tempfilePath, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        workbook.Write(xfile);
                    }
                    LogHelper.GetInstance.WriteDebugLog($"FTP Connection created: {DateTime.Now}");
                    CommonUtility.FTPFileUpload(tempfilePath, fileName, ftpfilePath, ftpUserName, ftpPassword);
                    LogHelper.GetInstance.WriteDebugLog($"File uploaded and FTP Connection closed: {DateTime.Now}");
                    ///delete the file from local server
                    System.IO.File.Delete(tempfilePath);
                    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    //Response.BinaryWrite(exportData.ToArray());
                    //Response.End();
                }

                //Response.End();
            }
        }


        public JsonResult BindProviceByRegionId(ProvinceIn objProvinceIn)
        {
            objProvinceIn.userId = Convert.ToInt32(Session["user_id"]);
            var objResp = new BLLayer().GetProvinceByRegionId(objProvinceIn);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        [System.Web.Services.WebMethod(true)]
        public ActionResult EntityExportReport(ExportEntitiesReportNew objExportEntitiesReport, string IsRequestFromInfo)
        {
            //LogHelper.GetInstance.WriteDebugLog($"***********************************Summary logs start ***********************************");
            DateTime startTime = DateTime.Now;
            var userdetails = (User)Session["userDetail"];

            var moduleAbbr = "EXRPT";
            // List<ConnectionMaster> con = new BLLayer().GetConnectionString(moduleAbbr);
            ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);
            //foreach(var conn in con)
            //{
            if (con != null)
            {
                objExportEntitiesReport.objReportFilters.connectionString = con.connection_string;
            }

            // }            

            objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
            objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportEntitiesReport.objReportFilters.SelectedParentUser != null && objExportEntitiesReport.objReportFilters.SelectedParentUser.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedParentUser.ToArray()) : "";//((User)Session["userDetail"]).user_id.ToString();
            objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportEntitiesReport.objReportFilters.SelectedUserId != null && objExportEntitiesReport.objReportFilters.SelectedUserId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedUserId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedLayerIds = objExportEntitiesReport.objReportFilters.SelectedLayerId != null && objExportEntitiesReport.objReportFilters.SelectedLayerId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedLayerId.ToArray()) : "";// objExportEntitiesReport.objReportFilters.SelectedLayerId != null && objExportEntitiesReport.objReportFilters.SelectedLayerId.Count > 0 ? ("'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedLayerId.ToArray()) + "'").ToString().ToLower() : "";
            objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportEntitiesReport.objReportFilters.SelectedProjectId != null && objExportEntitiesReport.objReportFilters.SelectedProjectId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProjectId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportEntitiesReport.objReportFilters.SelectedPlanningId != null && objExportEntitiesReport.objReportFilters.SelectedPlanningId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedPlanningId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportEntitiesReport.objReportFilters.SelectedWorkOrderId != null && objExportEntitiesReport.objReportFilters.SelectedWorkOrderId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedWorkOrderId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportEntitiesReport.objReportFilters.SelectedPurposeId != null && objExportEntitiesReport.objReportFilters.SelectedPurposeId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedPurposeId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.userId = Convert.ToInt32(userdetails.user_id);
            objExportEntitiesReport.objReportFilters.roleId = Convert.ToInt32(userdetails.role_id);
            objExportEntitiesReport.objReportFilters.is_all_provience_assigned = userdetails.is_all_provience_assigned;
            //objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportEntitiesReport.objReportFilters.SelectedOwnerShipId != null && objExportEntitiesReport.objReportFilters.SelectedOwnerShipId.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedOwnerShipId.ToArray()) + "'" : "";
            objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportEntitiesReport.objReportFilters.SelectedOwnerShipType != null ? objExportEntitiesReport.objReportFilters.SelectedOwnerShipType : "";
            objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorId != null && objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.selected_route_ids = objExportEntitiesReport.selected_route_ids != null && objExportEntitiesReport.selected_route_ids.Count > 0 ? string.Join(",", objExportEntitiesReport.selected_route_ids.ToArray()) : "";
            //if (objExportEntitiesReport.objReportFilters.SelectedParentUser == null)
            //{
            //    objExportEntitiesReport.objReportFilters.SelectedParentUser = new List<int>();
            //    //objExportEntitiesReport.objReportFilters.SelectedParentUser.Add(((User)Session["userDetail"]).user_id);
            //    objExportEntitiesReport.objReportFilters.SelectedParentUser.Add(1);
            //}
            //if (objExportEntitiesReport.objReportFilters.fromDate != null || objExportEntitiesReport.objReportFilters.customDate==9)
            var selectedLayers = objExportEntitiesReport.objReportFilters.SelectedLayerIds;
            if (!string.IsNullOrEmpty(IsRequestFromInfo) && Convert.ToBoolean(IsRequestFromInfo))
            {
                //LogHelper.GetInstance.WriteDebugLog($"Request Sent to get the data from database on: {DateTime.Now}");
                //string[] layer_ids = objExportEntitiesReport.objReportFilters.SelectedLayerIds.Split(',');
                objExportEntitiesReport.lstReportData = new BLLayer().GetExportReportSummary(objExportEntitiesReport.objReportFilters).OrderBy(m => m.entity_name).ToList();

                //object lockObj = new object();
                //var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

                //            Parallel.ForEach(layer_ids, layer =>
                //            {
                //                ExportReportFilterNew exportReportFilterNew = new ExportReportFilterNew();
                //	exportReportFilterNew = objExportEntitiesReport.objReportFilters;
                //	exportReportFilterNew.SelectedLayerIds = layer;
                //	EntitySummaryReport obj = new BLLayer().GetExportReportSummary(exportReportFilterNew);
                //	if (obj != null)
                //		objExportEntitiesReport.lstReportData.Add(obj);
                //});

                //ParallelLoopResult result = Parallel.ForEach(layer_ids, options, (item) =>
                //{
                //	lock (lockObj)
                //	{
                //		objExportEntitiesReport.objReportFilters.SelectedLayerIds = item;
                //		EntitySummaryReport obj = new BLLayer().GetExportReportSummary(objExportEntitiesReport.objReportFilters);
                //		if (obj != null)
                //			objExportEntitiesReport.lstReportData.Add(obj);
                //	}
                //});
                //LogHelper.GetInstance.WriteDebugLog($"data received from database on: {DateTime.Now}");
            }
            objExportEntitiesReport.objReportFilters.SelectedLayerIds = selectedLayers;
            // objExportEntitiesReport.lstReportData = BLConvertMLanguage.ExportMultilingualConvert(objExportEntitiesReport.lstReportData);
            Session["ExportReportFilterNew"] = objExportEntitiesReport.objReportFilters;

            BindReportDropdownNew(ref objExportEntitiesReport);
            Session["EntityExportSummaryData"] = objExportEntitiesReport;
            DateTime endTime = DateTime.Now;
            //LogHelper.GetInstance.WriteDebugLog($"Total Time taking in second:{endTime.Second - startTime.Second} ");
            //LogHelper.GetInstance.WriteDebugLog($"***********************************Summary logs end ***********************************");

            return PartialView("_EntityExportReport", objExportEntitiesReport);
        }
        public void BindReportDropdownNew(ref ExportEntitiesReportNew objExportEntitiesReport)
        {
            var userdetails = (User)Session["userDetail"];
            var moduleAbbr = "EXRPT";
            objExportEntitiesReport.lstfiletypes = blExportData.getfiletype(moduleAbbr);
            //Bind Layers..
            objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
            objExportEntitiesReport.lstRouteInfo = new BLLayer().getRouteInfo("0");
            //Bind Regions..
            objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }
            //if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.layerName))
            //{
            //    objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(objExportEntitiesReport.objReportFilters.layerName);
            //}
            //for parent user list deafult all under sa
            List<int> parentUser = new List<int>();
            parentUser.Add(1);
            if (userdetails.role_id == 1 || userdetails.is_all_provience_assigned)
                objExportEntitiesReport.lstParentUsers = new BLUser().GetUsersListByMGRIds(parentUser).OrderBy(x => x.user_name).ToList();//new BLUser().GetUsersListByMGRIds(parentUser).Where(x => x.user_id == Convert.ToInt32(Session["user_id"])).OrderBy(x => x.user_name).ToList();
            else
            {
                objExportEntitiesReport.lstParentUsers = new List<Models.User>();
                objExportEntitiesReport.lstParentUsers.Add(userdetails);// new BLUser().GetUserDetailByID(Convert.ToInt32(Session["user_id"])));// new BLUser().GetUsersListByMGRIds(parentUser).Where(x=> x.user_id == Convert.ToInt32(Session["user_id"])).OrderBy(x => x.user_name).ToList();
            }
            //objExportEntitiesReport.lstParentUsers = new List<User>();
            //for user list by manager id
            //new BLUser().GetUsersListByMGRIds(objExportEntitiesReport.objReportFilters.SelectedParentUser);
            if (objExportEntitiesReport.objReportFilters.SelectedParentUser != null)
            {
                if (userdetails.role_id == 1 || userdetails.is_all_provience_assigned)
                {
                    objExportEntitiesReport.lstUsers = new BLUser().GetUsersListByMGRIds(objExportEntitiesReport.objReportFilters.SelectedParentUser).OrderBy(x => x.user_name).ToList();
                }
                else
                {
                    var parentUser_ids = string.Join(",", objExportEntitiesReport.objReportFilters.SelectedParentUser.Select(n => n.ToString()).ToArray());
                    objExportEntitiesReport.lstUsers = new BLUser().GetUserReportDetailsList(parentUser_ids).ToList();
                }
            }
            //for project code,planning code,workordercode & purpose code
            objExportEntitiesReport.lstBindProjectCode = new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedNetworkStatues) ? "P" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "PLANNED" ? "P" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "AS BUILT" ? "A" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "DORMANT" ? "D" : "P");
            if (objExportEntitiesReport.objReportFilters.SelectedProjectId != null)
                objExportEntitiesReport.lstBindPlanningCode = new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(objExportEntitiesReport.objReportFilters.SelectedProjectId);
            if (objExportEntitiesReport.objReportFilters.SelectedPlanningId != null)
                objExportEntitiesReport.lstBindWorkorderCode = new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(objExportEntitiesReport.objReportFilters.SelectedPlanningId);
            if (objExportEntitiesReport.objReportFilters.SelectedWorkOrderId != null)
                objExportEntitiesReport.lstBindPurposeCode = new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(objExportEntitiesReport.objReportFilters.SelectedWorkOrderId);
            //for duration based on 
            objExportEntitiesReport.lstDurationBasedOn = new BLMisc().GetDropDownList("", DropDownType.Export_Report.ToString());

            //objExportEntitiesReport.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objExportEntitiesReport.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objExportEntitiesReport.lstNetworkStatus = new BLMisc().GetDropDownList("", DropDownType.ddlNetworkStatus.ToString());
            objExportEntitiesReport.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
        }
        public JsonResult GetUsersByParentUser(List<int> parentUser_ids)
        {
            var objResp = new BLUser().GetUsersListByMGRIds(parentUser_ids).OrderBy(x => x.user_name).ToList();
            JsonResult result = Json(new { Data = objResp.Select(x => new { x.user_id, x.user_name }).ToList() });
            result.MaxJsonLength = Int32.MaxValue;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult GetReportUsersByParentUser(string parentUser_ids)
        {
            var objResp = new BLUser().GetUserReportDetailsList(parentUser_ids, true).ToList();
            if (((User)Session["userDetail"]).role_id == 1)
            {
                objResp = objResp.Where(x => x.groupUser != "Others").ToList();
            }
            JsonResult result = Json(new { Data = objResp.Select(x => new { x.user_id, x.user_name, x.groupUser }).ToList() });
            result.MaxJsonLength = Int32.MaxValue;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult GetPlanningByProjectids(List<int> ddlproject_ids)
        {
            var objResp = new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(ddlproject_ids);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public JsonResult GetWokorderByPlanningids(List<int> ddlplanning_ids)
        {
            var objResp = new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(ddlplanning_ids);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public JsonResult GetPurposeByWorkOrderids(List<int> ddlworkorder_ids)
        {
            var objResp = new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(ddlworkorder_ids);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        //public void DownloadEntityReportNew_obsolete(string fileType, string entityids)
        //{
        //    if (!string.IsNullOrWhiteSpace(fileType))
        //    {
        //        Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
        //        if (fileType.ToUpper() == "EXCEL")
        //        {
        //            DownloadEntityReportNewIntoExcel(entityids);
        //        }
        //        else if (fileType.ToUpper() == "PDF")
        //        {
        //            DownloadEntityReportNewIntoPDF(entityids);
        //        }
        //        else if (fileType.ToUpper() == "ALLEXCEL")
        //        {
        //            DownloadEntityReportNewIntoExcelAll(entityids);
        //        }
        //        else if (fileType.ToUpper() == "KML" || fileType.ToUpper() == "XML")
        //        {
        //            DownloadEntitySummaryIntoKMLAll(entityids, fileType.ToUpper());
        //        }
        //        else if (fileType.ToUpper() == "DXF")
        //        {
        //            DownloadEntitySummaryIntoDXFAll(entityids);
        //        }


        //        else if (fileType.ToUpper() == "ALLSHAPE")
        //        {
        //            DownloadEntityReportNewIntoShapeAll(entityids);
        //        }
        //        else if (fileType.ToUpper() == "ALLCSV" || fileType.ToUpper() == "ALLTXT")
        //        {
        //            DownloadEntityReportNewIntoCSVAll(entityids, fileType.ToUpper());
        //        }
        //    }

        //}
        //[System.Web.Services.WebMethod(true)]

        [HttpPost]
        public JsonResult DownloadEntityReportNew(string fileType, string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {
            PageMessage objMsg = new PageMessage();
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
                ////create ftp folder if not exist

                string ftpFilePath = ApplicationSettings.FTPAttachment;
                string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                string[] ftplogReportDirectory = new string[] { ftpFolder.Replace("/", "") };
                CreateNestedDirectoryOnFTP(ftpFilePath, ftpUserName, ftpPwd, ftplogReportDirectory);

                if (reportType == null || !reportType.Any())
                {
                    reportType = new List<string> { "ALL" };
                }

                if (fileType.ToUpper() == "EXCEL")
                {
                    //DownloadEntityReportNewIntoExcel(entityids);
                    DownloadEntityReportNewIntoExcelNew(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "PDF")
                {
                    //DownloadFileFromFTP(entityids);
                    DownloadEntityReportNewIntoPDFNEW(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "ALLEXCEL")
                {
                    DownloadEntityReportIntoExcelAll(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }
                else if (fileType.ToUpper() == "XML")
                {
                    //DownloadEntitySummaryIntoKMLAll(entityids, fileType.ToUpper());
                    DownloadEntitySummaryIntoKMLAllNew(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "KML")
                {
                    //DownloadEntitySummaryIntoKMLAll(entityids, fileType.ToUpper());
                    DownloadEntitySummaryIntoAllKMLNew(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }
                else if (fileType.ToUpper() == "DXF")
                {
                    //DownloadEntitySummaryIntoDXFAll(entityids);
                    DownloadEntitySummaryIntoDXFAllNew(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }


                else if (fileType.ToUpper() == "ALLSHAPE")
                {
                    //DownloadEntityReportNewIntoShapeAll(entityids);
                    //DownloadEntityReportNewIntoShapeAllNew(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                    DownloadEntityReportIntoShapeAllNew(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }
                else if (fileType.ToUpper() == "ALLTXT")
                {
                    DownloadEntityReportNewIntoCSVAll(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }
                else if (fileType.ToUpper() == "ALLCSV")
                {
                    DownloadEntityReportIntoCSVAll(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }


            }
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Request is processing in background.Please check the export report log page.";
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public DataTable GetExportReportFilter(object obj)
        {
            var userdetails = (User)Session["userDetail"];
            var isAttr = ((List<string>)Session["ApplicableModuleList"]);
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            DataTable dt = new DataTable(Resources.Resources.SI_GBL_GBL_NET_FRM_165);
            dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_166);
            dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_167);
            DataRow dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_GBL_GBL_NET_FRM_098; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_065; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_066; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_063; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_068; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_069; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_GBL_GBL_GBL_GBL_147; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_FRM_030; dt.Rows.Add(dr); dr = dt.NewRow();
            if (isAttr.Contains("PROJ"))
            {
                dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_074; dt.Rows.Add(dr); dr = dt.NewRow();
                dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_076; dt.Rows.Add(dr); dr = dt.NewRow();
                dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_010; dt.Rows.Add(dr); dr = dt.NewRow();
                dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_011; dt.Rows.Add(dr); dr = dt.NewRow();
            }
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_071; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_072; dt.Rows.Add(dr); dr = dt.NewRow();
            if (obj.GetType().GetProperty("advancefilter") != null)
            {
                if (!String.IsNullOrEmpty(obj.GetType().GetProperty("advancefilter").GetValue(obj, null).ToString()))
                {
                    int rowCount = dt.Rows.Count;
                    dr["Filter Type"] = "Add-on Filter"; dt.Rows.Add(dr); dr = dt.NewRow();
                    List<ReportAdvanceFilter> advnanceFilter = (List<ReportAdvanceFilter>)obj.GetType().GetProperty("lstAdvanceFilters").GetValue(obj, null);
                    for (int i = 0; i < advnanceFilter.Count; i++)
                    {
                        dt.Rows[rowCount + i][1] = textInfo.ToTitleCase(advnanceFilter[i].searchBy.ToString().ToLower().Replace("_", " ")) + " " + advnanceFilter[i].searchType + " '" + advnanceFilter[i].searchText + "'";
                        if ((advnanceFilter.Count - 1) != i)
                        {
                            dt.Rows.Add(dr); dr = dt.NewRow();
                        }
                    }
                }
            }

            List<int> regionIds = (List<int>)obj.GetType().GetProperty("SelectedRegionId").GetValue(obj, null);
            var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());

            List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
            var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = string.Join(",", regionIds), userId = Convert.ToInt32(Session["user_id"]) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());

            string networkStatus = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedNetworkStatues").GetValue(obj, null).ToString().Replace("AS BUILT", "AS-BUILT").ToLower()).Replace("'", "");

            string ownershipType = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedOwnerShipType").GetValue(obj, null).ToString()).Replace("'", "");
            ownershipType = string.IsNullOrEmpty(ownershipType) ? "All" : ownershipType;

            List<int> thirdPartyVendorId = (List<int>)obj.GetType().GetProperty("SelectedThirdPartyVendorId").GetValue(obj, null);
            var thirdPartyVendorName = thirdPartyVendorId == null ? "All" : string.Join(",", BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList().Where(x => thirdPartyVendorId.Contains(Convert.ToInt32(x.key))).Select(x => x.value).ToList());

            List<int> parentUser = new List<int>();
            List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
            var parentUserName = string.Empty;
            if (userdetails.role_id == 1)
            {
                parentUser.Add(1);
                parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
            }
            else
            {
                parentUserName = parentUserIds == null ? "All" : new BLUser().GetUserDetailByID(userdetails.user_id).user_name;
            }

            List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
            var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

            //rt
            //var userdetails = (User)Session["userDetail"];
            List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
            var layerName = layerIds == null ? "All" : string.Join(",", new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").Where(x => layerIds.Contains(x.layer_id)).Select(x => x.layer_title).ToList());
            var projectCodeName = "";
            var planningCodeName = "";
            var workOrderCodeName = "";
            var purposeCodeName = "";
            if (isAttr.Contains("PROJ"))
            {
                List<int> projectCodeIds = (List<int>)obj.GetType().GetProperty("SelectedProjectId").GetValue(obj, null);
                projectCodeName = projectCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrEmpty(networkStatus) ? "P" : networkStatus.ToUpper() == "PLANNED" ? "P" : networkStatus.ToUpper() == "AS BUILT" ? "A" : networkStatus.ToUpper() == "DORMANT" ? "D" : "P").Where(x => projectCodeIds.Contains(x.system_id)).Select(x => x.project_code).ToList());

                List<int> planningCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPlanningId").GetValue(obj, null);
                planningCodeName = planningCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(projectCodeIds).Where(x => planningCodeIds.Contains(x.system_id)).Select(x => x.planning_code).ToList());

                List<int> workOrderCodeIds = (List<int>)obj.GetType().GetProperty("SelectedWorkOrderId").GetValue(obj, null);
                workOrderCodeName = workOrderCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(planningCodeIds).Where(x => workOrderCodeIds.Contains(x.system_id)).Select(x => x.workorder_code).ToList());

                List<int> purposeCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPurposeId").GetValue(obj, null);
                purposeCodeName = purposeCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(workOrderCodeIds).Where(x => purposeCodeIds.Contains(x.system_id)).Select(x => x.purpose_code).ToList());
            }
            string duration = "";
            string durationBasedOn = obj.GetType().GetProperty("durationbasedon").GetValue(obj, null).ToString().Replace("_", " ");
            if (obj.GetType().GetProperty("fromDate").GetValue(obj, null) != null && obj.GetType().GetProperty("toDate").GetValue(obj, null) != null)
            {
                duration = obj.GetType().GetProperty("fromDate").GetValue(obj, null).ToString() + " To " + obj.GetType().GetProperty("toDate").GetValue(obj, null).ToString();
            }
            else
            {
                duration = "All";
            }
            if (!networkStatus.Contains("Planned"))
                networkStatus = networkStatus.Replace("P", "Planned");
            if (!networkStatus.Contains("As-Built"))
                networkStatus = networkStatus.Replace("A", "As-Built");
            if (!networkStatus.Contains("Dormant"))
                networkStatus = networkStatus.Replace("D", "Dormant");

            dt.Rows[0][1] = layerName;
            dt.Rows[1][1] = regionName;
            dt.Rows[2][1] = provinceName;
            dt.Rows[3][1] = String.IsNullOrEmpty(networkStatus) ? "All" : networkStatus;
            dt.Rows[4][1] = parentUserName;
            dt.Rows[5][1] = userName;
            //dt.Rows[5][1] = layerName;

            dt.Rows[6][1] = ownershipType;
            dt.Rows[7][1] = thirdPartyVendorName;

            if (isAttr.Contains("PROJ"))
            {
                dt.Rows[8][1] = projectCodeName;
                dt.Rows[9][1] = planningCodeName;
                dt.Rows[10][1] = workOrderCodeName;
                dt.Rows[11][1] = purposeCodeName;
                dt.Rows[12][1] = durationBasedOn;
                dt.Rows[13][1] = duration;
            }
            else
            {
                dt.Rows[8][1] = durationBasedOn;
                dt.Rows[9][1] = duration;
            }
            return dt;
        }

        public ActionResult SplitExportReport(ExportEntitiesReportNew objExportEntitiesReport, string IsRequestFromInfo)
        {
            var userdetails = (User)Session["userDetail"];
            var moduleAbbr = "SPLIT_EXRPT";
            string selectedLayers = GetReportdata(objExportEntitiesReport, userdetails, moduleAbbr);
            if (!string.IsNullOrEmpty(IsRequestFromInfo) && Convert.ToBoolean(IsRequestFromInfo))
            {
                objExportEntitiesReport.lstReportData = new BLLayer().GetSplitReportSummary(objExportEntitiesReport.objReportFilters).ToList();
            }
            objExportEntitiesReport.objReportFilters.SelectedLayerIds = selectedLayers;
            BindReportDropdown(ref objExportEntitiesReport, moduleAbbr);
            return PartialView("_SplitLineEntityExportReport", objExportEntitiesReport);
        }

        [HttpPost]
        public ActionResult SplitEntityExportReport(ExportEntitiesReportNew objExportEntitiesReport, string IsRequestFromInfo)
        {
            var userdetails = (User)Session["userDetail"];
            var moduleAbbr = "SPLIT_EXRPT";
            string selectedLayers = GetReportdata(objExportEntitiesReport, userdetails, moduleAbbr);
            objExportEntitiesReport.objReportFilters.SelectedLayerIds = selectedLayers;

            if (!string.IsNullOrEmpty(IsRequestFromInfo) && Convert.ToBoolean(IsRequestFromInfo))
            {
                objExportEntitiesReport.lstReportData = new BLLayer().GetSplitReportSummary(objExportEntitiesReport.objReportFilters).ToList();
            }
            Session["SplitExportReportFilter"] = objExportEntitiesReport.objReportFilters;
            BindReportDropdown(ref objExportEntitiesReport, moduleAbbr);
            Session["SplitEntitySummaryData"] = objExportEntitiesReport;
            return PartialView("_SplitLineEntityExportReport", objExportEntitiesReport);
        }
        public void BindReportDropdown(ref ExportEntitiesReportNew objExportEntitiesReport, string moduleAbbr)
        {
            var userdetails = (User)Session["userDetail"];
            objExportEntitiesReport.lstfiletypes = blExportData.getfiletype(moduleAbbr);
            objExportEntitiesReport.lstLayers = (moduleAbbr == "AUDIT_HISTORY_EXRPT") ? new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").ToList() : new BLLayer().GetSplitReportLayers(userdetails.role_id, "ENTITY");
            objExportEntitiesReport.lstRouteInfo = new BLLayer().getRouteInfo("0");
            objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }
            List<int> parentUser = new List<int>();
            parentUser.Add(1);
            if (userdetails.role_id == 1 || userdetails.is_all_provience_assigned)
                objExportEntitiesReport.lstParentUsers = new BLUser().GetUsersListByMGRIds(parentUser).OrderBy(x => x.user_name).ToList();
            else
            {
                objExportEntitiesReport.lstParentUsers = new List<Models.User>();
                objExportEntitiesReport.lstParentUsers.Add(userdetails);
            }
            if (objExportEntitiesReport.objReportFilters.SelectedParentUser != null)
            {
                if (userdetails.role_id == 1 || userdetails.is_all_provience_assigned)
                {
                    objExportEntitiesReport.lstUsers = new BLUser().GetUsersListByMGRIds(objExportEntitiesReport.objReportFilters.SelectedParentUser).OrderBy(x => x.user_name).ToList();
                }
                else
                {
                    var parentUser_ids = string.Join(",", objExportEntitiesReport.objReportFilters.SelectedParentUser.Select(n => n.ToString()).ToArray());
                    objExportEntitiesReport.lstUsers = new BLUser().GetUserReportDetailsList(parentUser_ids).ToList();
                }
            }
            objExportEntitiesReport.lstBindProjectCode = new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedNetworkStatues) ? "P" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "PLANNED" ? "P" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "AS BUILT" ? "A" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "DORMANT" ? "D" : "P");
            if (objExportEntitiesReport.objReportFilters.SelectedProjectId != null)
                objExportEntitiesReport.lstBindPlanningCode = new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(objExportEntitiesReport.objReportFilters.SelectedProjectId);
            if (objExportEntitiesReport.objReportFilters.SelectedPlanningId != null)
                objExportEntitiesReport.lstBindWorkorderCode = new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(objExportEntitiesReport.objReportFilters.SelectedPlanningId);
            if (objExportEntitiesReport.objReportFilters.SelectedWorkOrderId != null)
                objExportEntitiesReport.lstBindPurposeCode = new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(objExportEntitiesReport.objReportFilters.SelectedWorkOrderId);
            objExportEntitiesReport.lstDurationBasedOn = new BLMisc().GetDropDownList("", DropDownType.Export_Report.ToString());
            objExportEntitiesReport.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objExportEntitiesReport.lstNetworkStatus = new BLMisc().GetDropDownList("", DropDownType.ddlNetworkStatus.ToString());
            objExportEntitiesReport.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
        }
        [HttpPost]
        public JsonResult DownloadSpliReport(string fileType, string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {
            PageMessage objMsg = new PageMessage();
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
                string ftpFilePath = ApplicationSettings.FTPAttachment;
                string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                string[] ftplogReportDirectory = new string[] { ftpFolder.Replace("/", "") };
                CreateNestedDirectoryOnFTP(ftpFilePath, ftpUserName, ftpPwd, ftplogReportDirectory);

                if (reportType == null || !reportType.Any())
                {
                    reportType = new List<string> { "ALL" };
                }

                if (fileType.ToUpper() == "EXCEL")
                {
                    DownloadEntitySplitReportIntoExcel(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "ALLEXCEL")
                {
                    DownloadSplitReportIntoExcelAll(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }
                else if (fileType.ToUpper() == "ALLCSV")
                {
                    DownloadSplitReportIntoCSVAll(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }
                else if (fileType.ToUpper() == "ALLSHAPE")
                {
                    DownloadSplitReportIntoShapeAll(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }
            }
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Request is processing in background.Please check the export report log page.";
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public void DownloadEntitySplitReportIntoExcel(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["SplitEntitySummaryData"] != null)
            {
                try
                {
                    var userdetails = (User)Session["userDetail"];
                    ExportEntitiesReportNew objExportEntitiesReport = new ExportEntitiesReportNew();
                    objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["SplitExportReportFilter"];
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    string fileName = "SplitSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");

                    objExportEntitiesReport = (ExportEntitiesReportNew)Session["SplitEntitySummaryData"];
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        List<EntitySummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                        if (objExportEntitiesReport.objReportFilters.SelectedLayerId != null)
                            objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);
                        dtReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                        objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                        objExportEntitiesReport.lstReportData = lstRprtData;
                        DataSet ds = new DataSet();
                        ds.Tables.Add(dtFilter);

                        int TotalEntityReport = 0;
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = "Excel";
                        exportReportLog.file_extension = ".xlsx";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        try
                        {
                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (!ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns.Remove("DORMANT_COUNT");
                                }
                                dtReport.Columns.Remove("entity_id");
                                dtReport.Columns.Remove("entity_name");
                                dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                dtReport.Columns["planned_count"].ColumnName = "Planned";
                                dtReport.Columns["as_built_count"].ColumnName = "As-Built";
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns["dormant_count"].ColumnName = "Dormant";
                                }

                                string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                if (networkstatusvalues.Length < 3)
                                {
                                    if (!networkstatusvalues.Contains("P"))
                                    {
                                        dtReport.Columns.Remove("PLANNED");
                                    }
                                    if (!networkstatusvalues.Contains("A"))
                                    {
                                        dtReport.Columns.Remove("AS-BUILT");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (!networkstatusvalues.Contains("D"))
                                        {
                                            dtReport.Columns.Remove("DORMANT");
                                        }
                                    }
                                }
                                DataRow row = dtReport.NewRow();
                                row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    row["Planned"] = dtReport.Compute("Sum(Planned)", "");
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    row["As-Built"] = dtReport.Compute("Sum([As-Built])", "");
                                }
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        row["Dormant"] = dtReport.Compute("Sum(Dormant)", "");
                                    }
                                }
                                dtReport.Rows.Add(row);
                                ds.Tables.Add(dtReport);
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    totalPlannedCount = Convert.ToInt32(row["Planned"]);
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    totalAsBuiltCount = Convert.ToInt32(row["As-Built"]);
                                }
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        totalDormantCount = Convert.ToInt32(row["Dormant"]);
                                    }
                                }
                            }

                            string tempFileName = fileName + exportReportLog.file_extension;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);

                            exportReportLog.planned = totalPlannedCount;
                            exportReportLog.asbuilt = totalAsBuiltCount;
                            exportReportLog.dormant = totalDormantCount;
                            exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntitySplitReportIntoExcel()", "Report", ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void DownloadSplitReportIntoExcelAll(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {
            if (Session["SplitExportReportFilter"] != null)
            {
                try
                {
                    ExportEntitiesReportNew entityExportSummaryData;
                    ExportEntitiesSummaryView objExportEntitiesReport;
                    ExportReportFilterNew objExportReportFilterNew;
                    List<int> SelectedLayerId, SelectedLayerIdSummary;
                    DataTable dtFilter;
                    GetDataExcelAllandCSVAll(entityids, out entityExportSummaryData, out objExportEntitiesReport, out objExportReportFilterNew, out SelectedLayerId, out SelectedLayerIdSummary, out dtFilter);

                    var userdetails = (User)Session["userDetail"];
                    //objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY")
                    //   .Where(layer => new List<string> { "Cable", "Trench", "Duct" }.Contains(layer.layer_name)).ToList();
                    objExportEntitiesReport.lstLayers = new BLLayer().GetSplitReportLayers(userdetails.role_id, "ENTITY").ToList();
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }

                    string parentFolder = $"SplitReport_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                    string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                    string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                    string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));

                    if (Directory.Exists(directoryPath).Equals(false))
                        Directory.CreateDirectory(directoryPath);
                    string fileName = $"{dtFilter.TableName}";
                    string tempFileName = $"{directoryPath}/{dtFilter.TableName}.xlsx";
                    ExportDataNew(dtFilter, fileName, tempFileName);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = parentFolder;
                        exportReportLog.file_type = "ALLEXCEL";
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        dtFilter = null;
                        try
                        {
                            var tasks = new List<Task>();
                            foreach (var layer in objExportEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        var layer_name = layer.layer_name;
                                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                        EntitySummaryReport recordCount = entityExportSummaryData.lstReportData.Where(x => x.entity_name.ToUpper() == layer.layer_name.ToUpper()).FirstOrDefault();
                                        int total_entity_count = 0;
                                        if (recordCount != null)
                                            total_entity_count = recordCount.planned_count + recordCount.as_built_count + recordCount.dormant_count;

                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;

                                        List<string> reportTypeString = reportType;
                                        lstExportEntitiesDetail = new BLLayer().GetSplitReportSummaryViewAllExcel(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        DataTable dtReport = new DataTable();
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType);
                                        dtReport.TableName = layer.layer_title;
                                        if (dtReport.Rows.Count > 0)
                                        {
                                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                            objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                                            if (dtReport.Rows.Count > 0)
                                            {
                                                IWorkbook workbook = new XSSFWorkbook();
                                                fileName = $"{dtReport.TableName}";
                                                tempFileName = $"{directoryPath}/{dtReport.TableName}.xlsx";
                                                SplitDataExcelMerge(workbook, dtReport, fileName, tempFileName);
                                            }
                                            exportReportLog.export_ended_on = DateTime.Now;
                                            exportReportLog.status = "Success";
                                            exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
                                            dtReport = null;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));
                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();
                            string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                            string zipfilePath = directoryPath + ".zip";
                            string fileNameValue = parentFolder + ".zip";
                            using (var zip = new ZipFile())
                            {
                                zip.AddDirectory(directoryPath);
                                zip.Save(zipfilePath);
                            }
                            if (System.IO.File.Exists(zipfilePath))
                            {
                                string fileZipName = Path.GetFileName(zipfilePath);
                                Directory.Delete(directoryPath, true);
                            }
                            FileInfo file = new FileInfo(zipfilePath);
                            CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);
                            System.IO.File.Delete(zipfilePath);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadSplitReportIntoExcelAll()", "Report", ex);
                            if (Directory.Exists(directoryPath).Equals(true))
                                Directory.Delete(directoryPath, true);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadSplitReportIntoCSVAll(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {
            if (Session["SplitExportReportFilter"] != null)
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                    ExportEntitiesReportNew ExportEntitiesReportSummary = new ExportEntitiesReportNew();
                    ExportEntitiesReportNew entityExportSummaryData;
                    ExportReportFilterNew objExportReportFilterNew;
                    List<int> SelectedLayerId, SelectedLayerIdSummary;
                    DataTable dtFilter;
                    GetDataExcelAllandCSVAll(entityids, out entityExportSummaryData, out objExportEntitiesReport, out objExportReportFilterNew, out SelectedLayerId, out SelectedLayerIdSummary, out dtFilter);

                    var userdetails = (User)Session["userDetail"];
                    //objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY")
                    //   .Where(layer => new List<string> { "Cable", "Trench", "Duct" }.Contains(layer.layer_name)).ToList();
                    objExportEntitiesReport.lstLayers = new BLLayer().GetSplitReportLayers(userdetails.role_id, "ENTITY").ToList();
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    string parentFolder = $"SplitReport_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                    string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                    string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                    string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));
                    if (Directory.Exists(directoryPath).Equals(false))
                        Directory.CreateDirectory(directoryPath);

                    string fileName = $"{parentFolder}/{dtFilter.TableName}.csv";
                    StreamCSVInFolder(dtFilter, fileName);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = parentFolder;
                        exportReportLog.file_type = fileType;
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        dtFilter = null;
                        try
                        {
                            List<EntitySummaryReport> lstRprtData = ExportEntitiesReportSummary.lstReportData;
                            if (lstRprtData.Count > 0)
                            {
                                if (!ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                                    ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                                if (!ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                                    ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                                if (!ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                                    ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");

                                DataTable dtSummaryReport = new DataTable();
                                dtSummaryReport = MiscHelper.ListToDataTable(ExportEntitiesReportSummary.lstReportData);
                                dtSummaryReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                                if (dtSummaryReport != null && dtSummaryReport.Rows.Count > 0)
                                {
                                    if (!ApplicationSettings.IsDormantEnabled)
                                    {
                                        dtSummaryReport.Columns.Remove("DORMANT_COUNT");
                                    }
                                    dtSummaryReport.Columns.Remove("entity_id");
                                    dtSummaryReport.Columns.Remove("entity_name");
                                    dtSummaryReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                    dtSummaryReport.Columns["planned_count"].ColumnName = "Planned";
                                    dtSummaryReport.Columns["as_built_count"].ColumnName = "As-Built";
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        dtSummaryReport.Columns["dormant_count"].ColumnName = "Dormant";
                                    }

                                    string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                    if (networkstatusvalues.Length < 3)
                                    {
                                        if (!networkstatusvalues.Contains("P"))
                                        {
                                            dtSummaryReport.Columns.Remove("PLANNED");
                                        }
                                        if (!networkstatusvalues.Contains("A"))
                                        {
                                            dtSummaryReport.Columns.Remove("AS-BUILT");
                                        }
                                        if (ApplicationSettings.IsDormantEnabled)
                                        {
                                            if (!networkstatusvalues.Contains("D"))
                                            {
                                                dtSummaryReport.Columns.Remove("DORMANT");
                                            }
                                        }
                                    }
                                    DataRow row = dtSummaryReport.NewRow();
                                    row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                                    if (dtSummaryReport.Columns.Contains("Planned"))
                                    {
                                        row["Planned"] = dtSummaryReport.Compute("Sum(Planned)", "");
                                    }
                                    if (dtSummaryReport.Columns.Contains("As-Built"))
                                    {
                                        row["As-Built"] = dtSummaryReport.Compute("Sum([As-Built])", "");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (dtSummaryReport.Columns.Contains("Dormant"))
                                        {
                                            row["Dormant"] = dtSummaryReport.Compute("Sum(Dormant)", "");
                                        }
                                    }
                                    dtSummaryReport.Rows.Add(row);
                                    string summaryFileName = $"{parentFolder}/{dtSummaryReport.TableName}.csv";
                                    StreamCSVInFolder(dtSummaryReport, summaryFileName);
                                }
                            }
                            var tasks = new List<Task>();
                            foreach (var layer in objExportEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        var layerdetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                        if (layerdetails != null)
                                        {
                                            if (layerdetails.is_dynamic_control_enable == null)
                                            {
                                                layerdetails.is_dynamic_control_enable = false;
                                            }
                                        }
                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                        List<string> reportTypeString = reportType;
                                        lstExportEntitiesDetail = new BLLayer().GetSplitReportSummaryViewAllCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        DataTable dtReport = new DataTable();
                                        if (lstExportEntitiesDetail != null && lstExportEntitiesDetail.Count > 0)
                                        {
                                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                                            dtReport.TableName = layer.layer_title;
                                        }
                                        objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                        objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                                        if (dtReport.Rows.Count > 0)
                                        {
                                            fileName = $"{parentFolder}/{layer.layer_title + "_SplitReport"}.csv";
                                            StreamNewCSVInFolder(dtReport, fileName);
                                            exportReportLog.export_ended_on = DateTime.Now;
                                            exportReportLog.status = "Success";
                                            exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
                                        }
                                        dtReport = null;
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));
                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();

                            string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                            string zipfilePath = directoryPath + ".zip";
                            string fileNameValue = parentFolder + ".zip";

                            using (var zip = new ZipFile())
                            {
                                zip.AddDirectory(directoryPath);
                                zip.Save(zipfilePath);
                            }
                            if (System.IO.File.Exists(zipfilePath))
                            {
                                string fileZipName = Path.GetFileName(zipfilePath);
                                Directory.Delete(directoryPath, true);
                            }
                            FileInfo file = new FileInfo(zipfilePath);
                            CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);
                            System.IO.File.Delete(zipfilePath);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            DateTime endTime = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadSplitReportIntoCSVAll()", "Report", ex);
                            if (Directory.Exists(directoryPath).Equals(true))
                                Directory.Delete(directoryPath, true);
                        }
                    });
                }
                catch (Exception ex)
                { throw ex; }
            }
        }
        public void DownloadSplitReportIntoShapeAll(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {
            if (Session["SplitExportReportFilter"] != null)
            {
                try
                {
                    ExportEntitiesSummaryView objExportEntitiesReport;
                    ExportReportFilterNew objExportReportFilterNew;
                    ExportEntitiesReportNew entityExportSummaryData;
                    List<int> SelectedLayerId, SelectedLayerIdSummary;
                    Getdata(entityids, out objExportEntitiesReport, out objExportReportFilterNew, out entityExportSummaryData, out SelectedLayerId, out SelectedLayerIdSummary);
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY")
                       .Where(layer => new List<string> { "Cable", "Trench", "Duct" }.Contains(layer.layer_name)).ToList();
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).DistinctBy(o => o.layer_id).ToList();
                    }
                    DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_type = "ALLSHAPE";
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        dtFilter = null;
                        string shapeFilePath = "";

                        try
                        {

                            string tempFileName = String.Empty;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                            string parentFolder = $"Shape_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                            string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                            string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                            shapeFilePath = Path.Combine(Server.MapPath(pathWithParentFolder));

                            if (Directory.Exists(shapeFilePath).Equals(false))
                                Directory.CreateDirectory(shapeFilePath);

                            var tasks = new List<Task>();
                            foreach (var layer in objExportEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                        EntitySummaryReport recordCount = entityExportSummaryData.lstReportData.Where(x => x.entity_name.ToUpper() == layer.layer_name.ToUpper()).FirstOrDefault();
                                        int total_entity_count = 0;
                                        if (recordCount != null)
                                            total_entity_count = recordCount.planned_count + recordCount.as_built_count + recordCount.dormant_count;
                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                        var layerdetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                        List<string> reportTypeString = reportType;
                                        lstExportEntitiesDetail = new BLLayer().GetSplitReportSummaryViewAllShape(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        DataTable dtReport = new DataTable();
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType);
                                        dtReport.TableName = layer.layer_title;
                                        string fileName = $"{parentFolder}.csv";
                                        if (lstExportEntitiesDetail != null && lstExportEntitiesDetail.Count > 0)
                                        {
                                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                                            dtReport.TableName = layer.layer_title;
                                        }
                                        if (dtReport.Rows.Count > 0)
                                        {
                                            fileName = $"{parentFolder}/{layer.layer_title}.xlsx";
                                            var tempshapeFilePath = $"{shapeFilePath}/{layer.layer_title}.xlsx";
                                            SplitDataExcelMergeWithoutCdb(dtReport, fileName, tempshapeFilePath);
                                        }
                                        dtReport = null;
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));

                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();
                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;

                            string zipshapePath = shapeFilePath + ".zip";
                            using (var zip = new ZipFile())
                            {
                                zip.AddDirectory(shapeFilePath);
                                zip.Save(zipshapePath);
                            }
                            if (System.IO.File.Exists(zipshapePath))
                            {
                                string fileName = Path.GetFileName(zipshapePath);
                                Directory.Delete(shapeFilePath, true);
                            }
                            FileInfo file = new FileInfo(zipshapePath);
                            tempFileName = Path.GetFileNameWithoutExtension(file.FullName);
                            CommonUtility.FTPFileUpload(zipshapePath, (tempFileName + ".zip"), ftpFilePath, ftpUserName, ftpPwd);
                            System.IO.File.Delete(zipshapePath);

                            exportReportLog.file_name = tempFileName;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName + exportReportLog.file_extension;
                            //Thread.Sleep(10000);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadSplitReportIntoShapeAll()", "Report", ex);
                            if (Directory.Exists(shapeFilePath).Equals(true))
                                Directory.Delete(shapeFilePath, true);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private void Getdata(string entityids, out ExportEntitiesSummaryView objExportEntitiesReport, out ExportReportFilterNew objExportReportFilterNew, out ExportEntitiesReportNew entityExportSummaryData, out List<int> SelectedLayerId, out List<int> SelectedLayerIdSummary)
        {
            objExportEntitiesReport = new ExportEntitiesSummaryView();
            objExportReportFilterNew = new ExportReportFilterNew();
            objExportReportFilterNew = (ExportReportFilterNew)Session["SplitExportReportFilter"];

            entityExportSummaryData = new ExportEntitiesReportNew();
            entityExportSummaryData = (ExportEntitiesReportNew)Session["SplitEntitySummaryData"];

            objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
            objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
            objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
            objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
            objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
            objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
            objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
            objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
            objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
            objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
            objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
            objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
            objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
            objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
            objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
            objExportEntitiesReport.objReportFilters.currentPage = 0;
            objExportEntitiesReport.objReportFilters.fileType = "SHAPE";
            objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
            objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
            objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
            objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
            objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

            SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
            SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;
            objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
            objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;
        }
        private void GetDataExcelAllandCSVAll(string entityids, out ExportEntitiesReportNew entityExportSummaryData, out ExportEntitiesSummaryView objExportEntitiesReport, out ExportReportFilterNew objExportReportFilterNew, out List<int> SelectedLayerId, out List<int> SelectedLayerIdSummary, out DataTable dtFilter)
        {
            entityExportSummaryData = new ExportEntitiesReportNew();
            entityExportSummaryData = (ExportEntitiesReportNew)Session["SplitEntitySummaryData"];
            objExportEntitiesReport = new ExportEntitiesSummaryView();
            objExportReportFilterNew = new ExportReportFilterNew();
            objExportReportFilterNew = (ExportReportFilterNew)Session["SplitExportReportFilter"];

            objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
            objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
            objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
            objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
            objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
            objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
            objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
            objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
            objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
            objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
            objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
            objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
            objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
            objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
            objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
            objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
            objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
            objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
            objExportEntitiesReport.objReportFilters.selected_route_ids = objExportReportFilterNew.selected_route_ids;
            objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
            objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

            objExportEntitiesReport.objReportFilters.currentPage = 0;
            SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
            SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;
            objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
            objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

            dtFilter = GetExportReportFilter(objExportReportFilterNew);
        }
        private void SplitDataExcelMergeWithoutCdb(DataTable dtReport, string fileName, string tempfileName)
        {
            using (var exportData = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = "SplitReport";

                    ISheet sheet1 = workbook.CreateSheet("SplitReport");
                    NPOIExcelHelper.DataTableToSheet(dtReport, sheet1);
                }
                workbook.Write(exportData);
                FileStream xfile = new FileStream(tempfileName, FileMode.Create, System.IO.FileAccess.Write);

                workbook.Write(xfile);
                xfile.Close();

            }
        }
        private void SplitDataExcelMerge(IWorkbook workbook, DataTable dtReport, string fileName, string tempfileName)
        {
            using (var exportData = new MemoryStream())
            {
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    ISheet sheet1 = workbook.CreateSheet("SplitReport");
                    NPOIExcelHelper.DataTableToSheet(dtReport, sheet1);
                }
                // Write the workbook to the MemoryStream
                workbook.Write(exportData);
                // Write the MemoryStream to the file
                FileStream xfile = new FileStream(tempfileName, FileMode.Create, System.IO.FileAccess.Write);
                workbook.Write(xfile);
                xfile.Close();

            }
        }
        private static string GetReportdata(ExportEntitiesReportNew objExportEntitiesReport, User userdetails, string moduleAbbr)
        {
            ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);
            if (con != null)
            {
                objExportEntitiesReport.objReportFilters.connectionString = con.connection_string;
            }
            objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
            objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportEntitiesReport.objReportFilters.SelectedParentUser != null && objExportEntitiesReport.objReportFilters.SelectedParentUser.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedParentUser.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportEntitiesReport.objReportFilters.SelectedUserId != null && objExportEntitiesReport.objReportFilters.SelectedUserId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedUserId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedLayerIds = objExportEntitiesReport.objReportFilters.SelectedLayerId != null && objExportEntitiesReport.objReportFilters.SelectedLayerId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedLayerId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportEntitiesReport.objReportFilters.SelectedProjectId != null && objExportEntitiesReport.objReportFilters.SelectedProjectId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProjectId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportEntitiesReport.objReportFilters.SelectedPlanningId != null && objExportEntitiesReport.objReportFilters.SelectedPlanningId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedPlanningId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportEntitiesReport.objReportFilters.SelectedWorkOrderId != null && objExportEntitiesReport.objReportFilters.SelectedWorkOrderId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedWorkOrderId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportEntitiesReport.objReportFilters.SelectedPurposeId != null && objExportEntitiesReport.objReportFilters.SelectedPurposeId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedPurposeId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.userId = Convert.ToInt32(userdetails.user_id);
            objExportEntitiesReport.objReportFilters.roleId = Convert.ToInt32(userdetails.role_id);
            objExportEntitiesReport.objReportFilters.is_all_provience_assigned = userdetails.is_all_provience_assigned;
            objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportEntitiesReport.objReportFilters.SelectedOwnerShipType != null ? objExportEntitiesReport.objReportFilters.SelectedOwnerShipType : "";
            objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorId != null && objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.selected_route_ids = objExportEntitiesReport.selected_route_ids != null && objExportEntitiesReport.selected_route_ids.Count > 0 ? string.Join(",", objExportEntitiesReport.selected_route_ids.ToArray()) : "";
            var selectedLayers = objExportEntitiesReport.objReportFilters.SelectedLayerIds;
            return selectedLayers;
        }

        public void DownloadEntityReportNewIntoExcel(string entityids)
        {
            if (Session["EntityExportSummaryData"] != null)//ExportReportFilterNew
            {
                try
                {
                    ExportEntitiesReportNew objExportEntitiesReport = new ExportEntitiesReportNew();

                    //objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["ExportReportFilterNew"];
                    //if (entityids != "")
                    //{
                    //    objExportEntitiesReport.objReportFilters.SelectedLayerIds = entityids;
                    //}
                    //objExportEntitiesReport.lstReportData = new BLLayer().GetExportReportSummary(objExportEntitiesReport.objReportFilters).OrderBy(m => m.entity_name).ToList();

                    objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["ExportReportFilterNew"];// for filter
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    objExportEntitiesReport = (ExportEntitiesReportNew)Session["EntityExportSummaryData"];
                    List<EntitySummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                    if (objExportEntitiesReport.objReportFilters.SelectedLayerId != null)
                        objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);
                    dtReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportEntitiesReport.lstReportData = lstRprtData;
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (!ApplicationSettings.IsDormantEnabled)
                        {
                            dtReport.Columns.Remove("DORMANT_COUNT");
                        }
                        dtReport.Columns.Remove("entity_id");
                        dtReport.Columns.Remove("entity_name");
                        dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                        dtReport.Columns["planned_count"].ColumnName = "Planned";
                        dtReport.Columns["as_built_count"].ColumnName = "As-Built";
                        if (ApplicationSettings.IsDormantEnabled)
                        {
                            dtReport.Columns["dormant_count"].ColumnName = "Dormant";
                        }

                        string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                        if (networkstatusvalues.Length < 3)
                        {
                            if (!networkstatusvalues.Contains("PLANNED"))
                            {
                                dtReport.Columns.Remove("PLANNED");
                            }
                            if (!networkstatusvalues.Contains("AS BUILT"))
                            {
                                dtReport.Columns.Remove("AS-BUILT");
                            }
                            if (ApplicationSettings.IsDormantEnabled)
                            {
                                if (!networkstatusvalues.Contains("DORMANT"))
                                {
                                    dtReport.Columns.Remove("DORMANT");
                                }
                            }
                        }
                        DataRow row = dtReport.NewRow();
                        row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                        if (dtReport.Columns.Contains("Planned"))
                        {
                            row["Planned"] = dtReport.Compute("Sum(Planned)", "");
                        }
                        if (dtReport.Columns.Contains("As-Built"))
                        {
                            row["As-Built"] = dtReport.Compute("Sum([As-Built])", "");
                        }
                        if (ApplicationSettings.IsDormantEnabled)
                        {
                            if (dtReport.Columns.Contains("Dormant"))
                            {
                                row["Dormant"] = dtReport.Compute("Sum(Dormant)", "");
                            }
                        }
                        dtReport.Rows.Add(row);
                        ds.Tables.Add(dtReport);
                        ExportData(ds, "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                        //ExportData(dtReport,"Entity_Summary_Report_" + MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadEntityReportNewIntoExcelNew(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["EntityExportSummaryData"] != null)//ExportReportFilterNew
            {

                try
                {
                    var userdetails = (User)Session["userDetail"];
                    ExportEntitiesReportNew objExportEntitiesReport = new ExportEntitiesReportNew();

                    //objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["ExportReportFilterNew"];
                    //if (entityids != "")
                    //{
                    //    objExportEntitiesReport.objReportFilters.SelectedLayerIds = entityids;
                    //}
                    //objExportEntitiesReport.lstReportData = new BLLayer().GetExportReportSummary(objExportEntitiesReport.objReportFilters).OrderBy(m => m.entity_name).ToList();

                    objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["ExportReportFilterNew"];// for filter
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    string fileName = "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");


                    objExportEntitiesReport = (ExportEntitiesReportNew)Session["EntityExportSummaryData"];
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        List<EntitySummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                        if (objExportEntitiesReport.objReportFilters.SelectedLayerId != null)
                            objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);
                        dtReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                        objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                        objExportEntitiesReport.lstReportData = lstRprtData;
                        DataSet ds = new DataSet();
                        ds.Tables.Add(dtFilter);


                        int TotalEntityReport = 0;
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = "Excel";
                        exportReportLog.file_extension = ".xlsx";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        try
                        {

                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (!ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns.Remove("DORMANT_COUNT");
                                }
                                dtReport.Columns.Remove("entity_id");
                                dtReport.Columns.Remove("entity_name");
                                dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                dtReport.Columns["planned_count"].ColumnName = "Planned";
                                dtReport.Columns["as_built_count"].ColumnName = "As-Built";
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns["dormant_count"].ColumnName = "Dormant";
                                }

                                string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                if (networkstatusvalues.Length < 3)
                                {
                                    if (!networkstatusvalues.Contains("P"))
                                    {
                                        dtReport.Columns.Remove("PLANNED");
                                    }
                                    if (!networkstatusvalues.Contains("A"))
                                    {
                                        dtReport.Columns.Remove("AS-BUILT");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (!networkstatusvalues.Contains("D"))
                                        {
                                            dtReport.Columns.Remove("DORMANT");
                                        }
                                    }
                                }
                                DataRow row = dtReport.NewRow();
                                row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    row["Planned"] = dtReport.Compute("Sum(Planned)", "");
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    row["As-Built"] = dtReport.Compute("Sum([As-Built])", "");
                                }
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        row["Dormant"] = dtReport.Compute("Sum(Dormant)", "");
                                    }
                                }
                                dtReport.Rows.Add(row);
                                ds.Tables.Add(dtReport);
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    totalPlannedCount = Convert.ToInt32(row["Planned"]);
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    totalAsBuiltCount = Convert.ToInt32(row["As-Built"]);
                                }
                                //totalDormantCount = Convert.ToInt32(row["Dormant"]);
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        totalDormantCount = Convert.ToInt32(row["Dormant"]);
                                    }
                                }
                                //commented by pk
                                //dtReport=Utility.CommonUtility.GetFormattedDataTable(dtReport,ApplicationSettings.numberFormatType);
                                //end commented by pk
                                //ExportData(ds,"ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss") );
                                //ExportData(dtReport,"Entity_Summary_Report_" + MiscHelper.getTimeStamp());
                            }

                            string tempFileName = fileName + exportReportLog.file_extension;
                            //string ftpFolder = "ExportReportLog/";
                            //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);

                            exportReportLog.planned = totalPlannedCount;
                            exportReportLog.asbuilt = totalAsBuiltCount;
                            exportReportLog.dormant = totalDormantCount;
                            exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            //Thread.Sleep(10000);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoExcelNew()", "Report", ex);
                        }
                    });

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadFiberAllocationReportIntoExcel(ExportEntitiesReport objExportEntitiesReport)
        {
            try
            {
                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToList().Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
                    objExportEntitiesReport.objReportFilters.pageSize = 10;
                    List<CablerouteInfo> lstrouteconnectionInfo = new List<CablerouteInfo>();
                    lstrouteconnectionInfo = new BLOSPSplicing().GetCablerouteInfo(objExportEntitiesReport.objReportFilters).lstConnectionInfo != null ? new BLOSPSplicing().GetCablerouteInfo(objExportEntitiesReport.objReportFilters).lstConnectionInfo.ToList() : new List<CablerouteInfo>();
                    var filename = "ConnectionRoute" + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";

                    string tempFileName = filename;
                    string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                    string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                    string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                    ExportReportLog exportReportLog = new ExportReportLog();
                    exportReportLog.user_id = 5; //userdetails.user_id;
                    exportReportLog.export_started_on = DateTime.Now;
                    exportReportLog.file_name = filename;
                    exportReportLog.file_type = "Excel";
                    exportReportLog.file_extension = ".xlsx";
                    exportReportLog.status = "InProgress";
                    exportReportLog.applied_filter = null;//JsonConvert.SerializeObject(dtFilter);
                    exportReportLog.file_location = ftpFolder + tempFileName;
                    exportReportLog.log_type = "FiberAllocation";
                    exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                    try
                    {
                        IWorkbook workbook = new XSSFWorkbook(); int i = 1; int j = 1;

                        var distinctFmsId = lstrouteconnectionInfo.GroupBy(m => m.fms_id).Select(group => group.First()).ToList().Select(m => m.source_network_id).ToList();
                        var cellstyle = NPOIExcelHelper.getCellStyle(workbook);
                        ISheet sheet1 = workbook.CreateSheet("Summary");
                        IRow currRowH = sheet1.CreateRow(0);
                        NPOIExcelHelper.CreateCustomCellFiberAllocation(currRowH, 0, "FMS Id", cellstyle, true, false);
                        NPOIExcelHelper.CreateCustomCellFiberAllocation(currRowH, 1, "Sheet Name", cellstyle, true, false);
                        foreach (var item in distinctFmsId)
                        {
                            ICellStyle hyperlinkStyle = workbook.CreateCellStyle();
                            IFont hyperlinkFont = workbook.CreateFont();
                            hyperlinkFont.Underline = FontUnderlineType.Single; // Underline the text
                            hyperlinkFont.Color = IndexedColors.Blue.Index; // Set the text color to blue
                            hyperlinkStyle.SetFont(hyperlinkFont);

                            IRow currRow = sheet1.CreateRow(j);
                            NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 0, item, cellstyle, true, false);
                            NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 1, "Sheet-" + j, hyperlinkStyle, true, false);
                            XSSFHyperlink hyperlink = new XSSFHyperlink(HyperlinkType.Document);
                            var sheetname = "Sheet-" + j;
                            hyperlink.Address = $"'{sheetname}'!B1";
                            hyperlink.Tooltip = "Click to go to " + sheetname;
                            currRow.GetCell(1).Hyperlink = hyperlink;
                            j = j + 1;
                        }
                        foreach (var item in lstrouteconnectionInfo.GroupBy(m => m.fms_id).Select(group => group.First()).ToList())
                        {
                            var filteredData = lstrouteconnectionInfo.Where(m => m.fms_id == item.fms_id).ToList();
                            ISheet sheet = workbook.CreateSheet("Sheet-" + i);
                            var from = filteredData.Select(m => m.source_network_id).FirstOrDefault();
                            var headerCount = filteredData.GroupBy(m => m.path_id).OrderByDescending(group => group.Count()).FirstOrDefault().Count();
                            var distinctPathCount = filteredData.Select(m => m.path_id).Distinct().Count();
                            bool isSpl = false;
                            int? fromsplId = 0;

                            foreach (var spl in filteredData.Where(m => m.splitter_id != null)
                                 .GroupBy(m => m.splitter_id)
                                 .Select(group => group.First())
                                 .ToList())
                            {

                                var splfilteredData = filteredData.Where(m => m.splitter_id == spl.splitter_id).ToList();
                                var fromspl = splfilteredData.Select(m => m.source_network_id).FirstOrDefault();

                                var headerCountspl = splfilteredData.GroupBy(m => m.path_id).OrderByDescending(group => group.Count()).FirstOrDefault().Count();

                                if (fromsplId != splfilteredData.Select(m => m.splitter_id).FirstOrDefault())
                                {
                                    isSpl = true;
                                    NPOIExcelHelper.AddHeader(workbook, sheet, headerCountspl, fromspl, isSpl, distinctPathCount);
                                }

                                workbook = NPOIExcelHelper.DataTableToExcelCableRoute(splfilteredData, workbook, "xlsx", sheet, fromspl, isSpl, distinctPathCount);
                                // isSpl = false;
                                fromsplId = splfilteredData.Select(m => m.splitter_id).FirstOrDefault();
                                //  isSpl = false;
                            }


                            NPOIExcelHelper.AddHeader(workbook, sheet, headerCount, from);
                            workbook = NPOIExcelHelper.DataTableToExcelCableRoute(filteredData, workbook, "xlsx", sheet, from);
                            i = i + 1;
                        }

                        using (var exportData = new MemoryStream())
                        {


                            LogHelper.GetInstance.WriteDebugLog("\r\n");
                            LogHelper.GetInstance.WriteDebugLog($"Start Process to write the data in excel: {DateTime.Now}");

                            LogHelper.GetInstance.WriteDebugLog($"End Process to write the data in excel: {DateTime.Now}");
                            LogHelper.GetInstance.WriteDebugLog("\r\n");
                            string downloadTempPath = Settings.ApplicationSettings.DownloadTempPath;
                            string directoryPath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"));
                            string tempfilePath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"), filename);
                            if (Directory.Exists(directoryPath).Equals(false))
                                Directory.CreateDirectory(directoryPath);
                            using (FileStream xfile = new FileStream(tempfilePath, FileMode.Create, System.IO.FileAccess.Write))
                            {
                                workbook.Write(xfile);
                            }
                            LogHelper.GetInstance.WriteDebugLog($"FTP Connection created: {DateTime.Now}");
                            CommonUtility.FTPFileUpload(tempfilePath, filename, ftpFilePath, ftpUserName, ftpPwd);
                            LogHelper.GetInstance.WriteDebugLog($"File uploaded and FTP Connection closed: {DateTime.Now}");

                            System.IO.File.Delete(tempfilePath);

                        }

                        exportReportLog.user_id = 5;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = filename;
                        exportReportLog.file_type = "Excel";
                        exportReportLog.file_extension = ".xlsx";
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Success";
                        exportReportLog.file_location = ftpFolder + tempFileName;
                        exportReportLog.log_type = "FiberAllocation";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                    }
                    catch (Exception ex)
                    {
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Error occurred while processing request";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        ErrorLogHelper.WriteErrorLog("DownloadFiberAllocationReportIntoExcel()", "Report", ex);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public void DownloadEntityReportNewIntoPDF(string entityids)
        {
            if (Session["EntityExportSummaryData"] != null)// ExportReportFilterNew
            {
                try
                {
                    ExportEntitiesReportNew objExportEntitiesReport = new ExportEntitiesReportNew();

                    //objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["ExportReportFilterNew"];
                    //if (entityids != "")
                    //{
                    //    objExportEntitiesReport.objReportFilters.SelectedLayerIds = entityids;
                    //}
                    //objExportEntitiesReport.lstReportData = new BLLayer().GetExportReportSummary(objExportEntitiesReport.objReportFilters).OrderBy(m => m.entity_name).ToList();
                    objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["ExportReportFilterNew"];
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    objExportEntitiesReport = (ExportEntitiesReportNew)Session["EntityExportSummaryData"];
                    List<EntitySummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                    if (objExportEntitiesReport.objReportFilters.SelectedLayerId != null)
                        objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();

                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);
                    dtReport.TableName = "EntitySummaryDetail";
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportEntitiesReport.lstReportData = lstRprtData;
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (!ApplicationSettings.IsDormantEnabled)
                        {
                            dtReport.Columns.Remove("DORMANT_COUNT");
                        }
                        dtReport.Columns.Remove("entity_id");
                        dtReport.Columns.Remove("entity_name");
                        dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                        dtReport.Columns["planned_count"].ColumnName = "Planned";
                        dtReport.Columns["as_built_count"].ColumnName = "As-Built";
                        if (ApplicationSettings.IsDormantEnabled)
                        {
                            dtReport.Columns["dormant_count"].ColumnName = "Dormant";
                        }
                        string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                        if (networkstatusvalues.Length < 3)
                        {
                            if (!networkstatusvalues.Contains("P"))
                            {
                                dtReport.Columns.Remove("PLANNED");
                            }
                            if (!networkstatusvalues.Contains("A"))
                            {
                                dtReport.Columns.Remove("AS-BUILT");
                            }
                            if (ApplicationSettings.IsDormantEnabled)
                            {
                                if (!networkstatusvalues.Contains("D"))
                                {
                                    dtReport.Columns.Remove("DORMANT");
                                }
                            }
                        }
                        DataRow row = dtReport.NewRow();
                        row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = Resources.Resources.SI_OSP_GBL_GBL_GBL_041;
                        if (dtReport.Columns.Contains("Planned"))
                        {
                            row["Planned"] = dtReport.Compute("Sum(Planned)", "");
                        }
                        if (dtReport.Columns.Contains("As-Built"))
                        {
                            row["As-Built"] = dtReport.Compute("Sum([As-Built])", "");
                        }
                        if (ApplicationSettings.IsDormantEnabled)
                        {
                            if (dtReport.Columns.Contains("Dormant"))
                            {
                                row["Dormant"] = dtReport.Compute("Sum(Dormant)", "");
                            }
                        }


                        dtReport.Rows.Add(row);
                        ds.Tables.Add(dtReport);


                        GenerateToPDF(ds, "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"), Resources.Resources.SI_OSP_GBL_NET_RPT_127);

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public void DownloadEntityReportNewIntoPDFNEW(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["EntityExportSummaryData"] != null)// ExportReportFilterNew
            {
                try
                {
                    var userdetails = (User)Session["userDetail"];
                    ExportEntitiesReportNew objExportEntitiesReport = new ExportEntitiesReportNew();

                    //objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["ExportReportFilterNew"];
                    //if (entityids != "")
                    //{
                    //    objExportEntitiesReport.objReportFilters.SelectedLayerIds = entityids;
                    //}
                    //objExportEntitiesReport.lstReportData = new BLLayer().GetExportReportSummary(objExportEntitiesReport.objReportFilters).OrderBy(m => m.entity_name).ToList();
                    objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["ExportReportFilterNew"];
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    objExportEntitiesReport = (ExportEntitiesReportNew)Session["EntityExportSummaryData"];

                    BaseFont baseFont = PDFHelper.GetFont();
                    string fileName = "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");

                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        //int TotalEntityReport = 0;


                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = "PDF";
                        exportReportLog.file_extension = ".pdf";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        try
                        {
                            List<EntitySummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                            if (objExportEntitiesReport.objReportFilters.SelectedLayerId != null)
                                objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();

                            DataTable dtReport = new DataTable();
                            dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);//,true)ApplicationSettings.numberFormatType,null
                            dtReport.TableName = "EntitySummaryDetail";
                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objExportEntitiesReport.lstReportData = lstRprtData;
                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (!ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns.Remove("DORMANT_COUNT");
                                }
                                dtReport.Columns.Remove("entity_id");
                                dtReport.Columns.Remove("entity_name");
                                dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                dtReport.Columns["planned_count"].ColumnName = "Planned";
                                dtReport.Columns["as_built_count"].ColumnName = "As-Built";
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns["dormant_count"].ColumnName = "Dormant";
                                }
                                string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                if (networkstatusvalues.Length < 3)
                                {
                                    if (!networkstatusvalues.Contains("P"))
                                    {
                                        dtReport.Columns.Remove("PLANNED");
                                    }
                                    if (!networkstatusvalues.Contains("A"))
                                    {
                                        dtReport.Columns.Remove("AS-BUILT");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (!networkstatusvalues.Contains("D"))
                                        {
                                            dtReport.Columns.Remove("DORMANT");
                                        }
                                    }
                                }
                                DataRow row = dtReport.NewRow();
                                row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = Resources.Resources.SI_OSP_GBL_GBL_GBL_041;
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    row["Planned"] = dtReport.Compute("Sum(Planned)", "");
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    row["As-Built"] = dtReport.Compute("Sum([As-Built])", "");
                                }
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        row["Dormant"] = dtReport.Compute("Sum(Dormant)", "");
                                    }
                                }
                                dtReport.Rows.Add(row);
                                ds.Tables.Add(dtReport);
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    totalPlannedCount = Convert.ToInt32(row["Planned"]);
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    totalAsBuiltCount = Convert.ToInt32(row["As-Built"]);
                                }
                                // totalDormantCount = Convert.ToInt32(row["Dormant"]);

                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        totalDormantCount = Convert.ToInt32(row["Dormant"]);
                                    }
                                }
                                //commented by pk
                                // dtReport = Utility.CommonUtility.GetFormattedDataTable(dtReport, ApplicationSettings.numberFormatType);

                            }

                            string tempFileName = fileName + exportReportLog.file_extension;
                            //string ftpFolder = "ExportReportLog/";
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            //ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                            GenerateToPDFNew(ds, tempFileName, Resources.Resources.SI_OSP_GBL_NET_RPT_127, ftpFilePath, ftpUserName, ftpPwd, baseFont);
                            exportReportLog.planned = totalPlannedCount;
                            exportReportLog.asbuilt = totalAsBuiltCount;
                            exportReportLog.dormant = totalDormantCount;
                            exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            //Thread.Sleep(10000);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoPDFNEW()", "Report", ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        [System.Web.Services.WebMethod(true)]
        //public void DownloadEntityReportNewIntoExcelAll(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, CancellationToken cancellationToken = default(CancellationToken))
        public void DownloadEntityReportNewIntoExcelAll(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {

            if (Session["ExportReportFilterNew"] != null)
            {
                try
                {

                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();

                    ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();

                    objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                    objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                    objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                    objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                    objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                    objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                    objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                    objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    string fileName = "ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");

                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        int TotalEntityReport = 0;
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = "ALLEXCEL";
                        exportReportLog.file_extension = ".xlsx";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        try
                        {
                            for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                            {
                                objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                                var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                                // lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                                DataTable dtReport = new DataTable();
                                dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
                                dtReport.TableName = layerDetail.layer_title;
                                //dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName;
                                if (dtReport != null && dtReport.Rows.Count > 0)
                                {
                                    if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                    if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                    if (dtReport.Columns.Contains("Fn Get Date")) { dtReport.Columns.Remove("Fn Get Date"); }
                                }
                                if (dtReport.Rows.Count > 0)
                                    ds.Tables.Add(dtReport);
                                TotalEntityReport += dtReport.Rows.Count;
                            }
                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                            //ExportData(ds, fileName,);
                            string tempFileName = fileName + exportReportLog.file_extension;
                            //string ftpFolder = "ExportReportLog/";
                            //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            //Thread.Sleep(10000);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoExcelAll()", "Report", ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        [System.Web.Services.WebMethod(true)]
        public void DownloadEntityReportIntoExcelAll(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {

            if (Session["ExportReportFilterNew"] != null)
            {
                try
                {
                    ExportEntitiesReportNew entityExportSummaryData = new ExportEntitiesReportNew();

                    entityExportSummaryData = (ExportEntitiesReportNew)Session["EntityExportSummaryData"];
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();

                    ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();

                    objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                    objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                    objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                    objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                    objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                    objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                    objExportEntitiesReport.objReportFilters.selected_route_ids = objExportReportFilterNew.selected_route_ids;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                    objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                    objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }

                    string parentFolder = $"ExportReport_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                    string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                    string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                    string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));

                    // Create Parent folder for temparary basic on server 
                    if (Directory.Exists(directoryPath).Equals(false))
                        Directory.CreateDirectory(directoryPath);
                    string fileName = $"{dtFilter.TableName}";
                    string tempFileName = $"{directoryPath}/{dtFilter.TableName}.xlsx";
                    ExportDataNew(dtFilter, fileName, tempFileName);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = parentFolder;
                        exportReportLog.file_type = "ALLEXCEL";
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        dtFilter = null;
                        try
                        {
                            var tasks = new List<Task>();
                            foreach (var layer in objExportEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        var layer_name = layer.layer_name;
                                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                        EntitySummaryReport recordCount = entityExportSummaryData.lstReportData.Where(x => x.entity_name.ToUpper() == layer.layer_name.ToUpper()).FirstOrDefault();
                                        int total_entity_count = 0;
                                        if (recordCount != null)
                                            total_entity_count = recordCount.planned_count + recordCount.as_built_count + recordCount.dormant_count;


                                        //bool textType = true;
                                        //var dataSet = GetDataSetFromDataTable(objExportEntitiesReport, reportType, layer_name, layerDetail, total_entity_count, textType);

                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                        List<Dictionary<string, string>> lstExportEntitiesDetailAdditional = null;
                                        List<Dictionary<string, string>> lstExportEntitiesDetailCdb = null;
                                        if (layerDetail != null && layerDetail.is_dynamic_control_enable != true)
                                        {
                                            layerDetail.is_dynamic_control_enable = false;
                                        }

                                        List<string> reportTypeString = reportType;
                                        if (total_entity_count > ApplicationSettings.ExcelReportLimitCount)
                                        {
                                            if (reportTypeString[0].Contains("GIS"))
                                            {
                                                lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            if (reportTypeString[0].Contains("CDB") && objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                                            {
                                                lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCSVCdb(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail != null && layerDetail.is_dynamic_control_enable)
                                            {
                                                lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            //if (reportTypeString[0].Trim().Contains("") || reportTypeString[0].Contains("ALL"))
                                            //{
                                            //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            //    if (objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                                            //    {
                                            //        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCSVCdb(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            //    }
                                            //    if (layerDetail.is_dynamic_control_enable)
                                            //    {
                                            //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            //    }
                                            //}

                                        }
                                        else
                                        {
                                            if (reportTypeString[0].Contains("GIS"))
                                            {
                                                lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            if (reportTypeString[0].Contains("CDB") && objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                                            {
                                                lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewNewCdb(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail != null && layerDetail.is_dynamic_control_enable)
                                            {
                                                lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            //if (reportTypeString[0].Contains("ALL"))
                                            //{
                                            //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            //    if (layerDetail.is_dynamic_control_enable)
                                            //    {
                                            //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            //    }
                                            //    if (objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                                            //    {
                                            //        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewNewCdb(objExportEntitiesReport.objReportFilters, layer.layer_name);

                                            //    }
                                            //}

                                        }

                                        //DataTable dtReport = dataSet.Tables[0];
                                        //DataTable dtReportCdb = dataSet.Tables[1];
                                        //DataTable dtReportAdditional = dataSet.Tables[2];
                                        DataTable dtReport = new DataTable();
                                        DataTable dtReportCdb = new DataTable();
                                        DataTable dtReportAdditional = new DataTable();
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
                                        dtReportCdb = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailCdb, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
                                        dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });

                                        dtReport.TableName = layer.layer_title;
                                        dtReportCdb.TableName = layer.layer_title;
                                        dtReportAdditional.TableName = layer.layer_title;

                                        if (dtReport != null && dtReport.Rows.Count > 0)
                                        {
                                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                            if (dtReport.Columns.Contains("Fn Get Date")) { dtReport.Columns.Remove("Fn Get Date"); }
                                        }

                                        if (dtReportCdb != null && dtReportCdb.Rows.Count > 0)
                                        {
                                            if (dtReportCdb.Columns.Contains("S_NO")) { dtReportCdb.Columns.Remove("S_NO"); }
                                            if (dtReportCdb.Columns.Contains("totalrecords")) { dtReportCdb.Columns.Remove("totalrecords"); }
                                            if (dtReportCdb.Columns.Contains("Barcode")) { dtReportCdb.Columns.Remove("Barcode"); }
                                            if (dtReportCdb.Columns.Contains("Fn Get Date")) { dtReportCdb.Columns.Remove("Fn Get Date"); }
                                        }

                                        if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                                        {
                                            if (dtReportAdditional.Columns.Contains("S_NO")) { dtReportAdditional.Columns.Remove("S_NO"); }
                                            if (dtReportAdditional.Columns.Contains("totalrecords")) { dtReportAdditional.Columns.Remove("totalrecords"); }
                                            if (dtReportAdditional.Columns.Contains("Barcode")) { dtReportAdditional.Columns.Remove("Barcode"); }
                                            if (dtReportAdditional.Columns.Contains("Fn Get Date")) { dtReportAdditional.Columns.Remove("Fn Get Date"); }
                                        }



                                        if (dtReport.Rows.Count > 0 || dtReportCdb.Rows.Count > 0 || dtReportAdditional.Rows.Count > 0)
                                        {
                                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                            objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;


                                            if (dtReport.Rows.Count > 0)
                                            {
                                                if (dtReport.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                                {
                                                    dtReport.TableName = dtReport.TableName + "_GisAttribute";
                                                    fileName = $"{dtReport.TableName}";
                                                    tempFileName = $"{parentFolder}/{dtReport.TableName}.csv";
                                                    StreamNewCSVInFolder(dtReport, tempFileName);
                                                }
                                                else
                                                {
                                                    IWorkbook workbook = new XSSFWorkbook();
                                                    //if()
                                                    fileName = $"{dtReport.TableName}";
                                                    tempFileName = $"{directoryPath}/{dtReport.TableName}.xlsx";
                                                    ExportDataExcelMerge(workbook, dtReport, dtReportCdb, dtReportAdditional, fileName, tempFileName);
                                                }
                                            }

                                            if (dtReportCdb.Rows.Count > 0)
                                            {
                                                if (dtReportCdb.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                                {
                                                    dtReportCdb.TableName = dtReportCdb.TableName + "_CdbAttribute";
                                                    tempFileName = $"{parentFolder}/{dtReportCdb.TableName}.csv";
                                                    StreamNewCSVInFolder(dtReportCdb, tempFileName);
                                                }
                                                else
                                                {
                                                    fileName = $"{dtReportCdb.TableName}";
                                                    IWorkbook workbook = new XSSFWorkbook();
                                                    tempFileName = $"{directoryPath}/{dtReport.TableName}.xlsx";
                                                    ExportDataExcelMerge(workbook, dtReport, dtReportCdb, dtReportAdditional, fileName, tempFileName);
                                                }
                                            }
                                            if (dtReportAdditional.Rows.Count > 0)
                                            {
                                                if (dtReportAdditional.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                                {
                                                    dtReportAdditional.TableName = dtReportAdditional.TableName + "_AdditionalAttribute";
                                                    tempFileName = $"{parentFolder}/{dtReportAdditional.TableName}.csv";
                                                    StreamNewCSVInFolder(dtReportAdditional, tempFileName);
                                                }
                                                else
                                                {
                                                    fileName = $"{dtReportAdditional.TableName}";
                                                    IWorkbook workbook = new XSSFWorkbook();
                                                    tempFileName = $"{directoryPath}/{dtReportAdditional.TableName}.xlsx";
                                                    ExportDataExcelMerge(workbook, dtReport, dtReportCdb, dtReportAdditional, fileName, tempFileName);
                                                }
                                            }


                                            exportReportLog.export_ended_on = DateTime.Now;
                                            exportReportLog.status = "Success";
                                            exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
                                            //Thread.Sleep(10000);
                                            dtReport = null;
                                            dtReportCdb = null;
                                            dtReportAdditional = null;
                                        }





                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));
                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();

                            // get FTP details
                            string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                            string zipfilePath = directoryPath + ".zip";
                            string fileNameValue = parentFolder + ".zip";

                            // Below code for Convert parent folder to ZIP and delete parent folder after ZIP on server
                            using (var zip = new ZipFile())
                            {
                                //zip.UseZip64WhenSaving = Zip64Option.Always;
                                //zip.CompressionMethod = CompressionMethod.BZip2;
                                zip.AddDirectory(directoryPath);
                                zip.Save(zipfilePath);
                            }
                            if (System.IO.File.Exists(zipfilePath))
                            {
                                string fileZipName = Path.GetFileName(zipfilePath);
                                Directory.Delete(directoryPath, true);
                            }
                            FileInfo file = new FileInfo(zipfilePath);

                            // ZIP File upload on FTP server
                            CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);
                            // Deleted ZIP on Server after uploaded on FTP server
                            System.IO.File.Delete(zipfilePath);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);


                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportIntoExcelAll()", "Report", ex);
                            // delete folder after error generate
                            if (Directory.Exists(directoryPath).Equals(true))
                                Directory.Delete(directoryPath, true);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public DataSet GetDataSetFromDataTable(ExportEntitiesSummaryView objExportEntitiesReport, List<string> reportType, string layer_name, layerDetail layerDetail, Int64 total_entity_count, bool textType)
        {
            DataSet dataSet = new DataSet();
            List<Dictionary<string, string>> lstExportEntitiesDetail = null;
            List<Dictionary<string, string>> lstExportEntitiesDetailAdditional = null;
            List<Dictionary<string, string>> lstExportEntitiesDetailCdb = null;

            List<string> reportTypeString = reportType;

            if (!textType && total_entity_count > ApplicationSettings.ExcelReportLimitCount)
            {
                if (reportTypeString[0].Contains("GIS"))
                {
                    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer_name);
                }
                if (reportTypeString[0].Contains("CDB") && layer_name == EntityType.Cable.ToString())
                {
                    lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCSVCdb(objExportEntitiesReport.objReportFilters, layer_name);
                }
                if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail.is_dynamic_control_enable)
                {
                    lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer_name);
                }
                //if (reportTypeString[0].Contains("") || reportTypeString[0].Contains("ALL"))
                //{
                //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer_name);
                //    if (layer_name == EntityType.Cable.ToString())
                //    {
                //        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCSVCdb(objExportEntitiesReport.objReportFilters, layer_name);
                //    }
                //    if (layerDetail.is_dynamic_control_enable)
                //    {
                //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer_name);
                //    }
                //}
            }
            else if (!textType && total_entity_count < ApplicationSettings.ExcelReportLimitCount)
            {
                if (reportTypeString[0].Contains("GIS"))
                {
                    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer_name);
                }
                if (reportTypeString[0].Contains("CDB") && objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                {
                    lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewNewCdb(objExportEntitiesReport.objReportFilters, layer_name);
                }
                if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail.is_dynamic_control_enable)
                {
                    lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer_name);
                }
                //if (reportTypeString[0].Contains("ALL"))
                //{
                //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer_name);
                //    if (layerDetail.is_dynamic_control_enable)
                //    {
                //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer_name);
                //    }
                //    if (layer_name == EntityType.Cable.ToString())
                //    {
                //        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewNewCdb(objExportEntitiesReport.objReportFilters, layer_name);

                //    }
                //}
            }

            // file type="Excel"
            if (textType)
            {
                if (total_entity_count < ApplicationSettings.ExcelReportLimitCount)
                {
                    if (reportTypeString[0].Contains("GIS"))
                    {
                        lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer_name);
                    }
                    if (reportTypeString[0].Contains("CDB") && objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                    {
                        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewNewCdb(objExportEntitiesReport.objReportFilters, layer_name);
                    }
                    if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail.is_dynamic_control_enable)
                    {
                        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer_name);
                    }
                    //if (reportTypeString[0].Contains("ALL"))
                    //{
                    //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer_name);
                    //    if (layerDetail.is_dynamic_control_enable)
                    //    {
                    //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer_name);
                    //    }
                    //    if (layer_name == EntityType.Cable.ToString())
                    //    {
                    //        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewNewCdb(objExportEntitiesReport.objReportFilters, layer_name);

                    //    }
                    //}
                }
                else
                {
                    if (reportTypeString[0].Contains("GIS"))
                    {
                        lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer_name);
                    }
                    if (reportTypeString[0].Contains("CDB") && layer_name == EntityType.Cable.ToString())
                    {
                        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCSVCdb(objExportEntitiesReport.objReportFilters, layer_name);
                    }
                    if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail.is_dynamic_control_enable)
                    {
                        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer_name);
                    }
                    //if (reportTypeString[0].Contains("") || reportTypeString[0].Contains("ALL"))
                    //{
                    //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer_name);
                    //    if (layer_name == EntityType.Cable.ToString())
                    //    {
                    //        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCSVCdb(objExportEntitiesReport.objReportFilters, layer_name);
                    //    }
                    //    if (layerDetail.is_dynamic_control_enable)
                    //    {
                    //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer_name);
                    //    }
                    //}
                }
            }




            DataTable dtReport = new DataTable();
            DataTable dtReportCdb = new DataTable();
            DataTable dtReportAdditional = new DataTable();
            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
            dtReportCdb = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailCdb, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
            dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
            // Create a new DataSet

            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                if (dtReport.Columns.Contains("Fn Get Date")) { dtReport.Columns.Remove("Fn Get Date"); }
            }

            if (dtReportCdb != null && dtReportCdb.Rows.Count > 0)
            {
                if (dtReportCdb.Columns.Contains("S_NO")) { dtReportCdb.Columns.Remove("S_NO"); }
                if (dtReportCdb.Columns.Contains("totalrecords")) { dtReportCdb.Columns.Remove("totalrecords"); }
                if (dtReportCdb.Columns.Contains("Barcode")) { dtReportCdb.Columns.Remove("Barcode"); }
                if (dtReportCdb.Columns.Contains("Fn Get Date")) { dtReportCdb.Columns.Remove("Fn Get Date"); }
            }

            if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
            {
                if (dtReportAdditional.Columns.Contains("S_NO")) { dtReportAdditional.Columns.Remove("S_NO"); }
                if (dtReportAdditional.Columns.Contains("totalrecords")) { dtReportAdditional.Columns.Remove("totalrecords"); }
                if (dtReportAdditional.Columns.Contains("Barcode")) { dtReportAdditional.Columns.Remove("Barcode"); }
                if (dtReportAdditional.Columns.Contains("Fn Get Date")) { dtReportAdditional.Columns.Remove("Fn Get Date"); }
            }


            // Add the DataTable to the DataSet
            dataSet.Tables.Add(dtReport);
            dataSet.Tables.Add(dtReportCdb);
            dataSet.Tables.Add(dtReportAdditional);

            // Return the DataSet
            return dataSet;
        }



        public void StreamNewCSVInFolderMerge(DataTable dataTable, DataTable dataTableCdb, DataTable dataTableAdditional, string remoteFilePath)
        {
            try
            {
                string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                string completePath = Path.Combine(attachmentLocalPath, remoteFilePath);
                string directoryPath = System.IO.Path.Combine(Server.MapPath(completePath));
                //LogHelper.GetInstance.WriteDebugLog($"{dataTable.TableName}.CSV file creation start on  : {DateTime.Now}");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(directoryPath))
                    {

                        // Write the data rows
                        foreach (DataRow row in dataTable.Rows)
                        {
                            writer.WriteLine('"' + row[0].ToString() + '"');

                        }
                        // Flush the StreamWriter to ensure data is written immediately
                        writer.Flush();
                        // Reset the memory stream position to the beginning
                        memoryStream.Position = 0;

                    }

                    using (StreamWriter writer = new StreamWriter(directoryPath))
                    {

                        // Write the data rows
                        foreach (DataRow row in dataTableCdb.Rows)
                        {
                            writer.WriteLine('"' + row[0].ToString() + '"');

                        }
                        // Flush the StreamWriter to ensure data is written immediately
                        writer.Flush();
                        // Reset the memory stream position to the beginning
                        memoryStream.Position = 0;

                    }

                    using (StreamWriter writer = new StreamWriter(directoryPath))
                    {

                        // Write the data rows
                        foreach (DataRow row in dataTableAdditional.Rows)
                        {
                            writer.WriteLine('"' + row[0].ToString() + '"');

                        }
                        // Flush the StreamWriter to ensure data is written immediately
                        writer.Flush();
                        // Reset the memory stream position to the beginning
                        memoryStream.Position = 0;

                    }

                    memoryStream.Close();
                }
                //LogHelper.GetInstance.WriteDebugLog($"{dataTable.TableName}.CSV file creation completed on : {DateTime.Now}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private void ExportDataExcelMerge(IWorkbook workbook, DataTable dtReport, DataTable dtReportCdb, DataTable dtReportAdditional, string fileName, string tempfileName)
        {
            using (var exportData = new MemoryStream())
            {
                // Create a new workbook

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = "Gis_Attribute"; // Default table name if empty

                    ISheet sheet1 = workbook.CreateSheet("Gis_Attribute");
                    NPOIExcelHelper.DataTableToSheet(dtReport, sheet1);
                }

                if (dtReportCdb != null && dtReportCdb.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReportCdb.TableName))
                        dtReportCdb.TableName = "Cdb_Attribute"; // Default table name if empty

                    ISheet sheet2 = workbook.CreateSheet("Cdb_Attribute");
                    NPOIExcelHelper.DataTableToSheet(dtReportCdb, sheet2);
                }

                if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReportAdditional.TableName))
                        dtReportAdditional.TableName = "Additional_Attribute"; // Default table name if empty

                    ISheet sheet3 = workbook.CreateSheet("Additional_Attribute");
                    NPOIExcelHelper.DataTableToSheet(dtReportAdditional, sheet3);
                }

                // Write the workbook to the MemoryStream
                workbook.Write(exportData);

                // Write the MemoryStream to the file
                FileStream xfile = new FileStream(tempfileName, FileMode.Create, System.IO.FileAccess.Write);

                workbook.Write(xfile);
                xfile.Close();

            }
        }

        private void ExportDataExcelMergeWithoutCdb(DataTable dtReport, DataTable dtReportAdditional, string fileName, string tempfileName)
        {
            using (var exportData = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook(); // Create a new workbook

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = "Gis_Attribute"; // Default table name if empty

                    ISheet sheet1 = workbook.CreateSheet("Gis_Attribute");
                    NPOIExcelHelper.DataTableToSheet(dtReport, sheet1);
                }


                if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReportAdditional.TableName))
                        dtReportAdditional.TableName = "Additional_Attribute"; // Default table name if empty

                    ISheet sheet2 = workbook.CreateSheet("Additional_Attribute");
                    NPOIExcelHelper.DataTableToSheet(dtReportAdditional, sheet2);
                }

                // Write the workbook to the MemoryStream
                workbook.Write(exportData);

                // Write the MemoryStream to the file
                FileStream xfile = new FileStream(tempfileName, FileMode.Create, System.IO.FileAccess.Write);

                workbook.Write(xfile);
                xfile.Close();

            }
        }

        private void GenerateToPDF(DataSet ds, string Name, string title)
        {

            iTextSharp.text.Font font = new iTextSharp.text.Font(PDFHelper.GetFont(), 12, iTextSharp.text.Font.NORMAL);
            Paragraph PoweredHeading = new Paragraph(new Chunk(Resources.Resources.SI_GBL_GBL_NET_FRM_165, font));
            PoweredHeading.Alignment = Element.ALIGN_CENTER;
            PoweredHeading.SpacingAfter = 20;

            PdfPTable table = new PdfPTable(ds.Tables[0].Columns.Count);
            table.WidthPercentage = 90f;
            PdfPCell cell = new PdfPCell(new Phrase(Resources.Resources.SI_GBL_GBL_NET_FRM_166, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(0, 186, 138);
            cell.FixedHeight = 25f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Resources.Resources.SI_GBL_GBL_NET_FRM_167, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(0, 186, 138);
            cell.FixedHeight = 25f;
            table.AddCell(cell);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                cell = new PdfPCell(new Phrase("" + ds.Tables[0].Rows[i][0] + "", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.FixedHeight = 25f;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("" + ds.Tables[0].Rows[i][1] + "", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.FixedHeight = 25f;
                table.AddCell(cell);


            }
            PdfPTable table1 = new PdfPTable(ds.Tables[1].Columns.Count);
            table1.WidthPercentage = 97f;//90f;
            Paragraph EntityHeading = new Paragraph(new Chunk(title + " " + Resources.Resources.SI_GBL_GBL_NET_FRM_168, font));
            EntityHeading.Alignment = Element.ALIGN_CENTER;
            EntityHeading.SpacingAfter = 20;
            PdfPCell cell1 = new PdfPCell();
            foreach (DataColumn column in ds.Tables[1].Columns)
            {
                if (column.ColumnName.ToUpper() != "UTILIZATION_TEXT")
                {
                    cell1 = new PdfPCell(new Phrase("" + column.ColumnName + "", font));
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = new BaseColor(0, 186, 138);
                    cell1.FixedHeight = 25f;
                    table1.AddCell(cell1);
                }
            }
            foreach (DataRow row in ds.Tables[1].Rows)
            {
                int cnt = 0;
                foreach (DataColumn column in ds.Tables[1].Columns)
                {
                    if (column.ColumnName.ToUpper() != "UTILIZATION_TEXT")
                    {
                        if (cnt == 4 && row[1].ToString() == "Duct")
                        {

                            cell1 = new PdfPCell(new Phrase("" + row[column] + " Duct " + row[8] + "", font));
                            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            //cell1.BackgroundColor = new BaseColor(0, 186, 138);
                            cell1.FixedHeight = 25f;
                            cell1.Colspan = 4;
                            table1.AddCell(cell1);
                            break;
                        }

                        else if (row[0] == Resources.Resources.SI_OSP_GBL_GBL_GBL_041)
                        {
                            cell1 = new PdfPCell(new Phrase("" + row[column] + "", font));
                            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell1.BackgroundColor = new BaseColor(0, 186, 138);
                            cell1.FixedHeight = 25f;
                            table1.AddCell(cell1);
                        }
                        else
                        {
                            cell1 = new PdfPCell(new Phrase("" + row[column] + "", font));
                            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell1.FixedHeight = 25f;
                            table1.AddCell(cell1);
                        }
                        cnt++;
                    }
                }
            }
            Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 25f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            writer.PageEvent = new PDFHelper.AllPdfPageEvents();
            pdfDoc.Open();
            pdfDoc.Add(PoweredHeading);
            pdfDoc.Add(table);
            pdfDoc.Add(EntityHeading);
            pdfDoc.Add(table1);
            // htmlparser.Parse(sr);
            pdfDoc.Close();


            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + Name + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        private void GenerateToPDFNew(DataSet ds, string fileName, string title, string ftpfilePath, string ftpUserName, string ftpPassword, BaseFont baseFont)
        {

            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
            Paragraph PoweredHeading = new Paragraph(new Chunk(Resources.Resources.SI_GBL_GBL_NET_FRM_165, font));
            PoweredHeading.Alignment = Element.ALIGN_CENTER;
            PoweredHeading.SpacingAfter = 20;

            PdfPTable table = new PdfPTable(ds.Tables[0].Columns.Count);
            table.WidthPercentage = 90f;
            PdfPCell cell = new PdfPCell(new Phrase(Resources.Resources.SI_GBL_GBL_NET_FRM_166, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(0, 186, 138);
            cell.FixedHeight = 25f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Resources.Resources.SI_GBL_GBL_NET_FRM_167, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(0, 186, 138);
            cell.FixedHeight = 25f;
            table.AddCell(cell);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                cell = new PdfPCell(new Phrase("" + ds.Tables[0].Rows[i][0] + "", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.FixedHeight = 25f;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("" + ds.Tables[0].Rows[i][1] + "", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.FixedHeight = 25f;
                table.AddCell(cell);


            }
            PdfPTable table1 = new PdfPTable(ds.Tables[1].Columns.Count);
            table1.WidthPercentage = 97f;//90f;
            Paragraph EntityHeading = new Paragraph(new Chunk(title + " " + Resources.Resources.SI_GBL_GBL_NET_FRM_168, font));
            EntityHeading.Alignment = Element.ALIGN_CENTER;
            EntityHeading.SpacingAfter = 20;
            PdfPCell cell1 = new PdfPCell();
            foreach (DataColumn column in ds.Tables[1].Columns)
            {
                if (column.ColumnName.ToUpper() != "UTILIZATION_TEXT")
                {
                    cell1 = new PdfPCell(new Phrase("" + column.ColumnName + "", font));
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = new BaseColor(0, 186, 138);
                    cell1.FixedHeight = 25f;
                    table1.AddCell(cell1);
                }
            }
            foreach (DataRow row in ds.Tables[1].Rows)
            {
                int cnt = 0;
                foreach (DataColumn column in ds.Tables[1].Columns)
                {
                    if (column.ColumnName.ToUpper() != "UTILIZATION_TEXT")
                    {
                        if (cnt == 4 && row[1].ToString() == "Duct")
                        {

                            cell1 = new PdfPCell(new Phrase("" + row[column] + " Duct " + row[8] + "", font));
                            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            //cell1.BackgroundColor = new BaseColor(0, 186, 138);
                            cell1.FixedHeight = 25f;
                            cell1.Colspan = 4;
                            table1.AddCell(cell1);
                            break;
                        }

                        else if (row[0] == Resources.Resources.SI_OSP_GBL_GBL_GBL_041)
                        {
                            cell1 = new PdfPCell(new Phrase("" + row[column] + "", font));
                            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell1.BackgroundColor = new BaseColor(0, 186, 138);
                            cell1.FixedHeight = 25f;
                            table1.AddCell(cell1);
                        }
                        else
                        {
                            cell1 = new PdfPCell(new Phrase("" + row[column] + "", font));
                            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell1.FixedHeight = 25f;
                            table1.AddCell(cell1);
                        }
                        cnt++;
                    }
                }
            }
            Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 25f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


            //commented by priyanka
            //string directoryPath = Path.Combine(Server.MapPath("~/Uploads/Temp/ExportReportLog"));
            //string tempfilePath = Path.Combine(Server.MapPath("~/Uploads/Temp/ExportReportLog"), fileName);
            //end commented by priyanka
            //implemented by priyanka
            string downloadTempPath = Settings.ApplicationSettings.DownloadTempPath;
            string directoryPath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"));
            string tempfilePath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"), fileName);
            //implemented by priyanka

            if (Directory.Exists(directoryPath).Equals(false))
                Directory.CreateDirectory(directoryPath);

            FileStream xfile = new FileStream(tempfilePath, FileMode.Create, System.IO.FileAccess.Write);

            //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, xfile);
            writer.PageEvent = new PDFHelper.AllPdfPageEvents();
            pdfDoc.Open();
            pdfDoc.Add(PoweredHeading);
            pdfDoc.Add(table);
            pdfDoc.Add(EntityHeading);
            pdfDoc.Add(table1);
            pdfDoc.Close();

            CommonUtility.FTPFileUpload(tempfilePath, fileName, ftpfilePath, ftpUserName, ftpPassword);
            System.IO.File.Delete(tempfilePath);

            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=" + Name + ".pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Write(pdfDoc);
            //Response.End();
        }


        public void DownloadEntitySummaryIntoKMLAll(string entityids, string fileType)
        {
            if (Session["ExportReportFilterNew"] != null)
            {

                DataSet ds = new DataSet();
                ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.currentPage = 0;
                objExportEntitiesReport.objReportFilters.fileType = "KML";
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                }
                DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);
                for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                {
                    objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);

                    dtReport.TableName = layerDetail.layer_title;
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                }

                objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                if (fileType == "KML")
                {
                    string TempkmlFileName = "ExportReport_KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
                    KMLHelper.DatasetToKML(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, TempkmlFileName, "", dtFilter, fileType);
                }
                else
                {
                    string TempkmlFileName = "ExportReport_XML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".xml";
                    KMLHelper.DatasetToKML(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, TempkmlFileName, "", dtFilter, fileType);
                }

            }
        }
        public void DownloadEntitySummaryIntoKMLAllNew(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["ExportReportFilterNew"] != null)
            {

                DataSet ds = new DataSet();
                ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.currentPage = 0;
                objExportEntitiesReport.objReportFilters.fileType = "KML";
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                }
                DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);

                string fileName = (fileType == "KML" ? "ExportReport_KML_" : "ExportReport_XML_") + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");

                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    //int TotalEntityReport = 0;
                    ExportReportLog exportReportLog = new ExportReportLog();
                    exportReportLog.user_id = userdetails.user_id;
                    exportReportLog.export_started_on = DateTime.Now;
                    //exportReportLog.file_name = fileName;
                    exportReportLog.file_type = fileType;
                    exportReportLog.file_extension = ".zip";
                    exportReportLog.status = "InProgress";
                    exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                    exportReportLog.planned = totalPlannedCount;
                    exportReportLog.asbuilt = totalAsBuiltCount;
                    exportReportLog.dormant = totalDormantCount;
                    exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                    exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                    try
                    {

                        for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                        {
                            objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                            var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                            List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                            DataTable dtReport = new DataTable();
                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] {""}

                            dtReport.TableName = layerDetail.layer_title;
                            if (dtReport.Rows.Count > 0)
                                ds.Tables.Add(dtReport);
                        }

                        objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                        objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;


                        string tempFileName = fileName + exportReportLog.file_extension;
                        //string ftpFolder = "ExportReportLog/";
                        //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                        string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                        string tempFolderName = string.Empty;
                        if (fileType == "KML")
                        {
                            string TempkmlFileName = fileName + ".kml";
                            tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
                            KMLHelper.DatasetToKMLNew(ftpFilePath, ftpUserName, ftpPwd, ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, tempFolderName, TempkmlFileName, "", dtFilter, fileType);
                        }
                        else
                        {
                            string TempkmlFileName = fileName + ".xml";
                            tempFolderName = "XML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
                            KMLHelper.DatasetToKMLNew(ftpFilePath, ftpUserName, ftpPwd, ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, tempFolderName, TempkmlFileName, "", dtFilter, fileType);
                        }
                        exportReportLog.file_name = tempFolderName;
                        exportReportLog.file_location = ftpFolder + tempFolderName + ".zip";
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Success";
                        //Thread.Sleep(10000);
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                    }
                    catch (Exception ex)
                    {
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Error occurred while processing request";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        ErrorLogHelper.WriteErrorLog("DownloadEntitySummaryIntoKMLAllNew()", "Report", ex);
                    }
                });
            }
        }
        public void DownloadEntitySummaryIntoAllKMLNew(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {
            if (Session["ExportReportFilterNew"] != null)
            {

                //DataSet ds = new DataSet();
                ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                ExportEntitiesReportNew entityExportSummaryData = new ExportEntitiesReportNew();

                entityExportSummaryData = (ExportEntitiesReportNew)Session["EntityExportSummaryData"];

                objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.currentPage = 0;
                objExportEntitiesReport.objReportFilters.fileType = "KML";
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                }
                DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);
                string parentFolder = $"KML_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));

                // Create Parent folder for temparary basic on server 
                if (Directory.Exists(directoryPath).Equals(false))
                    Directory.CreateDirectory(directoryPath);

                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    //int TotalEntityReport = 0;
                    ExportReportLog exportReportLog = new ExportReportLog();
                    exportReportLog.user_id = userdetails.user_id;
                    exportReportLog.export_started_on = DateTime.Now;
                    //exportReportLog.file_name = fileName;
                    exportReportLog.file_type = fileType;
                    exportReportLog.file_extension = ".zip";
                    exportReportLog.status = "InProgress";
                    exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                    exportReportLog.planned = totalPlannedCount;
                    exportReportLog.asbuilt = totalAsBuiltCount;
                    exportReportLog.dormant = totalDormantCount;
                    exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                    exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                    try
                    {

                        var tasks = new List<Task>();
                        foreach (var layer in objExportEntitiesReport.lstLayers)
                        {
                            tasks.Add(Task.Run(() =>
                            {
                                try
                                {

                                    objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;

                                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                    EntitySummaryReport recordCount = entityExportSummaryData.lstReportData.Where(x => x.entity_name.ToUpper() == layer.layer_name.ToUpper()).FirstOrDefault();
                                    int total_entity_count = 0;
                                    if (recordCount != null)
                                        total_entity_count = recordCount.planned_count + recordCount.as_built_count + recordCount.dormant_count;




                                    //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                    //LogHelper.GetInstance.WriteDebugLogTest($"==========================Layer Title==========={layer.layer_title}============================", layer.layer_name);
                                    List<Dictionary<string, string>> lstExportEntitiesDetailKml = null;
                                    List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                    List<Dictionary<string, string>> lstExportEntitiesDetailAdditional = null;

                                    lstExportEntitiesDetailKml = new BLLayer().GetExportSummaryViewKMLNew(objExportEntitiesReport.objReportFilters, layer.layer_name);


                                    var layerdetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                    if (layerdetails.is_dynamic_control_enable == null)
                                    {
                                        layerdetails.is_dynamic_control_enable = false;
                                    }


                                    List<string> reportTypeString = reportType;

                                    if (total_entity_count > ApplicationSettings.ExcelReportLimitCount)
                                    {
                                        if (reportTypeString[0].Contains("GIS"))
                                        {
                                            lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        }
                                        if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail.is_dynamic_control_enable)
                                        {
                                            lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        }
                                        //if (reportTypeString[0].Contains("") || reportTypeString[0].Contains("ALL"))
                                        //{
                                        //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);

                                        //    if (layerDetail.is_dynamic_control_enable)
                                        //    {
                                        //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        //    }
                                        //}
                                    }
                                    else
                                    {
                                        if (reportTypeString[0].Contains("GIS"))
                                        {
                                            lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        }
                                        if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail.is_dynamic_control_enable)
                                        {
                                            lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        }
                                        //if (reportTypeString.Contains("") || reportTypeString.Contains("ALL"))
                                        //{
                                        //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        //    if (layerDetail.is_dynamic_control_enable)
                                        //    {
                                        //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        //    }
                                        //}

                                    }

                                    DataTable dtReportKml = new DataTable();
                                    DataTable dtReport = new DataTable();
                                    DataTable dtReportAdditional = new DataTable();

                                    dtReportKml = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailKml);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
                                    dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });

                                    dtReportKml.TableName = layer.layer_title;
                                    dtReport.TableName = layer.layer_title;
                                    dtReportAdditional.TableName = layer.layer_title;
                                    string tempFileName = String.Empty;
                                    string fileName = layer.layer_title;
                                    string TempkmlFileName = fileName + ".kml";
                                    string finalkml = KMLHelper.GetKmlForEntityNew(dtReportKml, objExportEntitiesReport.lstLayers, dtFilter, TempkmlFileName, directoryPath);
                                    finalkml = finalkml.Replace("&", "&amp;");
                                    string kmlDesFullPath = directoryPath + "\\" + TempkmlFileName;

                                    if (lstExportEntitiesDetail != null && lstExportEntitiesDetail.Count > 0)
                                    {
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                                                                                                                    //dtReport.TableName = layerDetail.layer_title;
                                        dtReport.TableName = layer.layer_title;
                                        if (dtReport != null && dtReport.Rows.Count > 0)
                                        {
                                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                        }

                                    }

                                    if (lstExportEntitiesDetailAdditional != null && lstExportEntitiesDetailAdditional.Count > 0)
                                    {
                                        dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                                                                                                                                        //dtReport.TableName = layerDetail.layer_title;
                                        dtReportAdditional.TableName = layer.layer_title;
                                        if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                                        {
                                            if (dtReportAdditional.Columns.Contains("S_NO")) { dtReportAdditional.Columns.Remove("S_NO"); }
                                            if (dtReportAdditional.Columns.Contains("totalrecords")) { dtReportAdditional.Columns.Remove("totalrecords"); }
                                            if (dtReportAdditional.Columns.Contains("Barcode")) { dtReportAdditional.Columns.Remove("Barcode"); }
                                        }

                                    }

                                    if (dtReport.Rows.Count > 0 || dtReportAdditional.Rows.Count > 0)
                                    {
                                        if (dtReport.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                        {
                                            dtReport.TableName = dtReport.TableName + "_GisAttribute";
                                            tempFileName = $"{parentFolder}/{dtReport.TableName}.csv";
                                            StreamNewCSVInFolder(dtReport, tempFileName);
                                        }
                                        if (dtReportAdditional.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                        {
                                            dtReportAdditional.TableName = dtReport.TableName + "_AdditionalAttribute";
                                            tempFileName = $"{parentFolder}/{dtReportAdditional.TableName}.csv";
                                            StreamNewCSVInFolder(dtReportAdditional, tempFileName);
                                        }
                                        else
                                        {
                                            fileName = $"{parentFolder}/{layer.layer_title}.xlsx";
                                            var tempshapeFilePath = $"{directoryPath}/{layer.layer_title}.xlsx";
                                            ExportDataExcelMergeWithoutCdb(dtReport, dtReportAdditional, fileName, tempshapeFilePath);
                                        }
                                    }


                                    System.IO.File.WriteAllText(kmlDesFullPath, finalkml.ToString());
                                    dtReportKml = null;
                                    dtReport = null;
                                    dtReportAdditional = null;


                                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                    objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;


                                    //Thread.Sleep(10000);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }));
                        }
                        Task t = Task.WhenAll(tasks);
                        t.Wait();

                        dtFilter = null;
                        // get FTP details
                        string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                        string zipfilePath = directoryPath + ".zip";
                        string fileNameValue = parentFolder + ".zip";

                        // Below code for Convert parent folder to ZIP and delete parent folder after ZIP on server
                        using (var zip = new ZipFile())
                        {
                            //zip.UseZip64WhenSaving = Zip64Option.Always;
                            //zip.CompressionMethod = CompressionMethod.BZip2;
                            zip.AddDirectory(directoryPath);
                            zip.Save(zipfilePath);
                        }
                        if (System.IO.File.Exists(zipfilePath))
                        {
                            string fileZipName = Path.GetFileName(zipfilePath);
                            Directory.Delete(directoryPath, true);
                        }
                        FileInfo file = new FileInfo(zipfilePath);

                        //LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading start on FTP : {DateTime.Now}");
                        // ZIP File upload on FTP server
                        CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);
                        //LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading completed on FTP : {DateTime.Now}");

                        exportReportLog.file_name = parentFolder;
                        exportReportLog.file_location = ftpFolder + parentFolder + ".zip";
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Success";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        // Deleted ZIP on Server after uploaded on FTP server
                        System.IO.File.Delete(zipfilePath);
                    }
                    catch (Exception ex)
                    {
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Error occurred while processing request";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        ErrorLogHelper.WriteErrorLog("DownloadEntitySummaryIntoAllKMLNew()", "Report", ex);
                        // delete folder after error generate
                        if (Directory.Exists(directoryPath).Equals(true))
                            Directory.Delete(directoryPath, true);
                    }


                });
            }
        }

        public void DownloadEntitySummaryIntoDXFAll(string entityids)
        {
            if (Session["ExportReportFilterNew"] != null)
            {

                DataSet ds = new DataSet();
                ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.currentPage = 0;
                objExportEntitiesReport.objReportFilters.fileType = "KML";
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                }
                DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);
                for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                {
                    objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = layerDetail.layer_title;
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                }
                objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                string finalkml = KMLHelper.GetKmlForEntities(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, dtFilter);
                //Create a new subfolder under the current active folder
                string newPath = Path.Combine(Server.MapPath("~/Uploads/"));


                //dataContent is byte[]
                System.IO.File.WriteAllText(newPath + "report.kml", finalkml.ToString());
                string baseFolder = newPath + "report.kml";
                string kmlFolder = newPath;
                DataTable dataTable = new DataTable();
                var converter = new Convertor(baseFolder, "", kmlFolder, "");
                var response = converter.KmltoDXFConverter(newPath, "report");

                string attachment = "attachment; filename=ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".dxf";
                Response.ClearContent();
                Response.Clear();
                Response.ContentType = "application/xml";
                Response.AddHeader("content-disposition", attachment);
                Response.Write(response.Output);
                Response.End();
            }
        }

        public void DownloadEntitySummaryIntoDXFAllNew(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["ExportReportFilterNew"] != null)
            {

                DataSet ds = new DataSet();
                ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.currentPage = 0;
                objExportEntitiesReport.objReportFilters.fileType = "KML";
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                }
                DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);


                string fileName = "ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    //int TotalEntityReport = 0;
                    ExportReportLog exportReportLog = new ExportReportLog();
                    exportReportLog.user_id = userdetails.user_id;
                    exportReportLog.export_started_on = DateTime.Now;
                    exportReportLog.file_name = fileName;
                    exportReportLog.file_type = "DXF";
                    exportReportLog.file_extension = ".dxf";
                    exportReportLog.status = "InProgress";
                    exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                    exportReportLog.planned = totalPlannedCount;
                    exportReportLog.asbuilt = totalAsBuiltCount;
                    exportReportLog.dormant = totalDormantCount;
                    exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                    exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                    try
                    {

                        for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                        {
                            objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                            var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                            List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                            DataTable dtReport = new DataTable();
                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                            dtReport.TableName = layerDetail.layer_title;
                            if (dtReport.Rows.Count > 0)
                                ds.Tables.Add(dtReport);
                        }
                        objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                        objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                        string finalkml = KMLHelper.GetKmlForEntitiesNew(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, dtFilter);
                        //Create a new subfolder under the current active folder
                        string newPath = Path.Combine(Server.MapPath("~/Uploads/"));


                        //dataContent is byte[]
                        System.IO.File.WriteAllText(newPath + "report.kml", finalkml.ToString());
                        string baseFolder = newPath + "report.kml";
                        string kmlFolder = newPath;
                        DataTable dataTable = new DataTable();
                        var converter = new Convertor(baseFolder, "", kmlFolder, "");
                        var response = converter.KmltoDXFConverter(newPath, "report");


                        string tempFileName = fileName + exportReportLog.file_extension;
                        //string ftpFolder = "ExportReportLog/";
                        //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                        string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Success";
                        exportReportLog.file_location = ftpFolder + tempFileName;
                        //Thread.Sleep(10000);
                        CommonUtility.FTPFileUpload(response.OutputFile, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        System.IO.File.Delete(response.OutputFile);
                    }
                    catch (Exception ex)
                    {
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Error occurred while processing request";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        ErrorLogHelper.WriteErrorLog("DownloadEntitySummaryIntoDXFAllNew()", "Report", ex);
                    }

                });
                //string attachment = "attachment; filename=ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".dxf";
                //Response.ClearContent();
                //Response.Clear();
                //Response.ContentType = "application/xml";
                //Response.AddHeader("content-disposition", attachment);
                //Response.Write(response.Output);
                //Response.End();
            }
        }

        public void DownloadEntityReportNewIntoShapeAll(string entityids)
        {
            if (Session["ExportReportFilterNew"] != null)
            {
                try
                {
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                    ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                    objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                    objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                    objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                    objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "SHAPE";
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                    //objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                    objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                    objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                    {

                        objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        dtReport.TableName = layerDetail.layer_title;
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        }
                        if (dtReport.Rows.Count > 0)
                            ds.Tables.Add(dtReport);
                    }
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                    GetShapeFile(ds, "ExportReport");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void DownloadEntityReportIntoShapeAllNew(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {
            if (Session["ExportReportFilterNew"] != null)
            {
                try
                {
                    //LogHelper.GetInstance.WriteDebugLog($"***********************************Shape logs start ***  {DateTime.Now}********************************");

                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                    ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                    objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                    ExportEntitiesReportNew entityExportSummaryData = new ExportEntitiesReportNew();

                    entityExportSummaryData = (ExportEntitiesReportNew)Session["EntityExportSummaryData"];

                    objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                    objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                    objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                    objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "SHAPE";
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                    objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                    //objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                    objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                    objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).DistinctBy(o => o.layer_id).ToList();
                    }
                    DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);

                    //DataSet ds = new DataSet();


                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {

                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_type = "ALLSHAPE";
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        dtFilter = null;
                        string shapeFilePath = "";

                        try
                        {

                            string tempFileName = String.Empty;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                            string parentFolder = $"Shape_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                            string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                            string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                            shapeFilePath = Path.Combine(Server.MapPath(pathWithParentFolder));

                            if (Directory.Exists(shapeFilePath).Equals(false))
                                Directory.CreateDirectory(shapeFilePath);

                            var tasks = new List<Task>();
                            foreach (var layer in objExportEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        //LogHelper.GetInstance.WriteDebugLogTest($"====================================={layer.layer_name}====Start  {DateTime.Now}========================", layer.layer_name);

                                        objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                        EntitySummaryReport recordCount = entityExportSummaryData.lstReportData.Where(x => x.entity_name.ToUpper() == layer.layer_name.ToUpper()).FirstOrDefault();
                                        int total_entity_count = 0;
                                        if (recordCount != null)
                                            total_entity_count = recordCount.planned_count + recordCount.as_built_count + recordCount.dormant_count;




                                        //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                        //LogHelper.GetInstance.WriteDebugLogTest($"==========================Layer Title==========={layer.layer_title}============================", layer.layer_name);
                                        List<Dictionary<string, string>> lstExportEntitiesDetailShape = null;
                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                        List<Dictionary<string, string>> lstExportEntitiesDetailAdditional = null;
                                        lstExportEntitiesDetailShape = new BLLayer().GetExportSummaryViewKMLNew(objExportEntitiesReport.objReportFilters, layer.layer_name);

                                        var layerdetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                        if (layerdetails.is_dynamic_control_enable == null)
                                        {
                                            layerdetails.is_dynamic_control_enable = false;
                                        }


                                        List<string> reportTypeString = reportType;

                                        if (total_entity_count > ApplicationSettings.ExcelReportLimitCount)
                                        {
                                            if (reportTypeString[0].Contains("GIS"))
                                            {
                                                lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail.is_dynamic_control_enable)
                                            {
                                                lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            //if (reportTypeString.Contains("") || reportTypeString.Contains("ALL"))
                                            //{
                                            //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);

                                            //    if (layerDetail.is_dynamic_control_enable)
                                            //    {
                                            //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            //    }
                                            //}
                                        }
                                        else
                                        {
                                            if (reportTypeString[0].Contains("GIS"))
                                            {
                                                lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            if (reportTypeString[0].Contains("ADDITIONAL") && layerDetail.is_dynamic_control_enable)
                                            {
                                                lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            //if (reportTypeString.Contains("") || reportTypeString.Contains("ALL"))
                                            //{
                                            //    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            //    if (layerDetail.is_dynamic_control_enable)
                                            //    {
                                            //        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewNewAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            //    }
                                            //}

                                        }



                                        DataTable dtReportShape = new DataTable();
                                        DataTable dtReport = new DataTable();
                                        DataTable dtReportAdditional = new DataTable();

                                        dtReportShape = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailShape);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
                                        dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });

                                        dtReportShape.TableName = layer.layer_title;
                                        dtReport.TableName = layer.layer_title;
                                        dtReportAdditional.TableName = layer.layer_title;

                                        string fileName = $"{parentFolder}.csv";

                                        if (dtReportShape.Rows.Count > 0)
                                        {
                                            if (dtReportShape != null && dtReportShape.Rows.Count > 0)
                                            {
                                                if (dtReportShape.Columns.Contains("S_NO")) { dtReportShape.Columns.Remove("S_NO"); }
                                                if (dtReportShape.Columns.Contains("totalrecords")) { dtReportShape.Columns.Remove("totalrecords"); }
                                                if (dtReportShape.Columns.Contains("Barcode")) { dtReportShape.Columns.Remove("Barcode"); }
                                            }
                                            GetShapeFileOne(dtReportShape, "ExportReport", ftpFilePath, ftpUserName, ftpPwd, shapeFilePath, layer.layer_name);
                                        }


                                        if (lstExportEntitiesDetail != null && lstExportEntitiesDetail.Count > 0)
                                        {
                                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                                                                                                                        //dtReport.TableName = layerDetail.layer_title;
                                            dtReport.TableName = layer.layer_title;
                                            if (dtReport != null && dtReport.Rows.Count > 0)
                                            {
                                                if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                                if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                            }

                                        }

                                        if (lstExportEntitiesDetailAdditional != null && lstExportEntitiesDetailAdditional.Count > 0)
                                        {
                                            dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                                                                                                                                            //dtReport.TableName = layerDetail.layer_title;
                                            dtReportAdditional.TableName = layer.layer_title;
                                            if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                                            {
                                                if (dtReportAdditional.Columns.Contains("S_NO")) { dtReportAdditional.Columns.Remove("S_NO"); }
                                                if (dtReportAdditional.Columns.Contains("totalrecords")) { dtReportAdditional.Columns.Remove("totalrecords"); }
                                                if (dtReportAdditional.Columns.Contains("Barcode")) { dtReportAdditional.Columns.Remove("Barcode"); }
                                            }

                                        }

                                        if (dtReport.Rows.Count > 0 || dtReportAdditional.Rows.Count > 0)
                                        {
                                            if (dtReport.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                            {
                                                dtReport.TableName = dtReport.TableName + "_GisAttribute";
                                                tempFileName = $"{parentFolder}/{dtReport.TableName}.csv";
                                                StreamNewCSVInFolder(dtReport, tempFileName);
                                            }
                                            if (dtReportAdditional.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                            {
                                                dtReportAdditional.TableName = dtReport.TableName + "_AdditionalAttribute";
                                                tempFileName = $"{parentFolder}/{dtReportAdditional.TableName}.csv";
                                                StreamNewCSVInFolder(dtReportAdditional, tempFileName);
                                            }
                                            else
                                            {
                                                //if()
                                                fileName = $"{parentFolder}/{layer.layer_title}.xlsx";
                                                var tempshapeFilePath = $"{shapeFilePath}/{layer.layer_title}.xlsx";
                                                ExportDataExcelMergeWithoutCdb(dtReport, dtReportAdditional, fileName, tempshapeFilePath);
                                            }
                                        }
                                        dtReportShape = null;
                                        dtReport = null;
                                        dtReportAdditional = null;
                                        // LogHelper.GetInstance.WriteDebugLogTest($"====================================={layer.layer_name}====END {DateTime.Now}========================", layer.layer_name);
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));

                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();
                            //for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                            //{

                            //	objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                            //	var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                            //	List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters); 
                            //	DataTable dtReport = new DataTable();
                            //	dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                            //	dtReport.TableName = layerDetail.layer_title;
                            //	if (dtReport != null && dtReport.Rows.Count > 0)
                            //	{
                            //		if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            //		if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            //		if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                            //	}
                            //	//if (dtReport.Rows.Count > 0)
                            //	//	ds.Tables.Add(dtReport);
                            //	GetShapeFileOne(dtReport, "ExportReport", ftpFilePath, ftpUserName, ftpPwd, shapeFilePath);
                            //	dtReport = null;


                            //}


                            //LogHelper.GetInstance.WriteDebugLog($"All entity data fetched and save in shape file on: {DateTime.Now}");

                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;

                            string zipshapePath = shapeFilePath + ".zip";//result.Replace("success:", "");
                                                                         //zip the shape file
                            using (var zip = new ZipFile())
                            {
                                //zip.UseZip64WhenSaving = Zip64Option.Always;
                                //zip.CompressionMethod = CompressionMethod.BZip2;
                                zip.AddDirectory(shapeFilePath);
                                zip.Save(zipshapePath);
                            }
                            if (System.IO.File.Exists(zipshapePath))
                            {
                                string fileName = Path.GetFileName(zipshapePath);
                                Directory.Delete(shapeFilePath, true);
                            }
                            FileInfo file = new FileInfo(zipshapePath);
                            tempFileName = Path.GetFileNameWithoutExtension(file.FullName);

                            // LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading start on FTP : {DateTime.Now}");
                            CommonUtility.FTPFileUpload(zipshapePath, (tempFileName + ".zip"), ftpFilePath, ftpUserName, ftpPwd);
                            // LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading completed on FTP : {DateTime.Now}");
                            System.IO.File.Delete(zipshapePath);

                            exportReportLog.file_name = tempFileName;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName + exportReportLog.file_extension;
                            //Thread.Sleep(10000);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                            //LogHelper.GetInstance.WriteDebugLog($"***********************************Shape logs end ***  {DateTime.Now}********************************");
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportIntoShapeAllNew()", "Report", ex);
                            // delete folder after error generate
                            if (Directory.Exists(shapeFilePath).Equals(true))
                                Directory.Delete(shapeFilePath, true);
                        }
                    });



                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void DownloadEntityReportNewIntoShapeAllNew(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["ExportReportFilterNew"] != null)
            {
                try
                {
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                    ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                    objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                    objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                    objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                    objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                    objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "SHAPE";
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                    objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                    //objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                    objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                    objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);


                    DataSet ds = new DataSet();


                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {

                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_type = "ALLSHAPE";
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        try
                        {

                            for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                            {

                                objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                                var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                                DataTable dtReport = new DataTable();
                                dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                                dtReport.TableName = layerDetail.layer_title;
                                if (dtReport != null && dtReport.Rows.Count > 0)
                                {
                                    if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                    if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                }
                                if (dtReport.Rows.Count > 0)
                                    ds.Tables.Add(dtReport);
                            }
                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;


                            string tempFileName = String.Empty;
                            //string ftpFolder = "ExportReportLog/";
                            //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            GetShapeFileNew(ds, "ExportReport", ftpFilePath, ftpUserName, ftpPwd, ref tempFileName);
                            exportReportLog.file_name = tempFileName;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName + exportReportLog.file_extension;
                            //Thread.Sleep(10000);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoShapeAllNew()", "Report", ex);
                        }
                    });



                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        // for view report after entity summary
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult ExportSummaryView(ExportEntitiesSummaryView objExportEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {
            if (Session["ExportReportFilterNew"] != null)
            {
                //sort = String.IsNullOrEmpty(sort) ? "" : sort.Replace(" ", "_");
                ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();

                objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                objExportEntitiesReport.objReportFilters.SelectedRegionId = objExportReportFilterNew.SelectedRegionId;
                objExportEntitiesReport.objReportFilters.SelectedProvinceId = objExportReportFilterNew.SelectedProvinceId;
                objExportEntitiesReport.objReportFilters.SelectedParentUser = objExportReportFilterNew.SelectedParentUser;
                objExportEntitiesReport.objReportFilters.SelectedUserId = objExportReportFilterNew.SelectedUserId;
                objExportEntitiesReport.objReportFilters.SelectedProjectId = objExportReportFilterNew.SelectedProjectId;
                objExportEntitiesReport.objReportFilters.SelectedPlanningId = objExportReportFilterNew.SelectedPlanningId;
                objExportEntitiesReport.objReportFilters.SelectedWorkOrderId = objExportReportFilterNew.SelectedWorkOrderId;
                objExportEntitiesReport.objReportFilters.SelectedPurposeId = objExportReportFilterNew.SelectedPurposeId;
                objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;

                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.pageSize = 10;
                objExportEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
                objExportEntitiesReport.objReportFilters.sort = sort;
                objExportEntitiesReport.objReportFilters.sortdir = sortdir;
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;
                objExportEntitiesReport.objReportFilters.selected_route_ids = objExportReportFilterNew.selected_route_ids;
                // objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                BindReportDropdownSummaryView(ref objExportEntitiesReport);

                //rt
                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.objReportFilters.SelectedLayerId = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").Where(x => x.layer_name == objExportEntitiesReport.objReportFilters.layerName).Select(x => x.layer_id).ToList();
                objExportEntitiesReport.objReportFilters.lstAdvanceFilters = objExportEntitiesReport.lstAdvanceFilters;
                if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.layerName))
                {
                    objExportEntitiesReport.objReportFilters.advancefilter = getAdvanceFilter(objExportEntitiesReport.lstAdvanceFilters);
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryView(objExportEntitiesReport.objReportFilters);

                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE" };
                    objExportEntitiesReport.webColumns = BLConvertMLanguage.GetEntityWiseColumns(objExportEntitiesReport.objReportFilters.SelectedLayerId[0], objExportEntitiesReport.objReportFilters.layerName, "REPORT", arrIgnoreColumns, userdetails.role_id, userdetails.user_id);
                    foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                    {
                        var obj = (IDictionary<string, object>)new ExpandoObject();

                        foreach (var col in dic)
                        {
                            //if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                            //{
                            //    obj.Add(col.Key, col.Value);
                            //}
                            obj.Add(col.Key, col.Value);
                        }
                        objExportEntitiesReport.lstReportData.Add(obj);
                    }
                    //objExportEntitiesReport.lstReportData = BLConvertMLanguage.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                    objExportEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
                }
            }
            // if there is no filter and add one row by default...
            if (objExportEntitiesReport.lstAdvanceFilters.Count == 0)
            {
                objExportEntitiesReport.lstAdvanceFilters.Add(new ReportAdvanceFilter());
            }
            Session["EntityExportSummaryView"] = objExportEntitiesReport.objReportFilters;
            return PartialView("_EntityExportSummaryView", objExportEntitiesReport);
        }
        public string getAdvanceFilter(List<ReportAdvanceFilter> lstFilters)
        {
            StringBuilder sbFilter = new StringBuilder();
            if (lstFilters != null)
            {
                foreach (var item in lstFilters)
                {
                    if (!string.IsNullOrEmpty(item.searchBy) && !string.IsNullOrEmpty(item.searchType))
                    {
                        item.searchText = item.searchText ?? "";
                        sbFilter.Append(" and upper(COALESCE(a." + item.searchBy + "::text,'')) " + item.searchType + (item.searchType.ToUpper() == "LIKE" ? "'%" + item.searchText.Trim().ToUpper() + "%'" : "'" + item.searchText.Trim().ToUpper() + "'"));
                    }
                }
            }
            return sbFilter.ToString();
        }
        public void BindReportDropdownSummaryView(ref ExportEntitiesSummaryView objExportEntitiesReport)
        {
            //rt
            var userdetails = (User)Session["userDetail"];
            //Bind Layers..
            objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
            var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
            if (selectedlayerids != null)
            {
                if (selectedlayerids.Count > 0)
                    objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
            }
            //objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.objReportFilters.layerName == null ? objExportEntitiesReport.lstLayers[0].layer_name : objExportEntitiesReport.objReportFilters.layerName;
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.layerName))
            {
                objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(objExportEntitiesReport.objReportFilters.layerName);
            }
        }
        public JsonResult DownloadEntitySummaryView(string fileType)
        {
            PageMessage objMsg = new PageMessage();
            string fileName = "ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
                ////create ftp folder if not exist

                string ftpFilePath = ApplicationSettings.FTPAttachment;
                string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                string[] ftplogReportDirectory = new string[] { ftpFolder.Replace("/", "") };
                CreateNestedDirectoryOnFTP(ftpFilePath, ftpUserName, ftpPwd, ftplogReportDirectory);

                if (fileType.ToUpper() == "EXCEL")
                {
                    DownloadEntitySummaryViewIntoExcel(fileName, fileType);
                }
                else if (fileType.ToUpper() == "BARCODEEXCEL")
                {
                    ExportBarcodeSummaryViewIntoExcel(fileName, fileType);
                }
                else if (fileType.ToUpper() == "BARCODEPDF")
                {
                    ExportBarcodeSummaryViewIntoPDF(fileName, fileType);
                }
                else if (fileType.ToUpper() == "KML")
                {
                    DownloadEntitySummaryViewIntoKML(fileName, fileType);
                }
                else if (fileType.ToUpper() == "DXF")
                {
                    DownloadEntitySummaryViewIntoDXF(fileName, fileType);
                }
                else if (fileType.ToUpper() == "SHAPE")
                {
                    ExportSummaryViewIntoShape();
                }
                else if (fileType.ToUpper() == "CSV")
                {
                    DownloadEntitySummaryViewIntoCSV(fileName, fileType);
                }
            }
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Request is processing in background.Please check the Export Report Log.";
            return Json(objMsg, JsonRequestBehavior.AllowGet);

        }
        public void DownloadEntitySummaryViewIntoExcel(string fileName, string fileType)
        {

            if (Session["EntityExportSummaryView"] != null)
            {

                try
                {
                    var userdetails = (User)Session["userDetail"];
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportEntitiesSummaryViewFilter)Session["EntityExportSummaryView"];
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        int totalPlannedCount = 0, totalAsBuiltCount = 0, totalDormantCount = 0;
                        objExportEntitiesReport.objReportFilters.currentPage = 0;
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                        lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                        DataTable dtReport = new DataTable();
                        DataTable bldStatusHistory = new DataTable();
                        DataTable faultStatusHistory = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        if (dtReport.Columns.Contains("Network Status"))
                        {
                            totalPlannedCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Planned").ToList().Count;
                            totalAsBuiltCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "As Built").ToList().Count;
                            totalDormantCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Dormant").ToList().Count;
                        }
                        dtReport.TableName = layerDetail.layer_title;
                        DataSet dataset = new DataSet();
                        ExportReportLog exportReportLog = new ExportReportLog();

                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = fileType;
                        exportReportLog.file_extension = ".xlsx";
                        exportReportLog.status = "InProgress";
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        string tempFileName = fileName + exportReportLog.file_extension;
                        string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                        try
                        {
                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                            }

                            if (layerDetail.layer_name.ToUpper() == "BUILDING")
                            {
                                List<Dictionary<string, string>> lstBuildingComments = new BLLayer().GetBuildingStatusHistory(objExportEntitiesReport.objReportFilters);
                                lstBuildingComments = BLConvertMLanguage.ExportMultilingualConvert(lstBuildingComments);

                                bldStatusHistory = MiscHelper.GetDataTableFromDictionaries(lstBuildingComments);
                                if (bldStatusHistory != null && bldStatusHistory.Rows.Count > 0)
                                {
                                    bldStatusHistory.Columns.Add("Modified_On", typeof(System.String));
                                    foreach (DataRow dr in bldStatusHistory.Rows)
                                    {
                                        dr["Modified_On"] = MiscHelper.FormatDateTime((dr["status_modified_on"].ToString()));
                                    }
                                    if (bldStatusHistory.Columns.Contains("S_NO")) { bldStatusHistory.Columns.Remove("S_NO"); }
                                    if (bldStatusHistory.Columns.Contains("totalrecords")) { bldStatusHistory.Columns.Remove("totalrecords"); }
                                    if (bldStatusHistory.Columns.Contains("building_code")) { bldStatusHistory.Columns["building_code"].ColumnName = "Building Code"; }
                                    if (bldStatusHistory.Columns.Contains("building_status_history")) { bldStatusHistory.Columns["building_status_history"].ColumnName = "Building Status"; }
                                    if (bldStatusHistory.Columns.Contains("comment")) { bldStatusHistory.Columns["comment"].ColumnName = "Comment"; }
                                    if (bldStatusHistory.Columns.Contains("status_modified_by")) { bldStatusHistory.Columns["status_modified_by"].ColumnName = "Status Updated By"; }

                                    if (bldStatusHistory.Columns.Contains("Modified_On")) { bldStatusHistory.Columns["Modified_On"].ColumnName = "Status Updated On"; }
                                    if (bldStatusHistory.Columns.Contains("status_modified_on")) { bldStatusHistory.Columns.Remove("status_modified_on"); }
                                }
                                bldStatusHistory.TableName = "Building_Status_History";

                            }
                            if (layerDetail.layer_name.ToUpper() == "FAULT")
                            {
                                List<Dictionary<string, string>> lstFaultStatus = new BLLayer().GetFaultStatusHistory(objExportEntitiesReport.objReportFilters);
                                lstFaultStatus = BLConvertMLanguage.ExportMultilingualConvert(lstFaultStatus);
                                faultStatusHistory = MiscHelper.GetDataTableFromDictionaries(lstFaultStatus);
                                if (faultStatusHistory != null && faultStatusHistory.Rows.Count > 0)
                                {

                                    faultStatusHistory.Columns.Add("Select Date", typeof(System.String));
                                    faultStatusHistory.Columns.Add("Modified On", typeof(System.String));
                                    foreach (DataRow dr in faultStatusHistory.Rows)
                                    {
                                        dr["Select Date"] = MiscHelper.FormatDate((dr["fault_status_updated_on"].ToString()));
                                        dr["Modified On"] = MiscHelper.FormatDateTime((dr["status_modified_on"].ToString()));
                                    }
                                    if (faultStatusHistory.Columns.Contains("S_NO")) { faultStatusHistory.Columns.Remove("S_NO"); }
                                    if (faultStatusHistory.Columns.Contains("totalrecords")) { faultStatusHistory.Columns.Remove("totalrecords"); }
                                    //if (faultStatusHistory.Columns.Contains("building_code")) { faultStatusHistory.Columns["building_code"].ColumnName = "Building Code"; }
                                    if (faultStatusHistory.Columns.Contains("fault_status_history")) { faultStatusHistory.Columns["fault_status_history"].ColumnName = "Status"; }
                                    if (faultStatusHistory.Columns.Contains("rca")) { faultStatusHistory.Columns["rca"].ColumnName = "RCA"; }
                                    if (faultStatusHistory.Columns.Contains("updated_by")) { faultStatusHistory.Columns["updated_by"].ColumnName = "Updated By"; }
                                    if (faultStatusHistory.Columns.Contains("fault_code")) { faultStatusHistory.Columns["fault_code"].ColumnName = "Network Id"; }
                                    if (faultStatusHistory.Columns.Contains("requested_by")) { faultStatusHistory.Columns["requested_by"].ColumnName = "Requested By"; }
                                    if (faultStatusHistory.Columns.Contains("request_comment")) { faultStatusHistory.Columns["request_comment"].ColumnName = "Request Comment"; }
                                    if (faultStatusHistory.Columns.Contains("status_modified_on")) { faultStatusHistory.Columns.Remove("status_modified_on"); }
                                    if (faultStatusHistory.Columns.Contains("status_modified_by")) { faultStatusHistory.Columns.Remove("status_modified_by"); }
                                    if (faultStatusHistory.Columns.Contains("fault_status_updated_on")) { faultStatusHistory.Columns.Remove("fault_status_updated_on"); }
                                    //if (faultStatusHistory.Columns.Contains("Modified On")) { faultStatusHistory.Columns.Remove("Modified On"); }
                                }
                                faultStatusHistory.TableName = "Fault_Status_History";

                            }

                            if (layerDetail.is_reference_allowed)
                            {
                                #region Add the new sheet for entity Reference and export with Data set
                                DataSet ds = new DataSet();

                                var entityData = new BLMisc().GetEntityReferenceExportData<Dictionary<string, string>>(0, layerDetail.layer_name, string.Empty);
                                DataTable dtReference = MiscHelper.GetDataTableFromDictionaries(entityData);
                                dtReference.TableName = layerDetail.layer_title + "_Reference";
                                ds.Tables.Add(dtFilter);
                                ds.Tables.Add(dtReport);
                                if (layerDetail.layer_name.ToUpper() == "BUILDING")
                                {
                                    ds.Tables.Add(bldStatusHistory);
                                }
                                ds.Tables.Add(dtReference);
                                ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                                #endregion
                            }
                            else if (dtReport.Rows.Count > 0)
                            {
                                dataset.Tables.Add(dtFilter);
                                dataset.Tables.Add(dtReport);
                                if (layerDetail.layer_name.ToUpper() == "BUILDING")
                                {
                                    dataset.Tables.Add(bldStatusHistory);
                                }
                                if (layerDetail.layer_name.ToUpper() == "FAULT")
                                {
                                    dataset.Tables.Add(faultStatusHistory);
                                }
                                ExportData(dataset, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                            }
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoExcelNew()", "Report", ex);
                        }

                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void ExportBarcodeSummaryViewIntoExcel(string fileName, string fileType)
        {

            if (Session["EntityExportSummaryView"] != null)
            {
                try
                {
                    var userdetails = (User)Session["userDetail"];
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportEntitiesSummaryViewFilter)Session["EntityExportSummaryView"];
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportBarcodeBulkSummaryViewData(objExportEntitiesReport.objReportFilters);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);

                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        int totalPlannedCount = 0, totalAsBuiltCount = 0, totalDormantCount = 0;
                        objExportEntitiesReport.objReportFilters.currentPage = 0;
                        ExportReportLog exportReportLog = new ExportReportLog();
                        DataSet ds = new DataSet();
                        ds.Tables.Add(dtFilter);
                        if (lstExportEntitiesDetail.Count > 0)
                        {
                            DataTable dtReport = new DataTable();
                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                            if (dtReport.Columns.Contains("Network Status"))
                            {
                                totalPlannedCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Planned").ToList().Count;
                                totalAsBuiltCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "As Built").ToList().Count;
                                totalDormantCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Dormant").ToList().Count;
                            }
                            exportReportLog.user_id = userdetails.user_id;
                            exportReportLog.export_started_on = DateTime.Now;
                            exportReportLog.file_name = fileName;
                            exportReportLog.file_type = fileType;
                            exportReportLog.file_extension = ".xlsx";
                            exportReportLog.status = "InProgress";
                            exportReportLog.planned = totalPlannedCount;
                            exportReportLog.asbuilt = totalAsBuiltCount;
                            exportReportLog.dormant = totalDormantCount;
                            exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                            exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                            string tempFileName = fileName + exportReportLog.file_extension;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                            dtReport.TableName = layerDetail.layer_title;
                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                                if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            }
                            ds.Tables.Add(dtReport);
                            ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                    });

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }
        public void ExportBarcodeSummaryViewIntoPDF(string fileName, string fileType)
        {
            if (Session["EntityExportSummaryView"] != null)
            {
                try
                {
                    var userdetails = (User)Session["userDetail"];
                    //iTextSharp.text.Font font = new iTextSharp.text.Font(PDFHelper.GetFont(), 12, iTextSharp.text.Font.NORMAL);
                    //iTextSharp.text.Font font1 = new iTextSharp.text.Font(PDFHelper.GetFont(), 7, iTextSharp.text.Font.NORMAL);
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportEntitiesSummaryViewFilter)Session["EntityExportSummaryView"];
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportBarcodeBulkSummaryViewData(objExportEntitiesReport.objReportFilters);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);

                    BaseFont baseFont = PDFHelper.GetFont();

                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        int totalPlannedCount = 0, totalAsBuiltCount = 0, totalDormantCount = 0;
                        ExportReportLog exportReportLog = new ExportReportLog();
                        objExportEntitiesReport.objReportFilters.currentPage = 0;
                        DataSet ds = new DataSet();
                        ds.Tables.Add(dtFilter);
                        //sb.Append("<table><tr><td cellpadding='15' colspan='3'  align='center'>Filter Detail</td></tr></table><table border='1' align='center'><tr  bgcolor='#00ba8a' color='#ffffff' align='center'><th>Filter Type</th><th>Value</th></tr>");
                        //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        //{
                        //    sb.Append("<tr><td  align='center'>" + ds.Tables[0].Rows[i][0] + "</td><td  align='center'>" + ds.Tables[0].Rows[i][1] + "</td></tr>");
                        //}
                        //sb.Append("</table>");

                        //BarCodeTable.AddCell
                        try
                        {
                            if (lstExportEntitiesDetail.Count > 0)
                            {
                                //Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 40f, 40f);
                                //PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                                //pdfWriter.PageEvent = new PDFHelper.AllPdfPageEvents();
                                //pdfDoc.Open();

                                //PdfPTable BarCodeTable = new PdfPTable(2);
                                //BarCodeTable.SetTotalWidth(new float[] { 50f, 50f });
                                //BarCodeTable.DefaultCell.Border = PdfPCell.NO_BORDER;

                                //PdfPCell filterCell = new PdfPCell()
                                //{
                                //    PaddingBottom = 10f,
                                //    PaddingTop = 2f,
                                //    PaddingLeft = 5f,
                                //    PaddingRight = 5f,
                                //    BorderWidth = 1
                                //};
                                //var filterTitle = new Paragraph(new Chunk(Resources.Resources.SI_GBL_GBL_NET_FRM_165, font));
                                //filterTitle.Alignment = Element.ALIGN_CENTER; filterCell.Colspan = 2;
                                //filterCell.AddElement(filterTitle); BarCodeTable.AddCell(filterCell); filterCell = new PdfPCell();

                                //var filterHeader = new Paragraph(new Chunk(Resources.Resources.SI_GBL_GBL_NET_FRM_166, font));
                                //var filterHeader1 = new Paragraph(new Chunk(Resources.Resources.SI_GBL_GBL_NET_FRM_167, font));
                                //filterHeader.Alignment = Element.ALIGN_CENTER;
                                //filterCell.AddElement(filterHeader);
                                //BarCodeTable.AddCell(filterCell); filterCell = new PdfPCell();
                                //filterHeader1.Alignment = Element.ALIGN_CENTER;
                                //filterCell.AddElement(filterHeader1);
                                //BarCodeTable.AddCell(filterCell);
                                //for (int i = 0; i < dtFilter.Rows.Count; i++)
                                //{
                                //    filterCell = new PdfPCell();
                                //    var filterrow = new Paragraph(new Chunk(dtFilter.Rows[i][0].ToString(), font1));
                                //    var filterrow1 = new Paragraph(new Chunk(dtFilter.Rows[i][1].ToString(), FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                //    filterrow.Alignment = Element.ALIGN_CENTER;
                                //    filterCell.AddElement(filterrow);
                                //    BarCodeTable.AddCell(filterCell);

                                //    filterCell = new PdfPCell();
                                //    filterrow1.Alignment = Element.ALIGN_CENTER;
                                //    filterCell.AddElement(filterrow1);
                                //    BarCodeTable.AddCell(filterCell);
                                //}
                                //filterCell = new PdfPCell();
                                //var filterEnd = new Paragraph(new Chunk(" ", FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                                //filterCell.AddElement(filterEnd); filterCell.Border = 0;
                                //BarCodeTable.AddCell(filterCell); filterCell = new PdfPCell();

                                //filterCell.AddElement(filterEnd); filterCell.Border = 0;
                                //BarCodeTable.AddCell(filterCell); filterCell = new PdfPCell();

                                //PdfPCell cell = new PdfPCell()
                                //{
                                //    PaddingBottom = 10f,
                                //    PaddingTop = 10f,
                                //    PaddingLeft = 15f,
                                //    PaddingRight = 15f,
                                //    BorderWidth = 1
                                //};
                                //var BarcodeTitle = new Paragraph(new Chunk(Resources.Resources.SI_OSP_GBL_NET_FRM_428, font));
                                //BarcodeTitle.Alignment = Element.ALIGN_CENTER; cell.Colspan = 2; cell.AddElement(BarcodeTitle);
                                //BarCodeTable.AddCell(cell); cell = new PdfPCell();
                                //foreach (var item in lstExportEntitiesDetail)
                                //{
                                //    cell = new PdfPCell();
                                //    var barcode = new Paragraph(new Chunk("Barcode: " + item[Resources.Resources.SI_OSP_GBL_GBL_GBL_065], font1));
                                //    var name = new Paragraph(new Chunk("Name: " + item[Resources.Resources.SI_OSP_GBL_NET_RPT_008], font1));
                                //    barcode.Alignment = Element.ALIGN_CENTER;
                                //    name.Alignment = Element.ALIGN_CENTER;


                                //    if (!string.IsNullOrEmpty(item[Resources.Resources.SI_OSP_GBL_GBL_GBL_065]))
                                //    {

                                //        byte[] byt = BarcodeHelper.GenerateBarcode(item[Resources.Resources.SI_OSP_GBL_GBL_GBL_065].Trim(), true);
                                //        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(byt);
                                //        //img.ScaleAbsoluteHeight(5f);
                                //        img.Alignment = Element.ALIGN_CENTER;
                                //        cell.AddElement(img);
                                //        cell.AddElement(barcode);
                                //    }
                                //    else
                                //    {

                                //        var noBarCodeText = new Paragraph(new Chunk(Resources.Resources.SI_OSP_HTB_NET_FRM_011, font1));
                                //        noBarCodeText.Alignment = Element.ALIGN_CENTER;
                                //        cell.AddElement(noBarCodeText);

                                //    }


                                //    cell.AddElement(name);
                                //    BarCodeTable.AddCell(cell);


                                //}
                                //BarCodeTable.AddCell("");
                                DataTable dtReport = new DataTable();
                                dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                                if (dtReport.Columns.Contains("Network Status"))
                                {
                                    totalPlannedCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Planned").ToList().Count;
                                    totalAsBuiltCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "As Built").ToList().Count;
                                    totalDormantCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Dormant").ToList().Count;
                                }
                                exportReportLog.user_id = userdetails.user_id;
                                exportReportLog.export_started_on = DateTime.Now;
                                exportReportLog.file_name = fileName;
                                exportReportLog.file_type = fileType;
                                exportReportLog.file_extension = ".pdf";
                                exportReportLog.status = "InProgress";
                                exportReportLog.planned = totalPlannedCount;
                                exportReportLog.asbuilt = totalAsBuiltCount;
                                exportReportLog.dormant = totalDormantCount;
                                exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                                exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                                exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                                string tempFileName = fileName + exportReportLog.file_extension;
                                string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                                string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                                string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                                dtReport.TableName = layerDetail.layer_title;
                                if (dtReport != null && dtReport.Rows.Count > 0)
                                {
                                    if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                                    if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                }
                                ds.Tables.Add(dtReport);
                                GenerateToPDFNew(ds, tempFileName, Resources.Resources.SI_OSP_GBL_NET_RPT_127, ftpFilePath, ftpUserName, ftpPwd, baseFont);
                                exportReportLog.file_location = ftpFolder + tempFileName;
                                exportReportLog.export_ended_on = DateTime.Now;
                                exportReportLog.status = "Success";
                                exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            }
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoPDFNEW()", "Report", ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadEntitySummaryViewIntoKML(string fileName, string fileType)
        {
            if (Session["EntityExportSummaryView"] != null)
            {
                try
                {
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                    objExportEntitiesReport.objReportFilters = (ExportEntitiesSummaryViewFilter)Session["EntityExportSummaryView"];
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);
                    var userdetails = (User)Session["userDetail"];
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        int totalPlannedCount = 0, totalAsBuiltCount = 0, totalDormantCount = 0;
                        DataSet ds = new DataSet();
                        objExportEntitiesReport.objReportFilters.currentPage = 0;
                        objExportEntitiesReport.objReportFilters.fileType = "KML";
                        //Filter the Layer Detail
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        if (dtReport.Columns.Contains("Network Status"))
                        {
                            totalPlannedCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Planned").ToList().Count;
                            totalAsBuiltCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "As Built").ToList().Count;
                            totalDormantCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Dormant").ToList().Count;
                        }
                        dtReport.TableName = layerDetail.layer_name;
                        string layerName = layerDetail.layer_name;
                        layerName = layerName + "_";

                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = fileType;
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        string tempFileName = fileName + exportReportLog.file_extension;
                        string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                        try
                        {
                            if (dtReport.Rows.Count > 0)
                                ds.Tables.Add(dtReport);
                            objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                            if (objExportEntitiesReport.objReportFilters.layerName != null)
                            {
                                objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(m.layer_id)).ToList();
                            }
                            string TempkmlFileName = "ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
                            KMLHelper.DatasetToKMLNew(ftpFilePath, ftpUserName, ftpPwd, ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, fileName, TempkmlFileName, "", dtFilter, fileType);
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntitySummaryIntoKMLAllNew()", "Report", ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public string getKmlForSingleEntity()
        {
            string finalkml = string.Empty;
            if (Session["EntityExportSummaryView"] != null)
            {
                try
                {
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                    objExportEntitiesReport.objReportFilters = (ExportEntitiesSummaryViewFilter)Session["EntityExportSummaryView"];
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "KML";
                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    if (objExportEntitiesReport.objReportFilters.layerName != null)
                    {
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = layerDetail.layer_name;
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                    finalkml = KMLHelper.GetKmlForEntities(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, dtFilter);
                    return finalkml;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return finalkml;
        }
        public void DownloadEntitySummaryViewIntoDXF(string fileName, string fileType)
        {
            if (Session["EntityExportSummaryView"] != null)
            {
                ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                objExportEntitiesReport.objReportFilters = (ExportEntitiesSummaryViewFilter)Session["EntityExportSummaryView"];
                DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);
                var userdetails = (User)Session["userDetail"];
                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    int totalPlannedCount = 0, totalAsBuiltCount = 0, totalDormantCount = 0;
                    DataSet ds = new DataSet();
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "KML";
                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    if (dtReport.Columns.Contains("Network Status"))
                    {
                        totalPlannedCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Planned").ToList().Count;
                        totalAsBuiltCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "As Built").ToList().Count;
                        totalDormantCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Dormant").ToList().Count;
                    }
                    dtReport.TableName = layerDetail.layer_name;
                    string layerName = layerDetail.layer_name;
                    layerName = layerName + "_";
                    ExportReportLog exportReportLog = new ExportReportLog();
                    exportReportLog.user_id = userdetails.user_id;
                    exportReportLog.export_started_on = DateTime.Now;
                    exportReportLog.file_name = fileName;
                    exportReportLog.file_type = fileType;
                    exportReportLog.file_extension = ".dxf";
                    exportReportLog.status = "InProgress";
                    exportReportLog.planned = totalPlannedCount;
                    exportReportLog.asbuilt = totalAsBuiltCount;
                    exportReportLog.dormant = totalDormantCount;
                    exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                    exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                    exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                    try
                    {

                        if (dtReport.Rows.Count > 0)
                            ds.Tables.Add(dtReport);
                        objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                        if (objExportEntitiesReport.objReportFilters.layerName != null)
                        {
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(m.layer_id)).ToList();
                        }
                        string finalkml = KMLHelper.GetKmlForEntitiesNew(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, dtFilter);
                        //Create a new subfolder under the current active folder
                        string newPath = Path.Combine(Server.MapPath("~/Uploads/"));

                        //dataContent is byte[]
                        System.IO.File.WriteAllText(newPath + "report.kml", finalkml.ToString());
                        string baseFolder = newPath + "report.kml";
                        string kmlFolder = newPath;
                        DataTable dataTable = new DataTable();
                        var converter = new Convertor(baseFolder, "", kmlFolder, "");
                        var response = converter.KmltoDXFConverter(newPath, "report");

                        //string attachment = "attachment; filename=ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".dxf";
                        //Response.ClearContent();
                        //Response.Clear();
                        //Response.ContentType = "application/xml";
                        //Response.AddHeader("content-disposition", attachment);
                        //Response.Write(response.Output);
                        //Response.End();
                        // string tempFileName = fileName + exportReportLog.file_extension;
                        //string ftpFolder = "ExportReportLog/";
                        //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;

                        string tempFileName = fileName + exportReportLog.file_extension;
                        string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                        CommonUtility.FTPFileUpload(response.OutputFile, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                        exportReportLog.file_location = ftpFolder + tempFileName;
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Success";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        System.IO.File.Delete(response.OutputFile);
                    }
                    catch (Exception ex)
                    {
                        exportReportLog.export_ended_on = DateTime.Now;
                        exportReportLog.status = "Error occurred while processing request";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        ErrorLogHelper.WriteErrorLog("DownloadEntitySummaryIntoDXFAllNew()", "Report", ex);
                    }
                });
            }
        }
        public void ExportSummaryViewIntoShape()
        {

            if (Session["EntityExportSummaryView"] != null)
            {
                try
                {
                    var userdetails = (User)Session["userDetail"];
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportEntitiesSummaryViewFilter)Session["EntityExportSummaryView"];
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        int totalPlannedCount = 0, totalAsBuiltCount = 0, totalDormantCount = 0;
                        objExportEntitiesReport.objReportFilters.currentPage = 0;
                        objExportEntitiesReport.objReportFilters.fileType = "SHAPE";

                        //Filter the Layer Detail
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
                        DataTable dtReport = new DataTable();

                        ExportReportLog exportReportLog = new ExportReportLog();
                        try
                        {
                            if (lstExportEntitiesDetail.Count > 0)
                            {
                                dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                                if (dtReport.Columns.Contains("Network Status"))
                                {
                                    totalPlannedCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Planned").ToList().Count;
                                    totalAsBuiltCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "As Built").ToList().Count;
                                    totalDormantCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Dormant").ToList().Count;
                                }
                                exportReportLog.user_id = userdetails.user_id;
                                exportReportLog.export_started_on = DateTime.Now;
                                exportReportLog.file_type = "SHAPE";
                                exportReportLog.file_extension = ".zip";
                                exportReportLog.status = "InProgress";
                                exportReportLog.planned = totalPlannedCount;
                                exportReportLog.asbuilt = totalAsBuiltCount;
                                exportReportLog.dormant = totalDormantCount;
                                exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                                exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                                exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                                dtReport.TableName = layerDetail.layer_title;
                                if (dtReport != null && dtReport.Rows.Count > 0)
                                {
                                    if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                                    if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                }
                                //ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_Report_" + MiscHelper.getTimeStamp(), false);
                            }
                            DataSet ds = new DataSet();
                            ds.Tables.Add(dtReport);
                            string tempFileName = String.Empty;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            GetShapeFileNew(ds, "ExportReport", ftpFilePath, ftpUserName, ftpPwd, ref tempFileName);
                            exportReportLog.file_name = tempFileName;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName + exportReportLog.file_extension;
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoShapeAllNew()", "Report", ex);
                        }
                    });

                }

                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }
        public void GetShapeFile(DataSet ds, string fileNameValue)
        {
            string entity = string.Empty;
            string shapeFilePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"public\Attachments\SI_Shape_" + Guid.NewGuid().ToString().Substring(0, 8);//Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + "Downloads" + @"\SI_Shape_" + Guid.NewGuid().ToString().Substring(0, 8);
                                                                                                                                                                                //string entity = objExportEntitiesReport.objReportFilters.layerName;
            if (Directory.Exists(shapeFilePath).Equals(false))
                Directory.CreateDirectory(shapeFilePath);
            for (int j = 0; j < ds.Tables.Count; j++)
            {
                DataTable dtReport = ds.Tables[j];
                entity = dtReport.TableName;
                if (dtReport.AsEnumerable().Where(x => x.Field<string>("geom") != null).ToList().Count > 0)
                {
                    dtReport = dtReport.AsEnumerable().Where(x => x.Field<string>("geom") != null).CopyToDataTable();
                }
                else
                    dtReport.Clear();
                if (dtReport.Rows.Count > 0)
                {
                    var features = new List<Feature>();
                    string columnName = string.Empty, columnNameList = string.Empty;

                    foreach (DataRow row in dtReport.Rows)
                    {
                        //add shape attribute and value
                        var geomFactory = new GeometryFactory(new PrecisionModel(), 4326);
                        var wktReader = new WKTReader(geomFactory);
                        var geometry = wktReader.Read(row["geom"].ToString());
                        var attributesTable = new AttributesTable();
                        columnNameList = string.Empty;
                        foreach (DataColumn column in dtReport.Columns)
                        {
                            if (!column.ColumnName.Equals("sp_geometry") && !column.ColumnName.Equals("geom"))
                            {
                                columnName = column.ColumnName.Length > 10 ? column.ColumnName.Substring(0, 10) : column.ColumnName; //column name allow only 11 charaters
                                if (columnNameList.Contains("," + columnName + ","))
                                {
                                    int i = 1;
                                    while (true)
                                    {
                                        columnName = column.ColumnName.Substring(0, 9) + i.ToString();
                                        if (!columnNameList.Contains(columnName))
                                        {
                                            break;
                                        }
                                        i++;
                                    }
                                }
                                columnNameList = columnNameList + "," + columnName + ",";
                                attributesTable.AddAttribute(columnName, row[column.ColumnName].ToString());
                            }
                        }
                        features.Add(new Feature(geometry, attributesTable));
                    }

                    var shapeFileName = shapeFilePath + "\\" + entity;
                    var shapeFilePrjName = Path.Combine(shapeFilePath, entity + ".prj");

                    // Create the shapefile
                    var outGeomFactory = GeometryFactory.Default;
                    var writer = new ShapefileDataWriter(shapeFileName, outGeomFactory);
                    var outDbaseHeader = ShapefileDataWriter.GetHeader(features[0], features.Count);
                    writer.Header = outDbaseHeader;
                    writer.Write(features);

                    // Create the projection file
                    using (var streamWriter = new StreamWriter(shapeFilePrjName))
                    {
                        streamWriter.Write(GeographicCoordinateSystem.WGS84.WKT);
                    }

                }
                dtReport.Clear();
            }

            string zipshapePath = shapeFilePath + ".zip";//result.Replace("success:", "");
                                                         //zip the shape file
            using (var zip = new ZipFile())
            {
                //zip.UseZip64WhenSaving = Zip64Option.Always;
                //zip.CompressionMethod = CompressionMethod.BZip2;
                zip.AddDirectory(shapeFilePath);
                zip.Save(zipshapePath);
            }
            if (System.IO.File.Exists(zipshapePath))
            {
                string fileName = Path.GetFileName(zipshapePath);
                Directory.Delete(shapeFilePath, true);
            }
            FileInfo file = new FileInfo(zipshapePath);
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileNameValue + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "application/zip";
            Response.WriteFile(file.FullName);
            Response.Flush();
            Response.End();
            System.IO.File.Delete(zipshapePath);
        }
        public void GetShapeFileNew(DataSet ds, string fileNameValue, string ftpfilePath, string ftpUserName, string ftpPassword, ref string tempFileName)
        {
            string entity = string.Empty;
            string shapeFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/public/Attachments/SI_Shape_" + Guid.NewGuid().ToString().Substring(0, 8));
            //Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + "Downloads" + @"\SI_Shape_" + Guid.NewGuid().ToString().Substring(0, 8);
            //string entity = objExportEntitiesReport.objReportFilters.layerName;
            if (Directory.Exists(shapeFilePath).Equals(false))
                Directory.CreateDirectory(shapeFilePath);
            for (int j = 0; j < ds.Tables.Count; j++)
            {
                DataTable dtReport = ds.Tables[j];
                entity = dtReport.TableName;
                if (dtReport.AsEnumerable().Where(x => x.Field<string>("geom") != null).ToList().Count > 0)
                {
                    dtReport = dtReport.AsEnumerable().Where(x => x.Field<string>("geom") != null).CopyToDataTable();
                }
                else
                    dtReport.Clear();
                if (dtReport.Rows.Count > 0)
                {
                    var features = new List<Feature>();
                    string columnName = string.Empty, columnNameList = string.Empty;

                    foreach (DataRow row in dtReport.Rows)
                    {
                        //add shape attribute and value
                        var geomFactory = new GeometryFactory(new PrecisionModel(), 4326);
                        var wktReader = new WKTReader(geomFactory);
                        var geometry = wktReader.Read(row["geom"].ToString());
                        var attributesTable = new AttributesTable();
                        columnNameList = string.Empty;
                        foreach (DataColumn column in dtReport.Columns)
                        {
                            if (!column.ColumnName.Equals("sp_geometry") && !column.ColumnName.Equals("geom"))
                            {
                                columnName = column.ColumnName.Length > 10 ? column.ColumnName.Substring(0, 10) : column.ColumnName; //column name allow only 11 charaters
                                if (columnNameList.Contains("," + columnName + ","))
                                {
                                    int i = 1;
                                    while (true)
                                    {
                                        columnName = column.ColumnName.Substring(0, 9) + i.ToString();
                                        if (!columnNameList.Contains(columnName))
                                        {
                                            break;
                                        }
                                        i++;
                                    }
                                }
                                columnNameList = columnNameList + "," + columnName + ",";
                                attributesTable.AddAttribute(columnName, row[column.ColumnName].ToString());
                            }
                        }
                        features.Add(new Feature(geometry, attributesTable));
                    }

                    var shapeFileName = shapeFilePath + "\\" + entity;
                    var shapeFilePrjName = Path.Combine(shapeFilePath, entity + ".prj");

                    // Create the shapefile
                    var outGeomFactory = GeometryFactory.Default;
                    var writer = new ShapefileDataWriter(shapeFileName, outGeomFactory);
                    var outDbaseHeader = ShapefileDataWriter.GetHeader(features[0], features.Count);
                    writer.Header = outDbaseHeader;
                    writer.Write(features);

                    // Create the projection file
                    using (var streamWriter = new StreamWriter(shapeFilePrjName))
                    {
                        streamWriter.Write(GeographicCoordinateSystem.WGS84.WKT);
                    }

                }
                dtReport.Clear();
            }
            string zipshapePath = shapeFilePath + ".zip";//result.Replace("success:", "");
                                                         //zip the shape file
            using (var zip = new ZipFile())
            {
                //zip.UseZip64WhenSaving = Zip64Option.Always;
                //zip.CompressionMethod = CompressionMethod.BZip2;
                zip.AddDirectory(shapeFilePath);
                zip.Save(zipshapePath);
            }
            if (System.IO.File.Exists(zipshapePath))
            {
                string fileName = Path.GetFileName(zipshapePath);
                Directory.Delete(shapeFilePath, true);
            }
            FileInfo file = new FileInfo(zipshapePath);
            tempFileName = Path.GetFileNameWithoutExtension(file.FullName);

            CommonUtility.FTPFileUpload(zipshapePath, (tempFileName + ".zip"), ftpfilePath, ftpUserName, ftpPassword);
            //Response.Clear();
            //Response.AddHeader("Content-Disposition", "attachment; filename=" + fileNameValue + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
            //Response.AddHeader("Content-Length", file.Length.ToString());
            //Response.ContentType = "application/zip";
            //Response.WriteFile(file.FullName);
            //Response.Flush();
            //Response.End();
            System.IO.File.Delete(zipshapePath);
        }
        public void GetShapeFileOne(DataTable dtReport, string fileNameValue, string ftpfilePath, string ftpUserName, string ftpPassword, string shapeFilePath, string layerName)
        {
            // LogHelper.GetInstance.WriteDebugLogTest($"====Shape File Start:-{dtReport.TableName} on {DateTime.Now}======", layerName);
            string entity = string.Empty;
            entity = dtReport.TableName;
            if (dtReport.AsEnumerable().Where(x => x.Field<string>("geom") != null).ToList().Count > 0)
            {
                dtReport = dtReport.AsEnumerable().Where(x => x.Field<string>("geom") != null).CopyToDataTable();
            }
            else
                dtReport.Clear();
            if (dtReport.Rows.Count > 0)
            {
                var features = new List<Feature>();
                string columnName = string.Empty, columnNameList = string.Empty;

                foreach (DataRow row in dtReport.Rows)
                {
                    //add shape attribute and value
                    var geomFactory = new GeometryFactory(new PrecisionModel(), 4326);
                    var wktReader = new WKTReader(geomFactory);
                    var geometry = wktReader.Read(row["geom"].ToString());
                    var attributesTable = new AttributesTable();
                    columnNameList = string.Empty;
                    foreach (DataColumn column in dtReport.Columns)
                    {
                        if (!column.ColumnName.Equals("sp_geometry") && !column.ColumnName.Equals("geom"))
                        {
                            columnName = column.ColumnName.Length > 10 ? column.ColumnName.Substring(0, 10) : column.ColumnName; //column name allow only 11 charaters
                            if (columnNameList.Contains("," + columnName + ","))
                            {
                                int i = 1;
                                while (true)
                                {
                                    columnName = column.ColumnName.Substring(0, 9) + i.ToString();
                                    if (!columnNameList.Contains(columnName))
                                    {
                                        break;
                                    }
                                    i++;
                                }
                            }
                            columnNameList = columnNameList + "," + columnName + ",";
                            attributesTable.AddAttribute(columnName, row[column.ColumnName].ToString());
                        }
                    }
                    features.Add(new Feature(geometry, attributesTable));
                }

                var shapeFileName = shapeFilePath + "\\" + entity;
                var shapeFilePrjName = Path.Combine(shapeFilePath, entity + ".prj");
                // LogHelper.GetInstance.WriteDebugLogTest($"=====shape file name:-{shapeFileName} ===", layerName);

                // Create the shapefile
                var outGeomFactory = GeometryFactory.Default;
                var writer = new ShapefileDataWriter(shapeFileName, outGeomFactory);
                var outDbaseHeader = ShapefileDataWriter.GetHeader(features[0], features.Count);
                writer.Header = outDbaseHeader;
                writer.Write(features);

                // Create the projection file
                using (var streamWriter = new StreamWriter(shapeFilePrjName))
                {
                    streamWriter.Write(GeographicCoordinateSystem.WGS84.WKT);
                }
            }

            dtReport.Clear();
        }

        #endregion Export Entites Report



        #region Association Report
        [System.Web.Services.WebMethod(true)]
        public ActionResult EntityAssociationReport(AssociationEntitiesReport objAssociationEntitiesReport, string IsRequestFromInfo)
        {
            DateTime startTime = DateTime.Now;
            var userdetails = (User)Session["userDetail"];

            var moduleAbbr = "EXASSRPT";
            ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);

            if (con != null)
            {
                objAssociationEntitiesReport.objReportFilters.connectionString = con.connection_string;
            }


            objAssociationEntitiesReport.objReportFilters.SelectedRegionIds = objAssociationEntitiesReport.objReportFilters.SelectedRegionId != null && objAssociationEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objAssociationEntitiesReport.objReportFilters.SelectedProvinceIds = objAssociationEntitiesReport.objReportFilters.SelectedProvinceId != null && objAssociationEntitiesReport.objReportFilters.SelectedProvinceId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
            objAssociationEntitiesReport.objReportFilters.SelectedParentUsers = objAssociationEntitiesReport.objReportFilters.SelectedParentUser != null && objAssociationEntitiesReport.objReportFilters.SelectedParentUser.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedParentUser.ToArray()) : "";//((User)Session["userDetail"]).user_id.ToString();
            objAssociationEntitiesReport.objReportFilters.SelectedUserIds = objAssociationEntitiesReport.objReportFilters.SelectedUserId != null && objAssociationEntitiesReport.objReportFilters.SelectedUserId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedUserId.ToArray()) : "";
            objAssociationEntitiesReport.objReportFilters.SelectedLayerIds = objAssociationEntitiesReport.objReportFilters.SelectedLayerId != null && objAssociationEntitiesReport.objReportFilters.SelectedLayerId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedLayerId.ToArray()) : "";// objExportEntitiesReport.objReportFilters.SelectedLayerId != null && objExportEntitiesReport.objReportFilters.SelectedLayerId.Count > 0 ? ("'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedLayerId.ToArray()) + "'").ToString().ToLower() : "";
            objAssociationEntitiesReport.objReportFilters.SelectedProjectIds = objAssociationEntitiesReport.objReportFilters.SelectedProjectId != null && objAssociationEntitiesReport.objReportFilters.SelectedProjectId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedProjectId.ToArray()) : "";
            objAssociationEntitiesReport.objReportFilters.SelectedPlanningIds = objAssociationEntitiesReport.objReportFilters.SelectedPlanningId != null && objAssociationEntitiesReport.objReportFilters.SelectedPlanningId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedPlanningId.ToArray()) : "";
            objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderId != null && objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderId.ToArray()) : "";
            objAssociationEntitiesReport.objReportFilters.SelectedPurposeIds = objAssociationEntitiesReport.objReportFilters.SelectedPurposeId != null && objAssociationEntitiesReport.objReportFilters.SelectedPurposeId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedPurposeId.ToArray()) : "";
            objAssociationEntitiesReport.objReportFilters.userId = Convert.ToInt32(userdetails.user_id);
            objAssociationEntitiesReport.objReportFilters.roleId = Convert.ToInt32(userdetails.role_id);
            objAssociationEntitiesReport.objReportFilters.is_all_provience_assigned = userdetails.is_all_provience_assigned;
            objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType = objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType != null ? objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType : "";
            objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorId != null && objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorId.Count > 0 ? string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorId.ToArray()) : "";
            objAssociationEntitiesReport.objReportFilters.selected_route_ids = objAssociationEntitiesReport.selected_route_ids != null && objAssociationEntitiesReport.selected_route_ids.Count > 0 ? string.Join(",", objAssociationEntitiesReport.selected_route_ids.ToArray()) : "";

            var selectedLayers = objAssociationEntitiesReport.objReportFilters.SelectedLayerIds;
            //if (!string.IsNullOrEmpty(IsRequestFromInfo) && Convert.ToBoolean(IsRequestFromInfo))
            //{
            //	objAssociationEntitiesReport.lstReportData = new BLLayer().GetAssociationReportSummary(objAssociationEntitiesReport.objReportFilters).OrderBy(m => m.entity_name).ToList();
            //}
            objAssociationEntitiesReport.objReportFilters.SelectedLayerIds = selectedLayers;
            Session["AssociationReportFilter"] = objAssociationEntitiesReport.objReportFilters;

            BindAssociationReportDropdown(ref objAssociationEntitiesReport);
            Session["EntityAssociationSummaryData"] = objAssociationEntitiesReport;
            DateTime endTime = DateTime.Now;
            if (objAssociationEntitiesReport.objReportFilters.purpose == "" || objAssociationEntitiesReport.objReportFilters.purpose == null)
            {
                return PartialView("_EntityAssociationReport", objAssociationEntitiesReport);

            }
            else
            {
                //objAssociationEntitiesReport.lstReportData = new BLLayer().GetAssociationReportSummary(objAssociationEntitiesReport.objReportFilters).OrderBy(m => m.entity_name).ToList();
                //Session["EntityAssociationSummaryData"] = objAssociationEntitiesReport;
                DownloadAssociationEntityReport(objAssociationEntitiesReport.objReportFilters.purpose, objAssociationEntitiesReport.objReportFilters.SelectedLayerIds, 0, 0, 0);
                objAssociationEntitiesReport.popupmessage = "Request is processing in background.Please check the export report log page.";
                return PartialView("_EntityAssociationReport", objAssociationEntitiesReport);

            }

        }

        public void BindAssociationReportDropdown(ref AssociationEntitiesReport objAssociationEntitiesReport)
        {
            var userdetails = (User)Session["userDetail"];
            var moduleAbbr = "EXASSRPT";
            objAssociationEntitiesReport.lstfiletypes = blExportData.getfiletype(moduleAbbr);
            //Bind Layers..
            objAssociationEntitiesReport.lstLayers = new BLLayer().GetAssociationReportLayers(userdetails.role_id, "ENTITY");
            objAssociationEntitiesReport.lstRouteInfo = new BLLayer().getRouteInfo("0");
            //Bind Regions..
            objAssociationEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objAssociationEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objAssociationEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objAssociationEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }

            List<int> parentUser = new List<int>();
            parentUser.Add(1);
            if (userdetails.role_id == 1 || userdetails.is_all_provience_assigned)
                objAssociationEntitiesReport.lstParentUsers = new BLUser().GetUsersListByMGRIds(parentUser).OrderBy(x => x.user_name).ToList();//new BLUser().GetUsersListByMGRIds(parentUser).Where(x => x.user_id == Convert.ToInt32(Session["user_id"])).OrderBy(x => x.user_name).ToList();
            else
            {
                objAssociationEntitiesReport.lstParentUsers = new List<Models.User>();
                objAssociationEntitiesReport.lstParentUsers.Add(userdetails);// new BLUser().GetUserDetailByID(Convert.ToInt32(Session["user_id"])));// new BLUser().GetUsersListByMGRIds(parentUser).Where(x=> x.user_id == Convert.ToInt32(Session["user_id"])).OrderBy(x => x.user_name).ToList();
            }

            if (objAssociationEntitiesReport.objReportFilters.SelectedParentUser != null)
            {
                if (userdetails.role_id == 1 || userdetails.is_all_provience_assigned)
                {
                    objAssociationEntitiesReport.lstUsers = new BLUser().GetUsersListByMGRIds(objAssociationEntitiesReport.objReportFilters.SelectedParentUser).OrderBy(x => x.user_name).ToList();
                }
                else
                {
                    var parentUser_ids = string.Join(",", objAssociationEntitiesReport.objReportFilters.SelectedParentUser.Select(n => n.ToString()).ToArray());
                    objAssociationEntitiesReport.lstUsers = new BLUser().GetUserReportDetailsList(parentUser_ids).ToList();
                }
            }
            //for project code,planning code,workordercode & purpose code
            objAssociationEntitiesReport.lstBindProjectCode = new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrWhiteSpace(objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues) ? "P" : objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "PLANNED" ? "P" : objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "AS BUILT" ? "A" : objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "DORMANT" ? "D" : "P");
            if (objAssociationEntitiesReport.objReportFilters.SelectedProjectId != null)
                objAssociationEntitiesReport.lstBindPlanningCode = new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(objAssociationEntitiesReport.objReportFilters.SelectedProjectId);
            if (objAssociationEntitiesReport.objReportFilters.SelectedPlanningId != null)
                objAssociationEntitiesReport.lstBindWorkorderCode = new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(objAssociationEntitiesReport.objReportFilters.SelectedPlanningId);
            if (objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderId != null)
                objAssociationEntitiesReport.lstBindPurposeCode = new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderId);
            //for duration based on 
            objAssociationEntitiesReport.lstDurationBasedOn = new BLMisc().GetAssociationDropDownList("", DropDownType.Association_Report.ToString());

            //objExportEntitiesReport.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objAssociationEntitiesReport.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objAssociationEntitiesReport.lstNetworkStatus = new BLMisc().GetDropDownList("", DropDownType.ddlNetworkStatus.ToString());
            objAssociationEntitiesReport.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
        }

        public ActionResult EntityAssociationReportLog(AssociationReportLogVM ObjAssociationReportLogVM, int page = 0, string sort = "", string sortdir = "")
        {

            //System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => new Worker().StartProcessing(cancellationToken));
            var usrDetail = (User)Session["userDetail"];
            if (sort != "" || page != 0)
            {
                ObjAssociationReportLogVM.objGridAttributes = new CommonGridAttributes(); //(CommonGridAttributes)Session["printExportLog"];
            }
            var timeInteval = ApplicationSettings.PrintLogTimeInterval;
            ObjAssociationReportLogVM.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            ObjAssociationReportLogVM.objGridAttributes.currentPage = page == 0 ? 1 : page;
            ObjAssociationReportLogVM.objGridAttributes.sort = sort;
            ObjAssociationReportLogVM.objGridAttributes.orderBy = sortdir;
            ObjAssociationReportLogVM.ExportLog = new BLAssociationReportLog().GetAssociationAssociationLogList(ObjAssociationReportLogVM.objGridAttributes, usrDetail.user_id, timeInteval);
            ObjAssociationReportLogVM.objGridAttributes.totalRecord = ObjAssociationReportLogVM.ExportLog != null && ObjAssociationReportLogVM.ExportLog.Count > 0 ? ObjAssociationReportLogVM.ExportLog[0].totalRecords : 0;
            Session["EntityAssociationLog"] = ObjAssociationReportLogVM.objGridAttributes;
            return PartialView("_EntityAssociationReportLog", ObjAssociationReportLogVM);
        }

        [HttpPost]
        public JsonResult DownloadAssociationEntityReport(string fileType, string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            PageMessage objMsg = new PageMessage();
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
                ////create ftp folder if not exist

                string ftpFilePath = ApplicationSettings.FTPAttachment;
                string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                string[] ftplogReportDirectory = new string[] { ftpFolder.Replace("/", "") };
                CreateNestedDirectoryOnFTP(ftpFilePath, ftpUserName, ftpPwd, ftplogReportDirectory);

                if (fileType.ToUpper() == "EXCEL")
                {
                    //DownloadEntityReportNewIntoExcel(entityids);
                    DownloadAssociationEntityReportIntoExcel(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "PDF")
                {
                    //DownloadFileFromFTP(entityids);
                    DownloadAssociartionEntityReportIntoPDF(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "ALLEXCEL")
                {
                    DownloadAssociationEntityReportIntoExcelAll(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "XML")
                {
                    DownloadAssociationEntitySummaryIntoXML(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "KML")
                {
                    DownloadAssociationEntitySummaryIntoAllKML(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "DXF")
                {
                    DownloadAssociationEntitySummaryIntoDXF(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "ALLSHAPE")
                {
                    DownloadAssociationEntityReportIntoShape(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "ALLTXT")
                {
                    DownloadAssociationEntityReportIntoTXT(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "ALLCSV")
                {
                    DownloadAssociationEntityReportIntoCSV(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }


            }
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Request is processing in background.Please check the export report log page.";
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }

        public void DownloadAssociationEntityReportIntoExcel(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["EntityAssociationSummaryData"] != null)
            {

                try
                {
                    var userdetails = (User)Session["userDetail"];
                    AssociationEntitiesReport objAssociationEntitiesReport = new AssociationEntitiesReport();

                    objAssociationEntitiesReport.objReportFilters = (AssociationReportFilter)Session["AssociationReportFilter"];// for filter
                    List<int> SelectedLayerId = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (!objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                        objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                    if (!objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                        objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                    if (!objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                        objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");
                    DataTable dtFilter = GetExportReportFilter(objAssociationEntitiesReport.objReportFilters);

                    string fileName = "AssociationSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");

                    objAssociationEntitiesReport = (AssociationEntitiesReport)Session["EntityAssociationSummaryData"];
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        List<EntitySummaryReport> lstRprtData = objAssociationEntitiesReport.lstReportData;
                        if (objAssociationEntitiesReport.objReportFilters.SelectedLayerId != null)
                            objAssociationEntitiesReport.lstReportData = objAssociationEntitiesReport.lstReportData.Where(x => objAssociationEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.ListToDataTable(objAssociationEntitiesReport.lstReportData);
                        dtReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                        objAssociationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                        objAssociationEntitiesReport.lstReportData = lstRprtData;
                        DataSet ds = new DataSet();
                        ds.Tables.Add(dtFilter);

                        int TotalEntityReport = 0;
                        AssociationReportLog associationReportLog = new AssociationReportLog();
                        associationReportLog.user_id = userdetails.user_id;
                        associationReportLog.export_started_on = DateTime.Now;
                        associationReportLog.file_name = fileName;
                        associationReportLog.file_type = "Excel";
                        associationReportLog.file_extension = ".xlsx";
                        associationReportLog.status = "InProgress";
                        associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        associationReportLog.planned = totalPlannedCount;
                        associationReportLog.asbuilt = totalAsBuiltCount;
                        associationReportLog.dormant = totalDormantCount;
                        associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                        try
                        {

                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (!ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns.Remove("DORMANT_COUNT");
                                }
                                dtReport.Columns.Remove("entity_id");
                                dtReport.Columns.Remove("entity_name");
                                dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                dtReport.Columns["planned_count"].ColumnName = "Planned";
                                dtReport.Columns["as_built_count"].ColumnName = "As-Built";
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns["dormant_count"].ColumnName = "Dormant";
                                }

                                string[] networkstatusvalues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                if (networkstatusvalues.Length < 3)
                                {
                                    if (!networkstatusvalues.Contains("P"))
                                    {
                                        dtReport.Columns.Remove("PLANNED");
                                    }
                                    if (!networkstatusvalues.Contains("A"))
                                    {
                                        dtReport.Columns.Remove("AS-BUILT");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (!networkstatusvalues.Contains("D"))
                                        {
                                            dtReport.Columns.Remove("DORMANT");
                                        }
                                    }
                                }
                                DataRow row = dtReport.NewRow();
                                row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    row["Planned"] = dtReport.Compute("Sum(Planned)", "");
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    row["As-Built"] = dtReport.Compute("Sum([As-Built])", "");
                                }
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        row["Dormant"] = dtReport.Compute("Sum(Dormant)", "");
                                    }
                                }
                                dtReport.Rows.Add(row);
                                ds.Tables.Add(dtReport);
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    totalPlannedCount = Convert.ToInt32(row["Planned"]);
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    totalAsBuiltCount = Convert.ToInt32(row["As-Built"]);
                                }
                                //totalDormantCount = Convert.ToInt32(row["Dormant"]);
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        totalDormantCount = Convert.ToInt32(row["Dormant"]);
                                    }
                                }
                            }

                            string tempFileName = fileName + associationReportLog.file_extension;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);

                            associationReportLog.planned = totalPlannedCount;
                            associationReportLog.asbuilt = totalAsBuiltCount;
                            associationReportLog.dormant = totalDormantCount;
                            associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Success";
                            associationReportLog.file_location = ftpFolder + tempFileName;
                            //Thread.Sleep(10000);
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                        }
                        catch (Exception ex)
                        {
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Error occurred while processing request";
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadAssociationEntityReportIntoExcel()", "Report", ex);
                        }
                    });

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public ActionResult AssociationSummaryView(AssociationEntitiesSummaryView objAssociationEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {
            if (Session["AssociationReportFilter"] != null)
            {
                //sort = String.IsNullOrEmpty(sort) ? "" : sort.Replace(" ", "_");
                AssociationReportFilter objAssociationReportFilter = new AssociationReportFilter();

                objAssociationReportFilter = (AssociationReportFilter)Session["AssociationReportFilter"];

                objAssociationEntitiesReport.objReportFilters.SelectedRegionId = objAssociationReportFilter.SelectedRegionId;
                objAssociationEntitiesReport.objReportFilters.SelectedProvinceId = objAssociationReportFilter.SelectedProvinceId;
                objAssociationEntitiesReport.objReportFilters.SelectedParentUser = objAssociationReportFilter.SelectedParentUser;
                objAssociationEntitiesReport.objReportFilters.SelectedUserId = objAssociationReportFilter.SelectedUserId;
                objAssociationEntitiesReport.objReportFilters.SelectedProjectId = objAssociationReportFilter.SelectedProjectId;
                objAssociationEntitiesReport.objReportFilters.SelectedPlanningId = objAssociationReportFilter.SelectedPlanningId;
                objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderId = objAssociationReportFilter.SelectedWorkOrderId;
                objAssociationEntitiesReport.objReportFilters.SelectedPurposeId = objAssociationReportFilter.SelectedPurposeId;
                objAssociationEntitiesReport.objReportFilters.radius = objAssociationReportFilter.radius;

                objAssociationEntitiesReport.objReportFilters.SelectedRegionIds = objAssociationReportFilter.SelectedRegionIds;
                objAssociationEntitiesReport.objReportFilters.SelectedProvinceIds = objAssociationReportFilter.SelectedProvinceIds;
                objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationReportFilter.SelectedNetworkStatues;
                objAssociationEntitiesReport.objReportFilters.SelectedParentUsers = objAssociationReportFilter.SelectedParentUsers;
                objAssociationEntitiesReport.objReportFilters.SelectedUserIds = objAssociationReportFilter.SelectedUserIds;
                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = objAssociationReportFilter.SelectedLayerId;
                objAssociationEntitiesReport.objReportFilters.SelectedProjectIds = objAssociationReportFilter.SelectedProjectIds;
                objAssociationEntitiesReport.objReportFilters.SelectedPlanningIds = objAssociationReportFilter.SelectedPlanningIds;
                objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objAssociationReportFilter.SelectedWorkOrderIds;
                objAssociationEntitiesReport.objReportFilters.SelectedPurposeIds = objAssociationReportFilter.SelectedPurposeIds;
                objAssociationEntitiesReport.objReportFilters.durationbasedon = objAssociationReportFilter.durationbasedon;
                objAssociationEntitiesReport.objReportFilters.fromDate = objAssociationReportFilter.fromDate;
                objAssociationEntitiesReport.objReportFilters.toDate = objAssociationReportFilter.toDate;
                objAssociationEntitiesReport.objReportFilters.geom = objAssociationReportFilter.geom;
                objAssociationEntitiesReport.objReportFilters.pageSize = 10;
                objAssociationEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
                objAssociationEntitiesReport.objReportFilters.sort = sort;
                objAssociationEntitiesReport.objReportFilters.sortdir = sortdir;
                objAssociationEntitiesReport.objReportFilters.userId = objAssociationReportFilter.userId;
                objAssociationEntitiesReport.objReportFilters.roleId = objAssociationReportFilter.roleId;
                objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType = objAssociationReportFilter.SelectedOwnerShipType;
                objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objAssociationReportFilter.SelectedThirdPartyVendorIds;
                objAssociationEntitiesReport.objReportFilters.selected_route_ids = objAssociationReportFilter.selected_route_ids;
                BindAssociationReportDropdownSummaryView(ref objAssociationEntitiesReport);

                //rt
                var userdetails = (User)Session["userDetail"];
                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").Where(x => x.layer_name == objAssociationEntitiesReport.objReportFilters.layerName).Select(x => x.layer_id).ToList();
                objAssociationEntitiesReport.objReportFilters.lstAdvanceFilters = objAssociationEntitiesReport.lstAdvanceFilters;
                if (!string.IsNullOrWhiteSpace(objAssociationEntitiesReport.objReportFilters.layerName))
                {
                    objAssociationEntitiesReport.objReportFilters.advancefilter = getAdvanceFilter(objAssociationEntitiesReport.lstAdvanceFilters);
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetAssociationReportSummaryView(objAssociationEntitiesReport.objReportFilters);

                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE" };
                    objAssociationEntitiesReport.webColumns = BLConvertMLanguage.GetEntityWiseColumns(objAssociationEntitiesReport.objReportFilters.SelectedLayerId[0], objAssociationEntitiesReport.objReportFilters.layerName, "REPORT", arrIgnoreColumns, userdetails.role_id, userdetails.user_id);
                    foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                    {
                        var obj = (IDictionary<string, object>)new ExpandoObject();

                        foreach (var col in dic)
                        {
                            //if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                            //{
                            //    obj.Add(col.Key, col.Value);
                            //}
                            obj.Add(col.Key, col.Value);
                        }
                        objAssociationEntitiesReport.lstReportData.Add(obj);
                    }
                    //objExportEntitiesReport.lstReportData = BLConvertMLanguage.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                    objAssociationEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
                }
            }
            // if there is no filter and add one row by default...
            if (objAssociationEntitiesReport.lstAdvanceFilters.Count == 0)
            {
                objAssociationEntitiesReport.lstAdvanceFilters.Add(new ReportAdvanceFilter());
            }
            Session["EntityAssociationSummaryView"] = objAssociationEntitiesReport.objReportFilters;
            return PartialView("_EntityAssociationSummaryView", objAssociationEntitiesReport);
        }

        public void BindAssociationReportDropdownSummaryView(ref AssociationEntitiesSummaryView objAssociationEntitiesReport)
        {
            //rt
            var userdetails = (User)Session["userDetail"];
            //Bind Layers..
            objAssociationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
            var selectedlayerids = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
            if (selectedlayerids != null)
            {
                if (selectedlayerids.Count > 0)
                    objAssociationEntitiesReport.lstLayers = objAssociationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
            }
            //objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.objReportFilters.layerName == null ? objExportEntitiesReport.lstLayers[0].layer_name : objExportEntitiesReport.objReportFilters.layerName;
            if (!string.IsNullOrWhiteSpace(objAssociationEntitiesReport.objReportFilters.layerName))
            {
                objAssociationEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(objAssociationEntitiesReport.objReportFilters.layerName);
            }
        }

        public void DownloadAssociartionEntityReportIntoPDF(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["EntityAssociationSummaryData"] != null)
                try
                {
                    var userdetails = (User)Session["userDetail"];
                    AssociationEntitiesReport objAssociationEntitiesReport = new AssociationEntitiesReport();

                    objAssociationEntitiesReport.objReportFilters = (AssociationReportFilter)Session["AssociationReportFilter"];
                    List<int> SelectedLayerId = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (!objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                        objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                    if (!objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                        objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                    if (!objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                        objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");
                    DataTable dtFilter = GetAssociationReportFilter(objAssociationEntitiesReport.objReportFilters);// GetAssociationReportFilter

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    objAssociationEntitiesReport = (AssociationEntitiesReport)Session["EntityAssociationSummaryData"];

                    BaseFont baseFont = PDFHelper.GetFont();
                    string fileName = "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");

                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        //int TotalEntityReport = 0;


                        AssociationReportLog associationReportLog = new AssociationReportLog();
                        associationReportLog.user_id = userdetails.user_id;
                        associationReportLog.export_started_on = DateTime.Now;
                        associationReportLog.file_name = fileName;
                        associationReportLog.file_type = "PDF";
                        associationReportLog.file_extension = ".pdf";
                        associationReportLog.status = "InProgress";
                        associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        associationReportLog.planned = totalPlannedCount;
                        associationReportLog.asbuilt = totalAsBuiltCount;
                        associationReportLog.dormant = totalDormantCount;
                        associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                        try
                        {
                            List<EntitySummaryReport> lstRprtData = objAssociationEntitiesReport.lstReportData;
                            if (objAssociationEntitiesReport.objReportFilters.SelectedLayerId != null)
                                objAssociationEntitiesReport.lstReportData = objAssociationEntitiesReport.lstReportData.Where(x => objAssociationEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();

                            DataTable dtReport = new DataTable();
                            dtReport = MiscHelper.ListToDataTable(objAssociationEntitiesReport.lstReportData);//,true)ApplicationSettings.numberFormatType,null
                            dtReport.TableName = "EntitySummaryDetail";
                            objAssociationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objAssociationEntitiesReport.lstReportData = lstRprtData;
                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (!ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns.Remove("DORMANT_COUNT");
                                }
                                dtReport.Columns.Remove("entity_id");
                                dtReport.Columns.Remove("entity_name");
                                dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                dtReport.Columns["planned_count"].ColumnName = "Planned";
                                dtReport.Columns["as_built_count"].ColumnName = "As-Built";
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns["dormant_count"].ColumnName = "Dormant";
                                }
                                string[] networkstatusvalues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                if (networkstatusvalues.Length < 3)
                                {
                                    if (!networkstatusvalues.Contains("P"))
                                    {
                                        dtReport.Columns.Remove("PLANNED");
                                    }
                                    if (!networkstatusvalues.Contains("A"))
                                    {
                                        dtReport.Columns.Remove("AS-BUILT");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (!networkstatusvalues.Contains("D"))
                                        {
                                            dtReport.Columns.Remove("DORMANT");
                                        }
                                    }
                                }
                                DataRow row = dtReport.NewRow();
                                row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = Resources.Resources.SI_OSP_GBL_GBL_GBL_041;
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    row["Planned"] = dtReport.Compute("Sum(Planned)", "");
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    row["As-Built"] = dtReport.Compute("Sum([As-Built])", "");
                                }
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        row["Dormant"] = dtReport.Compute("Sum(Dormant)", "");
                                    }
                                }
                                dtReport.Rows.Add(row);
                                ds.Tables.Add(dtReport);
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    totalPlannedCount = Convert.ToInt32(row["Planned"]);
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    totalAsBuiltCount = Convert.ToInt32(row["As-Built"]);
                                }
                                // totalDormantCount = Convert.ToInt32(row["Dormant"]);

                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        totalDormantCount = Convert.ToInt32(row["Dormant"]);
                                    }
                                }
                                //commented by pk
                                // dtReport = Utility.CommonUtility.GetFormattedDataTable(dtReport, ApplicationSettings.numberFormatType);

                            }

                            string tempFileName = fileName + associationReportLog.file_extension;
                            //string ftpFolder = "ExportReportLog/";
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            //ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                            GenerateToPDFNew(ds, tempFileName, Resources.Resources.SI_OSP_GBL_NET_RPT_127, ftpFilePath, ftpUserName, ftpPwd, baseFont);
                            associationReportLog.planned = totalPlannedCount;
                            associationReportLog.asbuilt = totalAsBuiltCount;
                            associationReportLog.dormant = totalDormantCount;
                            associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Success";
                            associationReportLog.file_location = ftpFolder + tempFileName;
                            //Thread.Sleep(10000);
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        }
                        catch (Exception ex)
                        {
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Error occurred while processing request";
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadAssociationEntityReportIntoPDF()", "Report", ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
        }

        public DataTable GetAssociationReportFilter(object obj)
        {
            var userdetails = (User)Session["userDetail"];
            var isAttr = ((List<string>)Session["ApplicableModuleList"]);
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            DataTable dt = new DataTable(Resources.Resources.SI_GBL_GBL_NET_FRM_165);
            dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_166);
            dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_167);
            DataRow dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_GBL_GBL_NET_FRM_098; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_065; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_066; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_063; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_068; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_069; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_GBL_GBL_GBL_GBL_147; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_FRM_030; dt.Rows.Add(dr); dr = dt.NewRow();
            if (isAttr.Contains("PROJ"))
            {
                dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_074; dt.Rows.Add(dr); dr = dt.NewRow();
                dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_076; dt.Rows.Add(dr); dr = dt.NewRow();
                dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_010; dt.Rows.Add(dr); dr = dt.NewRow();
                dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_011; dt.Rows.Add(dr); dr = dt.NewRow();
            }
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_071; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_072; dt.Rows.Add(dr); dr = dt.NewRow();
            if (obj.GetType().GetProperty("advancefilter") != null)
            {
                if (!String.IsNullOrEmpty(obj.GetType().GetProperty("advancefilter").GetValue(obj, null).ToString()))
                {
                    int rowCount = dt.Rows.Count;
                    dr["Filter Type"] = "Add-on Filter"; dt.Rows.Add(dr); dr = dt.NewRow();
                    List<ReportAdvanceFilter> advnanceFilter = (List<ReportAdvanceFilter>)obj.GetType().GetProperty("lstAdvanceFilters").GetValue(obj, null);
                    for (int i = 0; i < advnanceFilter.Count; i++)
                    {
                        dt.Rows[rowCount + i][1] = textInfo.ToTitleCase(advnanceFilter[i].searchBy.ToString().ToLower().Replace("_", " ")) + " " + advnanceFilter[i].searchType + " '" + advnanceFilter[i].searchText + "'";
                        if ((advnanceFilter.Count - 1) != i)
                        {
                            dt.Rows.Add(dr); dr = dt.NewRow();
                        }
                    }
                }
            }

            List<int> regionIds = (List<int>)obj.GetType().GetProperty("SelectedRegionId").GetValue(obj, null);
            var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());

            List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
            var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = string.Join(",", regionIds), userId = Convert.ToInt32(Session["user_id"]) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());

            string networkStatus = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedNetworkStatues").GetValue(obj, null).ToString().Replace("AS BUILT", "AS-BUILT").ToLower()).Replace("'", "");

            string ownershipType = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedOwnerShipType").GetValue(obj, null).ToString()).Replace("'", "");
            ownershipType = string.IsNullOrEmpty(ownershipType) ? "All" : ownershipType;

            List<int> thirdPartyVendorId = (List<int>)obj.GetType().GetProperty("SelectedThirdPartyVendorId").GetValue(obj, null);
            var thirdPartyVendorName = thirdPartyVendorId == null ? "All" : string.Join(",", BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList().Where(x => thirdPartyVendorId.Contains(Convert.ToInt32(x.key))).Select(x => x.value).ToList());

            List<int> parentUser = new List<int>();
            List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
            var parentUserName = string.Empty;
            if (userdetails.role_id == 1)
            {
                parentUser.Add(1);
                parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
            }
            else
            {
                parentUserName = parentUserIds == null ? "All" : new BLUser().GetUserDetailByID(userdetails.user_id).user_name;
            }

            List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
            var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

            //rt
            //var userdetails = (User)Session["userDetail"];
            List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
            var layerName = layerIds == null ? "All" : string.Join(",", new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").Where(x => layerIds.Contains(x.layer_id)).Select(x => x.layer_title).ToList());
            var projectCodeName = "";
            var planningCodeName = "";
            var workOrderCodeName = "";
            var purposeCodeName = "";
            if (isAttr.Contains("PROJ"))
            {
                List<int> projectCodeIds = (List<int>)obj.GetType().GetProperty("SelectedProjectId").GetValue(obj, null);
                projectCodeName = projectCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrEmpty(networkStatus) ? "P" : networkStatus.ToUpper() == "PLANNED" ? "P" : networkStatus.ToUpper() == "AS BUILT" ? "A" : networkStatus.ToUpper() == "DORMANT" ? "D" : "P").Where(x => projectCodeIds.Contains(x.system_id)).Select(x => x.project_code).ToList());

                List<int> planningCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPlanningId").GetValue(obj, null);
                planningCodeName = planningCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(projectCodeIds).Where(x => planningCodeIds.Contains(x.system_id)).Select(x => x.planning_code).ToList());

                List<int> workOrderCodeIds = (List<int>)obj.GetType().GetProperty("SelectedWorkOrderId").GetValue(obj, null);
                workOrderCodeName = workOrderCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(planningCodeIds).Where(x => workOrderCodeIds.Contains(x.system_id)).Select(x => x.workorder_code).ToList());

                List<int> purposeCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPurposeId").GetValue(obj, null);
                purposeCodeName = purposeCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(workOrderCodeIds).Where(x => purposeCodeIds.Contains(x.system_id)).Select(x => x.purpose_code).ToList());
            }
            string duration = "";
            string durationBasedOn = obj.GetType().GetProperty("durationbasedon").GetValue(obj, null).ToString().Replace("_", " ");
            if (obj.GetType().GetProperty("fromDate").GetValue(obj, null) != null && obj.GetType().GetProperty("toDate").GetValue(obj, null) != null)
            {
                duration = obj.GetType().GetProperty("fromDate").GetValue(obj, null).ToString() + " To " + obj.GetType().GetProperty("toDate").GetValue(obj, null).ToString();
            }
            else
            {
                duration = "All";
            }
            if (!networkStatus.Contains("Planned"))
                networkStatus = networkStatus.Replace("P", "Planned");
            if (!networkStatus.Contains("As-Built"))
                networkStatus = networkStatus.Replace("A", "As-Built");
            if (!networkStatus.Contains("Dormant"))
                networkStatus = networkStatus.Replace("D", "Dormant");

            dt.Rows[0][1] = layerName;
            dt.Rows[1][1] = regionName;
            dt.Rows[2][1] = provinceName;
            dt.Rows[3][1] = String.IsNullOrEmpty(networkStatus) ? "All" : networkStatus;
            dt.Rows[4][1] = parentUserName;
            dt.Rows[5][1] = userName;
            //dt.Rows[5][1] = layerName;

            dt.Rows[6][1] = ownershipType;
            dt.Rows[7][1] = thirdPartyVendorName;

            if (isAttr.Contains("PROJ"))
            {
                dt.Rows[8][1] = projectCodeName;
                dt.Rows[9][1] = planningCodeName;
                dt.Rows[10][1] = workOrderCodeName;
                dt.Rows[11][1] = purposeCodeName;
                dt.Rows[12][1] = durationBasedOn;
                dt.Rows[13][1] = duration;
            }
            else
            {
                dt.Rows[8][1] = durationBasedOn;
                dt.Rows[9][1] = duration;
            }
            return dt;
        }

        [System.Web.Services.WebMethod(true)]
        public void DownloadAssociationEntityReportIntoExcelAll(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {

            if (Session["AssociationReportFilter"] != null)
            {
                try
                {
                    AssociationEntitiesReport entityAssociationSummaryData = new AssociationEntitiesReport();
                    entityAssociationSummaryData = (AssociationEntitiesReport)Session["EntityAssociationSummaryData"];
                    AssociationEntitiesSummaryView objAssociationEntitiesReport = new AssociationEntitiesSummaryView();

                    AssociationReportFilter objAssociationReportFilter = new AssociationReportFilter();

                    objAssociationReportFilter = (AssociationReportFilter)Session["AssociationReportFilter"];

                    objAssociationEntitiesReport.objReportFilters.connectionString = objAssociationReportFilter.connectionString;
                    objAssociationEntitiesReport.objReportFilters.SelectedRegionIds = objAssociationReportFilter.SelectedRegionIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedProvinceIds = objAssociationReportFilter.SelectedProvinceIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationReportFilter.SelectedNetworkStatues;
                    objAssociationEntitiesReport.objReportFilters.SelectedParentUsers = objAssociationReportFilter.SelectedParentUsers;
                    objAssociationEntitiesReport.objReportFilters.SelectedUserIds = objAssociationReportFilter.SelectedUserIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = objAssociationReportFilter.SelectedLayerId;
                    objAssociationEntitiesReport.objReportFilters.SelectedProjectIds = objAssociationReportFilter.SelectedProjectIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedPlanningIds = objAssociationReportFilter.SelectedPlanningIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objAssociationReportFilter.SelectedWorkOrderIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedPurposeIds = objAssociationReportFilter.SelectedPurposeIds;
                    objAssociationEntitiesReport.objReportFilters.durationbasedon = objAssociationReportFilter.durationbasedon;
                    objAssociationEntitiesReport.objReportFilters.fromDate = objAssociationReportFilter.fromDate;
                    objAssociationEntitiesReport.objReportFilters.toDate = objAssociationReportFilter.toDate;
                    objAssociationEntitiesReport.objReportFilters.geom = objAssociationReportFilter.geom;
                    objAssociationEntitiesReport.objReportFilters.userId = objAssociationReportFilter.userId;
                    objAssociationEntitiesReport.objReportFilters.roleId = objAssociationReportFilter.roleId;
                    objAssociationEntitiesReport.objReportFilters.radius = objAssociationReportFilter.radius;
                    objAssociationEntitiesReport.objReportFilters.selected_route_ids = objAssociationReportFilter.selected_route_ids;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType = objAssociationReportFilter.SelectedOwnerShipType;
                    objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objAssociationReportFilter.SelectedThirdPartyVendorIds;

                    objAssociationEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objAssociationReportFilter.SelectedLayerId;

                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    objAssociationReportFilter.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationReportFilter.SelectedLayerId;

                    DataTable dtFilter = GetAssociationReportFilter(objAssociationReportFilter);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objAssociationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objAssociationEntitiesReport.lstLayers = objAssociationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    else
                    {
                        objAssociationEntitiesReport.lstLayers = objAssociationEntitiesReport.lstLayers;
                    }

                    string parentFolder = $"AssociationReport_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                    string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                    string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                    string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));

                    // Create Parent folder for temparary basic on server 
                    if (Directory.Exists(directoryPath).Equals(false))
                        Directory.CreateDirectory(directoryPath);
                    string fileName = $"{dtFilter.TableName}";
                    string tempFileName = $"{directoryPath}/{dtFilter.TableName}.xlsx";
                    ExportDataNew(dtFilter, fileName, tempFileName);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        AssociationReportLog associationReportLog = new AssociationReportLog();
                        associationReportLog.user_id = userdetails.user_id;
                        associationReportLog.export_started_on = DateTime.Now;
                        associationReportLog.file_name = parentFolder;
                        associationReportLog.file_type = "ALLEXCEL";
                        associationReportLog.file_extension = ".zip";
                        associationReportLog.status = "InProgress";
                        associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        associationReportLog.planned = totalPlannedCount;
                        associationReportLog.asbuilt = totalAsBuiltCount;
                        associationReportLog.dormant = totalDormantCount;
                        associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        dtFilter = null;
                        try
                        {
                            var tasks = new List<Task>();
                            foreach (var layer in objAssociationEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        objAssociationEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objAssociationEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                        EntitySummaryReport recordCount = entityAssociationSummaryData.lstReportData.Where(x => x.entity_name.ToUpper() == layer.layer_name.ToUpper()).FirstOrDefault();
                                        int total_entity_count = 0;
                                        if (recordCount != null)
                                            total_entity_count = recordCount.planned_count + recordCount.as_built_count + recordCount.dormant_count;
                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                        if (total_entity_count > ApplicationSettings.ExcelReportLimitCount)
                                        {
                                            lstExportEntitiesDetail = new BLLayer().GetAssociationReportSummaryViewCSV(objAssociationEntitiesReport.objReportFilters, layer.layer_name);

                                        }
                                        else
                                        {
                                            lstExportEntitiesDetail = new BLLayer().GetAssociationReportSummaryView(objAssociationEntitiesReport.objReportFilters, layer.layer_name);
                                        }
                                        // lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                                        DataTable dtReport = new DataTable();
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
                                        dtReport.TableName = layer.layer_title;
                                        //dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName;
                                        if (dtReport != null && dtReport.Rows.Count > 0)
                                        {
                                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                            if (dtReport.Columns.Contains("Fn Get Date")) { dtReport.Columns.Remove("Fn Get Date"); }
                                        }
                                        if (dtReport.Rows.Count > 0)
                                        {
                                            objAssociationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                            objAssociationReportFilter.SelectedLayerId = SelectedLayerIdSummary;
                                            fileName = $"{dtReport.TableName}";
                                            if (dtReport.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                            {
                                                tempFileName = $"{parentFolder}/{dtReport.TableName}.csv";
                                                StreamNewCSVInFolder(dtReport, tempFileName);
                                            }
                                            else
                                            {
                                                tempFileName = $"{directoryPath}/{dtReport.TableName}.xlsx";
                                                ExportDataNew(dtReport, fileName, tempFileName);

                                            }

                                            associationReportLog.export_ended_on = DateTime.Now;
                                            associationReportLog.status = "Success";
                                            associationReportLog.file_location = ftpFolder + parentFolder + associationReportLog.file_extension;
                                            //Thread.Sleep(10000);
                                            dtReport = null;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));
                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();

                            // get FTP details
                            string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                            string zipfilePath = directoryPath + ".zip";
                            string fileNameValue = parentFolder + ".zip";

                            // Below code for Convert parent folder to ZIP and delete parent folder after ZIP on server
                            using (var zip = new ZipFile())
                            {
                                //zip.UseZip64WhenSaving = Zip64Option.Always;
                                //zip.CompressionMethod = CompressionMethod.BZip2;
                                zip.AddDirectory(directoryPath);
                                zip.Save(zipfilePath);
                            }
                            if (System.IO.File.Exists(zipfilePath))
                            {
                                string fileZipName = Path.GetFileName(zipfilePath);
                                Directory.Delete(directoryPath, true);
                            }
                            FileInfo file = new FileInfo(zipfilePath);

                            // ZIP File upload on FTP server
                            CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);
                            // Deleted ZIP on Server after uploaded on FTP server
                            System.IO.File.Delete(zipfilePath);
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);


                        }
                        catch (Exception ex)
                        {
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Error occurred while processing request";
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportIntoExcelAll()", "Report", ex);
                            // delete folder after error generate
                            if (Directory.Exists(directoryPath).Equals(true))
                                Directory.Delete(directoryPath, true);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public void DownloadAssociationEntitySummaryIntoXML(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["AssociationReportFilter"] != null)
            {

                DataSet ds = new DataSet();
                AssociationEntitiesSummaryView objAssociationEntitiesReport = new AssociationEntitiesSummaryView();
                AssociationReportFilter objAssociationReportFilter = new AssociationReportFilter();
                objAssociationReportFilter = (AssociationReportFilter)Session["AssociationReportFilter"];

                objAssociationEntitiesReport.objReportFilters.connectionString = objAssociationReportFilter.connectionString;
                objAssociationEntitiesReport.objReportFilters.SelectedRegionIds = objAssociationReportFilter.SelectedRegionIds;
                objAssociationEntitiesReport.objReportFilters.SelectedProvinceIds = objAssociationReportFilter.SelectedProvinceIds;
                objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationReportFilter.SelectedNetworkStatues;
                objAssociationEntitiesReport.objReportFilters.SelectedParentUsers = objAssociationReportFilter.SelectedParentUsers;
                objAssociationEntitiesReport.objReportFilters.SelectedUserIds = objAssociationReportFilter.SelectedUserIds;
                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = objAssociationReportFilter.SelectedLayerId;
                objAssociationEntitiesReport.objReportFilters.SelectedProjectIds = objAssociationReportFilter.SelectedProjectIds;
                objAssociationEntitiesReport.objReportFilters.SelectedPlanningIds = objAssociationReportFilter.SelectedPlanningIds;
                objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objAssociationReportFilter.SelectedWorkOrderIds;
                objAssociationEntitiesReport.objReportFilters.SelectedPurposeIds = objAssociationReportFilter.SelectedPurposeIds;
                objAssociationEntitiesReport.objReportFilters.durationbasedon = objAssociationReportFilter.durationbasedon;
                objAssociationEntitiesReport.objReportFilters.fromDate = objAssociationReportFilter.fromDate;
                objAssociationEntitiesReport.objReportFilters.toDate = objAssociationReportFilter.toDate;
                objAssociationEntitiesReport.objReportFilters.geom = objAssociationReportFilter.geom;
                objAssociationEntitiesReport.objReportFilters.currentPage = 0;
                objAssociationEntitiesReport.objReportFilters.fileType = "KML";
                objAssociationEntitiesReport.objReportFilters.userId = objAssociationReportFilter.userId;
                objAssociationEntitiesReport.objReportFilters.roleId = objAssociationReportFilter.roleId;
                objAssociationEntitiesReport.objReportFilters.radius = objAssociationReportFilter.radius;
                objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType = objAssociationReportFilter.SelectedOwnerShipType;
                objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objAssociationReportFilter.SelectedThirdPartyVendorIds;

                List<int> SelectedLayerId = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objAssociationReportFilter.SelectedLayerId;

                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                objAssociationReportFilter.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationReportFilter.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objAssociationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                var selectedlayerids = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objAssociationEntitiesReport.lstLayers = objAssociationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                }
                DataTable dtFilter = GetExportReportFilter(objAssociationReportFilter);

                string fileName = (fileType == "KML" ? "ExportReport_KML_" : "ExportReport_XML_") + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");

                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    //int TotalEntityReport = 0;
                    AssociationReportLog associationReportLog = new AssociationReportLog();
                    associationReportLog.user_id = userdetails.user_id;
                    associationReportLog.export_started_on = DateTime.Now;
                    associationReportLog.file_type = fileType;
                    associationReportLog.file_extension = ".zip";
                    associationReportLog.status = "InProgress";
                    associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                    associationReportLog.planned = totalPlannedCount;
                    associationReportLog.asbuilt = totalAsBuiltCount;
                    associationReportLog.dormant = totalDormantCount;
                    associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                    associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                    try
                    {

                        for (int i = 0; i < objAssociationEntitiesReport.lstLayers.Count; i++)
                        {
                            objAssociationEntitiesReport.objReportFilters.layerName = objAssociationEntitiesReport.lstLayers[i].layer_name;
                            var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objAssociationEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                            List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetAssociationSummaryViewKML(objAssociationEntitiesReport.objReportFilters);
                            DataTable dtReport = new DataTable();
                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] {""}

                            dtReport.TableName = layerDetail.layer_title;
                            if (dtReport.Rows.Count > 0)
                                ds.Tables.Add(dtReport);
                        }

                        objAssociationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                        objAssociationReportFilter.SelectedLayerId = SelectedLayerIdSummary;


                        string tempFileName = fileName + associationReportLog.file_extension;
                        //string ftpFolder = "ExportReportLog/";
                        //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                        string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                        string tempFolderName = string.Empty;
                        if (fileType == "KML")
                        {
                            string TempkmlFileName = fileName + ".kml";
                            tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
                            KMLHelper.DatasetToKMLNew(ftpFilePath, ftpUserName, ftpPwd, ds, objAssociationEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, tempFolderName, TempkmlFileName, "", dtFilter, fileType);
                        }
                        else
                        {
                            string TempkmlFileName = fileName + ".xml";
                            tempFolderName = "XML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
                            KMLHelper.DatasetToKMLNew(ftpFilePath, ftpUserName, ftpPwd, ds, objAssociationEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, tempFolderName, TempkmlFileName, "", dtFilter, fileType);
                        }
                        associationReportLog.file_name = tempFolderName;
                        associationReportLog.file_location = ftpFolder + tempFolderName + ".zip";
                        associationReportLog.export_ended_on = DateTime.Now;
                        associationReportLog.status = "Success";
                        //Thread.Sleep(10000);
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                    }
                    catch (Exception ex)
                    {
                        associationReportLog.export_ended_on = DateTime.Now;
                        associationReportLog.status = "Error occurred while processing request";
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        ErrorLogHelper.WriteErrorLog("DownloadAssociationEntitySummaryIntoXML()", "Report", ex);
                    }
                });
            }
        }

        public void DownloadAssociationEntitySummaryIntoAllKML(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["AssociationReportFilter"] != null)
            {

                //DataSet ds = new DataSet();
                AssociationEntitiesSummaryView objAssociationEntitiesReport = new AssociationEntitiesSummaryView();
                AssociationReportFilter objAssociationReportFilter = new AssociationReportFilter();
                objAssociationReportFilter = (AssociationReportFilter)Session["AssociationReportFilter"];

                objAssociationEntitiesReport.objReportFilters.connectionString = objAssociationReportFilter.connectionString;
                objAssociationEntitiesReport.objReportFilters.SelectedRegionIds = objAssociationReportFilter.SelectedRegionIds;
                objAssociationEntitiesReport.objReportFilters.SelectedProvinceIds = objAssociationReportFilter.SelectedProvinceIds;
                objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationReportFilter.SelectedNetworkStatues;
                objAssociationEntitiesReport.objReportFilters.SelectedParentUsers = objAssociationReportFilter.SelectedParentUsers;
                objAssociationEntitiesReport.objReportFilters.SelectedUserIds = objAssociationReportFilter.SelectedUserIds;
                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = objAssociationReportFilter.SelectedLayerId;
                objAssociationEntitiesReport.objReportFilters.SelectedProjectIds = objAssociationReportFilter.SelectedProjectIds;
                objAssociationEntitiesReport.objReportFilters.SelectedPlanningIds = objAssociationReportFilter.SelectedPlanningIds;
                objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objAssociationReportFilter.SelectedWorkOrderIds;
                objAssociationEntitiesReport.objReportFilters.SelectedPurposeIds = objAssociationReportFilter.SelectedPurposeIds;
                objAssociationEntitiesReport.objReportFilters.durationbasedon = objAssociationReportFilter.durationbasedon;
                objAssociationEntitiesReport.objReportFilters.fromDate = objAssociationReportFilter.fromDate;
                objAssociationEntitiesReport.objReportFilters.toDate = objAssociationReportFilter.toDate;
                objAssociationEntitiesReport.objReportFilters.geom = objAssociationReportFilter.geom;
                objAssociationEntitiesReport.objReportFilters.currentPage = 0;
                objAssociationEntitiesReport.objReportFilters.fileType = "KML";
                objAssociationEntitiesReport.objReportFilters.userId = objAssociationReportFilter.userId;
                objAssociationEntitiesReport.objReportFilters.roleId = objAssociationReportFilter.roleId;
                objAssociationEntitiesReport.objReportFilters.radius = objAssociationReportFilter.radius;
                objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType = objAssociationReportFilter.SelectedOwnerShipType;
                objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objAssociationReportFilter.SelectedThirdPartyVendorIds;

                List<int> SelectedLayerId = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objAssociationReportFilter.SelectedLayerId;

                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                objAssociationReportFilter.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationReportFilter.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objAssociationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                var selectedlayerids = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objAssociationEntitiesReport.lstLayers = objAssociationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                }
                DataTable dtFilter = GetAssociationReportFilter(objAssociationReportFilter);
                string parentFolder = $"KML_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));

                // Create Parent folder for temparary basic on server 
                if (Directory.Exists(directoryPath).Equals(false))
                    Directory.CreateDirectory(directoryPath);

                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    //int TotalEntityReport = 0;
                    AssociationReportLog associationReportLog = new AssociationReportLog();
                    associationReportLog.user_id = userdetails.user_id;
                    associationReportLog.export_started_on = DateTime.Now;
                    associationReportLog.file_type = fileType;
                    associationReportLog.file_extension = ".zip";
                    associationReportLog.status = "InProgress";
                    associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                    associationReportLog.planned = totalPlannedCount;
                    associationReportLog.asbuilt = totalAsBuiltCount;
                    associationReportLog.dormant = totalDormantCount;
                    associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                    associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                    try
                    {

                        var tasks = new List<Task>();
                        foreach (var layer in objAssociationEntitiesReport.lstLayers)
                        {
                            tasks.Add(Task.Run(() =>
                            {
                                try
                                {

                                    objAssociationEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetAssociationSummaryViewKMLNew(objAssociationEntitiesReport.objReportFilters, layer.layer_name);
                                    DataTable dtReport = new DataTable();
                                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] {""}

                                    dtReport.TableName = layer.layer_title;

                                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                    objAssociationReportFilter.SelectedLayerId = SelectedLayerIdSummary;

                                    string fileName = layer.layer_title;

                                    string TempkmlFileName = fileName + ".kml";
                                    string finalkml = KMLHelper.GetKmlForEntityNew(dtReport, objAssociationEntitiesReport.lstLayers, dtFilter, TempkmlFileName, directoryPath);
                                    finalkml = finalkml.Replace("&", "&amp;");
                                    dtReport = null;
                                    string kmlDesFullPath = directoryPath + "\\" + TempkmlFileName;
                                    System.IO.File.WriteAllText(kmlDesFullPath, finalkml.ToString());


                                    //Thread.Sleep(10000);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }));
                        }
                        Task t = Task.WhenAll(tasks);
                        t.Wait();

                        dtFilter = null;
                        // get FTP details
                        string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                        string zipfilePath = directoryPath + ".zip";
                        string fileNameValue = parentFolder + ".zip";

                        // Below code for Convert parent folder to ZIP and delete parent folder after ZIP on server
                        using (var zip = new ZipFile())
                        {
                            //zip.UseZip64WhenSaving = Zip64Option.Always;
                            //zip.CompressionMethod = CompressionMethod.BZip2;
                            zip.AddDirectory(directoryPath);
                            zip.Save(zipfilePath);
                        }
                        if (System.IO.File.Exists(zipfilePath))
                        {
                            string fileZipName = Path.GetFileName(zipfilePath);
                            Directory.Delete(directoryPath, true);
                        }
                        FileInfo file = new FileInfo(zipfilePath);

                        //LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading start on FTP : {DateTime.Now}");
                        // ZIP File upload on FTP server
                        CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);
                        //LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading completed on FTP : {DateTime.Now}");

                        associationReportLog.file_name = parentFolder;
                        associationReportLog.file_location = ftpFolder + parentFolder + ".zip";
                        associationReportLog.export_ended_on = DateTime.Now;
                        associationReportLog.status = "Success";
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                        // Deleted ZIP on Server after uploaded on FTP server
                        System.IO.File.Delete(zipfilePath);
                    }
                    catch (Exception ex)
                    {
                        associationReportLog.export_ended_on = DateTime.Now;
                        associationReportLog.status = "Error occurred while processing request";
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        ErrorLogHelper.WriteErrorLog("DownloadAssociationEntitySummaryIntoAllKML()", "Report", ex);
                        // delete folder after error generate
                        if (Directory.Exists(directoryPath).Equals(true))
                            Directory.Delete(directoryPath, true);
                    }
                });
            }
        }

        public void DownloadAssociationEntitySummaryIntoDXF(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["AssociationReportFilter"] != null)
            {

                DataSet ds = new DataSet();
                AssociationEntitiesSummaryView objAssociationEntitiesReport = new AssociationEntitiesSummaryView();
                AssociationReportFilter objAssociationReportFilter = new AssociationReportFilter();
                objAssociationReportFilter = (AssociationReportFilter)Session["AssociationReportFilter"];

                objAssociationEntitiesReport.objReportFilters.connectionString = objAssociationReportFilter.connectionString;
                objAssociationEntitiesReport.objReportFilters.SelectedRegionIds = objAssociationReportFilter.SelectedRegionIds;
                objAssociationEntitiesReport.objReportFilters.SelectedProvinceIds = objAssociationReportFilter.SelectedProvinceIds;
                objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationReportFilter.SelectedNetworkStatues;
                objAssociationEntitiesReport.objReportFilters.SelectedParentUsers = objAssociationReportFilter.SelectedParentUsers;
                objAssociationEntitiesReport.objReportFilters.SelectedUserIds = objAssociationReportFilter.SelectedUserIds;
                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = objAssociationReportFilter.SelectedLayerId;
                objAssociationEntitiesReport.objReportFilters.SelectedProjectIds = objAssociationReportFilter.SelectedProjectIds;
                objAssociationEntitiesReport.objReportFilters.SelectedPlanningIds = objAssociationReportFilter.SelectedPlanningIds;
                objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objAssociationReportFilter.SelectedWorkOrderIds;
                objAssociationEntitiesReport.objReportFilters.SelectedPurposeIds = objAssociationReportFilter.SelectedPurposeIds;
                objAssociationEntitiesReport.objReportFilters.durationbasedon = objAssociationReportFilter.durationbasedon;
                objAssociationEntitiesReport.objReportFilters.fromDate = objAssociationReportFilter.fromDate;
                objAssociationEntitiesReport.objReportFilters.toDate = objAssociationReportFilter.toDate;
                objAssociationEntitiesReport.objReportFilters.geom = objAssociationReportFilter.geom;
                objAssociationEntitiesReport.objReportFilters.currentPage = 0;
                objAssociationEntitiesReport.objReportFilters.fileType = "KML";
                objAssociationEntitiesReport.objReportFilters.userId = objAssociationReportFilter.userId;
                objAssociationEntitiesReport.objReportFilters.roleId = objAssociationReportFilter.roleId;
                objAssociationEntitiesReport.objReportFilters.radius = objAssociationReportFilter.radius;
                objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType = objAssociationReportFilter.SelectedOwnerShipType;
                objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objAssociationReportFilter.SelectedThirdPartyVendorIds;

                List<int> SelectedLayerId = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objAssociationReportFilter.SelectedLayerId;

                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                objAssociationReportFilter.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationReportFilter.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objAssociationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                var selectedlayerids = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objAssociationEntitiesReport.lstLayers = objAssociationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                }
                DataTable dtFilter = GetAssociationReportFilter(objAssociationReportFilter);


                string fileName = "AssociationReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    //int TotalEntityReport = 0;
                    AssociationReportLog associationReportLog = new AssociationReportLog();
                    associationReportLog.user_id = userdetails.user_id;
                    associationReportLog.export_started_on = DateTime.Now;
                    associationReportLog.file_name = fileName;
                    associationReportLog.file_type = "DXF";
                    associationReportLog.file_extension = ".dxf";
                    associationReportLog.status = "InProgress";
                    associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                    associationReportLog.planned = totalPlannedCount;
                    associationReportLog.asbuilt = totalAsBuiltCount;
                    associationReportLog.dormant = totalDormantCount;
                    associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                    associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                    try
                    {

                        for (int i = 0; i < objAssociationEntitiesReport.lstLayers.Count; i++)
                        {
                            objAssociationEntitiesReport.objReportFilters.layerName = objAssociationEntitiesReport.lstLayers[i].layer_name;
                            var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objAssociationEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                            List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetAssociationSummaryViewKML(objAssociationEntitiesReport.objReportFilters);
                            DataTable dtReport = new DataTable();
                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                            dtReport.TableName = layerDetail.layer_title;
                            if (dtReport.Rows.Count > 0)
                                ds.Tables.Add(dtReport);
                        }
                        objAssociationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                        objAssociationReportFilter.SelectedLayerId = SelectedLayerIdSummary;
                        string finalkml = KMLHelper.GetKmlForEntitiesNew(ds, objAssociationEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, dtFilter);
                        //Create a new subfolder under the current active folder
                        string newPath = Path.Combine(Server.MapPath("~/Uploads/"));


                        //dataContent is byte[]
                        System.IO.File.WriteAllText(newPath + "report.kml", finalkml.ToString());
                        string baseFolder = newPath + "report.kml";
                        string kmlFolder = newPath;
                        DataTable dataTable = new DataTable();
                        var converter = new Convertor(baseFolder, "", kmlFolder, "");
                        var response = converter.KmltoDXFConverter(newPath, "report");


                        string tempFileName = fileName + associationReportLog.file_extension;
                        //string ftpFolder = "ExportReportLog/";
                        //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                        string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPwd = ApplicationSettings.FTPPasswordAttachment;

                        associationReportLog.export_ended_on = DateTime.Now;
                        associationReportLog.status = "Success";
                        associationReportLog.file_location = ftpFolder + tempFileName;
                        //Thread.Sleep(10000);
                        CommonUtility.FTPFileUpload(response.OutputFile, tempFileName, ftpFilePath, ftpUserName, ftpPwd);
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        System.IO.File.Delete(response.OutputFile);
                    }
                    catch (Exception ex)
                    {
                        associationReportLog.export_ended_on = DateTime.Now;
                        associationReportLog.status = "Error occurred while processing request";
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        ErrorLogHelper.WriteErrorLog("DownloadAssociationEntitySummaryIntoDXF()", "Report", ex);
                    }

                });
                //string attachment = "attachment; filename=ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".dxf";
                //Response.ClearContent();
                //Response.Clear();
                //Response.ContentType = "application/xml";
                //Response.AddHeader("content-disposition", attachment);
                //Response.Write(response.Output);
                //Response.End();
            }
        }

        public void DownloadAssociationEntityReportIntoShape(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["AssociationReportFilter"] != null)
            {
                try
                {
                    //LogHelper.GetInstance.WriteDebugLog($"***********************************Shape logs start ***  {DateTime.Now}********************************");

                    AssociationEntitiesSummaryView objAssociationEntitiesReport = new AssociationEntitiesSummaryView();
                    AssociationReportFilter objAssociationReportFilter = new AssociationReportFilter();
                    objAssociationReportFilter = (AssociationReportFilter)Session["AssociationReportFilter"];

                    objAssociationEntitiesReport.objReportFilters.connectionString = objAssociationReportFilter.connectionString;
                    objAssociationEntitiesReport.objReportFilters.SelectedRegionIds = objAssociationReportFilter.SelectedRegionIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedProvinceIds = objAssociationReportFilter.SelectedProvinceIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationReportFilter.SelectedNetworkStatues;
                    objAssociationEntitiesReport.objReportFilters.SelectedParentUsers = objAssociationReportFilter.SelectedParentUsers;
                    objAssociationEntitiesReport.objReportFilters.SelectedUserIds = objAssociationReportFilter.SelectedUserIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = objAssociationReportFilter.SelectedLayerId;
                    objAssociationEntitiesReport.objReportFilters.SelectedProjectIds = objAssociationReportFilter.SelectedProjectIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedPlanningIds = objAssociationReportFilter.SelectedPlanningIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objAssociationReportFilter.SelectedWorkOrderIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedPurposeIds = objAssociationReportFilter.SelectedPurposeIds;
                    objAssociationEntitiesReport.objReportFilters.durationbasedon = objAssociationReportFilter.durationbasedon;
                    objAssociationEntitiesReport.objReportFilters.fromDate = objAssociationReportFilter.fromDate;
                    objAssociationEntitiesReport.objReportFilters.toDate = objAssociationReportFilter.toDate;
                    objAssociationEntitiesReport.objReportFilters.geom = objAssociationReportFilter.geom;
                    objAssociationEntitiesReport.objReportFilters.currentPage = 0;
                    objAssociationEntitiesReport.objReportFilters.fileType = "SHAPE";
                    objAssociationEntitiesReport.objReportFilters.userId = objAssociationReportFilter.userId;
                    objAssociationEntitiesReport.objReportFilters.roleId = objAssociationReportFilter.roleId;
                    objAssociationEntitiesReport.objReportFilters.radius = objAssociationReportFilter.radius;
                    objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType = objAssociationReportFilter.SelectedOwnerShipType;
                    objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objAssociationReportFilter.SelectedThirdPartyVendorIds;

                    List<int> SelectedLayerId = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objAssociationReportFilter.SelectedLayerId;

                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    objAssociationReportFilter.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationReportFilter.SelectedLayerId;

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objAssociationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objAssociationEntitiesReport.lstLayers = objAssociationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).DistinctBy(o => o.layer_id).ToList();
                    }
                    DataTable dtFilter = GetAssociationReportFilter(objAssociationReportFilter);


                    //DataSet ds = new DataSet();


                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {

                        AssociationReportLog associationReportLog = new AssociationReportLog();
                        associationReportLog.user_id = userdetails.user_id;
                        associationReportLog.export_started_on = DateTime.Now;
                        associationReportLog.file_type = "ALLSHAPE";
                        associationReportLog.file_extension = ".zip";
                        associationReportLog.status = "InProgress";
                        associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        associationReportLog.planned = totalPlannedCount;
                        associationReportLog.asbuilt = totalAsBuiltCount;
                        associationReportLog.dormant = totalDormantCount;
                        associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        dtFilter = null;
                        string shapeFilePath = "";

                        try
                        {

                            string tempFileName = String.Empty;
                            //string ftpFolder = "ExportReportLog/";
                            //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            //string shapeFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/public/Attachments/SI_Shape_" + Guid.NewGuid().ToString().Substring(0, 8));
                            //string parentFolder = $"SI_Shape_{Guid.NewGuid().ToString().Substring(0, 8)}_{userdetails.user_id}";
                            string parentFolder = $"Shape_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                            string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                            string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                            shapeFilePath = Path.Combine(Server.MapPath(pathWithParentFolder));

                            if (Directory.Exists(shapeFilePath).Equals(false))
                                Directory.CreateDirectory(shapeFilePath);

                            var tasks = new List<Task>();
                            foreach (var layer in objAssociationEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        objAssociationEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetAssociationSummaryViewKMLNew(objAssociationEntitiesReport.objReportFilters, layer.layer_name);
                                        DataTable dtReport = new DataTable();
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                                        dtReport.TableName = layer.layer_title;
                                        if (dtReport != null && dtReport.Rows.Count > 0)
                                        {
                                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                        }
                                        GetShapeFileOne(dtReport, "AssociationReport", ftpFilePath, ftpUserName, ftpPwd, shapeFilePath, layer.layer_name);
                                        dtReport = null;
                                        // LogHelper.GetInstance.WriteDebugLogTest($"====================================={layer.layer_name}====END {DateTime.Now}========================", layer.layer_name);

                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));

                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();

                            objAssociationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objAssociationReportFilter.SelectedLayerId = SelectedLayerIdSummary;

                            string zipshapePath = shapeFilePath + ".zip";//result.Replace("success:", "");
                                                                         //zip the shape file
                            using (var zip = new ZipFile())
                            {
                                zip.AddDirectory(shapeFilePath);
                                zip.Save(zipshapePath);
                            }
                            if (System.IO.File.Exists(zipshapePath))
                            {
                                string fileName = Path.GetFileName(zipshapePath);
                                Directory.Delete(shapeFilePath, true);
                            }
                            FileInfo file = new FileInfo(zipshapePath);
                            tempFileName = Path.GetFileNameWithoutExtension(file.FullName);

                            // LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading start on FTP : {DateTime.Now}");
                            CommonUtility.FTPFileUpload(zipshapePath, (tempFileName + ".zip"), ftpFilePath, ftpUserName, ftpPwd);
                            // LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading completed on FTP : {DateTime.Now}");
                            System.IO.File.Delete(zipshapePath);

                            associationReportLog.file_name = tempFileName;
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Success";
                            associationReportLog.file_location = ftpFolder + tempFileName + associationReportLog.file_extension;
                            //Thread.Sleep(10000);
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                            //LogHelper.GetInstance.WriteDebugLog($"***********************************Shape logs end ***  {DateTime.Now}********************************");
                        }
                        catch (Exception ex)
                        {
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Error occurred while processing request";
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadAssociationEntityReportIntoShape()", "Report", ex);
                            // delete folder after error generate
                            if (Directory.Exists(shapeFilePath).Equals(true))
                                Directory.Delete(shapeFilePath, true);
                        }
                    });



                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void DownloadAssociationEntityReportIntoTXT(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {

            if (Session["AssociationReportFilter"] != null)
            {
                try
                {
                    AssociationEntitiesSummaryView objAssociationEntitiesSummaryView = new AssociationEntitiesSummaryView();

                    AssociationReportFilter objAssociationReportFilter = new AssociationReportFilter();

                    objAssociationReportFilter = (AssociationReportFilter)Session["AssociationReportFilter"];

                    objAssociationEntitiesSummaryView.objReportFilters.connectionString = objAssociationReportFilter.connectionString;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedRegionIds = objAssociationReportFilter.SelectedRegionIds;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedProvinceIds = objAssociationReportFilter.SelectedProvinceIds;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedNetworkStatues = objAssociationReportFilter.SelectedNetworkStatues;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedParentUsers = objAssociationReportFilter.SelectedParentUsers;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedUserIds = objAssociationReportFilter.SelectedUserIds;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedLayerId = objAssociationReportFilter.SelectedLayerId;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedProjectIds = objAssociationReportFilter.SelectedProjectIds;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedPlanningIds = objAssociationReportFilter.SelectedPlanningIds;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedWorkOrderIds = objAssociationReportFilter.SelectedWorkOrderIds;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedPurposeIds = objAssociationReportFilter.SelectedPurposeIds;
                    objAssociationEntitiesSummaryView.objReportFilters.durationbasedon = objAssociationReportFilter.durationbasedon;
                    objAssociationEntitiesSummaryView.objReportFilters.fromDate = objAssociationReportFilter.fromDate;
                    objAssociationEntitiesSummaryView.objReportFilters.toDate = objAssociationReportFilter.toDate;
                    objAssociationEntitiesSummaryView.objReportFilters.geom = objAssociationReportFilter.geom;
                    objAssociationEntitiesSummaryView.objReportFilters.userId = objAssociationReportFilter.userId;
                    objAssociationEntitiesSummaryView.objReportFilters.roleId = objAssociationReportFilter.roleId;
                    objAssociationEntitiesSummaryView.objReportFilters.radius = objAssociationReportFilter.radius;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedOwnerShipType = objAssociationReportFilter.SelectedOwnerShipType;
                    objAssociationEntitiesSummaryView.objReportFilters.SelectedThirdPartyVendorIds = objAssociationReportFilter.SelectedThirdPartyVendorIds;

                    objAssociationEntitiesSummaryView.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objAssociationEntitiesSummaryView.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objAssociationReportFilter.SelectedLayerId;

                    objAssociationEntitiesSummaryView.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesSummaryView.objReportFilters.SelectedLayerId;
                    objAssociationReportFilter.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationReportFilter.SelectedLayerId;

                    DataTable dtFilter = GetAssociationReportFilter(objAssociationReportFilter);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objAssociationEntitiesSummaryView.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objAssociationEntitiesSummaryView.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objAssociationEntitiesSummaryView.lstLayers = objAssociationEntitiesSummaryView.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);


                    string fileName = "AssociationReport_" + (fileType == "ALLCSV" ? "CSV_" : "TXT_") + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");

                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        //int TotalEntityReport = 0;
                        AssociationReportLog associationReportLog = new AssociationReportLog();
                        associationReportLog.user_id = userdetails.user_id;
                        associationReportLog.export_started_on = DateTime.Now;
                        associationReportLog.file_name = fileName;
                        associationReportLog.file_type = fileType;
                        associationReportLog.file_extension = ".zip";
                        associationReportLog.status = "InProgress";
                        associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        associationReportLog.planned = totalPlannedCount;
                        associationReportLog.asbuilt = totalAsBuiltCount;
                        associationReportLog.dormant = totalDormantCount;
                        associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);

                        try
                        {
                            for (int i = 0; i < objAssociationEntitiesSummaryView.lstLayers.Count; i++)
                            {
                                objAssociationEntitiesSummaryView.objReportFilters.layerName = objAssociationEntitiesSummaryView.lstLayers[i].layer_name;
                                List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetAssociationReportSummaryView(objAssociationEntitiesSummaryView.objReportFilters);
                                if (lstExportEntitiesDetail.Count > 0)
                                {
                                    DataTable dtReport = new DataTable();
                                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                                                                                                                //dtReport.TableName = layerDetail.layer_title;
                                    dtReport.TableName = objAssociationEntitiesSummaryView.objReportFilters.layerName;
                                    if (dtReport != null && dtReport.Rows.Count > 0)
                                    {
                                        if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                        if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                        if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                    }
                                    if (dtReport.Rows.Count > 0)
                                        ds.Tables.Add(dtReport);
                                }
                            }
                            objAssociationEntitiesSummaryView.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objAssociationReportFilter.SelectedLayerId = SelectedLayerIdSummary;

                            string tempFileName = fileName + associationReportLog.file_extension;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            AssociationReportCSV(ds, tempFileName, ApplicationSettings.CsvDelimiter, fileType, ftpFilePath, ftpUserName, ftpPwd);
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Success";
                            associationReportLog.file_location = ftpFolder + tempFileName;
                            //Thread.Sleep(10000);
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        }
                        catch (Exception ex)
                        {
                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Error occurred while processing request";
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoCSVAll()", "Report", ex);
                        }

                    });

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public void AssociationReportCSV(DataSet ds, string fileNameValue, string delimiter, string fileType, string ftpfilePath, string ftpUserName, string ftpPassword)
        {
            List<Files> txtDatas = new List<Files>();
            foreach (DataTable dt in ds.Tables)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(Utility.CommonUtility.ConvertDataTableToString(dt, delimiter)); //Convert DataTable to string with specific delimiter
                txtDatas.Add(new Files() { TableName = dt.TableName, Bytes = bytes });
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (Files txtData in txtDatas)
                {
                    if (fileType == "ALLCSV")
                    {
                        zip.AddEntry(txtData.TableName + ".csv", txtData.Bytes);
                    }
                    else
                    {
                        zip.AddEntry(txtData.TableName + ".txt", txtData.Bytes);
                    }
                }
                string downloadTempPath = Settings.ApplicationSettings.DownloadTempPath;
                string tempfilePath = Path.Combine(Server.MapPath(downloadTempPath + "AssociationReportLog"), fileNameValue);

                zip.Save(tempfilePath);
                CommonUtility.FTPFileUpload(tempfilePath, fileNameValue, ftpfilePath, ftpUserName, ftpPassword);
                System.IO.File.Delete(tempfilePath);
            }
        }

        public void DownloadAssociationEntityReportIntoCSV(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {


            if (Session["AssociationReportFilter"] != null)
            {
                try
                {
                    //LogHelper.GetInstance.WriteDebugLog($"***********************************CSV logs start ***********************************");

                    DateTime startTime = DateTime.Now;
                    AssociationEntitiesSummaryView objAssociationEntitiesReport = new AssociationEntitiesSummaryView();
                    AssociationEntitiesReport AssociationEntitiesReportSummary = new AssociationEntitiesReport();
                    AssociationEntitiesReportSummary = (AssociationEntitiesReport)Session["EntityAssociationSummaryData"];
                    AssociationReportFilter objAssociationReportFilter = new AssociationReportFilter();

                    objAssociationReportFilter = (AssociationReportFilter)Session["AssociationReportFilter"];

                    objAssociationEntitiesReport.objReportFilters.connectionString = objAssociationReportFilter.connectionString;
                    objAssociationEntitiesReport.objReportFilters.SelectedRegionIds = objAssociationReportFilter.SelectedRegionIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedProvinceIds = objAssociationReportFilter.SelectedProvinceIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatues = objAssociationReportFilter.SelectedNetworkStatues;
                    objAssociationEntitiesReport.objReportFilters.SelectedParentUsers = objAssociationReportFilter.SelectedParentUsers;
                    objAssociationEntitiesReport.objReportFilters.SelectedUserIds = objAssociationReportFilter.SelectedUserIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = objAssociationReportFilter.SelectedLayerId;
                    objAssociationEntitiesReport.objReportFilters.SelectedProjectIds = objAssociationReportFilter.SelectedProjectIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedPlanningIds = objAssociationReportFilter.SelectedPlanningIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objAssociationReportFilter.SelectedWorkOrderIds;
                    objAssociationEntitiesReport.objReportFilters.SelectedPurposeIds = objAssociationReportFilter.SelectedPurposeIds;
                    objAssociationEntitiesReport.objReportFilters.durationbasedon = objAssociationReportFilter.durationbasedon;
                    objAssociationEntitiesReport.objReportFilters.fromDate = objAssociationReportFilter.fromDate;
                    objAssociationEntitiesReport.objReportFilters.toDate = objAssociationReportFilter.toDate;
                    objAssociationEntitiesReport.objReportFilters.geom = objAssociationReportFilter.geom;
                    objAssociationEntitiesReport.objReportFilters.userId = objAssociationReportFilter.userId;
                    objAssociationEntitiesReport.objReportFilters.roleId = objAssociationReportFilter.roleId;
                    objAssociationEntitiesReport.objReportFilters.radius = objAssociationReportFilter.radius;
                    objAssociationEntitiesReport.objReportFilters.selected_route_ids = objAssociationReportFilter.selected_route_ids;
                    objAssociationEntitiesReport.objReportFilters.SelectedOwnerShipType = objAssociationReportFilter.SelectedOwnerShipType;
                    objAssociationEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objAssociationReportFilter.SelectedThirdPartyVendorIds;

                    objAssociationEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objAssociationReportFilter.SelectedLayerId;

                    objAssociationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    objAssociationReportFilter.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objAssociationReportFilter.SelectedLayerId;

                    DataTable dtFilter = GetAssociationReportFilter(objAssociationReportFilter);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objAssociationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objAssociationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objAssociationEntitiesReport.lstLayers = objAssociationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    string parentFolder = $"AssociationReport_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                    string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                    string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                    string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));

                    // Create Parent folder for temparary basic on server 
                    if (Directory.Exists(directoryPath).Equals(false))
                        Directory.CreateDirectory(directoryPath);

                    string fileName = $"{parentFolder}/{dtFilter.TableName}.csv";
                    StreamCSVInFolder(dtFilter, fileName);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        //int TotalEntityReport = 0;
                        AssociationReportLog associationReportLog = new AssociationReportLog();
                        associationReportLog.user_id = userdetails.user_id;
                        associationReportLog.export_started_on = DateTime.Now;
                        associationReportLog.file_name = parentFolder;
                        associationReportLog.file_type = fileType;
                        associationReportLog.file_extension = ".zip";
                        associationReportLog.status = "InProgress";
                        associationReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        associationReportLog.planned = totalPlannedCount;
                        associationReportLog.asbuilt = totalAsBuiltCount;
                        associationReportLog.dormant = totalDormantCount;
                        associationReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                        dtFilter = null;
                        try
                        {
                            // ---Start-- Below code for generate export report summary in csv file -------
                            List<EntitySummaryReport> lstRprtData = AssociationEntitiesReportSummary.lstReportData;
                            if (lstRprtData.Count > 0)
                            {
                                if (!AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                                    AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                                if (!AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                                    AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                                if (!AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                                    AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = AssociationEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");

                                DataTable dtSummaryReport = new DataTable();
                                dtSummaryReport = MiscHelper.ListToDataTable(AssociationEntitiesReportSummary.lstReportData);
                                dtSummaryReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                                if (dtSummaryReport != null && dtSummaryReport.Rows.Count > 0)
                                {
                                    if (!ApplicationSettings.IsDormantEnabled)
                                    {
                                        dtSummaryReport.Columns.Remove("DORMANT_COUNT");
                                    }
                                    dtSummaryReport.Columns.Remove("entity_id");
                                    dtSummaryReport.Columns.Remove("entity_name");
                                    dtSummaryReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                    dtSummaryReport.Columns["planned_count"].ColumnName = "Planned";
                                    dtSummaryReport.Columns["as_built_count"].ColumnName = "As-Built";
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        dtSummaryReport.Columns["dormant_count"].ColumnName = "Dormant";
                                    }

                                    string[] networkstatusvalues = objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objAssociationEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                    if (networkstatusvalues.Length < 3)
                                    {
                                        if (!networkstatusvalues.Contains("P"))
                                        {
                                            dtSummaryReport.Columns.Remove("PLANNED");
                                        }
                                        if (!networkstatusvalues.Contains("A"))
                                        {
                                            dtSummaryReport.Columns.Remove("AS-BUILT");
                                        }
                                        if (ApplicationSettings.IsDormantEnabled)
                                        {
                                            if (!networkstatusvalues.Contains("D"))
                                            {
                                                dtSummaryReport.Columns.Remove("DORMANT");
                                            }
                                        }
                                    }
                                    DataRow row = dtSummaryReport.NewRow();
                                    row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                                    if (dtSummaryReport.Columns.Contains("Planned"))
                                    {
                                        row["Planned"] = dtSummaryReport.Compute("Sum(Planned)", "");
                                    }
                                    if (dtSummaryReport.Columns.Contains("As-Built"))
                                    {
                                        row["As-Built"] = dtSummaryReport.Compute("Sum([As-Built])", "");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (dtSummaryReport.Columns.Contains("Dormant"))
                                        {
                                            row["Dormant"] = dtSummaryReport.Compute("Sum(Dormant)", "");
                                        }
                                    }
                                    dtSummaryReport.Rows.Add(row);
                                    string summaryFileName = $"{parentFolder}/{dtSummaryReport.TableName}.csv";
                                    StreamCSVInFolder(dtSummaryReport, summaryFileName);
                                }
                            }
                            var tasks = new List<Task>();
                            foreach (var layer in objAssociationEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {

                                        objAssociationEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetAssociationReportSummaryViewCSV(objAssociationEntitiesReport.objReportFilters, layer.layer_name);
                                        if (lstExportEntitiesDetail.Count > 0)
                                        {
                                            DataTable dtReport = new DataTable();
                                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                                                                                                                        //dtReport.TableName = layerDetail.layer_title;
                                            dtReport.TableName = layer.layer_title;
                                            if (dtReport != null && dtReport.Rows.Count > 0)
                                            {
                                                if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                                if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                            }
                                            if (dtReport.Rows.Count > 0)
                                            {
                                                //	ds.Tables.Add(dtReport);
                                                objAssociationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                                objAssociationReportFilter.SelectedLayerId = SelectedLayerIdSummary;
                                                fileName = $"{parentFolder}/{dtReport.TableName}.csv";
                                                StreamNewCSVInFolder(dtReport, fileName);
                                                associationReportLog.export_ended_on = DateTime.Now;
                                                associationReportLog.status = "Success";
                                                associationReportLog.file_location = ftpFolder + parentFolder + associationReportLog.file_extension;
                                            }
                                            dtReport = null;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));
                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();

                            string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                            string zipfilePath = directoryPath + ".zip";
                            string fileNameValue = parentFolder + ".zip";

                            // Below code for Convert parent folder to ZIP and delete parent folder after ZIP on server
                            using (var zip = new ZipFile())
                            {
                                //zip.UseZip64WhenSaving = Zip64Option.Always;
                                //zip.CompressionMethod = CompressionMethod.BZip2;
                                zip.AddDirectory(directoryPath);
                                zip.Save(zipfilePath);
                            }
                            if (System.IO.File.Exists(zipfilePath))
                            {
                                string fileZipName = Path.GetFileName(zipfilePath);
                                Directory.Delete(directoryPath, true);
                            }
                            FileInfo file = new FileInfo(zipfilePath);

                            // ZIP File upload on FTP server
                            CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);

                            // Deleted ZIP on Server after uploaded on FTP server
                            System.IO.File.Delete(zipfilePath);
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                            DateTime endTime = DateTime.Now;
                        }
                        catch (Exception ex)
                        {

                            associationReportLog.export_ended_on = DateTime.Now;
                            associationReportLog.status = "Error occurred while processing request";
                            associationReportLog = new BLAssociationReportLog().SaveAssociationReportLog(associationReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadAssociationEntityReportIntoCSV()", "Report", ex);
                            // delete folder after error generate
                            if (Directory.Exists(directoryPath).Equals(true))
                                Directory.Delete(directoryPath, true);
                        }

                    });

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

        }

        #endregion

        #region BOM/BOQ Report

        [HttpPost]

        public ActionResult ShowBOMReport(BOMViewModel objBOMViewModel)
        {
            //System.Threading.Thread.Sleep(10000);
            //BOMViewModel objBOMViewModel = new BOMViewModel();
            var userdetails = (User)Session["userDetail"];
            objBOMViewModel.objBomInput.SelectedProvinceIds = objBOMViewModel.objBomInput.provinceId != null && objBOMViewModel.objBomInput.provinceId.ToList().Count > 0 ? string.Join(",", objBOMViewModel.objBomInput.provinceId.ToArray()) : "";
            objBOMViewModel.objBomInput.SelectedRegionIds = objBOMViewModel.objBomInput.regionId != null && objBOMViewModel.objBomInput.regionId.Count() > 0 ? string.Join(",", objBOMViewModel.objBomInput.regionId.ToArray()) : "";
            objBOMViewModel.objBomInput.userId = Convert.ToInt32(userdetails.user_id);
            objBOMViewModel.objBomInput.roleId = Convert.ToInt32(userdetails.role_id);
            Session["BomBoqInput"] = objBOMViewModel.objBomInput;
            objBOMViewModel.lstBomReport = new BLBom().getBOMReport(objBOMViewModel.objBomInput);
            //Bind region province dropdowns
            BindBOMDropdown(ref objBOMViewModel);
            return PartialView("_BOMReport", objBOMViewModel);
        }


        public void BindBOMDropdown(ref BOMViewModel objBOMViewModel)
        {
            //if (string.IsNullOrWhiteSpace(objBOMViewModel.objBomInput.geom)) { }
            //Bind Regions..
            objBOMViewModel.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { geom = objBOMViewModel.objBomInput.geom, buffRadius = objBOMViewModel.objBomInput.buff_Radius, geomType = objBOMViewModel.objBomInput.geomType, userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objBOMViewModel.objBomInput.SelectedRegionIds))
            {
                objBOMViewModel.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { geom = objBOMViewModel.objBomInput.geom, buffRadius = objBOMViewModel.objBomInput.buff_Radius, geomType = objBOMViewModel.objBomInput.geomType, regionIds = objBOMViewModel.objBomInput.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }
        }

        [HttpPost]
        public ActionResult ValidatePotentialArea(string geom, string geomType, double buff_Radius = 0.0, string network_status = null)
        {
            JsonResponse<string> objBom = new JsonResponse<string>();
            var objValid = new BLMisc().ValidatePotentialArea(geom, Convert.ToInt32(Session["user_id"]), geomType, buff_Radius);
            if (objValid.status == false)
            {
                objBom.status = ResponseStatus.FAILED.ToString();
                objBom.message = BLConvertMLanguage.MultilingualMessageConvert(objValid.message);//objValid.message;
            }
            else
            {
                objBom.status = "SUCCESS";
            }
            return Json(objBom, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ValidateLMCPotentialArea(string geom, string geomType, double buff_Radius = 0.0, string network_status = null)
        {
            JsonResponse<string> objBom = new JsonResponse<string>();
            var objValid = new BLMisc().ValidateLMCPotentialArea(geom, Convert.ToInt32(Session["user_id"]), geomType, buff_Radius);
            if (objValid.status == false)
            {
                objBom.status = ResponseStatus.FAILED.ToString();
                objBom.message = BLConvertMLanguage.MultilingualMessageConvert(objValid.message);//objValid.message;
            }
            else
            {
                objBom.status = "SUCCESS";
            }
            return Json(objBom, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void ExportBOMBOQReport(string type)
        {
            if (Session["BomBoqInput"] != null && !string.IsNullOrWhiteSpace(type))
            {
                BOMInput objBomInput = (BOMInput)Session["BomBoqInput"];
                List<BOMReport> lstBomReport = new BLBom().getBOMReport(objBomInput);
                if (string.IsNullOrWhiteSpace(objBomInput.network_status))
                {
                    PDFHelper.ExportBOMBOQReportInPDF(lstBomReport, type);
                }
                else
                {
                    PDFHelper.ExportNetowrkStatusWiseBOMBOQReportInPDF(lstBomReport, objBomInput.network_status, type);
                }
            }
        }

        public void ExportBOMWithDetailInExcel()
        {
            if (Session["BomBoqInput"] != null)
            {
                DataSet ds = new DataSet();
                IWorkbook workbook = null;
                string strSheetName = "BOM_BOQ_REPORT";
                string fileName = "BomBOQ_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                BOMInput objBomInput = (BOMInput)Session["BomBoqInput"];
                List<BOMReport> lstBomReport = new BLBom().getBOMReport(objBomInput);
                DataTable dtBOM = MiscHelper.ListToDataTable<BOMReport>(lstBomReport);
                if (!string.IsNullOrWhiteSpace(objBomInput.network_status))
                {
                    workbook = GetNetworkStatusWiseBOMBOQWorkBook(lstBomReport, objBomInput.network_status, strSheetName);
                }
                else
                {
                    workbook = GetBOMBOQWorkBook(lstBomReport, strSheetName);
                }



                //export into excel...
                ExportWorkBookToExcel(workbook, fileName);
            }
        }

        public void ExportISPBOMDetailInExcel(int structure_id, int building_id = 0)
        {
            if (structure_id > 0)
            {
                DataSet ds = new DataSet();
                IWorkbook workbook = null;
                string strSheetName = "ISP_BOM_BOQ_REPORT";
                string fileName = "ISP_BomBOQ_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");

                List<BOMReport> lstBomReport = new BLBom().getISPBOMReport(structure_id, building_id);
                DataTable dtBOM = MiscHelper.ListToDataTable<BOMReport>(lstBomReport);
                workbook = GetBOMBOQWorkBook(lstBomReport, strSheetName);

                //export into excel...
                ExportWorkBookToExcel(workbook, fileName);
            }
        }

        public IWorkbook GetNetworkStatusWiseBOMBOQWorkBook(List<BOMReport> lstBomReport, string networkStatus, string sheetName)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            IWorkbook workbook = new XSSFWorkbook();
            ISheet bomSheet = workbook.CreateSheet(sheetName);

            #region ADD HEADER COLUMNS
            ICellStyle headerStyle = NPOIExcelHelper.getCellStyle(workbook, "HEADER");

            //first row..
            IRow row1 = bomSheet.CreateRow(0);
            NPOIExcelHelper.CreateCustomCell(row1, 0, Resources.Resources.SI_OSP_GBL_NET_GBL_015, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 1, Resources.Resources.SI_OSP_GBL_NET_FRM_012, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 2, string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_143, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 3, string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_142, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 4, networkStatus.ToUpper() == "P" ? "Planned" : (networkStatus.ToUpper() == "A" ? "As-Built" : "Dormant"), headerStyle);

            //second row..
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 1, 0, 0));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 1, 1, 1));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 1, 2, 2));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 1, 3, 3));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 0, 4, 8));



            IRow row2 = bomSheet.CreateRow(1);
            NPOIExcelHelper.CreateCustomCell(row2, 4, Resources.Resources.SI_OSP_GBL_NET_RPT_021, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 5, string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 6, string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 7, Resources.Resources.SI_OSP_GBL_NET_RPT_023, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 8, Resources.Resources.SI_OSP_GBL_NET_RPT_024, headerStyle);

            NPOIExcelHelper.setBordersToMergedCells(workbook, bomSheet);

            #endregion

            #region ADD ROW DATA
            int rowCount = 2;
            foreach (var objBom in lstBomReport)
            {
                IRow newRow = bomSheet.CreateRow(rowCount);
                var cellStyle = NPOIExcelHelper.getCellStyle(workbook, objBom.is_header ? "SUB_HEADER" : "");
                var entity_type = objBom.is_header ? objBom.entity_type : objBom.entity_sub_type;
                var cost_per_unit = objBom.is_header ? "" : objBom.cost_per_unit.ToString();
                var serviceCostPerunit = objBom.is_header ? "" : objBom.service_cost_per_unit.ToString();
                var count = networkStatus.ToUpper() == "P" ? objBom.total_planned_count.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_count.ToString() : objBom.total_dormant_count.ToString());
                var total_cost = networkStatus.ToUpper() == "P" ? objBom.total_planned_cost.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_cost.ToString() : objBom.total_dormant_cost.ToString());
                var total_Service_cost = networkStatus.ToUpper() == "P" ? objBom.total_planned_service_cost.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_service_cost.ToString() : objBom.total_dormant_service_cost.ToString());
                var total_calc_length = objBom.geom_type.ToUpper() == "LINE" ? (networkStatus.ToUpper() == "P" ? objBom.total_planned_calc_length.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_calc_length.ToString() : objBom.total_dormant_calc_length.ToString())) : "NA";
                var total_gis_length = objBom.geom_type.ToUpper() == "LINE" ? (networkStatus.ToUpper() == "P" ? objBom.total_planned_gis_length.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_gis_length.ToString() : objBom.total_dormant_gis_length.ToString())) : "NA";

                NPOIExcelHelper.CreateCustomCell(newRow, 0, entity_type, cellStyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 1, objBom.specification, cellStyle);
                //NPOIExcelHelper.CreateCustomCell(newRow, 2, objBom.item_code, cellStyle);
                // NPOIExcelHelper.CreateCustomCell(newRow, 3, objBom.name, cellStyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 2, cost_per_unit, cellStyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 3, serviceCostPerunit, cellStyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 4, count, cellStyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 5, total_cost, cellStyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 6, total_Service_cost, cellStyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 7, total_calc_length, cellStyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 8, total_gis_length, cellStyle);
                rowCount++;
            }
            #endregion

            #region ADD FOOTER DATA
            var query = lstBomReport.Where(p => p.is_header == true);

            var totalCount = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_count).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_count).ToString() : query.Sum(p => p.total_dormant_count).ToString());
            var totalCost = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_cost).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_cost).ToString() : query.Sum(p => p.total_dormant_cost).ToString());
            var totalServiceCost = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_service_cost).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_service_cost).ToString() : query.Sum(p => p.total_dormant_service_cost).ToString());
            var totalCalcLength = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_calc_length).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_calc_length).ToString() : query.Sum(p => p.total_dormant_calc_length).ToString());
            var totalGISLength = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_gis_length).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_gis_length).ToString() : query.Sum(p => p.total_dormant_gis_length).ToString());
            IRow footerRow = bomSheet.CreateRow(rowCount);
            NPOIExcelHelper.CreateCustomCell(footerRow, 0, "Total", headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 1, "", headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 2, "", headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 3, "", headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 4, totalCount, headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 5, totalCost, headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 6, totalServiceCost, headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 7, totalCalcLength, headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 8, totalGISLength, headerStyle);

            #endregion

            for (int i = 0; i < 7; i++)
            {
                bomSheet.AutoSizeColumn(i, true);
            }

            return workbook;
        }

        public IWorkbook GetBOMBOQWorkBook(List<BOMReport> lstBomReport, string sheetName)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

            IWorkbook workbook = new XSSFWorkbook();
            ISheet bomSheet = workbook.CreateSheet(sheetName);

            #region ADD HEADER COLUMNS
            ICellStyle headerStyle = NPOIExcelHelper.getCellStyle(workbook, "HEADER");

            //first row..
            IRow row1 = bomSheet.CreateRow(0);
            NPOIExcelHelper.CreateCustomCell(row1, 0, Resources.Resources.SI_OSP_GBL_NET_FRM_020, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 1, Resources.Resources.SI_OSP_GBL_NET_FRM_012, headerStyle);
            //NPOIExcelHelper.CreateCustomCell(row1, 2, Resources.Resources.SI_OSP_GBL_GBL_GBL_143, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 2, string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_143, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 3, String.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_142, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 4, "Planned", headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 9, "As-Built", headerStyle);
            NPOIExcelHelper.CreateCustomCell(row1, 14, "Dormant", headerStyle);

            //second row..
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 1, 0, 0));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 1, 1, 1));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 1, 2, 2));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 1, 3, 3));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 0, 4, 8));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 0, 9, 13));
            bomSheet.AddMergedRegion(new CellRangeAddress(0, 0, 14, 18));

            IRow row2 = bomSheet.CreateRow(1);
            NPOIExcelHelper.CreateCustomCell(row2, 4, Resources.Resources.SI_OSP_GBL_NET_RPT_021, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 5, string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 6, string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 7, Resources.Resources.SI_OSP_GBL_NET_RPT_023, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 8, Resources.Resources.SI_OSP_GBL_NET_RPT_024, headerStyle);

            NPOIExcelHelper.CreateCustomCell(row2, 9, Resources.Resources.SI_OSP_GBL_NET_RPT_021, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 10, string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 11, string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 12, Resources.Resources.SI_OSP_GBL_NET_RPT_023, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 13, Resources.Resources.SI_OSP_GBL_NET_RPT_024, headerStyle);

            NPOIExcelHelper.CreateCustomCell(row2, 14, Resources.Resources.SI_OSP_GBL_NET_RPT_021, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 15, string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 16, string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 17, Resources.Resources.SI_OSP_GBL_NET_RPT_023, headerStyle);
            NPOIExcelHelper.CreateCustomCell(row2, 18, Resources.Resources.SI_OSP_GBL_NET_RPT_024, headerStyle);

            NPOIExcelHelper.setBordersToMergedCells(workbook, bomSheet);

            #endregion

            #region ADD ROW DATA
            int rowCount = 2;
            foreach (var objBom in lstBomReport)
            {
                IRow newRow = bomSheet.CreateRow(rowCount);

                var entity_type = objBom.is_header ? objBom.entity_type : objBom.entity_sub_type;
                var cost_per_unit = objBom.is_header ? "" : objBom.cost_per_unit.ToString();
                var service_cost_per_unit = objBom.is_header ? "" : objBom.service_cost_per_unit.ToString();
                var cellstyle = NPOIExcelHelper.getCellStyle(workbook, objBom.is_header ? "SUB_HEADER" : "");
                NPOIExcelHelper.CreateCustomCell(newRow, 0, entity_type, cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 1, objBom.specification, cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 2, cost_per_unit, cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 3, service_cost_per_unit, cellstyle);
                // NPOIExcelHelper.CreateCustomCell(newRow, 4, cost_per_unit, cellstyle);
                // NPOIExcelHelper.CreateCustomCell(newRow, 5, service_cost_per_unit, cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 4, objBom.total_planned_count.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 5, objBom.total_planned_cost.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 6, objBom.total_planned_service_cost.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 7, objBom.geom_type.ToUpper() == "LINE" ? objBom.total_planned_calc_length.ToString() : "NA", cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 8, objBom.geom_type.ToUpper() == "LINE" ? objBom.total_planned_gis_length.ToString() : "NA", cellstyle);

                NPOIExcelHelper.CreateCustomCell(newRow, 9, objBom.total_asbuilt_count.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 10, objBom.total_asbuilt_cost.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 11, objBom.total_asbuilt_service_cost.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 12, objBom.geom_type.ToUpper() == "LINE" ? objBom.total_asbuilt_calc_length.ToString() : "NA", cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 13, objBom.geom_type.ToUpper() == "LINE" ? objBom.total_asbuilt_gis_length.ToString() : "NA", cellstyle);

                NPOIExcelHelper.CreateCustomCell(newRow, 14, objBom.total_dormant_count.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 15, objBom.total_dormant_cost.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 16, objBom.total_dormant_service_cost.ToString(), cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 17, objBom.geom_type.ToUpper() == "LINE" ? objBom.total_dormant_calc_length.ToString() : "NA", cellstyle);
                NPOIExcelHelper.CreateCustomCell(newRow, 18, objBom.geom_type.ToUpper() == "LINE" ? objBom.total_dormant_gis_length.ToString() : "NA", cellstyle);
                rowCount++;
            }
            #endregion

            #region ADD FOOTER DATA
            var query = lstBomReport.Where(p => p.is_header == true);

            IRow footerRow = bomSheet.CreateRow(rowCount);
            NPOIExcelHelper.CreateCustomCell(footerRow, 0, "Total", headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 1, "", headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 2, "", headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 3, "", headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 4, query.Sum(m => m.total_planned_count).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 5, query.Sum(m => m.total_planned_cost).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 6, query.Sum(m => m.total_planned_service_cost).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 7, query.Sum(m => m.total_planned_calc_length).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 8, query.Sum(m => m.total_planned_gis_length).ToString(), headerStyle);

            NPOIExcelHelper.CreateCustomCell(footerRow, 9, query.Sum(m => m.total_asbuilt_count).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 10, query.Sum(m => m.total_asbuilt_cost).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 11, query.Sum(m => m.total_asbuilt_service_cost).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 12, query.Sum(m => m.total_asbuilt_calc_length).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 13, query.Sum(m => m.total_asbuilt_gis_length).ToString(), headerStyle);

            NPOIExcelHelper.CreateCustomCell(footerRow, 14, query.Sum(m => m.total_dormant_count).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 15, query.Sum(m => m.total_dormant_cost).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 16, query.Sum(m => m.total_dormant_service_cost).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 17, query.Sum(m => m.total_dormant_calc_length).ToString(), headerStyle);
            NPOIExcelHelper.CreateCustomCell(footerRow, 18, query.Sum(m => m.total_dormant_gis_length).ToString(), headerStyle);

            #endregion
            for (int i = 0; i < 15; i++)
            {
                bomSheet.AutoSizeColumn(i, true);
            }
            return workbook;
        }


        public void ExportWorkBookToExcel(IWorkbook workbook, string fileName)
        {

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                workbook.Write(exportData);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                Response.BinaryWrite(exportData.ToArray());
                Response.End();
            }
        }

        #endregion

        #region ROW
        public ActionResult ExportROWReport(ExportEntitiesReport objExportEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {

            objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
            objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToList().Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.pageSize = 10;
            objExportEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
            objExportEntitiesReport.objReportFilters.sort = sort;
            objExportEntitiesReport.objReportFilters.sortdir = sortdir;
            objExportEntitiesReport.objReportFilters.userId = Convert.ToInt32(Session["user_id"]);
            if (!string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.layerName))
            {
                List<Dictionary<string, string>> lstExportEntitiesDetail = BLROW.Instance.GetExportReportData(objExportEntitiesReport.objReportFilters);
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE" };
                foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                {
                    var obj = (IDictionary<string, object>)new ExpandoObject();

                    foreach (var col in dic)
                    {
                        //if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                        //{
                        //    obj.Add(col.Key, col.Value);
                        //}
                        obj.Add(col.Key, col.Value);
                    }
                    objExportEntitiesReport.lstReportData.Add(obj);
                }

                objExportEntitiesReport.lstReportData = BLConvertMLanguage.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                objExportEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
            }
            Session["ExportReportFilter"] = objExportEntitiesReport.objReportFilters;
            BindROWReportDropdown(ref objExportEntitiesReport);
            return PartialView("_ExportROWReport", objExportEntitiesReport);
        }
        public void BindROWReportDropdown(ref ExportEntitiesReport objExportEntitiesReport)
        {

            //Bind Regions..
            objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }

            objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(EntityType.ROW.ToString());
            objExportEntitiesReport.lstLayerDurationBasedColumns = new BLLayer().GetDurationBasedColumnName(EntityType.ROW.ToString());


        }
        public ActionResult ExportFiberAllocationReport(ExportEntitiesReport objExportEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {

            objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
            objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToList().Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.pageSize = 10;
            objExportEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
            objExportEntitiesReport.objReportFilters.sort = sort;
            objExportEntitiesReport.objReportFilters.sortdir = sortdir;
            objExportEntitiesReport.objReportFilters.userId = Convert.ToInt32(Session["user_id"]);
            if (!string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.layerName))
            {
                PageMessage objMsg = new PageMessage();
                Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
                ////create ftp folder if not exist

                string ftpFilePath = ApplicationSettings.FTPAttachment;
                string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                string[] ftplogReportDirectory = new string[] { ftpFolder.Replace("/", "") };
                CreateNestedDirectoryOnFTP(ftpFilePath, ftpUserName, ftpPwd, ftplogReportDirectory);
                DownloadFiberAllocationReportIntoExcel(objExportEntitiesReport);
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Request is processing in background.Please check the export report log page.";
                objExportEntitiesReport.pageMsg = objMsg;

            }
            Session["ExportReportFilter"] = objExportEntitiesReport.objReportFilters;
            BindExportFiberAllocationReportDropdown(ref objExportEntitiesReport);
            return PartialView("_ExportFiberAllocationReport", objExportEntitiesReport);
        }
        public void BindExportFiberAllocationReportDropdown(ref ExportEntitiesReport objExportEntitiesReport)
        {

            //Bind Regions..
            objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }

            objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(EntityType.FMS.ToString());
            objExportEntitiesReport.lstLayerDurationBasedColumns = new BLLayer().GetDurationBasedColumnName(EntityType.FMS.ToString());


        }
        public void DownloadROWReport(string fileType, string reportType)
        {
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                if (reportType.ToUpper() == "APPROVAL")
                {
                    DownloadApprovelReportIntoExcel(reportType);
                }
                else if (reportType.ToUpper() == "ALL" && fileType.ToUpper() == "EXCEL")
                {
                    DownloadROWReportIntoExcel(reportType);
                }
                else if (reportType.ToUpper() == "ALL" && fileType.ToUpper() == "KML")
                {
                    DownloadROWReportIntoKML();
                }
                else if (reportType.ToUpper() == "RECURRING")
                {
                    DownloadRecurringReportIntoExcel(reportType);
                }

            }

        }
        public void DownloadApprovelReportIntoExcel(string fileType)
        {

            if (Session["ExportReportFilter"] != null)
            {

                try
                {

                    ExportReportFilter objReportFilter = (ExportReportFilter)Session["ExportReportFilter"];
                    objReportFilter.currentPage = 0;
                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objReportFilter.layerName.ToUpper()).FirstOrDefault();

                    List<Dictionary<string, string>> lstExportEntitiesDetail = BLROW.Instance.GetExportApprovelReportData(objReportFilter);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = layerDetail.layer_title;
                    if (dtReport != null && dtReport.Rows.Count >= 0)
                    {
                        if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                        if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                    }


                    if (dtReport.Rows.Count >= 0)
                    {
                        ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_" + fileType + "_" + MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadROWReportIntoExcel(string reportType)
        {

            if (Session["ExportReportFilter"] != null)
            {

                try
                {

                    ExportReportFilter objReportFilter = (ExportReportFilter)Session["ExportReportFilter"];
                    objReportFilter.currentPage = 0;
                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objReportFilter.layerName.ToUpper()).FirstOrDefault();

                    List<Dictionary<string, string>> lstExportEntitiesDetail = BLROW.Instance.GetExportReportData(objReportFilter);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = layerDetail.layer_title;
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                        if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                    }


                    if (dtReport.Rows.Count > 0)
                    {
                        ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_Report_" + MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public JsonResult getROWStageRecordCount()
        {
            ROWStageRecordCount result = null;
            if (Session["ExportReportFilter"] != null)
            {
                ExportReportFilter objReportFilter = (ExportReportFilter)Session["ExportReportFilter"];
                if (!string.IsNullOrEmpty(objReportFilter.layerName))
                {
                    result = BLROW.Instance.GetROWStageRecordCount(objReportFilter);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void DownloadRecurringReportIntoExcel(string fileType)
        {
            if (Session["ExportReportFilter"] != null)
            {
                try
                {
                    ExportReportFilter objReportFilter = (ExportReportFilter)Session["ExportReportFilter"];
                    objReportFilter.currentPage = 0;
                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objReportFilter.layerName.ToUpper()).FirstOrDefault();

                    List<Dictionary<string, string>> lstExportEntitiesDetail = BLROW.Instance.GetExportRecurringReportData(objReportFilter);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = layerDetail.layer_title;
                    if (dtReport.Rows.Count > 0)
                    {
                        ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_Recurring_Access_Charge_Report_" + MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void DownloadROWBudgetReport(int systemId)
        {
            List<Dictionary<string, string>> lstExportEntitiesDetail = BLROW.Instance.GetExportBudgetReportData(systemId);
            lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
            rowApplyStage rowApply = BLROW.Instance.getROWApplyDetails(systemId);
            ROWMaster objROW = new BLMisc().GetEntityDetailById<ROWMaster>(systemId, EntityType.ROW);
            DataTable dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
            dtReport.TableName = "BudgetApprovalReport";
            if (dtReport.Rows.Count > 0)
            {
                ExportROWBudgetData(dtReport, "ROW_Budget_Approval_Report_" + MiscHelper.getTimeStamp(), rowApply.road_name, objROW.network_id);
            }
        }
        public void DownloadROWReportIntoKML()
        {
            if (Session["ExportReportFilter"] != null)
            {
                StringBuilder sbLine = new StringBuilder();
                StringBuilder sbPoint = new StringBuilder();
                StringBuilder sbPolygon = new StringBuilder();
                sbLine.Append("<Folder>");
                sbPoint.Append("<Folder>");
                sbPolygon.Append("<Folder>");
                try
                {
                    BLLayer objBLLayer = new BLLayer();
                    List<ExportReportKML> lstExportReportKML = new List<ExportReportKML>();
                    ExportReportFilter objReportFilter = (ExportReportFilter)Session["ExportReportFilter"];
                    lstExportReportKML = new BLROW().GetExportReportDataKML(objReportFilter);

                    foreach (var objEntity in lstExportReportKML)
                    {

                        if (objEntity.geom_type.ToUpper() == "POLYGON")
                        {
                            sbPolygon.Append("<Placemark><name>" + objEntity.entity_title + "</name><description>" + objEntity.network_id + "</description><styleUrl>#transGreenPoly</styleUrl>");
                            sbPolygon.Append("<Polygon><outerBoundaryIs><LinearRing><coordinates>");
                            if (!string.IsNullOrEmpty(objEntity.geom))
                            {
                                string t = objEntity.geom.Substring(9, objEntity.geom.Length - 12);
                                string[] x = t.Split(',');
                                foreach (string y in x)
                                {
                                    sbPolygon.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                                }
                            }
                            sbPolygon.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
                        }
                    }

                    sbLine.Append("</Folder>");
                    sbPoint.Append("</Folder>");
                    sbPolygon.Append("</Folder>");

                    string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                               "<Document>  <!-- Begin Style Definitions -->" +
                                "<Style id=\"cable\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                "<Style id=\"transGreenPoly\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>AEAEAC</color></PolyStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                sbPoint.ToString() + sbLine.ToString() + sbPolygon.ToString() + "</Document></kml>";


                    string attachment = "attachment; filename=export_" + lstExportReportKML[0].entity_title + ".kml";
                    Response.ClearContent();
                    Response.ContentType = "application/xml";
                    Response.AddHeader("content-disposition", attachment);
                    Response.Write(finalKMLString);
                    Response.End();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region LMC Report
        public ActionResult ExportLMCReport(ExportLMCEntitiesReport objExportEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {

            objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
            objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToList().Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.pageSize = 10;
            objExportEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
            //objExportEntitiesReport.objReportFilters.sort = String.IsNullOrEmpty(sort) ? "" : sort.Replace(" ", "_"); 
            objExportEntitiesReport.objReportFilters.sort = sort;
            objExportEntitiesReport.objReportFilters.sortdir = sortdir;
            objExportEntitiesReport.objReportFilters.userId = Convert.ToInt32(Session["user_id"]);
            if (!string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.sort))
            {
                var result = new BLLmcInfo().GetColumnNameByDisplayName(objExportEntitiesReport.objReportFilters.sort, objExportEntitiesReport.objReportFilters.lmcType, objExportEntitiesReport.objReportFilters.entityType);
                objExportEntitiesReport.objReportFilters.sort = result;
            }
            //if (objExportEntitiesReport.objReportFilters.sort=="Can_Id")
            //{
            //    objExportEntitiesReport.objReportFilters.sort = "network_id";

            //}
            if (!string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.entityType))
            {
                List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLmcInfo().GetExportLMCReportData(objExportEntitiesReport.objReportFilters);
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "SYSTEM ID" };
                foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                {
                    var obj = (IDictionary<string, object>)new ExpandoObject();

                    foreach (var col in dic)
                    {
                        //if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                        //{
                        //    obj.Add(col.Key, col.Value);
                        //}
                        obj.Add(col.Key, col.Value);
                    }
                    objExportEntitiesReport.lstReportData.Add(obj);
                }
                objExportEntitiesReport.lstReportData = BLConvertMLanguage.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                objExportEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
            }
            Session["ExportLMCReportFilter"] = objExportEntitiesReport.objReportFilters;
            BindLMCReportDropdown(ref objExportEntitiesReport);
            return PartialView("_ExportLMCReport", objExportEntitiesReport);
        }

        public void BindLMCReportDropdown(ref ExportLMCEntitiesReport objExportEntitiesReport)
        {

            //Bind Regions..
            objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.entityType) && !string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.lmcType))
            {
                objExportEntitiesReport.lstLayerColumns = new BLLayer().GetLMCReportSearchByColumnName(objExportEntitiesReport.objReportFilters.entityType, objExportEntitiesReport.objReportFilters.lmcType);

            }
            // objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(EntityType.ROW.ToString());
            //  objExportEntitiesReport.lstLayerDurationBasedColumns = new BLLayer().GetDurationBasedColumnName(EntityType.ROW.ToString());  
        }
        public JsonResult BindLMCSearchBy(string entity_type, string lmc_type)
        {
            var objResp = new BLLayer().GetLMCReportSearchByColumnName(entity_type, lmc_type);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        public void DownloadLMCReport(string fileType, string reportType, string entityType)
        {
            if (!string.IsNullOrWhiteSpace(fileType))
            {

                if (reportType.ToUpper() == "ALL" && fileType.ToUpper() == "EXCEL")
                {
                    DownloadLMCReportIntoExcel(reportType, entityType);
                }
                else if (reportType.ToUpper() == "ALL" && fileType.ToUpper() == "KML")
                {
                    DownloadLMCReportIntoKML();
                }
            }

        }
        public void DownloadLMCReportIntoExcel(string reportType, string entityType)
        {

            if (Session["ExportLMCReportFilter"] != null)
            {

                try
                {

                    ExportLMCReportFilter objReportFilter = (ExportLMCReportFilter)Session["ExportLMCReportFilter"];
                    objReportFilter.currentPage = 0;
                    //Filter the Layer Detail
                    //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objReportFilter.layerName.ToUpper()).FirstOrDefault();

                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLmcInfo().GetExportLMCReportData(objReportFilter);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    // dtReport.TableName = layerDetail.layer_title;
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                        if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        if (dtReport.Columns.Contains("system Id")) { dtReport.Columns.Remove("system Id"); }
                    }


                    if (dtReport.Rows.Count > 0)
                    {
                        ExportData(dtReport, entityType + "_Report_" + MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }


        public void DownloadLMCReportIntoKML()
        {
            if (Session["ExportLMCReportFilter"] != null)
            {
                try
                {
                    BLLayer objBLLayer = new BLLayer();
                    List<ExportReportKML> lstExportReportKML = new List<ExportReportKML>();
                    ExportLMCReportFilter objReportFilter = (ExportLMCReportFilter)Session["ExportLMCReportFilter"];
                    objReportFilter.currentPage = 0;
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLmcInfo().GetExportLMCReportDataKML(objReportFilter);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                    objReportFilter.currentPage = 1;
                    DataSet ds = new DataSet();
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = "LMCREPORT";
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                    KMLHelper.GetKMLForLMCEntities(ds, ApplicationSettings.DownloadTempPath, objReportFilter);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region VSAT Report
        public ActionResult ExportVSATReport(ExportVSATEntitiesReport objExportEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {

            //objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
            objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToList().Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objExportEntitiesReport.objReportFilters.pageSize = 10;
            objExportEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
            //objExportEntitiesReport.objReportFilters.sort = String.IsNullOrEmpty(sort) ? "" : sort.Replace(" ", "_"); 
            objExportEntitiesReport.objReportFilters.sort = sort;
            objExportEntitiesReport.objReportFilters.sortdir = sortdir;
            objExportEntitiesReport.objReportFilters.userid = Convert.ToInt32(Session["user_id"]);
            if (!string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.sort))
            {
                var result = new BLMisc().GetVsatColumnNameByDisplayName(objExportEntitiesReport.objReportFilters.sort);
                objExportEntitiesReport.objReportFilters.sort = result;
            }
            //if (objExportEntitiesReport.objReportFilters.sort=="Can_Id")
            //{
            //    objExportEntitiesReport.objReportFilters.sort = "network_id";

            //}
            if (!string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.DurationBasedColumnName) || !string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.geom))
            {
                List<Dictionary<string, string>> lstExportEntitiesDetail = new BLMisc().getVSATReport(Convert.ToInt32(Session["user_id"]), objExportEntitiesReport.objReportFilters);
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "SYSTEM_ID", "PARENT_SYSTEM_ID", "PARENT_ENTITY_TYPE", "REGION ID", "PROVINCE ID", "CREATED_BY_ID" };
                foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                {
                    var obj = (IDictionary<string, object>)new ExpandoObject();

                    foreach (var col in dic)
                    {
                        obj.Add(col.Key, col.Value);
                    }
                    objExportEntitiesReport.lstReportData.Add(obj);
                }
                objExportEntitiesReport.lstReportData = BLConvertMLanguage.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                objExportEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
            }
            Session["ExportVSATReportFilter"] = objExportEntitiesReport.objReportFilters;
            BindVSATReportDropdown(ref objExportEntitiesReport);
            return PartialView("_ExportVSATReport", objExportEntitiesReport);
        }

        public void BindVSATReportDropdown(ref ExportVSATEntitiesReport objExportEntitiesReport)
        {
            //Bind Regions..
            objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }
        }
        public JsonResult BindVSATSearchBy(string entity_type, string lmc_type)
        {
            var objResp = new BLLayer().GetLMCReportSearchByColumnName(entity_type, lmc_type);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        public void DownloadVSATReport(string fileType, string reportType, string entityType)
        {
            if (!string.IsNullOrWhiteSpace(fileType))
            {

                if (reportType.ToUpper() == "ALL" && fileType.ToUpper() == "EXCEL")
                {
                    DownloadVSATReportIntoExcel(reportType, entityType);
                }

            }

        }
        public void DownloadVSATReportIntoExcel(string reportType, string entityType)
        {

            if (Session["ExportVSATReportFilter"] != null)
            {

                try
                {

                    ExportVSATReportFilter objReportFilter = (ExportVSATReportFilter)Session["ExportVSATReportFilter"];
                    objReportFilter.currentPage = 0;
                    //Filter the Layer Detail
                    //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objReportFilter.layerName.ToUpper()).FirstOrDefault();

                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLMisc().getVSATReport(Convert.ToInt32(Session["user_id"]), objReportFilter);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    // dtReport.TableName = layerDetail.layer_title;
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                        if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        if (dtReport.Columns.Contains("CREATED_ON")) { dtReport.Columns.Remove("CREATED_ON"); }
                        if (dtReport.Columns.Contains("system Id")) { dtReport.Columns.Remove("system Id"); }
                        if (dtReport.Columns.Contains("PARENT_SYSTEM_ID")) { dtReport.Columns.Remove("PARENT_SYSTEM_ID"); }
                        if (dtReport.Columns.Contains("PARENT_ENTITY_TYPE")) { dtReport.Columns.Remove("PARENT_ENTITY_TYPE"); }
                    }


                    if (dtReport.Rows.Count > 0)
                    {
                        ExportData(dtReport, entityType + "_Report_" + MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public ActionResult ValidateVSATPotentialArea(string geom, string geomType, double buff_Radius = 0.0)
        {
            JsonResponse<string> objBom = new JsonResponse<string>();
            var objValid = new BLMisc().ValidateVSATPotentialArea(geom, Convert.ToInt32(Session["user_id"]), geomType, buff_Radius);
            if (objValid.status == false)
            {
                objBom.status = ResponseStatus.FAILED.ToString();
                objBom.message = BLConvertMLanguage.MultilingualMessageConvert(objValid.message);//objValid.message;
            }
            else
            {
                objBom.status = "SUCCESS";
            }
            return Json(objBom, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region UtilizationReport
        public ActionResult UtilizationExportReport(UtilizationReport objUtilizationReport)
        {
            var userdetails = (User)Session["userDetail"];

            objUtilizationReport.objReportFilters.SelectedRegionIds = objUtilizationReport.objReportFilters.SelectedRegionId != null && objUtilizationReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objUtilizationReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objUtilizationReport.objReportFilters.SelectedProvinceIds = objUtilizationReport.objReportFilters.SelectedProvinceId != null && objUtilizationReport.objReportFilters.SelectedProvinceId.Count > 0 ? string.Join(",", objUtilizationReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objUtilizationReport.objReportFilters.SelectedNetworkStatues = objUtilizationReport.objReportFilters.SelectedNetworkStatus != null && objUtilizationReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objUtilizationReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
            objUtilizationReport.objReportFilters.SelectedLayerIds = objUtilizationReport.objReportFilters.SelectedLayerId != null && objUtilizationReport.objReportFilters.SelectedLayerId.Count > 0 ? string.Join(",", objUtilizationReport.objReportFilters.SelectedLayerId.ToArray()) : "";
            objUtilizationReport.objReportFilters.SelectedProjectIds = objUtilizationReport.objReportFilters.SelectedProjectId != null && objUtilizationReport.objReportFilters.SelectedProjectId.Count > 0 ? string.Join(",", objUtilizationReport.objReportFilters.SelectedProjectId.ToArray()) : "";
            objUtilizationReport.objReportFilters.SelectedPlanningIds = objUtilizationReport.objReportFilters.SelectedPlanningId != null && objUtilizationReport.objReportFilters.SelectedPlanningId.Count > 0 ? string.Join(",", objUtilizationReport.objReportFilters.SelectedPlanningId.ToArray()) : "";
            objUtilizationReport.objReportFilters.SelectedWorkOrderIds = objUtilizationReport.objReportFilters.SelectedWorkOrderId != null && objUtilizationReport.objReportFilters.SelectedWorkOrderId.Count > 0 ? string.Join(",", objUtilizationReport.objReportFilters.SelectedWorkOrderId.ToArray()) : "";
            objUtilizationReport.objReportFilters.SelectedPurposeIds = objUtilizationReport.objReportFilters.SelectedPurposeId != null && objUtilizationReport.objReportFilters.SelectedPurposeId.Count > 0 ? string.Join(",", objUtilizationReport.objReportFilters.SelectedPurposeId.ToArray()) : "";
            objUtilizationReport.objReportFilters.userId = Convert.ToInt32(userdetails.user_id);
            objUtilizationReport.objReportFilters.roleId = Convert.ToInt32(userdetails.role_id);
            BindUtilizationDropdown(ref objUtilizationReport);
            if (objUtilizationReport.isGetData)
            {
                objUtilizationReport.lstReportData = new BLLayer().GetUtilizationReportSummary(objUtilizationReport.objReportFilters).ToList();
                Session["UtilizationReportFilter"] = objUtilizationReport.objReportFilters;
                Session["UtilizationSummaryData"] = objUtilizationReport;
                objUtilizationReport.isGetData = false;
            }
            return PartialView("_UtilizationExportReport", objUtilizationReport);
        }
        public void BindUtilizationDropdown(ref UtilizationReport objUtilizationReport)
        {
            var userdetails = (User)Session["userDetail"];
            //Bind Layers..
            objUtilizationReport.lstLayers = new BLLayer().GetReportLayers(Convert.ToInt32(userdetails.role_id), "UTILIZATION");
            //Bind Regions..
            objUtilizationReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objUtilizationReport.objReportFilters.SelectedRegionIds))
            {
                objUtilizationReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objUtilizationReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }
            //for project code,planning code,workordercode & purpose code
            objUtilizationReport.lstBindProjectCode = new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrWhiteSpace(objUtilizationReport.objReportFilters.SelectedNetworkStatues) ? "P" : objUtilizationReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "PLANNED" ? "P" : objUtilizationReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "AS BUILT" ? "A" : objUtilizationReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "DORMANT" ? "D" : "P");
            if (objUtilizationReport.objReportFilters.SelectedProjectId != null)
                objUtilizationReport.lstBindPlanningCode = new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(objUtilizationReport.objReportFilters.SelectedProjectId);
            if (objUtilizationReport.objReportFilters.SelectedPlanningId != null)
                objUtilizationReport.lstBindWorkorderCode = new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(objUtilizationReport.objReportFilters.SelectedPlanningId);
            if (objUtilizationReport.objReportFilters.SelectedWorkOrderId != null)
                objUtilizationReport.lstBindPurposeCode = new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(objUtilizationReport.objReportFilters.SelectedWorkOrderId);
        }
        public void DownloadUtilizationReport(string fileType, string summaryids)
        {
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
                if (fileType.ToUpper() == "EXCEL")
                {
                    DownloadUtilizationReportIntoExcel(summaryids, fileType);
                }
                else if (fileType.ToUpper() == "PDF")
                {
                    DownloadUtilizationReportIntoPDF(summaryids);
                }
                else if (fileType.ToUpper() == "ALLEXCEL")
                {
                    DownloadUtilizationReportIntoExcelAll(summaryids);
                }
                else if (fileType.ToUpper() == "KML" || fileType.ToUpper() == "XML")
                {
                    DownloadUtilizationSummaryIntoKMLAll(summaryids, fileType.ToUpper());
                }

                else if (fileType.ToUpper() == "ALLSHAPE")
                {
                    DownloadUtilizationReportIntoShapeAll(summaryids);
                }
                else if (fileType.ToUpper() == "ALLCSV" || fileType.ToUpper() == "ALLTXT")
                {
                    DownloadUtilizationReportIntoCSVAll(summaryids, fileType);
                }
            }

        }
        public DataTable GetUtilizationReportFilter(object obj)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            DataTable dt = new DataTable(Resources.Resources.SI_GBL_GBL_NET_FRM_165);
            dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_166);
            dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_167);
            DataRow dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_065; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_066; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_063; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_GBL_GBL_NET_FRM_098; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_074; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_076; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_010; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_011; dt.Rows.Add(dr); dr = dt.NewRow();

            var userdetails = (User)Session["userDetail"];
            List<int> regionIds = (List<int>)obj.GetType().GetProperty("SelectedRegionId").GetValue(obj, null);
            var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());

            List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
            var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { userId = Convert.ToInt32(Session["user_id"]) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());

            string networkStatus = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedNetworkStatues").GetValue(obj, null).ToString().Replace("AS BUILT", "AS-BUILT").ToLower()).Replace("'", "");

            //rt
            //var userdetails = (User)Session["userDetail"];
            List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
            var layerName = layerIds == null ? "All" : string.Join(",", new BLLayer().GetReportLayers(userdetails.role_id, "UTILIZATION").Where(x => layerIds.Contains(x.layer_id)).OrderBy(x => x.layer_title).Select(x => x.layer_title).ToList());

            List<int> projectCodeIds = (List<int>)obj.GetType().GetProperty("SelectedProjectId").GetValue(obj, null);
            var projectCodeName = projectCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrEmpty(networkStatus) ? "P" : networkStatus.ToUpper() == "PLANNED" ? "P" : networkStatus.ToUpper() == "AS BUILT" ? "A" : networkStatus.ToUpper() == "DORMANT" ? "D" : "P").Where(x => projectCodeIds.Contains(x.system_id)).Select(x => x.project_code).ToList());

            List<int> planningCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPlanningId").GetValue(obj, null);
            var planningCodeName = planningCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(projectCodeIds).Where(x => planningCodeIds.Contains(x.system_id)).Select(x => x.planning_code).ToList());

            List<int> workOrderCodeIds = (List<int>)obj.GetType().GetProperty("SelectedWorkOrderId").GetValue(obj, null);
            var workOrderCodeName = workOrderCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(planningCodeIds).Where(x => workOrderCodeIds.Contains(x.system_id)).Select(x => x.workorder_code).ToList());

            List<int> purposeCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPurposeId").GetValue(obj, null);
            var purposeCodeName = purposeCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(workOrderCodeIds).Where(x => purposeCodeIds.Contains(x.system_id)).Select(x => x.purpose_code).ToList());

            dt.Rows[0][1] = regionName;
            dt.Rows[1][1] = provinceName;
            dt.Rows[2][1] = String.IsNullOrEmpty(networkStatus) ? "All" : networkStatus;
            dt.Rows[3][1] = layerName;
            dt.Rows[4][1] = projectCodeName;
            dt.Rows[5][1] = planningCodeName;
            dt.Rows[6][1] = workOrderCodeName;
            dt.Rows[7][1] = purposeCodeName;
            return dt;
        }
        public DataTable GetUtilizationModifiedTable(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Network_Status"] = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(dt.Rows[i]["Network_Status"].ToString().ToLower().Contains("built") ? dt.Rows[i]["Network_Status"].ToString().ToLower().Replace(" ", "-") : dt.Rows[i]["Network_Status"].ToString().ToLower());
            }
            return dt;
        }
        public DataSet GetSummaryExcelPDFData(string summaryids, string filetype = "")
        {
            DataSet ds = new DataSet();
            if (Session["UtilizationSummaryData"] != null)
            {
                try
                {
                    UtilizationReport objExportEntitiesReport = new UtilizationReport();

                    objExportEntitiesReport.objReportFilters = (UtilizationFilter)Session["UtilizationReportFilter"];// for filter
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedSummaryId = new List<int>();
                    if (!String.IsNullOrEmpty(summaryids))
                        SelectedSummaryId = summaryids.Split(',').Select(int.Parse).ToList();
                    //objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;


                    objExportEntitiesReport = (UtilizationReport)Session["UtilizationSummaryData"];
                    List<UtilizationSummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                    if (SelectedSummaryId.Count > 0)
                    {
                        objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => SelectedSummaryId.Contains(x.summary_id)).ToList();
                        objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportEntitiesReport.lstReportData.Select(x => x.entity_id).Distinct().ToList();
                    }
                    DataTable dtFilter = GetUtilizationReportFilter(objExportEntitiesReport.objReportFilters);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);
                    dtReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportEntitiesReport.lstReportData = lstRprtData;

                    ds.Tables.Add(dtFilter);
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        dtReport = GetUtilizationModifiedTable(dtReport);
                        if (!String.IsNullOrEmpty(filetype))
                        {
                            dtReport.Columns.Remove("utilization_text");
                        }
                        dtReport.Columns.Remove("summary_id");
                        dtReport.Columns.Remove("entity_id");
                        dtReport.Columns.Remove("entity_name");
                        dtReport.Columns.Remove("region_id");
                        dtReport.Columns.Remove("province_id");
                        dtReport.Columns["network_status"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_067;
                        dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                        dtReport.Columns["region"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_065;
                        dtReport.Columns["province"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_066;

                        string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();

                        DataRow row = dtReport.NewRow();
                        row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = Resources.Resources.SI_OSP_GBL_GBL_GBL_041;
                        if (dtReport.Columns.Contains("low_count"))
                        {
                            row["low_count"] = dtReport.Compute("Sum(low_count)", "");
                        }
                        if (dtReport.Columns.Contains("moderate_count"))
                        {
                            row["moderate_count"] = dtReport.Compute("Sum([moderate_count])", "");
                        }
                        if (dtReport.Columns.Contains("high_count"))
                        {
                            row["high_count"] = dtReport.Compute("Sum(high_count)", "");
                        }
                        if (dtReport.Columns.Contains("over_count"))
                        {
                            row["over_count"] = dtReport.Compute("Sum(over_count)", "");
                        }

                        dtReport.Columns["low_count"].ColumnName = Resources.Resources.SI_GBL_GBL_GBL_GBL_107;
                        dtReport.Columns["moderate_count"].ColumnName = Resources.Resources.SI_GBL_GBL_GBL_GBL_108;
                        dtReport.Columns["high_count"].ColumnName = Resources.Resources.SI_GBL_GBL_GBL_GBL_109;
                        dtReport.Columns["over_count"].ColumnName = Resources.Resources.SI_GBL_GBL_GBL_GBL_110;

                        dtReport.Rows.Add(row);
                        ds.Tables.Add(dtReport);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ds;
        }
        public void DownloadUtilizationReportIntoExcel(string summaryids, string filetype)
        {
            if (Session["UtilizationSummaryData"] != null)
            {
                try
                {
                    DataSet ds = GetSummaryExcelPDFData(summaryids, filetype);
                    ExportData(ds, "UtilizationSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadUtilizationReportIntoPDF(string summaryids)
        {
            if (Session["UtilizationSummaryData"] != null)
            {
                try
                {
                    DataSet ds = GetSummaryExcelPDFData(summaryids, "");
                    GenerateToPDF(ds, "UtilizationSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"), Resources.Resources.SI_OSP_GBL_NET_GBL_116);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public string GetUtilizationAdvFilter(List<UtilizationSummaryReport> entityData)
        {
            string result = string.Empty;
            var lstSubQuery = new List<string>();
            var statusList = entityData.Select(x => x.network_status).Distinct().ToList();
            for (int i = 0; i < statusList.Count; i++)
            {
                var strSubQuery = "(upper(a.network_status)='" + statusList[i].ToUpper() + "";
                var lstProvinceIds = entityData.Where(x => x.network_status == statusList[i]).Select(m => m.province_id).ToList();
                if (lstProvinceIds.Count > 0)
                {
                    strSubQuery += "' and a.province_id in (" + string.Join(",", lstProvinceIds.ToArray()) + ")";
                }
                strSubQuery += ")";
                lstSubQuery.Add(strSubQuery);
            }
            if (lstSubQuery.Count > 0)
            {
                result = " and " + string.Join(" or ", lstSubQuery.ToArray());
            }
            return result;
        }
        public void DownloadUtilizationReportIntoExcelAll(string summaryids)
        {

            if (Session["UtilizationReportFilter"] != null)
            {
                try
                {
                    UtilizationReport objUtilizationSummary = new UtilizationReport();

                    objUtilizationSummary = (UtilizationReport)Session["UtilizationSummaryData"];

                    UtilizationEntitiesSummaryView objUtilizationEntitiesReport = new UtilizationEntitiesSummaryView();

                    UtilizationFilter objUtilizationFilter = new UtilizationFilter();

                    objUtilizationFilter = (UtilizationFilter)Session["UtilizationReportFilter"];

                    objUtilizationEntitiesReport.objReportFilters.SelectedRegionIds = objUtilizationFilter.SelectedRegionIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds = objUtilizationFilter.SelectedProvinceIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues = objUtilizationFilter.SelectedNetworkStatues;
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = objUtilizationFilter.SelectedLayerId;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProjectIds = objUtilizationFilter.SelectedProjectIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPlanningIds = objUtilizationFilter.SelectedPlanningIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objUtilizationFilter.SelectedWorkOrderIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPurposeIds = objUtilizationFilter.SelectedPurposeIds;
                    objUtilizationEntitiesReport.objReportFilters.geom = objUtilizationFilter.geom;
                    objUtilizationEntitiesReport.objReportFilters.userId = objUtilizationFilter.userId;
                    objUtilizationEntitiesReport.objReportFilters.roleId = objUtilizationFilter.roleId;

                    objUtilizationEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objUtilizationFilter.SelectedLayerId;
                    List<UtilizationSummaryReport> LstReportDataOld = objUtilizationSummary.lstReportData;
                    objUtilizationSummary.lstReportData = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Where(x => (summaryids.Split(',').Select(int.Parse).ToList()).Contains(x.summary_id)).ToList() : objUtilizationSummary.lstReportData.ToList();

                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;

                    //objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (Filterobj.Count >0) ? Filterobj.Select(x=> Convert.ToInt32(x.id)).ToList(): objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    //objUtilizationFilter.SelectedLayerId = (Filterobj.Count > 0) ? Filterobj.Select(x => Convert.ToInt32(x.id)).ToList() : objUtilizationFilter.SelectedLayerId;

                    DataTable dtFilter = GetUtilizationReportFilter(objUtilizationFilter);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objUtilizationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "UTILIZATION");//GetLayerDetailsForReport().Where(x => x.is_utilization_enabled == true).ToList();
                    var selectedlayerids = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objUtilizationEntitiesReport.lstLayers = objUtilizationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    for (int i = 0; i < objUtilizationEntitiesReport.lstLayers.Count; i++)
                    {
                        objUtilizationEntitiesReport.objReportFilters.layerName = objUtilizationEntitiesReport.lstLayers[i].layer_name;
                        string advFilter = string.Empty;
                        if (!String.IsNullOrEmpty(summaryids))
                        {
                            advFilter = GetUtilizationAdvFilter(objUtilizationSummary.lstReportData.Where(x => x.entity_id == objUtilizationEntitiesReport.lstLayers[i].layer_id).ToList());
                        }
                        objUtilizationEntitiesReport.objReportFilters.advancefilter = (!string.IsNullOrEmpty(advFilter)) ? advFilter : objUtilizationEntitiesReport.objReportFilters.advancefilter;
                        //objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues = (Filterobj.Count > 0) ? Filterobj.Where(x => Convert.ToInt32(x.id) == objUtilizationEntitiesReport.lstLayers[i].layer_id).Select(x => x.data.network_status).ToList()[0].ToString() : objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues;
                        //objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds = (Filterobj.Count > 0) ? Filterobj.Where(x => Convert.ToInt32(x.id) == objUtilizationEntitiesReport.lstLayers[i].layer_id).Select(x => x.data.province_id).ToList()[0].ToString() : objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds;
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objUtilizationEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstUtilizationEntitiesDetail = new BLLayer().GetUtilizationReportSummaryView(objUtilizationEntitiesReport.objReportFilters);
                        //lstUtilizationEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstUtilizationEntitiesDetail);

                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstUtilizationEntitiesDetail);
                        dtReport.TableName = layerDetail.layer_title;
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        }
                        if (dtReport.Rows.Count > 0)
                            ds.Tables.Add(dtReport);
                    }
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = SelectedLayerIdSummary;
                    objUtilizationSummary.lstReportData = LstReportDataOld;
                    ExportData(ds, "UtilizationReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss"));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadUtilizationSummaryIntoKMLAll(string summaryids, string fileType)
        {
            if (Session["UtilizationReportFilter"] != null)
            {
                StringBuilder sbLine = new StringBuilder();
                StringBuilder sbPoint = new StringBuilder();
                StringBuilder sbPolygon = new StringBuilder();
                sbLine.Append("<Folder><name>Line</name>");
                sbPoint.Append("<Folder><name>Point</name>");
                sbPolygon.Append("<Folder><name>Polygon</name>");
                try
                {
                    string iconpath = string.Empty;
                    string cablecolor = string.Empty;
                    string polycolor = string.Empty;
                    int lineWidth = 2;
                    UtilizationReport objUtilizationSummary = new UtilizationReport();
                    objUtilizationSummary = (UtilizationReport)Session["UtilizationSummaryData"];

                    UtilizationEntitiesSummaryView objUtilizationEntitiesReport = new UtilizationEntitiesSummaryView();
                    UtilizationFilter objUtilizationFilter = new UtilizationFilter();
                    objUtilizationFilter = (UtilizationFilter)Session["UtilizationReportFilter"];

                    objUtilizationEntitiesReport.objReportFilters.SelectedRegionIds = objUtilizationFilter.SelectedRegionIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds = objUtilizationFilter.SelectedProvinceIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues = objUtilizationFilter.SelectedNetworkStatues;
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = objUtilizationFilter.SelectedLayerId;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProjectIds = objUtilizationFilter.SelectedProjectIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPlanningIds = objUtilizationFilter.SelectedPlanningIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objUtilizationFilter.SelectedWorkOrderIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPurposeIds = objUtilizationFilter.SelectedPurposeIds;
                    objUtilizationEntitiesReport.objReportFilters.geom = objUtilizationFilter.geom;
                    objUtilizationEntitiesReport.objReportFilters.currentPage = 0;
                    objUtilizationEntitiesReport.objReportFilters.fileType = "KML";
                    objUtilizationEntitiesReport.objReportFilters.userId = objUtilizationFilter.userId;
                    objUtilizationEntitiesReport.objReportFilters.roleId = objUtilizationFilter.roleId;

                    List<int> SelectedLayerId = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objUtilizationFilter.SelectedLayerId;

                    objUtilizationSummary.lstReportData = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Where(x => (summaryids.Split(',').Select(int.Parse).ToList()).Contains(x.summary_id)).ToList() : objUtilizationSummary.lstReportData.ToList();

                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;

                    //objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (Filterobj.Count > 0) ? Filterobj.Select(x => Convert.ToInt32(x.id)).ToList() : objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    //objUtilizationFilter.SelectedLayerId = (Filterobj.Count > 0) ? Filterobj.Select(x => Convert.ToInt32(x.id)).ToList() : objUtilizationFilter.SelectedLayerId;

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objUtilizationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "UTILIZATION");
                    var selectedlayerids = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objUtilizationEntitiesReport.lstLayers = objUtilizationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataTable dtFilter = GetUtilizationReportFilter(objUtilizationFilter);
                    // PREPARE POINT ENTITY LEGEND..
                    string desFolderPath = string.Empty;
                    string tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
                    string desTempFolderPath = Server.MapPath(ApplicationSettings.DownloadTempPath) + tempFolderName;
                    if (Directory.Exists(desTempFolderPath).Equals(false))
                        Directory.CreateDirectory(desTempFolderPath);


                    for (int i = 0; i < objUtilizationEntitiesReport.lstLayers.Count; i++)
                    {
                        string advFilter = string.Empty;
                        if (!String.IsNullOrEmpty(summaryids))
                        {
                            advFilter = GetUtilizationAdvFilter(objUtilizationSummary.lstReportData.Where(x => x.entity_id == objUtilizationEntitiesReport.lstLayers[i].layer_id).ToList());
                        }
                        objUtilizationEntitiesReport.objReportFilters.advancefilter = (!string.IsNullOrEmpty(advFilter)) ? advFilter : objUtilizationEntitiesReport.objReportFilters.advancefilter;

                        // "http://localhost:49911/Content/images/icons/lib/circle/" 
                        // ApplicationSettings.KMLIconURL.ToString()

                        //if (!System.IO.File.Exists(Server.MapPath("~/" + new Uri(iconpath).AbsolutePath)))
                        //{
                        //    iconpath = ApplicationSettings.KMLIconURL.ToString() + "DEFAULT.png";
                        //}
                        objUtilizationEntitiesReport.objReportFilters.layerName = objUtilizationEntitiesReport.lstLayers[i].layer_name;
                        List<Dictionary<string, string>> lstUtilizationEntitiesDetail = new BLLayer().GetUtilizationSummaryViewKMLShape(objUtilizationEntitiesReport.objReportFilters);
                        #region ANTRA
                        foreach (Dictionary<string, string> dic in lstUtilizationEntitiesDetail)
                        {
                            var obj = (IDictionary<string, object>)new ExpandoObject();
                            string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE", "GEOM_TYPE", "ENTITY_TITLE", "ENTITY_NAME", "GEOM", "UTILIZATION" };
                            string[] arrIgnoreColumnskml = { "TOTALRECORDS", "S_NO", "BARCODE" };
                            objUtilizationEntitiesReport.lstReportData = new List<dynamic>();
                            StringBuilder description = new StringBuilder();
                            var Isutilization = 0;
                            foreach (var col in dic)
                            {
                                if (col.Key.ToUpper() == "UTILIZATION")
                                {
                                    Isutilization = 1;
                                }
                                if (!Array.Exists(arrIgnoreColumnskml, m => m == col.Key.ToUpper()))
                                {
                                    obj.Add(col.Key.ToUpper() == "NETWORK ID" ? "network_id" : (col.Key.ToUpper() == "CABLE TYPE" ? "cable_type" : col.Key), col.Value);
                                }
                                if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                                {
                                    description.Append("<SimpleData name='" + col.Key + "'>" + (String.IsNullOrEmpty(col.Value) ? col.Value : col.Value.Replace("&", "&amp;")) + "</SimpleData>");
                                }
                            }
                            objUtilizationEntitiesReport.lstReportData.Add(obj);
                            if (objUtilizationEntitiesReport.lstReportData[0].geom_type.ToUpper() != "LINE")
                            {
                                //var pointLayers = objUtilizationEntitiesReport.lstLayers.Where(m => m.geom_type.ToUpper() == "POINT").ToList();
                                var pointLayers = objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString();

                                if (pointLayers != null)
                                {
                                    foreach (var lyr in pointLayers)
                                    {
                                        var strIconPath = string.Concat(Server.MapPath(ApplicationSettings.UtilizationIconURL), pointLayers, "_", objUtilizationEntitiesReport.lstReportData[0].Utilization.ToUpper(), ".png");
                                        if (System.IO.File.Exists(strIconPath))
                                        {
                                            var legendFolderPath = desTempFolderPath + "\\Legend\\";
                                            if (Directory.Exists(legendFolderPath).Equals(false))
                                                Directory.CreateDirectory(legendFolderPath);
                                            var layerImageName = Path.GetFileName(strIconPath.ToString());
                                            MiscHelper mh = new MiscHelper();
                                            if (!System.IO.File.Exists(legendFolderPath + layerImageName))
                                            {
                                                mh.CopyFile(Server.MapPath(ApplicationSettings.UtilizationIconURL), legendFolderPath, layerImageName, layerImageName);
                                            }
                                        }

                                    }
                                }

                                if (Isutilization == 1)
                                {
                                    iconpath = string.Concat("Legend\\" + "\\" + objUtilizationEntitiesReport.lstLayers[i].layer_name.ToUpper() + "_" + objUtilizationEntitiesReport.lstReportData[0].Utilization.ToUpper() + ".png");
                                }
                                //else
                                //{
                                //    iconpath = ApplicationSettings.KMLIconURL.ToString() + "DEFAULT.png";
                                //}
                            }
                            if (objUtilizationEntitiesReport.lstReportData[0].geom_type.ToUpper() == "LINE")
                            {
                                if (objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "DUCT" || objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "TRENCH")
                                {
                                    lineWidth = 4;
                                }
                                cablecolor = objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "DUCT" ? "#f1021f8" : objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "TRENCH" ? "#ff0000ff" : "#ff0000ff";

                                if (objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "CABLE")
                                {
                                    if (Isutilization == 1)
                                    {
                                        string utilization = objUtilizationEntitiesReport.lstReportData[0].Utilization.ToUpper();
                                        //cablecolor = utilization == "L" ? "#ff2f80ED" : utilization == "M" ? "#ffF0B500" : utilization == "H" ? "#ffF0B500" : utilization == "O" ? "#ffEB5757" : "#ff2F80ED";
                                        cablecolor = utilization == "L" ? "#ffEC8033" : utilization == "M" ? "#ff4BC8F5" : utilization == "H" ? "#ff4D9AF1" : utilization == "O" ? "#ff5658EC" : "#ffEC8033";
                                    }
                                    else
                                    {
                                        cablecolor = "#ff0000ff";
                                    }
                                }
                                sbLine.Append("<Placemark><name>" + objUtilizationEntitiesReport.lstReportData[0].entity_title + " (" + objUtilizationEntitiesReport.lstReportData[0].network_id + ")" + "</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");//<mode>dashed</mode><dashsize>5</dashsize><alternatecolor>ff000000</alternatecolor>
                                sbLine.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
                                if (!string.IsNullOrEmpty(objUtilizationEntitiesReport.lstReportData[0].geom))
                                {
                                    string t = objUtilizationEntitiesReport.lstReportData[0].geom.Substring(11, objUtilizationEntitiesReport.lstReportData[0].geom.Length - 13);
                                    string[] x = t.Split(',');
                                    foreach (string y in x)
                                    {
                                        sbLine.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                                    }
                                }
                                sbLine.Append("</coordinates></LineString></Placemark>");
                            }
                            if (objUtilizationEntitiesReport.lstReportData[0].geom_type.ToUpper() == "POINT")
                            {
                                if (!string.IsNullOrEmpty(objUtilizationEntitiesReport.lstReportData[0].entity_name))
                                {
                                    //sbPoint.Append("<Placemark><name>" + objUtilizationEntitiesReport.lstReportData[0].entity_title + "(" + objUtilizationEntitiesReport.lstReportData[0].network_id + " [" + objUtilizationEntitiesReport.lstReportData[0].entity_name + "]" + ")" + "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                                    sbPoint.Append("<Placemark><name>" + objUtilizationEntitiesReport.lstReportData[0].entity_title + " (" + objUtilizationEntitiesReport.lstReportData[0].network_id + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");// to reduce size of icon put <scale>.5</scale> under icon tab 1 for actual size and anything less than 1 (0.1 - 0.9) - reduces the size of icon
                                }
                                else
                                {
                                    sbPoint.Append("<Placemark><name>" + objUtilizationEntitiesReport.lstReportData[0].entity_title + " (" + objUtilizationEntitiesReport.lstReportData[0].network_id + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                                }
                                sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                                if (!string.IsNullOrEmpty(objUtilizationEntitiesReport.lstReportData[0].geom))
                                {
                                    string t = objUtilizationEntitiesReport.lstReportData[0].geom.Substring(6, objUtilizationEntitiesReport.lstReportData[0].geom.Length - 8);
                                    sbPoint.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                                }
                                sbPoint.Append("</coordinates></Point></Placemark>");

                            }
                            if (objUtilizationEntitiesReport.lstReportData[0].geom_type.ToUpper() == "POLYGON")
                            {
                                //polycolor = objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "AREA" ? "ffe8dab4" : objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "SUBAREA" ? "ff71e9d7" : "#ff0000ff";
                                polycolor = objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "AREA" ? "7fe8dab4" : objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() == "SUBAREA" ? "7f71e9d7" : "#ff0000ff";
                                sbPolygon.Append("<Style id=\"transGreenPoly" + objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() + "\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>" + polycolor + "</color></PolyStyle></Style><Placemark><name>" + objUtilizationEntitiesReport.lstReportData[0].entity_title + " (" + objUtilizationEntitiesReport.lstReportData[0].network_id + ")" + "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + objUtilizationEntitiesReport.lstReportData[0].entity_title.ToString().ToUpper() + "</styleUrl>");
                                sbPolygon.Append("<Polygon><outerBoundaryIs><LinearRing><coordinates>");
                                if (!string.IsNullOrEmpty(objUtilizationEntitiesReport.lstReportData[0].geom))
                                {
                                    string t = objUtilizationEntitiesReport.lstReportData[0].geom.Substring(9, objUtilizationEntitiesReport.lstReportData[0].geom.Length - 12);
                                    string[] x = t.Split(',');
                                    foreach (string y in x)
                                    {
                                        sbPolygon.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                                    }
                                }
                                sbPolygon.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
                            }
                        }
                    }
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = SelectedLayerIdSummary;
                    sbLine.Append("</Folder>");
                    sbPoint.Append("</Folder>");
                    sbPolygon.Append("</Folder>");
                    StringBuilder filterValues = new StringBuilder();
                    filterValues.Append("<table><tr><th>Filter Type</th><th>Value</th></tr>");
                    filterValues.AppendLine();
                    for (int i = 0; i < dtFilter.Rows.Count; i++)
                    {
                        filterValues.Append("<tr><td>" + dtFilter.Rows[i][0] + "</td><td>" + dtFilter.Rows[i][1] + "</td></tr>");
                        filterValues.AppendLine();
                    }
                    filterValues.Append("</table>");

                    string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                 "<filter>" + filterValues + "</filter>" +
                                "<Document>  <!-- Begin Style Definitions -->" +
                                //"<Style id=\"cable\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                //"<Style id=\"transGreenPoly\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>#ff0000ff</color></PolyStyle></Style>" +
                                //"<Style id=\"transGreenPoly\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>f1021f8</color><colorMode>random</colorMode><fill>1</fill><outline>1</outline></PolyStyle></Style>" +
                                //"<Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                sbPolygon.ToString() + sbLine.ToString() + sbPoint.ToString() + "</Document></kml>";
                    string TempkmlFileName = "";
                    if (fileType == "KML") { TempkmlFileName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml"; }
                    else { TempkmlFileName = "XML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".xml"; }

                    string kmlDesFullPath = desTempFolderPath + "\\" + TempkmlFileName;
                    System.IO.File.WriteAllText(kmlDesFullPath, finalKMLString.ToString());
                    string zipfilePath = desTempFolderPath + ".zip";

                    using (var zip = new ZipFile())
                    {
                        //zip.UseZip64WhenSaving = Zip64Option.Always;
                        //zip.CompressionMethod = CompressionMethod.BZip2;
                        zip.AddDirectory(desTempFolderPath);
                        zip.Save(zipfilePath);
                    }
                    if (System.IO.File.Exists(zipfilePath))
                    {
                        string fileName = Path.GetFileName(zipfilePath);
                        Directory.Delete(desTempFolderPath, true);
                    }
                    FileInfo file = new FileInfo(zipfilePath);
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=UtilizationReport_" + fileType + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/zip";
                    Response.WriteFile(file.FullName);
                    Response.Flush();
                    Response.End();
                    System.IO.File.Delete(zipfilePath);
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void DownloadUtilizationReportIntoShapeAll(string summaryids)
        {
            if (Session["UtilizationReportFilter"] != null)
            {
                try
                {
                    UtilizationReport objUtilizationSummary = new UtilizationReport();
                    objUtilizationSummary = (UtilizationReport)Session["UtilizationSummaryData"];

                    UtilizationEntitiesSummaryView objUtilizationEntitiesReport = new UtilizationEntitiesSummaryView();
                    UtilizationFilter objUtilizationFilter = new UtilizationFilter();
                    objUtilizationFilter = (UtilizationFilter)Session["UtilizationReportFilter"];

                    objUtilizationEntitiesReport.objReportFilters.SelectedRegionIds = objUtilizationFilter.SelectedRegionIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds = objUtilizationFilter.SelectedProvinceIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues = objUtilizationFilter.SelectedNetworkStatues;
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = objUtilizationFilter.SelectedLayerId;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProjectIds = objUtilizationFilter.SelectedProjectIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPlanningIds = objUtilizationFilter.SelectedPlanningIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objUtilizationFilter.SelectedWorkOrderIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPurposeIds = objUtilizationFilter.SelectedPurposeIds;
                    objUtilizationEntitiesReport.objReportFilters.geom = objUtilizationFilter.geom;
                    objUtilizationEntitiesReport.objReportFilters.currentPage = 0;
                    objUtilizationEntitiesReport.objReportFilters.fileType = "SHAPE";
                    objUtilizationEntitiesReport.objReportFilters.userId = objUtilizationFilter.userId;
                    objUtilizationEntitiesReport.objReportFilters.roleId = objUtilizationFilter.roleId;

                    List<int> SelectedLayerId = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objUtilizationFilter.SelectedLayerId;
                    objUtilizationSummary.lstReportData = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Where(x => (summaryids.Split(',').Select(int.Parse).ToList()).Contains(x.summary_id)).ToList() : objUtilizationSummary.lstReportData.ToList();

                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;

                    //objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (Filterobj.Count > 0) ? Filterobj.Select(x => Convert.ToInt32(x.id)).ToList() : objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    //objUtilizationFilter.SelectedLayerId = (Filterobj.Count > 0) ? Filterobj.Select(x => Convert.ToInt32(x.id)).ToList() : objUtilizationFilter.SelectedLayerId;

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objUtilizationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "UTILIZATION").Where(x => x.is_utilization_enabled == true).ToList();
                    var selectedlayerids = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objUtilizationEntitiesReport.lstLayers = objUtilizationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    for (int i = 0; i < objUtilizationEntitiesReport.lstLayers.Count; i++)
                    {
                        objUtilizationEntitiesReport.objReportFilters.layerName = objUtilizationEntitiesReport.lstLayers[i].layer_name;
                        string advFilter = string.Empty;
                        if (!String.IsNullOrEmpty(summaryids))
                        {
                            advFilter = GetUtilizationAdvFilter(objUtilizationSummary.lstReportData.Where(x => x.entity_id == objUtilizationEntitiesReport.lstLayers[i].layer_id).ToList());
                        }
                        objUtilizationEntitiesReport.objReportFilters.advancefilter = (!string.IsNullOrEmpty(advFilter)) ? advFilter : objUtilizationEntitiesReport.objReportFilters.advancefilter;
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objUtilizationEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstUtilizationEntitiesDetail = new BLLayer().GetUtilizationSummaryViewKMLShape(objUtilizationEntitiesReport.objReportFilters);
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstUtilizationEntitiesDetail);
                        dtReport.TableName = layerDetail.layer_title;
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        }
                        if (dtReport.Rows.Count > 0)
                            ds.Tables.Add(dtReport);
                    }
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = SelectedLayerIdSummary;
                    GetShapeFile(ds, "UtilizationReport");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public JsonResult ShowUtilizationOnMap(string summaryids, string utilizationType, string ductutilization)
        {
            string lstUtilizationEntitiesDetail = string.Empty;
            if (Session["UtilizationReportFilter"] != null)
            {
                try
                {
                    UtilizationReport objUtilizationSummary = new UtilizationReport();
                    objUtilizationSummary = (UtilizationReport)Session["UtilizationSummaryData"];

                    UtilizationEntitiesSummaryView objUtilizationEntitiesReport = new UtilizationEntitiesSummaryView();
                    UtilizationFilter objUtilizationFilter = new UtilizationFilter();
                    objUtilizationFilter = (UtilizationFilter)Session["UtilizationReportFilter"];

                    objUtilizationEntitiesReport.objReportFilters.SelectedRegionIds = objUtilizationFilter.SelectedRegionIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds = objUtilizationFilter.SelectedProvinceIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues = objUtilizationFilter.SelectedNetworkStatues;
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = objUtilizationFilter.SelectedLayerId;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProjectIds = objUtilizationFilter.SelectedProjectIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPlanningIds = objUtilizationFilter.SelectedPlanningIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objUtilizationFilter.SelectedWorkOrderIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPurposeIds = objUtilizationFilter.SelectedPurposeIds;
                    objUtilizationEntitiesReport.objReportFilters.geom = objUtilizationFilter.geom;
                    objUtilizationEntitiesReport.objReportFilters.currentPage = 0;
                    objUtilizationEntitiesReport.objReportFilters.utilizationType = utilizationType;
                    objUtilizationEntitiesReport.objReportFilters.ductutilization = ductutilization;

                    List<int> SelectedLayerId = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objUtilizationFilter.SelectedLayerId;
                    var utilizationSummaryData = objUtilizationSummary.lstReportData;
                    objUtilizationSummary.lstReportData = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Where(x => (summaryids.Split(',').Select(int.Parse).ToList()).Contains(x.summary_id)).ToList() : objUtilizationSummary.lstReportData.ToList();

                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objUtilizationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "UTILIZATION");
                    var selectedlayerids = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objUtilizationEntitiesReport.lstLayers = objUtilizationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    for (int i = 0; i < objUtilizationEntitiesReport.lstLayers.Count; i++)
                    {
                        objUtilizationEntitiesReport.objReportFilters.layerName = objUtilizationEntitiesReport.lstLayers[i].layer_name;
                        string advFilter = string.Empty;
                        if (!String.IsNullOrEmpty(summaryids))
                        {
                            advFilter = GetUtilizationAdvFilter(objUtilizationSummary.lstReportData.Where(x => x.entity_id == objUtilizationEntitiesReport.lstLayers[i].layer_id).ToList());
                        }
                        objUtilizationEntitiesReport.objReportFilters.advancefilter = (!string.IsNullOrEmpty(advFilter)) ? advFilter : objUtilizationEntitiesReport.objReportFilters.advancefilter;
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objUtilizationEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        lstUtilizationEntitiesDetail = new BLLayer().ShowUtilizationOnMap(objUtilizationEntitiesReport.objReportFilters);
                    }
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = SelectedLayerIdSummary;
                    objUtilizationSummary.lstReportData = utilizationSummaryData;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            //return Json(new { Data = lstUtilizationEntitiesDetail, JsonRequestBehavior.AllowGet });
            return new JsonResult { Data = lstUtilizationEntitiesDetail, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public void DownloadUtilizationReportIntoCSVAll(string summaryids, string fileType)
        {
            if (Session["UtilizationReportFilter"] != null)
            {
                try
                {
                    UtilizationReport objUtilizationSummary = new UtilizationReport();

                    objUtilizationSummary = (UtilizationReport)Session["UtilizationSummaryData"];

                    UtilizationEntitiesSummaryView objUtilizationEntitiesReport = new UtilizationEntitiesSummaryView();

                    UtilizationFilter objUtilizationFilter = new UtilizationFilter();

                    objUtilizationFilter = (UtilizationFilter)Session["UtilizationReportFilter"];

                    objUtilizationEntitiesReport.objReportFilters.SelectedRegionIds = objUtilizationFilter.SelectedRegionIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds = objUtilizationFilter.SelectedProvinceIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues = objUtilizationFilter.SelectedNetworkStatues;
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = objUtilizationFilter.SelectedLayerId;
                    objUtilizationEntitiesReport.objReportFilters.SelectedProjectIds = objUtilizationFilter.SelectedProjectIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPlanningIds = objUtilizationFilter.SelectedPlanningIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedWorkOrderIds = objUtilizationFilter.SelectedWorkOrderIds;
                    objUtilizationEntitiesReport.objReportFilters.SelectedPurposeIds = objUtilizationFilter.SelectedPurposeIds;
                    objUtilizationEntitiesReport.objReportFilters.geom = objUtilizationFilter.geom;
                    objUtilizationEntitiesReport.objReportFilters.userId = objUtilizationFilter.userId;
                    objUtilizationEntitiesReport.objReportFilters.roleId = objUtilizationFilter.roleId;

                    objUtilizationEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objUtilizationFilter.SelectedLayerId;
                    List<UtilizationSummaryReport> LstReportDataOld = objUtilizationSummary.lstReportData;
                    objUtilizationSummary.lstReportData = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Where(x => (summaryids.Split(',').Select(int.Parse).ToList()).Contains(x.summary_id)).ToList() : objUtilizationSummary.lstReportData.ToList();

                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = (!String.IsNullOrEmpty(summaryids)) ? objUtilizationSummary.lstReportData.Select(x => x.entity_id).ToList() : objUtilizationSummary.objReportFilters.SelectedLayerId;

                    //objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = (Filterobj.Count >0) ? Filterobj.Select(x=> Convert.ToInt32(x.id)).ToList(): objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    //objUtilizationFilter.SelectedLayerId = (Filterobj.Count > 0) ? Filterobj.Select(x => Convert.ToInt32(x.id)).ToList() : objUtilizationFilter.SelectedLayerId;

                    DataTable dtFilter = GetUtilizationReportFilter(objUtilizationFilter);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objUtilizationEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "UTILIZATION");//GetLayerDetailsForReport().Where(x => x.is_utilization_enabled == true).ToList();
                    var selectedlayerids = objUtilizationEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objUtilizationEntitiesReport.lstLayers = objUtilizationEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    for (int i = 0; i < objUtilizationEntitiesReport.lstLayers.Count; i++)
                    {
                        objUtilizationEntitiesReport.objReportFilters.layerName = objUtilizationEntitiesReport.lstLayers[i].layer_name;
                        string advFilter = string.Empty;
                        if (!String.IsNullOrEmpty(summaryids))
                        {
                            advFilter = GetUtilizationAdvFilter(objUtilizationSummary.lstReportData.Where(x => x.entity_id == objUtilizationEntitiesReport.lstLayers[i].layer_id).ToList());
                        }
                        objUtilizationEntitiesReport.objReportFilters.advancefilter = (!string.IsNullOrEmpty(advFilter)) ? advFilter : objUtilizationEntitiesReport.objReportFilters.advancefilter;
                        //objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues = (Filterobj.Count > 0) ? Filterobj.Where(x => Convert.ToInt32(x.id) == objUtilizationEntitiesReport.lstLayers[i].layer_id).Select(x => x.data.network_status).ToList()[0].ToString() : objUtilizationEntitiesReport.objReportFilters.SelectedNetworkStatues;
                        //objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds = (Filterobj.Count > 0) ? Filterobj.Where(x => Convert.ToInt32(x.id) == objUtilizationEntitiesReport.lstLayers[i].layer_id).Select(x => x.data.province_id).ToList()[0].ToString() : objUtilizationEntitiesReport.objReportFilters.SelectedProvinceIds;
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objUtilizationEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstUtilizationEntitiesDetail = new BLLayer().GetUtilizationReportSummaryView(objUtilizationEntitiesReport.objReportFilters);
                        lstUtilizationEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstUtilizationEntitiesDetail);

                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstUtilizationEntitiesDetail);
                        dtReport.TableName = layerDetail.layer_title;
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        }
                        if (dtReport.Rows.Count > 0)
                            ds.Tables.Add(dtReport);
                    }
                    objUtilizationEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objUtilizationFilter.SelectedLayerId = SelectedLayerIdSummary;
                    objUtilizationSummary.lstReportData = LstReportDataOld;
                    ExportReportCSV(ds, "UtilizationReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss"), ApplicationSettings.CsvDelimiter, fileType);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void ExportCSV(DataSet ds, string fileNameValue, string delimiter)
        {
            List<Files> txtDatas = new List<Files>();
            foreach (DataTable dt in ds.Tables)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(Utility.CommonUtility.ConvertDataTableToString(dt, delimiter)); //Convert DataTable to string with specific delimiter
                txtDatas.Add(new Files() { TableName = dt.TableName, Bytes = bytes });
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (Files txtData in txtDatas)
                {
                    zip.AddEntry(txtData.TableName + ".csv", txtData.Bytes);
                }

                Response.Clear();
                Response.BufferOutput = false;
                string zipName = String.Format("{0}_CSV.zip", fileNameValue);
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(Response.OutputStream);
                Response.End();
            }
        }


        public void ExportReportCSVNew(DataSet ds, string fileNameValue, string delimiter, string fileType, string ftpfilePath, string ftpUserName, string ftpPassword)
        {
            List<Files> txtDatas = new List<Files>();
            foreach (DataTable dt in ds.Tables)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(Utility.CommonUtility.ConvertDataTableToString(dt, delimiter)); //Convert DataTable to string with specific delimiter
                txtDatas.Add(new Files() { TableName = dt.TableName, Bytes = bytes });
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (Files txtData in txtDatas)
                {
                    if (fileType == "ALLCSV")
                    {
                        zip.AddEntry(txtData.TableName + ".csv", txtData.Bytes);
                    }
                    else
                    {
                        zip.AddEntry(txtData.TableName + ".txt", txtData.Bytes);
                    }
                }
                //Commented by priyanka
                //string tempfilePath = Path.Combine(Server.MapPath("~/Uploads/Temp/ExportReportLog"), fileNameValue);
                //Commented by priyanka end

                string downloadTempPath = Settings.ApplicationSettings.DownloadTempPath;
                string tempfilePath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"), fileNameValue);

                zip.Save(tempfilePath);
                CommonUtility.FTPFileUpload(tempfilePath, fileNameValue, ftpfilePath, ftpUserName, ftpPassword);

                System.IO.File.Delete(tempfilePath);
                //Response.Clear();
                //Response.BufferOutput = false;
                //if (fileType == "ALLCSV")
                //{
                //    string zipName = String.Format("{0}_CSV.zip", fileNameValue);
                //    Response.ContentType = "application/zip";
                //    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                //    zip.Save(Response.OutputStream);
                //    Response.End();
                //}
                //else
                //{
                //    string zipName = String.Format("{0}.zip", fileNameValue);
                //    Response.ContentType = "application/zip";
                //    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                //    zip.Save(Response.OutputStream);
                //    Response.End();
                //}

            }
        }

        public void ExportReportCSV(DataSet ds, string fileNameValue, string delimiter, string fileType)
        {
            List<Files> txtDatas = new List<Files>();
            foreach (DataTable dt in ds.Tables)
            {

                byte[] bytes = Encoding.ASCII.GetBytes(Utility.CommonUtility.ConvertDataTableToString(dt, delimiter)); //Convert DataTable to string with specific delimiter
                txtDatas.Add(new Files() { TableName = dt.TableName, Bytes = bytes });
            }

            using (ZipFile zip = new ZipFile())
            {

                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (Files txtData in txtDatas)
                {
                    if (fileType == "ALLCSV")
                    {

                        zip.AddEntry(txtData.TableName + ".csv", txtData.Bytes);
                    }
                    else
                    {
                        zip.AddEntry(txtData.TableName + ".txt", txtData.Bytes);
                    }
                }


                string downloadTempPath = Settings.ApplicationSettings.DownloadTempPath;
                string tempfilePath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"), fileNameValue + ".zip");


                zip.Save(tempfilePath);


                Response.Clear();
                Response.BufferOutput = false;
                if (fileType == "ALLCSV")
                {

                    string zipName = String.Format("{0}_CSV.zip", fileNameValue);
                    Response.ContentType = "application/zip";
                    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                    using (ZipFile zip1 = new ZipFile())
                    {
                        zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                        foreach (Files txtData in txtDatas)
                        {
                            zip1.AddEntry(txtData.TableName + ".csv", txtData.Bytes);
                        }
                        zip1.Save(Response.OutputStream);
                    }
                    //zip.Save(Response.OutputStream);
                    Response.End();
                }
                else
                {

                    string zipName = String.Format("{0}.zip", fileNameValue);
                    Response.ContentType = "application/zip";
                    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                    using (ZipFile zip2 = new ZipFile())
                    {
                        zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                        foreach (Files txtData in txtDatas)
                        {
                            zip2.AddEntry(txtData.TableName + ".txt", txtData.Bytes);
                        }
                        zip2.Save(Response.OutputStream);
                    }
                    Response.End();
                }

            }
        }


        public void DownloadEntityReportNewIntoCSVAll(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {

            if (Session["ExportReportFilterNew"] != null)
            {
                try
                {

                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();

                    ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();

                    objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                    objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                    objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                    objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                    objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                    objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                    objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                    objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    DataSet dsCdb = new DataSet();
                    DataSet dsAdditional = new DataSet();
                    ds.Tables.Add(dtFilter);
                    string fileName = "ExportReport_" + (fileType == "ALLCSV" ? "CSV_" : "TXT_") + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");

                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        //int TotalEntityReport = 0;
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = fileType;
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        try
                        {
                            for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                            {
                                objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                                var layerdetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                // var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                List<Dictionary<string, string>> lstExportEntitiesDetailCdb = null;
                                List<Dictionary<string, string>> lstExportEntitiesDetailAdditional = null;
                                List<string> reportTypeString = reportType;

                                if (reportTypeString.Contains("GIS"))
                                {
                                    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                                }
                                if (reportTypeString.Contains("CDB") && objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                                {
                                    DataTable dtCdbFilter = GetExportReportFilter(objExportReportFilterNew);
                                    dsCdb.Tables.Add(dtCdbFilter);
                                    lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCdb(objExportEntitiesReport.objReportFilters);
                                }
                                if (reportTypeString.Contains("ADDITIONAL") && layerdetails.is_dynamic_control_enable)
                                {
                                    DataTable dtAdditionalFilter = GetExportReportFilter(objExportReportFilterNew);
                                    dsAdditional.Tables.Add(dtAdditionalFilter);
                                    lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewAdditional(objExportEntitiesReport.objReportFilters);
                                }
                                if (reportTypeString.Contains("ALL"))
                                {
                                    lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                                    if (layerdetails.is_dynamic_control_enable)
                                    {
                                        lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewAdditional(objExportEntitiesReport.objReportFilters);
                                    }
                                    if (objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                                    {
                                        lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCdb(objExportEntitiesReport.objReportFilters);
                                    }
                                }

                                if (lstExportEntitiesDetail != null)
                                {
                                    if (lstExportEntitiesDetail.Count > 0)
                                    {
                                        //lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                                        DataTable dtReport = new DataTable();
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                                                                                                                    //dtReport.TableName = layerDetail.layer_title;
                                        dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName;
                                        if (dtReport != null && dtReport.Rows.Count > 0)
                                        {
                                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                        }
                                        if (dtReport.Rows.Count > 0)
                                            ds.Tables.Add(dtReport);
                                    }
                                }
                                if (lstExportEntitiesDetailCdb != null)
                                {
                                    if (lstExportEntitiesDetailCdb.Count > 0)
                                    {
                                        //lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                                        DataTable dtReportCdb = new DataTable();
                                        dtReportCdb = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailCdb);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                                                                                                                          //dtReport.TableName = layerDetail.layer_title;
                                        dtReportCdb.TableName = objExportEntitiesReport.objReportFilters.layerName;
                                        if (dtReportCdb != null && dtReportCdb.Rows.Count > 0)
                                        {
                                            if (dtReportCdb.Columns.Contains("S_NO")) { dtReportCdb.Columns.Remove("S_NO"); }
                                            if (dtReportCdb.Columns.Contains("totalrecords")) { dtReportCdb.Columns.Remove("totalrecords"); }
                                            if (dtReportCdb.Columns.Contains("Barcode")) { dtReportCdb.Columns.Remove("Barcode"); }
                                        }
                                        if (dtReportCdb.Rows.Count > 0)
                                            dsCdb.Tables.Add(dtReportCdb);
                                    }
                                }

                                if (lstExportEntitiesDetailAdditional != null)
                                {
                                    if (lstExportEntitiesDetailAdditional.Count > 0)
                                    {
                                        //lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                                        DataTable dtReportAdditional = new DataTable();
                                        dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                                                                                                                                        //dtReport.TableName = layerDetail.layer_title;
                                        dtReportAdditional.TableName = objExportEntitiesReport.objReportFilters.layerName;
                                        if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                                        {
                                            if (dtReportAdditional.Columns.Contains("S_NO")) { dtReportAdditional.Columns.Remove("S_NO"); }
                                            if (dtReportAdditional.Columns.Contains("totalrecords")) { dtReportAdditional.Columns.Remove("totalrecords"); }
                                            if (dtReportAdditional.Columns.Contains("Barcode")) { dtReportAdditional.Columns.Remove("Barcode"); }
                                        }
                                        if (dtReportAdditional.Rows.Count > 0)
                                            dsAdditional.Tables.Add(dtReportAdditional);
                                    }
                                }

                            }

                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;

                            string tempFileName = fileName + exportReportLog.file_extension;
                            //string ftpFolder = "ExportReportLog/";
                            //string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            ExportReportCSVMerge(ds, dsCdb, dsAdditional, tempFileName, ApplicationSettings.CsvDelimiter, fileType, ftpFilePath, ftpUserName, ftpPwd);
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            //Thread.Sleep(10000);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoCSVAll()", "Report", ex);
                        }

                    });

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void ExportReportCSVMerge(DataSet ds, DataSet dsCdb, DataSet dsAdditional, string fileNameValue, string delimiter, string fileType, string ftpfilePath, string ftpUserName, string ftpPassword)
        {
            List<Files> txtDatas = new List<Files>();
            foreach (DataTable dt in ds.Tables)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(Utility.CommonUtility.ConvertDataTableToString(dt, delimiter)); //Convert DataTable to string with specific delimiter
                txtDatas.Add(new Files() { TableName = dt.TableName, Bytes = bytes });
            }

            foreach (DataTable dt in dsCdb.Tables)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(Utility.CommonUtility.ConvertDataTableToString(dt, delimiter)); //Convert DataTable to string with specific delimiter
                txtDatas.Add(new Files() { TableName = dt.TableName, Bytes = bytes });
            }

            foreach (DataTable dt in dsAdditional.Tables)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(Utility.CommonUtility.ConvertDataTableToString(dt, delimiter)); //Convert DataTable to string with specific delimiter
                txtDatas.Add(new Files() { TableName = dt.TableName, Bytes = bytes });
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (Files txtData in txtDatas)
                {
                    if (fileType == "ALLCSV")
                    {
                        zip.AddEntry(txtData.TableName + ".csv", txtData.Bytes);
                    }
                    else
                    {
                        zip.AddEntry(txtData.TableName + ".txt", txtData.Bytes);
                    }
                }
                //Commented by priyanka
                //string tempfilePath = Path.Combine(Server.MapPath("~/Uploads/Temp/ExportReportLog"), fileNameValue);
                //Commented by priyanka end

                string downloadTempPath = Settings.ApplicationSettings.DownloadTempPath;
                string tempfilePath = Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"), fileNameValue);

                zip.Save(tempfilePath);
                CommonUtility.FTPFileUpload(tempfilePath, fileNameValue, ftpfilePath, ftpUserName, ftpPassword);

                System.IO.File.Delete(tempfilePath);
                //Response.Clear();
                //Response.BufferOutput = false;
                //if (fileType == "ALLCSV")
                //{
                //    string zipName = String.Format("{0}_CSV.zip", fileNameValue);
                //    Response.ContentType = "application/zip";
                //    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                //    zip.Save(Response.OutputStream);
                //    Response.End();
                //}
                //else
                //{
                //    string zipName = String.Format("{0}.zip", fileNameValue);
                //    Response.ContentType = "application/zip";
                //    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                //    zip.Save(Response.OutputStream);
                //    Response.End();
                //}

            }
        }

        public void DownloadEntityReportIntoCSVAll(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {


            if (Session["ExportReportFilterNew"] != null)
            {
                try
                {
                    //LogHelper.GetInstance.WriteDebugLog($"***********************************CSV logs start ***********************************");

                    DateTime startTime = DateTime.Now;
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                    ExportEntitiesReportNew ExportEntitiesReportSummary = new ExportEntitiesReportNew();
                    ExportEntitiesReportSummary = (ExportEntitiesReportNew)Session["EntityExportSummaryData"];
                    ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();

                    objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                    objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                    objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                    objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                    objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                    objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                    objExportEntitiesReport.objReportFilters.selected_route_ids = objExportReportFilterNew.selected_route_ids;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnerShipId = objExportReportFilterNew.SelectedOwnerShipId;
                    // objExportEntitiesReport.objReportFilters.SelectedOwnershipIds = objExportReportFilterNew.SelectedOwnershipIds;
                    objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                    objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }
                    string parentFolder = $"ExportReport_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                    string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                    string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                    string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));

                    // Create Parent folder for temparary basic on server 
                    if (Directory.Exists(directoryPath).Equals(false))
                        Directory.CreateDirectory(directoryPath);

                    string fileName = $"{parentFolder}/{dtFilter.TableName}.csv";
                    StreamCSVInFolder(dtFilter, fileName);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        //int TotalEntityReport = 0;
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = parentFolder;
                        exportReportLog.file_type = fileType;
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        dtFilter = null;
                        try
                        {
                            // ---Start-- Below code for generate export report summary in csv file -------
                            List<EntitySummaryReport> lstRprtData = ExportEntitiesReportSummary.lstReportData;
                            if (lstRprtData.Count > 0)
                            {
                                if (!ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                                    ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                                if (!ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                                    ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                                if (!ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                                    ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues = ExportEntitiesReportSummary.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");

                                DataTable dtSummaryReport = new DataTable();
                                dtSummaryReport = MiscHelper.ListToDataTable(ExportEntitiesReportSummary.lstReportData);
                                dtSummaryReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                                if (dtSummaryReport != null && dtSummaryReport.Rows.Count > 0)
                                {
                                    if (!ApplicationSettings.IsDormantEnabled)
                                    {
                                        dtSummaryReport.Columns.Remove("DORMANT_COUNT");
                                    }
                                    dtSummaryReport.Columns.Remove("entity_id");
                                    dtSummaryReport.Columns.Remove("entity_name");
                                    dtSummaryReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                    dtSummaryReport.Columns["planned_count"].ColumnName = "Planned";
                                    dtSummaryReport.Columns["as_built_count"].ColumnName = "As-Built";
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        dtSummaryReport.Columns["dormant_count"].ColumnName = "Dormant";
                                    }

                                    string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                    if (networkstatusvalues.Length < 3)
                                    {
                                        if (!networkstatusvalues.Contains("P"))
                                        {
                                            dtSummaryReport.Columns.Remove("PLANNED");
                                        }
                                        if (!networkstatusvalues.Contains("A"))
                                        {
                                            dtSummaryReport.Columns.Remove("AS-BUILT");
                                        }
                                        if (ApplicationSettings.IsDormantEnabled)
                                        {
                                            if (!networkstatusvalues.Contains("D"))
                                            {
                                                dtSummaryReport.Columns.Remove("DORMANT");
                                            }
                                        }
                                    }
                                    DataRow row = dtSummaryReport.NewRow();
                                    row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                                    if (dtSummaryReport.Columns.Contains("Planned"))
                                    {
                                        row["Planned"] = dtSummaryReport.Compute("Sum(Planned)", "");
                                    }
                                    if (dtSummaryReport.Columns.Contains("As-Built"))
                                    {
                                        row["As-Built"] = dtSummaryReport.Compute("Sum([As-Built])", "");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (dtSummaryReport.Columns.Contains("Dormant"))
                                        {
                                            row["Dormant"] = dtSummaryReport.Compute("Sum(Dormant)", "");
                                        }
                                    }
                                    dtSummaryReport.Rows.Add(row);
                                    string summaryFileName = $"{parentFolder}/{dtSummaryReport.TableName}.csv";
                                    StreamCSVInFolder(dtSummaryReport, summaryFileName);
                                }
                            }
                            var tasks = new List<Task>();
                            foreach (var layer in objExportEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {

                                        //LogHelper.GetInstance.WriteDebugLogTest($"====================================={layer.layer_name}============================", layer.layer_name);
                                        objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        // var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                                        //LogHelper.GetInstance.WriteDebugLogTest($"Request Sent to get the data from database on: {DateTime.Now}", layer.layer_name);
                                        var layerdetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                        if (layerdetails != null)
                                        {
                                            if (layerdetails.is_dynamic_control_enable == null)
                                            {
                                                layerdetails.is_dynamic_control_enable = false;
                                            }
                                        }
                                        //bool textType = false;
                                        //var dataSet = GetDataSetFromDataTable(objExportEntitiesReport, reportType, layer.layer_name, layerdetails, exportReportLog.total_entity, textType);
                                        //DataTable dtReport = dataSet.Tables[0];
                                        //DataTable dtReportCdb = dataSet.Tables[1];
                                        //DataTable dtReportAdditional = dataSet.Tables[2];

                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                        List<Dictionary<string, string>> lstExportEntitiesDetailCdb = null;
                                        List<Dictionary<string, string>> lstExportEntitiesDetailAdditional = null;
                                        List<string> reportTypeString = reportType;

                                        if (reportTypeString.Contains("GIS"))
                                        {
                                            lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        }
                                        if (reportTypeString.Contains("CDB") && objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                                        {
                                            lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCSVCdb(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        }
                                        if (reportTypeString.Contains("ADDITIONAL") && layerdetails.is_dynamic_control_enable)
                                        {
                                            lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                        }
                                        if (reportTypeString.Contains("") || reportTypeString.Contains("ALL"))
                                        {
                                            lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            if (layerdetails.is_dynamic_control_enable)
                                            {
                                                lstExportEntitiesDetailAdditional = new BLLayer().GetExportReportSummaryViewCSVAdditional(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                            if (objExportEntitiesReport.objReportFilters.layerName == EntityType.Cable.ToString())
                                            {
                                                lstExportEntitiesDetailCdb = new BLLayer().GetExportReportSummaryViewCSVCdb(objExportEntitiesReport.objReportFilters, layer.layer_name);
                                            }
                                        }
                                        DataTable dtReport = new DataTable();
                                        DataTable dtReportCdb = new DataTable();
                                        DataTable dtReportAdditional = new DataTable();

                                        if (lstExportEntitiesDetail != null && lstExportEntitiesDetail.Count > 0)
                                        {
                                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                                                                                                                        //dtReport.TableName = layerDetail.layer_title;
                                            dtReport.TableName = layer.layer_title;
                                            if (dtReport != null && dtReport.Rows.Count > 0)
                                            {
                                                if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                                if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                            }

                                        }

                                        if (lstExportEntitiesDetailCdb != null && lstExportEntitiesDetailCdb.Count > 0)
                                        {
                                            dtReportCdb = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailCdb);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                                                                                                                              //dtReport.TableName = layerDetail.layer_title;
                                            dtReportCdb.TableName = layer.layer_title;
                                            if (dtReportCdb != null && dtReportCdb.Rows.Count > 0)
                                            {
                                                if (dtReportCdb.Columns.Contains("S_NO")) { dtReportCdb.Columns.Remove("S_NO"); }
                                                if (dtReportCdb.Columns.Contains("totalrecords")) { dtReportCdb.Columns.Remove("totalrecords"); }
                                                if (dtReportCdb.Columns.Contains("Barcode")) { dtReportCdb.Columns.Remove("Barcode"); }
                                            }

                                        }

                                        if (lstExportEntitiesDetailAdditional != null && lstExportEntitiesDetailAdditional.Count > 0)
                                        {
                                            dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                                                                                                                                            //dtReport.TableName = layerDetail.layer_title;
                                            dtReportAdditional.TableName = layer.layer_title;
                                            if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                                            {
                                                if (dtReportAdditional.Columns.Contains("S_NO")) { dtReportAdditional.Columns.Remove("S_NO"); }
                                                if (dtReportAdditional.Columns.Contains("totalrecords")) { dtReportAdditional.Columns.Remove("totalrecords"); }
                                                if (dtReportAdditional.Columns.Contains("Barcode")) { dtReportAdditional.Columns.Remove("Barcode"); }
                                            }

                                        }



                                        objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                        objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;

                                        if (dtReportCdb.Rows.Count > 0)
                                        {
                                            fileName = $"{parentFolder}/{layer.layer_title + "_CdbAttributes"}.csv";
                                            StreamNewCSVInFolder(dtReportCdb, fileName);
                                            exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
                                        }
                                        if (dtReportAdditional.Rows.Count > 0)
                                        {
                                            fileName = $"{parentFolder}/{layer.layer_title + "_AdditionalAttributes"}.csv";
                                            StreamNewCSVInFolder(dtReportCdb, fileName);
                                            exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
                                        }
                                        if (dtReport.Rows.Count > 0)
                                        {
                                            fileName = $"{parentFolder}/{layer.layer_title + "_GisAttributes"}.csv";

                                            StreamNewCSVInFolder(dtReport, fileName);
                                            //StreamNewCSVInFolder(dtReportCdb, fileName);
                                            //StreamNewCSVInFolder(dtReportAdditional, fileName);
                                            exportReportLog.export_ended_on = DateTime.Now;
                                            exportReportLog.status = "Success";
                                            exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
                                        }

                                        //Thread.Sleep(10000);
                                        dtReport = null;
                                        dtReportCdb = null;
                                        dtReportAdditional = null;




                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }));
                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();
                            //----------------END----------------------------------
                            //                     object lockObj = new object();
                            //                     var lstChunks = objExportEntitiesReport.lstLayers.ToChunks(1).ToList();
                            //                     var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };							
                            //ParallelLoopResult result = Parallel.ForEach(lstChunks, options, (item) =>
                            //                     {
                            //                         lock (lockObj)
                            //                         {
                            //		LogHelper.GetInstance.WriteDebugLog($"====================================={item[0].layer_name}============================");
                            //		objExportEntitiesReport.objReportFilters.layerName = item[0].layer_name;
                            //		// var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                            //		LogHelper.GetInstance.WriteDebugLog($"Request Sent to get the data from database on: {DateTime.Now}");
                            //		List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters);
                            //		LogHelper.GetInstance.WriteDebugLog($"data received from database on: {DateTime.Now}");
                            //		LogHelper.GetInstance.WriteDebugLog($"Received data count: {lstExportEntitiesDetail.Count}");
                            //		if (lstExportEntitiesDetail.Count > 0)
                            //                             {
                            //                                 //lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                            //                                 DataTable dtReport = new DataTable();
                            //                                 dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//, true, ApplicationSettings.numberFormatType, new string[] { "" }
                            //                                                                                                             //dtReport.TableName = layerDetail.layer_title;
                            //                                 dtReport.TableName = item[0].layer_title;
                            //                                 if (dtReport != null && dtReport.Rows.Count > 0)
                            //                                 {
                            //                                     if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            //                                     if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            //                                     if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                            //                                 }
                            //                                 if (dtReport.Rows.Count > 0)
                            //                                 {
                            //                                     //	ds.Tables.Add(dtReport);
                            //                                     objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                            //                                     objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                            //                                     fileName = $"{parentFolder}/{dtReport.TableName}.csv";
                            //				StreamNewCSVInFolder(dtReport, fileName);
                            //                                     exportReportLog.export_ended_on = DateTime.Now;
                            //                                     exportReportLog.status = "Success";
                            //                                     exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
                            //                                 }
                            //                                 //Thread.Sleep(10000);
                            //                                 dtReport = null;
                            //                             }
                            //                         }
                            //                     });
                            //LogHelper.GetInstance.WriteDebugLog($"All entity data fetched and save in csv file on: {DateTime.Now}");
                            // get FTP details
                            string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                            string zipfilePath = directoryPath + ".zip";
                            string fileNameValue = parentFolder + ".zip";

                            // Below code for Convert parent folder to ZIP and delete parent folder after ZIP on server
                            using (var zip = new ZipFile())
                            {
                                //zip.UseZip64WhenSaving = Zip64Option.Always;
                                //zip.CompressionMethod = CompressionMethod.BZip2;
                                zip.AddDirectory(directoryPath);
                                zip.Save(zipfilePath);
                            }
                            if (System.IO.File.Exists(zipfilePath))
                            {
                                string fileZipName = Path.GetFileName(zipfilePath);
                                Directory.Delete(directoryPath, true);
                            }
                            FileInfo file = new FileInfo(zipfilePath);

                            //LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading start on FTP : {DateTime.Now}");
                            // ZIP File upload on FTP server
                            CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);
                            //LogHelper.GetInstance.WriteDebugLog($"Combined file Uploading completed on FTP : {DateTime.Now}");

                            // Deleted ZIP on Server after uploaded on FTP server
                            System.IO.File.Delete(zipfilePath);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            DateTime endTime = DateTime.Now;
                            //LogHelper.GetInstance.WriteDebugLog($"Total Time taking to create combined csv in second:{endTime.Second - startTime.Second} ");
                            //LogHelper.GetInstance.WriteDebugLog($"***********************************CSV logs end ***********************************");
                        }
                        catch (Exception ex)
                        {

                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportIntoCSVAll()", "Report", ex);
                            // delete folder after error generate
                            if (Directory.Exists(directoryPath).Equals(true))
                                Directory.Delete(directoryPath, true);
                        }

                    });

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

        }

        public void DownloadEntitySummaryViewIntoCSV(string fileName, string fileType)
        {

            if (Session["EntityExportSummaryView"] != null)
            {

                try
                {
                    var userdetails = (User)Session["userDetail"];
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportEntitiesSummaryViewFilter)Session["EntityExportSummaryView"];
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        int totalPlannedCount = 0, totalAsBuiltCount = 0, totalDormantCount = 0;
                        objExportEntitiesReport.objReportFilters.currentPage = 0;
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();


                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                        lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                        DataTable dtReport = new DataTable();
                        DataTable bldStatusHistory = new DataTable();
                        DataTable faultStatusHistory = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        if (dtReport.Columns.Contains("Network Status"))
                        {
                            totalPlannedCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Planned").ToList().Count;
                            totalAsBuiltCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "As Built").ToList().Count;
                            totalDormantCount = dtReport.AsEnumerable().Where(x => x["Network Status"].ToString() == "Dormant").ToList().Count;
                        }
                        dtReport.TableName = layerDetail.layer_title; DataSet dataset = new DataSet();
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = fileType;
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        string tempFileName = fileName + exportReportLog.file_extension;
                        string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                        string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                        string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                        try
                        {
                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                            }

                            if (layerDetail.layer_name.ToUpper() == "BUILDING")
                            {
                                List<Dictionary<string, string>> lstBuildingComments = new BLLayer().GetBuildingStatusHistory(objExportEntitiesReport.objReportFilters);
                                lstBuildingComments = BLConvertMLanguage.ExportMultilingualConvert(lstBuildingComments);

                                bldStatusHistory = MiscHelper.GetDataTableFromDictionaries(lstBuildingComments);
                                if (bldStatusHistory != null && bldStatusHistory.Rows.Count > 0)
                                {
                                    bldStatusHistory.Columns.Add("Modified_On", typeof(System.String));
                                    foreach (DataRow dr in bldStatusHistory.Rows)
                                    {
                                        dr["Modified_On"] = MiscHelper.FormatDateTime((dr["status_modified_on"].ToString()));
                                    }
                                    if (bldStatusHistory.Columns.Contains("S_NO")) { bldStatusHistory.Columns.Remove("S_NO"); }
                                    if (bldStatusHistory.Columns.Contains("totalrecords")) { bldStatusHistory.Columns.Remove("totalrecords"); }
                                    if (bldStatusHistory.Columns.Contains("building_code")) { bldStatusHistory.Columns["building_code"].ColumnName = "Building Code"; }
                                    if (bldStatusHistory.Columns.Contains("building_status_history")) { bldStatusHistory.Columns["building_status_history"].ColumnName = "Building Status"; }
                                    if (bldStatusHistory.Columns.Contains("comment")) { bldStatusHistory.Columns["comment"].ColumnName = "Comment"; }
                                    if (bldStatusHistory.Columns.Contains("status_modified_by")) { bldStatusHistory.Columns["status_modified_by"].ColumnName = "Status Updated By"; }

                                    if (bldStatusHistory.Columns.Contains("Modified_On")) { bldStatusHistory.Columns["Modified_On"].ColumnName = "Status Updated On"; }
                                    if (bldStatusHistory.Columns.Contains("status_modified_on")) { bldStatusHistory.Columns.Remove("status_modified_on"); }
                                }
                                bldStatusHistory.TableName = "Building_Status_History";

                            }
                            if (layerDetail.layer_name.ToUpper() == "FAULT")
                            {
                                List<Dictionary<string, string>> lstFaultStatus = new BLLayer().GetFaultStatusHistory(objExportEntitiesReport.objReportFilters);
                                lstFaultStatus = BLConvertMLanguage.ExportMultilingualConvert(lstFaultStatus);
                                faultStatusHistory = MiscHelper.GetDataTableFromDictionaries(lstFaultStatus);
                                if (faultStatusHistory != null && faultStatusHistory.Rows.Count > 0)
                                {

                                    faultStatusHistory.Columns.Add("Select Date", typeof(System.String));
                                    faultStatusHistory.Columns.Add("Modified On", typeof(System.String));
                                    foreach (DataRow dr in faultStatusHistory.Rows)
                                    {
                                        dr["Select Date"] = MiscHelper.FormatDate((dr["fault_status_updated_on"].ToString()));
                                        dr["Modified On"] = MiscHelper.FormatDateTime((dr["status_modified_on"].ToString()));
                                    }
                                    if (faultStatusHistory.Columns.Contains("S_NO")) { faultStatusHistory.Columns.Remove("S_NO"); }
                                    if (faultStatusHistory.Columns.Contains("totalrecords")) { faultStatusHistory.Columns.Remove("totalrecords"); }
                                    //if (faultStatusHistory.Columns.Contains("building_code")) { faultStatusHistory.Columns["building_code"].ColumnName = "Building Code"; }
                                    if (faultStatusHistory.Columns.Contains("fault_status_history")) { faultStatusHistory.Columns["fault_status_history"].ColumnName = "Status"; }
                                    if (faultStatusHistory.Columns.Contains("rca")) { faultStatusHistory.Columns["rca"].ColumnName = "RCA"; }
                                    if (faultStatusHistory.Columns.Contains("updated_by")) { faultStatusHistory.Columns["updated_by"].ColumnName = "Updated By"; }
                                    if (faultStatusHistory.Columns.Contains("fault_code")) { faultStatusHistory.Columns["fault_code"].ColumnName = "Network Id"; }
                                    if (faultStatusHistory.Columns.Contains("requested_by")) { faultStatusHistory.Columns["requested_by"].ColumnName = "Requested By"; }
                                    if (faultStatusHistory.Columns.Contains("request_comment")) { faultStatusHistory.Columns["request_comment"].ColumnName = "Request Comment"; }
                                    if (faultStatusHistory.Columns.Contains("status_modified_on")) { faultStatusHistory.Columns.Remove("status_modified_on"); }
                                    if (faultStatusHistory.Columns.Contains("status_modified_by")) { faultStatusHistory.Columns.Remove("status_modified_by"); }
                                    if (faultStatusHistory.Columns.Contains("fault_status_updated_on")) { faultStatusHistory.Columns.Remove("fault_status_updated_on"); }
                                    //if (faultStatusHistory.Columns.Contains("Modified On")) { faultStatusHistory.Columns.Remove("Modified On"); }
                                }
                                faultStatusHistory.TableName = "Fault_Status_History";

                            }

                            if (layerDetail.is_reference_allowed)
                            {
                                #region Add the new sheet for entity Reference and export with Data set
                                DataSet ds = new DataSet();

                                var entityData = new BLMisc().GetEntityReferenceExportData<Dictionary<string, string>>(0, layerDetail.layer_name, string.Empty);
                                DataTable dtReference = MiscHelper.GetDataTableFromDictionaries(entityData);
                                dtReference.TableName = layerDetail.layer_title + "_Reference";
                                ds.Tables.Add(dtFilter);
                                ds.Tables.Add(dtReport);
                                if (layerDetail.layer_name.ToUpper() == "BUILDING")
                                {
                                    ds.Tables.Add(bldStatusHistory);
                                }
                                ds.Tables.Add(dtReference);
                                ExportReportCSVNew(ds, tempFileName, ApplicationSettings.CsvDelimiter, fileType, ftpFilePath, ftpUserName, ftpPwd);
                                #endregion
                            }
                            else if (dtReport.Rows.Count > 0)
                            {
                                dataset.Tables.Add(dtFilter);
                                dataset.Tables.Add(dtReport);
                                if (layerDetail.layer_name.ToUpper() == "BUILDING")
                                {
                                    dataset.Tables.Add(bldStatusHistory);
                                }
                                if (layerDetail.layer_name.ToUpper() == "FAULT")
                                {
                                    dataset.Tables.Add(faultStatusHistory);
                                }
                                if (dataset.Tables.Count > 0)
                                {
                                    ExportReportCSVNew(dataset, tempFileName, ApplicationSettings.CsvDelimiter, fileType, ftpFilePath, ftpUserName, ftpPwd);
                                }
                                //else
                                //{
                                //    var csvFile = Utility.CommonUtility.ConvertDataTableToString(dtReport, ";");
                                //  //  string fileName = "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                                //    var response = System.Web.HttpContext.Current.Response;
                                //    response.BufferOutput = true;
                                //    response.Clear();
                                //    response.ClearHeaders();
                                //    response.ContentEncoding = Encoding.Unicode;
                                //    response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".CSV ");
                                //    response.ContentType = "text/plain";
                                //    response.Write(csvFile);
                                //    response.End();
                                //}
                                // ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_SummaryView_" + MiscHelper.getTimeStamp());
                            }
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadEntityReportNewIntoCSVAll()", "Report", ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        #endregion


        #region LandBase Layer Report
        public ActionResult LandBaseLayerExportReport(ExportLandBaseLayerReport objExportLBLayerReport)
        {
            var userdetails = (User)Session["userDetail"];

            objExportLBLayerReport.objReportFilters.SelectedRegionIds = objExportLBLayerReport.objReportFilters.SelectedRegionId != null && objExportLBLayerReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportLBLayerReport.objReportFilters.SelectedRegionId.ToArray()) : "";
            objExportLBLayerReport.objReportFilters.SelectedProvinceIds = objExportLBLayerReport.objReportFilters.SelectedProvinceId != null && objExportLBLayerReport.objReportFilters.SelectedProvinceId.Count > 0 ? string.Join(",", objExportLBLayerReport.objReportFilters.SelectedProvinceId.ToArray()) : "";

            objExportLBLayerReport.objReportFilters.SelectedParentUsers = objExportLBLayerReport.objReportFilters.SelectedParentUser != null && objExportLBLayerReport.objReportFilters.SelectedParentUser.Count > 0 ? string.Join(",", objExportLBLayerReport.objReportFilters.SelectedParentUser.ToArray()) : "";
            objExportLBLayerReport.objReportFilters.SelectedUserIds = objExportLBLayerReport.objReportFilters.SelectedUserId != null && objExportLBLayerReport.objReportFilters.SelectedUserId.Count > 0 ? string.Join(",", objExportLBLayerReport.objReportFilters.SelectedUserId.ToArray()) : "";
            objExportLBLayerReport.objReportFilters.SelectedLayerIds = objExportLBLayerReport.objReportFilters.SelectedLayerId != null && objExportLBLayerReport.objReportFilters.SelectedLayerId.Count > 0 ? string.Join(",", objExportLBLayerReport.objReportFilters.SelectedLayerId.ToArray()) : "";
            objExportLBLayerReport.objReportFilters.userId = Convert.ToInt32(userdetails.user_id);
            objExportLBLayerReport.objReportFilters.roleId = Convert.ToInt32(userdetails.role_id);

            if (objExportLBLayerReport.objReportFilters.fromDate != null)
                objExportLBLayerReport.lstReportData = new BLLandBaseLayer().GetExportReportSummary(objExportLBLayerReport.objReportFilters).OrderBy(m => m.entity_name).ToList();

            Session["ExportLandBaseLayerFilterNew"] = objExportLBLayerReport.objReportFilters;
            BindLandBaseReportDropdownNew(ref objExportLBLayerReport);
            Session["LandBaseLayerExportSummaryData"] = objExportLBLayerReport;
            return PartialView("~/views/LandBaseLayer/LandBaseLayerReport.cshtml", objExportLBLayerReport);
        }

        public void BindLandBaseReportDropdownNew(ref ExportLandBaseLayerReport objExportEntitiesReport)
        {
            var userdetails = (User)Session["userDetail"];
            //Bind Layers..
            objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
            //Bind Regions..
            objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
            }
            List<int> parentUser = new List<int>();
            parentUser.Add(1);
            if (userdetails.role_id == 1)
                objExportEntitiesReport.lstParentUsers = new BLUser().GetUsersListByMGRIds(parentUser).OrderBy(x => x.user_name).ToList();//new BLUser().GetUsersListByMGRIds(parentUser).Where(x => x.user_id == Convert.ToInt32(Session["user_id"])).OrderBy(x => x.user_name).ToList();
            else
            {
                objExportEntitiesReport.lstParentUsers = new List<Models.User>();
                objExportEntitiesReport.lstParentUsers.Add(userdetails);// new BLUser().GetUserDetailByID(Convert.ToInt32(Session["user_id"])));// new BLUser().GetUsersListByMGRIds(parentUser).Where(x=> x.user_id == Convert.ToInt32(Session["user_id"])).OrderBy(x => x.user_name).ToList();
            }

            if (objExportEntitiesReport.objReportFilters.SelectedParentUser != null)
            {
                if (userdetails.role_id != 1)
                {
                    var parentUser_ids = string.Join(",", objExportEntitiesReport.objReportFilters.SelectedParentUser.Select(n => n.ToString()).ToArray());
                    objExportEntitiesReport.lstUsers = new BLUser().GetUserReportDetailsList(parentUser_ids).ToList();
                }
                else
                {
                    objExportEntitiesReport.lstUsers = new BLUser().GetUsersListByMGRIds(objExportEntitiesReport.objReportFilters.SelectedParentUser).OrderBy(x => x.user_name).ToList();
                }
            }

            //for duration based on 
            objExportEntitiesReport.lstDurationBasedOn = new BLMisc().GetDropDownList("", DropDownType.Export_Report.ToString());
        }

        public ActionResult ExportLBEntitySummaryView(ExportLandBaseLayerSummaryView objExportEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {
            if (Session["ExportLandBaseLayerFilterNew"] != null)
            {
                //sort = String.IsNullOrLandBaseLayerExportReportEmpty(sort) ? "" : sort.Replace(" ", "_");
                ExportLandBaseLayerReportFilter objExportReportFilter = new ExportLandBaseLayerReportFilter();

                objExportReportFilter = (ExportLandBaseLayerReportFilter)Session["ExportLandBaseLayerFilterNew"];

                objExportEntitiesReport.objReportFilters.SelectedRegionId = objExportReportFilter.SelectedRegionId;
                objExportEntitiesReport.objReportFilters.SelectedProvinceId = objExportReportFilter.SelectedProvinceId;
                objExportEntitiesReport.objReportFilters.SelectedParentUser = objExportReportFilter.SelectedParentUser;
                objExportEntitiesReport.objReportFilters.SelectedUserId = objExportReportFilter.SelectedUserId;

                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilter.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilter.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilter.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilter.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilter.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilter.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilter.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilter.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilter.geom;
                objExportEntitiesReport.objReportFilters.pageSize = 10;
                objExportEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
                objExportEntitiesReport.objReportFilters.sort = sort;
                objExportEntitiesReport.objReportFilters.sortdir = sortdir;
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilter.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilter.roleId;
                BindLBReportDropdownSummaryView(ref objExportEntitiesReport);

                //rt
                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.objReportFilters.SelectedLayerId = new BLLandBaseLayer().getLandBaseLayersByName(objExportEntitiesReport.objReportFilters.layerName).Where(x => x.layer_name == objExportEntitiesReport.objReportFilters.layerName).Select(x => x.id).ToList();
                objExportEntitiesReport.objReportFilters.lstAdvanceFilters = objExportEntitiesReport.lstAdvanceFilters;
                if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.layerName))
                {
                    objExportEntitiesReport.objReportFilters.advancefilter = getLBAdvanceFilter(objExportEntitiesReport.lstAdvanceFilters);
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetLandBaseExportReportSummaryView(objExportEntitiesReport.objReportFilters);

                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE", "GEOMETRY" };
                    objExportEntitiesReport.webColumns = BLConvertMLanguage.GetLandBaseEntityWiseColumns(objExportEntitiesReport.objReportFilters.SelectedLayerId[0], objExportEntitiesReport.objReportFilters.layerName, "REPORT", arrIgnoreColumns, userdetails.role_id, userdetails.user_id);
                    foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                    {
                        var obj = (IDictionary<string, object>)new ExpandoObject();

                        foreach (var col in dic)
                        {
                            //if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                            //{
                            //    obj.Add(col.Key, col.Value);
                            //}
                            obj.Add(col.Key, col.Value);
                        }
                        objExportEntitiesReport.lstReportData.Add(obj);
                    }
                    //objExportEntitiesReport.lstReportData = ConvertMultilingual.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                    objExportEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
                }
            }
            // if there is no filter and add one row by default...
            if (objExportEntitiesReport.lstAdvanceFilters.Count == 0)
            {
                objExportEntitiesReport.lstAdvanceFilters.Add(new ReportLandBaseLayerAdvanceFilter());
            }
            Session["LandBaseLayerExportSummaryView"] = objExportEntitiesReport.objReportFilters;
            return PartialView("~/views/LandBaseLayer/LandBaseLayerReportView.cshtml", objExportEntitiesReport);
        }

        public void BindLBReportDropdownSummaryView(ref ExportLandBaseLayerSummaryView objExportEntitiesReport)
        {
            //rt
            var userdetails = (User)Session["userDetail"];
            //Bind Layers..
            objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
            var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
            if (selectedlayerids != null)
            {
                if (selectedlayerids.Count > 0)
                    objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.id)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.layerName))
            {
                objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByLandBaseColumnName(objExportEntitiesReport.objReportFilters.layerName);
            }
        }

        public string getLBAdvanceFilter(List<ReportLandBaseLayerAdvanceFilter> lstFilters)
        {
            StringBuilder sbFilter = new StringBuilder();
            if (lstFilters != null)
            {
                foreach (var item in lstFilters)
                {
                    if (!string.IsNullOrEmpty(item.searchBy) && !string.IsNullOrEmpty(item.searchType))
                    {
                        item.searchText = item.searchText ?? "";
                        sbFilter.Append(" and upper(COALESCE(a." + item.searchBy + "::text,'')) " + item.searchType + (item.searchType.ToUpper() == "LIKE" ? "'%" + item.searchText.Trim().ToUpper() + "%'" : "'" + item.searchText.Trim().ToUpper() + "'"));
                    }
                }
            }
            return sbFilter.ToString();
        }


        public void DownloadLandBaselayerSummaryView(string fileType)
        {
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                if (fileType.ToUpper() == "EXCEL")
                {
                    DownloadLandBaselayerSummaryViewIntoExcel();
                }
                //else if (fileType.ToUpper() == "BARCODEEXCEL")
                //{
                //    ExportBarcodeSummaryViewIntoExcel();
                //}
                //else if (fileType.ToUpper() == "BARCODEPDF")
                //{
                //    ExportBarcodeSummaryViewIntoPDF();
                //}
                else if (fileType.ToUpper() == "DXF")
                {
                    DownloadLandBaseLayerSummaryViewIntoDXF();
                }
                else if (fileType.ToUpper() == "KML")
                {
                    DownloadLandBaseLayerSummaryViewIntoKML();
                }
                else if (fileType.ToUpper() == "SHAPE")
                {
                    ExportLandBaseLayerViewIntoShape();
                }
                else if (fileType.ToUpper() == "CSV")
                {
                    DownloadLandBaseLayerSummaryViewIntoCSV();
                }

            }

        }
        public void DownloadLandBaselayerSummaryViewIntoExcel()
        {

            if (Session["LandBaseLayerExportSummaryView"] != null)
            {

                try
                {
                    ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportLandBaseLayerSummaryViewFilter)Session["LandBaseLayerExportSummaryView"];

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();


                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetLandBaseExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                    DataTable dtReport = new DataTable();
                    // DataTable bldStatusHistory = new DataTable();
                    //  DataTable faultStatusHistory = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                    DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportEntitiesReport.objReportFilters);
                    DataSet dataset = new DataSet();
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                        if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                    }

                    if (dtReport.Rows.Count > 0)
                    {
                        dataset.Tables.Add(dtFilter);
                        dataset.Tables.Add(dtReport);
                        ExportData(dataset, "ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                        // ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_SummaryView_" + MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void ExportLandBaseLayerViewIntoShape()
        {

            if (Session["LandBaseLayerExportSummaryView"] != null)
            {

                try
                {
                    ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportLandBaseLayerSummaryViewFilter)Session["LandBaseLayerExportSummaryView"];

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "SHAPE";

                    //Filter the Layer Detail
                    //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetExportLandBaseLayerSummaryViewKML(objExportEntitiesReport.objReportFilters);
                    DataTable dtReport = new DataTable();
                    if (lstExportEntitiesDetail.Count > 0)
                    {
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);

                        dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        }
                        //ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_Report_" + MiscHelper.getTimeStamp(), false);
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtReport);
                    GetShapeFile(ds, "ExportReport");

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }
        public string getKmlForSingleLandBaseLayer()
        {
            string finalkml = string.Empty;
            if (Session["LandBaseLayerExportSummaryView"] != null)
            {

                try
                {
                    ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportLandBaseLayerSummaryViewFilter)Session["LandBaseLayerExportSummaryView"];
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "KML";
                    //Filter the Layer Detail
                    //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetExportLandBaseLayerSummaryViewKML(objExportEntitiesReport.objReportFilters);
                    DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportEntitiesReport.objReportFilters);
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
                    if (objExportEntitiesReport.objReportFilters.layerName != null)
                    {
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(m.id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                    finalkml = KMLHelper.GetKmlForLandBaseLayer(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, dtFilter);
                    return finalkml;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return finalkml;
        }
        public void DownloadLandBaseLayerSummaryViewIntoDXF()
        {
            string finalkml = getKmlForSingleLandBaseLayer();
            string newPath = Path.Combine(Server.MapPath("~/Uploads/"));


            //dataContent is byte[]
            System.IO.File.WriteAllText(newPath + "report.kml", finalkml.ToString());
            string baseFolder = newPath + "report.kml";
            string kmlFolder = newPath;
            DataTable dataTable = new DataTable();
            var converter = new Convertor(baseFolder, "", kmlFolder, "");
            var response = converter.KmltoDXFConverter(newPath, "report");

            string attachment = "attachment; filename=ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".dxf";
            Response.ClearContent();
            Response.Clear();
            Response.ContentType = "application/xml";
            Response.AddHeader("content-disposition", attachment);
            Response.Write(response.Output);
            Response.End();
        }
        public void DownloadLandBaseLayerSummaryViewIntoKML()
        {
            if (Session["LandBaseLayerExportSummaryView"] != null)
            {

                try
                {
                    DataSet ds = new DataSet();
                    ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportLandBaseLayerSummaryViewFilter)Session["LandBaseLayerExportSummaryView"];
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "KML";
                    //Filter the Layer Detail
                    // var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetExportLandBaseLayerSummaryViewKML(objExportEntitiesReport.objReportFilters);
                    DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportEntitiesReport.objReportFilters);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                    string layerName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                    layerName = layerName + "_";
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
                    if (objExportEntitiesReport.objReportFilters.layerName != null)
                    {
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(m.id)).ToList();
                    }
                    string TempkmlFileName = "ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
                    KMLHelper.LandBaseDatasetToKML(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, TempkmlFileName, layerName, dtFilter);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void DownloadLandBaseLayerSummaryViewIntoCSV()
        {

            if (Session["LandBaseLayerExportSummaryView"] != null)
            {

                try
                {
                    ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();

                    objExportEntitiesReport.objReportFilters = (ExportLandBaseLayerSummaryViewFilter)Session["LandBaseLayerExportSummaryView"];

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();


                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetLandBaseExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                    lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                    DataTable dtReport = new DataTable();

                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                    DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportEntitiesReport.objReportFilters);
                    DataSet dataset = new DataSet();
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                        if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                    }

                    if (dtReport.Rows.Count > 0)
                    {
                        dataset.Tables.Add(dtFilter);
                        dataset.Tables.Add(dtReport);

                        if (dataset.Tables.Count > 0)
                        {
                            ExportCSV(dataset, "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss"), ";");
                        }
                        else
                        {
                            var csvFile = Utility.CommonUtility.ConvertDataTableToString(dtReport, ";");
                            string fileName = "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                            var response = System.Web.HttpContext.Current.Response;
                            response.BufferOutput = true;
                            response.Clear();
                            response.ClearHeaders();
                            response.ContentEncoding = Encoding.Unicode;
                            response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".CSV ");
                            response.ContentType = "text/plain";
                            response.Write(csvFile);
                            response.End();
                        }
                        // ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_SummaryView_" + MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public DataTable GetLandBaseLayerExportReportFilter(object obj)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            DataTable dt = new DataTable(Resources.Resources.SI_GBL_GBL_NET_FRM_165);
            dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_166);
            dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_167);
            DataRow dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_065; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_066; dt.Rows.Add(dr); dr = dt.NewRow();

            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_068; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_069; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_GBL_GBL_NET_FRM_098; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_071; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_072; dt.Rows.Add(dr); dr = dt.NewRow();
            if (obj.GetType().GetProperty("advancefilter") != null)
            {
                if (!String.IsNullOrEmpty(obj.GetType().GetProperty("advancefilter").GetValue(obj, null).ToString()))
                {
                    dr["Filter Type"] = "Add-on Filter"; dt.Rows.Add(dr); dr = dt.NewRow();
                    List<ReportLandBaseLayerAdvanceFilter> advnanceFilter = (List<ReportLandBaseLayerAdvanceFilter>)obj.GetType().GetProperty("lstAdvanceFilters").GetValue(obj, null);
                    for (int i = 0; i < advnanceFilter.Count; i++)
                    {
                        dt.Rows[7 + i][1] = textInfo.ToTitleCase(advnanceFilter[i].searchBy.ToString().ToLower().Replace("_", " ")) + " " + advnanceFilter[i].searchType + " '" + advnanceFilter[i].searchText + "'";
                        // if ((advnanceFilter.Count-1) != i)
                        //{
                        dt.Rows.Add(dr); dr = dt.NewRow();
                        //}
                    }
                }
            }
            List<int> regionIds = (List<int>)obj.GetType().GetProperty("SelectedRegionId").GetValue(obj, null);

            var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());


            List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
            var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { userId = Convert.ToInt32(Session["user_id"]) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());


            List<int> parentUser = new List<int>();
            parentUser.Add(1);
            List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
            var parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

            List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
            var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

            //rt
            var userdetails = (User)Session["userDetail"];
            List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
            var layerName = layerIds == null ? "All" : string.Join(",", new BLLandBaseLayer().getLandBaseLayers().Where(x => layerIds.Contains(x.id)).Select(x => x.layer_name).ToList());


            string durationBasedOn = obj.GetType().GetProperty("durationbasedon").GetValue(obj, null).ToString().Replace("_", " ");

            string duration = obj.GetType().GetProperty("fromDate").GetValue(obj, null).ToString() + " To " + obj.GetType().GetProperty("toDate").GetValue(obj, null).ToString();

            dt.Rows[0][1] = regionName;
            dt.Rows[1][1] = provinceName;
            dt.Rows[2][1] = parentUserName;
            dt.Rows[3][1] = userName;
            dt.Rows[4][1] = layerName;
            dt.Rows[5][1] = durationBasedOn;
            dt.Rows[6][1] = duration;
            return dt;
        }

        public void DownloadLandBaseLayerReportSummary(string fileType, string entityids, bool singleFileDownload)
        {
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
                if (fileType.ToUpper() == "EXCEL")
                {
                    DownloadLandBaseLayerReportIntoExcel(entityids);
                }
                else if (fileType.ToUpper() == "PDF")
                {
                    DownloadLandBaseReportIntoPDF(entityids);
                }
                else if (fileType.ToUpper() == "ALLEXCEL")
                {
                    DownloadLandBaseLayerReportExcelAll(entityids);
                }
                else if (fileType.ToUpper() == "KML")
                {
                    DownloadLandBaseLayerSummaryIntoKMLAll(entityids);
                }

                else if (fileType.ToUpper() == "DXF")
                {
                    DownloadLandBaseLayerSummaryIntoDXFAll(entityids);
                }
                else if (fileType.ToUpper() == "ALLSHAPE")
                {
                    DownloadLandBaseLayerReportNewIntoShapeAll(entityids, singleFileDownload);
                }
                else if (fileType.ToUpper() == "ALLCSV")
                {
                    DownloadLandBaseLayerReportIntoCSVAll(entityids);
                }
            }

        }

        public void DownloadLandBaseLayerReportIntoExcel(string entityids)
        {
            if (Session["LandBaseLayerExportSummaryData"] != null)
            {
                try
                {
                    ExportLandBaseLayerReport objExportEntitiesReport = new ExportLandBaseLayerReport();
                    objExportEntitiesReport.objReportFilters = (ExportLandBaseLayerReportFilter)Session["ExportLandBaseLayerFilterNew"];// for filter
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportEntitiesReport.objReportFilters);

                    objExportEntitiesReport = (ExportLandBaseLayerReport)Session["LandBaseLayerExportSummaryData"];
                    List<LandBaseLayerSummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                    if (objExportEntitiesReport.objReportFilters.SelectedLayerId != null)
                        objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);
                    dtReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportEntitiesReport.lstReportData = lstRprtData;
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        dtReport.Columns.Remove("entity_id");
                        dtReport.Columns.Remove("entity_name");
                        dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                        DataRow row = dtReport.NewRow();
                        row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                        if (dtReport.Columns.Contains("ENTITY_COUNT"))
                        {
                            row["Entity_Count"] = dtReport.Compute("Sum(ENTITY_COUNT)", "");
                            dtReport.Columns["Entity_Count"].ColumnName = "Count";
                        }
                        dtReport.Rows.Add(row);
                        ds.Tables.Add(dtReport);
                        ExportData(ds, "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public void DownloadLandBaseReportIntoPDF(string entityids)
        {
            if (Session["LandBaseLayerExportSummaryData"] != null)// ExportReportFilterNew
            {
                try
                {
                    ExportLandBaseLayerReport objExportEntitiesReport = new ExportLandBaseLayerReport();

                    objExportEntitiesReport.objReportFilters = (ExportLandBaseLayerReportFilter)Session["ExportLandBaseLayerFilterNew"];
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;

                    DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportEntitiesReport.objReportFilters);
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    objExportEntitiesReport = (ExportLandBaseLayerReport)Session["LandBaseLayerExportSummaryData"];
                    List<LandBaseLayerSummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                    if (objExportEntitiesReport.objReportFilters.SelectedLayerId != null)
                        objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();

                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);
                    dtReport.TableName = "EntitySummaryDetail";
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportEntitiesReport.lstReportData = lstRprtData;
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        dtReport.Columns.Remove("entity_id");
                        dtReport.Columns.Remove("entity_name");
                        dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;


                        DataRow row = dtReport.NewRow();
                        row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = Resources.Resources.SI_OSP_GBL_GBL_GBL_041;
                        if (dtReport.Columns.Contains("ENTITY_COUNT"))
                        {
                            row["Entity_Count"] = dtReport.Compute("Sum(ENTITY_COUNT)", "");
                            dtReport.Columns["Entity_Count"].ColumnName = "Count";
                        }
                        dtReport.Rows.Add(row);
                        ds.Tables.Add(dtReport);
                        GenerateToPDF(ds, "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"), Resources.Resources.SI_OSP_GBL_NET_RPT_127);

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadLandBaseLayerReportExcelAll(string entityids)
        {

            if (Session["ExportLandBaseLayerFilterNew"] != null)
            {
                try
                {

                    ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();

                    ExportLandBaseLayerReportFilter objExportReportFilterNew = new ExportLandBaseLayerReportFilter();

                    objExportReportFilterNew = (ExportLandBaseLayerReportFilter)Session["ExportLandBaseLayerFilterNew"];

                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportReportFilterNew);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                    {
                        objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                        //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetLandBaseExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                        lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        }
                        if (dtReport.Rows.Count > 0)
                            ds.Tables.Add(dtReport);
                    }
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                    ExportData(ds, "ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss"));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public void DownloadLandBaseLayerSummaryIntoKMLAll(string entityids)
        {
            if (Session["ExportLandBaseLayerFilterNew"] != null)
            {

                DataSet ds = new DataSet();
                ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();
                ExportLandBaseLayerReportFilter objExportReportFilterNew = new ExportLandBaseLayerReportFilter();
                objExportReportFilterNew = (ExportLandBaseLayerReportFilter)Session["ExportLandBaseLayerFilterNew"];

                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.currentPage = 0;
                objExportEntitiesReport.objReportFilters.fileType = "KML";
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;

                List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
                var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.id)).ToList();
                }
                DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportReportFilterNew);
                for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                {
                    objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                    // var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetExportLandBaseLayerSummaryViewKML(objExportEntitiesReport.objReportFilters);

                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);

                    dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                }

                objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                string TempkmlFileName = "ExportReport_KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
                KMLHelper.LandBaseDatasetToKML(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, TempkmlFileName, "", dtFilter);
            }
        }
        public void DownloadLandBaseLayerSummaryIntoDXFAll(string entityids)
        {
            if (Session["ExportLandBaseLayerFilterNew"] != null)
            {

                DataSet ds = new DataSet();
                ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();
                ExportLandBaseLayerReportFilter objExportReportFilterNew = new ExportLandBaseLayerReportFilter();
                objExportReportFilterNew = (ExportLandBaseLayerReportFilter)Session["ExportLandBaseLayerFilterNew"];

                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.currentPage = 0;
                objExportEntitiesReport.objReportFilters.fileType = "KML";
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;

                List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
                var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.id)).ToList();
                }
                DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportReportFilterNew);
                for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                {
                    objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                    //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetExportLandBaseLayerSummaryViewKML(objExportEntitiesReport.objReportFilters);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                    dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                }
                objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                string finalkml = KMLHelper.GetKmlForLandBaseLayer(ds, objExportEntitiesReport.lstLayers, ApplicationSettings.DownloadTempPath, dtFilter);
                //Create a new subfolder under the current active folder
                string newPath = Path.Combine(Server.MapPath("~/Uploads/"));


                //dataContent is byte[]
                System.IO.File.WriteAllText(newPath + "report.kml", finalkml.ToString());
                string baseFolder = newPath + "report.kml";
                string kmlFolder = newPath;
                DataTable dataTable = new DataTable();
                var converter = new Convertor(baseFolder, "", kmlFolder, "");
                var response = converter.KmltoDXFConverter(newPath, "report");

                string attachment = "attachment; filename=ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".dxf";
                Response.ClearContent();
                Response.Clear();
                Response.ContentType = "application/xml";
                Response.AddHeader("content-disposition", attachment);
                Response.Write(response.Output);
                Response.End();
            }
        }
        public void DownloadLandBaseLayerReportNewIntoShapeAll(string entityids, bool singleFileDownload)
        {
            if (Session["ExportLandBaseLayerFilterNew"] != null)
            {
                try
                {
                    DataSet ds = new DataSet();
                    ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();
                    ExportLandBaseLayerReportFilter objExportReportFilterNew = new ExportLandBaseLayerReportFilter();
                    objExportReportFilterNew = (ExportLandBaseLayerReportFilter)Session["ExportLandBaseLayerFilterNew"];

                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    objExportEntitiesReport.objReportFilters.fileType = "SHAPE";
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;

                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.id)).ToList();
                    }
                    // DataTable main = new DataTable();
                    for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                    {

                        objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                        // var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetExportLandBaseLayerSummaryViewKML(objExportEntitiesReport.objReportFilters);
                        DataTable dtReport = new DataTable();

                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        }
                        if (dtReport.Rows.Count > 0)
                            ds.Tables.Add(dtReport);
                        // main.Merge(dtReport);
                        // ds.Tables.Add(main);
                    }
                    if (singleFileDownload)
                    {
                        DataSet dtset = new DataSet();
                        DataTable main = new DataTable();
                        //// 
                        DataTable dtpoint = new DataTable();
                        dtpoint.TableName = "Point_Data";
                        DataTable dtline = new DataTable();
                        dtline.TableName = "Line_Data";
                        DataTable dtpoly = new DataTable();
                        dtpoly.TableName = "Polygon_Data";
                        for (int j = 0; j < ds.Tables.Count; j++)
                        {
                            var geomType = objExportEntitiesReport.lstLayers.Where(m => m.layer_name.ToUpper() == ds.Tables[j].TableName.ToUpper()).FirstOrDefault().geom_type;
                            if (geomType.ToUpper() == "POINT")
                            {
                                dtpoint.Merge(ds.Tables[j]);
                            }
                            else if (geomType.ToUpper() == "LINE")
                            {
                                dtline.Merge(ds.Tables[j]);
                            }
                            else if (geomType.ToUpper() == "POLYGON")
                            {
                                dtpoly.Merge(ds.Tables[j]);
                            }
                            //main.Merge(ds.Tables[j]);
                        }
                        ds = new DataSet();
                        if (dtpoint.Rows.Count > 0)
                            ds.Tables.Add(dtpoint);
                        if (dtline.Rows.Count > 0)
                            ds.Tables.Add(dtline);
                        if (dtpoly.Rows.Count > 0)
                            ds.Tables.Add(dtpoly);
                        //ds = new DataSet();
                        //ds.Merge(main); 

                    }
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                    GetShapeFile(ds, "LandBase_ExportReport");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void DownloadLandBaseLayerReportIntoCSVAll(string entityids)
        {

            if (Session["ExportLandBaseLayerFilterNew"] != null)
            {
                try
                {

                    ExportLandBaseLayerSummaryView objExportEntitiesReport = new ExportLandBaseLayerSummaryView();

                    ExportLandBaseLayerReportFilter objExportReportFilterNew = new ExportLandBaseLayerReportFilter();

                    objExportReportFilterNew = (ExportLandBaseLayerReportFilter)Session["ExportLandBaseLayerFilterNew"];

                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;

                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    DataTable dtFilter = GetLandBaseLayerExportReportFilter(objExportReportFilterNew);

                    //rt
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.id)).ToList();
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtFilter);
                    for (int i = 0; i < objExportEntitiesReport.lstLayers.Count; i++)
                    {
                        objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.lstLayers[i].layer_name;
                        // var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLandBaseLayer().GetLandBaseExportReportSummaryView(objExportEntitiesReport.objReportFilters);
                        lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        dtReport.TableName = objExportEntitiesReport.objReportFilters.layerName.ToUpper();
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                        }
                        if (dtReport.Rows.Count > 0)
                            ds.Tables.Add(dtReport);
                    }
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                    ExportCSV(ds, "ExportReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss"), ApplicationSettings.CsvDelimiter);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        #endregion

        #region  Plan tool
        public void ExportPlanBOMBOQReport(int plan_id)
        {

            if (plan_id > 0)
            {
                string plan_name = new BLPlan().GetNetworkPlanningById(plan_id).plan_name;
                string fileName = plan_name + "_Network_Planing_BomBOQ_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                List<PlanBom> lstBomReport = new BLPlan().GetPlanBomByPlanId(plan_id, user_id);

                DataTable dtReport = MiscHelper.ListToDataTable<PlanBom>(lstBomReport);
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("entity_type")) { dtReport.Columns["entity_type"].ColumnName = "Entity Type"; }
                    if (dtReport.Columns.Contains("length_qty")) { dtReport.Columns["length_qty"].ColumnName = "Length/Qty"; }
                    if (dtReport.Columns.Contains("cost_per_unit")) { dtReport.Columns["cost_per_unit"].ColumnName = "Cost Per " + String.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_016, ApplicationSettings.Currency); }
                    if (dtReport.Columns.Contains("service_cost_per_unit")) { dtReport.Columns["service_cost_per_unit"].ColumnName = "Service Cost Per " + string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_016, ApplicationSettings.Currency); }

                    //if (dtReport.Columns.Contains("service_cost_per_unit")) { dtReport.Columns["service_cost_per_unit"].ColumnName = /*Resources.Resources.SI_OSP_GBL_NET_RPT_017 + " " + */string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_017, ApplicationSettings.Currency); }
                    if (dtReport.Columns.Contains("amount")) { dtReport.Columns["amount"].ColumnName = string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency); }
                    if (dtReport.Columns.Contains("msg")) { dtReport.Columns.Remove("msg"); }
                    if (dtReport.Columns.Contains("is_template_extis")) { dtReport.Columns.Remove("is_template_extis"); }
                    if (dtReport.Columns.Contains("temp_plan_id")) { dtReport.Columns.Remove("temp_plan_id"); }
                    if (dtReport.Columns.Contains("cable_length")) { dtReport.Columns.Remove("cable_length"); }
                    if (dtReport.Columns.Contains("geometry")) { dtReport.Columns.Remove("geometry"); }
                    //if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                    //if (dtReport.Columns.Contains("system Id")) { dtReport.Columns.Remove("system Id"); }
                }
                if (dtReport.Rows.Count > 0)
                {
                    ExportData(dtReport, fileName);
                }
            }
        }
        #endregion


        public JsonResult GenerateFile(int p_entity_id)
        {

            ProcessSummary process = new ProcessSummary();
            process.ticket_id = 1;
            process.process_start_time = DateTime.Now;
            process.entity_type = EntityType.SubArea.ToString();
            process.enitity_id = p_entity_id;
            process.nas_status = "Failed";
            //process.ring_no = 0;
            var userId = Convert.ToInt32(Session["user_id"]);
            var result = new BLProcess().SaveProcessSummary(process);
            JsonResponse<string> jResp = new JsonResponse<string>();
            try
            {
                var ProcessData = new BLProcess().SaveXMLSummary(p_entity_id, EntityType.SubArea.ToString(), result.process_id, userId);


                BLDataProcessing objDataProcess = new BLDataProcessing();
                string localPath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ReportFolderPath"]);
                //Fetch File Path            
                string ftpPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string Username = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string Password = ConfigurationManager.AppSettings["FTPPasswordAttachment"];
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                // Fetch Data from database to create XML 
                //Console.WriteLine("Fetching Data from Database");
                DataTable dtStructure = objDataProcess.GetStructure(result.process_id);
                DataTable dtSpliceClosure = objDataProcess.GetSpliceclosure(result.process_id);
                DataTable dtEquipment = objDataProcess.GetEquipment(result.process_id);
                DataTable dtEquipmentChassis = objDataProcess.GetEquipmentChassis(result.process_id);
                DataTable dtTransMedia = objDataProcess.GetTransmedia(result.process_id);
                DataTable dtTransMediaUnits = objDataProcess.GetTransmediaUnits(result.process_id);
                DataTable dtConnection = objDataProcess.GetConnections(result.process_id);


                //Console.WriteLine("Creating root node of XML...");
                XmlDocument objXMLDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = objXMLDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = objXMLDoc.DocumentElement;
                objXMLDoc.InsertBefore(xmlDeclaration, root);
                //Prepare root element
                XmlElement objRootElement = objXMLDoc.CreateElement(string.Empty, "INVENTORY", string.Empty);

                //Add Namespace in Root node as attribute
                XmlAttribute attNameSpace1 = objXMLDoc.CreateAttribute("xmlns:xsd");
                attNameSpace1.Value = "http://www.w3.org/2001/XMLSchema";
                objRootElement.Attributes.Append(attNameSpace1);

                XmlAttribute attNameSpace2 = objXMLDoc.CreateAttribute("xmlns:xsi");
                attNameSpace2.Value = "http://www.w3.org/2001/XMLSchema-instance";
                objRootElement.Attributes.Append(attNameSpace2);
                //Add  Root node in Doc
                objXMLDoc.AppendChild(objRootElement);

                //Append Structure
                //Console.WriteLine("Creating nodes for entities...");
                AddEntitiesDataInXML(dtStructure, "STRUCTURE", objXMLDoc, objRootElement);
                //Append Spliceclosure
                AddEntitiesDataInXML(dtSpliceClosure, "SPLICE_CLOSURE", objXMLDoc, objRootElement);
                //Append Equipment
                AddEntitiesDataInXML(dtEquipment, "EQUIPMENT", objXMLDoc, objRootElement);
                //Append Equipment Chassis
                AddEntitiesDataInXML(dtEquipmentChassis, "CHASSIS", objXMLDoc, objRootElement);
                //Append Transmedia
                AddEntitiesDataInXML(dtTransMedia, "TRANSMEDIA", objXMLDoc, objRootElement);
                //Append Transmedia Units
                AddEntitiesDataInXML(dtTransMediaUnits, "TRANSMEDIA_UNIT", objXMLDoc, objRootElement);
                //Append Connections
                AddEntitiesDataInXML(dtConnection, "CONNECTION", objXMLDoc, objRootElement);
                //Save File in Disk.
                string sFileName = Convert.ToString(ProcessData.CsaDesignId) + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss") + ".xml";
                localPath = localPath + "\\" + sFileName;
                var TotalEntitiesCount = new BLProcess().GetProcessedEntitiesCount(process);
                process.total_entity = TotalEntitiesCount.total_entities;
                process.file_name = sFileName;
                process.file_extension = Path.GetExtension(sFileName);
                process.csa_id = ProcessData.CsaDesignId;
                process.process_end_time = DateTime.Now;
                process.created_by = Convert.ToInt32(Session["user_id"]);
                var status = "";
                var message = "";
                var FileVersion = new BLProcess().UpdateFileVersion(process);
                process.file_version = FileVersion.file_version;
                string CsaDesignIds = new BLProcess().GetEquipmentCSAID(result.process_id);
                if (string.IsNullOrEmpty(CsaDesignIds))
                {

                    jResp.message = "CSA DesignId cannot be blank!";
                    jResp.status = StatusCodes.VALIDATION_FAILED.ToString();
                    if (result.process_id > 0)
                    {
                        new BLProcess().ResetProcessedXMLDetails(result.process_id, -1 * Convert.ToInt32(Session["user_id"]));
                    }
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(process.file_version))
                {

                    jResp.message = "File version cannot be blank!";
                    jResp.status = StatusCodes.VALIDATION_FAILED.ToString();
                    if (result.process_id > 0)
                    {
                        new BLProcess().ResetProcessedXMLDetails(result.process_id, -1 * Convert.ToInt32(Session["user_id"]));
                    }
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(sFileName))
                {

                    jResp.message = "File name cannot be blank!";
                    jResp.status = StatusCodes.VALIDATION_FAILED.ToString();
                    if (result.process_id > 0)
                    {
                        new BLProcess().ResetProcessedXMLDetails(result.process_id, -1 * Convert.ToInt32(Session["user_id"]));
                    }
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                if (ApplicationSettings.IsNASEnabled)
                {

                    var userdetails = (User)Session["userDetail"];
                    NASStatusIn NASobj = new NASStatusIn();
                    NASobj.FormFile = Encoding.ASCII.GetBytes(objXMLDoc.InnerXml);
                    NASobj.CSAID = CsaDesignIds;
                    NASobj.inserted_by = userdetails.user_name;
                    NASobj.system = "Lepton";
                    NASobj.xm_file_name = sFileName;
                    NASobj.version_name = process.file_version;
                    string _url = "api/main/NEXMLImportToNAS";
                    var response = WebAPIRequest.PostIntegrationAPIRequest<NASStatusOut>(_url, NASobj, "", "");
                    status = response.results.status;
                    message = response.results.message;


                    #region Save the file to NAS shared location BY ANTRA
                    //string NASlocalPath = ConfigurationManager.AppSettings["SharedDicPath"];
                    //new NeworkFileSystemHelper().CreateFile(objXMLDoc.InnerXml, NASlocalPath, sFileName, "");
                    #endregion

                    //var userdetails = (User)Session["userDetail"];
                    //NASStatusIn obj = new NASStatusIn();
                    //obj.csa_id= ProcessData.CsaDesignId;
                    //obj.inserted_by = userdetails.user_name;
                    //obj.status = "CSA NE XML copied to NAS";
                    //obj.system = "Lepton";
                    //obj.xm_file_name = process.file_name;
                    //obj.version_name = process.file_version;
                    //string url = "api/main/UpdateXmlNASStatus";               
                    //var response = WebAPIRequest.PostIntegrationAPIRequest<NASStatusIn>(url, obj, "", "");
                    //status = response.status;
                }
                if (!string.IsNullOrEmpty(status))
                {
                    process.nas_status = status;
                }
                new BLProcess().SaveProcessSummary(process);

                //Save File on Local folder               
                //objXMLDoc.Save(localPath);
                SaveTextinFile(localPath, objXMLDoc.InnerXml);
                //Console.WriteLine("File saved successfully on configured local directory.");
                // Upload the same file to the FTP Path
                FtpPath(localPath, sFileName, ftpPath, Username, Password);


                jResp.message = String.IsNullOrEmpty(message) ? "NAS API NOT ENABLED!" : message;

                //jResp.message = message;
                jResp.status = StatusCodes.OK.ToString();
                jResp.result = sFileName;

                //Console.WriteLine("Process Completed");
                //Console.ReadLine();            
            }
            catch (Exception ex)
            {
                if (result.process_id > 0)
                {
                    new BLProcess().ResetProcessedXMLDetails(result.process_id, -1 * Convert.ToInt32(Session["user_id"]));
                }
            }
            return Json(jResp, JsonRequestBehavior.AllowGet);
        }
        public static void SaveTextinFile(string sFilePath, string sXMLText)
        {
            using (FileStream oFileStream = new FileStream(sFilePath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter oStreamWriter = new StreamWriter(oFileStream);
                oStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                oStreamWriter.Write(sXMLText);
                oStreamWriter.Flush();
                oStreamWriter.Close();
            }
        }


        /// <summary>
        /// This function is used to Add all records of provided datatable into the Doc
        /// </summary>
        /// <param name="dtRecords"></param>
        /// <param name="sEntityName"></param>
        /// <param name="objXMLDoc"></param>
        /// <param name="objRootElement"></param>
        private static void AddEntitiesDataInXML(DataTable dtRecords, string sEntityName, XmlDocument objXMLDoc, XmlElement objRootElement)
        {

            foreach (DataRow objDataRow in dtRecords.Rows)
            {
                XmlElement objStructure = objXMLDoc.CreateElement(string.Empty, sEntityName, string.Empty);
                objRootElement.AppendChild(objStructure);
                foreach (DataColumn dataColumn in dtRecords.Columns)
                {
                    string sColumnName = dataColumn.ColumnName;
                    if (sColumnName != "process_id")
                    {
                        string sColumnValue = objDataRow[dataColumn] == DBNull.Value ? string.Empty : Convert.ToString(objDataRow[dataColumn]);
                        string sColumnDataType = dataColumn.DataType.Name;
                        XmlElement objProp = objXMLDoc.CreateElement(string.Empty, sColumnName.ToUpper(), string.Empty);
                        //Format the date in required format
                        if (sColumnDataType == "DateTime")
                        {
                            sColumnValue = String.IsNullOrEmpty(sColumnValue) ? string.Empty : DateTime.Parse(sColumnValue).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                        // Handle the Geometries for cables
                        if (sColumnName.ToLower() == "geometries")
                        {
                            string[] GeometryValues = sColumnValue.Split(',');
                            if (GeometryValues.Length > 1)
                            {
                                foreach (string GeometryValue in GeometryValues)
                                {
                                    if (GeometryValue != "")
                                    {
                                        XmlElement objGeomProp = objXMLDoc.CreateElement(string.Empty, "GEOMETRY", string.Empty);
                                        objGeomProp.InnerText = EvaluateNodeValue(GeometryValue);
                                        objProp.AppendChild(objGeomProp);
                                    }
                                }
                            }
                            objStructure.AppendChild(objProp);
                        }
                        else
                        {
                            objProp.InnerText = EvaluateNodeValue(sColumnValue);
                            objStructure.AppendChild(objProp);
                        }
                    }
                }
            }
        }



        private static string EvaluateNodeValue(string sValues)
        {
            string toxml = sValues;
            if (!string.IsNullOrEmpty(toxml))
            {
                // replace literal values with entities
                //toxml = toxml.Replace(">", "&gt;");
                //toxml = toxml.Replace("<", "&lt;");
            }
            return toxml;
        }

        public FileResult DownloadXMLFile(string fileName)
        {

            string zipName = string.Empty;
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    zip.AlternateEncodingUsage = Ionic.Zip.ZipOption.AsNecessary;
                    //zip.AddDirectoryByName("Files");
                    #region Get the slected files

                    string fullPath = "", FileName = "", localPath = "";

                    fullPath = FtpUrl + "/" + fileName;
                    FileName = "/" + fileName;
                    localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + fileName + "";

                    var request = (FtpWebRequest)WebRequest.Create(fullPath);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(UserName, PassWord);
                    request.UseBinary = true;
                    try
                    {
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
                        zip.AddFile(localPath, "");
                    }
                    catch (Exception)
                    {
                    }
                    //zip.AddFile(localPath, "Files");


                    #endregion
                    zipName = String.Format("{0}.zip", fileName);
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

            return null;
        }

        public static void FtpPath(string _filePathToUpload, string filename, string ftpPath, string sUserName, string sPassword)
        {
            WebClient client = null;
            try
            {
                string _filepath = _filePathToUpload;
                string _filename = filename;
                string _ftppath = Path.Combine(ftpPath, _filename);
                client = new WebClient();
                client.Credentials = new System.Net.NetworkCredential(sUserName, sPassword);
                client.UploadFile(_ftppath, _filepath);

                //using (FileStream fs = new FileStream(_filepath, FileMode.Create))
                //{
                //    byte[] buffer = new byte[102400];
                //    int read = 0;

                //    while (true)
                //    {

                //        fs.Write(buffer, 0, read);
                //    }
                //}

                //Console.WriteLine("File uploaded on configured FTP successfully");
            }
            catch (Exception)
            {
                //Console.WriteLine("Error found while uploading File", "Log");
            }
            finally { client.Dispose(); }

        }

        public ActionResult EntityExportReportLog(ExportReportLogVM ObjExportReportLogVM, int page = 0, string sort = "", string sortdir = "", string log_type = "")
        {

             //System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => new Worker().StartProcessing(cancellationToken));
            var usrDetail = (User)Session["userDetail"];
            if (sort != "" || page != 0)
            {
                ObjExportReportLogVM.objGridAttributes = new CommonGridAttributes(); //(CommonGridAttributes)Session["printExportLog"];
            }
            var timeInteval = ApplicationSettings.PrintLogTimeInterval;
            ObjExportReportLogVM.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            ObjExportReportLogVM.objGridAttributes.currentPage = page == 0 ? 1 : page;
            ObjExportReportLogVM.objGridAttributes.sort = sort;
            ObjExportReportLogVM.objGridAttributes.orderBy = sortdir;
            ObjExportReportLogVM.ExportLog = new BLExportReportLog().GetExportExportLogList(ObjExportReportLogVM.objGridAttributes, usrDetail.user_id, timeInteval, log_type);
            ObjExportReportLogVM.objGridAttributes.totalRecord = ObjExportReportLogVM.ExportLog != null && ObjExportReportLogVM.ExportLog.Count > 0 ? ObjExportReportLogVM.ExportLog[0].totalRecords : 0;
            Session["EntityExportLog"] = ObjExportReportLogVM.objGridAttributes;
            ObjExportReportLogVM.objGridAttributes.log_type = log_type == "" ? ObjExportReportLogVM.ExportLog.Select(m => m.log_type).FirstOrDefault() : log_type;
            return PartialView("_EntityExportReportLog", ObjExportReportLogVM);
        }

        public FileResult DownloadFileFromFTP(string filePath, string fileExtension)
        {
            string localPath = string.Empty;
            try
            {
                string FtpUrl = ApplicationSettings.FTPAttachment;
                string UserName = ApplicationSettings.FTPUserNameAttachment;
                string PassWord = ApplicationSettings.FTPPasswordAttachment;

                string fullPath = FtpUrl + filePath;
                string fileNameFormatted = string.Format("Export_Report_{0}{1}", MiscHelper.getTimeStamp(), fileExtension);
                localPath = string.Concat(Server.MapPath(ApplicationSettings.AttachmentLocalPath), fileNameFormatted);

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
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameFormatted);
            }
            catch (Exception ex)
            {
                //Write error log
                ErrorLogHelper.WriteErrorLog("DownloadFileFromFTP()", "Report", ex);
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    System.IO.File.Delete(localPath);
                }
            }
            return null;

        }

        private string CreateNestedDirectoryOnFTP(string strFTPPath, string strUserName, string strPassWord, params string[] directories)
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
                            if (!IsFtpDirectoryExist(strFTPFilePath, strUserName, strPassWord))
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
                        }
                        catch (WebException ex)
                        {
                            FtpWebResponse response = (FtpWebResponse)ex.Response;
                            if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable) { response.Close(); }
                            else { throw new Exception("Error in creating directory/sub-directory!", ex); }
                        }
                    }
                }
                return strFTPFilePath;
            }
            catch { throw; }
        }

        public bool IsFtpDirectoryExist(string dirPath, string strUserName, string strPassWord)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(strUserName, strPassWord);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        public ActionResult AuditLogExportReport(ExportEntitiesReportNew objExportEntitiesReport, string IsRequestFromInfo)
        {
            var userdetails = (User)Session["userDetail"];
            var moduleAbbr = "AUDIT_HISTORY_EXRPT";
            string selectedLayers = GetReportdata(objExportEntitiesReport, userdetails, moduleAbbr);
            objExportEntitiesReport.objReportFilters.SelectedLayerIds = selectedLayers;
            BindReportDropdown(ref objExportEntitiesReport, moduleAbbr);
            return PartialView("_AuditLogExportReport", objExportEntitiesReport);
        }

        [HttpPost]
        public ActionResult AuditLogEntityExportReport(ExportEntitiesReportNew objExportEntitiesReport, string IsRequestFromInfo)
        {
            var userdetails = (User)Session["userDetail"];
            var moduleAbbr = "AUDIT_HISTORY_EXRPT";
            string selectedLayers = GetReportdata(objExportEntitiesReport, userdetails, moduleAbbr);
            objExportEntitiesReport.objReportFilters.SelectedLayerIds = selectedLayers;
            if (!string.IsNullOrEmpty(IsRequestFromInfo) && Convert.ToBoolean(IsRequestFromInfo))
            {
                objExportEntitiesReport.lstReportData = new BLLayer().GetAuditLogReportSummary(objExportEntitiesReport.objReportFilters).ToList().OrderBy(m => m.entity_name).ToList();
            }
            Session["AuditLogExportReportFilter"] = objExportEntitiesReport.objReportFilters;
            BindReportDropdown(ref objExportEntitiesReport, moduleAbbr);
            Session["AuditLogEntitySummaryData"] = objExportEntitiesReport;
            return PartialView("_AuditLogExportReport", objExportEntitiesReport);
        }
        [HttpPost]
        public JsonResult DownloadAuditLogReport(string fileType, string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {
            PageMessage objMsg = new PageMessage();
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                Response.Cookies.Add(new HttpCookie("downloadStarted", "1"));
                string ftpFilePath = ApplicationSettings.FTPAttachment;
                string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                string[] ftplogReportDirectory = new string[] { ftpFolder.Replace("/", "") };
                CreateNestedDirectoryOnFTP(ftpFilePath, ftpUserName, ftpPwd, ftplogReportDirectory);

                if (reportType == null || !reportType.Any())
                {
                    reportType = new List<string> { "ALL" };
                }

                if (fileType.ToUpper() == "EXCEL")
                {
                    DownloadAuditLogReportIntoExcel(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
                }
                else if (fileType.ToUpper() == "ALLEXCEL")
                {
                    DownloadAuditLogReportIntoExcelAll(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount, reportType);
                }
            }
            objMsg.status = ResponseStatus.OK.ToString();
            objMsg.message = "Request is processing in background.Please check the export report log page.";
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public void DownloadAuditLogReportIntoExcel(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
        {
            if (Session["AuditLogEntitySummaryData"] != null)
            {
                try
                {
                    var userdetails = (User)Session["userDetail"];
                    ExportEntitiesReportNew objExportEntitiesReport = new ExportEntitiesReportNew();
                    objExportEntitiesReport.objReportFilters = (ExportReportFilterNew)Session["AuditLogExportReportFilter"];
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Planned"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("P", "Planned");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("As-Built"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("A", "As-Built");
                    if (!objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Contains("Dormant"))
                        objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.Replace("D", "Dormant");
                    DataTable dtFilter = GetExportReportFilter(objExportEntitiesReport.objReportFilters);

                    string fileName = "AuditLogSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");

                    objExportEntitiesReport = (ExportEntitiesReportNew)Session["AuditLogEntitySummaryData"];
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        List<EntitySummaryReport> lstRprtData = objExportEntitiesReport.lstReportData;
                        if (objExportEntitiesReport.objReportFilters.SelectedLayerId != null)
                            objExportEntitiesReport.lstReportData = objExportEntitiesReport.lstReportData.Where(x => objExportEntitiesReport.objReportFilters.SelectedLayerId.Contains(x.entity_id)).ToList();
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.ListToDataTable(objExportEntitiesReport.lstReportData);
                        dtReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
                        objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                        objExportEntitiesReport.lstReportData = lstRprtData;
                        DataSet ds = new DataSet();
                        ds.Tables.Add(dtFilter);

                        int TotalEntityReport = 0;
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = fileName;
                        exportReportLog.file_type = "Excel";
                        exportReportLog.file_extension = ".xlsx";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog.log_type = "audit";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        try
                        {
                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                if (!ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns.Remove("DORMANT_COUNT");
                                }
                                dtReport.Columns.Remove("entity_id");
                                dtReport.Columns.Remove("entity_name");
                                dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
                                dtReport.Columns["planned_count"].ColumnName = "Planned";
                                dtReport.Columns["as_built_count"].ColumnName = "As-Built";
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    dtReport.Columns["dormant_count"].ColumnName = "Dormant";
                                }

                                string[] networkstatusvalues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus == null ? new string[3] { "PLANNED", "AS BUILT", "DORMANT" } : objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray();
                                if (networkstatusvalues.Length < 3)
                                {
                                    if (!networkstatusvalues.Contains("P"))
                                    {
                                        dtReport.Columns.Remove("PLANNED");
                                    }
                                    if (!networkstatusvalues.Contains("A"))
                                    {
                                        dtReport.Columns.Remove("AS-BUILT");
                                    }
                                    if (ApplicationSettings.IsDormantEnabled)
                                    {
                                        if (!networkstatusvalues.Contains("D"))
                                        {
                                            dtReport.Columns.Remove("DORMANT");
                                        }
                                    }
                                }
                                DataRow row = dtReport.NewRow();
                                row[Resources.Resources.SI_OSP_GBL_GBL_GBL_144] = "Total";
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    row["Planned"] = dtReport.Compute("Sum(Planned)", "");
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    row["As-Built"] = dtReport.Compute("Sum([As-Built])", "");
                                }
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        row["Dormant"] = dtReport.Compute("Sum(Dormant)", "");
                                    }
                                }
                                dtReport.Rows.Add(row);
                                ds.Tables.Add(dtReport);
                                if (dtReport.Columns.Contains("Planned"))
                                {
                                    totalPlannedCount = Convert.ToInt32(row["Planned"]);
                                }
                                if (dtReport.Columns.Contains("As-Built"))
                                {
                                    totalAsBuiltCount = Convert.ToInt32(row["As-Built"]);
                                }
                                if (ApplicationSettings.IsDormantEnabled)
                                {
                                    if (dtReport.Columns.Contains("Dormant"))
                                    {
                                        totalDormantCount = Convert.ToInt32(row["Dormant"]);
                                    }
                                }
                            }

                            string tempFileName = fileName + exportReportLog.file_extension;
                            string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
                            ExportData(ds, tempFileName, ftpFilePath, ftpUserName, ftpPwd);

                            exportReportLog.planned = totalPlannedCount;
                            exportReportLog.asbuilt = totalAsBuiltCount;
                            exportReportLog.dormant = totalDormantCount;
                            exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Success";
                            exportReportLog.file_location = ftpFolder + tempFileName;
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);

                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadAuditLogReportIntoExcel()", "Report", ex);
                        }
                    });

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        [System.Web.Services.WebMethod(true)]
        public void DownloadAuditLogReportIntoExcelAll(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount, List<string> reportType)
        {

            if (Session["AuditLogExportReportFilter"] != null)
            {
                try
                {
                    ExportEntitiesReportNew entityExportSummaryData = new ExportEntitiesReportNew();
                    entityExportSummaryData = (ExportEntitiesReportNew)Session["AuditLogEntitySummaryData"];
                    ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                    ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                    objExportReportFilterNew = (ExportReportFilterNew)Session["AuditLogExportReportFilter"];
                    objExportEntitiesReport.objReportFilters.connectionString = objExportReportFilterNew.connectionString;
                    objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                    objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                    objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                    objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                    objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                    objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                    objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                    objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                    objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                    objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                    objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                    objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                    objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                    objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                    objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                    objExportEntitiesReport.objReportFilters.selected_route_ids = objExportReportFilterNew.selected_route_ids;
                    objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                    objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;
                    objExportEntitiesReport.objReportFilters.currentPage = 0;
                    List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;
                    objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                    DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);
                    var userdetails = (User)Session["userDetail"];
                    objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                    if (selectedlayerids != null)
                    {
                        if (selectedlayerids.Count > 0)
                            objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).ToList();
                    }

                    string parentFolder = $"ExportReport_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_{userdetails.user_id}";
                    string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                    string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                    string directoryPath = Path.Combine(Server.MapPath(pathWithParentFolder));

                    if (Directory.Exists(directoryPath).Equals(false))
                        Directory.CreateDirectory(directoryPath);
                    string fileName = $"{dtFilter.TableName}";
                    string tempFileName = $"{directoryPath}/{dtFilter.TableName}.xlsx";
                    ExportDataNew(dtFilter, fileName, tempFileName);
                    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        ExportReportLog exportReportLog = new ExportReportLog();
                        exportReportLog.user_id = userdetails.user_id;
                        exportReportLog.export_started_on = DateTime.Now;
                        exportReportLog.file_name = parentFolder;
                        exportReportLog.file_type = "ALLEXCEL";
                        exportReportLog.file_extension = ".zip";
                        exportReportLog.status = "InProgress";
                        exportReportLog.applied_filter = JsonConvert.SerializeObject(dtFilter);
                        exportReportLog.planned = totalPlannedCount;
                        exportReportLog.asbuilt = totalAsBuiltCount;
                        exportReportLog.dormant = totalDormantCount;
                        exportReportLog.total_entity = totalPlannedCount + totalAsBuiltCount + totalDormantCount;
                        exportReportLog.log_type = "audit";
                        exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        dtFilter = null;
                        try
                        {
                            var tasks = new List<Task>();
                            foreach (var layer in objExportEntitiesReport.lstLayers)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {

                                        objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
                                        var layer_name = layer.layer_name;
                                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                                        EntitySummaryReport recordCount = entityExportSummaryData.lstReportData.Where(x => x.entity_name.ToUpper() == layer.layer_name.ToUpper()).FirstOrDefault();
                                        int total_entity_count = 0;
                                        if (recordCount != null)
                                            total_entity_count = recordCount.planned_count + recordCount.as_built_count + recordCount.dormant_count;

                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;
                                        List<Dictionary<string, string>> lstExportEntitiesDetailAdditional = null;
                                        lstExportEntitiesDetail = new BLLayer().GetAuditLogReportSummaryView(objExportEntitiesReport.objReportFilters, layer.layer_name);

                                        DataTable dtReport = new DataTable();
                                        DataTable dtReportAdditional = new DataTable();
                                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
                                        dtReportAdditional = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailAdditional, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });

                                        dtReport.TableName = layer.layer_title;
                                        dtReportAdditional.TableName = layer.layer_title;

                                        if (dtReport != null && dtReport.Rows.Count > 0)
                                        {
                                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                                            if (dtReport.Columns.Contains("Fn Get Date")) { dtReport.Columns.Remove("Fn Get Date"); }
                                        }

                                        if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                                        {
                                            if (dtReportAdditional.Columns.Contains("S_NO")) { dtReportAdditional.Columns.Remove("S_NO"); }
                                            if (dtReportAdditional.Columns.Contains("totalrecords")) { dtReportAdditional.Columns.Remove("totalrecords"); }
                                            if (dtReportAdditional.Columns.Contains("Barcode")) { dtReportAdditional.Columns.Remove("Barcode"); }
                                            if (dtReportAdditional.Columns.Contains("Fn Get Date")) { dtReportAdditional.Columns.Remove("Fn Get Date"); }
                                        }
                                        if (dtReport.Rows.Count > 0 || dtReportAdditional.Rows.Count > 0)
                                        {
                                            objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
                                            objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
                                            if (dtReport.Rows.Count > 0)
                                            {
                                                if (dtReport.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                                {
                                                    dtReport.TableName = dtReport.TableName + "_GisAttribute";
                                                    fileName = $"{dtReport.TableName}";
                                                    tempFileName = $"{parentFolder}/{dtReport.TableName}.csv";
                                                    //StreamNewCSVInFolder(dtReport, tempFileName);
                                                    StreamCSVInFolder(dtReport, tempFileName);



                                                }
                                                else
                                                {
                                                    IWorkbook workbook = new XSSFWorkbook();
                                                    fileName = $"{dtReport.TableName}";
                                                    tempFileName = $"{directoryPath}/{dtReport.TableName}.xlsx";
                                                    AuditLogDataExcelMerge(workbook, dtReport, dtReportAdditional, fileName, tempFileName);
                                                }
                                            }
                                            if (dtReportAdditional.Rows.Count > 0)
                                            {
                                                if (dtReportAdditional.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                                {
                                                    dtReportAdditional.TableName = dtReportAdditional.TableName + "_AdditionalAttribute";
                                                    tempFileName = $"{parentFolder}/{dtReportAdditional.TableName}.csv";
                                                    StreamNewCSVInFolder(dtReportAdditional, tempFileName);
                                                }
                                                else
                                                {
                                                    fileName = $"{dtReportAdditional.TableName}";
                                                    IWorkbook workbook = new XSSFWorkbook();
                                                    tempFileName = $"{directoryPath}/{dtReportAdditional.TableName}.xlsx";
                                                    AuditLogDataExcelMerge(workbook, dtReport, dtReportAdditional, fileName, tempFileName);
                                                }
                                            }

                                            exportReportLog.export_ended_on = DateTime.Now;
                                            exportReportLog.status = "Success";
                                            exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
                                            dtReport = null;
                                            dtReportAdditional = null;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw;
                                    }
                                }));
                            }
                            Task t = Task.WhenAll(tasks);
                            t.Wait();

                            string ftpServer = ApplicationSettings.FTPAttachment + ftpFolder;
                            string ftpUsername = ApplicationSettings.FTPUserNameAttachment;
                            string ftpPassword = ApplicationSettings.FTPPasswordAttachment;

                            string zipfilePath = directoryPath + ".zip";
                            string fileNameValue = parentFolder + ".zip";
                            using (var zip = new ZipFile())
                            {
                                zip.AddDirectory(directoryPath);
                                zip.Save(zipfilePath);
                            }
                            if (System.IO.File.Exists(zipfilePath))
                            {
                                string fileZipName = Path.GetFileName(zipfilePath);
                                Directory.Delete(directoryPath, true);
                            }
                            FileInfo file = new FileInfo(zipfilePath);
                            CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpServer, ftpUsername, ftpPassword);
                            System.IO.File.Delete(zipfilePath);
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                        }
                        catch (Exception ex)
                        {
                            exportReportLog.export_ended_on = DateTime.Now;
                            exportReportLog.status = "Error occurred while processing request";
                            exportReportLog = new BLExportReportLog().SaveExportReportLog(exportReportLog);
                            ErrorLogHelper.WriteErrorLog("DownloadAuditLogReportIntoExcelAll()", "Report", ex);
                            if (Directory.Exists(directoryPath).Equals(true))
                                Directory.Delete(directoryPath, true);
                        }
                    });
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("AuditLogDataExcelMerge()", "Report", ex);
                    throw ex;
                }
            }
        }
        private void AuditLogDataExcelMerge(IWorkbook workbook, DataTable dtReport, DataTable dtReportAdditional, string fileName, string tempfileName)
        {
            try
            {
                using (var exportData = new MemoryStream())
                {
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (string.IsNullOrEmpty(dtReport.TableName))
                            dtReport.TableName = "Gis_Attribute";
                        ISheet sheet1 = workbook.CreateSheet("Gis_Attribute");
                        NPOIExcelHelper.DataTableToSheet(dtReport, sheet1);
                    }
                    if (dtReportAdditional != null && dtReportAdditional.Rows.Count > 0)
                    {
                        if (string.IsNullOrEmpty(dtReportAdditional.TableName))
                            dtReportAdditional.TableName = "Additional_Attribute";
                        ISheet sheet3 = workbook.CreateSheet("Additional_Attribute");
                        NPOIExcelHelper.DataTableToSheet(dtReportAdditional, sheet3);
                    }
                    workbook.Write(exportData);
                    FileStream xfile = new FileStream(tempfileName, FileMode.Create, System.IO.FileAccess.Write);
                    workbook.Write(xfile);
                    xfile.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("AuditLogDataExcelMerge()", "Report", ex);
                throw ex;
            }
        }
        public ActionResult AuditlogExportReportLog(ExportReportLogVM ObjExportReportLogVM, int page = 0, string sort = "", string sortdir = "")
        {
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (sort != "" || page != 0)
                {
                    ObjExportReportLogVM.objGridAttributes = new CommonGridAttributes();
                }
                var timeInteval = ApplicationSettings.PrintLogTimeInterval;
                ObjExportReportLogVM.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
                ObjExportReportLogVM.objGridAttributes.currentPage = page == 0 ? 1 : page;
                ObjExportReportLogVM.objGridAttributes.sort = sort;
                ObjExportReportLogVM.objGridAttributes.orderBy = sortdir;
                ObjExportReportLogVM.ExportLog = new BLExportReportLog().GetAuditlogExportExportLogList(ObjExportReportLogVM.objGridAttributes, usrDetail.user_id, timeInteval);
                ObjExportReportLogVM.objGridAttributes.totalRecord = ObjExportReportLogVM.ExportLog != null && ObjExportReportLogVM.ExportLog.Count > 0 ? ObjExportReportLogVM.ExportLog[0].totalRecords : 0;
                Session["EntityExportLog"] = ObjExportReportLogVM.objGridAttributes;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("AuditlogExportReportLog()", "Report", ex);
                throw ex;
            }
            return PartialView("_AuditLogEntityExportReportLog", ObjExportReportLogVM);
        }
        #region Site Report
        public ActionResult ExportSiteReport(ExportEntitiesReport objExportEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {
            try
            {
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToList().Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
                objExportEntitiesReport.objReportFilters.pageSize = 10;
                objExportEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
                objExportEntitiesReport.objReportFilters.sort = sort;
                objExportEntitiesReport.objReportFilters.sortdir = sortdir;
                objExportEntitiesReport.objReportFilters.userId = Convert.ToInt32(Session["user_id"]);
                if (!string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.layerName))
                {
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLSite().GetSiteReportData(objExportEntitiesReport.objReportFilters);
                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE" };
                    foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                    {
                        var obj = (IDictionary<string, object>)new ExpandoObject();

                        foreach (var col in dic)
                        {
                            obj.Add(col.Key, col.Value);
                        }
                        objExportEntitiesReport.lstReportData.Add(obj);
                    }

                    objExportEntitiesReport.lstReportData = BLConvertMLanguage.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                    objExportEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("ExportSiteReport()", "Report", ex);
                throw ex;
            }
            Session["ExportReportFilter"] = objExportEntitiesReport.objReportFilters;
            BindReportDropdown(ref objExportEntitiesReport, EntityType.POD);
            return PartialView("_ExportSiteReport", objExportEntitiesReport);
        }

        public void BindReportDropdown(ref ExportEntitiesReport objExportEntitiesReport, EntityType entityType)
        {
            try
            {
                //Bind Regions..
                objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
                //Bind Provinces..
                if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
                {
                    objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
                }

                objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(entityType.ToString());
                objExportEntitiesReport.lstLayerDurationBasedColumns = new BLLayer().GetDurationBasedColumnName(entityType.ToString());
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("BindReportDropdown()", "Report", ex);
                throw ex;
            }

        }
        public void DownloadSiteReport(string fileType, string reportType)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(fileType))
                {
                    if (reportType.ToUpper() == "ALL" && fileType.ToUpper() == "EXCEL")
                    {
                        DownloadSiteReportIntoExcel(reportType);
                    }
                    else if (reportType.ToUpper() == "ALL" && fileType.ToUpper() == "KML")
                    {
                        DownloadSiteReportIntoKML();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadSiteReport()", "Report", ex);
            }
        }
        public void DownloadSiteReportIntoExcel(string reportType)
        {
            try
            {
                if (Session["ExportReportFilter"] != null)
                {
                    try
                    {
                        ExportReportFilter objReportFilter = (ExportReportFilter)Session["ExportReportFilter"];
                        objReportFilter.currentPage =0;
                        //Filter the Layer Detail
                        var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objReportFilter.layerName.ToUpper()).FirstOrDefault();

                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLSite().GetSiteReportData(objReportFilter);
                        lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        dtReport.TableName = layerDetail.layer_title;
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                            if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                        }

                        if (dtReport.Rows.Count > 0)
                        {
                            ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_Report_" + MiscHelper.getTimeStamp());
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogHelper.WriteErrorLog("DownloadSiteReportIntoExcel()", "Report", ex);
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadSiteReportIntoExcel()", "Report", ex);
                throw ex;
            }
        }
        public void DownloadSiteReportIntoKML()
        {
            if (Session["ExportReportFilter"] != null)
            {
                StringBuilder sbLine = new StringBuilder();
                StringBuilder sbPoint = new StringBuilder();
                StringBuilder sbPolygon = new StringBuilder();
                sbLine.Append("<Folder>");
                sbPoint.Append("<Folder>");
                sbPolygon.Append("<Folder>");
                try
                {
                    BLLayer objBLLayer = new BLLayer();
                    List<ExportReportKML> lstExportReportKML = new List<ExportReportKML>();
                    ExportReportFilter objReportFilter = (ExportReportFilter)Session["ExportReportFilter"];
                    lstExportReportKML = new BLSite().GetExportReportDataKML(objReportFilter);

                    foreach (var objEntity in lstExportReportKML)
                    {
                        if (objEntity.geom_type.ToUpper() == "POINT")
                        {
                            sbPoint.Append("<Placemark><name>" + new XText(objEntity.entity_title) + "</name>");
                            sbPoint.Append("<description>" + objEntity.entity_name + "</description>");
                            sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                            if (!string.IsNullOrEmpty(objEntity.geom))
                            {
                                sbPoint.Append(objEntity.geom);
                            }
                            sbPoint.Append("</coordinates></Point></Placemark>");
                        }
                    }

                    sbLine.Append("</Folder>");
                    sbPoint.Append("</Folder>");
                    sbPolygon.Append("</Folder>");

                    string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                               "<Document>  <!-- Begin Style Definitions -->" +
                                "<Style id =\"feasibility_id\"><LineStyle><color>" + "#FF0000FF" + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                sbPoint.ToString() + sbLine.ToString() + "</Document></kml>";

                    string attachment = "attachment; filename=export_" + lstExportReportKML[0].entity_title + ".kml";
                    Response.ClearContent();
                    Response.ContentType = "application/xml";
                    Response.AddHeader("content-disposition", attachment);
                    Response.Write(finalKMLString);
                    Response.End();
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("DownloadSiteReportIntoKML()", "Report", ex);
                    throw ex;
                }
            }
        }
        #endregion

        #region Site Awarding process 
        public ActionResult SiteAwarding(ViewUserModel objViewUser, int page = 0, string sort = "", string sortdir = "", string refrenceData = "")
        {
            try
            {

                objViewUser = GetVendorList(objViewUser, page, sort, sortdir, refrenceData);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("SiteAwarding()", "Report", ex);
                throw ex;
            }

            return PartialView("_SiteAwarding", objViewUser);
        }

        public ActionResult ItemSiteAwarding(ViewItemVendorCost objViewItemVendorCost, int page = 0, string sort = "", string sortdir = "", string refrenceData = "")
        {
            try
            {
               // ViewItemVendorCost objViewItemVendorCost = new ViewItemVendorCost();
                CommonGridAttr objGridAttributes = new CommonGridAttr();
                
                if (sort != "" || page != 0)
                {
                    objViewItemVendorCost.objGridAttributes = (CommonGridAttr)Session["ViewItemVendorCost"];
                }
                objViewItemVendorCost.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
                objViewItemVendorCost.objGridAttributes.currentPage = page == 0 ? 1 : page;
                objViewItemVendorCost.objGridAttributes.sort = sort;
                objViewItemVendorCost.objGridAttributes.orderBy = sortdir;
                objViewItemVendorCost.lstItem = new BLVendorSpecification().ItemVendorCost(objViewItemVendorCost.objGridAttributes).ToList();

                var users = objViewItemVendorCost.lstItem
                    .Select(x => new { x.user_id, x.user_name })
                    .Distinct()
                    .OrderBy(x => x.user_id) // Ensure ordering is based on user_name
                    .ToList();


                Session["ViewItemVendorCost"] = objViewItemVendorCost.objGridAttributes;
                // Transform data dynamically
                var transformedData = objViewItemVendorCost.lstItem
                        .GroupBy(x => new { x.code, x.specification, x.category_reference, x.unit_measurement, x.layer_id })
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
                                dict[$"User_{user.user_name + "/" + user.user_id}"] = costValue;
                            }

                            return row;
                        })
                    .ToList();

                ViewBag.transformedData = transformedData;


                objViewItemVendorCost.objGridAttributes.totalRecord = objViewItemVendorCost.lstItem.Select(a => a.totalRecord).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("SiteAwarding()", "Report", ex);
                throw ex;
            }

            return PartialView("_ItemSiteAwarding", objViewItemVendorCost);
        }

        private ViewUserModel GetVendorList(ViewUserModel objViewUser, int page = 0, string sort = "", string sortdir = "", string refrenceData = "")
        {
            try
            {

                if (refrenceData != "")
                {
                    Session["refrenceData"] = refrenceData;
                }
                var objLgnUsrDtl = (User)Session["userDetail"];
                string SearchVar = "";

                if (sort != "" || page != 0)
                {
                    objViewUser.objGridAttributes = (CommonGridAttributes)Session["GridAttributes"];
                }


                objViewUser.lstSearchBy = GetVendorSearchByColumns();
                objViewUser.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
                objViewUser.objGridAttributes.currentPage = page == 0 ? 1 : page;
                objViewUser.objGridAttributes.sort = sort;
                objViewUser.objGridAttributes.orderBy = sortdir;


                if ((objViewUser.objGridAttributes.searchBy == "name" || objViewUser.objGridAttributes.searchBy == "user_email")
                    && !string.IsNullOrEmpty(objViewUser.objGridAttributes.searchText))
                {
                    SearchVar = objViewUser.objGridAttributes.searchText;
                    objViewUser.objGridAttributes.searchText = "";
                    objViewUser.objGridAttributes.currentPage = 0;

                }

                var user = new BLUser().GetVendorList(objViewUser.objGridAttributes, objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);

                if (user.Count > 0)
                {
                    if (SearchVar != "" && objViewUser.objGridAttributes.searchBy == "name")
                    {
                        user = user.Where(c => MiscHelper.Decrypt(c.name).ToLower().Contains(SearchVar.ToLower())).ToList();
                        objViewUser.objGridAttributes.pageSize = 1000;
                    }
                    else if (SearchVar != "" && objViewUser.objGridAttributes.searchBy == "user_email")
                    {
                        user = user.Where(c => MiscHelper.Decrypt(c.user_email).ToLower().Contains(SearchVar.ToLower())).ToList();
                        objViewUser.objGridAttributes.pageSize = 1000;
                    }



                    foreach (var item in user)
                    {
                        item.user_email = MiscHelper.Decrypt(item.user_email);
                        item.mobile_number = MiscHelper.Decrypt(item.mobile_number);
                        item.name = MiscHelper.Decrypt(item.name);
                    }
                    objViewUser.lstUsers = user;
                }


                objViewUser.loginId_UserId = objLgnUsrDtl.is_admin_rights_enabled ? objLgnUsrDtl.user_id : 0;
                //  objViewUser.objGridAttributes.totalRecord = objViewUser.lstUsers != null && objViewUser.lstUsers.Count > 0 ?  objViewUser.lstUsers[0].totalRecords : 0;
                objViewUser.objGridAttributes.totalRecord = objViewUser.lstUsers != null && objViewUser.lstUsers.Count > 0 ? SearchVar != "" ? objViewUser.lstUsers.Count : objViewUser.lstUsers[0].totalRecords : 0;
                Session["GridAttributes"] = objViewUser.objGridAttributes;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetVendorList()", "Report", ex);
                throw ex;
            }

            return objViewUser;
        }
        public List<KeyValueDropDown> GetVendorSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "User Name", value = "user_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Name", value = "name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Email", value = "user_email" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }

        public ActionResult AwardSiteToSelectedVendor(int userId, double vendorCost = 0)
        {
            JsonResponse<string> objBom = new JsonResponse<string>();
            try
            {
                string refData = Session["refrenceData"].ToString();
                string[] data = refData.Split(',');
                int refrenceId = Convert.ToInt32(data[0]);
                int region_id = Convert.ToInt32(data[1]);
                int province_id = Convert.ToInt32(data[2]);
                var user = new BLUser().AwardSiteToSelectedVendor(refrenceId, userId, vendorCost);
                if (user.Count > 0)
                {
                    #region Network ticket
                    NetworkTicket objTicketMaster = new NetworkTicket();
                    objTicketMaster.assigned_to = userId;
                    objTicketMaster.reference_type = "GIS";
                    objTicketMaster.region_id = region_id;
                    objTicketMaster.province_id = province_id;
                    objTicketMaster.target_date = System.DateTime.Now.AddDays(30);
                    objTicketMaster.ticket_status_id = 5;// InProgress
                    objTicketMaster.for_network_type = "P";
                    objTicketMaster.ticket_type_id = 7;//Construction
                    objTicketMaster.name = "Award Site";// default name
                    objTicketMaster.pageMsg.message = new BLNetworkTicket().SaveNetworkTicket(objTicketMaster, Convert.ToInt32(Session["user_id"]));
                    // always retunrs "Save" ;
                    #endregion

                    Session["refrenceData"] = null;
                    objBom.status = ResponseStatus.OK.ToString();
                    objBom.message = "Site awarded successfully!";

                }
                else
                {
                    objBom.status = ResponseStatus.FAILED.ToString();
                    objBom.message = "Something went wrong!";
                }


            }
            catch (Exception ex)
            {
                objBom.status = ResponseStatus.FAILED.ToString();
                objBom.message = "An error occurred while processing your request.";
                ErrorLogHelper.WriteErrorLog("AwardSiteToSelectedVendor()", "Report", ex);
                throw ex;
            }

            return Json(objBom, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public void DownloadVendorDetails()
        {

            var objLgnUsrDtl = (User)Session["userDetail"];
            CommonGridAttributes objViewFilter = (CommonGridAttributes)Session["GridAttributes"];
            List<UserDetail> lstViewUserDetails = new List<UserDetail>();
            objViewFilter.currentPage = 0;
            objViewFilter.pageSize = 0;
            lstViewUserDetails = new BLUser().GetVendorList(objViewFilter, objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);

            DataTable dtReport = new DataTable();
            dtReport = MiscHelper.ListToDataTable<UserDetail>(lstViewUserDetails);


            for (int i = 0; i < dtReport.Rows.Count; i++)
            {
                dtReport.Rows[i]["user_email"] = MiscHelper.Decrypt(dtReport.Rows[i]["user_email"].ToString());
                dtReport.Rows[i]["mobile_number"] = MiscHelper.Decrypt(dtReport.Rows[i]["mobile_number"].ToString());
                dtReport.Rows[i]["name"] = MiscHelper.Decrypt(dtReport.Rows[i]["name"].ToString());
            }
            dtReport.Columns.Remove("USER_ID");
            dtReport.Columns.Remove("PASSWORD");
            dtReport.Columns.Remove("ISACTIVE");
            dtReport.Columns.Remove("MANAGER_ID");
            dtReport.Columns.Remove("ROLE_ID");
            dtReport.Columns.Remove("MODULE_ID");
            dtReport.Columns.Remove("USER_IMG");
            dtReport.Columns.Remove("TEMPLATE_ID");
            dtReport.Columns.Remove("GROUP_ID");
            dtReport.Columns.Remove("IS_DELETED");
            dtReport.Columns.Remove("REMARKS");
            dtReport.Columns.Remove("CREATED_BY");
            dtReport.Columns.Remove("TOTALRECORDS");
            dtReport.Columns.Remove("MODIFIED_BY");
            dtReport.Columns.Remove("IS_ACTIVE");
            dtReport.Columns.Remove("CREATED_ON");
            dtReport.Columns.Remove("MODIFIED_ON");
            dtReport.Columns.Remove("HISTORY_ID");
            dtReport.Columns.Remove("REPORTING_MANAGER");
            dtReport.Columns.Remove("MODIFIED_BY_TEXT");
            dtReport.Columns.Remove("CREATED_BY_TEXT");
            dtReport.Columns.Remove("APPLICATION_ACCESS");
            dtReport.Columns.Remove("role_name");
            dtReport.Columns.Remove("user_name");
            dtReport.Columns.Remove("user_type");

            dtReport.Columns["s_no"].SetOrdinal(0);
            dtReport.Columns["name"].SetOrdinal(1);
            dtReport.Columns["mobile_number"].SetOrdinal(2);
            dtReport.Columns["user_email"].SetOrdinal(3);

            dtReport.Columns["name"].ColumnName = "Name";
            dtReport.Columns["user_email"].ColumnName = "Email";
            dtReport.Columns["mobile_number"].ColumnName = "Mobile No.";
            var filename = "VendorDetails";
            ExportVendorList(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));

        }
        private void ExportVendorList(DataTable dtReport, string fileName)
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

        #region Site Topology process 
        public ActionResult SiteTopology(string systemid,PODMaster objToplogyPlan, int page = 0, string sort = "", string sortdir = "")
        {
            try
            {
                    objToplogyPlan.max_distance_peer = ApplicationSettings.MaxSitePeerDisatence;
                    objToplogyPlan.lsttopologytype = new BLMisc().GetToplogyDropDownList(DropDownType.Topology_Type.ToString());
                    objToplogyPlan.lstringtype = new BLMisc().GetToplogyDropDownList(DropDownType.Ring_Capacity.ToString());
                    objToplogyPlan.lstnoofsites = new BLMisc().GetToplogyDropDownList(DropDownType.NoOf_Sites.ToString());
                    objToplogyPlan.lstTopologyRegionMaster = new BLProject().getTopologyRegionDetails();

                if (systemid != "")
                {
                    objToplogyPlan.system_id = Convert.ToInt32(systemid); // Store in ViewBag to access in View

                    var siteInfo = new BLProject().getSiteIdName(objToplogyPlan.system_id);
                    objToplogyPlan.site_id = siteInfo.FirstOrDefault().site_id;
                    objToplogyPlan.site_name = siteInfo.FirstOrDefault().site_name;
                }
               

            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("SiteTopology()", "Report", ex);
                throw ex;
            }

            return PartialView("_SiteTopologyPlan", objToplogyPlan);
        }
        public JsonResult GetSegmentsByRegion(int id,string aggregate1, string aggregate2)
        {
            // Fetch segments based on regionId
            var segments = new BLProject().getSegmentDetailByIdList(id, aggregate1, aggregate2);
            //var segmentcode = new BLProject().GetSegmentCode();
            // Transform into key-value pairs for dropdown
            var segmentDropdownData = segments.Select(s => new
            {
                Value = s.id,      // Segment ID as value
                Text = s.segment_code      // Segment name as text getRingDetailByIdList
            }).ToList();

            // Return as JSON
            return Json(segmentDropdownData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSegmentsDetails(int regionId)
        {
           // Fetch segments based on regionId
            var segmentcode = new BLProject().GetSegmentCode();
            return Json(segmentcode, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSegmentsCode()
        {
            PageMessage objMsg = new PageMessage();
            TopologySegment TopologySegment = new TopologySegment();


            try
            {

                //pODMaster.lstTopologyRegionMaster = new BLProject().getTopologyRegionDetails();

                var segmentcode = new BLProject().GetSegmentCode();
                TopologySegment = segmentcode;
                

            }
            catch
            {
               
            }

            return Json(TopologySegment, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Gettopologygetsites(int systemId, int ringId, int distance,int segment_id)
        {
            distance = distance * 1000;// Converting killometer into meter
            PODMaster pODMaster = new PODMaster();
            pODMaster.lsttopologygetsites = new BLProject().Bindtopologygetsites(systemId, ringId, segment_id, distance, Convert.ToInt32(Session["user_id"])).ToList();
            return Json(pODMaster, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getExistingSegmentDetails(int regionId, int agg1_site_id, int agg2_site_id,string route)
        {

            segmentMaster segmentMaster = new segmentMaster();


           var Segmentdata = new BLProject().getExistingSegmentDetails( regionId, agg1_site_id, agg2_site_id, route, Convert.ToInt32(Session["user_id"]));
            if (Segmentdata.Count > 0)
            {
                segmentMaster.id = Segmentdata.FirstOrDefault().id;
                segmentMaster.sequence = Segmentdata.FirstOrDefault().sequence;
                segmentMaster.region_name = Segmentdata.FirstOrDefault().region_name;
                segmentMaster.segment_code = Segmentdata.FirstOrDefault().segment_code;
                segmentMaster.route_name = Segmentdata.FirstOrDefault().route_name;
                segmentMaster.description = Segmentdata.FirstOrDefault().description;
            }


            return Json(segmentMaster, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getSegmentDetailsRoutewise(int systemId)
        {

            PODMaster pODMaster = new PODMaster();
            pODMaster.lstsegment = new BLProject().getSegmentDetailsRoutewise(systemId, Convert.ToInt32(Session["user_id"])).ToList();
            
                var siteInfo = new BLProject().getSiteDetails(systemId, Convert.ToInt32(Session["user_id"]));
                if(siteInfo.Count>0)
                {
                    pODMaster.site_id = siteInfo.FirstOrDefault().site_id;
                    pODMaster.site_name = siteInfo.FirstOrDefault().site_name;
                    pODMaster.topology_type = siteInfo.FirstOrDefault().top_type;
                    pODMaster.ring_id = siteInfo.FirstOrDefault().ring_id;
                    pODMaster.ring = siteInfo.FirstOrDefault().ring_code;
                    pODMaster.ring_capacity = siteInfo.FirstOrDefault().ring_capacity;
                    pODMaster.region_name = siteInfo.FirstOrDefault().region_name;
                    pODMaster.segment = siteInfo.FirstOrDefault().segment_code;
                    pODMaster.agg_01 = siteInfo.FirstOrDefault().agg1_site_id;
                    pODMaster.agg_02 = siteInfo.FirstOrDefault().agg2_site_id;
                    pODMaster.no_of_sites = siteInfo.FirstOrDefault().no_of_sites;
                    pODMaster.max_distance_peer = siteInfo.FirstOrDefault().max_distance_peer;
                    pODMaster.ring_a_site_id = siteInfo.FirstOrDefault().ring_a_site_id;
                    pODMaster.ring_b_site_id = siteInfo.FirstOrDefault().ring_b_site_id;
                }
         
            
            return Json(pODMaster, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Removetopologygetsitedissociation(int basesystem_id, int systemId, int ringId, int distance)
        {
            distance = distance * 1000;// Converting killometer into meter
            PODMaster pODMaster = new PODMaster();
            // Fetch segments based on regionId
            pODMaster.lsttopologygetsites = new BLProject().Bindtopologygetsitessitedissociation(basesystem_id,systemId, ringId, distance, Convert.ToInt32(Session["user_id"])).ToList();
            // Return as JSON
            return Json(pODMaster, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetRingCode(int ring)
        //{
           
        //    var ringCode = new BLProject().GetRingCode(ring);
        //    return Json(ringCode, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetRingCode(int ring,int segmentid,string segment,string partialringcode)
        {
            PageMessage objMsg = new PageMessage();
            PODMaster pODMaster = new PODMaster();
            try
            {
                var ringCode = new BLProject().GetRingCode(ring, segmentid);
                pODMaster.site_id = segment;
                pODMaster.agg_02 = partialringcode + Convert.ToString(ringCode.ring_code);
                pODMaster.sequence = ringCode.sequence;
                if (ringCode == null)
                {

                    pODMaster.objPM.status = ResponseStatus.ERROR.ToString();
                    pODMaster.objPM.message = "Failed to fetch segment code.";

                }
                
            }catch
            {
                pODMaster.objPM.status = ResponseStatus.ERROR.ToString();
                pODMaster.objPM.message = "Failed to fetch segment code.";
            }

            return PartialView("AddRingDetails", pODMaster);
        }
        public JsonResult GetRingTypesByRegion(int segmentId, int numberofsites, string ringcapacity = "")
        {
            var ringsdata = new BLProject().getRingDetailByIdList(segmentId, numberofsites, ringcapacity);
            // Transform into key-value pairs for dropdown
            //var ringsDropdownData = ringsdata
            // .OrderBy(s => s.ring_code) // Sorts by ring_code in ascending order
            // .Select(s => new
            // {
            //     Value = s.id,
            //     Text = s.ring_code
            // })
            // .ToList();

            // Return as JSON
            return Json(ringsdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRingCodeTypesByRegion(int segmentId)
        {
            var ringsdata = new BLProject().getRingCodeDetailByIdList(segmentId);
            // Transform into key-value pairs for dropdown
            var ringsDropdownData = ringsdata
             .OrderBy(s => s.ring_code) // Sorts by ring_code in ascending order
             .Select(s => new
             {
                 Value = s.id,
                 Text = s.ring_code
             }).ToList();

            // Return as JSON
            return Json(ringsDropdownData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveSiteTopology(PODMaster pODMaster)
        {
            ModelState.Clear();
            var response = new { Success = false, Message = "Save failed" }; // Default failure response

            new BLProject().Savetopsegmentringcablemapping(pODMaster.agg1SystemId,pODMaster.agg2SystemId,Convert.ToInt32(Session["user_id"]),pODMaster.ring_id ?? 0, pODMaster.segment_id, pODMaster.top_type, pODMaster.system_id);

            int ring_id = pODMaster.ring_id ?? 0;
            pODMaster = new BLProject().updatetopology(pODMaster);

                // Check if the save operation was successful
                if (pODMaster != null)
                {
                if(pODMaster.ring_id== ring_id && pODMaster.site_id==null)
                    response = new { Success = false, Message = "Selected ring already associated !" };
                else
                response = new { Success = true, Message = "The site has been connected to the selected topology." };
                }
            

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSegmentdata(string agg1_site_id, string agg2_site_id)
        {
            TopologySegment topologySegment = new TopologySegment();
            topologySegment.agg1_site_id =  agg1_site_id;
            topologySegment.agg2_site_id = agg2_site_id;

            var siteList = new BLProject().GetSegment(topologySegment);
            //var result = siteList.Select(s => new {
            //    label = s.site_id.ToString(),
            //    value = s.site_id.ToString(),
            //    siteName = s.site_name // Include site name in response
            //}).ToList();

            return Json(siteList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getRouteConnectedElementDetail(int routeid)
        {
            JsonResponse<vmRingConnectedElementDetails> objResp = new JsonResponse<vmRingConnectedElementDetails>();

            objResp.result = new BLProject().getRouteConnectedElementDetail(routeid, Convert.ToInt32(Session["user_id"]));

            return Json(objResp.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCableRouteDetails(string regionId, string agg1_site_id, string agg2_site_id)
        {
            TopologySegment topologySegment = new TopologySegment();
            topologySegment.agg1_site_id = agg1_site_id;
            topologySegment.agg2_site_id = agg2_site_id;

            var cableList = new BLProject().GetCableRoute(topologySegment,Convert.ToInt32(Session["user_id"]));
           
            return Json(cableList, JsonRequestBehavior.AllowGet);
        }

        #region Manual Route 
        public JsonResult getManualRouteDetails( int agg1, int agg2 )
        {

            string geom = Session["routeGeom"].ToString();
            var routeList = new BLProject().GetSelectedRoute(geom, agg1, agg2, Convert.ToInt32(Session["user_id"]));

            return Json(new { status = "OK", message = "Route attached successfully!", result= routeList });
           // return Json(cableList, JsonRequestBehavior.AllowGet);
            }
        #endregion


        public JsonResult GetSiteIds(string term)
        {
            var siteList = new BLProject().getSiteIdList(term);
            var result = siteList.Select(s => new {
                label = s.site_id.ToString(),  // Displayed in dropdown
                value = s.site_id.ToString(),  // Stored in textbox
                siteName = s.site_name +" ("+ s.network_id+" )",        // Site name
                systemId = s.system_id         // Include system_id in response
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AllAGGListRoutewise(int siteid, string term)
        {
            var sitelist = new BLProject().getAllAGGListRoutewise(siteid, Convert.ToInt32(Session["user_id"]), term).ToList();

            var result = sitelist.Select(s => new
            {
                label = (s.site_id ?? "N/A") + " (" + (s.site_name ?? "Unknown") + ")",  // Correct formatting
                value = (s.site_id ?? "N/A") + " (" + (s.site_name ?? "Unknown") + ")",  // Ensuring consistency with label
                systemId = s.system_id // Handle null system_id
            }).ToList();



            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAGG1(string term)
        {           
            var sitenameList = new BLProject().getAGG1List(term);

            var result = sitenameList.Select(s => new
            {
                label = (s.site_id ?? "N/A") + " (" + (s.site_name ?? "Unknown") + ")",  // Correct formatting
                Name = (s.site_id ?? "N/A") + " (" + (s.site_name ?? "Unknown") + ")",  // Ensuring consistency with label
                systemId = s.system_id // Handle null system_id
            }).ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetAGG2(string term)
        {
            var sitenameList = new BLProject().getAGG2List(term);
            //var result = sitenameList.Select(s => new
            //{
            //    label = s.agg_02.ToString(),  // Display in dropdown
            //    value = s.agg_02.ToString(),  // Store agg_02 in textbox
            //    systemId = s.system_id        // Include system_id in response
            //}).ToList();
            var result = sitenameList.Select(s => new
            {
                label = (s.site_id ?? "N/A") + " (" + (s.site_name ?? "Unknown") + ")",  // Correct formatting
                value = (s.site_id ?? "N/A") + " (" + (s.site_name ?? "Unknown") + ")",  // Ensuring consistency with label
                systemId = s.system_id // Handle null system_id
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveRing(int segmentcode, string ringcode, string description,string ringcapacity)
        {
            ModelState.Clear();
            var response = new { Success = false, Message = "Save failed" }; // Default failure response
            TopologyRingMaster topologySegment = new TopologyRingMaster();
            topologySegment.segment_id = segmentcode;
            topologySegment.ring_code = ringcode;
            topologySegment.description = description;
            topologySegment.ring_capacity = ringcapacity;
            // Save to database
            topologySegment = new BLProject().SaveRing(topologySegment);

            if (topologySegment != null)
            {
                response = new { Success = true, Message = "Segment saved successfully" };
            }

            return Json(response);
        }

        [HttpPost]
        public JsonResult SaveSegment(string segmentcode,int region_code, string description,int Agg1SystemId,int Agg2SystemId, int route_id, int SegmentId=0, int SequenceId = 0)
        {
            try
            {
                TopologySegment topologySegment = new TopologySegment();
                // topologySegment.id = SegmentId;
                topologySegment.sequence = SequenceId;
                topologySegment.segment_code = segmentcode;
                topologySegment.region_id = region_code;
                topologySegment.agg1_site_id = Agg1SystemId.ToString();
                topologySegment.agg2_site_id = Agg2SystemId.ToString();
                topologySegment.description = description;
                topologySegment.route_id = route_id;


                // var topology_get_segment_cables= new BLProject().Gettopologysegmentcables(Agg1SystemId, Agg2SystemId, Convert.ToInt32(Session["user_id"])).ToList();

                // Save to segment database
                topologySegment = new BLProject().SaveSegment(topologySegment);

                if(topologySegment.id ==0)
                {
                    return Json(new { success = false, message = "Segment code already exist!" });
                }
                // Save to segment database
                new BLProject().Savetopsegmentcablemapping(Agg1SystemId, Agg2SystemId, Convert.ToInt32(Session["user_id"]), topologySegment.id, route_id);

                return Json(new { success = true, message = "Segment saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Create segment
        public ActionResult CreateSegment(LineEntityIn objIn)
        {
            PageMessage objMsg = new PageMessage();
            PODMaster pODMaster = new PODMaster();


            try
            {
                Session["routeGeom"] = objIn.geom;
               // pODMaster.lstroute = new BLProject().GetSelectedRoute(objIn.geom, Convert.ToInt32(Session["user_id"]));
                pODMaster.lstTopologyRegionMaster = new BLProject().getTopologyRegionDetails();

            }
            catch
            {
                pODMaster.objPM.status = ResponseStatus.ERROR.ToString();
                pODMaster.objPM.message = "Failed to fetch segment code.";
            }

            return PartialView("_AddSegment", pODMaster);
        }
        #endregion

        #region Segment details

        public ActionResult ShowSegmentReport(ExportEntitiesReport objExportEntitiesReport, int page = 1, string sort = "", string sortdir = "")
        {
            try
            {
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportEntitiesReport.objReportFilters.SelectedNetworkStatus != null && objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objExportEntitiesReport.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportEntitiesReport.objReportFilters.SelectedProvinceId != null && objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToList().Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedProvinceId.ToArray()) : "";
                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportEntitiesReport.objReportFilters.SelectedRegionId != null && objExportEntitiesReport.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objExportEntitiesReport.objReportFilters.SelectedRegionId.ToArray()) : "";
                objExportEntitiesReport.objReportFilters.pageSize = 10;
                objExportEntitiesReport.objReportFilters.currentPage = page == 0 ? 1 : page;
                objExportEntitiesReport.objReportFilters.sort = sort;
                objExportEntitiesReport.objReportFilters.sortdir = sortdir;
                objExportEntitiesReport.objReportFilters.userId = Convert.ToInt32(Session["user_id"]);
                if (!string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.layerName))
                {
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLSite().GetSegmentReportData(objExportEntitiesReport.objReportFilters);
                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE" };
                    foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                    {
                        var obj = (IDictionary<string, object>)new ExpandoObject();

                        foreach (var col in dic)
                        {
                            obj.Add(col.Key, col.Value);
                        }
                        objExportEntitiesReport.lstReportData.Add(obj);
                    }

                    objExportEntitiesReport.lstReportData = BLConvertMLanguage.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                    objExportEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("ShowSegmentReport()", "Report", ex);
                throw ex;
            }
            Session["ExportSegmentReportFilter"] = objExportEntitiesReport.objReportFilters;
            return PartialView("_ViewSegment", objExportEntitiesReport);
        }

        public void DownloadSegmentReport(string fileType, string reportType)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(fileType))
                {
                    if (reportType.ToUpper() == "ALL" && fileType.ToUpper() == "EXCEL")
                    {
                        DownloadSegmentReportIntoExcel(reportType);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadSegmentReport()", "Report", ex);
            }
        }
        public void DownloadSegmentReportIntoExcel(string reportType)
        {
            try
            {
                if (Session["ExportSegmentReportFilter"] != null)
                {
                    try
                    {
                        ExportReportFilter objReportFilter = (ExportReportFilter)Session["ExportSegmentReportFilter"];
                        objReportFilter.currentPage = 0;
                       
                        List<Dictionary<string, string>> lstExportEntitiesDetail = new BLSite().GetSegmentReportData(objReportFilter);
                        lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
                        DataTable dtReport = new DataTable();
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);
                        dtReport.TableName = "Segment";
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                            if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
                            if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                        }

                        if (dtReport.Rows.Count > 0)
                        {
                            ExportData(dtReport, "SEGMENT_Report_" + MiscHelper.getTimeStamp());
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogHelper.WriteErrorLog("DownloadSegmentReportIntoExcel()", "Report", ex);
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadSegmentReportIntoExcel()", "Report", ex);
                throw ex;
            }
        }
        #endregion

        public ActionResult SiteBomBoqSummary(ViewItemVendorCost objViewItemVendorCost, int page = 0, string sort = "", string sortdir = "", string refrenceData = "", string searchBy = "", string searchText="")
        {

            var userdetails = (User)Session["userDetail"];
            var firstItem = string.Empty;
            if (!string.IsNullOrEmpty(searchBy) || !string.IsNullOrEmpty(searchText))
            {
                firstItem = Session["systemid"].ToString();//
            }
            else 
            {
                Session["systemid"] = null;
                firstItem = refrenceData.Split(',')[0];
            }
            
            
            CommonGridAttr objGridAttributes = new CommonGridAttr();
            BindSearchBy(objViewItemVendorCost);
            if (sort != "" || page != 0)
            {
                objViewItemVendorCost.objGridAttributes = (CommonGridAttr)Session["ViewItemVendorCost"];
            }
            objViewItemVendorCost.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewItemVendorCost.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewItemVendorCost.objGridAttributes.sort = sort;
            objViewItemVendorCost.objGridAttributes.orderBy = sortdir;
            objViewItemVendorCost.objGridAttributes.searchText = searchText;
            objViewItemVendorCost.objGridAttributes.searchBy = searchBy;
            //var firstItem = Session["systemid"]!=null? Session["systemid"].ToString():refrenceData.Split(',')[0];
            var siteplanid = new BomBoq().getSiteplanid(Convert.ToInt32(firstItem));
            Session["SitePlanId"] = siteplanid;
            Session["systemid"] = firstItem;
            // objViewItemVendorCost.objGridAttributes.SelectedLayerIds =  string.Join(",", objBomBoq.objReportFilters.SelectedLayerId.ToArray());
            objViewItemVendorCost.lstItem = new BomBoq().getSiteBOMBOQReport(objViewItemVendorCost.objGridAttributes, siteplanid);

            var users = objViewItemVendorCost.lstItem.Where(u => u.user_id != 0)
                .Select(x => new { x.user_id, x.user_name })
                .Distinct()
                .OrderBy(x => x.user_id) // Ensure ordering is based on user_name
                .ToList();

            foreach (var item in users)
            {
                parentuser newUser = new parentuser
                {
                    user_id = item.user_id,
                    user_name = item.user_name
                };
                objViewItemVendorCost.lstUserDetails.Add(newUser);
            }
            //objViewItemVendorCost.lstUserDetails = users.Cast<parentuser>().ToList(); //new BLUser().GetPartnerUser().ToList();

            Session["ViewItemVendorCost"] = objViewItemVendorCost.objGridAttributes;
            // Transform data dynamically
            var transformedData = objViewItemVendorCost.lstItem.Where(u => u.user_id != 0)
                    .GroupBy(x => new { x.code, x.specification, x.category_reference, x.unit_measurement, x.totalqty,x.user_id })
                    .Select(g =>
                    {
                        dynamic row = new ExpandoObject();
                        var dict = (IDictionary<string, object>)row;

                        // Fixed properties
                        dict["code"] = g.Key.code;
                        dict["specification"] = g.Key.specification;
                        dict["entity_type"] = g.Key.category_reference;
                        dict["uom"] = g.Key.unit_measurement;
                        dict["totalqty"] = g.Key.totalqty;


                        // Dynamically add user columns
                        foreach (var user in users)
                        {
                            var costValue = g.FirstOrDefault(x => x.user_id == user.user_id)?.cost_per_unit.ToString() ?? "";
                            //dict[$"User_{user.user_name}"] = costValue+"/"+user.user_id;
                            dict[$"User_{user.user_name + "/" + user.user_id}"] = costValue;
                        }

                        return row;
                    })
                .ToList();

            ViewBag.transformedData = transformedData;


            objViewItemVendorCost.objGridAttributes.totalRecord = transformedData.Count;// objViewItemVendorCost.lstItem.Select(a => a.totalRecord).FirstOrDefault();

            return PartialView("_ItemSiteAwarding", objViewItemVendorCost);
        }

        [HttpPost]
        public JsonResult AwardSitetoUser(List<SiteAwardDetails> objivcm)
        {

            PageMessage objMsg = new PageMessage();

            SiteAwardDetails objResp = new SiteAwardDetails();
            DbMessage objDBMessage = new DbMessage();
            // var status = new BLVendorSpecification().SaveSiteAwardDetails(objivcm, Convert.ToInt32(Session["user_id"]));
            try
            {
                objDBMessage = new BLVendorSpecification().SaveSiteAwardDetails(objivcm, Convert.ToInt32(Session["user_id"]));
                var sitePlanId = objivcm.Select(a => a.site_plan_id).FirstOrDefault();
                CombineCableGeom objCombineCableGeom = new CombineCableGeom();
                objCombineCableGeom = new BLVendorSpecification().GetCombileCableGeom(sitePlanId);
                //----------------------------------------------network tickect assignment--------------------------------------
                if (objDBMessage.status)
                {
                  
                    int refrenceId = objCombineCableGeom.system_id;
                    int region_id = objCombineCableGeom.region_id;
                    int province_id = objCombineCableGeom.province_id;
                    //var user = new BLUser().AwardSiteToSelectedVendor(refrenceId, userId, vendorCost);        
                    #region Network ticket
                    NetworkTicket objTicketMaster = new NetworkTicket();
                    objTicketMaster.assigned_to = objivcm.Select(u => u.user_id).FirstOrDefault();
                    objTicketMaster.reference_type = "GIS";
                    objTicketMaster.region_id = region_id;
                    objTicketMaster.province_id = province_id;
                    objTicketMaster.target_date = System.DateTime.Now.AddDays(30);
                    objTicketMaster.ticket_status_id = 5;// InProgress
                    objTicketMaster.for_network_type = "P";
                    objTicketMaster.ticket_type_id = 7;//Construction
                    ;
                    objTicketMaster.name = "Award Site";// default name
                    objTicketMaster.network_id = objCombineCableGeom.network_id;
                    objTicketMaster.geom = objCombineCableGeom.combine_geom;
                    objTicketMaster.pageMsg.message = new BLNetworkTicket().SaveNetworkTicketfromItemVCost(objTicketMaster, Convert.ToInt32(Session["user_id"]));
                    // always retunrs "Save" ;
                    #endregion

                    Session["refrenceData"] = null;
                    Session["SitePlanId"] = sitePlanId; 
                }
                    //-------------------------------------------------end-------------------------------------------------------

                if (objDBMessage.status)
                {

                    objResp.pageMsg.status = ResponseStatus.OK.ToString();
                    objResp.pageMsg.message = objDBMessage.message;

                }
                else
                {
                    objResp.pageMsg.status = ResponseStatus.OK.ToString();
                    objResp.pageMsg.message = objDBMessage.message;

                }

            }
            catch
            {
                objResp.pageMsg.status = ResponseStatus.ERROR.ToString();
                objResp.pageMsg.message = "Some error occured  while site awarding";
            }

            //objSA.pageMsg = objDBMessage;
            return Json(objDBMessage, JsonRequestBehavior.AllowGet);

        }
        public IList<KeyValueDropDown> BindSearchBy(ViewItemVendorCost objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Item Code", value = "code" });
            items.Add(new KeyValueDropDown { key = "Specification", value = "specification" });
            //items.Add(new KeyValueDropDown { key = "Entity Type", value = "category_reference" });
            //items.Add(new KeyValueDropDown { key = "UOM", value = "unit_measurement" });
            return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();

        }
        //public void DownloadReport()
        //{
        //    try
        //    {
               
        //            try
        //            {
        //                ViewItemVendorCost objViewItemVendorCost = new ViewItemVendorCost();
        //                CommonGridAttr objGridAttributes = new CommonGridAttr();
        //            int page = 0;
        //            objViewItemVendorCost.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
        //            objViewItemVendorCost.objGridAttributes.currentPage = page == 0 ? 1 : page;
        //            objViewItemVendorCost.lstItem = new BomBoq().getSiteBOMBOQReport(objViewItemVendorCost.objGridAttributes, Convert.ToInt32(Session["SitePlanId"]));
        //                DataTable dtReport = new DataTable();
                        
        //                dtReport = MiscHelper.ListToDataTable<VendorSpecificationMaster>(objViewItemVendorCost.lstItem.Where(a=>a.user_id!=0).ToList());
        //                dtReport.TableName = "Site export Report";
        //            if (dtReport != null && dtReport.Rows.Count > 0)
        //            {
        //                List<string> columnsToKeep = new List<string> { "code", "specification", "unit_measurement", "totalqty", "user_name" };
        //                //dtReport.Columns["code"].ColumnName = "Item Code";
        //                //dtReport.Columns["specification"].ColumnName = "Specification";
        //                //dtReport.Columns["unit_measurement"].ColumnName = "UOM";
        //                //dtReport.Columns["totalqty"].ColumnName = "Quanty/Length";
        //                //dtReport.Columns["user_name"].ColumnName = "User Name";
                       
                       
        //                for (int i = dtReport.Columns.Count - 1; i >= 0; i--)
        //                {
        //                    if (!columnsToKeep.Contains(dtReport.Columns[i].ColumnName.ToLower()))
        //                    {
        //                        dtReport.Columns.RemoveAt(i);
        //                    }
        //                }
        //            }

        //                if (dtReport.Rows.Count > 0)
        //                {
        //                    ExportData(dtReport, "Site" + "_Report_" + MiscHelper.getTimeStamp());
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ErrorLogHelper.WriteErrorLog("DownloadSiteReport()", "Report", ex);
        //                throw ex;
        //            }
        //        }
            
        //    catch (Exception ex)
        //    {
        //        ErrorLogHelper.WriteErrorLog("DownloadSiteReport()", "Report", ex);
        //        throw ex;
        //    }
        //}
        public void DownloadReport()
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
                    objViewItemVendorCost.lstItem = new BomBoq().getSiteBOMBOQReport(objViewItemVendorCost.objGridAttributes, Convert.ToInt32(Session["SitePlanId"]));

                    var users = objViewItemVendorCost.lstItem.Where(a=>a.user_id!=0)
                   .Select(x => new { x.user_id, x.user_name })
                   .Distinct()
                   //.OrderByDescending(x => x.user_id) // Ensure ordering is based on user_name
                   .ToList();
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("Sheet-1");

                    var currR = 0;
                    IRow currRow = null;

                    currRow = sheet.CreateRow(0);
                    var finalData = new List<string> { "Item Code", "Specification", "UOM","Quantity/Length"};
                    finalData.AddRange(users.Select(u => Convert.ToString(u.user_name)).Distinct());

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
            for (int i = 0; i <= users.Count - 1; i++)
            {
                int columnIndex = i;
                var cell = row.CreateCell(columnIndex);
                cell.SetCellValue(users[i % ct]);
                cell.CellStyle = headerStyle;
            }





        }

        public static IWorkbook DataTableToExcelVendorCostDetails(List<VendorSpecificationMaster> filteredData, IWorkbook workbook, string extension, ISheet sheet, IRow currRow, int usercount)//arvind
        {

            var currR = 0;

            currR = currRow.RowNum;
            currRow = sheet.CreateRow(currR + 1);
            currR = currRow.RowNum;
            //-----------------------------------loops through data--------------------------------------------------------------------------------        
            var cellstyle = NPOIExcelHelper.getCellStyle(workbook);
            int col = 0; int colctn = 0;
            foreach (var (item1, index) in filteredData.GroupBy(m => m.user_id).Select(group => group.Key).Select((item, index) => (item, index)).ToList())
            {
                if (colctn == 1) { col = col + 1; colctn = 0; }
                foreach (var item in filteredData.Where(m => m.user_id == item1 && m.user_id!=0))
                {
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 0, item.code, cellstyle, true, false);
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 1, item.specification, cellstyle, true, false);
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 2, item.unit_measurement, cellstyle, true, false);
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, 3, item.totalqty.ToString(), cellstyle, true, false);
                    NPOIExcelHelper.CreateCustomCellFiberAllocation(currRow, col + 4, string.IsNullOrEmpty(Convert.ToString(item.cost_per_unit)) ? "0" : Convert.ToString(item.cost_per_unit), cellstyle, true, false);
                  
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
        [HttpPost]
        public ActionResult UploadSiteFiles(HttpPostedFileBase files1)
        {
            DataTable dtExcelData = new DataTable();
            var fileName = string.Empty;
            var filepath = string.Empty;
            var dataFileName = string.Empty;
            int userId = Convert.ToInt32(Session["user_id"]);
            HttpFileCollectionBase files = Request.Files;
            DataTable dataTable = new DataTable();
            var timeStamp = DateTimeHelper.Now.ToString("yyyyMMddHHmmssfff");
            if (files.Count > 1)
            {
               
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    fileName = Request.Files[i].FileName;
                    fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), timeStamp, Path.GetExtension(fileName));
                    filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\Buildings\\"), fileName);
                    file.SaveAs(filepath);
                }
            }
            else
            {
                HttpPostedFileBase file = files[0];
                fileName = AppendTimeStamp(Request.Files[0].FileName);
                filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\Site\\"), fileName);
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
            }
            // Optionally log the values
            List<PODMaster> lstSiteImportModel = new List<PODMaster>();
            var now = DateTime.Now; // Set once to keep consistent timestamp for all records

            var siteList = dtExcelData.AsEnumerable().Select(row => new PODMaster
            {
                site_id = row["Site ID"]?.ToString()?.Trim(),
                site_name = row["Site Name"]?.ToString()?.Trim(),

                maximum_cost = int.TryParse(row["Maximum Cost"]?.ToString(), out var maxCost) ? maxCost : (int?)null,
                project_category = row["Project Category"]?.ToString()?.Trim(),
                priority = int.TryParse(row["Priority"]?.ToString(), out var Priority) ? maxCost : (int?)null,


                cable_plan_cores = row["Cable Plan(Cores)"]?.ToString()?.Trim(),
                fiber_link_type_linkid_prefix = row["Fiber Link Type(Link ID Prefix)"]?.ToString()?.Trim(),
                comment = row["Comment"]?.ToString()?.Trim(),

                plan_cost = int.TryParse(row["Plan Cost"]?.ToString(), out var planCost) ? planCost : (int?)null,
                fiber_distance = int.TryParse(row["Fiber Distance"]?.ToString(), out var fiberDist) ? fiberDist : (int?)null,

                fiber_link_type = row["Fiber Link Type"]?.ToString()?.Trim(),
                fiber_link_code = row["Fiber Link Code"]?.ToString()?.Trim(),
                created_on = now
            }).ToList();

           
           var site=  new BLProject().updateSiteDetails(siteList);
            new BLProject().SaveSiteProjectDetails(siteList, userId);
            return Json(new { success = true, message = $"{siteList.Count} Site imported successfully." });
        }

        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(
            Path.GetFileNameWithoutExtension(fileName),
            DateTimeHelper.Now.ToString("yyyyMMddHHmmssfff"),
            Path.GetExtension(fileName)
            );

        }
        public FileResult DownloadTemplate()
        {
            string filePath = Server.MapPath("~\\Content\\Templates\\Site\\ImportSite.xlsx");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "ImportSite.xlsx";

            return File(filePath, contentType, fileName);
        }
    }
}
