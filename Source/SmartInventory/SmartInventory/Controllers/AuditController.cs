using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using BusinessLogics;
using SmartInventory.Settings;
using System.Data;
using Utility;
using System.IO;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using System.Dynamic;
using System.Globalization;
using SmartInventory.Filters;
using System.Text;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using ProjNet.CoordinateSystems;
using Ionic.Zip;


namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class AuditController : Controller
    {
        #region History 
        public ActionResult GetHistory(int systemId = 0, string eType = "", int page = 0, string sort = "", string sortdir = "")
        {
            ViewAuditMasterModel objAudit = new ViewAuditMasterModel();
            objAudit.systemId = systemId;
            objAudit.eType = eType;
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.systemid = systemId;
            objAudit.objFilterAttributes.entityType = eType;

            List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditEntityDetailById(systemId, eType.ToUpper(), objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
            string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
            var userdetails = (User)Session["userDetail"];
            objAudit.webColumns = BLConvertMLanguage.GetEntityWiseColumns(0, objAudit.objFilterAttributes.entityType, "HISTORY", arrIgnoreColumns, userdetails.role_id, userdetails.user_id);
            foreach (Dictionary<string, string> dic in lstReportData)
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
                objAudit.lstData.Add(obj);
            }
            //objAudit.lstData = BLConvertMLanguage.MultilingualConvert(objAudit.lstData, arrIgnoreColumns);
            objAudit.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;
            Session["viewAuditHistory"] = objAudit.objFilterAttributes;
           // GetGeometryHistory();
            return PartialView("_AuditHistory", objAudit);
        }

        public void DownloadAuditHistory()
        {
            if (Session["viewAuditHistory"] != null)
            {
                FilterHistoryAttr objViewFilter = (FilterHistoryAttr)Session["viewAuditHistory"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditEntityDetailById(objViewFilter.systemid, objViewFilter.entityType, objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                lstReportData = BLConvertMLanguage.ExportMultilingualConvert(lstReportData);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData);
               
                var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objViewFilter.entityType.ToUpper()).FirstOrDefault();
                dtReport.TableName = layerDetail.layer_title;
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }
                    
                }
                if (dtReport.Rows.Count > 0)
                {
                    ExportHistoryData(dtReport, "Export_" + layerDetail.layer_title + "_Attribute_History_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }

        private void ExportHistoryData(DataTable dtReport, string fileName)
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

        public ActionResult GetGeometryHistory(int systemId = 0, string eType = "", int page = 0, string sort = "", string sortdir = "")
        {
            FilterHistoryAttr objViewFilter = (FilterHistoryAttr)Session["viewAuditHistory"];
            ViewAuditMasterModel objAudit = new ViewAuditMasterModel();
            objAudit.systemId = systemId;
            objAudit.eType = eType;
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.systemid = systemId;
            objAudit.objFilterAttributes.entityType = eType;

            List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditEntityGeometryDetail(systemId, eType.ToUpper(), objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
            string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "GEOM", "ENTITY_TITLE" };
            var userdetails = (User)Session["userDetail"];
            foreach (Dictionary<string, string> dic in lstReportData)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();

                foreach (var col in dic)
                {

                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                objAudit.lstGeometryData.Add(obj);
                


            }
            objAudit.lstGeometryData = BLConvertMLanguage.MultilingualConvert(objAudit.lstGeometryData, arrIgnoreColumns);
            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;
            Session["_viewAuditHistory"] = objAudit.objFilterAttributes;
            return PartialView("_AuditGeometryHistory", objAudit);
        }
        public void DownloadAuditGeometryHistory(string fileType)
        {
            if (!string.IsNullOrWhiteSpace(fileType))
            {
                if (fileType.ToUpper() == "EXCEL")
                {
                    DownloadAuditGeometryHistoryIntoExcel();
                }
               
                else if (fileType.ToUpper() == "KML")
                {
                    DownloadAuditGeometryHistoryIntoKML();
                }
                else if (fileType.ToUpper() == "SHAPE")
                {
                    ExportAuditGeometryHistoryIntoShape();
                }
               

            }

        }
        public void DownloadAuditGeometryHistoryIntoExcel()
        {

            if (Session["_viewAuditHistory"] != null)
            {

                try
                {
                    ViewAuditMasterModel objAudit = new ViewAuditMasterModel();
                    objAudit.objFilterAttributes = (FilterHistoryAttr)Session["_viewAuditHistory"];
                    objAudit.objFilterAttributes.currentPage = 0;
                    List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditEntityGeometryDetail(objAudit.objFilterAttributes.systemid, objAudit.objFilterAttributes.entityType.ToUpper(), objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
                    lstReportData = BLConvertMLanguage.ExportMultilingualConvert(lstReportData);
                    DataTable dtReport = new DataTable();
                    DataTable bldStatusHistory = new DataTable();
                    DataTable faultStatusHistory = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData, true, ApplicationSettings.numberFormatType, new string[] { "Item_Code", "item code", "PTS_code","Latitude","Longitude" });
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objAudit.objFilterAttributes.entityType.ToUpper()).FirstOrDefault();
                    dtReport.TableName = layerDetail.layer_title;
                    if (dtReport != null && dtReport.Rows.Count > 0)
                    {
                        if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                        if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        if (dtReport.Columns.Contains("geom_type")) { dtReport.Columns.Remove("geom_type"); }
                        if (dtReport.Columns.Contains("entity_type")) { dtReport.Columns.Remove("entity_type"); }
                        if (dtReport.Columns.Contains("geom")) { dtReport.Columns.Remove("geom"); }
                        if (dtReport.Columns.Contains("entity_title")) { dtReport.Columns.Remove("entity_title"); }
                        if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }
                        if (dtReport.Columns.Contains("audit_id")) { dtReport.Columns.Remove("audit_id"); }
                    }
                    //dtReport = Utility.CommonUtility.GetFormattedDataTable(dtReport, ApplicationSettings.numberFormatType);
                    if (dtReport.Rows.Count > 0)
                    {
                        ExportHistoryData(dtReport, "Export_" + layerDetail.layer_title + "_Geometry_Changes_" + Utility.MiscHelper.getTimeStamp());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void DownloadAuditGeometryHistoryIntoKML()
        {
            if (Session["viewAuditHistory"] != null)
            {
                try
                {
                    DataSet ds = new DataSet();
                    ViewAuditMasterModel objAudit = new ViewAuditMasterModel();
                    objAudit.objFilterAttributes = (FilterHistoryAttr)Session["viewAuditHistory"];
                    objAudit.objFilterAttributes.currentPage = 0;
                    objAudit.objFilterAttributes.fileType = "KML";
                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objAudit.objFilterAttributes.entityType.ToUpper()).FirstOrDefault();
                    var userdetails = (User)Session["userDetail"];
                    objAudit.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY");
                    if (objAudit.objFilterAttributes.entityType != null)
                    {
                            objAudit.lstLayers = objAudit.lstLayers.Where(m => objAudit.objFilterAttributes.entityType.Contains(m.layer_name)).ToList();
                    }
                    List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditEntityGeometryDetail(objAudit.objFilterAttributes.systemid, objAudit.objFilterAttributes.entityType.ToUpper(), objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
                    lstReportData = BLConvertMLanguage.ExportMultilingualConvert(lstReportData);
                    DataTable dtReport = new DataTable();
                    dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData, true, ApplicationSettings.numberFormatType, new string[] { "Item_Code", "item code", "PTS_code" });
                    string TempkmlFileName = "ExportAuditGeom_KML_" + layerDetail.layer_title+"_"+ DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
                    string layerName = layerDetail.layer_name;
                    layerName = layerName + "_";
                    dtReport.TableName = layerDetail.layer_title;
                    //dtReport = Utility.CommonUtility.GetFormattedDataTable(dtReport, ApplicationSettings.numberFormatType);
                    if (dtReport.Rows.Count > 0)
                        ds.Tables.Add(dtReport);
                    KMLHelper.DatasetToKML(ds, objAudit.lstLayers, ApplicationSettings.DownloadTempPath, TempkmlFileName, layerName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void ExportAuditGeometryHistoryIntoShape()
        {

            if (Session["viewAuditHistory"] != null)
            {
                try
                {
                    ViewAuditMasterModel objAudit = new ViewAuditMasterModel();
                    objAudit.objFilterAttributes = (FilterHistoryAttr)Session["viewAuditHistory"];
                    objAudit.objFilterAttributes.currentPage = 0;
                    objAudit.objFilterAttributes.fileType = "SHAPE";

                    //Filter the Layer Detail
                    var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objAudit.objFilterAttributes.entityType.ToUpper()).FirstOrDefault();
                    List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditEntityGeometryDetail(objAudit.objFilterAttributes.systemid, objAudit.objFilterAttributes.entityType.ToUpper(), objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
                    lstReportData = BLConvertMLanguage.ExportMultilingualConvert(lstReportData);
                    DataTable dtReport = new DataTable();
                    if (lstReportData.Count > 0)
                    {
                        dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData, true, ApplicationSettings.numberFormatType, new string[] { "Item_Code", "item code", "PTS_code" });
                        dtReport.TableName = layerDetail.layer_title; 
                        if (dtReport != null && dtReport.Rows.Count > 0)
                        {
                            if (dtReport.Columns.Contains("S_NO")) { dtReport.Columns.Remove("S_NO"); }
                            if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                        }
                        //dtReport = Utility.CommonUtility.GetFormattedDataTable(dtReport, ApplicationSettings.numberFormatType);
                        //ExportData(dtReport, layerDetail.layer_title.ToUpper() + "_Report_" + MiscHelper.getTimeStamp(), false);
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtReport);
                    GetShapeFile(ds, "ExportAuditGeom_"+ layerDetail.layer_title+"");
                   
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
            string shapeFilePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"public\Attachments\SI_Shape_" + Guid.NewGuid().ToString().Substring(0, 8);
                                                                                                                                                                                
            if (Directory.Exists(shapeFilePath).Equals(false))
                Directory.CreateDirectory(shapeFilePath);
            for (int j = 0; j < ds.Tables.Count; j++)
            {
                DataTable dtAuditGeom = ds.Tables[j];
                entity = dtAuditGeom.TableName;
                if (dtAuditGeom.AsEnumerable().Where(x => x.Field<string>("geom") != null).ToList().Count > 0)
                {
                    dtAuditGeom = dtAuditGeom.AsEnumerable().Where(x => x.Field<string>("geom") != null).CopyToDataTable();
                }
                else
                    dtAuditGeom.Clear();
                if (dtAuditGeom.Rows.Count > 0)
                {
                    var features = new List<Feature>();
                    string columnName = string.Empty, columnNameList = string.Empty;

                    foreach (DataRow row in dtAuditGeom.Rows)
                    {
                        //add shape attribute and value
                        var geomFactory = new GeometryFactory(new PrecisionModel(), 4326);
                        var wktReader = new WKTReader(geomFactory);
                        var geometry = wktReader.Read(row["geom"].ToString());
                        var attributesTable = new AttributesTable();
                        columnNameList = string.Empty;
                        foreach (DataColumn column in dtAuditGeom.Columns)
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
                dtAuditGeom.Clear();
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
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileNameValue + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "application/zip";
            Response.WriteFile(file.FullName);
            Response.Flush();
            Response.End();
            System.IO.File.Delete(zipshapePath);
        }
        #endregion

        #region Site History 
        public ActionResult GetSiteHistory(int siteId, string lmcType, int page = 0, string sort = "", string sortdir = "")
        {
            ViewAuditSiteInfo objAudit = new ViewAuditSiteInfo();
            objAudit.siteId = siteId;
            objAudit.lmcType = lmcType;
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.siteId = siteId;
            objAudit.objFilterAttributes.lmcType = lmcType;

            List<Dictionary<string, string>> lstAuditGeomData = new BLMisc().GetAuditSiteInfoById(siteId, lmcType.ToUpper(), objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
            foreach (Dictionary<string, string> dic in lstAuditGeomData)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                objAudit.lstData.Add(obj);
            }
            objAudit.objFilterAttributes.totalRecord = lstAuditGeomData.Count > 0 ? Convert.ToInt32(lstAuditGeomData[0].FirstOrDefault().Value) : 0;
            Session["viewSiteAuditHistory"] = objAudit.objFilterAttributes;
            return PartialView("_SiteAuditHistory", objAudit);
        }


        public void DownloadSiteAuditHistory()
        {
            if (Session["viewSiteAuditHistory"] != null)
            {
                FilterSiteHistoryAttr objViewFilter = (FilterSiteHistoryAttr)Session["viewSiteAuditHistory"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                List<Dictionary<string, string>> lstAuditGeomData = new BLMisc().GetAuditSiteInfoById(objViewFilter.siteId, objViewFilter.lmcType, objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                DataTable dtAuditGeom = new DataTable();
                dtAuditGeom = MiscHelper.GetDataTableFromDictionaries(lstAuditGeomData);
                //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objViewFilter.entityType.ToUpper()).FirstOrDefault();
                //dtReport.TableName = layerDetail.layer_title;
                if (dtAuditGeom != null && dtAuditGeom.Rows.Count > 0)
                {
                    if (dtAuditGeom.Columns.Contains("totalrecords")) { dtAuditGeom.Columns.Remove("totalrecords"); }
                    if (dtAuditGeom.Columns.Contains("S_No")) { dtAuditGeom.Columns.Remove("S_No"); }

                }
                if (dtAuditGeom.Rows.Count > 0)
                {
                    ExportHistoryData(dtAuditGeom, "Export_History_SiteInfo_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }
        #endregion

        #region LMC Histroy
        public ActionResult GetLMCHistory(int LMCId, string lmcType, int page = 0, string sort = "", string sortdir = "")
        {
            ViewAuditLMCInfo objAudit = new ViewAuditLMCInfo(); 
            objAudit.LMCId = LMCId;
            objAudit.lmcType = lmcType;
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.LMCId = LMCId;
            objAudit.objFilterAttributes.lmcType = lmcType;

            List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditLMCInfoById(LMCId, lmcType.ToUpper(), objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
            foreach (Dictionary<string, string> dic in lstReportData)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO","SYSTEM ID" };
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                objAudit.lstData.Add(obj);
                objAudit.lstData = BLConvertMLanguage.MultilingualConvert(objAudit.lstData, arrIgnoreColumns);
            }
            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;
            Session["viewLMCAuditHistory"] = objAudit.objFilterAttributes;
            return PartialView("_LMCAuditHistory", objAudit);
        }


        public void DownloadLMCAuditHistory() 
        {
            if (Session["viewLMCAuditHistory"] != null)
            {
                FilterLMCHistoryAttr objViewFilter = (FilterLMCHistoryAttr)Session["viewLMCAuditHistory"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditLMCInfoById(objViewFilter.LMCId, objViewFilter.lmcType.ToUpper(), objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                lstReportData= BLConvertMLanguage.ExportMultilingualConvert(lstReportData);
                // List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditSiteInfoById(objViewFilter.LMCId, objViewFilter.lmcType, objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData);
                //var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objViewFilter.entityType.ToUpper()).FirstOrDefault();
                //dtReport.TableName = layerDetail.layer_title;
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }
                    if (dtReport.Columns.Contains("system id")) { dtReport.Columns.Remove("system id"); }

                }
                if (dtReport.Rows.Count > 0)
                {
                    ExportHistoryData(dtReport, "Export_History_LMCInfo_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }
        #endregion LMC History

        #region Accessories Histroy
        public ActionResult GetAccessoriesHistory(int accessoriesId,  int page = 0, string sort = "", string sortdir = "")
        {
            ViewAuditAccessoriesInfo objAudit = new ViewAuditAccessoriesInfo();
            objAudit.accessories_id = accessoriesId;
            objAudit.objFilterAttributes.accessories_id = accessoriesId;
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;


            objAudit.lstData = new BLMisc().GetAuditAccessoriesById(accessoriesId, objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
            //foreach (Dictionary<string, string> dic in lstReportData)
            //{
            //    var obj = (IDictionary<string, object>)new ExpandoObject();
            //    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "SYSTEM ID" };
            //    foreach (var col in dic)
            //    {
            //        if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
            //        {
            //            obj.Add(col.Key, col.Value);
            //        }
            //    }
            //    objAudit.lstData.Add(obj);
            //}
            objAudit.objFilterAttributes.totalRecord = objAudit.lstData != null && objAudit.lstData.Count > 0 ? objAudit.lstData[0].totalRecords : 0;
            Session["viewAccessoriesAuditHistory"] = objAudit.objFilterAttributes;
            return PartialView("_AccessoriesAuditHistory", objAudit);
        }


        public void DownloadAccessoriesAuditHistory()
        {
            if (Session["viewAccessoriesAuditHistory"] != null)
            {
                FilterAccessoriesHistoryAttr objViewFilter = (FilterAccessoriesHistoryAttr)Session["viewAccessoriesAuditHistory"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                //List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditLMCInfoById(objViewFilter.LMCId, objViewFilter.lmcType.ToUpper(), objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                List<AccessoriesAuditModel> lstReportData = new BLMisc().GetAuditAccessoriesById(objViewFilter.accessories_id, objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable(lstReportData);
                dtReport.TableName = "Accessories History";
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }
                    if (dtReport.Columns.Contains("CREATED_ON")) dtReport.Columns.Remove("CREATED_ON");
                    if (dtReport.Columns.Contains("MODIFIED_ON")) dtReport.Columns.Remove("MODIFIED_ON");
                    if (dtReport.Columns.Contains("action")) dtReport.Columns.Remove("action");

                    if (dtReport.Columns.Contains("entity_type")) { dtReport.Columns["entity_type"].ColumnName= "Entity Type"; }
                    if (dtReport.Columns.Contains("quantity")) { dtReport.Columns["quantity"].ColumnName = "Quantity"; }
                    if (dtReport.Columns.Contains("remarks")) { dtReport.Columns["remarks"].ColumnName = "Remark"; }
                    if (dtReport.Columns.Contains("specification")) { dtReport.Columns["specification"].ColumnName = "Specification"; }
                    if (dtReport.Columns.Contains("vendor_name")) { dtReport.Columns["vendor_name"].ColumnName = "Vendor Name"; }
                    if (dtReport.Columns.Contains("item_code")) { dtReport.Columns["item_code"].ColumnName = "Item Code"; }
                    if (dtReport.Columns.Contains("category")) { dtReport.Columns["category"].ColumnName = "Category"; }
                    if (dtReport.Columns.Contains("subcategory1")) { dtReport.Columns["subcategory1"].ColumnName = "Subcategory 1"; }
                    if (dtReport.Columns.Contains("subcategory2")) { dtReport.Columns["subcategory2"].ColumnName = "Subcategory 2"; }
                    if (dtReport.Columns.Contains("subcategory3")) { dtReport.Columns["subcategory3"].ColumnName = "Subcategory 3"; }
                    if (dtReport.Columns.Contains("parent_network_id")) { dtReport.Columns["parent_network_id"].ColumnName = "Parent Network Id"; }
                    if (dtReport.Columns.Contains("parent_entity_type")) { dtReport.Columns["parent_entity_type"].ColumnName = "Parent Entity Type"; }
                    if (dtReport.Columns.Contains("status")) { dtReport.Columns["status"].ColumnName = "Status"; }
                    if (dtReport.Columns.Contains("network_status")) { dtReport.Columns["network_status"].ColumnName = "Network Status"; }
                    if (dtReport.Columns.Contains("created_by_text")) { dtReport.Columns["created_by_text"].ColumnName = "Created By"; }
                    if (dtReport.Columns.Contains("created_on")) { dtReport.Columns["created_on"].ColumnName = "Created On"; }
                    if (dtReport.Columns.Contains("modified_by_text")) { dtReport.Columns["modified_by_text"].ColumnName = "Modified By"; }
                    if (dtReport.Columns.Contains("modified_on")) { dtReport.Columns["modified_on"].ColumnName = "Modified On"; }
                }
                if (dtReport.Rows.Count > 0)
                {
                    ExportHistoryData(dtReport, "Export_History_Accossories_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }
        #endregion Accessories History

        #region Fiber Link History
        public ActionResult GetFiberLinkHistory(int LinkSystemId, int page = 0, string sort = "", string sortdir = "")
        {
            ViewAuditFiberLinkInfo objAudit = new ViewAuditFiberLinkInfo();
            objAudit.LinkSystemId = LinkSystemId; 
            objAudit.objFilterAttributes.pageSize = 10;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.LinkSystemId = LinkSystemId; 

            List<Dictionary<string, string>> lstReportData = new BLMisc().GetFiberLinkHistoryById(LinkSystemId, objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
            foreach (Dictionary<string, string> dic in lstReportData)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "SYSTEM_ID" };
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                objAudit.lstData.Add(obj);
                objAudit.lstData = BLConvertMLanguage.MultilingualConvert(objAudit.lstData, arrIgnoreColumns);
            }
            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;
            Session["viewFiberLinkHistory"] = objAudit.objFilterAttributes;
            return PartialView("~/views/FiberLink/_AuditFiberLink.cshtml", objAudit);
        }
        public void DownloadFiberLinkHistory()
        {
            if (Session["viewFiberLinkHistory"] != null)
            {
                FilterFiberLinkHistoryAttr objViewFilter = (FilterFiberLinkHistoryAttr)Session["viewFiberLinkHistory"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;

                List<Dictionary<string, string>> lstReportData = new BLMisc().GetFiberLinkHistoryById(objViewFilter.LinkSystemId, objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                lstReportData = BLConvertMLanguage.ExportMultilingualConvert(lstReportData);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData);

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }
                    if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); } 
                }
                if (dtReport.Rows.Count > 0)
                {
                    ExportHistoryData(dtReport, "Export_History_FiberLink_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }
        #endregion Fiber Link History

        #region LandBase Histroy

    
        public ActionResult GetLandBaseLayerHistory(int systemId = 0,string layerTitle="", int landbase_layer_id = 0, int page = 0, string sort = "", string sortdir = "")
        {
            ViewLandbaseAuditMasterModel objAudit = new ViewLandbaseAuditMasterModel();
            objAudit.systemId = systemId;
            objAudit.landbase_layer_id = landbase_layer_id;
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.id = systemId;
            objAudit.objFilterAttributes.landbase_layer_id = landbase_layer_id;

            List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditLandBaseInfoById(systemId, landbase_layer_id, objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
            foreach (Dictionary<string, string> dic in lstReportData)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "SYSTEM ID"  };
                //objAudit.webColumns = ConvertMultilingual.GetLandbaseEntityWiseColumns(landbase_layer_id,  arrIgnoreColumns);

                //objExportEntitiesReport.webColumns = ConvertMultilingual.GetLandBaseEntityWiseColumns(objExportEntitiesReport.objReportFilters.SelectedLayerId[0], objExportEntitiesReport.objReportFilters.layerName, "REPORT", arrIgnoreColumns, userdetails.role_id, userdetails.user_id);

                var userdetails = (User)Session["userDetail"];
                objAudit.webColumns = BLConvertMLanguage.GetLandBaseEntityWiseColumns(landbase_layer_id, layerTitle,  "HISTORY", arrIgnoreColumns, userdetails.role_id, userdetails.user_id);
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        //if (col.Key.ToUpper() == "ST_ASTEXT")
                        //{
                        //    var extent = col.Value.TrimStart("POINT(".ToCharArray()).TrimEnd(")".ToCharArray());
                        //    string[] lnglat = extent.Split(new string[] { " " }, StringSplitOptions.None);

                        //    obj.Add("latitude", lnglat[1].ToString());
                        //    obj.Add("longitude", lnglat[0].ToString());
                        //}
                        //else
                        //{
                            obj.Add(col.Key, col.Value == null ? "" : col.Value);
                        //}
                        
                    }
                }
                objAudit.lstData.Add(obj);
                objAudit.lstData = BLConvertMLanguage.MultilingualConvert(objAudit.lstData, arrIgnoreColumns);
            }
            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;
            Session["viewLandBaseAuditHistory"] = objAudit.objFilterAttributes;
            return PartialView("_LandBaseLayerAuditHistory", objAudit);
        }


        public void DownloadLandBaseLayerAuditHistory()
        {
            if (Session["viewLandBaseAuditHistory"] != null)
            {
                FilterLandBaseHistoryAttr objViewFilter = (FilterLandBaseHistoryAttr)Session["viewLandBaseAuditHistory"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditLandBaseInfoById(objViewFilter.id, objViewFilter.landbase_layer_id, objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                lstReportData = BLConvertMLanguage.ExportMultilingualConvert(lstReportData);

                //List<Dictionary<string, string>> lstReport = new List<Dictionary<string, string>>(); ;

                //foreach (Dictionary<string, string> dic in lstReportData)
                //{
                //    var obj = new Dictionary<string, string>();
                //    foreach (var col in dic)
                //    {

                //        if (col.Key.ToUpper() == "ST_ASTEXT")
                //        {
                //            var extent = col.Value.TrimStart("POINT(".ToCharArray()).TrimEnd(")".ToCharArray());
                //            string[] lnglat = extent.Split(new string[] { " " }, StringSplitOptions.None);

                //            obj.Add("latitude", lnglat[1].ToString());
                //            obj.Add("longitude", lnglat[0].ToString());
                //        }
                //        else
                //        {
                //            obj.Add(col.Key, col.Value == null ? "" : col.Value);
                //        }
                //    }

                //    lstReport.Add(obj);
                //}

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData); 
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }
                    if (dtReport.Columns.Contains("system id")) { dtReport.Columns.Remove("system id"); }

                }
                if (dtReport.Rows.Count > 0)
                {
                    ExportHistoryData(dtReport, "Export_History_LandBase_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }
        #endregion LMC History
    }
}