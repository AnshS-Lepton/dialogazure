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
    public class DuctMaster : DuctTemplateMaster, IProjectSpecification,IOwnershipInfo,IAcquireFrom,IReference,IGeographicDetails,IAdditionalFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
       
        public string duct_name { get; set; }
        public int a_system_id { get; set; }
       // [Required]
        public string a_location { get; set; }
        public string a_entity_type { get; set; }
        public int b_system_id { get; set; }
        //[Required]
        public string b_location { get; set; }
        public string b_entity_type { get; set; }
        [Required]
        public double calculated_length { get; set; }
        [Required]
        public double manual_length { get; set; }
        public int trench_id { get; set; }
        public int? sequence_id { get; set; }

        //public string duct_category { get; set; }
        public string network_status { get; set; }
        public string status { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
        public string pin_code { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public int? utilization { get; set; }
        public int no_of_cables { get; set; }
        public double offset_value { get; set; }
        public double inner_dimension { get; set; }
        public double outer_dimension { get; set; }
        public string duct_type { get; set; }
        public string color_code { get; set; }
        public string acquire_from { get; set; }
        public string remarks { get; set; }
        public int created_by { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int mappedEntId { get; set; }
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
         public int? no_of_ducts_created { get; set; }

        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
        [Required]
        public string ownership_type { get; set; }
        public string third_party_vendor_id { get; set; }
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
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        [NotMapped]
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        [NotMapped]
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        [NotMapped]
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }

        [NotMapped]
        public IList<DropDownMaster> NoofDuctsCreated { get; set; }

        [NotMapped]
        public IList<DropDownMaster> DuctTypeIn { get; set; }
        [NotMapped]
        public IList<DropDownMaster> DuctColorIn { get; set; }
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
        //   public string served_by_ring { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstBOMSubCategory  { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstServedByRing { get; set; }
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
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
        public IList<DropDownMaster> DuctCount { get; set; }
        [NotMapped]
        public int duct_count { get; set; }    
        public string duct_color { get; set; }
        public int? total_slack_count { get; set; }
        public int? total_slack_length { get; set; }
        [NotMapped]
        public int structure_id { get; set; }
        [NotMapped]
        public string ispLineGeom { get; set; }
        [NotMapped]
        public string cable_type { get; set; }
        public DuctMaster()
        {
            objPM = new PageMessage();
            lstTP = new List<NetworkDtl>();
            a_system_id = 0;
            b_system_id = 0;

            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            EntityReference = new EntityReference();
            project_id = 0;
            workorder_id = 0;
            purpose_id = 0;
            planning_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
    }  
}
