using BusinessLogics.DaFiFeasibilityAPI;
using DataAccess;
using DataAccess.Admin;
using DataAccess.ISP;
using Models;
using Models.Admin;
using Models.API;
using Models.DaFiFeasibilityAPI;
using Models.ISP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.GoogleMapAPI;
using static Models.RouteAssetDeatils;
//using static Models.GoogleMapAPI;

namespace BusinessLogics
{
    public class BLMisc
    {
        DAMisc objDAMisc = new DAMisc();
        DADropDownItems objDADropDownItems = new DADropDownItems();
        //public static List<Models.EmailSettingsModel> EmailSettings { get; set; }
        public List<MicrowaveLinkViewModel> GetMicroWaveLinkAssociatedElements(int systemId)
        {
            return new DAMisc().GetMicroWaveLinkAssociatedElements(systemId);
        }
        public string Getnetworkid(string systemid)
        {
            return new FMSDAMisce().Getnetworkid(Convert.ToInt32(systemid)).ToString();
        }
        public List<ViewEntityNotifications> GetEntityNotificationList(EntityNotificationsFilter objEntityNotiFilter)
        {
            return new DAMiscNoti().GetEntityNotificationList(objEntityNotiFilter);
        }
        public List<ViwUtilizationSetting> GetUtilizationSettingsList(ViewEntityUtilizationSettingsFilter objEntityUtiliSettingsFilter)
        {
            return new DAMiscellaneous().GetUtilizationSettingsList(objEntityUtiliSettingsFilter);
        }

        //public List<ViewDropdownMasterSetting> GetDropdownMasterGridList(DropdownMasterSettingsFilter objDropdownMasterSettingsFilter)
        //{
        //    return new DAMiscellaneous().GetDropdownMasterGridList(objDropdownMasterSettingsFilter);
        //}
        public List<DropDownMaster> GetDropDownList(string enType, string ddType = "")
        {
            return objDAMisc.GetDropDownList(enType, ddType);
        }

        public List<DropDownMaster> GetAssociationDropDownList(string enType, string ddType = "")
        {
            return objDAMisc.GetAssociationDropDownList(enType, ddType);
        }
        public List<DropDownMaster> GetPortRatio(string splitterType)
        {
            return objDAMisc.GetPortRatio(splitterType);
        }
        public List<DropDownMaster> GetDropDownListJson(string enType, string ddType = "")
        {
            return objDAMisc.GetDropDownListJson(enType, ddType);
        }

        public List<DropDownMaster> GetDropDownListParent(string layerName, string ddType = "")
        {
            return objDAMisc.GetDropDownListParent(layerName, ddType);
        }
        public List<EntityDetail> getNearByEntities(double latitude, double longitude, int bufferInMtr, string source_ref_id, string source_ref_type, int user_id = 0)
        {
            return objDAMisc.getNearByEntities(latitude, longitude, bufferInMtr, source_ref_id, source_ref_type, user_id);

        }

        public List<bool> validateTopologyEntity(double latitude, double longitude, string geom,int user_id = 0)
        {
            return objDAMisc.validateTopologyEntity(latitude, longitude, geom, user_id);

        }
        public List<EntityDetail> GetNearByTopologyEntity(double latitude, double longitude, int bufferInMtr, string source_ref_id, string source_ref_type, int user_id = 0)
        {
            return objDAMisc.GetNearByTopologyEntity(latitude, longitude, bufferInMtr, source_ref_id, source_ref_type, user_id);

        }
        public List<EntityDetail> getNearByFeasibility(double latitude, double longitude, int bufferInMtr)
        {
            return objDAMisc.getNearByFeasibility(latitude, longitude, bufferInMtr);

        }
        public List<RouteBuffer> getRouteBufferFeasibility(string coordinates, int route_buffer)
        {
            return objDAMisc.getRouteBufferFeasibility(coordinates,route_buffer);

        }
        public List<Points> getStartEndPointsFeasibility(string coordinates)
        {
            return objDAMisc.getStartEndPointsFeasibility(coordinates);

        }
        public List<EntityDetailWithAttribute> GetNearByEntitiesWithAttribute(double latitude, double longitude, int bufferInMtr, int ticket_id, int user_id = 0)
        {
            return objDAMisc.GetNearByEntitiesWithAttribute(latitude, longitude, bufferInMtr,ticket_id, user_id);

        }
        public bool ComputeHomePass(int system_Id, string entity_name)
        {
            return objDAMisc.ComputeHomePass(system_Id, entity_name);

        }
        public List<nearByNetworkEntities> getNearByNetworkEntities(double latitude, double longitude, string buildingCode, string buildingName, string entityTypes, int bufferInMtr)
        {
            return objDAMisc.getNearByNetworkEntities(latitude, longitude, buildingCode, buildingName, entityTypes, bufferInMtr);

        }
        public List<nearByEntityDetails> GetNearbyEntityDetailsWithAtributes(double latitude, double longitude, int bufferInMtr, int user_id, int batch_size, int last_record_number, int pageno, int pagerecord, string layer_name)
        {
            return objDAMisc.GetNearbyEntityDetailsWithAtributes(latitude, longitude, bufferInMtr, user_id, batch_size, last_record_number, pageno, pagerecord, layer_name);

        }
        public int GetNearbyEntityDetailsWithAtributesCount(double latitude, double longitude, int bufferInMtr, int user_id, string layer_name)
        {
            return objDAMisc.GetNearbyEntityDetailsWithAtributesCount(latitude, longitude, bufferInMtr, user_id, layer_name);

        }

        //public List<EntityDetail> getNearByEntities_NEW(double latitude, double longitude, int bufferInMtr, int role_id)
        //{
        //    return objDAMisc.getNearByEntities_NEW(latitude, longitude, bufferInMtr, role_id);

        //}

        public List<EntityDetail> getEntitiesNearbyFault(double latitude, double longitude)
        {
            return objDAMisc.getEntitiesNearbyFault(latitude, longitude);
        }
        public List<EntityDetail> getNearByCables(int systemId, string entityType)
        {
            return objDAMisc.getNearByCables(systemId, entityType);
        }       
        public List<EntityDetail> getNearByDucts(int systemId, string entityType)
        {
            return objDAMisc.getNearByDucts(systemId, entityType);
        }
        public List<EntityDetail> getNearByMicroducts(int systemId, string entityType)
        {
            return objDAMisc.getNearByMicroducts(systemId, entityType);
        }
        public List<EntityDetail> getNearByTrenchs(int systemId, string entityType)
        {
            return objDAMisc.getNearByTrenchs(systemId, entityType);
        }
        public List<EntityDetail> getNearByConduits(int systemId, string entityType)
        {
            return objDAMisc.getNearByConduits(systemId, entityType);
        }
        public SplitCableEntity getSplitCableEntity(int splitEntitySystemId, string splitEntityType, int cableId, string entity_type)
        {
            return objDAMisc.getSplitCableEntity(splitEntitySystemId, splitEntityType, cableId, entity_type);
        }
        public SplitDuctEntity getSplitDuctEntity(int splitEntitySystemId, string splitEntityType, string splitEnityNetworkId, int ductId, string entity_type)
        {
            return objDAMisc.getSplitDuctEntity(splitEntitySystemId, splitEntityType, splitEnityNetworkId, ductId, entity_type);
        }
        public SplitMicroductEntity getSplitMicroductEntity(int splitEntitySystemId, string splitEntityType, string splitEnityNetworkId, int microductId, string entity_type)
        {
            return objDAMisc.getSplitMicroductEntity(splitEntitySystemId, splitEntityType, splitEnityNetworkId, microductId, entity_type);
        }
        public SplitTrenchEntity getSplitTrenchEntity(int splitEntitySystemId, string splitEntityType, string splitEnityNetworkId, int trenchId, string entity_type)
        {
            return objDAMisc.getSplitTrenchEntity(splitEntitySystemId, splitEntityType, splitEnityNetworkId, trenchId, entity_type);
        }

