using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Models
{
    public class NetworkLayer : LayerRights
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int layerId { get; set; }
        public string layerName { get; set; }
        public int? parentLayerId { get; set; }
        public string parentLayerName { get; set; }
        public int minZoomLevel { get; set; }
        public int maxZoomLevel { get; set; }
        public string layerAbbr { get; set; }
        public string mapAbbr { get; set; }
        public string layerTitle { get; set; }
        public string layerFormUrl { get; set; }
        public bool isVisibleInNeLibrary { get; set; }
        public bool isNetworkEntity { get; set; }
        public bool isTemplateRequired { get; set; }
        public string networkIdType { get; set; }
        public string geomType { get; set; }
        public string templateFormUrl { get; set; }
        public int templateId { get; set; }
        public string saveEntityUrl { get; set; }
        public bool isDirectSave { get; set; }
        public bool is_visible_in_mobile_lib { get; set; }
        public bool isNetworkTypeRequired { get; set; }
        public string layerNetworkGroup { get; set; }
        public int? maplayerseq { get; set; }
        public bool is_isp_child_layer { get; set; }
        public bool is_shaft_element { get; set; }
        public bool is_isp_parent_layer { get; set; }
        public bool is_floor_element { get; set; }
        public bool isvisible { get; set; }
        public bool is_osp_layer_freezed_in_library { get; set; }
        public bool is_isp_layer_freezed_in_library { get; set; }
        public bool isVirtualPortAllowed { get; set; }
        public bool isLogicalViewEnabled { get; set; }
        public bool is_visible_on_mobile_map { get; set; }
        public bool is_network_ticket_entity { get; set; }
        [NotMapped]
        public string parentEntities { get; set; }
        [NotMapped]
        public double strokewidth { get; set; }
        public bool is_moredetails_enable { get; set; }
        public bool is_mobile_isp_layer { get; set; }
        public bool is_offline_allowed { get; set; }
        //[NotMapped]
        //public LayerRights layer_rights { get; set; }
        //[NotMapped]
        //public bool planned_view { get; set; }
        //[NotMapped]
        //public bool planned_add { get; set; }
        //[NotMapped]
        //public bool planned_edit { get; set; }
        //[NotMapped]
        //public bool planned_delete { get; set; }
        //[NotMapped]
        //public bool asbuild_view { get; set; }
        //[NotMapped]
        //public bool asbuild_add { get; set; }
        //[NotMapped]
        //public bool asbuild_edit { get; set; }
        //[NotMapped]
        //public bool asbuild_delete { get; set; }
        //[NotMapped]
        //public bool dormant_view { get; set; }
        //[NotMapped]
        //public bool dormant_add { get; set; }
        //[NotMapped]
        //public bool dormant_edit { get; set; }
        //[NotMapped]
        //public bool dormant_delete { get; set; }
        [NotMapped]
        public string network_code_seperator { get; set; }
        [NotMapped]
        public string network_code_format { get; set; }
        public bool is_remark_required_from_mobile { get; set; }
        public bool is_isp_layer { get; set; }
        public bool is_split_allowed { get; set; }
        public bool is_association_enabled { get; set; }
        public bool is_association_mandatory { get; set; }

    }
    public class LayerRights
    {
        [NotMapped]
        public bool planned_view { get; set; }
        [NotMapped]
        public bool planned_add { get; set; }
        [NotMapped]
        public bool planned_edit { get; set; }
        [NotMapped]
        public bool planned_delete { get; set; }
        [NotMapped]
        public bool asbuild_view { get; set; }
        [NotMapped]
        public bool asbuild_add { get; set; }
        [NotMapped]
        public bool asbuild_edit { get; set; }
        [NotMapped]
        public bool asbuild_delete { get; set; }
        [NotMapped]
        public bool dormant_view { get; set; }
        [NotMapped]
        public bool dormant_add { get; set; }
        [NotMapped]
        public bool dormant_edit { get; set; }
        [NotMapped]
        public bool dormant_delete { get; set; }
    }

    //public class NetwrokLayerPermission:LayerRights
    //{
    //    public int layerId { get; set; }
    //    public string layerName { get; set; }
    //    public string layerTitle { get; set; }
    //}

    public class RegionProvince
    {
        public int regionId { get; set; }
        public string regionName { get; set; }
        public string regionAbbr { get; set; }
        public int provinceId { get; set; }
        public string provinceName { get; set; }
        public string CountryName { get; set; }
        public string provinceAbbr { get; set; }
        public bool regionVisibility { get; set; }
        public bool provinceVisibility { get; set; }
        public int role_id { get; set; }
    }
    public class MobileRegionProvince
    {
        public int regionId { get; set; }
        public string regionName { get; set; }
        public string regionAbbr { get; set; }
        public int provinceId { get; set; }
        public string provinceName { get; set; }
        public string CountryName { get; set; }
        public string provinceAbbr { get; set; }
        public bool regionVisibility { get; set; }
        public bool provinceVisibility { get; set; }
        public double offline_map_file_size { get; set; }

    }
    public class RegionIn
    {
        public int userId { get; set; }
        public string geom { get; set; }
        public double buffRadius { get; set; }
        public string geomType { get; set; }
    }

    public class ProvinceIn
    {
        public int userId { get; set; }
        public string regionIds { get; set; }
        public string geom { get; set; }
        public double buffRadius { get; set; }
        public string geomType { get; set; }
        [NotMapped]
        public int user_id { get; set; }
    }

    public class Region
    {
        public int regionId { get; set; }
        public string regionName { get; set; }
        public string regionAbbr { get; set; }
        public bool regionVisibility { get; set; }
    }
    public class Province
    {
        public int regionId { get; set; }
        public int provinceId { get; set; }
        public string provinceName { get; set; }
        public string provinceAbbr { get; set; }
        public bool? provinceVisibility { get; set; }
        public int role_id { get; set; }
    }
    public class RegionProvinceLayer
    {
        public int regionId { get; set; }
        public string regionName { get; set; }
        public string regionAbbr { get; set; }

        public bool regionVisibility { get; set; }
        public List<Province> lstProvince { get; set; }
        public string countryname { get; set; }
        public int role_id { get; set; }
        public RegionProvinceLayer()
        {
            lstProvince = new List<Province>();
        }
    }




    public class layerDetail
    {
        [Key]
        public int layer_id { get; set; }
        public string layer_name { get; set; }
        public bool isvisible { get; set; }
        public int minzoomlevel { get; set; }
        public int maxzoomlevel { get; set; }
        public int minboundvalue { get; set; }
        public int maxboundvalue { get; set; }
        public int? parent_layer_id { get; set; }
        public string layer_abbr { get; set; }
        public string map_abbr { get; set; }
        public string layer_title { get; set; }
        public string layer_form_url { get; set; }
        public int? layer_seq { get; set; }
        public bool is_network_entity { get; set; }
        public bool is_visible_in_ne_library { get; set; }
        public bool is_template_required { get; set; }
        public string network_id_type { get; set; }
        public string geom_type { get; set; }
        public string template_form_url { get; set; }
        public string layer_table { get; set; }
        public string network_code_seperator { get; set; }
        //public string network_code_format { get; set; }
        public string save_entity_url { get; set; }
        public bool? is_direct_save { get; set; }
        public bool is_isp_layer { get; set; }
        public bool is_osp_layer { get; set; }
        public bool is_shaft_element { get; set; }
        public bool is_vendor_spec_required { get; set; }
        public string unit_type { get; set; }
        public string unit_input_type { get; set; }
        public bool is_multi_clone { get; set; }
        public bool is_report_enable { get; set; }
        public bool is_label_change_allowed { get; set; }
        public bool is_multi_association { get; set; }
        public bool is_history_enabled { get; set; }
        public bool is_info_enabled { get; set; }
        public bool is_reference_allowed { get; set; }
        public bool is_virtual_port_allowed { get; set; }
        public bool is_isp_child_layer { get; set; }
        public int? map_layer_seq { get; set; }
        public bool is_floor_element { get; set; }
        public bool is_barcode_enabled { get; set; }
        public string barcode_column { get; set; }

        public bool is_at_enabled { get; set; }

        public bool is_maintainence_charges_enabled { get; set; }
        public bool is_row_association_enabled { get; set; }
        public bool is_site_enabled { get; set; }
        public bool is_networkcode_change_enabled { get; set; }
        public bool is_middleware_entity { get; set; }
        public bool is_lmc_enabled { get; set; }
        public bool is_utilization_enabled { get; set; }
        public bool is_layer_for_rights_permission { get; set; }
        public bool is_data_upload_enabled { get; set; }
        public int data_upload_max_count { get; set; }
        public bool is_moredetails_enable { get; set; }
        public string data_upload_table { get; set; }
        public string layer_view { get; set; }
        //[NotMapped]
        public bool is_fiber_link_enabled { get; set; }
        public bool is_loop_allowed { get; set; }
        public bool is_pod_association_allowed { get; set; }
        public bool is_trayinfo_enabled { get; set; }
        public bool is_split_allowed { get; set; }
        public bool is_auto_plan_end_point { get; set; }
        public bool is_tp_layer { get; set; }
        public bool is_clone { get; set; }
        public bool is_mobile_layer { get; set; }
        public bool is_visible_in_mobile_lib { get; set; }
        public bool is_splicer { get; set; }
        public string report_view_name { get; set; }
        public bool is_networktype_required { get; set; }
        public bool is_isp_splicer { get; set; }
        public string layer_network_group { get; set; }
        public string history_view_name { get; set; }
        public bool is_cpe_entity { get; set; }
        public string layer_template_table { get; set; }
        public bool is_isp_parent_layer { get; set; }
        public bool is_feasibility_layer { get; set; }
        public bool is_osp_layer_freezed_in_library { get; set; }
        public bool is_isp_layer_freezed_in_library { get; set; }
        public string feasibility_network_group { get; set; }
        public bool is_other_wcr_layer { get; set; }
        public bool is_logicalview_enabled { get; set; }
        public bool is_project_spec_allowed { get; set; }
        public bool is_visible_on_mobile_map { get; set; }
        public bool is_association_enabled { get; set; }
        public bool is_association_mandatory { get; set; }
        public bool is_network_ticket_entity { get; set; }
        public bool is_mobile_isp_layer { get; set; }
        public string specification_dropdown_type { get; set; }

        public bool is_entity_along_direction { get; set; }
        public bool is_vsat_enabled { get; set; }
        [NotMapped]
        public int id { get; set; }
        [NotMapped]
        public string column_name { get; set; }
        [NotMapped]
        public string description { get; set; }
        [NotMapped]
        public int max_value { get; set; }
        [NotMapped]
        public int min_value { get; set; }
        [NotMapped]
        public string data_type { get; set; }
        [NotMapped]
        public bool is_edit_allowed { get; set; }
        [NotMapped]
        public string column_value { get; set; }
        public string audit_table_name { get; set; }
        public string other_info_view { get; set; }
        public string other_info_view_audit { get; set; }
        public bool is_dynamic_control_enable { get; set; }
        public bool is_dynamic_enabled { get; set; }
        [NotMapped]
        public List<MapScaleSettings> lstzoom { get; set; }
        [NotMapped]
        public List<layerDetail> lstlayerdetails { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public layerDetail()
        {
            lstlayerdetails = new List<layerDetail>();
            lstzoom = new List<MapScaleSettings>();
            pageMsg = new PageMessage();
        }
    }

    public class MapScaleSettings
    {
        [Key]
        public int id { get; set; }
        public int zoom { get; set; }
        public int scale { get; set; }
        public bool isactive { get; set; }
    }

    public class InfoEnityColumname
    {
        [Key]
        public string ordinal_position { get; set; }
        public string column_name { get; set; }


    }

    public class LayerDetails
    {
        public string Layer_name { get; set; }
        public string network_code_format { get; set; }
        public string network_id_type { get; set; }
        public string network_code_seperator { get; set; }
    }

    public class Kmp
    {
        public string id { get; set; }
        public string type { get; set; }
        public string geom { get; set; }
        public string element_id { get; set; }
        public string geo_type { get; set; }
    }
    public class LayerMapping
    {
        public int parent_layer_id { get; set; }
        public string parent_layer_name { get; set; }
        public string parent_geom_type { get; set; }
        public int child_layer_id { get; set; }
        public string child_layer_name { get; set; }
        public string child_geom_type { get; set; }
        public bool is_enable_inside_parent_info { get; set; }
    }
    public class ParentChildLayerMapping
    {
        [Key]
        public int id { get; set; }
        //[Key, Column(Order = 0)]
        public int child_layer_id { get; set; }
        //[Key, Column(Order = 1)]
        public int parent_layer_id { get; set; }
        public string parent_layer_name { get; set; }
        public string parent_geom_type { get; set; }

        public string child_layer_name { get; set; }
        public string child_geom_type { get; set; }
        public bool is_enable_inside_parent_info { get; set; }
        public string child_layer_table { get; set; }
        public string parent_layer_table { get; set; }
        public int parent_sequence { get; set; }
        public bool is_used_for_network_id { get; set; }
        public string network_code_format { get; set; }
        [NotMapped]
        public int order { get; set; }
        public bool is_default_parent { get; set; }
}
    public class vwLayerActionMapping
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public int structure_id { get; set; } 
        public string gis_design_id { get; set; }
        public List<LayerActionMapping> lstLayerActionMapping { get; set; }
        public vwLayerActionMapping()
        {
            lstLayerActionMapping = new List<LayerActionMapping>();
        }
    }
        public class LayerActionMapping
    {
        public int id { get; set; }
        public int layer_id { get; set; }
        public string action_name { get; set; }
        public bool is_active { get; set; }
        public int action_sequence { get; set; }
        public string action_title { get; set; }
        public bool is_visible { get; set; }
        public bool add { get; set; }
        public bool edit { get; set; }
        public bool delete { get; set; }
        public bool viewonly { get; set; }
        public bool is_enabled { get; set; }
        public bool is_template_filled { get; set; }
        public int parent_action_id { get; set; }
        public bool is_buffer_needed { get; set; }
        public string action_abbr { get; set; }
        public int action_layer_id { get; set; }
        public bool delete_authority { get; set; }

    }

    
    public class LayerActionDetails
    {
        [Key]
        public int id { get; set; }
        public int layer_id { get; set; }
        public string action_name { get; set; }
        public bool is_active { get; set; }
        public int action_sequence { get; set; }
        [Required(ErrorMessage = "Action title field is required!")]
        public string action_title { get; set; }
        public bool is_visible { get; set; }
        public bool is_isp_action { get; set; }
        public bool is_osp_action { get; set; }
        public string action_abbr { get; set; }
        public int action_layer_id { get; set; }
        public int action_module_id { get; set; }
        public bool is_mobile_action { get; set; }
        public bool is_web_action { get; set; }
        public int action_mobile_module_id { get; set; }
    }

    public class ISPNetworkLayerDetail
    {
        public int system_id { get; set; }
        public int shaft_id { get; set; }
        public int floor_id { get; set; }
        public string entity_name { get; set; }
        public string entity_title { get; set; }
        public string shaft_name { get; set; }
        public string floor_name { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
    }

    public class ISPLayerFloorDetail
    {
        public string floor_name { get; set; }
        public int floor_entity_count { get; set; }
    }

    public class ISPNetworkLayerElement
    {
        public int structure_id { get; set; }
        public string entity_name { get; set; }
        public string entity_title { get; set; }
        public int entity_count { get; set; }
        public int system_id { get; set; }
        public List<ISPNetworkDisplayElement> listChildElement { get; set; }
    }
    public class ISPNetworkDisplayElement
    {
        public string entity_name { get; set; }
        public string entity_title { get; set; }
        public int entity_count { get; set; }
    }

    public class LayerMapFilter
    {
        public int layer_id { get; set; }
        public string as_built { get { return is_as_built ? "A" : ""; } }
        public string planned { get { return is_planned ? "P" : ""; } }
        public string dormant { get { return is_dormant ? "D" : ""; } }
        public string labels { get { return is_labels ? "L" : ""; } }
        public bool is_as_built { get; set; }
        public bool is_planned { get; set; }
        public bool is_dormant { get; set; }
        public bool is_labels { get; set; }
    }
    public class layerReportDetail
    {
        public int layer_id { get; set; }
        public string layer_name { get; set; }
        public string layer_title { get; set; }
        public string geom_type { get; set; }
        public bool is_utilization_enabled { get; set; }
        public bool is_barcode_enabled { get; set; }
    }

    public class EntityLayerActions
    {
        public int id { get; set; }
        public int layer_id { get; set; }
        public string layer_name { get; set; }
        public string layer_title { get; set; }
        public string action_name { get; set; }
        public string action_title { get; set; }
        public bool is_visible { get; set; }
        public string network_status { get; set; }
        public bool add { get; set; }
        public bool edit { get; set; }
        public bool delete { get; set; }

    }

    public class landBaseLayres
    {
        public int id { get; set; }
        public string layer_name { get; set; }
        public string geom_type { get; set; }
        public string layer_abbr { get; set; }
        public string map_abbr { get; set; }
        public string map_border_color { get; set; }
        public int map_seq { get; set; }
        public int map_border_thickness { get; set; }
        public string map_bg_color { get; set; }
        public int map_bg_opacity { get; set; }
        public bool is_active { get; set; }
        public bool is_center_line_enable { get; set; }
        public string network_id_type { get; set; }
        public string network_code_seperator { get; set; } 
        public string icon_path { get; set; }


        public landBaseLayres()
        {
             
        }
    }
   
    public class TemplateForDropDownLayer
    {
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstEntityDetails { get; set; }


        [NotMapped]
        public List<KeyValueDropDown> lstAllVendor { get; set; }

    }
    public class ViewLayerDetailList
    {
        [Key]
        public int layer_id { get; set; }
        public string layer_name { get; set; }
        public bool isvisible { get; set; }
        public int minzoomlevel { get; set; }
        public int maxzoomlevel { get; set; }
        public int totalRecords { get; set; }

        public int minboundvalue { get; set; }
        public int maxboundvalue { get; set; }
        public int? parent_layer_id { get; set; }
        public string layer_abbr { get; set; }
        public string map_abbr { get; set; }
        public string layer_title { get; set; }
        public string layer_form_url { get; set; }
        public int? layer_seq { get; set; }
        public bool is_network_entity { get; set; }
        public bool is_visible_in_ne_library { get; set; }
        public bool is_template_required { get; set; }
        public string network_id_type { get; set; }
        public string geom_type { get; set; }
        public string template_form_url { get; set; }
        public string layer_table { get; set; }
        public string network_code_seperator { get; set; }
        //public string network_code_format { get; set; }
        public string save_entity_url { get; set; }
        public bool? is_direct_save { get; set; }
        public bool is_isp_layer { get; set; }
        public bool is_osp_layer { get; set; }
        public bool is_shaft_element { get; set; }
        public bool is_vendor_spec_required { get; set; }
        public string unit_type { get; set; }
        public string unit_input_type { get; set; }
        public bool is_multi_clone { get; set; }
        public bool is_report_enable { get; set; }
        public bool is_label_change_allowed { get; set; }
        public bool is_multi_association { get; set; }
        public bool is_history_enabled { get; set; }
        public bool is_info_enabled { get; set; }
        public bool is_reference_allowed { get; set; }
        public bool is_virtual_port_allowed { get; set; }
        public bool is_isp_child_layer { get; set; }
        public int? map_layer_seq { get; set; }
        public bool is_floor_element { get; set; }
        public bool is_barcode_enabled { get; set; }
        public string barcode_column { get; set; }

        public bool is_at_enabled { get; set; }
        public bool is_maintainence_charges_enabled { get; set; }
        public bool is_row_association_enabled { get; set; }
        public bool is_site_enabled { get; set; }
        public bool is_networkcode_change_enabled { get; set; }
        public bool is_middleware_entity { get; set; }
        public bool is_lmc_enabled { get; set; }
        public bool is_utilization_enabled { get; set; }
        public bool is_layer_for_rights_permission { get; set; }
        public bool is_data_upload_enabled { get; set; }
        public int data_upload_max_count { get; set; }
        public bool is_moredetails_enable { get; set; }
        public string data_upload_table { get; set; }
        public string layer_view { get; set; }
        //[NotMapped]
        public bool is_fiber_link_enabled { get; set; }
        public bool is_loop_allowed { get; set; }
        public bool is_pod_association_allowed { get; set; }
        public bool is_trayinfo_enabled { get; set; }
        public bool is_split_allowed { get; set; }
        public bool is_auto_plan_end_point { get; set; }
        public bool is_tp_layer { get; set; }
        public bool is_clone { get; set; }
        public bool is_mobile_layer { get; set; }
        public bool is_visible_in_mobile_lib { get; set; }
        public bool is_splicer { get; set; }
        public string report_view_name { get; set; }
        public bool is_networktype_required { get; set; }
        public bool is_isp_splicer { get; set; }
        public string layer_network_group { get; set; }
        public string history_view_name { get; set; }
        public bool is_cpe_entity { get; set; }
        public string layer_template_table { get; set; }
        public bool is_isp_parent_layer { get; set; }
        public bool is_feasibility_layer { get; set; }
        public bool is_osp_layer_freezed_in_library { get; set; }
        public bool is_isp_layer_freezed_in_library { get; set; }
        public string feasibility_network_group { get; set; }
        public bool is_other_wcr_layer { get; set; }
        public bool is_logicalview_enabled { get; set; }
        public bool is_project_spec_allowed { get; set; }
        public bool is_visible_on_mobile_map { get; set; }
        public bool is_association_enabled { get; set; }
        public bool is_association_mandatory { get; set; }
        public bool is_network_ticket_entity { get; set; }
        public bool is_mobile_isp_layer { get; set; }
        [NotMapped]
        public int id { get; set; }
        [NotMapped]
        public string column_name { get; set; }
        [NotMapped]
        public string description { get; set; }
        [NotMapped]
        public int max_value { get; set; }
        [NotMapped]
        public int mix_value { get; set; }
        [NotMapped]
        public string data_type { get; set; }
        [NotMapped]
        public bool is_edit_allowed { get; set; }
        [NotMapped]
        public string column_value { get; set; }

    }
    public class ViewLayerDetails
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
        public Boolean is_active { get; set; }


    }
    public class ViewLayerSettingDetailList : TemplateForDropDownLayer
    {
        public IList<ViewLayerDetailList> ViewLayerDetailList { get; set; }
        public IList<layerDetail> ViewLayerDetailList1 { get; set; }
        public ViewLayerDetails viewLayerDetails { get; set; }

        public List<dynamic> lstlayerDetails { get; set; }
        public ViewLayerSettingDetailList()
        {
            viewLayerDetails = new ViewLayerDetails();
            lstlayerDetails = new List<dynamic>();
            viewLayerDetails.searchText = string.Empty;
            viewLayerDetails.searchBy = string.Empty;
            viewLayerDetails.is_active = true;
        }

    }

    public class ViewLayergrid : CommonGridAttributes
    {
        public List<layerDetail> listLayerDetailGrid { get; set; }
        public layerDetail objViewMore { get; set; }
        public ViewLayergrid()
        {
            listLayerDetailGrid = new List<layerDetail>();

        }
    }

    public class LayerDetailsColumn
    {
        [Key]
        public int id { get; set; }
        public int layer_id { get; set; }
        public string column_name { get; set; }
        public string description { get; set; }
        public int max_value { get; set; }
        public int min_value { get; set; }
        public string data_type { get; set; }
        public string maxLength { get; set; }
        public bool is_edit_allowed { get; set; }
        public string column_value { get; set; }
        public string column_display_name { get; set; }
        public PageMessage pageMsg { get; set; }
        public ViewLayerDetails viewLayerDetails { get; set; }
        public LayerDetailsColumn()
        {
            pageMsg = new PageMessage();
            viewLayerDetails = new ViewLayerDetails();

            viewLayerDetails.searchText = string.Empty;
            viewLayerDetails.searchBy = string.Empty;
            viewLayerDetails.is_active = true;

        }
    }

    public class UpdateRemarks
    {
        public int? systemId { get; set; }
        public string entityType { get; set; }
        public string networkId { get; set; }
        public string para1 { get; set; }
        public string para2 { get; set; }
        public string para3 { get; set; }
        public string para4 { get; set; }
        public string para5 { get; set; }
    }

    public class SubDistrict
    {
        public int subDistrictId { get; set; }
        public string subDistrictName { get; set; }

    }
    public class Block
    {
        public int blockId { get; set; }
        public string blockName { get; set; }

    }
   

}
