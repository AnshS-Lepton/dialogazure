using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using Utility;
using System.Net;

namespace SmartInventory.Filters
{
    public class HandleException : HandleErrorAttribute
    {
        
        public override void OnException(ExceptionContext filterContext)
        {
            try
            {
                //If the exeption is already handled we do nothing
                if (!filterContext.ExceptionHandled)
                {

                    string actionName = filterContext.RouteData.Values["action"].ToString() ?? "";
                    string controllerName = filterContext.RouteData.Values["controller"].ToString() ?? "";
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
                    objErrorLog.exception = filterContext.Exception;
                    LogHelper.GetInstance.WriteErrorLog(objErrorLog);

                }
            }
            catch { }
            finally
            {
                //Make sure that we mark the exception as handled
                filterContext.ExceptionHandled = true;
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectResult("~/Error/Index");
            }
        }
    }
}