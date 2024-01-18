
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Utility
{
    public class MiscHelper
    {
        public static string FormatDate(string date)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(date))
            {
                result = Convert.ToDateTime(date).ToString("dd-MMM-yyyy", new CultureInfo("en-US"));
            }
            return result;
        }


        public static string FormatDateTime(string date)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(date))
            {
                result = Convert.ToDateTime(date).ToString("dd-MMM-yy hh:mm tt", new CultureInfo("en-US"));
            }
            return result;
        }

        public static string FormatTime(string date)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(date))
            {
                result = Convert.ToDateTime(date).ToString("hh:mm tt", new CultureInfo("en-US"));
            }
            return result;
        }

        public static DataTable ListToDataTable<T>(IList<T> items,bool numberFormatRequired=false,string numberFormat=null, string[] columnsToExclude=null)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            if (items != null && items.Count > 0)
            {
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Defining type of data column gives proper data table 
                    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                    //Setting column names as Property names
                    if(numberFormatRequired)
                        dataTable.Columns.Add(prop.Name.ToUpper(), typeof(string));
                    else
                            dataTable.Columns.Add(prop.Name.ToUpper(), type);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        if ((numberFormatRequired && columnsToExclude==null) || (numberFormatRequired && !columnsToExclude.Contains(Props[i].Name.ToUpper(), StringComparer.OrdinalIgnoreCase)))
                        {
                            if(double.TryParse(Convert.ToString( Props[i].GetValue(item, null)),out double d))
                            {
                                values[i] = Utility.CommonUtility.GetFormattedNumber(d,numberFormat);
                            }
                            else if (int.TryParse(Convert.ToString(Props[i].GetValue(item, null)), out int number))
                            {
                                values[i] = Utility.CommonUtility.GetFormattedNumber(number, numberFormat);
                            }
                            else
                            {
                                values[i] = Props[i].GetValue(item, null);
                            }
                        }
                        else
                        {
                            //inserting property values to datatable rows
                            values[i] = Props[i].GetValue(item, null);
                        }
                    }
                    dataTable.Rows.Add(values);
                }
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }


        public static string EncodeTo64(string strText)
        {

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(strText);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;


        }

        public static string DecodeTo64(string strText)
        {
            byte[] toDecodeAsBytes = Convert.FromBase64String(strText);
            string returnValue = Encoding.UTF8.GetString(toDecodeAsBytes);
            return returnValue;
        }



        public static void CopyMatchingProperties(object source, object destination)
        {
            FieldInfo[] fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Concat(source.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                .Concat(source.GetType().BaseType.BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)).ToArray();

            foreach (FieldInfo fi in fields)
            {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }
        public static void CopyMatchingBaseProperties(object source, object destination)
        {


            FieldInfo[] fields = source.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToArray();

            foreach (FieldInfo fi in fields)
            {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }


        public static DataTable GetDataTableFromDictionaries<T>(List<Dictionary<string, T>> list, bool numberFormatRequired = false, string numberFormat = null, string[] columnsToExclude = null)
        {
            DataTable dataTable = new DataTable();

            if (list == null || !list.Any()) return dataTable;

            foreach (var column in list.First().Select(c => new DataColumn(c.Key, typeof(T))))
            {
                if (numberFormatRequired)
                    dataTable.Columns.Add(column.ColumnName, typeof(string));
                else
                    dataTable.Columns.Add(column);
            }

            foreach (var row in list.Select(
                r =>
                {
                    var dataRow = dataTable.NewRow();
                    r.ToList().ForEach(c => dataRow.SetField(c.Key,c.Value));
                    return dataRow;
                }))
            {
                if (numberFormatRequired)
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (  (columnsToExclude == null ||  !columnsToExclude.Contains(column.ColumnName, StringComparer.OrdinalIgnoreCase)))
                        {
                            if (double.TryParse(Convert.ToString(row[column.ColumnName]), out double d))
                            {
                                row[column.ColumnName] = Utility.CommonUtility.GetFormattedNumber(d, numberFormat);
                            }
                            else if (int.TryParse(Convert.ToString(row[column.ColumnName]), out int number))
                            {
                                row[column.ColumnName] = Utility.CommonUtility.GetFormattedNumber(number, numberFormat);
                            }
                        }
                            
                    }
                    dataTable.Rows.Add(row);
                }
                else
                    dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        public static DataTable GetDataTableFromDictionaries<T>(List<Dictionary<string, T>> list, bool isKeyValueFormat)
        {
            DataTable dataTable = new DataTable();
            if (list == null || !list.Any() || list.Count > 1) return dataTable;
            if (isKeyValueFormat)
            {
                dataTable.Columns.Add("FIELD_NAME");
                dataTable.Columns.Add("FIELD_VALUE");
                var dictionary = list[0];
                dictionary.Remove("system_id");
                foreach (var key in dictionary.Keys)
                {
                    DataRow dtRow = dataTable.NewRow();
                    dtRow[0] = key;
                    dtRow[1] = dictionary[key];
                    dataTable.Rows.Add(dtRow);
                }
            }
            return dataTable;
        }

        /// <summary>
        ///  Clean the directory before Upload the new shape file
        /// </summary>
        public static void CleanDirectorypath(string Dirpath, int user_id)
        {
            try
            {
                string[] filePaths = Directory.GetFiles(Dirpath);
                string fileName = string.Empty;

                foreach (string filePath in filePaths)
                {
                    fileName = Path.GetFileName(filePath);
                    fileName = Path.GetFileNameWithoutExtension(fileName);

                    if (fileName.Substring(fileName.LastIndexOf('_') + 1) == Convert.ToString(user_id))
                    {
                        System.IO.File.Delete(filePath);
                    }

                }
            }
            catch { }
        }


        public void CopyFile(string srcDirPath, string destDirPath, string srcFileName, string destFileName)
        {
            try
            {
                if (!Directory.Exists(destDirPath)) { Directory.CreateDirectory(destDirPath); }
                File.Copy(string.Concat(srcDirPath, "\\", srcFileName), string.Concat(destDirPath, "\\", destFileName));
            }
            catch { throw; }
        }
        //public void bindportdetails(dynamic objmaster, string entype, string ddltype)
        //{
        //    var layerdetails = new bllayer().getlayer(entype);
        //    if (layerdetails != null)
        //    {
        //        objmaster.unit_input_type = layerdetails.unit_input_type;
        //        if (layerdetails.unit_input_type == unitinputtype.iopddl.tostring())
        //        {
        //            var objresp = new blmisc().getdropdownlist(entype, ddltype);
        //            objmaster.lstiopdetails = objresp;
        //        }
        //    }
        //}
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
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

        public static string getTimeStamp()
        {
            return DateTimeHelper.Now.Year.ToString() + DateTimeHelper.Now.Month.ToString() + DateTimeHelper.Now.Hour.ToString() + DateTimeHelper.Now.Minute.ToString() + DateTimeHelper.Now.Second.ToString() + DateTimeHelper.Now.Millisecond.ToString();
        }

        public static string getPortName(int portNo)
        {
            return portNo.ToString().Substring(0, 1) == "-" ? portNo.ToString().Replace("-", "") + " IN" : portNo + " OUT";
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
        public static string TimeAgo(string dateTime)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(dateTime)) return string.Empty;

            var timeSpan = DateTimeHelper.Now.Subtract(Convert.ToDateTime(dateTime));
            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_134, timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = String.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_135, timeSpan.Minutes);
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = String.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_136, timeSpan.Hours);
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_137, timeSpan.Days) :
                   Resources.Resources.SI_GBL_GBL_GBL_GBL_140;
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = String.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_138, timeSpan.Days / 30);
            }
            else
            {
                result = String.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_139, timeSpan.Days / 365);
            }

            return result;
        }
        public static bool GetLayerEditPermission(int system_id, List<NetworkLayer> networkLayerList, string networkStatus, string layerName)
        {
            if (system_id == 0)
            {
                return true;
            }

            if (string.IsNullOrEmpty(networkStatus))
            {
                return true;
            }
            //((List<NetworkLayer>)Session["NerworkLayerDetails"]).Select(x => x.planned_edit).FirstOrDefault();
            var layerDetails = networkLayerList.Where(x => x.layerName.ToUpper() == layerName.ToUpper() || x.layerTitle.ToUpper() == layerName.ToUpper()).FirstOrDefault();
            if (layerDetails == null)
            {
                return true;
            }
            return networkStatus.ToUpper() == NetworkStatus.P.ToString() ? layerDetails.planned_edit : (networkStatus.ToUpper() == NetworkStatus.A.ToString() ? layerDetails.asbuild_edit : layerDetails.dormant_edit);
        }

        public static bool GetLayerAddPermission(List<NetworkLayer> networkLayerList, string networkStatus, string layerName)
        {
            if (string.IsNullOrEmpty(networkStatus))
            {
                return true;
            }
            //((List<NetworkLayer>)Session["NerworkLayerDetails"]).Select(x => x.planned_edit).FirstOrDefault();
            var layerDetails = networkLayerList.Where(x => x.layerName.ToUpper() == layerName.ToUpper() || x.layerTitle.ToUpper() == layerName.ToUpper()).FirstOrDefault();
            if (layerDetails == null)
            {
                return true;
            }
            return networkStatus.ToUpper() == NetworkStatus.P.ToString() ? layerDetails.planned_add : (networkStatus.ToUpper() == NetworkStatus.A.ToString() ? layerDetails.asbuild_add : layerDetails.dormant_add);
        }

        public static bool GetLayerDeletePermission(List<NetworkLayer> networkLayerList, string networkStatus, string layerName)
        {
            if (string.IsNullOrEmpty(networkStatus))
            {
                return true;
            }
            //((List<NetworkLayer>)Session["NerworkLayerDetails"]).Select(x => x.planned_edit).FirstOrDefault();
            var layerDetails = networkLayerList.Where(x => x.layerName.ToUpper() == layerName.ToUpper() || x.layerTitle.ToUpper() == layerName.ToUpper()).FirstOrDefault();
            if (layerDetails == null)
            {
                return true;
            }
            return networkStatus.ToUpper() == NetworkStatus.P.ToString() ? layerDetails.planned_delete : (networkStatus.ToUpper() == NetworkStatus.A.ToString() ? layerDetails.asbuild_delete : layerDetails.dormant_delete);
        }

        public static string GetNetworkStatus(string networkStatus)
        {
            if (string.IsNullOrEmpty(networkStatus)) return string.Empty;
            return networkStatus.ToUpper() == NetworkStatus.P.ToString() ? "Planned" : (networkStatus.ToUpper() == NetworkStatus.A.ToString() ? "As-Built" : "Dormant");
        }
        public static string ToCamelCase(string str)
        {
            StringBuilder sbOutput = new StringBuilder();
            string[] strArray = str.Split('_');
            foreach (string word in strArray)
            {
                if (!string.IsNullOrEmpty(word) && word.Length > 0)
                {
                    sbOutput.Append(" " + Char.ToUpperInvariant(word[0]) + word.Substring(1));
                }
            }
            return sbOutput.ToString().Substring(1);
        }

        public static string FormatBoolean(bool value)
        {
            var result = string.Empty;
            if (value)
            {
                result = "Yes";
            }
            else
            {
                result = "No";
            }
            return result;
        }

        public static string TextWithLimit(string text, int limit)
        {
            var dots = "...";
            if (text.Length > limit)
            {
                // you can also use substr instead of substring
                text = text.Substring(0, limit) + dots;
            }

            return text;
        }

        public static string MaskMobile(string phoneno)
        {
            return string.Format("{0}******{1}", phoneno.Substring(0, 2), phoneno.Substring(8, 2));
        }
        public static string MaskEmail(string email)
        {

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;

            string[] emailArr = email.Split('@');
            string domainExt = Path.GetExtension(email);

            string maskedEmail = string.Format("{0}****{1}@{2}****{3}{4}",
                emailArr[0][0],
                emailArr[0].Substring(emailArr[0].Length - 1),
                emailArr[1][0],
                emailArr[1].Substring(emailArr[1].Length - domainExt.Length - 1, 1),
                domainExt
                );

            return maskedEmail;
        }

        public static void CopyMatchingGISProperties(object source, object destination)
        {
            FieldInfo[] fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Concat(source.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)).ToArray();
            FieldInfo[] _fields = destination.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Concat(destination.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)).ToArray();
            foreach (FieldInfo fi in fields)
            {
                foreach (FieldInfo _fi in _fields)
                {
                    if (fi.GetValue(source) != null)
                    {
                        if (_fi.GetValue(destination) == null)
                        {
                            fi.SetValue(destination, fi.GetValue(source));
                        }
                    }
                }
            }
        }
    }

}
