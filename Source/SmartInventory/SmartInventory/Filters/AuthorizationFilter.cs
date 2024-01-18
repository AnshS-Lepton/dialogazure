using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartInventory.Filters
{
    public class AuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        //private string[] UserProfilesRequired { get; set; }
        //public AuthorizationFilter(params string[] roles)
        //{
        //    UserProfilesRequired = roles;
        //}
        //public void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
        //        || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
        //    {
        //        // Don't check for authorization as AllowAnonymous filter is applied to the action or controller
        //        return;
        //    }

        //    var usrDetail = (User)HttpContext.Current.Session["userDetail"];
        //    // Check for authorization
        //    if (usrDetail != null)
        //    {
        //        if (usrDetail.role_id == 1 && !UserProfilesRequired.Contains("Admin"))
        //        {
        //            filterContext.Controller.TempData.Add("RedirectReason", "You are not authorized to access this page.");
        //            filterContext.Result = new RedirectResult("~/Unauthorized/Index");
        //        }
        //    }
        //    else
        //    {
        //        filterContext.Result = new RedirectResult("~/Login/Index");
        //    }

        //}
    }
}