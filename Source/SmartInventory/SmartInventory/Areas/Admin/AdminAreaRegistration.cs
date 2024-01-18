using System.Web.Mvc;
using System.Web.Optimization;

namespace SmartInventory.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            RegisterRoutes(context);
            RegisterBundles();
        }

        private void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
              "Admin_default",
              "Admin/{controller}/{action}/{id}",
              new { action = "Index", id = UrlParameter.Optional }
          );
        }

        private void RegisterBundles()
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        } 

    }
}