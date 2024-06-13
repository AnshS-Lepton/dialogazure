using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Admin;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class TowerTemplateMaster : itemMaster
    {
        [NotMapped]
        public EntityReference EntityReference { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [NotMapped]
        public new int type { get; set; }
        [NotMapped]
        public new int brand { get; set; }
        [NotMapped]
        public new int model { get; set; }
        [NotMapped]
        public new int construction { get; set; }
        [NotMapped]
        public new int activation { get; set; }
        [NotMapped]
        public new int accessibility { get; set; }

    }
    //att_details_tower_pop


    public class TowerItemMaster : TowerTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public TowerItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class AssociatedPop
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
    }
    public class TowerAssociatedPop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int tower_id { get; set; }
        public int pop_id { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        [NotMapped]
        public string tower_name { get; set; }
        [NotMapped]
        public string pop_name { get; set; }
        [NotMapped]
        public string created_by_user { get; set; }
    }

    public class TowerAssociatedPopView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int tower_id { get; set; }
        public int pop_id { get; set; }
        public string tower_network_id { get; set; }
        public string pop_network_id { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public string tower_name { get; set; }
        public string pop_name { get; set; }
        public string created_by_user { get; set; }
    }
    public class TowerMaster : TowerTemplateMaster, IProjectSpecification, IOwnershipInfo, IInstallationInfo,IReference, IGeographicDetails,IAdditionalFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        [Required]
        public string network_name { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public string address { get; set; }
        public double elevation { get; set; }
        [Required]
        public double tower_height { get; set; }
        public string operator_name { get; set; }
        [Required]
        public int no_of_sectors { get; set; }
        public string tenancy { get; set; }
        public string network_type { get; set; }
        public string remark { get; set; }
        public string third_party_vendor_id { get; set; }
        public string ownership_type { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }

        public string status_remark { get; set; }
        public int? status_updated_by { get; set; }
        public DateTime? status_updated_on { get; set; }
        public string network_status { get; set; }
        public string status { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }

        public DateTime? modified_on { get; set; }
        public string acquire_from { get; set; }
        [NotMapped]
        public List<TowerAssociatedPopView> lstTowerAssociatedPop { get; set; }

        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
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
        [NotMapped]
        public string circuit_id { get; set; }
        [NotMapped]
        public string thirdparty_circuit_id { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstTenancy { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> listOwnPartyVendorId { get; set; }
        [NotMapped]
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        [NotMapped]
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        [NotMapped]
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        [NotMapped]
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
        
        public string installation_number { get; set; }
       
        public string installation_year { get; set; }
      
        public string production_year { get; set; }
       
        public string installation_company { get; set; }
       
        public string installation_technician { get; set; }
     
        public string installation { get; set; }
        public bool is_new_entity { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [NotMapped]
        public List<NELoopDetails> lstLoopMangment { get; set; }

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
        //public string served_by_ring { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
        public TowerMaster()
        {
            objPM = new PageMessage();
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            EntityReference = new EntityReference();
            objIspEntityMap = new IspEntityMapping();
            lstTowerAssociatedPop = new List<TowerAssociatedPopView>();
            lstLoopMangment = new List<NELoopDetails>();
            planning_id = 0;
            project_id = 0;
            workorder_id = 0;
            purpose_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
    }
}
