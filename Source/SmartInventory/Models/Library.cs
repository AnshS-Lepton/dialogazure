using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Models.Admin;
using System.Web.Helpers;
using Models.ISP;
using System.Collections;
using System.Web.Mvc;

namespace Models
{
	public interface IProjectSpecification
	{
		//string project_name { get; set; }
		//string project_code { get; set; }
		//string planning_name { get; set; }
		//string planning_code { get; set; }
		//string purpose_name { get; set; }
		//string purpose_code { get; set; }
		//string workorder_name { get; set; }
		//string workorder_code { get; set; }
		int? project_id { get; set; }
		int? planning_id { get; set; }
		int? workorder_id { get; set; }
		int? purpose_id { get; set; }
		List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		List<PurposeCodeMaster> lstBindPurposeCode { get; set; }

	}

	public interface IOwnershipInfo
	{
		string ownership_type { get; set; }
		string third_party_vendor_id { get; set; }
		string circuit_id { get; set; }
		string thirdparty_circuit_id { get; set; }
		string entityType { get; set; }
		[NotMapped]
		List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
	}


	public interface IInstallationInfo
	{
		string installation_number { get; set; }
		string installation_year { get; set; }
		string production_year { get; set; }
		string installation_company { get; set; }
		string installation_technician { get; set; }
		string installation { get; set; }
		int created_by { get; set; }
		DateTime created_on { get; set; }
		int? modified_by { get; set; }
		DateTime? modified_on { get; set; }
	}
	public class InstallationInfo : IInstallationInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int installation_id { get; set; }
		public int entity_system_id { get; set; }
		public string entity_type { get; set; }
		public string installation_number { get; set; }
		public string installation_year { get; set; }
		public string production_year { get; set; }
		public string installation_company { get; set; }
		public string installation_technician { get; set; }
		public string installation { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
	}



