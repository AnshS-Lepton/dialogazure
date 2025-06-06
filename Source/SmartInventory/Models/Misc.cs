using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Models.Admin;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Models
{
	public class PageMessage
	{
		public string status { get; set; }
		public string message { get; set; }
		public bool isNewEntity { get; set; }
		public int systemId { get; set; }
		public string entityType { get; set; }
		public string NetworkId { get; set; }
		public int structureId { get; set; }
		public int shaftId { get; set; }
		public int floorId { get; set; }
		public int pSystemId { get; set; }
		public string pEntityType { get; set; }
		public string logData { get; set; }
	}
	public class JsonResponse<T> where T : class
	{
		public JsonResponse()
		{
			status = string.Empty;
			message = string.Empty;
		}
		public string status { get; set; }
		public string message { get; set; }
		public T result { get; set; }
	}
	public class JsonPlannerResponse<T> where T : class
	{
		public JsonPlannerResponse()
		{
			status = string.Empty;
			error_message = string.Empty;
		}
		public string status { get; set; }
		public string error_message { get; set; }
		public T results { get; set; }
	}
	public class ChildElementDetail
	{
		public string System_Id { get; set; }
		public string Network_Id { get; set; }
		public string Entity_Type { get; set; }
		public string geom_Type { get; set; }
		public string display_name { get; set; }
	}
	public class ImpactDetail
	{
		public bool moveConnected { get; set; }
		public List<ChildElementDetail> ChildElements { get; set; }
		public ImpactDetail()
		{
			ChildElements = new List<ChildElementDetail>();
		}
	}
	public class ImpactDetailIn
	{
		public int systemId { get; set; }
		public string entityType { get; set; }
		public string geomType { get; set; }
		public string impactType { get; set; }
		public int user_id { get; set; }
	}
	public class NetworkDtl
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string network_name { get; set; }
		public string node_type { get; set; } // used for ISP/ Vertical cables only..
		public string actualLatLng { get; set; }
		public string mode { get; set; }
		public string entity_name { get; set; }
		//public string mode { get; set; }

	}
	public class NetworkCodeDetail
	{
		[Key]
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string parent_geom_type { get; set; }
		public string network_code { get; set; }
		public int sequence_id { get; set; }
		public string err_msg { get; set; }

	}
	public class EntityCodeDetail
	{
		public int o_p_system_id { get; set; }
		public string o_p_network_id { get; set; }
		public string o_p_entity_type { get; set; }
		public int o_sequence_id { get; set; }
		public bool status { get; set; }
		public string message { get; set; }

	}
	public class ISPNetworkCodeDetail
	{
		public string network_code { get; set; }
		public int sequence_id { get; set; }
		public string err_msg { get; set; }
	}
	public class NetworkCodeIn
	{
		public int parent_sysId { get; set; }
		public string parent_eType { get; set; }
		public string eType { get; set; }
		public string gType { get; set; }
		public string eGeom { get; set; }
		public NetworkCodeIn()
		{
			parent_sysId = 0;
			parent_eType = "";
		}
	}
	public class ISPNetworkCodeIn
	{
		public int parent_sysId { get; set; }
		public string parent_eType { get; set; }
		public string eType { get; set; }
		public string parent_networkId { get; set; }
		public int structureId { get; set; }
		public ISPNetworkCodeIn()
		{
			parent_sysId = 0;
			parent_eType = "";
			parent_networkId = "";
		}
	}
	public class LibIn
	{
		public int systemId { get; set; }
		public string geom { get; set; }
		public string networkIdType { get; set; }
		public bool isDirectSave { get; set; }
		public LibIn()
		{
			geom = "";
		}
	}
	public class RegionProvinceDetail
	{
		public int region_id { get; set; }
		public string region_name { get; set; }
		public string region_abbreviation { get; set; }
		public int province_id { get; set; }
		public string province_name { get; set; }
		public string province_abbreviation { get; set; }
	}
	public class startendpoint
	{
		public string a_latitude { get; set; }
		public string b_longitude { get; set; }
		public string tstart_region { get; set; }
		public string tEnd_region { get; set; }
		public string tstrat_province { get; set; }
		public string tEnd_province { get; set; }
		public string province_abbr { get; set; }
	}
	public class TerminationPointDtl
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string entity_name { get; set; }
		public string latlng { get; set; }
		public DateTime created_on { get; set; }
		public string display_name { get; set; }

	}
	public class PropertyComparer<T> : IEqualityComparer<T>
	{
		private PropertyInfo _PropertyInfo;

		/// <summary>
		/// Creates a new instance of PropertyComparer.
		/// </summary>
		/// <param name="propertyName">The name of the property on type T 
		/// to perform the comparison on.</param>
		public PropertyComparer(string propertyName)
		{
			//store a reference to the property info object for use during the comparison
			_PropertyInfo = typeof(T).GetProperty(propertyName,
		BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
			if (_PropertyInfo == null)
			{
				throw new ArgumentException(string.Format("{0} is not a property of type {1}.", propertyName, typeof(T)));
			}
		}

		#region IEqualityComparer<T> Members

		public bool Equals(T x, T y)
		{
			//get the current value of the comparison property of x and of y
			object xValue = _PropertyInfo.GetValue(x, null);
			object yValue = _PropertyInfo.GetValue(y, null);

			//if the xValue is null then we consider them equal if and only if yValue is null
			if (xValue == null)
				return yValue == null;

			//use the default comparer for whatever type the comparison property is.
			return xValue.Equals(yValue);
		}

		public int GetHashCode(T obj)
		{
			//get the value of the comparison property out of obj
			object propertyValue = _PropertyInfo.GetValue(obj, null);

			if (propertyValue == null)
				return 0;

			else
				return propertyValue.GetHashCode();
		}

		#endregion
	}
	public class LineNetworkCodeDetail
	{

		public string network_code { get; set; }
		public int sequence_id { get; set; }

	}
	public class KeyValueDropDown
	{
		public string key { get; set; }
		public string value { get; set; }
		public string ddtype { get; set; }
		public string type { get; set; }

	}
	public class TopologyGetSites
	{
		public int siteid { get; set; }
        public int ringid { get; set; }
        public string sitename { get; set; }
		public decimal sitedistance { get; set; }
        public bool is_agg_site { get; set; }

    }

    public class Topologysegment
    {
        public int id { get; set; }
        public string segment_code { get; set; }
        public int region_id { get; set; }
        public string agg1_site_id { get; set; }
        public string agg2_site_id { get; set; }
        public int agg1_system_id { get; set; }
        public int agg2_system_id { get; set; }


    }

    public class TopologySegmentCables
	{

		public string cable_name { get; set; }
		public string network_id { get; set; }

	}

	public class IvcKeyValueDropDown
	{
		public int key { get; set; }
		public string value { get; set; }


	}
	public class EntityLstCount
	{
		public string entity_type { get; set; }
		public string network_status { get; set; }
		public int entity_count { get; set; }
		public string system_id { get; set; }
		public int totalRecords { get; set; }
		public bool is_networktype_required { get; set; }
		public bool is_project_spec_allowed { get; set; }
		public string layer_name { get; set; }
		public bool is_pod_association_allowed { get; set; }
		public string isheader { get; set; }
		public string entity_sub_type { get; set; }
		public int entity_have_child { get; set; }
		public int p_roleid { get; set; }
		public string p_parentusers { get; set; }
		public string p_userids { get; set; }
	}
	public class BulkProjSpecific
	{
		public string geom { get; set; }
		public string selection_type { get; set; }
		public double buff_Radius { get; set; }
		public int user_id { get; set; }
		public string ntk_type { get; set; }
		public string entity_select { get; set; }
		public int project_id { get; set; }
		public int planning_id { get; set; }
		public int purpose_id { get; set; }
		public int workorder_id { get; set; }
		public string entity_sub_type { get; set; }
	}

	public class BulkDelete
	{
		public string geom { get; set; }
		public string selection_type { get; set; }
		public double buff_Radius { get; set; }
		public int user_id { get; set; }
		public string ntk_type { get; set; }
		public string entity_select { get; set; }
		public string entity_sub_type { get; set; }
		public int system_id { get; set; }
		public int rootid { get; set; }
		public string selectedUsers { get; set; }
	}
	public class ProjectSpecificView : IProjectSpecification, IOwnershipInfo
	{
		public string network_status { get; set; }
		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
		public string bld_rfs { get; set; }
		public string cable_type { get; set; }
		public string cable_category { get; set; }
		public string splitter_type { get; set; }
		public string fault_type { get; set; }
		public string ownership { get; set; }

		public string sector_type { get; set; }
		public string frequency { get; set; }
		public int primary_pop_system_id { get; set; }
		public int secondary_pop_system_id { get; set; }
		public Boolean is_middleware { get; set; }
		[NotMapped]
		public Boolean is_middlewareInLayer { get; set; }
		[NotMapped]
		public List<KeyValueDropDown> lstPODAssociation { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstSectorType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstFrequencyType { get; set; }

		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		[NotMapped]
		public string childModel { get; set; }
		//[NotMapped]
		//public IList<DropDownMaster> lstBuildingRFS { get; set; }
		[NotMapped]
		public IList<CheckboxMaster> lstBuildingRFS { get; set; }
		[NotMapped]
		public IList<DropDownMaster> listcableType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> listcableCategory { get; set; }

		[NotMapped]
		public IList<DropDownMaster> listFaultType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> listsplittertype { get; set; }
		[NotMapped]
		public string entityType { get; set; }
		public string ownership_type { get; set; }
		public string third_party_vendor_id { get; set; }
		public string own_vendor_id { get; set; }

		[NotMapped]
		public string circuit_id { get; set; }
		[NotMapped]
		public string thirdparty_circuit_id { get; set; }
		[NotMapped]
		public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
		[NotMapped]
		public List<KeyValueDropDown> listOwnVendorId { get; set; }
		[NotMapped]
		public List<string> lstUserModule { get; set; }
		[NotMapped]
		public List<int> selected_route_ids { get; set; }
		[NotMapped]
		public List<RouteInfo> lstRouteInfo { get; set; }
		[NotMapped]
		public string gis_design_id { get; set; }
		public ProjectSpecificView()
		{
			network_status = "";
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			lstUserModule = new List<string>();
			lstRouteInfo = new List<RouteInfo>();
		}

		public string entity_sub_type { get; set; }
	}
	public class CommonGridAttributes
	{
		public int pageSize { get; set; }
		public int totalRecord { get; set; }
		public int currentPage { get; set; }
		public string sort { get; set; }
		public string orderBy { get; set; }
		public string searchText { get; set; }
		public string searchBy { get; set; }
		public Boolean is_active { get; set; }
		public string application_access { get; set; }
		public string fileType { get; set; }
		public int customDate { get; set; }
		public DateTime? fromDate { get; set; }
		public DateTime? toDate { get; set; }
		public int history_id { get; set; }
		public string log_type { get; set; }
		public CommonGridAttributes()
		{
			sort = "";
			orderBy = "";
		}
	}
	public class MinValue : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value != null && Convert.ToInt32(value) == 0)
			{
				return new ValidationResult("Zero does not allow as minimum value!");
			}
			else { return ValidationResult.Success; }
		}
	}
	public class AuthorizeMessage
	{
		public bool status { get; set; }
		public string message { get; set; }
	}
	public class PrintMapFooterAttribute
	{
		public string jobId { get; set; }
		public string department { get; set; }
		public string plottedBy { get; set; }
		public string team { get; set; }
		public string drawnBy { get; set; }
		public string checkedBy { get; set; }
		public string recheckedBy { get; set; }
		public string approvedBy { get; set; }
		public string X_Document_Index { get; set; }
		public string Y_Document_Index { get; set; }
		public string phase { get; set; }
		public string plan { get; set; }
		public string provDir { get; set; }
		public string FDCNo { get; set; }
		public string OLT { get; set; }

		public bool IsSavedAsTemplate { get; set; }
		public string Template_name { get; set; }
		[NotMapped]
		public int templateId { get; set; }
	}
	[Serializable]
	public class PrintMapOut
	{
		public byte[] fileBytes { get; set; }
		public string fileExtension { get; set; }
		public string fileName { get; set; }
		public string localResultFilePath { get; set; }
	}
	public class PrintMap
	{
		public string layerURL { get; set; }
		public string pageSize { get; set; }
		public string mapCordinates { get; set; }
		//[Required]
		//[MaxLength(50, ErrorMessage = "Maximum 50 characters are allowed!")]
		public string pageTitle { get; set; }
		public double radius { get; set; }
		public dynamic mapCenterLat { get; set; }
		public dynamic mapCenterLng { get; set; }
		public int mapCurrentZoom { get; set; }
		public string mapType { get; set; }
		public bool isStaticMapEnabled { get; set; }
		public string staticMapImgUrl { get; set; }
		public string pageLayout { get; set; }
		public int mapCanvasWidth { get; set; }
		public int mapCanvasHeight { get; set; }
		public List<layerFilters> layerFilters { get; set; }
		public List<layerUrl> layerUrls { get; set; }
		public string legendData { get; set; }
		public dynamic mapRightLng { get; set; }
		public dynamic mapLeftLng { get; set; }
		public dynamic mapTopLat { get; set; }
		public dynamic mapBottomLat { get; set; }
		public string mapSelectedGeom { get; set; }
		public List<LayerMapFilter> layerMapFilter { get; set; }
		public int printScale { get; set; }
		public bool printLegend { get; set; }
		public bool backgroundMap { get; set; }
		public bool backgroundClipping { get; set; }
		public string remarks { get; set; }
		public int[] ScaleLimit { get; set; }
		public bool isFooterTemplateEnabled { get; set; }
		public int pageScale { get; set; }
		public PrintMapFooterAttribute printMapAttr { get; set; }
		public List<string> ApplicableModuleList { get; set; }
		public int roleId { get; set; }
		public int userId { get; set; }
		public int sheetCount { get; set; }
		public string fileExtension { get; set; }
		public string AttachmentLocalPath { get; set; }
		public string checkBox_ImagePath { get; set; }
		public int maxPDflimit { get; set; }
		public bool isCropByScreen { get; set; }

		public string ftpPath { get; set; }
		public string ftpUserName { get; set; }
		public string ftpPassword { get; set; }
		public string ftpFinalPath { get; set; }
		public string printFolderName { get; set; }

		/////krishna
		public DateTime CreatedOn { get; set; }
		public List<PrintSavedTemplate> ddlSavedTemplate { get; set; }

		public int PdfCellCnt { get; set; } = 3;
		[NotMapped]
		public int printHistoryID { get; set; } = 0;

		public string selectedGeomType { get; set; }

		public string mapScaleRatio { get; set; }

		public List<MapScales> ddlMapScale { get; set; }
		[NotMapped]
		public string ClientLogoImageBytesForWeb { get; set; }
		public bool IsVisiblePrintLegendEntityCount { get; set; }
	}
	public class MapLayout
	{
		public dynamic mapCenterLatLong { get; set; }
		public int mapCurrentZoom { get; set; }

	}
	public class MapScales
	{
		public int Zoom { get; set; }
		public string Scale { get; set; }
	}
	public class layerFilters
	{
		public string DisplayName { get; set; }
		public string MapFilePath { get; set; }
		public string Name { get; set; }
		public List<Filters> filters { get; set; }
	}
	public class Filters
	{
		public string Field { get; set; }
		public string value { get; set; }
	}
	public class layerUrl
	{
		public string url { get; set; }
	}
	public class IspPortInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }

		public string network_id { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string port_name { get; set; }
		public int port_number { get; set; }
		public string port_type { get; set; }
		public string input_output { get; set; }
		public string port_status { get; set; }
		public int? destination_system_id { get; set; }
		public string destination_network_id { get; set; }
		public string destination_entity_type { get; set; }
		public int? created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int port_status_id { get; set; }
		public string comment { get; set; }
		[NotMapped]
		public string type { get; set; }
		//public int port_id { get; set; }
		//public int parent_system_id { get; set; }
		//public string parent_network_id { get; set; }
		//public string parent_entity_type { get; set; }
		//public int port_number { get; set; }
		//public string port_name { get; set; }
		//public string input_output { get; set; }
		//public string port_status { get; set; }
		//public string port_type { get; set; }
		//public int? destination_system_id { get; set; }
		//public string destination_network_id { get; set; }
		//public string destination_entity_type { get; set; }
		//public int created_by { get; set; }
		//public DateTime created_on { get; set; }
		//public int? modified_by { get; set; }
		//public DateTime? modified_on { get; set; }
		[NotMapped]
		public List<FibrePortStatusCount> ViewPortStatusCount { get; set; }
		public IspPortInfo()
		{
			ViewPortStatusCount = new List<FibrePortStatusCount>();

		}
	}
	public class BufferEntity
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
	}
	public class AssociateEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public int entity_system_id { get; set; }
		public string entity_network_id { get; set; }
		public string entity_type { get; set; }
		public int associated_system_id { get; set; }
		public string associated_network_id { get; set; }
		public string associated_entity_type { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public bool is_termination_point { get; set; }
		[NotMapped]
		public List<BufferEntity> listBufferEntity { get; set; }
		[NotMapped]
		public string geomType { get; set; }

		public AssociateEntity()
		{
			listBufferEntity = new List<BufferEntity>();
		}
	}
	public class AssociateLineEntity
	{
		public int parent_system_id { get; set; }
		public string parent_entity_type { get; set; }
		public string parent_network_id { get; set; }
		public bool parent_multi_association { get; set; }
		public PageMessage pageMsg { get; set; }
		public List<LineEntityInfo> listLineEntityInfo { get; set; }
		public bool parent_is_buried { get; set; }
		public int userId { get; set; }
		[NotMapped]
		public string parent_geom { get; set; }
		[NotMapped]
		public string parent_geom_type { get; set; }
		[NotMapped]
		public int manhole_count { get; set; }
		public AssociateLineEntity()
		{
			listLineEntityInfo = new List<LineEntityInfo>();
			pageMsg = new PageMessage();
		}

	}

	public class AssociateRoute
	{
		public int parent_system_id { get; set; }
		public string parent_entity_type { get; set; }
		public bool parent_multi_association { get; set; }
		public PageMessage pageMsg { get; set; }
		public List<RouteInfo> listrouteInfo { get; set; }
		public int userId { get; set; }
		public string parent_network_id { get; set; }
		public bool parent_is_buried { get; set; }
		public AssociateRoute()
		{
			listrouteInfo = new List<RouteInfo>();
			pageMsg = new PageMessage();
		}

	}
	public class RouteInfo
	{
		public int id { get; set; }
		public int cable_id { get; set; }
		public int entity_id { get; set; }
		public string entity_type { get; set; }
		public string entity_network_id { get; set; }
		public string route_id { get; set; }
		public string route_name { get; set; }
		public bool is_associated { get; set; }
		public bool is_multi_association { get; set; }
		public bool is_disabled { get; set; }
		public bool is_termination_point { get; set; }
		public string entity_title { get; set; }
		public string created_by { get; set; }
		public string created_on { get; set; }


		public RouteInfo()
		{
			is_associated = false;
		}
	}
	public class LineEntityInfo
	{
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string entity_network_id { get; set; }
		public bool is_associated { get; set; }
		public string entity_geom { get; set; }
		public bool is_multi_association { get; set; }
		public bool is_snapping_enabled { get; set; }
		public bool is_disabled { get; set; }
		public string associated_on { get; set; }
		public string associated_by { get; set; }
		public bool is_termination_point { get; set; }
		public string geom_type { get; set; }
		public string entity_title { get; set; }
		public string display_name { get; set; }

		public LineEntityInfo()
		{
			is_associated = false;
		}
	}
	public class ViewEntityLstCountModel
	{
		public BomBoqExportFilter objReportFilters { get; set; }
		public List<User> lstParentUsers { get; set; }
		public List<User> lstUsers { get; set; }
		public List<EntityLstCount> lstEntityList { get; set; }
		public BulkAsBuiltFilterAttribute objFilterAttributes { get; set; }
		public ViewEntityLstCountModel()
		{
			objFilterAttributes = new BulkAsBuiltFilterAttribute();
			objReportFilters = new BomBoqExportFilter();
			lstUsers = new List<User>();
		}

		//public List<KeyValueDropDown> lstNetworkStates { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstNetworkStatus { get; set; }
		public List<KeyValueDropDown> lstBindProjectCode { get; set; }
		public List<KeyValueDropDown> lstBindRootId { get; set; }

	}
	public class BulkAsBuiltFilterAttribute : CommonGridAttributes
	{
		public int userid { get; set; }
		public string dd_networkStatus { get; set; }
		public string geom { get; set; }
		public string selection_type { get; set; }
		public double buff_Radius { get; set; }
		public string entityType { get; set; }
		public string lstBindProjectCode { get; set; }
		public string lstBindRootId { get; set; }
		public List<int> SelectedParentUser { get; set; }
		public List<int> SelectedUserId { get; set; }
		public int roleid { get; set; }
		public string SelectedParentUsers { get; set; }
		public string SelectedUserIds { get; set; }
	}
	public class EmailSettingsVM
	{
		public EmailSettingsModel lstEmailSettingsModel { get; set; }
		public PageMessage pageMsg { get; set; }
		public EmailSettingsVM()
		{
			pageMsg = new PageMessage();
		}
	}
	public class EmailSettingsModel
	{
		public int id { get; set; }
		[Required]
		//[RegularExpression(@"^[1-9]\d*(\.\d+)?$", ErrorMessage = "Only Number and Decimal Allow!")]       
		//Commenting RegularExpression because it binds to "Number and Decimal" only and smtp_host formation is like "smtp.verio.com" type containing "characters" Commented By - Mohit_Kalkhanda
		public string smtp_host { get; set; }
		[Required]
		public int port { get; set; }
		[Required]
		[RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$", ErrorMessage = "Invalid Email Address!")]
		public string email_address { get; set; }
		[Required]
		public string email_password { get; set; }
		public bool auth { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime modified_on { get; set; }
		public bool enablessl { get; set; }
		public bool usedefaultcredentials { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }
		public EmailSettingsModel()
		{
			pageMsg = new PageMessage();
		}
	}
	public class VWTerminationPointMaster
	{
		[Key]
		public int id { get; set; }
		public int layer_id { get; set; }
		public string layer_name { get; set; }
		public int tp_layer_id { get; set; }
		public string tp_layer_name { get; set; }
		public bool is_isp_tp { get; set; }
		public bool is_osp_tp { get; set; }
		public bool is_enabled { get; set; }
		public int created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

	}

	public class TerminationPointMaster
	{
		[Key]
		public int id { get; set; }
		public int layer_id { get; set; }
		[NotMapped]
		public string layer_name { get; set; }
		public int tp_layer_id { get; set; }
		[NotMapped]
		public string tp_layer_name { get; set; }
		public bool is_isp_tp { get; set; }
		public bool is_osp_tp { get; set; }
		public bool is_enabled { get; set; }
		public int? created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public string created_by_text { get; set; }
		[NotMapped]
		public string modified_by_text { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }
		[NotMapped]

		public List<layerDetail> lstLineLayerDetails { get; set; }
		[NotMapped]
		public List<TPDropdown> lstTPLayerDetails { get; set; }
		[NotMapped]
		public List<int> lstTPLayer { get; set; }

		public TerminationPointMaster()
		{
			lstLineLayerDetails = new List<layerDetail>();
			lstTPLayerDetails = new List<TPDropdown>();
		}


	}
	public class TPDropdown
	{
		public int layer_id { get; set; }
		public string layer_name { get; set; }
	}
	public class ViewTPMaster : CommonGridAttributes
	{
		public List<TerminationPointMaster> listTPM { get; set; }
		public ViewTPMaster()
		{
			listTPM = new List<TerminationPointMaster>();
		}
	}
	public class FormInputSettings
	{
		[Key]
		public int id { get; set; }
		public string form_name { get; set; }
		public string form_feature_name { get; set; }
		public string form_feature_type { get; set; }
		public string feature_description { get; set; }
		public bool is_active { get; set; }
		public bool is_required { get; set; }
		public int? created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		public bool is_readonly { get; set; }
	}
	public class vwFormInputSettings
	{
		public string formName { get; set; }
		public List<string> lstFormNames { get; set; }
		public List<FormInputSettings> lstFormInputSettings { get; set; }
		public int user_id { get; set; }
		public PageMessage pageMsg { get; set; }
		public vwFormInputSettings()
		{
			lstFormNames = new List<string>();
			lstFormInputSettings = new List<FormInputSettings>();
			pageMsg = new PageMessage();
		}
	}
	public class propertyDetails
	{
		public string GetPropertyName<T>(System.Linq.Expressions.Expression<Func<T>> propertyLambda)
		{
			var me = propertyLambda.Body as System.Linq.Expressions.MemberExpression;

			if (me == null)
			{
				throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
			}

			return me.Member.Name;
		}
	}
	public class Reference
	{
		[Key]
		public int id { get; set; }
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string landmark { get; set; }
		public double? distance { get; set; }
		public string direction { get; set; }
		public string entry_point { get; set; }
		public int created_by { get; set; }
		public int modified_by { get; set; }
		public DateTime? modified_on { get; set; }

	}

	public class EntityReference
	{
		[NotMapped]
		public IList<DropDownMaster> listRefrencedirection { get; set; }
		[NotMapped]
		public layerDetail entityLayer { get; set; }
		public List<Reference> listPointAReference { get; set; }
		public List<Reference> listPointBReference { get; set; }
	}
	public class SpliceTrayInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string network_name { get; set; }
		public int tray_number { get; set; }
		public int no_of_ports { get; set; }
		public int parent_system_id { get; set; }
		public string parent_entity_type { get; set; }
		public string parent_network_id { get; set; }
		public int sequence_id { get; set; }
		public int created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public int used_port_count { get; set; }

	}

	public class DistributionBoxInfo
	{
		public List<DistributionBox> lstDistribitionBox { get; set; }
		public List<SplitterMaster> lstSplitter { get; set; }
	}

	public class DistributionBoxEntityInfo
	{
		public DistributionBox DistributionBox { get; set; }
		public List<SplitterMaster> lstSplitter { get; set; }
	}

	public class SplitterPortInfo
	{
		public SplitterMaster splitter { get; set; }
		public List<PortDetail> lstPort { get; set; }
	}


	#region AT Status
	public class ATExport
	{
		public string entity_network_id { get; set; }

		public string status { get; set; }

		public string status_date { get; set; }
		public string remark { get; set; }


	}
	public class entityAtAcceptance
	{
		[Key]
		public int id { get; set; }
		public int system_id { get; set; }
		public string entity_type { get; set; }


		public string status { get; set; }
		public DateTime status_date { get; set; }
		public string remark { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }


		//[NotMapped]
		//public IList<DropDownMaster> listATStatus { get; set; }

		//[NotMapped]
		//public List<entityAtAcceptance> listAtAcceptance { get; set; }

	}
	public class entityATStatusAtachmentsList
	{
		public int systemId { get; set; }
		public string entityType { get; set; }
		public int createdBy { get; set; }
		public IList<DropDownMaster> listATStatus { get; set; }
		public List<entityAtAcceptance> listAtStatusRecords { get; set; }
		public List<LibraryAttachment> listAtAttachments { get; set; }
		public entityATStatusAtachmentsList()
		{
			listAtStatusRecords = new List<entityAtAcceptance>();
			//listAtAttachments = new List<ATAcceptanceAttachments>();
			listAtAttachments = new List<LibraryAttachment>();
		}
	}
	public class TrenchExecution
	{
		[Key]
		public int id { get; set; }
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string execution_method { get; set; }
		public string execution_length { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public IList<DropDownMaster> ExecutionMethodsIn { get; set; }

	}
	public class trenchExecutionList
	{
		public int systemId { get; set; }
		public string entityType { get; set; }
		public int createdBy { get; set; }
		public string execution_method { get; set; }
		public string execution_length { get; set; }
		public IList<DropDownMaster> listExecutionmethod { get; set; }
		public List<TrenchExecution> listExecutionRecords { get; set; }
		//public List<LibraryAttachment> listAtAttachments { get; set; }
		public trenchExecutionList()
		{
			listExecutionRecords = new List<TrenchExecution>();
			//listAtAttachments = new List<ATAcceptanceAttachments>();
			//listAtAttachments = new List<LibraryAttachment>();
		}
	}

	#endregion



	#region Maintainence Charges

	public class EMCExport
	{
		public string entity_network_id { get; set; }
		public string type_of_activity_charge { get; set; }
		public string charge_category { get; set; }
		public string activity_start_date { get; set; }
		public string activity_end_date { get; set; }
		public double? total_cost { get; set; }
		public string remark { get; set; }
	}

	public class EntityMaintainenceCharges
	{
		[Key]
		public int id { get; set; }
		[NotMapped]
		public string entity_network_id { get; set; }
		public int entity_id { get; set; }
		public string entity_type { get; set; }
		[Required]
		public string type_of_activity_charge { get; set; }
		[Required]
		public string charge_category { get; set; }
		public DateTime activity_start_date { get; set; }
		public DateTime activity_end_date { get; set; }
		[Required]
		public double? total_cost { get; set; }
		public string remark { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public string activityStartDate { get; set; }
		[NotMapped]
		public string activityEndDate { get; set; }
		[NotMapped]
		public List<DropDownMaster> listChargeCategory { get; set; }
		[NotMapped]
		public List<DropDownMaster> listTypeOfActivityCharge { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		public EntityMaintainenceCharges()
		{
			objPM = new PageMessage();
			listChargeCategory = new List<DropDownMaster>();
			listTypeOfActivityCharge = new List<DropDownMaster>();
		}
	}

	public class EntityMaintainenceChargesList
	{
		public int entityId { get; set; }
		public string entityType { get; set; }
		public int createdBy { get; set; }
		public List<DropDownMaster> listChargeCategory { get; set; }
		public List<DropDownMaster> listTypeOfActivityCharge { get; set; }
		public List<EntityMaintainenceCharges> listEntityMaintainenceChargesRecords { get; set; }
		public List<LibraryAttachment> listEntityMaintainenceChargesAttachments { get; set; }
		public EntityMaintainenceChargesList()
		{
			listEntityMaintainenceChargesAttachments = new List<LibraryAttachment>();
			listEntityMaintainenceChargesRecords = new List<EntityMaintainenceCharges>();
		}
	}
	public class ROWAttachments
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int row_system_id { get; set; }
		public string row_stage { get; set; }
		public string org_file_name { get; set; }
		public string file_name { get; set; }
		public string file_extension { get; set; }
		public string file_location { get; set; }
		public string upload_type { get; set; }
		public string uploaded_by { get; set; }
		public int file_size { get; set; }
		[NotMapped]
		public string current_stage { get; set; }

		[NotMapped]
		public string file_size_converted { get; set; }
		[NotMapped]
		public string current_row_stage { get; set; }

	}

	#endregion


	public class ChangePassword
	{
		[Required]
		[DisplayName("Current Password")]
		public string currentPassword { get; set; }
		[Required]
		[DisplayName("New Password")]
		public string NewPassword { get; set; }

		[Required]
		[DisplayName("Confirm New Password")]
		public string confirmNewPassword { get; set; }

	}

	#region getCrmTickts
	public class CrmTicketInfo
	{

		public int ticket_id { get; set; }
		public string ticket_type { get; set; }
		public int can_id { get; set; }
		public string color_code { get; set; }
		public string icon_content { get; set; }
		public string icon_class { get; set; }
		public string customer_name { get; set; }
		public string address { get; set; }
		public string contact_no { get; set; }
		public string building_rfs_type { get; set; }
		public string building_code { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		public string assigned_by { get; set; }
		public DateTime? assigned_date { get; set; }
		public string ticket_reference { get; set; }
		public string reference_type { get; set; }
		public string reference_ticket_id { get; set; }
		public DateTime? target_date { get; set; }
		public string ticket_status { get; set; }
		public int ticket_type_id { get; set; }
		public DateTime? completed_on { get; set; }
		public string completed_by { get; set; }
		public string assigned_to { get; set; }
		public string parent_rfs_type { get; set; }
	}

	public class CrmTicketIn
	{

		public int user_id { get; set; }

	}
	public class crmticket
	{
		public List<CrmTicketInfo> CrmTicketsInfo { get; set; }
		public crmticket()
		{
			CrmTicketsInfo = new List<CrmTicketInfo>();
		}
	}
	#endregion

	public class lineTerminationPoint
	{
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public double longitude { get; set; }
		public double latitude { get; set; }
		public string network_id { get; set; }
		public bool is_a_end { get; set; }
	}
	public class validateEntity
	{
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string network_id { get; set; }
		public int parent_system_id { get; set; }
		public string parent_entity_type { get; set; }

		#region Structure        
		public int business_pass { get; set; }
		public int home_pass { get; set; }
		#endregion

		#region ISP Entity
		public int shaft_id { get; set; }
		public int floor_id { get; set; }
		#endregion

		#region Building
		public int total_tower { get; set; }
		#endregion

		#region UNIT
		public double room_height { get; set; }
		public double room_width { get; set; }
		public double room_length { get; set; }
		#endregion

		#region Cable
		public int a_system_id { get; set; }
		public string a_entity_type { get; set; }
		public int b_system_id { get; set; }
		public string b_entity_type { get; set; }

		#endregion

		#region ROW
		public string old_row_type { get; set; }
		public string new_row_type { get; set; }
		#endregion

		#region FMS
		public string specification { get; set; }
		public int vendor_id { get; set; }
		public string item_code { get; set; }
		public int no_of_input_port { get; set; }
		public int no_of_output_port { get; set; }
		public int no_of_port { get; set; }
		#endregion
	}

	public class ticketStepDetails
	{
		public int step_id { get; set; }
		public string step_name { get; set; }
		public string step_description { get; set; }
		public string icon_content { get; set; }
		public string icon_class { get; set; }
		public bool is_processed { get; set; }
		public int step_order { get; set; }
	}
	public class ticketStepsDetailIn
	{
		public int ticket_id { get; set; }
		public int ticket_type_id { get; set; }
		public string building_rfs_type { get; set; }
	}

	public class DBInfoViewModel
	{
		public List<DBInfo> lstDBinfo { get; set; }
		public DBInfoViewModel()
		{
			lstDBinfo = new List<DBInfo>();
		}
	}

	public class DBInfo
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
		[NotMapped]
		public string entity_name { get; set; }
		[NotMapped]
		public string entity_title { get; set; }
		[NotMapped]
		public string box_type { get; set; }
		[NotMapped]
		public string box_name { get; set; }
		public string latitude { get; set; }
		public string longitude { get; set; }
		public int no_of_port { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string distance { get; set; }
		public string splitter_available_ports { get; set; }
		[NotMapped]
		public string color_code { get; set; }

	}
	public class BuildingGeomInfo
	{
		public List<BuildingGeom> lstBuildingGeom { get; set; }
		public BuildingGeomInfo()
		{
			lstBuildingGeom = new List<BuildingGeom>();
		}
	}

	public class BuildingGeom
	{
		public string building_geom { get; set; }
	}
	public class Entitygeom
	{
		public string entityGeom { get; set; }
	}
	public class StaticPageMasterInfo
	{
		public string content { get; set; }
	}

	public class StaticPageMasterIn
	{
		public string name { get; set; }

	}
	public class StaticPageMasterViewModel
	{
		public List<StaticPageMasterInfo> lstStaticPageMasterInfo { get; set; }
		public StaticPageMasterViewModel()
		{
			lstStaticPageMasterInfo = new List<StaticPageMasterInfo>();
		}
	}

	public class UserModule
	{
		public int Id { get; set; }
		public string module_name { get; set; }
		[NotMapped]
		public bool is_selected { get; set; }
		public string module_description { get; set; }
		public string icon_content { get; set; }
		public string icon_class { get; set; }
		public string type { get; set; }

		public string module_abbr { get; set; }

		public int parent_module_id { get; set; }


		public int module_sequence { get; set; }
		[NotMapped]
		public bool is_offline_enabled { get; set; }

		[NotMapped]
		public List<UserModule> lstSubModule { get; set; }
		public string form_url { get; set; }

		//public UserModule()
		//{
		//    lstSubModule = new List<UserSubModule>();
		//}
	}


	public class userModuleViewModel
	{
		public List<UserModule> lstUserModule { get; set; }

		public userModuleViewModel()
		{
			lstUserModule = new List<UserModule>();
		}
	}
	public class Modules
	{
		public int id { get; set; }
		public string module_name { get; set; }
		public bool is_selected { get; set; }
		public bool is_active { get; set; }
		public string module_description { get; set; }
		public string icon_content { get; set; }
		public string icon_class { get; set; }
		public string type { get; set; }

		public string module_abbr { get; set; }

		public int parent_module_id { get; set; }



		public int module_sequence { get; set; }

		[NotMapped]
		public List<UserModule> lstSubModule { get; set; }

	}


	public class UserModuleMapping
	{
		[Key]
		public int id { get; set; }
		public int user_id { get; set; }
		public int module_id { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int modified_by { get; set; }
		public DateTime? modified_on { get; set; }
	}
	public class attachment_type_master
	{
		public int id { get; set; }
		public int layer_id { get; set; }
		public string category { get; set; }
		public string attachment_type { get; set; }
		public bool is_mandatory { get; set; }
		public int attachment_limit { get; set; }
	}

	public class SiteCustomerAttachment
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }

		public int site_id { get; set; }
		public string lmc_type { get; set; }
		public int parent_entity_system_id { get; set; }
		public string parent_entity_type { get; set; }
		public string org_file_name { get; set; }
		public string file_name { get; set; }
		public string file_extension { get; set; }
		public string file_location { get; set; }
		public string upload_type { get; set; }
		public string uploaded_by { get; set; }
		public DateTime uploaded_on { get; set; }
		public int file_size { get; set; }
		[NotMapped]
		public string file_size_converted { get; set; }
	}

	public class ChangeNetworkCode
	{
		public List<layerDetail> lstLayers { get; set; }
		public int layer_id { get; set; }
		public string layer_name { get; set; }
		public string old_network_id { get; set; }
		public string new_network_id { get; set; }
		public string remarks { get; set; }
		public int created_by { get; set; }
		public PageMessage pageMsg { get; set; }
		public DbMessage dbMsg { get; set; }

		public ChangeNetworkCode()
		{
			lstLayers = new List<layerDetail>();
			pageMsg = new PageMessage();
			dbMsg = new DbMessage();
		}
	}
	public class entityValidationStatus
	{
		public int id { get; set; }
		public string network_id { get; set; }
		public string entity_type { get; set; }
		public bool status { get; set; }
		public string message { get; set; }
	}

	#region ANTRA
	public class ViwUtilizationSetting
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public string Province { get; set; }
		public string Region { get; set; }
		public string layer_name { get; set; }
		public string utilization_range_Low { get; set; }
		public string utilization_range_High { get; set; }
		public string utilization_range_Moderate { get; set; }
		public string utilization_range_Over { get; set; }
		public string network_status { get; set; }
		public string created_by_text { get; set; }
		public string modified_by_text { get; set; }
		public int? created_by { get; set; }
		public int? modified_by { get; set; }
		public DateTime? created_on { get; set; }
		public int region_id { get; set; }
		public int province_id { get; set; }
		public int layer_id { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		public int entity_utilization_percentage { get; set; }
		public string entity_to_be_utilized { get; set; }
	}

	public class ViewEntityUtilizationSettings
	{
		public ViewEntityUtilizationSettingsFilter objEntityUtiliSettingsFilter { get; set; }
		public List<Region> lstRegion { get; set; }
		public List<Province> lstProvince { get; set; }
		public List<layerDetail> lstLayers { get; set; }
		public List<ViwUtilizationSetting> lstViwUtilizationSetting { get; set; }

		public ViewEntityUtilizationSettings()
		{
			lstLayers = new List<layerDetail>();
			lstProvince = new List<Province>();
			lstRegion = new List<Region>();
			objEntityUtiliSettingsFilter = new ViewEntityUtilizationSettingsFilter();
			lstViwUtilizationSetting = new List<ViwUtilizationSetting>();
		}
	}
	public class ViewEntityUtilizationSettingsFilter : CommonGridAttributes
	{
		public int layer_id { get; set; }
		public int region_id { get; set; }
		public int province_id { get; set; }
	}

	public class AddNewEntityUtilization
	{

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		[Required]
		public int layer_id { get; set; }
		[Required]
		public string network_status { get; set; }
		[Required]
		public int region_id { get; set; }
		[Required]
		public int province_id { get; set; }
		public string utilization_range_low_from { get; set; }
		public string utilization_range_low_to { get; set; }
		public string utilization_range_moderate_from { get; set; }
		public string utilization_range_moderate_to { get; set; }
		public string utilization_range_high_from { get; set; }
		public string utilization_range_high_to { get; set; }
		public string utilization_range_over_from { get; set; }
		public string utilization_range_over_to { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		[Required]
		public List<Region> lstRegion { get; set; }
		[NotMapped]
		public List<Province> lstProvince { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		[NotMapped]
		[Required]
		public List<layerDetail> lstLayers { get; set; }
		[NotMapped]
		public List<layerDetail> lstUtilizationReportLayers { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }
		public int? entity_utilization_percentage { get; set; }
		public int? entity_to_be_utilized { get; set; }

		public AddNewEntityUtilization()
		{
			lstRegion = new List<Region>();
			lstProvince = new List<Province>();
			lstLayers = new List<layerDetail>();
			pageMsg = new PageMessage();
		}

	}

	#endregion

	#region DropDown Master

	public class RowCountResult
	{
		public string message { get; set; }
		public string status { get; set; }

		public string rowcount { get; set; }

	}

	public class ViewDropDownMasterSetting
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int s_no { get; set; }
		public string layer_name { get; set; }
		public string dropdown_type { get; set; }
		public string dropdown_value { get; set; }
		public bool dropdown_status { get; set; }

		public string Status { get; set; }

		public int id { get; set; }
		public bool isvisible { get; set; }
		public int layer_id { get; set; }

		public bool? is_action_allowed { get; set; }

		public int? created_by { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public string created_by_text { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		[NotMapped]
		public string modified_by_text { get; set; }
		public DateTime? modified_on { get; set; }
	}


	public class ViewEntityDropdownMasterSettings
	{
		public ViewEntityDropdownMasterSettingsFilter objdrpFilter { get; set; }
		public ViewEntityRCAMasterSettingsFilter objRCAFilter { get; set; }

		public List<ViewDropDownMasterSetting> lstViewDropdownMasterSetting { get; set; }
		public List<ViewRCADetail> ViewRCADetails { get; set; }
		public List<layerDetail> lstLayers { get; set; }
		public List<dropdown_master> lstDropdownTypes { get; set; }
		public List<RCA_master> rcaDropdownTypes { get; set; }
		//public List<WfmRca> rcaTypes { get; set; }

		public int id { get; set; }
		[Required]

		public string RCA { get; set; }
		//[Required(ErrorMessage = "Please enter RCA")]

		public string status { get; set; }
		//[Required(ErrorMessage = "Please enter RC")]
		public int? created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public int totalRecords { get; set; }
		[NotMapped]
		public DateTime? modified_on { get; set; }
		// public PageMessage pageMsg { get; set; }
		public int layer_id { get; set; }
		public string layer_name { get; set; }

		public string dropdown_type { get; set; }

		public string Value { get; set; }
		public string OldValue { get; set; }
		[NotMapped]
		public int user_id { get; set; }

		public bool IsVisible { get; set; }

		public PageMessage pageMsg { get; set; }
		public string parent_value { get; set; }
		public int parent_id { get; set; }
		[NotMapped]
		public List<ParentDropdownMasterMapping> objDropdownMasterMapping { get; set; }

		public int dropdown_mapping_key { get; set; }
		public string dropdown_mapping_value { get; set; }
		public string dropdown_parent_value { get; set; }
		public string dropdown_value { get; set; }

		public ViewEntityDropdownMasterSettings()
		{
			lstLayers = new List<layerDetail>();
			objdrpFilter = new ViewEntityDropdownMasterSettingsFilter();
			objRCAFilter = new ViewEntityRCAMasterSettingsFilter();
			lstViewDropdownMasterSetting = new List<ViewDropDownMasterSetting>();
			pageMsg = new PageMessage();
			lstDropdownTypes = new List<dropdown_master>();
			objDropdownMasterMapping = new List<ParentDropdownMasterMapping>();
		}
	}
	public class ViewEntityDropdownMasterSettingsFilter : CommonGridAttributes
	{
		public int totalrecords { get; set; }
		public int s_no { get; set; }
		public string dropdown_type { get; set; }
		public string dropdown_value { get; set; }
		public bool dropdown_status { get; set; }
		public string layer_name { get; set; }
		public int id { get; set; }
		public bool isvisible { get; set; }
		public int layer_id { get; set; }
	}

	public class ViewEntityRCAMasterSettingsFilter
	{
		public int totalRecords { get; set; }
		[NotMapped]
		public int pageSize { get; set; }
		public string sort { get; set; }
		public string orderBy { get; set; }
		public int currentPage { get; set; }

		public string dropdown_value { get; set; }

		public int layer_id { get; set; }
		public string layer_name { get; set; }
		public string Value { get; set; }
		public string dropdown_type { get; set; }
		public int id { get; set; }


		public string RCA { get; set; }
		[Required(ErrorMessage = "Please enter RCA")]

		public string status { get; set; }
		[Required(ErrorMessage = "Please enter RC")]
		public int? created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
	}


	public class ViewRCADetail
	{
		//public List<ViewRCADetail> ViewRCADetails;
		//public int? id { get; set; }
		//      public string RCA { get; set; }


		public string status { get; set; }

		//public int? created_by { get; set; }
		//public DateTime? created_on { get; set; }
		//public int? modified_by { get; set; }
		//public DateTime? modify_on { get; set; }

		////public List<vw_jo_order_material_detail> ticketDetails { get; set; }
		//public ViewMateriaDetail()
		//{
		//    ticketDetails = new List<ViewMateriaDetail>();
		//}
	}
	public class DropdownMasterMapping
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int mapping_id { get; set; }
		public int parent_mapping_id { get; set; }
	}

	public class ParentDropdownMasterMapping
	{
		public int dropdown_mapping_key { get; set; }
		public string dropdown_mapping_value { get; set; }
		public string dropdown_value { get; set; }
	}


	#endregion

	public class LMCCableInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public int cable_system_id { get; set; }
		public string cable_network_id { get; set; }
		public string lmc_id { get; set; }
		public string lmc_type { get; set; }
		public string lmc_standalone_redundant { get; set; }
		public string customer_site_id { get; set; }
		public string rtn_name { get; set; }
		public double rtn_latitude { get; set; }
		public double rtn_longitude { get; set; }
		public string rtn_building_side_tapping_point { get; set; }
		public double rtn_building_side_tapping_latitude { get; set; }
		public double rtn_building_side_tapping_longitude { get; set; }
		public string lmc_cascaded_standalone { get; set; }
		public string cascading_from_site_name { get; set; }
		public double otdr_length { get; set; }
		public int no_of_core_used { get; set; }
		public string core_numbers { get; set; }
		public string termination_detail { get; set; }
		public string pop_type { get; set; }
		public string pop_infra_id { get; set; }
		public string pop_infra_provider { get; set; }
		public int no_of_patch_cord { get; set; }
		public string paf_no { get; set; }
		public double intra_society_length { get; set; }
		public double ug_length { get; set; }
		public double row_ri_deposit { get; set; }
		public DateTime? permit_date { get; set; }
		public int handhole { get; set; }
		public double otl_length { get; set; }
		public double otm_length { get; set; }
		public string po_number { get; set; }
		public string pop_name { get; set; }
		public double pop_latitude { get; set; }
		public double pop_longitude { get; set; }
		public string route_details { get; set; }
		public string route_no { get; set; }
		public double ri_length { get; set; }
		public double ibd_length { get; set; }
		public int no_of_pits { get; set; }
		[NotMapped]
		public string fiber_type { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		// [NotMapped]
		public int customer_system_id { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		[NotMapped]
		public string ActionTab { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstLMCType { get; set; }
		[NotMapped]
		public List<LibraryAttachment> lstLMCAttachment { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstLMCCascadedStandalone { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstRTNBuildingSiteTappingPoint { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstFiberCount { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstTerminationDetails { get; set; }
		[NotMapped]
		public int user_id { get; set; }
		public LMCCableInfo()
		{
			objPM = new PageMessage();
		}
	}
	public class lmcdetails
	{
		public string lmc_id { get; set; }
		public string sequence_id { get; set; }
	}
	public class cableStartLatlong
	{
		public double a_latitude { get; set; }
		public double a_longitude { get; set; }
	}

	public class GridDataModel<T>
	{
		public List<T> ListData { get; set; }
		public int PageSize { get; set; }
		public int TotalRecord { get; set; }
		public int NoOfPages { get; set; }

	}
	public class EntityNotifications
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int notification_id { get; set; }
		public string notification_type { get; set; }
		public string entity_type { get; set; }
		public string notification_text { get; set; }
		public int entity_system_id { get; set; }
		public int entity_total_ports { get; set; }
		public int entity_used_ports { get; set; }
		public bool is_closed { get; set; }
		public int created_by { get; set; }
		public int? modified_by { get; set; }
		public DateTime created_on { get; set; }
		public DateTime? modified_on { get; set; }


	}

	public class ViewEntityNotifications
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int notification_id { get; set; }
		public string notification_type { get; set; }
		public string entity_type { get; set; }
		public string notification_text { get; set; }
		public int entity_system_id { get; set; }
		public bool is_closed { get; set; }
		public int created_by { get; set; }
		public int? modified_by { get; set; }
		public DateTime created_on { get; set; }
		public DateTime? modified_on { get; set; }
		public string created_by_text { get; set; }
		public string modified_by_text { get; set; }
		public string entity_network_id { get; set; }
		public string entity_network_status { get; set; }
		public int entity_total_ports { get; set; }
		public int entity_used_ports { get; set; }
		public bool is_logicalview_enabled { get; set; }
		public bool is_virtual_port_allowed { get; set; }
		public string layer_title { get; set; }
		public string geom_type { get; set; }
		public int totalRecords { get; set; }
		public int comments_count { get; set; }
		public string cable_type { get; set; }

	}
	public class ViewEntityNotificationComments
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]

		public int comment_id { get; set; }
		public int notification_id { get; set; }
		public string comment_text { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		[NotMapped]
		public string created_by_text { get; set; }

	}
	public class EntityNotificationComments
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int comment_id { get; set; }
		public int notification_id { get; set; }
		public string comment_text { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		[NotMapped]
		public string created_by_text { get; set; }
		[NotMapped]
		public List<ViewEntityNotificationComments> lstViewEntityNotificationComments { get; set; }
		public EntityNotificationComments()
		{
			lstViewEntityNotificationComments = new List<ViewEntityNotificationComments>();
		}
	}

	public class EntityNotificationsVM
	{
		public EntityNotificationsFilter objEntityNotiFilter { get; set; }
		public List<layerReportDetail> lstUtilLayers { get; set; }
		public List<ViewEntityNotifications> lstViewEntityNotifications { get; set; }
		public EntityNotificationsVM()
		{
			lstUtilLayers = new List<layerReportDetail>();
			lstViewEntityNotifications = new List<ViewEntityNotifications>();
			objEntityNotiFilter = new EntityNotificationsFilter();
		}
	}
	public class EntityNotificationsFilter : CommonGridAttributes
	{
		// will add more pproperties based on requirement.
		public int userId { get; set; }
		public int roleId { get; set; }
	}

	public class OSPHeader
	{
		public string ApplicationName { get; set; }
		public User objuser { get; set; }
		public string ADFSEndPoint { get; set; }
		public long UnreadNotificationCount { get; set; }

		public OSPHeader()
		{
			objuser = new User();
		}
	}
	public class EntityNotificationStatus
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }

		[Column("system_id")]
		public int systemId { get; set; }
		[Column("entity_type")]
		public string entityType { get; set; }
		public bool status { get; set; }
		public string remarks { get; set; }
		[Column("created_by")]
		public int createdBy { get; set; }
		[Column("created_on")]
		public DateTime createdOn { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstComment { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		public EntityNotificationStatus()
		{
			objPM = new PageMessage();

		}
	}

	public class bulkDeleteOperation
	{
		public string entity_name { get; set; }
		public string network_code { get; set; }
		public bool is_deleted { get; set; }
		public string deleted_status { get; set; }
		public string message { get; set; }
	}

	public class entityPortInfo
	{
		public string network_id { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public string port_name { get; set; }
		public int port_number { get; set; }
		public string port_type { get; set; }
		public string input_output { get; set; }
		public string port_status { get; set; }
		public int? destination_system_id { get; set; }
		public string destination_network_id { get; set; }
		public string destination_entity_type { get; set; }
		public int? created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		public int port_status_id { get; set; }
		//    public int port_id { get; set; }
		//    public int port_number { get; set; }
		//    public string port_name { get; set; }
		//    public string port_type { get; set; }
		//    public string input_output { get; set; }
		//    public string port_status { get; set; }
		//    public int destination_system_id { get; set; }
		//    public string destination_network_id { get; set; }
		//    public string destination_entity_type { get; set; }
		//    public int created_by { get; set; }
		//    public DateTime crearted_on { get; set; }
		//    public DateTime? modified_on { get; set; }
		//    public int modified_by { get; set; }
	}
	public class ParentInfo
	{

		public int p_system_id { get; set; }
		public string p_network_id { get; set; }
		public string p_entity_type { get; set; }
	}

	public class AssociateEntityRequest
	{
		public int systemId { get; set; }
		public string entityType { get; set; }
		public string networkId { get; set; }
		public string parentGeom { get; set; }
		public string parentGeomType { get; set; }
		public int userId { get; set; }
	}
	public class AssociateRouteRequest
	{
		public int systemId { get; set; }
		public string entityType { get; set; }
		public string networkId { get; set; }
		public string parentGeom { get; set; }
		public string parentGeomType { get; set; }
		public int userId { get; set; }
	}

	public class TerminationEntityRequest
	{
		public string txtGeom { get; set; }
		public string entityType { get; set; }
		public int mtrBuffer { get; set; }
		public int userId { get; set; }
	}
	public class AdditionalAttributes
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int system_id { get; set; }
		public string erp_code { get; set; }
		public string entity_type { get; set; }
		public string erp_name { get; set; }
		public string ef_customers { get; set; }
		public string status { get; set; }
		public string zone { get; set; }
		public DateTime rfs_date { get; set; }
		public string hub_maintained { get; set; }
		public string hm_power_bb { get; set; }
		public string hm_earthing_rating { get; set; }
		public string hm_rack { get; set; }
		public string hm_olt_bb { get; set; }
		public string hm_fms { get; set; }
		public string splicing_machine { get; set; }
		public string optical_power_meter { get; set; }
		public string otdr { get; set; }
		public string laptop_with_giga_port { get; set; }
		public string l3_updation_on_inms { get; set; }
		[NotMapped]
		public string rfsSetDate { get; set; }
	}
	public class Api_Logs
	{
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_type { get; set; }
		public string attribute_info { get; set; }
		public string header_attribute { get; set; }
		public int user_id { get; set; }
	}
	public class RevertEntity
	{
		public int ticketid { get; set; }
		public int systemId { get; set; }
		public string entityType { get; set; }
		public string geomType { get; set; }
		public string action { get; set; }
		public int userid { get; set; }
		public ImpactDetail childElements { get; set; }
	}
	public class SubmitNetworkParam
	{
		public int ticket_id { get; set; }
		public string entity_ids_types { get; set; }
		public string remarks { get; set; }
		public int user_id { get; set; }
		public string source { get; set; }
		public string status { get; set; }
	}
	public class UserDashboardParam
	{
		public int manager_id { get; set; }
	}
	public class UserDashboard
	{
		public int user_id { get; set; }
		public string user_name { get; set; }
		public string user_email { get; set; }
		public string last_login_time { get; set; }
		public string last_logout_time { get; set; }
		public string last_tracking { get; set; }
		public string latitude { get; set; }
		public string longitude { get; set; }
		public string ticket_id { get; set; }
		public string ticket_status { get; set; }
	}

	public class NearByAssociatedEntities
	{
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string entity_title { get; set; }
		public string entity_network_id { get; set; }
		public string geom_type { get; set; }
		public bool is_isp_entity { get; set; }
		public DateTime created_on { get; set; }
		public string display_name { get; set; }

	}
	public class BulkPodAssociation
	{
		public string geom { get; set; }
		public string selection_type { get; set; }
		public double buff_Radius { get; set; }
		public int user_id { get; set; }
		public string ntk_type { get; set; }
		public string entity_select { get; set; }
		public int primary_pod_system_id { get; set; }
		public int secondary_pod_system_id { get; set; }
		public string entity_sub_type { get; set; }
	}

	public class _RegionProvince
	{
		public int region_id { get; set; }
		public int province_id { get; set; }
		public string region_name { get; set; }
		public string province_name { get; set; }
		public string province_abbreviation { get; set; }
		public string country_name { get; set; }
		public string province_geom { get; set; }
		public int? created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		public string created_by_text { get; set; }
		public string modified_by_text { get; set; }
		public string region_geom { get; set; }
	}

	public class ViewCloneDependent
	{
		public int refId { get; set; }
		public string entityName { get; set; }
		public string geomType { get; set; }
		public string geom { get; set; }
		public int childEntityCount { get; set; }
		public int associatedEntityCount { get; set; }
		public string networkid { get; set; }
		public bool is_accessories { get; set; }
		public List<CloneDependent> lstCloneDependent { get; set; }
		public ViewCloneDependent()
		{
			lstCloneDependent = new List<CloneDependent>();
			// is_accessories = true;
		}
	}
	public class CloneDependent
	{
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string network_id { get; set; }
		public string display_name { get; set; }
		public bool is_child_entity { get; set; }
		public bool is_associated_entity { get; set; }
		public bool is_accessories_placed { get; set; }
		public bool is_include_in_clone { get; set; }
		public string layer_title { get; set; }
	}

	public class ViewGroupLibrary
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		[NotMapped]
		public int system_id { get; set; }
		public string entity_type { get; set; }
		[NotMapped]
		public int childEntityCount { get; set; }
		[NotMapped]
		public int associatedEntityCount { get; set; }
		public bool is_accessories_required { get; set; }
		public bool is_child_entity { get; set; }
		public bool is_associated_entity { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public int parent_id { get; set; }
		public int? created_by { get; set; }
		public string entity_data { get; set; }
		public string accessories_data { get; set; }
		[NotMapped]
		public string geomType { get; set; }
		public DateTime? created_on { get; set; }
		[NotMapped]
		public bool is_visible_in_ne_library { get; set; }
		[NotMapped]
		public bool is_osp_layer_freezed_in_library { get; set; }
		[NotMapped]
		public bool planned_add { get; set; }

		[NotMapped]
		public List<CloneDependent> lstCloneDependent { get; set; }
		public ViewGroupLibrary()
		{
			lstCloneDependent = new List<CloneDependent>();
			// is_accessories = true;
		}
	}


	public class getGroupLibrary
	{
		public int id { get; set; }
		[NotMapped]
		public int system_id { get; set; }
		public string entity_type { get; set; }
		[NotMapped]
		public int childEntityCount { get; set; }
		[NotMapped]
		public int associatedEntityCount { get; set; }
		public bool is_accessories_required { get; set; }
		public bool is_child_entity { get; set; }
		public bool is_associated_entity { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public int parent_id { get; set; }
		public int? created_by { get; set; }
		public string entity_data { get; set; }
		public string accessories_data { get; set; }
		[NotMapped]
		public string geomType { get; set; }
		public DateTime? created_on { get; set; }

		public bool is_visible_in_ne_library { get; set; }

		public bool is_osp_layer_freezed_in_library { get; set; }

		public bool planned_add { get; set; }
	}
	public class GroupLibraryAccessories
	{
		public string specification { get; set; }
		public string accessories_name { get; set; }
		public string vendor_name { get; set; }
		public string item_code { get; set; }
		public string subcategory1 { get; set; }
		public string subcategory2 { get; set; }
		public string subcategory3 { get; set; }
		public string remarks { get; set; }
		public int quantity { get; set; }

	}
	public class GroupLibraryDetails
	{
		public int id { get; set; }
		public int parent_id { get; set; }
		public string layer_title { get; set; }
		public string specification { get; set; }
		public string vendor_name { get; set; }
		public string item_code { get; set; }
		public string subcategory1 { get; set; }
		public string subcategory2 { get; set; }
		public string subcategory3 { get; set; }
		public bool is_associated_entity { get; set; }
		public bool is_child_entity { get; set; }
		public List<GroupLibraryAccessories> accessories_data { get; set; }

	}
	public class GLLineDetails
	{
		public string geom { get; set; }
		public int library_id { get; set; }
		public int line_id { get; set; }
		//public List<TerminationPoint> lstGLTerminationPoint { get; set; }
		//	public GLLineDetails() {
		//		lstGLTerminationPoint = new Array[]();
		//}

	}
	public class TerminationPoint
	{
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string network_name { get; set; }
		public string termination_type { get; set; }
		public string lnglat { get; set; }
		public string actualLatLng { get; set; }
		public string node_type { get; set; }
		public string display_name { get; set; }
		public int line_id { get; set; }
	}

	public class GroupLibrary
	{
		public bool status { get; set; }
		public string networkid { get; set; }
		public string entity_type { get; set; }


	}
	public class Propertie
	{
		public int library_id { get; set; }
		public int line_id { get; set; }
		public int user_id { get; set; }
		public int? system_id { get; set; }
		public string network_id { get; set; }
		public string network_name { get; set; }
		public string termination_type { get; set; }
		public string node_type { get; set; }
		public string display_name { get; set; }
		public string type { get; set; }
		public string coordinates { get; set; }

	}
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class GrpLibraryFeature
	{
		public string type { get; set; }
		public Properties properties { get; set; }
		public GrpLibraryGeometry geometry { get; set; }
	}

	public class GrpLibraryGeometry
	{
		public string type { get; set; }
		public List<object> coordinates { get; set; }
	}

	public class Properties
	{
		public int library_id { get; set; }
		public int line_id { get; set; }
		public int user_id { get; set; }
		public int? system_id { get; set; }
		public string network_id { get; set; }
		public string network_name { get; set; }
		public string termination_type { get; set; }
		public string node_type { get; set; }
		public string display_name { get; set; }
		public string type { get; set; }
		public string geom { get; set; }
		public string feature_type { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
	}

	public class Root
	{
		public string type { get; set; }
		public List<GrpLibraryFeature> features { get; set; }
		public PageMessage pageMsg { get; set; }
		public int user_id { get; set; }
	}

	public class test
	{
		public int system_id { get; set; }

	}
	public class VSATDetails : IReference
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public string network_id { get; set; }
		public string site_name { get; set; }
		public int region_id { get; set; }
		public int province_id { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string address { get; set; }


		public string category { get; set; }

		public string antenna_type { get; set; }

		public string service_type { get; set; }

		public string service_id { get; set; }
		public string transmission_type { get; set; }
		public string forward_link { get; set; }
		public string return_link { get; set; }
		public int uplink_carrier { get; set; }
		public int downlink_carrier { get; set; }
		public int uplink_datarate { get; set; }
		public int downlink_datarate { get; set; }
		public int carrier_bandwidth { get; set; }
		public string modulation_technique { get; set; }
		public string document_type { get; set; }
		public bool is_agreement_signed_one { get; set; }

		public string agreemented_by_name_one { get; set; }
		public string agreemented_by_position_one { get; set; }
		public string company_name_one { get; set; }
		public bool is_agreement_signed_two { get; set; }
		public string agreemented_by_name_two { get; set; }
		public string agreemented_by_position_two { get; set; }
		public string company_name_two { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public bool is_vsat_updated { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public IList<DropDownMaster> VSATCategory { get; set; }
		[NotMapped]
		public IList<DropDownMaster> VSATAntennaType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> VSATServiceType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> VSATTransmissionType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> VSATForwardLink { get; set; }
		[NotMapped]
		public IList<DropDownMaster> VSATReturnLink { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }
		public string origin_from { get; set; }
		public string origin_ref_id { get; set; }
		public string origin_ref_code { get; set; }
		public string origin_ref_description { get; set; }
		public string request_ref_id { get; set; }
		public string requested_by { get; set; }
		public string request_approved_by { get; set; }
	}

	#region VSAT Report
	public class ExportVSATEntitiesReport
	{
		public ExportVSATReportFilter objReportFilters { get; set; }
		public List<Region> lstRegion { get; set; }
		public List<Province> lstProvince { get; set; }
		public List<dynamic> lstReportData { get; set; }
		public List<layerDetail> lstLayers { get; set; }
		public List<KeyValueDropDown> lstLayerColumns { get; set; }
		public List<KeyValueDropDown> lstLayerDurationBasedColumns { get; set; }

		public ExportVSATEntitiesReport()
		{
			lstLayers = new List<layerDetail>();
			lstLayerColumns = new List<KeyValueDropDown>();
			lstProvince = new List<Province>();
			lstReportData = new List<dynamic>();
			lstRegion = new List<Region>();
			objReportFilters = new ExportVSATReportFilter();
			lstLayerDurationBasedColumns = new List<KeyValueDropDown>();
		}

	}
	[Serializable]
	public class ExportVSATReportFilter
	{
		public int system_id { get; set; }
		public string Searchtext { get; set; }
		public string SearchbyText { get; set; }
		public DateTime? fromDate { get; set; }
		public DateTime? toDate { get; set; }
		public int pageSize { get; set; }
		public int totalRecord { get; set; }
		public int currentPage { get; set; }
		public string sort { get; set; }
		public string sortdir { get; set; }
		public string geom { get; set; }
		public string geomType { get; set; }
		public double radius { get; set; }
		public int customDate { get; set; }
		public int userid { get; set; }
		public string orderBy { get; set; }
		public string DurationBasedColumnName { get; set; }
		public List<int> SelectedRegionId { get; set; }
		public string SelectedProvinceIds { get; set; }
		public string SelectedRegionIds { get; set; }
		public int[] SelectedProvinceId { get; set; }

	}
	#endregion

	#region AssociateEntityMaster
	public class AssociateEntityMaster
	{
		[Key]
		public int system_id { get; set; }
		public int layer_id { get; set; }
		[NotMapped]
		public string layer_name { get; set; }
		public int associate_layer_id { get; set; }
		[NotMapped]
		public string associate_layer_name { get; set; }
		public string layer_subtype { get; set; }
		public bool is_enabled { get; set; }
		public int? created_by { get; set; }
		[NotMapped]
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public string created_by_text { get; set; }
		[NotMapped]
		public string modified_by_text { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }
		[NotMapped]
		public List<layerDetail> lstSELayerDetails { get; set; }
		//[NotMapped]
		//public List<int> lstTPLayer { get; set; }

		public AssociateEntityMaster()
		{
			lstSELayerDetails = new List<layerDetail>();
		}



	}

	public class ViewAEMaster : CommonGridAttributes
	{
		public List<AssociateEntityMaster> listAEM { get; set; }
		public ViewAEMaster()
		{
			listAEM = new List<AssociateEntityMaster>();
		}
	}
	#endregion

	#region Layer Icom Mapping
	public class LayerIconMapping
	{
		[Key]
		[Column(Order = 0)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		[Key]
		[Column(Order = 1)]
		[Required]
		public int layer_id { get; set; }
		[NotMapped]
		public string layer_title { get; set; }
		[Key]
		[Column(Order = 3)]
		public string category { get; set; }
		public string subcategory { get; set; }

		public string icon_name { get; set; }
		public string icon_path { get; set; }
		[Key]
		[Column(Order = 2)]
		[Required]
		public string network_status { get; set; }
		public bool status { get; set; }
		public int? created_by { get; set; }
		public DateTime? modified_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? created_on { get; set; }
		public bool is_virtual { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public string created_by_text { get; set; }
		[NotMapped]
		public string modified_by_text { get; set; }
		[NotMapped]
		public List<layerDetail> lstlayerDetails { get; set; }
		[NotMapped]
		public List<CatogryDropdown> lstDropdown_master { get; set; }
		public int? landbase_layer_id { get; set; }
		public LayerIconMapping()
		{
			lstlayerDetails = new List<layerDetail>();
			lstDropdown_master = new List<CatogryDropdown>();

		}
	}

	public class CatogryDropdown
	{
		public string dropdown_value { get; set; }
		public string dropdown_key { get; set; }

	}
	public class ViewLayerIcon : CommonGridAttributes
	{
		public List<LayerIconMapping> listLayerIcon { get; set; }
		public ViewLayerIcon()
		{
			listLayerIcon = new List<LayerIconMapping>();
		}

	}
	#endregion

	public class CPEInstallation
	{
		public bool status { get; set; }
		public string message { get; set; }
		// wifi router
		public string WIFI_NetworkId { get; set; }
		public string CPE_NetworkId { get; set; }
		public int WIFI_SystemId { get; set; }
		public int CPE_SystemId { get; set; }
	}

	public class ViewServiceFacilityJobOrder
	{
		public int id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public bool is_selected { get; set; }
		public string module_description { get; set; }
		public string type { get; set; }
		public int role_id { get; set; }
	}
	public class JsonProcessResponse<T> where T : class
	{
		public JsonProcessResponse()
		{
			status = string.Empty;
			message = string.Empty;
			ProcessId = string.Empty;
			CsaDesignName = string.Empty;
		}
		public string status { get; set; }
		public string message { get; set; }
		public string ProcessId { get; set; }
		public string CsaDesignName { get; set; }
	}
	public class geocodfication_logs
	{
		public string entity_type { get; set; }
		public string network_id { get; set; }
		public string design_id { get; set; }
		public string status { get; set; }
		public string error_message { get; set; }
	}
	public class VoiceCommandMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public string actual_command_name { get; set; }
		[NotMapped]
		public List<VoiceCommandMaster> lstVoiceCommand { get; set; }
		[NotMapped]
		public string command_pronounce { get; set; }
		[NotMapped]
		public int command_id { get; set; }
		[NotMapped]
		public SaveVoiceCommandMaster saveVoiceCommandData { get; set; }
		[NotMapped]
		public IList<SaveVoiceCommandMaster> VoiceCommandDetail { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public List<KeyValueDropDown> lstBindSearchBy { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }
		[NotMapped]
		public int pageSize { get; set; }
		[NotMapped]
		public int totalrecords { get; set; }
		[NotMapped]
		public int currentPage { get; set; }
		[NotMapped]
		public string sort { get; set; }
		[NotMapped]
		public string orderBy { get; set; }
		[NotMapped]
		public string searchText { get; set; }
		[NotMapped]
		public string searchBy { get; set; }

	}

	public class SaveVoiceCommandMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		[Required]
		public int command_id { get; set; }
		[NotMapped]
		public string actual_command_name { get; set; }
		[NotMapped]
		public List<VoiceCommandMaster> lstVoiceCommand { get; set; }
		[Required]
		public string command_pronounce { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public int pageSize { get; set; }
		[NotMapped]
		public int totalrecords { get; set; }
		[NotMapped]
		public Boolean is_valid { get; set; }
		[NotMapped]
		public string error_msg { get; set; }

	}
	public class TempSaveVoiceCommandMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public int id { get; set; }
		[Required]
		public int command_id { get; set; }
		[Required]
		public string actual_command_name { get; set; }
		[Required]
		public string command_pronounce { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public int pageSize { get; set; }
		public Boolean is_valid { get; set; }
		public string error_msg { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; }

	}
	public class UserActivityLog
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int user_id { get; set; }
		public string user_name { get; set; }
		public string controller_name { get; set; }
		public string action_name { get; set; }
		public string client_ip { get; set; }
		public string server_ip { get; set; }
		public string source { get; set; }
		public DateTime? action_on { get; set; }
		public string description { get; set; }

		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public CommonGridAttributes objGridAttributes { get; set; }
		[NotMapped]
		public List<UserActivityLog> listUserActivityLog { get; set; }
		public UserActivityLog()
		{
			objGridAttributes = new CommonGridAttributes();
			listUserActivityLog = new List<UserActivityLog>();
		}
	}

	public class UserActivityLogSettings
	{
		public static List<Models.UserActivityLogSettings> userActivityLogSettings { get; set; }

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public string controller_name { get; set; }
		public string action_name { get; set; }
		public string display_action_name { get; set; }
		public string description { get; set; }
		public bool is_active { get; set; }
		public string source { get; set; }
		public string project_name { get; set; }
		public string created_by { get; set; }
		public DateTime? created_on { get; set; }
		public string modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public static List<UserActivityLogSettings> lstUserActivityLogSettings { get; set; }
		public string res_key { get; set; }
	}
	public class UserActivityLogSettingsPage
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public string controller_name { get; set; }
		public string action_name { get; set; }
		public string display_action_name { get; set; }
		public string description { get; set; }
		public bool is_active { get; set; }
		public string source { get; set; }
		public string project_name { get; set; }
		public int? created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public int S_No { get; set; }
	}
	public class ViewUserActivityLogSettings : CommonGridAttributes
	{
		public List<UserActivityLogSettingsPage> lstlogs { get; set; }
		public CommonGridAttributes objGridAttributes { get; set; }
		public ViewUserActivityLogSettings()
		{
			objGridAttributes = new CommonGridAttributes();
			objGridAttributes.searchText = string.Empty;
			objGridAttributes.is_active = true;

		}
		[NotMapped]
		public List<KeyValueDropDown> lstSearchBy { get; set; }
	}
	public class AutoCodificationLog
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string action_by { get; set; }
		public DateTime? action_on { get; set; }
		public string action_type { get; set; }
		public string err_message { get; set; }
		[NotMapped]
		public string description { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public CommonGridAttributes objGridAttributes { get; set; }
		[NotMapped]
		public List<AutoCodificationLog> listCodificationLog { get; set; }
		public AutoCodificationLog()
		{
			objGridAttributes = new CommonGridAttributes();
			listCodificationLog = new List<AutoCodificationLog>();
		}
	}
	public class EntityDeleteLog
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int system_id { get; set; }
		public string entity_type { get; set; }
		public string network_id { get; set; }
		public DateTime? action_date { get; set; }
		public string action_by { get; set; }
		[NotMapped]
		public string description { get; set; }
		[NotMapped]
		public int totalRecords { get; set; }
		[NotMapped]
		public CommonGridAttributes objGridAttributes { get; set; }
		[NotMapped]
		public List<EntityDeleteLog> listEntityDeleteLog { get; set; }
		public EntityDeleteLog()
		{
			objGridAttributes = new CommonGridAttributes();
			listEntityDeleteLog = new List<EntityDeleteLog>();
		}
	}

	#region Trench customer details
	public class TrenchCustomerDetails
	{
		[Key]
		public int system_id { get; set; }
		public int trench_id { get; set; }
		public string state { get; set; }
		public string customer_name { get; set; }
		public string service_type { get; set; }
		public string section_name { get; set; }
		public string pair_requirement { get; set; }
		public double length_in_kms { get; set; }
		public string status { get; set; }
		public string po_no { get; set; }
		public double po_length_km { get; set; }
		public string po_release_date { get; set; }
		public string period { get; set; }
		public string from_date { get; set; }
		public string to_date { get; set; }
		public string hoto_no { get; set; }
		public string urid { get; set; }
		public string hoto_status { get; set; }
		public string hoto_date { get; set; }
		public string route_name { get; set; }
		public double hoto_length { get; set; }
		public int total_core { get; set; }
		public int live_core { get; set; }
		public int reserved_core { get; set; }
		public int created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int modified_by { get; set; }
		public DateTime? modified_on { get; set; }

		[NotMapped]
		public PageMessage objPM { get; set; }
		public TrenchCustomerDetails()
		{
			objPM = new PageMessage();
		}

	}

	public class TrenchCustomerDetailsList
	{
		public int customer_id { get; set; }
		public int trench_id { get; set; }
		public List<TrenchCustomerDetails> listTrenchCustomerDetailsRecords { get; set; }
		public List<TrenchCustomerDetailsAttachment> lstDocuments { get; set; }
		public TrenchCustomerDetailsList()
		{

			listTrenchCustomerDetailsRecords = new List<TrenchCustomerDetails>();
			lstDocuments = new List<TrenchCustomerDetailsAttachment>();
		}


	}
	#endregion
	public class BulkAssociationRequestLog
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int user_id { get; set; }
		public DateTime? created_on { get; set; }
		public int subarea_system_id { get; set; }
		public bool is_association_done { get; set; }
	}
	public class WorkAreaMarking
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int marking_id { get; set; }
		public string marking_type { get; set; }
		public int workspace_id { get; set; }
		public int zoom { get; set; }
		public string geom { get; set; }
		public int created_by { get; set; }
		public DateTime? created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public List<WorkAreaMarking> lstMarkings { get; set; }
		public WorkAreaMarking()
		{
			lstMarkings = new List<WorkAreaMarking>();

		}

	}
	public class EventEmailTemplateDetail
	{
		public int template_id { get; set; }
		public string project_phase { get; set; }
		public string trigger_event { get; set; }
		public string recipient_role { get; set; }
		public string recipient_list { get; set; }
		public string subject { get; set; }
		public string template { get; set; }
	}

	public class EmailSetting
	{
		public static EmailSettingsModel GetEmailSetting()
		{

			EmailSettingsModel objEmailSettingsModel = new EmailSettingsModel()
			{
				id = 1,
				port = 25,
				email_address = "no_reply_smartinventory@safaricom.co.ke",
				smtp_host = "webmail.safaricom.co.ke",
				email_password = "",
				enablessl = false,
				usedefaultcredentials = false
			};
			return objEmailSettingsModel;
		}
	}

	public class RingAssociation
    {
		public int ring_id { get; set; }
		public string ring_code { get; set; }
		public string region_code { get; set; }
		public string segment_code { get; set; }
		public string ring_capacity { get; set; }
       
     }
	public class RingAssociationDetails
	{
        public int region_id { get; set; }
        public int segment_id { get; set; }
        public int ring_id { get; set; }
        public string cable_id { get; set; }
		public List<TopologyRegionMaster> lstTopologyRegionMaster { get; set; }
		public List<TopologySegmentMaster> lstTopologySegmentMaster { get; set; }
		public List<TopologyRingMaster> lstTopologyRingMaster { get; set; }
		public List<RingAssociation> ringAssociations { get; set; }

		public RingAssociationDetails() {
            lstTopologySegmentMaster = new List<TopologySegmentMaster>();
            lstTopologyRingMaster = new List<TopologyRingMaster>();
            lstTopologyRegionMaster = new List<TopologyRegionMaster>();
            ringAssociations = new List<RingAssociation>();
	    }
    }

    public class NearestSiteDetails
    {
        public int id { get; set; }
        public double? fiber_ug_distance_to_network { get; set; }
        public double? fiber_oh_distance_to_network { get; set; }
        public double? total_fiber_distance { get; set; }
        public double? fiber_distance_to_nearest_site { get; set; }
        public string nearest_site { get; set; }
        public string sp_geometry { get; set; }
        public string network_id { get; set; }
        public int system_id { get; set; }
        public string site_geometry { get; set; }
        public double distance { get; set; }
        public double google_site_distance { get; set; }
        public string line_geometry { get; set; }

    }
}

