using Models.Admin;
using Models.ISP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Models
{
    public class CableMaster : CableTemplateMaster, IProjectSpecification, IOwnershipInfo,IAcquireFrom,IReference,IGeographicDetails,IAdditionalFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
      
        public string cable_name { get; set; }
        //[Required]       
        public string a_location { get; set; }
        //[Required]
        public string b_location { get; set; }

		public string a_location_name { get; set; }
		//[Required]
		public string b_location_name { get; set; }
		public string a_latitude { get; set; }
        public string b_longitude { get; set; }
        public string a_region { get; set; }
        public string b_region { get; set; }
        public string a_city { get; set; }
        public string b_city { get; set; }
        [Required]
        public float? cable_measured_length { get; set; }
        [Required]
        public double? cable_calculated_length { get; set; }
        public string coreaccess { get; set; }
        public string wavelength { get; set; }
        public string optical_output_power { get; set; }
        public string frequency { get; set; }
        public string attenuation_db { get; set; }
        public string resistance_ohm { get; set; }

        public string status { get; set; }
        public string network_status { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
        public string pin_code { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }

        public string utilization { get; set; }
        public string totalattenuationloss { get; set; }
        public string chromaticdb { get; set; }
        public string chromaticdispersion { get; set; }
        public string totalchromaticloss { get; set; }
        public string remarks { get; set; }
        [Required]
        public string route_id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int a_system_id { get; set; }
        public string a_entity_type { get; set; }
        public int b_system_id { get; set; }
        public string b_entity_type { get; set; }
        public int duct_id { get; set; }
        public int trench_id { get; set; }
        public int sequence_id { get; set; }

        [NotMapped]
        public string geom_source { get; set; }
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
        [NotMapped]
        public int mappedEntId { get; set; }
        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }
        public string ownership_type { get; set; }
        public string third_party_vendor_id { get; set; }
        public string circuit_id { get; set; }
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
        public string ispLineGeom { get; set; }
        public int structure_id { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }

        public int total_loop_count { get; set; }

        public int total_loop_length { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        //public string cable_status { get; set; }
        //public string cable_status_comment { get; set; }
        //[NotMapped]
        //public List<TubeCoreInfo> lstTubeCore { get; set; }
        [NotMapped]
        public TubeCoreLstIn lstTubeCore { get; set; }

        public double? start_reading { get; set; }

        public double? end_reading { get; set; }
        public string execution_method { get; set; }
        public string acquire_from { get; set; }

        [NotMapped]
        public IList<DropDownMaster> listExecutionMethod { get; set; }

        [NotMapped]
        public EntityReference EntityReference { get; set; }
        [NotMapped]
        public string a_node_type { get; set; }
        [NotMapped]
        public string b_node_type { get; set; }

        [NotMapped]
        public entityATStatusAtachmentsList ATAcceptance { get; set; }

        [NotMapped]
        public LMCCableInfo LMCCableInfo { get; set; }

        [NotMapped]
        public EntityMaintainenceChargesList MaintainenceCharges { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public string actionTab { get; set; }
        [NotMapped]
        public string a_long_lat { get; set; }
        [NotMapped]
        public string b_long_lat { get; set; }

        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }
        public int? primary_pod_system_id { get; set; }
        public int? secondary_pod_system_id { get; set; }
        public string status_remark { get; set; }
        public bool is_new_entity { get; set; }
        public double inner_dimension { get; set; }
        public double outer_dimension { get; set; }
        [NotMapped]
        public string childModel { get; set; }
        public string calculated_length_remark { get; set; }
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
        [Required]
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
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        [Required]
        public string route_name { get; set; }
        [NotMapped]
        public CDBAttribute LstCDBAttribute { get; set; }
        public CableMaster()
        {
            objPM = new PageMessage();
            lstTP = new List<NetworkDtl>();
            a_system_id = 0;
            b_system_id = 0;
            structure_id = 0;
            cable_calculated_length = null;
            cable_measured_length = null;
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            EntityReference = new EntityReference();
            LstCDBAttribute = new CDBAttribute();
            purpose_id = 0;
            planning_id = 0;
            workorder_id = 0;
            project_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
    }


    public class LineEntityIn
    {
        public string geom { get; set; }
        public string enType { get; set; }
        public int systemId { get; set; }
        public string cableType { get; set; }
        public string networkIdType { get; set; }
        public List<NetworkDtl> lstTP { get; set; }
        public string centerLineGeom { get; set; }

        public ElementInfo ModelInfo { get; set; }
        public decimal buffer_width { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public int system_id { get; set; }
        //[NotMapped]
        //public List<string> lstUserModule { get; set; }

        public LineEntityIn()
        {
            ModelInfo = null;
            geom = "";
            enType = "";
            systemId = 0;
            cableType = "";
            lstTP = new List<NetworkDtl>();
            centerLineGeom = "";
            pSystemId = 0;
            pEntityType = "";
            pNetworkId = "";
            //lstUserModule = new List<string>();
        }
    }

    public class EditLineTP
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public List<EditLineTPIn> tpDetail { get; set; }
        public string entityGeom { get; set; }
        public string message { get; set; }
        public EditLineTP()
        {
            tpDetail = new List<EditLineTPIn>();
        }
    }
    public class EditLineTPIn
    {
        public string mode { get; set; }
        public string network_id { get; set; }
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public string actualLatLng { get; set; }
    }

    public class TubeCoreInfo
    {
        public int tube_number { get; set; }
        public string tube_color { get; set; }
        public string tube_color_code { get; set; }
        public int core_number { get; set; }
        public string core_color { get; set; }
        public string core_color_code { get; set; }
    }

    public class TubeCoreLstIn
    {
        public List<TubeCoreInfo> objTube { get; set; }
        public List<TubeCoreInfo> objCore { get; set; }
    }
    public class OffsetGeometry
    {
        public string offsetGeom { get; set; }
        public string cable_type { get; set; }
    }
    public class CableFiberHeaders
    {
        [NotMapped]
        public string cable_network_id { get; set; }
        [NotMapped]
        public string cable_display_name { get; set; }
    }
    public class CableFiberDetail
    {
        public int? fiber_number { get; set; }
        public int tube_number { get; set; }
        public string tube_color { get; set; }
        //public string tube_color_code { get; set; }
        public string tube_color_code { get; set; }
        public int core_number { get; set; }
        public string core_color { get; set; }
        //public string core_color_code { get; set; }
        public string core_color_code { get; set; }

        // public string fiber_usage_status { get; set; }

        //public string coonected_or_idle { get; set; }
        public string a_end_status { get; set; }
        public string b_end_status { get; set; }
        public string PhysicalUtil { get; set; }

        [NotMapped]
        public int cable_id { get; set; }
        [NotMapped]
        public string cable_network_id { get; set; }
        [NotMapped]
        public int link_system_id { get; set; }
        [NotMapped]
        public string link_id { get; set; }

        [NotMapped]
        public int system_id { get; set; }
        [NotMapped]
        public string core_status { get; set; }
        [NotMapped]
        public string type { get; set; }
        [NotMapped]
        public string comment { get; set; }
        [NotMapped]
        public string cable_network_status { get; set; }
        //public string tube_number { get; set; }
        //public string tube_color { get; set; }
        //public string core_number { get; set; }
        //public string core_color { get; set; }

        //public string fiber_usage_status { get; set; }
        //public string is_connected { get; set; }
        //public string logicallink_id { get; set; }
        //public string service_id { get; set; }

        //public string physical_circuit_id { get; set; }
        //public string customer_name { get; set; }
        //public string ring_equipment { get; set; }
        //public string ring_name { get; set; }

        //public string ring_type { get; set; }
        //public string mpsl_ring_name { get; set; }

        //public string Remark_1 { get; set; }
        //public string Remark_2 { get; set; }
        //public string Remark_3 { get; set; }
        //public string Remark_4 { get; set; }
        //public string Remark_5 { get; set; }
        //public string Remark_6 { get; set; }
        //public string Remark_7 { get; set; }
        //public string Remark_8 { get; set; }

        //public string Remark_9 { get; set; }
        //public string Remark_10 { get; set; }
        //public string Remark_11 { get; set; }
        //public string Remark_12 { get; set; }
        //public string Remark_13 { get; set; }
        //public string Remark_14 { get; set; }

        //public string Remark_15 { get; set; }


        public List<CableFiberDetail> ViewCableFiberDetail { get; set; }
        public List<FibrePortStatusCount> ViewPortStatusCount { get; set; }
        public CableFiberDetail()
        {

            ViewCableFiberDetail = new List<CableFiberDetail>();
            ViewPortStatusCount = new List<FibrePortStatusCount>();

        }
    }
    public class FibrePortStatusCount
    {
        public string PortStatus { get; set; }
        public int StatusCount { get; set; }      

    }
    public class PatchCordMaster : PatchCordTemplateMaster, IProjectSpecification,IReference,IAdditionalFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
       
        public string patch_cord_name { get; set; }
        //[Required]       
        public string a_location { get; set; }
        //[Required]
        public string b_location { get; set; }
        public string coreaccess { get; set; }
        public string wavelength { get; set; }
        public string optical_output_power { get; set; }
        public string frequency { get; set; }
        public string attenuation_db { get; set; }
        public string resistance_ohm { get; set; }

        public string status { get; set; }
        public string network_status { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
        public string pin_code { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }

        public int utilization { get; set; }
        public string totalattenuationloss { get; set; }
        public string chromaticdb { get; set; }
        public string chromaticdispersion { get; set; }
        public string totalchromaticloss { get; set; }
        public string remarks { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int a_system_id { get; set; }
        public string a_entity_type { get; set; }
        public int b_system_id { get; set; }
        public string b_entity_type { get; set; }
        public int sequence_id { get; set; }
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
        [NotMapped]
        public int mappedEntId { get; set; }
        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }
        [NotMapped]
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        [NotMapped]
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        [NotMapped]
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        [NotMapped]
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }

        [NotMapped]
        public string ispLineGeom { get; set; }
        public int structure_id { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        [NotMapped]
        public TubeCoreLstIn lstTubeCore { get; set; }
        public string execution_method { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listExecutionMethod { get; set; }

        [NotMapped]
        public EntityReference EntityReference { get; set; }
        [NotMapped]
        public string a_node_type { get; set; }
        [NotMapped]
        public string b_node_type { get; set; }
        [NotMapped]
        public bool isEquipmentPatching { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        [Required]
        public string bom_sub_category { get; set; }
        //  public string served_by_ring { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstBOMSubCategory  { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstServedByRing { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public PatchCordMaster()
        {
            objPM = new PageMessage();
            lstTP = new List<NetworkDtl>();
            a_system_id = 0;
            b_system_id = 0;
            structure_id = 0;
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            EntityReference = new EntityReference();
            planning_id = 0;
            project_id = 0;
            workorder_id = 0;
            purpose_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
    }

    public class CableMergeStatus
    {
        public string status { get; set; }
        public string message { get; set; }
    }
}
