using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Feasibility
{
    public class CommonHelpers
    {
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

        public static List<T> DataTableToList<T>(DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();
                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetRandomSereis(int Size)
        {
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string GetUniqueId(int maxSize, string maxSeqId, string prefixText, bool isZero)
        {
            int includeZeros = 0;
            includeZeros = (maxSize > maxSeqId.Length ? maxSize - maxSeqId.Length : maxSeqId.Length);
            string appendId = string.Empty;
            string setReturn = string.Empty;

            appendId = prefixText;
            if (includeZeros < maxSize)
            {
                for (int i = 0; i < includeZeros; i++)
                {
                    if (isZero)
                        appendId += "0";
                    else
                        appendId += "" + i + "";

                }
                setReturn = string.Concat(appendId, maxSeqId);
            }
            else
                setReturn = string.Concat(appendId, maxSeqId);

            return setReturn;
        }
    }
}
