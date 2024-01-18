using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
  public  class TempHandhole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string handhole_name { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
        public string specification { get; set; }
        [JsonProperty("vendor")]
        public string vendor_name { get; set; }
        public int upload_id { get; set; }
        public int created_by { get; set; }
        public bool is_virtual { get; set; }
        public bool is_valid { get; set; }
        public string construction_type { get; set; }
        public string error_msg { get; set; }
        public int row_order { get; set; }
        public int batch_id { get; set; }
        public string remarks { get; set; }
    }
}
