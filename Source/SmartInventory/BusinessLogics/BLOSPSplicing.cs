using DataAccess;
using DataAccess.ISP;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLOSPSplicing
    {

        public List<SplicingEntity> getEntityForSplicing(double latitude, double longitude, double bufferRadius, int roleId)
        {
            return new DAOSPSplicing().getEntityForSplicing(latitude, longitude, bufferRadius, roleId);
        }
        public List<SplicingConectionInfo> getSplicingInfo(connectionInput objSplicingIn, string listConnectors)
        {
            return new DAOSPSplicing().getSplicingInfo(objSplicingIn, listConnectors);
        }

        public Boolean CheckSplicingPermission(int system_id, string entity_type)
        {
            return new DAOSPSplicing().CheckSplicingPermission(system_id, entity_type);
        }
        public List<SplicingConectionInfo> getSplicingInfoReport(connectionInput objSplicingIn, string listConnectors)
        {
            return new DAOSPSplicing().getSplicingInfoReport(objSplicingIn, listConnectors);
        }
        public List<ExportSplicing> getEntitySplicingReport(connectionInput objSplicingIn)
        {
            return new DAOSPSplicing().getEntitySplicingReport(objSplicingIn);
        }
        public AvailablePorts getAvailablePorts(int systemId, string entityType)
        {
            return new DAOSPSplicing().getAvailablePorts(systemId, entityType);
        }
        public List<EquipementSearchResult> GetSearchEquipmentResult(string srchText, string entityType, int userId)
        {
            return new DAOSPSplicing().GetSearchEquipmentResult(srchText, entityType, userId);
        }
        public List<EquipementSearchResult> GetSearchEquipmentResult(string srchText, int userId, string linkId)
        {
            return new DAOSPSplicing().GetSearchEquipmentResult(srchText, userId, linkId);
        }
        public List<EquipementSearchResult> GetSearchEquipmentResult(int userId, string linkId)
        {
            return new DAOSPSplicing().GetSearchEquipmentResult(userId, linkId);
        }
        public List<LogicalViewEquipementSearch> GetLogicalViewSearchEquipmentResult(string srchText, string entityType, int userId)
        {
            return new DAOSPSplicing().GetLogicalViewSearchEquipmentResult(srchText, entityType, userId);
        }
        public List<EquipementPort> GetEquipmentPort(int systemid, string entity_type)
        {
            return new DAOSPSplicing().GetEquipmentPort(systemid, entity_type);
        }
        public connectionInfoPath GetConnectionInfoPath(ConnectionInfoFilter objFilterAttributes)
        {
            return new DAOSPSplicing().GetConnectionInfoPath(objFilterAttributes);
        }
        public SchematicView GetSchematicView(ConnectionInfoFilter objFilterAttributes, bool isUpstream, bool isConnectedPort)
        {
            return new DAOSPSplicing().GetSchematicView(objFilterAttributes, isUpstream, isConnectedPort);
        }
        public VizButterFlyNetwork GetVizButterflyNetwork(int systemId, string entityType)
        {
            return new DAOSPSplicing().GetVizButterflyNetwork(systemId, entityType);
        }
        public SLDModel GetSLDDiagram(int entityId, string entityType, string sldType)
        {
            return new DAOSPSplicing().GetSLDDiagram(entityId, entityType, sldType);
        }
        public List<ConnectionInfo> GetConnectionInfo(ConnectionInfoFilter objFilterAttributes)
        {
            return new DAOSPSplicing().GetConnectionInfo(objFilterAttributes);
        }
        public List<connectedCusotmer> GetConnectedCustomerDetails(ConnectionInfoFilter objFilterAttributes)
        {
            return new DAOSPSplicing().GetConnectedCustomerDetails(objFilterAttributes);
        }
        public List<middleWarePorts> middleWarePorts(int systemId, string entityType)
        {
            return new DAOSPSplicing().middleWarePorts(systemId, entityType);
        }

        public List<ViewLinkBudgetDataDetail> GetLinkBudgetDetails(LinkBudgetFilter objFilterAttributes)
        {
            return new DAOSPSplicing().GetLinkBudgetDetails(objFilterAttributes);
        }
        public List<CPFElements> GetCPFElement(ConnectionInfoFilter objFilterAttributes)
        {
            return new DAOSPSplicing().GetCPFElement(objFilterAttributes);
        }
        public DbMessage SaveConnectionInfo(string connections)
        {
            return new DAConnectionInfo().SaveConnectionInfo(connections);
        }
        public DbMessage SaveUtilizationNotification(ConnectionInfoMaster objConection)
        {
            return new DAConnectionInfo().SaveUtilizationNotification(objConection);
        }
        public DbMessage deleteConnection(string listConnection)
        {
            return new DAConnectionInfo().deleteConnection(listConnection);
        }
        public DbMessage deleteModelConnection(int connectionId)
        {
            return new DAConnectionInfo().deleteModelConnection(connectionId);
        }
        public DbMessage utilizationReset(string listConnection)
        {
            return new DAConnectionInfo().utilizationReset(listConnection);
        }

        //public DbMessage uploadBulkConnections(string listConnectionInfo, int userId)
        //{
        //    return new DATempConnectionInfo().uploadBulkConnections(listConnectionInfo, userId);
        //}

        public DbMessage uploadBulkConnections(string splicing_type, int userId)
        {
            return new DATempConnectionInfo().uploadBulkConnection(splicing_type, userId);
        }

        public DbMessage BulkUploadvalidateTempConnection(int userId, string splcing_type)
        {
            return new DATempConnectionInfo().BulkUploadvalidateTempConnection(userId, splcing_type);
        }
        public void BulkUploadTempConnection(List<TempConnectionInfoMaster> listConnectionInfo)
        {
            new DATempConnectionInfo().BulkUploadTempConnection(listConnectionInfo);
        }
        public Tuple<int, int> getTotalUploadConnectionfailureAndSuccess(int UserId)
        {
            return new DATempConnectionInfo().getTotalUploadConnectionfailureAndSuccess(UserId);
        }
        public List<TempConnectionInfoMaster> GetUploadConnectionLogs(int userId)
        {
            return new DATempConnectionInfo().GetUploadConnectionLogs(userId);
        }
        public void DeleteTempConnectiongData(int UserId)
        {
            new DATempConnectionInfo().DeleteTempConnectiongData(UserId);
        }
        public List<SplicingEntity> getISPEntityForSplicing(int structureId, int systemId, string entityType, int point_x, int point_y, int roleId)
        {
            return new DAOSPSplicing().getISPEntityForSplicing(structureId, systemId, entityType, point_x, point_y, roleId);
        }
        public DbMessage ValidtaeConnections(string connections)
        {
            return new DAConnectionInfo().ValidtaeConnections(connections);
        }
        public DbMessage ValidateConnections(string entitytype, int systemId, int from, int to)
        {
            return new DAConnectionInfo().ValidateConnections(entitytype, systemId, from, to);
        }
        public List<LogicalViewPortDetail> getEntityLogicalView(int systemid, string entitytype)
        {
            return new DAOSPSplicing().getEntityLogicalView(systemid, entitytype);
        }
        public DbMessage UpdateCorePortStatus(UpdateCorePortStatus obj)
        {
            return new DAConnectionInfo().UpdateCorePortStatus(obj);
        }

        public List<DropDownMaster> getPODList(int structureId)
        {
            return DARoomInfo.Instance.getPODList(structureId);
        }
        public List<SplicingConectionInfo> getModelSplicingInfo(int sourceId, string sourceType, int destinationId, string destinationType, bool isInsideConnectivity)
        {
            return new DAOSPSplicing().getModelSplicingInfo(sourceId, sourceType, destinationId, destinationType, isInsideConnectivity);
        }
        public List<MultipleConnections> getMultipleConnections(int systemId, int portNo, string portType)
        {
            return new DAOSPSplicing().getMultipleConnections(systemId, portNo, portType);
        }

        public List<ExportPatchingInfo> getPatchingConnection(int source_id, string source_type, string parent_type, int parent_id)
        {
            return new DAOSPSplicing().getPatchingConnection(source_id, source_type, parent_type, parent_id);
        }
        public List<PortHistory> viewPortHistory(int systemid)
        {
            return new DAOSPSplicing().viewPortHistory(systemid);
        }
        public List<connectedCusotmer> GetConnectedCustomerDetailsInInfo(ConnectionInfoFilter objFilterAttributes)
        {
            return new DAOSPSplicing().GetConnectedCustomerDetailsInInfo(objFilterAttributes);
        }
        public List<NotificationUtlization> GetNotificationUtlization(ConnectionInfoMaster objConection)
        {
            return new DAOSPSplicing().GetNotificationUtlization(objConection);
        }

        public List<string> GetNotificationFor50PercentUtlization()
        {
            return new DAOSPSplicing().GetNotificationFor50PercentUtlization();
        }

    }
}
