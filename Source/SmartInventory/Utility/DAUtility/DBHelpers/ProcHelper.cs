using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utility.DAUtility.DBHelpers
{
    public static class ProcHelper 
    {
        #region POSTGRESQL
        /// <summary>
        /// GENERIC FUNCTIONS TO GET FINAL QUERY TO EXECUTE A POSTGRES PROCEDURE
        /// </summary>
        /// <param name="procName">NAME OF PROCEDURE</param>
        /// <param name="inputParams"> OBJECT OF INPUT PARAMETERS</param>
        /// <param name="finalQuery"> REF TYPE VARIABLE TO GET FINAL QUERY </param>
        /// <returns> LIST OF  NPGSQL PARAMTERS </returns>
        public static List<NpgsqlParameter> GetInputParamsWithFinalQuery(string procName,object inputParams, ref string finalQuery)
        {
            finalQuery = "select * from " + procName + "(";
            // PREPARE PARAMETERS LIST...
            List<NpgsqlParameter> lstParams = new List<NpgsqlParameter>();
            if (inputParams != null)
            {
                foreach (var prop in inputParams.GetType().GetProperties())
                {
                    // ADD PARAMETER TO LIST OBJECT.....
                    NpgsqlParameter param = new NpgsqlParameter("@" + prop.Name, GetPropValue(prop, inputParams));
                    param.NpgsqlDbType = getPostgresDbType(prop);
                    param.Direction = System.Data.ParameterDirection.Input;
                    lstParams.Add(param);

                    // ADD PARAMETER NAME TO QUERY...
                    finalQuery += "@" + prop.Name + ",";
                }
            }
            //TRIM COMMA FROM LAST...
            finalQuery = finalQuery.TrimEnd(',');
            // ADD BRACKET TO END THE QUERY..
            finalQuery = finalQuery + ");";
            return lstParams;
        }

        private static NpgsqlDbType getPostgresDbType(PropertyInfo propInfo)
        {
            var propName = propInfo.PropertyType.Name.ToUpper();

            if (propName == PropType.INT.ToString() || propName == PropType.INT32.ToString() || propName == PropType.INT64.ToString())
                return NpgsqlDbType.Integer;
            else if (propName == PropType.DOUBLE.ToString())
                return NpgsqlDbType.Double;
            else if (propName == PropType.DECIMAL.ToString())
                return NpgsqlDbType.Double;
            else if (propName == PropType.BOOL.ToString() || propName == PropType.BOOLEAN.ToString())
                return NpgsqlDbType.Boolean;
            else if (propName == PropType.DATETIME.ToString())
                return NpgsqlDbType.Timestamp;
            else if (propName == PropType.STRING.ToString())
                return NpgsqlDbType.Varchar;
            else
            {
                if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propName = propInfo.PropertyType.GetGenericArguments()[0].Name.ToUpper();
                    if (propName == PropType.INT.ToString() || propName == PropType.INT32.ToString() || propName == PropType.INT64.ToString())
                        return NpgsqlDbType.Integer;
                    else if (propName == PropType.DOUBLE.ToString())
                        return NpgsqlDbType.Double;
                    else if (propName == PropType.DECIMAL.ToString())
                        return NpgsqlDbType.Double;
                    else if (propName == PropType.BOOL.ToString() || propName == PropType.BOOLEAN.ToString())
                        return NpgsqlDbType.Boolean;
                    else if (propName == PropType.DATETIME.ToString())
                        return NpgsqlDbType.Timestamp;
                    else if (propName == PropType.STRING.ToString())
                        return NpgsqlDbType.Varchar;
                    else
                        return NpgsqlDbType.Varchar;
                }
                else
                {
                    return NpgsqlDbType.Varchar;
                }
            }
        }

        private static dynamic GetPropValue(PropertyInfo propInfo, object inputParams)
        {
            var propName = propInfo.PropertyType.Name.ToUpper();
            if (propName == PropType.STRING.ToString())
            {
                if (propInfo.GetValue(inputParams, null) == null)
                    return "";
                else
                    return propInfo.GetValue(inputParams, null);
            }
            else
            {
                return propInfo.GetValue(inputParams, null);
            }
        }

        public static List<T> ConvertJsonToObject<T>(List<string> lstJSON)
        {
            List<T> lstObjects = new List<T>();
            foreach (var objJSON in lstJSON)
            {
                lstObjects.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(objJSON));
            }
            return lstObjects;
        }

        #endregion
 
    }
    public enum PropType
    {
        INT,
        INT32,
        INT64,
        DOUBLE,
        DECIMAL,
        BOOL,
        BOOLEAN,
        DATETIME,
        DATE,
        STRING,
        FLOAT,
    }
}
