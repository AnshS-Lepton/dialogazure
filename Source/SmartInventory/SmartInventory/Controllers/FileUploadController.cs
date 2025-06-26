using Models;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics;
using System.Diagnostics;
using SmartInventory.Settings;
using Utility;
using System.Data;
using Ionic.Zip;
using System.Net;
using Lepton.GISConvertor;
using Models.WFM;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class FileUploadController : Controller
    {
        //
        // GET: /FileUpload/
        public ActionResult Index()
        {
            return View();
        }
        #region External Data Uploader
        public ActionResult UploadExternalData()
        { return PartialView("_UploadExternalData"); }
        [HttpPost]
        public ActionResult UploadOtherFile(string filetype)

        {
            string extention = string.Empty;
            string file_extention = string.Empty;
            int filesize = 0;
            String[] validfiles = null;
            var file_folderpath = ConfigurationManager.AppSettings["ExternalDataPath"] + "\\" + filetype.ToUpper();
            int user_id = Convert.ToInt32(Session["user_id"]);
            List<string> missingfiles = new List<string>();
            int filecount = 0;
            switch (filetype.ToLower())
            {
                case "tab":
                    file_extention = ".tab";
                    validfiles = new string[4] { "DAT", "ID", "MAP", "TAB" };
                    filecount = validfiles.Length;
                    break;
                case "shape":
                    file_extention = ".shp";
                    validfiles = new string[4] { "shp", "dbf", "prj", "shx" };
                    filecount = validfiles.Length;
                    break;

                case "dxf":
                    file_extention = ".dxf";
                    validfiles = new string[1] { "dxf"};
                    filecount = validfiles.Length;
                    break;
                default:
                    break;
            }


            if (Request != null)
            {
                if (Request.Files.Count >= filecount)
                {
                    var userfilename = filetype + "_" + DateTimeHelper.Now.ToString("yyyyMMddHHmmss");
                    var userAccessType = Request.Form["rdbAccess"];
                    HttpFileCollectionBase files_upload = Request.Files;

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = files_upload[i];
                        var objfile = Request.Files[i];
                        var fileName = objfile.FileName;
                        extention = Path.GetExtension(fileName);
                        extention = extention.Split('.')[1];
                        filesize += file.ContentLength;
                        if (Array.Exists(validfiles, element => element.ToUpper() == extention.ToUpper()))
                        {
                            var filepath = Path.Combine(Server.MapPath(file_folderpath), userfilename + "." + extention);
                            objfile.SaveAs(filepath);
                        }
                        else
                        {
                            missingfiles.Add(fileName);
                        }
                    }

                    var ExternalDatafolder = Server.MapPath(ConfigurationManager.AppSettings["ExternalDataPath"] + "\\KML\\");
                    var inputfolder = Server.MapPath(ConfigurationManager.AppSettings["ExternalDataPath"] + "\\" + filetype + "\\");

                    string dirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
                    if (filetype.ToLower() == "dxf")
                    {
                        dirPath = Path.GetDirectoryName(Path.GetDirectoryName(dirPath))+ "\\";
                        var SourceSridId = Request.Form["SourceId"];
                        
                        string baseFolder = Path.Combine(Server.MapPath("~/Uploads/ExternalData/DXF/"));
                        string projectName = "SmartPlanner";
                        string kmlFolder = Path.Combine(Server.MapPath("~/Uploads/ExternalData/KML/"));
                        string filePath = baseFolder;
                        string GeoJsonFolder = Path.Combine(Server.MapPath("~/Uploads/GeoJson/"));
                        DataTable dataTable = new DataTable();
                        var converter = new Convertor(baseFolder, projectName, kmlFolder, GeoJsonFolder);
                        ConvertorResponse response = converter.getKML("DXF", filePath, userfilename, userfilename, SourceSridId);
                        //string shp2kml_convertorfolder = "ogr2ogr_convertor";
                        //var arguments = "-f KML " + ExternalDatafolder + userfilename + ".kml " + inputfolder + /*"\\" +*/ userfilename + file_extention + 
                        //    " -t_srs EPSG:4326 " + SourceSridId;
                        //var workingdir = dirPath + "GIS_Convertor\\" + shp2kml_convertorfolder;
                        //var exe_filename = dirPath + "GIS_Convertor\\" + shp2kml_convertorfolder + "\\ogr2ogr.exe";
                        //var proc = new Process
                        //{
                        //    StartInfo = new ProcessStartInfo
                        //    {
                        //        WorkingDirectory = workingdir,
                        //        FileName = exe_filename,
                        //        Arguments = arguments,
                        //        UseShellExecute = false,
                        //        RedirectStandardOutput = true
                        //    }
                        //};
                        //proc.Start();
                        //proc.WaitForExit();

                        
                    }
                    else
                    {
                        string shp2kml_convertorfolder = "shp2kml_convertor";
                        var arguments = "-f KML " + ExternalDatafolder + userfilename + ".kml " + inputfolder + userfilename + file_extention + " -t_srs EPSG:4326";

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


                    DirectoryInfo folder = new DirectoryInfo(inputfolder);
                    if (folder.Exists) // else: Invalid folder!
                    {
                        FileInfo[] files = folder.GetFiles(userfilename + ".*");

                        //foreach (FileInfo file in files)
                        //{
                        //    file.Delete();
                        //}
                    }


                    ExternalDataUploader extrnaldata = new ExternalDataUploader();
                    extrnaldata.file_name = userfilename;
                    extrnaldata.type = "kml";
                    extrnaldata.is_public = userAccessType == "Public" ? true : false;
                    extrnaldata.created_by = user_id;
                    extrnaldata.display_filename = Request.Form["txtfilename"] + file_extention;
                    extrnaldata.file_size = filesize;
                    bool savefile = new BLFileUploader().SaveExternalFileDtl(extrnaldata, user_id);

                    if (savefile)
                    {
                        return Json(new { strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_154, msg = "Success" }, JsonRequestBehavior.AllowGet);//"File uploaded successfully!"
                    }
                    else if (missingfiles.Count > 0)
                    {
                        return Json(new { strReturn = string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_068, string.Join(",", missingfiles)) , msg = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { strReturn = Resources.Resources.SI_OSP_BUL_JQ_RPT_004, msg = "Error" }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new { strReturn = string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_069, string.Join(",", validfiles)), msg = "Error" }, JsonRequestBehavior.AllowGet);//"Please upload all files with following extentions : " + string.Join(",", validfiles)
                }
            }
            else
            {
                return Json(new { strReturn = Resources.Resources.SI_OSP_BUL_JQ_RPT_004, msg = "Error" }, JsonRequestBehavior.AllowGet);
            }
           
        }

        [HttpPost]
        public ActionResult UploadKMLZFile()
        {
            ExternalDataUploader extrnldata = new ExternalDataUploader();
            string extention = string.Empty;
            var folderpath = ConfigurationManager.AppSettings["ExternalDataPath"];
            int user_id = Convert.ToInt32(Session["user_id"]);
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files[0];
                var objfile = Request.Files[0];
                //var userfilename = Request.Form["txtfilename"] + "_" + DateTimeHelper.Now.ToString("yyyyMMddHHmmss");
                var userAccessType = Request.Form["rdbAccess"];
                var fileName = Request.Files[0].FileName;
                //extention = fileName.Split('.')[1];
                extention = Path.GetExtension(fileName);
                extention = extention.Split('.')[1];
                var userfilename = extention + "_" + DateTimeHelper.Now.ToString("yyyyMMddHHmmss");//0987657556566545646.kml
                var filepath = Path.Combine(Server.MapPath(folderpath + extention.ToUpper()), userfilename + "." + extention);
                objfile.SaveAs(filepath);
                extrnldata.file_name = userfilename;
                extrnldata.type = extention;
                extrnldata.is_public = userAccessType == "Public" ? true : false;
                extrnldata.created_by = user_id;
                extrnldata.display_filename = Request.Form["txtfilename"] + "." + extention;
                extrnldata.file_size = file.ContentLength;
                bool savefile = new BLFileUploader().SaveExternalFileDtl(extrnldata, user_id);

                if (savefile)
                {
                    return Json(new { strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_154, msg = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { strReturn = Resources.Resources.SI_OSP_BUL_JQ_RPT_004, msg = "Error" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { strReturn = Resources.Resources.SI_OSP_BUL_JQ_RPT_004, msg = "Error" }, JsonRequestBehavior.AllowGet);
            }

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
        public ActionResult DeleteExtrnlDataFile(string filename, int fileid, string type)
        {
            var filePath = ConfigurationManager.AppSettings["ExternalDataPath"];
            filename = filename + "." + type;
            string extention = Path.GetExtension(filename);
            extention = extention.Split('.')[1];
            string folderPath = Server.MapPath(filePath + extention + "\\");
            string result = string.Empty;
            FileInfo file = new FileInfo(folderPath + filename);
            if (file.Exists)//check file exsit or not
            {
                file.Delete();

                int deleteChk = new BLExternalFileUploader().DeleteExtrnlDataFileDtl(fileid);
                if (deleteChk == 1)
                {
                    result = Resources.Resources.SI_ISP_GBL_JQ_FRM_003;
                    return Json(new { strReturn = result, msg = "OK" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { strReturn = "", msg = "Error" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { strReturn = result, msg = "Error" }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion
        public ActionResult GetFileUploader(ModelExternalDataDetails fdtl)
        {
            var userdetails = (User)Session["userDetail"];
            fdtl.lstUserModule= new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            fdtl.doctypeddllist = new BLLayer().GetDropDownList("ddldocumenttype");  //"LinkType" ddldocumenttype
            var objDDL = new BLMisc().GetDropDownList(fdtl.eType);
            fdtl.lstImageUpload = objDDL.Where(x => x.dropdown_type == DropDownType.ddl_entity_checklist.ToString()).ToList();

            return PartialView("_UploadFile", fdtl);
        }

        public ActionResult UploadProfileImage()
        {
            return PartialView("_UploadProfileImage");
        }

        //static void uploadImagefiles(HttpPostedFile postedFile, string newfilename)
        //{
        //    try
        //    {

        //        System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"] + "/" + newfilename);
        //        string tempPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"];

        //        ftpReq.UseBinary = true;
        //        ftpReq.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
        //        ftpReq.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"].Normalize(), System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"].Normalize());

        //        string savepath = HttpContext.Current.Server.MapPath(tempPath);
        //        postedFile.SaveAs(savepath + @"\" + newfilename);
        //        byte[] b = File.ReadAllBytes(@"" + savepath + "/" + newfilename);
        //        ftpReq.ContentLength = b.Length;
        //        using (Stream s = ftpReq.GetRequestStream())
        //        {
        //            s.Write(b, 0, b.Length);
        //        }

        //        System.Net.FtpWebResponse ftpResp = (System.Net.FtpWebResponse)ftpReq.GetResponse();
        //        System.IO.File.Delete(@"" + savepath + "/" + newfilename);
        //    }
        //    catch (Exception ex)
        //    {
        //      //  ErrorLog.LogErrorToLogFile(ex, "[uploadImage][imageHandler.ashx]");
        //    }


        //}
        public ActionResult getExternalData(ModelExternalDataDetails objfiledetail)
        {
            var filepath = ConfigurationManager.AppSettings["ExternalDataPath"];
            var objUser = ((User)Session["userDetail"]);
            objfiledetail.objExternalDataFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objfiledetail.objExternalDataFilter.currentPage = 1;
            objfiledetail.objExternalDataFilter.sort = "";
            objfiledetail.objExternalDataFilter.orderBy = "";
            var fileList = new BLFileUploader().GetExternalDataFileDetails(objfiledetail.objExternalDataFilter, objUser.user_id);
            objfiledetail.objExternalDataFilter.totalRecord = fileList.Count > 0 ? fileList[0].totalRecords : 0;
            string Filename = string.Empty;
            foreach (var item in fileList)
            {
                Filename = item.file_name + '.' + item.type;
                item.login_user = objUser.user_id;
                item.filepath = filepath + item.type.ToUpper();
                //item.created_by_text = objUser.user_name;
                item.created_on = MiscHelper.FormatDateTime(item.created_on.ToString());
                item.file_size = BytesToString(Convert.ToInt32(item.file_size));
            }
            objfiledetail.lstFileDetails = fileList;
            objfiledetail.lstUserModule = new BLLayer().GetUserModuleAbbrList(objUser.user_id, UserType.Web.ToString());
            return PartialView("_ViewExternalData", objfiledetail);
        }
        public PartialViewResult GetExternalFileData(ExternalDataFilter objFilter)
        {
            ModelExternalDataDetails objfiledetails = new ModelExternalDataDetails();
            var objUser = ((User)Session["userDetail"]);
            var filepath = ConfigurationManager.AppSettings["ExternalDataPath"];
            objfiledetails.objExternalDataFilter = objFilter;
            objfiledetails.objExternalDataFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            var fileList = new BLFileUploader().GetExternalDataFileDetails(objfiledetails.objExternalDataFilter, objUser.user_id);
            string Filename = string.Empty;
            foreach (var item in fileList)
            {
                item.login_user = objUser.user_id;
                item.filepath = filepath + item.type.ToUpper();
                // item.created_by_text = objUser.user_name;
                item.created_on = MiscHelper.FormatDateTime(item.created_on.ToString());
                item.file_size = BytesToString(Convert.ToInt32(item.file_size));
            }
            objfiledetails.lstFileDetails = fileList;
            return PartialView("_ViewExternalDataList", objfiledetails.lstFileDetails);
        }
        public ActionResult DownloadExternlDataFile(int fileid)
        {
            MiscHelper mh = new MiscHelper();
            try
            {
                string zipName = string.Empty;
                var FileDetails = new BLFileUploader().getDownloadFileDetails(fileid);
                var extention = Path.GetExtension(FileDetails.display_filename);
                extention = extention.Split('.')[1];
                var folderPath = Server.MapPath(ConfigurationManager.AppSettings["ExternalDataPath"] + "\\" + (extention.ToUpper() == "SHP" ? "SHAPE" : extention.ToUpper()) + "\\");
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    DirectoryInfo fileDirectory = new DirectoryInfo(folderPath);
                    if (fileDirectory.Exists) // else: Invalid folder!
                    {
                        FileInfo[] files = fileDirectory.GetFiles(FileDetails.file_name + ".*");
                        foreach (FileInfo filedtl in files)
                        {
                            string filePath = folderPath + "\\" + filedtl.Name;
                            zip.AddFile(filePath, "");
                        }
                    }
                    zipName = String.Format("{0}.zip", FileDetails.display_filename.Split('.')[0]);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        zip.Save(memoryStream);
                        return File(memoryStream.ToArray(), "application/zip", zipName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getFileSizeExternalData(ModelExternalDataDetails objfiledetail)
        {
            
            var filepath = ConfigurationManager.AppSettings["ExternalDataPath"];
            var objUser = ((User)Session["userDetail"]);
            objfiledetail.objExternalDataFilter.pageSize = 0;
            objfiledetail.objExternalDataFilter.currentPage = 0;
            objfiledetail.objExternalDataFilter.sort = "";
            objfiledetail.objExternalDataFilter.orderBy = "";
            var fileList = new BLFileUploader().GetExternalDataFileDetails(objfiledetail.objExternalDataFilter, objUser.user_id);
            foreach (var item in fileList)
            {
            
                var extention = Path.GetExtension(item.display_filename);
                extention = extention.Split('.')[1];
                var folderPath = Server.MapPath(ConfigurationManager.AppSettings["ExternalDataPath"] + "\\" + (extention.ToUpper() == "SHP" ? "SHAPE" : extention.ToUpper()) + "\\");
                DirectoryInfo fileDirectory = new DirectoryInfo(folderPath);
                HttpFileCollectionBase files_upload = Request.Files;
                if (fileDirectory.Exists) // else: Invalid folder!
                {
                    int filesize = 0;
                    FileInfo[] files = fileDirectory.GetFiles(item.file_name + ".*");
                    foreach (FileInfo filedtl in files)
                    {
                        filesize += Convert.ToInt32(filedtl.Length);                        
                    }
                    if (filesize > 0)
                    {
                        bool updateFileSize = new BLFileUploader().getUpdatedFileSize(new ExternalDataUploader() { file_size = filesize, id=item.id });
                    }
                }
            }
        }
    }
}