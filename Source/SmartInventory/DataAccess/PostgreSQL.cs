using Newtonsoft.Json;
using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess
{
    public class PostgreSQL
    {
        public static string conString = Convert.ToBoolean(ConfigurationManager.AppSettings["ISEncryptedConnection"]) == true? MiscHelper.Decrypt(ConfigurationManager.AppSettings["constr"].Trim()):ConfigurationManager.ConnectionStrings["constr"].ConnectionString;       
        public delegate void NotificationEventHandlerRep(dynamic data);
        public event NotificationEventHandlerRep PostgresNotificationEvent;

        public void ExecuteQuery(string query)
        {

            using (var con = new NpgsqlConnection(conString))
            {
                //await new NpgsqlConnection(conString);
                //await con.OpenAsync();
                con.Open();
                con.Notification += LogNotificationHelper;
                //con.Notice += LogNoticeHelper; Use it to get all notice event from postgres
                //await using (var cmd = new NpgsqlCommand())
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.CommandTimeout = 120000;
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    con.Dispose();
                    con.Close();
                }
            }

            //while (true)
            //{
            //    // Waiting for Event
            //    //con.Wait();
            //}
        }

        private void LogNoticeHelper(object sender, NpgsqlNoticeEventArgs e)
        {
            string notice = e.Notice.ToString();
            string Code = e.Notice.Code;
            string columnName = e.Notice.ColumnName;
            string ConstraintName = e.Notice.ConstraintName;
            string DataTypeName = e.Notice.DataTypeName;
            string Detail = e.Notice.Detail;
            string ErrorSql = e.Notice.ErrorSql;
            string File = e.Notice.File;
            string Hint = e.Notice.Hint;
            string InternalPosition = e.Notice.InternalPosition;
            string InternalQuery = e.Notice.InternalQuery;
            string Line = e.Notice.Line;
            string Message = e.Notice.Message;
            string Position = e.Notice.Position;
            string Routine = e.Notice.Routine;
            string SchemaName = e.Notice.SchemaName;
            string Severity = e.Notice.Severity;
            string TableName = e.Notice.TableName;
            string Where = e.Notice.Where;
            //if (PostgresNotificationEvent != null)
            //    PostgresNotificationEvent.Invoke(Message);
        }

        private void LogNotificationHelper(object sender, NpgsqlNotificationEventArgs e)
        {
            //Deserialize Payload Data 
            int pid = e.PID;
            string table = e.AdditionalInformation;
            string condition = e.Condition;
            if (PostgresNotificationEvent != null)
                PostgresNotificationEvent.Invoke(table);

            //BroadCastUploadStatus(AdditionalInformation);
            //string aaa = e.GetType();
            //var dataPayload = JsonConvert.DeserializeObject<tbllogInfo>(e.PID.ToString());
            //Console.WriteLine("{0}", dataPayload.table + " :: " + dataPayload.action + " :: " + dataPayload.data.logdetails);

            //Notify Client using SignalR
        }
    }


}
