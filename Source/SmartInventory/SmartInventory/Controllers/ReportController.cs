using BusinessLogics;
using Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Utility;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZXing.QrCode;
using System.Web.UI;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Globalization;
using System.Threading;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using ProjNet.CoordinateSystems;
using Ionic.Zip;
using System.Net;
using System.Web.Script.Serialization;
using Lepton.GISConvertor;
using System.Configuration;
using System.Xml;
using Lepton.Utility;
using Newtonsoft.Json;
using System.Threading.Tasks;
using NPOI.Util;
using Models.ISP;
using static iTextSharp.text.pdf.AcroFields;
using System.Net.NetworkInformation;



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
		private void ExportDataNew(DataTable dtReport, string fileName,string tempfileName)
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
		public JsonResult DownloadEntityReportNew(string fileType, string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
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
					DownloadEntityReportNewIntoExcelNew(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
				}
				else if (fileType.ToUpper() == "PDF")
				{
					//DownloadFileFromFTP(entityids);
					DownloadEntityReportNewIntoPDFNEW(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
				}
				else if (fileType.ToUpper() == "ALLEXCEL")
				{					
			    	DownloadEntityReportIntoExcelAll(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);					
				}
				else if (fileType.ToUpper() == "XML")
				{
					//DownloadEntitySummaryIntoKMLAll(entityids, fileType.ToUpper());
					DownloadEntitySummaryIntoKMLAllNew(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
				}
				else if (fileType.ToUpper() == "KML" )
				{
					//DownloadEntitySummaryIntoKMLAll(entityids, fileType.ToUpper());
					DownloadEntitySummaryIntoAllKMLNew(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
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
					DownloadEntityReportIntoShapeAllNew(entityids, totalPlannedCount, totalAsBuiltCount, totalDormantCount);
				}
				else if (fileType.ToUpper() == "ALLTXT")
				{
					DownloadEntityReportNewIntoCSVAll(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
				}
				else if (fileType.ToUpper() == "ALLCSV")
				{
					DownloadEntityReportIntoCSVAll(entityids, fileType.ToUpper(), totalPlannedCount, totalAsBuiltCount, totalDormantCount);
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
                                dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail,true,ApplicationSettings.numberFormatType,new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });
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
		public void DownloadEntityReportIntoExcelAll(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
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
										var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
										EntitySummaryReport recordCount = entityExportSummaryData.lstReportData.Where(x => x.entity_name.ToUpper()== layer.layer_name.ToUpper()).FirstOrDefault();
										int total_entity_count =0;
										if(recordCount != null)
											total_entity_count = recordCount.planned_count + recordCount.as_built_count + recordCount.dormant_count;
                                        List<Dictionary<string, string>> lstExportEntitiesDetail = null;
										if (total_entity_count > ApplicationSettings.ExcelReportLimitCount)
										{
											lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);

										}
										else
										{
											lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
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
											objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
											objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
											fileName = $"{dtReport.TableName}";
											if(dtReport.Rows.Count  > ApplicationSettings.ExcelReportLimitCount)
											{
												tempFileName = $"{parentFolder}/{dtReport.TableName}.csv";
												StreamNewCSVInFolder(dtReport, tempFileName);
											}
											else
											{
												tempFileName = $"{directoryPath}/{dtReport.TableName}.xlsx";
												ExportDataNew(dtReport, fileName, tempFileName);

											}

											exportReportLog.export_ended_on = DateTime.Now;
											exportReportLog.status = "Success";
											exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
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
		public void DownloadEntitySummaryIntoAllKMLNew(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
		{
			if (Session["ExportReportFilterNew"] != null)
			{

				//DataSet ds = new DataSet();
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
							//var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
							List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKMLNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
						    //List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKML(objExportEntitiesReport.objReportFilters);
							DataTable dtReport = new DataTable();
							dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] {""}

							dtReport.TableName = layer.layer_title;
							//if (dtReport.Rows.Count > 0)
							//	ds.Tables.Add(dtReport);


							objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
							objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;

							string fileName = layer.layer_title;

									string TempkmlFileName = fileName + ".kml";
							string finalkml = KMLHelper.GetKmlForEntityNew(dtReport, objExportEntitiesReport.lstLayers, dtFilter, TempkmlFileName, directoryPath);
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
		public void DownloadEntityReportIntoShapeAllNew(string entityids, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
		{
			if (Session["ExportReportFilterNew"] != null)
			{
				try
				{
					//LogHelper.GetInstance.WriteDebugLog($"***********************************Shape logs start ***  {DateTime.Now}********************************");
					
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
							objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).DistinctBy(o=>o.layer_id).ToList();
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
							foreach (var layer in objExportEntitiesReport.lstLayers)
							{
								tasks.Add(Task.Run(() =>
								{
									try
									{
										//LogHelper.GetInstance.WriteDebugLogTest($"====================================={layer.layer_name}====Start  {DateTime.Now}========================", layer.layer_name);
										
										objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
										//var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
										//LogHelper.GetInstance.WriteDebugLogTest($"==========================Layer Title==========={layer.layer_title}============================", layer.layer_name);
										List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportSummaryViewKMLNew(objExportEntitiesReport.objReportFilters, layer.layer_name);

										// LogHelper.GetInstance.WriteDebugLogTest($"data received from database on: {DateTime.Now}", layer.layer_name);
										// LogHelper.GetInstance.WriteDebugLogTest($"Received data count: {lstExportEntitiesDetail.Count}", layer.layer_name); 
										DataTable dtReport = new DataTable();
										dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
										dtReport.TableName = layer.layer_title;
										if (dtReport != null && dtReport.Rows.Count > 0)
										{
											if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
											if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
											if (dtReport.Columns.Contains("Barcode")) { dtReport.Columns.Remove("Barcode"); }
										}
										//if (dtReport.Rows.Count > 0)
										//	ds.Tables.Add(dtReport);
										GetShapeFileOne(dtReport, "ExportReport", ftpFilePath, ftpUserName, ftpPwd, shapeFilePath, layer.layer_name);
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
			// LogHelper.GetInstance.WriteDebugLogTest($"====Shape File END:-{dtReport.TableName} on {DateTime.Now}=====", layerName);

		}


		#endregion Export Entites Report

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
                string tempfilePath =Path.Combine(Server.MapPath(downloadTempPath + "ExportReportLog"), fileNameValue);

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


        public void DownloadEntityReportNewIntoCSVAll(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
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
								// var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();
								List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryView(objExportEntitiesReport.objReportFilters);
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
							objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
							objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;

							string tempFileName = fileName + exportReportLog.file_extension;
							//string ftpFolder = "ExportReportLog/";
							//string tempFileName = "ExportReportLog/" + fileName + "." + exportReportLog.file_type;
							string ftpFilePath = ApplicationSettings.FTPAttachment + ftpFolder;
							string ftpUserName = ApplicationSettings.FTPUserNameAttachment;
							string ftpPwd = ApplicationSettings.FTPPasswordAttachment;
							ExportReportCSVNew(ds, tempFileName, ApplicationSettings.CsvDelimiter, fileType, ftpFilePath, ftpUserName, ftpPwd);
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



		public void DownloadEntityReportIntoCSVAll(string entityids, string fileType, int totalPlannedCount, int totalAsBuiltCount, int totalDormantCount)
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
										List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
										//LogHelper.GetInstance.WriteDebugLogTest($"data received from database on: {DateTime.Now}", layer.layer_name);
										//LogHelper.GetInstance.WriteDebugLogTest($"Received data count: {lstExportEntitiesDetail.Count}", layer.layer_name);
										if (lstExportEntitiesDetail.Count > 0)
										{
											//lstExportEntitiesDetail = BLConvertMLanguage.ExportMultilingualConvert(lstExportEntitiesDetail);
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
												objExportEntitiesReport.objReportFilters.SelectedLayerId = SelectedLayerId;
												objExportReportFilterNew.SelectedLayerId = SelectedLayerIdSummary;
												fileName = $"{parentFolder}/{dtReport.TableName}.csv";
												StreamNewCSVInFolder(dtReport, fileName);
												exportReportLog.export_ended_on = DateTime.Now;
												exportReportLog.status = "Success";
												exportReportLog.file_location = ftpFolder + parentFolder + exportReportLog.file_extension;
											}
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
					if (dtReport.Columns.Contains("service_cost_per_unit")) { dtReport.Columns["service_cost_per_unit"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_017 + " " + string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_016, ApplicationSettings.Currency); }
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

		public ActionResult EntityExportReportLog(ExportReportLogVM ObjExportReportLogVM, int page = 0, string sort = "", string sortdir = "")
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
			ObjExportReportLogVM.ExportLog = new BLExportReportLog().GetExportExportLogList(ObjExportReportLogVM.objGridAttributes, usrDetail.user_id, timeInteval);
			ObjExportReportLogVM.objGridAttributes.totalRecord = ObjExportReportLogVM.ExportLog != null && ObjExportReportLogVM.ExportLog.Count > 0 ? ObjExportReportLogVM.ExportLog[0].totalRecords : 0;
			Session["EntityExportLog"] = ObjExportReportLogVM.objGridAttributes;
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


	}
}