        public SplitConduitEntity getSplitConduitEntity(int splitEntitySystemId, string splitEntityType, string splitEnityNetworkId, int conduitId, string entity_type)
        {
            return objDAMisc.getSplitConduitEntity(splitEntitySystemId, splitEntityType, splitEnityNetworkId, conduitId, entity_type);
        }
        public Entitygeom GetFaultEntityGeomInfo(int entity_SystemId, string entityType, double sourceEntityLat, double sourceEntityLong)
        {
            return objDAMisc.getEntityGeomInfo(entity_SystemId, entityType, sourceEntityLat, sourceEntityLong);
        }
        //public List<EntityDetail> GetNearEntitiesByEntityName(double latitude, double longitude, int bufferInMtr, string entity_name)
        //{
        //    return objDAMisc.GetNearEntitiesByEntityName(latitude, longitude, bufferInMtr, entity_name);
        //}



        public List<EntityDetail> GetNearEntitiesByEntityType(int c_system_id, string c_entity_type, int bufferInMtr, string entity_type)
        {
            return objDAMisc.GetNearEntitiesByEntityType(c_system_id, c_entity_type, bufferInMtr, entity_type);
        }

        public List<entityInfo> getEntityInfo(int systemId, string entityType, string geomType, int userId)
        {
            return objDAMisc.getEntityInfo(systemId, entityType, geomType, userId);
        }
        public Dictionary<string, string> getMobileEntityInfo(int systemId, string entityType, string geomType)
        {
            return objDAMisc.getMobileEntityInfo(systemId, entityType, geomType);
        }
        public Dictionary<string, string> getSiteCustomerDetails(string searchText, string columnName, string lmcType)
        {
            return objDAMisc.getSiteCustomerDetails(searchText, columnName, lmcType);
        }
        public Dictionary<string, string> getFiberLinkDetails(string searchText, string columnName)
        {
            return objDAMisc.getFiberLinkDetails(searchText, columnName);
        }

        public Dictionary<string, string> getEntityAdvanceAttribute(int systemId, string entityType, string geomType)
        {
            return objDAMisc.getEntityAdvanceAttribute(systemId, entityType, geomType);
        }

        //public List<LayerActionMapping> getEntityActionInfo(string layerName)
        //{
        //    return objDAMisc.getEntityActionInfo(layerName);
        //}

        public List<LayerActionMapping> getLayerActions(int systemId, string layerName, bool isOSP_Type, string network_status, int role_id, int user_id, bool isMobileAction, string source_ref_id, string source_ref_type)
        {
            return objDAMisc.getLayerActions(systemId, layerName, isOSP_Type, network_status, role_id, user_id, isMobileAction, source_ref_id, source_ref_type);
        }

        public List<MobileRegionProvince> getRegionProvince(int user_id)
        {
            return objDAMisc.getRegionProvince(user_id);
        }

        public List<CrmTicketInfo> getTicketInfo(int userid)
        {
            return objDAMisc.getTicketInfo(userid);
        }
        public List<StaticPageMasterInfo> getStaticPageDetails(string name)
        {
            return objDAMisc.getStaticPageDetails(name);
        }

        public List<ticketStepDetails> getTicketStepDetails(int ticket_id, int ticket_type_id, string rfs_type)
        {
            return objDAMisc.getTicketStepDetails(ticket_id, ticket_type_id, rfs_type);
        }
        public List<StructureEntityView> getShaftNFloor(int system_id)
        {
            return DAISP.Instance.getShaftNFloor(system_id);
        }
        public List<StructureElement> GetStructureElementInfo(int system_id, string associated_entity_types, string can_id, bool isOnlyServingDb, string rfs_type, string module_abbr)
        {
            return objDAMisc.GetStructureElementInfo(system_id, associated_entity_types, can_id, isOnlyServingDb, rfs_type, module_abbr);
        }

        public List<ChildElementDetail> getDependentChildElements(ImpactDetailIn objImpactDetailIn)
        {
            return objDAMisc.getDependentChildElements(objImpactDetailIn);
        }

        public bool chkEntityIsMiddleWare(string entityType)
        {
            return objDAMisc.chkEntityIsMiddleWare(entityType);
        }        
        public T GetEntityDetailById<T>(int systemid, EntityType eType, int userId = 0) where T : new()
        {
            return objDAMisc.GetEntityDetailById<T>(systemid, eType, userId);
        }
		public T GetNetworkTicketIdByEntityId<T>(int systemid, EntityType eType) where T : new()
		{
			return objDAMisc.GetNetworkTicketIdByEntityId<T>(systemid, eType);
		}
		public BarCode GetEntityDetailForBarCode(int systemid, string eType)
        {
            return objDAMisc.GetEntityDetailForBarCode(systemid, eType);
        }

        public NetworkDtl GetNetworkDetails(string geom, string geomType, string entityType)
        {
            return new DAMisc().GetNetworkDetails(geom, geomType, entityType);
        }

        public NetworkCodeDetail GetNetworkCodeDetail(NetworkCodeIn objIn)
        {
            return new DAMisc().GetNetworkCodeDetail(objIn);
        }

        public EntityCodeDetail GetEntityNetworkCodeDetail(NetworkCodeIn objIn)
        {
            return new DAMisc().GetEntityNetworkCodeDetail(objIn);
        }
        public ISPNetworkCodeDetail GetISPNetworkCodeDetail(ISPNetworkCodeIn objIn)
        {
            return new DAMisc().GetISPNetworkCodeDetail(objIn);
        }

        public ISPNetworkCodeDetail GetISPModelNetworkCodeDetail(ISPNetworkCodeIn objIn)
        {
            return new DAMisc().GetISPModelNetworkCodeDetail(objIn);
        }
        public double GetCableLength(string geom)
        {
            return objDAMisc.GetCableLength(geom);
        }
        public List<startendpoint> GetEndPoints(string geom)
        {
            return objDAMisc.getEndPoints(geom);
        }

        public List<TerminationPointDtl> GetTerminationDetail(string geom, int buffer, string entityType, int userId)
        {
            return new DAMisc().GetTerminationDetail(geom, buffer, entityType, userId);
        }
        public bool chkNetworkIdExist(string networkId, string entityType, string networkStage)
        {
            return new DAMisc().chkNetworkIdExist(networkId, entityType, networkStage);
        }
        public List<T> GetEntityExportData<T>(int systemid, string eType, string networkStage) where T : new()
        {
            return objDAMisc.GetEntityExportData<T>(systemid, eType, networkStage);
        }
        public List<T> ExportChildEntityByParentCode<T>(int parentSystemId, string parentNetworkId, string parentEntityType, string childEntityType) where T : new()
        {
            return objDAMisc.ExportChildEntityByParentCode<T>(parentSystemId, parentNetworkId, parentEntityType, childEntityType);
        }
        public List<T> GetEntityReferenceExportData<T>(int systemid, string eType, string networkStage) where T : new()
        {
            return objDAMisc.GetEntityReferenceExportData<T>(systemid, eType, networkStage);
        }
        public List<T> GetSpliceTrayExportData<T>(int systemid, string eType, string networkStage) where T : new()
        {
            return objDAMisc.GetSpliceTrayExportData<T>(systemid, eType, networkStage);
        }
        //public List<T> GetEntityLabel<T>(int systemid, string eType) where T : new()
        //{
        //    return objDAMisc.GetEntityLabel<T>(systemid, eType);
        //}
        public List<EntityCount> GetEntityLabel(int systemid, string eType)
        {
            return objDAMisc.GetEntityLabel(systemid, eType);
        }

        //public List<T> GetEntityExportForBom<T>(string geom, string network_status, string entity_type, double p_radius, string p_type, string provinceids, string regionids) where T : new()
        //{
        //    return objDAMisc.GetEntityExportForBom<T>(geom, network_status, entity_type, p_radius, p_type, provinceids, regionids);
        //}

        public bool chkEntityDataExist(int systemid, string entityType, string networkStage)
        {
            return objDAMisc.chkEntityDataExist(systemid, entityType, networkStage);
        }
        public NetworkCodeDetail GetLineNetworkCode(string start_network_id, string end_network_id, string enName, string geometry, string CableType, int pSystemId = 0, string pEntityType = "")
        {
            return new DAMisc().GetLineNetworkCode(start_network_id, end_network_id, enName, geometry, CableType, pSystemId, pEntityType);
        }

