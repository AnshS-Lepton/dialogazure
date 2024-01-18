using SmartFeasibility.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SmartFeasibility
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ApplicationSettings.InitializeGlobalSettings();
        }

        /// <summary>
        /// This Function is Use for Convert global multilingual application.
        /// Set language globle in coockis .
        /// 
        /// </summary>

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //HttpCookie cookie = HttpContext.Current.Request.Cookies["Language"];

            //if (cookie != null && cookie.Value != null)
            //{
            //    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
            //    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            //}
            //else
            //{

            //    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            //    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            //}
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            string session = null;
            if (context.Session !=null && context.Session["Language"] !=null)
            {
                 session = (context.Session["Language"]).ToString();

            }

            if (session != null )
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(session);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(session);
            }
            else
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            }

        }
    }
}
