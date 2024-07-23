using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Models.GoogleMapAPI;

namespace GoogleMapServices
{
    public class GoogleMapService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string apiKey;

        public GoogleMapService(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<GeoCodeResult> Geocode(GeocodeRequest request)
        {
            var url = BuildGeocodeUrl(request);// Dynamic URL
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GeoCodeResult>(responseBody);
        }

        public async Task<ReverseGeoCodeResult> ReverseGeocode(ReverseGeocodeRequest request)
        {
            var url = BuildReverseGeocodeUrl(request);//Dynamic URL
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReverseGeoCodeResult>(responseBody);
        }

        public async Task<DirectionResult> GetDirections(DirectionsRequest request)
        {
            var url = BuildDirectionsUrl(request);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DirectionResult>(responseBody);
        }

        public async Task<DistanceMatrixResult> GetDistanceMatrix(DistanceMatrixRequest request)
        {
            var url = BuildDistanceMatrixUrl(request);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DistanceMatrixResult>(responseBody);
        }
        public async Task<PlaceDetailsResult> GetPlaceDetails(PlaceDetailsRequest request)
        {
            var url = BuildPlaceDetailsUrl(request);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PlaceDetailsResult>(responseBody);
        }
        //
        public async Task<PlaceSearchResult> PlaceSearchDetails(PlaceSearchRequest request)
        {
            var url = BuildPlaceSearchUrl(request);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PlaceSearchResult>(responseBody);
        }

        public async Task<PlaceAutocompleteResult> GetPlaceAutocomplete(PlaceAutocompleteRequest request)
        {
            var url = BuildPlaceAutocompleteUrl(request);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PlaceAutocompleteResult>(responseBody);
        }

        private string BuildGeocodeUrl(GeocodeRequest request)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?key={apiKey}";

            if (!string.IsNullOrEmpty(request.Address))
            {
                url += $"&address={HttpUtility.UrlEncode(request.Address)}";
            }
            if (!string.IsNullOrEmpty(request.Components))
            {
                url += $"&components={HttpUtility.UrlEncode(request.Components)}";
            }
            if (!string.IsNullOrEmpty(request.Bounds))
            {
                url += $"&bounds={HttpUtility.UrlEncode(request.Bounds)}";
            }
            if (!string.IsNullOrEmpty(request.Region))
            {
                url += $"&region={HttpUtility.UrlEncode(request.Region)}";
            }
            if (!string.IsNullOrEmpty(request.Language))
            {
                url += $"&language={HttpUtility.UrlEncode(request.Language)}";
            }

            return url;
        }
        private string BuildReverseGeocodeUrl(ReverseGeocodeRequest request)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={request.Latitude},{request.Longitude}&key={apiKey}";

