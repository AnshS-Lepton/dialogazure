using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Newtonsoft.Json;
using Utility;


namespace SmartInventoryServices.Helper
{
    public class OIAMHelper
    {

        public OAIMApiResponse GenerateOIAMToken(string userName, string password)
        {

            OAIMApiResponse rilApiResponce = new OAIMApiResponse();
            var response = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(ConfigurationManager.AppSettings["OIAM_TOKEN_URL"].ToString());
            bool isAgent = userName.IndexOf('.') < 0 ? true : false;
            bool isEmail = Regex.IsMatch(userName, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (isEmail)
                isAgent = true;

            if (isAgent == true)
            {
                sb.Append("?state=state_test&scope=agent&client_id=" + ConfigurationManager.AppSettings["OIAM_CLIENTID"].ToString() + "&client_secret=" + ConfigurationManager.AppSettings["OAIM_CLIENTSECRET"].ToString() + "&grant_type=password");
            }
            else
            {
                sb.Append("?state=state_test&scope=employee&client_id=" + ConfigurationManager.AppSettings["OIAM_CLIENTID"].ToString() + "&client_secret=" + ConfigurationManager.AppSettings["OAIM_CLIENTSECRET"].ToString() + "&grant_type=password");
            }

            //using (WebClient client = new WebClient())
            //{
            //    string apiBaseUri = sb.ToString();
            //    client.Headers.Clear();
            //    client.Headers[HttpRequestHeader.ContentType] = "application/json";
            //    client.Headers.Add("username", (userName));
            //    client.Headers.Add("password", (password));
            //    try
            //    {
            //        //if (!this._ByPassSECO)
            //        //response = client.UploadString.PostAsync("", null).Result;

            //        response = client.UploadString(apiBaseUri, null);
            //        rilApiResponce = JsonConvert.DeserializeObject<OAIMApiResponce>(response);
            //    }
            //    catch (Exception ex)
            //    {
            //        ErrorLogHelper logHelper = new ErrorLogHelper();
            //        logHelper.ApiLogWriter("GenerateTokenOIAM()", "OIAMHelper class", sb.ToString(), ex);
            //    }
            //}
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(sb.ToString());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("username", (userName));
                client.DefaultRequestHeaders.Add("password", (password));
                HttpResponseMessage httpResponse = new HttpResponseMessage();
                try
                {
                    //if (!this._ByPassSECO)
                    httpResponse = client.PostAsync("", null).Result;
                    var json = JsonConvert.SerializeObject(httpResponse);
                    rilApiResponce = JsonConvert.DeserializeObject<OAIMApiResponse>(json);
                }
                catch (Exception ex)
                {

                }
            }
            return rilApiResponce;
        }
    }

    public class OAIMApiResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public employee associate { get; set; }
    }



    public class employee
    {
        public string agentCode { get; set; }
        public string FirstName { get; set; }
        public string lastName { get; set; }
        public string displayname { get; set; }
        public string username { get; set; }
        public string Email { get; set; }
        public string employeeID { get; set; }
        public string mobile { get; set; }
        public string streetAddress { get; set; }
        public string dn { get; set; }
    }
}
