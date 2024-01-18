using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
    public class TempFDB:IReference
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }

        public string fdb_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public string specification { get; set; }
        public int specification_id { get; set; }
        public int vendor_id { get; set; }
        [JsonProperty("vendor")]
        public string vendor_name { get; set; }
        public int upload_id { get; set; }
        public int created_by { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public int batch_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int row_order { get; set; } 
        public string shaft_name { get; set; }
        public string floor_name { get; set; }
        public string remarks { get; set; }
        public int client_id { get; set; }
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