        public string GetEntityName(string network_name, int system_id)
        {
            return new DAMisc().GetEntityName(network_name, system_id);
        }
        public DbMessage SaveCloneEntity(int refId, string entityName, string geomType, string geom, int userId)
        {
            return new DAMisc().SaveCloneEntity(refId, entityName, geomType, geom, userId);
        }
        public DbMessage AssociateFiberLinkWithCable(int link_system_id, int cable_id, int fiber_no, string action)
        {
            return new DAMisc().AssociateFiberLinkWithCable(link_system_id, cable_id, fiber_no, action);
        }

        public DbMessage SaveBulkCloneEntity(ViewCloneDependent objCloneDependen, int userid)
        {
            return new DAMisc().SaveBulkCloneEntity(objCloneDependen, userid);
        }
        public int SaveGroupLibrary(ViewGroupLibrary objGroupLibrary)
        {
            return new DAGroupLibrary().SaveGroupLibrary(objGroupLibrary);
        }
        public List<EntityLstCount> GetEntityLstByGeom(string geom, int userId, string geomType, double buff_Radius)
        {
            return new DAMisc().GetEntityLstByGeom(geom, userId, geomType, buff_Radius);
        }
        public List<GroupLibrary> SaveGroupLibraryEntity(List<Properties> lstProperties)
        {
            return new DAGroupLibrary().SaveGroupLibraryEntity(lstProperties);
        }
        //public DbMessage SaveCablegroupLibrary(List<GLLineDetails> lstGroupLineTP, List<TerminationPoint> lstGLTerminationPoint, int user_id)
        //{
        //    return new DAGroupLibrary().SaveCablegroupLibrary(lstGroupLineTP, lstGLTerminationPoint, user_id);
        //}

        public PageMessage SaveDuctgroupLibrary(string geom, int Id, int user_id, List<TerminationPoint> lstTerminationPoint, dynamic entity_date, string entity_type)
        {
            return new DAGroupLibrary().SaveDuctgroupLibrary(geom, Id, user_id, lstTerminationPoint, entity_date, entity_type);
        }
        public PageMessage SaveTrenchgroupLibrary(string geom, int Id, int user_id, List<TerminationPoint> lstTerminationPoint, dynamic entity_date, string entity_type)
        {
            return new DAGroupLibrary().SaveTrenchgroupLibrary(geom, Id, user_id, lstTerminationPoint, entity_date, entity_type);
        }
        public string DeleteGroupLibraryById(int Id)
        {
            return new DAGroupLibrary().DeleteGroupLibraryById(Id);
        }
        public List<GroupLibraryDetails> GetGroupLibraryByid(int id, int userId)
        {
            return new DAGroupLibrary().GetGroupLibraryByid(id, userId);
        }
        public ViewGroupLibrary GetLineGroupLibraryByid(int id)
        {
            return new DAGroupLibrary().GetLineGroupLibraryByid(id);
        }
        public List<EntityLstCount> GeBulkOperationLstCount(BulkAsBuiltFilterAttribute objfilter)
        {
            return new DAMisc().GeBulkOperationLstCount(objfilter);
        }

        public DbMessage ValidateEntityByGeom(string geom, int userId, string geomType, double buff_Radius)
        {
            return new DAMisc().ValidateEntityByGeom(geom, userId, geomType, buff_Radius);
        }

        public DbMessage ValidateEntityCreationArea(string geom, int userId, string geomType, int ticket_id)
        {
            return new DAMisc().ValidateEntityCreationArea(geom, userId, geomType, ticket_id);
        }
        public DbMessage ValidateLBEntityCreationArea(string geom, int userId, string geomType, int ticket_id)
        {
            return new DAMisc().ValidateLBEntityCreationArea(geom, userId, geomType, ticket_id);
        }
        public DbMessage ValidatePotentialArea(string geom, int userId, string geomType, double buff_Radius)
        {
            return new DAMisc().ValidatePotentialArea(geom, userId, geomType, buff_Radius);
        }
        public DbMessage ValidateLMCPotentialArea(string geom, int userId, string geomType, double buff_Radius)
        {
            return new DAMisc().ValidateLMCPotentialArea(geom, userId, geomType, buff_Radius);
        }

        public DbMessage ValidateVSATPotentialArea(string geom, int userId, string geomType, double buff_Radius)
        {
            return new DAVSAT().ValidateVSATPotentialArea(geom, userId, geomType, buff_Radius);
        }
        public DbMessage SaveBulkProjSpecific(BulkProjSpecific objProjSpec)
        {
            return new DAMisc().SaveBulkProjSpecific(objProjSpec);
        }
        public DbMessage SaveBulkPodAssociation(BulkPodAssociation objPodAssctn)
        {
            return new DAMisc().SaveBulkPodAssociation(objPodAssctn);
        }
        public List<bulkDeleteOperation> BulkDeleteProcess(BulkDelete objBulkDelete)
        {
            return new DAMisc().BulkDeleteProcess(objBulkDelete);
        }
        public bool InsertPortInfo(int inputPort, int OutPutPort, string entityType, int systemId, string networkId, int userId)
        {
            return new DAMisc().InsertPortInfo(inputPort, OutPutPort, entityType, systemId, networkId, userId);
        }
        public DbMessage isPortConnected(int systemId, string entityType, string specification, int vendor_id, string item_code)
        {
            return new DAMisc().isPortConnected(systemId, entityType, specification, vendor_id, item_code);
        }
        public List<IspPortInfo> GetPortInfo(int system_id, string entity_type)
        {
            return new DASplitterAllocationPortInfo().GetPortInfo(system_id, entity_type);
        }
        public DistributionBoxInfo GetDistributionBoxInfo(int user_id, double latitude, double longitude, string box_type)
        {
            return new DASplitterAllocationPortInfo().GetDistributionBoxInfo(user_id, latitude, longitude, box_type);
        }
        public List<DBInfo> GetNearByDBInfo(string latitude, string longitude, string box_type, string rfs_type, int bufferInMtr, string module_abbr)
        {
            return new DASplitterAllocationPortInfo().GetNearByDBInfo(latitude, longitude, box_type, rfs_type, bufferInMtr, module_abbr);
        }
        public List<UserModule> GetUserModule(int userId, string UserType)
        {
            return new DASplitterAllocationPortInfo().GetUserModule(userId, UserType);
        }

        public List<UserModule> GetRoleModule(int role_id)
        {
            return new DASplitterAllocationPortInfo().GetRoleModule(role_id);
        }
        public List<UserModule> GetAllModules()
        {
            return new DASplitterAllocationPortInfo().GetAllModules();
        }

        public List<UserModule> GetUserModuleMasterList()
        {
            return new DASplitterAllocationPortInfo().GetUserModuleMasterList();
        }

        public DbMessage UpdateEntityBarCode(int? system_id, string entity_type, string barcode, string network_id)
        {
            return new DAMisc().UpdateEntityBarCode(system_id, entity_type, barcode, network_id);
        }
		public DbMessage UpdatePowerMeterReading(int? system_id, string entity_type, double? power_meter_reading, bool is_manual_meter_reading)
		{
			return new DAMisc().UpdatePowerMeterReading(system_id, entity_type, power_meter_reading, is_manual_meter_reading);
		}

		public DistributionBoxEntityInfo GetDistributionBoxEntityInfo(int user_id, int system_id, string entity_type)
        {
            return new DASplitterAllocationPortInfo().GetDistributionBoxEntityInfo(user_id, system_id, entity_type);
        }

        public SplitterPortInfo GetSplitterPortInfo(int user_id, int system_id, string entity_type, string can_id)
        {
            return new DASplitterAllocationPortInfo().GetSplitterPortInfo(user_id, system_id, entity_type, can_id);
        }

        public int UpdateSplitterAllocationPort(List<IspPortInfo> PortInfo)
        {
            return new DASplitterAllocationPortInfo().UpdateSplitterAllocationPort(PortInfo);

        }
        public IspPortInfo SaveSplitterCustomerMapping(int parent_system_id, int parent_port_number, string parent_entity_type, int destination_system_id, int user_id, string destination_network_id, string destination_entity_type)
        {
            return new DASplitterAllocationPortInfo().SaveSplitterCustomerMapping(parent_system_id, parent_port_number, parent_entity_type, destination_system_id, user_id, destination_network_id, destination_entity_type);

        }

