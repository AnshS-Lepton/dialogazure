using BusinessLogics;
using Models;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartInventory.Filters
{
    [AttributeUsage(AttributeTargets.Class |
   AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        
        {

            HttpContext ctx = HttpContext.Current;
            if (ctx.Session["userDetail"] == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.Result = new JsonResult
                    {
                        Data = new { error = "Session expired" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Login/Logout");
                }
            }
            base.OnActionExecuting(filterContext);

        }
        //other methods...
    }

    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)

        {

            var usrDetail = (User)HttpContext.Current.Session["userDetail"];

            if (usrDetail != null && usrDetail.role_id != 1 && usrDetail.is_admin_rights_enabled == false)
            {
                filterContext.Result = new RedirectResult("~/UnAuthorized/Index");
            }

        }
        //other methods...
    }

    public class UserPermissionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (ApplicationSettings.isMultiLoginAllowed)
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    User objUserDetails = new User();
                    var usrDetail = (User)HttpContext.Current.Session["userDetail"];
                    string UserName = usrDetail.user_name;

                    if (UserName != null && UserName != "")
                    {
                        objUserDetails = new BLUser().validateUserLoginHistory(UserName);
                        if (objUserDetails != null &&( objUserDetails.session_id == null || HttpContext.Current.Session.SessionID != null ))
                        {
                            HttpContext.Current.Session.Abandon();
                            filterContext.Result = new RedirectResult("/");
                        }
                    }
                }
            }

        }
    }
}