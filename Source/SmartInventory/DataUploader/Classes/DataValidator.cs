using BusinessLogics;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility;

namespace DataUploader
{
    public class DataValidator
    {
        BLDataUploader bLDataUploader;
        public DataValidator()
        {
            bLDataUploader = new BLDataUploader();
            //Add Notification Handler if required here
        }

        public static ErrorMessage ValidateExcel(UploadSummary summary, DataTable dataTable)
        {
            try
            {
                BLDataUploader bLDataUploader = new BLDataUploader();
                List<Mapping> lstMapping = bLDataUploader.GetMappings(summary.entity_type);
                BLLayer objBLLayer = new BLLayer();
                layerDetail networkLayerDetails = objBLLayer.GetLayerDetails(summary.entity_type);
                List<ParentChildLayerMapping> layerMapping = objBLLayer.GetParentChildLayerMappings();
                return ValidateFieldsData(lstMapping, dataTable, networkLayerDetails, layerMapping, summary.entity_type);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "ValidateExcel", ex);
                throw;
            }
        }
        private static ErrorMessage validateMandatoryColumn(List<Mapping> lstMapping, DataTable dt, layerDetail networkLayer, ErrorMessage errorMessage)
        {
            try
            {
                List<Mapping> lstMandatoryMapping = lstMapping.Where(m => m.IsMandatory == true).ToList();
                var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                object lockObj = new object();
                Parallel.ForEach(dt.AsEnumerable(), options, (dr) =>
                {
                    lock (lockObj)
                    {
                        string strMessge = "Mandatory fields are blank for columns :";
                        string dbMandatoryColName = string.Empty;
                        bool isInValid = false;
                        foreach (Mapping mandatoryField in lstMandatoryMapping)
                        {
                            if (networkLayer.network_id_type == "A" && mandatoryField.DbColName.ToLower() == "network_id")
                            {
                                continue;
                            }
                            if (!dt.Columns.Contains(mandatoryField.TemplateColName))
                            {
                                if (dt.Columns.Contains(mandatoryField.DbColName))
                                {
                                    string mandatoryValue = Convert.ToString(dr[mandatoryField.DbColName]);
                                    if (string.IsNullOrEmpty(mandatoryValue))
                                    {
                                        isInValid = true;
                                        dbMandatoryColName += mandatoryField.DbColName + " ,";
                                    }
                                }
                            }
                            else if (dt.Columns.Contains(mandatoryField.TemplateColName))
                            {

                                string mandatoryValue = Convert.ToString(dr[mandatoryField.TemplateColName]);
                                if (string.IsNullOrEmpty(mandatoryValue))
                                {
                                    isInValid = true;
                                    dbMandatoryColName += mandatoryField.TemplateColName + " ,";
                                }
                            }
                        }
                        if (isInValid)
                        {
                            dr["error_msg"] += strMessge + dbMandatoryColName.TrimEnd(',');
                            dr["is_valid"] = false;
                        }
                        else
                        {
                            dr["is_valid"] = true;
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                errorMessage.status = StatusCodes.FAILED.ToString();
                errorMessage.error_msg = ex.Message;
                ErrorLogHelper.WriteErrorLog("DataValidator", "validateMandatoryColumn", ex);
            }
            return errorMessage;
        }

        private static ErrorMessage CheckNetworkIDFormat(List<Mapping> lstMapping, DataTable dt, List<ParentChildLayerMapping> layerMapping, ErrorMessage errorMessage, string entityType)
        {
            try
            {
                //if (networkLayer.network_id_type == "M")
                //{
                Mapping networkIdmapping = lstMapping.Where(m => m.DbColName.ToLower() == "network_id").First();
                Mapping parentTypeMapping = lstMapping.Where(m => m.DbColName.ToLower() == "parent_entity_type").First();

                DataRow[] drRemainingValidRecords = dt.Select("is_valid=" + true);
                var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                object lockObj = new object();
                if (dt.Columns.Contains(parentTypeMapping.TemplateColName) && dt.Columns.Contains(networkIdmapping.TemplateColName))
                {
                    Parallel.ForEach(drRemainingValidRecords, options, (dr) =>
                    {
                        lock (lockObj)
                        {
                            // string parentType = dr[parentTypeMapping.TemplateColName].ToString().ToUpper(); 

                            string parentType = Convert.ToString(dr[parentTypeMapping.TemplateColName]).ToUpper();
                            if (string.IsNullOrEmpty(parentType))
                            {
                                var isDefaultParentExist = layerMapping.Where(m => string.Equals(m.child_layer_name, entityType, StringComparison.CurrentCultureIgnoreCase) && m.is_default_parent == true).FirstOrDefault();
                                if (isDefaultParentExist != null)
                                {
                                    parentType = isDefaultParentExist.parent_layer_name.ToUpper();
                                }
                            } 

                            // parentType = string.IsNullOrEmpty(dr[parentTypeMapping.TemplateColName].ToString().ToUpper()) ? "Province" : dr[parentTypeMapping.TemplateColName].ToString().ToUpper();


                            string regularExpression = GetRegularExpression(layerMapping, parentType, entityType);
                            string network_id = dr[networkIdmapping.TemplateColName].ToString().ToUpper();
                            // Handle the full network id scnerio and update the required network id section into datatable
                            if (entityType != "Customer")
                            {

                                if (!string.IsNullOrEmpty(network_id))
                                {
                                network_id = network_id.Split('-').Last();
                                dr[networkIdmapping.TemplateColName] = network_id;
                                bool isValidParent = false;
                                var match = Regex.Match(network_id, regularExpression);
                                isValidParent = match.Success;
                                if (string.IsNullOrEmpty(regularExpression)) { isValidParent = false; }
                                if (!isValidParent && string.IsNullOrEmpty(regularExpression))
                                {
                                    network_id = network_id.Split('-').Last();
                                    dr[networkIdmapping.TemplateColName] = network_id;

                                    match = Regex.Match(network_id, regularExpression);
                                    if (!match.Success)
                                    {
                                        dr["is_valid"] = false;
                                        dr["error_msg"] = "Invalid Parent Type;";
                                    }
                                    else if (!string.IsNullOrEmpty(network_id) && !(match.Success && match.Value.Length == network_id.Length))
                                    {
                                        dr["is_valid"] = false;
                                        dr["error_msg"] = "Invalid Network ID: " + network_id + ";";
                                    }
                                }
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                errorMessage.status = StatusCodes.FAILED.ToString();
                errorMessage.error_msg = ex.Message;
                ErrorLogHelper.WriteErrorLog("DataValidator", "CheckNetworkIDFormat", ex);
            }
            return errorMessage;
        }

        public static string GetRegularExpression(List<ParentChildLayerMapping> layerMapping, string parentType, string entityType)
        {
            try
            {
                ParentChildLayerMapping objMappedDetails = layerMapping.Where(m => string.Equals(m.child_layer_name, entityType, StringComparison.CurrentCultureIgnoreCase) && string.Equals(m.parent_layer_name, parentType, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                string regEx = string.Empty;
                //ADBxxxxBCDnnnn
                if (objMappedDetails != null)
                {
                    foreach (char c in objMappedDetails.network_code_format)
                    {
                        bool isUpper = char.IsUpper(c);
                        if (isUpper)
                        {
                            // regEx += "[A-Z]{1}";
                            regEx += "[" + c + "]{1}";
                        }
                        else if (c.ToString() == "x")
                        {
                            regEx += "[A-Z0-9]{1}";
                        }
                        else
                        {
                            regEx += "[0-9]{1}";
                        }
                    }
                }
                return regEx;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "GetRegularExpression", ex);
                throw;
            }
        }

        public static ErrorMessage ValidateData(UploadSummary summary, DataTable dataTable, List<Mapping> lstMapping)
        {
            ErrorMessage status = new ErrorMessage();
            try
            {

                // validate uploaded excel column with template mapping...
                status = CommonUtility.validateTemplateColumn(lstMapping, dataTable);
                if (string.IsNullOrWhiteSpace(status.error_msg))
                {
                    status.error_code = StatusCodes.OK.ToString();
                    status.error_msg = "";
                    status.status = StatusCodes.OK.ToString();
                }
                else
                {
                    new BLDataUploader().UpdateFailedStatus(summary, status);
                    status.status = StatusCodes.INVALID_INPUTS.ToString();
                    status.error_code = StatusCodes.FAILED.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "ValidateData", ex);
            }
            return status;
        }
        internal static List<T> GetObjectList<T>(DataTable dataTable, UploadSummary summary)
        {
            return ConvertDataTableToList<T>(dataTable, summary);
        }
        public static List<T> ConvertDataTableToList<T>(DataTable dt, UploadSummary summary)
        {
            try
            {
                List<Mapping> lstMapping = new BLDataUploader().GetMappings(summary.entity_type);
                List<T> data = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row, lstMapping);
                    data.Add(item);
                }
                return data;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "ConvertDataTableToList", ex);
                throw;
            }

        }
        private static T GetItem<T>(DataRow dr, List<Mapping> lstMapping)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            try
            {
                string[] arrIncludeProperties = { "is_valid", "error_msg", "row_order" };

                foreach (DataColumn column in dr.Table.Columns)
                {
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {

                        string colName = string.Empty;
                        string actualColumnName = string.Empty;
                        Mapping mapping = lstMapping.Where(m => m.DbColName.ToLower() == pro.Name.ToLower()).FirstOrDefault();

                        if (mapping == null)
                        {
                            if (Array.Exists(arrIncludeProperties, m => m == pro.Name.ToLower()))
                            {
                                actualColumnName = column.ColumnName;
                                colName = column.ColumnName;
                            }
                        }
                        else
                        {
                            actualColumnName = mapping.TemplateColName;
                            colName = mapping.DbColName;

                            if (!dr.Table.Columns.Contains(actualColumnName))
                            {
                                actualColumnName = colName;
                            }
                        }
                        if (pro.Name.ToLower() == colName.ToLower() && dr.Table.Columns.Contains(actualColumnName))
                        {
                            //Use Switch Case here
                            if (pro.PropertyType.Name == "Double")
                            {
                                double douValue = 0;
                                double.TryParse(dr[actualColumnName].ToString(), out douValue);
                                pro.SetValue(obj, douValue, null);
                            }
                            else if (pro.PropertyType.Name == "Decimal")
                            {
                                decimal douValue = 0;
                                decimal.TryParse(dr[actualColumnName].ToString(), out douValue);
                                pro.SetValue(obj, douValue, null);
                            }
                            else if (pro.PropertyType.Name == "Boolean")
                            {
                                //if (dr[actualColumnName].ToString().ToUpper() == "YES")
                                //    dr[actualColumnName] = true.ToString();
                                //else if (dr[actualColumnName].ToString().ToUpper() == "NO")
                                //    dr[actualColumnName] = false.ToString();

                                if (!string.IsNullOrEmpty(dr[actualColumnName].ToString().Trim()))
                                {
                                    if (dr[actualColumnName].ToString().ToUpper() == "YES" || (dr[actualColumnName].ToString().ToUpper() == "TRUE"))
                                    {
                                        dr[actualColumnName] = true.ToString();
                                    }
                                    else if (dr[actualColumnName].ToString().ToUpper() == "NO" || (dr[actualColumnName].ToString().ToUpper() == "FALSE"))
                                    {
                                        dr[actualColumnName] = false.ToString();
                                    }
                                    else
                                    {
                                        dr[actualColumnName] = false.ToString();
                                    }
                                }
                                else
                                {
                                    dr[actualColumnName] = false.ToString();
                                }

                                // pro.SetValue(obj, Convert.ToBoolean(dr[actualColumnName]), null);
                                // pro.SetValue(obj, Convert.ToBoolean(string.IsNullOrEmpty(dr[actualColumnName].ToString()) == true ? false : Convert.ToString(dr[actualColumnName]).ToUpper() != "YES" || Convert.ToString(dr[actualColumnName]).ToUpper() != "NO" ? false: Convert.ToBoolean(dr[actualColumnName])), null);

                                pro.SetValue(obj, Convert.ToBoolean(string.IsNullOrEmpty(dr[actualColumnName].ToString()) == true ? false : Convert.ToBoolean(dr[actualColumnName])), null);
                            }
                            else if (pro.PropertyType.Name == "String")
                            {
                                pro.SetValue(obj, Convert.ToString(dr[actualColumnName]), null);
                            }
                            else if (pro.PropertyType.Name == "DateTime")
                            {
                                DateTime dateValue = DateTimeHelper.Now;
                                DateTime.TryParse(dr[actualColumnName].ToString(), out dateValue);
                                pro.SetValue(obj, Convert.ToDateTime(dr[actualColumnName]), null);
                            }

                            else if (pro.PropertyType.Name == "Int32")
                            {
                                // pro.SetValue(obj, Convert.ToInt32(dr[actualColumnName]), null);

                                int douValue = 0;
                                int.TryParse(dr[actualColumnName].ToString(), out douValue);
                                pro.SetValue(obj, douValue, null);
                            }
                            else if (pro.PropertyType.Name == "Nullable`1" && !mapping.IsNullable)
                            {
                                if (pro.PropertyType == typeof(Nullable<double>))
                                {
                                    double douValue = 0;
                                    double.TryParse(dr[actualColumnName].ToString(), out douValue);
                                    pro.SetValue(obj, douValue, null);
                                }
                                else if (pro.PropertyType == typeof(Nullable<decimal>))
                                {
                                    decimal douValue = 0;
                                    decimal.TryParse(dr[actualColumnName].ToString(), out douValue);
                                    pro.SetValue(obj, douValue, null);
                                }
                                else if (pro.PropertyType == typeof(Nullable<bool>))
                                {
                                    //if (dr[actualColumnName].ToString().ToUpper() == "YES")
                                    //    dr[actualColumnName] = true.ToString();
                                    //else if (dr[actualColumnName].ToString().ToUpper() == "NO")
                                    //    dr[actualColumnName] = false.ToString();

                                    if (!string.IsNullOrEmpty(dr[actualColumnName].ToString().Trim()))
                                    {
                                        if (dr[actualColumnName].ToString().ToUpper() == "YES" || (dr[actualColumnName].ToString().ToUpper() == "TRUE"))
                                        {
                                            dr[actualColumnName] = true.ToString();
                                        }
                                        else if (dr[actualColumnName].ToString().ToUpper() == "NO" || (dr[actualColumnName].ToString().ToUpper() == "FALSE"))
                                        {
                                            dr[actualColumnName] = false.ToString();
                                        }
                                        else
                                        {
                                            dr[actualColumnName] = false.ToString();
                                        }
                                    }
                                    else
                                    {
                                        dr[actualColumnName] = false.ToString();
                                    }

                                    pro.SetValue(obj, Convert.ToBoolean(string.IsNullOrEmpty(dr[actualColumnName].ToString()) == true ? false : Convert.ToBoolean(dr[actualColumnName])), null);
                                }
                                else if (pro.PropertyType == typeof(Nullable<Int32>))
                                {
                                    int douValue = 0;
                                    int.TryParse(dr[actualColumnName].ToString(), out douValue);
                                    pro.SetValue(obj, douValue, null);
                                    //pro.SetValue(obj, Convert.ToInt32(dr[actualColumnName]), null);
                                }
                                else if (pro.PropertyType == typeof(Nullable<DateTime>))
                                {
                                    if (!string.IsNullOrEmpty(dr[actualColumnName].ToString()))
                                    {
                                        DateTime douValue;
                                        DateTime.TryParse(dr[actualColumnName].ToString(), out douValue);
                                        //douValue = Convert.ToDateTime(douValue);
                                        if (douValue.ToShortDateString() == "1/1/0001")
                                        {
                                            pro.SetValue(obj, null, null);
                                        }
                                        else
                                        {
                                            pro.SetValue(obj, douValue, null);
                                        }
                                    }
                                    else
                                    {
                                        pro.SetValue(obj, null, null);
                                    }
                                }
                                else { pro.SetValue(obj, null, null); }

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dr[actualColumnName])))
                                {
                                    int douValue = 0;
                                    int.TryParse(dr[actualColumnName].ToString(), out douValue);
                                    pro.SetValue(obj, douValue, null);
                                }
                                else
                                {
                                    //pro.SetValue(obj, !string.IsNullOrEmpty(Convert.ToString(dr[actualColumnName])) ? dr[actualColumnName] : null, null);
                                    pro.SetValue(obj, null, null);
                                }
                            }
                        }
                        else
                            continue;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "GetItem", ex);
                throw ex;
            }
            return obj;
        }
        internal static List<T> GetObjectList<T>(DataTable dataTable)
        {
            return ConvertDataTableToList<T>(dataTable);
        }
        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            try
            {
                List<T> data = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row);
                    data.Add(item);
                }
                return data;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "ConvertDataTableToList", ex);
                throw;
            }


        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            try
            {
                foreach (DataColumn column in dr.Table.Columns)
                {
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (pro.Name == column.ColumnName)
                        {
                            //Use Switch Case here
                            if (pro.PropertyType.Name == "Double")
                            {
                                double douValue = 0;
                                double.TryParse(dr[column.ColumnName].ToString(), out douValue);
                                pro.SetValue(obj, douValue, null);
                            }
                            else if (pro.PropertyType.Name == "Decimal")
                            {
                                decimal douValue = 0;
                                decimal.TryParse(dr[column.ColumnName].ToString(), out douValue);
                                pro.SetValue(obj, douValue, null);
                            }
                            else if (pro.PropertyType.Name == "Boolean")
                            {
                                pro.SetValue(obj, Convert.ToBoolean(dr[column.ColumnName]), null);
                            }
                            else if (pro.PropertyType.Name == "String")
                            {
                                pro.SetValue(obj, Convert.ToString(dr[column.ColumnName]), null);
                            }
                            else if (pro.PropertyType.Name == "DateTime")
                            {
                                DateTime dateValue = DateTimeHelper.Now;
                                DateTime.TryParse(dr[column.ColumnName].ToString(), out dateValue);
                                pro.SetValue(obj, Convert.ToDateTime(dr[column.ColumnName]), null);
                            }
                            else if (pro.PropertyType.Name == "Int32")
                            {
                                pro.SetValue(obj, Convert.ToInt32(dr[column.ColumnName]), null);
                            }
                            else
                            {
                                pro.SetValue(obj, dr[column.ColumnName], null);
                            }
                        }
                        else
                            continue;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "GetItem", ex);
                throw ex;
            }
            return obj;
        }
        private static ErrorMessage DuplicateRecords(List<Mapping> lstMapping, DataTable dataTable, layerDetail networkLayerDetails, ErrorMessage errorMessage)
        {
            try
            {
                if ((networkLayerDetails.layer_name).ToUpper() != "LANDBASE")
                {
                    //if (networkLayerDetails.network_id_type == "M")
                    //{

                    // ---------------------- old code for finding duplicate network_id.... Commented on 16/07/2021 by Abhimanyu --- //
                    //Mapping networkIdmapping = lstMapping.Where(m => m.DbColName.ToLower() == "network_id").First();
                    //var duplicates = dataTable.AsEnumerable().Select(dr => dr.Field<string>(networkIdmapping.TemplateColName)).GroupBy(x => x)
                    //               .Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                    ////Set DataTable to Duplicate Record Here
                    //foreach (string strDuplicate in duplicates)
                    //{
                    //    DataRow[] duplicateRow = dataTable.Select(networkIdmapping.TemplateColName + "='" + strDuplicate + "'");
                    //    foreach (DataRow dr in duplicateRow)
                    //    {
                    //        dr["is_valid"] = false;
                    //        dr["error_msg"] = "Excel has duplicate " + networkIdmapping.TemplateColName + ": " + strDuplicate;
                    //    }
                    //}


                    Mapping networkIdmapping = lstMapping.Where(m => m.DbColName.ToLower() == "network_id").First();
                    Mapping parentnetworkIdmapping = lstMapping.Where(m => (m.DbColName.ToLower() == "parent_network_id")).First();
                    if (dataTable.Columns.Contains(networkIdmapping.TemplateColName.ToLower()) && dataTable.Columns.Contains(parentnetworkIdmapping.TemplateColName.ToLower()))
                    {
                        var duplicates = dataTable.AsEnumerable().Select(dr => new DuplicateNetworkIdCheck { network_id = dr.Field<string>(networkIdmapping.TemplateColName), parent_network_id = dr.Field<string>(parentnetworkIdmapping.TemplateColName) }).Where(y => (string.IsNullOrEmpty(y.network_id)) == false && (string.IsNullOrEmpty(y.parent_network_id)) == false).GroupBy(x => new { x.network_id, x.parent_network_id }).Where(g => g.Count() > 1).Select(g => g.Key).ToArray().ToList();
                        //Set DataTable to Duplicate Record Here
                        foreach (var item in duplicates)
                        {
                            //string _sqlWhere = "" + networkIdmapping.TemplateColName + " = '" + item.network_id + "'  and " + parentnetworkIdmapping.TemplateColName + " = '" + item.parent_network_id + "'";  
                            // DataRow[] duplicateRow = dataTable.Select(_sqlWhere);

                            var duplicateRow = from myRow in dataTable.AsEnumerable()
                                               where myRow.Field<string>(networkIdmapping.TemplateColName) == item.network_id && myRow.Field<string>(parentnetworkIdmapping.TemplateColName) == item.parent_network_id
                                               select myRow;
                            foreach (DataRow dr in duplicateRow)
                            {
                                dr["is_valid"] = false;
                                dr["error_msg"] = "Excel has duplicate " + networkIdmapping.TemplateColName + ": " + item.network_id;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage.status = StatusCodes.FAILED.ToString();
                errorMessage.error_msg = ex.Message;
                ErrorLogHelper.WriteErrorLog("DataValidator", "DuplicateRecords", ex);
            }
            return errorMessage;
        }
        private static ErrorMessage ValidateFieldsData(List<Mapping> lstMapping, DataTable dataTable, layerDetail networkLayerDetails, List<ParentChildLayerMapping> layerMapping, string entityType)
        {
            try
            {

                //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                //stopwatch.Start();
                ErrorMessage errorMessage = new ErrorMessage();
                errorMessage.status = StatusCodes.OK.ToString();
                errorMessage = validateMandatoryColumn(lstMapping, dataTable, networkLayerDetails, errorMessage);
                if (errorMessage.status == StatusCodes.OK.ToString())
                {
                    errorMessage = DuplicateRecords(lstMapping, dataTable, networkLayerDetails, errorMessage);
                }

                //stopwatch.Stop();
                //TimeSpan timeSpan = stopwatch.Elapsed;
                //string timeTaken = string.Format("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                //ErrorLogHelper.WriteErrorLog("validateMandatoryColumn Executes in:", timeTaken, null);

                //stopwatch.Start();
                //Data Type and Length of Column Checking
                if (errorMessage.status == StatusCodes.OK.ToString())
                {
                    errorMessage = validateDataTypeAndLength(lstMapping, dataTable, errorMessage);
                }
                //stopwatch.Stop();
                //timeSpan = stopwatch.Elapsed;
                //timeTaken = string.Format("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                //ErrorLogHelper.WriteErrorLog("validateDataTypeAndLength Executes in:", timeTaken, null);

                //stopwatch.Start();
                //Checking the Network ID format given in Excel is valid or not
                if (errorMessage.status == StatusCodes.OK.ToString() && entityType.ToUpper() != "LANDBASE")
                {
                    errorMessage = CheckNetworkIDFormat(lstMapping, dataTable, layerMapping, errorMessage, entityType);
                }
                //stopwatch.Stop();
                //timeSpan = stopwatch.Elapsed;
                //timeTaken = string.Format("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                //ErrorLogHelper.WriteErrorLog("CheckNetworkIDFormat Executes in:", timeTaken, null);

                //stopwatch.Start();
                //Checking the value of Drop Down list in Excel is valid or not
                if (errorMessage.status == StatusCodes.OK.ToString())
                {
                    errorMessage = validateDropDownColumn(lstMapping, dataTable, networkLayerDetails, errorMessage);
                }
                //if (errorMessage.status == StatusCodes.OK.ToString())
                //{
                //    errorMessage = ValidateShaft_Floor_Name(lstMapping, dataTable, networkLayerDetails, errorMessage);
                //}
                return errorMessage;
                //stopwatch.Stop();
                //timeSpan = stopwatch.Elapsed;
                //timeTaken = string.Format("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                //ErrorLogHelper.WriteErrorLog("validateDropDownColumn Executes in:", timeTaken, null);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "ValidateFieldsData", ex);
                throw;
            }
        }

        public static ErrorMessage validateDataTypeAndLength(List<Mapping> lstMapping, DataTable dt, ErrorMessage errorMessage)
        {
            try
            {
                DataRow[] drRemainingValidRecords = dt.Select("is_valid=" + true);
                foreach (DataRow dr in drRemainingValidRecords)
                {
                    string strMessge = "Invalid Values for columns : ";
                    string dbMandatoryColName = string.Empty;
                    bool isInValid = false;
                    foreach (Mapping mandatoryField in lstMapping)
                    {

                        if (dt.Columns.Contains(mandatoryField.TemplateColName))
                        {
                            string strCellValue = dr[mandatoryField.TemplateColName].ToString();
                            if (mandatoryField.DBColumnDataType == "int4" && mandatoryField.IsNullable)
                            {
                                if (string.IsNullOrEmpty(strCellValue))
                                {
                                    dr[mandatoryField.TemplateColName] = null;
                                }
                                else
                                {
                                    int intVal = 0;
                                    bool isConvertd = int.TryParse(strCellValue, out intVal);
                                    if (!isConvertd)
                                    {
                                        isInValid = true;
                                        dbMandatoryColName += mandatoryField.TemplateColName + " ,";
                                    }
                                    else if (!string.IsNullOrEmpty(strCellValue) && (strCellValue.Trim().Length > Convert.ToInt32(mandatoryField.CharacterMaxLength)))
                                    {
                                        isInValid = true;
                                        dbMandatoryColName += "Invalid Length for column: " + mandatoryField.TemplateColName
                                            + " ,Length must be less than " + mandatoryField.CharacterMaxLength + ",";
                                        dr[mandatoryField.TemplateColName] = strCellValue;
                                    }
                                    
                                    else if (!string.IsNullOrEmpty(strCellValue) && !string.IsNullOrEmpty(Convert.ToString(mandatoryField.min_value)) && Convert.ToInt32(mandatoryField.min_value) > Convert.ToInt32(strCellValue))
                                        {
                                            isInValid = true;
                                            dbMandatoryColName +=  mandatoryField.TemplateColName
                                                + " must be greater than " + mandatoryField.min_value + ",";
                                            dr[mandatoryField.TemplateColName] = strCellValue;
                                        }
                                    else if (!string.IsNullOrEmpty(strCellValue) &&  !string.IsNullOrEmpty(Convert.ToString(mandatoryField.max_value)) && Convert.ToInt32(mandatoryField.max_value) < Convert.ToInt32(strCellValue))
                                        {
                                            isInValid = true;
                                            dbMandatoryColName +=  mandatoryField.TemplateColName
                                                + " must be less than " + mandatoryField.max_value + ",";
                                            dr[mandatoryField.TemplateColName] = strCellValue;
                                        }
                                    
                                }
                            }
                            else if (mandatoryField.DBColumnDataType == "int4" && mandatoryField.IsNullable == false)
                            {
                                int intVal = 0;
                                bool isConvertd = int.TryParse(strCellValue, out intVal);
                                if (!isConvertd)
                                {
                                    isInValid = true;
                                    dbMandatoryColName += mandatoryField.TemplateColName + " ,";
                                }
                                else if (!string.IsNullOrEmpty(strCellValue) && (strCellValue.Trim().Length > Convert.ToInt32(mandatoryField.CharacterMaxLength)))
                                {
                                    isInValid = true;
                                    dbMandatoryColName += "Invalid Length for column: " + mandatoryField.TemplateColName
                                        + " ,Length must be less than " + mandatoryField.CharacterMaxLength + ",";
                                    dr[mandatoryField.TemplateColName] = strCellValue;
                                }
                                else if (!string.IsNullOrEmpty(strCellValue) && !string.IsNullOrEmpty(Convert.ToString(mandatoryField.min_value)) && !string.IsNullOrEmpty(Convert.ToString(mandatoryField.max_value)))
                                {
                                    if (Convert.ToInt32(mandatoryField.min_value) > Convert.ToInt32(strCellValue))
                                    {
                                        isInValid = true;
                                        dbMandatoryColName += mandatoryField.TemplateColName
                                            + " must be greater than " + mandatoryField.min_value + ",";
                                        dr[mandatoryField.TemplateColName] = strCellValue;
                                    }
                                    else if (Convert.ToInt32(mandatoryField.max_value) < Convert.ToInt32(strCellValue))
                                    {
                                        isInValid = true;
                                        dbMandatoryColName +=  mandatoryField.TemplateColName
                                            + " must be less than " + mandatoryField.max_value + ",";
                                        dr[mandatoryField.TemplateColName] = strCellValue;
                                    }
                                }
                            }
                            else if (mandatoryField.DBColumnDataType == "float8" && mandatoryField.IsNullable)
                            {
                                if (string.IsNullOrEmpty(strCellValue))
                                {
                                    dr[mandatoryField.TemplateColName] = null;
                                }
                                else
                                {
                                    double douVal = 0;
                                    bool isConvertd = double.TryParse(strCellValue, out douVal);
                                    if (!isConvertd)
                                    {
                                        isInValid = true;
                                        dbMandatoryColName += mandatoryField.TemplateColName + " ,";
                                    }
                                    else if (!string.IsNullOrEmpty(strCellValue) && (strCellValue.Trim().Length > Convert.ToInt32(mandatoryField.CharacterMaxLength)))
                                    {
                                        isInValid = true;
                                        dbMandatoryColName += "Invalid Length for column: " + mandatoryField.TemplateColName
                                            + " ,Length must be less than " + mandatoryField.CharacterMaxLength + ",";
                                        dr[mandatoryField.TemplateColName] = strCellValue;
                                    }
                                    else if (!string.IsNullOrEmpty(strCellValue) && !string.IsNullOrEmpty(Convert.ToString(mandatoryField.min_value)) && !string.IsNullOrEmpty(Convert.ToString(mandatoryField.max_value)))
                                    {
                                        if (Convert.ToInt32(mandatoryField.min_value) > Convert.ToDecimal(strCellValue))
                                        {
                                            isInValid = true;
                                            dbMandatoryColName += mandatoryField.TemplateColName
                                                + " must be greater than " + mandatoryField.min_value + ",";
                                            dr[mandatoryField.TemplateColName] = strCellValue;
                                        }
                                        else if (Convert.ToInt32(mandatoryField.max_value) < Convert.ToDecimal(strCellValue))
                                        {
                                            isInValid = true;
                                            dbMandatoryColName +=  mandatoryField.TemplateColName
                                                + " must be less than " + mandatoryField.max_value + ",";
                                            dr[mandatoryField.TemplateColName] = strCellValue;
                                        }
                                    }
                                }

                            }
                            else if (mandatoryField.DBColumnDataType == "float8" && mandatoryField.IsNullable == false)
                            {
                                double douVal = 0;
                                ErrorLogHelper.WriteErrorLog("DataValidator strCellValue 4" + strCellValue, "validateDataTypeAndLength", null);

                                bool isConvertedInDouble = double.TryParse(strCellValue, out douVal);
                                string pattern = @"[-+]?\d+(\.\d+)?[eE][-+]?\d+";
                                bool IsScientiticFloat = Regex.IsMatch(strCellValue, pattern);
                                if (IsScientiticFloat)
                                {
                                    isInValid = true;
                                    strMessge = "Scientific Float";

                                }
                                else
                                {
                                    if (!isConvertedInDouble)
                                    {
                                        isInValid = true;
                                        dbMandatoryColName += mandatoryField.TemplateColName + " ,";
                                        dr[mandatoryField.TemplateColName] = strCellValue;
                                    }
                                    else if (!string.IsNullOrEmpty(strCellValue) && (strCellValue.Trim().Length > Convert.ToInt32(mandatoryField.CharacterMaxLength)))
                                    {
                                        isInValid = true;
                                        dbMandatoryColName += "Invalid Length for column: " + mandatoryField.TemplateColName
                                            + " ,Length must be less than" + mandatoryField.CharacterMaxLength + ",";
                                        dr[mandatoryField.TemplateColName] = strCellValue;
                                    }
                                    else if (!string.IsNullOrEmpty(strCellValue) && !string.IsNullOrEmpty(Convert.ToString(mandatoryField.min_value)) && !string.IsNullOrEmpty(Convert.ToString(mandatoryField.max_value)))
                                    {
                                        if (Convert.ToInt32(mandatoryField.min_value) > Convert.ToDecimal(strCellValue))
                                        {
                                            isInValid = true;
                                            dbMandatoryColName += mandatoryField.TemplateColName
                                                + " must be greater than " + mandatoryField.min_value + ",";
                                            dr[mandatoryField.TemplateColName] = strCellValue;
                                        }
                                        else if (Convert.ToInt32(mandatoryField.max_value) < Convert.ToDecimal(strCellValue))
                                        {
                                            isInValid = true;
                                            dbMandatoryColName += mandatoryField.TemplateColName
                                                + " must be less than " + mandatoryField.max_value + ",";
                                            dr[mandatoryField.TemplateColName] = strCellValue;
                                        }
                                    }


                                }
                            }
                            else if (mandatoryField.DBColumnDataType == "varchar")
                            {
                                if (strCellValue.Trim().Length > Convert.ToInt32(mandatoryField.CharacterMaxLength))
                                {
                                    isInValid = true;
                                    dbMandatoryColName += "Invalid Length for column: " + mandatoryField.TemplateColName
                                        + " ,Length must be less than " + mandatoryField.CharacterMaxLength + ",";
                                    dr[mandatoryField.TemplateColName] = strCellValue;
                                }
                            }
                            else if (mandatoryField.DBColumnDataType == "bool")
                            {
                                if (strCellValue.ToUpper() == "YES" || strCellValue.ToUpper() == "TRUE")
                                    strCellValue = true.ToString();
                                else if (strCellValue.ToUpper() == "NO" || strCellValue.ToUpper() == "FALSE" || string.IsNullOrEmpty(strCellValue))
                                    strCellValue = false.ToString();
                                else
                                {
                                    isInValid = true;
                                    dbMandatoryColName = mandatoryField.TemplateColName + " ,";
                                }
                            }
                            //else if (mandatoryField.UdtName == "timestamp")
                            //{

                            //}

                            //if (string.IsNullOrEmpty(strCellValue))
                            //{
                            //    isInValid = true;
                            //    dbMandatoryColName += mandatoryField.TemplateColName + ",";
                            //}
                        }
                    }
                    if (isInValid)
                    {
                        dr["error_msg"] += strMessge + dbMandatoryColName.Trim().TrimEnd(',');
                        dr["is_valid"] = false;
                    }
                    else
                    {
                        dr["is_valid"] = true;
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage.status = StatusCodes.FAILED.ToString();
                errorMessage.error_msg = ex.Message;
                ErrorLogHelper.WriteErrorLog("DataValidator", "validateDataTypeAndLength", ex);
            }
            return errorMessage;
        }


        private static ErrorMessage validateDropDownColumn(List<Mapping> lstMapping, DataTable dt, layerDetail networkLayer, ErrorMessage errorMessage)
        {
            try
            {
                List<Mapping> lstDropDownMapping = lstMapping.Where(m => m.IsDropDown == true).ToList();
                if (lstDropDownMapping.Count > 0)
                {
                    var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                    object lockObj = new object();
                    DataRow[] drRemainingValidRecords = dt.Select("is_valid=" + true);
                    List<KeyValueDropDown> lstVendor = BLItemTemplate.Instance.GetAllVendorList().ToList();

                    var lstentity_dropdown = new BLMisc().GetDropDownList(networkLayer.layer_name);
                    List<KeyValueDropDown> lstSpecifications = new List<KeyValueDropDown>();

                    string strMessge = "Invalid value of field : ";
                    List<string> lstIteratedspecification = new List<string>();
                    List<string> lstIteratedVendor = new List<string>();
                    Parallel.ForEach(drRemainingValidRecords, options, (dr) =>
                    {
                        lock (lockObj)
                        {
                            string dbDropdownColName = string.Empty;
                            bool isInValid = false;
                            string recordvalue = string.Empty;
                            string dbColName = string.Empty;
                            //Check Specification
                            if (networkLayer.layer_name.ToUpper() != "LANDBASE" && networkLayer.layer_name.ToUpper() != "BUILDING" && networkLayer.layer_name.ToUpper() != "UNIT")
                            {
                                if (dt.Columns.Contains("specification"))
                                {
                                    string specification = string.IsNullOrEmpty(Convert.ToString(dr["specification"])) ? "" : Convert.ToString(dr["specification"]);
                                    KeyValueDropDown keyValueSpecification = lstSpecifications.Find(x => x.key.Equals(specification, StringComparison.OrdinalIgnoreCase));
                                    if (!lstIteratedspecification.Contains(specification))
                                    {
                                        if (keyValueSpecification == null)
                                        {
                                            lstIteratedspecification.Add(specification);
                                            List<KeyValueDropDown> _lstSpecifications = BLItemTemplate.Instance.GetItemSpecification(networkLayer.layer_name, 0, 0, specification).ToList();
                                            lstSpecifications.AddRange(_lstSpecifications);
                                            lstSpecifications = lstSpecifications.Union(_lstSpecifications).Distinct().ToList();
                                        }
                                    }
                                }
                            }

                            if (networkLayer.layer_name.ToUpper() == "LANDBASE")
                            {
                                lstDropDownMapping = lstMapping.Where(m => m.IsDropDown == true && m.IsMandatory == true).ToList();
                            }

                            foreach (Mapping mandatoryField in lstDropDownMapping)
                            {
                                //if (lstDropDownMapping.Contains(dr[mandatoryField.TemplateColName]))
                                //{
                                //}

                                //recordvalue = Convert.ToString(dr[mandatoryField.TemplateColName]);


                                if (!dt.Columns.Contains(mandatoryField.TemplateColName))
                                {
                                    if (dt.Columns.Contains(mandatoryField.DbColName))
                                    {
                                        recordvalue = Convert.ToString(dr[mandatoryField.DbColName]);
                                    }
                                }
                                else if (dt.Columns.Contains(mandatoryField.TemplateColName))
                                {
                                    recordvalue = Convert.ToString(dr[mandatoryField.TemplateColName]);
                                }

                                dbColName = Convert.ToString(mandatoryField.DbColName);

                                if (dt.Columns.Contains(mandatoryField.TemplateColName))
                                {
                                    switch (dbColName)
                                    {
                                        case "specification":
                                            if (lstSpecifications.Count(x => x.ddtype.Equals(dbColName, StringComparison.OrdinalIgnoreCase) && x.key.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "vendor_name":
                                            if (lstVendor.Count(x => x.ddtype.Equals(dbColName, StringComparison.OrdinalIgnoreCase) && x.value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "entity_category":
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals("Entity_Type", StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "category":
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals(dbColName, StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "sub_category":
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals("SubCategory", StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "classification":
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals(dbColName, StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "layer_type":
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals("Layer Type", StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "rfs_status":
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals(dbColName, StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "tenancy":
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals(dbColName, StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        case "unit_type":
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals("Unit_Type", StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                        default:
                                            if (lstentity_dropdown.Count(x => x.dropdown_type.Equals(dbColName, StringComparison.OrdinalIgnoreCase) && x.dropdown_value.Equals(recordvalue, StringComparison.OrdinalIgnoreCase)) == 0)
                                            {
                                                isInValid = true;
                                                dbDropdownColName += mandatoryField.TemplateColName + ",";
                                            }
                                            break;
                                    }
                                }
                            }
                            if (isInValid)
                            {
                                dr["error_msg"] += strMessge + dbDropdownColName.TrimEnd(',');
                                dr["is_valid"] = false;
                            }
                            else
                            {
                                dr["is_valid"] = true;
                            }
                        }
                    });

                }
            }
            catch (Exception ex)
            {
                errorMessage.status = StatusCodes.FAILED.ToString();
                errorMessage.error_msg = ex.Message;
                ErrorLogHelper.WriteErrorLog("DataValidator", "validateDropDownColumn", ex);
            }
            return errorMessage;
        }

        private static ErrorMessage ValidateShaft_Floor_Name(List<Mapping> lstMapping, DataTable dataTable, layerDetail networkLayerDetails, ErrorMessage errorMessage)
        {
            try
            {
                if (dataTable != null)
                {
                    if (Convert.ToString(dataTable.Rows[0]["Parent_Entity_Type"]) == "Structure")
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dataTable.Rows[0]["Shaft_Name"])) && !string.IsNullOrEmpty(Convert.ToString(dataTable.Rows[0]["Floor_Name"])))
                        {
                            BLTempONT bLTempONT = new BLTempONT();
                            var result = bLTempONT.validateShaftFloorinfo(Convert.ToString(dataTable.Rows[0]["Shaft_Name"]), Convert.ToString(dataTable.Rows[0]["Floor_Name"]), Convert.ToString(dataTable.Rows[0]["Parent_Network_Id"]));
                            if (result == false)
                            {
                                //errorMessage.status = StatusCodes.FAILED.ToString();
                                errorMessage.error_msg = "Shaft and Floor name does not exist";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage.status = StatusCodes.FAILED.ToString();
                errorMessage.error_msg = ex.Message;
                ErrorLogHelper.WriteErrorLog("DataValidator", "Shaft and Floor name does not exist", ex);
            }
            return errorMessage;
        }

        public class DuplicateNetworkIdCheck
        {
            public string network_id { get; set; }
            public string parent_network_id { get; set; }
        }
        public static List<T> ConvertDataTableToListForCDB<T>(DataTable dt, UploadSummary summary)
        {
            try
            {
                List<Mapping> lstMapping = new BLDataUploader().GetMappingsForCDBCable(summary.entity_type);
                List<T> data = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row, lstMapping);
                    data.Add(item);
                }
                return data;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DataValidator", "ConvertDataTableToListForCDB", ex);
                throw;
            }
        }

    }
}