        public DbMessage SaveCustomerAssociation(string destination_network_id, int p_box_id, string p_box_type, int source_system_id, int source_port_number, int user_id, string jsonlstWCRMatrial, string rfs_type, bool is_box_changed, int ticket_id, string routeGeom)
        {
            return new DASplitterAllocationPortInfo().SaveCustomerAssociation(destination_network_id, p_box_id, p_box_type, source_system_id, source_port_number, user_id, jsonlstWCRMatrial, rfs_type, is_box_changed, ticket_id, routeGeom);

        }
        public CPEInstallation SaveCPEInstallation(string source_network_id, string source_entity_type, int source_port_number, int user_id, string latitude, string longitude)
        {
            return new DASplitterAllocationPortInfo().SaveCPEInstallation(source_network_id, source_entity_type, source_port_number, user_id, latitude, longitude);

        }


        public List<BufferEntity> getEntityInBuffer(int systemId, string entityType, string pEntityType, string pgeomType)
        {
            return new DAMisc().getEntityInBuffer(systemId, entityType, pEntityType, pgeomType);
        }
        public List<LineEntityInfo> getLineEntityInLineBuffer(int systemId, string entityType, int pSystemId, string pEntityType, string pParentGeom, string pParentGeomType)
        {
            return new DAMisc().getLineEntityInLineBuffer(systemId, entityType, pSystemId, pEntityType, pParentGeom, pParentGeomType);
        }
        public List<RouteInfo> getRouteEntityInLineBuffer(int systemId, string entityType)
        {
            return new DAMisc().getRouteEntityInLineBuffer(systemId, entityType);
        }
        public List<RouteInfo> getRouteEntityInLineBuffer(string geom)
        {
            return new DAMisc().getRouteEntityInLineBuffer(geom);
        }
        public List<LineEntityInfo> getAutoLineEntityInLineBuffer(int systemId, string entityType, int pSystemId, string pEntityType, string pParentGeom, string pParentGeomType)
        {
            return new DAMisc().getAutoLineEntityInLineBuffer(systemId, entityType, pSystemId, pEntityType, pParentGeom, pParentGeomType);
        }
        public List<NearByAssociatedEntities> getnearbyassociatedentities(string entityType, string geom)
        {
            return new DAMisc().getnearbyassociatedentities(entityType, geom);
        }
        public List<_RegionProvince> GetProvinceDetail(int province_id)
        {
            return new DAMisc().GetProvinceDetail(province_id);
        }
        public DbMessage checkIsBuried(int systemId, string entityType)
        {
            return new DAMisc().checkIsBuried(systemId, entityType);
        }
        public List<LineEntityInfo> viewEntityAssociation(int pSystemId, string pEntityType)
        {
            return new DAMisc().viewEntityAssociation(pSystemId, pEntityType);
        }
        public DbMessage saveLineEntityAssocition(string lineEnAssociteInfo, int pSystemId, string pEntityType, int userId,int pManholeCount)
        {
            return new DAMisc().saveLineEntityAssocition(lineEnAssociteInfo, pSystemId, pEntityType, userId, pManholeCount);
        }
        public DbMessage saveRouteAssocition(string routeAssociteInfo, int pSystemId, string pEntityType, int userId)
        {
            return new DAMisc().saveRouteAssocition(routeAssociteInfo, pSystemId, pEntityType, userId);
        }

        public DbMessage saveLineEntityAutoAssocition(int pSystemId, string pEntityType, int userId)
        {
            return new DAMisc().saveLineEntityAutoAssocition(pSystemId, pEntityType, userId);
        }
        public DbMessage validateBulkAssosiation(int pSystemId, string pEntityType, int userId)
        {
            return new DAMisc().validateBulkAssosiation(pSystemId, pEntityType, userId);
        }        
        public AssociateEntity SaveAssociation(AssociateEntity objAssociateEnt)
        {
            return new DAAssociateEntity().SaveAssociation(objAssociateEnt);
        }
        public List<AssociateEntity> getAssociateEntity(int system_id, string entityType)
        {
            return new DAAssociateEntity().getAssociateEntity(system_id, entityType);
        }
        public List<T> GetConnectionExportData<T>(int sourceId, int connectorId, int destinationId, string connectorEntityType) where T : new()
        {
            return objDAMisc.GetConnectionExportData<T>(sourceId, connectorId, destinationId, connectorEntityType);
        }

        public List<ExportConnectionReport> GetConnectionExportData(ExportConnectionRequest objExportConnection)
        {
            return objDAMisc.GetConnectionExportData(objExportConnection);
        }



        public List<T> GetAssociationExportData<T>(int systemId, string entityType) where T : new()
        {
            return objDAMisc.GetAssociationExportData<T>(systemId, entityType);
        }
        public List<T> GetRouteAssociationExportData<T>(int systemId, string entityType) where T : new()
        {
            return objDAMisc.GetRouteAssociationExportData<T>(systemId, entityType);
        }
        public List<T> DownloadBulkAssociationLog<T>(int systemId, int userId) where T : new()
        {
            return objDAMisc.DownloadBulkAssociationLog<T>(systemId, userId);
        }
        public bool BulkAssociationRequestLog(int systemId) 
        {
            return objDAMisc.BulkAssociationRequestLog(systemId);
        }
        public List<T> GetROWAssociationExportData<T>(int systemId, string entityType) where T : new()
        {
            return objDAMisc.GetROWAssociationExportData<T>(systemId, entityType);
        }
        public List<LegendDetail> GetLegendDetail(int user_id, int role_Id)
        {
            return objDAMisc.GetLegendDetail(user_id, role_Id);
        }

        public List<LegendDetail> GetMobileLegendDetail(int user_id)
        {
            return objDAMisc.GetMobileLegendDetail(user_id);
        }

        public List<LegendDetail> GetLegendByGeom(string geom, string filter, int role_id)
        {

            return objDAMisc.GetLegendByGeom(geom, filter, role_id);
        }

        //public List<LegendDetail> GetLegendDetail(int user_id, string geom, string selectiontype)
        //{
        //    return objDAMisc.GetLegendDetail(user_id, geom, selectiontype);
        //}
        public List<LegendDetail> GetMobileLegendDetail(int user_id, string group_name)
        {
            return objDAMisc.GetMobileLegendDetail(user_id, group_name);
        }
        public DbMessage EntityConversion(string aEntityType, string aEntityNetworkId, int aEntitySystemId, string bEntityType, string bEntityNetworkId, int bEntitySystemId, string geom, int userId)
        {
            return new DAMisc().EntityConversion(aEntityType, aEntityNetworkId, aEntitySystemId, bEntityType, bEntityNetworkId, bEntitySystemId, geom, userId);
        }
        public List<Dictionary<string, string>> GetAuditEntityDetailById(int systemid, string eType, int currentPage, int pageSize, string sort, string orderBy)
        {
            return objDAMisc.GetAuditEntityDetailById(systemid, eType, currentPage, pageSize, sort, orderBy);
        }
        public List<Dictionary<string, string>> GetAuditEntityGeometryDetail(int systemid, string eType, int currentPage, int pageSize, string sort, string orderBy)
        {
            return objDAMisc.GetAuditEntityGeometryDetail(systemid, eType, currentPage, pageSize, sort, orderBy);
        }

        public List<Dictionary<string, string>> GetAuditSiteInfoById(int siteId, string lmcType, int currentPage, int pageSize, string sort, string orderBy)
        {
            return objDAMisc.GetAuditSiteInfoById(siteId, lmcType, currentPage, pageSize, sort, orderBy);
        }
        public List<Dictionary<string, string>> GetAuditLMCInfoById(int LMCId, string lmcType, int currentPage, int pageSize, string sort, string orderBy)
        {
            return objDAMisc.GetAuditLMCInfoById(LMCId, lmcType, currentPage, pageSize, sort, orderBy);
        }
        public List<Dictionary<string, string>> GetAuditLandBaseInfoById(int id, int landBaseLayerId, int currentPage, int pageSize, string sort, string orderBy)
        {
            return objDAMisc.GetAuditLandBaseInfoById(id, landBaseLayerId, currentPage, pageSize, sort, orderBy);
        }
        public List<Dictionary<string, string>> GetFiberLinkHistoryById(int linkSystemId, int currentPage, int pageSize, string sort, string orderBy)
        {
            return objDAMisc.GetFiberLinkHistoryById(linkSystemId, currentPage, pageSize, sort, orderBy);
        }

