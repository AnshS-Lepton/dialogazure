using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Area: IReference,IGeographicDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string network_id { get; set; }
       
        public string area_name { get; set; }
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
        
        public string status { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
       

        public string  area_rfs { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstAreaRFS { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public int systemId { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        public string status_remark { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }
        public bool is_new_entity { get; set; }
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
        [NotMapped]
        public string network_status { get; set; }
        public Area()
        {
            objPM = new PageMessage();
            systemId = 0;
            isDirectSave = false;
            no_of_home_pass = 0;
            lstUserModule = new List<string>();
            entityType = "Area";
        }

    }
   
    public class SubArea:IReference,IGeographicDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string network_id { get; set; }
      
        public string subarea_name { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public string remarks { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }
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
       // [NotMapped]
        public string status { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        public string subarea_rfs { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstSubAreaRFS { get; set; }

        public int building_system_id { get; set; }
        public string building_code { get; set; }
        [NotMapped]
		public int user_id { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
		public bool is_new_entity { get; set; }
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
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        public string partner_name { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        [NotMapped]
        public string network_status { get; set; }
        public SubArea()
        {
            objPM = new PageMessage();
            no_of_home_pass=0;
            entityType = "SubArea";
            lstUserModule = new List<string>();
        }
    }

    public class SubAreaIn
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string area_name { get; set; }
    }
    public class SurveyArea: IReference,IGeographicDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string network_id { get; set; }

        public string surveyarea_name { get; set; }
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
        //[NotMapped]
        public string status { get; set; }
        [NotMapped]
        public string surveyarea_status { get; set; }
        [NotMapped]     
        public string province_name { get; set; }
        [NotMapped]       
        public string region_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [Required]
        public DateTime? due_date { get; set; }
        [NotMapped]
        public string childModel { get; set; }
        public string status_remark { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
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
        [NotMapped]
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
		public bool is_new_entity { get; set; }
		public string mzone_id { get; set; }
        [NotMapped]
         public string entityType { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        public string network_status { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public int systemId { get; set; }
        [NotMapped]
        public bool isSurveyModuleAssigned { get; set; }

        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public double latitude { get; set; }
        [NotMapped]
        public double longitude { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }

        public SurveyArea()
        {
            objPM = new PageMessage();
            entityType = "SurveyArea";
            lstUserModule = new List<string>();
        }

    }
    public class RestrictedArea :  IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        [Required]
        public string restricted_area_name { get; set; }
        public string category { get; set; }
        public string sub_category { get; set; }
        public bool is_network_creation_allowed { get; set; }
        public bool is_feasibility_allowed { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }
        public string network_status { get; set; }
        public bool is_visible_on_map { get; set; }
        public bool is_new_entity { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string area_id { get; set; }
        public string dsa_id { get; set; }
        public string csa_id { get; set; }
        public string gis_design_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }

        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int systemId { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstRestrictedAreaRFS { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstcategoryRFS { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstsubcategoryRFS { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstQualificationType { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstAllowedNetworkType { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string geographic_id { get; set; }
        [NotMapped]
        public string region_abbreviation { get; set; }
        [NotMapped]
        public string province_abbreviation { get; set; }
        [Required]
        public string qualification_type { get; set; }

        public string allowed_network { get; set; }

        public RestrictedArea()
        {
            objPM = new PageMessage();
            systemId = 0;
            isDirectSave = false;
        }

    }
    public class MobileSurveyArea
    {
        public string geom { get; set; }
    }
}
