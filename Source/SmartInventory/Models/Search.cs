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
}

