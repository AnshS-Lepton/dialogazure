using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLLayer
    {
        DALayer objDALayer = new DALayer();
        DAParentChildLayer objDAParentChildLayer = new DAParentChildLayer();
        public List<layerDetail> GetLayerDetails()
        {
            return objDALayer.GetLayerDetails();
        }

        public List<layerDetail> GetLayerDetailsForAutoPlanning()
        {
            return objDALayer.GetLayerDetailsForAutoPlanning();
        }
        public List<layerDetail> GetLayerDetailsForEntityDirection()
        {
            return objDALayer.GetLayerDetailsForEntityDirection();
        }
        public layerDetail GetLayerDetails(string layerName)
        {
            return objDALayer.GetLayerDetails(layerName);
        }
        public List<layerDetail> GetOSPLayers()
        {
            return objDALayer.GetOSPLayers();
        }
        public List<layerDetail> GetDataUploadLayers(int roleId)
        {
            return objDALayer.GetDataUploadLayers(roleId);
        }
        public List<LandBaseDetail> GetLandBaseDataUploadLayers(int roleId)
        {
            return objDALayer.GetLandBaseDataUploadLayers(roleId);
        }
        public List<layerDetail> GetVendorSpecLayers()
        {
            return objDALayer.GetVendorSpecLayers();
        }
        public List<layerDetail> GetAllOSPLayers()
        {
            return objDALayer.GetAllOSPLayers();
        }
        //public List<NetworkLayer> GetNetworkLayers(int userId, int groupId)
        //{
        //    return objDALayer.GetNetworkLayers(userId, groupId);
        //}
        public List<NetworkLayer> GetNetworkLayers(int userId, int groupId, int roleId = 0, string connectionString="")
        {
            return objDALayer.GetNetworkLayers(userId, groupId, roleId, connectionString);
        }
        public List<DropDownMaster> GetCablecategoryList()
        {
            return objDALayer.GetCablecategoryList();
        }
        public List<landBaseLayres> GetLandBaseLayres(int userId, int roleId = 0)
        {
            return objDALayer.GetLandBaseLayres(userId, roleId);
        }
        public List<BusnessLayerforAPI> GetWMSWMTSLayres(int userId, int roleId = 0)
        {
            return objDALayer.GetWMSWMTSLayres(userId, roleId);
        }
        public List<NetworkLayer> GetAllNetworkLayersPermissions(int userId)
        {
            return objDALayer.GetAllNetworkLayersPermissions(userId);
        }
        public List<string> GetUserModuleAbbrList(int userId, string userType)
        {
            return objDALayer.GetUserModuleAbbrList(userId, userType);
        }
        //priyanka

        public List<BoundaryPushToGis> GetBoundaryPushToGis(BoundaryPushFilter objFilter)
        {
            return objDALayer.GetBoundaryPushToGis(objFilter);
        }
        public BoundaryPushToGis GetBoundaryPushStatus(BoundaryPushFilter objFilter)
        {
            return objDALayer.GetBoundaryPushStatus(objFilter);
        }




        public NetworkLayer GetNetworkLayers(int userId, int groupId, string layerName)
        {
            return objDALayer.GetNetworkLayers(userId, groupId, layerName);
        }
        public List<NetworkLayer> GetISPNetworkLayers(int userId, int groupId, int role_id)
        {
            return objDALayer.GetISPNetworkLayers(userId, groupId, role_id);
        }
        //public List<NetworkLayer> GetISPNetworkLayers(int userId, int groupId)
        //{
        //    return objDALayer.GetISPNetworkLayers(userId, groupId);
        //}
        public List<Kmp> Getkmp(string kmpType)
        {
            return objDALayer.Getkmp(kmpType);
        }

        public List<RegionProvinceLayer> GetRegionProvinceLayers(int userId)
        {
            return objDALayer.GetRegionProvinceLayers(userId);
        }
        public List<Province> GetProvincebyRegionID(string regionId)
        {
            return objDALayer.GetProvincebyRegionID(regionId);
        }
        public List<ISPNetworkLayerElement> GetISPNetworkLayerElements(int structureId, int role_id)
        {
            return objDALayer.GetISPNetworkLayerElements(structureId, role_id);
        }
        public Dictionary<string, string> getLayerDetail(int layerId, string layerName)
        {
            return objDALayer.getLayerDetail(layerId, layerName);
        }
        public List<LayerMapping> getLayerMapping(string layerName)
        {
            return objDALayer.getLayerMapping(layerName);
        }
        public List<ParentChildLayerMapping> GetParentChildLayerMappings()
        {
            return objDAParentChildLayer.GetParentChildLayerDetails();
        }
        public DataTable GetParentChildLayerDetails(int layer_id)
        {

            return objDAParentChildLayer.GetParentChildLayerDetails(layer_id);
        }
        public layerDetail getLayerDetailsByName(string layerName)
        {
            return objDALayer.getLayerDetailsByName(layerName);
        }
        public layerDetail getLayer(string layerName)
        {
            return objDALayer.getLayer(layerName);
        }
        public List<layerDetail> SaveLayerDetails(List<layerDetail> objLay)
        {
            return new DALayer().SaveLayerDetails(objLay);
        }
        public List<NetworkLayer> GetMobileNetworkLayers(int userId, int groupId, bool isLibraryElement)
        {
            return objDALayer.GetMobileNetworkLayers(userId, groupId, isLibraryElement);
        }
        public List<LayerDetails> GetSpltParentBoxDetails()
        {
            return objDALayer.GetSpltParentBoxDetails();
        }

        public List<KeyValueDropDown> GetSearchByColumnName(string layer_name)
        {
            return new DALayer().GetSearchByColumnName(layer_name);
        }
        public List<KeyValueDropDown> GetSearchByLandBaseColumnName(string layer_name)
        {
            return new DALayer().GetSearchByLandBaseColumnName(layer_name);
        }
        public List<KeyValueDropDown> GetLMCReportSearchByColumnName(string entity_type, string lmc_type)
        {
            return new DALayer().GetLMCReportSearchByColumnName(entity_type, lmc_type);
        }
        public List<KeyValueDropDown> GetDurationBasedColumnName(string layer_name)
        {
            return new DALayer().GetDurationBasedColumnName(layer_name);
        }

        public List<ViewRCADetail> GetAllRCADetail()
        {
            return objDALayer.GetAllRCADetail();
        }

        public List<layerDetail> GetAllLayerDetail()
        {
            return objDALayer.GetAllLayerDetail();
        }
        public List<layerDetail> GetAllDropdownLayerDetail()
        {
            return objDALayer.GetAllDropdownLayerDetail();
        }
        public List<layerDetail> GetLayerDetailsForReport()
        {
            return objDALayer.GetLayerDetailsForReport();
        }
        public List<layerDetail> GetLayerDetailsForUtilization()
        {
            return objDALayer.GetLayerDetailsForUtilization();
        }
        public List<layerReportDetail> GetReportLayers(int roleId, string purpose)
        {
            return objDALayer.GetReportLayers(roleId, purpose);
        }
        public List<layerReportDetail> GetSplitReportLayers(int roleId, string purpose)
        {
            return objDALayer.GetSplitReportLayers(roleId, purpose);
        }
        public List<layerDetail> GetLayerDetailsForHistory()
        {
            return objDALayer.GetLayerDetailsForHistory();
        }
        public List<layerDetail> GetInfoLayers()
        {
            return objDALayer.GetInfoLayers();
        }
        public List<layerDetail> GetChangeNetworkInfoLayers()
        {
            return objDALayer.GetChangeNetworkInfoLayers();
        }
        public List<Region> GetAllRegion(RegionIn objRegionIn)
        {
            return new DALayer().GetAllRegion(objRegionIn);
        }
        public List<DbMessage> CreateEntityAlongDirection(string entity_types, string line_geom, int user_id)
        {
            return new DALayer().CreateEntityAlongDirection(entity_types, line_geom, user_id);
        }

        public List<Province> GetProvinceByRegionId(ProvinceIn objProvinceIn)
        {
            return new DALayer().GetProvinceByRegionId(objProvinceIn);
        }
        public List<layerDetail> GetStartEndPointType()
        {
            return new DALayer().GetStartEndPointType();
        }
        //public List<ConnectionMaster> GetConnectionString(string moduleAbbr)
        public ConnectionMaster GetConnectionString(string moduleAbbr)
        {
            return new DALayer().GetConnectionString(moduleAbbr);
        }
        public List<EntitySummaryReport> GetExportReportSummary(ExportReportFilterNew objReportFilter)
        {
            return new DALayer().GetExportReportSummary(objReportFilter);
        }
        public List<EntitySummaryReport> GetSplitReportSummary(ExportReportFilterNew objReportFilter)
        {
            return new DALayer().GetSplitReportSummary(objReportFilter);
        }
        public List<EntitySummaryReport> GetAuditLogReportSummary(ExportReportFilterNew objReportFilter)
        {
            return new DALayer().GetAuditLogReportSummary(objReportFilter);
        }
        public List<Dictionary<string, string>> GetExportReportSummaryView(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetExportReportSummaryView(objReportFilter);
        }
        public List<Dictionary<string, string>> GetExportReportSummaryViewCdb(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetExportReportSummaryViewCdb(objReportFilter);
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewAdditional(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetExportReportSummaryViewAdditional(objReportFilter);
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewNew(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
		{
			return new DALayer().GetExportReportSummaryViewNew(objReportFilter, layerName);
		}
        public List<Dictionary<string, string>> GetAuditLogReportSummaryView(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetAuditLogReportSummaryView(objReportFilter, layerName);
        }
        public List<Dictionary<string, string>> GetExportReportSummaryViewNewCdb(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetExportReportSummaryViewNewCdb(objReportFilter, layerName);
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewNewAdditional(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetExportReportSummaryViewNewAdditional(objReportFilter, layerName);
        }
        public List<Dictionary<string, string>> GetExportReportSummaryViewCSV(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
		{
			return new DALayer().GetExportReportSummaryViewCSV(objReportFilter, layerName);
		}
        public List<Dictionary<string, string>> GetSplitReportSummaryViewAllExcel(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetSplitReportSummaryViewAllExcel(objReportFilter, layerName);
        }
        public List<Dictionary<string, string>> GetSplitReportSummaryViewAllCSV(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetSplitReportSummaryViewAllCSV(objReportFilter, layerName);
        }
        public List<Dictionary<string, string>> GetSplitReportSummaryViewAllShape(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetSplitReportSummaryViewAllShape(objReportFilter, layerName);
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewCSVCdb(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetExportReportSummaryViewCSVCdb(objReportFilter, layerName);
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewCSVAdditional(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetExportReportSummaryViewCSVAdditional(objReportFilter, layerName);
        }


        public List<WebGridColumns> GetEntityWiseColumns(int entity_id, string entity_name, string setting_type, int user_id, int role_id)
        {
            return new DALayer().GetEntityWiseColumns(entity_id, entity_name, setting_type, user_id, role_id);
        }
        public string getCircleBoundary(double radius, string centelLatLong)
        {
            return new DALayer().getCircleBoundary(radius, centelLatLong);
        }
        public List<WebGridColumns> GetLandbaseEntityWiseColumns(int entity_id)
        {
            return new DALayer().GetLandbaseEntityWiseColumns(entity_id);
        }
        public List<WebGridColumns> GetLandBaseLayerWiseColumns(int entity_id, string layer_name, string setting_type, int user_id, int role_id)
        {
            return new DALayer().GetLandBaseLayerWiseColumns(entity_id, layer_name, setting_type, user_id, role_id);
        }

        public List<Dictionary<string, string>> GetExportBarcodeBulkSummaryViewData(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetExportBarcodeBulkSummaryViewData(objReportFilter);
        }
        public List<Dictionary<string, string>> GetExportSummaryViewKML(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetExportSummaryViewKML(objReportFilter);
        }
		public List<Dictionary<string, string>> GetExportSummaryViewKMLNew(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
		{
			return new DALayer().GetExportSummaryViewKMLNew(objReportFilter, layerName);
		}
		public List<Dictionary<string, string>> GetBuildingStatusHistory(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetBuildingStatusHistory(objReportFilter);
        }

        public List<Dictionary<string, string>> GetFaultStatusHistory(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetFaultStatusHistory(objReportFilter);
        }
        public List<UtilizationSummaryReport> GetUtilizationReportSummary(UtilizationFilter objReportFilter)
        {
            return new DALayer().GetUtilizationReportSummary(objReportFilter);
        }
        public List<Dictionary<string, string>> GetUtilizationReportSummaryView(UtilizationEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetUtilizationReportSummaryView(objReportFilter);
        }
        public List<Dictionary<string, string>> GetUtilizationSummaryViewKMLShape(UtilizationEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetUtilizationSummaryViewKMLShape(objReportFilter);
        }
        public string ShowUtilizationOnMap(UtilizationEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().ShowUtilizationOnMap(objReportFilter);
        }
        public List<layerDetail> GetActiveLayers()
        {
            return new DALayer().GetActiveLayers();
        }
        //------shazia 
        public bool GetSpecificationAllowed(string entityType)
        {
            return new DALayer().GetSpecificationAllowed(entityType);
        }
        //---------------end-----------------------------

        #region For Additional Attributes
        public layerDetail GetLayerDetailsbyID(int layerID)
        {
            return objDALayer.GetLayerDetailsbyID(layerID);
        }

        public List<layerDetail> GetAdditionalAttributesLayers()
        {
            return objDALayer.GetAdditionalAttributesLayers();
        }
        #endregion

        public void UpdateRemarks(int? system_id, string entity_type, string networkId, string remark)
        {
            new DALayer().UpdateRemarks(system_id, entity_type, networkId, remark);
        }

        public List<SubDistrict> GetSubdistrictByProvinceId(string stateid)
        {
            return objDALayer.GetSubdistrictByProvinceId(stateid);
        }
        public List<Block> GetBlockBySubDistrictId(string subdistrictid)
        {
            return objDALayer.GetBlockBySubDistrictId(subdistrictid);
        }
        public List<RouteInfo> getRouteInfo(string province_ids)
        {
            return objDALayer.getRouteInfo(province_ids);
        }


        //Association Report

        public List<EntitySummaryReport> GetAssociationReportSummary(AssociationReportFilter objReportFilter)
        {
            return new DALayer().GetAssociationReportSummary(objReportFilter);
        }

        public List<layerReportDetail> GetAssociationReportLayers(int roleId, string purpose)
        {
            return objDALayer.GetAssociationReportLayers(roleId, purpose);
        }

        public List<Dictionary<string, string>> GetAssociationReportSummaryView(AssociationEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetAssociationReportSummaryView(objReportFilter);
        }

        public List<Dictionary<string, string>> GetAssociationReportSummaryViewCSV(AssociationEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetAssociationReportSummaryViewCSV(objReportFilter, layerName);
        }

        public List<Dictionary<string, string>> GetAssociationReportSummaryView(AssociationEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetAssociationReportSummaryView(objReportFilter, layerName);
        }

        public List<Dictionary<string, string>> GetAssociationSummaryViewKML(AssociationEntitiesSummaryViewFilter objReportFilter)
        {
            return new DALayer().GetAssociationSummaryViewKML(objReportFilter);
        }
        public List<Dictionary<string, string>> GetAssociationSummaryViewKMLNew(AssociationEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            return new DALayer().GetAssociationSummaryViewKMLNew(objReportFilter, layerName);
        }
        public List<DropDownMaster> GetDropDownList(string doctype)
        {
            return new DALayer().GetDropDownList(doctype);
        }
        // End Association Report

    }
}
