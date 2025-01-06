using DataAccess;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Script.Serialization;

namespace BusinessLogics
{
    public class BLConvertMLanguage
    {
        private BLConvertMLanguage()
        {

        }

        public static List<dynamic> MultilingualConvert(List<dynamic> list, string[] arrIgnoreColumns)
        {
            var currentLang = CultureInfo.CurrentUICulture;
            string culture = currentLang.Name;
            var keysHavingText = new List<dynamic>();
            var dict = new BLResources().GetResourceAuditList(culture);

            foreach (var dic in list)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();

                foreach (KeyValuePair<string, object> item in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == item.Key.ToUpper()))
                    {
                        var entry = dict.FirstOrDefault(e => e.key.ToString() == item.Key.ToUpper());

                        if (entry == null)
                        {
                            obj.Add(item.Key.ToString(), item.Value);
                        }
                        else
                        {
                            if (entry.key == item.Key.ToUpper())
                            {
                                if (arrIgnoreColumns.Count() > 0 && !Array.Exists(arrIgnoreColumns, m => m == item.Key.ToUpper()))
                                {
                                    obj.Add(entry.value.ToString(), item.Value);
                                }
                                else
                                {
                                    obj.Add(entry.value.ToString(), item.Value);
                                }
                            }
                        }
                    }

                }
                keysHavingText.Add(obj);
            }
            return keysHavingText;
        }
        public static List<Dictionary<string, string>> ExportMultilingualConvert(List<Dictionary<string, string>> list)
        {
            var currentLang = CultureInfo.CurrentUICulture;
            string culture = currentLang.Name;
            var keysHavingText = new List<Dictionary<string, string>>();
            var dict = new BLResources().GetResourceAuditList(culture);
            foreach (var dic in list)
            {
                var obj = new Dictionary<string, string>();

                foreach (var item in dic)
                {
                    var entry = dict.FirstOrDefault(e => e.key.ToString() == item.Key.ToUpper());
                    if (entry == null)
                    {
                        obj.Add(item.Key.ToString(), item.Value);
                    }
                    else
                    {
                        if (entry.key == item.Key.ToUpper())
                        {
                            obj.Add(entry.value.ToString(), item.Value);
                        }
                    }

                }
                keysHavingText.Add(obj);
            }
            return keysHavingText;
        }


        public static Dictionary<string, string> MultilingualConvertinfo(Dictionary<string, string> list)
        {
            var currentLang = CultureInfo.CurrentUICulture;
            string culture = currentLang.Name;
            var obj = new Dictionary<string, string>();
            var dict = new BLResources().GetResourceAuditList(culture);
            foreach (var dic in list)
            {



                var entry = dict.FirstOrDefault(e => e.key.ToString() == dic.Key);
                if (entry == null)
                {
                    obj.Add(dic.Key.ToString(), dic.Value);
                }
                else
                {
                    if (entry.key == dic.Key)
                    {
                        obj.Add(entry.value.ToString(), dic.Value);
                    }
                }



            }
            return obj;
        }

        public static List<WebGridColumn> GetEntityWiseColumns(int layer_id, string entity_name, string setting_type, string[] arrIgnoreColumns, int role_id, int user_id)
        {
            List<WebGridColumn> columns = new List<WebGridColumn>();
            List<WebGridColumns> lstofColumns = new BLLayer().GetEntityWiseColumns(layer_id, entity_name, setting_type, user_id, role_id);
            for (int i = 0; i < lstofColumns.Count; i++)
            {
                if (!Array.Exists(arrIgnoreColumns, m => m == lstofColumns[i].display_name.ToUpper()))
                {
                    columns.Add(new WebGridColumn()
                    {
                        ColumnName = lstofColumns[i].column_name,
                        Header = BLConvertMLanguage.MultilingualMessageConvert(lstofColumns[i].display_name.ToString()),
                        CanSort = true
                    });
                }
            }
            return columns;
        }

        public static List<WebGridColumn> GetLandbaseEntityWiseColumns(int layer_id, string[] arrIgnoreColumns)
        {
            List<WebGridColumn> columns = new List<WebGridColumn>();
            List<WebGridColumns> lstofColumns = new BLLayer().GetLandbaseEntityWiseColumns(layer_id);
            for (int i = 0; i < lstofColumns.Count; i++)
            {
                if (!Array.Exists(arrIgnoreColumns, m => m == lstofColumns[i].display_name.ToUpper()))
                {

                    if (lstofColumns[i].display_name.ToUpper() == "GEOMETRY")
                    {
                        columns.Add(new WebGridColumn()
                        {
                            ColumnName = "latitude",
                            Header = BLConvertMLanguage.MultilingualMessageConvert("Latitude"),
                            CanSort = true
                        });
                        columns.Add(new WebGridColumn()
                        {
                            ColumnName = "longitude",
                            Header = BLConvertMLanguage.MultilingualMessageConvert("Longitude"),
                            CanSort = true
                        });
                    }
                    else
                    {
                        columns.Add(new WebGridColumn()
                        {
                            ColumnName = lstofColumns[i].column_name,
                            Header = BLConvertMLanguage.MultilingualMessageConvert(lstofColumns[i].display_name.ToString()),
                            CanSort = true
                        });
                    }


                }
            }
            return columns;
        }
        public static string MultilingualMessageConvert(string message)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(message))
            {
                var Messages = message.Trim().Split('[', ']');
                foreach (var res in Messages)
                {
                    if (!string.IsNullOrEmpty(res))
                    {
                        var propertyDetail = typeof(Resources.Resources).GetProperty(res);
                        if (propertyDetail == null)
                        {
                            result += string.IsNullOrEmpty(result) ? res : " " + res;
                        }
                        else
                        {
                            result += string.IsNullOrEmpty(result) ? propertyDetail.GetValue(propertyDetail.Name) : " " + propertyDetail.GetValue(propertyDetail.Name);
                        }
                    }
                }
            }
            return result;
        }
        public static List<WebGridColumn> GetLandBaseEntityWiseColumns(int layer_id, string layer_name, string setting_type, string[] arrIgnoreColumns, int role_id, int user_id)
        {
            List<WebGridColumn> columns = new List<WebGridColumn>();
            List<WebGridColumns> lstofColumns = new BLLayer().GetLandBaseLayerWiseColumns(layer_id, layer_name, setting_type, user_id, role_id);
            for (int i = 0; i < lstofColumns.Count; i++)
            {
                if (!Array.Exists(arrIgnoreColumns, m => m == lstofColumns[i].display_name.ToUpper()))
                {
                    columns.Add(new WebGridColumn()
                    {
                        ColumnName = lstofColumns[i].column_name,
                        Header = BLConvertMLanguage.MultilingualMessageConvert(lstofColumns[i].display_name.ToString()),
                        CanSort = true
                    });
                }
            }
            return columns;
        }
        public static List<T> MultilingualConvertModel<T>(List<T> list, string[] arrIgnoreColumns, string culture = "en") where T : new()
        {
            var keysHavingText = new List<dynamic>();
            var dict = new BLResources().GetResourceAuditList(culture);
            List<dynamic> lstobj = new List<dynamic>();
            List<T> objT = new List<T>();
            var json = JsonConvert.SerializeObject(list);
            lstobj = new JavaScriptSerializer().Deserialize<List<dynamic>>(json);
            foreach (var dic in lstobj)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                foreach (KeyValuePair<string, object> item in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == item.Key.ToUpper()))
                    {
                        if (item.Value == null)
                            obj.Add(item.Key.ToString(), item.Value);
                        if (item.Value != null)
                        {

                            var entry = dict.FirstOrDefault(e => e.key.ToString() == item.Value.ToString());

                            if (entry == null)
                            {
                                obj.Add(item.Key.ToString(), item.Value);
                            }
                            else
                            {
                                if (entry.key == item.Value.ToString())
                                {
                                    if (arrIgnoreColumns.Count() > 0 && !Array.Exists(arrIgnoreColumns, m => m == item.Value.ToString().ToUpper()))
                                    {
                                        obj.Add(item.Key, entry.value.ToString());
                                    }
                                    else
                                    {
                                        obj.Add(item.Key, entry.value.ToString());
                                    }
                                }
                            }
                        }
                    }

                }

                keysHavingText.Add(obj);
            }
            var jsonNew = JsonConvert.SerializeObject(keysHavingText);
            return objT = new JavaScriptSerializer().Deserialize<List<T>>(jsonNew);
        }
    }
}

