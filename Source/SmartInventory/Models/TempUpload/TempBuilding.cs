using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.TempUpload
{
    public class TempBuilding:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int system_id { get; set; }

        public string network_id { get; set; }
        public string building_name { get; set; }
        public int surveyarea_id { get; set; }

        [JsonProperty("latitude")]
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public string building_no { get; set; }
        public string address { get; set; }
        public string area { get; set; }
        public string pin_code { get; set; }

        [JsonProperty("tenancy")]
        public string tenancy { get; set; }
        public string category { get; set; }

        [JsonProperty("no_of_bp")]
        public int? business_pass { get; set; }

        [JsonProperty("home_pass_count")]
        public int? home_pass { get; set; }
        public string media { get; set; }
        // public int structure { get; set; }
        [JsonProperty("total_stucture")]
        public int? total_tower  { get; set; }
        public string pod_name { get; set; }
        public string pod_code { get; set; }
        public string rfs_status { get; set; }
        // public DateTime rfs_date { get; set; }
        public DateTime? rfs_date { get; set; }
       // public string rwa { get; set; }
       // public string rwa_contact_no { get; set; }
        public string remarks { get; set; }
        public int upload_id { get; set; }
        public int created_by { get; set; }
       // public DateTime created_on { get; set; }
       public string parent_entity_type { get; set; }
        public string parent_network_id { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int batch_id { get; set; }
        public int row_order { get; set; }
        public string sp_geometry { get; set; }
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
