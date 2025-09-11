using DataAccess.DBHelpers;
using Models;
using Models.API;
using Models.ISP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using Models.Admin;
using System.Web.UI;
using GeometryType = Models.GeometryType;
using NPOI.SS.Formula.Functions;
using Models.DaFiFeasibilityAPI;
using Models.WFM;
//using System.Data.Entity.Core.Metadata.Edm;
//using EntityType = Models.EntityType;


namespace DataAccess
{
    public class DAMisc : Repository<DropDownMaster>
    {
        public List<EntityLayerActions> getEntityLayerActions(int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<EntityLayerActions>("fn_get_layer_action_by_userId", new { p_user_id = user_id }, true);
            }
            catch { throw; }
        }

        public List<MicrowaveLinkViewModel> GetMicroWaveLinkAssociatedElements(int systemId)
        {
            return repo.ExecuteProcedure<MicrowaveLinkViewModel>("fn_get_microwavelinkinfo", new { p_microwavelinkid = systemId }, true).ToList();
        }

        public List<DropDownMaster> GetDropDownList(string enType, string ddType = "")
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_dropdownlist", new { entitytype = enType, dropdownType = ddType });
            }
            catch { throw; }
        }


        public List<DropDownMaster> GetAssociationDropDownList(string enType, string ddType = "")
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_association_dropdownlist", new { entitytype = enType, dropdownType = ddType });
            }
            catch { throw; }
        }
        public List<DropDownMaster> GetDropDownListJson(string enType, string ddType = "")
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_dropdownlist_json", new { entitytype = enType, dropdownType = ddType }, true);
            }
            catch { throw; }
        }


        public List<DropDownMaster> GetDropDownListParent(string _layerName, string ddType = "")
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_dropdownlist_by_parent", new { layerName = _layerName, parent_value = ddType }, true);
            }
            catch { throw; }
        }

        public int GetInValidRecordCount(UploadSummary summary)
        {
            try
            {
                return repo.ExecuteProcedure<int>("fn_uploader_get_invalid_record_count", new { p_upload_id = summary.id, p_entity_type = summary.entity_type }).FirstOrDefault();
            }
            catch { throw; }

        }
        public int GetCount(EntityType entityType, UploadSummary summary)
        {
            DataTable dt;
            switch (entityType)
            {
                case EntityType.Cable:
                    dt = repo.GetDataTable("select count(*) from line_master where upper(entity_type)= '" + entityType.ToString().ToUpper() + "' and db_flag=" + summary.id);
                    break;
                case EntityType.Duct:
                    dt = repo.GetDataTable("select count(*) from line_master where upper(entity_type)= '" + entityType.ToString().ToUpper() + "' and db_flag=" + summary.id);
                    break;
                case EntityType.Trench:
                    dt = repo.GetDataTable("select count(*) from line_master where upper(entity_type)= '" + entityType.ToString().ToUpper() + "' and db_flag=" + summary.id);
                    break;
                case EntityType.Sector:
                    dt = repo.GetDataTable("select count(*) from polygon_master where upper(entity_type)= '" + entityType.ToString().ToUpper() + "' and db_flag=" + summary.id);
                    break;
                case EntityType.ROW:
                    dt = repo.GetDataTable("select count(*) from polygon_master where upper(entity_type)= '" + entityType.ToString().ToUpper() + "' and db_flag=" + summary.id);
                    break;
                case EntityType.LandBase:
                    dt = repo.GetDataTable("select count(*) from att_details_landbase where db_flag=" + summary.id);
                    break;
                case EntityType.UNIT:
                    dt = repo.GetDataTable("select count(*) from isp_room_info where source_ref_id=" + summary.id);
                    break;
                case EntityType.Building:
                    dt = repo.GetDataTable("select count(*) from polygon_master where upper(entity_type)= '" + entityType.ToString().ToUpper() + "' and db_flag=" + summary.id);
                    break;
                default:
                    dt = repo.GetDataTable("select count(*) from point_master where upper(entity_type)= '" + entityType.ToString().ToUpper() + "' and db_flag=" + summary.id);
                    break;
            }
            //if (entityType == EntityType.Cable || entityType == EntityType.Duct || entityType == EntityType.Trench)
            //{
            //    dt = repo.GetDataTable("select count(*) from line_master where db_flag=" + summary.id);
            //}
            //else if (entityType == EntityType.POI)
            //{
            //    dt = repo.GetDataTable("select count(*) from poi_information where du_history_id=" + summary.id);
            //}
            //else if (entityType == EntityType.BTSLayer)
            //{
            //    dt = repo.GetDataTable("select count(*) from batch_los_bts where du_history_id=" + summary.id);
            //}
            //else if (entityType == EntityType.SifySector || entityType == EntityType.OtherSector)
            //{
            //    dt = repo.GetDataTable("select count(*) from public.tower_info where du_history_id =" + summary.id);
            //}
            //else if (entityType == EntityType.WirelessSify || entityType == EntityType.WirelessOthers)
            //{
            //    dt = repo.GetDataTable("select count(*) from public.batch_los_bts where du_history_id =" + summary.id);
            //}
            //else if (entityType == EntityType.WirelessCustomer)
            //{
            //    dt = repo.GetDataTable("select count(*) from public.customer_information where du_history_id =" + summary.id);
            //}
            //else if (entityType == EntityType.WirelessPOP)
            //{
            //    dt = repo.GetDataTable("select count(*) from public.pop_information where du_history_id =" + summary.id);
            //}
            //else if (entityType == EntityType.Building)
            //{
            //    dt = repo.GetDataTable("select count(*) from public.att_details_building where upload_id =" + summary.id);
            //}
            //else
            //{
            //    dt = repo.GetDataTable("select count(*) from point_master where db_flag=" + summary.id);
            //}

            if (dt != null && dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0][0]);
            else
                return 0;
        }


        //public List<EntityDetail> getNearByEntities(double latitude, double longitude, int bufferInMtr)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<EntityDetail>("fn_getnearbyentities", new { lat = latitude, lng = longitude, mtrBuffer = bufferInMtr });
        //    }
        //    catch { throw; }
        //}
        public List<EntityDetail> getNearByEntities(double latitude, double longitude, int bufferInMtr, string source_ref_id, string source_ref_type, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_getnearbyentities", new
                {
                    lat = latitude,
                    lng = longitude,
                    mtrBuffer = bufferInMtr,
                    p_user_id = user_id,
                    p_source_ref_id = source_ref_id,
                    p_source_ref_type = source_ref_type

                });
            }
            catch { throw; }
        }


        public List<bool> validateTopologyEntity(double latitude, double longitude, string geom, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<bool>("fn_getValidateRouteTopologyentities", new { lat = latitude, lng = longitude, p_geom= geom, p_user_id= user_id });

            
            }
            catch { throw; }
        }
        public List<EntityDetail> GetNearByTopologyEntity(double latitude, double longitude, int bufferInMtr, string source_ref_id, string source_ref_type, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_getnearbytopologyentities", new
                {
                    lat = latitude,
                    lng = longitude,
                    mtrBuffer = bufferInMtr,
                    p_user_id = user_id,
                    p_source_ref_id = source_ref_id,
                    p_source_ref_type = source_ref_type

                });
            }
            catch { throw; }
        }

        public List<nearestFiberPoint> getNearestFiberPoint(double latitude, double longitude, int bufferInMtr)
        {
            try
            {
                return repo.ExecuteProcedure<nearestFiberPoint>("fn_get_nearestfiberpoint", new
                {
                    lat = latitude,
                    lng = longitude,
                    mtrBuffer = bufferInMtr

                });
            }
            catch { throw; }
        }
        
        public List<nearestStructure> getNearestNetworkStructure(double latitude, double longitude, string network_id)
        {
            try
            {
                return repo.ExecuteProcedure<nearestStructure>("fn_get_connected_structure", new
                {
                    lat = latitude,
                    lng = longitude,
                    p_network_id = network_id

                });
            }
            catch { throw; }
        }

        public List<customerToRoad> getcustomerToRoad(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            try
            {
                return repo.ExecuteProcedure<customerToRoad>("fn_get_linestring", new
                {
                    p_latitude1 = latitude1,
                    p_longitude1 = longitude1,
                    p_latitude2 = latitude2,
                    p_longitude2 = longitude2,

                });
            }
            catch { throw; }
        }

        public List<EntityDetail> getNearByFeasibility(double latitude, double longitude, int bufferInMtr)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_getnearestfeasibility", new
                {
                    lat = latitude,
                    lng = longitude,
                    mtrBuffer = bufferInMtr

                });
            }
            catch { throw; }
        }
        public List<RouteBuffer> getRouteBufferFeasibility(string coordinates, int route_buffer)
        {
            try
            {
                return repo.ExecuteProcedure<RouteBuffer>("fn_getroutebufferfeasibility", new
                {
                    p_coordinates = coordinates,
                    mtrbuffer = route_buffer

                });
            }
            catch { throw; }
        }
        public List<Points> getStartEndPointsFeasibility(string coordinates)
        {
            try
            {
                return repo.ExecuteProcedure<Points>("fn_get_start_endpoints", new
                {
                    p_coordinates = coordinates

                });
            }
            catch { throw; }
        }
        public List<EntityDetailWithAttribute> GetNearByEntitiesWithAttribute(double latitude, double longitude, int bufferInMtr, int ticket_id, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetailWithAttribute>("fn_api_getnearbyentitywithatributes", new { p_lat = latitude, p_lng = longitude, mtrBuffer = bufferInMtr, p_ticket_id = ticket_id });
            }
            catch { throw; }
        }

        public bool ComputeHomePass(int system_id, string entity_name)
        {
            try
            {
                repo.ExecuteProcedure("fn_update_home_pass", new { p_system_id = system_id, p_entity_name = entity_name });
                return true;
            }
            catch { throw; }
        }
        public List<nearByNetworkEntities> getNearByNetworkEntities(double latitude, double longitude, string buildingCode, string buildingName, string entityTypes, int bufferInMtr)
        {
            try
            {
                return repo.ExecuteProcedure<nearByNetworkEntities>("fn_getNearByNetworkEntities", new { p_lat = latitude, p_lng = longitude, p_buildingCode = buildingCode, p_buildingName = buildingName, p_entityTypes = entityTypes, p_mtrBuffer = bufferInMtr });
            }
            catch { throw; }
        }
        public List<nearByEntityDetails> GetNearbyEntityDetailsWithAtributes(double latitude, double longitude, int bufferInMtr, int user_id, int batch_size, int last_record_number, int pageno, int pagerecord, string layer_name)
        {
            try
            {
                var result = repo.ExecuteProcedure<nearByEntityDetails>("fn_get_nearby_entities_with_attributes", new { p_latitude = latitude, p_longitude = longitude, p_mtrbuffer = bufferInMtr, p_user_id = user_id, p_batch_size = batch_size, p_last_record_number = last_record_number, p_pageno = pageno, p_pagerecord = pagerecord, p_layer_name = layer_name }, true).ToList();
                return result != null && result.Count > 0 ? result : new List<nearByEntityDetails>();
            }
            catch { throw; }
        }

        public int GetNearbyEntityDetailsWithAtributesCount(double latitude, double longitude, int bufferInMtr, int user_id, string layer_name)
        {
            try
            {
                var result = repo.ExecuteProcedure<int>("fn_get_nearby_entities_with_attributes_count", new { p_latitude = latitude, p_longitude = longitude, p_mtrbuffer = bufferInMtr, p_user_id = user_id, p_layer_name = layer_name }).FirstOrDefault();
                return result;
            }
            catch { throw; }

        }
        public List<EntityDetail> getEntitiesNearbyFault(double latitude, double longitude)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_get_entities_nearby_fault", new { lat = latitude, lng = longitude });
            }
            catch { throw; }
        }

        //public Dictionary<string, string> getEntityInfo(int systemId, string entityType, string geomType)
        //{
        //	try
        //	{
        //		var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_entity_info", new { p_systemId = systemId, p_entityType = entityType, p_geomType = geomType }, true);
        //		return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
        //	}
        //	catch { throw; }
        //}
        public List<entityInfo> getEntityInfo(int systemId, string entityType, string geomType, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<entityInfo>("fn_get_entity_info", new { p_systemId = systemId, p_entityType = entityType, p_geomType = geomType, p_user_id = userId }, true).ToList();
            }
            catch { throw; }
        }
        public Dictionary<string, string> getMobileEntityInfo(int systemId, string entityType, string geomType)
        {
            try
            {
                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_api_get_entity_info", new { p_systemId = systemId, p_entityType = entityType, p_geomType = geomType }, true);
                return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
            }
            catch { throw; }
        }
        public Dictionary<string, string> getSiteCustomerDetails(string searchText, string columnName, string lmcType)
        {
            try
            {
                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_site_customer_details", new { p_searchtext = searchText, p_columnName = columnName, p_lmc_type = lmcType }, true);
                return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
            }
            catch { throw; }
        }

        public Dictionary<string, string> getFiberLinkDetails(string searchText, string columnName)
        {
            try
            {
                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_fiber_link_details", new { p_searchtext = searchText, p_columnName = columnName }, true);
                return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
            }
            catch { throw; }
        }

        public Dictionary<string, string> getEntityAdvanceAttribute(int systemId, string entityType, string geomType)
        {
            try
            {
                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_entity_advance_attribute", new { p_systemId = systemId, p_entityType = entityType, p_geomType = geomType }, true);
                return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
            }
            catch { throw; }
        }

        //public List<LayerActionMapping> getEntityActionInfo(string layerName)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<LayerActionMapping>("fn_get_layer_actions", new { p_layer_name = layerName }, true);

        //    }
        //    catch { throw; }
        //}
        //public List<LayerActionMapping> getEntityActionInfo(string layerName)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<LayerActionMapping>("fn_get_isp_layer_actions", new { p_layer_name = layerName }, true);

        //    }
        //    catch { throw; }
        //}

        public List<LayerActionMapping> getLayerActions(int systemId, string layerName, bool isOSP_Type, string network_status, int role_id, int user_id, bool isMobileAction, string source_ref_id, string source_ref_type)
        {
            try
            {
                return repo.ExecuteProcedure<LayerActionMapping>("fn_get_layer_actions", new { p_layer_name = layerName, p_isOsp_type = isOSP_Type, p_network_status = network_status, p_role_id = role_id, p_user_id = user_id, p_isMobileAction = isMobileAction, p_system_id = systemId, p_source_ref_id = source_ref_id, p_source_ref_type = source_ref_type }, true);
            }
            catch { throw; }
        }

        public List<MobileRegionProvince> getRegionProvince(int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<MobileRegionProvince>("fn_get_region_province_layers_mobile", new { p_user_id = user_id });
            }
            catch { throw; }
        }
        public List<CrmTicketInfo> getTicketInfo(int userId)
        {
            try
            {
                var result = repo.ExecuteProcedure<CrmTicketInfo>("fn_api_get_user_tickets_info", new { p_userid = userId }, true).OrderByDescending(m => m.created_on).ToList();
                return result != null && result.Count > 0 ? result : new List<CrmTicketInfo>();
            }
            catch { throw; }
        }

        public DbMessage UpdateEntityBarCode(int? system_id, string entity_type, string barcode, string network_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_update_entity_barcode", new { p_systemid = system_id, p_entitytype = entity_type, p_barcode = barcode, p_network_id = network_id }).FirstOrDefault();
            }
            catch { throw; }
        }

        public DbMessage UpdatePowerMeterReading(int? system_id, string entity_type, double? power_meter_reading, bool is_manual_meter_reading)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_update_splitter_power_meter_reading", new { p_systemid = system_id, p_entitytype = entity_type, p_powermeterreading = power_meter_reading, p_ismanualmeterreading = is_manual_meter_reading }).FirstOrDefault();
            }
            catch { throw; }
        }


        public List<StaticPageMasterInfo> getStaticPageDetails(string name)
        {
            try
            {
                return repo.ExecuteProcedure<StaticPageMasterInfo>("fn_get_static_page_details", new { p_name = name }, true).ToList();

            }
            catch { throw; }
        }

        public List<ticketStepDetails> getTicketStepDetails(int ticket_id, int ticket_type_id, string rfs_type)
        {
            try
            {
                return repo.ExecuteProcedure<ticketStepDetails>("fn_api_ticket_steps_detail", new { p_ticket_id = ticket_id, p_ticket_type_id = ticket_type_id, p_rfs_type = rfs_type }, true);
                //return result != null && result.Count > 0 ? result : new List<ticketStepDetails>(); 
            }
            catch { throw; }
        }

        public List<StructureElement> GetStructureElementInfo(int system_id, string associated_entity_types, string can_id, bool isOnlyServingDb, string rfs_type, string module_abbr)
        {
            try
            {
                var result = repo.ExecuteProcedure<StructureElement>("fn_get_structure_element_info", new { p_structure_id = system_id, p_structure_entities = associated_entity_types, p_can_id = can_id, p_isonly_servingdb = isOnlyServingDb, p_rfs_type = rfs_type, p_module_abbr = module_abbr }, true);
                return result != null && result.Count > 0 ? result : new List<StructureElement>();
            }
            catch { throw; }
        }

        public List<ChildElementDetail> getDependentChildElements(ImpactDetailIn objImpactDetailIn)
        {
            try
            {
                var result = repo.ExecuteProcedure<ChildElementDetail>("fn_get_dependent_child_elements", new { p_systemId = objImpactDetailIn.systemId, p_entityType = objImpactDetailIn.entityType, p_geomType = objImpactDetailIn.geomType, p_impactType = objImpactDetailIn.impactType });
                return result != null && result.Count > 0 ? result : new List<ChildElementDetail>();
            }
            catch { throw; }
        }

        public bool chkEntityIsMiddleWare(string entityType)
        {

            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_chk_entity_is_middleware", new { p_entity_type = entityType });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;

            }

            catch { throw; }

        }
        public T GetNetworkTicketIdByEntityId<T>(int systemid, EntityType eType) where T : new()
        {
            try
            {
                var lstItems = new T();
                if (systemid > 0)
                {
                    lstItems = repo.ExecuteProcedure<T>("fn_get_networkTicketid_by_entityid", new { p_systemid = systemid, p_entity_name = eType.ToString() }, true).FirstOrDefault();
                }
                return lstItems;
            }
            catch { throw; }
        }

        public T GetEntityDetailById<T>(int systemid, EntityType eType, int userId) where T : new()
        {
            try
            {
                var lstItems = new T();
                if (systemid > 0)
                {
                    lstItems = repo.ExecuteProcedure<T>("fn_get_entitydetail_by_id", new { p_systemid = systemid, p_entity_name = eType.ToString(), p_user_id = userId }, true).FirstOrDefault();
                }
                return lstItems;
            }
            catch { throw; }
        }
        public BarCode GetEntityDetailForBarCode(int systemid, string eType)
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<BarCode>("fn_get_entitydetail_for_barcode", new { p_systemid = systemid, p_entity_name = eType }, true);
                return lstItems != null && lstItems.Count > 0 ? lstItems[0] : new BarCode();
            }
            catch { throw; }
        }
        public NetworkDtl GetNetworkDetails(string geom, string geomType, string entityType)
        {
            try
            {
                var lstGeomDetails = repo.ExecuteProcedure<NetworkDtl>("fn_get_networkdetail", new { geometry = geom, geomType = geomType, entityType = entityType });
                return lstGeomDetails != null && lstGeomDetails.Count > 0 ? lstGeomDetails[0] : new NetworkDtl();
            }
            catch { throw; }
        }

        public NetworkCodeDetail GetNetworkCodeDetail(NetworkCodeIn objIn)
        {
            try
            {
                var lstNetworkCodeDetails = repo.ExecuteProcedure<NetworkCodeDetail>("fn_get_network_code", new { etype = objIn.eType, gtype = objIn.gType, parent_sysid = objIn.parent_sysId, parent_etype = objIn.parent_eType, geometry = objIn.eGeom });
                return lstNetworkCodeDetails != null && lstNetworkCodeDetails.Count > 0 ? lstNetworkCodeDetails[0] : new NetworkCodeDetail();
            }
            catch { throw; }
        }
        public EntityCodeDetail GetEntityNetworkCodeDetail(NetworkCodeIn objIn)
        {
            try
            {
                var lstNetworkCodeDetails = repo.ExecuteProcedure<EntityCodeDetail>("fn_get_entity_auto_network_code", new { p_entity = objIn.eType, p_gtype = objIn.gType, p_geom = objIn.eGeom, p_parent_system_id = objIn.parent_sysId, p_parent_entity_type = objIn.parent_eType });
                return lstNetworkCodeDetails != null && lstNetworkCodeDetails.Count > 0 ? lstNetworkCodeDetails[0] : new EntityCodeDetail();
            }
            catch { throw; }
        }
        public ISPNetworkCodeDetail GetISPNetworkCodeDetail(ISPNetworkCodeIn objIn)
        {
            try
            {
                //var lstISPNetworkCodeDetails = repo.ExecuteProcedure<ISPNetworkCodeDetail>("fn_get_isp_network_code", new { etype = objIn.eType, parent_sysid = objIn.parent_sysId, parent_etype = objIn.parent_eType });
                var lstISPNetworkCodeDetails = repo.ExecuteProcedure<ISPNetworkCodeDetail>("fn_isp_get_network_code", new { etype = objIn.eType, parent_sysid = objIn.parent_sysId, parent_etype = objIn.parent_eType, p_structure_id = objIn.structureId });
                return lstISPNetworkCodeDetails != null && lstISPNetworkCodeDetails.Count > 0 ? lstISPNetworkCodeDetails[0] : new ISPNetworkCodeDetail();
            }
            catch { throw; }
        }
        public ISPNetworkCodeDetail GetISPModelNetworkCodeDetail(ISPNetworkCodeIn objIn)
        {
            try
            {
                var lstISPNetworkCodeDetails = repo.ExecuteProcedure<ISPNetworkCodeDetail>("fn_isp_get_model_network_code", new { etype = objIn.eType, parent_sysid = objIn.parent_sysId, parent_etype = objIn.parent_eType });
                return lstISPNetworkCodeDetails != null && lstISPNetworkCodeDetails.Count > 0 ? lstISPNetworkCodeDetails[0] : new ISPNetworkCodeDetail();
            }
            catch { throw; }
        }
        public List<EntityDetail> getNearByCables(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_get_near_cables", new { P_systemId = systemId, P_entityType = entityType });
            }
            catch { throw; }
        }
        public List<EntityDetail> getNearByDucts(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_get_near_ducts", new { P_systemId = systemId, P_entityType = entityType });
            }
            catch { throw; }
        }
        public List<EntityDetail> getNearByMicroducts(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_get_near_microducts", new { P_systemId = systemId, P_entityType = entityType });
            }
            catch { throw; }
        }
        #region ycode
        public List<EntityDetail> getNearByTrenchs(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_get_near_trenchs", new { P_systemId = systemId, P_entityType = entityType });
            }
            catch { throw; }
        }
        #endregion
        public List<EntityDetail> getNearByConduits(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_get_near_conduits", new { P_systemId = systemId, P_entityType = entityType });
            }
            catch { throw; }
        }
        public SplitCableEntity getSplitCableEntity(int splitEntitySystemId, string splitEntityType, int cableId, string entity_type)
        {

            try
            {
                var lstSplitCable = repo.ExecuteProcedure<SplitCableEntity>("fn_get_split_cable_details", new { P_splitEntitySystemId = splitEntitySystemId, P_splitEntityType = splitEntityType, P_cableId = cableId, P_entity_type = entity_type });
                return lstSplitCable != null && lstSplitCable.Count > 0 ? lstSplitCable[0] : new SplitCableEntity();
            }
            catch (Exception ex) { throw; }
        }
        public SplitDuctEntity getSplitDuctEntity(int splitEntitySystemId, string splitEntityType, string splitEnityNetworkId, int ductId, string entity_type)
        {

            try
            {
                var lstSplitDuct = repo.ExecuteProcedure<SplitDuctEntity>("fn_get_split_duct_details", new { P_splitEntitySystemId = splitEntitySystemId, P_splitEntityType = splitEntityType, p_split_entity_networkId = splitEnityNetworkId, P_ductId = ductId, P_entity_type = entity_type });
                return lstSplitDuct != null && lstSplitDuct.Count > 0 ? lstSplitDuct[0] : new SplitDuctEntity();
            }
            catch { throw; }
        }
        public SplitMicroductEntity getSplitMicroductEntity(int splitEntitySystemId, string splitEntityType, string splitEnityNetworkId, int microductId, string entity_type)
        {

            try
            {
                var lstSplitMicroduct = repo.ExecuteProcedure<SplitMicroductEntity>("fn_get_split_duct_details", new { P_splitEntitySystemId = splitEntitySystemId, P_splitEntityType = splitEntityType, p_split_entity_networkId = splitEnityNetworkId, P_microductId = microductId, P_entity_type = entity_type });
                return lstSplitMicroduct != null && lstSplitMicroduct.Count > 0 ? lstSplitMicroduct[0] : new SplitMicroductEntity();
            }
            catch { throw; }
        }

        #region start ycode
        public SplitTrenchEntity getSplitTrenchEntity(int splitEntitySystemId, string splitEntityType, string splitEnityNetworkId, int trenchId, string entity_type)
        {
            try
            {
                return repo.ExecuteProcedure<SplitTrenchEntity>("fn_get_split_trench_details", new { P_splitEntitySystemId = splitEntitySystemId, P_splitEntityType = splitEntityType, p_split_entity_networkId = splitEnityNetworkId, P_trenchId = trenchId, P_entity_type = entity_type }).FirstOrDefault();

            }
            catch { throw; }
        }
        #endregion //end ycode


        public SplitConduitEntity getSplitConduitEntity(int splitEntitySystemId, string splitEntityType, string splitEnityNetworkId, int conduitId, string entity_type)
        {

            try
            {
                var lstSplitConduit = repo.ExecuteProcedure<SplitConduitEntity>("fn_get_split_conduit_details", new { P_splitEntitySystemId = splitEntitySystemId, P_splitEntityType = splitEntityType, p_split_entity_networkId = splitEnityNetworkId, P_ductId = conduitId, P_entity_type = entity_type });
                return lstSplitConduit != null && lstSplitConduit.Count > 0 ? lstSplitConduit[0] : new SplitConduitEntity();
            }
            catch { throw; }
        }

        public Entitygeom getEntityGeomInfo(int entity_system_id, string entity_type, double sourceEntityLat, double sourceEntityLong)
        {
            try
            {
                var result = repo.ExecuteProcedure<Entitygeom>("fn_get_entity_latlong", new { p_system_id = entity_system_id, p_entitytype = entity_type, p_sourceEntityLat = sourceEntityLat, p_sourceEntityLong = sourceEntityLong }, true);
                return result != null && result.Count > 0 ? result[0] : new Entitygeom();
            }
            catch { throw; }
        }

        //Get Cable Measured Length
        public double GetCableLength(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<double>("getgeometrylength", new { p_longlat = geom }).FirstOrDefault();

            }
            catch { throw; }
        }
        public List<startendpoint> getEndPoints(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<startendpoint>("fn_get_cable_end_points", new { p_longlat = geom });

            }
            catch { throw; }
        }
        public List<TerminationPointDtl> GetTerminationDetail(string geom, int buffer, string entityType, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<TerminationPointDtl>("fn_get_termination_pt_detail", new { p_longlat = geom, buff_in_mtr = buffer, p_entityType = entityType, p_user_id = userId });

            }
            catch { throw; }
        }
        public bool chkNetworkIdExist(string networkId, string entityType, string networkStage)
        {

            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_chk_networkid_exist", new { p_networkid = networkId, p_entity_type = entityType, p_network_status = networkStage });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;

            }

            catch { throw; }

        }

        public List<T> GetEntityExportData<T>(int systemid, string eType, string networkStage) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_get_entity_export", new { p_system_id = systemid, p_entity_type = eType }, true);
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }
        public List<T> ExportChildEntityByParentCode<T>(int parentSystemId, string parentNetworkId, string parentEntityType, string childEntityType) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_get_export_child_entity", new { p_parentSystemId = parentSystemId, p_parentNetworkId = parentNetworkId, p_parentEntityType = parentEntityType, p_childEntityType = childEntityType }, true);
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

        public List<T> GetEntityReferenceExportData<T>(int systemid, string eType, string networkStage) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_get_entity_reference_export", new { p_system_id = systemid, p_entity_type = eType }, true);
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }
        public List<T> GetSpliceTrayExportData<T>(int systemid, string eType, string networkStage) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_get_tray_info_export", new { p_system_id = systemid, p_entity_type = eType }, true);
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }
        public List<EntityCount> GetEntityLabel(int systemid, string eType)
        {
            try
            {
                return repo.ExecuteProcedure<EntityCount>("fn_get_entity_label", new { systemid = systemid, eType = eType });

            }
            catch { throw; }
        }



        public bool chkEntityDataExist(int systemid, string entityType, string networkStage)
        {

            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_chk_entity_data_exist", new { p_system_id = systemid, p_entity_type = entityType, p_network_status = networkStage });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;

            }

            catch { throw; }

        }

        public NetworkCodeDetail GetLineNetworkCode(string start_network_id, string end_network_id, string enName, string geometry, string lineType, int pSystemId = 0, string pEntityType = "")
        {
            try
            {
                var lstlineNetworkCode = repo.ExecuteProcedure<NetworkCodeDetail>("fn_get_line_network_code", new { etype = enName, start_network_id = start_network_id, end_network_id = end_network_id, geometry = geometry, line_type = lineType, pSystemId = pSystemId, pEntityType = pEntityType });
                return lstlineNetworkCode != null && lstlineNetworkCode.Count > 0 ? lstlineNetworkCode[0] : new NetworkCodeDetail();
            }
            catch { throw; }
        }

        public string GetEntityName(string network_name, int system_id)
        {
            var result = repo.ExecuteProcedure<string>("fn_get_network_name", new { p_entity_type = network_name, p_entity_system_id = system_id }, false).FirstOrDefault();
            return result;
        }
        public DbMessage SaveCloneEntity(int refId, string entityName, string geomType, string geom, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_save_clone_entity", new { p_ref_id = refId, p_entity = entityName, p_gtype = geomType, p_geom = geom, p_user_id = userId }).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage AssociateFiberLinkWithCable(int link_system_id, int cable_id, int fiber_no, string action)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_associate_fiber_link_cable", new { p_cable_id = cable_id, p_link_system_id = link_system_id, p_fiber_number = fiber_no, p_action = action }).FirstOrDefault();
            }
            catch { throw; }
        }

        public DbMessage SaveBulkCloneEntity(ViewCloneDependent objCloneDependen, int userid)
        {
            try
            {
                string lstCloneDependent = JsonConvert.SerializeObject(objCloneDependen.lstCloneDependent);
                return repo.ExecuteProcedure<DbMessage>("fn_save_bulk_clone_entity", new
                {

                    p_ref_id = objCloneDependen.refId,
                    p_entity = objCloneDependen.entityName,
                    p_gtype = objCloneDependen.geomType,
                    p_geom = objCloneDependen.geom,
                    p_created_by = userid,
                    p_child_entities = lstCloneDependent,
                    p_is_accessories = objCloneDependen.is_accessories
                }).FirstOrDefault();
            }
            catch (Exception ex) { throw ex; }
        }

        public List<EntityLstCount> GetEntityLstByGeom(string geom, int userId, string geomType, double buff_Radius)
        {
            try
            {
                return repo.ExecuteProcedure<EntityLstCount>("fn_get_entitylst_count", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_radius = buff_Radius });
            }
            catch { throw; }
        }

        // Navin
        public List<EntityLstCount> GeBulkOperationLstCount(BulkAsBuiltFilterAttribute objfilter)
        {
            try
            {
                //return repo.ExecuteProcedure<EntityLstCount>("fn_get_bulkoperation_entitylst_count",
                //    new { p_geom = objfilter.geom, p_userId = objfilter.userid, p_selectiontype = objfilter.selection_type, p_radius = objfilter.buff_Radius, p_network_status = objfilter.dd_networkStatus }, true);


                return repo.ExecuteProcedure<EntityLstCount>("fn_get_bulkoperation_entities", new
                {
                    p_searchby = objfilter.searchBy,
                    p_searchtext = objfilter.searchText,
                    P_PAGENO = objfilter.currentPage,
                    P_PAGERECORD = objfilter.pageSize,
                    P_SORTCOLNAME = objfilter.sort,
                    P_SORTTYPE = objfilter.orderBy,
                    p_geom = objfilter.geom,
                    p_userid = objfilter.userid,
                    p_selectiontype = objfilter.selection_type,
                    p_radius = objfilter.buff_Radius,
                    p_network_status = objfilter.dd_networkStatus,
                    project_id = objfilter.lstBindProjectCode == null ? 0 : Convert.ToInt32(objfilter.lstBindProjectCode),//372  
                    p_parentusers = objfilter.SelectedParentUsers,
                    p_userids = objfilter.SelectedUserIds == "" ? objfilter.userid.ToString() : objfilter.SelectedUserIds,
                    p_roleid = objfilter.roleid

                }, true);
            }
            catch { throw; }
        }

        public DbMessage ValidateEntityByGeom(string geom, int userId, string geomType, double buff_Radius)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_get_validate_bulk_entity", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_radius = buff_Radius }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage ValidateEntityCreationArea(string geom, int userId, string geomType, int ticket_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_get_validate_EntityCreation_area", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_ticket_id = ticket_id }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage ValidateLBEntityCreationArea(string geom, int userId, string geomType, int ticket_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_landBase_validate_EntityCreation_area", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_ticket_id = ticket_id }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage ValidatePotentialArea(string geom, int userId, string geomType, double buff_Radius)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_get_validate_potential_area", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_radius = buff_Radius }).FirstOrDefault();

            }
            catch { throw; }
        }

        public DbMessage ValidateLMCPotentialArea(string geom, int userId, string geomType, double buff_Radius)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_get_validate_LMC_Bulk_Report_area", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_radius = buff_Radius }).FirstOrDefault();

            }
            catch { throw; }
        }



        public DbMessage SaveBulkProjSpecific(BulkProjSpecific objProjSpec)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_save_bulk_proj_specific",
                    new
                    {
                        p_geom = objProjSpec.geom,
                        p_user_id = objProjSpec.user_id,
                        p_select_type = objProjSpec.selection_type,
                        p_radius = objProjSpec.buff_Radius,
                        p_proj_id = objProjSpec.project_id,
                        p_planning_id = objProjSpec.planning_id,
                        p_purpose_id = objProjSpec.purpose_id,
                        p_workorder_id = objProjSpec.workorder_id,
                        p_entity_select = objProjSpec.entity_select,
                        p_ntk_type = objProjSpec.ntk_type,
                        entity_sub_type = objProjSpec.entity_sub_type
                    }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage SaveBulkPodAssociation(BulkPodAssociation objPodAssctn)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_save_bulk_pod_association",
                    new
                    {
                        p_geom = objPodAssctn.geom,
                        p_user_id = objPodAssctn.user_id,
                        p_select_type = objPodAssctn.selection_type,
                        p_radius = objPodAssctn.buff_Radius,
                        p_primary_pod_system_id = objPodAssctn.primary_pod_system_id,
                        p_secondary_pod_system_id = objPodAssctn.secondary_pod_system_id,
                        p_entity_select = objPodAssctn.entity_select,
                        p_ntk_type = objPodAssctn.ntk_type,
                        entity_sub_type = objPodAssctn.entity_sub_type
                    }).FirstOrDefault();

            }
            catch { throw; }
        }
        //public DbMessage BulkDeleteProcess(BulkDelete objBulkDelete)
        //{
        //    try
        //    {

        //        return repo.ExecuteProcedure<DbMessage>("fn_delete_bulkoperation_entities",
        //            new
        //            {
        //                p_geom = objBulkDelete.geom,
        //                p_userid = objBulkDelete.user_id,
        //                p_selectiontype = objBulkDelete.selection_type,
        //                p_radius = objBulkDelete.buff_Radius,
        //                p_network_status = objBulkDelete.ntk_type,
        //                p_entity_type = objBulkDelete.entity_select
        //            }).FirstOrDefault();

        //    }
        //    catch { throw; }
        //}

        public List<bulkDeleteOperation> BulkDeleteProcess(BulkDelete objBulkDelete)
        {
            try
            {

                return repo.ExecuteProcedure<bulkDeleteOperation>("fn_delete_bulkoperation_entities",
                    new
                    {
                        p_geom = objBulkDelete.geom,//
                        p_userid = objBulkDelete.user_id,
                        p_selectiontype = objBulkDelete.selection_type,
                        p_radius = objBulkDelete.buff_Radius,
                        p_network_status = objBulkDelete.ntk_type,
                        p_entity_type = objBulkDelete.entity_select,
                        P_entity_sub_type = objBulkDelete.entity_sub_type,
                        p_system_id = objBulkDelete.system_id,
                        p_root_id = objBulkDelete.rootid,
                        p_selectedUsers = objBulkDelete.selectedUsers
                    }).ToList();

            }
            catch { throw; }
        }

        public List<DropDownMaster> GetEntityTypeDropdownList(int layer_id)
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_entity_type_dropdownlist", new { p_layer_id = layer_id });
            }
            catch { throw; }
        }

        public bool InsertPortInfo(int inputPort, int OutPutPort, string entityType, int systemId, string networkId, int userId)
        {
            try
            {
                repo.ExecuteProcedure<object>("fn_bulk_insert_port_info", new { p_input_port = inputPort, p_output_port = OutPutPort, p_entity_type = entityType, p_system_id = systemId, p_networkId = networkId, p_userid = userId });
                return true;
            }
            catch { throw; }
        }
        public DbMessage isPortConnected(int systemId, string entityType, string specification, int vendor_id, string item_code)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_entity_is_connected", new { p_system_id = systemId, p_entity_type = entityType, p_specification = specification, p_vendor_id = vendor_id, p_item_code = item_code }).FirstOrDefault();
            }
            catch { throw; }
        }

        //public List<IspPortInfo> GetPortInfo(int system_id, string entity_type)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<IspPortInfo>("fn_get_port_info", new { p_systemid = system_id, p_entity_type = entity_type });
        //    }
        //    catch { throw; }
        //}



        //public List<EntityDetail> GetNearEntitiesByEntityName(double latitude, double longitude, int bufferInMtr, string entity_name)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<EntityDetail>("fn_getnearestentitybyentity_name", new { lat = latitude, lng = longitude, mtrBuffer = bufferInMtr, entity_name = entity_name });
        //    }
        //    catch { throw; }
        //}



        public List<EntityDetail> GetNearEntitiesByEntityType(int c_system_id, string c_entity_type, int bufferInMtr, string entity_type)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_getnearestentitybyentity_type", new { c_system_id = c_system_id, c_entity_type = c_entity_type, mtrbuffer = bufferInMtr, search_entity_type = entity_type });
            }
            catch { throw; }
        }

        public List<BufferEntity> getEntityInBuffer(int systemId, string entityType, string pEntityType, string pgeomType)
        {
            try
            {
                return repo.ExecuteProcedure<BufferEntity>("fn_get_entity_in_buffer", new { p_system_id = systemId, p_entity_type = entityType, p_pentity_type = pEntityType, p_pgeomType = pgeomType }, true).ToList();
            }
            catch { throw; }
        }
        public List<NearByAssociatedEntities> getnearbyassociatedentities(string entityType, string geom)
        {
            try
            {
                return repo.ExecuteProcedure<NearByAssociatedEntities>("fn_get_nearby_associated_entity", new { p_entity_type = entityType, p_geom = geom }, true).ToList();
            }
            catch { throw; }
        }

        public List<_RegionProvince> GetProvinceDetail(int province_id)
        {
            try
            {
                return repo.ExecuteProcedure<_RegionProvince>("fn_get_provincedetail", new { p_provinceId = province_id }, true).ToList();
            }
            catch { throw; }
        }
        public List<LineEntityInfo> getLineEntityInLineBuffer(int systemId, string entityType, int pSystemId, string pEntityType, string pParentGeom, string pParentGeomType)
        {
            try
            {
                return repo.ExecuteProcedure<LineEntityInfo>("fn_get_associate_entity", new { p_system_id = pSystemId, p_entity_type = pEntityType, p_parent_geom = pParentGeom, p_parent_geom_type = pParentGeomType }, true).ToList();
            }
            catch { throw; }
        }
        public List<RouteInfo> getRouteEntityInLineBuffer(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<RouteInfo>("fn_get_near_by_route", new { p_system_id = systemId, p_entity_type = entityType }, true).ToList();
            }
            catch { throw; }
        }
        public List<RouteInfo> getRouteEntityInLineBuffer(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<RouteInfo>("fn_get_near_by_route", new { p_geom = geom }, true).ToList();
            }
            catch { throw; }
        }
        public List<LineEntityInfo> getAutoLineEntityInLineBuffer(int systemId, string entityType, int pSystemId, string pEntityType, string pParentGeom, string pParentGeomType)
        {
            try
            {
                return repo.ExecuteProcedure<LineEntityInfo>("fn_get_associate_entity_test", new { p_parent_geom = pParentGeom }, true).ToList();
            }
            catch { throw; }
        }
        public DbMessage checkIsBuried(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_check_entity_is_buried", new { p_system_id = systemId, p_entity_type = entityType }, false).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<LineEntityInfo> viewEntityAssociation(int pSystemId, string pEntityType)
        {
            try
            {
                return repo.ExecuteProcedure<LineEntityInfo>("fn_get_entity_association", new { p_system_id = pSystemId, p_pentity_type = pEntityType }, true).ToList();
            }
            catch { throw; }
        }
        public DbMessage saveLineEntityAssocition(string objLineEnAssocite, int pSystemId, string pEntityType, int userId, int pManholeCount)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_save_Entity_Assocition", new { p_line_associate_info = objLineEnAssocite, p_parent_system_id = pSystemId, p_parent_entity_type = pEntityType, p_user_id = userId, p_manhole_count = pManholeCount }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage saveRouteAssocition(string objRouteAssocite, int pSystemId, string pEntityType, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_save_Route_Assocition", new { p_route_associate_info = objRouteAssocite, p_parent_system_id = pSystemId, p_parent_entity_type = pEntityType, p_user_id = userId }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public DbMessage saveLineEntityAutoAssocition(int pSystemId, string pEntityType, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_bulk_associate_entity", new { p_system_id = pSystemId, p_entity_type = pEntityType, p_user_id = userId }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public DbMessage validateBulkAssosiation(int pSystemId, string pEntityType, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_bulk_associate_validate", new { p_system_id = pSystemId, p_entity_type = pEntityType, p_user_id = userId }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage EntityConversion(string aEntityType, string aEntityNetworkId, int aEntitySystemId, string bEntityType, string bEntityNetworkId, int bEntitySystemId, string geom, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_convert_entity", new { p_aEntityType = aEntityType, p_aEntityNetworkId = aEntityNetworkId, p_aEntitySystemId = aEntitySystemId, p_bEntityType = bEntityType, p_bEntityNetworkId = bEntityNetworkId, p_bEntitySystemId = bEntitySystemId, p_user_id = userId }).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<T> GetConnectionExportData<T>(int sourceId, int connectorId, int destinationId, string connectorEntityType) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_get_export_connections", new { p_sourceid = sourceId, p_connectorid = connectorId, p_destinationid = destinationId, p_connectorentitytype = connectorEntityType }, true).ToList();
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

        public List<ExportConnectionReport> GetConnectionExportData(ExportConnectionRequest objExportConnection)
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<ExportConnectionReport>("fn_get_connection_report", new
                {
                    p_source_system_id = objExportConnection.p_source_system_id,
                    p_source_type = objExportConnection.p_source_type,
                    p_is_source_connected = objExportConnection.p_is_source_connected,
                    p_connecting_entity = objExportConnection.p_connecting_entity,
                    p_destination_system_id = objExportConnection.p_destination_system_id,
                    p_destination_type = objExportConnection.p_destination_type,
                    p_is_destination_connected = objExportConnection.p_is_destination_connected
                }, true).ToList();
                return lstItems != null ? lstItems : new List<ExportConnectionReport>();
            }
            catch { throw; }
        }

        public List<T> GetAssociationExportData<T>(int systemId, string entityType) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_get_export_associate_entity", new { p_system_id = systemId, p_entity_type = entityType }, true).ToList();
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }
        public List<T> GetRouteAssociationExportData<T>(int systemId, string entityType) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_get_near_by_route", new { p_system_id = systemId, p_entity_type = entityType }, true).ToList();
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

        public List<T> DownloadBulkAssociationLog<T>(int systemId, int userId) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_bulk_association_log", new { p_system_id = systemId, p_user_id = userId }, true).ToList();
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

        public bool BulkAssociationRequestLog(int systemId)
        {
            try
            {
                var value = repo.ExecuteProcedure<bool>("fn_bulk_assocation_request_logs", new { p_system_id = systemId }).FirstOrDefault();
                return value;
            }
            catch { throw; }
        }
        //public List<LegendDetail> GetLegendDetail(int user_id)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<LegendDetail>("fn_get_legend_details", new { p_userid = user_id });
        //    }
        //    catch { throw; }
        //}
        public List<LegendDetail> GetLegendDetail(int user_id, int role_Id)
        {
            try
            {
                return repo.ExecuteProcedure<LegendDetail>("fn_get_legend_details", new { p_userid = user_id, p_role_id = role_Id });
            }
            catch { throw; }
        }
        public List<LegendDetail> GetMobileLegendDetail(int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<LegendDetail>("fn_get_mobile_legend_details", new { p_userid = user_id });
            }
            catch { throw; }
        }
        //public List<LegendDetail> GetLegendByGeom(string geom, string filter)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<LegendDetail>("fn_get_legend_by_geom", new { p_geom = geom, p_filter = filter });
        //    }
        //    catch { throw; }
        //}
        public List<LegendDetail> GetLegendByGeom(string geom, string filter, int role_id)
        {
            try
            {
                return repo.ExecuteProcedure<LegendDetail>("fn_get_legend_by_geom", new { p_geom = geom, p_filter = filter, p_role_id = role_id });
            }
            catch { throw; }
        }
        //public List<LegendDetail> GetLegendDetail(int user_id, string geom, string selection_type)
        //{
        //	try
        //	{
        //		return repo.ExecuteProcedure<LegendDetail>("fn_get_entity_icon_details", new { p_geom = geom, p_userid = user_id, p_selectiontype = selection_type });
        //	}
        //	catch { throw; }
        //}

        public List<LegendDetail> GetMobileLegendDetail(int user_id, string group_name)
        {
            try
            {
                var result = repo.ExecuteProcedure<LegendDetail>("fn_api_get_mobile_legend_details", new { p_userid = user_id, p_group_name = group_name }, true);
                return result != null ? result : new List<LegendDetail>();

            }
            catch { throw; }
        }



        public List<Dictionary<string, string>> GetAuditEntityDetailById(int systemid, string eType, int currentPage, int pageSize, string sort, string orderBy)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_audit_entitydetail_by_id",
                    new
                    {
                        P_PAGENO = currentPage,
                        P_PAGERECORD = pageSize,
                        P_SORTCOLNAME = sort,
                        P_SORTTYPE = orderBy,
                        p_systemid = systemid,
                        p_entity_name = eType.ToString()
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetAuditEntityGeometryDetail(int systemid, string eType, int currentPage, int pageSize, string sort, string orderBy)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_audit_entity_geometry_detail",
                    new
                    {
                        P_PAGENO = currentPage,
                        P_PAGERECORD = pageSize,
                        P_SORTCOLNAME = sort,
                        P_SORTTYPE = orderBy,
                        p_systemid = systemid,
                        p_entity_name = eType.ToString()
                    }, true);
                return lst;
            }
            catch { throw; }
        }


        public List<Dictionary<string, string>> GetAuditSiteInfoById(int siteId, string lmcType, int currentPage, int pageSize, string sort, string orderBy)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_audit_sitedetail_by_id",
                    new
                    {
                        P_PAGENO = currentPage,
                        P_PAGERECORD = pageSize,
                        P_SORTCOLNAME = sort,
                        P_SORTTYPE = orderBy,
                        p_siteId = siteId,
                        p_lmcType = lmcType.ToString()
                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetAuditLMCInfoById(int LMCId, string lmcType, int currentPage, int pageSize, string sort, string orderBy)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_audit_LMCdetail_by_id",
                    new
                    {
                        P_PAGENO = currentPage,
                        P_PAGERECORD = pageSize,
                        P_SORTCOLNAME = sort,
                        P_SORTTYPE = orderBy,
                        p_lmcId = LMCId,
                        p_lmcType = lmcType.ToString()
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetAuditLandBaseInfoById(int id, int landBaseLayerId, int currentPage, int pageSize, string sort, string orderBy)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_landbase_get_audit_by_id",
                    new
                    {
                        P_PAGENO = currentPage,
                        P_PAGERECORD = pageSize,
                        P_SORTCOLNAME = sort,
                        P_SORTTYPE = orderBy,
                        p_id = id,
                        p_landBaseLayerId = landBaseLayerId
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetFiberLinkHistoryById(int LinkSystemId, int currentPage, int pageSize, string sort, string orderBy)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_fiber_link_audit_by_id",
                    new
                    {
                        P_PAGENO = currentPage,
                        P_PAGERECORD = pageSize,
                        P_SORTCOLNAME = sort,
                        P_SORTTYPE = orderBy,
                        p_link_system_id = LinkSystemId
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<AccessoriesAuditModel> GetAuditAccessoriesById(int accessoriesId, int currentPage, int pageSize, string sort, string orderBy)
        {
            try
            {
                var lst = repo.ExecuteProcedure<AccessoriesAuditModel>("fn_get_audit_accessories_by_id",
                    new
                    {
                        P_PAGENO = currentPage,
                        P_PAGERECORD = pageSize,
                        P_SORTCOLNAME = sort,
                        P_SORTTYPE = orderBy,
                        p_accessories_id = accessoriesId
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public DbMessage deleteEntity(int systemId, string entityType, string geomType, int userId)
        {
            try
            {
                try
                {
                    return repo.ExecuteProcedure<DbMessage>("fn_delete_entity", new { p_system_id = systemId, p_entity_type = entityType, p_geom_type = geomType, p_user_id = userId }).FirstOrDefault();

                }
                catch { throw; }
            }
            catch { throw; }
        }
        public DbMessage deleteSite(int siteId, int structureId)
        {
            try
            {
                try
                {
                    return repo.ExecuteProcedure<DbMessage>("fn_delete_site", new { p_siteId = siteId, p_structureId = structureId }).FirstOrDefault();

                }
                catch { throw; }
            }
            catch { throw; }
        }
        public DbMessage deleteLMC(int lmcId, int cableId)
        {
            try
            {
                try
                {
                    return repo.ExecuteProcedure<DbMessage>("fn_delete_lmc", new { p_lmcId = lmcId, p_cableId = cableId }).FirstOrDefault();

                }
                catch { throw; }
            }
            catch { throw; }
        }

        public DbMessage RevertEntityChanges(RevertEntity objRevertEntity)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_nwt_revert_entity_changes", new { p_ticket_id = objRevertEntity.ticketid, p_system_id = objRevertEntity.systemId, p_entity_type = objRevertEntity.entityType, p_action = objRevertEntity.action, p_userId = objRevertEntity.userid, p_geom_type = objRevertEntity.geomType }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage SubmitNetwork(SubmitNetworkParam objSubmitNetwork)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_nwt_submit_entities", new
                {
                    p_ticket_id = objSubmitNetwork.ticket_id,
                    p_entity_ids_types = objSubmitNetwork.entity_ids_types,
                    p_remarks = objSubmitNetwork.remarks,
                    p_source = objSubmitNetwork.source,
                    p_userId = objSubmitNetwork.user_id,
                    p_status = objSubmitNetwork.status
                }).FirstOrDefault();

            }
            catch { throw; }
        }
        public string getEntityGeom(int systemId, string entityType)
        {
            var value = repo.ExecuteProcedure<string>("fn_get_entity_geom", new { p_system_id = systemId, p_entityType = entityType }).FirstOrDefault();
            return value;
        }
        public string getEntitylatlong(int a_entity_system_id, int b_entity_system_id, string a_entity_type, string b_entity_type)
        {
            return repo.ExecuteProcedure<string>("fn_api_get_entity_latlong", new { a_entity_system_id = a_entity_system_id, b_entity_system_id = b_entity_system_id, a_entity_type = a_entity_type, b_entity_type = b_entity_type }).FirstOrDefault();

        }

        public DbMessage SetAssociationWithSplitentity(int entity_one_system_id, int entity_two_system_id, string entity_one_network_id, string entity_two_network_id, string splitentitytype, int split_entity_system_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_associate_split_entities", new { p_entity_one_system_id = entity_one_system_id, p_entity_two_system_id = entity_two_system_id, p_entity_one_network_id = entity_one_network_id, p_entity_two_network_id = entity_two_network_id, p_entity_type = splitentitytype, p_old_entity_system_id = split_entity_system_id }).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage deleteParentSplitEntity(int systemId, string entityType)
        {
            try
            {
                try
                {
                    return repo.ExecuteProcedure<DbMessage>("fn_delete_split_entity", new { p_system_id = systemId, p_entity_type = entityType }).FirstOrDefault();

                }
                catch { throw; }
            }
            catch { throw; }
        }
        public DbMessage validateEntity(validateEntity entityInfo, bool isNew)
        {
            try
            {
                List<Models.validateEntity> objValidateList = new List<Models.validateEntity>();
                objValidateList.Add(entityInfo);
                return repo.ExecuteProcedure<DbMessage>("fn_validate_entity", new { p_entity_info = JsonConvert.SerializeObject(objValidateList), p_is_new_entity = isNew }).FirstOrDefault();
            }
            catch { throw; }
        }

        public DbMessage validateUnitsOnFloors(validateEntity entityInfo, bool isNew)
        {
            try
            {
                List<Models.validateEntity> objValidateList = new List<Models.validateEntity>();
                objValidateList.Add(entityInfo);
                return repo.ExecuteProcedure<DbMessage>("fn_validate_unitfloor", new { p_entity_info = JsonConvert.SerializeObject(objValidateList), p_is_new_entity = isNew }).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<DropDownMaster> GetTicketDropdownList(string ddType)
        {
            try
            {
                return repo.GetAll(m => m.dropdown_type == ddType).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<T> GetROWAssociationExportData<T>(int systemId, string entityType) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_row_get_export_associate_entity", new { p_system_id = systemId, p_entity_type = entityType }, true).ToList();
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }
        public List<attachment_type_master> getAttachment(string entityType, string category)
        {
            try
            {
                return repo.ExecuteProcedure<attachment_type_master>("fn_get_attachment_type", new { p_layer_name = entityType, p_category = category }, true).ToList();
            }
            catch { throw; }
        }
        public string getNewNetworkcode(string etype, string entity_network_id)
        {
            var value = repo.ExecuteProcedure<string>("fn_get_new_network_code", new { etype = etype, entity_network_id = entity_network_id }).FirstOrDefault();
            return value;
        }
        public DbMessage SaveChangeNetworkCode(ChangeNetworkCode objparam)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_Save_New_Network_Code",
                    new
                    {
                        p_entity_type = objparam.layer_name,
                        p_old_network_id = objparam.old_network_id,
                        p_new_network_id = objparam.new_network_id,
                        p_updated_by = objparam.created_by,
                        p_remarks = objparam.remarks,
                        p_entity_new_network_id = ""
                    }).FirstOrDefault();

            }
            catch { throw; }
        }
        public List<entityValidationStatus> getNetworkIdDependency(string etype, string old_network_id)
        {
            try
            {
                return repo.ExecuteProcedure<entityValidationStatus>("fn_check_entity_dependency", new { p_system_id = 0, p_entity_type = etype, p_old_network_id = old_network_id }, true).ToList();
            }
            catch { throw; }
        }

        public List<Mapping> GetMapping(string layerName)
        {
            try
            {
                return repo.ExecuteProcedure<Mapping>("fn_uploader_get_entity_template", new { p_entity_type = layerName }, true);
            }
            catch { throw; }
        }
        public List<Mapping> GetRegionProvinceMapping(string boundary_type)
        {
            try
            {
                return repo.ExecuteProcedure<Mapping>("fn_uploader_get_RegionProvince_template", new { p_entity_type = boundary_type }, true);
            }
            catch { throw; }
        }
        public List<Mapping> GetLandBaseMapping(string layerName)
        {
            try
            {
                return repo.ExecuteProcedure<Mapping>("fn_landbase_uploader_get_entity_template", new { p_entity_type = layerName }, true);
            }
            catch { throw; }
        }
        public List<AccessoriesInfoModel> GetAccessoriesByParent(AccessoriesViewModel model)
        {
            try
            {

                var lst = repo.ExecuteProcedure<AccessoriesInfoModel>("fn_get_accessories_details",
                    new
                    {
                        p_searchby = Convert.ToString(model.objFilterAttributes.searchBy),
                        p_searchbytext = model.objFilterAttributes.searchText,
                        P_PAGENO = model.objFilterAttributes.currentPage,
                        P_PAGERECORD = model.objFilterAttributes.pageSize,
                        P_SORTCOLNAME = model.objFilterAttributes.sort,
                        P_SORTTYPE = model.objFilterAttributes.orderBy,
                        p_totalrecords = model.objFilterAttributes.totalRecord,
                        p_systemid = model.objFilterAttributes.parent_systemId,
                        p_entity_name = model.objFilterAttributes.parent_entityType.ToString()


                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public List<AccessoriesReportModel> GetAccessoriesByParent(FilterAccessoriesAttr filterData)
        {
            try
            {

                var lst = repo.ExecuteProcedure<AccessoriesReportModel>("fn_get_accessories_details",
                    new
                    {
                        p_searchby = Convert.ToString(filterData.searchBy),
                        p_searchbytext = filterData.searchText,
                        P_PAGENO = filterData.currentPage,
                        P_PAGERECORD = filterData.pageSize,
                        P_SORTCOLNAME = filterData.sort,
                        P_SORTTYPE = filterData.orderBy,
                        p_totalrecords = filterData.totalRecord,
                        p_systemid = filterData.parent_systemId,
                        p_entity_name = filterData.parent_entityType.ToString()


                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public ParentInfo getParentInfo(NetworkCodeIn objIn)
        {
            try
            {
                return repo.ExecuteProcedure<ParentInfo>("fn_get_deassociate_paarent", new { etype = objIn.eType, gtype = objIn.gType, parent_sysid = 0, parent_etype = "", geometry = objIn.eGeom }).FirstOrDefault();

            }
            catch { throw; }
        }

        public bool insertApiLogs(Api_Logs objapilogs)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_insert_api_request_logs", new
                {
                    p_entity_system_id = objapilogs.system_id,
                    p_entity_type = objapilogs.entity_type,
                    p_source_ref_id = objapilogs.source_ref_id,
                    p_source_ref_type = objapilogs.source_ref_type,
                    p_attribute_info = objapilogs.attribute_info,
                    p_userid = objapilogs.user_id,
                    P_header_attribute = objapilogs.header_attribute,


                }).FirstOrDefault();
                return response.status;
            }
            catch { throw; }
        }
        public List<UserDashboard> GetTeamStatus(int manager_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<UserDashboard>("fn_get_team_status",
                    new
                    {
                        p_managerid = manager_id
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public DbMessage validateEntityGeom(string geomType, string entityType, string longlat, int userId, int parentId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_validate_entity_geom",
                new { p_geom_type = geomType, p_entity_type = entityType, p_longlat = longlat, p_userid = userId, p_parent_system_id = parentId }).FirstOrDefault();

            }
            catch { throw; }
        }
        public List<CloneDependent> GetCloneDependent(string entityType, int parentId)
        {
            try
            {
                var lst = repo.ExecuteProcedure<CloneDependent>("fn_get_child_clone_list",
                    new
                    {
                        p_entity_name = entityType,
                        p_system_id = parentId
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public DbMessage GetCentroidByGeom(string entity_geom, string geom_type)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_get_centroid_by_entity_geom", new { p_entity_geom = entity_geom, p_geom_type = geom_type }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage GetJpBoundaryByGeom(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_get_jp_boundary_by_geom", new { p_geom = geom }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage ResetDesignID(string pEntityType, int pSystemId, int p_user_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_auto_codification_reset", new { p_entity_id = pSystemId, p_entity_type = pEntityType, P_userid = p_user_id }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage ResetPartialDesignID(string pEntityType, int pSystemId, int p_user_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_auto_codification_partial_reset", new { p_entity_id = pSystemId, p_entity_type = pEntityType, P_userid = p_user_id }).FirstOrDefault();

            }
            catch { throw; }
        }
        public List<DropDownMaster> GetPortRatio(string splitterType)
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_splitter_port_ratio", new { p_splitterType = splitterType }).ToList();
            }
            catch { throw; }
        }

        public DbMessage updateGeojsonEntityAttribute(int system_id, string entityType, int? province_id, int action_id)
        {
            try
            {

                return repo.ExecuteProcedure<DbMessage>("fn_geojson_update_entity_attribute", new { p_system_id = system_id, p_entity_type = entityType, p_province_id = province_id, p_action_id = action_id, is_location_edit = false }, false).FirstOrDefault();
            }
            catch { throw; }
        }
        public RouteCreation createRouteId(int system_id, string entityType)
        {
            try
            {

                return repo.ExecuteProcedure<RouteCreation>("FN_CREATE_ROUTE_ID", new { P_SYSTEM_ID = system_id, P_ENTITY_TYPE = entityType, P_ASSOCIATE_SYSTEM_ID = 0, P_ASSOCIATE_ENTITY_TYPE = "Cable", }, false).FirstOrDefault();
            }
            catch { throw; }
        }
        public UserRegionProvince GetRegionProvinceBasedOnLocation(string geom, int userId)
        {
            UserRegionProvince objUserRegionProvince = new UserRegionProvince();
            try
            {
                objUserRegionProvince = repo.ExecuteProcedure<UserRegionProvince>("fn_get_GetRegionProvinceBasedOnLocation", new { p_geom = geom, p_userId = userId }).FirstOrDefault();
                return objUserRegionProvince;

            }
            catch { throw; }
        }
        public PageMessage GetValidatePointGeometry(int systemId, string entityType, string latitude, string longitude, int? region_id, int? province_id)
        {
            PageMessage response;
            try
            {
                response = repo.ExecuteProcedure<PageMessage>("fn_validate_point_geometry", new { p_system_id = systemId, p_entity_type = entityType, p_latitude = latitude, p_longitude = longitude, p_region_id = region_id, p_province_id = province_id }).FirstOrDefault();
                return response;

            }
            catch { throw; }
        }
        public List<DropDownMaster> GetToplogyDropDownList(string ddType)
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_dropdownlist", new { entitytype = "", dropdownType = ddType });
            }
            catch { throw; }
        }
    }
    public class DAMisce : Repository<EmailSettingsModel>
    {
        public EmailSettingsModel getEmailSettings()
        {

            try
            {
                return repo.GetAll().FirstOrDefault();
            }
            catch { throw; }
        }
        public EmailSettingsModel updateEmailSettings(EmailSettingsModel obj, int id)
        {
            try
            {
                var objEmailSettings = repo.Get(m => m.id == id);
                if (objEmailSettings != null)
                {
                    objEmailSettings.smtp_host = obj.smtp_host;
                    objEmailSettings.port = obj.port;
                    objEmailSettings.email_address = obj.email_address;
                    objEmailSettings.email_password = obj.email_password;
                    objEmailSettings.auth = obj.auth;
                    objEmailSettings.enablessl = obj.enablessl;
                    objEmailSettings.usedefaultcredentials = obj.usedefaultcredentials;

                    return repo.Update(objEmailSettings);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<EmailSettingsModel> getAllEmailData()
        {

            try
            {
                return repo.GetAll().ToList();
            }
            catch { throw; }
        }

        public EmailSettingsModel SaveEmailSettings(EmailSettingsModel objEmailSettingsModel, string EncodePassword, int userid)
        {
            try
            {
                if (objEmailSettingsModel.id != 0)
                {
                    objEmailSettingsModel.email_password = EncodePassword;
                    objEmailSettingsModel.modified_by = userid;
                    objEmailSettingsModel.auth = true;
                    objEmailSettingsModel.modified_on = DateTimeHelper.Now;
                    return repo.Update(objEmailSettingsModel);

                }
                else
                {
                    objEmailSettingsModel.email_password = EncodePassword;
                    objEmailSettingsModel.auth = true;
                    objEmailSettingsModel.created_by = userid;
                    objEmailSettingsModel.modified_by = userid;
                    objEmailSettingsModel.modified_on = DateTimeHelper.Now;
                    return repo.Insert(objEmailSettingsModel);
                }

            }

            catch { throw; }

        }
    }

    public class DAAssociateEntity : Repository<AssociateEntity>
    {
        public void SaveEntityAssociation(string associated_entity_type, int associated_system_id, string entity_network_id, int entity_system_id, string entity_type, int userId)
        {

            try
            {
                if (associated_system_id > 0 && !string.IsNullOrEmpty(associated_entity_type))
                {
                    AssociateEntity objAE = new AssociateEntity();
                    objAE.associated_entity_type = associated_entity_type;
                    objAE.associated_system_id = associated_system_id;
                    EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), entity_type);
                    var assInfo = new DAMisc().GetEntityDetailById<dynamic>(associated_system_id, enType, userId);
                    objAE.associated_network_id = assInfo.network_id; ;
                    objAE.entity_network_id = entity_network_id;
                    objAE.entity_system_id = entity_system_id;
                    objAE.entity_type = entity_type;
                    objAE.created_by = userId;
                    objAE.created_on = DateTimeHelper.Now;
                    SaveAssociation(objAE);
                }
            }
            catch { throw; }
        }
        public AssociateEntity SaveAssociation(AssociateEntity objAssociate)
        {
            try
            {
                var obj = repo.GetById(m => m.system_id == objAssociate.system_id);
                if (obj != null)
                {
                    if (objAssociate.entity_system_id == 0)
                    {
                        var response = repo.Delete(obj);
                        return new AssociateEntity();
                    }
                    else
                    {
                        obj.entity_type = objAssociate.entity_type;
                        obj.entity_system_id = objAssociate.entity_system_id;
                        obj.entity_network_id = objAssociate.entity_network_id;
                        var response = repo.Update(obj);
                        repo.ExecuteProcedure<DbMessage>("fn_update_associate_entity_geom", new { p_associated_system_id = objAssociate.associated_system_id, p_associated_entity_type = objAssociate.associated_entity_type, p_system_id = objAssociate.entity_system_id, p_entity_type = objAssociate.entity_type, p_userid = objAssociate.created_by }).FirstOrDefault();
                        return response;
                    }

                }
                else
                {
                    var response = repo.Insert(objAssociate);
                    repo.ExecuteProcedure<DbMessage>("fn_update_associate_entity_geom", new { p_associated_system_id = objAssociate.associated_system_id, p_associated_entity_type = objAssociate.associated_entity_type, p_system_id = objAssociate.entity_system_id, p_entity_type = objAssociate.entity_type, p_userid = objAssociate.created_by }).FirstOrDefault();
                    return response;
                }

            }
            catch { throw; }
        }

        public AssociateEntity SaveAssociationForEndToEnd(AssociateEntity objAssociate)
        {
            try
            {
                var obj = repo.GetById(m => m.system_id == objAssociate.system_id);
                if (obj != null)
                {
                    if (objAssociate.entity_system_id == 0)
                    {
                        var response = repo.Delete(obj);
                        return new AssociateEntity();
                    }
                    else
                    {
                        obj.entity_type = objAssociate.entity_type;
                        obj.entity_system_id = objAssociate.entity_system_id;
                        obj.entity_network_id = objAssociate.entity_network_id;
                        var response = repo.Update(obj);
                        repo.ExecuteProcedure<DbMessage>("fn_update_associate_entity_geom_end_to_end", new { p_associated_system_id = objAssociate.associated_system_id, p_associated_entity_type = objAssociate.associated_entity_type, p_system_id = objAssociate.entity_system_id, p_entity_type = objAssociate.entity_type, p_userid = objAssociate.created_by }).FirstOrDefault();
                        return response;
                    }

                }
                else
                {
                    var response = repo.Insert(objAssociate);
                    repo.ExecuteProcedure<DbMessage>("fn_update_associate_entity_geom_end_to_end", new { p_associated_system_id = objAssociate.associated_system_id, p_associated_entity_type = objAssociate.associated_entity_type, p_system_id = objAssociate.entity_system_id, p_entity_type = objAssociate.entity_type, p_userid = objAssociate.created_by }).FirstOrDefault();
                    return response;
                }

            }
            catch { throw; }
        }
        public List<AssociateEntity> getAssociateEntity(int system_id, string entityType)
        {
            try
            {
                return repo.GetAll(m => ((m.associated_system_id == system_id && m.associated_entity_type.ToUpper() == entityType.ToUpper()) || (m.entity_system_id == system_id && m.entity_type.ToUpper() == entityType.ToUpper())) && m.is_termination_point == false).ToList();
            }
            catch { throw; }
        }
        public void UpdateTPAssociation(int systemId, string entityType, int userId)
        {

            repo.ExecuteProcedure<DbMessage>("fn_update_tp_association", new
            {
                p_system_id = systemId,
                p_entity_type = entityType,
                p_user_id = userId
            });
        }
    }
    public class DASplitterAllocationPortInfo : Repository<IspPortInfo>
    {
        public List<IspPortInfo> GetPortInfo(int system_id, string entity_type)
        {
            try
            {
                //return repo.GetAll().Where(x => x.parent_system_id == system_id && x.parent_entity_type == entity_type).ToList();
                return repo.ExecuteProcedure<IspPortInfo>("fn_get_entity_port_info", new { p_systemid = system_id, p_entity_type = entity_type }).ToList();

            }
            catch { throw; }

        }

        public IspPortInfo SaveSplitterCustomerMapping(int parent_system_id, int parent_port_number, string parent_entity_type, int destination_system_id, int user_id, string destination_network_id, string destination_entity_type)
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<IspPortInfo>("fn_api_update_entity_mapping", new { p_parent_system_id = parent_system_id, p_parent_port_no = parent_port_number, p_parent_entity_type = parent_entity_type, p_destination_system_id = destination_system_id, p_user_id = user_id, p_destination_network_id = destination_network_id, p_destination_entity_type = destination_entity_type }, true).FirstOrDefault();
                return lstItems != null ? lstItems : new IspPortInfo();
            }
            catch { throw; }
        }

        public DbMessage SaveCustomerAssociation(string destination_network_id, int p_box_id, string p_box_type, int source_system_id, int source_port_number, int user_id, string jsonlstWCRMatrial, string rfs_type, bool is_box_changed, int ticket_id, string routeGeom)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_auto_provisioning", new
                {
                    p_can_id = destination_network_id,
                    p_box_id = p_box_id,
                    p_box_type = p_box_type,
                    p_splitter_id = source_system_id,
                    p_splitter_port_no = source_port_number,
                    p_user_id = user_id,
                    p_wcr_list = jsonlstWCRMatrial,
                    p_rfs_type = rfs_type,
                    p_is_box_changed = is_box_changed,
                    p_ticket_id = ticket_id,
                    p_line_geom = routeGeom
                }).FirstOrDefault();
                //return lstItems != null ? lstItems : new IspPortInfo(); 
            }
            catch { throw; }
        }
        public CPEInstallation SaveCPEInstallation(string source_network_id, string source_entity_type, int source_port_number, int user_id, string latitude, string longitude)
        {
            try
            {
                return repo.ExecuteProcedure<CPEInstallation>("fn_save_cpe_installation", new
                {
                    p_source_network_id = source_network_id,
                    p_source_entity_type = source_entity_type,
                    p_source_port_number = source_port_number,
                    p_user_id = user_id,
                    p_latitude = latitude,
                    p_longitude = longitude
                }).FirstOrDefault();
            }
            catch { throw; }
        }

        public DistributionBoxInfo GetDistributionBoxInfo(int user_id, double latitude, double longitude, string box_type)
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<DistributionBoxInfo>("fn_api_get_nearby_db_and_splitter_info", new { p_userid = user_id, p_latitude = latitude, p_longitude = longitude, p_boxtype = box_type }, true).FirstOrDefault(); ;
                return lstItems != null ? lstItems : new DistributionBoxInfo();
            }
            catch { throw; }

        }
        public List<DBInfo> GetNearByDBInfo(string latitude, string longitude, string box_type, string rfs_type, int bufferInMtr, string module_abbr)
        {
            try
            {
                return repo.ExecuteProcedure<DBInfo>("fn_api_get_nearby_db_info", new { p_latitude = latitude, p_longitude = longitude, p_boxtype = box_type, p_bufferInMtr = bufferInMtr, p_rfs_type = rfs_type, p_module_abbr = module_abbr }, true);

            }
            catch { throw; }

        }
        public List<UserModule> GetUserModule(int userId, string UserType)
        {
            try
            {
                return repo.ExecuteProcedure<UserModule>("fn_get_user_module", new { p_userid = userId, p_userType = UserType }, true);

            }
            catch { throw; }

        }

        //public List<UserModule> GetRoleModule(int role_id)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<UserModule>("fn_get_role_module_mapping", new { p_role_id = role_id }, true).ToList();

        //    }
        //    catch { throw; }

        //}
        public List<UserModule> GetRoleModule(int role_id)
        {
            try
            {
                return repo.ExecuteProcedure<UserModule>("fn_get_role_module_mapping", new { p_role_id = role_id }, false);

            }
            catch { throw; }

        }
        public List<UserModule> GetAllModules()
        {
            try
            {
                return repo.ExecuteProcedure<UserModule>("fn_get_all_modules", null, false);

            }
            catch { throw; }

        }

        public List<UserModule> GetUserModuleMasterList()
        {
            try
            {
                return repo.ExecuteProcedure<UserModule>("fn_get_user_module_master", null, false);

            }
            catch { throw; }

        }
        public DistributionBoxEntityInfo GetDistributionBoxEntityInfo(int user_id, int system_id, string entity_type)
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<DistributionBoxEntityInfo>("fn_api_get_distribution_box_entity_info", new { p_userid = user_id, p_system_id = system_id, p_entity_type = entity_type }, true).FirstOrDefault();
                return lstItems != null ? lstItems : new DistributionBoxEntityInfo();
            }
            catch { throw; }

        }

        public SplitterPortInfo GetSplitterPortInfo(int user_id, int system_id, string entity_type, string can_id)
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<SplitterPortInfo>("fn_api_get_splitter_port_info", new { p_userid = user_id, p_system_id = system_id, p_entity_type = entity_type, p_can_id = can_id }, true).FirstOrDefault();
                return lstItems != null ? lstItems : new SplitterPortInfo();
            }
            catch { throw; }

        }

        public int UpdateSplitterAllocationPort(List<IspPortInfo> PortInfo)
        {
            return repo.Update(PortInfo);
        }

        public List<ViewServiceFacilityJobOrder> GetRoleServiceFacilityJobOrder(int role_id)
        {
            try
            {
                return repo.ExecuteProcedure<ViewServiceFacilityJobOrder>("fn_get_role_Service_facility_mapping", new { p_role_id = role_id }, false);

            }
            catch { throw; }

        }

        public List<ViewServiceFacilityJobOrder> GetUserServiceFacilityJobOrder(int role_id, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<ViewServiceFacilityJobOrder>("fn_get_user_role_service_facility_mapping", new { p_role_id = role_id, p_user_id = user_id }, false);

            }
            catch { throw; }

        }
        public List<ViewServiceFacilityJobOrder> GetUserServiceFacilityJobOrderFE(string rm_id, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<ViewServiceFacilityJobOrder>("fn_get_fe_user_service_facility_mapping", new { p_rm_id = rm_id, p_user_id = user_id }, false);

            }
            catch { throw; }

        }

    }

    //public class DAModuleMaster : Repository<UserModule>
    //{
    //    public List<UserModule> GetAllModule()
    //    {
    //        return repo.GetAll().Select(m =>new UserModule()
    //        {
    //            Id = m.Equals("id"),
    //            module_name = m.module_name,
    //            module_description = m.module_description,
    //            icon_content = m.icon_content,
    //            icon_class = m.icon_class,
    //            type = m.type,
    //            module_abbr = m.module_abbr,
    //            parent_module_id = m.parent_module_id,
    //            module_sequence = m.module_sequence
    //        }).ToList();
    //    }
    //}


    public class DANetworkStatus : Repository<object>
    {

        // info window update
        public DbMessage UpdateNetworkStatus(int systemid, string entityType, string currentNetworkStatus, int userid)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_update_entity_network_status", new { p_system_id = systemid, p_entity_type = entityType, p_current_status = currentNetworkStatus, p_userid = userid }).FirstOrDefault();

                bool chkstatus = false;
                if (response.status)
                {
                    chkstatus = true;
                }

                return response;
            }
            catch { throw; }

        }
        // Bulk operation 
        public bool ConvertBulkNetworkEntity(string geom, int userId, string geomType, double buff_Radius, string currStatus, string entityType, string changeNetworkStatus, string entitySubtype)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_update_entity_wise_network_status", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_radius = buff_Radius, p_current_status = currStatus, p_entity_type = entityType, p_change_network_status = changeNetworkStatus, p_entity_sub_type = entitySubtype }).FirstOrDefault();

                bool chkstatus = false;
                if (response.status)
                {
                    chkstatus = true;
                }

                return chkstatus;
            }
            catch { throw; }

        }
        public bool BulkAsBuiltDormant(string geom, int userId, string geomType, double buff_Radius, string currStatus, string newStatus)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_bulk_update_network_status", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_radius = buff_Radius, p_current_status = currStatus, p_new_status = newStatus }).FirstOrDefault();

                bool chkstatus = false;
                if (response.status)
                {
                    chkstatus = true;
                }

                return chkstatus;
            }
            catch { throw; }
        }

        public bool isNetworkIdExist(string networkId, string entityType, int userid)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_get_fiber_link_entity_network_id", new { p_network_id = networkId, p_entity_type = entityType, p_userid = userid }).FirstOrDefault();

                bool chkstatus = false;
                if (response.status)
                {
                    chkstatus = true;
                }

                return chkstatus;
            }
            catch { throw; }

        }

    }


    public class DATerminationPointsMaster : Repository<VWTerminationPointMaster>
    {

        public List<VWTerminationPointMaster> getOSPTerminationPoints(string layerName)
        {
            try
            {
                return repo.GetAll(m => m.layer_name.ToUpper() == layerName.ToUpper() && m.is_osp_tp == true && m.is_enabled == true).ToList();
            }
            catch { throw; }
        }
        public List<VWTerminationPointMaster> getISPTerminationPoints(string layerName)
        {
            try
            {
                return repo.GetAll(m => m.layer_name.ToUpper() == layerName.ToUpper() && m.is_isp_tp == true && m.is_enabled == true).ToList();
            }
            catch { throw; }
        }
    }
    public class FMSDAMisce : Repository<FMSMaster>
    {
        public string Getnetworkid(int system_id)
        {
            try
            {
                var objExisiting = repo.GetById(m => m.system_id == system_id).network_id;
                return objExisiting;
            }
            catch { throw; }
        }
    }
    public class DAFormInputSettings : Repository<FormInputSettings>
    {
        public List<FormInputSettings> getformInputSettings()
        {
            try
            {
                return repo.GetAll().ToList();
            }
            catch { throw; }
        }
        public List<string> getDistinctFormNames()
        {
            try
            {
                var res = repo.GetAll().Select(m => m.form_name).Distinct().ToList();
                return res;
            }
            catch { throw; }
        }

        public List<FormInputSettings> getformInputSettings(string formName)
        {
            try
            {
                var res = repo.GetAll(m => m.form_name.ToUpper().Trim() == formName.ToUpper().Trim()).ToList();
                return res;
            }
            catch { throw; }
        }
        public bool SaveFormInputSettings(vwFormInputSettings objFormInputSettings)
        {
            try
            {
                var lstExisitingInputSettings = repo.GetAll(m => m.form_name.ToUpper().Trim() == objFormInputSettings.formName.ToUpper().Trim()).ToList();
                foreach (var item in objFormInputSettings.lstFormInputSettings)
                {
                    var itemToChange = lstExisitingInputSettings.FirstOrDefault(d => d.id == item.id);
                    if (itemToChange != null)
                    {
                        itemToChange.modified_by = objFormInputSettings.user_id;
                        itemToChange.modified_on = DateTimeHelper.Now;
                        itemToChange.is_active = item.is_active;
                    }
                }
                repo.Update(lstExisitingInputSettings);
                return true;
            }
            catch { throw; }
        }
    }
    public class DAPortStatusColor : Repository<portStatusMaster>
    {
        public List<portStatusMaster> getPortStatus()
        {
            return repo.GetAll().Where(s => s.is_active == true).OrderBy(m => m.system_id).ToList();
        }
        public List<portStatusMaster> getPortStatusFiber()
        {
            return repo.GetAll().Where(s => s.is_active == true && s.is_manual_status == true).OrderBy(m => m.system_id).ToList();
        }
    }

    public class DAPlan : Repository<NearestMahhole>
    {
        public List<NetworkPlanning> GetPlanDetails(NetworkPlanningDataFilter objExtnlDtaFilter, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<NetworkPlanning>("fn_get_network_planning_list", new
                {
                    p_user_id = user_id,
                    p_searchby = objExtnlDtaFilter.searchBy,
                    p_searchtext = objExtnlDtaFilter.searchText,
                    p_pageno = objExtnlDtaFilter.currentPage,
                    p_pagerecord = objExtnlDtaFilter.pageSize,
                    p_sortcolname = objExtnlDtaFilter.orderBy,
                    p_sorttype = objExtnlDtaFilter.sort,
                    p_totalrecords = objExtnlDtaFilter.totalRecord
                }, true);
                return res;
            }
            catch { throw; }
        }

        public List<BackBonePlanning> GetBackbonePlanHistoryDetails(BackBonePlanningDataFilter objExtnlDtaFilter, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<BackBonePlanning>("fn_backbone_get_plan_list", new
                {
                    p_user_id = user_id,
                    p_searchby = objExtnlDtaFilter.searchBy,
                    p_searchtext = objExtnlDtaFilter.searchText,
                    p_pageno = objExtnlDtaFilter.currentPage,
                    p_pagerecord = objExtnlDtaFilter.pageSize,
                    p_sortcolname = objExtnlDtaFilter.orderBy,
                    p_sorttype = objExtnlDtaFilter.sort,
                    p_totalrecords = objExtnlDtaFilter.totalRecord
                }, true);
                return res;
            }
            catch { throw; }
        }


        public string GetPlanElement(int plan_id)
        {
            try
            {
                var json = repo.ExecuteProcedure<string>("fn_get_network_planning_network", new { p_plan_id = plan_id }, false)[0];
                return json.ToString();
            }
            catch { throw; }
        }
        public dynamic GetNearByEndPoint(double lat, double lng, string Entity_Type, double End_point_buffer)
        {
            dynamic lst = repo.ExecuteProcedure<dynamic>("fn_get_auto_planning_near_by_end_point", new { lat = lat, lng = lng, entity_type = Entity_Type, buffer = End_point_buffer }, true);
            return lst;
        }
        public List<NearestMahhole> getNearestManholes(string buildingIDs)
        {
            try
            {
                return repo.ExecuteProcedure<NearestMahhole>("fn_get_nearest_manholes", new { buildingIDs = buildingIDs }, false);
            }
            catch { throw; }
        }

        public List<DbMessage> processPlan(int building_id, int manhole_id, string geom, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<DbMessage>("fn_poc_auto_network_planning", new { p_manhole_system_id = manhole_id, p_building_system_id = building_id, p_line_geom = geom, p_user_id = user_id });
                return res;
            }
            catch { throw; }
        }
        public List<DbMessage> Point2PointPlan(string geom, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<DbMessage>("fn_poc_point_to_point_auto_network_planning", new { p_line_geom = geom, p_user_id = user_id });
                return res;
            }
            catch { throw; }
        }

        public List<PlanBom> GetPlanBomByPlanId(int plan_id, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<PlanBom>("fn_network_planning_get_plan_bom_list", new { p_plan_id = plan_id, p_user_id = user_id }, true);
                return res;
            }
            catch { throw; }
        }
        public List<BackBonePlanBom> GetBackBonePlanBomByPlanId(int plan_id, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<BackBonePlanBom>("fn_backbone_get_plan_bom", new { p_plan_id = plan_id, p_user_id = user_id }, true);
                return res;
            }
            catch { throw; }
        }
        public List<BackBonePlanKMLBom> GetBackBonePlanBomKMLByPlanId(int plan_id, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<BackBonePlanKMLBom>("fn_backbone_get_plan_kml_bom", new { p_plan_id = plan_id, p_user_id = user_id }, true);
                return res;
            }
            catch { throw; }
        }
        public List<PlanBom> PlanBom(NetworkPlanning model, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<PlanBom>("fn_network_planning_get_plan_bom_list", new { p_plan_name = model.plan_name, p_plan_mode = model.planning_mode, p_cable_type = model.cable_type, is_create_trench = model.is_create_trench, is_create_duct = model.is_create_duct, p_line_geom = model.geometry, p_cable_length = model.cable_length, p_distance = model.pole_manhole_distance, p_user_id = user_id, p_temp_plan_id = model.temp_plan_id, p_is_loop_require = model.is_loop_required, p_is_loop_update = model.is_loop_update, p_loop_length = model.loop_length , p_polespecvendor = model.poleSpecVendor , p_manholespecvendor  = model.manholeSpecVendor , p_scspecvendor  = model.spliceclosureSpecVendor, p_loop_span = model.loop_span }, true);
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DbMessageForPlan savePoint2Point(NetworkPlanning objPlan)
        {
            try
            {
                var resp = repo.ExecuteProcedure<DbMessageForPlan>("fn_network_planning_save_auto_planning", new { p_plan_mode = objPlan.planning_mode, p_cable_type = objPlan.cable_type, is_create_trench = objPlan.is_create_trench, is_create_duct = objPlan.is_create_duct, p_line_geom = objPlan.geometry, p_cable_length = objPlan.cable_length, p_distance = objPlan.pole_manhole_distance, p_user_id = objPlan.created_by, p_plan_name = objPlan.plan_name, p_startpoint = objPlan.start_point, p_endpoint = objPlan.end_point, p_end_point_type = objPlan.end_point_type, p_end_point_buffer = objPlan.end_point_buffer, p_edit_path = objPlan.edit_path, p_end_point_entity_id = objPlan.end_point_entity, p_end_point_entity_type = objPlan.end_point_type, p_temp_plan_id = objPlan.temp_plan_id, is_loop_required = objPlan.is_loop_required, loop_length = objPlan.loop_length, p_polespecvendor = objPlan.poleSpecVendor, p_manholespecvendor = objPlan.manholeSpecVendor, p_scspecvendor = objPlan.spliceclosureSpecVendor }).FirstOrDefault();
                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DbMessageForBackbonePlan> saveBackbonePlanning(BackBonePlanning objPlan)
        {
            try
            {
                var res = repo.ExecuteProcedure<DbMessageForBackbonePlan>("fn_backbone_save_plan", new { 
                    is_create_trench = objPlan.is_create_trench,
                    is_create_duct = objPlan.is_create_duct, 
                    p_line_geom = objPlan.geometry, 
                    p_user_id = objPlan.created_by, 
                    //p_plan_name = objPlan.plan_name, 
                   // p_startpoint = objPlan.start_point,    
                   // p_endpoint = objPlan.end_point, 
                    plan_id = objPlan.plan_id,                   
                });
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DbMessageForPlan> DeletePlanByPlanId(int plan_id, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<DbMessageForPlan>("fn_network_planning_delete_by_plan_id", new { p_plan_id = plan_id, p_user_id = user_id });
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<DbMessageForPlan> DeleteBackbonePlanByPlanId(int plan_id, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<DbMessageForPlan>("fn_backbone_delete_plan", new { p_plan_id = plan_id, p_user_id = user_id });
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PlanBom> GetPointOfCable(NetworkPlanning objPlan, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<PlanBom>("fn_temp_network_planning", new { p_plan_mode = objPlan.planning_mode, p_cable_type = objPlan.cable_type, is_create_trench = objPlan.is_create_trench, is_create_duct = objPlan.is_create_duct, p_line_geom = objPlan.geometry, p_cable_length = objPlan.cable_length, p_distance = objPlan.pole_manhole_distance, p_user_id = objPlan.created_by, p_plan_name = objPlan.plan_name, p_startpoint = objPlan.start_point, p_endpoint = objPlan.end_point, p_end_point_type = objPlan.end_point_type, p_end_point_buffer = objPlan.end_point_buffer, p_edit_path = objPlan.edit_path, p_end_point_entity_id = objPlan.end_point_entity, p_end_point_entity_type = objPlan.end_point_type, p_temp_plan_id = objPlan.temp_plan_id });
                return res;
            }
            catch { throw; }
        }

        public List<PlanBom> GetTempCableLengthGemo(NetworkPlanning objPlan, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<PlanBom>("fn_Get_line_geom_length", new { p_cable_type = objPlan.cable_type, p_line_geom = objPlan.geometry, p_cable_length = objPlan.cable_length, p_distance = objPlan.pole_manhole_distance, p_user_id = objPlan.created_by });
                return res;
            }
            catch { throw; }
        }
        public OffsetGeometry getCableGeom(string cablegemo, double offset)
        {
            try
            {
                return repo.ExecuteProcedure<OffsetGeometry>("fn_network_planning_get_geom_offset", new { p_cablegemo = cablegemo, p_offset = offset }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public double GetLineLength(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<double>("fn_network_planning_line_length", new { line_geom = geom }, false).FirstOrDefault();
            }
            catch { throw; }
        }
        public BackBoneSitePlanDetails GetNearestSiteList(string geom, double buffer,int planId)
        {
            try
            {
                var jsonResult = repo.ExecuteProcedure<string>("fn_backbone_get_nearest_sites", new { line_geom = geom, buffer = buffer, planid = planId }, false).FirstOrDefault();
                if (jsonResult != null)
                {
                    return JsonConvert.DeserializeObject<BackBoneSitePlanDetails>(jsonResult);
                }
                return null;
            }
            catch { throw; }
        }
        public List<BackBoneSproutFiberDetails> GetBackbonePlanningList(BackBonePlanning plan, int userId)
        {
            try
            {
                var res = repo.ExecuteProcedure<BackBoneSproutFiberDetails>("fn_backbone_draft_network",
                    new
                    {
                        is_create_trench = plan.is_create_trench,
                        is_create_duct = plan.is_create_duct,
                        p_line_geom = plan.geometry,
                        p_user_id = userId,
                        p_plan_name = plan.plan_name,
                        p_startpoint = plan.start_point,
                        p_endpoint = plan.end_point,
                        p_backbone_fiber_type = plan.backbone_fiber_type,
                        p_pole_span = Convert.ToDouble(plan.pole_distance),
                        p_manhole_span = Convert.ToDouble(plan.manhole_distance),
                        v_buffer = plan.buffer,
                        p_threshold = plan.threshold,
                        p_looplength = plan.loop_length,
                        p_is_looprequired = plan.is_loop_required,
                        cable_drum_length = plan.cable_length,
                        p_loop_span = plan.loop_span
                    }, true).ToList();
                return res;
            }
            catch { throw; }
        }

        public void updateSiteLineGeometry(string lineGeom, int systemId, int planId, int userId)
        {
            try
            {
                repo.ExecuteProcedure<SitePlanList>("fn_backbone_update_sprout_network",
                    new { p_lineGeom = lineGeom ,p_systemId = systemId,p_plan_id = planId, p_user_id = userId }, true);                
            }
            catch { throw; }
        }

        public List<BackBoneBOMOBOQResponse> BackBonePlanBom(BackBonePlanning model, int userId)
        {
            try
            {
                var result = repo.ExecuteProcedure<BackBoneBOMOBOQResponse>("fn_backbone_get_plan_bom",
                    new { p_plan_id = model.plan_id,
                        p_user_id= userId, 
                        p_backbone_fiber_type= model.backbone_fiber_type,
                        p_sprout_fibertype = model.sprout_fiber_type,
                        p_backbone_line_geom = model.geometry,
                        p_iscreateduct = model.is_create_duct,
                        p_iscreatetrench = model.is_create_trench
                    }, true).ToList();
                if(result == null || result.Count == 0)
                {
                    return null;
                }
                return result;
            }
            catch { throw; }
        }
        public List<SiteBufferGeometryRaw> BackBonePlanDraftLineGeometry(int planId, int userId)
        {
            try
            {
                var rawList = repo.ExecuteProcedure<SiteBufferGeometryRaw>(
                    "fn_backbone_get_draft_line_geometry", 
                    new
                    {                        
                        p_plan_id = planId,
                        p_user_id = userId
                    }                    
                ).ToList();

                if (rawList == null || !rawList.Any())
                    return null;

                return rawList;
            }
            catch
            {
                throw;
            }
        }

        public List<DropDownMaster> GetBackboneFiberTypeDropDownList()
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_backbone_get_fibertype_dropdownlist", new {
                },true);
            }
            catch { throw; }
        }        
 
    }
    public class DABackBonePlan : Repository<BackBonePlanning>
    {
        public void getUpdateBackbonePlan(bool createplan, int planId)
        {
            try
            {
                var objPlan = repo.Get(x => x.plan_id == planId);
                objPlan.is_create_plan = createplan;
                objPlan.sprout_fiber_type = "48";
                objPlan.total_cable_length = objPlan.total_cable_length == 0 || objPlan.total_cable_length == null  ? 1 : objPlan.total_cable_length;
                objPlan.loop_span = objPlan.loop_span == 0 || objPlan.loop_span == null ? 1 : objPlan.loop_span; // or some default
                objPlan.loop_length = objPlan.loop_length == 0 ? 1 : objPlan.loop_length;
                if (objPlan != null)
                {
                    repo.Update(objPlan);
                };
            }
            catch { throw; }
        }
    }
    public class DABackBoneNetworkPlan : Repository<BackbonePlanNetworkDetails>
    {
        public List<BackbonePlanNetworkDetails> GetBackBoneLoopList(int planId, int userId,bool p_isloop_required, string line_geom, double loopSpan, double loopLength)
        {
            try
            {                            
                return repo.GetAll(x => x.plan_id == planId && x.is_loop_required == true && x.created_by == userId).OrderBy(x => x.system_id).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateLoopLengthByBackBonePlanId(int planId, double looplength)
        {
            try
            {
                var allPointByPlan_idList = repo.GetAll(x => x.plan_id == planId).OrderBy(x => x.system_id).ToList();
                allPointByPlan_idList.ForEach(x => x.loop_length = looplength);
                repo.Update(allPointByPlan_idList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateBackBoneLoopLength(List<BackbonePlanNetworkDetails> model)
        {
            try
            {
                repo.Update(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
    public class DABackBonePlanSite : Repository<SitePlanList>
    {
        public List<BackBoneSproutFiberDetails> SaveNearestSite(List<SitePlanList> model,int userId)
        {
            try
            {
                if (model.Count > 0)
                {
                    int planId = Convert.ToInt32(model[0].plan_id);
                    List<SitePlanList> List = repo.GetAll(m => m.plan_id == planId).ToList();
                    if (List.Count > 0)
                    {
                        repo.DeleteRange(List);
                    }
                    repo.Insert(model);
                    var lst = repo.ExecuteProcedure<BackBoneSproutFiberDetails>("fn_backbone_draft_network",
                           new
                           {
                               p_planid = planId,
                               p_user_id = userId
                           }, true).ToList();
                    return lst;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<SitePlanList> getSiteList(int planId, int userId)
        {
            try
            {
                return repo.GetAll(m => m.plan_id == planId && m.user_id == userId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SitePlanList updateSiteRoute(string geom, int planId, int p_systemId,int userId)
        {
            try
            {
                SitePlanList sitePlanList = new SitePlanList();
                var objSystmId = repo.Get(x => x.id == p_systemId);
                 objSystmId.is_update = true;
                if (objSystmId != null)
                {
                    repo.Update(objSystmId);
                }
                var planDetail = repo.ExecuteProcedure<SitePlanList>("fn_backbone_update_sprout_network",
                new { p_lineGeom = geom, p_systemid = p_systemId, p_plan_id = planId, p_user_id = userId}, true);
                if (planDetail.Count > 0)
                {
                    sitePlanList.total_sp_route_length = planDetail[0].total_sp_route_length;
                    sitePlanList.sprout_route_length = planDetail[0].sprout_route_length;
                }
                return sitePlanList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SitePlanList> getNearestSiteHistoryList(int planId)
        {
            try
            {
                var planDetail = repo.ExecuteProcedure<SitePlanList>("fn_backbone_get_history_sites",
                new {planid = planId}, true);
                if (planDetail.Count > 0) { 
                return planDetail;
                }
                else
                {
                    return new List<SitePlanList>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
        public class DAtemp_auto_network_plan : Repository<temp_auto_network_plan>
    {
        public double GetTotalLoopLength(int plan_id)
        {
            try
            {
                return repo.GetAll(x => x.plan_id == plan_id).Sum(x => x.loop_length);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetcountOfPointEntity(int plan_id)
        {
            try
            {
                return repo.GetAll(x => x.plan_id == plan_id).Count();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<temp_auto_network_plan> GetTempNetwork(int temp_plan_id,string SiteId)
        {
            try
            {
                if (SiteId.Contains("--DESC"))
                {
                    return repo.GetAll(x => x.plan_id == temp_plan_id).OrderByDescending(x => x.system_id).ToList();
                } 
                return repo.GetAll(x => x.plan_id == temp_plan_id).OrderBy(x => x.system_id).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateLoopLengthByPlanId(int temp_plan_id, double looplength)
        {
            try
            {
                var allPointByPlan_idList = repo.GetAll(x => x.plan_id == temp_plan_id).OrderBy(x => x.system_id).ToList();
                allPointByPlan_idList.ForEach(x => x.loop_length = looplength);
                repo.Update(allPointByPlan_idList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateTempLoop(List<temp_auto_network_plan> model)
        {
            try
            {
                repo.Update(model);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
    public class DANetworkPlanning : Repository<NetworkPlanning>
    {
        public List<NetworkPlanning> GetNetworkPlanning(int userId)
        {
            try
            {
                var newInsteredItem = repo.GetAll().Where(x => x.created_by == userId).ToList();
                return newInsteredItem;
            }
            catch { throw; }

        }
        public NetworkPlanning SaveNetworkPlanning(NetworkPlanning objPlanning)
        {
            try
            {
                var newInsteredItem = repo.Insert(objPlanning);
                return newInsteredItem;


            }
            catch { throw; }
        }

        public NetworkPlanning GetNetworkPlanningById(int planId)
        {
            try
            {
                var newInsteredItem = repo.GetAll(x => x.planid == planId).FirstOrDefault();
                return newInsteredItem;
            }
            catch { throw; }
        }


        public BackBonePlanning GetBackbonePlanningById(int planId)
        {
            try
            {
                var res = repo.ExecuteProcedure<BackBonePlanning>("fn_backbone_get_plan_network", new
                {
                    p_plan_id = planId
                }, true).FirstOrDefault();
                if (res != null)
                {
                    return res;
                }
                else
                {
                    return new BackBonePlanning();
                }
            }
            catch { throw; }
        }

        public NetworkPlanning GetNetworkForMap(int planId)
        {

            try
            {
                var res = repo.ExecuteProcedure<NetworkPlanning>("fn_get_network_planning_network", new
                {
                    p_plan_id = planId
                }, true).FirstOrDefault();
                return res;
            }
            catch { throw; }
        }

        public BackBonePlanning GetBackboneForMap(int planId)
        {

            try
            {
                var res = repo.ExecuteProcedure<BackBonePlanning>("fn_backbone_plan_network", new
                {
                    p_plan_id = planId
                }, true).FirstOrDefault();
                return res;
            }
            catch { throw; }
        }

        public List<NetworkPlanning> GetTempNetworkForMap(int planId)
        {

            try
            {
                var res = repo.ExecuteProcedure<NetworkPlanning>("fn_get_temp_network_planning_network", new
                {
                    p_plan_id = planId
                }, true);
                return res;
            }
            catch { throw; }
        }
    }
    public class DASiteCircle : Repository<SiteCircleList>
    {
        public List<SiteCircleList> GetCircleList()
        {
            try
            {

                var res = repo.ExecuteProcedure<SiteCircleList>("fn_get_siteCircle", new { }, true);
                return res;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    #region ANTRA
    public class DAMiscellaneous : Repository<AddNewEntityUtilization>
    {
        public AddNewEntityUtilization SaveEntityUtilizationDetails(AddNewEntityUtilization objAddEntityUtilSetting)
        {
            try
            {
                var objExistingUtil = repo.Get(x => x.system_id == objAddEntityUtilSetting.system_id);

                if (objExistingUtil != null)
                {
                    objExistingUtil.utilization_range_low_from = objAddEntityUtilSetting.utilization_range_low_from;
                    objExistingUtil.utilization_range_low_to = objAddEntityUtilSetting.utilization_range_low_to;
                    objExistingUtil.utilization_range_moderate_from = objAddEntityUtilSetting.utilization_range_moderate_from;
                    objExistingUtil.utilization_range_moderate_to = objAddEntityUtilSetting.utilization_range_moderate_to;
                    objExistingUtil.utilization_range_high_from = objAddEntityUtilSetting.utilization_range_high_from;
                    objExistingUtil.utilization_range_high_to = objAddEntityUtilSetting.utilization_range_high_to;
                    objExistingUtil.utilization_range_over_from = objAddEntityUtilSetting.utilization_range_over_from;
                    objExistingUtil.utilization_range_over_to = objAddEntityUtilSetting.utilization_range_over_to;
                    objExistingUtil.modified_by = objAddEntityUtilSetting.user_id;
                    objExistingUtil.modified_on = DateTimeHelper.Now;
                    objExistingUtil.entity_utilization_percentage = objAddEntityUtilSetting.entity_utilization_percentage;
                    return repo.Update(objExistingUtil);
                }
                else
                {
                    objAddEntityUtilSetting.created_by = objAddEntityUtilSetting.user_id;
                    objAddEntityUtilSetting.created_on = DateTimeHelper.Now;
                    return repo.Insert(objAddEntityUtilSetting);

                }

            }
            catch { throw; }
        }
        public int DeleteUtilizationSettingDetailById(int id)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }
        public AddNewEntityUtilization ChkUtilizationExist(AddNewEntityUtilization objUtilization)
        {
            try
            {
                return repo.Get(u => u.region_id == objUtilization.region_id && u.province_id == objUtilization.province_id && u.network_status == objUtilization.network_status && u.layer_id == objUtilization.layer_id);
            }
            catch
            {
                throw;
            }
        }
        public AddNewEntityUtilization GetUtilizationDetailByID(int id)
        {
            var obj = repo.Get(m => m.system_id == id);
            return obj != null ? obj : new AddNewEntityUtilization();
        }

        public List<ViwUtilizationSetting> GetUtilizationSettingsList(ViewEntityUtilizationSettingsFilter objEntityUtiliSettingsFilter)
        {
            try
            {
                return repo.ExecuteProcedure<ViwUtilizationSetting>("fn_get_entity_utilization_settings", new
                {
                    region_id = objEntityUtiliSettingsFilter.region_id,
                    province_id = objEntityUtiliSettingsFilter.province_id,
                    layer_id = objEntityUtiliSettingsFilter.layer_id,
                    P_PAGENO = objEntityUtiliSettingsFilter.currentPage,
                    P_PAGERECORD = objEntityUtiliSettingsFilter.pageSize,
                    P_SORTCOLNAME = objEntityUtiliSettingsFilter.sort,
                    P_SORTTYPE = objEntityUtiliSettingsFilter.orderBy
                }, true);
            }
            catch { throw; }
        }

    }
    #endregion
    public class DAMiscNotiComm : Repository<EntityNotificationComments>
    {
        public EntityNotificationComments SaveEntityNotificationComment(EntityNotificationComments objNotificationComment)
        {
            try
            {
                return repo.Insert(objNotificationComment);
            }
            catch { throw; }
        }
        public List<ViewEntityNotificationComments> getEntityNotificationComment(int notificationId)
        {
            try
            {
                var res = repo.ExecuteProcedure<ViewEntityNotificationComments>("fn_get_entity_notification_comment", new { notificationId }, true);
                return res;
            }
            catch { throw; }
        }
    }
    #region UTILNOTIFICATION ANTRA
    public class DAMiscNoti : Repository<EntityNotifications>
    {
        public List<ViewEntityNotifications> GetEntityNotificationList(EntityNotificationsFilter objEntityUtiliSettingsFilter)
        {
            try
            {
                var res = repo.ExecuteProcedure<ViewEntityNotifications>("fn_get_entity_notifications", new
                {
                    p_searchby = objEntityUtiliSettingsFilter.searchBy,
                    p_searchtext = objEntityUtiliSettingsFilter.searchText,
                    p_pageno = objEntityUtiliSettingsFilter.currentPage,
                    p_pagerecord = objEntityUtiliSettingsFilter.pageSize,
                    p_sortcolname = objEntityUtiliSettingsFilter.sort,
                    p_sorttype = objEntityUtiliSettingsFilter.orderBy,
                    p_totalrecords = objEntityUtiliSettingsFilter.totalRecord,
                    p_userid = objEntityUtiliSettingsFilter.userId,
                    p_roleid = objEntityUtiliSettingsFilter.roleId
                }, true);
                return res;
            }
            catch { throw; }
        }

        public EntityNotifications UpdateNotificationCloseStatus(int notificationId, int user_id)
        {
            try
            {
                var objExisting = repo.Get(x => x.notification_id == notificationId);
                if (objExisting != null)
                {
                    objExisting.is_closed = true;
                    objExisting.modified_on = DateTimeHelper.Now;
                    objExisting.modified_by = user_id;
                    repo.Update(objExisting);

                }
                return objExisting;
            }
            catch { throw; }


        }
        public long GetUnreadNotificationCount(int userid, int roleid)
        {
            try
            {
                var lstResult = repo.ExecuteProcedure<long>("fn_GetEntityNotificationCount",
                    new
                    {
                        p_userid = userid,
                        p_roleid = roleid
                    }
                    , false);
                return lstResult.Count > 0 ? lstResult[0] : 0;
            }
            catch { throw; }
        }
        public List<EntityNotifications> GetEntityNotificationById(int notificationId)
        {
            try
            {
                return repo.GetAll(p => p.notification_id == notificationId).ToList();
            }
            catch { throw; }
        }
    }
    #endregion UTILNOTIFICATION ANTRA

    public class DAGroupLibrary : Repository<ViewGroupLibrary>
    {
        public int SaveGroupLibrary(ViewGroupLibrary objGroupLibrary)
        {
            try
            {
                //string lstCloneDependent = JsonConvert.SerializeObject(objCloneDependen.lstCloneDependent);
                return repo.ExecuteProcedure<int>("fn_save_group_library_entity", new
                {
                    p_system_id = objGroupLibrary.system_id,
                    p_entity_type = objGroupLibrary.entity_type,
                    p_parent_id = objGroupLibrary.parent_id,
                    p_is_accessories_required = objGroupLibrary.is_accessories_required,
                    p_is_associated_entity = objGroupLibrary.is_associated_entity,
                    p_is_child_entiy = objGroupLibrary.is_child_entity,
                    p_name = objGroupLibrary.name,
                    p_description = objGroupLibrary.description,
                    p_created_by = objGroupLibrary.created_by
                }).FirstOrDefault();
            }
            catch (Exception ex) { throw ex; }
        }
        public List<getGroupLibrary> GetGroupLibrary(int user_id)
        {
            try
            {
                // return repo.GetAll(p => p.parent_id == 0 && p.created_by== user_id).OrderByDescending(m => m.created_on).ToList();
                return repo.ExecuteProcedure<getGroupLibrary>("fn_get_group_library", new { p_user_id = user_id }).ToList();
            }
            catch (Exception ex) { throw ex; }
        }

        public List<GroupLibrary> SaveGroupLibraryEntity(List<Properties> lstProperties)
        {
            try
            {
                string p_lstProperties = JsonConvert.SerializeObject(lstProperties);
                var res = repo.ExecuteProcedure<GroupLibrary>("fn_grouplibrary_save_entity", new
                {
                    p_properties = p_lstProperties,
                }).ToList();
                return res;
            }
            catch (Exception ex) { throw ex; }
        }

        public string DeleteGroupLibraryById(int id)
        {
            try
            {
                var result = "False";

                var lstGroupLibrary = repo.GetAll(x => x.id == id || x.parent_id == id);

                if (lstGroupLibrary != null)
                {
                    repo.DeleteRange(lstGroupLibrary.ToList());
                    result = "DELETE";
                }



                return result;

            }
            catch { throw; }

        }

        public List<GroupLibraryDetails> GetGroupLibraryByid(int id, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<GroupLibraryDetails>("fn_get_group_library_details", new { P_group_library_id = id, p_userId = userId }, true);
            }
            catch { throw; }
        }
        public ViewGroupLibrary GetLineGroupLibraryByid(int id)
        {
            try
            {
                return repo.GetAll(x => x.id == id).FirstOrDefault();
            }
            catch { throw; }
        }

        //public DbMessage SaveCablegroupLibrary(List<GLLineDetails> lstGroupLineTP, List<TerminationPoint> lstGLTerminationPoint, int user_id)
        //{
        //    try
        //    {
        //        string lstGroupLine = JsonConvert.SerializeObject(lstGroupLineTP);

        //        string lstTerminationPoint = JsonConvert.SerializeObject(lstGLTerminationPoint);
        //        return repo.ExecuteProcedure<DbMessage>("fn_save_line_group_library", new
        //        {
        //            p_lstgroupline = lstGroupLine,
        //            p_lstterminationpoint = lstTerminationPoint,
        //            p_created_by = user_id
        //        }).FirstOrDefault();
        //    }
        //    catch (Exception ex) { throw ex; }
        //    //try
        //    //{
        //    //    NetworkCodeIn objIn = new NetworkCodeIn();
        //    //    objIn.gType = GeometryType.Line.ToString();
        //    //    objIn.eType = entity_type;
        //    //    objIn.eGeom = geom;
        //    //    DbMessage objmsg = new DbMessage();
        //    //    CableMaster objCbl = new CableMaster();
        //    //    List<InRegionProvince> objRegionProvince = DABuilding.Instance.GetRegionProvince(geom, GeometryType.Line.ToString());
        //    //    var Parent_Details = new DAMisc().getParentInfo(objIn);
        //    //    var isNew = objCbl.system_id > 0 ? false : true;
        //    //foreach (var item in lstTerminationPoint)
        //    //{
        //    //    if (item.termination_type.ToUpper() == "start".ToUpper())
        //    //    {
        //    //        objCbl.a_entity_type = item.network_name;
        //    //        objCbl.a_location = item.network_id;
        //    //        objCbl.a_long_lat = item.lnglat;
        //    //        objCbl.a_node_type = item.node_type;
        //    //        objCbl.a_system_id = item.system_id;
        //    //    }
        //    //    if (item.termination_type.ToUpper() == "end".ToUpper())
        //    //    {
        //    //        objCbl.b_entity_type = item.network_name;
        //    //        objCbl.b_location = item.network_id;
        //    //        objCbl.b_long_lat = item.lnglat;
        //    //        objCbl.b_node_type = item.node_type;
        //    //        objCbl.b_system_id = item.system_id;
        //    //    }
        //    //}
        //    //var networkCodeDetail = new DAMisc().GetLineNetworkCode("", "", entity_type, geom, "OSP");
        //    //objCbl.cable_name = networkCodeDetail.network_code;
        //    //objCbl.network_id = networkCodeDetail.network_code;
        //    //objCbl.cable_measured_length = Convert.ToInt64(entity_data["cable_measured_length"]);
        //    //objCbl.cable_calculated_length = Convert.ToInt64(entity_data["cable_calculated_length"]);
        //    //objCbl.total_core = entity_data["total_core"];
        //    //objCbl.no_of_tube = entity_data["no_of_tube"];
        //    //objCbl.no_of_core_per_tube = entity_data["no_of_core_per_tube"];
        //    //objCbl.no_of_core_per_tube = entity_data["no_of_core_per_tube"];
        //    //objCbl.specification = entity_data["specification"];
        //    //objCbl.category = entity_data["category"];
        //    //objCbl.subcategory1 = entity_data["subcategory1"];
        //    //objCbl.subcategory2 = entity_data["subcategory2"];
        //    //objCbl.subcategory3 = entity_data["subcategory3"];
        //    //objCbl.item_code = entity_data["item_code"];
        //    //objCbl.vendor_id = entity_data["vendor_id"];
        //    //objCbl.network_status = entity_data["network_status"];
        //    //objCbl.status = entity_data["status"];
        //    //objCbl.pin_code = entity_data["pin_code"];
        //    //objCbl.region_id = objRegionProvince[0].region_id;
        //    //objCbl.province_id = objRegionProvince[0].province_id;
        //    //objCbl.created_by = user_id;
        //    //objCbl.total_loop_length = entity_data["total_loop_length"];
        //    //objCbl.total_loop_count = entity_data["total_loop_count"];
        //    //objCbl.cable_type = entity_data["cable_type"];
        //    //objCbl.parent_network_id = Parent_Details.p_network_id;
        //    //objCbl.parent_entity_type = Parent_Details.p_entity_type;
        //    //objCbl.parent_system_id = Parent_Details.p_system_id;
        //    //objCbl.utilization = entity_data["utilization"];
        //    //objCbl.ownership_type = entity_data["ownership_type"];
        //    //objCbl.source_ref_id = entity_data["source_ref_id"];
        //    //objCbl.source_ref_type = entity_data["source_ref_type"];
        //    //objCbl.is_acquire_from = entity_data["is_acquire_from"];
        //    //objCbl.audit_item_master_id = entity_data["audit_item_master_id"];
        //    //objCbl.geom = geom;
        //    //    objCbl.sequence_id = networkCodeDetail.sequence_id;
        //    //objCbl = DACable.Instance.SaveCable(objCbl, user_id);

        //    //if (isNew)
        //    //{
        //    //    objCbl.objPM.status = ResponseStatus.OK.ToString();
        //    //    objCbl.objPM.isNewEntity = isNew;
        //    //    objCbl.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, EntityType.Cable.ToString());
        //    //}
        //    //return objCbl.objPM;
        //    //}
        //    //catch (Exception ex) { throw ex; }
        //}

        public PageMessage SaveDuctgroupLibrary(string geom, int Id, int user_id, List<TerminationPoint> lstTerminationPoint, dynamic entity_data, string entity_type)
        {
            try
            {
                NetworkCodeIn objIn = new NetworkCodeIn();
                objIn.gType = GeometryType.Line.ToString();
                objIn.eType = entity_type;
                objIn.eGeom = geom;
                DbMessage objmsg = new DbMessage();
                DuctMaster objCbl = new DuctMaster();
                List<InRegionProvince> objRegionProvince = DABuilding.Instance.GetRegionProvince(geom, GeometryType.Line.ToString());
                var Parent_Details = new DAMisc().getParentInfo(objIn);
                var isNew = objCbl.system_id > 0 ? false : true;
                foreach (var item in lstTerminationPoint)
                {
                    if (item.termination_type.ToUpper() == "start".ToUpper())
                    {
                        objCbl.a_entity_type = item.network_name;
                        objCbl.a_location = item.network_id;
                        objCbl.a_long_lat = item.lnglat;
                        objCbl.a_node_type = item.node_type;
                        objCbl.a_system_id = item.system_id;
                    }
                    if (item.termination_type.ToUpper() == "end".ToUpper())
                    {
                        objCbl.b_entity_type = item.network_name;
                        objCbl.b_location = item.network_id;
                        objCbl.b_long_lat = item.lnglat;
                        objCbl.b_node_type = item.node_type;
                        objCbl.b_system_id = item.system_id;
                    }
                }
                var networkCodeDetail = new DAMisc().GetLineNetworkCode(objCbl.a_location, objCbl.b_location, entity_type, geom, "OSP");
                objCbl.duct_name = networkCodeDetail.network_code;
                objCbl.network_id = networkCodeDetail.network_code;
                objCbl.manual_length = Convert.ToInt64(entity_data["manual_length"]);
                objCbl.calculated_length = Convert.ToInt64(entity_data["calculated_length"]);
                objCbl.specification = entity_data["specification"];
                objCbl.category = entity_data["category"];
                objCbl.subcategory1 = entity_data["subcategory1"];
                objCbl.subcategory2 = entity_data["subcategory2"];
                objCbl.subcategory3 = entity_data["subcategory3"];
                objCbl.item_code = entity_data["item_code"];
                objCbl.vendor_id = entity_data["vendor_id"];
                objCbl.network_status = entity_data["network_status"];
                objCbl.status = entity_data["status"];
                objCbl.pin_code = entity_data["pin_code"];
                objCbl.region_id = objRegionProvince[0].region_id;
                objCbl.province_id = objRegionProvince[0].province_id;
                objCbl.created_by = user_id;
                objCbl.duct_type = entity_data["duct_type"];
                objCbl.parent_network_id = Parent_Details.p_network_id;
                objCbl.parent_entity_type = Parent_Details.p_entity_type;
                objCbl.parent_system_id = Parent_Details.p_system_id;
                objCbl.utilization = entity_data["utilization"];
                objCbl.ownership_type = entity_data["ownership_type"];
                objCbl.is_acquire_from = entity_data["is_acquire_from"];
                objCbl.audit_item_master_id = entity_data["audit_item_master_id"];
                objCbl.geom = geom;
                objCbl.sequence_id = networkCodeDetail.sequence_id;
                objCbl = DADuct.Instance.SaveDuct(objCbl, user_id); ;
                if (isNew)
                {
                    objCbl.objPM.status = ResponseStatus.OK.ToString();
                    objCbl.objPM.isNewEntity = isNew;
                    objCbl.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, EntityType.Duct.ToString());
                }
                return objCbl.objPM;
            }
            catch (Exception ex) { throw ex; }
        }

        public PageMessage SaveTrenchgroupLibrary(string geom, int Id, int user_id, List<TerminationPoint> lstTerminationPoint, dynamic entity_data, string entity_type)
        {
            try
            {
                NetworkCodeIn objIn = new NetworkCodeIn();
                objIn.gType = GeometryType.Line.ToString();
                objIn.eType = entity_type;
                objIn.eGeom = geom;
                DbMessage objmsg = new DbMessage();
                TrenchMaster objCbl = new TrenchMaster();
                List<InRegionProvince> objRegionProvince = DABuilding.Instance.GetRegionProvince(geom, GeometryType.Line.ToString());
                var Parent_Details = new DAMisc().getParentInfo(objIn);
                var isNew = objCbl.system_id > 0 ? false : true;
                foreach (var item in lstTerminationPoint)
                {
                    if (item.termination_type.ToUpper() == "start".ToUpper())
                    {
                        objCbl.a_entity_type = item.network_name;
                        objCbl.a_location = item.network_id;
                        objCbl.a_long_lat = item.lnglat;
                        objCbl.a_node_type = item.node_type;
                        objCbl.a_system_id = item.system_id;
                    }
                    if (item.termination_type.ToUpper() == "end".ToUpper())
                    {
                        objCbl.b_entity_type = item.network_name;
                        objCbl.b_location = item.network_id;
                        objCbl.b_long_lat = item.lnglat;
                        objCbl.b_node_type = item.node_type;
                        objCbl.b_system_id = item.system_id;
                    }
                }
                var networkCodeDetail = new DAMisc().GetLineNetworkCode(objCbl.a_location, objCbl.b_location, entity_type, geom, "OSP");
                objCbl.trench_name = networkCodeDetail.network_code;
                objCbl.network_id = networkCodeDetail.network_code;
                objCbl.trench_length = Convert.ToInt64(entity_data["trench_length"]);
                objCbl.specification = entity_data["specification"];
                objCbl.category = entity_data["category"];
                objCbl.subcategory1 = entity_data["subcategory1"];
                objCbl.subcategory2 = entity_data["subcategory2"];
                objCbl.subcategory3 = entity_data["subcategory3"];
                objCbl.item_code = entity_data["item_code"];
                objCbl.vendor_id = entity_data["vendor_id"];
                objCbl.network_status = entity_data["network_status"];
                objCbl.status = entity_data["status"];
                objCbl.pin_code = entity_data["pin_code"];
                objCbl.region_id = objRegionProvince[0].region_id;
                objCbl.province_id = objRegionProvince[0].province_id;
                objCbl.created_by = user_id;
                objCbl.trench_type = entity_data["trench_type"];
                objCbl.parent_network_id = Parent_Details.p_network_id;
                objCbl.parent_entity_type = Parent_Details.p_entity_type;
                objCbl.parent_system_id = Parent_Details.p_system_id;
                objCbl.utilization = entity_data["utilization"];
                objCbl.ownership_type = entity_data["ownership_type"];
                objCbl.is_acquire_from = entity_data["is_acquire_from"];
                objCbl.audit_item_master_id = entity_data["audit_item_master_id"];
                objCbl.geom = geom;
                objCbl.sequence_id = networkCodeDetail.sequence_id;
                objCbl = DATrench.Instance.SaveTrench(objCbl, user_id); ;
                if (isNew)
                {
                    objCbl.objPM.status = ResponseStatus.OK.ToString();
                    objCbl.objPM.isNewEntity = isNew;
                    objCbl.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, EntityType.Trench.ToString());
                }
                return objCbl.objPM;
            }
            catch (Exception ex) { throw ex; }
        }

    }
    public class DANotificationStatus : Repository<EntityNotificationStatus>
    {
        public bool getNotificationStatus(int systemId, string entityType)
        {
            try
            {
                bool isEnabled = true;
                var notification = repo.GetAll(m => m.systemId == systemId && m.entityType.ToUpper() == entityType.ToUpper()).OrderByDescending(m => m.id).FirstOrDefault();
                if (notification != null) { isEnabled = notification.status; }
                return isEnabled;
            }
            catch { throw; }
        }
        public EntityNotificationStatus SaveNotificationStatus(EntityNotificationStatus objNotifyStatus, int userId)
        {
            try
            {
                objNotifyStatus.createdBy = userId;
                objNotifyStatus.createdOn = DateTime.Now;
                return repo.Insert(objNotifyStatus);
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> getNotificationStatusHistory(int systemid, string eType, int currentPage, int pageSize, string sort, string orderBy)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_notification_status_history",
                    new
                    {
                        P_PAGENO = currentPage,
                        P_PAGERECORD = pageSize,
                        P_SORTCOLNAME = sort,
                        P_SORTTYPE = orderBy,
                        p_systemid = systemid,
                        p_entity_name = eType.ToString()
                    }, true);
                return lst;
            }
            catch { throw; }

        }

    }

    public class DATPMaster : Repository<TerminationPointMaster>
    {
        public List<TerminationPointMaster> GetTPDetails(ViewTPMaster objTPMaster)
        {
            try
            {
                var lst = repo.ExecuteProcedure<TerminationPointMaster>("fn_get_tp_details", new
                {
                    p_pageno = objTPMaster.currentPage,
                    p_pagerecord = objTPMaster.pageSize,
                    p_sortcolname = objTPMaster.sort,
                    p_sorttype = objTPMaster.orderBy,
                    p_searchBy = objTPMaster.searchBy,
                    p_searchText = objTPMaster.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }
        public string SaveTerminationPoint(TerminationPointMaster objTerminationPoint)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_validate_termination_point", new
                {
                    p_tp_id = objTerminationPoint.id,
                    p_layer_id = objTerminationPoint.layer_id,
                    p_tp_layer_id = objTerminationPoint.tp_layer_id,
                    p_is_osp_tp = objTerminationPoint.is_osp_tp,
                    p_is_isp_tp = objTerminationPoint.is_isp_tp,
                    p_is_active = objTerminationPoint.is_enabled,
                    p_flag = "",
                }).FirstOrDefault();
                var result = "Failed";
                var validateTP = repo.GetAll(m => m.layer_id == objTerminationPoint.layer_id && m.tp_layer_id == objTerminationPoint.tp_layer_id).FirstOrDefault();

                var objExisiting = repo.GetById(m => m.id == objTerminationPoint.id);
                if (objExisiting != null)
                {
                    if (response.status)
                    {
                        objExisiting.is_enabled = objTerminationPoint.is_enabled;
                        objExisiting.is_isp_tp = objTerminationPoint.is_isp_tp;
                        objExisiting.is_osp_tp = objTerminationPoint.is_osp_tp;
                        objExisiting.modified_by = objTerminationPoint.created_by;
                        objExisiting.modified_on = DateTimeHelper.Now;
                        repo.Update(objExisiting);
                        result = "Update";
                    }
                    else
                    {
                        result = response.message;
                    }
                }
                else
                {
                    if (validateTP == null)
                    {
                        objTerminationPoint.created_on = DateTimeHelper.Now;
                        objTerminationPoint.created_by = objTerminationPoint.created_by;
                        objTerminationPoint.modified_by = objTerminationPoint.created_by;
                        objTerminationPoint.modified_on = DateTimeHelper.Now;
                        repo.Insert(objTerminationPoint);
                        result = "Save";
                    }
                }

                return result;
            }
            catch { throw; }
        }
        public TerminationPointMaster GetTerminationPointById(int id)
        {
            try
            {
                return repo.Get(u => u.id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string DeleteTerminationPointById(int id)
        {
            try
            {
                var result = "False";
                var response = repo.ExecuteProcedure<DbMessage>("fn_validate_termination_point", new
                {
                    p_tp_id = id,
                    p_layer_id = 0,
                    p_tp_layer_id = 0,
                    p_is_osp_tp = false,
                    p_is_isp_tp = false,
                    p_is_active = false,
                    p_flag = "DELETE",
                }).FirstOrDefault();
                var objId = repo.Get(x => x.id == id);
                if (objId != null)
                {
                    if (response.status)
                    {
                        repo.Delete(objId.id);
                        result = "DELETE";
                    }
                    else
                    {
                        result = response.message;
                    }
                }

                return result;

            }
            catch { throw; }

        }


        public List<TPDropdown> TPDropdownList(int layer_id = 0, string flage = "")
        {
            try
            {
                return repo.ExecuteProcedure<TPDropdown>("fn_get_tp_dropdownlist", new { p_layer_id = layer_id, p_flage = flage });
            }
            catch (Exception)

            { throw; }
        }
    }

    public class DATemplateColumn : Repository<TemplateColumn>
    {
        public List<TemplateColumn> GetTemplateColumn(ViewTemplateColumn objTemplateColumn)
        {
            try
            {
                var lst = repo.ExecuteProcedure<TemplateColumn>("fn_get_template_column_list", new
                {
                    p_pageno = objTemplateColumn.currentPage,
                    p_pagerecord = objTemplateColumn.pageSize,
                    p_sortcolname = objTemplateColumn.sort,
                    p_sorttype = objTemplateColumn.orderBy,
                    p_searchBy = objTemplateColumn.searchBy,
                    p_searchText = objTemplateColumn.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        public List<TemplateColumnDropdown> GetTemplateColumnDropdown(int layer_id = 0, string flag = "")
        {
            try
            {
                return repo.ExecuteProcedure<TemplateColumnDropdown>("fn_get_du_column_dropdownlist", new { p_layer_id = layer_id, p_flag = flag });
            }
            catch { throw; }
        }


        public string SaveTemplateColumn(TemplateColumn objTemplateColumn)
        {
            try
            {

                var result = "Failed";

                var objExisiting = repo.GetById(m => m.id == objTemplateColumn.id);
                if (objExisiting != null)
                {
                    objExisiting.template_column_name = objTemplateColumn.template_column_name;
                    objExisiting.example_value = objTemplateColumn.example_value;
                    objExisiting.description = objTemplateColumn.description;
                    objExisiting.max_length = objTemplateColumn.max_length;
                    objExisiting.is_dropdown = objTemplateColumn.is_dropdown;
                    objExisiting.is_nullable = objTemplateColumn.is_nullable;
                    objExisiting.is_excel_attribute = objTemplateColumn.is_excel_attribute;
                    objExisiting.is_kml_attribute = objTemplateColumn.is_kml_attribute;
                    objExisiting.modified_by = objTemplateColumn.created_by;
                    objExisiting.modified_on = DateTimeHelper.Now;
                    repo.Update(objExisiting);
                    result = "Update";
                }


                else
                {
                    objTemplateColumn.created_on = DateTimeHelper.Now;
                    objTemplateColumn.created_by = objTemplateColumn.created_by;
                    objTemplateColumn.modified_by = objTemplateColumn.created_by;
                    objTemplateColumn.modified_on = DateTimeHelper.Now;
                    repo.Insert(objTemplateColumn);
                    result = "Save";
                }


                return result;
            }
            catch { throw; }
        }
        public TemplateColumn GetTemplateColumnById(int id)
        {
            try
            {
                return repo.Get(u => u.id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string DeleteTemplateColumnById(int id)
        {
            try
            {
                var result = "False";

                var objId = repo.Get(x => x.id == id);
                if (objId != null)
                {
                    repo.Delete(objId.id);
                    result = "DELETE";
                }

                return result;

            }
            catch { throw; }

        }
    }

    public class DAPortStatus : Repository<portStatusMaster>
    {
        public List<portStatusMaster> GetportStatus(LogicalViewVM objportStatus)
        {
            try
            {
                var lst = repo.ExecuteProcedure<portStatusMaster>("fn_get_port_status_master", new
                {
                    p_pageno = objportStatus.currentPage,
                    p_pagerecord = objportStatus.pageSize,
                    p_sortcolname = objportStatus.sort,
                    p_sorttype = objportStatus.orderBy,
                    p_searchBy = objportStatus.searchBy,
                    p_searchText = objportStatus.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        public List<TemplateColumnDropdown> GetTemplateColumnDropdown(int layer_id = 0, string flag = "")
        {
            try
            {
                return repo.ExecuteProcedure<TemplateColumnDropdown>("fn_get_du_column_dropdownlist", new { p_layer_id = layer_id, p_flag = flag });
            }
            catch { throw; }
        }


        public string SavePortStatus(portStatusMaster objportStatus)
        {
            try
            {

                var result = "Failed";

                var objExisiting = repo.GetById(m => m.system_id == objportStatus.system_id);
                if (objExisiting != null)
                {
                    objExisiting.status = objportStatus.status;
                    objExisiting.color_code = objportStatus.color_code;
                    objExisiting.is_manual_status = objportStatus.is_manual_status;
                    objExisiting.is_active = objportStatus.is_active;
                    objExisiting.is_splicing_enabled = objportStatus.is_splicing_enabled;
                    objExisiting.modified_by = objportStatus.created_by;
                    objExisiting.modified_on = DateTimeHelper.Now;
                    repo.Update(objExisiting);
                    result = "Update";
                }


                else
                {
                    objportStatus.created_on = DateTimeHelper.Now;
                    objportStatus.created_by = objportStatus.created_by;
                    repo.Insert(objportStatus);
                    result = "Save";
                }


                return result;
            }
            catch { throw; }
        }
        public portStatusMaster GetPortStatusById(int id)
        {
            try
            {
                return repo.Get(u => u.system_id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DbMessage DeletePortStatusById(int systemId)
        {

            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_delete_portstatus", new { p_system_id = systemId }).FirstOrDefault();

            }
            catch { throw; }

        }

        public bool CheckColorCode(string color_code, int system_id)
        {
            try
            {
                var result = false;
                portStatusMaster objportStatusMaster = new portStatusMaster();
                if (system_id > 0)
                    objportStatusMaster = repo.GetById(m => m.color_code == color_code && m.system_id != system_id);
                else
                    objportStatusMaster = repo.GetById(m => m.color_code == color_code);
                if (objportStatusMaster != null)
                    result = false;
                else
                    result = true;

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }

    public class DAVSAT : Repository<VSATDetails>
    {
        public VSATDetails GetVsatById(int id)
        {
            try
            {
                return repo.GetById(m => m.system_id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string SaveVsat(VSATDetails objVSAT, int parent_system_id)
        {
            try
            {

                var result = "Failed";

                var objExisiting = repo.GetById(m => m.parent_system_id == parent_system_id);
                if (objExisiting != null)
                {
                    objExisiting.site_name = objVSAT.site_name;
                    objExisiting.category = objVSAT.category;
                    objExisiting.antenna_type = objVSAT.antenna_type;
                    objExisiting.service_type = objVSAT.service_type;
                    objExisiting.service_id = objVSAT.service_id;
                    objExisiting.address = objVSAT.address;
                    objExisiting.transmission_type = objVSAT.transmission_type;
                    objExisiting.forward_link = objVSAT.forward_link;
                    objExisiting.return_link = objVSAT.return_link;
                    objExisiting.uplink_carrier = objVSAT.uplink_carrier;
                    objExisiting.downlink_carrier = objVSAT.downlink_carrier;
                    objExisiting.uplink_datarate = objVSAT.uplink_datarate;
                    objExisiting.carrier_bandwidth = objVSAT.carrier_bandwidth;
                    objExisiting.downlink_datarate = objVSAT.downlink_datarate;
                    objExisiting.modulation_technique = objVSAT.modulation_technique;
                    objExisiting.document_type = objVSAT.document_type;
                    objExisiting.is_agreement_signed_one = objVSAT.is_agreement_signed_one;
                    objExisiting.agreemented_by_name_one = objVSAT.agreemented_by_name_one;
                    objExisiting.agreemented_by_position_one = objVSAT.agreemented_by_position_one;
                    objExisiting.company_name_one = objVSAT.company_name_one;
                    objExisiting.is_agreement_signed_two = objVSAT.is_agreement_signed_two;
                    objExisiting.agreemented_by_name_two = objVSAT.agreemented_by_name_two;
                    objExisiting.agreemented_by_position_two = objVSAT.agreemented_by_position_two;
                    objExisiting.company_name_two = objVSAT.company_name_two;
                    objExisiting.modified_by = objVSAT.created_by;
                    objExisiting.modified_on = DateTimeHelper.Now;
                    repo.Update(objExisiting);
                    result = "Update";
                }


                else
                {
                    objVSAT.created_on = DateTimeHelper.Now;
                    repo.Insert(objVSAT);
                    result = "Save";
                }


                return result;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> getVSATReport(int userId, ExportVSATReportFilter objVSATReportFilter)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_vsat_report", new
                {
                    p_systemid = objVSATReportFilter.system_id,
                    p_searchby = objVSATReportFilter.SearchbyText,
                    p_searchtext = objVSATReportFilter.Searchtext,
                    P_PAGENO = objVSATReportFilter.currentPage,
                    P_PAGERECORD = objVSATReportFilter.pageSize,
                    P_SORTCOLNAME = objVSATReportFilter.sort,
                    P_SORTTYPE = objVSATReportFilter.orderBy,
                    p_userid = userId,
                    p_searchfrom = objVSATReportFilter.fromDate,
                    p_searchto = objVSATReportFilter.toDate,
                    p_provinceids = objVSATReportFilter.SelectedProvinceIds,
                    p_regionids = objVSATReportFilter.SelectedRegionIds,
                    geom = objVSATReportFilter.geom,
                    p_radius = objVSATReportFilter.radius
                }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string GetVsatColumnNameByDisplayName(string displayName)
        {
            try
            {
                return repo.ExecuteProcedure<string>("fn__vsat_get_columnname_by_displayname", new { p_display_name = displayName }).FirstOrDefault();

            }
            catch { throw; }
        }

        public DbMessage ValidateVSATPotentialArea(string geom, int userId, string geomType, double buff_Radius)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_get_validate_vsat_bulk_report_area", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_radius = buff_Radius }).FirstOrDefault();

            }
            catch { throw; }
        }

    }

    public class DAAccMapping : Repository<AssociateEntityMaster>
    {
        public List<AssociateEntityMaster> GetAMDetails(ViewAEMaster objAEMaster)
        {
            try
            {
                var lst = repo.ExecuteProcedure<AssociateEntityMaster>("fn_get_accessories_mapping_list", new
                {
                    p_pageno = objAEMaster.currentPage,
                    p_pagerecord = objAEMaster.pageSize,
                    p_sortcolname = objAEMaster.sort,
                    p_sorttype = objAEMaster.orderBy,
                    p_searchBy = objAEMaster.searchBy,
                    p_searchText = objAEMaster.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }
        public string SaveAssociateEntity(AssociateEntityMaster objAssociateEntity)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_validate_associated_entity", new
                {
                    p_system_id = objAssociateEntity.system_id,
                    p_layer_id = objAssociateEntity.layer_id,
                    p_associate_layer_id = objAssociateEntity.associate_layer_id,
                    p_is_active = objAssociateEntity.is_enabled,
                    p_flag = "",
                }).FirstOrDefault();
                var result = "Failed";
                var validateTP = repo.GetAll(m => m.layer_id == objAssociateEntity.layer_id && m.associate_layer_id == objAssociateEntity.associate_layer_id).FirstOrDefault();


                if (validateTP == null)
                {
                    objAssociateEntity.created_on = DateTimeHelper.Now;
                    objAssociateEntity.created_by = objAssociateEntity.created_by;
                    objAssociateEntity.modified_on = DateTimeHelper.Now;
                    objAssociateEntity.modified_by = objAssociateEntity.created_by;
                    repo.Insert(objAssociateEntity);
                    result = "Save";
                }


                return result;
            }
            catch (Exception ex) { throw ex; }
        }
        public string DeleteAssociateEntityById(int id)
        {
            try
            {
                var result = "False";
                var response = repo.ExecuteProcedure<DbMessage>("fn_validate_associated_entity", new
                {
                    p_system_id = id,
                    p_layer_id = 0,
                    p_associate_layer_id = 0,
                    p_is_active = true,
                    p_flag = "DELETE",
                }).FirstOrDefault();
                var objId = repo.Get(x => x.system_id == id);
                if (objId != null)
                {
                    if (response.status)
                    {
                        repo.Delete(objId.system_id);
                        result = "DELETE";
                    }
                    else
                    {
                        result = response.message;
                    }
                }

                return result;

            }
            catch { throw; }

        }

    }
    #region Layer Icom
    public class DALayerIcon : Repository<LayerIconMapping>
    {
        public List<LayerIconMapping> GetLayerIcom(ViewLayerIcon objIspModelImage)
        {
            try
            {
                var lst = repo.ExecuteProcedure<LayerIconMapping>("fn_get_layer_icon", new
                {
                    p_pageno = objIspModelImage.currentPage,
                    p_pagerecord = objIspModelImage.pageSize,
                    p_sortcolname = objIspModelImage.sort,
                    p_sorttype = objIspModelImage.orderBy,
                    p_searchBy = objIspModelImage.searchBy,
                    p_searchText = objIspModelImage.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        public LayerIconMapping saveLayerIcon(LayerIconMapping input)
        {
            try
            {


                var objIcon = repo.Get(x => x.layer_id == input.layer_id && x.network_status == input.network_status && x.status == input.status && x.category == input.category && x.is_virtual == input.is_virtual);
                if (objIcon != null)
                {
                    objIcon.icon_name = input.icon_name;
                    objIcon.icon_path = input.icon_path;
                    objIcon.status = input.status;
                    objIcon.modified_by = input.created_by;
                    objIcon.modified_on = DateTimeHelper.Now;
                    return repo.Update(objIcon);

                }
                else
                {

                    input.created_by = input.created_by;
                    input.created_on = DateTimeHelper.Now;
                    return repo.Insert(input);


                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //public List<DropDownMaster> GetEntityTypeDropdownList(int layer_id)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<DropDownMaster>("fn_get_entity_type_dropdownlist", new { p_layer_id = layer_id });
        //    }
        //    catch { throw; }
        //}

        public void saveLandbaseDetailsInLayerIcon(LandBaseMaster obj)
        {
            try
            {
                LayerIconMapping ObjLIM = new LayerIconMapping();
                var lst = repo.ExecuteProcedure<LayerIconMapping>("fn_insert_landbasedetailsinlayericon", new
                {
                    p_icon_name = obj.icon_name,
                    p_icon_path = obj.icon_path,
                    p_network_status = "O",
                    p_status = true,
                    p_landbase_layer_id = obj.id,
                    p_userId = obj.user_id
                }, true); ;
            }
            catch { throw; }
        }

    }
    #endregion


    public class DATicketType : Repository<TicketTypeMaster>
    {
        public List<TicketTypeMaster> GetTicketTypeDetails(ViewTicketType objTPMaster)
        {
            try
            {
                var lst = repo.ExecuteProcedure<TicketTypeMaster>("fn_get_ticket_type_details", new
                {
                    p_pageno = objTPMaster.currentPage,
                    p_pagerecord = objTPMaster.pageSize,
                    p_sortcolname = objTPMaster.sort,
                    p_sorttype = objTPMaster.orderBy,
                    p_searchBy = objTPMaster.searchBy,
                    p_searchText = objTPMaster.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }
        public string SaveTicketType(TicketTypeMaster objTicketType)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_validate_ticket_type", new
                {
                    p_id = objTicketType.id,
                    p_ticket_type = objTicketType.ticket_type,
                    p_flag = "",
                }).FirstOrDefault();
                var result = "Failed";
                var objExisiting = repo.GetById(m => m.id == objTicketType.id);
                if (objExisiting != null)
                {
                    objExisiting.ticket_type = objTicketType.ticket_type;
                    objExisiting.description = objTicketType.description;
                    objExisiting.color_code = objTicketType.color_code;
                    objExisiting.icon_class = objTicketType.icon_class;
                    objExisiting.icon_content = objTicketType.icon_content;
                    objExisiting.abbreviation = objTicketType.abbreviation;
                    objExisiting.modified_by = objTicketType.created_by;
                    objExisiting.modified_on = DateTimeHelper.Now;
                    repo.Update(objExisiting);
                    result = "Update";
                }
                else
                {
                    if (response.status)
                    {
                        objTicketType.created_on = DateTimeHelper.Now;
                        objTicketType.created_by = objTicketType.created_by;
                        repo.Insert(objTicketType);
                        result = "Save";
                    }
                    else
                    {
                        result = response.message;
                    }

                }

                return result;
            }
            catch { throw; }
        }
        public TicketTypeMaster GetTicketTypeById(int id)
        {
            try
            {
                return repo.Get(u => u.id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<TicketTypeMaster> GetAllTicketType()
        {
            try
            {
                return repo.GetAll().ToList();
                //var lst = repo.ExecuteProcedure<TicketTypeMaster>("fn_get_ticket_type", new
                //{

                //}, true);
                //return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string DeleteTicketTypeById(int id)
        {
            try
            {
                var result = "False";
                var response = repo.ExecuteProcedure<DbMessage>("fn_validate_ticket_type", new
                {
                    p_id = id,
                    p_ticket_type = "",
                    p_flag = "DELETE",
                }).FirstOrDefault();
                var objId = repo.Get(x => x.id == id);
                if (objId != null)
                {
                    if (response.status)
                    {
                        repo.Delete(objId.id);
                        result = "DELETE";
                    }
                    else
                    {
                        result = response.message;
                    }
                }

                return result;

            }
            catch { throw; }

        }
        public string GetDesignId(int systemId, string EntityType)
        {
            return repo.ExecuteProcedure<string>("FN_GET_DESIGN_ID", new { p_system_id = systemId, p_entity_type = EntityType }).FirstOrDefault();
        }

        //priyankaint userID,
        public Pushgislogrequest Getpushrequest(int systemId, string EntityType, int userID, string sAction, string process_id, string sMessage = "")
        {
            Pushgislogrequest objPushgislogrequest = new Pushgislogrequest();
            try
            {
                objPushgislogrequest = repo.ExecuteProcedure<Pushgislogrequest>("fn_push_to_gis_requestlog", new
                {
                    p_system_id = systemId,
                    p_entity_type = EntityType,
                    status = sMessage,
                    p_created_by = userID,
                    p_action = sAction,
                    p_process_id = process_id
                }, true).FirstOrDefault();
                if (objPushgislogrequest == null)
                {
                    objPushgislogrequest = new Pushgislogrequest();
                    objPushgislogrequest.process_message = "";
                    objPushgislogrequest.bt_lock = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objPushgislogrequest;
        }
        public Pushgislogrequest Getpushrequestselect(int systemId, string EntityType)
        {
            Pushgislogrequest objPushgislogrequest = new Pushgislogrequest();
            try
            {
                objPushgislogrequest = repo.ExecuteProcedure<Pushgislogrequest>("fn_push_to_gis_requestlogselect", new
                {
                    p_system_id = systemId,
                    p_entity_type = EntityType,

                }, true).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objPushgislogrequest;
        }
        public Pushgislogrequest GetpuUpdatepushrequestshrequest(int systemId, string EntityType, int userID, string sAction, string sMessage = "")
        {
            Pushgislogrequest objPushgislogrequest = new Pushgislogrequest();
            try
            {
                objPushgislogrequest = repo.ExecuteProcedure<Pushgislogrequest>("fn_push_to_gis_requestlog", new
                {
                    p_system_id = systemId,
                    p_entity_type = EntityType,
                    status = sMessage,
                    p_created_by = userID,
                    p_action = sAction
                }, true).FirstOrDefault();
                if (objPushgislogrequest == null)
                {
                    objPushgislogrequest = new Pushgislogrequest();
                    objPushgislogrequest.process_message = "";
                    objPushgislogrequest.bt_lock = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objPushgislogrequest;
        }
    }

    public class DAVoiceCommandMaster : Repository<VoiceCommandMaster>
    {
        public List<VoiceCommandMaster> getVoiceCommandMaster()
        {
            try
            {
                return repo.GetAll().ToList();
            }
            catch { throw; }
        }

        public List<SaveVoiceCommandMaster> GetVoiceCommandDetail(VoiceCommandMaster model)
        {
            try
            {
                var res = repo.ExecuteProcedure<SaveVoiceCommandMaster>("fn_voicecommand_get_detail", new
                {

                    p_pageno = model.currentPage,
                    p_pagerecord = model.pageSize,
                    p_sortcolname = model.sort,
                    p_sorttype = model.orderBy,
                    p_searchby = Convert.ToString(model.searchBy),
                    p_searchtext = Convert.ToString(model.searchText),
                }, true);
                return res;
            }
            catch { throw; }
        }

        public string GetVoiceCommandJsonString()
        {
            try
            {
                return repo.ExecuteProcedure<string>("fn_getvoicecommand_jsondata", new { }).FirstOrDefault();

            }
            catch { throw; }
        }
    }

    public class DASaveVoiceCommandMaster : Repository<SaveVoiceCommandMaster>
    {
        public SaveVoiceCommandMaster GetVoiceCommandDetailsByID(int id)
        {
            return repo.Get(m => m.id == id);
        }
        public SaveVoiceCommandMaster SaveVoiceCommandSettings(SaveVoiceCommandMaster objVoiceCommandMaster)
        {

            try
            {

                if (objVoiceCommandMaster.id != 0)
                {

                    objVoiceCommandMaster.modified_by = objVoiceCommandMaster.created_by;
                    objVoiceCommandMaster.modified_on = DateTimeHelper.Now;
                    objVoiceCommandMaster.created_on = objVoiceCommandMaster.created_on;
                    return repo.Update(objVoiceCommandMaster);

                }

                else
                {
                    objVoiceCommandMaster.created_by = objVoiceCommandMaster.created_by;
                    objVoiceCommandMaster.created_on = DateTimeHelper.Now;
                    return repo.Insert(objVoiceCommandMaster);
                }
            }
            catch { throw; }
        }
    }

    public class DAVVoiceCommandMappingbulk : Repository<TempSaveVoiceCommandMaster>

    {
        public void VoiceCommandMappingBulkUpload(List<TempSaveVoiceCommandMaster> SaveVoiceCommandMappinglist)
        {
            try
            {
                repo.Insert(SaveVoiceCommandMappinglist);
            }
            catch { throw; }
        }
        public Tuple<int, int> getTotalUploadVoiceCommandMappingfailureAndSuccess(int UserId)
        {
            try
            {
                var getTotalUploadVoiceCommandMappingfailure = repo.GetAll().Where(x => x.created_by == UserId & x.is_valid == false).Count();
                var getTotalUploadVoiceCommandMappingSuccess = repo.GetAll().Where(x => x.created_by == UserId & x.is_valid == true).Count();
                return Tuple.Create(getTotalUploadVoiceCommandMappingSuccess, getTotalUploadVoiceCommandMappingfailure);
            }
            catch { throw; }
        }
        public DbMessage UploadVoiceCommandMappingForInsert(int userID)
        {
            try
            {
                // return repo.ExecuteProcedure<DbMessage>("fn_bulk_upload_VendorSpecifation_insert", new { P_UserId = userID }).FirstOrDefault();
                return repo.ExecuteProcedure<DbMessage>("fn_bulk_upload_voicecommandmapping_insert", new { P_UserId = userID }).FirstOrDefault();
            }
            catch { throw; }
        }

        public void DeleteTempVoiceCommandMappingData(int UserId)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_temp_voice_command_mapping_data", new { P_Userid = UserId });
            }
            catch { throw; }
        }

        public List<TempSaveVoiceCommandMaster> GetUploadVoiceCommandMappingLogs(int UserId)
        {
            try
            {
                return repo.GetAll().Where(x => x.created_by == UserId).ToList();
            }
            catch { throw; }
        }


    }

    public class DAUserActivityLogSettings : Repository<UserActivityLogSettings>
    {
        DAUserActivityLogSettings()
        {

        }
        private static DAUserActivityLogSettings objUserActivityLogSettings = null;
        private static readonly object lockObject = new object();
        public static DAUserActivityLogSettings Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objUserActivityLogSettings == null)
                    {
                        objUserActivityLogSettings = new DAUserActivityLogSettings();
                    }
                }
                return objUserActivityLogSettings;
            }
        }

        public List<UserActivityLogSettings> GetUserActivityLogSettings()
        {
            try
            {
                return repo.GetAll().Where(x => x.is_active == true).ToList();

            }
            catch { throw; }
        }
    }
    public class DAUserActivityLog : Repository<UserActivityLog>
    {
        DAUserActivityLog()
        {

        }
        private static DAUserActivityLog objUserActivityLog = null;
        private static readonly object lockObject = new object();
        public static DAUserActivityLog Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objUserActivityLog == null)
                    {
                        objUserActivityLog = new DAUserActivityLog();
                    }
                }
                return objUserActivityLog;
            }
        }

        public List<UserActivityLog> GetUserActivityLogDetails(CommonGridAttributes objUserActivityLogFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<UserActivityLog>("fn_get_user_activitylog", new
                {
                    p_pageno = objUserActivityLogFilter.currentPage,
                    p_pagerecord = objUserActivityLogFilter.pageSize,
                    p_sortcolname = objUserActivityLogFilter.sort,
                    p_sorttype = objUserActivityLogFilter.orderBy,
                    p_searchBy = objUserActivityLogFilter.searchBy,
                    p_searchText = objUserActivityLogFilter.searchText,
                    p_searchfrom = objUserActivityLogFilter.fromDate,
                    p_searchto = objUserActivityLogFilter.toDate,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        public UserActivityLog SaveUserActivityLog(UserActivityLog obj)
        {
            try
            {
                return repo.Insert(obj);
            }
            catch { throw; }
        }
    }
    public class DAUserActivityLogSettingsPage : Repository<UserActivityLogSettings>
    {
        public List<UserActivityLogSettingsPage> GetUserActivityLogsSettings(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<UserActivityLogSettingsPage>("fn_get_user_activity_log_settings", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    is_active = objGridAttributes.is_active,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy
                }, true);
            }
            catch { throw; }
        }
        public bool EditLogStatus(int id, bool status)
        {

            try
            {
                bool success = false;
                var objLog = repo.Get(u => u.id == id);
                if (objLog != null)
                {
                    objLog.is_active = status;
                    repo.Update(objLog);
                    success = true;
                }
                return success;
            }
            catch { throw; }
        }


    }
    public class DAEntityDeleteLog : Repository<EntityDeleteLog>
    {
        DAEntityDeleteLog()
        {

        }
        private static DAEntityDeleteLog objEntityDeleteLog = null;
        private static readonly object lockObject = new object();
        public static DAEntityDeleteLog Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objEntityDeleteLog == null)
                    {
                        objEntityDeleteLog = new DAEntityDeleteLog();
                    }
                }
                return objEntityDeleteLog;
            }
        }

        public List<EntityDeleteLog> GetEntityDeleteLogDetails(CommonGridAttributes objEntityDeleteLogFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<EntityDeleteLog>("fn_get_entity_delete_log", new
                {
                    p_pageno = objEntityDeleteLogFilter.currentPage,
                    p_pagerecord = objEntityDeleteLogFilter.pageSize,
                    p_sortcolname = objEntityDeleteLogFilter.sort,
                    p_sorttype = objEntityDeleteLogFilter.orderBy,
                    p_searchby = objEntityDeleteLogFilter.searchBy,
                    p_searchtext = objEntityDeleteLogFilter.searchText,
                    p_searchfrom = objEntityDeleteLogFilter.fromDate,
                    p_searchto = objEntityDeleteLogFilter.toDate,
                }, true);
                //var lst = repo.GetAll().ToList();
                return lst;

            }
            catch { throw; }
        }

    }
    public class DAAutoCodificationLog : Repository<AutoCodificationLog>
    {
        DAAutoCodificationLog()
        {

        }
        private static DAAutoCodificationLog objAutoCodificationLog = null;
        private static readonly object lockObject = new object();
        public static DAAutoCodificationLog Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objAutoCodificationLog == null)
                    {
                        objAutoCodificationLog = new DAAutoCodificationLog();
                    }
                }
                return objAutoCodificationLog;
            }
        }

        public List<AutoCodificationLog> GetAutoCodificationLogDetails(CommonGridAttributes objAutoCodificationLogFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<AutoCodificationLog>("fn_get_Auto_Codification_log", new
                {
                    p_pageno = objAutoCodificationLogFilter.currentPage,
                    p_pagerecord = objAutoCodificationLogFilter.pageSize,
                    p_sortcolname = objAutoCodificationLogFilter.sort,
                    p_sorttype = objAutoCodificationLogFilter.orderBy,
                    p_searchBy = objAutoCodificationLogFilter.searchBy,
                    p_searchText = objAutoCodificationLogFilter.searchText,
                    p_searchfrom = objAutoCodificationLogFilter.fromDate,
                    p_searchto = objAutoCodificationLogFilter.toDate,
                }, true);
                //var lst = repo.GetAll().ToList();
                return lst;

            }
            catch { throw; }
        }


        //priyanka
        //priyankaint userID,
        public Pushgislogrequest Getpushrequest(int systemId, string EntityType, int userID, string sAction, string sMessage = "")
        {
            Pushgislogrequest objPushgislogrequest = new Pushgislogrequest();
            try
            {
                objPushgislogrequest = repo.ExecuteProcedure<Pushgislogrequest>("fn_push_to_gis_requestlog", new
                {
                    p_system_id = systemId,
                    p_entity_type = EntityType,
                    status = sMessage,
                    p_created_by = userID,
                    p_action = sAction
                }, true).FirstOrDefault();
                if (objPushgislogrequest == null)
                {
                    objPushgislogrequest = new Pushgislogrequest();
                    objPushgislogrequest.process_message = "";
                    objPushgislogrequest.bt_lock = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objPushgislogrequest;
        }
        public Pushgislogrequest Getpushrequestselect(int systemId, string EntityType)
        {
            Pushgislogrequest objPushgislogrequest = new Pushgislogrequest();
            try
            {
                objPushgislogrequest = repo.ExecuteProcedure<Pushgislogrequest>("fn_push_to_gis_requestlogselect", new
                {
                    p_system_id = systemId,
                    p_entity_type = EntityType,

                }, true).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objPushgislogrequest;
        }




        public Pushgislogrequest GetpuUpdatepushrequestshrequest(int systemId, string EntityType, int userID, string sAction, string sMessage = "")
        {
            Pushgislogrequest objPushgislogrequest = new Pushgislogrequest();
            try
            {
                objPushgislogrequest = repo.ExecuteProcedure<Pushgislogrequest>("fn_push_to_gis_requestlog", new
                {
                    p_system_id = systemId,
                    p_entity_type = EntityType,
                    status = sMessage,
                    p_created_by = userID,
                    p_action = sAction
                }, true).FirstOrDefault();
                if (objPushgislogrequest == null)
                {
                    objPushgislogrequest = new Pushgislogrequest();
                    objPushgislogrequest.process_message = "";
                    objPushgislogrequest.bt_lock = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objPushgislogrequest;
        }



    }
    public class DABulkAssociatonRequestLog : Repository<BulkAssociationRequestLog>
    {
        public BulkAssociationRequestLog SaveBulkAssociationLog(BulkAssociationRequestLog obj)
        {
            try
            {
                return repo.Insert(obj);
            }
            catch { throw; }
        }
    }
    public class DAWorkAreaMarking : Repository<WorkAreaMarking>
    {
        public DbMessage SaveWorkAreaMarking(WorkAreaMarking obj)
        {
            try
            {
                string _lstMarkings = JsonConvert.SerializeObject(obj.lstMarkings);
                return repo.ExecuteProcedure<DbMessage>("fn_save_workarea_marking", new
                {
                    p_marking_type = obj.marking_type,
                    p_marking_id = obj.marking_id,
                    p_lstmarkings = _lstMarkings,
                    p_workspace_id = obj.workspace_id,
                    p_geom = obj.geom,
                    p_zoom = obj.zoom,
                    p_created_by = obj.created_by
                }).FirstOrDefault();
            }
            catch { throw; }
        }
        public WorkAreaMarking GetWorkAreaMarkingById(int marking_id)
        {
            try
            {
                return repo.Get(u => u.marking_id == marking_id);
            }
            catch (Exception ex) { throw ex; }
        }
        public List<WorkAreaMarking> GetWorkareaByWorkspaceId(int workspace_id)
        {
            try
            {
                return repo.GetAll(u => u.workspace_id == workspace_id).ToList();
            }
            catch (Exception ex) { throw ex; }
        }
        public int DeleteMarkings(int workSpaceId)
        {
            try
            {
                int result = 0;
                var objworkspace = repo.GetAll(u => u.marking_id == workSpaceId).ToList();
                if (objworkspace.Count > 0)
                {
                    foreach (var item in objworkspace)
                    {
                        result = repo.Delete(item.marking_id);
                    }
                    return result;
                }
                else
                {
                    return -1;
                }
                return result;
            }
            catch (Exception ex) { throw; }
        }

    }

}

