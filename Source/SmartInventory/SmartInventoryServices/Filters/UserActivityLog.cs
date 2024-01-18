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
using System.Web.Http.Filters;
using SmartInventoryServices.Helper;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using static SmartInventoryServices.Filters.CustomAuthorization;

namespace SmartInventory.Filters
{
    public class UserActivityLogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            try
            {
                var descriptor = filterContext.ActionDescriptor;
                UserActivityLogSettings userActivityLogSettings = UserActivityLogSettings.userActivityLogSettings.FirstOrDefault(x => (x.source.ToLower() == "mobile" &&  x.controller_name == descriptor.ControllerDescriptor.ControllerName && x.action_name == descriptor.ActionName));
                if (userActivityLogSettings != null)
                {
                    UserActivityLog obj = new UserActivityLog();

                    var ActionParameter = filterContext.ActionArguments;

                    //---GET ACCESS TOKEN and DECODE
                    if (filterContext.Request.Headers.Authorization != null)
                    {
                        var access_token = filterContext.Request.Headers.Authorization.Parameter;// GET Access Token From Header Request
                        var secureDataFormat = new Microsoft.Owin.Security.DataHandler.TicketDataFormat(new MachineKeyProtector());
                        Microsoft.Owin.Security.AuthenticationTicket ticket = secureDataFormat.Unprotect(access_token);

                        //-- GET ACCESS TOKEN PARAMATERS

                        obj.user_id = Convert.ToInt32(ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userId").Value);
                        obj.user_name = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userName").Value;
                        obj.controller_name = descriptor.ControllerDescriptor.ControllerName;
                        obj.action_name = descriptor.ActionName;
                        obj.client_ip = IPHelper.GetIPAddress();
                        obj.server_ip = Dns.GetHostEntry(Dns.GetHostName()).HostName ?? "";
                        obj.source = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "source").Value;
                        obj.action_on = DateTime.Now;
                        obj.description = string.IsNullOrEmpty(userActivityLogSettings.res_key) ? userActivityLogSettings.description : Resources.Helper.MultilingualMessageConvert(userActivityLogSettings.res_key);
                        new BLUserActivityLog().SaveUserActivityLog(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<dynamic>();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("OnActionExecuting()", "UserActivityLogAttribute", "", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
                throw ex;
            }
        }
    }
    
}