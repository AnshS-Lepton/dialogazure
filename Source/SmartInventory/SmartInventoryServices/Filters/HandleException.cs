using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Web.Mvc;
using Models;
using Utility;
using static SmartInventoryServices.Filters.CustomAuthorization;

namespace SmartInventoryServices.Filters
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
                    User user = new User(); //Utils.getUserDetailsFromToken();
                    ApiErrorLogInput objErrorLog = new ApiErrorLogInput();
                    objErrorLog.userId = user.user_id;
                    objErrorLog.UserName = user.user_name;
                    objErrorLog.fromPage = controllerName;
                    objErrorLog.fromMethod = method_Name;
                    objErrorLog.exception = filterContext.Exception;
                    objErrorLog.EntityObject = user;
                    ApiLogHelper.GetInstance.WriteErrorLog(objErrorLog);
                }
            }
            catch (Exception ex)
            {
                //MessageLog.WriteLog(ex); // in case of any exception occoured in writing logs the error page will show to end user 
            }
            finally
            {
                //Make sure that we mark the exception as handled
                //filterContext.ExceptionHandled = true;
                //var resp = new ApiResponse<User>();
                //resp.error_message = filterContext.Exception.Message.ToString();
                //resp.status = StatusCodes.UNKNOWN_ERROR.ToString();
            }
        }
    }
    public class APIExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext filterContext)
        {

            try
            {
                //If the exeption is already handled we do nothing

                string controllerName = filterContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
                string actionName = filterContext.ActionContext.ActionDescriptor.ActionName;
                string method_Name = actionName + "/" + controllerName;
                User user = new User();
                //Utils.getUserDetailsFromToken();
                var access_token = filterContext.Request.Headers.Authorization.Parameter;// GET Access Token From Header Request
                var secureDataFormat = new Microsoft.Owin.Security.DataHandler.TicketDataFormat(new MachineKeyProtector());
                Microsoft.Owin.Security.AuthenticationTicket ticket = secureDataFormat.Unprotect(access_token);

                //-- GET ACCESS TOKEN PARAMATERS

                user.user_name = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userName").Value;
                user.user_id = Convert.ToInt32(ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userId").Value);

                ApiErrorLogInput objErrorLog = new ApiErrorLogInput();
                objErrorLog.userId = user.user_id;
                objErrorLog.UserName = user.user_name;
                objErrorLog.fromPage = controllerName;
                objErrorLog.fromMethod = method_Name;
                objErrorLog.exception = filterContext.Exception;
                objErrorLog.EntityObject = user;
                ApiLogHelper.GetInstance.WriteErrorLog(objErrorLog);

            }
            catch (Exception ex)
            {
                //MessageLog.WriteLog(ex); // in case of any exception occoured in writing logs the error page will show to end user 
            }
            finally
            {
                //Make sure that we mark the exception as handled
                //filterContext.ExceptionHandled = true;
                //var resp = new ApiResponse<User>();
                //resp.error_message = filterContext.Exception.Message.ToString();
                //resp.status = StatusCodes.UNKNOWN_ERROR.ToString();
            }
        }
    }
}