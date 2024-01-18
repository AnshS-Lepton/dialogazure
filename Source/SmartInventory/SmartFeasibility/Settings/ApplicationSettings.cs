using BusinessLogics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartFeasibility.Settings
{
    public class ApplicationSettings
    {
        public static int MapRegionProvinceLimit = 0;
        public static string ApplicationName = ConfigurationManager.AppSettings["ApplicationName"].ToString().Trim();
        public static string ClientName = ConfigurationManager.AppSettings["ClientName"].ToString().Trim();
        public static string AppVersion = ConfigurationManager.AppSettings["AppVersion"].ToString().Trim();
        public static string ADFSEndPoint = ConfigurationManager.AppSettings["ADFSEndPoint"].ToString().Trim();
        public static string ADFSRelPartyUri = ConfigurationManager.AppSettings["ADFSRelPartyUri"].ToString().Trim();
        public static string ADFSUserNamePreFix = ConfigurationManager.AppSettings["ADFSUserNamePreFix"].ToString().Trim();
        public static string ADFSAutheticationBasedOn = ConfigurationManager.AppSettings["ADFSAutheticationBasedOn"].ToString().Trim();

        public static string MapClient = ConfigurationManager.AppSettings["MapClient"];
        public static string MapChannel = ConfigurationManager.AppSettings["MapChannel"];
        public static string MapKey = ConfigurationManager.AppSettings["MapKey"];
        public static string MapAuthType = ConfigurationManager.AppSettings["MapAuthType"];
        public static string MapServerURL = ConfigurationManager.AppSettings["mapServerURL"];

        public static string MapDirPath = ConfigurationManager.AppSettings["mapDirPath"];
        public static string SIReferrer = ConfigurationManager.AppSettings["SIReferrer"];

        public static string DemoVersion = ConfigurationManager.AppSettings["DemoVersion"];
        public static string RoutingAPIUrl = ConfigurationManager.AppSettings["routingAPIUrl"];
        public static string DefaultMapZoomLevel = ConfigurationManager.AppSettings["zoomSize"];
        public static string CountryCenterLatLong = ConfigurationManager.AppSettings["CountryCenterLatLong"];
        public static bool isMultilingual = Convert.ToBoolean( ConfigurationManager.AppSettings["isMultilingual"]);

        public static string CustomerCurrency = ConfigurationManager.AppSettings["CustomerCurrency"];
        public static bool IsMapScaleDynamic = Convert.ToBoolean(ConfigurationManager.AppSettings["IsMapScaleDynamic"]);
        public static int EntitySearchLength = 0;

        public static string ClientLogoImageBytesForWeb = string.Empty;
        public static string ApplicationLogoImageBytesForWeb = string.Empty;

        public static void InitializeGlobalSettings()
        {
            var globalSettings = new BLGlobalSetting().GetGlobalSettings("WEB");
            foreach (var objSetting in globalSettings)
            {
                if (objSetting.key == "ClientLogoImageBytesForWeb")
                {
                    ClientLogoImageBytesForWeb = objSetting.value;
                }
                if (objSetting.key == "MapRegionProvinceLimit")
                {
                    MapRegionProvinceLimit = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "ApplicationLogoImageBytesForWeb")
                {
                    ApplicationLogoImageBytesForWeb = objSetting.value;
                }

            }
        }
    }
}