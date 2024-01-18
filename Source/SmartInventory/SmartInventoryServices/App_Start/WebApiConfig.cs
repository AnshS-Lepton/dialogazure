using Models;
using SmartInventory.Filters;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace SmartInventoryServices
{
    public static class WebApiConfig 
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //For Enabling Cross-Origin Requests
            config.EnableCors();
            config.Filters.Add(new APIExceptionFilter());
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

           

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );    
          config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =new Newtonsoft.Json.Serialization.DefaultContractResolver { IgnoreSerializableAttribute = true };

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));// Added for uploadDocument API multipart format
            if (ApplicationSettings.IsUserActivityLogEnabled)
            {
                config.Filters.Add(new UserActivityLogAttribute());
            }
        }
    }
}
