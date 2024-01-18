using DataAccess;
using DataAccess.ISP;
using Models;
using Models.ISP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.ISP
{
    public class BLISP
    {
        private static BLISP objBLISP = null;
        private static readonly object lockObject = new object();
        public static BLISP Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLISP == null)
                    {
                        objBLISP = new BLISP();
                    }
                }
                return objBLISP;
            }
        }
        public ISPViewModel getShaftNFloor(int structureId)
        {
            ISPViewModel objISPViewModel = new ISPViewModel();
            var shaftFloors = DAISP.Instance.getShaftNFloor(structureId);
            objISPViewModel.ShaftList = shaftFloors.Where(m => m.isshaft == true).ToList();
            objISPViewModel.FloorList = shaftFloors.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();
            return objISPViewModel;

        }
        public FloorInfo UpdateFloorInfo(FloorInfo model)
        {
            return DAFloorInfo.Instance.UpdateFloorInfo(model);
        }
        public bool SaveShaft(ShaftInfo model, int userid)
        {
            return DAShaft.Instance.SaveShaftInfo(model, userid);
        }
        public bool SaveShaftRange(List<ispShaftRange> model, int shaftId, int structureId, int userid)
        {
            return DAShaftRange.Instance.SaveShaftRange(model, shaftId, structureId, userid);
        }

        public bool DeleteShaftRange(int shaftId, int structureId)
        {
            return DAShaftRange.Instance.DeleteShaftRange(shaftId, structureId);
        }
        public List<ElementTemplateView> getElementTemplate(string ElementType, int userId)
        {
            return DAElementTemplate.Instance.getElementTemplate(ElementType, userId);
        }

        //public List<ShaftFloorElementView> shaftNfloorElements(int structureId)
        //{
        //    return DAISP.Instance.shaftNfloorElements(structureId);
        //}
        public List<StructureElement> StructureElements(int structureId,int role_id)
        {
            return DAISP.Instance.StructureElements(structureId, role_id);
        }
        public List<StructureElement> CurrentStructureElements(int structureId, int systemId, string entityType)
        {
            return DAISP.Instance.CurrentStructureElements(structureId, systemId, entityType);
        }
        public int getTotalOSPCable(int structureId)
        {
            return DACable.Instance.getTotalOSPCable(structureId);
        }
        public List<StructureCable> getStructureCables(int structureId)
        {
            return DAISP.Instance.getStructureCables(structureId);
        }
        public List<SplitterParentView> getSplitterParents(int structureId)
        {
            return DAISP.Instance.getSplitterParents(structureId);
        }
        public bool DeleteRoomById(int systemId)
        {
            return DARoomInfo.Instance.DeleteRoomById(systemId);
        }
        public RoomInfo getRoomDetails(int roomId)
        {
            return DARoomInfo.Instance.getRoomDetails(roomId);
        }
        public IspEntityMapping getMappingByEntityId(int systemId, string entityTye)
        {
            return DAISPEntityMapping.Instance.getMappingByEntityId(systemId, entityTye);
        }
        public HTBInfo getHTBDetails(int systemId)
        {
            return DAHTBInfo.Instance.getHTBDetails(systemId);
        }
        public OpticalRepeaterInfo getOpticalRepeaterDetails(int systemId)
        {
            return DAOpticalRepeaterInfo.Instance.getOpticalRepeaterDetails(systemId);
        }
        public FDBInfo getFDBDetails(int systemId)
        {
            return DAFDBInfo.Instance.getFDBDetails(systemId);
        }
        public List<FDBInfo> getFDBDetailsbySourceRefId(string sourceRefId)
        {
            return DAFDBInfo.Instance.getFDBDetailsbySourceRefId(sourceRefId);
        }
		public DbMessage getEntityVerificationStatusbySourceRefId(string sourceRefId)
		{
			return DAFDBInfo.Instance.getEntityVerificationStatusbySourceRefId(sourceRefId);
		}
		public Boolean IsAllSplitterTraceValid(string sourceRefId)
		{
			return DAFDBInfo.Instance.IsAllSplitterTraceValid(sourceRefId);
		}

		public RoomInfo SaveRoomDetails(RoomInfo model)
        {
            return DARoomInfo.Instance.SaveRoomDetails(model);
        }
        public HTBInfo SaveHTBDetails(HTBInfo model, int userid)
        {
            return DAHTBInfo.Instance.SaveHTBDetails(model, userid);
        }
        public OpticalRepeaterInfo SaveOpticalRepeaterDetails(OpticalRepeaterInfo model, int userid)
        {
            return DAOpticalRepeaterInfo.Instance.SaveOpticalRepeaterDetails(model, userid);
        }
        public FDBInfo SaveFDBDetails(FDBInfo model)
        {
            return DAFDBInfo.Instance.SaveFDBDetails(model);
        }
        public layerDetail getLayerDetails(string ElementType)
        {
            return ISPLayerDetails.Instance.getLayerDetails(ElementType);
        }
        public RoomTemplate getRoomTemplate(int userId)
        {
            return DaRoomTemplate.Instance.getRoomTemplate(userId);
        }
        public HTBTemplate getHTBTemplate(int userId)
        {
            return DAHTBTemplate.Instance.getHTBTemplate(userId);
        }
        public OpticalRepeaterTemplate getOpticalRepeaterTemplate(int userId)
        {
            return DAOpticalRepeaterTemplate.Instance.getOpticalRepeaterTemplate(userId);
        }
        public FDBTemplate getFDBTemplate(int userId)
        {
            return DAFDBTemplate.Instance.getFDBTemplate(userId);
        }
        public StructureMaster getSructureDetailsByCode(int strId)
        {
            return DAStructure.Instance.getSructureDetailsById(strId);

        }
        public FloorInfo getFloorInfo(int? floorId)
        {
            return DAFloorInfo.Instance.getFloorInfo(floorId);
        }
        public List<ispShaftRange> getShaftRangeInfo(int shaftId)
        {
            return DAShaftRange.Instance.getShaftRangeInfo(shaftId);
        }
        public List<FloorInfo> getFloorList(int stuctureId)
        {
            return DAFloorInfo.Instance.getFloorList(stuctureId);
        }
        public List<ispShaftRange> getShaftRangeByStructure(int Structureid)
        {
            return DAShaftRange.Instance.getShaftRangeByStructure(Structureid);
        }
        public ShaftInfo getShaftInfo(int? shaftId)
        {
            return DAShaft.Instance.getShaftInfo(shaftId);
        }
        public bool DeleteHTBById(int systemId)
        {
            return DAHTBInfo.Instance.DeleteHTBById(systemId);
        }
        public bool DeleteOpticalRepeaterById(int systemId)
        {
            return DAOpticalRepeaterInfo.Instance.DeleteOpticalRepeaterById(systemId);
        }
        public bool DeleteFDBById(int systemId)
        {
            return DAFDBInfo.Instance.DeleteFDBById(systemId);
        }
        public bool VerifiedBarcode(int systemId, string barcode)
        {
            return DAFDBInfo.Instance.VerifiedBarcode(systemId, barcode);
        }
		public DbMessage GetCreateTicketPermissionByGeom(string ticketType, string entityType, int? systemID)
		{
			return DAFDBInfo.Instance.GetCreateTicketPermissionByGeom(ticketType, entityType, systemID);
		}
		public bool UpdateManualBarcode(int systemId)
        {
            return DAFDBInfo.Instance.UpdateManualBarcode(systemId);
        }
        public List<RoomDimension> getRoomByFloorId(int structureId, int? floorId)
        {
            return DARoomInfo.Instance.getRoomByFloorId(structureId, floorId);
        }
       

        public RoomTemplate UpdateUnitTemplate(RoomTemplate UnitTemplateMaster)
        {
            return DaRoomTemplate.Instance.UpdateUnitTemplate(UnitTemplateMaster);
        }
        public HTBTemplate saveHTBTemplate(HTBTemplate HTBTemplateMaster)
        {
            return DAHTBTemplate.Instance.saveHTBTemplate(HTBTemplateMaster);
        }
        public OpticalRepeaterTemplate saveOpticalRepeaterTemplate(OpticalRepeaterTemplate OpticalRepeaterTemplateMaster)
        {
            return DAOpticalRepeaterTemplate.Instance.saveOpticalRepeaterTemplate(OpticalRepeaterTemplateMaster);
        }
        public FDBTemplate saveFDBTemplate(FDBTemplate HTBTemplateMaster,int userId)
        {
            return DAFDBTemplate.Instance.saveFDBTemplate(HTBTemplateMaster, userId);
        }
        public bool checkEntityAssociation(int systemId, string entityType)
        {
            return DAISP.Instance.checkEntityAssociation(systemId, entityType);
        }
        public bool checkTemplateExist(string entityType, int userId)
        {
            return DAISP.Instance.ChkEntityTemplateExist(entityType, userId);
        }
        public bool updateFloorName(int floorId, int structureid, string floorName)
        {
            return DAFloorInfo.Instance.updateFloorName(floorId, structureid, floorName);
        }
        public bool updateShaftName(int floorId, int structureid, string shaftName)
        {
            return DAShaft.Instance.updateShaftName(floorId, structureid, shaftName);
        }
        public List<ISPParentEntities> getParentEntities(int structureId)
        {
            return DAISP.Instance.getParentEntities(structureId);
        }
        public NewEntity getNewEntity(int systemId, string entityType, string networkId)
        {
            return DAISP.Instance.getNewEntity(systemId, entityType, networkId);
        }
        public void getShftEntityDetails(ShiftEntity objShiftEntity)
        {
            var layerDetails = ISPLayerDetails.Instance.getLayerDetails(objShiftEntity.entityType);
            var layerMappings = new DALayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == objShiftEntity.entityType).FirstOrDefault() != null)
            {
                objShiftEntity.isValidParent = true;
                objShiftEntity.UnitList = DARoomInfo.Instance.getAllParentInFloor(objShiftEntity.structureId, objShiftEntity.floorId, EntityType.UNIT.ToString());
            }
            var shaftFloors = DAISP.Instance.getShaftNFloor(objShiftEntity.structureId);

            objShiftEntity.ShaftList = shaftFloors.Where(m => m.isshaft == true).ToList();
            objShiftEntity.FloorList = shaftFloors.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();
            objShiftEntity.isShaftElement = layerDetails.is_shaft_element;
            objShiftEntity.isFloorElement = layerDetails.is_floor_element;
            objShiftEntity.isISPChildLayer = layerDetails.is_isp_child_layer;

        }
        public DbMessage SaveEntityPosition(ShiftEntity objShiftEntity)
        {
            return DAISP.Instance.SaveEntityPosition(objShiftEntity);
        }
        public List<StructureElement> getAllParentInFloor(int structureId, int floorId, string parentType)
        {
            return DARoomInfo.Instance.getAllParentInFloor(structureId, floorId, parentType);
        }
        public List<ConnectionInfo> getCPEConnections(int structureId)
        {
            return DAISP.Instance.getCPEConnections(structureId);
        }
        public List<EquipmentExportResult> GetEquipmentExportDetail(string SourceType, string SourceId, string parent_type, string parent_id)
        {
            return DAISP.Instance.GetEquipmentExportDetail(SourceType,SourceId, parent_type,parent_id);
        }

        public DbMessage UpdateEquipment(int system_id, string equipmentList, int user_id)
        {
            return DAISP.Instance.UpdateEquipment(system_id, equipmentList, user_id);
        }

        #region Additional-Attributes
        public string GetOtherInfoFDB(int systemId)
        {
            return DAFDBInfo.Instance.GetOtherInfoFDB(systemId);
        }
        #endregion

        #region Additional-Attributes
        public string GetOtherInfoHTB(int systemId)
        {
            return DAHTBInfo.Instance.GetOtherInfoHTB(systemId);
        }
        #endregion
        #region Additional-Attributes
        public string GetOtherInfoOpticalRepeater(int systemId)
        {
            return DAOpticalRepeaterInfo.Instance.GetOtherInfoOpticalRepeater(systemId);
        }
        #endregion
    }
}
