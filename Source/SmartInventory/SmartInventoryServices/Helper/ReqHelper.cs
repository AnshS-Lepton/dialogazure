using BusinessLogics;
using Models.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace SmartInventoryServices.Helper
{
    public class ReqHelper
    {
        public static T GetRequestData<T>(ReqInput userData)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(userData.data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static T GetRequestData<T>(Models.WFM.ReqInput userData)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(userData.data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static HeaderAttributes getHeaderValue(List<KeyValuePair<string, IEnumerable<string>>> obj)
        {
            var headerData = new HeaderAttributes();
            foreach (var item in obj)
            {
                if (!string.IsNullOrEmpty(item.Key) && headerData.GetType().GetTypeInfo().GetDeclaredProperty(item.Key.ToLower()) != null)
                {
                    if (item.Key.ToLower() == "user_id")
                        headerData.GetType().GetProperty(item.Key.ToLower()).SetValue(headerData, !string.IsNullOrEmpty(item.Value.FirstOrDefault()) ? Convert.ToInt32((item.Value.FirstOrDefault())) : 0);
                    else if (item.Key.ToLower() == "structure_id")
                        headerData.GetType().GetProperty(item.Key.ToLower()).SetValue(headerData, !string.IsNullOrEmpty(item.Value.FirstOrDefault()) ? Convert.ToInt32((item.Value.FirstOrDefault())) : 0);
                    else if(item.Key.ToLower() == "is_new_entity")
                        headerData.GetType().GetProperty(item.Key.ToLower()).SetValue(headerData, !string.IsNullOrEmpty(item.Value.FirstOrDefault()) ? Convert.ToBoolean((item.Value.FirstOrDefault())) : false);
                    else
                        headerData.GetType().GetProperty(item.Key.ToLower()).SetValue(headerData, (item.Value.FirstOrDefault()));
                }
            }
           
            return headerData;
        }

        public static string MergeHeaderAttributeInJsonObject(ReqInput data, HeaderAttributes headerAttribute)
        {
            UserRequestIn objRequestIn = ReqHelper.GetRequestData<UserRequestIn>(data);
            if (objRequestIn.user_id == 0 && objRequestIn.userId == 0)
                headerAttribute.user_id = new BLUser().getUserDetailsByUserName("iuser").user_id;
            else if (objRequestIn.user_id > 0)
                headerAttribute.user_id = objRequestIn.user_id;
            else if (objRequestIn.userId > 0)
                headerAttribute.user_id = objRequestIn.userId;

            headerAttribute.source_ref_type = String.IsNullOrEmpty(headerAttribute.source_ref_type) ? headerAttribute.source : headerAttribute.source_ref_type;

            var json = JsonConvert.SerializeObject(headerAttribute);
            JObject o1 = JObject.Parse(data.data);
            JObject o2 = JObject.Parse(json);
            o1.Merge(o2, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return JsonConvert.SerializeObject(o1);
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
        public static void DeleteFileFromFTP(string filePath)
        {
            try
            {
                string strFTPPath = System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {

                    System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(strFTPPath + @"\" + filePath);

                    //If you need to use network credentials
                    request.Credentials = new System.Net.NetworkCredential(strFTPUserName, strFTPPassWord);
                    //additionally, if you want to use the current user's network credentials, just use:
                    //System.Net.CredentialCache.DefaultNetworkCredentials
                    request.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                    System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();
                    response.Close();
                }
            }
            catch { throw; }
        }
        public static bool isValidFTPConnection(string ftpUrl, string strUserName, string strPassWord)
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
        public static string CreateNestedDirectoryOnFTP(string strFTPPath, string strUserName, string strPassWord, params string[] directories)
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

        public static string UploadfileOnFTP(string sEntityType, string sEntityId, HttpPostedFile postedFile, string sUploadType, string newfilename, string featureType = null)
        {
            try
            {
                string strFTPFilePath = "";
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {
                    // Create Directory if not exists and get Final FTP path to save file..
                    strFTPFilePath = CreateNestedDirectoryOnFTP(strFTPPath, strFTPUserName, strFTPPassWord, featureType, sEntityType, sEntityId, sUploadType);

                    //Prepare FTP Request..
                    if (sUploadType.ToUpper() == "IMAGE")
                    {
                        string thumnailImageName = "Thumb_" + newfilename;
                        FtpWebRequest ftpThumbnailImage = (FtpWebRequest)WebRequest.Create(strFTPFilePath + thumnailImageName);
                        ftpThumbnailImage.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                        ftpThumbnailImage.Method = WebRequestMethods.Ftp.UploadFile;
                        ftpThumbnailImage.UseBinary = true;
                        // var image = System.Drawing.Image.FromStream(postedFile.InputStream);
                        System.Drawing.Bitmap bmThumb = new System.Drawing.Bitmap(postedFile.InputStream);
                        System.Drawing.Image bmp2 = bmThumb.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                        string saveThumnailPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                        bmp2.Save(saveThumnailPath + @"\" + thumnailImageName);
                        byte[] c = System.IO.File.ReadAllBytes(@"" + saveThumnailPath + "/" + thumnailImageName);
                        ftpThumbnailImage.ContentLength = c.Length;
                        using (Stream s = ftpThumbnailImage.GetRequestStream())
                        {
                            s.Write(c, 0, c.Length);
                        }

                        try
                        {
                            ftpThumbnailImage.GetResponse();
                        }
                        catch { throw; }
                        finally
                        {
                            //Delete from local path.. 
                            // System.IO.File.Delete(@"" + saveThumnailPath + "/" + newfilename);
                        }
                        //Image image = Image.FromFile(fileName);
                        //Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                        //thumb.Save(Path.ChangeExtension(fileName, "thumb"));
                    }
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

        #region GeoTaggedImages BY ANTRA
        public static string UploadGeoTaggingfileOnFTP(string sUploadType,string UserId, string newfilename, HttpPostedFile postedFile)
        {
            try
            {
                string strFTPFilePath = "";
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {
                    // Create Directory if not exists and get Final FTP path to save file..
                    strFTPFilePath = CreateNestedDirectoryOnFTP(strFTPPath, strFTPUserName, strFTPPassWord, "GeoTaggedImageGallery",UserId, sUploadType);

                    //Prepare FTP Request..
                    if (sUploadType.ToUpper() == "IMAGE")
                    {
                        string thumnailImageName = "Thumb_" + newfilename;
                        FtpWebRequest ftpThumbnailImage = (FtpWebRequest)WebRequest.Create(strFTPFilePath + thumnailImageName);
                        ftpThumbnailImage.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                        ftpThumbnailImage.Method = WebRequestMethods.Ftp.UploadFile;
                        ftpThumbnailImage.UseBinary = true;
                        System.Drawing.Bitmap bmThumb = new System.Drawing.Bitmap(postedFile.InputStream);
                        System.Drawing.Image bmp2 = bmThumb.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                        string saveThumnailPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                        bmp2.Save(saveThumnailPath + @"\" + thumnailImageName);
                        byte[] c = System.IO.File.ReadAllBytes(@"" + saveThumnailPath + "/" + thumnailImageName);
                        ftpThumbnailImage.ContentLength = c.Length;
                        using (Stream s = ftpThumbnailImage.GetRequestStream())
                        {
                            s.Write(c, 0, c.Length);
                        }

                        try
                        {
                            ftpThumbnailImage.GetResponse();
                        }
                        catch { throw; }
                        finally
                        {
                            //Delete from local path.. 
                            // System.IO.File.Delete(@"" + saveThumnailPath + "/" + newfilename);
                        }
                    }
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
        #endregion

    }
}