using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Models.GoogleMapAPI;

namespace BusinessLogics.DaFiFeasibilityAPI
{
    public class GoogleDirectionsServiceHelper
    {
        public static async Task<(string GeoJson, double LengthInMeters)> GetRouteGeoJsonAndLength(string origin, string destination, string apiKey, string travelMode="walking")
        {
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&key={apiKey}&mode={travelMode}";

            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                using (System.IO.Stream resData = response.GetResponseStream())
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(resData))
                    {
                        string responseText = reader.ReadToEnd();
                        var directionsResponse = JsonConvert.DeserializeObject<DirectionResult>(responseText);

                        if (directionsResponse.Routes != null && directionsResponse.Routes.Count() > 0)
                        {
                            var route = directionsResponse.Routes[0];
                            string geoJson = ConvertPolylineToGeoJson(route.OverviewPolyline.Points);

                            // Calculate total length
                            double totalLengthInMeters = 0;
                            foreach (var leg in route.Legs)
                            {
                                totalLengthInMeters += leg.Distance.Value;
                            }

                            return (geoJson, totalLengthInMeters);
                        }

                        throw new Exception("No route found.");
                    }
                }
            }


            //using (HttpClient client = new HttpClient())
            //{
            //    var response = await client.GetAsync(url);

            //    var directionsResponse = JsonConvert.DeserializeObject<DirectionResult>(response);

            //    if (directionsResponse.Routes != null && directionsResponse.Routes.Count() > 0)
            //    {
            //        var route = directionsResponse.Routes[0];
            //        string geoJson = ConvertPolylineToGeoJson(route.OverviewPolyline.Points);

            //        // Calculate total length
            //        double totalLengthInMeters = 0;
            //        foreach (var leg in route.Legs)
            //        {
            //            totalLengthInMeters += leg.Distance.Value; 
            //        }

            //        return (geoJson, totalLengthInMeters);
            //    }

            //    throw new Exception("No route found.");
            //}
        }

        private static string ConvertPolylineToGeoJson(string polyline)
        {
            var decodedPolyline = PolylineDecode.DecodePolyline(polyline);
            var geoJson = new
            {
                type = "LineString",
                coordinates = new List<List<double>>()
            };

            foreach (var point in decodedPolyline)
            {
                geoJson.coordinates.Add(new List<double> { point.longitude, point.latitude });
            }

            return JsonConvert.SerializeObject(geoJson);
        }
    }

    public static class PolylineDecode
    {
        public static List<(double latitude, double longitude)> DecodePolyline(string encodedPolyline)
        {
            var polyline = new List<(double latitude, double longitude)>();
            int index = 0, len = encodedPolyline.Length;
            int lat = 0, lng = 0;

            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encodedPolyline[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;
                do
                {
                    b = encodedPolyline[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                var latitude = lat * 1e-5;
                var longitude = lng * 1e-5;
                polyline.Add((latitude, longitude));
            }

            return polyline;
        }
    }
}
