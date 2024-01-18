using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Npgsql;
using System.Data;
using System.IO;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UtilizationEmailScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            new Program().GetEmailDetailAndSend();

        }

        public void GetEmailDetailAndSend()
        {
            try
            {
                var filename = "UtilizationReport" + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                string paths = Directory.GetCurrentDirectory() + "//Upload//" + filename;
                string excelFilePath = new Program().EmailAttachmentFilePath(paths);
                DataTable dtEmailContentDetail = GetEmailData();
                Email.SendEmailWithAttachment(dtEmailContentDetail.Rows[0]["template"].ToString(), dtEmailContentDetail.Rows[0]["subject"].ToString(), excelFilePath, dtEmailContentDetail.Rows[0]["recipient_list"].ToString().Split(','));
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message+"." +(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.ToString():"") : string.Empty;
                string stackTrace = ex.StackTrace;
                Email.CaptureErrorInFile(message, innerException, stackTrace);
                

                // Further handling or logging of the exception details
            }

        }
        public DataTable GetEmailData()
        {
            DataTable dtList = new DataTable();
            string strConnString = ConfigurationManager.AppSettings["constr"].ToString();
            try
            {
                NpgsqlConnection objConn = new NpgsqlConnection(strConnString);
                objConn.Open();
                string strSelectCmd = "select * from fn_get_scheduler_eventemailtemplate('PercentUtilization50')";
                NpgsqlCommand objDataAdapter = new NpgsqlCommand(strSelectCmd, objConn);
                var DataReader = objDataAdapter.ExecuteReader();
                dtList.Load(DataReader);
                objConn.Close();
                return dtList;
            }
            catch 
            {
                throw;
            }
            return dtList;
        }

        public DataTable GetUtilizationData()
        {
            DataTable dtList = new DataTable();
            string strConnString = ConfigurationManager.AppSettings["constr"].ToString();
            try
            {
                NpgsqlConnection objConn = new NpgsqlConnection(strConnString);
                objConn.Open();
                string strSelectCmd = "select * from fn_get_UtilizationData()";
                NpgsqlCommand objDataAdapter = new NpgsqlCommand(strSelectCmd, objConn);
                var DataReader = objDataAdapter.ExecuteReader();
                dtList.Load(DataReader);
                dtList.TableName = "UtilizationReport";
                objConn.Close();
                return dtList;
            }
            catch 
            {
                throw;
            }
            return dtList;
        }
        public string EmailAttachmentFilePath(string filepath)
        {
            try
            {

                DataTable dt = new Program().GetUtilizationData();
                if (dt.Rows.Count > 0)
                {
                    dt.TableName = "UtilizationReport";
                    string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                }
            }
            catch 
            {
                throw;
            }
            return filepath;
        }
    }
}
