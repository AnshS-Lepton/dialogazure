using DialogDTSIntegration.BLDTS;
using Models;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DialogDTSIntegration
{
    
    class Program
    {
        static async Task Main(string[] args)
        {
            //await Task.Delay(150000);
            SiteSync siteSync = new SiteSync();
            siteSync.start_datetime = DateTime.Now;
            siteSync.status = "Processing";

            //var obj = new BLDTS.BLSiteSync().Save(siteSync);
            var sitelst = await ConsumeApiAsync();
            await CheckandUpdateSite(sitelst);

            //obj.end_datetime = DateTime.Now;
            //obj.status = "Success";
            //obj.lastsuccess_sync = DateTime.Now;
            //new BLDTS.BLSiteSync().Save(siteSync);
        }

        static async Task<List<SiteList>> ConsumeApiAsync()
        {
            List<SiteList> sitelst = new List<SiteList>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8070/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("api/values/GetSiteListMaster");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        var x = JsonConvert.DeserializeObject<SiteList>(responseBody);
                        sitelst.Add(x);
                        Console.WriteLine(responseBody);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to call the API. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
            return sitelst;
        }

        static async Task CheckandUpdateSite(List<SiteList> sites)
        {
            var dailogSites = sites[0].Response.ToList();

           // var syncStatus = new BLDTS.BLSiteSync().GetAll();

            //var latestSuccessItem = syncStatus
            // .Where(item => item.status == "Success")
            // .OrderByDescending(item => item.lastsuccess_sync)
            // .FirstOrDefault();
            //DateTime latestSuccessItem = DateTime.Now.AddMinutes(-5);
            //BLSite bLSite = new BLSite();
            //var lst = bLSite.GetAll(latestSuccessItem);

            //var matchingIds = dailogSites.Select(item => item.Site_Id).ToList();
            //var matchingItems = lst.Where(item => matchingIds.Contains(item.site_id)).ToList();

            foreach (var item in dailogSites)
            {
                Site site = new Site();

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8070/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync($"api/values/GetSiteDetailsById/{item.Site_Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        //site = Newtonsoft.Json.JsonConvert.DeserializeObject<Site>(responseBody);
                        var apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                        //site = apiResponse.Response[0];
                        if (apiResponse?.Response != null && apiResponse.Response.Count > 0)
                        {
                            var siteToSave = apiResponse.Response[0];
                            BLSite.Save(siteToSave, 1);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to call the API. Status code: {response.StatusCode}");
                    }
                }
            }
        }
    }
}
