using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Models.API;
using Newtonsoft.Json;

namespace ServiceabilityWinService.Utility
{
    public static class IntegrationWebRequest
    {
        static readonly string baseAPIUrl = new Uri(ConfigurationManager.AppSettings["ServiceUrl"]).ToString();
        public static GTokenDetail GetAPIToken(string userName, string password, string source)
        {
            var pairs = new List<KeyValuePair<string, string>>
                    {
                       new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "Password", password ),
                        new KeyValuePair<string, string> ( "source", source )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(baseAPIUrl + "GenerateToken", content).Result;
                return JsonConvert.DeserializeObject<GTokenDetail>(response.Content.ReadAsStringAsync().Result);
            }
        }
        public static ApiResponse<T> PostIntegrationAPIRequest<T>(string url, string access_token, string requestId, string latitude, string longitude, string segment) where T : class, new()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(access_token))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
                }
              
                var data = new[]
                {
                      new KeyValuePair<string, string>("latitude", latitude),
                      new KeyValuePair<string, string>("longitude", longitude),
                      new KeyValuePair<string, string>("requestId", requestId),
                      new KeyValuePair<string, string>("segment", segment)
                };

                var response = client.PostAsync(baseAPIUrl + url, new FormUrlEncodedContent(data)).Result;
                return JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
    [Serializable]
    public class GTokenDetail
    {
        public string status { get; set; }
        public string error_message { get; set; }
        public dynamic result { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
