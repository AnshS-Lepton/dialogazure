using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utility;
using System.Text.RegularExpressions;
using System.Web.Security;
using static Mono.Security.X509.X520;
using Models.WFM;
using static NPOI.HSSF.Util.HSSFColor;
using System.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class FeToolsController : Controller
    {
        #region
        [HttpPost]
        public ActionResult AddFEtools(int group_id = 0)
        {
            userFeToolMapping objfetools = new userFeToolMapping();
            PageMessage objMsg = new PageMessage();

            try
            {
            

                if (group_id == 0)
                {
                    var objLgnUsrDtl = (User)Session["userDetail"];
                    int user_id = objLgnUsrDtl.user_id;
                    //objfetools.lstusername = new BLUser().GetUsernameDetails();

                    objfetools.lstusername = new BL_Fe_Tools().GetFEUserDeatils();
                    objfetools.lstFEtool = new BLUser().BindFETool();
                }
                else
                {
                    objfetools = new BL_Fe_Tools().getfetoolid(group_id);
                    objfetools.lstusername = new BLUser().GetUsernameDetails();
                    objfetools.lstFEtool = new BLUser().BindFETool();
                    var lst = new BLUser().GetUsernameDetails(objfetools.user_id);
                    objfetools.user_id = Convert.ToInt32(lst[0].value);
                    var lstfetool = new BLUser().BindFETooldropdown(objfetools.tool_id);
                    objfetools.tool_id = Convert.ToInt32(lstfetool[0].value);
                    objfetools.date_v = DateTime.Parse(objfetools.date_value.ToString().Split(' ')[0]).ToString("dd-MMM-yyyy");


                }


                //}

            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("AddFEtools()", "AddFEtools/FeTools", ex);
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while mapping Fetools!!";

            }
            return View("AddFEtools", objfetools);
        }
        [HttpPost]
        public JsonResult SaveFETools(userFeToolMapping objLyrGroup)
        {
            ModelState.Clear();
            var userid = Convert.ToInt32(Session["user_id"]);
           
            PageMessage objMsg = new PageMessage();
            try
            {
                if (objLyrGroup.user_id != 0 && objLyrGroup.tool_id != 0)
                {

                    var response = new BL_Fe_Tools().SaveFeToolsdetails(objLyrGroup, userid);
                    if (response.action_type == "Save")
                    {
                        Session["Fetools_id"] = response.id;
                        Session["Fetuser_id"] = response.user_id;
                        Session["tools_id"] = response.tool_id;

                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.message = "User details saved successfully!";
                    }
                    else if (response.action_type == "Update")
                    {
                        Session["Fetools_id"] = objLyrGroup.id;
                        Session["Fetuser_id"] = objLyrGroup.user_id;
                        Session["tools_id"] = objLyrGroup.tool_id;

                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.message = " Updated successfully!";
                    }
                    else if (response.action_type == "Duplicate")
                    {
                        Session["Fetools_id"] = objLyrGroup.id;
                        Session["Fetuser_id"] = objLyrGroup.user_id;
                        Session["tools_id"] = objLyrGroup.tool_id;

                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.message = "User details already exist.";
                    }
                    else if (string.IsNullOrEmpty(response.action_type))
                    {
                        objMsg.status = ResponseStatus.FAILED.ToString();
                        objMsg.message = "Unable to update FE Tools as it is already been mapped!";
                    }
                }
                else
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "Mandatory fields required";
                }
                objLyrGroup.pageMsg = objMsg;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("SaveFETools()", "SaveFETools/FeTools", ex);
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while mapping Fetools!!";
            }
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        public IList<KeyValueDropDown> BindSearchBy(ViewFETools vwlyrgrp)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "User Name", value = "user_name" });
            return vwlyrgrp.lstSearchBy = items.OrderBy(m => m.key).ToList();
        }
        public ActionResult Viewfetools(ViewFETools objViewFetools, int page = 0, string sort = "", string sortdir = "")
        {
            var userid = Convert.ToInt32(Session["user_id"]);
            var objLgnUsrDtl = (User)Session["userDetail"];
            ViewBag.user_id = userid;
            ViewBag.created_by = objLgnUsrDtl.created_by;
            ViewBag.roleid = objLgnUsrDtl.role_id;

            BindSearchBy(objViewFetools);
            if (sort != "" || page != 0)
            {
                objViewFetools.objGridAttributes = (CommonGridAttributes)Session["viewfetools"];
            }
            objViewFetools.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewFetools.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewFetools.objGridAttributes.sort = sort;
            objViewFetools.objGridAttributes.orderBy = sortdir;
            objViewFetools.fetools = new BL_Fe_Tools().GetFettoollist(objViewFetools.objGridAttributes, userid);
          


            objViewFetools.objGridAttributes.totalRecord = objViewFetools.fetools != null && objViewFetools.fetools.Count > 0 ? objViewFetools.fetools[0].totalRecords : 0;
            Session["viewfetools"] = objViewFetools.objGridAttributes;
            return View("Viewfetools", objViewFetools);
        }
        [HttpPost]
        public ActionResult UploadFTEImage(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    //var systemId = "fe_tools"//collection["system_Id"];
                    var entityType = "fe_tools";//collection["entity_type"];
                    //var featureName = collection["feature_name"];
                    var attachmentType = collection["document_type"];
                    int systemId = Convert.ToInt32(Session["Fetools_id"]);
                    int feuserid = Convert.ToInt32(Session["Fetuser_id"]);
                    int tools_id = Convert.ToInt32(Session["tools_id"]);
                    User objUser = (User)(Session["userDetail"]);

                    if (systemId != 0)
                    {

                        HttpFileCollectionBase files = Request.Files;
                        VailidateAttachment obj = ValidateDocumentFileTypefetools(files);
                        if (!string.IsNullOrEmpty(obj.invalidattachmentType))
                        {
                            jResp.message = obj.invalidattachmentType;
                            jResp.status = StatusCodes.INVALID_FILE.ToString();
                            return Json(jResp, JsonRequestBehavior.AllowGet);

                        }
                        else if (!string.IsNullOrEmpty(obj.invalidattachmentsize))
                        {
                            jResp.message = obj.invalidattachmentsize;
                            jResp.status = StatusCodes.INVALID_FILE.ToString();
                            return Json(jResp, JsonRequestBehavior.AllowGet);

                        }
                        else if (!string.IsNullOrEmpty(obj.invalidattachmentename))
                        {
                            jResp.message = obj.invalidattachmentename;
                            jResp.status = StatusCodes.INVALID_FILE.ToString();
                            return Json(jResp, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];
                                string FileName = file.FileName;
                                string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);

                                string strFilePath = FETOOLSUploadfileOnFTP(entityType, systemId, file, "Images", strNewfilename, feuserid, tools_id);

                                // }

                                FETOOLS_Attachment objAttachment = new FETOOLS_Attachment();
                                objAttachment.tools_mapping_id = Convert.ToInt32(systemId);
                                objAttachment.file_name = strNewfilename;
                                objAttachment.file_extension = Path.GetExtension(FileName);
                                objAttachment.file_location = strFilePath;
                                objAttachment.upload_type = attachmentType;
                                objAttachment.uploaded_by = objUser.user_id.ToString();
                                objAttachment.file_size = file.ContentLength;
                                objAttachment.uploaded_on = DateTime.Now;
                                //Save Image on FTP and related detail in database..
                                var savefile = new BLFetools_Attachement().SaveFetools_attachement(objAttachment);
                            }
                            //jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_154;
                            jResp.status = StatusCodes.OK.ToString();
                            return Json(jResp, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        jResp.message = "Oops Something went wrong!.";
                        jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                        return Json(jResp, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadDocument()", "fEtOOLS", ex);
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_243;
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = "No files selected.";
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadFTEDocument(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    var entityType = "fe_tools";
                    var attachmentType = "Document";
                    int systemId = Convert.ToInt32(Session["Fetools_id"]);
                    int feuserid = Convert.ToInt32(Session["Fetuser_id"]);
                    int tools_id = Convert.ToInt32(Session["tools_id"]);
                    User objUser = (User)(Session["userDetail"]);

                    if (systemId != 0)
                    {

                        HttpFileCollectionBase files = Request.Files;
                        VailidateAttachment obj = ValidateDocumentFileTypefetools(files);
                        if (!string.IsNullOrEmpty(obj.invalidattachmentType))
                        {
                            jResp.message = obj.invalidattachmentType;
                            jResp.status = StatusCodes.INVALID_FILE.ToString();
                            return Json(jResp, JsonRequestBehavior.AllowGet);

                        }
                        else if (!string.IsNullOrEmpty(obj.invalidattachmentsize))
                        {
                            jResp.message = obj.invalidattachmentsize;
                            jResp.status = StatusCodes.INVALID_FILE.ToString();
                            return Json(jResp, JsonRequestBehavior.AllowGet);

                        }
                        else if (!string.IsNullOrEmpty(obj.invalidattachmentename))
                        {
                            jResp.message = obj.invalidattachmentename;
                            jResp.status = StatusCodes.INVALID_FILE.ToString();
                            return Json(jResp, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];
                                string FileName = file.FileName;
                                string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                                string strFilePath = "";
                                strFilePath = FETOOLSUploadfileOnFTP(entityType, systemId, file, attachmentType, strNewfilename, feuserid, tools_id );


                                // get User Detail..
                                FETOOLS_Attachment objAttachment = new FETOOLS_Attachment();
                                objAttachment.tools_mapping_id = Convert.ToInt32(systemId);

                                objAttachment.file_name = strNewfilename;
                                objAttachment.file_extension = Path.GetExtension(FileName);
                                objAttachment.file_location = strFilePath;
                                objAttachment.upload_type = attachmentType;
                                objAttachment.uploaded_by = objUser.user_id.ToString();
                                objAttachment.file_size = file.ContentLength;
                                objAttachment.uploaded_on = DateTime.Now;
                                //Save Image on FTP and related detail in database..
                                var savefile = new BLFetools_Attachement().SaveFetools_attachement(objAttachment);
                            }
                            jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_154;
                            jResp.status = StatusCodes.OK.ToString();
                            return Json(jResp, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        jResp.message = "Oops Something went wrong!.";
                        jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                        return Json(jResp, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadFTEDocument()", "FeTools", ex);
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_243;
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = "No files selected.";
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }
       public static string FETOOLSUploadfileOnFTP(string sEntityType, int sEntityId, HttpPostedFileBase postedFile, string sUploadType, string newfilename,int user_id,int tool_id, string featureType = null)
        {
            try
            {
                string strFTPFilePath = "";
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];
                

                if (isValidfeFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {
                    // Create Directory if not exists and get Final FTP path to save file..
                    strFTPFilePath = CreateNestedDirectoryOnFTP(strFTPPath, strFTPUserName, strFTPPassWord, featureType, sEntityType, user_id.ToString(), tool_id.ToString(), sUploadType);

                    //Prepare FTP Request..
                    //if (sUploadType.ToUpper() == "IMAGES")
                    //{
                    //    string thumnailImageName = "Thumb_" + newfilename;
                    //    FtpWebRequest ftpThumbnailImage = (FtpWebRequest)WebRequest.Create(strFTPFilePath + thumnailImageName);
                    //    ftpThumbnailImage.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                    //    ftpThumbnailImage.Method = WebRequestMethods.Ftp.UploadFile;
                    //    ftpThumbnailImage.UseBinary = true;
                    //    // var image = System.Drawing.Image.FromStream(postedFile.InputStream);
                    //    System.Drawing.Bitmap bmThumb = new System.Drawing.Bitmap(postedFile.InputStream);
                    //    System.Drawing.Image bmp2 = bmThumb.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                    //    string saveThumnailPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                    //    bmp2.Save(saveThumnailPath + @"\" + thumnailImageName);
                    //    byte[] c = System.IO.File.ReadAllBytes(@"" + saveThumnailPath + "/" + thumnailImageName);
                    //    ftpThumbnailImage.ContentLength = c.Length;
                    //    using (Stream s = ftpThumbnailImage.GetRequestStream())
                    //    {
                    //        s.Write(c, 0, c.Length);
                    //    }

                    //    try
                    //    {
                    //        ftpThumbnailImage.GetResponse();
                    //    }
                    //    catch { throw; }
                    //    finally
                    //    {

                    //    }

                    //}
                    FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(strFTPFilePath + newfilename);
                    ftpReq.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                    ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpReq.UseBinary = true;

                    //Save file temporarily on local path..
                    string savepath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                    postedFile.SaveAs(savepath + @"\" + newfilename);
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
                    finally
                    {
                        //Delete from local path.. 
                        System.IO.File.Delete(@"" + savepath + "/" + newfilename);
                    }
                }
                return strFTPFilePath.Replace(strFTPPath, ""); // return file path
            }
            catch { throw; }
        }
        private static string CreateNestedDirectoryOnFTP(string strFTPPath, string strUserName, string strPassWord, params string[] directories)
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
                }
                return strFTPFilePath;
            }
            catch { throw; }
        }
        private static bool isValidfeFTPConnection(string ftpUrl, string strUserName, string strPassWord)
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
        public VailidateAttachment ValidateDocumentFileTypefetools(HttpFileCollectionBase files)
        {
            VailidateAttachment obj = new VailidateAttachment();
            int maxallowedAttachmentSize = ApplicationSettings.MaxFileUploadSizeLimit * ApplicationSettings.MaxFileCountLimit * 1024;
            List<string> invalidAttachmentType = new List<string>();
            List<string> invalidAttachmentName = new List<string>();
            int totalUploadedAttachmentSize = 0;
            for (int i = 0; i < files.Count; i++)
            {
                // var allowedFileTypes = ApplicationSettings.allowedDocumentAttachmentType;
                var validDocumentTypes = ApplicationSettings.validDocumentTypesFetools.Split(new string[] { "," }, StringSplitOptions.None);
                var fileExtension = Path.GetExtension(files[i].FileName);//var fileExtension = files[i].FileName.Split('.').LastOrDefault()?.ToLower();
                totalUploadedAttachmentSize = totalUploadedAttachmentSize + files[i].ContentLength;
                if (!validDocumentTypes.Contains(fileExtension.ToLower()))//if (allowedFileTypes.IndexOf(fileExtension) == -1)
                {
                    invalidAttachmentType.Add(files[i].FileName);
                }
                if (files[i].FileName.Length > 100)
                {
                    invalidAttachmentName.Add(files[i].FileName);

                }
            }
            if (totalUploadedAttachmentSize > maxallowedAttachmentSize)
            {
                totalUploadedAttachmentSize = 0;
                obj.invalidattachmentsize = "Total file size is too large.Maximum total file size allowed is, " + maxallowedAttachmentSize / 1024 + " MB";
            }
            obj.invalidattachmentType = invalidAttachmentType.Count > 0 ? "The following files are not of allowed file type are " + string.Join(", ", invalidAttachmentType) : string.Empty;
            obj.invalidattachmentename = invalidAttachmentName.Count > 0 ? "File Name length should be less than 100 characters. invalid files are " + string.Join(", ", invalidAttachmentType) : string.Empty;
            return obj;
        }

        public FileResult DownloadfE_toolsFiles(string json, string entity_type = "")
        {
            var listPathName = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImageDownload>>(json);
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
                    foreach (var item in listPathName)
                    {
                        string fullPath = "", FileName = "", localPath = "";
                        if (item.location.ToLower() == "image")
                        {
                            var data = new BLFetools_Attachement().getFE_toolsAttachmentsbyid(item.systemId, "Image");
                            if (data != null)
                            {
                                fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                                FileName = data.file_location + "/" + data.file_name;
                                localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"])  + data.file_name + "";
                            }
                        }
                        else if (item.location.ToLower() == "document")
                        {
                            var data = new BLFetools_Attachement().getFE_toolsAttachmentsbyid(item.systemId, "Document");
                            if (data != null)
                            {
                                fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                                FileName = data.file_location + "/" + data.file_name;
                                localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + data.file_name + "";
                            }
                        }
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

                    }
                    #endregion
                    zipName = String.Format("{0}{1}{2}{3}.zip", entity_type, DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));
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
            finally
            {
                //string FileAddress = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "Attachments";
                string FileAddress = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                System.IO.DirectoryInfo di = new DirectoryInfo(FileAddress);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            return null;
        }

        [HttpGet]
        public void DownloadFE_ToolsDetail()
        {
            if (Session["viewfetools"] != null)
            {
                CommonGridAttributes objGridAttributes = (CommonGridAttributes)Session["viewfetools"];
                List<FE_Tools_Details> lstViewGroupDetails = new List<FE_Tools_Details>();
                objGridAttributes.currentPage = 0;
                objGridAttributes.pageSize = 0;
                var userid = Convert.ToInt32(Session["user_id"]);

                lstViewGroupDetails = new BL_Fe_Tools().GetFettoollist(objGridAttributes, userid);


                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<FE_Tools_Details>(lstViewGroupDetails);
                dtReport.TableName = "View_User_Tools_Details";
                dtReport.Columns.Add("Date", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Date"] = !String.IsNullOrEmpty(dtReport.Rows[i]["DATE_VALUE"].ToString()) ? MiscHelper.FormatDate(dtReport.Rows[i]["DATE_VALUE"].ToString()) : dtReport.Rows[i]["DATE_VALUE"];
                }
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("CREATED_BY_TEXT");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("MODIFIED_BY_TEXT");
                //dtReport.Columns.Remove("IMAGE_VALUE");
                //dtReport.Columns.Remove("DOCUMENT_VALUE");
                dtReport.Columns.Remove("DATE_VALUE");
                dtReport.Columns.Remove("ID");
               

                dtReport.Columns["USER_NAME"].ColumnName = "User Name";
                dtReport.Columns["TOOL_NAME"].ColumnName = "Tool Name";
                dtReport.Columns["BARCODE"].ColumnName = "Barcode";
                dtReport.Columns["SERIAL_NUMBER"].ColumnName = "Serial Number";
                dtReport.Columns["UPLOAD_TYPE"].ColumnName = "Upload Type";
                //dtReport.Columns["DATE_VALUE"].ColumnName = "Date";
                //dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                //dtReport.Columns["modified on"].ColumnName = "Modified On";
                var filename = "User_Tools";
                ExportFEToolsDataTableToExcel(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
            
        }
        [HttpPost]
        public JsonResult DeleteFettolsSpecification(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
               
                var output = new BL_Fe_Tools().DeleteFetoolsSpecificationById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "user Name detail is deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting User Name!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "user Name not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AcceptedUserTools(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BL_Fe_Tools().AcceptedUserTool(id);
                if (output>0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "User Tool is accepted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while accepting User Tool!";
                }
            }
            catch(Exception ex)
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Something went wrong while accepting User Tool!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult RejectedUserTools(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BL_Fe_Tools().RejectedUserTool(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "User Tool is rejected successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while rejecting User Tool!";
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Something went wrong while rejecting User Tool!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);

        }
        


        [HttpPost]
        public JsonResult Getuserdetails(string userid)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            try
            {
                var date_value = new BL_Fe_Tools().GetUserDetailsbyid(Convert.ToInt32(userid));

                
                //if (!string.IsNullOrEmpty(date_value))
               // {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.result = date_value.ToString();

                //}

            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.result = "";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public void ExportFEToolsDataTableToExcel(DataTable dtReport, string fileName)
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

    }
}