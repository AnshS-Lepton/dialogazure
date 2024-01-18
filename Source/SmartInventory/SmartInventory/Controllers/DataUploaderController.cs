using BusinessLogics;
using DataUploader;
using Ionic.Zip;
using Lepton.GISConvertor;
using Models;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NPOI.SS.UserModel;
using ProjNet.CoordinateSystems;
using SharpKml.Engine;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;
using Microsoft.SqlServer.Server;
using NPOI.XSSF.Model;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Lepton.Utility;
using Models;
using Models.TempUpload;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class DataUploaderController : Controller
    {
        //private UploadStatus uploadStatus;
        private readonly TempToMainTable tempToMainTable;
        private readonly UploadExcelOrKML uploadExcelOrKML;
        private readonly BLDataUploader blDataUploader;
        public DataTable dataTable;
        public DataUploaderController()
        {
            tempToMainTable = new TempToMainTable();
            tempToMainTable.UploadStatusEventTemp += GetStatus;
            uploadExcelOrKML = new UploadExcelOrKML();
            uploadExcelOrKML.UploadExcelOrKMLEvent += GetStatus;
            blDataUploader = new BLDataUploader();
            blDataUploader.DataUploaderNotifyEventHandler += GetStatus;
            //uploadStatus = UploadStatus.Instance;
        }
        public void GetStatus(dynamic response)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var data = jsonSerializer.Deserialize<UploadSummary>(response.ToString());
            SmartInventoryHub smartInventoryhub = SmartInventoryHub.Instance;
            string message = data.status_message;
            //SmartInventoryHub smartInventoryhub = new SmartInventoryHub(response, User.Identity.Name);
            //smartInventoryhub.BroadCastUploadStatus(message);

            NotificationOutPut objNotification = new NotificationOutPut();
            objNotification.info = message;
            objNotification.notificationType = notificationType.Upload.ToString();
            smartInventoryhub.BroadCastInfo(objNotification);
            smartInventoryhub.BroadCastUploadStatus(message);

        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object 
            ErrorMessage status = new ErrorMessage();
            UploadSummary summary = new UploadSummary();
            string entityname = Request.Form["entity"].ToString();

            summary.entity_type = entityname;
            EntityType EnumEntityType = (EntityType)Enum.Parse(typeof(EntityType), entityname);
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    summary = CheckFileExtension(files, summary);
                    if (summary.status == StatusCodes.INVALID_FILE.ToString())
                    {
                        return Json(summary);
                    }
                    string fname = String.Empty;
                    int count = 0;

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        var extension = Path.GetExtension(file.FileName).ToLower();

                        if (extension == ".dat" || extension == ".id" || extension == ".map" || extension == ".tab")
                        {
                            // Checking for Internet Explorer  
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }

                            fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);

                            file.SaveAs(fname);

                        }
                    }
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        // string fname;
                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        string FileName = Path.GetFileNameWithoutExtension(fname);
                        var extension = Path.GetExtension(file.FileName).ToLower();

                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);

                        file.SaveAs(fname);
                        //string FileName = Path.GetFileNameWithoutExtension(fname);
                        var SourceId = "";
                        if (Path.GetExtension(fname) == ".dxf")
                        {
                            SourceId = Request.Form["SourceId"];
                        }
                        else
                        {
                            SourceId = "";
                        }

                        if (extension == ".dxf" || extension == ".dat")
                        {
                            dataTable = CheckUploadedFiles(FileName, Path.GetExtension(fname).Replace(".", ""), SourceId);
                            file.SaveAs(fname);
                        }
                        int userId = Convert.ToInt32(Session["user_id"]);
                        if (!isSupportFile(fname))
                        {

                            Session["fileName"] = fname;
                            getFileData(summary.entity_type);
                            //if (!file.FileName.Contains("kml"))
                            //{

                            // dataTable = NPOIExcelHelper.ExcelToTable(fname);
                            dataTable = CommonUtility.CheckDataTableForBlankRecords(dataTable);

                            if (dataTable.Rows.Count == 0)
                            {
                                summary.file_name = file.FileName;
                                summary.entity_type = entityname;
                                summary.start_on = DateTimeHelper.Now;
                                summary.status = StatusCodes.INVALID_FILE.ToString();
                                summary.status_message = StatusCodes.FAILED.ToString();
                                summary.err_description = Resources.Resources.SI_OSP_GBL_NET_FRM_519;
                                summary.end_on = DateTimeHelper.Now;
                                //summary = blDataUploader.Save(summary);

                                return Json(summary);
                            }
                            Session["FileCoulms"] = dataTable.Columns;

                            summary.user_id = userId;
                            int maxUploadCount = 0;
                            bool ValidCount = CheckMaximumRecordAllowed(dataTable.Rows.Count, EnumEntityType, out maxUploadCount);
                            if (!ValidCount)
                            {
                                summary.file_name = file.FileName;
                                summary.entity_type = entityname;
                                summary.start_on = DateTimeHelper.Now;
                                summary.status = StatusCodes.FAILED.ToString();
                                summary.status_message = StatusCodes.FAILED.ToString();
                                summary.end_on = DateTimeHelper.Now;
                                summary.err_description = string.Format(Resources.Resources.SI_OSP_DU_NET_FRM_031, maxUploadCount);
                                //Maximum 1000 records can be uploaded at a time!
                                //summary = blDataUploader.Save(summary);

                                return Json(summary);
                            }

                        }

                        if (count == 0 && dataTable != null)
                        {
                            summary.user_id = userId;
                            summary.total_record = dataTable.Rows.Count;
                            summary.success_record = 0;
                            summary.failed_record = 0;
                            summary.other_record = 0;
                            summary.entity_type = entityname;
                            summary.start_on = DateTimeHelper.Now;
                            summary.status = StatusCodes.OK.ToString();
                            summary.file_name = file.FileName;
                            summary.execution_type = ConstantsKeys.START;
                            summary = blDataUploader.Save(summary);
                            summary.status_message = ConstantsKeys.PROCESSING;
                            count = summary.id;
                        }
                        blDataUploader.UpdateStatus(summary);
                        //status = uploadExcelOrKML.UploadExcelorKML(summary, file, fname, EnumEntityType, dataTable);
                        //if (!status.is_valid)
                        //{
                        //    summary.status_message = status.error_msg;
                        //    blDataUploader.UpdateStatus(summary);
                        //    break;
                        //}
                    }
                    //}
                    count = 0;
                    status.status = StatusCodes.OK.ToString();
                    if (status.status != StatusCodes.INVALID_INPUTS.ToString())
                    {
                        blDataUploader.UpdateStatus(summary);
                    }
                    if (status.status == StatusCodes.OK.ToString())
                    {
                        return Json(summary);
                    }
                    else
                        return Json(status);
                }
                catch (Exception ex)
                {
                    summary.status = StatusCodes.FAILED.ToString();
                    summary.status_message = StatusCodes.FAILED.ToString();
                    summary.err_description = ex.Message;
                    blDataUploader.UpdateStatus(summary);
                    ErrorLogHelper.WriteErrorLog("UploadFiles", "LibraryController", ex);
                    return Json(summary);

                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        public DataTable CheckUploadedFiles(string filename, string entityName, string SourceId)
        {

            string fileType = entityName.ToUpper() != "DXF" ? "TAB" : entityName.ToUpper();
            string fileName = filename;
            string fileInternalName = filename;
            string baseFolder = Path.Combine(Server.MapPath("~/Uploads/"));
            string projectName = "SmartPlanner";
            string kmlFolder = Path.Combine(Server.MapPath("~/Uploads/KML/"));
            string filePath = baseFolder;
            string GeoJsonFolder = Path.Combine(Server.MapPath("~/Uploads/GeoJson/"));
            int maxUploadCount;
            DataTable dataTable = new DataTable();
            var converter = new Convertor(baseFolder, projectName, kmlFolder, GeoJsonFolder);
            ConvertorResponse response = converter.getKML(fileType, filePath, fileName, fileInternalName, SourceId);
            if (response.Status == true)
            {
                dataTable = converter.readkml(response.OutputFile);
                var checkMaxrecord = CheckMaximumRecordAllowed(dataTable.Rows.Count, EntityType.Building, out maxUploadCount);
                AddOtherColumns(dataTable);
                string[] columnNames = (from dc in dataTable.Columns.Cast<DataColumn>()
                                        select dc.ColumnName).ToArray();
                string columns = string.Join(",", columnNames);
                int totalRecords = dataTable.Rows.Count;

                //UploadSummary summary = SaveUploadSummaryLog(lstFileUploadedInfo.First(), lstFileUploadedInfo.Count, totalRecords, columns, RandomUploadId);
                ////if (EntityName == EntityType.RoadCenterLine.ToString()) { SaveRoadNetwork(); }
                ////ErrorMessage status = uploadExcelOrKML.UploadFile(summary, dataTable);


                //apiResponse.status = StatusCodes.OK.ToString();
                //apiResponse.error_message = response.Message;
                //apiResponse.results = new DataUploaderModel();
                //apiResponse.results.converterResponse = response;
                //apiResponse.results.summary = summary;
                //return Json(apiResponse);
            }
            //apiResponse.status = StatusCodes.INVALID_INPUTS.ToString();
            //apiResponse.error_message = response.Message;
            //apiResponse.results = null;
            return dataTable;
            //return Json(apiResponse);
        }

        private UploadSummary SaveUploadSummaryLog(int totalFiles, int totalRecords, string columns, string RandomUploadId)
        {
            //var userId = User.GetUserId();
            int userId = Convert.ToInt32(Session["UserId"]);
            UploadSummary summary = new UploadSummary();
            ///summary.entity_type = fileStatus.EntityName;
            //summary.execution_type = UploadStep.UPLOADED.ToString();
            summary.failed_record = 0;
            // summary.file_name = fileStatus.FileName;
            // summary.file_no = totalFiles;
            // summary.internal_file_name = fileStatus.FileInternalName;
            // summary.file_type = fileStatus.FileType;
            summary.status = StatusCodes.OK.ToString();
            summary.success_record = 0;
            summary.start_on = DateTime.Now;
            summary.user_id = userId;
            summary.total = totalRecords;
            summary.total_record = totalRecords;
            summary.status_message = ConstantsKeys.UPLOADED_SUCCESSFULLY;
            summary.end_on = DateTime.Now;
            //summary.columns = columns;
            //summary.file_path = fileStatus.FilePath;
            //summary.file_extension = fileStatus.FileExtension;
            //summary.file_name_without_extension = fileStatus.FileNameWithoutExtension;
            //summary.random_id = RandomUploadId;
            //summary.plan_id = fileStatus.plan_id;
            summary = blDataUploader.Save(summary);
            return summary;
        }

        private void AddOtherColumns(DataTable dataTable)
        {
            DataColumnCollection columns = dataTable.Columns;
            if (!columns.Contains("is_valid"))
            {
                dataTable.Columns.Add("is_valid", typeof(bool));

            }
            if (!columns.Contains("error_msg"))
            {
                dataTable.Columns.Add("error_msg", typeof(string));
            }
            if (!columns.Contains("row_order"))
            {
                DataColumn colSRNo = new DataColumn("row_order", typeof(int));
                dataTable.Columns.Add(colSRNo);
                int srNo = 0;
                foreach (DataRow dr in dataTable.Rows)
                {
                    dr["row_order"] = ++srNo;
                    dr["is_valid"] = true;
                }
            }
        }
        public void getFileData(string entityType)
        {
            string geomTempColName = string.Empty;
            string latTempColName = string.Empty;
            string longTempColName = string.Empty;
            string fname = (string)Session["fileName"];
            List<Mapping> lstMapping = blDataUploader.GetMappings(entityType);

            if (entityType == EntityType.Cable.ToString() || entityType == EntityType.Duct.ToString() || entityType == EntityType.Trench.ToString() || entityType == EntityType.Microduct.ToString() || entityType == EntityType.LandBase.ToString())
            {
                geomTempColName = lstMapping.Where(m => m.DbColName.ToLower() == "sp_geometry").FirstOrDefault().TemplateColName;
            }
            else
            {
                latTempColName = lstMapping.Where(m => m.DbColName.ToLower() == "latitude").FirstOrDefault().TemplateColName;
                longTempColName = lstMapping.Where(m => m.DbColName.ToLower() == "longitude").FirstOrDefault().TemplateColName;
            }

            switch (Path.GetExtension(fname).ToUpper())
            {
                case ".KMZ":
                    {
                        string kmlPath = string.Empty;
                        using (StreamReader sr = new StreamReader(Path.Combine(Server.MapPath("~/Uploads/"), fname)))
                        {
                            using (var kmzFile = KmzFile.Open(sr.BaseStream))
                            {
                                string kml = kmzFile.ReadKml();
                                fname = Path.Combine(Server.MapPath("~/Uploads/"), Path.GetFileNameWithoutExtension(fname) + ".kml");
                                System.IO.File.WriteAllText(fname, kml);

                            }
                        }
                        dataTable = NPOIExcelHelper.KMLToTable(fname, geomTempColName, latTempColName, longTempColName, entityType);
                    }
                    break;
                case ".KML":
                    {
                        dataTable = NPOIExcelHelper.KMLToTable(fname, geomTempColName, latTempColName, longTempColName, entityType); break;
                    }
                case ".XLSX": { dataTable = NPOIExcelHelper.ExcelToTable(fname, geomTempColName); break; }
                case ".XLX": { dataTable = NPOIExcelHelper.ExcelToTable(fname, geomTempColName); break; }
                case ".SHP": { dataTable = NPOIExcelHelper.ShapeToDataTable(fname); break; }
                case ".TAB":
                    {
                        Convertor converter = new Convertor();
                        ConvertorResponse response = converter.getKMLfromTabFile(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname), Path.GetExtension(fname));
                        if (response.Status == true)
                        {
                            // dataTable = NPOIExcelHelper.KMLToTable(response.OutputFile, geomTempColName, latTempColName, longTempColName, entityType);

                            dataTable = converter.readkml(response.OutputFile);
                        }
                        break;
                    }
                //case ".DXF": { var converter = new Lepton.GISConvertor.Convertor();
                //        //dataTable = converter.}
                case ".JSON":
                    {
                        dataTable = JsonToDataTable(entityType, fname);
                        break;
                    }
            }
            ////This will remove all empty rows from datable:
            if (dataTable != null)
            {
                dataTable = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string ?? field.ToString()))).CopyToDataTable();
            }
        }
        public DataTable JsonToDataTable(string entityType, string fname)
        {
            string Json = System.IO.File.ReadAllText(fname);
            try
            {
                string JeoObject = JObject.Parse(Json).ToString();
                if (entityType.ToUpper() == EntityType.ADB.ToString().ToUpper())
                {
                    Root<TempADB> geoJsonObject = JsonConvert.DeserializeObject<Root<TempADB>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Antenna.ToString().ToUpper())
                {
                    Root<TempAntenna> geoJsonObject = JsonConvert.DeserializeObject<Root<TempAntenna>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.BDB.ToString().ToUpper())
                {
                    Root<TempBDB> geoJsonObject = JsonConvert.DeserializeObject<Root<TempBDB>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Building.ToString().ToUpper())
                {
                    Root<TempBuilding> geoJsonObject = JsonConvert.DeserializeObject<Root<TempBuilding>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Cabinet.ToString().ToUpper())
                {
                    Root<TempCabinet> geoJsonObject = JsonConvert.DeserializeObject<Root<TempCabinet>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Cable.ToString().ToUpper())
                {
                    Root<TempCable> geoJsonObject = JsonConvert.DeserializeObject<Root<TempCable>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.CDB.ToString().ToUpper())
                {
                    Root<TempCDB> geoJsonObject = JsonConvert.DeserializeObject<Root<TempCDB>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Coupler.ToString().ToUpper())
                {
                    Root<TempCoupler> geoJsonObject = JsonConvert.DeserializeObject<Root<TempCoupler>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Customer.ToString().ToUpper())
                {
                    Root<TempWirelineCustomer> geoJsonObject = JsonConvert.DeserializeObject<Root<TempWirelineCustomer>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Duct.ToString().ToUpper())
                {
                    Root<TempDuct> geoJsonObject = JsonConvert.DeserializeObject<Root<TempDuct>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.FDB.ToString().ToUpper())
                {
                    Root<TempFDB> geoJsonObject = JsonConvert.DeserializeObject<Root<TempFDB>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.FMS.ToString().ToUpper())
                {
                    Root<TempFMS> geoJsonObject = JsonConvert.DeserializeObject<Root<TempFMS>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.HTB.ToString().ToUpper())
                {
                    Root<TempHTB> geoJsonObject = JsonConvert.DeserializeObject<Root<TempHTB>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.LandBase.ToString().ToUpper())
                {
                    Root<TempLandBase> geoJsonObject = JsonConvert.DeserializeObject<Root<TempLandBase>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Loop.ToString().ToUpper())
                {
                    Root<TempLoop> geoJsonObject = JsonConvert.DeserializeObject<Root<TempLoop>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }

                if (entityType.ToUpper() == EntityType.Manhole.ToString().ToUpper())
                {
                    Root<TempManhole> geoJsonObject = JsonConvert.DeserializeObject<Root<TempManhole>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Microduct.ToString().ToUpper())
                {
                    Root<TempMicroduct> geoJsonObject = JsonConvert.DeserializeObject<Root<TempMicroduct>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.MPOD.ToString().ToUpper())
                {
                    Root<TempMPOD> geoJsonObject = JsonConvert.DeserializeObject<Root<TempMPOD>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.ONT.ToString().ToUpper())
                {
                    Root<TempONT> geoJsonObject = JsonConvert.DeserializeObject<Root<TempONT>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.PatchPanel.ToString().ToUpper())
                {
                    Root<TempPatchPanel> geoJsonObject = JsonConvert.DeserializeObject<Root<TempPatchPanel>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.POD.ToString().ToUpper())
                {
                    Root<TempPOD> geoJsonObject = JsonConvert.DeserializeObject<Root<TempPOD>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Pole.ToString().ToUpper())
                {
                    Root<TempPole> geoJsonObject = JsonConvert.DeserializeObject<Root<TempPole>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.UNIT.ToString().ToUpper())
                {
                    Root<TempRoom> geoJsonObject = JsonConvert.DeserializeObject<Root<TempRoom>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Sector.ToString().ToUpper())
                {
                    Root<TempSector> geoJsonObject = JsonConvert.DeserializeObject<Root<TempSector>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.SpliceClosure.ToString().ToUpper())
                {
                    Root<TempSpliceClosure> geoJsonObject = JsonConvert.DeserializeObject<Root<TempSpliceClosure>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Splitter.ToString().ToUpper())
                {
                    Root<TempSplitter> geoJsonObject = JsonConvert.DeserializeObject<Root<TempSplitter>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Structure.ToString().ToUpper())
                {
                    Root<TempStructure> geoJsonObject = JsonConvert.DeserializeObject<Root<TempStructure>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Tower.ToString().ToUpper())
                {
                    Root<TempTower> geoJsonObject = JsonConvert.DeserializeObject<Root<TempTower>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Tree.ToString().ToUpper())
                {
                    Root<TempTree> geoJsonObject = JsonConvert.DeserializeObject<Root<TempTree>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Trench.ToString().ToUpper())
                {
                    Root<TempTrench> geoJsonObject = JsonConvert.DeserializeObject<Root<TempTrench>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.Vault.ToString().ToUpper())
                {
                    Root<TempVault> geoJsonObject = JsonConvert.DeserializeObject<Root<TempVault>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                if (entityType.ToUpper() == EntityType.WallMount.ToString().ToUpper())
                {
                    Root<TempWallMount> geoJsonObject = JsonConvert.DeserializeObject<Root<TempWallMount>>(JeoObject);
                    dataTable = GetDataTable(geoJsonObject.features, entityType);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                JsonResponse<string> objResp = new JsonResponse<string>();
                throw new Exception("Please upload a valid JSON File!", ex);
            }
        }
        public ActionResult getColumnMappping(string layerName, int templateId)
        {
            var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == layerName.ToUpper()).FirstOrDefault();
            DataColumnCollection dc = (DataColumnCollection)Session["FileCoulms"];
            ColumnMappingTemplate objColumnMapping = new ColumnMappingTemplate();
            objColumnMapping.layer_id = layerDetail.layer_id;

            //if (templateId > 0)
            //{
            //    objColumnMapping.listMappedColumns = blDataUploader.getColumnsMapping(templateId);
            //    foreach (var column in objColumnMapping.listMappedColumns)
            //    {  
            //        objColumnMapping.listImportedColumns.Add(column.imported_column_name.ToString());
            //    }
            //}
            //else
            //{
            //    objColumnMapping.listMappedColumns = blDataUploader.getEntityTemplateColumns(layerName);
            //    foreach (var column in dc)
            //    {
            //        objColumnMapping.listImportedColumns.Add(column.ToString());
            //    }
            //}
            if (templateId > 0)
            {
                objColumnMapping.listMappedColumns = blDataUploader.getColumnsMapping(templateId);
            }
            else
            {
                objColumnMapping.listMappedColumns = blDataUploader.getEntityTemplateColumns(layerName);
            }
            foreach (var column in dc)
            {
                objColumnMapping.listImportedColumns.Add(column.ToString());
            }

            objColumnMapping.listTemplates = blDataUploader.getMappingTemplates(layerDetail.layer_id, Convert.ToInt32(Session["user_id"]));
            objColumnMapping.id = templateId;
            return PartialView("_ColumnMappping", objColumnMapping);
        }
        public ActionResult templateName()
        {
            return PartialView("_templateName");
        }
        public ActionResult SaveMappingTemplate(ColumnMappingTemplate objTemplate)
        {
            PageMessage objPM = new PageMessage();
            objPM.status = StatusCodes.VALIDATION_FAILED.ToString();
            if (objTemplate.isFinalMapping)
            {
                List<NewColumns> listNewColumn = new List<NewColumns>();
                ErrorMessage status = new ErrorMessage();
                UploadSummary summary = blDataUploader.Get(objTemplate.uploadId);
                var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_id == objTemplate.layer_id).FirstOrDefault();
                summary.entity_type = layerDetail.layer_name;
                getFileData(layerDetail.layer_name);
                List<Mapping> lstMapping = blDataUploader.GetMappings(layerDetail.layer_name);
                foreach (var item in objTemplate.listMappedColumns)
                {
                    if (!string.IsNullOrEmpty(item.imported_column_name) && !string.IsNullOrEmpty(item.template_column_name) && item.template_column_name.ToLower() != item.imported_column_name.ToLower())
                    {
                        bool isSource = lstMapping.Where(m => m.TemplateColName.ToUpper() == item.imported_column_name.ToUpper()).ToList().Count > 0;
                        listNewColumn.Add(
                            new NewColumns
                            {

                                sourceColumn = dataTable.Columns[item.imported_column_name].ColumnName,
                                destinationColumn = item.template_column_name,
                                isSourceTemplateColumn = isSource
                            });
                        DataColumnCollection dtColumns = dataTable.Columns;
                        if (!dtColumns.Contains(item.template_column_name))
                        {
                            dataTable.Columns.Add(item.template_column_name, dataTable.Columns[item.imported_column_name].DataType);
                        }
                        //dataTable.Columns[item.imported_column_name].ColumnName = item.template_column_name;
                    }
                    else if (string.IsNullOrEmpty(item.imported_column_name))
                    {
                        if (dataTable.Columns.Contains(item.template_column_name))
                        {
                            dataTable.AsEnumerable().Where(s => Convert.ToString(s[item.template_column_name]) != "").ToList().ForEach(D => D.SetField(item.template_column_name, ""));

                            dataTable.AcceptChanges();
                        }
                    }
                }
                CopyColumns(dataTable, listNewColumn);


                status = DataValidator.ValidateData(summary, dataTable, lstMapping);
                if (status.status == StatusCodes.INVALID_INPUTS.ToString())
                {
                    //blDataUploader.UpdateStatus(summary);
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                EntityType EnumEntityType = (EntityType)Enum.Parse(typeof(EntityType), summary.entity_type);
                string fname = (string)Session["fileName"];
                Path.GetFileName(fname);


                DataColumnCollection columns = dataTable.Columns;
                if (!columns.Contains("is_valid"))
                {
                    DataColumn colIsValid = dataTable.Columns.Add("is_valid", typeof(bool));

                }
                if (!columns.Contains("error_msg"))
                {
                    DataColumn colErrorMessage = dataTable.Columns.Add("error_msg", typeof(string));
                }


                //  string specif_Colname = string.Empty;
                string defaultColname = string.Empty;
                List<Mapping> defaultColumns = new List<Mapping>();
                //var specfi = lstMapping.Where(x => x.DefaultValue != null).FirstOrDefault();
                defaultColumns = lstMapping.Where(x => !String.IsNullOrEmpty(x.DefaultValue)).ToList();
                //if (default_columns != null)
                //{
                //    specif_colname = specfi.TemplateColName;
                //}

                // string vendor_colname = string.Empty;
                //var vendor = lstMapping.Where(x => x.DbColName.ToUpper() == "VENDOR_NAME").FirstOrDefault();
                //if (vendor != null)
                //{
                //    vendor_colname = vendor.TemplateColName;
                //}

                if (!columns.Contains("row_order"))
                {
                    DataColumn colSRNo = new DataColumn("row_order", typeof(int));
                    dataTable.Columns.Add(colSRNo);
                }
                int srNo = 0;
                foreach (DataRow dr in dataTable.Rows)
                {
                    dr["row_order"] = ++srNo;
                    for (int i = 0; i < defaultColumns.Count(); i++)
                    {

                        defaultColname = defaultColumns[i].TemplateColName;

                        if ((!String.IsNullOrEmpty(defaultColname)) && dataTable.Columns.Contains(defaultColname))
                        {
                            dr[defaultColname] = !string.IsNullOrEmpty(Convert.ToString(dr[defaultColname])) ? dr[defaultColname] : defaultColumns[i].DefaultValue.ToString();
                        }

                    }
                }

                //summary.user_id = Convert.ToInt32(Session["user_id"]);
                //summary.total_record = dataTable.Rows.Count;
                //summary.success_record = 0;
                //summary.failed_record = 0;
                //summary.other_record = 0;
                //summary.entity_type = summary.entity_type;
                //summary.start_on = DateTimeHelper.Now;
                //summary.status = StatusCodes.OK.ToString();
                //summary.file_name = Path.GetFileName(fname);
                //summary.execution_type = ConstantsKeys.START;
                //summary = blDataUploader.Save(summary);
                //summary.status_message = ConstantsKeys.PROCESSING;
                //blDataUploader.UpdateStatus(summary);

                //string filter = "Parent_Network_Id <>''"; 

                //DataView view = new DataView(dataTable);
                //view.RowFilter = filter;
                //dataTable = view.ToTable();
                new BLDataUploader().DeleteRecordFromTempTable(EnumEntityType.ToString(),summary.id);
                status = uploadExcelOrKML.UploadExcelorKML(summary, null, null, EnumEntityType, dataTable);
                if (!status.is_valid)
                {
                    summary.status_message = status.error_msg;
                    blDataUploader.UpdateStatus(summary);
                    //break;
                }
            }
            else
            {
                var objMappings = new BLDataUploader().SaveMappingTemplate(objTemplate, Convert.ToInt32(Session["user_id"]));
                objMappings.objPM.status = StatusCodes.OK.ToString();
                objMappings.objPM.message = "Template has been saved successfully!";
                return Json(objMappings, JsonRequestBehavior.AllowGet);
            }
            return Json(objPM, JsonRequestBehavior.AllowGet); ;
        }

        public void CopyColumns(DataTable dt, List<NewColumns> listNewColumn)
        {

            foreach (DataRow sourcerow in dt.Rows)
            {
                foreach (var column in listNewColumn.OrderByDescending(m => m.isSourceTemplateColumn).ToList())
                {
                    sourcerow[column.destinationColumn] = sourcerow[column.sourceColumn];
                }
            }
        }
        public UploadSummary CheckFileExtension(HttpFileCollectionBase files, UploadSummary summary)
        {
            summary.status = StatusCodes.OK.ToString();
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string extension = Path.GetExtension(file.FileName).ToLower();
                if (!(extension == ".kml" || extension == ".xlsx" || extension == ".xls" || extension == ".kmz" || extension == ".shx" || extension == ".shp" || extension == ".prj" || extension == ".dbf" ||
                    extension == ".dxf" || extension == ".map" || extension == ".id" || extension == ".tab" || extension == ".dat" || extension == ".json"))
                {
                    summary.status = StatusCodes.INVALID_FILE.ToString();
                    summary.status_message = StatusCodes.INVALID_FILE.ToString();
                    summary.err_description = Resources.Resources.SI_OSP_DU_NET_FRM_032;
                    //blDataUploader.UpdateStatus(summary);
                    return summary;
                }

            }
            return summary;
        }
        public bool isSupportFile(string fileName)
        {
            bool isSupportveFile = false;
            string extension = Path.GetExtension(fileName).ToLower();
            if ((extension == ".shx" || extension == ".prj" || extension == ".dbf"))
            {
                return true;
            }
            return isSupportveFile;
        }

        public ActionResult ValidateData(int uploadId)
        {
            UploadSummary summary = blDataUploader.Get(uploadId);
            EntityType EnumEntityType = (EntityType)Enum.Parse(typeof(EntityType), summary.entity_type);
            summary = tempToMainTable.Validate(EnumEntityType, summary);
            return Json(summary);
        }

        public ActionResult ProcessData(int uploadId)
        {
            UploadSummary summary = blDataUploader.Get(uploadId);
            try
            {
                EntityType EnumEntityType = (EntityType)Enum.Parse(typeof(EntityType), summary.entity_type);
                summary = tempToMainTable.InsertDataInMainTable(EnumEntityType, summary);
                BLDataUploader bLDataUploader = new BLDataUploader();
                if (summary.lstErrorMessage == null || summary.lstErrorMessage.Count == 0)
                {
                    bLDataUploader.DeleteDataFromTempTable(summary);
                }
                return Json(summary);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("ProcessData", "DataUploader", ex);
                return Json(summary);
            }
        }

        public ActionResult UploadData()
        {
            User objUser = (User)(Session["userDetail"]);
            BLLayer objBLLayer = new BLLayer();
            var moduleAbbr = "DUP";
            DataUploaderModel dataUploaderModel = new DataUploaderModel();
            dataUploaderModel.lstfiletypes = blDataUploader.getfiletype(moduleAbbr);
            dataUploaderModel.lstLayerDetails = new BLLayer().GetDataUploadLayers(objUser.role_id);
            dataUploaderModel.lstUserModule = objBLLayer.GetUserModuleAbbrList(objUser.user_id, UserType.Web.ToString());
            if (dataUploaderModel.lstUserModule.Contains("IMD") && ApplicationSettings.isSmartPlannerEnabled)
            {
                var planDetail = ImportDataAPIRequest.GetSmartPlannerAPIPlanListRequest();
                var data = JsonConvert.DeserializeObject<JsonPlannerResponse<List<List<PlanUserList>>>>(planDetail);
                if (data.results[0] != null)
                {
                    dataUploaderModel.AllUsers = data.results[0].DistinctBy(x => x.user_name).OrderBy(x => x.user_name).ToList();
                }
            }
            //dataUploaderModel.lstDxfSourceList = new BLDataUploader().getDxfSourceList()
            return PartialView("_UploadData", dataUploaderModel);
        }

        public JsonResult GetSmartPlannerPlanList(string user_name)
        {
            var planDetail = ImportDataAPIRequest.GetSmartPlannerAPIPlanListRequest();
            var data = JsonConvert.DeserializeObject<JsonPlannerResponse<List<List<PlanUserList>>>>(planDetail);
            var output = data.results[0].Where(x => x.user_name == user_name).ToList();
            return Json(output, JsonRequestBehavior.AllowGet); ;

        }


        public string getDxfSourceList()
        {
            var result = new BLDataUploader().getDxfSourceList();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var output = serializer.Serialize(result);
            return output;
        }

        //[HttpPost]
        public ActionResult getUploadSummary(DataUploaderModel objDataUploaderModel, int page = 1)
        {
            User objUser = (User)(Session["userDetail"]);
            int pagesize = ApplicationSettings.uploadSummaryPageSize;
            objDataUploaderModel.objUploadLogFilter.pageSize = pagesize;
            objDataUploaderModel.objUploadLogFilter.page = page;
            objDataUploaderModel.objUploadLogFilter.userId = Convert.ToInt32(Session["user_id"]);
            objDataUploaderModel.lstUploadSummary = blDataUploader.GetUploadSummaryForGrid(objDataUploaderModel.objUploadLogFilter);
            objDataUploaderModel.objUploadLogFilter.totalRecords = objDataUploaderModel.lstUploadSummary.Count > 0 ? objDataUploaderModel.lstUploadSummary[0].total : 0;
            objDataUploaderModel.lstLayerDetails = new BLLayer().GetDataUploadLayers(objUser.role_id);
            return View("_getUploadSummary", objDataUploaderModel);
        }

        //To show the entities on map uploaded by upload summary
        public string ShowOnMap(int id)
        {
            string objresp = string.Empty;
            objresp = blDataUploader.ShowOnMap(id);
            return objresp;
        }
        public JsonResult CheckTemplateExist(string entityType)
        {
            var response = blDataUploader.checkTemplateExists(entityType);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public void downloadTemplate(string entityType, string templateType, string geomType)
        {
            List<Dictionary<string, string>> lstSampleRecords = blDataUploader.getUploadTemplateSampleRecords(entityType);
            List<Dictionary<string, string>> lstTemplateGuideLines = blDataUploader.getUploadTemplateGuideLines(entityType);
            DataTable dtSamples = MiscHelper.GetDataTableFromDictionaries(lstSampleRecords);
            DataTable dtGuideLines = MiscHelper.GetDataTableFromDictionaries(lstTemplateGuideLines);
            var heading1 = Convert.ToString(dtSamples.Rows[0]["Heading1"]);
            var heading2 = Convert.ToString(dtSamples.Rows[0]["Heading2"]);
            // dtSamples.TableName = entityType + "_Details";
            dtSamples.TableName = entityType;
            dtGuideLines.TableName = "Guidelines";
            dtSamples.Columns.Remove("Heading1");
            dtSamples.Columns.Remove("Heading2");
            DataSet ds = new DataSet();
            ds.Tables.Add(dtSamples);
            ds.Tables.Add(dtGuideLines);

            if (dtSamples.Rows.Count > 0)
            {
                ExportUploadertemplate(ds, entityType + "_Template", heading1, heading2, entityType, templateType, geomType);
            }
        }

        private void ExportUploadertemplate(DataSet ds, string fileName, string heading1, string heading2, string entityType, string templateType, string geomType)
        {
            var layerTitle = string.Empty;
            var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == entityType.ToUpper()).FirstOrDefault();
            if (layerDetail != null) { layerTitle = layerDetail.layer_title; }

            switch (templateType)
            {
                case "KML":
                    {
                        List<Dictionary<string, string>> listKMLTemplate = blDataUploader.getKMLTemplate(entityType);
                        DataTable dtKML = MiscHelper.GetDataTableFromDictionaries(listKMLTemplate);
                        var ExternalDatafolder = Server.MapPath(ConfigurationManager.AppSettings["ExternalDataPath"] + "\\KML\\");

                        if (dtKML.Rows.Count > 0)
                        {
                            string kmlString = generateKMLTemplate(entityType, dtKML, geomType);
                            byte[] KMLArray = Encoding.ASCII.GetBytes(kmlString);
                            fileName = layerTitle + "_Template";
                            Response.Clear();
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".kml");

                            Response.AddHeader("Content-Length", KMLArray.Length.ToString());
                            Response.ContentType = "application/xml";
                            Response.BinaryWrite(KMLArray);
                            Response.Flush();
                            Response.End();
                        }
                        break;
                    }
                case "EXL":
                    {
                        MemoryStream exportData = new MemoryStream();
                        IWorkbook workbook = null;
                        using (exportData = new MemoryStream())
                        {
                            if (ds != null && ds.Tables.Count > 0)
                            {
                                workbook = NPOIExcelHelper.uploaderTemplateToExcel("xlsx", ds, heading1, heading2);
                                workbook.Write(exportData);
                            }
                        }

                        fileName = layerTitle + "_Template";
                        Response.Clear();
                        workbook.Write(exportData);
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.End();
                        //if (entityType == EntityType.Cable.ToString()
                        //|| entityType == EntityType.Duct.ToString()
                        //|| entityType == EntityType.Trench.ToString())
                        //{
                        //    byte[] zipBytes = new byte[exportData.ToArray().Length];
                        //    using (var memoryStream = new MemoryStream())
                        //    {
                        //        // note "leaveOpen" true, to not dispose memoryStream too early
                        //        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
                        //        {
                        //            var excelTemplate = zipArchive.CreateEntry(fileName + ".xlsx");
                        //            using (Stream entryStream = excelTemplate.Open())
                        //            {
                        //                entryStream.Write(exportData.ToArray(), 0, exportData.ToArray().Length);
                        //            }

                        //            List<Dictionary<string, string>> listKMLTemplate = blDataUploader.getKMLTemplate(entityType);
                        //            DataTable dtKML = MiscHelper.GetDataTableFromDictionaries(listKMLTemplate);
                        //            if (dtKML.Rows.Count > 0)
                        //            {
                        //                var kmlTemplate = zipArchive.CreateEntry(fileName + ".kml");
                        //                string kmlString = generateKMLTemplate(entityType, dtKML);
                        //                byte[] KMLArray = Encoding.ASCII.GetBytes(kmlString);
                        //                using (Stream entryStream2 = kmlTemplate.Open())
                        //                {
                        //                    entryStream2.Write(KMLArray, 0, KMLArray.Length);
                        //                }
                        //            }

                        //        }
                        //        // now, after zipArchive is disposed - all is written to memory stream
                        //        zipBytes = memoryStream.ToArray();
                        //    }
                        //    fileName = layerTitle + "_Template";
                        //    Response.Clear();
                        //    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".zip");
                        //    Response.AddHeader("Content-Length", zipBytes.Length.ToString());
                        //    Response.ContentType = "application/zip";
                        //    Response.BinaryWrite(zipBytes);
                        //    Response.Flush();
                        //    Response.End();
                        //}
                        //else
                        //{
                        //    fileName = layerTitle + "_Template";
                        //    Response.Clear();
                        //    workbook.Write(exportData);
                        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                        //    Response.BinaryWrite(exportData.ToArray());
                        //    Response.End();
                        //}
                        break;
                    }
                case "SHP":
                    {
                        DataSet dsShp = new DataSet();
                        dsShp.Tables.Add(ds.Tables[0].Copy());

                        string gType = geomType.ToUpper() == "LINE" ? "LINESTRING" : geomType.ToUpper();
                        if (dsShp.Tables[0].Columns.Contains("sp_geometry"))
                        {
                            for (int i = 0; i < dsShp.Tables[0].Rows.Count; i++)
                            {
                                dsShp.Tables[0].Rows[i]["sp_geometry"] = GetGeomForShapeFile(gType, dsShp.Tables[0].Rows[i]["sp_geometry"].ToString());//"LINESTRING(77.357063 28.64987,77.357063 28.64987,77.357283 28.649987,77.357283 28.649987)";// "LINESTRING(" + dsShp.Tables[0].Rows[i]["geom"] + ")";
                            }
                        }
                        else
                        {
                            dsShp.Tables[0].Columns.Add("sp_geometry");
                            for (int i = 0; i < dsShp.Tables[0].Rows.Count; i++)
                            {
                                dsShp.Tables[0].Rows[i]["sp_geometry"] = GetGeomForShapeFile(gType, "", dsShp.Tables[0].Rows[i]["latitude"].ToString(), dsShp.Tables[0].Rows[i]["longitude"].ToString());
                            }
                        }
                        GetShapeFile(dsShp, (layerTitle + "_Template"));
                        break;
                    }
                case "TAB":
                    {
                        DataSet dsTab = new DataSet();
                        dsTab.Tables.Add(ds.Tables[0].Copy());

                        string gType = geomType.ToUpper() == "LINE" ? "LINESTRING" : geomType.ToUpper();
                        if (dsTab.Tables[0].Columns.Contains("sp_geometry"))
                        {
                            for (int i = 0; i < dsTab.Tables[0].Rows.Count; i++)
                            {
                                dsTab.Tables[0].Rows[i]["sp_geometry"] = GetGeomForShapeFile(gType, dsTab.Tables[0].Rows[i]["sp_geometry"].ToString());//"LINESTRING(77.357063 28.64987,77.357063 28.64987,77.357283 28.649987,77.357283 28.649987)";// "LINESTRING(" + dsShp.Tables[0].Rows[i]["geom"] + ")";
                            }
                        }
                        else
                        {
                            dsTab.Tables[0].Columns.Add("sp_geometry");
                            for (int i = 0; i < dsTab.Tables[0].Rows.Count; i++)
                            {
                                dsTab.Tables[0].Rows[i]["sp_geometry"] = GetGeomForShapeFile(gType, "", dsTab.Tables[0].Rows[i]["latitude"].ToString(), dsTab.Tables[0].Rows[i]["longitude"].ToString());
                            }
                        }
                        GetTabFile(dsTab, (layerTitle + "_Template"));
                        break;
                    }
                case "JSON":
                    {

                        DataSet dsJson = new DataSet();
                        dsJson.Tables.Add(ds.Tables[0].Copy());
                        var JSONString = new StringBuilder();

                        if (geomType == "Point")
                        {
                            var columnName = dsJson.Tables[0].Rows[0]["Longitude"] + "," + dsJson.Tables[0].Rows[0]["Latitude"];

                            dynamic ab = ('"' + "type" + '"' + ":" + '"' + "FeatureCollection" + '"' + ",");
                            dynamic ab1 = ("{ " + '"' + "type" + '"' + ":" + '"' + "Feature" + '"' + ",");

                            dynamic ab2 = ('"' + "geometry" + '"' + ":" + "{" + '"' + "type" + '"' + ":" + '"' + "Point" + '"' + "," + '"' + "coordinates" + '"' + ":" + "[" + columnName.ToString() + "]},");


                            if (dsJson.Tables[0].Rows.Count > 0)
                            {
                                JSONString.Append("{");
                                JSONString.Append(ab);
                                JSONString.Append('"' + "features" + '"' + ":" + "[");
                                JSONString.Append(ab1);
                                JSONString.Append(ab2);
                                JSONString.Append('"' + "properties" + '"' + ":");
                                for (int i = 0; i < dsJson.Tables[0].Rows.Count; i++)
                                {
                                    JSONString.Append("{");
                                    for (int j = 0; j < dsJson.Tables[0].Columns.Count; j++)
                                    {
                                        if (j < dsJson.Tables[0].Columns.Count - 1)
                                        {
                                            JSONString.Append("\"" + dsJson.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + dsJson.Tables[0].Rows[i][j].ToString() + "\",");
                                        }
                                        else if (j == dsJson.Tables[0].Columns.Count - 1)
                                        {
                                            JSONString.Append("\"" + dsJson.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + dsJson.Tables[0].Rows[i][j].ToString() + "\"");
                                        }
                                    }
                                    if (i == dsJson.Tables[0].Rows.Count - 1)
                                    {
                                        JSONString.Append("}");
                                    }
                                    else
                                    {
                                        JSONString.Append("},");
                                    }
                                }

                                JSONString.Append("}");
                                JSONString.Append("]");
                                JSONString.Append("}");

                            }
                            fileName = layerTitle + "_Template";
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JSONString.ToString());
                            var content = new System.IO.MemoryStream(bytes);
                            byte[] jsonArray = Encoding.ASCII.GetBytes(JSONString.ToString());
                            Response.Clear();
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".json");
                            Response.AddHeader("Content-Length", jsonArray.Length.ToString());
                            Response.ContentType = "application/json";
                            Response.BinaryWrite(jsonArray);
                            Response.Flush();
                            Response.End();
                            break;
                        }
                        else
                        {
                            var geonumber = "";
                            if (geomType == "Line")
                            {
                                string geometry = string.Empty;
                                var data = ds.Tables[0].Rows[0]["sp_geometry"].ToString();

                                var geomData = data.Replace(",0", "],[").Split(',');
                                geometry = "[";
                                for (int i = 0; i < geomData.Length; i++)
                                {
                                    geometry = (geometry == (geomType + ",")) ? geometry + geomData[i].ToString().Replace(",", " ") : geometry + "," + geomData[i].ToString().Replace(",", " ");
                                }
                                geometry = geometry.Remove(geometry.Length - 1, 1) + "]";


                                var unQuotedString = geometry.Substring(2, geometry.Length - 2);
                                geonumber = unQuotedString.Remove(unQuotedString.Length - 2, 2);
                            }
                            if (geomType == "Polygon")
                            {

                                geonumber = ds.Tables[0].Rows[0]["sp geometry"].ToString();

                            }

                            dynamic ab = ('"' + "type" + '"' + ":" + '"' + "FeatureCollection" + '"' + ",");
                            dynamic ab1 = ("{ " + '"' + "type" + '"' + ":" + '"' + "Feature" + '"' + ",");
                            dynamic ab2 = string.Empty;
                            if (geomType == "Line")
                            {
                                ab2 = ('"' + "geometry" + '"' + ":" + "{" + '"' + "type" + '"' + ":" + '"' + geomType.ToString().Replace("Line", "LineString") + '"' + "," + '"' + "coordinates" + '"' + ":[" + "[" + geonumber + "]" + "},");

                            }
                            if (geomType == "Polygon")
                            {
                                ab2 = ('"' + "geometry" + '"' + ":" + "{" + '"' + "type" + '"' + ":" + '"' + geomType.ToString() + '"' + "},");
                            }

                            if (dsJson.Tables[0].Rows.Count > 0)
                            {
                                JSONString.Append("{");
                                JSONString.Append(ab);
                                JSONString.Append('"' + "features" + '"' + ":" + "[");
                                JSONString.Append(ab1);
                                JSONString.Append(ab2);
                                if (geomType == "Line")
                                {
                                    JSONString.Append('"' + "properties" + '"' + ":");
                                }
                                if (geomType == "Polygon")
                                {
                                    JSONString.Append('"' + "Polygon" + '"' + ":");
                                }
                                for (int i = 0; i < dsJson.Tables[0].Rows.Count; i++)
                                {
                                    JSONString.Append("{");
                                    for (int j = 0; j < dsJson.Tables[0].Columns.Count; j++)
                                    {
                                        if (j < dsJson.Tables[0].Columns.Count - 1)
                                        {
                                            JSONString.Append("\"" + dsJson.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + dsJson.Tables[0].Rows[i][j].ToString() + "\",");
                                        }
                                        else if (j == dsJson.Tables[0].Columns.Count - 1)
                                        {
                                            JSONString.Append("\"" + dsJson.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + dsJson.Tables[0].Rows[i][j].ToString() + "\"");
                                        }
                                    }
                                    if (i == dsJson.Tables[0].Rows.Count - 1)
                                    {
                                        JSONString.Append("}");
                                    }
                                    else
                                    {
                                        JSONString.Append("},");
                                    }
                                }

                                JSONString.Append("}");
                                JSONString.Append("]");
                                JSONString.Append("}");

                            }
                            fileName = layerTitle + "_Template";
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JSONString.ToString());
                            var content = new System.IO.MemoryStream(bytes);
                            byte[] jsonArray = Encoding.ASCII.GetBytes(JSONString.ToString());
                            Response.Clear();
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".json");
                            Response.AddHeader("Content-Length", jsonArray.Length.ToString());
                            Response.ContentType = "application/json";
                            Response.BinaryWrite(jsonArray);
                            Response.Flush();
                            Response.End();
                            break;
                        }
                    }
                default: { break; }
            }

        }
        public string GetGeomForShapeFile(string geomType, string geom, string lat = "", string lng = "")
        {
            string geometry = string.Empty;
            if (string.IsNullOrEmpty(geom))
            {
                geometry = geomType + "(" + lng + " " + lat + ")";
            }
            else
            {
                var geomData = geom.Replace(",0", "~").Split('~');
                geometry = geomType + "(";
                for (int i = 0; i < geomData.Length; i++)
                {
                    geometry = (geometry == (geomType + "(")) ? geometry + geomData[i].ToString().Replace(",", " ") : geometry + "," + geomData[i].ToString().Replace(",", " ");
                }
                geometry = geometry.Remove(geometry.Length - 1, 1) + ")";
            }
            return geometry;
        }
        public void GetShapeFile(DataSet ds, string filename)
        {
            string entity = string.Empty;
            string shapeFilePath = Server.MapPath(ApplicationSettings.AttachmentLocalPath) + "/Attachments/SI_Shape_" + Guid.NewGuid().ToString().Substring(0, 8);//Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + "Downloads" + @"\SI_Shape_" + Guid.NewGuid().ToString().Substring(0, 8);
                                                                                                                                                                  //string entity = objExportEntitiesReport.objReportFilters.layerName;
            if (Directory.Exists(shapeFilePath).Equals(false))
                Directory.CreateDirectory(shapeFilePath);
            for (int j = 0; j < ds.Tables.Count; j++)
            {
                DataTable dtReport = ds.Tables[j];
                entity = dtReport.TableName;
                if (dtReport.AsEnumerable().Where(x => x.Field<string>("sp_geometry") != null).ToList().Count > 0)
                {
                    dtReport = dtReport.AsEnumerable().Where(x => x.Field<string>("sp_geometry") != null).CopyToDataTable();
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
                        var geometry = wktReader.Read(row["sp_geometry"].ToString());
                        var attributesTable = new AttributesTable();
                        columnNameList = string.Empty;
                        foreach (DataColumn column in dtReport.Columns)
                        {
                            if (!column.ColumnName.ToLower().Equals("sp_geometry") && !column.ColumnName.ToLower().Equals("geom"))
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

                    var shapeFileName = shapeFilePath + "\\" + filename;
                    var shapeFilePrjName = Path.Combine(shapeFilePath, filename + ".prj");

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
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".zip");
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "application/zip";
            Response.WriteFile(file.FullName);
            Response.Flush();
            Response.End();
            System.IO.File.Delete(zipshapePath);
        }
        private bool CheckMaximumRecordAllowed(int uploadRecordCount, EntityType enumEntityType, out int maxUploadCount)
        {

            BLLayer objBLLayer = new BLLayer();
            layerDetail networkLayerDetails = objBLLayer.GetLayerDetails(enumEntityType.ToString());
            maxUploadCount = networkLayerDetails.data_upload_max_count;
            if (uploadRecordCount <= networkLayerDetails.data_upload_max_count)
                return true;
            return false;
        }
        public string generateKMLTemplate_Old(string entityType, DataTable dtKML, string geomType)
        {

            string geometry = string.Empty;
            StringBuilder sbLine = new StringBuilder();
            sbLine.Append("<Placemark>");
            string longitude = "";
            string latitude = "";
            string geomTempColName = string.Empty;
            string latTempColName = string.Empty;
            string longTempColName = string.Empty;
            List<Mapping> lstMapping = blDataUploader.GetMappings(entityType);
            for (int i = 0; i < dtKML.Rows.Count; i++)
            {

                if (entityType.ToUpper() != "LANDBASE")
                {
                    if (Convert.ToString(dtKML.Rows[i][0]) != "sp_geometry" && Convert.ToString(dtKML.Rows[i][0]) != "latitude" && Convert.ToString(dtKML.Rows[i][0]) != "longitude")
                    {
                        var columnName = "\"" + dtKML.Rows[i][2] + "\"";
                        sbLine.Append("<" + dtKML.Rows[i][0] + " template_column_name=" + columnName + ">" + dtKML.Rows[i][1] + "</" + dtKML.Rows[i][0] + ">");
                    }
                    if (Convert.ToString(dtKML.Rows[i][0]).ToLower() == "sp_geometry" && entityType.ToUpper() != EntityType.Building.ToString().ToUpper())
                    {
                        geometry = Convert.ToString(dtKML.Rows[i][1]);
                    }
                }
                else
                {
                    if (Convert.ToString(dtKML.Rows[i][0]) != "latitude" && Convert.ToString(dtKML.Rows[i][0]) != "longitude")
                    {
                        var columnName = "\"" + dtKML.Rows[i][2] + "\"";
                        sbLine.Append("<" + dtKML.Rows[i][0] + " template_column_name=" + columnName + ">" + dtKML.Rows[i][1] + "</" + dtKML.Rows[i][0] + ">");
                    }
                }

                if (entityType.ToUpper() == "BUILDING" && geomType == "Point" && Convert.ToString(dtKML.Rows[i][0]).ToLower() == "sp_geometry")
                {
                    var columnName = "\"" + dtKML.Rows[i][2] + "\"";
                    sbLine.Append("<" + dtKML.Rows[i][0] + " template_column_name=" + columnName + ">" + dtKML.Rows[i][1] + "</" + dtKML.Rows[i][0] + ">");
                }

                if (geomType == "Point" && Convert.ToString(dtKML.Rows[i][0]).ToLower() == "longitude")
                {
                    longitude = Convert.ToString(dtKML.Rows[i][1]);

                }
                if (geomType == "Point" && Convert.ToString(dtKML.Rows[i][0]).ToLower() == "latitude")
                {
                    latitude = Convert.ToString(dtKML.Rows[i][1]);
                }
            }
            if (geomType == "Line")
            {
                sbLine.Append("<description>Unknown Line Type</description>");
                sbLine.Append("<styleUrl>#line1</styleUrl><LineString><coordinates>");
                sbLine.Append(geometry);
                sbLine.Append("</coordinates></LineString></Placemark>");
            }
            else if (geomType == "Point")
            {
                sbLine.Append("<description>Unknown poit Type</description>");
                sbLine.Append("<styleUrl>#line1</styleUrl><Point><coordinates>");

                if (entityType.ToUpper() != "LANDBASE")
                {
                    sbLine.Append(longitude + "," + latitude + ",0");
                }
                sbLine.Append("</coordinates></Point></Placemark>");
            }
            string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                    "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                    "<Document>  <!-- Begin Style Definitions -->" +
                                    "<name>" + entityType + "</name>" +
                                    "<open>1</open>" +
                                    "<Style id=\"line1\"><LineStyle><color>ff00ffff</color><width>4</width></LineStyle></Style>" +
                                     sbLine.ToString() + "</Document></kml>";
            return finalKMLString;
        }

        public string generateKMLTemplate(string entityType, DataTable dtKML, string geomType)
        {

            string geometry = string.Empty;
            StringBuilder sbLine = new StringBuilder();
            sbLine.Append("<Placemark><ExtendedData><SchemaData schemaUrl='#UG'>");
            string longitude = "";
            string latitude = "";
            string azimuth = "";
            string sector_type = "";
            double OutVal = 0;
            string geomTempColName = string.Empty;
            string latTempColName = string.Empty;
            string longTempColName = string.Empty;
            List<Mapping> lstMapping = blDataUploader.GetMappings(entityType);
            for (int i = 0; i < dtKML.Rows.Count; i++)
            {

                if (entityType.ToUpper() != "LANDBASE")
                {
                    if (Convert.ToString(dtKML.Rows[i][0]) != "sp_geometry" && Convert.ToString(dtKML.Rows[i][0]) != "latitude" && Convert.ToString(dtKML.Rows[i][0]) != "longitude")
                    {
                        var columnName = "\"" + dtKML.Rows[i][2] + "\"";
                        var db_columnName = "\"" + dtKML.Rows[i][0] + "\"";
                        // sbLine.Append("<" + dtKML.Rows[i][0] + " template_column_name=" + columnName + ">" + dtKML.Rows[i][1] + "</" + dtKML.Rows[i][0] + ">");

                        sbLine.Append("<SimpleData name=" + columnName + " db_column_name=" + db_columnName + ">" + dtKML.Rows[i][1] + "</SimpleData>");

                    }
                    if (Convert.ToString(dtKML.Rows[i][0]).ToLower() == "sp_geometry" && entityType.ToUpper() != EntityType.Building.ToString().ToUpper())
                    {
                        geometry = Convert.ToString(dtKML.Rows[i][1]);
                    }
                }
                else
                {
                    if (Convert.ToString(dtKML.Rows[i][0]) != "latitude" && Convert.ToString(dtKML.Rows[i][0]) != "longitude")
                    {
                        var columnName = "\"" + dtKML.Rows[i][2] + "\"";
                        var db_columnName = "\"" + dtKML.Rows[i][0] + "\"";
                        // sbLine.Append("<" + dtKML.Rows[i][0] + " template_column_name=" + columnName + ">" + dtKML.Rows[i][1] + "</" + dtKML.Rows[i][0] + ">");
                        sbLine.Append("<SimpleData name=" + columnName + " db_column_name=" + db_columnName + ">" + dtKML.Rows[i][1] + "</SimpleData>");
                    }
                }

                if (entityType.ToUpper() == "BUILDING" && geomType == "Point" && Convert.ToString(dtKML.Rows[i][0]).ToLower() == "sp_geometry")
                {
                    var columnName = "\"" + dtKML.Rows[i][2] + "\"";
                    var db_columnName = "\"" + dtKML.Rows[i][0] + "\"";
                    // sbLine.Append("<" + dtKML.Rows[i][0] + " template_column_name=" + columnName + ">" + dtKML.Rows[i][1] + "</" + dtKML.Rows[i][0] + ">");
                    sbLine.Append("<SimpleData name=" + columnName + " db_column_name=" + db_columnName + ">" + dtKML.Rows[i][1] + "</SimpleData>");
                }

                if ((geomType == "Point" || geomType == "Polygon") && Convert.ToString(dtKML.Rows[i][0]).ToLower() == "longitude")
                {
                    longitude = Convert.ToString(dtKML.Rows[i][1]);

                }
                if ((geomType == "Point" || geomType == "Polygon") && Convert.ToString(dtKML.Rows[i][0]).ToLower() == "latitude")
                {
                    latitude = Convert.ToString(dtKML.Rows[i][1]);
                }
                if (geomType == "Polygon" && Convert.ToString(dtKML.Rows[i][0]).ToLower() == "azimuth")
                {
                    azimuth = Convert.ToString(dtKML.Rows[i][1]);
                }
                if (geomType == "Polygon" && Convert.ToString(dtKML.Rows[i][0]).ToLower() == "sector_type")
                {
                    sector_type = Convert.ToString(dtKML.Rows[i][1]);
                }
            }
            if (geomType.ToUpper() == "LINE")
            {
                //sbLine.Append("</SchemaData></ExtendedData><description>Unknown Line Type</description>");

                sbLine.Append("</SchemaData></ExtendedData><styleUrl>#Line1</styleUrl><LineString><coordinates>");
                sbLine.Append(geometry);
                sbLine.Append("</coordinates></LineString></Placemark>");
            }
            else if (geomType.ToUpper() == "POINT")
            {
                //sbLine.Append("</SchemaData></ExtendedData><description>Unknown Point Type</description>");

                sbLine.Append("</SchemaData></ExtendedData><styleUrl>#Point1</styleUrl><Point><coordinates>");

                if (entityType.ToUpper() != "LANDBASE")
                {
                    sbLine.Append(longitude + "," + latitude + ",0");
                }
                sbLine.Append("</coordinates></Point></Placemark>");
            }
            else if (geomType.ToUpper() == "POLYGON")
            {
                if (entityType.ToUpper() == "SECTOR")
                {
                    sbLine.Append("</SchemaData></ExtendedData><styleUrl>#Point1</styleUrl><Point><coordinates>");
                    sbLine.Append(longitude + "," + latitude + ",0");
                    sbLine.Append("</coordinates></Point></Placemark>");
                }
                else
                {
                    sbLine.Append("</SchemaData></ExtendedData><styleUrl>#Polygon1</styleUrl><Polygon><outerBoundaryIs><LinearRing><coordinates>");

                    var geom = Common.GetSectorsGeometry(Convert.ToDouble(latitude), Convert.ToDouble(longitude), Convert.ToDouble(azimuth), sector_type);

                    if (!string.IsNullOrEmpty(Convert.ToString(geom)))
                    {
                        string[] x = geom.Split(',');
                        foreach (string y in x)
                        {
                            sbLine.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                        }
                    }
                    sbLine.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
                }
            }

            string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                    "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                    "<Document>  <!-- Begin Style Definitions -->" +
                                    "<name>" + entityType + "</name>" +
                                    "<open>1</open>" +
                                    "<Style id=\"line1\"><LineStyle><color>ff00ffff</color><width>4</width></LineStyle></Style> " +
                                     sbLine.ToString() + "</Document></kml>";
            return finalKMLString;
        }

        public void GetTabFile(DataSet ds, string filename)
        {
            string entity = string.Empty;
            string guid = Guid.NewGuid().ToString().Substring(0, 8);
            string shapeFilePath = Server.MapPath(ApplicationSettings.AttachmentLocalPath) + "/Attachments/SI_Shape_" + guid;
            string tabFilePath = Server.MapPath(ApplicationSettings.AttachmentLocalPath) + "/Attachments/SI_Tab_" + guid;


            if (Directory.Exists(shapeFilePath).Equals(false))
                Directory.CreateDirectory(shapeFilePath);
            if (Directory.Exists(tabFilePath).Equals(false))
                Directory.CreateDirectory(tabFilePath);

            for (int j = 0; j < ds.Tables.Count; j++)
            {
                DataTable dtReport = ds.Tables[j];
                entity = dtReport.TableName;
                if (dtReport.AsEnumerable().Where(x => x.Field<string>("sp_geometry") != null).ToList().Count > 0)
                {
                    dtReport = dtReport.AsEnumerable().Where(x => x.Field<string>("sp_geometry") != null).CopyToDataTable();
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
                        var geometry = wktReader.Read(row["sp_geometry"].ToString());
                        var attributesTable = new AttributesTable();
                        columnNameList = string.Empty;
                        foreach (DataColumn column in dtReport.Columns)
                        {
                            if (!column.ColumnName.ToLower().Equals("sp_geometry") && !column.ColumnName.ToLower().Equals("geom"))
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

                    var shapeFileName = shapeFilePath + "\\" + filename;
                    var shapeFilePrjName = Path.Combine(shapeFilePath, filename + ".prj");

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

            if (Directory.Exists(shapeFilePath).Equals(true))
            {
                string shapeFilePathExt = ("" + "\"" + shapeFilePath + "/" + filename + ".shp" + "" + "\"");
                string tabFilePathExt = ("" + "\"" + tabFilePath + "/" + filename + ".tab" + "" + "\"");

                string dirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
                string shp2kml_convertorfolder = "shp2kml_convertor";

                var arguments = "-f " + "\"MapInfo File\"" + " " + tabFilePathExt + " " + shapeFilePathExt + " ";

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = dirPath + shp2kml_convertorfolder,
                        FileName = dirPath + shp2kml_convertorfolder + "\\ogr2ogr.exe",
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };
                proc.Start();
                proc.WaitForExit();
            }


            string ziptabPath = tabFilePath + ".zip";//result.Replace("success:", "");
                                                     //zip the shape file
            using (var zip = new ZipFile())
            {
                zip.AddDirectory(tabFilePath);
                zip.Save(ziptabPath);
            }
            if (System.IO.File.Exists(ziptabPath))
            {
                string fileName = Path.GetFileName(ziptabPath);
                Directory.Delete(tabFilePath, true);
                Directory.Delete(shapeFilePath, true);
            }
            FileInfo file = new FileInfo(ziptabPath);
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".zip");
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "application/zip";
            Response.WriteFile(file.FullName);
            Response.Flush();
            Response.End();
            System.IO.File.Delete(ziptabPath);
        }

        #region Import Data

        public static DataTable GetDataTable(dynamic lst, string entity_type)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.TableName = entity_type;
                foreach (var item in lst[0].properties.GetType().GetProperties())
                {
                    dt.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
                }
                for (int i = 0; i < lst.Count; i++)
                {
                    string geometry = string.Empty;

                    if (entity_type == EntityType.Cable.ToString() || entity_type == EntityType.Duct.ToString() || entity_type == EntityType.Trench.ToString())
                    {
                        for (int j = 0; j < lst[i].geometry.coordinates.Count; j++)
                        {
                            geometry += Convert.ToString(lst[i].geometry.coordinates[j].First.Value) + " " + Convert.ToString(lst[i].geometry.coordinates[j].Last.Value) + ",";
                        }
                        geometry = geometry.TrimEnd(',');
                    }
                    DataRow row = dt.NewRow();
                    foreach (var item in lst[0].properties.GetType().GetProperties())
                    {
                        if (item.Name == "sp_geometry")
                        {
                            row[item.Name] = geometry;
                        }
                        else
                        {
                            row[item.Name] = item.GetValue(lst[i].properties) ?? DBNull.Value;
                            if (item.Name == "specification" || item.Name == "vendor_name")
                            {
                                row[item.Name] = !string.IsNullOrEmpty(Convert.ToString(row[item.Name])) ? row[item.Name] : "Generic";
                            }
                            if (item.Name == "longitude")
                            {
                                row[item.Name] = Convert.ToString(lst[i].geometry.coordinates[0]);
                            }
                            if (item.Name == "latitude")
                            {
                                row[item.Name] = Convert.ToString(lst[i].geometry.coordinates[1]);
                            }
                        }

                    }
                    if (entity_type == EntityType.ADB.ToString() || entity_type == EntityType.BDB.ToString() || entity_type == EntityType.CDB.ToString() || entity_type == EntityType.FDB.ToString())
                    {
                        if (dt.Columns.Contains("client_id"))
                        {
                            row["client_id"] = lst[i].id;
                        }
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            }
            catch (Exception ex)
            {
                JsonResponse<string> objResp = new JsonResponse<string>();
                throw new Exception("Please upload a valid File!", ex);
            }

        }

        [HttpPost]
        public ActionResult ImportData()
        {
            // Checking no of files injected in Request object 
            ErrorMessage status = new ErrorMessage();
            UploadSummary summary = new UploadSummary();
            string planId = Request.Form["planId"].ToString();
            StringBuilder strParameters = new StringBuilder();
            if (planId != "")
            {
                strParameters.Append("planid=" + planId);
            }
            #region Local Testing
            ////string file = "D:\\JsonFiles\\generated.json";
            //string testfile = "D:\\JsonFiles\\generatedArray.json";
            ////string Jsontext = System.IO.File.ReadAllText(file);
            //string JsonTesttext = System.IO.File.ReadAllText(testfile);
            //var data = JsonConvert.DeserializeObject<Models.JsonPlannerResponse<List<DataImport>>>(JsonTesttext);
            #endregion 

            var response = ImportDataAPIRequest.GetSmartPlannerAPIRequest(Convert.ToString(strParameters));
            var data = JsonConvert.DeserializeObject<Models.JsonPlannerResponse<List<DataImport>>>(response);

            if (data.status == StatusCodes.OK.ToString() && data.error_message == null)
            {
                DataImport di = data.results[0];
                DataSet ds = new DataSet();
                if (di != null)
                {
                    summary.user_id = Convert.ToInt32(Session["user_id"]);
                    summary.start_on = DateTimeHelper.Now;
                    summary.status = StatusCodes.OK.ToString();
                    summary.file_name = "Generated.json";
                    summary.execution_type = ConstantsKeys.START;
                    summary.plan_id = planId;

                    return Json(summary);
                }
                else
                {
                    return Json(Resources.Resources.SI_OSP_DU_NET_FRM_033);
                }
            }
            else
            {
                return Json(Resources.Resources.SI_OSP_DU_NET_FRM_034);
            }

        }

        public ActionResult Insert_ValidateData(string planId)
        {
            //UploadSummary summary = new UploadSummary();

            List<UploadSummary> lstsummary = new List<UploadSummary>();
            List<UploadSummary> lstOrderedSummary = new List<UploadSummary>();
            ErrorMessage status = new ErrorMessage();
            DataTable table = new DataTable();
            StringBuilder strParameters = new StringBuilder();
            if (planId != "")
            {
                strParameters.Append("planid=" + planId);
            }
            #region Local Testing
            //string testfile = "D:\\JsonFiles\\generatedArray.json";

            ////string Jsontext = System.IO.File.ReadAllText(file);
            //string JsonTesttext = System.IO.File.ReadAllText(testfile);
            //var data = JsonConvert.DeserializeObject<Models.JsonPlannerResponse<List<DataImport>>>(JsonTesttext);
            #endregion
            var response = ImportDataAPIRequest.GetSmartPlannerAPIRequest(strParameters.ToString());
            var data = JsonConvert.DeserializeObject<Models.JsonPlannerResponse<List<DataImport>>>(response);
            List<string> childEntity = new List<string>();
            if (data.status == StatusCodes.OK.ToString() && data.error_message == null)
            {
                DataImport di = data.results[0];

                DataSet ds = new DataSet();
                #region All Entity

                if (di != null)
                {
                    if (di.lstTempADB != null)
                    {
                        table = GetDataTable(di.lstTempADB, EntityType.ADB.ToString());
                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempAntenna != null)
                    {
                        table = GetDataTable(di.lstTempAntenna, EntityType.Antenna.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempBDB != null)
                    {
                        table = GetDataTable(di.lstTempBDB, EntityType.BDB.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempBuilding != null)
                    {
                        table = GetDataTable(di.lstTempBuilding, EntityType.Building.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempCabinet != null)
                    {
                        table = GetDataTable(di.lstTempCabinet, EntityType.Cabinet.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempCable != null)
                    {
                        table = GetDataTable(di.lstTempCable, EntityType.Cable.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempCDB != null)
                    {
                        table = GetDataTable(di.lstTempCDB, EntityType.CDB.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempCoupler != null)
                    {
                        table = GetDataTable(di.lstTempCoupler, EntityType.Coupler.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempDuct != null)
                    {
                        table = GetDataTable(di.lstTempDuct, EntityType.Duct.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempFDB != null)
                    {
                        table = GetDataTable(di.lstTempFDB, EntityType.FDB.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempFMS != null)
                    {
                        table = GetDataTable(di.lstTempFMS, EntityType.FMS.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempHTB != null)
                    {
                        table = GetDataTable(di.lstTempHTB, EntityType.HTB.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempManhole != null)
                    {
                        table = GetDataTable(di.lstTempManhole, EntityType.Manhole.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempMPOD != null)
                    {
                        table = GetDataTable(di.lstTempMPOD, EntityType.MPOD.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempONT != null)
                    {
                        table = GetDataTable(di.lstTempONT, EntityType.ONT.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempPatchPanel != null)
                    {
                        table = GetDataTable(di.lstTempPatchPanel, EntityType.PatchPanel.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempPOD != null)
                    {
                        table = GetDataTable(di.lstTempPOD, EntityType.POD.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempPole != null)
                    {
                        table = GetDataTable(di.lstTempPole, EntityType.Pole.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempRoom != null)
                    {
                        table = GetDataTable(di.lstTempRoom, EntityType.UNIT.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempSector != null)
                    {
                        table = GetDataTable(di.lstTempSector, EntityType.Sector.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempSpliceClosure != null)
                    {
                        table = GetDataTable(di.lstTempSpliceClosure, EntityType.SpliceClosure.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempSplitter != null)
                    {
                        table = GetDataTable(di.lstTempSplitter, EntityType.Splitter.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }

                        if (table.Columns.Contains("parent_client_id"))
                        {
                            foreach (DataRow dr in table.Rows)
                            {
                                if (Convert.ToInt32(dr["parent_client_id"]) > 0)
                                {
                                    if (!childEntity.Contains(EntityType.Splitter.ToString()))
                                    {
                                        childEntity.Add(EntityType.Splitter.ToString());
                                    }
                                }
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempStructure != null)
                    {
                        table = GetDataTable(di.lstTempStructure, EntityType.Structure.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempTower != null)
                    {
                        table = GetDataTable(di.lstTempTower, EntityType.Tower.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempTree != null)
                    {
                        table = GetDataTable(di.lstTempTree, EntityType.Tree.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempTrench != null)
                    {
                        table = GetDataTable(di.lstTempTrench, EntityType.Trench.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempVault != null)
                    {
                        table = GetDataTable(di.lstTempVault, EntityType.Vault.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }

                    if (di.lstTempWallMount != null)
                    {
                        table = GetDataTable(di.lstTempWallMount, EntityType.WallMount.ToString());

                        if (table.Columns.Contains("row_order"))
                        {
                            int srNo = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                dr["row_order"] = ++srNo;
                            }
                        }
                        ds.Tables.Add(table);
                    }
                }
                #endregion

                if (ds != null)
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        UploadSummary summary = new UploadSummary();
                        summary.user_id = Convert.ToInt32(Session["user_id"]);
                        summary.total_record = dt.Rows.Count;
                        summary.success_record = 0;
                        summary.failed_record = 0;
                        summary.other_record = 0;
                        summary.entity_type = dt.TableName;
                        summary.start_on = DateTimeHelper.Now;
                        summary.status = StatusCodes.OK.ToString();
                        summary.file_name = "Generated Json";
                        summary.execution_type = ConstantsKeys.START;
                        summary.plan_id = planId;
                        summary.is_child_entity = childEntity.Contains(dt.TableName);
                        summary = blDataUploader.Save(summary);
                        summary.status_message = ConstantsKeys.PROCESSING;
                        blDataUploader.UpdateStatus(summary);

                        EntityType EnumEntityType = (EntityType)Enum.Parse(typeof(EntityType), summary.entity_type);
                        status = uploadExcelOrKML.UploadExcelorKML(summary, null, null, EnumEntityType, dt);
                        if (!status.is_valid)
                        {
                            summary.status_message = status.error_msg;
                            blDataUploader.UpdateStatus(summary);
                        }
                        summary = tempToMainTable.Validate(EnumEntityType, summary);

                        lstsummary.Add(summary);
                    }

                    //DataTable dat = blDataUploader.GetUploadId(planId, Convert.ToInt32(Session["user_id"]), null);
                    //for (int i = 0; i < dat.Rows.Count; i++)
                    //{
                    lstOrderedSummary = lstsummary.OrderBy(x => x.is_child_entity).ToList();
                    //}
                }
            }
            return Json(lstOrderedSummary);
        }

        public ActionResult ProcessImportData(string planId, string entity_type, bool isSingleExecutionFinished = false)
        {
            DataTable dt = blDataUploader.GetUploadId(planId, Convert.ToInt32(Session["user_id"]), entity_type);
            List<UploadSummary> lstSummary = new List<UploadSummary>();
            UploadSummary summary = new UploadSummary();
            BLDataUploader bLDataUploader = new BLDataUploader();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int validCount = Convert.ToInt32(dt.Rows[i]["total_record"]) - Convert.ToInt32(dt.Rows[i]["failed_record"]);
                    if (validCount > 0)
                    {
                        summary = blDataUploader.Get(Convert.ToInt32(dt.Rows[i]["uploadId"]));
                        if (!isSingleExecutionFinished)
                        {
                            try
                            {
                                EntityType EnumEntityType = (EntityType)Enum.Parse(typeof(EntityType), summary.entity_type);
                                summary = tempToMainTable.InsertDataInMainTable(EnumEntityType, summary);
                            }
                            catch (Exception ex)
                            {
                                ErrorLogHelper.WriteErrorLog("ProcessData", "DataUploader", ex);
                            }
                        }
                        lstSummary.Add(summary);
                    }
                }
                if (entity_type == null || isSingleExecutionFinished)
                {
                    foreach (var item in lstSummary)
                    {
                        var summaryUploaded = item;
                        if (summaryUploaded != null)
                        {
                            if (summaryUploaded.lstErrorMessage == null || summaryUploaded.lstErrorMessage.Count == 0)
                            {
                                bLDataUploader.DeleteDataFromTempTable(summaryUploaded);
                            }
                        }
                    }
                }
            }
            return Json(summary);
        }
    }
    #endregion

}




