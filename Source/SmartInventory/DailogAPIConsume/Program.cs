using BusinessLogics;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DailogAPIConsume
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            SiteSync siteSync = new SiteSync();
            siteSync.start_datetime = DateTime.Now;
            siteSync.status = "Processing";

            var obj=new BLSiteSync().Save(siteSync);
            var sitelst = await ConsumeApiAsync();
            await CheckandUpdateSite(sitelst);

            obj.end_datetime = DateTime.Now;
            obj.status = "Success";
            obj.lastsuccess_sync = DateTime.Now;
            new BLSiteSync().Save(siteSync);
        }

        static async Task<List<SiteList>> ConsumeApiAsync()
        {
            List<SiteList> sitelst = new List<SiteList>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.example.com/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("endpoint/path");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        var x = Newtonsoft.Json.JsonConvert.DeserializeObject<SiteList>(responseBody);
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
            
            var syncStatus = new BLSiteSync().GetAll();
            
            var latestSuccessItem = syncStatus
             .Where(item => item.status == "Success")
             .OrderByDescending(item => item.lastsuccess_sync)
             .FirstOrDefault();

            BLSite bLSite = new BLSite();
            var lst = bLSite.GetAll(latestSuccessItem.lastsuccess_sync);

            var matchingIds = dailogSites.Select(item =>item.Site_Id ).ToList();
            var matchingItems = lst.Where(item => matchingIds.Contains(item.site_id)).ToList();
            foreach ( var item in matchingItems )
            {
                Site site = new Site();

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.example.com/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("endpoint/path");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        site = Newtonsoft.Json.JsonConvert.DeserializeObject<Site>(responseBody);

                        bLSite.Save(site,1);
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

