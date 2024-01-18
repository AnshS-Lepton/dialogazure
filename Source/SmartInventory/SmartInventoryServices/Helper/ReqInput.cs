using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace SmartInventoryServices.Helper
{
    public class ReqInput
    {
        public string data { get; set; }

    }

    public class UserLogOutIn
    {
        public int userId { get; set; }
        public string IsMasterLogin { get; set; }
    }
    public class ItemDropDownIn
    {
        public string entityType { get; set; }
        public string ddlType { get; set; }
    }


    public class UserLocationTrackingIn
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string networkProvider { get; set; }
        public int userId { get; set; }
        public int loginHistoryId { get; set; }
        public DateTime mobileTime { get; set; }
        public string entityType { get; set; }
        public int entitySystemId { get; set; }
        public double location_accuracy { get; set; }
        public string IsMasterLogin { get; set; }
    }
    public class UpdateBuildingGeomIn
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int userId { get; set; }
        public int system_Id { get; set; }
    }
    public class BulidingInfoByStatusIn
    {
        public int userId { get; set; }
        public int surveyAreaId { get; set; }
        public string status { get; set; }
    }

    public class DeleteEntityFromInfo
    {
        public int systemId { get; set; }
        public string entityType { get; set; }
        public string geomType { get; set; }
        public int userId { get; set; }
        public ImpactDetail childElements { get; set; }
    }
    public class NetworkLayersIn
    {
        public int Group_Id { get; set; }
        public int User_Id { get; set; }
        public bool isLibraryElement { get; set; }
    }
    public class NetworkLayersOut
    {

        public List<NetworkLayers> lstNetworkLayers { get; set; }
        public List<landBaseLayres> lstLandBaseLayers { get; set; }
        public List<BusnessLayerforAPI> lstWMSWMTSLayers { get; set; }
    }
    public class NetworkLayers
    {
        public int layerId { get; set; }
        public string layerName { get; set; }
        public string layerTitle { get; set; }
        public int minZoomLevel { get; set; }
        public int maxZoomLevel { get; set; }
        public string mapAbbr { get; set; }

        public bool isTemplateRequired { get; set; }
        public string networkIdType { get; set; }
        public string geomType { get; set; }
        public int templateId { get; set; }
        public bool isDirectSave { get; set; }
        public bool is_visible_in_mobile_lib { get; set; }
        public bool isNetworkTypeRequired { get; set; }
        public string layerNetworkGroup { get; set; }
        public bool isVirtualPortAllowed { get; set; }
        public bool isLogicalViewEnabled { get; set; }
        public bool is_moredetails_enable { get; set; }
        public bool planned_view { get; set; }
        public bool asbuild_view { get; set; }
        public bool dormant_view { get; set; }
        public int? mapLayerSeq { get; set; }
        // public string networkCodeSeperator { get; set; }       
        //public string networkCodeFormat { get; set; }
        public bool is_visible_on_mobile_map { get; set; }
        public bool is_remark_required_from_mobile { get; set; }
        public bool is_shaft_element { get; set; }
        public bool is_floor_element { get; set; }
        public bool is_isp_layer { get; set; }
        public bool is_split_allowed { get; set; }
        public bool is_association_enabled { get; set; }
        public bool is_association_mandatory { get; set; }
        public bool is_network_ticket_entity { get; set; }
        public bool is_mobile_isp_layer { get; set; }
        public bool is_offline_allowed { get; set; }
    }
    public class BuildinginfoIn
    {
        public int System_Id { get; set; }
        public int user_id { get; set; }
        public string EntityNetwork_Id { get; set; }
        public string EntityType { get; set; }
        public string status { get; set; }
        public double SWLat { get; set; }
        public double SWLng { get; set; }
        public double NELat { get; set; }
        public double NELng { get; set; }
    }

    public class EntityDropDownItemList
    {
        public string value { get; set; }
        public string key { get; set; }
    }
    public class SurveyInfobyBuildingidIn
    {
        public int buildingId { get; set; }
        public int userId { get; set; }
    }
    public class EntityTemplateIn
    {
        public int userId { get; set; }
        public string entityType { get; set; }
        public int templateId { get; set; }

    }
    public class ItemTemplateIn
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string pEntityType { get; set; }
    }
    public class ItemTemplateOut
    {
        public int id { get; set; }
    }
    public class SplitterDetailIn
    {
        public int userId { get; set; }
        public int systemId { get; set; }
        public string geom { get; set; }
    }
    public class ModileItemMaster
    {
        public string specification { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public int vendor_id { get; set; }
        public string item_code { get; set; }
        public string unitValue { get; set; }
        public string unit_input_type { get; set; }
    }
    public class SplitterMobileTemplateMaster : ModileItemMaster
    {
        public int id { get; set; }
        public int created_by { get; set; }
        public string splitter_ratio { get; set; }
        public string splitter_type { get; set; }
    }
    public class ADBMobileTemplateMaster : ModileItemMaster
    {
        public int no_of_input_port { get; set; }
        public int no_of_output_port { get; set; }
        public int no_of_port { get; set; }
        public int id { get; set; }
        public int created_by { get; set; }
        public string entity_category { get; set; }
    }
    public class ADBMobileTemplateOut
    {
        public int id { get; set; }
    }
    public class CDBMobileTemplateOut
    {
        public int id { get; set; }
    }
    public class SplitterMobileTemplateOut
    {
        public int id { get; set; }
    }
    public class CDBMobileTemplateMaster : ModileItemMaster
    {
        public int no_of_input_port { get; set; }
        public int no_of_output_port { get; set; }
        public int no_of_port { get; set; }
        public int id { get; set; }
        public int created_by { get; set; }
        public string entity_category { get; set; }
    }
    public class SplitterMobileMaster
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string splitter_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string splitter_ratio { get; set; }
        public string splitter_type { get; set; }
        public string geom { get; set; }
        public string boxType { get; set; }
        public int userid { get; set; }
        public string parent_network_id { get; set; }
        public string adb_network_id { get; set; }
        public string cdb_network_id { get; set; }
        public string address { get; set; }
        public List<LayerDetails> LayerDetails { get; set; }
        //public string specification { get; set; }
        //public string category { get; set; }
        //public string subcategory1 { get; set; }
        //public string subcategory2 { get; set; }
        //public string subcategory3 { get; set; }
        //public virtual int no_of_input_port { get; set; }
        //public virtual int no_of_output_port { get; set; }
        //public virtual int no_of_port { get; set; }
        //public virtual string unit { get; set; }
        //public virtual string other { get; set; }
        //public int vendor_id { get; set; }
        //public string item_code { get; set; }
        //public string unitValue { get; set; }
        //public int type { get; set; }
        //public int brand { get; set; }
        //public int model { get; set; }
        //public int construction { get; set; }
        //public int activation { get; set; }
        //public int accessibility { get; set; }                  
    }

    public class SaveSplitterMobileIn
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string network_id_type { get; set; }
        public string splitter_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string splitter_ratio { get; set; }
        public string splitter_type { get; set; }
        public string geom { get; set; }
        public string boxType { get; set; }
        public int userid { get; set; }
        public string parent_network_id { get; set; }
        public string adb_network_id { get; set; }
        public string cdb_network_id { get; set; }
        public string adb_network_id_type { get; set; }
        public string cdb_network_id_type { get; set; }
        public string address { get; set; }

    }
    public class saveSplitterOut
    {
        public int System_id { get; set; }

    }
    public class NearByEntitiesIn
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int bufferInMtrs { get; set; }
        public string entity_name { get; set; }
        public int userId { get; set; }
        public int ticket_id { get; set; }
    }

    public class NearByEntitiesByType
    {
        public int c_system_id { get; set; }
        public string c_entity_type { get; set; }
        public int bufferInMtrs { get; set; }
        public string search_entity_type { get; set; }
    }


    public class EntityInfoIn
    {
        public string entityType { get; set; }
        public string geomType { get; set; }
        public int systemId { get; set; }
        public int user_id { get; set; }
    }
    public class InsertUpdateBuildingOut
    {
        public int System_Id { get; set; }

    }

    public class SearchResultIn
    {
        public string SearchText { get; set; }
        public int userRoleId { get; set; }
        public int userId { get; set; }
    }
    public class VendorDropDownIn
    {
        public string specification { get; set; }
    }

    public class ItemCategoryIn
    {
        public string entitytype { get; set; }
        public string specification { get; set; }
        public int vendorId { get; set; }
    }

    public class ItemCategoryDetailOut
    {
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public string item_code { get; set; }
        public int no_of_input_port { get; set; }
        public int no_of_output_port { get; set; }
        public int no_of_port { get; set; }
        public string entity_category { get; set; }
        public string unit { get; set; }
        public string other { get; set; }
        public int no_of_tube { get; set; }
        public int no_of_core_per_tube { get; set; }

    }
    public class EntityTemplateIsExistIn
    {
        public string entityType { get; set; }
        public int userId { get; set; }
    }

    public class DistributionBoxInfoIn
    {
        public int userid { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string boxtype { get; set; }
    }

    public class DistributionBoxEntityInfoIn
    {
        public int userid { get; set; }
        public int system_id { get; set; }
        public string entitytype { get; set; }
    }
    public class SplitterPortInfoIn
    {
        public int userid { get; set; }
        public int system_id { get; set; }
        public string entitytype { get; set; }
        public string can_id { get; set; }

    }



    #region Customer
    //public class CustomerMobileMaster
    //{
    //    public int system_id { get; set; }
    //    public string network_id { get; set; }
    //    public string customer_name { get; set; }
    //    public int province_id { get; set; }
    //    public int region_id { get; set; }
    //    public double latitude { get; set; }
    //    public double longitude { get; set; }
    //    public string pin_code { get; set; }
    //    public string address { get; set; }
    //    public string activation_status { get; set; }
    //    public string status { get; set; }
    //    public string remarks { get; set; }
    //    public int parent_system_id { get; set; }
    //    public string parent_network_id { get; set; }
    //    public string parent_entity_type { get; set; }
    //    public string geom { get; set; }

    //    public int userid { get; set; }
    //    [NotMapped]
    //    public IspEntityMapping objIspEntityMap { get; set; }
    //    public string networkIdType { get; set; }

    //    public string region_name { get; set; }
    //    public string province_name { get; set; }

    //    public string rfsType { get; set; }
    //    public string building_code { get; set; }
    //    public string structure_name { get; set; }
    //    public int? floor_id { get; set; }
    //    public int structure_id { get; set; }
    //}


    public class getCustomerIn
    {
        public int userId { get; set; }
        public string can_id { get; set; }
        public int ticket_id { get; set; }
    }


    public class saveCustomerOut
    {
        public int system_id { get; set; }

    }


    public class SaveCustomerIn
    {
        public int system_id { get; set; }
        public string can_id { get; set; }
        public string customer_name { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string address { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string geom { get; set; }
        public int userid { get; set; }
        public string networkIdType { get; set; }
        public string region_name { get; set; }
        public string province_name { get; set; }
        public int sequence_id { get; set; }
        public string building_rfs_type { get; set; }
        public string building_Code { get; set; }
        public int structure_id { get; set; }
        public int floor_id { get; set; }
        public int ticket_id { get; set; }
        public string reference_type { get; set; }
        public string step_name { get; set; }
        public int step_id { get; set; }

    }

    #endregion


    #region JOB PACK MASTER

    public class JobPackIn
    {
        public int userId { get; set; }
        public int systemId { get; set; }
        public string stage1 { get; set; }
        public string stage2 { get; set; }
        public string stage3 { get; set; }
        public bool isCompleted { get; set; }

        public string status { get; set; }
    }
    #endregion

}
//#region getCrmTickts
//public class CrmTicketInfo
//{

//    public int id { get; set; }
//    public string ticket_type { get; set; }
//    public int can_id { get; set; }
//    public string name { get; set; }
//    public string address { get; set; }
//    public string contact_no { get; set; }
//    public int user_id { get; set; }
//    public string rfs_type { get; set; }

//}
//public class crmticket
//{
//    public List<CrmTicketInfo> CrmTicketsInfo { get; set; }
//    public crmticket()
//    {
//        CrmTicketsInfo = new List<CrmTicketInfo>();
//    }
//}
//#endregion
public class BuildingDetailIn
{
    public string network_id { get; set; }

}
public class StructureDetailIn
{
    public int system_id { get; set; }
    public string associated_entity_types { get; set; }
    public string can_id { get; set; }
    public bool isOnly_servingdb { get; set; }
    public string rfs_type { get; set; }
    public string module_abbr { get; set; }
}
public class BuildingCommentsIn
{
    public int building_system_id { get; set; }
}
public class LegendDetailIn
{
    public int userid { get; set; }
    public string group_name { get; set; }
    public int system_id { get; set; }
}
public class SaveCustomerAssociationIn
{
    public int user_id { get; set; }
    public int source_system_id { get; set; }
    public int source_port_number { get; set; }
    public int destination_system_id { get; set; }
    public string destination_network_id { get; set; }
    public string source_entity_type { get; set; }
    public string building_rfs_type { get; set; }
    public string destination_entity_type { get; set; }
    public List<WCRMaterialIN> lstWCRMaterial { get; set; }
    public string step_name { get; set; }
    public int step_id { get; set; }
    public int ticket_id { get; set; }
    public string reference_type { get; set; }
    public int p_box_id { get; set; }
    public string p_box_type { get; set; }
    public bool is_box_changed { get; set; }
    public string routeGeom { get; set; }
}

public class WCRMaterialIN
{
    public int id { get; set; }
    public string entity_type { get; set; }
    public string unit_measurement { get; set; }
    [NotMapped]
    public double qty_length { get; set; }
    public string drumNumber { get; set; }
    public double startReading { get; set; }
    public double endReading { get; set; }
    public double loopLength { get; set; }
    public double loopDistanceLength { get; set; }
}

public class saveCustomerAssociationOut
{
    public int? customer_system_id { get; set; }


}

public class GetWCRMatrialIn
{
    public string rfs_type { get; set; }
}

public class saveEntityAssociation
{
    public IspPortInfo IspPortInfo { get; set; }
    public List<CableMaster> lstCableInfo { get; set; }
    public List<WCRMaterialIN> lstWCRMaterialInfo { get; set; }
    public List<Models.ISP.HTBInfo> lstHTBInfo { get; set; }
    public List<ONTMaster> lstONTInfo { get; set; }
    public saveEntityAssociation()
    {
        lstCableInfo = new List<CableMaster>();
        lstHTBInfo = new List<Models.ISP.HTBInfo>();
        lstONTInfo = new List<ONTMaster>();
    }


}

public class NearByDistributionBoxInfoIn
{
    public string latitude { get; set; }
    public string longitude { get; set; }
    public string box_type { get; set; }
    public int bufferInMtr { get; set; }
    public string rfs_type { get; set; }
    public string module_abbr { get; set; }
}
public class BuildingIn
{
    public int system_id { get; set; }
    public string building_code { get; set; }
}

public class BarCodeIn
{
    public int? system_id { get; set; }
    public string entity_type { get; set; }
    public string barcode { get; set; }
    public string network_id { get; set; }
}
public class PowerMeterReadingIn
{
	public int? system_id { get; set; }
	public string entity_type { get; set; }
	public bool is_manual_meter_reading { get; set; }
	public double? power_meter_reading { get; set; }
}
public class UserModuleIn
{
    public int userId { get; set; }
    //  public string UserType { get; set; }
}
public class EntityAdvanceAttributeIn
{
    public string entityType { get; set; }
    public string geomType { get; set; }
    public int systemId { get; set; }
}
public class GetEquipmentSearchResultIn
{
    public string searchText { get; set; }
    public string entityType { get; set; }
    public int userId { get; set; }
}

public class GetEquipmentPortInfoIn
{
    public int entity_id { get; set; }
    public string entity_type { get; set; }
}
public class GetConnectionInfoIn
{
    public int entityid { get; set; }
    public int port_no { get; set; }
    public string entity_type { get; set; }
}

public class GetCPFelementPathIn
{
    public int entityid { get; set; }
    public string entity_type { get; set; }
    public int port_no { get; set; }
    public bool isStartingPoint { get; set; }
}

public class GetLogicalDiagramIn
{
    public int entityId { get; set; }
    public string entityType { get; set; }
    public string userName { get; set; }
    public string password { get; set; }
}

public class GetEntityLogicalViewIn
{
    public int userId { get; set; }
    public int systemId { get; set; }
    public string entityType { get; set; }
}

public class GetLogicalViewEquipmentSearchIn
{
    public string searchText { get; set; }
    public string entityType { get; set; }
    public int userId { get; set; }
}
public class BuildingDetailsIn
{
    public int system_id { get; set; }
    public int userId { get; set; }
}

public class GetLegendDetailsIn
{
    public string userName { get; set; }
    public string password { get; set; }
}

public class GetEntityLayerActionsIn
{
    public string layer_Name { get; set; }
    public string network_status { get; set; }
    public int role_id { get; set; }
    public int user_id { get; set; }
    public int systemId { get; set; }
}
public class GetNearbyEntityDetailsIn
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    public int bufferInMtrs { get; set; }
    public int user_id { get; set; }
    public int batch_size { get; set; }
    public int last_record_number { get; set; }
    public int pageno { get; set; }
    public int pagerecord { get; set; }
    public string layer_name { get; set; }

}
public class GetAllEntityActionsIn
{
    public int user_id { get; set; }
}

public class GetRegionProvinceLayersIn
{
    public int user_id { get; set; }
}

public class DownloadMapTilesIn
{
    public string userName { get; set; }
    public string password { get; set; }
    public int provinceId { get; set; }
    public string fileExtension { get; set; }
}


public class ShareLegendsDataIn
{
    public string userName { get; set; }
    public string password { get; set; }

}


public class UserRequestIn
{
    public int user_id { get; set; }
    public int userId { get; set; }
}

public class ResourcesRequestIn
{
    public string MobileResourcesKeyPassword { get; set; }
}
public class GetNearByNetworkEntitiesIn
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    public int bufferInMtrs { get; set; }
    public string buildingCode { get; set; }
    public string buildingName { get; set; }
    public string entityTypes { get; set; }
    public int userId { get; set; }
}
public class UploadDocumentIn
{
    public string entityType { get; set; }
    public string entitySystemId { get; set; }
    public string uploadType { get; set; }
    //public string fileName { get; set; }
    public string featureType { get; set; }
    // public HttpPostedFileBase postedFile { get; set; }
    // public int fileSizeInBytes { get; set; }
    public int userId { get; set; }
}
public class GetAttachmentDetailsIn
{
    public string entityType { get; set; }
    public int entitySystemId { get; set; }
    //public string uploadType { get; set; }
}
public class DeleteAttachmentsIn
{
    public int attachmentId { get; set; }
}
public class GetEntityImagesIn
{
    public string entityType { get; set; }
    public int entitySystemId { get; set; }
}
public class DownloadAttachmentIn
{
    public int attachmentId { get; set; }
}