	public class ADBMaster : ADBTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string adb_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string acquire_from { get; set; }
		public string remarks { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		//[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

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
		public string status { get; set; }
		public string network_status { get; set; }
		public bool is_servingdb { get; set; }
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
		public EntityReference EntityReference { get; set; }
		public string utilization { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<SpliceTrayInfo> lstSpliceTrayInfo { get; set; }
		public string status_remark { get; set; }
		public bool is_new_entity { get; set; }
		[NotMapped]
		public int split_cable_system_id { get; set; }
		[NotMapped]
		public int structure_id { get; set; }

		[NotMapped]
		public IspEntityMapping objIspEntityMap { get; set; }
		//public int? shaft_id { get; set; }
		//public int? floor_id { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
        public ADBMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();
			lstSpliceTrayInfo = new List<SpliceTrayInfo>();
			//shaft_id = 0;
			//floor_id = 0;
			objIspEntityMap = new IspEntityMapping();
			planning_id = 0;
			workorder_id = 0;
			purpose_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
	}

	public class PODMaster : PODTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }
		public string pod_name { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string acquire_from { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? structure_id { get; set; }
		public int? shaft_id { get; set; }
		public int? floor_id { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		//[NotMapped]
		//public List<ShaftFloorList> lstStructure { get; set; }
		[NotMapped]
		public List<ShaftFloorList> lstShaft { get; set; }
		[NotMapped]
		public List<ShaftFloorList> lstFloor { get; set; }

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
		public bool isDirectSave { get; set; }
		[NotMapped]
		public int user_id { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		[NotMapped]
		public AdditionalAttributes extraAttributes { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
		public PODMaster()
		{

			shaft_id = 0;
			floor_id = 0;
			lstShaft = new List<ShaftFloorList>();
			lstFloor = new List<ShaftFloorList>();
			objPM = new PageMessage();

			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			objIspEntityMap = new IspEntityMapping();
			parent_entity_type = string.Empty;
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();
			extraAttributes = new AdditionalAttributes();
			planning_id = 0;
			purpose_id = 0;
			workorder_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
			lstUserModule = new List<string>();
		}



	}

	public class ADBSubArea
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string subarea_name { get; set; }
	}

	public class PoleArea
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string area_name { get; set; }
	}


	public class PoleMaster : PoleTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string pole_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public string address { get; set; }
		public string pole_height { get; set; }
		public string pole_no { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string acquire_from { get; set; }
		public string remarks { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }


		public string status { get; set; }
		public string network_status { get; set; }
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
		public string status_remark { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }

		[NotMapped]
		public EntityReference EntityReference { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		public int? primary_pod_system_id { get; set; }
		public int? secondary_pod_system_id { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstServedByRing { get; set; }
		public string st_x { get; set; }
		public string st_y { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
		[NotMapped]
		public List<string> lstUserModule { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public List<RouteInfo> lstRouteInfo { get; set; }
        [NotMapped]
        public List<int> selected_route_ids { get; set; }
        public PoleMaster()
		{
			objPM = new PageMessage();

			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			//lstLoopMangment = new List<NELoopDetails>();
			lstLoopMangment = new List<NELoopDetails>();
			EntityReference = new EntityReference();
            lstRouteInfo = new List<RouteInfo>();
            planning_id = 0;
			workorder_id = 0;
			purpose_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
			lstUserModule = new List<string>();
		}
	}
	public class ProjectSpecification
	{
		public string project_name { get; set; }
		public string project_code { get; set; }
		public string planning_name { get; set; }
		public string planning_code { get; set; }
		public string purpose_name { get; set; }
		public string purpose_code { get; set; }
		public string workorder_name { get; set; }
		public string workorder_code { get; set; }
	}

	public class SplitterMaster : SplitterTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string splitter_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }
		public string acquire_from { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		public int structure_id { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }
		[NotMapped]
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
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
		public int templateId { get; set; }
		[NotMapped]
		public IspEntityMapping objIspEntityMap { get; set; }
		[NotMapped]
		public string adb_network_code { get; set; }
		[NotMapped]
		public string cdb_network_code { get; set; }
		[NotMapped]
		public string splitter_network_code { get; set; }
		[NotMapped]
		public EntityReference EntityReference { get; set; }
		[NotMapped]
		public int available_ports { get; set; }
		public string utilization { get; set; }

		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstServedByRing { get; set; }
		[NotMapped]
		public int addSplitterInBox = 0;
		public string st_x { get; set; }
		public string st_y { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public bool is_meter_reading_verified { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string file_location { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }		
		public double? power_meter_reading { get; set; }
        [NotMapped]
        public List<RouteInfo> lstRouteInfo { get; set; }
        [NotMapped]
        public List<int> selected_route_ids { get; set; }
        public SplitterMaster()
		{
			objPM = new PageMessage();
			objIspEntityMap = new IspEntityMapping();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			EntityReference = new EntityReference();
			project_id = 0;
			planning_id = 0;
			workorder_id = 0;
			purpose_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
            lstRouteInfo = new List<RouteInfo>();
        }
	}

	public class TreeMaster : TreeTemplateMaster, IProjectSpecification, IReference, IGeographicDetails, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string tree_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }
		public string tree_height { get; set; }
		public string tree_no { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
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
		public int? primary_pod_system_id { get; set; }
		public int? secondary_pod_system_id { get; set; }


		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }

		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }

		[NotMapped]
		public EntityReference EntityReference { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		public string status_remark { get; set; }
		public bool is_new_entity { get; set; }
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
		public string st_x { get; set; }
		public string st_y { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
		[NotMapped]
		public List<string> lstUserModule { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        public TreeMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			lstLoopMangment = new List<NELoopDetails>();
			EntityReference = new EntityReference();
			project_id = 0;
			workorder_id = 0;
			planning_id = 0;
			purpose_id = 0;
			lstUserModule = new List<string>();

		}
	}

	public class ManholeMaster : ManholeTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string manhole_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		public bool is_virtual { get; set; }
		public bool is_buried { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
        public string mcgm_ward { get; set; }
        public string status { get; set; }
		public string network_status { get; set; }
		public string construction_type { get; set; }
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
		[Required]
		public string ownership_type { get; set; }
		public string third_party_vendor_id { get; set; }
		[NotMapped]
		public string circuit_id { get; set; }
		[NotMapped]
		public string thirdparty_circuit_id { get; set; }
		public int? primary_pod_system_id { get; set; }
		public int? secondary_pod_system_id { get; set; }
		public string status_remark { get; set; }
		public string acquire_from { get; set; }
		public bool is_acquire_from { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public IList<DropDownMaster> listConstructionType { get; set; }
		[NotMapped]
		public EntityReference EntityReference { get; set; }
        [NotMapped]
        public IList<DropDownMaster> MCGMWardIn { get; set; }

        [NotMapped]
		public entityATStatusAtachmentsList ATAcceptance { get; set; }

		[NotMapped]
		public EntityMaintainenceChargesList MaintainenceCharges { get; set; }

		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		public bool is_new_entity { get; set; }
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
		//	public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstServedByRing { get; set; }
		public string st_x { get; set; }
		public string st_y { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
        public string route_name { get; set; }
        public string area { get; set; }
        public string authority { get; set; }
        [NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public List<RouteInfo> lstRouteInfo { get; set; }
        [NotMapped]
        public List<int> selected_route_ids { get; set; }
        public ManholeMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			lstLoopMangment = new List<NELoopDetails>();
            lstRouteInfo = new List<RouteInfo>();
            planning_id = 0;
			purpose_id = 0;
			workorder_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
	}

	public class CouplerMaster : CouplerTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string coupler_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public double inner_dimention { get; set; }
		public double outer_dimention { get; set; }
		public string coupler_type { get; set; }
		public string acquire_from { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		public bool is_virtual { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }


		public string status { get; set; }
		public string network_status { get; set; }
		public string construction_type { get; set; }
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
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		[NotMapped]
		public IList<DropDownMaster> listCouplerType { get; set; }
		[NotMapped]
		public EntityReference EntityReference { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstServedByRing { get; set; }
		public string st_x { get; set; }
		public string st_y { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }

		public CouplerMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			planning_id = 0;
			purpose_id = 0;
			workorder_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
	}
	public class CDBMaster : CDBTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string cdb_name { get; set; }
		public bool is_servingdb { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string acquire_from { get; set; }
		public string remarks { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

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

		public string status { get; set; }
		public string network_status { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
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
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string pNetworkId { get; set; }
		[NotMapped]
		public EntityReference EntityReference { get; set; }
		[NotMapped]
		public bool isConvert { get; set; }
		[NotMapped]
		public int sc_system_id { get; set; }
		public string utilization { get; set; }
		[NotMapped]
		public int no_of_ports { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<SpliceTrayInfo> lstSpliceTrayInfo { get; set; }
		public string status_remark { get; set; }
		public bool is_new_entity { get; set; }
		[NotMapped]
		public int split_cable_system_id { get; set; }
		[NotMapped]
		public int structure_id { get; set; }

		[NotMapped]
		public IspEntityMapping objIspEntityMap { get; set; }
		//public int? shaft_id { get; set; }
		//public int? floor_id { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
        public CDBMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			lstLoopMangment = new List<NELoopDetails>();
			lstSpliceTrayInfo = new List<SpliceTrayInfo>();
			//shaft_id = 0;
			//floor_id = 0;
			objIspEntityMap = new IspEntityMapping();
			planning_id = 0;
			purpose_id = 0;
			project_id = 0;
			workorder_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
	}
	public class BDBMaster : BDBTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string bdb_name { get; set; }
		public bool is_servingdb { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string acquire_from { get; set; }
		public string remarks { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }
		[NotMapped]
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string pNetworkId { get; set; }

		[NotMapped]
		public PageMessage objPM { get; set; }
		public string status { get; set; }
		public string network_status { get; set; }
		public int? shaft_id { get; set; }
		public int? floor_id { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
		[NotMapped]
		public int templateId { get; set; }
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
		public int structure_id { get; set; }

		[NotMapped]
		public IspEntityMapping objIspEntityMap { get; set; }

		[NotMapped]
		public EntityReference EntityReference { get; set; }
		public string utilization { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<SpliceTrayInfo> lstSpliceTrayInfo { get; set; }
		public string status_remark { get; set; }
		public bool is_new_entity { get; set; }
		[NotMapped]
		public int split_cable_system_id { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
        public List<RouteInfo> lstRouteInfo { get; set; }
        [NotMapped]
        public List<int> selected_route_ids { get; set; }
        public BDBMaster()
		{
			shaft_id = 0;
			floor_id = 0;
			objIspEntityMap = new IspEntityMapping();
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			parent_entity_type = string.Empty;
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();
			lstSpliceTrayInfo = new List<SpliceTrayInfo>();
			purpose_id = 0;
			project_id = 0;
			workorder_id = 0;
			planning_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
            lstRouteInfo = new List<RouteInfo>();
        }
	}
	public class SCMaster : SCTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string spliceclosure_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		[NotMapped]
		public string subcode { get; set; }
		public string acquire_from { get; set; }
		public bool is_buried { get; set; }

		public string address { get; set; }
		public string remarks { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		public bool is_virtual { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }

		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
		//public int? structure_id { get; set; }
		[NotMapped]
		public int structure_id { get; set; }
		public int no_of_ports { get; set; }
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
		public int templateId { get; set; }
		[NotMapped]
		public IspEntityMapping objIspEntityMap { get; set; }
		[NotMapped]
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string pNetworkId { get; set; }
		[NotMapped]
		public EntityReference EntityReference { get; set; }
		[NotMapped]
		public bool isConvert { get; set; }
		[NotMapped]
		public int cdb_system_id { get; set; }
		public string utilization { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<SpliceTrayInfo> lstSpliceTrayInfo { get; set; }
        [NotMapped]
        public List<RouteInfo> lstRouteInfo { get; set; }        
        public string status_remark { get; set; }
		public bool is_new_entity { get; set; }
		[NotMapped]
		public int split_cable_system_id { get; set; }
		[NotMapped]
		public string associated_entity_type { get; set; }
		[NotMapped]
		public int associated_system_id { get; set; }
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
        [NotMapped]
        public List<int> selected_route_ids { get; set; }
        //public string served_by_ring { get; set; }
        [NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
		public SCMaster()
		{
			objPM = new PageMessage();
			objIspEntityMap = new IspEntityMapping();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();
			lstSpliceTrayInfo = new List<SpliceTrayInfo>();
			lstRouteInfo = new List<RouteInfo>();
			project_id = 0;
			planning_id = 0;
			workorder_id = 0;
			purpose_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
			lstUserModule = new List<string>();
		}
	}

	public class FMSMaster : FMSTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string fms_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string acquire_from { get; set; }
		[NotMapped]
		public string subcode { get; set; }

		public string address { get; set; }
		public string remarks { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }

		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
		public int structure_id { get; set; }
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
		public int templateId { get; set; }
		[NotMapped]
		public IspEntityMapping objIspEntityMap { get; set; }
		[NotMapped]
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string pNetworkId { get; set; }
		public string utilization { get; set; }
		[NotMapped]
		public string display_name { get; set; }
		[NotMapped]
		public string DisplayNameWithPort
		{
			get
			{
				return string.Format("{0}-({1})-({2})", display_name, no_of_port, network_status);
			}
		}
		[NotMapped]
		public bool is_isp_mapped { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }
		public string status_remark { get; set; }
		public bool is_new_entity { get; set; }
		[NotMapped]
		public int split_cable_system_id { get; set; }
		public bool is_acquire_from { get; set; }
		public dynamic other_info { get; set; }
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
		//public string served_by_ring { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
        public List<RouteInfo> lstRouteInfo { get; set; }
        [NotMapped]
        public List<int> selected_route_ids { get; set; }
        public FMSMaster()
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
			project_id = 0;
			workorder_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
            lstRouteInfo = new List<RouteInfo>();
        }
	}



	public class ISPModelType
	{
		public int id { get; set; }
		public int? created_by { get; set; }
		public DateTime? modified_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? created_on { get; set; }
		public bool is_active { get; set; }
		public int? model_id { get; set; }
		public string key { get; set; }
		public string value { get; set; }
		public string color_code { get; set; }
		public string stroke_code { get; set; }
		public bool is_middleware_model_type { get; set; }
		public string type_abbr { get; set; }
		public int? model_color_id { get; set; }

	}


	public class MPODMaster : MPODTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string mpod_name { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public string acquire_from { get; set; }
		public string remarks { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
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
		public bool isDirectSave { get; set; }
		[NotMapped]
		public int user_id { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }

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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
        public MPODMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			objIspEntityMap = new IspEntityMapping();
			parent_entity_type = string.Empty;
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();
			planning_id = 0;
			purpose_id = 0;
			workorder_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
	}
	//ROOMMANAGEMENT
	public class UnitMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }
		[Required]
		public string room_name { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public string acquire_from { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
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
		public bool isDirectSave { get; set; }
		[NotMapped]
		public int user_id { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }


		public UnitMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			objIspEntityMap = new IspEntityMapping();
			parent_entity_type = string.Empty;
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();

		}
	}
	//ROOMMANAGEMENT
	public class FloorMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }
		[Required]
		public string floor_name { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public string acquire_from { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
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
		public bool isDirectSave { get; set; }
		[NotMapped]
		public int user_id { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }


		public FloorMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			objIspEntityMap = new IspEntityMapping();
			parent_entity_type = string.Empty;
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();

		}
	}

	public class ONTMaster : ONTTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }
		public string ont_name { get; set; }
		[Required]
		public string cpe_type { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public string serial_no { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string acquire_from { get; set; }
		public string remarks { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		public int structure_id { get; set; }
		[NotMapped]
		public int templateId { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }
		[NotMapped]
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string pNetworkId { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
		public int? primary_pod_system_id { get; set; }
		public int? secondary_pod_system_id { get; set; }
		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		[NotMapped]
		public IspEntityMapping objIspEntityMap { get; set; }
		[NotMapped]
		public bool isPortAssociated { get; set; }
		[NotMapped]
		public EntityReference EntityReference { get; set; }
		public string utilization { get; set; }
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
		public int user_id { get; set; }
		[NotMapped]
		public int systemId { get; set; }
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
		public int userid { get; set; }
		[NotMapped]
		public string operation { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }
		public string status_remark { get; set; }
		public bool is_new_entity { get; set; }
		public bool is_acquire_from { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstCpeType { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
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
        public ONTMaster()
		{
			objIspEntityMap = new IspEntityMapping();
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			pEntityType = string.Empty;
			isPortAssociated = false;
			EntityReference = new EntityReference();
			systemId = 0;
			isDirectSave = false;
			lstLoopMangment = new List<NELoopDetails>();
			project_id = 0;
			planning_id = 0;
			workorder_id = 0;
			purpose_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
	}
	public class WallMountMaster : WallMountTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string wallmount_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }
		public string wallmount_height { get; set; }
		public string wallmount_no { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string acquire_from { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public EntityReference EntityReference { get; set; }

		[NotMapped]
		public int user_id { get; set; }

		[NotMapped]
		public bool isDirectSave { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
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
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstServedByRing { get; set; }
		public string st_x { get; set; }
		public string st_y { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
		[NotMapped]
		public List<string> lstUserModule { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }

        public WallMountMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			lstLoopMangment = new List<NELoopDetails>();
			EntityReference = new EntityReference();
			planning_id = 0;
			workorder_id = 0;
			purpose_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
			lstUserModule = new List<string>();
		}
	}

	public class ShaftFloorList
	{
		public int systemid { get; set; }
		public string entityname { get; set; }
		public int structureid { get; set; }
		public bool isLeftShaft { get; set; }
		public bool isVirtualShaft { get; set; }
		public bool isshaft { get; set; }

	}

	public class ExportEntitiesReport
	{
		public ExportReportFilter objReportFilters { get; set; }
		public List<Region> lstRegion { get; set; }
		public List<Province> lstProvince { get; set; }
		public List<dynamic> lstReportData { get; set; }
		public List<layerDetail> lstLayers { get; set; }
		public List<KeyValueDropDown> lstLayerColumns { get; set; }
		public List<KeyValueDropDown> lstLayerDurationBasedColumns { get; set; }

		public ExportEntitiesReport()
		{
			lstLayers = new List<layerDetail>();
			lstLayerColumns = new List<KeyValueDropDown>();
			lstProvince = new List<Province>();
			lstReportData = new List<dynamic>();
			lstRegion = new List<Region>();
			objReportFilters = new ExportReportFilter();
			lstLayerDurationBasedColumns = new List<KeyValueDropDown>();
		}

	}
	[Serializable]
	public class ExportReportFilter
	{
		public int userId { get; set; }
		public int layerId { get; set; }
		public string SearchbyColumnName { get; set; }
		public string SearchbyText { get; set; }
		public string fromDate { get; set; }
		public string toDate { get; set; }
		public string layerName { get; set; }
		public string SelectedNetworkStatues { get; set; }
		public string SelectedProvinceIds { get; set; }
		public string SelectedRegionIds { get; set; }
		public string geom { get; set; }
		public string geomType { get; set; }
		public List<string> SelectedNetworkStatus { get; set; }
		public List<int> SelectedRegionId { get; set; }
		public int[] SelectedProvinceId { get; set; }
		public int pageSize { get; set; }
		public int totalRecord { get; set; }
		public int currentPage { get; set; }
		public string sort { get; set; }
		public string sortdir { get; set; }
		public int customDate { get; set; }
		public string DurationBasedColumnName { get; set; }
		public double radius { get; set; }

	}


	public class ExportReportKML
	{
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string entity_title { get; set; }
		public string geom { get; set; }
		public string network_id { get; set; }
		public string entity_name { get; set; }
		public string geom_type { get; set; }
	}
	#region Loop Mangment

	public class NELoopDetails : IReference, IGeographicDetails
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public string network_id { set; get; }
		public double? loop_length { set; get; }
		public int associated_system_id { get; set; }
		public string associated_network_id { get; set; }
		public string associated_entity_type { get; set; }
		public int cable_system_id { set; get; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		public int? created_by { set; get; }
		public DateTime? created_on { set; get; }
		public int? modified_by { set; get; }
		public DateTime? modified_on { set; get; }
		[NotMapped]
		public string cable_id { set; get; }
		public string network_status { get; set; }
		[NotMapped]
		public string cable_name { get; set; }
		public double? start_reading { get; set; }
		public double? end_reading { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		[NotMapped]
		public string associated_System_Type { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		[NotMapped]
		public int structure_id { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }
		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }
		public string status_remark { get; set; }
		[NotMapped]
		public double? cable_calculated_length { get; set; }
		[NotMapped]
		public int total_loop_count { get; set; }
		[NotMapped]
		public double? available_calculated_length { get; set; }
		[NotMapped]
		public int total_loop_length { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
		public bool is_new_entity { get; set; }
		[NotMapped]
		public List<FormInputSettings> formInputSettings { get; set; }
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
		public NELoopDetails()
		{
			objPM = new PageMessage();
			structure_id = 0;
			lstLoopMangment = new List<NELoopDetails>();
			entityType = "Loop";
		}
	}





    //public class NELoopCables
    //{
    //    //public string system_id { set; get; }
    //    //public string cable_id { set; get; }

    //     [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    //    public int system_id { get; set; }
    //    public string cable_system_id { set; get; }
    //    public string cable_id { set; get; }
    //    public string network_id { set; get; }
    //    public int loop_length { set; get; }
    //    [NotMapped]
    //    public int associated_system_id { get; set; }
    //    [NotMapped]
    //    public string associated_entity_type { get; set; }

    //    [NotMapped]
    //    public string associated_network_id { get; set; }

    //    [NotMapped]
    //    public int? created_by { get; set; }

    //    [NotMapped]
    //    public int? modified_by { get; set; }

    //}



    #endregion Loop Mangment

    #region EntityExportReport
    public class ConnectionMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string connection_string { get; set; }
        public string module_abbr { get; set; }        
    }
    public class ExportEntitiesReportNew
	{
        public static List<Models.ConnectionMaster> lstConnections { get; set; }
        public ExportReportFilterNew objReportFilters { get; set; }
		public List<Region> lstRegion { get; set; }
		public List<Province> lstProvince { get; set; }
		public List<EntitySummaryReport> lstReportData { get; set; }
		public List<layerReportDetail> lstLayers { get; set; }
		//public List<KeyValueDropDown> lstLayerColumns { get; set; }
		//need to define for parent user
		public List<User> lstParentUsers { get; set; }
		public List<User> lstUsers { get; set; }
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		public IList<DropDownMaster> lstDurationBasedOn { get; set; }
		//public List<DropDownMaster> listOwnership { get; set; }
		public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
		[NotMapped]
		public string entityids { get; set; }
		[NotMapped]
		public string fileType { get; set; }
		public List<fileTypes> lstfiletypes { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstNetworkStatus { get; set; }
		[NotMapped]
        public List<string> lstUserModule { get; set; }
        [NotMapped]
        public List<int> selected_route_ids { get; set; }
        [NotMapped]
        public List<RouteInfo> lstRouteInfo { get; set; }
        public ExportEntitiesReportNew()
		{
			lstLayers = new List<layerReportDetail>();
			//lstLayerColumns = new List<KeyValueDropDown>();
			lstProvince = new List<Province>();
			lstReportData = new List<EntitySummaryReport>();
			lstRegion = new List<Region>();
			objReportFilters = new ExportReportFilterNew();
			lstUsers = new List<User>();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			// listOwnership = new List<DropDownMaster>();
			list3rdPartyVendorId = new List<KeyValueDropDown>();
            lstUserModule = new List<string>();
			lstfiletypes = new List<fileTypes>();
            lstRouteInfo = new List<RouteInfo>();
        }

	}
	[Serializable]
	public class ExportReportFilterNew
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
		public List<int> SelectedProvinceId { get; set; }//int[]
		public List<int> SelectedThirdPartyVendorId { get; set; }
		public int customDate { get; set; }
		public List<int> SelectedProjectId { get; set; }
		public List<int> SelectedPlanningId { get; set; }
		public List<int> SelectedWorkOrderId { get; set; }
		public List<int> SelectedPurposeId { get; set; }

		public string durationbasedon { get; set; }
        public bool is_all_provience_assigned { get; set; }
		public double radius { get; set; }
        public string connectionString { get; set; }
        public List<int> selected_route_id { get; set; }//int[]
        public string selected_route_ids { get; set; }
    }

	public class EntitySummaryReport
	{
		public int entity_id { get; set; }
		public string entity_title { get; set; }
		public string entity_name { get; set; }
		public int planned_count { get; set; }
		public int as_built_count { get; set; }
		public int dormant_count { get; set; }
	}

	/// <summary>
	/// for view report after entity summary
	/// </summary>
	public class ExportEntitiesSummaryView
	{
		public ExportEntitiesSummaryViewFilter objReportFilters { get; set; }
		public List<dynamic> lstReportData { get; set; }

		public List<WebGridColumn> webColumns { get; set; }
		public List<layerReportDetail> lstLayers { get; set; }
		public List<KeyValueDropDown> lstLayerColumns { get; set; }
		public List<ReportAdvanceFilter> lstAdvanceFilters { get; set; }
		public ExportEntitiesSummaryView()
		{
			lstLayers = new List<layerReportDetail>();
			lstLayerColumns = new List<KeyValueDropDown>();
			lstReportData = new List<dynamic>();
			webColumns = new List<WebGridColumn>();
			objReportFilters = new ExportEntitiesSummaryViewFilter();
			lstAdvanceFilters = new List<ReportAdvanceFilter>();
		}
	}
	public class WebGridColumns
	{
		public string column_name { get; set; }
		public string display_name { get; set; }
	}
	[Serializable]
	public class ExportEntitiesSummaryViewFilter
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
		public List<ReportAdvanceFilter> lstAdvanceFilters { get; set; }
		public string fileType { get; set; }
		[NotMapped]
		public string SelectedLayerIds { get; set; }
		public double radius { get; set; }
        public string connectionString { get; set; }
        public string selected_route_ids { get; set; }
    }

	public class ReportAdvanceFilter
	{
		public string searchBy { get; set; }
		public string searchType { get; set; }
		public string searchText { get; set; }
	}

	public class ExportSummaryViewKML
	{
		public string entity_title { get; set; }
		public string entity_name { get; set; }
		public string geom_type { get; set; }
		public List<dynamic> lstReportData { get; set; }
	}
	#endregion

	#region Association Report
	public class AssociationEntitiesReport
	{
		public static List<Models.ConnectionMaster> lstConnections { get; set; }
		public AssociationReportFilter objReportFilters { get; set; }
		public List<Region> lstRegion { get; set; }
		public List<Province> lstProvince { get; set; }
		public List<EntitySummaryReport> lstReportData { get; set; }
		public List<layerReportDetail> lstLayers { get; set; }
		//public List<KeyValueDropDown> lstLayerColumns { get; set; }
		//need to define for parent user
		public List<User> lstParentUsers { get; set; }
		public List<User> lstUsers { get; set; }
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		public IList<DropDownMaster> lstDurationBasedOn { get; set; }
		//public List<DropDownMaster> listOwnership { get; set; }
		public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
		[NotMapped]
		public string entityids { get; set; }
		[NotMapped]
		public string fileType { get; set; }
		public List<fileTypes> lstfiletypes { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstNetworkStatus { get; set; }
		[NotMapped]
		public List<string> lstUserModule { get; set; }
		[NotMapped]
		public List<int> selected_route_ids { get; set; }
		[NotMapped]
		public List<RouteInfo> lstRouteInfo { get; set; }
		public AssociationEntitiesReport()
		{
			lstLayers = new List<layerReportDetail>();
			//lstLayerColumns = new List<KeyValueDropDown>();
			lstProvince = new List<Province>();
			lstReportData = new List<EntitySummaryReport>();
			lstRegion = new List<Region>();
			objReportFilters = new AssociationReportFilter();
			lstUsers = new List<User>();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			// listOwnership = new List<DropDownMaster>();
			list3rdPartyVendorId = new List<KeyValueDropDown>();
			lstUserModule = new List<string>();
			lstfiletypes = new List<fileTypes>();
			lstRouteInfo = new List<RouteInfo>();
		}

	}
	[Serializable]
	public class AssociationReportFilter
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
		public List<int> SelectedProvinceId { get; set; }//int[]
		public List<int> SelectedThirdPartyVendorId { get; set; }
		public int customDate { get; set; }
		public List<int> SelectedProjectId { get; set; }
		public List<int> SelectedPlanningId { get; set; }
		public List<int> SelectedWorkOrderId { get; set; }
		public List<int> SelectedPurposeId { get; set; }
		public string durationbasedon { get; set; }
		public bool is_all_provience_assigned { get; set; }
		public double radius { get; set; }
		public string connectionString { get; set; }
		public List<int> selected_route_id { get; set; }//int[]
		public string selected_route_ids { get; set; }
		public string purpose { get; set; }
	}

	public class AssociationEntitiesSummaryView
	{
		public AssociationEntitiesSummaryViewFilter objReportFilters { get; set; }
		public List<dynamic> lstReportData { get; set; }

		public List<WebGridColumn> webColumns { get; set; }
		public List<layerReportDetail> lstLayers { get; set; }
		public List<KeyValueDropDown> lstLayerColumns { get; set; }
		public List<ReportAdvanceFilter> lstAdvanceFilters { get; set; }
		public AssociationEntitiesSummaryView()
		{
			lstLayers = new List<layerReportDetail>();
			lstLayerColumns = new List<KeyValueDropDown>();
			lstReportData = new List<dynamic>();
			webColumns = new List<WebGridColumn>();
			objReportFilters = new AssociationEntitiesSummaryViewFilter();
			lstAdvanceFilters = new List<ReportAdvanceFilter>();
		}
	}

	[Serializable]
	public class AssociationEntitiesSummaryViewFilter
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
		public List<ReportAdvanceFilter> lstAdvanceFilters { get; set; }
		public string fileType { get; set; }
		[NotMapped]
		public string SelectedLayerIds { get; set; }
		public double radius { get; set; }
		public string connectionString { get; set; }
		public string selected_route_ids { get; set; }
	}
	#endregion

	#region LMC Report
	public class ExportLMCEntitiesReport
	{
		public ExportLMCReportFilter objReportFilters { get; set; }
		public List<Region> lstRegion { get; set; }
		public List<Province> lstProvince { get; set; }
		public List<dynamic> lstReportData { get; set; }
		public List<layerDetail> lstLayers { get; set; }
		public List<KeyValueDropDown> lstLayerColumns { get; set; }
		public List<KeyValueDropDown> lstLayerDurationBasedColumns { get; set; }

		public ExportLMCEntitiesReport()
		{
			lstLayers = new List<layerDetail>();
			lstLayerColumns = new List<KeyValueDropDown>();
			lstProvince = new List<Province>();
			lstReportData = new List<dynamic>();
			lstRegion = new List<Region>();
			objReportFilters = new ExportLMCReportFilter();
			lstLayerDurationBasedColumns = new List<KeyValueDropDown>();
		}

	}
	[Serializable]
	public class ExportLMCReportFilter
	{
		public int userId { get; set; }
		public int layerId { get; set; }
		public string SearchbyColumnName { get; set; }
		public string SearchbyText { get; set; }
		public string fromDate { get; set; }
		public string toDate { get; set; }
		public string entityType { get; set; }
		public string SelectedNetworkStatues { get; set; }
		public string SelectedProvinceIds { get; set; }
		public string SelectedRegionIds { get; set; }
		public string geom { get; set; }
		public string geomType { get; set; }
		public List<string> SelectedNetworkStatus { get; set; }
		public List<int> SelectedRegionId { get; set; }
		public int[] SelectedProvinceId { get; set; }
		public int pageSize { get; set; }
		public int totalRecord { get; set; }
		public int currentPage { get; set; }
		public string sort { get; set; }
		public string sortdir { get; set; }
		public int customDate { get; set; }
		public string DurationBasedColumnName { get; set; }
		public double radius { get; set; }
		public string lmcType { get; set; }

	}
	#endregion



	#region UtilizationReport
	public class UtilizationReport
	{
		public bool isGetData { get; set; }
		public UtilizationFilter objReportFilters { get; set; }
		public List<Region> lstRegion { get; set; }
		public List<Province> lstProvince { get; set; }
		public List<UtilizationSummaryReport> lstReportData { get; set; }
		public List<layerReportDetail> lstLayers { get; set; }
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		public IList<DropDownMaster> lstDurationBasedOn { get; set; }
		public UtilizationReport()
		{
			lstProvince = new List<Province>();
			lstReportData = new List<UtilizationSummaryReport>();
			lstRegion = new List<Region>();
			objReportFilters = new UtilizationFilter();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
		}

	}
	[Serializable]
	public class UtilizationFilter
	{
		public int userId { get; set; }
		public int roleId { get; set; }
		public string SelectedLayerIds { get; set; }
		public string SelectedNetworkStatues { get; set; }
		public string SelectedProvinceIds { get; set; }
		public string SelectedRegionIds { get; set; }
		public string SelectedProjectIds { get; set; }
		public string SelectedPlanningIds { get; set; }
		public string SelectedWorkOrderIds { get; set; }
		public string SelectedPurposeIds { get; set; }
		public string geom { get; set; }
		public string geomType { get; set; }
		public List<int> SelectedLayerId { get; set; }
		public List<string> SelectedNetworkStatus { get; set; }
		public List<int> SelectedRegionId { get; set; }
		public List<int> SelectedProvinceId { get; set; }
		public List<int> SelectedProjectId { get; set; }
		public List<int> SelectedPlanningId { get; set; }
		public List<int> SelectedWorkOrderId { get; set; }
		public List<int> SelectedPurposeId { get; set; }
	}
	public class UtilizationSummaryReport
	{
		public int summary_id { get; set; }
		public string network_status { get; set; }
		public int entity_id { get; set; }
		public string entity_title { get; set; }
		public string entity_name { get; set; }
		public int region_id { get; set; }
		public string region { get; set; }
		public int province_id { get; set; }
		public string province { get; set; }
		public int low_count { get; set; }
		public int moderate_count { get; set; }
		public int high_count { get; set; }
		public int over_count { get; set; }
       

        public string utilization_text { get; set; }
	}

	public class UtilizationEntitiesSummaryView
	{
		public UtilizationEntitiesSummaryViewFilter objReportFilters { get; set; }
		public List<dynamic> lstReportData { get; set; }
		public List<layerReportDetail> lstLayers { get; set; }
		public List<KeyValueDropDown> lstLayerColumns { get; set; }
		public List<ReportAdvanceFilter> lstAdvanceFilters { get; set; }

		public UtilizationEntitiesSummaryView()
		{
			lstLayers = new List<layerReportDetail>();
			lstLayerColumns = new List<KeyValueDropDown>();
			lstReportData = new List<dynamic>();
			objReportFilters = new UtilizationEntitiesSummaryViewFilter();
			lstAdvanceFilters = new List<ReportAdvanceFilter>();
		}
	}
	[Serializable]
	public class UtilizationEntitiesSummaryViewFilter
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
		public string SelectedNetworkStatues { get; set; }
		public string SelectedProvinceIds { get; set; }
		public string SelectedRegionIds { get; set; }
		public string SelectedProjectIds { get; set; }
		public string SelectedPlanningIds { get; set; }
		public string SelectedWorkOrderIds { get; set; }
		public string SelectedPurposeIds { get; set; }
		public string geom { get; set; }
		public string geomType { get; set; }
		public string advancefilter { get; set; }
		public List<int> SelectedLayerId { get; set; }
		public string filtertype { get; set; }
		public List<Array> lstdynamicobject { get; set; }

		public List<string> SelectedNetworkStatus { get; set; }
		public List<int> SelectedRegionId { get; set; }
		public List<int> SelectedProvinceId { get; set; }
		public List<int> SelectedProjectId { get; set; }
		public List<int> SelectedPlanningId { get; set; }
		public List<int> SelectedWorkOrderId { get; set; }
		public List<int> SelectedPurposeId { get; set; }
		public List<ReportAdvanceFilter> lstAdvanceFilters { get; set; }
		public string fileType { get; set; }
		public string utilizationType { get; set; }
		public string ductutilization { get; set; }
	}

	public class UtilizationAdvFilterDetail
	{
		public string id { get; set; }
		public UtilizationAdvFilterDataDetail data { get; set; }
	}
	public class UtilizationAdvFilterDataDetail
	{
		public string network_status { get; set; }
		public string province_id { get; set; }
	}
	#endregion
	public class DataUploaderModel
	{
		[NotMapped]
		public string Plan_Id { get; set; }
		public UploadLogFilter objUploadLogFilter { get; set; }
		public List<ViewUploadSummary> lstUploadSummary { get; set; }
		public List<layerDetail> lstLayerDetails { get; set; }
		public List<string> lstUserModule { get; set; }

		public dynamic AllUsers { get; set; }
		public List<fileTypes> lstfiletypes { get; set; }

		public DataUploaderModel()
		{
			objUploadLogFilter = new UploadLogFilter();
			lstfiletypes = new List<fileTypes>();
		}

	}

	public class fileTypes
	{
		public int id { get; set; }
		public string file_type { get; set; }
		public string file_extension { get; set; }
		public string file_display_name { get; set; }

	}

	[Serializable]
	public class UploadLogFilter
	{
		public int layerId { get; set; }
		public string fromDate { get; set; }
		public string toDate { get; set; }
		public string entityName { get; set; }
		public int pageSize { get; set; }
		public int page { get; set; }
		public int totalRecords { get; set; }
		public int userId { get; set; }
	}


	//public class MicroductTemplateMaster : itemMaster
	//{
	//	[NotMapped]
	//	public EntityReference EntityReference { get; set; }
	//	[NotMapped]
	//	public override int no_of_input_port { get; set; }
	//	[NotMapped]
	//	public override int no_of_output_port { get; set; }
	//	[NotMapped]
	//	public override int no_of_port { get; set; }
	//	[NotMapped]
	//	public override string unit { get; set; }
	//	[NotMapped]
	//	public override string other { get; set; }
	//	[NotMapped]
	//	public new int type { get; set; }
	//	[NotMapped]
	//	public new int brand { get; set; }
	//	[NotMapped]
	//	public new int model { get; set; }
	//	[NotMapped]
	//	public new int construction { get; set; }
	//	[NotMapped]
	//	public new int activation { get; set; }
	//	[NotMapped]
	//	public new int accessibility { get; set; }
	//	public string no_of_ways { get; set; }
	//	[NotMapped]
	//	public IList<DropDownMaster> lstNoOfWays { get; set; }
	//}


	//public class MicroductItemMaster : MicroductTemplateMaster
	//{
	//	//ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

	//	[Key]
	//	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	//	public int id { get; set; }
	//	public int created_by { get; set; }
	//	[NotMapped]
	//	public DateTime created_on { get; set; }
	//	public int? modified_by { get; set; }
	//	public DateTime? modified_on { get; set; }
	//	[NotMapped]
	//	public PageMessage objPM { get; set; }
	//	[NotMapped]
	//	public int userId { get; set; }
	//	public MicroductItemMaster()
	//	{
	//		objPM = new PageMessage();
	//	}
	//}




	public class EntityInfo
	{
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string entity_action { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_type { get; set; }
		public string attribute_info { get; set; }
		public string geometry { get; set; }
		public int user_id { get; set; }
		public PageMessage objPM { get; set; }

		public EntityInfo()
		{
			objPM = new PageMessage();

		}
	}

	//public class MicroductMaster : MicroductTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields
	//{
	//	[Key]
	//	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	//	public int system_id { get; set; }
	//	public string network_id { get; set; }

	//	public string network_name { get; set; }
	//	public int a_system_id { get; set; }

	//	public string a_location { get; set; }
	//	public string a_entity_type { get; set; }
	//	public int b_system_id { get; set; }

	//	public string b_location { get; set; }
	//	public string b_entity_type { get; set; }
	//	[Required]
	//	public double calculated_length { get; set; }
	//	[Required]
	//	public double manual_length { get; set; }
	//	public int trench_id { get; set; }
	//	public int? sequence_id { get; set; }
	//	public string network_status { get; set; }
	//	public string status { get; set; }
	//	[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
	//	public string pin_code { get; set; }
	//	public int province_id { get; set; }
	//	public int region_id { get; set; }
	//	public int? utilization { get; set; }
	//	public int? no_of_cables { get; set; }
	//	public double offset_value { get; set; }
	//	public double inner_dimension { get; set; }
	//	public double outer_dimension { get; set; }
	//	public string microduct_type { get; set; }
	//	public string color_code { get; set; }
	//	public string acquire_from { get; set; }
	//	public string remarks { get; set; }
	//	public int created_by { get; set; }
	//	public int parent_system_id { get; set; }
	//	public string parent_network_id { get; set; }
	//	public string parent_entity_type { get; set; }
	//	[NotMapped]
	//	public DateTime created_on { get; set; }
	//	public int? modified_by { get; set; }
	//	public DateTime? modified_on { get; set; }
	//	[NotMapped]
	//	public int mappedEntId { get; set; }
	//	[NotMapped]
	//	public string region_name { get; set; }
	//	[NotMapped]
	//	public string province_name { get; set; }
	//	[NotMapped]
	//	public string geom { get; set; }
	//	[NotMapped]
	//	public PageMessage objPM { get; set; }

	//	[NotMapped]
	//	public string networkIdType { get; set; }
	//	[NotMapped]
	//	public List<NetworkDtl> lstTP { get; set; }
	//	public int? project_id { get; set; }
	//	public int? planning_id { get; set; }
	//	public int? workorder_id { get; set; }
	//	public int? purpose_id { get; set; }
	//	[NotMapped]
	//	public int pSystemId { get; set; }
	//	[NotMapped]
	//	public string pEntityType { get; set; }
	//	[NotMapped]
	//	public string pNetworkId { get; set; }
	//	[Required]
	//	public string ownership_type { get; set; }
	//	public string third_party_vendor_id { get; set; }
	//	[NotMapped]
	//	public string circuit_id { get; set; }
	//	[NotMapped]
	//	public string thirdparty_circuit_id { get; set; }
	//	public int? primary_pod_system_id { get; set; }
	//	public int? secondary_pod_system_id { get; set; }
	//	public bool is_acquire_from { get; set; }

	//	[NotMapped]
	//	public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
	//	[NotMapped]
	//	public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
	//	[NotMapped]
	//	public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
	//	[NotMapped]
	//	public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
	//	[NotMapped]
	//	public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }

	//	[NotMapped]
	//	public IList<DropDownMaster> DuctTypeIn { get; set; }
	//	[NotMapped]
	//	public IList<DropDownMaster> DuctColorIn { get; set; }

	//	[NotMapped]
	//	public entityATStatusAtachmentsList ATAcceptance { get; set; }

	//	[NotMapped]
	//	public EntityMaintainenceChargesList MaintainenceCharges { get; set; }
	//	[NotMapped]
	//	public int user_id { get; set; }
	//	[NotMapped]
	//	public bool isDirectSave { get; set; }
	//	public string status_remark { get; set; }
	//	public bool is_new_entity { get; set; }
	//	public string internal_diameter { get; set; }
	//	public string external_diameter { get; set; }
	//	public string material_type { get; set; }
	//	[NotMapped]
	//	public IList<DropDownMaster> lstInternalDiameter { get; set; }
	//	[NotMapped]
	//	public IList<DropDownMaster> lstExternalDiameter { get; set; }
	//	[NotMapped]
	//	public IList<DropDownMaster> lstMaterialType { get; set; }
	//	public string other_info { get; set; }
	//	[NotMapped]
	//	public vm_dynamic_form objDynamicControls { get; set; }
	//	public string origin_from { get; set; }
	//	public string origin_ref_id { get; set; }
	//	public string origin_ref_code { get; set; }
	//	public string origin_ref_description { get; set; }
	//	public string request_ref_id { get; set; }
	//	public string requested_by { get; set; }
	//	public string request_approved_by { get; set; }
	//	public string subarea_id { get; set; }
	//	public string area_id { get; set; }
	//	public string dsa_id { get; set; }
	//	public string csa_id { get; set; }
	//	public string gis_design_id { get; set; }
	//	[NotMapped]
	//	public string geographic_id { get; set; }
	//	[NotMapped]
	//	public string region_abbreviation { get; set; }
	//	[NotMapped]
	//	public string province_abbreviation { get; set; }
	//	[Required]
	//	public string bom_sub_category { get; set; }
	//	//public string served_by_ring { get; set; }
	//	[NotMapped]
	//	public List<DropDownMaster> lstBOMSubCategory { get; set; }
	//	[NotMapped]
	//	public List<DropDownMaster> lstServedByRing { get; set; }
	//	public string ne_id { get; set; }
	//	public string prms_id { get; set; }
	//	public string jc_id { get; set; }
	//	public string mzone_id { get; set; }
	//	[NotMapped]
	//	public string partner_name { get; set; }
 //       [NotMapped]
 //       public List<string> lstUserModule { get; set; }
 //       public MicroductMaster()
	//	{
	//		objPM = new PageMessage();
	//		lstTP = new List<NetworkDtl>();
	//		a_system_id = 0;
	//		b_system_id = 0;
	//		lstBindProjectCode = new List<ProjectCodeMaster>();
	//		lstBindPlanningCode = new List<PlanningCodeMaster>();
	//		lstBindWorkorderCode = new List<WorkorderCodeMaster>();
	//		lstBindPurposeCode = new List<PurposeCodeMaster>();
	//		EntityReference = new EntityReference();
	//		project_id = 0;
	//		planning_id = 0;
	//		workorder_id = 0;
	//		purpose_id = 0;
	//		bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
 //           lstUserModule = new List<string>();
 //       }

	//}

	public class ExportConnectionRequest
	{
		public int p_source_system_id { get; set; }
		public string p_source_type { get; set; }
		public bool p_is_source_connected { get; set; }
		public string p_connecting_entity { get; set; }
		public int p_destination_system_id { get; set; }
		public string p_destination_type { get; set; }
		public bool p_is_destination_connected { get; set; }
	}

	public class ExportConnectionReport
	{
		public string entity_type { get; set; }
		public string source_network_id { get; set; }
		public int core_port_number { get; set; }
		public string via_entity_type { get; set; }
		public string via_network_id { get; set; }
		public int via_port_no { get; set; }
		public string destination_network_id { get; set; }
		public string destination_entity_type { get; set; }
		public int destination_port_no { get; set; }

	}

	public class Files
	{
		public string TableName { get; set; }
		public byte[] Bytes { get; set; }
	}
	public class PODDetail
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string network_status { get; set; }
		public string distance { get; set; }
		public string pod_type { get; set; }
		public string pod_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int associated_system_id { get; set; }
		public string associated_System_Type { get; set; }
	}
	public class PODAssociation
	{
		public int system_id { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int associated_system_id { get; set; }
		public string associated_entity_Type { get; set; }
		public int? primary_pod_system_id { get; set; }
		public int? secondary_pod_system_id { get; set; }
		public List<PODDetail> lstPODAssociation { get; set; }
		public string geom { get; set; }

		public string entity_sub_type { get; set; }
		public PODAssociation()
		{
			lstPODAssociation = new List<PODDetail>();
		}
	}

	//cabinet shazia 
	public class CabinetMaster : CabinetTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string cabinet_name { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }
		public string acquire_from { get; set; }
		public bool is_acquire_from { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
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
		public bool isDirectSave { get; set; }
		[NotMapped]
		public int user_id { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		public bool is_new_entity { get; set; }
		public int? primary_pod_system_id { get; set; }
		public int? secondary_pod_system_id { get; set; }
		[Required]
		[Range(0.1, 20, ErrorMessage = "Cabinet length cannot be greater than 20.")]
		public double? length { get; set; }
		[Range(0.1, 20, ErrorMessage = "Cabinet width cannot be greater than 20.")]
		[Required]
		public double? width { get; set; }
		[Range(0.1, 20, ErrorMessage = "Cabinet height cannot be greater than 20.")]
		[Required]
		public double? height { get; set; }
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
        public CabinetMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			objIspEntityMap = new IspEntityMapping();
			parent_entity_type = string.Empty;
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();
			planning_id = 0;
			purpose_id = 0;
			workorder_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
	}

	//cabinet shazia end 
	//vault shazia 
	public class VaultMaster : VaultTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string vault_name { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }

		public int no_of_entry_exit_points { get; set; }
		public string acquire_from { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
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
		public bool isDirectSave { get; set; }
		[NotMapped]
		public int user_id { get; set; }

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		public bool is_new_entity { get; set; }
		public bool is_acquire_from { get; set; }
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
		public VaultMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			objIspEntityMap = new IspEntityMapping();
			parent_entity_type = string.Empty;
			EntityReference = new EntityReference();
			lstLoopMangment = new List<NELoopDetails>();
			planning_id = 0;
			purpose_id = 0;
			workorder_id = 0;
			project_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
			lstUserModule = new List<string>();
		}
	}

	//Vault shazia end

	public class NECableDetails
	{
		public string network_id { set; get; }
		public int cable_system_id { set; get; }
		public string created_on { set; get; }
		public string cable_id { set; get; }
		public string cable_name { get; set; }
		public string cable_type { get; set; }
		public float? cable_calculated_length { get; set; }
		public int total_loop_length { get; set; }
		public int total_loop_count { get; set; }

		public float? available_cable_length { get; set; }


	}

	//Handhole entity by ANTRA
	public class HandholeMaster : HandholeTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string handhole_name { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		public string address { get; set; }
		public string remarks { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		public bool is_virtual { get; set; }
		public bool is_buried { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }


		public string status { get; set; }
		public string network_status { get; set; }
		public string construction_type { get; set; }
		public string acquire_from { get; set; }
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
		[Required]
		public string ownership_type { get; set; }
		public string third_party_vendor_id { get; set; }
		[NotMapped]
		public string circuit_id { get; set; }
		[NotMapped]
		public string thirdparty_circuit_id { get; set; }
		public int? primary_pod_system_id { get; set; }
		public int? secondary_pod_system_id { get; set; }
		public string status_remark { get; set; }
		public bool is_acquire_from { get; set; }
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
		public List<NELoopDetails> lstLoopMangment { get; set; }
		[NotMapped]
		public IList<DropDownMaster> listConstructionType { get; set; }
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

		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		public bool is_new_entity { get; set; }
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
		public string st_x { get; set; }
		public string st_y { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
		public string partner_name { get; set; }
        [NotMapped]
        public vm_dynamic_form objDynamicControls { get; set; }
        public string other_info { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public HandholeMaster()

		{
			objPM = new PageMessage();
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
	public class PatchPanelMaster : PatchPanelTemplateMaster, IProjectSpecification, IOwnershipInfo, IAcquireFrom, IReference, IGeographicDetails, IAdditionalFields, ICustomCoordinate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public string network_id { get; set; }

		public string patchpanel_name { get; set; }
		[Required]
		public string patchpanel_type { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public int province_id { get; set; }
		public int region_id { get; set; }
		[StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
		public string pincode { get; set; }
		public string acquire_from { get; set; }
		[NotMapped]
		public string subcode { get; set; }

		public string address { get; set; }
		public string remarks { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }

		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }

		public string status { get; set; }
		public string network_status { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
		public int structure_id { get; set; }
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
		public int templateId { get; set; }
		[NotMapped]
		public IspEntityMapping objIspEntityMap { get; set; }
		[NotMapped]
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string pNetworkId { get; set; }
		public string utilization { get; set; }
		[NotMapped]
		public string display_name { get; set; }
		[NotMapped]
		public string DisplayNameWithPort
		{
			get
			{
				return string.Format("{0}-({1})-({2})", display_name, no_of_port, network_status);
			}
		}
		[NotMapped]
		public bool is_isp_mapped { get; set; }
		[NotMapped]
		public bool isDirectSave { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		public List<NELoopDetails> lstLoopMangment { get; set; }
		public string status_remark { get; set; }
		public bool is_new_entity { get; set; }
		[NotMapped]
		public int split_cable_system_id { get; set; }
		public bool is_acquire_from { get; set; }
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
		public PatchPanelMaster()
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
			project_id = 0;
			workorder_id = 0;
			bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
			lstUserModule = new List<string>();
		}
	}


	public interface IAcquireFrom
	{
		string acquire_from { get; set; }
		bool is_acquire_from { get; set; }

	}
	public interface ICustomCoordinate
	{
		string st_x { get; set; }
		string st_y { get; set; }

	}
	public class GeoTaggedImagesFilter
	{
		public int id { get; set; }
		public string searchText { get; set; }
		public string searchByText { get; set; }
		public int pageSize { get; set; }
		public int totalRecord { get; set; }
		public int currentPage { get; set; }
		public string sort { get; set; }
		public string sortdir { get; set; }
		public int userId { get; set; }
		public string orderBy { get; set; }
		public List<dynamic> lstGeoTaggedImghistory { get; set; }
		public string image_uploaded_by { get; set; }
		public DateTime image_uploaded_on { get; set; }
		public List<int> SelectedRegionId { get; set; }
		public List<int> SelectedProvinceId { get; set; }
		public string SelectedProvinceIds { get; set; }
		public string SelectedRegionIds { get; set; }
		public string thumbimage_location { get; set; }
		public string file_name { get; set; }
		public string file_location { get; set; }
		public string org_file_name { get; set; }
		public string Thumbgeotaggedpath { get; set; }
		public string Org_geotaggedpath { get; set; }
		public Nullable<double> latitude { get; set; }
		public Nullable<double> longitude { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		public GeoTaggedImagesFilter()
		{
			lstGeoTaggedImghistory = new List<dynamic>();
			objPM = new PageMessage();
		}

	}

	#region Reference Tab By Antra
	public interface IReference
	{
		string origin_from { get; set; }
		string origin_ref_id { get; set; }
		string origin_ref_code { get; set; }
		string origin_ref_description { get; set; }
		string request_ref_id { get; set; }
		string requested_by { get; set; }
		string request_approved_by { get; set; }
	}
	#endregion

	#region Geographic Details By ANTRA
	public interface IGeographicDetails
	{
		string subarea_id { get; set; }
		string area_id { get; set; }
		string dsa_id { get; set; }
		string csa_id { get; set; }
		string province_name { get; set; }
		string region_name { get; set; }
		string geographic_id { get; set; }
		string gis_design_id { get; set; }
		string region_abbreviation { get; set; }
		string province_abbreviation { get; set; }
		string ne_id { get; set; }
		string prms_id { get; set; }
		string jc_id { get; set; }
		string mzone_id { get; set; }
		string partner_name { get; set; }
		string entityType { get; set; }

	}
	#endregion


	public interface IAdditionalFields
	{
		string bom_sub_category { get; set; }
		//string served_by_ring { get; set; }
		[NotMapped]
		List<DropDownMaster> lstBOMSubCategory { get; set; }
		[NotMapped]
		List<DropDownMaster> lstServedByRing { get; set; }

	}
	#region process splitter
	public class ProcessSummary
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int process_id { get; set; }
		public int ticket_id { get; set; }
		public DateTime process_start_time { get; set; }
		public DateTime process_end_time { get; set; }
		public string file_name { get; set; }
		public string file_extension { get; set; }
		public string stataus { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public string remarks { get; set; }
		public int? enitity_id { get; set; }
		public string entity_type { get; set; }
		public string csa_id { get; set; }
		public bool import_status { get; set; }
		public string entity_design_id { get; set; }
		public int? total_entity { get; set; }
		[NotMapped]
		public int userId { get; set; }
		[NotMapped]
		public string processed_entity_title { get; set; }
		public string nas_status { get; set; }
		public string file_version { get; set; }
		//public int ring_no { get; set; }
	}

	public class ProcessSummaryFilter : CommonGridAttributes
	{
		public ProcessSummary objProcessSummary { get; set; }
		[NotMapped]
		public List<dynamic> lstProcessedXMLDetails { get; set; }
		[NotMapped]
		public string sortdir { get; set; }
		[NotMapped]
		public string data_uploaded_by { get; set; }
		[NotMapped]
		public DateTime data_uploaded_on { get; set; }
		[NotMapped]
		public string searchByText { get; set; }
		[NotMapped]
		public string ps_port { get; set; }
		[NotMapped]
		public int systemId { get; set; }
		[NotMapped]
		public string entityType { get; set; }
		[NotMapped]
		public List<string> lstUserModulePermission { get; set; }
		public ProcessSummaryFilter()
		{
			objProcessSummary = new ProcessSummary();
			lstProcessedXMLDetails = new List<dynamic>();
		}
	}


    public class BoundaryPushFilter : TemplateForDropDownBoundaryPush
    {
        public List<BoundaryPushToGis> pushboundaryDetailList { get; set; }
        public ViewBoundaryPush viewBoundaryPush { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public int p_system_id { get; set; }
		public string p_entity_type { get; set; }
        public Nullable<int> gis_object_id { get; set; }
        public int system_id { get; set; }
        public string process_message { get; set; }
        public string entity_type { get; set; }
        public int user_id { get; set; }
        public string VersionName { get; set; }
        public Nullable<int> user_name { get; set; }
        public string gis_design_id { get; set; }
        public string request_time { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public bool bt_lock { get; set; }
        public List<string> lstUserModulePermission { get; set; }
        public string transaction_id { get; set; }
        public string process_id { get; set; }
        public BoundaryPushFilter()
        {
            viewBoundaryPush = new ViewBoundaryPush();
            viewBoundaryPush.searchText = string.Empty;
            lstUserModule = new List<string>();
			
        }
    }



    public class TemplateForDropDownBoundaryPush
    {
        [NotMapped]
        public List<KeyValueDropDown> lstBoundaryBindSearchBy { get; set; }
    }
    #endregion

    public class ViewBoundaryPush
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
    }



	public class Pushgislogrequest
	{
		public int id { get; set; }
		public int system_id { get; set; }
		public string entity_type { get; set; }
        public string status { get; set; }
        public string process_message { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }
        public bool bt_lock { get; set; }
		public string process_id { get; set; }
}

    public class PushgisProcessSummary
    {
        
        public int system_id { get; set; }
        public int sub_area_system_id { get; set; }
        public string sub_area_name { get; set; }
        public string process_status { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime modified_on { get; set; }
        public DateTime process_start_time { get; set; }
        public DateTime process_end_time { get; set; }
        public string remarks { get; set; }
        public string approval_status { get; set; }
    }


    public class ProcessedEntities
	{
		public int total_entities { get; set; }
		public string processed_entities { get; set; }

	}
	public class SeconarySplitterListFilter : CommonGridAttributes
	{
		[NotMapped]
		public List<dynamic> lstSeconarySplitterDetails { get; set; }
		[NotMapped]
		public string sortdir { get; set; }
		[NotMapped]
		public string data_uploaded_by { get; set; }
		[NotMapped]
		public DateTime data_uploaded_on { get; set; }
		[NotMapped]
		public string searchByText { get; set; }
		[NotMapped]
		public int systemId { get; set; }
		[NotMapped]
		public string entityType { get; set; }

		[NotMapped]
		public string source_ref_id { get; set; }
		[NotMapped]
		public string action_name { get; set; }
		[NotMapped]
		public List<SelectListItem> column_filter { get; set; }
		public SeconarySplitterListFilter()
		{
			lstSeconarySplitterDetails = new List<dynamic>();
		}
	}

	public class EntitySummary
	{
		public string CsaDesignId { get; set; }
		public bool status { get; set; }
		public string message { get; set; }
		public int? ProcessId { get; set; }
		public string pEntityType { get; set; }
		public string pSystemId { get; set; }
		public string logs { get; set; }
		public List<log> listLog { get; set; }
		public string gis_design_id { get; set; }
		public int? entity_id { get; set; }
		public int? user_id { get; set; }
        public int? path { get; set; }

    }
	public class SecondarySpilterSummary
	{
		//public string CsaDesignId { get; set; }
		public bool status { get; set; }
		public string message { get; set; }
		//public int? ProcessId { get; set; }
		//public string pEntityType { get; set; }
		//public string pSystemId { get; set; }
		//public string logs { get; set; }
		//public List<log> listLog { get; set; }
		public string gis_design_id { get; set; }
		//public int? entity_id { get; set; }
		public int? user_id { get; set; }
		public int? path { get; set; }
		public string error_code { get; set; }
		public int? request_log_id { get; set; }
		public int ? s2_system_id{ get; set; }

	}
	public class SecondarySpilterLogSummary
	{
		public string design_id { get; set; }
		public string network_id { get; set; }
		public string entity_type { get; set; }
		public string err_message { get; set; }
		public string network_status { get; set; }
		public string entity_title { get; set; }

	}
	public class log
	{
		public int entity_id { get; set; }
		public string entity_type { get; set; }
		public string gis_design_id { get; set; }
		public bool is_processed { get; set; }
		public string entity_title { get; set; }
		public string network_status { get; set; }
	}
	
	public class ApiReturnData
	{
		public string transaction_id { get; set; }
		public string InDateTime { get; set; }
		public string OutDateTime { get; set; }
		public string latency { get; set; }
		public string CreatedOn { get; set; }

	}

	public class GISInputDetails
	{
		public int systemId { get; set; }
		public string entityType { get; set; }
		public string VersionName { get; set; }
        public int user_id { get; set; }
        public string gis_design_id { get; set; }
    }





    public class BoundaryPushToGis
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public string display_name { get; set; }
        public int created_by { get; set; }
        public DateTime? created_on { get; set; }
		public Nullable<int> user_id { get; set; }
        public string user_name { get; set; }
        public string gis_design_id { get; set; }
        public Nullable<int> gis_object_id { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public Nullable<DateTime> request_time { get; set; }
        public int totalRecords { get; set; }
        public int page_count { get; set; }
        public int S_No { get; set; }
        public string VersionName { get; set; }
        public DateTime? modified_on { get; set; }		
		public string entity_title { get; set; }
		public string process_message { get; set; }
    }
    public class GISEntityRequest
	{
		public Ring geometry { get; set; }
		public GISAttributes attributes { get; set; }
    }

	public class GISSpatialReference
	{
		public int wkid { get; set; }
		public int latestWkid { get; set; }
        public GISSpatialReference()
		{
			wkid = 4326;
			latestWkid = 4326;
        }
	}
	public class GISAttributes
	{
		public string status { get; set; }
		public int OBJECTID { get; set; }
		public string CITY { get; set; }
		public int? CONSUMER_COUNT { get; set; }
		public string STATE { get; set; }
		public string PRMS_ID { get; set; }
		public string VENDOR_ID { get; set; }
		public string PAN_NO { get; set; }
		public string JPF_ID { get; set; }
		public string PSA_ID { get; set; }
		public string JC_SAP_ID { get; set; }
		public string NETWORK_TYPE { get; set; }
		public string R4G_STATE_CODE { get; set; }
		public string R4G_CITY_CODE { get; set; }
		public string FSA_ID { get; set; }
		public string DESIGN { get; set; }
		public string RFC_DATE { get; set; }
		public string R4GSTATECODE { get; set; }
		public string R4GCITYCODE { get; set; }
		public string DATASOURCE { get; set; }
		public int? FEEDER_NO { get; set; }
		public string DSA_ID { get; set; }
		public string CSA_ID { get; set; }
		public string JPF_NAME { get; set; }
		public string message { get; set; }
        public string WARD_NUMBER { get; set; }
        public string CIRCLE { get; set; }
        public string OLT_FACILITY_ID { get; set; }
        public string FACILITY_STATUS { get; set; }
        public string SAP_ID { get; set; }
        public string CREATED_USER { get; set; }
        public long CREATED_DATE { get; set; }
        public string LAST_EDITED_USER { get; set; }
        public long LAST_EDITED_DATE { get; set; }
        public string PROJECT_ID { get; set; }
        public string FSA_WBS_ID { get; set; }
        public string FSA_WBS_STATUS { get; set; }
        public string FSA_STATUS { get; set; }
        public string GLOBALID { get; set; }
        public string SERVICE_WBS_ID { get; set; }
        public string FSANAME { get; set; }
        public string POST_DATE { get; set; }
        public string ENODEB_SAP_ID { get; set; }
        public string COLO_SAP_ID { get; set; }
        public string MAINTENANCEZONECODE { get; set; }
        public string REMARK { get; set; }
        public string POP_ID { get; set; }
        public string CAPEX_APPROVAL_DATE { get; set; }
        public string JIOPOINT_NAME { get; set; }
        public string JIOPOINT_SAPID { get; set; }
        public GISAttributes()
		{
			status = "Test";
		}
	}
	public class Ring
	{
		public ArrayList rings { get; set; }
		public GISSpatialReference spatialReference { get; set; }
	}
	public class Geometery
	{
		public double longitude { get; set; }
		public double latitude { get; set; }
		public int objectId { get; set; }
        public string gisdesignId { get; set; }
    }

	public class GISOutput
	{
		public int objectId { get; set; }
		public bool success { get; set; }
		public string globalId { get; set; }
	}
	public class GISAPIResponse
	{
		public List<GISOutput> addResults { get; set; }
		public List<GISOutput> updateResults { get; set; }
		public List<GISOutput> deleteResults { get; set; }
		public string InDateTime { get; set; }
		public string OutDateTime { get; set; }
		public string Transactionid { get; set; }
		public string Latency { get; set; }
		public GISAPIError error { get; set; }
    }
	public class GISAPIError
	{
		public int code { get; set; }
		public string message { get; set; }
		public List<string> details { get; set; }
	}

    public class GISFeature
    {
        public GISAttributes attributes { get; set; }
        public GISRing geometry { get; set; }
    }
    public class GISRing
    {
        public ArrayList rings { get; set; }
    }

    public class GISFields
    {
        public string name { get; set; }
        public string alias { get; set; }
        public string type { get; set; }
        public int? length { get; set; }
    }

    public class FetchGISData
    {
        public string objectIdFieldName { get; set; }
        public string globalIdFieldName { get; set; }
        public string geometryType { get; set; }
        public GISSpatialReference spatialReference { get; set; }
        public List<GISFields> fields { get; set; }
        public List<GISFeature> features { get; set; }
    }
    public class NASStatusIn
	{
		public string csa_id { get; set; }
		public string inserted_by { get; set; }
		public string status { get; set; }
		public string system { get; set; }
		public string xm_file_name { get; set; }
		public string version_name { get; set; }
		public string CSAID { get; set; }
		public byte[] FormFile { get; set; }
        public int user_id { get; set; }
        public string gis_design_id { get; set; }
        public int systemId { get; set; }
        public string entityType { get; set; }
        public string process_id { get; set; }
    }
	public class NASStatusOut
	{
		public string status { get; set; }
		public string status_code { get; set; }
		public string status_message { get; set; }
		public string InDateTime { get; set; }
		public string OutDateTime { get; set; }
		public string Transactionid { get; set; }
		public string Latency { get; set; }
		public string message { get; set; }
        public int user_id { get; set; }
        public string gis_design_id { get; set; }
        public int systemId { get; set; }
        public string entityType { get; set; }
    }
	public class AutoCodification
	{
		public bool status { get; set; }
		public string message { get; set; }
		public string logs { get; set; }
		public List<CodificationLog> listLog { get; set; }
	}
	public class CodificationLog
	{
		public string system_id { get; set; }
		public string entity_type { get; set; }
		public string network_id { get; set; }
		public string gis_design_id { get; set; }
		public string status { get; set; }
		public string error_message { get; set; }
	}

	public class CreateVersionIn
	{
		public string UserName { get; set; }
		public string VersionName { get; set; }
        public int user_id { get; set; }
        public string gis_design_id { get; set; }
        public int systemId { get; set; }
        public string entityType { get; set; }
        public string transaction_id { get; set; }
        public string process_id { get; set; }

        //updated by pk


        //public int system_id { get; set; }
        //public string entity_type { get; set; }
        //public string display_name { get; set; }
        //public int created_by { get; set; }
        //public DateTime? created_on { get; set; }
        //public string user_name { get; set; }
        //public Nullable<int> gis_object_id { get; set; }
        //public string status { get; set; }
        //public string message { get; set; }
        //public Nullable<DateTime> request_time { get; set; }
        //public int totalRecords { get; set; }
        //public int page_count { get; set; }
        //public int S_No { get; set; }


        //public DateTime? modified_on { get; set; }
    }
	public class CreateVersionOut
	{

		public string UserName { get; set; }
		public string VersionName { get; set; }
		public string Message { get; set; }
		public string Status { get; set; }

	}
	public class PostVersionIn
	{
		public string VersionName { get; set; }
		public string SchemaName { get; set; }
		public string RequestedBy { get; set; }
		public PostVersionIn()
		{
			SchemaName = "R4G_FTTX";
		}
        public int user_id { get; set; }
        public string gis_design_id { get; set; }
        public int systemId { get; set; }
        public string entityType { get; set; }
		public int objectId { get; set; }
        public string transaction_id { get; set; }
		public string process_id { get; set; }

	}
	public class PostVersionOut
	{
		public string UserName { get; set; }
		public string VersionName { get; set; }
		public string Message { get; set; }
		public string Status { get; set; }
	}
	//start ycode

	public class XMLBuilderDashboardFilter
	{
		public string searchText { get; set; }
		public int pageno { get; set; }
		public int currentPage { get; set; }
		public int pagerecord { get; set; }
		public string sortcolname { get; set; }
		public string sorttype { get; set; }
		public int totalrecords { get; set; }
		public List<XMLBuilderDashboardMaster> lstXML { get; set; }
		public List<dynamic> lstXMLBuilderDashboardEntityHistory { get; set; }

		public XMLBuilderDashboardFilter()
		{
			lstXML = new List<XMLBuilderDashboardMaster>();
			lstXMLBuilderDashboardEntityHistory = new List<dynamic>();
		}
	}
	public class XMLBuilderDashboardMaster
	{
		public int totalrecords { get; set; }
		public string file_name { get; set; }
		public string nas_status { get; set; }
		public string created_by { get; set; }
		public DateTime? created_on { get; set; }
		public string reset_by { get; set; }
		public DateTime? reset_on { get; set; }
	}
    //end ycode

	public class PUSHLOGS
	{
		public string WHERE { get; set; }
		public string outFields { get; set; }
        public string f { get; set; }
		public string objectId { get; set; }
        public string gdbVersion { get; set; }
        public string applyEdits { get; set; }
		public dynamic fields { get; set; }

        public PUSHLOGS()
		{
			f = "json";
			outFields = "*";


        }

    }
	public class SlackMaster : IReference, IGeographicDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { set; get; }
        public double? slack_length { set; get; }
        public int associated_system_id { get; set; }
        public string associated_network_id { get; set; }
        public string associated_entity_type { get; set; }
        public int duct_system_id { set; get; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }
        public int? created_by { set; get; }
        public DateTime? created_on { set; get; }
        public int? modified_by { set; get; }
        public DateTime? modified_on { set; get; }
        [NotMapped]
        public string duct_id { set; get; }
        public string network_status { get; set; }
        [NotMapped]
        public string duct_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        [NotMapped]
        public string associated_System_Type { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int structure_id { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public List<SlackMaster> lstSlack { get; set; }
        public string status_remark { get; set; }
        [NotMapped]
        public double? duct_calculated_length { get; set; }
        [NotMapped]
        public int? total_slack_count { get; set; }
        [NotMapped]
        public double? available_calculated_length { get; set; }
        [NotMapped]
        public int? total_slack_length { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        public bool is_new_entity { get; set; }
        [NotMapped]
        public List<FormInputSettings> formInputSettings { get; set; }
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
        public SlackMaster()
        {
            objPM = new PageMessage();
            structure_id = 0;
            lstSlack = new List<SlackMaster>();
            entityType = "Slack";
        }
    }
    public class NEDuctDetails
    {
        public string network_id { set; get; }
        public int duct_system_id { set; get; }
        public string created_on { set; get; }
        public string duct_id { set; get; }
        public string duct_name { get; set; }
        public string duct_type { get; set; }
        public float? duct_calculated_length { get; set; }
        public int total_slack_length { get; set; }
        public int total_slack_count { get; set; }
        public float? available_duct_length { get; set; }
    }
    public class CDBAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? cable_id { get; set; }
        public double? execution { get; set; }
        public double? row_availablity { get; set; }
        public double? iru_given_airtel { get; set; }
        public double? iru_given_jio { get; set; }
        public double? iru_given_ttsl_or_ttml { get; set; }
        public double? iru_given_tcl { get; set; }
        public double? iru_given_others { get; set; }
        public double? distance { get; set; }
        public DateTime? row_valid_or_exp { get; set; }
        public int? fiber_pairs_laid { get; set; }
        public int? total_used_pair { get; set; }
        public int? fiber_pairs_used_by_vil { get; set; }
        public int? fiber_pairs_given_to_airtel { get; set; }
        public int? fiber_pairs_given_to_others { get; set; }
        public int? fiber_pairs_free { get; set; }
        public int? faulty_fiber_pairs { get; set; }
        public double? start_latitude { get; set; }
        public double? start_longitude { get; set; }
        public double? end_latitude { get; set; }
        public double? end_longitude { get; set; }
        public string count_non_vil_tenancies_on_route { get; set; }
        public DateTime? route_lit_up_date { get; set; }
        public double? aerial_km { get; set; }
        public double? avg_loss_per_km { get; set; }
        public double? avg_last_six_months_fiber_cut { get; set; }
        public double? row { get; set; }
        public double? material { get; set; }
        public string fiber_type { get; set; }
        public string major_route_name { get; set; }
        public string route_id { get; set; }
        public string section_name { get; set; }
        public string section_id { get; set; }
        public string route_category { get; set; }
        public string network_category { get; set; }
        public string remarks { get; set; }
        public string cable_owner { get; set; }
        public string route_type { get; set; }
        public string operator_type { get; set; }
        public string circle_name { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstOperator { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstRoute { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstFiber { get; set; }
    }
}


