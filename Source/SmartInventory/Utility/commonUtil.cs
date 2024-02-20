
//using BusinessLogics;
using Microsoft.SqlServer.Server;
using Models;
using Models.WFM;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Xml;


namespace Utility
{
    public static class commonUtil
    {
        public static string ToCamelCase(this string str)
        {
            StringBuilder sbOutput = new StringBuilder();
            string[] strArray = str.Split('_');
            foreach (string word in strArray)
            {
                if (!string.IsNullOrEmpty(word) && word.Length > 1)
                {
                    sbOutput.Append("_" + Char.ToUpperInvariant(word[0]) + word.Substring(1));
                }
            }
            return sbOutput.ToString().Substring(1);
        }
        public static List<List<T>> ToChunks<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
        public static string RemoveSpecialCaseAndConvertToUpper(this string str, string character)
        {

            var _str = str.ToString().Replace(character, string.Empty).ToUpper();
            return _str;
        }
        public static string RemoveSpecialCaseAndConvertToLower(this string str, string character)
        {

            var _str = str.ToString().Replace(character, string.Empty).ToLower();
            return _str;
        }
        public static string RemoveSpecialCase(this string str, string character)
        {

            var _str = str.ToString().Replace(character, string.Empty);
            return _str;
        }

        public static List<Dictionary<string, object>> GetTableRows(DataTable dtData)
        {
            List<Dictionary<string, object>>
            lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dictRow = null;

            foreach (DataRow dr in dtData.Rows)
            {
                dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dtData.Columns)
                {
                    dictRow.Add(col.ColumnName, dr[col]);
                }
                lstRows.Add(dictRow);
            }
            return lstRows;
        }
        public static string Reverse(this string str)
        {
            if (str.Length <= 1) return str;
            else return Reverse(str.Substring(1)) + str[0];
        }
        public static double ToDouble(string value)
        {
            double outval = 0;
            Double.TryParse(value, out outval);
            return outval;

        }
        public static int ToInt(string value)
        {
            int outval = 0;
            int.TryParse(value, out outval);
            return outval;

        }

        public static bool ToBool(string value)
        {
            bool outval = false;
            bool.TryParse(value, out outval);
            return outval;

        }
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataSet GetDsFromXml(string resp)
        {
            var mySourceDoc = new XmlDocument();
            byte[] encodedString = Encoding.UTF8.GetBytes(resp);

            MemoryStream ms = new MemoryStream(encodedString);
            ms.Flush();
            ms.Position = 0;

            //load the file from the stream
            mySourceDoc.Load(ms);

            DataSet ds = new DataSet();
            ds.ReadXml(new XmlNodeReader(mySourceDoc));

            return ds;

        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        public static List<T> CreateListFromTable<T>(DataTable tbl) where T : new()
        {
            // define return list
            List<T> lst = new List<T>();

            // go through each row
            foreach (DataRow r in tbl.Rows)
            {
                // add to the list
                lst.Add(CreateItemFromRow<T>(r));
            }

            // return the list
            return lst;
        }

        // function that creates an object from the given data row
        public static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            // create a new object
            T item = new T();

            // set the item
            SetItemFromRow(item, row);

            // return 
            return item;
        }

