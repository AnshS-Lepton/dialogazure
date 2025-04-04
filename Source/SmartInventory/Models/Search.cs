using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{

    public class SearchResult
    {
        public string label { get; set; }
        public string value { get; set; }
        public string geomType { get; set; }
        public string entityName { get; set; }
        public string networkId { get; set; }
        public string entityTitle { get; set; }
        public int systemId { get; set; }
        public string status { get; set; }
        public string entityID { get; set; }
        public string networkStatus { get; set; }
        public int landbase_layer_id { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
    }
    public class latlong
    {
        public string Lat { get; set; }
        public string Long { get; set; }
    }
    public class GeomDetailIn
    {
        public string systemId { get; set; }
        public string entityType { get; set; }
        public string geomType { get; set; }
        public int user_id { get; set; }
        public string connectionString { get; set; }
    }
    public class GeometryDetail
    {
        public Int32 entity_id { get; set; }
        public string entity_type { get; set; }     
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string common_name { get; set; }
        public int user_id { get; set; }
        public string approval_flag { get; set; }
        public string sp_geometry { get; set; }
        public string sp_centroid { get; set; }  
        public int approver_id { get; set; }
        public DateTime approval_date { get; set; }
        public string creator_remark { get; set; }
        public string approver_remark { get; set; }
        //BBOX
        public string geometry_extent { get; set; }
        public latlong southWest { get; set; }
        public latlong northEast { get; set; }
        public string entity_sub_type { get; set; }
        public ImpactDetail childElements { get; set; }
        public string sp_center { get; set; }
        public double radious { get; set; }
        public string center_line_geom { get; set; }
        public List<EditLineTPIn> lstTP { get; set; }
        public string display_name { get; set; }
        public int landbase_layer_id { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }

    }
    public class GeomRingDetailIn
    {
        
        public string geom { get; set; }
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string site_id { get; set; }
        public string site_name { get; set; }
    }

    public class cableinfo
    {

        public string cable_geom { get; set; }
        public int cable_system_id { get; set; }
        public string cable_network_id { get; set; }
        public string cable_type { get; set; }
        
    }
    public class connectedelements
    {

        public string connected_entity_geom { get; set; }
        public int connected_system_id { get; set; }
        public string connected_network_id { get; set; }
        public string connected_entity_type { get; set; }
        public bool is_virtual { get; set; }

    }

    public class vmGeomRingDetailIn
    {
        public List<GeomRingDetailIn> lstSitedetails { get; set; }
        public vmGeomRingDetailIn()
        {
            lstSitedetails = new List<GeomRingDetailIn>();
        }
    }

    public class vmRingConnectedElementDetails
    {
        public List<cableinfo> lstcableinfo { get; set; }
        public List<connectedelements> lstconnectedelements { get; set; }
        public vmRingConnectedElementDetails()
        {
            lstcableinfo = new List<cableinfo>();
            lstconnectedelements = new List<connectedelements>();
        }
    }

}

