
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
    public class LogHelper
    {

        private LogHelper()
        {

        }

        private static LogHelper _instance;
        private static bool _isLogRequiredInDB;
        private static bool _isLogRequiredInFile;
        private static string _logFolderPath;
        public static LogHelper GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogHelper();
                    _isLogRequiredInDB = ConfigurationManager.AppSettings["isLogRequiredInDB"].ToUpper().Trim() == "TRUE" ? true : false;
                    _isLogRequiredInFile = ConfigurationManager.AppSettings["isLogRequiredInFile"].ToUpper().Trim() == "TRUE" ? true : false;
                    _logFolderPath = ConfigurationManager.AppSettings["logFolderPath"].ToString();
                }
                return _instance;
            }
        }
        public void WriteErrorLog(ErrorLogInput objErrorLogInput)
        {
            string errDesc = string.Empty;

            //Get All input parameters using reflection from object...              
            string inputParametrs = objErrorLogInput.EntityObject != null ? GetAllEntityClasssProperties(objErrorLogInput.EntityObject) : string.Empty;

            //Prepare error log object..
            ErrorLog errorEntity = new ErrorLog();
            errorEntity.user_id = objErrorLogInput.userId;
            errorEntity.user_name = objErrorLogInput.UserName;
            errorEntity.controller_name = objErrorLogInput.fromPage;
            errorEntity.action_name = objErrorLogInput.fromMethod;

            errorEntity.client_ip = objErrorLogInput.clientIp;
            errorEntity.server_ip = objErrorLogInput.serverIp;
            errorEntity.browser_name = objErrorLogInput.browserName;
            errorEntity.browser_version = objErrorLogInput.browserVersion;
            errorEntity.created_on= DateTimeHelper.Now;
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
                BLErrorLog.SaveErrorLog(errorEntity);
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

        public void WriteDebugLog(string LogMessage)
        {
            string errDesc = string.Empty;
            // Log error in file...
            if (_isLogRequiredInFile)
            {
                //using (StreamWriter sw = File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath(_logFolderPath + "DebugLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
                //{
                //    //sw.WriteLine("\r\nLog Entry:==========>");
                //    //sw.WriteLine("Log on Time: {0}", DateTimeHelper.Now);
                //    sw.WriteLine(LogMessage);
                //}
            }

        }
		public void WriteDebugLogTest(string LogMessage, string fileName)
		{
			string errDesc = string.Empty;
			// Log error in file...
			if (_isLogRequiredInFile)
			{
				//using (StreamWriter sw = File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath(_logFolderPath + fileName + "DebugLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
				//{
				//	//sw.WriteLine("\r\nLog Entry:==========>");
				//	//sw.WriteLine("Log on Time: {0}", DateTimeHelper.Now);
				//	sw.WriteLine(LogMessage);
				//}
			}

		}
		public void WriteApplicationEventLog(string LogMessage)
        {
            //using (StreamWriter sw = File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath(_logFolderPath + "ApplicationLogs-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
            //{
            //    sw.WriteLine("Log Message : {0} {1}", LogMessage, DateTimeHelper.Now);
            //    sw.WriteLine("\r\n----------------------");
            //}
        }

    }
}
