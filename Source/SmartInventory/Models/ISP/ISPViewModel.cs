using Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.ISP
{
    public class ISPViewModel
    {
        public List<StructureElement> StructureElements { get; set; }
        public List<StructureEntityView> ShaftList { get; set; }
        public List<StructureEntityView> FloorList { get; set; }
        public List<layerDetail> ISPLayers { get; set; }
        public string StructureName { get; set; }
        public string StructureCode { get; set; }
        public int StructureId { get; set; }
        public int totalOSPCable { get; set; }
        public int total_unit { get; set; }
        public List<NetworkLayer> NetworkLayers { get; set; }
        public int UserRoleId { get; set; }
        public List<ISPNetworkLayerElement> NetworkLayerElements { get; set; }
        public EntityInformation entityInformation { get; set; }
        public List<ISPParentEntities> ParentEntitiesList { get; set; }
        public StructureElement parentElementInfo { get; set; }

        public List<StructureCable> lstStructurCables { get; set; }
        public List<ispShaftRange> lstShaftRange { get; set; }
        public List<LayerActionMapping> listLayerAction { get; set; }
        public List<string> lstUserModule { get; set; }
        public int POPId { get; set; }
        public string POPType { get; set; }
        public string network_status { get; set; }
        public ISPViewModel()
        {
            entityInformation = new EntityInformation();
            lstStructurCables = new List<StructureCable>();
            lstShaftRange = new List<ispShaftRange>();
            lstUserModule = new List<string>();
        }
    }
    public class ispLayers
    {
        public int layer_id { get; set; }
        public string layer_name { get; set; }
        public string layer_title { get; set; }
        public int is_isp_child_layer { get; set; }
    }
    public class ispShaftRange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int shaft_id { get; set; }
        public int structure_id { get; set; }
        public int shaft_start_range { get; set; }
        public int shaft_end_range { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime modified_on { get; set; }
    }
    public class ShaftInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int structure_id { get; set; }
        [Required(ErrorMessage = "Please enter the Shaft Name")]
        public string shaft_name { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public bool is_virtual { get; set; }
        public bool with_riser { get; set; }
        public string shaft_position { get; set; }
        public string network_status { get; set; }
        public bool is_partial_shaft { get; set; }
        [NotMapped]
        public List<FloorInfo> FloorList { get; set; }

        [NotMapped]
        public List<ispShaftRange> ShaftRangelist { get; set; }


        [NotMapped]
        public PageMessage objPM { get; set; }
        public ShaftInfo()
        {
            objPM = new PageMessage();
            ShaftRangelist = new List<ispShaftRange>();
            FloorList = new List<FloorInfo>();
        }
    }
    public class FloorInfo:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int structure_id { get; set; }
        [Required(ErrorMessage = "Please enter the floor Name.")]
        public string floor_name { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [Required(ErrorMessage = "Please enter the floor height.")]
        public double? height { get; set; }
        [Required(ErrorMessage = "Please enter the floor width.")]
        public double? width { get; set; }
        [Required(ErrorMessage = "Please enter the floor length.")]
        public double? length { get; set; }

        public int no_of_units { get; set; }
        public string network_status { get; set; }
        public string network_id { get; set; }
        public string parent_entity_type { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string parent_network_id { get; set; }
        public int parent_system_id { get; set; }
        public int? sequence_id { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public FloorInfo()
        {
            objPM = new PageMessage();
        }

    }
    public class RoomInfo : RoomTemplateMaster,IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int structure_id { get; set; }
        public int floor_id { get; set; }
        public string network_id { get; set; }
        
        public string room_name { get; set; }
        public int? region_id { get; set; }
        public int? province_id { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string network_status { get; set; }
        public int project_id { get; set; }
        public int planning_id { get; set; }
        public int purpose_id { get; set; }
        public int workorder_id { get; set; }
        public int sequence_id { get; set; }
        public string parent_entity_type { get; set; }
        public int parent_system_id { get; set; }
        public string door_type { get; set; }
        public double? door_width { get; set; }
        public string remarks { get; set; }
        public string door_position { get; set; }
        [NotMapped]
        public int templateId { get; set; }
        [NotMapped]
        public int userId { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        public RoomInfo()
        {
            objPM = new PageMessage();
            objIspEntityMap = new IspEntityMapping();
        }


    }
    public class RoomDimension
    {
        public int system_id { get; set; }
        public double room_height { get; set; }
        public double room_width { get; set; }
        public double room_length { get; set; }
    }
    public class HTBInfo : HTBTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom,IReference,IGeographicDetails,IAdditionalFields,ICustomCoordinate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public int structure_id { get; set; }
        public string parent_network_id { get; set; }
        public int floor_id { get; set; }
        public int shaft_id { get; set; }
    
        public string htb_name { get; set; }
        public string acquire_from { get; set; }
        public Boolean is_middleware { get; set; }
        [NotMapped]
        public Boolean is_middlewareInLayer { get; set; }
        public string remarks { get; set; }
        public string htb_type { get; set; }
        public string status { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int? province_id { get; set; }
        public int? region_id { get; set; }
        public int? building_id { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        public string network_status { get; set; }
        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }
        public int sequence_id { get; set; }
        public string parent_entity_type { get; set; }
        public int parent_system_id { get; set; }
        public string utilization { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [Required]
        public string ownership_type { get; set; }
        public string third_party_vendor_id { get; set; }
        public int? primary_pod_system_id { get; set; }
        public int? secondary_pod_system_id { get; set; }
        [NotMapped]
        public string circuit_id { get; set; }
        [NotMapped]
        public string thirdparty_circuit_id { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listOwnership { get; set; }
        [NotMapped]
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        [NotMapped]
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        [NotMapped]
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        [NotMapped]
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }

        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public int userId { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int templateId { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [NotMapped]
        public EntityReference EntityReference { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
        [NotMapped]
        public string elementType { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public List<NELoopDetails> lstLoopMangment { get; set; }
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
        //  public string served_by_ring { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstBOMSubCategory  { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstServedByRing { get; set; }
        public string st_x { get; set; }
        public string st_y { get; set; }
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public HTBInfo()
        {
            objPM = new PageMessage();
            objIspEntityMap = new IspEntityMapping();
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            lstLoopMangment = new List<NELoopDetails>();
            planning_id = 0;
            purpose_id = 0;
            workorder_id = 0;
            project_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
    }
    public class FDBInfo : FDBTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom,IReference,IGeographicDetails,IAdditionalFields,ICustomCoordinate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public int structure_id { get; set; }
        public int floor_id { get; set; }
        public int shaft_id { get; set; }
       
        public string fdb_name { get; set; }
        public string fdb_type { get; set; }
        public string acquire_from { get; set; }
        public string remarks { get; set; }
        public string status { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int? province_id { get; set; }
        public int? region_id { get; set; }
        public int? building_id { get; set; }

        public string network_status { get; set; }
        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }
        public int sequence_id { get; set; }
        public string parent_entity_type { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public int userId { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public int templateId { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [Required]
        public string ownership_type { get; set; }
        public string third_party_vendor_id { get; set; }
        [NotMapped]
        public string circuit_id { get; set; }
        [NotMapped]
        public string thirdparty_circuit_id { get; set; }
        public int? primary_pod_system_id { get; set; }
        public int? secondary_pod_system_id { get; set; }
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
        public PageMessage objPM { get; set; }
        [NotMapped]
        public EntityReference EntityReference { get; set; }
        public string utilization { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public int systemId { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public string elementType { get; set; }
        [NotMapped]
        public int structureid { get; set; }
        [NotMapped]
        public int floorid { get; set; }
        [NotMapped]
        public int shaftid { get; set; }
        [NotMapped]
        public string operation { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }
        [NotMapped]
        public List<NELoopDetails> lstLoopMangment { get; set; }
        public string status_remark { get; set; }
        [NotMapped]
        public List<SpliceTrayInfo> lstSpliceTrayInfo { get; set; }
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
        //  public string served_by_ring { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstBOMSubCategory  { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstServedByRing { get; set; }
        public string st_x { get; set; }
        public string st_y { get; set; }
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        [NotMapped]
        public string file_location { get; set; }
        public FDBInfo()
        {
            objPM = new PageMessage();
            objIspEntityMap = new IspEntityMapping();
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            EntityReference = new EntityReference();
            systemId = 0;
            lstLoopMangment = new List<NELoopDetails>();
            lstSpliceTrayInfo = new List<SpliceTrayInfo>();
            planning_id = 0;
            workorder_id = 0;
            project_id = 0;
            purpose_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }

        public string barcode { get; set; }

        public bool is_manual_barcode { get; set; }
        public bool is_barcode_verified { get; set; }
    }
    public class ElementTemplateView
    {
        public int systemid { get; set; }
        public string Element_type { get; set; }
        public double Element_height { get; set; }
        public double Element_width { get; set; }
        public double Element_length { get; set; }
        public string Element_Template_Name { get; set; }
        public string template_form_url { get; set; }
        public bool isTemplateFilled { get; set; }
        public string entity_category { get; set; }
        public string entityclass { get; set; }

    }
    public class StructureEntityView
    {
        public int systemid { get; set; }
        public string entityname { get; set; }
        public int structureid { get; set; }
        public bool isLeftShaft { get; set; }
        public bool isVirtualShaft { get; set; }
        public bool isshaft { get; set; }
        public double? height { get; set; }
        public double? width { get; set; }
        public double? length { get; set; }
        public bool withriser { get; set; }
        public int no_of_units { get; set; }
        public bool is_partial_shaft { get; set; }
        public int total_Placed_Units { get; set; }
        public int total_Placed_Racks { get; set; }
        public int total_loop { get; set; }
    }
    //public class ShaftFloorElementView
    //{
    //    public int systemid { get; set; }
    //    public int structureid { get; set; }
    //    public int floor { get; set; }
    //    public int shaft { get; set; }
    //    public string elementname { get; set; }
    //    public double element_width { get; set; }
    //    public double element_height { get; set; }
    //    public double element_length { get; set; }
    //    public bool is_shaft_element { get; set; }
    //    public string element_type { get; set; }
    //    public int parent_system_id { get; set; }
    //    public string parent_entity_type { get; set; }

    //}

    public class StructureElement
    {
        public int mapping_id { get; set; }
        public int structure_id { get; set; }
        public int floor_id { get; set; }
        public int shaft_id { get; set; }
        public string entity_type { get; set; }
        public int entity_system_id { get; set; }
        public string network_id { get; set; }
        public string entity_name { get; set; }
        public string entity_title { get; set; }
        public string additional_attributes { get; set; }
        public int parent_mapping_id { get; set; }
        public bool is_Element_Drawn { get; set; }
        public bool is_isp_splicer { get; set; }
        public int? osp_cable_id { get; set; }
        public string total_ports { get; set; }
        public string network_status { get; set; }
        public string entityClass { get; set; }
        public bool is_isp_child_layer { get; set; }
        public string parentEntities { get; set; }
        public string entity_display_name { get; set; }
        public bool is_isp_tp { get; set; }
        public string entity_category { get; set; }
        public bool is_cpe_entity { get; set; }
    }

    public class StructureCable
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public int entity_system_id { get; set; }
        public string cable_name { get; set; }
        public string a_entity_type { get; set; }
        public string b_entity_type { get; set; }
        public int b_system_id { get; set; }
        public int a_system_id { get; set; }
        public int total_core { get; set; }
        public string line_geom { get; set; }
        public string cable_type { get; set; }
        public string network_status { get; set; }
        public string a_location { get; set; }
        public string b_location { get; set; }
        public string entity_name { get; set; }
        public string entity_title { get; set; }
        public string a_node_type { get; set; }
        public string b_node_type { get; set; }
        public string display_name { get; set; }
    }
    public class SplitterParentView
    {
        public int systemid { get; set; }
        public int structureid { get; set; }
        public int floor { get; set; }
        public int shaft { get; set; }
        public string elementname { get; set; }
        public double element_width { get; set; }
        public double element_height { get; set; }
        public double element_length { get; set; }
        public bool is_shaft_element { get; set; }
        public string element_type { get; set; }
        public string network_id { get; set; }

    }
    public class ElementInfo
    {
        public string elementType { get; set; }
        public int structureid { get; set; }
        public int templateId { get; set; }
        public int floorid { get; set; }
        public int shaftid { get; set; }
        public int userid { get; set; }
        public string operation { get; set; }
    }

    public class EntityInformation
    {
        public List<DisplayEntityInformation> lstEntityInformation { get; set; }
        public EntityInformationDetail entityInformationDetail { get; set; }

    }
    public class EntityInformationDetail
    {
        public int system_id { get; set; }
        public string entity_name { get; set; }
        public string entity_title { get; set; }
        public string geom_type { get; set; }
        public string network_status { get; set; }
        // public Dictionary<string, string> EntityKeyValues { get; set; }
        public List<entityInfo> EntityKeyValues { get; set; }
        public List<LayerActionMapping> listLayerAction { get; set; }
    }

    public class DisplayEntityInformation
    {
        public int system_id { get; set; }
        public string entity_name { get; set; }
        public string entity_title { get; set; }
        public string geom_type { get; set; }
        public string network_id { get; set; }
        public string network_stage { get; set; }
        public string network_status { get; set; }
    }
    public class ISPParentEntities
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public string network_id { get; set; }
        public string entity_name { get; set; }
        public string layer_title { get; set; }
        public string display_name { get; set; }
    }
    public class NewEntity
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string entity_type { get; set; }
        public string layer_title { get; set; }
        public bool is_isp_child_layer { get; set; }
        public bool is_isp_parent_layer { get; set; }
        public string geom_type { get; set; }
    }
    public class ShiftEntity
    {
        public int structureId { get; set; }
        public int shaftId { get; set; }
        public int floorId { get; set; }
        public int parent_system_id { get; set; }
        public string parent_entity_type { get; set; }
        public string parent_network_id { get; set; }
        public int entityId { get; set; }
        public string entityType { get; set; }
        public bool isShaftElement { get; set; }
        public bool isFloorElement { get; set; }
        public bool isISPChildLayer { get; set; }
        public List<StructureEntityView> ShaftList { get; set; }
        public List<StructureEntityView> FloorList { get; set; }
        public List<StructureElement> UnitList { get; set; }
        public PageMessage objPM { get; set; }
        public bool isValidParent { get; set; }
        public ShiftEntity()
        {
            objPM = new PageMessage();
            ShaftList = new List<StructureEntityView>();
            FloorList = new List<StructureEntityView>();
            UnitList = new List<StructureElement>();
        }
    }
    public class OpticalRepeaterInfo : OpticalRepeaterTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom,IReference,IGeographicDetails,IAdditionalFields,ICustomCoordinate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public int structure_id { get; set; }
        public string parent_network_id { get; set; }
        public int floor_id { get; set; }
        public int shaft_id { get; set; }
       
        public string opticalrepeater_name { get; set; }
        public string acquire_from { get; set; }
        public bool is_acquire_from { get; set; }
        public string remarks { get; set; }
        public string amplifier_type { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstAmplifierType { get; set; }
        public string amplifier_model { get; set; }
        [Range(1530, 1565)]
        public double amplifier_wavelength { get; set; }
        [Range(12, 16)]
        public double signal_boost_value { get; set; }
        public string noise_figure { get; set; }
        public string special_gain_flatness { get; set; }
        public string status { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int? province_id { get; set; }
        public int? region_id { get; set; }
        public int? building_id { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        public string network_status { get; set; }
        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }
        public int sequence_id { get; set; }
        public string parent_entity_type { get; set; }
        public int parent_system_id { get; set; }
        public string utilization { get; set; }
        [NotMapped]
        public string geom { get; set; }
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
        public IList<DropDownMaster> listOwnership { get; set; }
        [NotMapped]
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        [NotMapped]
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        [NotMapped]
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        [NotMapped]
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }

        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public int userId { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int templateId { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [NotMapped]
        public EntityReference EntityReference { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
        [NotMapped]
        public string elementType { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public List<NELoopDetails> lstLoopMangment { get; set; }
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
        public string st_x { get; set; }
        public string st_y { get; set; }
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public OpticalRepeaterInfo()
        {
            objPM = new PageMessage();
            objIspEntityMap = new IspEntityMapping();
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            lstLoopMangment = new List<NELoopDetails>();
            planning_id = 0;
            project_id = 0;
            purpose_id = 0;
            workorder_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
    }
}
