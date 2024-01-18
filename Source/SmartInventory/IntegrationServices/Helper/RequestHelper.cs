using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IntegrationServices.Helper
{
    public class RequestHelper
    {
        public static T GetRequestData<T>(Models.RequestInput userData)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(userData.data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}