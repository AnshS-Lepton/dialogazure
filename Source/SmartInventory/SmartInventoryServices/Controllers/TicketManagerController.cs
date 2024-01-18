using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using BusinessLogics;
using BusinessLogics.WFM;
using Models;
using Models.API;
using Org.BouncyCastle.Asn1.Ocsp;
using SmartInventory.Settings;
using SmartInventoryServices.Helper;
using Utility;
using User = Models.User;

namespace SmartInventoryServices.Controllers
{
    public class TicketManagerController : Controller
    {
        public ActionResult AddHPSMTicket(string JsonData)
        
        {
            BLUser objBLUser = new BLUser();
            try
            {
                string JonString = MiscHelper.DecodeTo64(JsonData).ToString();
                string jsonFormattedString = JonString.Replace("\\\"", "\"");
                TicketMaster objTicketMaster = new TicketMaster();
                User user = new JavaScriptSerializer().Deserialize<User>(jsonFormattedString);
                string Password = MiscHelper.EncodeTo64(user.password);

                string ADFSEndPoint = string.Empty;
                
                ADFSEndPoint = System.Configuration.ConfigurationManager.AppSettings["ADFSEndPoint"].ToString().Trim();

                //user = objBLUser.ValidateUser(user.user_name, (String.IsNullOrEmpty(ADFSEndPoint) ? Password : ""), UserType.Mobile.ToString());
                if (ApplicationSettings.isADOIDEnabled || !String.IsNullOrEmpty(ADFSEndPoint))
                {

                    user = objBLUser.ValidateUser(user.user_name, "", UserType.Mobile.ToString());
                }
                else
                {
                    user = objBLUser.ValidateUser(user.user_name, Password, UserType.Mobile.ToString());
                }
                if (user != null)
                {
                    if (user.is_active)
                    {
                        objTicketMaster.user_id = user.user_id;
                        objTicketMaster.contact_no = user.mobile_number.ToString();

                        objTicketMaster.customer_name = user.user_name;
                        objTicketMaster.ticket_reference = "JFP";
                        //objTicketMaster.lstTicketTypeMaster = new BLNetworkTicket().GetTicketTypeByModule("HPSM",objTicketMaster.user_id);
                        objTicketMaster.lstTicketTypeMaster = new BLNetworkTicket().GetHPSMTicketType("HPSM");
                        return PartialView("_HPSMTicket", objTicketMaster);
                    }
                    else
                    {
                        return View("_Unauthorize");
                    }
                }
                else
                {
                    return View("_Unauthorize");
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, status = "FAILED" }, JsonRequestBehavior.AllowGet);
                throw;
            }
        }

