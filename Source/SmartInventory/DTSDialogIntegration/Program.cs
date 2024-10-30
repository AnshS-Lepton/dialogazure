using DTSDialogIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Linq.Expressions;
using Models.API;

namespace DialogDTSIntegration
{
    public class Program
    {
        public static DTSApiSettings apiSettings = new DTSApiSettings();
        static string accessToken = string.Empty;
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            configuration.GetSection("ApiSettings").Bind(apiSettings);

            if (string.IsNullOrEmpty(apiSettings.GetSiteListURL) || string.IsNullOrEmpty(apiSettings.GetSiteDetailsURL))
            {
                Console.WriteLine("API settings are not configured properly in appsettings.json.");
                WriteLog.WriteLogFile("API settings are not configured properly in appsettings.json.");
                return;
            }
            var blSite = new BLDTSIntegration(configuration);
            WriteLog.WriteLogFile("Process started at: " + DateTime.Now.ToString());
            // Procedure to Entry LogIn Process Summary
            var processId = blSite.EntryLogInProcessSummary();
            //procedure to get Access Token
            GetAccessToken();
            //Procedure to get All Sites List in DTS Domain and Save in DB
            ConsumeSiteListApi(configuration, processId);
            //Procedure to get Site Details by Site ID and Save in DB
            CheckandUpdateSite(processId, configuration);
            //Procedure to Exit LogIn Process Summary
            blSite.ExitLogInProcessSummary(processId);
            WriteLog.WriteLogFile("Process completed for Process ID :" + processId + "at: " + DateTime.Now.ToString());
            Console.WriteLine("Process ID: " + processId);
        }

        static void ConsumeSiteListApi(IConfiguration configuration, int processId)
        {
            WriteLog.WriteLogFile("ConsumeApiAsync started");
            var blSite = new BLDTSIntegration(configuration);
            List<SiteList> sitelst = new List<SiteList>();
            try
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        accessToken = accessToken.Trim();
                        WriteLog.WriteLogFile("token " + accessToken);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                        WriteLog.WriteLogFile("Calling API: " + apiSettings.GetSiteListURL);
                        //var requestUri = new Uri($"{apiSettings.GetSiteListURL}"); //For Local Environment Testing 
                        var requestUri = new Uri($"{apiSettings.GetSiteListURL}?Status=live");
                        WriteLog.WriteLogFile("Request URI: " + requestUri);
                        HttpResponseMessage response = client.GetAsync(requestUri).GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            var siteList = JsonConvert.DeserializeObject<SiteList>(responseBody);
                            sitelst.Add(siteList);
                            Console.WriteLine(responseBody);
                            WriteLog.WriteLogFile("API Response: " + responseBody);
                        }
                        else
                        {
                            WriteLog.WriteLogFile($"Failed to call the API. Status code: {response.StatusCode}");
                            Console.WriteLine($"Failed to call the API. Status code: {response.StatusCode}");
                        }
                    }
                    var sites = sitelst[0].Response.ToList();
                    blSite.SaveSitesList(sites, processId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                WriteLog.WriteLogFile("Exception caught at ConsumeApiAsync: " + ex.Message);
            }
        }

        static void CheckandUpdateSite(int progressID, IConfiguration config)
        {
            WriteLog.WriteLogFile("CheckandUpdateSite start");
            try
            {
                var blSiteSync = new BLDTSIntegration(config);
                var sitelist = blSiteSync.GetSiteIdsByProcessId(progressID);
                foreach (var siteID in sitelist)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                        var requestUri = new Uri($"{apiSettings.GetSiteDetailsURL}?Site_Id={siteID}");
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
                                WriteLog.WriteLogFile("Site Details: " + siteToSave.site_id + " " + siteToSave.site_name);
                                blSiteSync.SaveSiteDetails(siteToSave, 1, progressID);
                            }
                        }
                        else
                        {
                            var message = $"Failed to call the API. Status code: {response.StatusCode}";
                            blSiteSync.UpdateSiteAsync(siteID, progressID, message);
                            WriteLog.WriteLogFile(message);
                        }
                    }
                }
                if (sitelist.Count > 0)
                {
                    blSiteSync.SaveSiteDetilsInMainTable(progressID);
                    blSiteSync.UpdateNetworkandGeomDetails(progressID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                WriteLog.WriteLogFile("Exception caught at CheckandUpdateSite: " + ex.Message);
            }
        }


        static void GetAccessToken()
        {
            WriteLog.WriteLogFile("Getting Access Token");
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var bearerToken = apiSettings.bearerToken;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);


                    var requestUri = new Uri($"{apiSettings.AccessTokenEndpoint}?{apiSettings.AccessKey}={apiSettings.AccessValue}");
                    WriteLog.WriteLogFile("requestUri: " + requestUri);

                    HttpResponseMessage response = client.PostAsync(requestUri, null).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(responseBody);
                        accessToken = accessTokenResponse.access_token;
                        WriteLog.WriteLogFile("API Access Token: " + accessToken);
                        WriteLog.WriteLogFile("API Response: " + responseBody);
                    }
                    else
                    {
                        WriteLog.WriteLogFile($"Failed to call the API. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogFile("Exception caught at GetAccessToken: " + ex.Message);
                }
            }
        }
    }
}
