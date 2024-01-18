using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
     
    public class TempADB:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string adb_name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string address { get; set; }
        public string pin_code { get; set; }
        public string entity_category { get; set; }
        public string specification { get; set; }
        [JsonProperty("vendor")]
        public string vendor_name { get; set; }
        public int upload_id { get; set; }
        public int created_by { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int batch_id { get; set; }
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

    #region Reference Details By Antra
    public interface IReference
    {
        string origin_from { get; set; }
        string origin_ref_id { get; set; }
        string origin_ref_code { get; set; }
        string origin_ref_description { get; set; }
        string request_ref_id { get; set; }
        string requested_by { get; set; }
        string request_approved_by { get; set; }
    }
    #endregion
}
