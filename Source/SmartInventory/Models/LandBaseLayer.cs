using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Models
{
    public class LandBaseLayer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string network_id { get; set; }
        public int landbase_layer_id { get; set; }
        [Required]
        public string name { get; set; }
        [NotMapped]
        public string layer_name { get; set; }
        public string status { get; set; }
        public string attribute_1 { get; set; }
        public string attribute_2 { get; set; }
        public string attribute_3 { get; set; }
        public string attribute_4 { get; set; }
        public string attribute_5 { get; set; }
        public string attribute_6 { get; set; }
        public string attribute_7 { get; set; }
        public string attribute_8 { get; set; }
        public string attribute_9 { get; set; }
        public string attribute_10 { get; set; } 
        public string address { get; set; }
        public int province_id { get; set; }
        public int sequence_id { get; set; }
        public int region_id { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }

        public string province_name { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public double latitude { get; set; }
        [NotMapped]
        public double longitude { get; set; }

        public string region_name { get; set; }
        [NotMapped]
        public string geomType { get; set; }
        //[NotMapped]
        public string sp_geometry { get; set; }
        [NotMapped]
        public int userId { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<LandBaseMaster> lstlandBaseLyaerName { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        //[NotMapped]
        public double buffer_width { get; set; }
        public int? category_id { get; set; }
        public int? sub_category_id { get; set; }
        public int? classification_id { get; set; }
        [NotMapped]
        public bool _isBufferEnable { get; set; }

        [NotMapped]
        public List<LandbaseDropdownMaster> landbaseCategoryList { get; set; }
        [NotMapped]
        public List<LandbaseDropdownMaster> landbaseSubCategoryList { get; set; }
        [NotMapped]
        public List<LandbaseDropdownMaster> landbaseClassificationList { get; set; }

        //[NotMapped]
        public string buffer_geom { get; set; }
        public LandBaseLayer()
        {
            pageMsg = new PageMessage();
            landbaseCategoryList = new List<LandbaseDropdownMaster>();
            landbaseSubCategoryList = new List<LandbaseDropdownMaster>();
            landbaseClassificationList   = new List<LandbaseDropdownMaster>();

        }
    }

    public class LandBaseDetail
    {
        public int att_id { get; set; }
        public int landbase_layer_id { get; set; }
        public string geom_type { get; set; }
        public string name { get; set; }
        public string layer_name { get; set; }
        public string layer_title { get; set; }
        public string network_id { get; set; }
        public string status { get; set; }
        public string geom { get; set; }
        public string centroid_geom { get; set; }

        public bool is_center_line_enable { get; set; }
        //public List<LayerActionMapping> listLayerAction { get; set; }
    }
    public class LandBaseMaster
    {
        public int id { get; set; }
        [Required]
        public string geom_type { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9_ ]*$", ErrorMessage = "Only underscore are allowed")]
        public string layer_name { get; set; }
        [Required]
        public bool is_active { get; set; }
        public bool is_center_line_enable { get; set; }
        [Required]
        public string layer_abbr { get; set; }
        [Required]
        public string map_abbr { get; set; }
        [Required]
        public string map_border_color { get; set; }
        [Required]
        public int map_seq { get; set; }
        [Required]
        public int map_border_thickness { get; set; }
        [Required]
        public string map_bg_color { get; set; }
        [Required]
        public int map_bg_opacity { get; set; }
        [Required]
        public string network_id_type { get; set; }
        [Required]
        public string network_code_seperator { get; set; } 
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }

        public DateTime? modified_on { get; set; }
        public Nullable<int> modified_by { get; set; }
        public string report_view_name { get; set; }
        public string audit_view_name { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public bool IsSubmit { get; set; } = false;
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public int S_No { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public List<LandBaseMaster> lstLayers { get; set; }
        [NotMapped]
        public CommonGridAttributes objGridAttributes { get; set; }
        public bool is_icon_display_enabled { get; set; }
        [NotMapped]
        public string icon_name { get; set; }
        [NotMapped]
        public string icon_path { get; set; }
        [NotMapped]
        public int user_id { get; set; }

        public LandBaseMaster()
        {
            map_border_color = "#ffffff";
            map_bg_color = "#ffffff";
            pageMsg = new PageMessage();
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
            is_active = true;
            map_bg_opacity = 80;
        }
    }
    public class LandBaseInputGeom
    {
        public int id { get; set; }
        public string geomType { get; set; }
        public string longLat { get; set; }
        public int landBaseLayerId { get; set; }
        public string center_line_geom { get; set; }
        public LandBaseInputGeom()
        {

        }
    }
    public class LandbaseNetworkCodeIn
    {
        public int landbase_layer_id { get; set; }
        public string geomType { get; set; }
        public string sp_geometry { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }

    }
    public class LandBaseNetworkCodeOut
    {
        public string network_id { get; set; }
        public int sequence_id { get; set; }
    }
    public class LandbaseGeomDetail
    {
        public string sp_geometry { get; set; }
        public string buffer_geom { get; set; }
        public string layername { get; set; }

    }

    #region EntityExportReport
    public class ExportLandBaseLayerReport
    {
        public ExportLandBaseLayerReportFilter objReportFilters { get; set; }
        public List<Region> lstRegion { get; set; }
        public List<Province> lstProvince { get; set; }
        public List<LandBaseLayerSummaryReport> lstReportData { get; set; }
        public List<LandBaseMaster> lstLayers { get; set; }  
        public List<User> lstParentUsers { get; set; }
        public List<User> lstUsers { get; set; }
        
        public IList<DropDownMaster> lstDurationBasedOn { get; set; } 
        
        [NotMapped]
        public string entityids { get; set; }
        [NotMapped]
        public string fileType { get; set; } 
        public ExportLandBaseLayerReport()
        {
            lstLayers = new List<LandBaseMaster>(); 
            lstProvince = new List<Province>();
            lstReportData = new List<LandBaseLayerSummaryReport>();
            lstRegion = new List<Region>();
            objReportFilters = new ExportLandBaseLayerReportFilter();
            lstUsers = new List<User>();   
        }

    }
    [Serializable]
    public class ExportLandBaseLayerReportFilter
    {
        public int userId { get; set; }
        public int roleId { get; set; }
        public string SelectedLayerIds { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string SelectedParentUsers { get; set; }
       
        public string SelectedProvinceIds { get; set; }
        public string SelectedRegionIds { get; set; }
        public string SelectedUserIds { get; set; }
        
        public string geom { get; set; }
        public string geomType { get; set; }
     
        public List<int> SelectedRegionId { get; set; }
        public List<int> SelectedLayerId { get; set; }
       
        public List<int> SelectedUserId { get; set; }
        public List<int> SelectedParentUser { get; set; }
        public List<int> SelectedProvinceId { get; set; } 
       
        public int customDate { get; set; }
        
        public string durationbasedon { get; set; }
    }

    public class LandBaseLayerSummaryReport
    {
        public int entity_id { get; set; }
        public string entity_title { get; set; }
        public string entity_name { get; set; }
        public int entity_count { get; set; }
    }

    /// <summary>
    /// for view report after entity summary
    /// </summary>
    public class ExportLandBaseLayerSummaryView
    {
        public ExportLandBaseLayerSummaryViewFilter objReportFilters { get; set; }
        public List<dynamic> lstReportData { get; set; }

        public List<WebGridColumn> webColumns { get; set; }
        public List<LandBaseMaster> lstLayers { get; set; }
        public List<KeyValueDropDown> lstLayerColumns { get; set; }
        public List<ReportLandBaseLayerAdvanceFilter> lstAdvanceFilters { get; set; }
        public ExportLandBaseLayerSummaryView()
        {
            lstLayers = new List<LandBaseMaster>();
            lstLayerColumns = new List<KeyValueDropDown>();
            lstReportData = new List<dynamic>();
            webColumns = new List<WebGridColumn>();
            objReportFilters = new ExportLandBaseLayerSummaryViewFilter();
            lstAdvanceFilters = new List<ReportLandBaseLayerAdvanceFilter>();
        }
    }
    public class  GridColumns
    {
        public string column_name { get; set; }
        public string display_name { get; set; }
    }
    [Serializable]
    public class ExportLandBaseLayerSummaryViewFilter
    {
        public int userId { get; set; }
        public int roleId { get; set; }
        public int layerId { get; set; }
        public string SearchbyColumnName { get; set; }
        public string SearchbyBasedOn { get; set; }
        public string SearchbyText { get; set; }
        public string layerName { get; set; }
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }

        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string SelectedParentUsers { get; set; }
        public string SelectedNetworkStatues { get; set; }
        public string SelectedProvinceIds { get; set; }
        public string SelectedRegionIds { get; set; }
        public string SelectedUserIds { get; set; }
        public string SelectedProjectIds { get; set; }
        public string SelectedPlanningIds { get; set; }
        public string SelectedWorkOrderIds { get; set; }
        public string SelectedPurposeIds { get; set; }
        public string geom { get; set; }
        public string geomType { get; set; }
        public string durationbasedon { get; set; }
        public string advancefilter { get; set; }
        public string SelectedThirdPartyVendorIds { get; set; }
        public List<int> SelectedLayerId { get; set; }
        public string filtertype { get; set; }
        public List<Array> lstdynamicobject { get; set; }
        public string SelectedOwnerShipType { get; set; }
        public List<string> SelectedNetworkStatus { get; set; }
        public List<int> SelectedRegionId { get; set; }
        public List<int> SelectedUserId { get; set; }
        public List<int> SelectedParentUser { get; set; }
        public List<int> SelectedProvinceId { get; set; }
        public List<int> SelectedThirdPartyVendorId { get; set; }
        public int customDate { get; set; }
        public List<int> SelectedProjectId { get; set; }
        public List<int> SelectedPlanningId { get; set; }
        public List<int> SelectedWorkOrderId { get; set; }
        public List<int> SelectedPurposeId { get; set; }
        public List<ReportLandBaseLayerAdvanceFilter> lstAdvanceFilters { get; set; }
        public string fileType { get; set; }
        [NotMapped]
        public string SelectedLayerIds { get; set; }
    }

    public class ReportLandBaseLayerAdvanceFilter
    {
        public string searchBy { get; set; }
        public string searchType { get; set; }
        public string searchText { get; set; }
    }

    public class ExportLandBaseLayerSummaryViewKML
    {
        public string entity_title { get; set; }
        public string entity_name { get; set; }
        public string geom_type { get; set; }
        public List<dynamic> lstReportData { get; set; }
    }
    #endregion
public class LandbaseDropdownMaster
    {
        public int id { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
}
