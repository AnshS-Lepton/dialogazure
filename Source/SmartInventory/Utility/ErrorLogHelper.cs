using Models;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace Utility
{
    public class ErrorLogHelper
    {
        public static void WriteErrorLog(string actionName, string controllerName, Exception ex, int userId = 0)
        {
            string method_Name = actionName + "/" + controllerName;
            ErrorLogInput objErrorLog = new ErrorLogInput();
            objErrorLog.userId = HttpContext.Current!=null? Convert.ToInt32(HttpContext.Current.Session["user_id"]):userId;
            objErrorLog.UserName = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name : "";
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
        public static void WriteErrorLog(string actionName, string controllerName, Exception ex, object EntityObject,int userId=0)
        {
            string method_Name = actionName + "/" + controllerName;
            ErrorLogInput objErrorLog = new ErrorLogInput();
            objErrorLog.userId = HttpContext.Current != null ? Convert.ToInt32(HttpContext.Current.Session["user_id"]): userId;
            objErrorLog.UserName = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name:"";
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

        public void GetUserInfo(ref int id, ref string userName)
        {
            try
            {
                ClaimsPrincipal icp = Thread.CurrentPrincipal as ClaimsPrincipal;
                // Access IClaimsIdentity which contains claims
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)icp.Identity;
                // Access claims
                foreach (Claim claim in claimsIdentity.Claims)
                {
                    if (claim.Type == "username")
                        userName = claim.Value;
                    else if (claim.Type == "userid")
                        id = Convert.ToInt32(claim.Value);
                }
            }
            catch
            {
                id = 0;
                userName = "";
            }
        }
        public void ApiLogWriter(string methodName, string ControllerName, string requestData, Exception ex, int userId = 0)
        {

            int id = 0;
            string userName = "";
            ApiErrorLogInput objErrorLog = new ApiErrorLogInput();
            GetUserInfo(ref id, ref userName);
            objErrorLog.userId = id == 0 ? userId : id;  // in case token does not exists pass userId
            objErrorLog.UserName = userName;
            objErrorLog.fromPage = ControllerName;
            objErrorLog.fromMethod = methodName;
            objErrorLog.requestData = requestData;
            objErrorLog.exception = ex;
            ApiLogHelper.GetInstance.WriteErrorLog(objErrorLog);
        }
        public void WriteDebugLog(string LogMessage)
        {
            ApiLogHelper.GetInstance.WriteDebugLog(LogMessage);
        }
        public void PartnerAPILogs(PartnerAPILog LogMessage)
        {
            ApiLogHelper.GetInstance.PartnerAPILogs(LogMessage);
        }
    }
}
