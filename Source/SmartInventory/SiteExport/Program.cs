using Models;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogics.DaFiFeasibilityAPI;
using System.ComponentModel;
using System.IO;
using BusinessLogics;
using SmartInventory.Settings;

namespace SiteExport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.WriteDebugLog("----start---------");
            program.UpdateSiteFiberDistance();
            program.WriteDebugLog("----end---------");

        }
        public void UpdateSiteFiberDistance()
        {
            WriteDebugLog("----start UpdateSiteFiberDistance---------");

            string connectionString  = ConfigurationManager.AppSettings["constr"].ToString();
            string mapkey = ConfigurationManager.AppSettings["MapKey"];
            var siteList = new List<NearestSiteDetails>();
            var nearestSiteList = new List<NearestSiteDetails>();
            siteList = GetAllFilteredSite();
            var nearlinegeom = "";
            foreach (var site in siteList)
            {
                nearestSiteList = new BLSite().getNearrestSitelistData(site.system_id, site.network_id, ApplicationSettings.SiteBuffer);

                if (nearestSiteList != null && nearestSiteList.Count > 0)
                {
                    var route = GoogleDirectionsServiceHelper.GetRouteGeoJsonAndLength(site.sp_geometry, nearestSiteList[0].nearest_cable_end_geom, mapkey);
                    if (route.Result.LengthInMeters > 1)
                    {
                        var newbuilt = JsonConvert.DeserializeObject<GeoJsonLineString>(route.Result.GeoJson);
                        string lineGeom = string.Empty;
                        string[] siteGeomParts = site.sp_geometry.Split(' ');

                        lineGeom = siteGeomParts[1] + " " + siteGeomParts[0] + ",";
                        foreach (var cordinates in newbuilt.coordinates)
                        {
                            lineGeom += cordinates[0].ToString() + " " + cordinates[1].ToString() + ",";
                        }
                        lineGeom = lineGeom.TrimEnd(',');
                        nearlinegeom = lineGeom;
                    }
                    else
                    {
                        nearlinegeom = nearestSiteList[0].nearest_cable_end_geom;
                    }
                }

                if (nearestSiteList.Count >= 1)
                {
                    new BLSite().getUpdateSiteFiberDistance(nearestSiteList[0].line_geometry, nearestSiteList[0].system_id, site.system_id, nearestSiteList[0].distance, nearestSiteList[0].nearest_cable_end_geom, nearlinegeom, nearestSiteList[0].nearest_cable_system_id);
                }
            }
            WriteDebugLog("----end UpdateSiteFiberDistance---------");

        }
        public List<NearestSiteDetails> GetAllFilteredSite()
        {
            var result = new List<NearestSiteDetails>();
            string connectionString = ConfigurationManager.AppSettings["constr"].ToString();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand("SELECT * FROM fn_get_site_list()", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string jsonData = reader.GetString(0);
                                var site = JsonConvert.DeserializeObject<NearestSiteDetails>(jsonData);

                                result.Add(site);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDebugLog("Message : "+ ex.Message +" " + "StackTrace : " +ex.StackTrace);
            }
            return result;
        }
        public List<NearestSiteDetails> GetNearestSite(int system_id, string network_id)
        {
            var result = new List<NearestSiteDetails>();
            string connectionString = ConfigurationManager.AppSettings["constr"].ToString();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand("SELECT * FROM fn_get_nearest_site_records(@p_system_id, @p_network_id,@buffer)", connection))
                    {
                        command.Parameters.AddWithValue("@p_system_id", system_id);
                        command.Parameters.AddWithValue("@p_network_id", network_id);
                        command.Parameters.AddWithValue("@buffer", 100);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string jsonData = reader.GetString(0);
                                var site = JsonConvert.DeserializeObject<NearestSiteDetails>(jsonData);
                                result.Add(site);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDebugLog("Message : " + ex.Message + " " + "StackTrace : " + ex.StackTrace);
            }
            return result;
        }
        public void GetUpdateSiteFiberDistance(string lineString, int nearestSiteSystemId, int SiteSystemId,double nearestSiteDistance)
        {
            string connectionString = ConfigurationManager.AppSettings["constr"].ToString();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand("SELECT fn_get_update_site_fiber_details(@line, @nearest_id, @site_id, @nearestsite_distance)", connection))
                    {
                        command.Parameters.AddWithValue("@line", lineString);
                        command.Parameters.AddWithValue("@nearest_id", nearestSiteSystemId);
                        command.Parameters.AddWithValue("@site_id", SiteSystemId);
                        command.Parameters.AddWithValue("@nearestsite_distance", nearestSiteDistance);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                WriteDebugLog("Message : " + ex.Message + " " + "StackTrace : " + ex.StackTrace);
            }
        }
        public void WriteDebugLog(string LogMessage)
        {
            try
            {
                string _logFolderName = ConfigurationManager.AppSettings["LogFolderName"].ToString(); ;
                    // Ensure the directory exists
                    string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logFolderName);
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                   string logFilePath = Path.Combine(logDirectory, "DebugLog-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");

                    using (StreamWriter sw = File.AppendText(logFilePath))
                    {
                        // Optional: add timestamp for each log entry
                        sw.WriteLine($"======={DateTime.Now:yyyy-MM-dd HH:mm:ss} - {LogMessage}==========");
                    }
                }
            catch (Exception ex)
            {
                // Handle any exceptions, maybe log to console or ignore
                Console.WriteLine("Failed to write log: " + ex.Message);
            }
        }
    }
}
