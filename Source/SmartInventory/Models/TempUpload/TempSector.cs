using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
    public class TempSector:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string system_id { get; set; }
        public string network_id { get; set; }
        public string network_name { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public double azimuth { get; set; }
        public string technology { get; set; }
        public double port_name { get; set; }
        public string down_link { get; set; }
        public string uplink { get; set; }
        public string brand_name { get; set; }
        public string sector_type { get; set; }
        public string frequency { get; set; }
        public string network_type { get; set; }
        public string ownership_type { get; set; }
        public string vendor_name { get; set; }
        public string specification { get; set; }
        public int upload_id { get; set; }
        public int batch_id { get; set; }
        public int created_by { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int row_order { get; set; }
        public string sp_geometry { get; set; }
        public string remarks { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string network_status { get; set; }
    }
}
