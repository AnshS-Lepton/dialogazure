using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Admin;

namespace Models
{
    public class MicroductMasterTemplateMaster : itemMaster
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

    public class MicroductMaster : MicroductMasterTemplateMaster, IProjectSpecification, IOwnershipInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        [Required]
        public string network_name { get; set; }
        public int a_system_id { get; set; }
       
        public string a_location { get; set; }
        public string a_entity_type { get; set; }
        public int b_system_id { get; set; }
        
        public string b_location { get; set; }
        public string b_entity_type { get; set; }
        [Required]
        public double calculated_length { get; set; }
        [Required]
        public double manual_length { get; set; }
        public int trench_id { get; set; }
        public int? sequence_id { get; set; }
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
        public IList<DropDownMaster> DuctTypeIn { get; set; }
        [NotMapped]
        public IList<DropDownMaster> DuctColorIn { get; set; }

        [NotMapped]
        public entityATStatusAtachmentsList ATAcceptance { get; set; }

        [NotMapped]
        public EntityMaintainenceChargesList MaintainenceCharges { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }

        public MicroductMaster()
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
        }
    }
}
