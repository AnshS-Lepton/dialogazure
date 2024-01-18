using Models;
using SmartInventory.Settings;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SmartInventoryServices
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()        
        {           
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ApplicationSettings.InitializeGlobalSettings();
            MvcHandler.DisableMvcResponseHeader = true;
            if (ApplicationSettings.IsUserActivityLogEnabled)
            {
                UserActivityLogSettings.userActivityLogSettings = new BLUserActivityLog().GetUserActivityLogSettings();
            }
        }
    }
}
