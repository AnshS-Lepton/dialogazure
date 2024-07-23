using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GoogleMapAPI
    {
        public class ReverseGeocodeRequest
        {
            public double Latitude { get; set; }  // Required
            public double Longitude { get; set; }   // Required
            public string LocationType { get; set; } // e.g., "ROOFTOP"
            public string ResultType { get; set; } // e.g., "street_address"
            public string Language { get; set; } // e.g., "en"
        }
        public class GeocodeRequest
        {
            public string Address { get; set; }  // Required
            public string Components { get; set; } // e.g., "country:IN|postal_code:94043"
            public string Bounds { get; set; } // e.g., "34.172684,-118.604794|34.236144,-118.500938"
            public string Region { get; set; } // e.g., "us"
            public string Language { get; set; } // e.g., "en"
        }
        public class DirectionsRequest
        {
            public string Origin { get; set; } // Required
            public string Destination { get; set; } // Required
            public string Mode { get; set; } // e.g., "driving", "walking", "bicycling", "transit"
            public string[] Waypoints { get; set; } // Optional, intermediate waypoints
            public string Avoid { get; set; } // e.g., "tolls", "highways", "ferries"
            public string Language { get; set; } // e.g., "en"
            public string Units { get; set; } // e.g., "metric", "imperial"
            public string Region { get; set; } // e.g., "us"
        }
        //public class PlaceRequest
        //{
        //    public string Query { get; set; }
        //}
        public class PlaceSearchRequest
        {
            public string Query { get; set; } // e.g., "restaurants in New York"
            public string Location { get; set; } // e.g., "40.748817,-73.985428"
            public string Radius { get; set; } // e.g., "5000"
            public string Type { get; set; } // e.g., "restaurant"
            public string Keyword { get; set; }
            public string MinPrice { get; set; }
            public string MaxPrice { get; set; }
            public string RankBy { get; set; } // e.g., "prominence" or "distance"
            public string Language { get; set; } // e.g., "en"
            public bool OpenNow { get; set; }
            public string PageToken { get; set; }
        }

        public class PlaceDetailsRequest
        {
            public string PlaceId { get; set; } // Required
            public string Language { get; set; } // e.g., "en"
            public string Region { get; set; }
        }

        public class PlaceAutocompleteRequest
        {
            public string Input { get; set; } // Required
            public string Offset { get; set; }
            public string Location { get; set; } // e.g., "40.748817,-73.985428"
            public string Radius { get; set; } // e.g., "5000"
            public string Language { get; set; } // e.g., "en"
            public string Types { get; set; } // e.g., "geocode"
            public string Components { get; set; } // e.g., "country:us"
            public string StrictBounds { get; set; }
        }
        public class DistanceMatrixRequest
        {
            public string[] Origins { get; set; } // Required
            public string[] Destinations { get; set; } // Required
            public string Mode { get; set; } // e.g., "driving", "walking", "bicycling", "transit"
            public string Language { get; set; } // e.g., "en"
            public string Units { get; set; } // e.g., "metric", "imperial"
            public string Avoid { get; set; } // e.g., "tolls", "highways", "ferries"
            public string DepartureTime { get; set; } // e.g., "now" or UNIX timestamp
            public string ArrivalTime { get; set; } // Optional
            public string TrafficModel { get; set; } // e.g., "best_guess", "pessimistic", "optimistic"
        }

        public partial class GeoCodeResult
        {
            [JsonProperty("results")]
            public Result[] Results { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }
        public partial class Result
        {
            [JsonProperty("address_components")]
            public AddressComponent[] AddressComponents { get; set; }

            [JsonProperty("formatted_address")]
            public string FormattedAddress { get; set; }

            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("plus_code")]
            public PlusCode PlusCode { get; set; }

            [JsonProperty("types")]
            public string[] Types { get; set; }
        }

        public partial class ReverseResult
        {
            [JsonProperty("address_components")]
            public AddressComponent[] AddressComponents { get; set; }

            [JsonProperty("formatted_address")]
            public string FormattedAddress { get; set; }

            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("plus_code", NullValueHandling = NullValueHandling.Ignore)]
            public PlusCode PlusCode { get; set; }

            [JsonProperty("types")]
            public string[] Types { get; set; }
        }
        public partial class AddressComponent
        {
            [JsonProperty("long_name")]
            public string LongName { get; set; }

            [JsonProperty("short_name")]
            public string ShortName { get; set; }

            [JsonProperty("types")]
            public string[] Types { get; set; }
        }
        public partial class Geometry
        {
            [JsonProperty("location")]
            public Location Location { get; set; }

            [JsonProperty("location_type")]
            public string LocationType { get; set; }

            [JsonProperty("viewport")]
            public Viewport Viewport { get; set; }

            [JsonProperty("bounds", NullValueHandling = NullValueHandling.Ignore)]
            public Bounds Bounds { get; set; }
        }
        public partial class Bounds
        {
            [JsonProperty("northeast")]
            public Location Northeast { get; set; }

            [JsonProperty("southwest")]
            public Location Southwest { get; set; }
        }
        public partial class Location
        {
            [JsonProperty("lat")]
            public double Lat { get; set; }

            [JsonProperty("lng")]
            public double Lng { get; set; }
        }

        public partial class Viewport
        {
            [JsonProperty("northeast")]
            public Location Northeast { get; set; }

            [JsonProperty("southwest")]
            public Location Southwest { get; set; }
        }

        public partial class PlusCode
        {
            [JsonProperty("compound_code")]
            public string CompoundCode { get; set; }

            [JsonProperty("global_code")]
            public string GlobalCode { get; set; }
        }

        public partial class ReverseGeoCodeResult
        {
            [JsonProperty("plus_code")]
            public PlusCode PlusCode { get; set; }

            [JsonProperty("results")]
            public ReverseResult[] Results { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public partial class DirectionResult
        {
            [JsonProperty("geocoded_waypoints")]
            public GeocodedWaypoint[] GeocodedWaypoints { get; set; }

            [JsonProperty("routes")]
            public Route[] Routes { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }
        public partial class GeocodedWaypoint
        {
            [JsonProperty("geocoder_status")]
            public string GeocoderStatus { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("types")]
            public string[] Types { get; set; }
        }
        public partial class Route
        {
            [JsonProperty("bounds")]
            public Bounds Bounds { get; set; }

            [JsonProperty("copyrights")]
            public string Copyrights { get; set; }

            [JsonProperty("legs")]
            public Leg[] Legs { get; set; }

            [JsonProperty("overview_polyline")]
            public Polyline OverviewPolyline { get; set; }

            [JsonProperty("summary")]
            public string Summary { get; set; }

            [JsonProperty("warnings")]
            public object[] Warnings { get; set; }

            [JsonProperty("waypoint_order")]
            public object[] WaypointOrder { get; set; }
        }

        public partial class Northeast
        {
            [JsonProperty("lat")]
            public double Lat { get; set; }

            [JsonProperty("lng")]
            public double Lng { get; set; }
        }

        public partial class Leg
        {
            [JsonProperty("distance")]
            public Distance Distance { get; set; }

            [JsonProperty("duration")]
            public Distance Duration { get; set; }

            [JsonProperty("end_address")]
            public string EndAddress { get; set; }

            [JsonProperty("end_location")]
            public Northeast EndLocation { get; set; }

            [JsonProperty("start_address")]
            public string StartAddress { get; set; }

            [JsonProperty("start_location")]
            public Northeast StartLocation { get; set; }

            [JsonProperty("steps")]
            public Step[] Steps { get; set; }

            [JsonProperty("traffic_speed_entry")]
            public object[] TrafficSpeedEntry { get; set; }

            [JsonProperty("via_waypoint")]
            public object[] ViaWaypoint { get; set; }
        }

        public partial class Distance
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("value")]
            public long Value { get; set; }
        }

        public partial class Step
        {
            [JsonProperty("distance")]
            public Distance Distance { get; set; }

            [JsonProperty("duration")]
            public Distance Duration { get; set; }

            [JsonProperty("end_location")]
            public Northeast EndLocation { get; set; }

            [JsonProperty("html_instructions")]
            public string HtmlInstructions { get; set; }

            [JsonProperty("polyline")]
            public Polyline Polyline { get; set; }

            [JsonProperty("start_location")]
            public Northeast StartLocation { get; set; }

            [JsonProperty("travel_mode")]
            public string TravelMode { get; set; }

            [JsonProperty("maneuver", NullValueHandling = NullValueHandling.Ignore)]
            public string Maneuver { get; set; }
        }

        public partial class Polyline
        {
            [JsonProperty("points")]
            public string Points { get; set; }
        }

        public partial class DistanceMatrixResult
        {
            [JsonProperty("destination_addresses")]
            public string[] DestinationAddresses { get; set; }

            [JsonProperty("origin_addresses")]
            public string[] OriginAddresses { get; set; }

            [JsonProperty("rows")]
            public Row[] Rows { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }
        public partial class Row
        {
            [JsonProperty("elements")]
            public Element[] Elements { get; set; }
        }
        public partial class Element
        {
            [JsonProperty("distance")]
            public Distance Distance { get; set; }

            [JsonProperty("duration")]
            public Distance Duration { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }
        public partial class PlaceSearchResult
        {
            [JsonProperty("html_attributions")]
            public object[] HtmlAttributions { get; set; }
            [JsonProperty("next_page_token")]
            public string NextPageToken { get; set; }

            [JsonProperty("results")]
            public PlaceResult[] Results { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public partial class PlaceResult
        {
            [JsonProperty("business_status")]
            public string BusinessStatus { get; set; }

            [JsonProperty("formatted_address")]
            public string FormattedAddress { get; set; }

            [JsonProperty("geometry")]
            public PlaceGeometry Geometry { get; set; }

            [JsonProperty("icon")]
            public Uri Icon { get; set; }

            [JsonProperty("icon_background_color")]
            public string IconBackgroundColor { get; set; }

            [JsonProperty("icon_mask_base_uri")]
            public Uri IconMaskBaseUri { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("opening_hours")]
            public OpeningHours OpeningHours { get; set; }

            [JsonProperty("photos")]
            public Photo[] Photos { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("plus_code")]
            public PlusCode PlusCode { get; set; }
            [JsonProperty("price_level", NullValueHandling = NullValueHandling.Ignore)]
            public long? PriceLevel { get; set; }

            [JsonProperty("rating")]
            public double Rating { get; set; }

            [JsonProperty("reference")]
            public string Reference { get; set; }

            [JsonProperty("types")]
            public string[] Types { get; set; }

            [JsonProperty("user_ratings_total")]
            public long UserRatingsTotal { get; set; }
        }
        public partial class PlaceGeometry
        {
            [JsonProperty("location")]
            public Location Location { get; set; }

            [JsonProperty("viewport")]
            public Viewport Viewport { get; set; }
        }
        public partial class OpeningHours
        {
            [JsonProperty("open_now")]
            public bool OpenNow { get; set; }
        }
        public partial class Photo
        {
            [JsonProperty("height")]
            public long Height { get; set; }

            [JsonProperty("html_attributions")]
            public string[] HtmlAttributions { get; set; }

            [JsonProperty("photo_reference")]
            public string PhotoReference { get; set; }

            [JsonProperty("width")]
            public long Width { get; set; }
        }

        public partial class PlaceAutocompleteResult
        {
            [JsonProperty("predictions")]
            public Prediction[] Predictions { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public partial class Prediction
        {
            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("matched_substrings")]
            public MatchedSubstring[] MatchedSubstrings { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("reference")]
            public string Reference { get; set; }

            [JsonProperty("structured_formatting")]
            public StructuredFormatting StructuredFormatting { get; set; }

            [JsonProperty("terms")]
            public Term[] Terms { get; set; }

            [JsonProperty("types")]
            public string[] Types { get; set; }
        }

        public partial class MatchedSubstring
        {
            [JsonProperty("length")]
            public long Length { get; set; }

            [JsonProperty("offset")]
            public long Offset { get; set; }
        }

        public partial class StructuredFormatting
        {
            [JsonProperty("main_text")]
            public string MainText { get; set; }

            [JsonProperty("main_text_matched_substrings")]
            public MatchedSubstring[] MainTextMatchedSubstrings { get; set; }

            [JsonProperty("secondary_text")]
            public string SecondaryText { get; set; }
        }

        public partial class Term
        {
            [JsonProperty("offset")]
            public long Offset { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public partial class PlaceDetailsResult
        {
            [JsonProperty("html_attributions")]
            public object[] HtmlAttributions { get; set; }

            [JsonProperty("result")]
            public PlaceDetailsOutputResult Result { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }
        public partial class PlaceDetailsOutputResult
        {
            [JsonProperty("address_components")]
            public AddressComponent[] AddressComponents { get; set; }

            [JsonProperty("adr_address")]
            public string AdrAddress { get; set; }

            [JsonProperty("business_status")]
            public string BusinessStatus { get; set; }

            [JsonProperty("current_opening_hours")]
            public CurrentOpeningHours CurrentOpeningHours { get; set; }

            [JsonProperty("formatted_address")]
            public string FormattedAddress { get; set; }

            [JsonProperty("formatted_phone_number")]
            public string FormattedPhoneNumber { get; set; }

            [JsonProperty("geometry")]
            public PlaceDetailsGeometry Geometry { get; set; }

            [JsonProperty("icon")]
            public Uri Icon { get; set; }

            [JsonProperty("icon_background_color")]
            public string IconBackgroundColor { get; set; }

            [JsonProperty("icon_mask_base_uri")]
            public Uri IconMaskBaseUri { get; set; }

            [JsonProperty("international_phone_number")]
            public string InternationalPhoneNumber { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("opening_hours")]
            public PlaceDetailsOpeningHours OpeningHours { get; set; }

            [JsonProperty("photos")]
            public Photo[] Photos { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("plus_code")]
            public PlusCode PlusCode { get; set; }

            [JsonProperty("rating")]
            public long Rating { get; set; }

            [JsonProperty("reference")]
            public string Reference { get; set; }

            [JsonProperty("reviews")]
            public Review[] Reviews { get; set; }

            [JsonProperty("types")]
            public string[] Types { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }

            [JsonProperty("user_ratings_total")]
            public long UserRatingsTotal { get; set; }

            [JsonProperty("utc_offset")]
            public long UtcOffset { get; set; }

            [JsonProperty("vicinity")]
            public string Vicinity { get; set; }

            [JsonProperty("website")]
            public Uri Website { get; set; }

            [JsonProperty("wheelchair_accessible_entrance")]
            public bool WheelchairAccessibleEntrance { get; set; }
        }

        public partial class CurrentOpeningHours
        {
            [JsonProperty("open_now")]
            public bool OpenNow { get; set; }

            [JsonProperty("periods")]
            public CurrentOpeningHoursPeriod[] Periods { get; set; }

            [JsonProperty("weekday_text")]
            public string[] WeekdayText { get; set; }
        }

        public partial class CurrentOpeningHoursPeriod
        {
            [JsonProperty("close")]
            public PurpleClose Close { get; set; }

            [JsonProperty("open")]
            public PurpleClose Open { get; set; }
        }

        public partial class PurpleClose
        {
            [JsonProperty("date")]
            public DateTimeOffset Date { get; set; }

            [JsonProperty("day")]
            public long Day { get; set; }

            [JsonProperty("time")]
            public string Time { get; set; }
        }

        public partial class PlaceDetailsGeometry
        {
            [JsonProperty("location")]
            public Location Location { get; set; }

            [JsonProperty("viewport")]
            public Viewport Viewport { get; set; }
        }
        public partial class PlaceDetailsOpeningHours
        {
            [JsonProperty("open_now")]
            public bool OpenNow { get; set; }

            [JsonProperty("periods")]
            public OpeningHoursPeriod[] Periods { get; set; }

            [JsonProperty("weekday_text")]
            public string[] WeekdayText { get; set; }
        }

        public partial class OpeningHoursPeriod
        {
            [JsonProperty("close")]
            public FluffyClose Close { get; set; }

            [JsonProperty("open")]
            public FluffyClose Open { get; set; }
        }

        public partial class FluffyClose
        {
            [JsonProperty("day")]
            public long Day { get; set; }

            [JsonProperty("time")]
            public string Time { get; set; }
        }
        public partial class Review
        {
            [JsonProperty("author_name")]
            public string AuthorName { get; set; }

            [JsonProperty("author_url")]
            public Uri AuthorUrl { get; set; }

            [JsonProperty("language")]
            public string Language { get; set; }

            [JsonProperty("original_language")]
            public string OriginalLanguage { get; set; }

            [JsonProperty("profile_photo_url")]
            public Uri ProfilePhotoUrl { get; set; }

            [JsonProperty("rating")]
            public long Rating { get; set; }

            [JsonProperty("relative_time_description")]
            public string RelativeTimeDescription { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("time")]
            public long Time { get; set; }

            [JsonProperty("translated")]
            public bool Translated { get; set; }
        }
    }
}
