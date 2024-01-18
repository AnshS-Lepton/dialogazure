
using BusinessLogics;
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
using static SmartInventory.Filters.AuthorizationFilter; 
namespace SmartInventory.Filters
{
    public class WebRequestLogFiltersAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var descriptor = filterContext.ActionDescriptor;
                WebRequestLog obj = new WebRequestLog();
                if (HttpContext.Current.User != null)
                {
                    var ActionParameter = JsonConvert.SerializeObject(filterContext.ActionParameters);
                    obj.user_name = HttpContext.Current.User.Identity.Name;
                    obj.controller_name = descriptor.ControllerDescriptor.ControllerName;
                    obj.action_name = descriptor.ActionName;
                    obj.client_ip = IPHelper.GetIPAddress();
                    obj.server_ip = Dns.GetHostEntry(Dns.GetHostName()).HostName ?? "";
                    obj.source = "Web";
                    obj.in_date_time= DateTime.Now;
                    obj.transaction_id = "TXN" + DateTime.Now.Ticks;
                    obj.request = ActionParameter;
                    new BLWebRequestLog().SaveWebRequestLog(obj);
                }
            }

            catch (Exception ex)
            {
                PageMessage objMsg = new PageMessage();

                ErrorLogHelper logHelper = new ErrorLogHelper();
                ErrorLogHelper.WriteErrorLog("OnActionExecuting()", "webRequestLogFilters", ex);
                throw ex;
            }
        }
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            try
            {
                WebRequestLog obj = new WebRequestLog();
                var descriptor = actionExecutedContext.ActionDescriptor;
                obj.action_name = descriptor.ActionName;
                obj.controller_name = descriptor.ControllerDescriptor.ControllerName;
                obj.out_date_time = DateTime.Now;
                obj.user_name = "admin";
                obj.source = "WEB";
                obj.response = JsonConvert.SerializeObject(actionExecutedContext.Result);
                new BLWebRequestLog().SaveWebRequestLog(obj);
            }
            catch (Exception ex)
            {
                PageMessage objMsg = new PageMessage();

                ErrorLogHelper logHelper = new ErrorLogHelper();
                ErrorLogHelper.WriteErrorLog("OnActionExecuted()", "webRequestLogFilters", ex);
                throw ex;
            }
        }
    }
}

