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
using Utility;

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
        public static bool DeleteFileFromFTP(string filePath)
        {
            string strFTPPath = System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"];
            string strFTPUserName = System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"];
            string strFTPPassWord = System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"];
            ErrorLogHelper errorLog = new ErrorLogHelper();

            try
            {
                errorLog.ApiLogWriter("DeleteAttachment6", "", $"FTP Path={filePath}", null);
                // Ensure the FTP path ends without a trailing slash
                // Normalize base FTP path
                strFTPPath = strFTPPath.TrimEnd('/');

                // Normalize file path
                filePath = filePath.TrimStart('/');

                // Build the full FTP file URI
                string ftpFilePath = $"{strFTPPath}/{filePath}";
                errorLog.ApiLogWriter("final URL", "", $"FTP Path={ftpFilePath}", null);

                // Create the request
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(strFTPUserName, strFTPPassWord);

                // Execute and close response
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    //Console.WriteLine($"Delete status: {response.StatusDescription}");
                }

                return true; // success
            }
            catch (WebException ex) { throw new Exception("Unable to Delete File to FTP Server", ex); }
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

        public static string UploadfileOnFTP(string sEntityType, string sEntityId, HttpPostedFile postedFile, string sUploadType, string newfilename, string featureType = null, string uploaddocType = null)
        {
            ErrorLogHelper errorLog = new ErrorLogHelper();
            string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
            string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
            string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];
            string strFTPFilePath = "";

            try
            {
                errorLog.ApiLogWriter("FTP Config", "",
                    $"Path={strFTPPath}, User={strFTPUserName}, UploadType={sUploadType}", null);

                if (!isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                    throw new Exception("Invalid FTP connection. Please check credentials or server path.");

                // Create directory structure on FTP (safe version)
                strFTPFilePath = CreateNestedDirectoryOnFTP(
                    strFTPPath, strFTPUserName, strFTPPassWord,
                    featureType, sEntityType, sEntityId, sUploadType, uploaddocType);

                errorLog.ApiLogWriter("Directory Created", "", $"FTP Path={strFTPFilePath}", null);

                string saveLocalPath = HttpContext.Current.Server.MapPath(
                    ConfigurationManager.AppSettings["AttachmentLocalPath"]);

                // ------------------ THUMBNAIL UPLOAD ------------------
                if (sUploadType.ToUpper() == "IMAGE")
                {
                    string thumnailImageName = "Thumb_" + newfilename;
                    string ftpThumbUrl = $"{strFTPFilePath.TrimEnd('/')}/{Uri.EscapeDataString(thumnailImageName)}";

                    errorLog.ApiLogWriter("Uploading Thumbnail", "", ftpThumbUrl, null);

                    // Create thumbnail
                    using (var bmThumb = new System.Drawing.Bitmap(postedFile.InputStream))
                    using (var bmp2 = bmThumb.GetThumbnailImage(100, 100, null, IntPtr.Zero))
                    {
                        string localThumbPath = Path.Combine(saveLocalPath, thumnailImageName);
                        bmp2.Save(localThumbPath);
                        byte[] thumbBytes = File.ReadAllBytes(localThumbPath);

                        // FTP upload request for thumbnail
                        var ftpThumbReq = (FtpWebRequest)WebRequest.Create(ftpThumbUrl);
                        ftpThumbReq.Credentials = new NetworkCredential(strFTPUserName, strFTPPassWord);
                        ftpThumbReq.Method = WebRequestMethods.Ftp.UploadFile;
                        ftpThumbReq.UseBinary = true;
                        ftpThumbReq.UsePassive = true;
                        ftpThumbReq.ContentLength = thumbBytes.Length;

                        try
                        {
                            using (Stream s = ftpThumbReq.GetRequestStream())
                            {
                                s.Write(thumbBytes, 0, thumbBytes.Length);
                            }
                            using (var resp = (FtpWebResponse)ftpThumbReq.GetResponse())
                            {
                                errorLog.ApiLogWriter("Thumbnail Uploaded", "", $"Status={resp.StatusDescription}", null);
                            }
                        }
                        catch (WebException wex)
                        {
                            errorLog.ApiLogWriter("Thumbnail Upload Failed", "", wex.ToString(), null);
                            throw;
                        }
                        finally
                        {
                            File.Delete(localThumbPath);
                        }
                    }
                }

                // ------------------ MAIN FILE UPLOAD ------------------
                string ftpFileUrl = $"{strFTPFilePath.TrimEnd('/')}/{Uri.EscapeDataString(newfilename)}";
                errorLog.ApiLogWriter("Uploading Main File", "", ftpFileUrl, null);

                string localFilePath = Path.Combine(saveLocalPath, newfilename);
                postedFile.SaveAs(localFilePath);
                byte[] fileBytes = File.ReadAllBytes(localFilePath);

                var ftpReq = (FtpWebRequest)WebRequest.Create(ftpFileUrl);
                ftpReq.Credentials = new NetworkCredential(strFTPUserName, strFTPPassWord);
                ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                ftpReq.UseBinary = true;
                ftpReq.UsePassive = true;
                ftpReq.ContentLength = fileBytes.Length;

                try
                {
                    using (Stream s = ftpReq.GetRequestStream())
                    {
                        s.Write(fileBytes, 0, fileBytes.Length);
                    }
                    using (var resp = (FtpWebResponse)ftpReq.GetResponse())
                    {
                        errorLog.ApiLogWriter("Main File Uploaded", "", $"Status={resp.StatusDescription}", null);
                    }
                }
                catch (WebException wex)
                {
                    errorLog.ApiLogWriter("Main File Upload Failed", "", wex.ToString(), null);
                    throw;
                }
                finally
                {
                    File.Delete(localFilePath);
                }
                strFTPFilePath = strFTPFilePath + "/";
                return strFTPFilePath.Replace(strFTPPath, "");
            }
            catch (Exception ex)
            {
                errorLog.ApiLogWriter("UploadfileOnFTP Error", "", ex.ToString(), null);
                throw;
            }
        }

        // ------------------ SAFE DIRECTORY CREATION ------------------
        public static string CreateNestedDirectoryOnFTP(string baseFtpPath, string ftpUserName, string ftpPassword, string featureType, string sEntityType, string sEntityId, string sUploadType, string uploaddocType)
        {
            List<string> pathParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(sEntityType)) pathParts.Add(sEntityType.Trim());
            if (!string.IsNullOrWhiteSpace(sEntityId)) pathParts.Add(sEntityId.Trim());
            if (!string.IsNullOrWhiteSpace(sUploadType)) pathParts.Add(sUploadType.Trim());
            if (!string.IsNullOrWhiteSpace(featureType)) pathParts.Add(featureType.Trim());
            //if (uploaddocType != null) { pathParts.Add(uploaddocType.Trim()); }

            string currentPath = baseFtpPath.TrimEnd('/');

            foreach (string part in pathParts)
            {
                string encodedPart = Uri.EscapeDataString(part);
                currentPath += "/" + encodedPart;

                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(currentPath);
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.KeepAlive = false;

                    using (var resp = (FtpWebResponse)request.GetResponse())
                    {
                        // Directory created successfully
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response is FtpWebResponse ftpResp &&
                        ftpResp.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        // Directory already exists — ignore
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return currentPath;
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