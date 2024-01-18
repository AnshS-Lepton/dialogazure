using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBContext;
using Npgsql;
using System.Reflection;

namespace DataAccess
{
    class DbHelper
    {

        public static List<T> ExecutePostgresProcedure<T>(MainContext context, string ProcName, NpgsqlParameter[] inputParams, out int outValue)
        {
            return ExecuteQuery<T>(context, ProcName, inputParams, out outValue, true);
        }

        public static List<T> ExecutePostgresProcedure<T>(MainContext context, string ProcName, NpgsqlParameter[] inputParams)
        {
            int outValue;
            return ExecuteQuery<T>(context, ProcName, inputParams, out outValue);
        }
        public static List<T> ExecuteQuery<T>(MainContext context, string ProcName, NpgsqlParameter[] inputParams, out int outValue, bool outputParam = false)
        {
            List<T> lstResult = new List<T>();
            outValue = 0;
            var npgCon = new NpgsqlConnection(context.Database.Connection.ConnectionString);
            npgCon.Open();
            var tran = npgCon.BeginTransaction();
            try
            {
                #region MyRegion
                var command = new NpgsqlCommand(ProcName, npgCon);
                var inParam = inputParams.Where(x => x.Direction.Equals(ParameterDirection.Input)).ToArray();
                if (inParam != null)
                    command.Parameters.AddRange(inParam);
                var outParam = inputParams.Where(x => x.Direction.Equals(ParameterDirection.Output)).FirstOrDefault();
                if (outParam != null)
                    command.Parameters.Add(outParam);
                command.CommandType = CommandType.StoredProcedure;
                var dr = command.ExecuteReader();
                while (dr.HasRows)
                {
                    var dtEntityData = new DataTable();
                    if (dr.FieldCount > 1)
                    {
                        dtEntityData.Load(dr);
                        lstResult = DataTableToList<T>(dtEntityData);
                    }
                    else if (outputParam)
                    {
                        dtEntityData.Load(dr);
                        outValue = Convert.ToInt32(dtEntityData.Rows[0][0]);
                    }
                }


                #endregion
            }
            catch (Exception ex)
            {
                if (tran != null)
                    tran.Rollback();
            }
            finally
            {
                npgCon.Close();
            }

            return lstResult;
        }

        public static List<T> DataTableToList<T>(DataTable dt)
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
                    if (pro.Name.ToLower() == column.ColumnName.ToLower() && dr[column.ColumnName] != DBNull.Value)
                        pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ? string.Empty : dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }





    }
}