        public List<AccessoriesAuditModel> GetAuditAccessoriesById(int accessoriesId, int currentPage, int pageSize, string sort, string orderBy)
        {
            return objDAMisc.GetAuditAccessoriesById(accessoriesId, currentPage, pageSize, sort, orderBy);
        }



        public DbMessage deleteEntity(int systemId, string entityType, string geomType, int userId)
        {
            return objDAMisc.deleteEntity(systemId, entityType, geomType, userId);
        }

        public DbMessage deleteSite(int siteId, int structureId)
        {
            return objDAMisc.deleteSite(siteId, structureId);
        }
        public DbMessage deleteLMC(int lmcId, int cableId)
        {
            return objDAMisc.deleteLMC(lmcId, cableId);
        }
        public DbMessage RevertEntityChanges(RevertEntity objRevertEntity)
        {
            return objDAMisc.RevertEntityChanges(objRevertEntity);
        }
        public EmailSettingsModel getEmailSettings()
        {
            return new DAMisce().getEmailSettings();
        }
        //public List<EmailSettingsModel> InitializeEmailSettings()
        //{
        //    EmailSettings = new DAMisce().getAllEmailData();
        //    return EmailSettings;
        //}

        public List<EmailSettingsModel> GetAllEmailSettings()
        {
            return new DAMisce().getAllEmailData();
        }

        public EmailSettingsModel SaveEmailSettings(EmailSettingsModel objUser, string EncodePassword, int userid)
        {
            return new DAMisce().SaveEmailSettings(objUser, EncodePassword, userid);
        }
        public string getEntityGeom(int systemId, string entityType)
        {
            return new DAMisc().getEntityGeom(systemId, entityType);
        }
        public string getEntitylatlong(int a_entity_system_id, int b_entity_system_id, string a_entity_type, string b_entity_type)
        {
            return new DAMisc().getEntitylatlong(a_entity_system_id, b_entity_system_id, a_entity_type, b_entity_type);
        }

        public DbMessage AssociateSplitEntities(int entity_one_system_id, int entity_two_system_id, string entity_one_network_id, string entity_two_network_id, string splitentitytype, int split_entity_system_id)
        {
            return new DAMisc().SetAssociationWithSplitentity(entity_one_system_id, entity_two_system_id, entity_one_network_id, entity_two_network_id, splitentitytype, split_entity_system_id);
        }
        public DbMessage deleteParentSplitEntity(int systemId, string entityType)
        {
            return objDAMisc.deleteParentSplitEntity(systemId, entityType);
        }

        public DbMessage validateEntity(validateEntity entityInfo, bool isNew)
        {
            return objDAMisc.validateEntity(entityInfo, isNew);
        }
        public DbMessage validateUnitsOnFloors(validateEntity entityInfo, bool isNew)
        {
            return objDAMisc.validateUnitsOnFloors(entityInfo, isNew);
        }

        public List<DropDownMaster> GetTicketDropdownList(string ddType, string enType = "")
        {
            return objDADropDownItems.GetTicketDropdownList(ddType, enType);
        }
        public List<attachment_type_master> getAttachment(string entityType, string category)
        {
            return new DAMisc().getAttachment(entityType, category);
        }
        public string getNewNetworkcode(string etype, string entity_network_id)
        {
            return new DAMisc().getNewNetworkcode(etype, entity_network_id);
        }
        public DbMessage SaveChangeNetworkCode(ChangeNetworkCode objparam)
        {
            return objDAMisc.SaveChangeNetworkCode(objparam);
        }
        public List<entityValidationStatus> getNetworkIdDependency(string etype, string old_network_id)
        {
            return objDAMisc.getNetworkIdDependency(etype, old_network_id);
        }
        public List<EntityLayerActions> getEntityLayerActions(int user_id)
        {
            return objDAMisc.getEntityLayerActions(user_id);
        }
        public int GetCount(EntityType entityType, UploadSummary summary)
        {
            return new DAMisc().GetCount(entityType, summary);
        }
        public int DeleteUtilizationSettingDetailById(int id)
        {
            return new DAMiscellaneous().DeleteUtilizationSettingDetailById(id);
        }
        public EntityNotifications UpdateNotificationCloseStatus(int notificationId, int user_id)
        {
            return new DAMiscNoti().UpdateNotificationCloseStatus(notificationId, user_id);
        }
        public EntityNotificationComments SaveEntityNotificationComment(EntityNotificationComments objNotificationComment)
        {
            return new DAMiscNotiComm().SaveEntityNotificationComment(objNotificationComment);
        }
        public List<ViewEntityNotificationComments> getEntityNotificationComment(int notificationId)
        {
            return new DAMiscNotiComm().getEntityNotificationComment(notificationId);
        }
        public List<EntityNotifications> GetEntityNotificationById(int notificationId)
        {
            return new DAMiscNoti().GetEntityNotificationById(notificationId);
        }
        public long GetUnreadNotificationCount(int userid, int roleid)
        {
            return new DAMiscNoti().GetUnreadNotificationCount(userid, roleid);
        }
        public EntityNotificationStatus SaveNotificationStatus(EntityNotificationStatus objNotifyStatus, int userId)
        {
            return new DANotificationStatus().SaveNotificationStatus(objNotifyStatus, userId);
        }
        public List<Dictionary<string, string>> getNotificationStatusHistory(int systemid, string eType, int currentPage, int pageSize, string sort, string orderBy)
        {
            return new DANotificationStatus().getNotificationStatusHistory(systemid, eType, currentPage, pageSize, sort, orderBy);
        }
        public bool getNotificationStatus(int systemId, string entityType)
        {
            return new DANotificationStatus().getNotificationStatus(systemId, entityType);
        }
        public List<getGroupLibrary> GetGroupLibrary(int user_id)
        {
            return new DAGroupLibrary().GetGroupLibrary(user_id);
        }
        public List<AccessoriesInfoModel> GetAccessoriesByParent(AccessoriesViewModel model)
        {
            return objDAMisc.GetAccessoriesByParent(model);
        }
        public List<AccessoriesReportModel> GetAccessoriesByParent(FilterAccessoriesAttr model)
        {
            return objDAMisc.GetAccessoriesByParent(model);
        }
        public ParentInfo getParentInfo(NetworkCodeIn objIn)
        {
            return new DAMisc().getParentInfo(objIn);
        }

        public bool insertApiLogs(Api_Logs objapilogs)
        {
            return new DAMisc().insertApiLogs(objapilogs);
        }
        public DbMessage SubmitNetwork(SubmitNetworkParam objSubmitNetwork)
        {
            return objDAMisc.SubmitNetwork(objSubmitNetwork);
        }
        public List<UserDashboard> GetTeamStatus(int manager_id)
        {
            return objDAMisc.GetTeamStatus(manager_id);
        }
        public DbMessage validateEntityGeom(string geomType, string entityType, string longlat, int userId, int parentId)
        {
            return new DAMisc().validateEntityGeom(geomType, entityType, longlat, userId, parentId);
        }
        public List<CloneDependent> GetCloneDependent(string entityType, int parentId)
        {
            return new DAMisc().GetCloneDependent(entityType, parentId);
        }
        public List<TerminationPointMaster> GetTPDetails(ViewTPMaster objTPMaster)
        {
            return new DATPMaster().GetTPDetails(objTPMaster);
        }

        public string SaveTerminationPoint(TerminationPointMaster objTerminationPoint)
        {
            return new DATPMaster().SaveTerminationPoint(objTerminationPoint);
        }
        public TerminationPointMaster GetTerminationPointById(int id)
        {
            return new DATPMaster().GetTerminationPointById(id);
        }
        public string DeleteTerminationPointById(int id)
        {
            return new DATPMaster().DeleteTerminationPointById(id);
        }
        public List<AssociateEntityMaster> GetAMDetails(ViewAEMaster objAEMaster)
        {
            return new DAAccMapping().GetAMDetails(objAEMaster);
        }
        public string SaveAssociateEntity(AssociateEntityMaster objAssociateEntity)
        {
            return new DAAccMapping().SaveAssociateEntity(objAssociateEntity);
        }
        public string DeleteAssociateEntityById(int id)
        {
            return new DAAccMapping().DeleteAssociateEntityById(id);
        }

