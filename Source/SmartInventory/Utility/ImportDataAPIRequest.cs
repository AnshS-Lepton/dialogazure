using Lepton.Entities;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace Lepton.Utility
{
    public static class ImportDataAPIRequest
    {
        static readonly string baseAPIUrl = new Uri(ConfigurationManager.AppSettings["SmartPlannerServiceURL"]).ToString();

        #region Generate Api Token 
        /// <summary>
        /// Get Api Token On Server Side
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>Token For Authentication</returns>
        public static TokenDetail GetAPIToken()
        {
            string username = ConfigurationManager.AppSettings["SmartPlannerServiceUserName"];
            string password = ConfigurationManager.AppSettings["SmartPlannerServicePassword"];
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {

                return null;
            }

            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        //new KeyValuePair<string, string>( "username", ConfigurationManager.AppSettings["SmartPlannerServiceUserName"] ),
                        //new KeyValuePair<string, string> ( "Password", ConfigurationManager.AppSettings["SmartPlannerServicePassword"] ),
                         new KeyValuePair<string, string>( "username", username),
                        new KeyValuePair<string, string> ( "Password", password),
                        new KeyValuePair<string, string> ( "source", "WEB" )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {

                var response = client.PostAsync(baseAPIUrl + "token", content).Result;
                //pk
                if (!response.IsSuccessStatusCode)
                {
                    // Handle error response
                    // Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var jsonContent = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TokenDetail>(jsonContent);

                //return JsonConvert.DeserializeObject<TokenDetail>(response.Content.ReadAsStringAsync().Result);
            }
        }
        #endregion


        #region Get Token Request
        /// <summary>
        /// Get Api Request by Token
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Request Detail</returns>
        public static string GetSmartPlannerAPIRequest(string urlParams )
        {
            try
            {

                var token = GetAPIToken();
                using (var client = GetClient(token.access_token))
                {
                    var response = client.GetAsync(baseAPIUrl+ "api" + "/"+"plangeojson"+"?"+ urlParams).Result;
                    //Lepton.Utility.ErrorLog.LogErrorToLogFile(null, response + "GetAPIRequestWithClientToken, line 75");
                    var x = response.Content.ReadAsStringAsync().Result;
                    //Lepton.Utility.ErrorLog.LogErrorToLogFile(null, x + "GetAPIRequestWithClientToken, line 77");
                    return x;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static string GetSmartPlannerAPIPlanListRequest()
        {
            try
            {
                var token = GetAPIToken();
                if (token == null)
                {
                    return null;
                }
                using (var client = GetClient(token.access_token))
                {

                    var response = client.GetAsync(baseAPIUrl + "api" + "/" + "getplanuserlist" ).Result;

                    var x = response.Content.ReadAsStringAsync().Result;
                    return x;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion


        #region Auth with bearer token
        /// <summary>
        /// Get Api Request by Token
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Request Detail</returns>
        public static HttpClient GetClient(string token)
        {
            var authValue = new AuthenticationHeaderValue("Bearer", token);
            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue }
                //Set some other client defaults like timeout / BaseAddress
            };
            return client;
        }
        #endregion
    }


}
