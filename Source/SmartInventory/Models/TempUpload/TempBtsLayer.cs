using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.TempUpload
{
    public class TempBtsLayer
    {
        [Key, Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string bts_id { get; set; }
        public string bts_name { get; set; }
        public string bts_add { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double twrht { get; set; }
        public string bts_tower_type { get; set; }
        public double bts_tower_height__in_meter_ { get; set; }
        public double building_height { get; set; }
        public string tower_type { get; set; }
        public string owner_name { get; set; }
        public string hub { get; set; }
        public string circle { get; set; }


        public string layer_type { get; set; }

        public int du_history_id { get; set; }

        public DateTime created_on { get; set; }

        public string amsl { get; set; }
        public string contact_no { get; set; }
        public string owner_address { get; set; }

        public string antenna_1_height { get; set; }
        public string antenna_2_height { get; set; }
        public string facility_id { get; set; }
        public string type_of_site { get; set; }
        public string state { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public string bstn_sw_ip { get; set; }
        public string bstn_router_ip { get; set; }
        public string colo_provider { get; set; }
        public string conn_to_noc { get; set; }
        public double bst_capacity { get; set; }
        public double bst_utilization { get; set; }

        public string category { get; set; }
        public string wsg_group { get; set; }
        public string bts_node_b_id { get; set; }
        public string bts_node_b_ip_address { get; set; }
        public int ap_count { get; set; }
        [NotMapped]
        public string sp_geometry { get; set; }
        public string bstn_provision_bandwidth { get; set; }

    }
}
