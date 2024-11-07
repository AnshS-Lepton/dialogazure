using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DTSIntegrationDialog
{
    internal class Program
    {
        static string accessToken { get; set; }
        public static void Main(string[] args)
        {
            WriteLog.WriteLogFile("Service Start at: " + DateTime.Now);
            var processId = BLDTS.EntryLogInProcessSummary();
            var token = GetAccessTokenAsync();
            ConsumeSiteListApi(token, processId);
            CheckandUpdateSite(token, processId);
            BLDTS.ExitLogOutProcessSummary(processId);
            WriteLog.WriteLogFile("Process completed for Process ID :" + processId + "at: " + DateTime.Now.ToString());
            Console.WriteLine("Process ID: " + processId);
        }
        public static string GetAccessTokenAsync()
        {
            WriteLog.WriteLogFile("Getting Access Token");
            string accessTokenEndpoint = ConfigurationManager.AppSettings["AccessTokenEndpoint"];
            string bearerToken = ConfigurationManager.AppSettings["bearerToken"];
            string accessKey = ConfigurationManager.AppSettings["AccessKey"];
            string accessValue = ConfigurationManager.AppSettings["AccessValue"];
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    WriteLog.WriteLogFile("Bearer Token: " + bearerToken);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
                    var requestUri = new Uri($"{accessTokenEndpoint}?{accessKey}={accessValue}");
                    WriteLog.WriteLogFile("requestUri: " + requestUri);
                    HttpResponseMessage response = client.PostAsync(requestUri, null).GetAwaiter().GetResult();
                    WriteLog.WriteLogFile("API Response: " + response);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var x = JsonConvert.DeserializeObject<AccessTokenResponse>(responseBody);
                        accessToken = x.access_token;
                        WriteLog.WriteLogFile("API Response: " + accessToken);
                        WriteLog.WriteLogFile("API Response: " + responseBody);
                        return accessToken; // Return the access token
                    }
                    else
                    {
                        WriteLog.WriteLogFile($"Failed to call the API. Status code: {response.StatusCode}");
                    }

                }
                catch (HttpRequestException httpEx)
                {
                    WriteLog.WriteLogFile("HTTP Request Exception: " + httpEx.Message);
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogFile("Exception caught at GetAccessTokenAsync: " + ex.Message);
                }
                return accessToken;
            }
        }
        public static void ConsumeSiteListApi(string token,int processID)
        {
            WriteLog.WriteLogFile("ConsumeApiAsync started");
            string GetSiteListURL = ConfigurationManager.AppSettings["GetSiteListURL"];
            List<SiteList> sitelst = new List<SiteList>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    token = token.Trim();
                    WriteLog.WriteLogFile("token " + token);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    WriteLog.WriteLogFile("Calling API: " + GetSiteListURL);

                    var requestUri = new Uri($"{GetSiteListURL}?Status=live");
                    //var requestUri = new Uri($"{GetSiteListURL}");
                    //string requestUri = $"{apiSettings.GetSiteListEndpoint}?Status=live";
                    WriteLog.WriteLogFile("Request URI: " + requestUri);
                    HttpResponseMessage response = client.GetAsync(requestUri).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var siteList = JsonConvert.DeserializeObject<SiteList>(responseBody);
                        sitelst.Add(siteList);
                        WriteLog.WriteLogFile("API Response: " + responseBody);
                    }
                    else
                    {
                        string errorResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        WriteLog.WriteLogFile($"Failed to call the API. Status code: {response.StatusCode}, Error: {errorResponse}");
                    }
                }
                var sites = sitelst[0].Response.ToList();
                BLDTS.SaveSitesList(sites, processID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                WriteLog.WriteLogFile("Exception caught at ConsumeApiAsync: " + ex.Message);
            }
        }

        public static void CheckandUpdateSite(string token,int progressID)
        {
            WriteLog.WriteLogFile("CheckandUpdateSite start");
            string GetSiteDetailsURL = ConfigurationManager.AppSettings["GetSiteDetailsURL"];

            try
            {
                token = token.Trim();
                WriteLog.WriteLogFile("token" + token);
                var sitelist = BLDTS.GetSiteIdsByProcessId(progressID);
                foreach (var siteID in sitelist)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                        var requestUri = new Uri($"{GetSiteDetailsURL}?Site_Id={siteID}");
                        WriteLog.WriteLogFile("Request URI: " + requestUri);
                        HttpResponseMessage response = client.GetAsync(requestUri).GetAwaiter().GetResult();

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            WriteLog.WriteLogFile("Site Details: " + responseBody);
                            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                            if (apiResponse?.Response != null && apiResponse.Response.Count > 0)
                            {
                                var siteToSave = apiResponse.Response[0];
                                WriteLog.WriteLogFile("Site Details: " + responseBody);
                                BLDTS.SaveSiteDetails(siteToSave, 1, progressID);
                            }
                        }
                        else
                        {
                            string errorResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            var message = $"Failed to call the API. Status code: {response.StatusCode}";
                            BLDTS.UpdateSiteList(siteID, progressID, message);
                            WriteLog.WriteLogFile(message);
                        }
                    }
                }
                if (sitelist.Count > 0)
                {
                    BLDTS.SaveSiteDetilsInMainTable(progressID);
                    BLDTS.UpdateNetworkandGeomDetails(progressID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                WriteLog.WriteLogFile("Exception caught at CheckandUpdateSite: " + ex.Message);
            }
        }
    }
}
