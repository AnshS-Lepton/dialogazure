using GoogleMapServices;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static Models.GoogleMapAPI;
using Utility;
using SmartInventoryServices.Filters;

namespace SmartInventoryServices.Controllers
{
    [RoutePrefix("api/mapsapi")]
    [APIExceptionFilter]
    public class GoogleMapAPIController : ApiController
    {
        public readonly GoogleMapService googleApiService;
        public GoogleMapAPIController()
        {
            // Initialize with your API key
            string apiKey = ConfigurationManager.AppSettings["GoogleMapsBackendAPIKey"].ToString();
            googleApiService = new GoogleMapService(apiKey);
        }

        [HttpPost]
        [Route("geocode")]
        public async Task<GeoCodeResult> Geocode(GeocodeRequest request)
        {
            var response = new GeoCodeResult();
            if (request == null || string.IsNullOrEmpty(request.Address))
            {
                response.Status = ResponseStatus.ERROR.ToString();
                response.Results = null;
                return response;
            }

            GeoCodeResult geocodeResult = await googleApiService.Geocode(request);
            response.Status = geocodeResult.Status;
            response.Results = geocodeResult.Results;
            return response;
        }

        [HttpPost]
        [Route("reversegeocode")]
        public async Task<ReverseGeoCodeResult> ReverseGeocode([FromBody] ReverseGeocodeRequest request)
        {
            var response = new ReverseGeoCodeResult();
            if (request == null || request.Latitude == 0 || request.Longitude == 0)
            {
                response.Status = ResponseStatus.ERROR.ToString();
                response.Results = null;
                return response;
            }


            ReverseGeoCodeResult geocodeResult = await googleApiService.ReverseGeocode(request);
            response.Status = geocodeResult.Status;
            response.Results = geocodeResult.Results;
            response.PlusCode = geocodeResult.PlusCode;
            return response;
        }

        [HttpPost]
        [Route("directions")]
        public async Task<DirectionResult> Directions([FromBody] DirectionsRequest request)
        {
            var response = new DirectionResult();
            if (request == null || string.IsNullOrEmpty(request.Origin) || string.IsNullOrEmpty(request.Destination))
            {
                response.Status = ResponseStatus.ERROR.ToString();
                return response;

            }

            DirectionResult directionsResult = await googleApiService.GetDirections(request);
            response.Status = directionsResult.Status;
            response.GeocodedWaypoints = directionsResult.GeocodedWaypoints;
            response.Routes = directionsResult.Routes;
            return response;
        }

        [HttpPost]
        [Route("distancematrix")]
        public async Task<DistanceMatrixResult> DistanceMatrix([FromBody] DistanceMatrixRequest request)
        {
            var response = new DistanceMatrixResult();
            if (request == null || request.Origins == null || request.Origins.Length == 0 || request.Destinations == null || request.Destinations.Length == 0)
            {
                response.Status = ResponseStatus.ERROR.ToString();
                return response;
            }
            DistanceMatrixResult distanceMatrixResult = await googleApiService.GetDistanceMatrix(request);
            response.Status = distanceMatrixResult.Status;
            response.DestinationAddresses = distanceMatrixResult.DestinationAddresses;
            response.Rows = distanceMatrixResult.Rows;
            response.OriginAddresses = distanceMatrixResult.OriginAddresses;

            return response;
        }

        [HttpPost]
        [Route("placesearch")]
        public async Task<PlaceSearchResult> PlaceSearch([FromBody] PlaceSearchRequest request)
        {
            var response = new PlaceSearchResult();
            if (request == null || string.IsNullOrEmpty(request.Query))
            {
                response.Status = ResponseStatus.ERROR.ToString();
                return response;
            }


            PlaceSearchResult placeResult = await googleApiService.PlaceSearchDetails(request);
            response.Status = placeResult.Status;
            response.Results = placeResult.Results;
            response.NextPageToken = placeResult.NextPageToken;
            response.HtmlAttributions = placeResult.HtmlAttributions;

            return response;
        }

        [HttpPost]
        [Route("placedetails")]
        public async Task<PlaceDetailsResult> PlaceDetails([FromBody] PlaceDetailsRequest request)
        {
            var response = new PlaceDetailsResult();
            if (request == null || string.IsNullOrEmpty(request.PlaceId))
            {
                response.Status = ResponseStatus.ERROR.ToString();
                return response;
            }

            PlaceDetailsResult placeResult = await googleApiService.GetPlaceDetails(request);
            response.Status = placeResult.Status;
            response.Result = placeResult.Result;
            response.HtmlAttributions = placeResult.HtmlAttributions;

            return response;
        }

        [HttpPost]
        [Route("placeautocomplete")]
        public async Task<PlaceAutocompleteResult> PlaceAutocomplete([FromBody] PlaceAutocompleteRequest request)
        {
            var response = new PlaceAutocompleteResult();
            if (request == null || string.IsNullOrEmpty(request.Input))
            {
                response.Status = ResponseStatus.ERROR.ToString();
                return response;
            }

            PlaceAutocompleteResult placeResult = await googleApiService.GetPlaceAutocomplete(request);
            response.Status = placeResult.Status;
            response.Predictions = placeResult.Predictions;

            return response;
        }

    }
}