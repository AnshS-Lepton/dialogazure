using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Utility;

namespace SmartInventory.Helper
{
    public class ErrorLogHelper
    {

        public static void WriteErrorLog(string actionName, string controllerName, Exception ex)
        {
            string method_Name = actionName + "/" + controllerName;
            ErrorLogInput objErrorLog = new ErrorLogInput();
            objErrorLog.userId = Convert.ToInt32(HttpContext.Current.Session["user_id"]);
            objErrorLog.UserName = HttpContext.Current.User.Identity.Name;
            objErrorLog.fromPage = controllerName;
            objErrorLog.fromMethod = method_Name;
            objErrorLog.clientIp = IPHelper.GetIPAddress();
            objErrorLog.serverIp = Dns.GetHostEntry(Dns.GetHostName()).HostName ?? "";
            BrowserHelper browserHelper = new BrowserHelper();
            objErrorLog.browserName = browserHelper.BrowserName;
            objErrorLog.browserVersion = browserHelper.BrowserVersion;
            objErrorLog.exception = ex;
            LogHelper.GetInstance.WriteErrorLog(objErrorLog);
        }
        public static void WriteErrorLog(string actionName, string controllerName, Exception ex, object EntityObject)
        {
            string method_Name = actionName + "/" + controllerName;
            ErrorLogInput objErrorLog = new ErrorLogInput();
            objErrorLog.userId = Convert.ToInt32(HttpContext.Current.Session["user_id"]);
            objErrorLog.UserName = HttpContext.Current.User.Identity.Name;
            objErrorLog.fromPage = controllerName;
            objErrorLog.fromMethod = method_Name;
            objErrorLog.clientIp = IPHelper.GetIPAddress();
            objErrorLog.serverIp = Dns.GetHostEntry(Dns.GetHostName()).HostName ?? "";
            BrowserHelper browserHelper = new BrowserHelper();
            objErrorLog.browserName = browserHelper.BrowserName;
            objErrorLog.browserVersion = browserHelper.BrowserVersion;
            objErrorLog.EntityObject = EntityObject;
            objErrorLog.exception = ex;
            LogHelper.GetInstance.WriteErrorLog(objErrorLog);
        }
    }
}