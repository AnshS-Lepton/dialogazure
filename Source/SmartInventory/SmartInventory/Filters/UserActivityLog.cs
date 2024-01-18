using BusinessLogics;
using Models.API;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Net;
using Utility;
using System.Web.Mvc;

namespace SmartInventory.Filters
{
    public class UserActivityLogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var descriptor = filterContext.ActionDescriptor;
                UserActivityLogSettings userActivityLogSettings =  UserActivityLogSettings.userActivityLogSettings.FirstOrDefault(x => (x.source.ToLower() == "web" && x.controller_name == descriptor.ControllerDescriptor.ControllerName && x.action_name == descriptor.ActionName));
                if (userActivityLogSettings != null)
                {
                    UserActivityLog obj = new UserActivityLog();

                    if (HttpContext.Current.User != null)
                    {
                        obj.user_id = Convert.ToInt32(HttpContext.Current.Session["user_id"]);
                        obj.user_name = HttpContext.Current.User.Identity.Name;

                        obj.controller_name = descriptor.ControllerDescriptor.ControllerName;
                        obj.action_name = descriptor.ActionName;
                        obj.client_ip = IPHelper.GetIPAddress();
                        obj.server_ip = Dns.GetHostEntry(Dns.GetHostName()).HostName ?? "";
                        obj.source = userActivityLogSettings.source;
                        obj.action_on = DateTime.Now;
                        obj.description = string.IsNullOrEmpty(userActivityLogSettings.res_key) ? userActivityLogSettings.description : Resources.Helper.MultilingualMessageConvert(userActivityLogSettings.res_key);
                        new BLUserActivityLog().SaveUserActivityLog(obj);
                    }
                }
            }
                
            catch (Exception ex)
            {
                PageMessage objMsg = new PageMessage();
              
                ErrorLogHelper logHelper = new ErrorLogHelper();
                ErrorLogHelper.WriteErrorLog("OnActionExecuting()", "UserActivityLogAttribute", ex);
                throw ex;
            }
        }
    }
}
