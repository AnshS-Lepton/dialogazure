using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EntityDetail
    {
        public int system_id { get; set; }
        public string geom_type { get; set; }
        public string entity_type { get; set; }
        public string entity_title { get; set; }
        public string common_name { get; set; }
        public string network_status { get; set; }
        public string geom { get; set; }
        public string centroid_geom { get; set; }
        public string display_name { get; set; }
        public string total_core { get; set; }
        public string utilized_port { get; set; }
        public List<LayerActionMapping> listLayerAction { get; set; }
    }
    
    public class EntityDetailWithAttribute
    {
        public string entity_type { get; set; }
        public string geom_type { get; set; }
        public string geom { get; set; }
        public string network_status { get; set; }
        public string entity_attribute { get; set; }
    }
    public class vmNearbyentityDetails
    {
        public int total_records { get; set; }
        public int last_record_number { get; set; }
        public List<nearByEntityDetails> lstNearByEntityDetails { get; set; }
        public vmNearbyentityDetails()
        {
            lstNearByEntityDetails = new List<nearByEntityDetails>();
        }
    }
    public class nearByEntityDetailsCount
    {
        public int totalrecords { get; set; }
    }
    public class nearByEntityDetails
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string entity_type { get; set; }
        public string layer_title { get; set; }
        public string geom_type { get; set; }
        public string geom { get; set; }
        public string network_status { get; set; }
        public string entity_category { get; set; }
        public string barcode { get; set; }
        public string entity_attribute { get; set; }
        public string display_attribute { get; set; }
    }
    public class nearByNetworkEntities
    {
        public string entity_type { get; set; }
        public string network_id { get; set; }
        public string Name { get; set; }
        public string core_count { get; set; }
        public string category { get; set; }
        public string network_status { get; set; }
        public double distance_in_mtr { get; set; }
    }
    public class entityInfo
    {
        public string db_column_name { get; set; }
        public string display_column_name { get; set; }
        public string old_value { get; set; }
        public string updated_value { get; set; }
        public bool is_updated { get; set; }
        public string DRAFT_MESSAGE { get; set; }
    }

    public class nearestFiberPoint
    {
        public string nearest_fiber_point { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public double distance_meters { get; set; }
    }

    public class nearestStructure
    {
        public string cable_network_id { get; set; }
        public string cable_geom { get; set; }
        public string structure_id { get; set; }
        public string structure_type { get; set; }
        public string structure_geom { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public double distance_meters { get; set; }
    }
    public class customerToRoad
    {
        public string geom { get; set; }
        public double length_meters { get; set; }
    }
}
