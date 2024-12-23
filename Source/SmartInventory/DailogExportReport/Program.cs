using BusinessLogics;
using DailogExportReport.Constants;
using DailogExportReport.Helper;
using Ionic.Zip;
using Models;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProjNet.CoordinateSystems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utility;

namespace DailogExportReport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            int userId = int.Parse(ConfigurationManager.AppSettings["UserId"]);
            int roleId = int.Parse(ConfigurationManager.AppSettings["RoleId"]);

            ExportReportFilterNew filter = new ExportReportFilterNew
            {
                connectionString = connectionString,
                userId = userId,
                roleId = roleId
            };
            ReportDownload reportDownload = new ReportDownload();
            List<EntitySummaryReport> reportSummary = reportDownload.GetExportReportSummary(filter);
            string entityIds = string.Join(",", reportSummary.Select(report => report.entity_id.ToString()));

            reportDownload.DownloadEntityReportIntoShapeAllNew(entityIds);
        }

    }
    public class ReportDownload
    {
        string ftpFolder = "ExportReportLog/";
        public List<layerDetail> listLayerDetails { get; set; }
        public List<EntitySummaryReport> GetExportReportSummary(ExportReportFilterNew objReportFilter)
        {
            var result = new List<EntitySummaryReport>();

            try
            {
                using (var connection = new NpgsqlConnection(objReportFilter.connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand("fn_get_export_report_summary", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        command.Parameters.AddWithValue("p_regionids", objReportFilter.SelectedRegionIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_provinceids", objReportFilter.SelectedProvinceIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_networkstatues", objReportFilter.SelectedNetworkStatues ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_parentusers", objReportFilter.SelectedParentUsers ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_userids", objReportFilter.SelectedUserIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_layerids", objReportFilter.SelectedLayerIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_projectcodes", objReportFilter.SelectedProjectIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_planningcodes", objReportFilter.SelectedPlanningIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_workordercodes", objReportFilter.SelectedWorkOrderIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_purposecodes", objReportFilter.SelectedPurposeIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_durationbasedon", objReportFilter.durationbasedon ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_fromdate", objReportFilter.fromDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_todate", objReportFilter.toDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_geom", objReportFilter.geom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_userid", objReportFilter.userId);
                        command.Parameters.AddWithValue("p_roleid", objReportFilter.roleId);
                        command.Parameters.AddWithValue("p_is_all_provience_assigned", objReportFilter.is_all_provience_assigned);
                        command.Parameters.AddWithValue("p_ownership_type", objReportFilter.SelectedOwnerShipType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_thirdparty_vendor_ids", objReportFilter.SelectedThirdPartyVendorIds ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_radious", objReportFilter.radius);
                        command.Parameters.AddWithValue("p_route", objReportFilter.selected_route_ids ?? (object)DBNull.Value);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var report = new EntitySummaryReport
                                {
                                    entity_id = reader.GetInt32(0),
                                    entity_title = reader.GetString(1),
                                    entity_name = reader.GetString(2)
                                };

                                result.Add(report);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return result;
        }
        public void DownloadEntityReportIntoShapeAllNew(string entityids)
        {

            try
            {
                ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                ExportReportFilterNew objExportReportFilterNew = new ExportReportFilterNew();
                ExportEntitiesReportNew entityExportSummaryData = new ExportEntitiesReportNew();

                var moduleAbbr = "EXRPT";
                ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);
                if (con != null)
                {
                    objExportEntitiesReport.objReportFilters.connectionString = con.connection_string;
                }
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
                objExportEntitiesReport.objReportFilters.userId = 5;
                objExportEntitiesReport.objReportFilters.roleId = 2;
                objExportEntitiesReport.objReportFilters.radius = objExportReportFilterNew.radius;
                objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;




                objExportEntitiesReport.objReportFilters.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportEntitiesReport.objReportFilters.SelectedLayerId;
                objExportReportFilterNew.SelectedLayerId = (!String.IsNullOrEmpty(entityids)) ? entityids.Split(',').Select(int.Parse).ToList() : objExportReportFilterNew.SelectedLayerId;

                List<int> SelectedLayerId = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                List<int> SelectedLayerIdSummary = objExportReportFilterNew.SelectedLayerId;

                objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(2, "ENTITY");
                var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerId;
                if (selectedlayerids != null)
                {
                    if (selectedlayerids.Count > 0)
                        objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id)).DistinctBy(o => o.layer_id).ToList();
                }
                DataTable dtFilter = GetExportReportFilter(objExportReportFilterNew);
                string shapeFilePath = "";

                try
                {
                    string tempFileName = String.Empty;
                    string ftpFilePath = ConfigurationManager.AppSettings["FTPAttachment"] + ftpFolder;
                    string ftpUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                    string ftpPwd = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                    string parentFolder = $"Shape_{DateTimeHelper.Now.ToString("ddMMyyyy")}-{DateTimeHelper.Now.ToString("HHmmssfff")}_1";
                    string attachmentLocalPath = Path.Combine(ConfigurationManager.AppSettings["AttachmentLocalPath"], ftpFolder);
                    string pathWithParentFolder = Path.Combine(attachmentLocalPath, parentFolder);
                    //string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string basePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, attachmentLocalPath);
                    shapeFilePath = Path.Combine(basePath, parentFolder);

                    if (Directory.Exists(shapeFilePath).Equals(false))
                        Directory.CreateDirectory(shapeFilePath);

                    var tasks = new List<Task>();

                    listLayerDetails = new BLLayer().GetLayerDetails();
                    foreach (var layer in objExportEntitiesReport.lstLayers)
                    {
                        try
                        {
                            objExportEntitiesReport.objReportFilters.layerName = layer.layer_name;
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

                            var layerdetails = listLayerDetails.Where(x => x.layer_name.ToUpper() == objExportEntitiesReport.objReportFilters.layerName.ToUpper()).FirstOrDefault();

                            if (layerdetails.is_dynamic_control_enable == null)
                            {
                                layerdetails.is_dynamic_control_enable = false;
                            }

                            lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewCSV(objExportEntitiesReport.objReportFilters, layer.layer_name);
                            lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryViewNew(objExportEntitiesReport.objReportFilters, layer.layer_name);
                            DataTable dtReportShape = new DataTable();
                            DataTable dtReport = new DataTable();

                            dtReportShape = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetailShape);//,true,ApplicationSettings.numberFormatType,new string[] { ""}
                            dtReport = MiscHelper.GetDataTableFromDictionaries(lstExportEntitiesDetail, true, ApplicationSettings.numberFormatType, new string[] { "Latitude", "Longitude", "Item Code", "Region ID", "Province ID", "Created By ID", "Source Ref ID", "Status Updated By", "Modified By", "created_by" });

                            dtReportShape.TableName = layer.layer_title;
                            dtReport.TableName = layer.layer_title;

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

                            if (dtReport.Rows.Count > 0 )
                            {
                                if (dtReport.Rows.Count > ApplicationSettings.ExcelReportLimitCount)
                                {
                                    dtReport.TableName = dtReport.TableName + "_GisAttribute";
                                    tempFileName = $"{parentFolder}/{dtReport.TableName}.csv";
                                    StreamNewCSVInFolder(dtReport, tempFileName);
                                }

                                fileName = $"{parentFolder}/{layer.layer_title}.xlsx";
                                var tempshapeFilePath = $"{shapeFilePath}/{layer.layer_title}.xlsx";
                                ExportDataExcelMergeWithoutCdb(dtReport, null, fileName, tempshapeFilePath);
                                //NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                            }
                            dtReportShape = null;
                            dtReport = null;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
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


                    FTPFileUpload(zipshapePath, (tempFileName + ".zip"), ftpFilePath, ftpUserName, ftpPwd);

                    System.IO.File.Delete(zipshapePath);

                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("DownloadEntityReportIntoShapeAllNew()", "Report", ex);

                    if (Directory.Exists(shapeFilePath).Equals(true))
                        Directory.Delete(shapeFilePath, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        public string FTPFileUpload(string filePathToUpload, string filename, string ftpPath, string sUserName, string sPassword)
        {

            WebClient client = null;
            try
            {

                string _filepath = filePathToUpload;
                string _filename = filename;
                string _ftppath = System.IO.Path.Combine(ftpPath, _filename);
                client = new WebClient();
                client.Credentials = new System.Net.NetworkCredential(sUserName, sPassword);
                client.UploadFile(_ftppath, _filepath);
                Console.WriteLine("File uploaded on configured FTP successfully");

            }

            catch (Exception)

            {

                Console.WriteLine("Error found while uploading File", "Log");

            }

            finally { client.Dispose(); }
            return "";
        }
        public void GetShapeFileOne(DataTable dtReport, string fileNameValue, string ftpfilePath, string ftpUserName, string ftpPassword, string shapeFilePath, string layerName)
        {
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

                var outGeomFactory = GeometryFactory.Default;
                var writer = new ShapefileDataWriter(shapeFileName, outGeomFactory);
                var outDbaseHeader = ShapefileDataWriter.GetHeader(features[0], features.Count);
                writer.Header = outDbaseHeader;
                writer.Write(features);


                using (var streamWriter = new StreamWriter(shapeFilePrjName))
                {
                    streamWriter.Write(GeographicCoordinateSystem.WGS84.WKT);
                }
            }

            dtReport.Clear();
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
            catch (WebException)
            {
                return false;
            }
        }
        public void StreamNewCSVInFolder(DataTable dataTable, string remoteFilePath)
        {
            try
            {
                // Combine base path and remote file path
                //string attachmentLocalPath = Path.Combine(ApplicationSettings.AttachmentLocalPath, ftpFolder);
                //string completePath = Path.Combine(attachmentLocalPath, remoteFilePath);
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;


                // Define the local attachment path relative to the application root
                string attachmentLocalPath = Path.Combine(projectDirectory, ApplicationSettings.AttachmentLocalPath, ftpFolder);

                // Combine with the remote file path to get the complete path
                string completePath = Path.Combine(attachmentLocalPath, remoteFilePath);

                // Ensure the directory exists
                string directoryPath = Path.GetDirectoryName(completePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Open a StreamWriter to write the CSV file
                using (StreamWriter writer = new StreamWriter(completePath))
                {
                    // Write column headers
                    bool isFirstRow = true;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (isFirstRow)
                        {
                            // Write column headers only once (assuming the first row is for headers)
                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                writer.Write($"\"{dataTable.Columns[i].ColumnName}\"");
                                if (i < dataTable.Columns.Count - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();
                            isFirstRow = false;
                        }

                        // Write data rows
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            writer.Write($"\"{row[i].ToString()}\"");
                            if (i < dataTable.Columns.Count - 1)
                                writer.Write(",");
                        }
                        writer.WriteLine();
                    }
                }

                Console.WriteLine($"CSV file created successfully at: {completePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public DataTable GetExportReportFilter(object obj)
        {
            //var userdetails = (Models.User)Session["userDetail"];
            //var isAttr = ((List<string>)Session["ApplicableModuleList"]);
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
            //if (isAttr.Contains("PROJ"))
            //{
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_324; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_074; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_076; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_010; dt.Rows.Add(dr); dr = dt.NewRow();
            dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_011; dt.Rows.Add(dr); dr = dt.NewRow();
            //}
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
            var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(5) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());

            List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
            var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = string.Join(",", regionIds), userId = Convert.ToInt32(5) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());

            string networkStatus = ""; //textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedNetworkStatues").GetValue(obj, null).ToString().Replace("AS BUILT", "AS-BUILT").ToLower()).Replace("'", "");

            string ownershipType = "";//textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedOwnerShipType").GetValue(obj, null).ToString()).Replace("'", "");
            //ownershipType = string.IsNullOrEmpty(ownershipType) ? "All" : ownershipType;

            List<int> thirdPartyVendorId = (List<int>)obj.GetType().GetProperty("SelectedThirdPartyVendorId").GetValue(obj, null);
            var thirdPartyVendorName = thirdPartyVendorId == null ? "All" : string.Join(",", BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList().Where(x => thirdPartyVendorId.Contains(Convert.ToInt32(x.key))).Select(x => x.value).ToList());

            List<int> parentUser = new List<int>();
            List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
            var parentUserName = string.Empty;
            //if (userdetails.role_id == 1)
            //{
            //    parentUser.Add(1);
            //    parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
            //}
            //else
            //{
            parentUserName = parentUserIds == null ? "All" : new BLUser().GetUserDetailByID(5).user_name;
            //}

            List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
            var userName = "All"; //userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

            //rt
            //var userdetails = (User)Session["userDetail"];
            List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
            var layerName = layerIds == null ? "All" : string.Join(",", new BLLayer().GetReportLayers(2, "ENTITY").Where(x => layerIds.Contains(x.layer_id)).Select(x => x.layer_title).ToList());
            var projectStatus = "";
            var projectCodeName = "";
            var planningCodeName = "";
            var workOrderCodeName = "";
            var purposeCodeName = "";
            ////if (isAttr.Contains("PROJ"))
            // {
            //string projectStatusName = obj.GetType().GetProperty("SelectedProjectStatuses").GetValue(obj, null).ToString().Replace("'", "");
            projectStatus = "All";//string.IsNullOrEmpty(projectStatusName) ? "All" : projectStatusName;

            //List<int> projectCodeIds = (List<int>)obj.GetType().GetProperty("SelectedProjectId").GetValue(obj, null);
            projectCodeName = "All";//projectCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrEmpty(networkStatus) ? "P" : networkStatus.ToUpper() == "PLANNED" ? "P" : networkStatus.ToUpper() == "AS BUILT" ? "A" : networkStatus.ToUpper() == "DORMANT" ? "D" : "P").Where(x => projectCodeIds.Contains(x.system_id)).Select(x => x.project_code).ToList());

            //List<int> planningCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPlanningId").GetValue(obj, null);
            planningCodeName = "All";//planningCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(projectCodeIds).Where(x => planningCodeIds.Contains(x.system_id)).Select(x => x.planning_code).ToList());

            //List<int> workOrderCodeIds = (List<int>)obj.GetType().GetProperty("SelectedWorkOrderId").GetValue(obj, null);
            workOrderCodeName = "All";//workOrderCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(planningCodeIds).Where(x => workOrderCodeIds.Contains(x.system_id)).Select(x => x.workorder_code).ToList());

            List<int> purposeCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPurposeId").GetValue(obj, null);
            purposeCodeName = "All";//purposeCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(workOrderCodeIds).Where(x => purposeCodeIds.Contains(x.system_id)).Select(x => x.purpose_code).ToList());
            // }
            string duration = "";
            string durationBasedOn = "Created On"; //obj.GetType().GetProperty("durationbasedon").GetValue(obj, null).ToString().Replace("_", " ");
            if (obj.GetType().GetProperty("fromDate").GetValue(obj, null) != null && obj.GetType().GetProperty("toDate").GetValue(obj, null) != null)
            {
                duration = "All";//obj.GetType().GetProperty("fromDate").GetValue(obj, null).ToString() + " To " + obj.GetType().GetProperty("toDate").GetValue(obj, null).ToString();
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

            ////if (isAttr.Contains("PROJ"))
            ////{
            dt.Rows[8][1] = projectStatus;
            dt.Rows[9][1] = projectCodeName;
            dt.Rows[10][1] = planningCodeName;
            dt.Rows[11][1] = workOrderCodeName;
            dt.Rows[12][1] = purposeCodeName;
            dt.Rows[13][1] = durationBasedOn;
            dt.Rows[14][1] = duration;
            ///}
            //else
            //{
            //    dt.Rows[8][1] = durationBasedOn;
            //    dt.Rows[9][1] = duration;
            //}
            return dt;
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
                workbook.Write(exportData);

               // Write the MemoryStream to the file
                //FileStream xfile = new FileStream(tempfileName, FileMode.Create, System.IO.FileAccess.Write);

                //workbook.Write(xfile);
                //xfile.Close();

            }
        }
        
    }
}
