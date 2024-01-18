//using BusinessLogics;
//using Models;
//using Models.API;
//using Newtonsoft.Json;
//using SmartInventoryServices.Helper;
//using System;
//using System.Linq;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
using BusinessLogics;
using Models;
using Models.API;
using Newtonsoft.Json;
using SmartInventoryServices.Helper;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Utility;
using static SmartInventoryServices.Filters.CustomAuthorization;

namespace PartnetServices.Filter
{
    public class CustomAction: ActionFilterAttribute
    {
        public string  transaction_id;
        public DateTime in_date_time;

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            try
            {
                APIRequestLog obj = new APIRequestLog();
                var headerAttribute =filterContext.Request.Headers.ToList();
                var json = new { data = JsonConvert.SerializeObject(headerAttribute) };
                obj.header_attribute = JsonConvert.SerializeObject(json);

                var descriptor = filterContext.ActionDescriptor;
                var ActionParameter = filterContext.ActionArguments;
                obj.action_name = descriptor.ActionName;
                obj.controller_name = descriptor.ControllerDescriptor.ControllerName;
                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                obj.client_ip = ip;
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
                IPAddress ServerIpAddress = ipHostInfo.AddressList[1];
                obj.server_ip = ServerIpAddress.ToString();
                obj.in_date_time = DateTime.Now;
                obj.transaction_id = "TXN"+DateTime.Now.Ticks;
                // obj.server_ip = HttpContext.Current.Request.
                ////---GET ACCESS TOKEN and DECODE
                //var access_token = filterContext.Request.Headers.Authorization.Parameter;// GET Access Token From Header Request
                //var secureDataFormat = new Microsoft.Owin.Security.DataHandler.TicketDataFormat(new MachineKeyProtector());
                //Microsoft.Owin.Security.AuthenticationTicket ticket = secureDataFormat.Unprotect(access_token);

                ////-- GET ACCESS TOKEN PARAMATERS
                //var isLogRequired = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "isLogRequired").Value;
                //obj.user_name = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userName").Value;
                //obj.os_name = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "osName").Value;
                //obj.os_version = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "osType").Value;
                //obj.source = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "source").Value;

                //temp fix
                obj.user_name = "Admin";
                obj.source = "WEB";
                //temp fix close
                transaction_id = obj.transaction_id;
                in_date_time = obj.in_date_time;
                if (filterContext.ActionArguments.Count > 0)
                    obj.request = JsonConvert.SerializeObject(filterContext.ActionArguments["data"]);
                //obj.os_version = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "IOS_Version").Value;
                //additionally putted 
                System.Web.HttpContext.Current.Items["filterobj"]=obj;
                 new BLAPIRequestLog().SavePartnerApiRequestLog(obj);
                ApiLogHelper.GetInstance.WriteApiLog(obj);
                //additionally putted end
                //if (!string.IsNullOrEmpty(isLogRequired) && Convert.ToBoolean(isLogRequired))
                //{
                //    new BLAPIRequestLog().SaveApiRequestLog(obj);
                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                APIRequestLog obj = new APIRequestLog();
                var headerAttribute = actionExecutedContext.Request.Headers.ToList();
                var json = new { data = JsonConvert.SerializeObject(headerAttribute) };
                obj.header_attribute = JsonConvert.SerializeObject(json);
                
                var descriptor = actionExecutedContext.ActionContext.ActionDescriptor;
                var ActionParameter = actionExecutedContext.ActionContext.ActionArguments; 
                obj.action_name = descriptor.ActionName;
                obj.controller_name = descriptor.ControllerDescriptor.ControllerName;
                obj.transaction_id = transaction_id;
                obj.in_date_time = in_date_time;
                APIRequestLog ControllerData = (APIRequestLog)System.Web.HttpContext.Current.Items["filterobj"];
                obj.out_date_time = ControllerData.out_date_time;
                obj.latency = ControllerData.latency;
                obj.client_ip = ControllerData.client_ip;
                obj.server_ip = ControllerData.server_ip;
                //---GET ACCESS TOKEN and DECODE
                //var access_token = actionExecutedContext.Request.Headers.Authorization.Parameter;// GET Access Token From Header Request
                //var secureDataFormat = new Microsoft.Owin.Security.DataHandler.TicketDataFormat(new MachineKeyProtector());
                //Microsoft.Owin.Security.AuthenticationTicket ticket = secureDataFormat.Unprotect(access_token);

                ////-- GET ACCESS TOKEN PARAMATERS
                //var isLogRequired = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "isLogRequired").Value;
                //obj.user_name = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userName").Value;
                //obj.os_name = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "osName").Value;
                //obj.os_version = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "osType").Value;
                //obj.source = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "source").Value;

                obj.user_name = "admin";
                obj.source = "WEB";

                obj.response = actionExecutedContext.ActionContext.Response.Content.ReadAsStringAsync().Result.ToString();
                
               //obj.os_version = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "IOS_Version").Value;

                //temporary fix
                new BLAPIRequestLog().SavePartnerApiRequestLog(obj);
                
                ApiLogHelper.GetInstance.WriteApiLog(obj);
                //
                //if (!string.IsNullOrEmpty(isLogRequired) && Convert.ToBoolean(isLogRequired))
                //{
                //    new BLAPIResponseLog().SaveApiRequestLog(obj);
                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}



