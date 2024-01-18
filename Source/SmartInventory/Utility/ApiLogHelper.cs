
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Utility.BlUtility;

namespace Utility
{
    public class ApiLogHelper
    {

        private ApiLogHelper()
        {

        }

        private static ApiLogHelper _instance;
        private static bool _isLogRequiredInDB;
        private static bool _isLogRequiredInFile;
        private static string _logFolderPath;
        public static ApiLogHelper GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ApiLogHelper();
                    _isLogRequiredInDB = ConfigurationManager.AppSettings["isApiLogRequiredInDB"].ToUpper().Trim() == "TRUE" ? true : false;
                    _isLogRequiredInFile = ConfigurationManager.AppSettings["isApiLogRequiredInFile"].ToUpper().Trim() == "TRUE" ? true : false;
                    _logFolderPath = ConfigurationManager.AppSettings["ApilogFolderPath"].ToString();
                }
                return _instance;
            }
        }
        public void WriteErrorLog(ApiErrorLogInput objErrorLogInput)
        {
            string errDesc = string.Empty;

            //Get All input parameters using reflection from object...              
            string inputParametrs = objErrorLogInput.EntityObject != null ? GetAllEntityClasssProperties(objErrorLogInput.EntityObject) : string.Empty;

            //Prepare error log object..
            ApiErrorLog errorEntity = new ApiErrorLog();
            errorEntity.user_id = objErrorLogInput.userId;
            errorEntity.user_name = objErrorLogInput.UserName;
            errorEntity.controller_name = objErrorLogInput.fromPage;
            errorEntity.action_name = objErrorLogInput.fromMethod;
            errorEntity.request_data = objErrorLogInput.requestData;
            //get errorDesc from Exception...
            if (objErrorLogInput.exception != null)
            {
                if (!string.IsNullOrEmpty(inputParametrs))
                {
                    errDesc = string.Format("execption Message: {0} | passed parametrs: {1}",
                    ((objErrorLogInput.exception.InnerException != null && objErrorLogInput.exception.InnerException.InnerException != null) ? Convert.ToString(objErrorLogInput.exception.InnerException.InnerException.Message) : objErrorLogInput.exception.Message),
                    inputParametrs);
                }
                else
                {
                    errDesc = string.Format("execption Message: {0} | exception type name: {1}",
                        ((objErrorLogInput.exception.InnerException != null && objErrorLogInput.exception.InnerException.InnerException != null) ?
                        Convert.ToString(objErrorLogInput.exception.InnerException.InnerException.Message) : objErrorLogInput.exception.Message), objErrorLogInput.exception.GetType().FullName);
                }


                errorEntity.err_type = objErrorLogInput.exception.GetType().ToString();
                errorEntity.err_label = objErrorLogInput.exception.Message;
                errorEntity.err_message = (objErrorLogInput.exception.InnerException == null ? objErrorLogInput.exception.Message : Convert.ToString(objErrorLogInput.exception.InnerException.Message));
                errorEntity.err_description = errDesc;
                errorEntity.stack_trace = objErrorLogInput.exception.StackTrace;
            }
            else
            {
                errorEntity.err_type = "";
                errorEntity.err_label = objErrorLogInput.errorMessage;
                errorEntity.err_message = objErrorLogInput.errorMessage;
                errorEntity.err_description = string.Format("execption Message: {0} | passed parametrs: {1}", objErrorLogInput.errorMessage, inputParametrs);
                errorEntity.stack_trace = objErrorLogInput.errorMessage;
            }
            // Log error in file...
            if (_isLogRequiredInFile)
            {
                using (StreamWriter sw = File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath(_logFolderPath + "ErrorLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
                {
                    sw.WriteLine("\r\nLog Entry:==========>");
                    sw.WriteLine("error on Time: {0}", DateTimeHelper.Now);
                    sw.WriteLine("error Logged By: {0}", errorEntity.user_name);
                    sw.WriteLine("from Page: {0}", errorEntity.controller_name);
                    sw.WriteLine("from Method: {0}", errorEntity.action_name);
                    sw.WriteLine("Request Data: {0}", errorEntity.request_data);

                    if (objErrorLogInput.exception != null) // log exception...
                    {
                        sw.WriteLine("error Type: {0}", errorEntity.err_type);
                        sw.WriteLine("error Label: {0}", errorEntity.err_label);
                        sw.WriteLine("error Message: {0}", errorEntity.err_message);
                        sw.WriteLine("error Description: {0}", errorEntity.err_description);
                        sw.WriteLine("error Stack Trace: {0}", errorEntity.stack_trace);
                        sw.WriteLine("===============================================================");
                    }
                    else // log user defined error message...
                    {
                        sw.WriteLine("passed Parameters: {0}", inputParametrs);
                        sw.WriteLine("Exception Message: {0}", objErrorLogInput.errorMessage);
                        sw.WriteLine("===============================================================");
                    }

                }
            }
            //log error in DB...
            if (_isLogRequiredInDB)
            {
                BLErrorLog.SaveApiErrorLog(errorEntity);
            }
        }

        public void WriteDebugLog(string LogMessage)
        {
            string errDesc = string.Empty;
            // Log error in file...
            if (_isLogRequiredInFile)
            {
                using (StreamWriter sw = File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath(_logFolderPath + "DebugLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
                {
                    sw.WriteLine("\r\nLog Entry:==========>");
                    sw.WriteLine("Log on Time: {0}", DateTimeHelper.Now);
                    sw.WriteLine("Log Message: {0}", LogMessage);
                }
            }
            
        }


        private string GetAllEntityClasssProperties(object objEntityClasss) //static
        {
            StringBuilder sb = null;
            try
            {
                PropertyInfo[] propertiesInfo = objEntityClasss.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                if (propertiesInfo != null)
                {
                    sb = new StringBuilder();
                    foreach (PropertyInfo property in propertiesInfo)
                    {
                        if (property.CanRead)
                            sb.Append(property.Name + ": " + property.GetValue(objEntityClasss, null) + " | ");
                    }
                }
            }
            catch (Exception)
            {
                //Console.WriteLine(ex.ToString());
            }
            return sb != null ? sb.ToString() : string.Empty;
        }

        public void WriteApiLog(APIRequestLog ReqResLog)
        {
            // Log in file...
            DirectoryInfo dirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(_logFolderPath));
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            if (ReqResLog.request != null)
            {
             

                using (StreamWriter sw = File.AppendText(HttpContext.Current.Server.MapPath(_logFolderPath + "ApiResquestResponseLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
                {
                    sw.WriteLine("\r\nLog Entry:==========>");
                    sw.WriteLine("Request transaction id: {0}", ReqResLog.transaction_id);
                    sw.WriteLine("Request client ip: {0}",ReqResLog.client_ip);
                    sw.WriteLine("Request server ip: {0}", ReqResLog.server_ip);
                    sw.WriteLine("Request in Time: {0}", ReqResLog.in_date_time);
                    sw.WriteLine("Reponse out Time: {0}", "");
                    sw.WriteLine("latency: {0}","");
                    sw.WriteLine("Request By: {0}", ReqResLog.user_name);
                    sw.WriteLine("Request Source:{0}",ReqResLog.source);
                    sw.WriteLine("from Page: {0}", ReqResLog.controller_name);
                    sw.WriteLine("from Method: {0}", ReqResLog.action_name);
                    sw.WriteLine("Request header attribute: {0}", ReqResLog.header_attribute);
                    sw.WriteLine("Request Data: {0}", ReqResLog.request);
                }
            }
            else if(ReqResLog.response!=null)
            {
                using (StreamWriter sw = File.AppendText(HttpContext.Current.Server.MapPath(_logFolderPath + "ApiResquestResponseLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
                {
                    sw.WriteLine("\r\nLog Entry:==========>");
                    sw.WriteLine("Response transaction id: {0}", ReqResLog.transaction_id);
                    sw.WriteLine("Request client ip: {0}", ReqResLog.client_ip);
                    sw.WriteLine("Request server ip: {0}", ReqResLog.server_ip);
                    sw.WriteLine("Request in Time: {0}", ReqResLog.in_date_time);
                    sw.WriteLine("Response out Time: {0}", ReqResLog.out_date_time);
                    sw.WriteLine("latency: {0}", ReqResLog.latency);
                    sw.WriteLine("Request By: {0}", ReqResLog.user_name);
                    sw.WriteLine("Response Source:{0}", ReqResLog.source);
                    sw.WriteLine("from Page: {0}", ReqResLog.controller_name);
                    sw.WriteLine("from Method: {0}", ReqResLog.action_name);
                    sw.WriteLine("Request header attribute: {0}", ReqResLog.header_attribute);
                    sw.WriteLine("Response Data: {0}", ReqResLog.response);
                }
            }

        }

        public void PartnerAPILogs(PartnerAPILog LogMessage)
        {
            using (StreamWriter sw = File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath(_logFolderPath + "PartnerAPILog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
            {
                sw.WriteLine("\r\n Partner API Log Entry:==========>");
                sw.WriteLine("Log on Time: {0}", DateTimeHelper.Now);
                sw.WriteLine("API URL: {0}", LogMessage.URL);
                sw.WriteLine("API Request: {0}", LogMessage.request);
                sw.WriteLine("API Response: {0}", LogMessage.response);
                sw.WriteLine("API Requested By: {0}", LogMessage.UserName);
                sw.WriteLine("API Transaction Id: {0}", LogMessage.Transactionid);
                sw.WriteLine("API InDateTime: {0}", LogMessage.InDateTime);
                sw.WriteLine("API OutDateTime: {0}", LogMessage.OutDateTime);
                sw.WriteLine("API Latency: {0}", LogMessage.Latency);
            }
        }

    }
}
