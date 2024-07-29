
using BusinessLogics;
using Ionic.Zip;
using iTextSharp.text;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web.Mvc;
using Utility;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class FileDownloadController : Controller
    {
        BLDataUploader bLDataUploader;
        public FileDownloadController()
        {
            bLDataUploader = new BLDataUploader();
        }

        public ActionResult DownloadUploadLogs(int id, string status)
        {
            try
            {
                DataTable dt = bLDataUploader.GetUploadLogs(id, status);
                if (dt.Rows.Count > 0)
                {
                    UploadSummary summary = bLDataUploader.Get(id);
                    List<Mapping> listMapping = bLDataUploader.GetMappings(summary.entity_type);
                    string[] selectedColumns;
                    // string[] selectedColumns = listMapping.OrderBy(x => x.ColumnOrder).Select(m => m.DbColName).ToArray();
                    if (status.ToUpper() == "SUCCESS")
                    {
                        // selectedColumns = listMapping.OrderBy(x => x.ColumnOrder).Select(m => m.DbColName).ToArray();
                        selectedColumns = listMapping.Where(x => x.TemplateColName != "client_id" && x.TemplateColName != "parent_client_id").OrderBy(x => x.ColumnOrder).Select(m => m.DbColName).ToArray();
                    }
                    else
                    {
                        selectedColumns = listMapping.OrderBy(x => x.ColumnOrder).Select(m => m.DbColName).ToArray();
                    }
                    string[] defaultColumns = { "uploaded_by", "uploaded_on", "is_valid_record", "message" };
                    selectedColumns = selectedColumns.Concat(defaultColumns).Distinct().ToArray();
                    DataTable dtFilter = new DataView(dt).ToTable(false, selectedColumns);
                    if (dtFilter != null && dtFilter.Columns.Contains("sp_geometry"))
                    {
                        dtFilter.Columns.Remove("sp_geometry");
                    }
                    foreach (DataColumn dc in dtFilter.Columns)
                    {
                        Mapping mapping = listMapping.Where(m => m.DbColName.ToUpper() == dc.ColumnName.ToUpper()).FirstOrDefault();
                        if (mapping == null)
                        {
                            dc.ColumnName = dc.ColumnName;//.ToCamelCase();
                        }
                        else
                        {
                            dc.ColumnName = mapping.TemplateColName;
                        }
                    }
                       if (dtFilter.Rows.Count > 0)
                      {
                        var layerDetails = Settings.ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == summary.entity_type.ToUpper()).FirstOrDefault();

                        //UploadLogs_{Layer Title}_DDMMYYYY-HHMMSS.xlsx---Given by Ram
                        var filename = "UploadLogs_" + layerDetails.layer_title.ToUpper() + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
                            //Commented by pk
                            //string filepath = System.Web.HttpContext.Current.Server.MapPath("~/uploads/temp/") + filename;
                            //Commented end by pk
                             string filepath = System.Web.HttpContext.Current.Server.MapPath(Settings.ApplicationSettings.DownloadTempPath) + filename;
                            string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dtFilter, filepath);
                            byte[] fileBytes = System.IO.File.ReadAllBytes(file);

                            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                        
                    }
                    else
                    {
                        return Json("File not Exists");
                    }
                }
                else
                {
                    return Json("File not Exists");
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadUploadLogs", "FileDownloadController", ex);
                return null;
            }
        }

        public ActionResult DownloadTemplate(string entityname)
        {
            try
            {
                if (entityname != null)
                {
                    var Excelfilename = entityname + "_template" + ".xlsx";
                    var KMLfilename = entityname + "_template" + ".kml";

                    if ((entityname == EntityType.Cable.ToString()) || (entityname == EntityType.Trench.ToString()) || (entityname == EntityType.Duct.ToString()))
                    {
                        string ExcelFileAddress = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ExcelTemplatePath"]) + "/" + Excelfilename;
                        string KMLFileAddress = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ExcelTemplatePath"]) + "/" + KMLfilename;

                        using (ZipFile zip = new ZipFile())
                        {
                            //Add files to zip files
                            zip.AddFile(ExcelFileAddress, "");
                            zip.AddFile(KMLFileAddress, "");

                            System.IO.MemoryStream output = new System.IO.MemoryStream();
                            zip.Save(output);

                            return File(output.ToArray(), "application/zip", entityname + ".zip");
                        }
                    }
                    else
                    {
                        string FileAddress = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ExcelTemplatePath"]) + "/" + Excelfilename;

                        byte[] fileBytes = System.IO.File.ReadAllBytes(FileAddress);

                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Excelfilename);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadUploadLogs", "FileDownloadController", ex);
            }
            return null;
        }
        public JsonResult checkLogFileExist(int id, string status)
        {
            DataTable dt = bLDataUploader.GetUploadLogs(id, status);
            PageMessage objPageMessage = new PageMessage();
            objPageMessage.status = dt.Rows.Count > 0 ? StatusCodes.OK.ToString() : StatusCodes.FAILED.ToString();
            objPageMessage.message = dt.Rows.Count == 0 ? "Record does not exist !" : "";
            return Json(objPageMessage, JsonRequestBehavior.AllowGet);
        }
    }
}