        public static void SetItemFromRow<T>(T item, DataRow row) where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }
        public static string encoding(string toEncode)
        {
            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(toEncode);
            string toReturn = System.Convert.ToBase64String(bytes);
            return toReturn;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static string DataTableToExcel(string extension, DataTable dt, string FilePath)
        {
            // dll refered NPOI.dll and NPOI.OOXML  

            IWorkbook workbook;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            CultureInfo Culture = Thread.CurrentThread.CurrentCulture;
            TextInfo TextInf = Culture.TextInfo;

            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                workbook = new HSSFWorkbook();
            }
            ISheet sheet1 = workbook.CreateSheet(dt.TableName);
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");
            //make a header row  
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                String columnName = TextInf.ToTitleCase(dt.Columns[j].ToString());
                cell.SetCellValue(columnName);
                cell.CellStyle = headerStyle;
            }

            //loops through data  
            var cellstyle = getCellStyle(workbook);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    cell.CellStyle = cellstyle;
                }
            }
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                sheet1.AutoSizeColumn(k, true);
            }
            // Declare one MemoryStream variable for write file in stream  
            var stream = new MemoryStream();
            workbook.Write(stream);

            using (var fs = File.Create(FilePath))
            {
                workbook.Write(fs);
            }
            workbook.Close();

            ////Write to file using file stream  
            //FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
            //stream.WriteTo(file);
            //file.Close();
            //stream.Close();

            return FilePath;
            // return workbook;
        }

        public static ICellStyle getCellStyle(IWorkbook workbook, string styleType = "")
        {
            ICellStyle cellStyle = (ICellStyle)workbook.CreateCellStyle();
            // create font style
            IFont myFont = (IFont)workbook.CreateFont();
            myFont.FontHeightInPoints = (short)10;
            myFont.FontName = "ARIAL";
            cellStyle.SetFont(myFont);
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.Alignment = HorizontalAlignment.Left;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            if (styleType.ToUpper() == "HEADER" || styleType.ToUpper() == "FOOTER")
            {
                cellStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;
            }
            else if (styleType.ToUpper() == "SUB_HEADER")
            {
                cellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;
            }
            return cellStyle;
        }

        public static string GetCSV(DataTable dtReport)
        {
            StringBuilder strBuild = new StringBuilder();
            string tab = "\t";
            string commma = "|";
            string newLine = "\n";

            strBuild.AppendLine(string.Join(commma, dtReport.Columns.Cast<DataColumn>().Select(c => c.ColumnName.Trim())));

            strBuild.Append(newLine);
            int i;
            foreach (DataRow dr in dtReport.Rows)
            {
                for (i = 0; i < dtReport.Columns.Count; i++)
                {
                    strBuild.Append(dr[i].ToString().Trim() + commma);
                    strBuild.Append(tab);
                }
                strBuild.Append(newLine);
            }
            return strBuild.ToString();
        }

        public static EmailLog SendEmail(string receiver, string subject, string message, EmailSettingsModel _objEmailSettingsModel, User user, string entity_id, string entity_type)
        {
            string MailStatus = "";
            try
            {
                bool isMailEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isMailEnabled"]);
                if (isMailEnabled)
                {

                    var senderEmail = new MailAddress(_objEmailSettingsModel.email_address);
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "";
                    var sub = subject;
                    var body = message;
                    var smtp = new SmtpClient
                    {
                        Host = _objEmailSettingsModel.smtp_host,
                        Port = _objEmailSettingsModel.port,
                        EnableSsl = _objEmailSettingsModel.enablessl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = _objEmailSettingsModel.usedefaultcredentials,
                        // Credentials = new NetworkCredential("USER@GMAIL.COM", "PASSWORD")
                        // Credentials = new NetworkCredential(_objEmailSettingsModel.email_address, DecodeTo64(_objEmailSettingsModel.email_password))

                        Credentials = CredentialCache.DefaultNetworkCredentials
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        IsBodyHtml = true,
                        Subject = subject,
                        Body = body

                    })
                        // bawait smtp.SendMailAsync(mess);
                        //{
                        smtp.Send(mess);
                    //}
                    //return SmtpStatusCode.Ok.ToString();
                    MailStatus = "Email Sent Successfully";
                }
                else
                {
                    MailStatus = "Email Configuration Not Enabled. Please enable it first";
                }

            }
            catch (Exception ex)
            {
                // throw ex;
                MailStatus = ex.Message;
            }
            EmailLog objEmailLog = setEmailLogValue(user, receiver, "", subject, message, entity_id, entity_type, MailStatus);
            return objEmailLog;
        }


        public static bool SendEmail(string[] receivers, string subject, string message, List<HttpPostedFileBase> files, out string mailSentMsg, List<EmailSettingsModel> listEmail, string user_email)
        {
            mailSentMsg = "";
            bool ismailsent = false;
            try
            {
                bool isMailEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isMailEnabled"]);


                var savedFilePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                if (isMailEnabled)
                {
                    foreach (var objEmailSetting in listEmail)
                    {
                        foreach (string receiver in receivers)
                        {
                            var senderEmail = !string.IsNullOrEmpty(user_email) ? (user_email) : objEmailSetting.email_address;
                            var sub = subject;
                            var body = message;

                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = objEmailSetting.smtp_host;
                            smtp.Timeout = 100000;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.EnableSsl = false;
                            NetworkCredential networkCredential = new NetworkCredential(objEmailSetting.email_address, MiscHelper.DecodeTo64(objEmailSetting.email_password));
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = networkCredential;
                            smtp.Port = objEmailSetting.port;

                            using (MailMessage mail = new MailMessage())
                            {
                                foreach (HttpPostedFileBase file in files)
                                {
                                    string[] filePath = Directory.GetFiles(savedFilePath, file.FileName);
                                    mail.Attachments.Add(new Attachment(filePath[0]));
                                }

                                mail.From = new MailAddress(objEmailSetting.email_address);
                                mail.To.Add(new MailAddress(receiver));
                                mail.Subject = subject;
                                mail.Body = message;
                                smtp.Send(mail);
                            }
                        }
                    }
                    mailSentMsg = "Email Sent Successfully";
                    ismailsent = true;

                }
                else
                {
                    ismailsent = false;
                    mailSentMsg = "Email Configuration Not Enabled. Please enable it";
                }
            }
            catch
            {
                ismailsent = false;
                throw;
            }
            return ismailsent;
        }
        public static bool SendEmailAsHtmlBody(string[] receivers, string subject, string message, List<HttpPostedFileBase> files, out string mailSentMsg, List<EmailSettingsModel> listEmail, List<string> filelist = null,string EmailEvent="")
        {
            mailSentMsg = "";
            bool ismailsent = false;
            try
            {
                bool isMailEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isMailEnabled"]);
                string path1 = System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"].ToString();
                var savedFilePath = HostingEnvironment.MapPath(path1);
                //var savedFilePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                if (isMailEnabled)
                {
                    foreach (var objEmailSetting in listEmail)
                    {
                        foreach (string receiver in receivers)
                        {
                            var senderEmail = new MailAddress(objEmailSetting.email_address);
                            var sub = subject;
                            var body = message;

                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = objEmailSetting.smtp_host;
                            smtp.Timeout = 100000;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.EnableSsl = false;
                            NetworkCredential networkCredential = new NetworkCredential(objEmailSetting.email_address, MiscHelper.DecodeTo64(objEmailSetting.email_password));
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = networkCredential;
                            smtp.Port = objEmailSetting.port;

                            using (MailMessage mail = new MailMessage())
                            {
                                if (files != null)
                                {
                                    foreach (HttpPostedFileBase file in files)
                                    {
                                        string[] filePath = Directory.GetFiles(savedFilePath, file.FileName);
                                        mail.Attachments.Add(new Attachment(filePath[0]));
                                    }
                                }
                                if (filelist != null)
                                {
                                    foreach (string file in filelist)
                                    {
                                        if (!string.IsNullOrEmpty(file))
                                        {
                                            mail.Attachments.Add(new Attachment(file));
                                        }
                                    }
                                }
                                mail.From = new MailAddress(objEmailSetting.email_address);
                                mail.To.Add(new MailAddress(receiver));
                                mail.Subject = subject;
                                mail.Body = message;
                                mail.IsBodyHtml = true;

                                smtp.Send(mail);
                                CaptureErrorInFile("EmailEvent :"+EmailEvent+" Subject:" + subject + ":Sender:" + objEmailSetting.email_address + ":receiver:" + receiver + ":Port:" + objEmailSetting.port.ToString() + ":smtp_host:" + objEmailSetting.smtp_host, "", "");
                            }
                        }
                    }

                    mailSentMsg = "EmailEvent :" + EmailEvent + ": Email Sent Successfully";
                    CaptureErrorInFile(mailSentMsg, "", "");
                    ismailsent = true;

                }
                else
                {
                    ismailsent = false;
                    mailSentMsg = "EmailEvent :" + EmailEvent + ": Email Configuration Not Enabled. Please enable it";
                    CaptureErrorInFile(mailSentMsg, "", "");
                }
            }
            catch
            {
                ismailsent = false;
                CaptureErrorInFile("Exception", "", "");
                throw;
            }
            return ismailsent;
        }

        public static EmailLog setEmailLogValue(User User, string emailTo, string emailCC, string subject, string body, string entity_id, string entity_type, string status)
        {
            EmailLog email_log = new EmailLog();
            email_log.user_id = User.user_id;
            email_log.user_name = User.user_name;
            email_log.entity_id = entity_id;
            email_log.entity_type = entity_type;
            email_log.email_from = User.user_email;
            email_log.email_to = emailTo;
            email_log.email_to_cc = emailCC;
            email_log.body = body;
            email_log.subject = subject;
            email_log.sent_on = DateTimeHelper.Now;
            email_log.status = status;
            return email_log;
        }
        public static List<geoloc> DecodePolylinePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
            List<geoloc> poly = new List<geoloc>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;
            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;
            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);
                    if (index >= polylinechars.Length)
                        break;
                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    geoloc p = new geoloc();
                    p.lat = Convert.ToDouble(currentLat) / 100000.0;
                    p.lng = Convert.ToDouble(currentLng) / 100000.0;
                    poly.Add(p);
                }
            }
            catch (Exception ex)
            {

            }
            return poly;
        }
        public static string getBetween(string strSource, string strStart, string strEnd, bool includeStartText)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return string.Concat(strStart, strSource.Substring(Start, End - Start));
            }
            else
            {
                return "";
            }
        }

        public static void Base64toImage(string fullOutputPath, string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
                image.Save(fullOutputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public static string GetEmailBody(Dictionary<string, string> objList, string message)
        {
            foreach (KeyValuePair<string, string> obj in objList)
            {
                message = message.Replace("{" + obj.Key + "}", obj.Value);
            }
            return message;
        }

        public static void SendEventBasedEmail(List<EventEmailTemplateDetail> objEventEmailTemplateDetail, Dictionary<string, string> objKeyValueList, List<HttpPostedFileBase> objHttpPostedFileBase, List<EmailSettingsModel> lstEmailSettings, List<string> objFileList = null, string ProjectName = "",string EmailEvent = "")
        {
            try
            {
                string emailBody = commonUtil.GetEmailBody(objKeyValueList, objEventEmailTemplateDetail[0].template.ToString());
                string subject = objEventEmailTemplateDetail[0].subject.Replace("XXX", ProjectName);
                string mailSentMessage;
                //List<EmailSettingsModel> objEmailSettingsModelList = BLMisc.EmailSettingsModel;
                if (objHttpPostedFileBase == null)
                {
                    objHttpPostedFileBase = new List<HttpPostedFileBase>();
                }
                bool IsSent = true;

                IsSent = commonUtil.SendEmailAsHtmlBody(objEventEmailTemplateDetail[0].recipient_list.Split(','), subject, emailBody, objHttpPostedFileBase, out mailSentMessage, lstEmailSettings, objFileList,EmailEvent);
            }
            catch (Exception ex)
            {
                string message = "EmailEvent : "+EmailEvent + " : "+ ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message + "." + (ex.InnerException.InnerException != null ? ex.InnerException.InnerException.ToString() : "") : string.Empty;
                string stackTrace = ex.StackTrace;
                commonUtil.CaptureErrorInFile(message, innerException, stackTrace);

            }
            finally { }
        }

        public static bool SendEmailWithAttachment(string[] receivers, string subject, string message, List<string> files, out string mailSentMsg, List<EmailSettingsModel> listEmail)
        {
            mailSentMsg = "";
            bool ismailsent = false;
            try
            {
                bool isMailEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isMailEnabled"]);


                var savedFilePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                if (isMailEnabled)
                {
                    foreach (var objEmailSetting in listEmail)
                    {
                        foreach (string receiver in receivers)
                        {
                            var senderEmail = new MailAddress(objEmailSetting.email_address);
                            var sub = subject;
                            var body = message;

                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = objEmailSetting.smtp_host;
                            smtp.Timeout = 100000;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.EnableSsl = false;
                            NetworkCredential networkCredential = new NetworkCredential(objEmailSetting.email_address, MiscHelper.DecodeTo64(objEmailSetting.email_password));
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = networkCredential;
                            smtp.Port = objEmailSetting.port;

                            using (MailMessage mail = new MailMessage())
                            {
                                foreach (string file in files)
                                {
                                    if (!string.IsNullOrEmpty(file))
                                    {
                                        string[] filePath = Directory.GetFiles(savedFilePath, file);
                                        mail.Attachments.Add(new Attachment(filePath[0]));
                                    }
                                }

                                mail.From = new MailAddress(objEmailSetting.email_address);
                                mail.To.Add(new MailAddress(receiver));
                                mail.Subject = subject;
                                mail.Body = message;
                                smtp.Send(mail);
                            }
                        }
                    }
                    mailSentMsg = "Email Sent Successfully";
                    ismailsent = true;

                }
                else
                {
                    ismailsent = false;
                    mailSentMsg = "Email Configuration Not Enabled. Please enable it";
                }
            }
            catch
            {
                ismailsent = false;
                throw;
            }
            return ismailsent;
        }

        public static void CaptureErrorInFile(string message, string innerexception, string stacktrace)
        {
            var filename = "EmailErrorLog" + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string directory = HostingEnvironment.MapPath(System.Configuration.ConfigurationManager.AppSettings["logFolderPath"].ToString());
//            string directory = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["logFolderPath"]);
            string logFilePath = directory + filename;
            // Get the current date and time for the log message
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // Log message to be written
            StringBuilder objErrorMessage = new StringBuilder();
            if (!String.IsNullOrEmpty(message))
                objErrorMessage.Append($"[{timestamp}] : Message :" + message + "\r\n");
            if(!String.IsNullOrEmpty(innerexception))
            objErrorMessage.Append($"[{timestamp}] : InnerException :" + innerexception + "\r\n");
            if (!String.IsNullOrEmpty(stacktrace))
                objErrorMessage.Append($"[{timestamp}] : StackTrace :" + stacktrace);
            string logMessage = $"[{timestamp}] : Message :" + message;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!File.Exists(logFilePath))
            {
                // Create the file
                using (FileStream fs = File.Create(logFilePath))
                {

                }
            }
            // Write the log message to the file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(objErrorMessage);
            }
        }

    }


    public static class ListExtensions
    {
        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 5)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }

    public class geoloc
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class KmlLang
    {
        public string pathType { get; set; }
        public List<geoloc> LatLng { get; set; }


    }
}