        public ActionResult SaveHPSMTicket(TicketMaster objHPSMTicket)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            List<HttpPostedFileBase> totalFiles = new List<HttpPostedFileBase>();
			ErrorLogHelper logHelper = new ErrorLogHelper();
			string extension = string.Empty, fileName;
            HttpPostedFileBase file;
			string mailsentmsg;
			int user_id = 0;
			string CatagoryName = "";
			string UploadStatus = "";
            objMsg.message = "something went wrong!";
			var request = Newtonsoft.Json.JsonConvert.SerializeObject(objHPSMTicket);
			try
			{			
		    int TicketAttachmentMaxSize = ApplicationSettings.TicketAttachmentMaxSize;
             new BLMisc().InitializeEmailSettings();
            //EmailSettings.InitializeEmailSettings();
             for (int i = 0; i < Request.Files.Count; i++)
            {
                file = Request.Files[i];
                if ((file.ContentLength / (1024 * 1024)) > TicketAttachmentMaxSize)
                {
						logHelper.ApiLogWriter("SaveHPSMTicket()", "TicketManager Controller", "Request:- "+ Convert.ToString(request) + ", Error:-Please select a file less than " + TicketAttachmentMaxSize + " MB", null);
						return Json(new { Message = "Please select a file less than " + TicketAttachmentMaxSize + " MB", msg = ResponseStatus.ERROR.ToString() }, JsonRequestBehavior.AllowGet);
                }
                fileName = file.FileName;
                    extension = System.IO.Path.GetExtension(fileName).Split('.')[1].ToLower();

                string[] RestrictedTicketAttachments = ApplicationSettings.RestrictedTicketAttachments.Split(',').Select(ext => ext.Trim()).ToArray();
                if (!RestrictedTicketAttachments.Contains(extension))
                {
						logHelper.ApiLogWriter("SaveHPSMTicket()", "TicketManager Controller", "Request:- " + Convert.ToString(request) + ", Error:-" + ApplicationSettings.RestrictedTicketAttachments + " not acceptable!", null);
                        //return Json(new { Message = ApplicationSettings.RestrictedTicketAttachments + " not acceptable!", msg = ResponseStatus.ERROR.ToString() }, JsonRequestBehavior.AllowGet);
                        return Json(new { Message = "Only " + ApplicationSettings.RestrictedTicketAttachments + " are allowed as attachment", msg = ResponseStatus.ERROR.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
           
                objHPSMTicket.created_by = Convert.ToInt32(Request.Form["user_id"]);
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
                var status = new BLHPSMTicket().SaveHPSMTicket(objHPSMTicket);
                if(status !=null)
                {
					objMsg.status = ResponseStatus.OK.ToString();
					objMsg.message = "Ticket Detail Saved successfully.";
					objHPSMTicket.pageMsg = objMsg;
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
						objMsg.message = objMsg.message + " But failed to upload HPSM Ticket!";
						objHPSMTicket.pageMsg = objMsg;
						logHelper.ApiLogWriter("SaveHPSMTicket()", "TicketManager Controller", "Request:- " + Convert.ToString(request) + ", Error:-" + UploadStatus, null);
						return Json(new { Message = objMsg.message, msg = objMsg.status }, JsonRequestBehavior.AllowGet);
					}
				}
                else
                {
					objMsg.status = ResponseStatus.ERROR.ToString();
					objMsg.message = "Error in saved ticket details!";
					objHPSMTicket.pageMsg = objMsg;
				}
                CatagoryName = Request.Form["TicketType"];
    //            objHPSMTicket.lstTicketTypeMaster = new BLNetworkTicket().GetTicketTypeByModule("HPSM", (objHPSMTicket.user_id));
    //            if (objHPSMTicket.lstTicketTypeMaster.Count > 0)
				//{
    //                CatagoryName = objHPSMTicket.lstTicketTypeMaster.SingleOrDefault(cat => cat.id == objHPSMTicket.ticket_type_id).ticket_type;

    //            }
                //Name//Mobile//Category//Business name
                //string subject = objHPSMTicket.customer_name + "/" + objHPSMTicket.contact_no + "/" + CatagoryName + "/" + objHPSMTicket.ticket_reference;
                string subject = "BIM" + ":" + "JFP" + ":" + CatagoryName;
                string[] aarReceiver = ApplicationSettings.TicketReceiverMailId.Split(',');

                User user = new BLUser().GetUserDetailByID(objHPSMTicket.user_id);
                string user_email = string.Empty;

                if (!string.IsNullOrEmpty(user.user_email) && MiscHelper.Decrypt(user.user_email).ToLower().Contains("@ril.com") && user.user_type.ToLower()=="own")
                {
                    user_email = MiscHelper.Decrypt(user.user_email);
                   // LogHelper.GetInstance.WriteDebugLog("Sender Email:" + user_email);
                }
               
                try
                {
                    commonUtil.SendEmail(aarReceiver, subject, objHPSMTicket.ticket_description, totalFiles, out mailsentmsg, BLMisc.EmailSettingsModel, user_email);
                }
                catch(Exception ex)
                {
					logHelper.ApiLogWriter("SaveHPSMTicket()", "TicketManager Controller", Convert.ToString(request), ex);
                    objMsg.message = objMsg.message + " But error in sending mail!";
					objMsg.status = StatusCodes.UNKNOWN_ERROR.ToString();
				}
            }
            catch (Exception ex) {              
                logHelper.ApiLogWriter("SaveHPSMTicket()", "TicketManager Controller", Convert.ToString(request), ex);
                objMsg.status = StatusCodes.UNKNOWN_ERROR.ToString();
                 //objMsg.message = msg + ex.Message.ToString();
                objMsg.message = objMsg.message;
			}
            return Json(new { Message = objMsg.message, Status = objMsg.status }, JsonRequestBehavior.AllowGet);
        }       
        [System.Web.Mvc.HttpPost]
        public string UploadTicketFile(TicketMaster objHPSMTicket)
        {
            List<TicketAttachments> lstTicketAttachments = new List<TicketAttachments>();
            string fileName;
            List<HttpPostedFileBase> PostedFiles = new List<HttpPostedFileBase>();
            int TicketAttachmentMaxSize = ApplicationSettings.TicketAttachmentMaxSize;
            try
            {
                int user_id = objHPSMTicket.user_id;


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