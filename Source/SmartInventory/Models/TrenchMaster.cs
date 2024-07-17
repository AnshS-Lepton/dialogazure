using Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TrenchMaster : TrenchTemplateMaster, IProjectSpecification,IOwnershipInfo,IAcquireFrom,IReference,IGeographicDetails,IAdditionalFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
     
        public string trench_name { get; set; }
        public int a_system_id { get; set; }
        //[Required]
        public string a_location { get; set; }
        public string a_entity_type { get; set; }
        public int b_system_id { get; set; }
        //[Required]
        public string b_location { get; set; }
        public string b_entity_type { get; set; }
        public double trench_length { get; set; }
        public int utilization  { get; set; }
        public string no_of_ducts { get; set; }
        public string no_of_gipipes { get; set; }
        public int? sequence_id { get; set; }
        public string network_status { get; set; }
        public string status { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
        public string pin_code { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
       
        public string remarks { get; set; }
        public string acquire_from { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public List<NetworkDtl> lstTP { get; set; }

        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }
        public string mcgm_ward { get; set; }
        public string strata_type { get; set; }
        public string surface_type { get; set; }
        public DateTime? manufacture_year { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        [Required]
        public string ownership_type { get; set; }
        public string third_party_vendor_id { get; set; }
        public string own_vendor_id { get; set; }

        [NotMapped]
        public string circuit_id { get; set; }
        [NotMapped]
        public string thirdparty_circuit_id { get; set; }
        public int? primary_pod_system_id { get; set; }
        public int? secondary_pod_system_id { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> listOwnVendorId { get; set; }
        [NotMapped]
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        [NotMapped]
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        [NotMapped]
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        [NotMapped]
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		[NotMapped]
		public List<DropDownMaster> listALocation { get; set; }
		[NotMapped]
		public List<DropDownMaster> listBLocation { get; set; }
		[NotMapped]
        public IList<DropDownMaster> MCGMWardIn { get; set; }
        [NotMapped]
        public IList<DropDownMaster> StrataTypeIn { get; set; }
        [NotMapped]
        public IList<DropDownMaster> SurfaceTypeIn { get; set; }

        [NotMapped]
        public EntityReference EntityReference { get; set; }
        [NotMapped]
        public entityATStatusAtachmentsList ATAcceptance { get; set; }

        [NotMapped]
        public EntityMaintainenceChargesList MaintainenceCharges { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public string a_node_type { get; set; }
        [NotMapped]
        public string b_node_type { get; set; }
        [NotMapped]
        public string a_long_lat { get; set; }
        [NotMapped]
        public string b_long_lat { get; set; }
        public string status_remark { get; set; }
        public bool is_new_entity { get; set; }
        public bool is_acquire_from { get; set; }
        public string other_info { get; set; }
        [NotMapped]
        public vm_dynamic_form objDynamicControls { get; set; }
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
        public string bom_sub_category { get; set; }
       // public string served_by_ring { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstBOMSubCategory  { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstServedByRing { get; set; }
        //[Required]
        //public string trench_serving_type { get; set; }
        //[NotMapped]
        //public List<DropDownMaster> lstTrenchServingType { get; set; }
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public string a_latitude { get; set; }
        public string b_longitude { get; set; }
        public string a_region { get; set; }
        public string b_region { get; set; }
        public string a_city { get; set; }
        public string b_city { get; set; }
        [NotMapped]
        public string entity_type { get; set; }
        [NotMapped]
        public trenchExecutionList ExecutionMethod { get; set; }
        public string parent_trench_netwok_id { get; set; }
        public string parent_trench_system_id { get; set; }
        public string splited_by { get; set; }
        public string splitted_on { get; set; }
        public string splitting_system_id { get; set; }
        public string splitting_netwok_id { get; set; }
        public string splitting_entitytype { get; set; }
		[Required]
		public string a_location_code { get; set; }
		[Required]
		public string b_location_code { get; set; }
		public TrenchMaster()
        {
            objPM = new PageMessage();
            lstTP = new List<NetworkDtl>();
            a_system_id = 0;
            b_system_id = 0;
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            MCGMWardIn = new List<DropDownMaster>();
            StrataTypeIn = new List<DropDownMaster>();
            SurfaceTypeIn = new List<DropDownMaster>();
            EntityReference = new EntityReference();
            lstTrenchServingType= new List<DropDownMaster>();
            planning_id = 0;
            purpose_id = 0;
            workorder_id = 0;
            project_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            pEntityType = "Trench";
            lstUserModule = new List<string>();
        }
    }
}
