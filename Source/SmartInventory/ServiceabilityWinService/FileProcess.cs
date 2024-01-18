using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using CsvHelper;
using Models;
using NPOI.SS.UserModel;
using ServiceabilityWinService.Utility;

namespace ServiceabilityWinService
{
    public static class FileProcess
    {
        public static DataTable dt = new DataTable("sheet1");
        public static DataColumn c = new DataColumn("Request_Id");
        public static DataColumn d = new DataColumn("NAP_ID");
        public static DataColumn e = new DataColumn("NAP_Distance_mtr");

        public static void BulkServiceability()
        {
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
                string InputFolder = Convert.ToString(ConfigurationManager.AppSettings["InputFolder"]);
                string OutputFolder = Convert.ToString(ConfigurationManager.AppSettings["OutputFolder"]);
                string ProcessedFolder = Convert.ToString(ConfigurationManager.AppSettings["ProcessedFolder"]);
                string fileUrl = string.Empty;
                string fileUrlOut = string.Empty;
                string fileUrlProcessed = string.Empty;
                string savepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceabilityOutput.xls");
                fileUrl = string.Concat(FtpUrl, InputFolder);

                string username = ConfigurationManager.AppSettings["Username"];
                string password = ConfigurationManager.AppSettings["Password"];
                string source = ConfigurationManager.AppSettings["source"];

                var list = new List<string>();

                Uri serverFile = new Uri(fileUrl);
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(serverFile);

                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Credentials = new NetworkCredential(UserName, PassWord);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                using (var res = (FtpWebResponse)reqFTP.GetResponse())
                {
                    using (var stream = res.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, true))
                        {
                            while (!reader.EndOfStream)
                            {
                                list.Add(reader.ReadLine());
                            }
                        }
                    }
                }
                if (list.Count > 0)
                {
                    var token = IntegrationWebRequest.GetAPIToken(username, password, source);
                    foreach (var item in list.Where(x => x.EndsWith(".xlsx") || x.EndsWith(".xls") || x.EndsWith(".csv")))
                    {
                        dt = new DataTable("sheet1");
                        c = new DataColumn("Request_Id");
                        d = new DataColumn("NAP_ID");
                        e = new DataColumn("NAP_Distance_mtr");
                        dt.Columns.Add(c);
                        dt.Columns.Add(d);
                        dt.Columns.Add(e);


                        WebClient request = new WebClient();
                        if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                            request.Credentials = new NetworkCredential(UserName, PassWord);
                        byte[] newFileData = request.DownloadData(fileUrl + item.ToString());





                        using (MemoryStream ms = new MemoryStream(newFileData))
                        {
                            if (item.EndsWith(".csv"))
                            {
                                StreamReader reader = new StreamReader(ms, System.Text.Encoding.UTF8, true);
                                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                                {
                                    using (var dr = new CsvDataReader(csv))
                                    {
                                        var dt_csv = new DataTable();
                                        dt_csv.Load(dr);
                                        foreach (DataRow dr_csv in dt_csv.Rows)
                                        {
                                            var requestId = dr_csv["Request_Id"].ToString();
                                            var latitude = dr_csv["Latitude"];
                                            var longitude = dr_csv["Longitude"];
                                            var customer_add = dr_csv["Customer Address"].ToString();
                                            var segment = dr_csv["Segment"].ToString();
                                            addDataRow(requestId, Convert.ToDouble(latitude), Convert.ToDouble(longitude), segment, token.access_token);
                                        }
                                    }
                                }
                                string[] s = item.ToString().Split('.');
                                fileUrlProcessed = string.Concat(ProcessedFolder, s[0] + "_Processed_" + commonUtil.getTimeStamp() + ".csv");
                                fileUrlOut = string.Concat(FtpUrl, OutputFolder, s[0] + "_Output_" + commonUtil.getTimeStamp() + ".xls");
                            }
                            else
                            {
                                IWorkbook wb = NPOI.SS.UserModel.WorkbookFactory.Create(ms);
                                NPOI.SS.UserModel.ISheet sheet = wb.GetSheetAt(0);
                                for (int row = 1; row <= sheet.LastRowNum; row++)
                                {
                                    var sheetRow = sheet.GetRow(row);
                                    if (sheetRow != null)
                                    {
                                        var requestId = sheetRow.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue;
                                        var latitude = sheetRow.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).NumericCellValue;
                                        var longitude = sheetRow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).NumericCellValue;
                                        var customer_add = sheetRow.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue;
                                        var segment = sheetRow.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue;
                                        addDataRow(requestId, latitude, longitude, segment, token.access_token);
                                    }
                                }
                                string[] s = item.ToString().Split('.');
                                fileUrlProcessed = string.Concat(ProcessedFolder, s[0] + "_Processed_" + commonUtil.getTimeStamp() + ".xls");
                                fileUrlOut = string.Concat(FtpUrl, OutputFolder, s[0] + "_Output_" + commonUtil.getTimeStamp() + ".xls");
                            }
                        }
                        Uri ser = new Uri(fileUrl + item.ToString());
                        FtpWebRequest requestFTP = (FtpWebRequest)FtpWebRequest.Create(ser);
                        requestFTP.Method = WebRequestMethods.Ftp.Rename;
                        requestFTP.RenameTo = fileUrlProcessed;
                        FtpWebResponse re = (FtpWebResponse)requestFTP.GetResponse();



                        NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, savepath);                        
                        FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(fileUrlOut);
                        ftpReq.Credentials = new NetworkCredential(UserName, PassWord);
                        ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                        ftpReq.UseBinary = true;

                        byte[] b = System.IO.File.ReadAllBytes(@"" + savepath);
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
                            System.IO.File.Delete(@"" + savepath);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                WriteToFile("FTP Exception: " + ex.Message + " {0}");
            }
        }
        private static void addDataRow(string requestId, double latitude, double longitude, string segment, string access_token)
        {
            string url = ConfigurationManager.AppSettings["ApiUrl"];
            var gpons = IntegrationWebRequest.PostIntegrationAPIRequest<ServiceabilityRoot>(url, access_token, requestId, latitude.ToString(), longitude.ToString(), segment);
            foreach (var item in gpons.results.devicelist.GPON)
            {
                DataRow r = dt.NewRow();
                r["Request_Id"] = requestId;
                r["NAP_ID"] = item.nap_id;
                r["NAP_Distance_mtr"] = item.distance;
                dt.Rows.Add(r);
            }
        }
        private static void WriteToFile(string text)
        {
            string path = "C:\\BulkServiceabilityLog.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }


    }

}
