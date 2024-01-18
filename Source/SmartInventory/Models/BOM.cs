using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Models.Admin;

namespace Models
{

    public class BOMReport
    {

        public int id { get; set; }
        public string entity_type { get; set; }
        public string geom_type { get; set; }
        public string entity_sub_type { get; set; }
        public decimal cost_per_unit { get; set; }
        public double service_cost_per_unit { get; set; }
        public string specification { get; set; }
        //priyanka
        public string served_by_ring { get; set; }
        public string item_code { get; set; }
        public string name { get; set; }
        public long total_planned_count { get; set; }
        public long total_asbuilt_count { get; set; }
        public long total_dormant_count { get; set; }
        public decimal total_planned_cost { get; set; }
        public decimal total_asbuilt_cost { get; set; }
        public decimal total_dormant_cost { get; set; }
        public decimal total_planned_service_cost { get; set; }
        public decimal total_asbuilt_service_cost { get; set; }
        public decimal total_dormant_service_cost { get; set; }
        public decimal total_planned_calc_length { get; set; }
        public decimal total_asbuilt_calc_length { get; set; }
        public decimal total_dormant_calc_length { get; set; }
        public decimal total_planned_gis_length { get; set; }
        public decimal total_asbuilt_gis_length { get; set; }
        public decimal total_dormant_gis_length { get; set; }
        public bool is_header { get; set; }

    }

    [Serializable]
    public class BOMInput
    {
        public int userId { get; set; }
        public int roleId { get; set; }
        public string geom { get; set; }
        public string geomType { get; set; }
        public double buff_Radius { get; set; }
        public string network_status { get; set; }
        public List<int> regionId { get; set; }
        public List<int> provinceId { get; set; }
        public string SelectedProvinceIds { get; set; }
        public string SelectedRegionIds { get; set; }
        public bool isAssociatedEntityBomBoq { get; set; }
        public int systemId { get; set; }
        public string entityType { get; set; }

        public BOMInput()
        {
            buff_Radius = 0.0;
            regionId = new List<int>();
            provinceId = new List<int>();
        }
    }

    public class BOMViewModel
    {
        public List<Region> lstRegion { get; set; }
        public List<Province> lstProvince { get; set; }
        public BOMInput objBomInput { get; set; }
        public List<BOMReport> lstBomReport { get; set; }
        public string childModel { get; set; }
        public BOMViewModel()
        {
            objBomInput = new BOMInput();
            lstBomReport = new List<BOMReport>();
            lstRegion = new List<Region>();
            lstProvince = new List<Province>();
        }
    }


    /////////////Krishna

    #region BOMBOQ   Model               --


    public class BOMBOQReport
    {

        public int id { get; set; }
        public string entity_type { get; set; }
        public string geom_type { get; set; }
        public string entity_sub_type { get; set; }
        public decimal cost_per_unit { get; set; }
        public double service_cost_per_unit { get; set; }
        public string specification { get; set; }
        public string served_by_ring { get; set; }
        public string item_code { get; set; }
        public string name { get; set; }
        public long total_count { get; set; }
        public decimal total_cost { get; set; }
        public decimal total_service_cost { get; set; }
        public decimal calculated_length { get; set; }
        public decimal gis_length { get; set; }

        public string NetworkStatus { get; set; }
        public bool is_header { get; set; }

    }

    public class BomBoqExportReport
    {
        public BomBoqExportFilter objReportFilters { get; set; }
        public List<Region> lstRegion { get; set; }
        public List<Province> lstProvince { get; set; }
        public List<EntitySummaryReport> lstReportData { get; set; }
        public List<layerReportDetail> lstLayers { get; set; }

        public List<User> lstParentUsers { get; set; }
        public List<User> lstUsers { get; set; }

        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
        public IList<DropDownMaster> lstDurationBasedOn { get; set; }