        public List<TemplateColumn> GetTemplateColumn(ViewTemplateColumn objTemplateColumn)
        {
            return new DATemplateColumn().GetTemplateColumn(objTemplateColumn);
        }
        public List<TemplateColumnDropdown> GetTemplateColumnDropdown(int layer_id, string flag)
        {
            return new DATemplateColumn().GetTemplateColumnDropdown(layer_id, flag);
        }
        public string SaveTemplateColumn(TemplateColumn objTemplateColumn)
        {
            return new DATemplateColumn().SaveTemplateColumn(objTemplateColumn);
        }
        public TemplateColumn GetTemplateColumnById(int id)
        {
            return new DATemplateColumn().GetTemplateColumnById(id);
        }
        public string DeleteTemplateColumnById(int id)
        {
            return new DATemplateColumn().DeleteTemplateColumnById(id);
        }

        public List<portStatusMaster> GetportStatus(LogicalViewVM objportStatu)
        {
            return new DAPortStatus().GetportStatus(objportStatu);
        }
        public string SavePortStatus(portStatusMaster objportStatus)
        {
            return new DAPortStatus().SavePortStatus(objportStatus);
        }
        public DbMessage DeletePortStatusById(int id)
        {
            return new DAPortStatus().DeletePortStatusById(id);
        }
        public portStatusMaster GetPortStatusById(int id)
        {
            return new DAPortStatus().GetPortStatusById(id);
        }
        public bool CheckColorCode(string ColorCode, int system_id)
        {
            return new DAPortStatus().CheckColorCode(ColorCode, system_id);
        }
        public List<TPDropdown> TPDropdownList(int layer_id, string flage)
        {
            return new DATPMaster().TPDropdownList(layer_id, flage);
        }
        public string SaveVsat(VSATDetails ObjVsat, int parent_system_id)
        {
            return new DAVSAT().SaveVsat(ObjVsat, parent_system_id);
        }
        public VSATDetails GetVsatById(int id)
        {
            return new DAVSAT().GetVsatById(id);
        }
        public List<Dictionary<string, string>> getVSATReport(int userId, ExportVSATReportFilter objVSATReportFilter)
        {
            return new DAVSAT().getVSATReport(userId, objVSATReportFilter);
        }
        public string GetVsatColumnNameByDisplayName(string displayName)
        {
            return new DAVSAT().GetVsatColumnNameByDisplayName(displayName);
        }

        public LayerIconMapping saveLayerIcon(LayerIconMapping objLayerIcon)
        {
            return new DALayerIcon().saveLayerIcon(objLayerIcon);
        }
        public List<LayerIconMapping> GetLayerIcom(ViewLayerIcon objLayerIcon)
        {
            return new DALayerIcon().GetLayerIcom(objLayerIcon);
        }
        public List<DropDownMaster> GetEntityTypeDropdownList(int layer_id)
        {
            return objDAMisc.GetEntityTypeDropdownList(layer_id);
        }

        public List<ViewServiceFacilityJobOrder> GetRoleServiceFacilityJobOrder(int role_id)
        {
            return new DASplitterAllocationPortInfo().GetRoleServiceFacilityJobOrder(role_id);
        }
        public List<ViewServiceFacilityJobOrder> GetUserServiceFacilityJobOrder(int role_id, int user_id)
        {
            return new DASplitterAllocationPortInfo().GetUserServiceFacilityJobOrder(role_id, user_id);
        }
        public List<ViewServiceFacilityJobOrder> GetUserServiceFacilityJobOrderFE(string rm_id, int user_id)
        {
            return new DASplitterAllocationPortInfo().GetUserServiceFacilityJobOrderFE(rm_id, user_id);
        }
        public DbMessage GetCentroidByGeom(string entity_geom, string geom_type)
        {
            return objDAMisc.GetCentroidByGeom(entity_geom, geom_type);
        }
        public DbMessage GetJpBoundaryByGeom(string geom)
        {
            return objDAMisc.GetJpBoundaryByGeom(geom);
        }
        public DbMessage ResetDesignID(string pEntityType, int pSystemId, int p_user_id)
        {
            return objDAMisc.ResetDesignID(pEntityType, pSystemId, p_user_id);
        }
        public DbMessage ResetPartialDesignID(string pEntityType, int pSystemId, int p_user_id)
        {
            return objDAMisc.ResetPartialDesignID(pEntityType, pSystemId, p_user_id);
        }
        public void BindPortDetails(dynamic objmaster, string entype, string ddltype)
        {
            DALayer objDALayer = new DALayer();
            var layerdetails = objDALayer.getLayer(entype);
            if (layerdetails != null)
            {
                objmaster.unit_input_type = layerdetails.unit_input_type;
                if (layerdetails.unit_input_type == "iopddl")
                {
                    var objresp = objDAMisc.GetDropDownList(entype, ddltype);
                    objmaster.lstIOPDetails = objresp;
                }
            }
        }
        public string GetNetworkIdForJobID(int systemId, string entityType)
        {

            var NetworkId = new DataAccess.DAPrintSavedTemplate().GetNetworkId(systemId, entityType);
            return NetworkId;
        }
        public List<downloadbckupfile> downloadstatusbck()
        {
            List<downloadbckupfile> lst = new List<downloadbckupfile>();
            lst = new DADownloadBckup().downloadstatusbck();
            return lst;
        }

        public EmailSettingsModel updateEmailSettings(EmailSettingsModel obj, int id)
        {
            return new DAMisce().updateEmailSettings(obj, id);
        }

        public UserRegionProvince GetRegionProvinceBasedOnLocation(string geom, int userId)
        {
            return new DAMisc().GetRegionProvinceBasedOnLocation(geom, userId);
        }
        public List<DropDownMaster> GetToplogyDropDownList(string ddType)
        {
            return objDAMisc.GetToplogyDropDownList(ddType);
        } 
    }

    //public class BLUserModule
    //{
    //    public List<UserModule> GetAllModule() {
    //        return new DAModuleMaster().GetAllModule();
    //    }
    //}

    public class BLNetworkStatus
    {
        public DbMessage UpdateNetworkStatus(int systemid, string entityType, string currentNetworkStatus, int userid)
        {
            return new DANetworkStatus().UpdateNetworkStatus(systemid, entityType, currentNetworkStatus, userid);
        }
        public bool ConvertBulkNetworkEntity(string geom, int userId, string geomType, double buff_Radius, string currStatus, string entityType, string changeNetworkStatus, string entitySubtype)
        {
            return new DANetworkStatus().ConvertBulkNetworkEntity(geom, userId, geomType, buff_Radius, currStatus, entityType, changeNetworkStatus, entitySubtype);
        }
        public bool BulkAsBuiltDormant(string geom, int userId, string geomType, double buff_Radius, string currStatus, string newStatus)
        {
            return new DANetworkStatus().BulkAsBuiltDormant(geom, userId, geomType, buff_Radius, currStatus, newStatus);
        }
    }

    public class BLTerminationPointsMaster
    {
        public List<VWTerminationPointMaster> getOSPTerminationPoints(string layerName)
        {
            return new DATerminationPointsMaster().getOSPTerminationPoints(layerName);
        }
        public List<VWTerminationPointMaster> getISPTerminationPoints(string layerName)
        {
            return new DATerminationPointsMaster().getISPTerminationPoints(layerName);
        }
    }
    public class BLFormInputSettings
    {
        public List<string> getDistinctFormNames()
        {
            return new DAFormInputSettings().getDistinctFormNames();
        }
        public List<FormInputSettings> getformInputSettings()
        {
            return new DAFormInputSettings().getformInputSettings();
        }
        public List<FormInputSettings> getformInputSettings(string formName)
        {
            return new DAFormInputSettings().getformInputSettings(formName);
        }
        public bool SaveFormInputSettings(vwFormInputSettings objFormInputSettings)
        {
            return new DAFormInputSettings().SaveFormInputSettings(objFormInputSettings);
        }
    }
    public class BLPortStatus
    {
        public List<portStatusMaster> getPortStatus()
        {
            return new DAPortStatusColor().getPortStatus();
        }
    }

