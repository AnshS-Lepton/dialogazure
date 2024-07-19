using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
    public class TempDuct:IReference
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string duct_name { get; set; }
        public int a_system_id { get; set; }
        public string a_network_id { get; set; }
        public string a_entity_type { get; set; }
        public int b_system_id { get; set; }
        public string b_network_id { get; set; }
        public string b_entity_type { get; set; }
        //public string pin_code { get; set; }
        //public int no_of_cables { get; set; }
        public double calculated_length { get; set; }
       // public double manual_length { get; set; }
       // public int trench_id { get; set; }
        //public decimal inner_diameter { get; set; }
        //public decimal outer_diameter { get; set; }
        //public string remarks { get; set; }
        public int created_by { get; set; }
        public string specification { get; set; }
        [JsonProperty("vendor")]
        public string vendor_name { get; set; }
        public int upload_id { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int batch_id { get; set; }
        public int row_order { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
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
        public string a_location_code { get; set; }
        public string b_location_code { get; set; }
    }
}
