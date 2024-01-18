using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using Ionic.Zip;
using NLog;
using Npgsql;
using System.Security.Cryptography;
using System.Text;

namespace DownloadBackUpFile
{
    /// <summary>
    /// Author: Ajanabi
    /// </summary>
    public class Program
    {
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Error("Backup Process Started..");
            logger.Log(LogLevel.Info, "Backup Process Args Value.. " + args);

            string query;
            string filetype;
            string path;

            if (args.Length > 0)
            {

                filetype = args[0] == "DBDownload" ? "DB File" : (args[0] == "AppDownloadwithoutatt" ? "App File without attachment" : "App File with attachment");
                try
                {
                    query = "insert into DownLoadStatus(filetype,status,download_strt,Download_by)values ('" + filetype + "','Started',LOCALTIMESTAMP,'" + args[1] + "')";
                    executeCMD(query);
                    if (args[0] != "DBDownload")
                    {
                        string AppFile = ConfigurationManager.AppSettings["AppFile"].ToString();
                        string excludefolder = ConfigurationManager.AppSettings["excludefolder"].ToString();
                        string loc_Path = ConfigurationManager.AppSettings["Temp_AppBackuppath"].ToString();
                        path = AppFile;
                        using (ZipFile zip = new ZipFile())
                        {
                            var file = zip.AddDirectory(path, "Files");
                            zip.UseZip64WhenSaving = Zip64Option.Always;
                            zip.Save(loc_Path);
                        }
                        if (args[0] == "AppDownloadwithoutatt")
                        {
                            using (ZipFile zip = ZipFile.Read(loc_Path))
                            {
                                zip.RemoveSelectedEntries(excludefolder);
                                zip.Save();
                            }

                        }
                    }
                    else
                    {
                        Downloadfile();
                    }

                    query = "update DownLoadStatus set status='Completed',download_end=LOCALTIMESTAMP where download_strt > current_date and filetype ='" + filetype + "'";
                    executeCMD(query);
                }
                catch (Exception ex)
                {
                    query = "update DownLoadStatus set status='Error',download_end=LOCALTIMESTAMP where download_strt > current_date and filetype ='" + filetype + "'";
                    executeCMD(query);
                    logger.Log(LogLevel.Info, "Exception Occured " + ex.StackTrace.ToString());
                    throw ex;
                }
            }


        }

        public static void Downloadfile()
        {
            logger.Log(LogLevel.Info, "Download Started ");
            List<Entity> fileList = new List<Entity>();
            string LocalFile = Convert.ToString(ConfigurationManager.AppSettings["Temp_DBBackuppath"]);
            logger.Log(LogLevel.Info, "Download Started " + LocalFile);
            string FtpDBFile = Convert.ToString(ConfigurationManager.AppSettings["FTP_DBPath"]);
            logger.Log(LogLevel.Info, "Download Started " + FtpDBFile);
            string userid = Convert.ToString(ConfigurationManager.AppSettings["User_ID"]);
            logger.Log(LogLevel.Info, "Download Started " + userid);
            string pwd = Convert.ToString(ConfigurationManager.AppSettings["Password"]);
            logger.Log(LogLevel.Info, "Download Started " + pwd);
            try
            {
                WebRequest requestfile = (FtpWebRequest)WebRequest.Create(FtpDBFile);
                requestfile.Credentials = ((string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(pwd)) ? new NetworkCredential() : new NetworkCredential(userid, pwd));
                requestfile.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                requestfile.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                List<string> Files = new List<string>();
                using (FtpWebResponse response = (FtpWebResponse)requestfile.GetResponse())
                {
                    StreamReader streamReader = new StreamReader(response.GetResponseStream());
                    string line = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        DateTime dDate;
                        int pos = line.LastIndexOf(" ") + 1;
                        line = line.Substring(pos, line.Length - pos);
                        //var fileDateSplits = line.Split('_');
                        logger.Log(LogLevel.Info, "fileDate Split " + line);
                        var fileDate = line.Length > 10 ? line.Substring(0, 10).Replace("_", "-") : line;
                        logger.Log(LogLevel.Info, "fileDate:" + fileDate);
                        if (DateTime.TryParse(fileDate, out dDate) && line.Length > 10)
                        {

                            string uri = Path.Combine(FtpDBFile, line);
                            logger.Log(LogLevel.Info, "uri:" + uri);
                            // FtpWebRequest requestlstmodfile = (FtpWebRequest)WebRequest.Create(uri);
                            // requestlstmodfile.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                            // FtpWebResponse responselstmodfile = (FtpWebResponse)requestlstmodfile.GetResponse();

                            fileList.Add(new Entity()
                            {
                                fileName = uri,
                                uploadDate = dDate,
                                MaxNo = Convert.ToInt32(uri.Split('_').LastOrDefault())
                            });
                        }


                        line = streamReader.ReadLine();

                    }
                    streamReader.Close();
                }
                if (fileList.Count > 0)
                {
                    var result = fileList.OrderByDescending(x => x.uploadDate).ThenByDescending(x => x.MaxNo).FirstOrDefault();
                    logger.Log(LogLevel.Info, "result: " + result.fileName + ConfigurationManager.AppSettings["InnerDbFolder"]);
                    using (WebClient ftpClient = new WebClient())
                    {
                        ftpClient.Credentials = new NetworkCredential(userid, pwd);
                        ftpClient.DownloadFile(result.fileName + ConfigurationManager.AppSettings["InnerDbFolder"], LocalFile);
                    }
                }
            }
            catch (WebException e)
            {
                String status = ((FtpWebResponse)e.Response).StatusDescription;
                logger.Log(LogLevel.Info, "Download Ex Exception Status " + status + " " + e);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Info, "Download Ex Exception Occured " + ex);
                throw ex;
            }

        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public static void executeCMD(String Query)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = Convert.ToBoolean(ConfigurationManager.AppSettings["ISEncryptedConnection"]) == true ? Decrypt(ConfigurationManager.AppSettings["constr"].Trim()) : ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    connection.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = Query;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
            }
            catch (SqlException ex)
            {
                logger.Log(LogLevel.Info, "Execute cmd" + ex);
                throw ex;
            }
            finally
            {
            }
        }
    }
}
