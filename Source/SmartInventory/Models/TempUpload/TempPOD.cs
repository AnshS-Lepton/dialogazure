
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
    public class TempPOD:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string pod_name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
        public string pin_code { get; set; }
        public string specification { get; set; }
        [JsonProperty("vendor")]
        public string vendor_name { get; set; }
        public int upload_id { get; set; }
        public int created_by { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int batch_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int row_order { get; set; }
        public string shaft_name { get; set; }
        public string floor_name { get; set; }
        public string unit_network_id { get; set; }
        public string remarks { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string network_status { get; set; }
        //Site Details Added below
        public string site_id { get; set; }
        public string site_name { get; set; }
        public string on_air_date { get; set; }
        public string removed_date { get; set; }
        public string tx_type { get; set; }
        public string tx_technology { get; set; }
        public string tx_segment { get; set; }
        public string tx_ring { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string depot { get; set; }
        public string ds_division { get; set; }
        public string local_authority { get; set; }
        public string owner_name { get; set; }
        public string access_24_7 { get; set; }
        public string tower_type { get; set; }
        public int? tower_height { get; set; }
        public string cabinet_type { get; set; }
        public string solution_type { get; set; }
        public int? site_rank { get; set; }
        public string self_tx_traffic { get; set; }
        public string agg_tx_traffic { get; set; }
        public int? csr_count { get; set; }
        public int? dti_circuit { get; set; }
        public string agg_01 { get; set; }
        public string agg_02 { get; set; }
        public int? bandwidth { get; set; }
        public string ring_type { get; set; }
        public string link_id { get; set; }
        public string alias_name { get; set; }
        //
        public string region { get; set; }

        public string region_address { get; set; }

        public string metro_ring_utilization { get; set; }

        //public bool is_visible_on_map { get; set; }
        //public DateTime? status_updated_on { get; set; }
        //public int? status_updated_by { get; }
        public int? target_ref_id { get; set; }
        public string target_ref_code { get; set; }
        public string target_ref_description { get; set; }

        public string tx_agg { get; set; }
        public string bh_status { get; set; }
        public string elevation { get; set; }
        public string segment { get; set; }
        public string ring { get; set; }
        public int? maximum_cost { get; set; }
        public string project_category { get; set; }
        public int? priority { get; set; }
        public int? no_of_cores { get; set; }
        public string fiber_link_type { get; set; }
        public string comment { get; set; }
        public int? plan_cost { get; set; }
        public int? fiber_distance { get; set; }
        public string fiber_link_code { get; set; }
        public string port_type { get; set; }
        public string destination_site_id { get; set; }
        public string destination_port_type { get; set; }
        public string destination_no_of_cores { get; set; }
    }
}