public class PlanningCodeMasterIn
{
    public List<int> projectIds { get; set; }
}
public class WorkorderCodeMasterIn
{
    public List<int> planningIds { get; set; }
}

public class PurposeCodeMasterIn
{
    public List<int> workorderIds { get; set; }
}

public class ExportReportIn
{
    public int userId { get; set; }
    public int roleId { get; set; }
}
public class ExportReportFilterNewIn
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
    public string SelectedOwnerShipType { get; set; }
    public int customDate { get; set; }
    public string durationbasedon { get; set; }
    public string layerName { get; set; }

}
public class EntityForConversionIn
{
    public int system_id { get; set; }
    public string current_entity_type { get; set; }
    public string result_entity_type { get; set; }
}
public class ItemSpecificationResult
{
    public int no_of_ports { get; set; }
    public int vendor_id { get; set; }
    public string geom { get; set; }
    public string specification { get; set; }
    public string vendor_name { get; set; }
    public string item_code { get; set; }
}

public class NearByCables
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    public int bufferInMtrs { get; set; }
}
public class NearByEntities
{
    public string entity_type { get; set; }
    public string geom { get; set; }
}

public class GetGeoImagesIn
{
    public int user_id { get; set; }
}
public class LandbaseEntityInfo
{     
         public int systemId { get; set; }
        public string entityType { get; set; }
        public string geomType { get; set; }
        public string settingType { get; set; }
           
}

public class CPEInstallationIn
{
    public int source_port_number { get; set; }
    public string source_entity_type { get; set; }
    public string source_network_id { get; set; }
    public string routeGeom { get; set; }
    public int user_id { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
}