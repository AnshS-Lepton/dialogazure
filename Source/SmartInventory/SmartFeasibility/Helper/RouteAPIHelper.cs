using Models.Feasibility;
using Newtonsoft.Json;
using SmartFeasibility.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SmartFeasibility.Helper
{
    public class RoutingAuth
    {
        //{"id":1,"firstName":"Avanish","lastName":"User","username":"avanish","password":null,"token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJuYmYiOjE1ODk3ODI1NjEsImV4cCI6MTU5MDM4NzM2MSwiaWF0IjoxNTg5NzgyNTYxfQ.zvKMlWfm7DixvW1wVEWfAnEHrzc0xAEsmDkEi1OhLe8"}
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string token { get; set; }
    }

    public class RouteAPIHelper
    {
        public string authenticate()
        {
            try
            {
                string sURL = ApplicationSettings.RoutingAPIUrl + "api/users/authenticate";
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);

                wrGETURL.Method = "POST";
                wrGETURL.ContentType = @"application/json;";
                using (var stream = new StreamWriter(wrGETURL.GetRequestStream()))
                {
                    var bodyContent = new
                    {
                        Username = "avanish",
                        Password = "lepton@123"
                    };

                    var json = JsonConvert.SerializeObject(bodyContent);

                    stream.Write(json);
                }
                HttpWebResponse webresponse = wrGETURL.GetResponse() as HttpWebResponse;

                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                string strResult = loResponseStream.ReadToEnd();
                loResponseStream.Close();
                webresponse.Close();

                RoutingAuth res = JsonConvert.DeserializeObject<RoutingAuth>(strResult);

                return res.token;
            }
            catch
            {
                return "";
            }
        }

        public List<DirectionsDetail> getRoutes(string src, string desti, double sourceBuffer, double destiBuffer)
        {
            List<DirectionsDetail> directionDetails = new List<DirectionsDetail>();
            string token = authenticate();
            int[] wp = { };
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    string sURL = ApplicationSettings.RoutingAPIUrl + "api/roadnetwork/roaddirections";
                    WebRequest wrGETURL;
                    wrGETURL = WebRequest.Create(sURL);

                    wrGETURL.Method = "POST";
                    wrGETURL.ContentType = @"application/json;";
                    wrGETURL.Headers.Add("Authorization", "Bearer " + token);
                    using (var stream = new StreamWriter(wrGETURL.GetRequestStream()))
                    {
                        var bodyContent = new
                        {
                            Src_Lat = 38.9899,
                            Src_Long = 38.9899,
                            Dest_Lat = 38.9899,
                            Dest_Long = 38.9899,
                            Source = src,
                            Destination = desti,
                            StartPointRadius = Convert.ToInt32(sourceBuffer),
                            EndPointRadius = Convert.ToInt32(destiBuffer),
                            Waypoints = wp,
                            IsDirected = true
                        };

                        var json = JsonConvert.SerializeObject(bodyContent);

                        stream.Write(json);
                    }
                    HttpWebResponse webresponse = wrGETURL.GetResponse() as HttpWebResponse;

                    Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                    string strResult = loResponseStream.ReadToEnd();
                    loResponseStream.Close();
                    webresponse.Close();

                    directionDetails = JsonConvert.DeserializeObject<List<DirectionsDetail>>(strResult);

                }
            }
            catch
            {
                return directionDetails;
            }

            return directionDetails;
        }
    }
}