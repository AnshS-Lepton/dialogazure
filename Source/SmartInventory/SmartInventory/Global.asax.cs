using BusinessLogics;
using Models;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Utility;

namespace SmartInventory
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            createPublishFile();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            ApplicationSettings.InitializeGlobalSettings();

            //new BLMisc().InitializeEmailSettings();
            EmailSettings.InitializeEmailSettings();

            if (ApplicationSettings.IsUserActivityLogEnabled) { 
            GlobalFilters.Filters.Add(new UserActivityLogAttribute());
            UserActivityLogSettings.userActivityLogSettings  = new BLUserActivityLog().GetUserActivityLogSettings();
            }
            LogHelper.GetInstance.WriteApplicationEventLog("Application Started");
        }

        /// <summary>
        /// This Function is Use for Convert global multilingual application .
        /// Set language globle in coockis .
        /// 
        /// </summary>
        /// 

        public void createPublishFile()
        {
            if (CommonUtility.GetAppSetting("Environment").ToUpper().ToString() == "LOCAL")
            {
                LogHelper.GetInstance.WriteApplicationEventLog("start createPublishFilee start");
                string directoryjsName = HttpContext.Current.Server.MapPath($"~/Content/js");
                string directorycssName = HttpContext.Current.Server.MapPath($"~/Content/css");
                CommonUtility.DeleteFile(directoryjsName);
                CommonUtility.DeleteFile(directorycssName);
                LogHelper.GetInstance.WriteApplicationEventLog("start createPublishFile 1");
                // js file rename
                string[] jsFileNames = { "Main", "ModalPopUp", "Utility", "Splicing", "NotifySignalR", "regionprovincedatauploader", "LandBase", "datauploader", "jspdf.min", "canvg.min", "html2canvas", };
               
              
                foreach (string fName in jsFileNames)
                {
                    string oldFileName = directoryjsName + $"/{fName}.js";
                    string updatedFileName = directoryjsName+$"/{fName}Version.{CommonUtility.getVersion()}.js";
                    CommonUtility.UpdateFileName(updatedFileName, oldFileName);
                }
                LogHelper.GetInstance.WriteApplicationEventLog("start createPublishFile 2");
                #region CSSS file rename

              
                string[] cssFileNames = { "main", "Splicing", "LandBase" };
                foreach (string fName in cssFileNames)
                {
                    string oldFileName = directorycssName +$"/{fName}.css";
                    // Get app version from wen config file
                   // string updatedFileName = HttpContext.Current.Server.MapPath($"~/Content/js/PublishFile/css/{fName}.{ConfigurationManager.AppSettings["AppVersion"]}.css");

                    // get app version automatically 
                   string updatedFileName = directorycssName + $"/{fName}Version.{CommonUtility.getVersion()}.css";

                    CommonUtility.UpdateFileName(updatedFileName, oldFileName);
                }
                #endregion
                LogHelper.GetInstance.WriteApplicationEventLog("start createPublishFile end");

            }
        }


        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
              
            HttpContext context = HttpContext.Current;
            string session = null;
            if (context.Session != null && context.Session["Language"] != null)
            {
                session = (context.Session["Language"]).ToString();

            }

            if (session != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(session);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(session);
                //System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat = new System.Globalization.CultureInfo("en-US").NumberFormat;
                System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat = new System.Globalization.CultureInfo("en-US").NumberFormat;

            }
            else
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            }

            if (!ApplicationSettings.isMultiLoginAllowed)
            {
                ValidateUser(context);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            var userDetails = (User)Session["userDetail"];

            if (userDetails != null)
            {
                var loginHistory = (UserLogin)Session["userLoginHistory"];
                int historyId = loginHistory != null ? loginHistory.history_id : 0;
                var userId = userDetails.user_id;
                new BLUserLogin().UpdateLogOutTime(userId, historyId,loginHistory.source);
            }
            else
            {
                Response.Redirect(Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~\\"));
            }
        }
        [HttpPost]
        public void ValidateUser(HttpContext context)
        {
            try
            { 
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    User objUserDetails = new User(); 
                    string UserName = context.Request.Params["LOGON_USER"].ToString();

                    if (UserName != null && UserName != "")
                    {
                        objUserDetails = new BLUser().validateUserLoginHistory(UserName); 
                            if (objUserDetails != null && HttpContext.Current.Session.SessionID != null && objUserDetails.session_id.ToString() != HttpContext.Current.Session.SessionID)
                            {
                                Session.Abandon(); 
                                HttpContext.Current.RewritePath("main/logout"); 
                            }  
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void Application_Error(object sender, EventArgs e)        
        {
            try
            {
                Response.Redirect(Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~\\"));
            }
            catch (Exception)
            {
            }  
        }
    }
}
