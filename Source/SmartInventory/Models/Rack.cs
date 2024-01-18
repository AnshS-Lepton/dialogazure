using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Admin;

namespace Models
{
    public class RackInfo : IProjectSpecification,IReference,IGeographicDetails,IAdditionalFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public int structure_id { get; set; }
        
        public string rack_name { get; set; }
        public string rack_type { get; set; }
        public string remarks { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int? province_id { get; set; }
        public int? region_id { get; set; }
        public string network_status { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string status { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double border_width { get; set; }
        public int no_of_units { get; set; }

        public double pos_x { get; set; }
        public double pos_y { get; set; }
        public double pos_z { get; set; }

        public string item_code { get; set; }
        public int vendor_id { get; set; }
        public string specification { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public int sequence_id { get; set; }
        public string border_color { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstAllVendor { get; set; }
        [NotMapped]
        public bool is_view_enabled { get; set; }
        public int? project_id { get; set; }

        public int? planning_id { get; set; }

        public int? workorder_id { get; set; }
        public int audit_item_master_id { get; set; }

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
        public List<DropDownMaster> lstRackType { get; set; }
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
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        public RackInfo()
        {
            lstRackType = new List<DropDownMaster>();
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            entityType = "Rack";
        }
    }
     
    public class EquipmentInfo : EquipmentTemplateMaster, IProjectSpecification, IReference, IGeographicDetails, IAdditionalFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public int? structure_id { get; set; }

        public string model_name { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int? province_id { get; set; }
        public int? region_id { get; set; }
        public string network_status { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double border_width { get; set; }
        public double pos_x { get; set; }
        public double pos_y { get; set; }
        public double pos_z { get; set; }

        //public string item_code { get; set; }
        //public int? vendor_id { get; set; }
        //public string specification { get; set; }
        //public string category { get; set; }
        //public string subcategory1 { get; set; }
        //public string subcategory2 { get; set; }
        //public string subcategory3 { get; set; }
        public int model_id { get; set; }
        public int? model_type_id { get; set; }
        public int? model_image_id { get; set; }
        public int sequence_id { get; set; }
        public int model_info_id { get; set; }

        public double rotation_angle { get; set; }
        public int super_parent { get; set; }
        public int model_view_id { get; set; }
        public string equipment_name { get; set; }
        public int rack_id { get; set; }
        public double unit_size { get; set; }
        public int? label_info_id { get; set; }
        public int? port_number { get; set; }
        public bool is_multi_connection { get; set; }
        public string short_network_id { get; set; }
        public string border_color { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }

        [NotMapped]
        public string image_data { get; set; }
        [NotMapped]
        public string color_code { get; set; }
        [NotMapped]
        public string stroke_code { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool isEditable { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool isStatic { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstAllVendor { get; set; }

        [NotMapped]
        public int item_template_id { get; set; }

        [NotMapped]
        public string model_type { get; set; }

        [NotMapped]
        public List<PortConnecton> lstPortConnection { get; set; }
        [NotMapped]
        public List<EquipmentInfo> lstEquipmentInfo { get; set; }
        [NotMapped]
        public string fms_network_id { get; set; }
        [NotMapped]
        public string super_parent_network_id { get; set; }
        [NotMapped]
        public string super_parent_model_type { get; set; }
        [NotMapped]
        public string font_size { get; set; }
        [NotMapped]
        public string font_color { get; set; }
        [NotMapped]
        public string text_orientation { get; set; }
        [NotMapped]
        public string background_image { get; set; }
        [NotMapped]
        public bool is_view_enabled { get; set; }
        [NotMapped]
        public string port_status { get; set; }
        [NotMapped]
        public int? port_status_id { get; set; }
        [NotMapped]
        public string port_comment { get; set; }
        [NotMapped]
        public string port_status_color { get; set; }
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
        public bool is_internal_connectivity_enabled { get; set; }
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

        public EquipmentInfo()
        {
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
    }

    public class EquipmentExportResult
    {
        public string equipment_type { get; set; }
        public string equipment_id { get; set; }
        public string equipment_name { get; set; }
        public string parent_type { get; set; }
        public string parent_id { get; set; }
        public string address { get; set; }
        public string vendor { get; set; }
        public string item_code { get; set; }
        public string specification { get; set; }
        public string description { get; set; }


    }
    public class nodelist
    {
        [NotMapped]
        public string network_id { get; set; }
        [NotMapped]
        public int child_id { get; set; }

        [NotMapped]
        public string child_name { get; set; }
        [NotMapped]
        public int parent_id { get; set; }
        [NotMapped]
        public int sequence_id { get; set; }

        [NotMapped]
        public int connection_count { get; set; }
        [NotMapped]
        public int model_id { get; set; }
        [NotMapped]
        public int? port_number { get; set; }
    }

    public class PortConnecton
    {
        public string source_network_id { get; set; }
        public int source_port_no { get; set; }
        public string destination_network_id { get; set; }
        public int destination_port_no { get; set; }
    }

    public class ModelConnection
    {

        public int rowid { get; set; }
        public int system_id { get; set; }
        public string entity_type { get; set; }

        public string network_id { get; set; }

        public int core_port_number { get; set; }

        public int? destination_system_id { get; set; }
        public string destination_entity_type { get; set; }

        public string destination_network_id { get; set; }

        public int? destination_port_no { get; set; }

        public int? source_port_id { get; set; }
        public string source_port_network_id { get; set; }
        public int? destination_port_id { get; set; }
        public string destination_port_network_id { get; set; }
    }



}