            if (!string.IsNullOrEmpty(request.LocationType))
            {
                url += $"&location_type={HttpUtility.UrlEncode(request.LocationType)}";
            }
            if (!string.IsNullOrEmpty(request.ResultType))
            {
                url += $"&result_type={HttpUtility.UrlEncode(request.ResultType)}";
            }
            if (!string.IsNullOrEmpty(request.Language))
            {
                url += $"&language={HttpUtility.UrlEncode(request.Language)}";
            }
            return url;
        }
        private string BuildDirectionsUrl(DirectionsRequest request)
        {
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={HttpUtility.UrlEncode(request.Origin)}&destination={HttpUtility.UrlEncode(request.Destination)}&key={apiKey}";

            if (!string.IsNullOrEmpty(request.Mode))
            {
                url += $"&mode={HttpUtility.UrlEncode(request.Mode)}";
            }
            if (request.Waypoints != null && request.Waypoints.Length > 0)
            {
                url += $"&waypoints={string.Join("|", request.Waypoints.Select(HttpUtility.UrlEncode))}";
            }
            if (!string.IsNullOrEmpty(request.Avoid))
            {
                url += $"&avoid={HttpUtility.UrlEncode(request.Avoid)}";
            }
            if (!string.IsNullOrEmpty(request.Language))
            {
                url += $"&language={HttpUtility.UrlEncode(request.Language)}";
            }
            if (!string.IsNullOrEmpty(request.Units))
            {
                url += $"&units={HttpUtility.UrlEncode(request.Units)}";
            }
            if (!string.IsNullOrEmpty(request.Region))
            {
                url += $"&region={HttpUtility.UrlEncode(request.Region)}";
            }

            return url;
        }
        private string BuildDistanceMatrixUrl(DistanceMatrixRequest request)
        {
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={string.Join("|", request.Origins.Select(HttpUtility.UrlEncode))}&destinations={string.Join("|", request.Destinations.Select(HttpUtility.UrlEncode))}&key={apiKey}";

            if (!string.IsNullOrEmpty(request.Mode))
            {
                url += $"&mode={HttpUtility.UrlEncode(request.Mode)}";
            }
            if (!string.IsNullOrEmpty(request.Language))
            {
                url += $"&language={HttpUtility.UrlEncode(request.Language)}";
            }
            if (!string.IsNullOrEmpty(request.Units))
            {
                url += $"&units={HttpUtility.UrlEncode(request.Units)}";
            }
            if (!string.IsNullOrEmpty(request.Avoid))
            {
                url += $"&avoid={HttpUtility.UrlEncode(request.Avoid)}";
            }
            if (!string.IsNullOrEmpty(request.DepartureTime))
            {
                url += $"&departure_time={HttpUtility.UrlEncode(request.DepartureTime)}";
            }
            if (!string.IsNullOrEmpty(request.ArrivalTime))
            {
                url += $"&arrival_time={HttpUtility.UrlEncode(request.ArrivalTime)}";
            }
            if (!string.IsNullOrEmpty(request.TrafficModel))
            {
                url += $"&traffic_model={HttpUtility.UrlEncode(request.TrafficModel)}";
            }

            return url;
        }

        private string BuildPlaceSearchUrl(PlaceSearchRequest request)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={HttpUtility.UrlEncode(request.Query)}&key={apiKey}";

            if (!string.IsNullOrEmpty(request.Location))
            {
                url += $"&location={HttpUtility.UrlEncode(request.Location)}";
            }
            if (!string.IsNullOrEmpty(request.Radius))
            {
                url += $"&radius={HttpUtility.UrlEncode(request.Radius)}";
            }
            if (!string.IsNullOrEmpty(request.Type))
            {
                url += $"&type={HttpUtility.UrlEncode(request.Type)}";
            }
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                url += $"&keyword={HttpUtility.UrlEncode(request.Keyword)}";
            }
            if (!string.IsNullOrEmpty(request.MinPrice))
            {
                url += $"&minprice={HttpUtility.UrlEncode(request.MinPrice)}";
            }
            if (!string.IsNullOrEmpty(request.MaxPrice))
            {
                url += $"&maxprice={HttpUtility.UrlEncode(request.MaxPrice)}";
            }
            if (!string.IsNullOrEmpty(request.RankBy))
            {
                url += $"&rankby={HttpUtility.UrlEncode(request.RankBy)}";
            }
            if (!string.IsNullOrEmpty(request.Language))
            {
                url += $"&language={HttpUtility.UrlEncode(request.Language)}";
            }
            if (request.OpenNow)
            {
                url += $"&opennow=true";
            }
            if (!string.IsNullOrEmpty(request.PageToken))
            {
                url += $"&pagetoken={HttpUtility.UrlEncode(request.PageToken)}";
            }

            return url;
        }
        private string BuildPlaceDetailsUrl(PlaceDetailsRequest request)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={HttpUtility.UrlEncode(request.PlaceId)}&key={apiKey}";

            if (!string.IsNullOrEmpty(request.Language))
            {
                url += $"&language={HttpUtility.UrlEncode(request.Language)}";
            }
            if (!string.IsNullOrEmpty(request.Region))
            {
                url += $"&region={HttpUtility.UrlEncode(request.Region)}";
            }

            return url;
        }

        private string BuildPlaceAutocompleteUrl(PlaceAutocompleteRequest request)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={HttpUtility.UrlEncode(request.Input)}&key={apiKey}";

            if (!string.IsNullOrEmpty(request.Offset))
            {
                url += $"&offset={HttpUtility.UrlEncode(request.Offset)}";
            }
            if (!string.IsNullOrEmpty(request.Location))
            {
                url += $"&location={HttpUtility.UrlEncode(request.Location)}";
            }
            if (!string.IsNullOrEmpty(request.Radius))
            {
                url += $"&radius={HttpUtility.UrlEncode(request.Radius)}";
            }
            if (!string.IsNullOrEmpty(request.Language))
            {
                url += $"&language={HttpUtility.UrlEncode(request.Language)}";
            }
            if (!string.IsNullOrEmpty(request.Types))
            {
                url += $"&types={HttpUtility.UrlEncode(request.Types)}";
            }
            if (!string.IsNullOrEmpty(request.Components))
            {
                url += $"&components={HttpUtility.UrlEncode(request.Components)}";
            }
            if (!string.IsNullOrEmpty(request.StrictBounds))
            {
                url += $"&strictbounds={HttpUtility.UrlEncode(request.StrictBounds)}";
            }

            return url;
        }
    }
}