    public class BLtemp_auto_network_plan
    {
        public List<temp_auto_network_plan> GetTempNetwork(int temp_plan_id)
        {
            return new DAtemp_auto_network_plan().GetTempNetwork(temp_plan_id);
        } 

        public void UpdateLoopLengthByPlanId(int temp_plan_id, double looplength)
        {
            try
            {
                new DAtemp_auto_network_plan().UpdateLoopLengthByPlanId(temp_plan_id, looplength);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateTempLoop(List<temp_auto_network_plan> model)
        {
            new DAtemp_auto_network_plan().UpdateTempLoop(model);
        }


        public double GetTotalLoopLength(int plan_id)
        {
            return new DAtemp_auto_network_plan().GetTotalLoopLength(plan_id);
        }

        public int GetcountOfPointEntity(int plan_id)
        {
            return new DAtemp_auto_network_plan().GetcountOfPointEntity(plan_id);
        }
    }
    public class BLPlan
    {
        public List<NearestMahhole> getNearestManholes(string buildingIDs)
        {
            return new DAPlan().getNearestManholes(buildingIDs);
        }
        public List<NetworkPlanning> GetNetworkPlanning(int userId)
        {
            return new DANetworkPlanning().GetNetworkPlanning(userId);
        }

        public NetworkPlanning GetNetworkPlanningById(int planId)
        {
            return new DANetworkPlanning().GetNetworkPlanningById(planId);
        }

        public BackBonePlanning GetBackbonePlanningById(int planId)
        {
            return new DANetworkPlanning().GetBackbonePlanningById(planId);
        }

        public NetworkPlanning GetNetworkForMap(int planId)
        {
            return new DANetworkPlanning().GetNetworkForMap(planId);
        }

        public BackBonePlanning GetBackboneForMap(int planId)
        {
            return new DANetworkPlanning().GetBackboneForMap(planId);
        }

        public List<NetworkPlanning> GetTempNetworkForMap(int planId)
        {
            return new DANetworkPlanning().GetTempNetworkForMap(planId);
        }

        public dynamic GetNearByEndPoint(double lat, double lng, string Entity_Type, double End_point_buffer)
        {
            return new DAPlan().GetNearByEndPoint(lat, lng, Entity_Type, End_point_buffer);
        }

        public string GetPlanElement(int plan_id)
        {
            return new DAPlan().GetPlanElement(plan_id);
        }

        public List<NetworkPlanning> GetPlanDetails(NetworkPlanningDataFilter objExtnlDtaFilter, int user_id)
        {
            return new DAPlan().GetPlanDetails(objExtnlDtaFilter, user_id);
        }

        public List<BackBonePlanning> GetBackbonePlanHistoryDetails(BackBonePlanningDataFilter objExtnlDtaFilter, int user_id)
        {
            return new DAPlan().GetBackbonePlanHistoryDetails(objExtnlDtaFilter, user_id);
        }
        public List<DbMessage> processPlan(int building_id, int manhole_id, string geom, int user_id)
        {
            return new DAPlan().processPlan(building_id, manhole_id, geom, user_id);
        }
        public List<DbMessage> Point2PointPlan(string geom, int user_id)
        {
            return new DAPlan().Point2PointPlan(geom, user_id);
        }
        public List<PlanBom> GetPlanBomByPlanId(int plan_id, int user_id)
        {
            return new DAPlan().GetPlanBomByPlanId(plan_id, user_id);
        }

        public List<PlanBom> PlanBom(NetworkPlanning model, int user_id)
        {
            return new DAPlan().PlanBom(model, user_id);
        }
        public List<DbMessageForPlan> savePoint2Point(NetworkPlanning objPlan)
        {
            return new DAPlan().savePoint2Point(objPlan);
        }

        public List<DbMessageForBackbonePlan> saveBackbonePlanning(BackBonePlanning objPlan)
        {
            return new DAPlan().saveBackbonePlanning(objPlan);
        }

        public List<DbMessageForPlan> DeletePlanByPlanId(int plan_id, int user_id)
        {
            return new DAPlan().DeletePlanByPlanId(plan_id, user_id);
        }

        public List<DbMessageForPlan> DeleteBackbonePlanByPlanId(int plan_id, int user_id)
        {
            return new DAPlan().DeleteBackbonePlanByPlanId(plan_id, user_id);
        }



        public List<PlanBom> GetTempCableLengthGemo(NetworkPlanning model, int user_id)
        {
            return new DAPlan().GetTempCableLengthGemo(model, user_id);
        }

        public OffsetGeometry getCableGeom(string cablegemo, double offset)
        {
            try
            {
                return new DAPlan().getCableGeom(cablegemo, offset);
            }
            catch { throw; }
        }

        public List<PlanBom> GetPointOfCable(NetworkPlanning model, int user_id)
        {
            return new DAPlan().GetPointOfCable(model, user_id);
        }

        public double GetLineLength(string geom)
        {
            return new DAPlan().GetLineLength(geom);
        }
        public BackBoneSitePlanDetails GetNearestSiteList(string geom, double buffer)
        {
            return new DAPlan().GetNearestSiteList(geom, buffer);
        }
        public List<BackBoneSproutFiberDetails> GetBackbonePlanningList(BackBonePlanning plan, int userId)
        {
            return new DAPlan().GetBackbonePlanningList(plan, userId);
        }
        public void updateSiteLineGeometry(string lineGeom, int systemId,double cableLength,double? threshold,int planId)
        {
             new DAPlan().updateSiteLineGeometry(lineGeom, systemId, cableLength, threshold, planId);
        }
        public List<BackBoneBOMOBOQResponse> BackBonePlanBom(BackBonePlanning model, int userId)
        {
             return new DAPlan().BackBonePlanBom(model, userId);
        }
        public List<BackBonePlanBom> GetBackBonePlanBomByPlanId(int plan_id, int user_id)
        {
            return new DAPlan().GetBackBonePlanBomByPlanId(plan_id, user_id);
        }

        public List<SiteBufferGeometryRaw> BackBonePlanDraftLineGeometry(int planId, int systemId)
        {
             return new DAPlan().BackBonePlanDraftLineGeometry(planId, systemId);
        } 
        public List<DropDownMaster> GetBackboneFiberTypeDropDownList()
        {
             return new DAPlan().GetBackboneFiberTypeDropDownList();
        }

        public void UpdateBackBoneLoopLength(List<BackbonePlanNetworkDetails> model)
        {
            new DABackBonePlan().UpdateBackBoneLoopLength(model);
        }
        public List<BackbonePlanNetworkDetails> GetBackBoneLoopList(int plan_id, int userId)
        {
            return new DABackBonePlan().GetBackBoneLoopList(plan_id, userId);
        }

    }
    public class BLSiteCircle
    {
        public List<SiteCircleList> GetCircleList()
        {
            return new DASiteCircle().GetCircleList();
        }
    }

}


public class BLNetworkStatus
{
    public DbMessage UpdateNetworkStatus(int systemid, string entityType, string currentNetworkStatus, int userid)
    {
        return new DANetworkStatus().UpdateNetworkStatus(systemid, entityType, currentNetworkStatus, userid);
    }
    public bool ConvertBulkNetworkEntity(string geom, int userId, string geomType, double buff_Radius, string currStatus, string entityType, string changeNetworkStatus, string entitySubtype)
    {
        return new DANetworkStatus().ConvertBulkNetworkEntity(geom, userId, geomType, buff_Radius, currStatus, entityType, changeNetworkStatus, entitySubtype);
    }
    public bool BulkAsBuiltDormant(string geom, int userId, string geomType, double buff_Radius, string currStatus, string newStatus)
    {
        return new DANetworkStatus().BulkAsBuiltDormant(geom, userId, geomType, buff_Radius, currStatus, newStatus);
    }


