using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using BusinessLogics;

namespace IntegrationServices.Settings
{
    public class ApplicationSettings
    {
        public static int Serviceability_Buffer = 0;
        public static int Serviceability_Entity_Limit = 0;
        public static int Is_Clientip_Required = 1;
        public static string Map_Key = ConfigurationManager.AppSettings["MapKey"].Trim();
        public static string Map_Key_Backend = ConfigurationManager.AppSettings["MapKeyBackend"].Trim();
        public static void InitializeGlobalSettings()
        {
            var globalSettings = new BLGlobalSetting().GetGlobalSettings("WEB");

            foreach (var objSetting in globalSettings)
            {
                if (objSetting.key == "serviceability_buffer")
                    Serviceability_Buffer = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "serviceability_entity_limit")
                    Serviceability_Entity_Limit = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "is_clientip_required")
                    Is_Clientip_Required = Convert.ToInt32(objSetting.value);
            }
        }
        public struct StatusMessage
        {
            public const string notExist = "does not exist !";
            public const string wrong = "something went wrong !";
        }
        public static string getUser(string claimType)
        {
            var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == claimType)
               .Select(c => c.Value).SingleOrDefault();
            return user_id;
        }
    }

}