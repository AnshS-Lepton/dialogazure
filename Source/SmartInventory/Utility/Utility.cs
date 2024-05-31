using Models;
using Models.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace Utility
{
    public static class CommonUtility
    {
        public static Dictionary<string, string> GetBulkUploadColumnMapping(string filepath)
        {
            Dictionary<string, string> dicMapping = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(filepath);
            return dicMapping = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
                .Select(p => new
                {
                    DbColName = p.Element("DbColName").Value.ToLower().Trim().Replace(" ", "_"),
                    TemplateColName = p.Element("TemplateColName").Value.ToLower().Trim().Replace(" ", "_")
                })
                .ToDictionary(t => t.DbColName, t => t.TemplateColName);
        }
        public static string GetFormattedNumber(decimal number, string formatType)
        {
            if (formatType.ToLower() == "saarc")
                return number.ToString("N0");
            else if (formatType.ToLower() == "europe")
                return number.ToString("N0", CultureInfo.GetCultureInfo("en-US"));
            else
                return number.ToString();

        }
        public static string GetFormattedNumber(double number, string formatType)
        {
            if (formatType.ToLower() == "saarc")
            {
                if (number % 1 == 0) // Check if the number is an integer
                {
                    return number.ToString("N0"); 
                }
                else
                {
                    return number.ToString("N2"); 
                }
            }
            else if (formatType.ToLower() == "europe")
                return number.ToString("N0", CultureInfo.GetCultureInfo("en-US"));
            else
                return number.ToString();

        }
        public static string GetFormattedNumber(long number, string formatType)
        {

            if (formatType.ToLower() == "saarc")
                return number.ToString("N0");
            else if (formatType.ToLower() == "europe")
                return number.ToString("N0", CultureInfo.GetCultureInfo("en-US"));
            else
                return number.ToString();
        }
        public static string GetFormattedNumber(int number, string formatType)
        {
            if (formatType.ToLower() == "saarc")
                return number.ToString("N0");
            else if (formatType.ToLower() == "europe")
                return number.ToString("N0", CultureInfo.GetCultureInfo("en-US"));
            else
                return number.ToString();
        }

        public static List<Mapping> GetBulkUploadColumnMappingTemplate(string filepath)
        {
            List<Mapping> lstMapping = new List<Mapping>();
            XDocument doc = XDocument.Load(filepath);
            Mapping ObjMap = new Mapping();
            var map = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
               .Select(p => new
               {
                   DbColName = p.Element("DbColName").Value.ToLower().Trim(),
                   TemplateColName = p.Element("TemplateColName").Value.Trim(),
                   IsMandatory = Convert.ToBoolean(p.Element("IsMandatory").Value),
                   IsNullable = p.Element("IsNullable").Value.ToLower(),
                   UdtName = p.Element("UdtName").Value,
                   CharacterMaxLength = p.Element("CharacterMaxLength").Value,
                   IsDropDown = p.Elements("IsDropDown").Any() ? Convert.ToBoolean(p.Element("IsDropDown").Value) : false,
               })
               .ToList();

            lstMapping = map.Select(m => new Mapping
            {
                DbColName = m.DbColName,
                TemplateColName = m.TemplateColName,
                IsMandatory = Convert.ToBoolean(m.IsMandatory),
                //IsNullable = m.IsNullable,
                DBColumnDataType = m.UdtName,
                CharacterMaxLength = m.CharacterMaxLength,
                IsDropDown = Convert.ToBoolean(m.IsDropDown),
            }).ToList();

            return lstMapping;
        }
        public static DataTable GetFormattedDataTable(DataTable dtReport, string formatType)
        {
            int noColumns = dtReport.Columns.Count;
            for (int i = 0; i < noColumns; i++)
            {
                dtReport.Columns.Add(dtReport.Columns[i].ColumnName + "_new", typeof(string));
            }
            //Copying data from one DT to another DT to avoid data issue
            foreach (DataRow dr in dtReport.Rows)
            {
                foreach (DataColumn dc in dtReport.Columns)
                {
                    if (dc.ColumnName.Contains("_new"))
                        dr[dc.ColumnName] = dr[dc.ColumnName.Replace("_new", "")];
                }
            }
            noColumns = dtReport.Columns.Count;
            List<string> dropColumns = new List<string>();
            for (int i = 0; i < noColumns; i++)
            {
                if (!dtReport.Columns[i].ColumnName.Contains("_new"))
                    dropColumns.Add(dtReport.Columns[i].ColumnName);
            }
            foreach (string colName in dropColumns)
                dtReport.Columns.Remove(colName);

            foreach (DataRow dr in dtReport.Rows)
            {
                foreach (DataColumn dc in dtReport.Columns)
                {
                    if (int.TryParse(Convert.ToString(dr[dc.ColumnName]), out int a))
                    {
                        dr[dc.ColumnName] = Utility.CommonUtility.GetFormattedNumber(a, formatType);
                    }
                    else if (Double.TryParse(Convert.ToString(dr[dc.ColumnName]), out Double d))
                    {
                        dr[dc.ColumnName] = Utility.CommonUtility.GetFormattedNumber(d, formatType);
                    }
                }
            }

            noColumns = dtReport.Columns.Count;
            for (int i = 0; i < noColumns; i++)
            {
                if (dtReport.Columns[i].ColumnName.Contains("_new"))
                    dtReport.Columns[i].ColumnName = dtReport.Columns[i].ColumnName.Replace("_new", "");
            }
            return dtReport;
        }
        public static ErrorMessage validateTemplateColumn(List<Mapping> dicColumnMapping, DataTable dt)
        {
            ErrorMessage ErrMessage = new ErrorMessage();
            string[] arrColumns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower().Trim()).ToArray();
            List<string> ErrColumnNames = new List<string>();
            //foreach (var pair in dicColumnMapping.Where(x => x.IsMandatory == true).ToList())
            foreach (var pair in dicColumnMapping.Where(x => x.is_template_column_required == true).ToList())
            {
                // if column not found in template and return error..
                if (!arrColumns.Contains(pair.TemplateColName.ToLower()))
                {
                    ErrColumnNames.Add(pair.TemplateColName);
                }
            }
            ErrMessage.error_msg = (ErrColumnNames.Count > 0) ? string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_112, "color:red", string.Join(",<br>", ErrColumnNames)) : "";
            ErrMessage.error_code = StatusCodes.INVALID_INPUTS.ToString();
            ErrMessage.status = StatusCodes.INVALID_INPUTS.ToString();
            ErrMessage.is_valid = false;
            return ErrMessage;
        }
        //This method is useless
        public static ErrorMessage validateTemplateColumn(Dictionary<string, string> dicColumnMapping, DataTable dt)
        {
            ErrorMessage ErrMessage = new ErrorMessage();
            string[] arrColumns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.Trim().ToLower().Replace(" ", "_")).ToArray();
            List<string> ErrColumnNames = new List<string>();
            foreach (var pair in dicColumnMapping)
            {
                // if column not found in template and return error..
                if (!arrColumns.Contains(pair.Value.ToLower()))
                {
                    ErrColumnNames.Add(pair.Value);
                }
            }
            ErrMessage.error_msg = (ErrColumnNames.Count > 0) ? string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_112, "color:red", string.Join(",<br>", ErrColumnNames)) : "";
            ErrMessage.error_code = StatusCodes.INVALID_INPUTS.ToString();
            ErrMessage.status = StatusCodes.INVALID_INPUTS.ToString();
            ErrMessage.is_valid = false;
            return ErrMessage;
        }

        public static string Wrap(string str, int maxLength, string prefix)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (maxLength <= 0) return prefix + str;

            var lines = new List<string>();

            // breaking the string into lines makes it easier to process.
            foreach (string line in str.Split("\n".ToCharArray()))
            {
                var remainingLine = line.Trim();
                do
                {
                    var newLine = GetLine(remainingLine, maxLength - prefix.Length);
                    lines.Add(newLine);
                    remainingLine = remainingLine.Substring(newLine.Length).Trim();
                    // Keep iterating as int as we've got words remaining 
                    // in the line.
                } while (remainingLine.Length > 0);
            }

            return string.Join(Environment.NewLine + prefix, lines.ToArray());
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
        private static string GetLine(string str, int maxLength)
        {
            // The string is less than the max length so just return it.
            if (str.Length <= maxLength) return str;

            // Search backwords in the string for a whitespace char
            // starting with the char one after the maximum length
            // (if the next char is a whitespace, the last word fits).
            for (int i = maxLength; i >= 0; i--)
            {
                if (char.IsWhiteSpace(str[i]))
                    return str.Substring(0, i).TrimEnd();
            }

            // No whitespace chars, just break the word at the maxlength.
            return str.Substring(0, maxLength);
        }
        public static DataTable CheckDataTableForBlankRecords(DataTable dataTable)
        {
            foreach (DataColumn dc in dataTable.Columns)
            {
                dc.ColumnName = dc.ColumnName.Trim().ToLower();
            }

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                //dataTable = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
                //dataTable = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();
                dataTable.Rows.Cast<DataRow>().ToList().FindAll(Row =>
                { return string.IsNullOrEmpty(string.Join("", Row.ItemArray)); }).ForEach(Row =>
                { dataTable.Rows.Remove(Row); });

            }
            return dataTable;
        }
        public static void AddErrorFields(DataTable dataTable, UploadSummary summary)
        {
            //ADD COLUMN TO DTEXCEL.. (UPLOADED_BY)
            DataColumn dcUploadedBy = new DataColumn("UPLOADED_BY", typeof(int));
            dcUploadedBy.DefaultValue = summary.user_id;
            dataTable.Columns.Add(dcUploadedBy);

            //ADD COLUMN TO DTEXCEL.. (IS_VALID)
            DataColumn dcIsValid = new DataColumn("IS_VALID", typeof(int));
            dataTable.Columns.Add(dcIsValid);
        }
        private static string getWorkbookTemplate()
        {
            var sb = new StringBuilder(818);
            sb.AppendFormat(@"<?xml version=""1.0""?>{0}", Environment.NewLine);
            sb.AppendFormat(@"<?mso-application progid=""Excel.Sheet""?>{0}", Environment.NewLine);
            sb.AppendFormat(@"<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""{0}", Environment.NewLine);
            sb.AppendFormat(@" xmlns:o=""urn:schemas-microsoft-com:office:office""{0}", Environment.NewLine);
            sb.AppendFormat(@" xmlns:x=""urn:schemas-microsoft-com:office:excel""{0}", Environment.NewLine);
            sb.AppendFormat(@" xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""{0}", Environment.NewLine);
            sb.AppendFormat(@" xmlns:html=""http://www.w3.org/TR/REC-html40"">{0}", Environment.NewLine);
            sb.AppendFormat(@" <Styles>{0}", Environment.NewLine);
            sb.AppendFormat(@"  <Style ss:ID=""Default"" ss:Name=""Normal"">{0}", Environment.NewLine);
            sb.AppendFormat(@"   <Alignment ss:Vertical=""Bottom""/>{0}", Environment.NewLine);
            sb.AppendFormat(@"   <Borders/>{0}", Environment.NewLine);
            sb.AppendFormat(@"   <Font ss:FontName=""Calibri"" x:Family=""Swiss"" ss:Size=""11"" ss:Color=""#000000""/>{0}", Environment.NewLine);
            sb.AppendFormat(@"   <Interior/>{0}", Environment.NewLine);
            sb.AppendFormat(@"   <NumberFormat/>{0}", Environment.NewLine);
            sb.AppendFormat(@"   <Protection/>{0}", Environment.NewLine);
            sb.AppendFormat(@"  </Style>{0}", Environment.NewLine);
            sb.AppendFormat(@"  <Style ss:ID=""s62"">{0}", Environment.NewLine);
            sb.AppendFormat(@"   <Font ss:FontName=""Calibri"" x:Family=""Swiss"" ss:Size=""11"" ss:Color=""#000000""{0}", Environment.NewLine);
            sb.AppendFormat(@"    ss:Bold=""1""/>{0}", Environment.NewLine);
            sb.AppendFormat(@"  </Style>{0}", Environment.NewLine);
            sb.AppendFormat(@"  <Style ss:ID=""s63"">{0}", Environment.NewLine);
            sb.AppendFormat(@"   <NumberFormat ss:Format=""Short Date""/>{0}", Environment.NewLine);
            sb.AppendFormat(@"  </Style>{0}", Environment.NewLine);
            sb.AppendFormat(@" </Styles>{0}", Environment.NewLine);
            sb.Append(@"{0}\r\n</Workbook>");
            return sb.ToString();
        }
        public static string GetIconURL(string path, string imageName)
        {
            string iconpath = string.Empty;
            try
            {
                HttpWebRequest request = WebRequest.Create(imageName) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";

                //Getting the Web Response
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                iconpath = imageName;
            }
            catch (Exception fl) { iconpath = path + "DEFAULT.png"; };

            return iconpath;
        }


        public static string ValidateGetTemplateInput(int userId)
        {
            if (userId == 0)
            {
                return "Invalid user id!";
            }
            return "";
        }

        public static string ConvertDataTableToString(DataTable dt, string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            string[] columnNames = dt.Columns.Cast<DataColumn>().
                              Select(column => column.ColumnName).
                              ToArray();
            sb.AppendLine(string.Join(delimiter, columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                            ToArray();
                sb.AppendLine(string.Join(delimiter, fields));
            }
            return sb.ToString();
        }

        public static string ConvertStringToShortFormat(string strValue, int minLength, int beforDot, int afterDot)
        {
            if (strValue.Length > minLength)
            {
                strValue = string.Concat(strValue.Substring(0, beforDot), "..." + strValue.Substring(strValue.Length - afterDot));
            }
            return strValue;
        }

        public static string FTPFileUpload(string filePathToUpload, string filename, string ftpPath, string sUserName, string sPassword)
        {

            WebClient client = null;
            try
            {

                string _filepath = filePathToUpload;
                string _filename = filename;
                string _ftppath = System.IO.Path.Combine(ftpPath, _filename);
                client = new WebClient();
                client.Credentials = new System.Net.NetworkCredential(sUserName, sPassword);
                client.UploadFile(_ftppath, _filepath);
                Console.WriteLine("File uploaded on configured FTP successfully");

            }

            catch (Exception)

            {

                Console.WriteLine("Error found while uploading File", "Log");

            }

            finally { client.Dispose(); }
            return "";
        }



    }
}