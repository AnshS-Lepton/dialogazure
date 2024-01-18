using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartFeasibility.Filters
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
    }
}