        public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
        [NotMapped]
        public string entityids { get; set; }
        [NotMapped]
        public string fileType { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstNetworkStatus { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        

        public BomBoqExportReport()
        {
            lstLayers = new List<layerReportDetail>();
            //lstLayerColumns = new List<KeyValueDropDown>();
            lstProvince = new List<Province>();
            lstReportData = new List<EntitySummaryReport>();
            lstRegion = new List<Region>();
            objReportFilters = new BomBoqExportFilter();
            lstUsers = new List<User>();
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            //listOwnership = new List<DropDownMaster>();
            list3rdPartyVendorId = new List<KeyValueDropDown>();
            lstUserModule = new List<string>();

        }

        public List<Models.Admin.LinkBudgetMaster> lstWave_length { get; set; }

        public List<BOMBOQReport> BomBoqReportList { get; set; }

        public BomBoqAdAttribute objAdAttribute { get; set; }
        public List<dBLossDetail> objdBLoss { get; set; }
    }

    [Serializable]
    public class BomBoqExportFilter
    {
        public int userId { get; set; }
        public int roleId { get; set; }
        public string SelectedLayerIds { get; set; }
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
        public string SelectedThirdPartyVendorIds { get; set; }
        public List<string> SelectedNetworkStatus { get; set; }
        public List<int> SelectedRegionId { get; set; }
        public List<int> SelectedLayerId { get; set; }
        public string SelectedOwnerShipType { get; set; }
        public List<int> SelectedUserId { get; set; }
        public List<int> SelectedParentUser { get; set; }
        public List<int> SelectedProvinceId { get; set; }
        public List<int> SelectedThirdPartyVendorId { get; set; }
        public int customDate { get; set; }
        public List<int> SelectedProjectId { get; set; }
        public List<int> SelectedPlanningId { get; set; }
        public List<int> SelectedWorkOrderId { get; set; }
        public List<int> SelectedPurposeId { get; set; }

        public string durationbasedon { get; set; }
        public double radius { get; set; }
        public int SelectedWavelength { get; set; }
        public double txt_Miscdbloss { get; set; } = 0.00;
        [NotMapped]
        public bool isdBLossAttrEnable { get; set; } = false;
        public int systemId { get; set; }
        public string entityType { get; set; }
        public string connectionString { get; set; }
    }

    public class BomBoqAdAttribute
    {
        public int id { get; set; }
        public int userid { get; set; }
        [Required]
        public string title { get; set; }
        [NotMapped]
        public string equipmenttype { get; set; }
        [NotMapped]
        public string equipmentname { get; set; }
        [NotMapped]
        public string popname { get; set; }
        [Required]
        public string estimatedby { get; set; }
        [Required]
        public string checkedby { get; set; }
        [Required]
        public string re_checkedby { get; set; }
        [Required]
        public string approvedby { get; set; }
        public DateTime? modified_on { get; set; }

        [NotMapped]
        public bool isEquipmentEnable { get; set; } = true;
        [NotMapped]
        public bool isAdditionalAttrEnable { get; set; } = true;

    }

    public class dBLossDetail
    {
        public string cable_category { get; set; }
        public float total_cable_length { get; set; }
        [NotMapped]
        public int cable_spliced_core_count { get; set; }
        public float cable_total_splice_loss { get; set; }
        public float misc_loss { get; set; }
        [NotMapped]
        public float totalLoss { get; set; }

    }



    [Serializable]
    public class BomBoqExportFilterDesign
    {
        public int userId { get; set; }
        public string user_name { get; set; }
        public int systemId { get; set; }
        public string entityName { get; set; }
        public int bom_boq_id { get; set; }
        [NotMapped]
        public string action { get; set; }
        public List<BOMBOQReport> BomBoqReportList { get; set; }
    }


    public class BomBoqInfoSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bom_boq_id { get; set; }
        public int fsa_system_id { get; set; }
        public string fsa_id { get; set; }
        public string fsa_name { get; set; }
        public string revision { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
        public int created_by_user_id { get; set; }
        public string created_by_user_name { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by_user_id { get; set; }
        public string modified_user_name { get; set; }
        public DateTime modified_on { get; set; }
        [NotMapped]
        public string action { get; set; }
        [NotMapped]
        public List<BomBoqInfo> lstBomBoqInfo { get; set; }
    }
    public class BomBoqInfo
    {
        public int id { get; set; }
        public int bom_boq_id { get; set; }
        public int fsa_system_id { get; set; }
        public string fsa_id { get; set; }
        public string fsa_name { get; set; }
        public string entity_type { get; set; }
        public string entity_class { get; set; }
        public string entity_sub_class { get; set; }
        public string specification { get; set; }
        public double design_qty { get; set; }
        public double additional_qty { get; set; }
        public double additional_non_design_material_qty { get; set; }
        public string item_code { get; set; }
        public string pts_code { get; set; }
        public double cost_per_unit { get; set; }
        public string sub_category_1 { get; set; }
        public string sub_category_2 { get; set; }
        public string uom_sap { get; set; }
        public int revision { get; set; }
        public double final_qty { get; set; }
        public double jpf_total_capex { get; set; }
        public string jc_sap_id { get; set; }
        public string jc_name { get; set; }
        public double gis_qty { get; set; }
        public string gis_uom { get; set; }
        public bool is_master { get; set; }
        public double overhead_percentage { get; set; }
        public int created_by_user_id { get; set; }
        public string created_by_user_name { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by_user_id { get; set; }
        public string modified_user_name { get; set; }
        public DateTime modified_on { get; set; }
        public string item_master_code { get; set; }
        public double original_design_qty { get; set; }
        public bool is_non_design_item { get; set; }
        public int hierarchy_level { get; set; }
        public bool is_dependent_calculation { get; set; }
        public int display_sequence { get; set; }
        public bool is_design_qty_editable { get; set; }
    }

    public class ConstructionBomDetailsInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string state { get; set; }
        public string jc { get; set; }
        public string town_name { get; set; }
        public string town_code { get; set; }
        public string fsa_id { get; set; }
        public double feeder_length { get; set; }
        public double distribution_length { get; set; }
        public double pole_count { get; set; }
        public string oh_feeder_cable { get; set; }
        public string oh_distribution_cable { get; set; }
        public string oh_clamps { get; set; }
        public string oh_jclamps { get; set; }
        public string oh_termination_set { get; set; }
        public string oh_suspension_set { get; set; }
        public int totalRecords { get; set; }
        public int page_count { get; set; }
        public int S_No { get; set; }
        public string feeder_gis_length { get; set; }
        public string distribution_gis_length { get; set; }
        public string jubilee_clamp { get; set; }
        public string termination_set { get; set; }
        public string suspension_set { get; set; }
        public string clamp { get; set; }
        public int fsa_system_id { get;set; }
        public DateTime? modified_on { get; set; }
        public bool is_default { get; set; }
    }
    public class ConstructionBomDetailsVM : TemplateForDropDownConstructionBom
    {
        public List<ConstructionBomDetailsInfo> constructionBomDetailList { get; set; }
        public ViewConstructionBomDetails viewConstructionBomDetails { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public string modified_on { get; set; }
        public ConstructionBomDetailsVM()
        {
            viewConstructionBomDetails = new ViewConstructionBomDetails();
            viewConstructionBomDetails.searchText = string.Empty;
            lstUserModule = new List<string>();
        }
    }
    public class ConstructionBomDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string state { get; set; }
        public string jc { get; set; }
        public string town_name { get; set; }
        public string town_code { get; set; }
        public string fsa_id { get; set; }
        public double feeder_length { get; set; }
        public double distribution_length { get; set; }
        public double pole_count { get; set; }
        public string oh_feeder_cable { get; set; }
        public string oh_distribution_cable { get; set; }
        public string oh_clamps { get; set; }
        public string oh_jclamps { get; set; }
        public string oh_termination_set { get; set; }
        public string oh_suspension_set { get; set; }
        [NotMapped]
        public string source { get; set; }

    }
    public class ViewConstructionBomDetails
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
    }
    public class TemplateForDropDownConstructionBom
    {
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }
    }
	#endregion
	public class FSAlockedDetailsInfo
	{
		public int id { get; set; }
		public string state { get; set; }
		public string jc { get; set; }
		public string town_name { get; set; }
		public string town_code { get; set; }
		public string fsa_id { get; set; }
		public int fsa_system_id { get; set; }
		public int totalRecords { get; set; }
		public int page_count { get; set; }
		public int S_No { get; set; }
		public DateTime? modified_on { get; set; }
        public string modified_by { get; set; }


    }
    public class FSAlockedDetailsVM : TemplateForlockedFSA
	{
		public List<FSAlockedDetailsInfo> lockedFSADetailList { get; set; }
		public ViewLockedDetails viewLockedDetails { get; set; }
		[NotMapped]
		public List<string> lstUserModule { get; set; }
		public DateTime? modified_on { get; set; }
        public string modified_by { get; set; }

        public FSAlockedDetailsVM()
		{
			viewLockedDetails = new ViewLockedDetails();
			viewLockedDetails.searchText = string.Empty;
			lstUserModule = new List<string>();
		}
	}
	public class ViewLockedDetails
	{
		public int pageSize { get; set; }
		public int totalRecord { get; set; }
		public int currentPage { get; set; }
		public string sort { get; set; }
		public string orderBy { get; set; }
		public string searchText { get; set; }
		public string searchBy { get; set; }
	}
	public class TemplateForlockedFSA
	{
		[NotMapped]
		public List<KeyValueDropDown> lstBindSearchBy { get; set; }
	}
}
