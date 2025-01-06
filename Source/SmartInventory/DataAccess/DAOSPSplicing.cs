using DataAccess.DBHelpers;
using Models;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAOSPSplicing : Repository<object>
    {
        public List<SplicingEntity> getEntityForSplicing(double latitude, double longitude, double bufferRadius, int roleId)
        {
            try
            {

                return repo.ExecuteProcedure<SplicingEntity>("fn_splicing_get_entity", new { longitude = longitude, latitude = latitude, p_buffer_radius = bufferRadius, p_role_id = roleId }, true).ToList();

            }
            catch
            {
                throw;
            }
        }
        public List<SplicingConectionInfo> getSplicingInfo(connectionInput objConInput, string listConnectors)
        {
            try
            {
                return repo.ExecuteProcedure<SplicingConectionInfo>("fn_splicing_get_connection",
                  new
                  {
                      p_source_system_id = objConInput.source_system_id,
                      p_source_entity_type = objConInput.source_entity_type,
                      p_is_source_start_point = objConInput.is_source_start_point,
                      p_connectors = listConnectors,
                      //p_connector_system_id =Convert.ToInt32(objConInput.connector_system_id),
                      //p_connector_entity_type = objConInput.connector_entity_type,
                      p_destination_system_id = objConInput.destination_system_id,
                      p_destination_entity_type = objConInput.destination_entity_type,
                      p_is_destination_start_point = objConInput.is_destination_start_point,
                      p_customer_ids = objConInput.customer_ids
                  }).ToList();
            }
            catch
            {
                throw;
            }
        }
        public Boolean CheckSplicingPermission(int system_id, string entity_type)
        {
            try
            {
                return repo.ExecuteProcedure<Boolean>("fn_Check_Splicing_Permission",
                  new
                  {
                      p_layer_name = entity_type,
                      p_system_id = system_id

                  }).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<SplicingConectionInfo> getSplicingInfoReport(connectionInput objConInput, string listConnectors)
        {
            try
            {
                return repo.ExecuteProcedure<SplicingConectionInfo>("fn_splicing_get_connection_report",
                  new
                  {
                      p_source_system_id = objConInput.source_system_id,
                      p_source_entity_type = objConInput.source_entity_type,
                      p_is_source_start_point = objConInput.is_source_start_point,
                      p_connectors = listConnectors,
                      //p_connector_system_id =Convert.ToInt32(objConInput.connector_system_id),
                      //p_connector_entity_type = objConInput.connector_entity_type,
                      p_destination_system_id = objConInput.destination_system_id,
                      p_destination_entity_type = objConInput.destination_entity_type,
                      p_is_destination_start_point = objConInput.is_destination_start_point,
                      p_customer_ids = objConInput.customer_ids
                  }).ToList();
            }
            catch
            {
                throw;
            }
        }


        public List<ExportSplicing> getEntitySplicingReport(connectionInput objConInput)
        {
            try
            {
                var lst = repo.ExecuteProcedure<ExportSplicing>("fn_splicing_get_entity_report",
                    new
                    {
                        P_PAGENO = objConInput.currentPage,
                        P_PAGERECORD = objConInput.pageSize,
                        P_SORTCOLNAME = objConInput.sort,
                        P_SORTTYPE = objConInput.orderBy,
                        p_systemid = objConInput.source_system_id,
                        p_entity_name = objConInput.source_entity_type.ToString()
                    }, true);
                return lst;

            }
            catch
            {
                throw;
            }
        }
        public AvailablePorts getAvailablePorts(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<AvailablePorts>("fn_get_available_ports",
                                  new
                                  {
                                      p_system_id = systemId,
                                      p_entity_type = entityType
                                  }).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }


        public List<EquipementSearchResult> GetSearchEquipmentResult(string srchText, string entityType, int userId)
        {
            try
            {

                return repo.ExecuteProcedure<EquipementSearchResult>("fn_get_search_equipment", new { p_searchtext = srchText.Trim(), p_entityType = entityType, p_user_id = userId });
            }
            catch { throw; }
        }
        public List<EquipementSearchResult> GetSearchEquipmentResult(string srchText, int userId, string linkId)
        {
            try
            {

                return repo.ExecuteProcedure<EquipementSearchResult>("fn_get_search_equipment", new { p_searchtext = srchText.Trim(), p_user_id = userId, p_link_id = linkId });
            }
            catch { throw; }
        }
        public List<EquipementSearchResult> GetSearchEquipmentResult(int userId, string linkId)
        {
            try
            {
                return repo.ExecuteProcedure<EquipementSearchResult>("fn_get_search_equipment", new { p_user_id = userId, p_link_id = linkId });
            }
            catch { throw; }
        }

        public List<LogicalViewEquipementSearch> GetLogicalViewSearchEquipmentResult(string srchText, string entityType, int userId)
        {
            try
            {

                return repo.ExecuteProcedure<LogicalViewEquipementSearch>("fn_get_logicalView_search_equipment", new { p_searchtext = srchText.Trim(), p_entityType = entityType, p_user_id = userId });
            }
            catch { throw; }
        }
        public List<EquipementPort> GetEquipmentPort(int systemid, string entity_type)
        {
            try
            {

                return repo.ExecuteProcedure<EquipementPort>("fn_get_equipement_port", new { p_system_id = systemid, p_entity_type = entity_type }, true);
            }
            catch { throw; }
        }
        public connectionInfoPath GetConnectionInfoPath(ConnectionInfoFilter objFilterAttributes)
        {
            try
            {

                return repo.ExecuteProcedure<connectionInfoPath>("fn_get_connection_info_path", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_entity_system_id = objFilterAttributes.entityid,
                    p_entity_port_no = objFilterAttributes.port_no,
                    p_entity_type = objFilterAttributes.entity_type
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public SchematicView GetSchematicView(ConnectionInfoFilter objFilterAttributes, bool isUpstream, bool isConnectedPort)
        {
            try
            {
                return repo.ExecuteProcedure<SchematicView>("fn_get_schematic_view", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_entity_system_id = objFilterAttributes.entityid,
                    p_entity_port_no = objFilterAttributes.port_no,
                    p_entity_type = objFilterAttributes.entity_type,
                    p_isUpstream = isUpstream,
                    p_isConnected_ports = isConnectedPort
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public VizButterFlyNetwork GetVizButterflyNetwork(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<VizButterFlyNetwork>("fn_get_vis_butterfly_network", new
                {
                    p_system_id = systemId,
                    p_entityType = entityType
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public SLDModel GetSLDDiagram(int entityId, string entityType, string sldType)
        {
            try
            {
                return repo.ExecuteProcedure<SLDModel>("fn_get_sld_data", new
                {
                    p_system_id = entityId,
                    p_entityType = entityType,
                    p_sldType = sldType
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<ConnectionInfo> GetConnectionInfo(ConnectionInfoFilter objFilterAttributes)
        {
            try
            {

                return repo.ExecuteProcedure<ConnectionInfo>("fn_get_connection_info", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_entity_system_id = objFilterAttributes.entityid,
                    p_entity_port_no = objFilterAttributes.port_no,
                    p_entity_type = objFilterAttributes.entity_type
                });
            }
            catch { throw; }
        }

        public List<connectedCusotmer> GetConnectedCustomerDetails(ConnectionInfoFilter objFilterAttributes)
        {
            try
            {

                return repo.ExecuteProcedure<connectedCusotmer>("fn_get_connected_customer_details", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_entity_system_id = objFilterAttributes.entityid,
                    p_entity_port_no = objFilterAttributes.port_no,
                    p_entity_type = objFilterAttributes.entity_type
                });
            }
            catch { throw; }
        }

        public List<ExportPatchingInfo> getPatchingConnection(int source_id, string source_type, string parent_type, int parent_id)
        {
            try
            {
                return repo.ExecuteProcedure<ExportPatchingInfo>("fn_export_patching_report", new
                {
                    p_source_system_id = source_id,
                    p_source_type = source_type,
                    p_parent_type = parent_type,
                    p_parent_id = parent_id.ToString()
                });
            }
            catch { throw; }
        }

        public List<middleWarePorts> middleWarePorts(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<middleWarePorts>("fn_isp_get_port_nested_names", new
                {
                    p_system_id = systemId,
                    p_entity_type = entityType
                }, true);
            }
            catch { throw; }
        }

        public List<ViewLinkBudgetDataDetail> GetLinkBudgetDetails(LinkBudgetFilter objLinkBudgetFilter)
        {
            try
            {
                return repo.ExecuteProcedure<ViewLinkBudgetDataDetail>("fn_get_link_budget_detail", new
                {
                    p_searchby = objLinkBudgetFilter.searchBy,
                    p_searchtext = objLinkBudgetFilter.searchText,
                    P_PAGENO = objLinkBudgetFilter.currentPage,
                    P_PAGERECORD = objLinkBudgetFilter.pageSize,
                    P_SORTCOLNAME = objLinkBudgetFilter.sort,
                    P_SORTTYPE = objLinkBudgetFilter.orderBy,
                    p_equipment_system_Id = objLinkBudgetFilter.equipmentsystemid,
                    p_equipment_portId = objLinkBudgetFilter.equipmentPortid,
                    p_equipmentType = objLinkBudgetFilter.equipmenttype,
                    p_Source_SystemId = objLinkBudgetFilter.sourcesystemid,
                    p_Source_port_no = objLinkBudgetFilter.sourceportno,
                    p_Source_entityType = objLinkBudgetFilter.sourceentitytype,
                    p_destination_SystemId = objLinkBudgetFilter.destinationsystemid,
                    p_destination_port_no = objLinkBudgetFilter.destinationportno,
                    p_destination_entityType = objLinkBudgetFilter.destinationentitytype,
                    p_transmit_power = objLinkBudgetFilter.transmitpower,
                    p_wavelength_id = objLinkBudgetFilter.wavelengthid,
                    p_is_upstream_only = objLinkBudgetFilter.isUpStreamOnly
                });
            }
            catch { throw; }
        }
        public List<CPFElements> GetCPFElement(ConnectionInfoFilter objFilterAttributes)
        {
            try
            {

                return repo.ExecuteProcedure<CPFElements>("fn_gettempconnection_element", new
                {

                    p_entityid = objFilterAttributes.entityid,
                    p_port_no = objFilterAttributes.port_no,
                    p_entitytype = objFilterAttributes.entity_type
                });
            }
            catch { throw; }
        }
        public List<PortHistory> viewPortHistory(int system_id)
        {
            try
            {
                return repo.ExecuteProcedure<PortHistory>("fn_view_port_history", new
                {
                    p_system_id = system_id

                });
            }
            catch { throw; }
        }
        public List<SplicingEntity> getISPEntityForSplicing(int structureId, int systemId, string entityType, int point_x, int point_y, int roleId)
        {
            try
            {

                return repo.ExecuteProcedure<SplicingEntity>("fn_splicing_get_isp_entity", new { p_structure_id = structureId, p_system_id = systemId, p_entity_type = entityType, p_point_entity_x = point_x, p_point_entity_y = point_y, p_role_id = roleId }, true).ToList();

            }
            catch
            {
                throw;
            }
        }

        public List<LogicalViewPortDetail> getEntityLogicalView(int systemid, string entitytype)
        {
            try
            {
                return repo.ExecuteProcedure<LogicalViewPortDetail>("fn_get_logical_view",
              new
              {
                  p_system_id = systemid,
                  p_entity_type = entitytype
              }, true).ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<SplicingConectionInfo> getModelSplicingInfo(int sourceId, string sourceType, int destinationId, string destinationType, bool isInsideConnectivity)
        {
            try
            {
                return repo.ExecuteProcedure<SplicingConectionInfo>("fn_splicing_isp_model_get_connection",
                  new
                  {
                      p_source_system_id = sourceId,
                      p_source_entity_type = sourceType,
                      p_destination_system_id = destinationId,
                      p_destination_entity_type = destinationType,
                      p_port_type = (isInsideConnectivity ? InOutPortType.I.ToString() : InOutPortType.O.ToString())
                  }).ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<MultipleConnections> getMultipleConnections(int systemId, int portNo, string portType)
        {
            try
            {
                return repo.ExecuteProcedure<MultipleConnections>("fn_splicing_get_model_connections_by_port_no", new
                {
                    p_system_id = systemId,
                    p_port_no = portNo,
                    p_port_type = portType
                }, true);
            }
            catch { throw; }
        }



        public List<connectedCusotmer> GetConnectedCustomerDetailsInInfo(ConnectionInfoFilter objFilterAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<connectedCusotmer>("fn_get_connected_customer_details_for_info", new
                {

                    p_entity_system_id = objFilterAttributes.entityid,

                    p_entity_type = objFilterAttributes.entity_type,
                });
            }
            catch { throw; }
        }
        public List<NotificationUtlization> GetNotificationUtlization(ConnectionInfoMaster objConection)
        {
            try
            {
                return repo.ExecuteProcedure<NotificationUtlization>("fn_get_UtilizationNotificationData", new
                {
                    p_source_system_id = objConection.source_system_id,
                    p_source_entity_type = objConection.source_entity_type,
                    p_destination_system_id = objConection.destination_system_id,
                    p_destination_entity_type = objConection.destination_entity_type,
                    p_equipment_id = objConection.equipment_system_id,
                    p_equipment_type = objConection.equipment_entity_type
                }, true);
            }
            catch { throw; }
        }

        public List<string> GetNotificationFor50PercentUtlization()
        {
            try
            {
                return repo.ExecuteProcedure<string>("fn_get_UtilizationData", new { }, false);
            }
            catch { throw; }
        }
    }
    public class DAConnectionInfo : Repository<ConnectionInfoMaster>
    {
        public DbMessage SaveConnectionInfo(string connections)
        {
            try
            {

                return repo.ExecuteProcedure<DbMessage>("fn_splicing_save_connections", new { p_connections = connections }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage SaveUtilizationNotification(ConnectionInfoMaster objConection)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_utilization_save_notification",
                    new
                    {
                        p_source_system_id = objConection.source_system_id,
                        p_source_entity_type = objConection.source_entity_type,
                        p_destination_system_id = objConection.destination_system_id,
                        p_destination_entity_type = objConection.destination_entity_type,
                        p_equipment_id = objConection.equipment_system_id,
                        p_equipment_type = objConection.equipment_entity_type,
                        p_created_by = objConection.created_by
                    }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public DbMessage deleteConnection(string listConnection)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_delete_connection", new { p_listConnection = listConnection }).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage utilizationReset(string listConnection)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_utilization_reset", new { p_listConnection = listConnection }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public bool checkIsLineEntitySpliced(int systemId, string entityType, bool isCableAEnd)
        {
            try
            {
                var connections = repo.GetAll(m => ((m.source_system_id == systemId && m.source_entity_type == entityType) || (m.destination_system_id == systemId && m.destination_entity_type == entityType)) && m.is_cable_a_end == isCableAEnd).ToList();
                return connections.Count > 0;
            }
            catch { throw; }
        }
        public DbMessage ValidtaeConnections(string connections)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_validate_connection", new { p_connections = connections }).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage ValidateConnections(string entitytype, int systemId, int from, int to)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_validate_connection_mobile", new { p_entitytype = entitytype, p_systemId = systemId, p_from = from, p_to = to }).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage UpdateCorePortStatus(UpdateCorePortStatus obj)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_update_port_status", new { p_entity_type = obj.entity_type, p_entity_system_id = obj.entity_system_id, p_core_port_no = obj.core_port_number, p_port_status_id = obj.portStatus, p_userid = obj.user_id, p_comment = obj.comment }, false).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage deleteModelConnection(int connectionId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_delete_model_connection", new { p_connection_id = connectionId }).FirstOrDefault();
            }
            catch { throw; }
        }
    }
    public class DATempConnectionInfo : Repository<TempConnectionInfoMaster>
    {


        public DbMessage BulkUploadvalidateTempConnection(int UserId, string splicing_type)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_validate_splicing", new { P_Userid = UserId, P_Splicing_Type = splicing_type }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public DbMessage uploadBulkConnection(string spcing_type, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_bulk_upload", new { p_splicing = spcing_type, p_user_id = userId }, true).FirstOrDefault();
            }
            catch { throw; }
        }


        public DbMessage uploadBulkConnections(string listConnectionInfo, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_upload_connections", new { p_connections = listConnectionInfo, userid = userId }, true).FirstOrDefault();

            }
            catch { throw; }
        }
        public void BulkUploadTempConnection(List<TempConnectionInfoMaster> tempCollectionList)
        {
            try
            {
                repo.Insert(tempCollectionList);

            }
            catch { throw; }
        }
        public Tuple<int, int> getTotalUploadConnectionfailureAndSuccess(int UserId)
        {
            try
            {
                var getTotalUploadConnectionfailure = repo.GetAll().Where(x => x.uploaded_by == UserId & x.is_valid == false).Count();
                var getTotalUploadConnectionSuccess = repo.GetAll().Where(x => x.uploaded_by == UserId & x.is_valid == true).Count();
                return Tuple.Create(getTotalUploadConnectionSuccess, getTotalUploadConnectionfailure);
            }
            catch { throw; }
        }
        public List<TempConnectionInfoMaster> GetUploadConnectionLogs(int userId)
        {
            try
            {
                return repo.GetAll(m => m.uploaded_by == userId).ToList();

            }
            catch { throw; }
        }
        public void DeleteTempConnectiongData(int UserId)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_temp_connections", new { P_Userid = UserId });
            }
            catch { throw; }
        }


    }

}