    public bool isNetworkIdExist(string networkId, string entityType, int userId)
    {
        return new DANetworkStatus().isNetworkIdExist(networkId, entityType, userId);
    }
}

public class BLTerminationPointsMaster
{
    public List<VWTerminationPointMaster> getOSPTerminationPoints(string layerName)
    {
        return new DATerminationPointsMaster().getOSPTerminationPoints(layerName);
    }
    public List<VWTerminationPointMaster> getISPTerminationPoints(string layerName)
    {
        return new DATerminationPointsMaster().getISPTerminationPoints(layerName);
    }
}
public class BLFormInputSettings
{
    public List<string> getDistinctFormNames()
    {
        return new DAFormInputSettings().getDistinctFormNames();
    }
    public List<FormInputSettings> getformInputSettings()
    {
        return new DAFormInputSettings().getformInputSettings();
    }
    public List<FormInputSettings> getformInputSettings(string formName)
    {
        return new DAFormInputSettings().getformInputSettings(formName);
    }
    public bool SaveFormInputSettings(vwFormInputSettings objFormInputSettings)
    {
        return new DAFormInputSettings().SaveFormInputSettings(objFormInputSettings);
    }

}
public class BLPortStatus
{
    public List<portStatusMaster> getPortStatus()
    {
        return new DAPortStatusColor().getPortStatus();
    }
    public List<portStatusMaster> getPortStatusFiber()
    {
        return new DAPortStatusColor().getPortStatusFiber();
    }
}
#region ANTRA
public class BLMiscellaneous
{
    DAMiscellaneous objDAUtil = new DAMiscellaneous();
    public AddNewEntityUtilization SaveEntityUtilizationDetails(AddNewEntityUtilization objAddEntityUtilSetting)
    {
        return new DAMiscellaneous().SaveEntityUtilizationDetails(objAddEntityUtilSetting);
    }
    public AddNewEntityUtilization ChkUtilizationExist(AddNewEntityUtilization objUtilization)
    {
        return objDAUtil.ChkUtilizationExist(objUtilization);
    }
    public AddNewEntityUtilization GetUtilizationDetailByID(int id)
    {
        return new DAMiscellaneous().GetUtilizationDetailByID(id);
    }
}
#endregion

public class BLTicketType
{
    public List<TicketTypeMaster> GetTicketTypeDetails(ViewTicketType objTTMaster)
    {
        return new DATicketType().GetTicketTypeDetails(objTTMaster);
    }
    public TicketTypeMaster DATicketType(int id)
    {
        return new DATicketType().GetTicketTypeById(id);
    }
    public string SaveTicketType(TicketTypeMaster objTerminationPoint)
    {
        return new DATicketType().SaveTicketType(objTerminationPoint);
    }
    public string DeleteTicketTypeById(int id)
    {
        return new DATicketType().DeleteTicketTypeById(id);
    }
    public string GetDesignId(int systemId, string EntityType)
    {
        return new DATicketType().GetDesignId(systemId, EntityType);
    }
    //int userID,userID, 
    public Pushgislogrequest Getpushrequest(int systemId,string EntityType, int userID, string sAction,  string process_id, string sMessage = "")
    {
        return new DATicketType().Getpushrequest(systemId, EntityType, userID, sAction, process_id,sMessage);
    }

    public Pushgislogrequest Getpushrequestselect(int systemId, string EntityType)
    {
        return new DATicketType().Getpushrequestselect(systemId, EntityType);
    }


    public List<TicketTypeMaster> GetAllTicketType()
    {
        return new DATicketType().GetAllTicketType();
    }

}

public class BLVoiceCommandMaster
{
    public List<VoiceCommandMaster> getVoiceCommandMaster()
    {
        return new DAVoiceCommandMaster().getVoiceCommandMaster();
    }
    public SaveVoiceCommandMaster SaveVoiceCommandMaster(SaveVoiceCommandMaster objVoiceCommandMaster)
    {
        return new DASaveVoiceCommandMaster().SaveVoiceCommandSettings(objVoiceCommandMaster);
    }
    public IList<SaveVoiceCommandMaster> GetVoiceCommandDetail(VoiceCommandMaster model)
    {
        return new DAVoiceCommandMaster().GetVoiceCommandDetail(model);
    }
    public SaveVoiceCommandMaster GetVoiceCommandDetailsByID(int id)
    {
        return new DASaveVoiceCommandMaster().GetVoiceCommandDetailsByID(id);
    }
    public void BulkUploadVoiceCommandMapping(List<TempSaveVoiceCommandMaster> BulkUploadVoiceCommandMapping)
    {
        new DAVVoiceCommandMappingbulk().VoiceCommandMappingBulkUpload(BulkUploadVoiceCommandMapping);
    }

    public void DeleteTempVoiceCommandMappingData(int UserId)
    {
        new DAVVoiceCommandMappingbulk().DeleteTempVoiceCommandMappingData(UserId);
    }
    public DbMessage UploadVoiceCommandMappingForInsert(int userID)
    {
        return new DAVVoiceCommandMappingbulk().UploadVoiceCommandMappingForInsert(userID);
    }
    public Tuple<int, int> getTotalUploadBuildingfailureAndSuccess(int UserId)
    {
        return new DAVVoiceCommandMappingbulk().getTotalUploadVoiceCommandMappingfailureAndSuccess(UserId);
    }
    public List<TempSaveVoiceCommandMaster> GetUploadVoiceCommandMappingLogs(int userId)
    {
        return new DAVVoiceCommandMappingbulk().GetUploadVoiceCommandMappingLogs(userId);
    }

    public string GetVoiceCommandJsonString()
    {
        return new DAVoiceCommandMaster().GetVoiceCommandJsonString();
    }
}

public class BLUserActivityLog
{
    public List<UserActivityLogSettings> GetUserActivityLogSettings()
    {
        return DAUserActivityLogSettings.Instance.GetUserActivityLogSettings();

    }
    public List<UserActivityLog> GetUserActivityLogDetails(CommonGridAttributes objUserActivityLogFilter)
    {
        return DAUserActivityLog.Instance.GetUserActivityLogDetails(objUserActivityLogFilter);

    }
    public UserActivityLog SaveUserActivityLog(UserActivityLog obj)
    {
        return DAUserActivityLog.Instance.SaveUserActivityLog(obj);
    }
    public List<UserActivityLogSettingsPage> GetUserActivityLogsSettings(CommonGridAttributes objGridAttributes)
    {
        return new DAUserActivityLogSettingsPage().GetUserActivityLogsSettings(objGridAttributes);
    }
    public bool EditLogStatus(int id, bool status)
    {
        return new DAUserActivityLogSettingsPage().EditLogStatus(id, status);
    }
}
public class BLAutoCodificationLog
    {
        public List<AutoCodificationLog> GetAutoCodificationLogDetails(CommonGridAttributes objAutoCodificationLogFilter)
    {
        return DAAutoCodificationLog.Instance.GetAutoCodificationLogDetails(objAutoCodificationLogFilter);

    }
}
public class BLEntityDeleteLog
{
    public List<EntityDeleteLog> GetEntityDeleteLogDetails(CommonGridAttributes objEntityDeleteLogFilter)
    {
        return DAEntityDeleteLog.Instance.GetEntityDeleteLogDetails(objEntityDeleteLogFilter);

    }
}
public class BLBulkAssociationRequestLog
{
    DABulkAssociatonRequestLog DAobj = new DABulkAssociatonRequestLog();
    public BulkAssociationRequestLog SaveBulkAssociationLog(BulkAssociationRequestLog objBulkAssociationLog)
    {
        return DAobj.SaveBulkAssociationLog(objBulkAssociationLog);

    }
}
public class BLWorkAreaMarking
{
    DAWorkAreaMarking DAobj = new DAWorkAreaMarking();
    public DbMessage SaveWorkAreaMarking(WorkAreaMarking objMarking)
    {
        return DAobj.SaveWorkAreaMarking(objMarking);

    }
    public WorkAreaMarking GetWorkAreaMarkingById(int marking_id)
    {
        return DAobj.GetWorkAreaMarkingById(marking_id);

    }
    public List<WorkAreaMarking> GetWorkareaByWorkspaceId(int workspace_id)
    {
        return DAobj.GetWorkareaByWorkspaceId(workspace_id);

    }
    public int DeleteMarkings(int workSpaceId)
    {
        return DAobj.DeleteMarkings(workSpaceId);
    }
    
}







