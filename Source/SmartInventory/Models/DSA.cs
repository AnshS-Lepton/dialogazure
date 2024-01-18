using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DSA:IReference,IGeographicDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string network_id { get; set; }
      
        public string dsa_name { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }
        public string remarks { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        //[NotMapped]
        public string status { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        public string status_remark { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string subarea_id { get; set; }
        public string area_id { get; set; }
        public string dsa_id { get; set; }
        public string csa_id { get; set; }
        public string gis_design_id { get; set; }
        [NotMapped]
        public string geographic_id { get; set; }
        [NotMapped]
        public string region_abbreviation { get; set; }
        [NotMapped]
        public string province_abbreviation { get; set; }
        [Required]
        public int? no_of_home_pass { get; set; }
        [Required]
        public string served_by_ring { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstServedByRing { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }
        [NotMapped]
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public bool is_new_entity { get; set; }
        [NotMapped]
        public string network_status { get; set; }
        public DSA()
        {
            objPM = new PageMessage();
            no_of_home_pass=0;
            entityType = "DSA";
            lstUserModule = new List<string>();
        }
    }

    public class CSA:IReference,IGeographicDetails
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string network_id { get; set; }
      
        public string csa_name { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }
        public string remarks { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        //[NotMapped]
        public string status { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        public string status_remark { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string subarea_id { get; set; }
        public string area_id { get; set; }
        public string dsa_id { get; set; }
        public string csa_id { get; set; }
        public string gis_design_id { get; set; }
        [NotMapped]
        public string geographic_id { get; set; }
        [NotMapped]
        public string region_abbreviation { get; set; }
        [NotMapped]
        public string province_abbreviation { get; set; }
        [Required]
        public int? no_of_home_pass { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstServedByRing { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstCsaRFS { get; set; }
        public string rfs_status { get; set; }
        [NotMapped]
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public bool is_new_entity { get; set; }
        [NotMapped]
        public string network_status { get; set; }
        public CSA()
        {
            objPM = new PageMessage();
            //no_of_home_pass=0;
            entityType = "CSA";
            lstUserModule = new List<string>();
        }
    }

    public class CSAIn
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string csa_name { get; set; }
    }


    public class EntityCount
    {
        public string entity_type { get; set; }
        public int? entity_count { get; set; }
    }
}
