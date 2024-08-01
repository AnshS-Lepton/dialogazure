using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{

    public class SiteList
    {
        public string Status_Code { get; set; }
        public string Status_Description { get; set; }
        public SiteDetails[] Response { get; set; }
    }

    public class SiteDetails
    {
        public string Site_Id { get; set; }
        public string Site_Name { get; set; }
        public string Status { get; set; }
    }

    public class SiteSync
    {
        public int id { get; set; }
        public DateTime start_datetime { get; set; }
        public DateTime end_datetime { get; set; }
        public DateTime lastsuccess_sync { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }

    public class Site
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string site_id { get; set; }
        [Required]
        public string site_name { get; set; }
        public DateTime on_air_date { get; set; }
        public DateTime removed_date { get; set; }
        [Required]
        public string tx_type { get; set; }
        [Required]
        public string tx_technology { get; set; }
        [Required]
        public string tx_segment { get; set; }
        [Required]
        public string tx_ring { get; set; }
        [Required]
        public string address { get; set; }
        public string region { get; set; }
        public string province { get; set; }
        [Required]
        public string district { get; set; }
        public string region_address { get; set; }
        public string depot { get; set; }
        public string ds_division { get; set; }
        public string local_authority { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        [Required]
        public string owner_name { get; set; }
        public string access_24_7 { get; set; }

        public string tower_type { get; set; }
        public int tower_height { get; set; }
        public string cabinet_type { get; set; }
        public string solution_type { get; set; }

        public int site_rank { get; set; }
        public int self_tx_traffic { get; set; }
        public int agg_tx_traffic { get; set; }
        public int metro_ring_utilization { get; set; }
        public int csr_count { get; set; }
        public int dti_circuit { get; set; }
        public string agg_01 { get; set; }
        public string agg_02 { get; set; }
        public int bandwidth { get; set; }
        public string ring_type { get; set; }
        [Required]
        public string link_id { get; set; }
        public string alias_name { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public string network_status { get; set; }

        public string network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string parent_network_id { get; set; }
        public int parent_system_id { get; set; }
        public int sequence_id { get; set; }
        public bool is_visible_on_map {  get; set; }    
        public DateTime? status_updated_on {  get; set; } 
        public int? status_updated_by { get;}
        public string source_ref_id { get; set; }
        public string source_ref_type { get; set; }
        public string target_ref_id { get; set; }
        public string target_ref_code { get; set; }
        public string target_ref_description { get; set; }
        public string gis_design_id { get; set; }

        [NotMapped]
        public string region_name { get; set; }

        [NotMapped]
        public string province_name { get; set; }

        [NotMapped]
        public string geom { get; set; }

        [NotMapped]
        public string networkIdType { get; set; }

        [NotMapped]
        public PageMessage objPM { get; set; } = new PageMessage();

        [NotMapped]
        public List<string> lstUserModule { get; set; } = new List<string>();

        [NotMapped]
        public string region_abbreviation { get; set; }
        [NotMapped]
        public string province_abbreviation { get; set; }
    }
}
