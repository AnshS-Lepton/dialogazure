using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace Models
{

	public class BuildingMaster:IReference,IGeographicDetails
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }
	
		public string building_name { get; set; }
		public string building_no { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string location { get; set; }
		public string area { get; set; }
		public string street { get; set; }
		public string address { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pin_code { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public int surveyarea_id { get; set; }

		[Required(ErrorMessage = "Business pass can't be blank")]
		[Range(0, int.MaxValue, ErrorMessage = "Business pass should be greater than or equal to 0")]
		public int business_pass { get; set; }
		public string building_type { get; set; }
		[Required(ErrorMessage = "Home pass can't be blank")]
		[Range(0, int.MaxValue, ErrorMessage = "Home pass should be greater than or equal to 0")]
		public int home_pass { get; set; }
		[Required(ErrorMessage = "Total tower can't be blank")]
		[Range(1, int.MaxValue, ErrorMessage = "Total tower should be greater than 0")]
		public int total_tower { get; set; }
		public int no_of_floor { get; set; }
		public int no_of_flat { get; set; }
		public int no_of_occupants { get; set; }
		public string building_status { get; set; }
		public string network_status { get; set; }
		public string status { get; set; }
		public int db_flag { get; set; }
		public string cluster_ref { get; set; }
		public string pod_name { get; set; }
		public string pod_code { get; set; }
		public string rfs_status { get; set; }
		public DateTime? rfs_date { get; set; }
		public string customer_name { get; set; }
		public string account_no { get; set; }
		public DateTime? activation_date { get; set; }
		public DateTime? deactivation_date { get; set; }
		public string media { get; set; }
		public string coverage_type_inside { get; set; }
		public string requesting_customer { get; set; }
		public string business_cluster { get; set; }
		public string traffic_status { get; set; }
		public string bldg_status_ring_spur { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public double building_height { get; set; }
		public int sequence_id { get; set; }
		[Required]
		public string tenancy { get; set; }
		[Required]
		public string category { get; set; }
		public string subcategory { get; set; }
		public string gis_address { get; set; }
		public string rwa { get; set; }
		public string rwa_contact_no { get; set; }
		public bool is_mobile { get; set; }
		public string remarks { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public int? status_updated_by { get; set; }
		public DateTime? status_updated_on { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }
		[NotMapped]
		public string createDate { get; set; }
		[NotMapped]
		public string rfsSetDate { get; set; }
		[NotMapped]
		public string surveyarea_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public BuildingAction bldAction { get; set; }
		[NotMapped]
		public int userid { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public int? vsat_system_id { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstTenancy { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstCategory { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstSubCategory { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstRFSStatus { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstMedia { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstBuildingType { get; set; }
		[NotMapped]
		public string childModel { get; set; }
		[NotMapped]
		public int role_id { get; set; }
		[NotMapped]
		public string surveyarea_code { get; set; }

		[NotMapped]
		public List<StructureMaster> lstStructureDetails { get; set; }
		[NotMapped]
		public int CJParent_system_id { get; set; }
		[NotMapped]
		public string CJParent_entity_type { get; set; }
		[NotMapped]
		public int manager_id { get; set; }
		public bool is_virtual { get; set; }
		[NotMapped]
		public string user_comments { get; set; }
		[NotMapped]
		public bool is_mobile_request { get; set; }

		[NotMapped]
		public entityATStatusAtachmentsList ATAcceptance { get; set; }
		[NotMapped]
		public BuildingComments buildingComment { get; set; }
		[NotMapped]
		public string buttonType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstRejectComment { get; set; }
		[NotMapped]
		public string formType { get; set; }
		[NotMapped]
		public List<string> lstUserModule { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public string btnaction { get; set; }
        [NotMapped]
        public string old_rfs_status { get; set; }
        [NotMapped]
       public string new_rfs_status { get; set; }
        [NotMapped]
       public string entityType { get; set; }
        public string status_remark { get; set; }
		[NotMapped]
		public VSATDetails objVSATDetails { get; set; }
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
		[NotMapped]
		public bool IsbldApprovedStatus { get; set; }=false;
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		[NotMapped]
		public vm_dynamic_form objDynamicControls { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
        public string sub_locality { get; set; }
        public string locality { get; set; }
		public bool is_new_entity { get; set; }
		public string other_info { get; set; }
        public string road { get; set; }		
		public BuildingMaster()
		{
			objVSATDetails = new VSATDetails();
			total_tower = 1;
			pageMsg = new PageMessage();
			business_pass = 1;
			home_pass = 1;
			entityType = "Building";
		}
	}

	public class BuildingRfSStatus
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int building_status_id { get; set; }

		public int building_id { get; set; }

		public string rfs_status { get; set; }
		public DateTime created_on { get; set; }
		public DateTime? rfs_date { get; set; }

	}
	public class BuildingComments
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int building_system_id { get; set; }
		public string building_status { get; set; }
		public string comment { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		[NotMapped]
		public string created_by_user { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstRejectComment { get; set; }
		[NotMapped]
		public string childModel { get; set; }
		[NotMapped]
		public string building_code { get; set; }

	}
	public class ExportBuildingComments
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int building_system_id { get; set; }
		public string building_code { get; set; }
		public string building_status { get; set; }
		public string comment { get; set; }
		public string modified_by { get; set; }
		public DateTime created_on { get; set; }

	}
	public class DropDownMaster
	{
		public string dropdown_type { get; set; }
		public string dropdown_value { get; set; }
		public string dropdown_key { get; set; }
		public bool dropdown_status { get; set; }
        public string parent_value { get; set; }
	}

	public class StructureMaster:IReference,IGeographicDetails
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }
		
		public string structure_name { get; set; }
		[Required(ErrorMessage = "Business pass not be blank")]
		[Range(0, int.MaxValue, ErrorMessage = "Business pass should be greater than or equal to 0")]
		public int business_pass { get; set; }
		[Required(ErrorMessage = "Home pass can not be blank")]
		[Range(0, int.MaxValue, ErrorMessage = "Home pass should be greater than or equal to 0")]
		public int home_pass { get; set; }
		//[Required(ErrorMessage = "No of Floor can't be blank")]
		//[Range(1, int.MaxValue, ErrorMessage = "No of Floor greater than 0")]
		[Required]
		public int no_of_floor { get; set; }
		[Required(ErrorMessage = "No of units not be blank")]
		[Range(1, int.MaxValue, ErrorMessage = "No of units should be greater than 0")]
		public int no_of_flat { get; set; }
		[Required(ErrorMessage = "No of Occupant not be blank")]
		[Range(1, int.MaxValue, ErrorMessage = "No of Occupant should be greater than 0")]
		public int no_of_occupants { get; set; }
		[Required]
		public int no_of_shaft { get; set; }
		[Required]
		public int building_id { get; set; }
		[Required]
		public int province_id { get; set; }
		[Required]
		public int region_id { get; set; }
		public string network_status { get; set; }
		public string status { get; set; }
		public string remarks { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }
		[NotMapped]
		public int userid { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public List<StructureShaftInfo> lstShaftInfo { get; set; }
		[NotMapped]
		public List<StructureFloorInfo> lstFloorInfo { get; set; }

		[NotMapped]
		public List<ISP.StructureElement> lstStructureElements { get; set; }
		[NotMapped]
		public string prevShaftWithoutRiser { get; set; }

		[NotMapped]
		public entityATStatusAtachmentsList ATAcceptance { get; set; }

		[NotMapped]
		public SiteInfo SiteInformation { get; set; }

		[NotMapped]
		public List<SiteCustomer> lstsitecustomer { get; set; }
		[NotMapped]
		public bool isDefault { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        [NotMapped]
        public string actionTab { get; set; }

        public string status_remark { get; set; }
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
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string entityType { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
		[NotMapped]
		public List<string> lstUserModule { get; set; }
		public StructureMaster()
		{
			pageMsg = new PageMessage();
			lstShaftInfo = new List<StructureShaftInfo>();
			lstFloorInfo = new List<StructureFloorInfo>();
			no_of_floor = 1;
			no_of_flat = 1;
			no_of_occupants = 1;
			lstStructureElements = new List<ISP.StructureElement>();
			lstsitecustomer = new List<SiteCustomer>();
			entityType = "Structure";
			lstUserModule = new List<string>();
		}
	}
	public class InRegionProvince
	{
		[Key]
		public int region_id { get; set; }
		public string region_name { get; set; }
		public string region_abbreviation { get; set; }
		public int province_id { get; set; }
		public string province_name { get; set; }
		public string province_abbreviation { get; set; }
	}

	public class InSuraveyArea
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string surveyarea_name { get; set; }
	}
    public class BuildingSurveyDetails
    {
        public string locality { get; set; }
        public string sub_locality { get; set; }
        public string geom { get; set; }
        public string city { get; set; }
        public string state_name { get; set; }
    }

    public class DbMessage
	{
		public bool status { get; set; }
		public string message { get; set; }
		public string result { get; set; }
	}
    public class RouteCreation
    {
        public bool STATUS { get; set; }
        public string ROUTEID { get; set; }
    }
    public class DbMessageForPlan
    {
        public bool status { get; set; }
        public string message { get; set; }

        public int plan_id { get; set; }
    }


	public class StructureShaftInfo
	{
		public int system_id { get; set; }
		public string shaft_name { get; set; }
		public bool is_virtual { get; set; }
		public bool with_riser { get; set; }
		public double length { get; set; }
		public double width { get; set; }
		public string shaft_position { get; set; }
		public bool is_partial_shaft { get; set; }
		public List<System.Web.Mvc.SelectListItem> lstPosition { get; set; }

		public StructureShaftInfo()
		{
			system_id = 0;
			// shaft_position = "left";
			length = 0;
			width = 0;
			lstPosition = new List<SelectListItem>{
								new SelectListItem{ Text="right", Value = "right" },
								new SelectListItem{ Text="left", Value = "left" }};
		}
	}
	public class StructureList
	{

		public int system_id { get; set; }
		public string structure_name { get; set; }

	}

	public class StructureFloorInfo
	{
		public int system_id { get; set; }
		public string floor_name { get; set; }
		public int no_of_units { get; set; }
		public double? length { get; set; }
		public double? width { get; set; }
		public double? height { get; set; }
		public int total_Placed_Units { get; set; }

		public StructureFloorInfo()
		{
			system_id = 0;
			length = 0;
			width = 0;
			height = 0;
		}
	}




	public class TempBuildingMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string building_name { get; set; }
		public string building_no { get; set; }
		public string address { get; set; }
		public string province_id { get; set; }
		public string region_id { get; set; }
		public string pin_code { get; set; }
		public string area { get; set; }
		public string tenancy { get; set; }
		public string category { get; set; }
		public string media { get; set; }
		public string location { get; set; }
		public string rfs_date { get; set; }
		public string rfs_status { get; set; }
		public string pod_name { get; set; }
		public string pod_code { get; set; }
		public string no_of_floor { get; set; }
		public string total_tower { get; set; }
		public string business_pass { get; set; }
		public string home_pass { get; set; }
		public string latitude { get; set; }
		public string longitude { get; set; }
		public string building_type { get; set; }
		public string remarks { get; set; }
		public int uploaded_by { get; set; }
		public bool is_valid { get; set; }
		public string error_msg { get; set; }
		[NotMapped]
		public int userid { get; set; }
		public string province_name { get; set; }
		public string regionname { get; set; }
	}

	public class CheckboxMaster
	{
		public string Value { get; set; }
		public string Text { get; set; }
		public bool IsChecked { get; set; }
		public string checkbox_type { get; set; }
	}

	public class SiteInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }

		//  [Required(ErrorMessage ="Site Name is required!")]
		public string site_name { get; set; }
		//[Required(ErrorMessage = "Circle is required!")]
		public string circle { get; set; }
		//[Required(ErrorMessage = "City is required!")]
		public string city { get; set; }
		//[Required(ErrorMessage = "LMC Type is required!")]
		public string lmc_type { get; set; }
		//[Required(ErrorMessage = "Latitude is required!")]
		public double latitude { get; set; }
		//[Required(ErrorMessage = "Longitude is required!")]
		public double longitude { get; set; }
		//[Required(ErrorMessage = "ETPIL ID is required!")]
		public string etpil_id { get; set; }
		//[Required(ErrorMessage = "Site Type is required!")]
		public string site_type { get; set; }
		//[Required(ErrorMessage = "Site Vendor is required!")]
		public string site_vendor { get; set; }
		//[Required(ErrorMessage = "Acq. Vendor is required!")]
		public string acq_vendor { get; set; }
		//[Required(ErrorMessage = "Structure Type is required!")]
		public string structure_type { get; set; }
		//[Required(ErrorMessage = "Structure Size is required!")]
		public string structure_size { get; set; }
		//[Required(ErrorMessage = "Building Height is required!")]
		public double building_height { get; set; }
		//[Required(ErrorMessage = "Structure Height is required!")]
		public double structure_height { get; set; }
		[Required]
		public bool is_ladder_required { get; set; }
		//[Required(ErrorMessage = "Site Area is required!")]
		public string site_area { get; set; }
		//[Required(ErrorMessage = "Home Pass is required!")]
		public int home_pass { get; set; }
		//[Required(ErrorMessage = "Acq. Date is required!")]
		public DateTime? acquisition_date { get; set; }
		public string parent_entity_type { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public DateTime created_on { get; set; }
		public int created_by { get; set; }
		public DateTime? modified_on { get; set; }
		public int modified_by { get; set; }
		[NotMapped]
		public string ActionTab { get; set; }

		[NotMapped]
		public List<SiteCustomer> lstSiteCustomer { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstLMCType { get; set; }

		[NotMapped]
		public List<DropDownMaster> lstSITEType { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstStructureType { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstStructureSize { get; set; }
		[NotMapped]
		public List<SiteCircleList> lstSiteCircle { get; set; }

		[NotMapped]
		public List<SiteInfo> lstSiteInfo { get; set; }
		[NotMapped]
		public List<siteVendorList> lstSiteVendor { get; set; }
		[NotMapped]
		public List<SiteCustomerFilter> lstsitecustomerfilter { get; set; }
		[NotMapped]
		public List<LibraryAttachment> lstSiteInfoAttachment { get; set; }
		//public List<SiteInfoAttachment> lstSiteInfoAttachment { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		public List<dynamic> lstData { get; set; }
		public SiteInfo()
		{
			lstSiteCustomer = new List<SiteCustomer>();
			lstSiteInfo = new List<SiteInfo>();
			lstSiteVendor = new List<siteVendorList>();
			lstsitecustomerfilter = new List<SiteCustomerFilter>();
			lstSiteInfoAttachment = new List<LibraryAttachment>();
			objPM = new Models.PageMessage();
			lstData = new List<dynamic>();
		}
	}
	public class SiteCustomer
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public int site_id { get; set; }
		//[Required(ErrorMessage ="Customer Site Id is Required!")]
		public string customer_site_id { get; set; }

		public string lmc_type { get; set; }
		//[Required(ErrorMessage = "Customer Name is Required!")] 
		public string customer_name { get; set; }
		// [Required(ErrorMessage = "Telco Name is Required!")]
		public string telco_name { get; set; }
		//[Required(ErrorMessage = "Customer area is Required!")]
		public string customer_area { get; set; }
		//[Required(ErrorMessage = "OPCO is Required!")]
		public string opco { get; set; }
		//[Required(ErrorMessage = "Electrical Meter Type is Required!")]
		public string electrical_meter_type { get; set; }
		//[Required(ErrorMessage = "RTN Name is Required!")]
		public string rtn_name { get; set; }
		//[Required(ErrorMessage = "Small Cell Installed is Required!")]
		public int small_cell_installed { get; set; }
		//[Required(ErrorMessage = "AGL is Required!")]
		public int agl { get; set; }
		public bool is_power_back_up_available { get; set; }
		//[Required(ErrorMessage = "Power bakup capacity is Required!")]
		public int power_back_up_capacity { get; set; }
		//[Required(ErrorMessage = "Cluster Name is Required!")]
		public string cluster_name { get; set; }

		public string co_name { get; set; }
		public string co_id { get; set; }
		public double co_latitude { get; set; }
		public double co_longitude { get; set; }
		//[Required(ErrorMessage = "Cluster ID is Required!")]
		public string cluster_id { get; set; }
		//[Required(ErrorMessage = "PAF No is Required!")]
		public string paf_no { get; set; }
		//[Required(ErrorMessage = "PAF Signing Date is Required!")]
		public DateTime? paf_signing_date { get; set; }
		//[Required(ErrorMessage = "PAF Expiry Date is Required!")]
		public DateTime? paf_expiry_date { get; set; }
		//[Required(ErrorMessage = "Cable Entry Point is Required!")]
		public string cable_entry_point { get; set; }
		//[Required(ErrorMessage = "Remote POP is Required!")]
		public string remote_pop { get; set; }
		//[Required(ErrorMessage = "Order Tenure is Required!")]
		public int order_tenure { get; set; }
		//[Required(ErrorMessage = "Site Details is Required!")]
		public string site_details { get; set; }
		//[Required(ErrorMessage = "PO Number is Required!")]
		public string po_number { get; set; }
		//[Required(ErrorMessage = "PO Issue Date is Required!")]
		public DateTime? po_issue_date { get; set; }
		//[Required(ErrorMessage = "PO Expiry Date is Required!")]
		public DateTime? po_expiry_date { get; set; }
		//[Required(ErrorMessage = "RFAI Date is Required!")]
		public DateTime? rfai_date { get; set; }
		//[Required(ErrorMessage = "RFS Date is Required!")]
		public DateTime? rfs_date { get; set; }
		//[Required(ErrorMessage = "Contract Ends Date is Required!")]
		public DateTime? contract_end_date { get; set; }
		public DateTime created_on { get; set; }
		public int created_by { get; set; }
		public DateTime? modified_on { get; set; }
		public int modified_by { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		[NotMapped]
		public int pageSize { get; set; }
		[NotMapped]
		public int totalRecord { get; set; }
		[NotMapped]
		public int currentPage { get; set; }
		[NotMapped]
		public string sort { get; set; }
		[NotMapped]
		public string sortdir { get; set; }
		[NotMapped]
		public string orderby { get; set; }
		[NotMapped]
		public string childModel { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstElectricalmeter { get; set; }

		[NotMapped]
		public List<DropDownMaster> lstCableEntryPoints { get; set; }
		[NotMapped]
		public List<LibraryAttachment> lstSiteCustomerAttachment { get; set; }
		//public List<SiteCustomerAttachment> lstSiteCustomerAttachment { get; set; }

		public SiteCustomer()
		{
			objPM = new PageMessage();
			lstSiteCustomerAttachment = new List<LibraryAttachment>();
		}
	}

	public class siteVendorList
	{
		public string siteVendor { get; set; }
	}
	public class SiteCustomerFilter
	{
		public int system_id { get; set; }
		public int site_id { get; set; }
		public string lmc_type { get; set; }
		public string customer_site_id { get; set; }
		public string customer_name { get; set; }
		public string telco_name { get; set; }
		public string customer_area { get; set; }
		public string opco { get; set; }
		public string electrical_meter_type { get; set; }
		public string rtn_name { get; set; }
		public int small_cell_installed { get; set; }
		public int agl { get; set; }
		public bool is_power_back_up_available { get; set; }
		public int power_back_up_capacity { get; set; }
		public string cluster_name { get; set; }
		public string co_name { get; set; }
		public string co_id { get; set; }
		public double co_latitude { get; set; }
		public double co_longitude { get; set; }
		public string cluster_id { get; set; }
		public string paf_no { get; set; }
		public DateTime? paf_signing_date { get; set; }
		public DateTime? paf_expiry_date { get; set; }
		public string cable_entry_point { get; set; }
		public string remote_pop { get; set; }
		public int order_tenure { get; set; }
		public string site_details { get; set; }
		public string po_number { get; set; }
		public DateTime? po_issue_date { get; set; }
		public DateTime? po_expiry_date { get; set; }
		public DateTime? rfai_date { get; set; }
		public DateTime? rfs_date { get; set; }
		public DateTime? contract_end_date { get; set; }
		public DateTime created_on { get; set; }
		public string created_by { get; set; }
		public DateTime? modified_on { get; set; }
		public string modified_by { get; set; }
		public List<dynamic> lstData { get; set; }
		public SiteCustomerFilter()
		{
			lstData = new List<dynamic>();
		}

	}

	public class SiteCircleList
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public string circle_name { get; set; }
		public string circle_type { get; set; }

	}
	public class InGeographicDetails
	{
		public string entity_type { get; set; }
		public string entity_network_id { get; set; }
		public string region_abbreviation { get; set; }
		public string province_abbreviation { get; set; }
	}
    public class Customer_Response
    {
        public string reference_id { get; set; }
        public string ticketid { get; set; }
    }
    public class DbMessageForDaFiFeasibility
    {
        public string status { get; set; }
        public string message { get; set; }

    }
}
