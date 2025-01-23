using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DaFiFeasibilityAPI
{
    public class DaFiFeasibilityResponse
    {
        public string request_id { get; set; }
        public string feasibility_result { get; set; }
        public List<RouteDto> routes { get; set; }
    }

    public class Route
    {
        public string route_id { get; set; }
        public string route_type { get; set; }
        public List<CableSegment> cable_segments { get; set; }
        public string geojson_existing_built { get; set; }
        public string geojson_new_built { get; set; }
        public double total_existing_length { get; set; }
        public double total_new_length { get; set; }
        public Entity start_entity { get; set; }
        public Entity end_entity { get; set; }
        public string existing_start_point { get; set; }
        public string existing_end_point { get; set; }
    }

    public class RouteDto
    {
        public string route_id { get; set; }
        public string route_type { get; set; }
        public List<CableSegment> cable_segments { get; set; }
        public string geojson_existing_built { get; set; }
        public string geojson_new_built { get; set; }
        public double total_existing_length { get; set; }
        public double total_new_length { get; set; }
        public Entity start_entity { get; set; }
        public Entity end_entity { get; set; }
    }

    public class CableSegment
    {
        public string cable_id { get; set; }
        public string cable_name { get; set; }
        public double length { get; set; }
        public string region { get; set; }
        public string province { get; set; }
        public Entity start_entity { get; set; }
        public Entity end_entity { get; set; }

    }

    public class Entity
    {
        public string entity_type { get; set; }
        public string entity_id { get; set; }

    }
    public class ReserveFeasibility
    {
        [Required]
        public string feasibility_request_id { get; set; }
        [Required]
        public string route_id { get; set; }
        [Required]
        public string customer_id { get; set; }
        [Required]
        public string customer_name { get; set; }    

    }
    public class ReleaseFeasibility
    {
        [Required]
        public string feasibility_request_id { get; set; }
        [Required]
        public string route_id { get; set; }

    }

    public class FeasibileRouteKML
    {
        public string route_id { get; set; }
        public string kml_existing_built { get; set; }
        public string kml_new_built { get; set; }
    }
}
