using DataAccess.DBHelpers;
using Models;
using Models.ISP;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityType = Models.EntityType;

namespace DataAccess.ISP
{
    public class DAISP : Repository<ISPViewModel>
    {
        DAISP()
        {

        }
        private static DAISP objDAISP = null;
        private static readonly object lockObject = new object();
		public static DAISP Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAISP == null)
                    {
                        objDAISP = new DAISP();
                    }
                }
                return objDAISP;
            }
        }
        public List<StructureEntityView> getShaftNFloor(int structureId)
        {
            try
            {
                return repo.ExecuteProcedure<StructureEntityView>("fn_get_isp_records", new { structId = structureId }).ToList();
            }
            catch { throw; }
        }

        //public List<ShaftFloorElementView> shaftNfloorElements(int structureId)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<ShaftFloorElementView>("fn_get_isp_shaft_floor_elements", new { structId = structureId }).ToList();
        //    }
        //    catch { throw; }
        //}
        //public List<StructureElement> StructureElements(int structureId)
        //{
        //    try
        //    {

        //        //return repo.ExecuteProcedure<StructureElement>("fn_get_structure_elements", new { structId = structureId }, true).ToList();
        //        return repo.ExecuteProcedure<StructureElement>("fn_isp_get_structure_elements", new { structId = structureId }, true).ToList();
        //    }
        //    catch { throw; }
        //}
        public List<StructureElement> StructureElements(int structureId, int role_id)
        {
            try
            {

                //return repo.ExecuteProcedure<StructureElement>("fn_get_structure_elements", new { structId = structureId }, true).ToList();
                return repo.ExecuteProcedure<StructureElement>("fn_isp_get_structure_elements", new { structId = structureId, p_role_id = role_id }, true).ToList();
            }
            catch { throw; }
        }
        public List<StructureElement> CurrentStructureElements(int structureId, int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<StructureElement>("fn_isp_get_current_structure_element", new { p_structid = structureId, p_entity_systemid = systemId, p_entity_type = entityType }, true).ToList();
            }
            catch { throw; }
        }
        public List<StructureCable> getStructureCables(int structureId)
        {
            try
            {
                return repo.ExecuteProcedure<StructureCable>("fn_get_structure_cables", new { structId = structureId }).ToList();
            }
            catch { throw; }
        }

        public List<SplitterParentView> getSplitterParents(int structureId)
        {
            try
            {
                return repo.ExecuteProcedure<SplitterParentView>("fn_get_splitter_parent", new { structId = structureId }).ToList();
            }
            catch { throw; }
        }
        public bool checkEntityAssociation(int systemId, string entityType)
        {
            int asociationCount = repo.ExecuteProcedure<int>("fn_get_entity_association", new { p_shaft_id = 0, p_system_id = systemId, p_entity_type = entityType, p_structure_id = 0 }).FirstOrDefault();
            return !(asociationCount > 0);
        }
        public bool ChkEntityTemplateExist(string entity_type, int user_id)
        {
            try
            {
                var lstItem = repo.ExecuteProcedure<ElementTemplateView>("fn_getelementtemplate", new { elementtype = entity_type.ToString(), userid = user_id, }).Where(m => m.systemid > 0).ToList();
                return lstItem != null && lstItem.Count > 0 ? true : false;
            }
            catch { throw; }
        }
        public List<ISPParentEntities> getParentEntities(int structureId)
        {
            try
            {
                return repo.ExecuteProcedure<ISPParentEntities>("fn_isp_get_parent_entities", new { p_structure_id = structureId }, true).ToList();
            }
            catch { throw; }
        }
        public NewEntity getNewEntity(int systemId, string entityType, string networkId)
        {
            try
            {
                return repo.ExecuteProcedure<NewEntity>("fn_isp_get_new_entity", new { p_system_id = systemId, p_network_id = networkId, p_entity_type = entityType }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage SaveEntityPosition(ShiftEntity objShiftEntity)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_isp_shift_entity", new { p_structure_id = objShiftEntity.structureId, p_shaft_id = objShiftEntity.shaftId, p_floor_id = objShiftEntity.floorId, objShiftEntity.entityId, objShiftEntity.entityType, p_parent_system_id = objShiftEntity.parent_system_id, p_parent_entity_type = objShiftEntity.parent_entity_type, p_parent_network_id = objShiftEntity.parent_network_id }).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<ConnectionInfo> getCPEConnections(int structureId)
        {
            try
            {
                return repo.ExecuteProcedure<ConnectionInfo>("fn_isp_get_cpe_connections", new { structid = structureId }, true).ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<EquipmentExportResult> GetEquipmentExportDetail(string SourceType, string SourceId, string parent_type, string parent_id)
        {
            try
            {
                return repo.ExecuteProcedure<EquipmentExportResult>("fn_get_equipment_export_detail", new { vtype = SourceType, vid = SourceId, p_parent_type = parent_type, p_parent_id = parent_id }, false).ToList();
            }
            catch
            {
                throw;
            }
        }

        public DbMessage UpdateEquipment(int system_id, string equipmentList, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_update_osp_equipment", new { p_system_id = system_id, p_equipments = equipmentList, p_user_id = user_id }, true).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

    }
    public class DAElementTemplate : Repository<ElementTemplateView>
    {
        DAElementTemplate() { }
        private static DAElementTemplate objDAElementTemplate = null;
        private static readonly object lockObject = new object();
        public static DAElementTemplate Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAElementTemplate == null)
                    {
                        objDAElementTemplate = new DAElementTemplate();
                    }
                }
                return objDAElementTemplate;
            }
        }
        public List<ElementTemplateView> getElementTemplate(string ElementType, int userId)
        {
            var roomTemplate = repo.ExecuteProcedure<ElementTemplateView>("fn_getElementTemplate", new { elementType = ElementType, userid = userId }).ToList();
            return roomTemplate;
        }
    }
    public class DAShaftRange : Repository<ispShaftRange>
    {
        DAShaftRange()
        {

        }
        private static DAShaftRange objDAShaftRangeInfo = null;
        private static readonly object lockObject = new object();
        public static DAShaftRange Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAShaftRangeInfo == null)
                    {
                        objDAShaftRangeInfo = new DAShaftRange();
                    }
                }
                return objDAShaftRangeInfo;
            }
        }

        public bool SaveShaftRange(List<ispShaftRange> model, int shaftId, int structureId, int userid)
        {
            try
            {
                var rangeDetails = repo.GetAll(m => m.shaft_id == shaftId && m.structure_id == structureId).ToList();
                if (rangeDetails.Count > 0)
                {
                    repo.DeleteRange(rangeDetails);
                }
                foreach (var item in model)
                {
                    item.created_by = userid;
                    item.created_on = DateTimeHelper.Now;
                    item.shaft_id = shaftId;
                    item.structure_id = structureId;
                    item.shaft_start_range = item.shaft_start_range < item.shaft_end_range ? item.shaft_start_range : item.shaft_end_range;
                    item.shaft_end_range = item.shaft_end_range > item.shaft_start_range ? item.shaft_end_range : item.shaft_start_range;
                }
                repo.Insert(model);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool DeleteShaftRange(int shaftId, int structureId)
        {
            try
            {
                var rangeDetails = repo.GetAll(m => m.shaft_id == shaftId && m.structure_id == structureId).ToList();
                if (rangeDetails.Count > 0)
                {
                    repo.DeleteRange(rangeDetails);
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //return repo.DeleteRange(model) > 0;
            //return result;
        }
        public List<ispShaftRange> getShaftRangeInfo(int shaftId)
        {
            try
            {
                return repo.GetAll(m => m.shaft_id == shaftId).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<ispShaftRange> getShaftRangeByStructure(int Structureid)
        {
            try
            {
                return repo.GetAll(m => m.structure_id == Structureid).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool isShaftAvailable(int Structureid, int shaftId, int floorId)
        {
            try
            {
                return repo.GetAll(m => m.structure_id == Structureid && m.shaft_id == shaftId && (floorId >= m.shaft_start_range && floorId <= m.shaft_end_range)).ToList().Count > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
    public class DAFloorInfo : Repository<FloorInfo>
    {
        DAFloorInfo()
        {

        }
        private static DAFloorInfo objDAFloorInfo = null;
        private static readonly object lockObject = new object();
        public static DAFloorInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAFloorInfo == null)
                    {
                        objDAFloorInfo = new DAFloorInfo();
                    }
                }
                return objDAFloorInfo;
            }
        }


        public FloorInfo UpdateFloorInfo(FloorInfo model)
        {
            try
            {
                FloorInfo result = new FloorInfo();
                var Floor = repo.GetById(m => m.system_id == model.system_id);
                if (Floor != null)
                {
                    Floor.modified_on = DateTimeHelper.Now;
                    Floor.floor_name = model.floor_name;
                    Floor.height = model.height;
                    Floor.width = model.width;
                    Floor.length = model.length;
                    Floor.no_of_units = model.no_of_units;
                    result = repo.Update(Floor);
                }
                return result;
            }
            catch { throw; }
        }
        public FloorInfo getFloorInfo(int? floorId)
        {
            return repo.GetById(m => m.system_id == floorId);
        }

        public bool updateFloorName(int floorId, int structureid, string floorName)
        {
            try
            {
                var floorInfo = repo.GetById(m => m.system_id == floorId && m.structure_id == structureid);
                if (floorInfo != null)
                {
                    floorInfo.floor_name = floorName;
                    repo.Update(floorInfo);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        public List<FloorInfo> getFloorList(int stuctureId)
        {
            try
            {
                return repo.GetAll(m => m.structure_id == stuctureId).OrderBy(m => m.system_id).ToList();
            }
            catch
            {
                throw;
            }
        }
    }

    public class DARoomInfo : Repository<RoomInfo>
    {
        DARoomInfo() { }
        private static DARoomInfo objDARoomInfo = null;
        private static readonly object lockObject = new object();
        public static DARoomInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDARoomInfo == null)
                    {
                        objDARoomInfo = new DARoomInfo();
                    }
                }
                return objDARoomInfo;
            }
        }

        public RoomInfo getRoomDetails(int roomId)
        {
            return repo.GetById(m => m.system_id == roomId);

        }
        public RoomInfo SaveRoomDetails(RoomInfo model)
        {
            if (model.objIspEntityMap.floor_id > 0)
            {
                PageMessage objPageValidate = new PageMessage();
                DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                {
                    system_id = model.system_id,
                    entity_type = EntityType.UNIT.ToString(),
                    floor_id = model.objIspEntityMap.floor_id ?? 0,
                    shaft_id = 0,
                    parent_system_id = model.parent_system_id,
                    parent_entity_type = model.parent_entity_type,
                    room_width = model.room_width,
                    room_height = model.room_height,
                    room_length = model.room_length
                }, model.system_id == 0);

                if (!string.IsNullOrEmpty(objMessage.message))
                {
                    objPageValidate.status = ResponseStatus.FAILED.ToString();
                    objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                    model.objPM = objPageValidate;
                    return model;
                }
            }

            if (model.system_id > 0)
            {

                var result = repo.GetById(model.system_id);
                if (result != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(model.modified_on, result.modified_on, model.modified_by, result.modified_by);
                    if (objPageValidate.message != null)
                    {
                        model.objPM = objPageValidate;
                        return model;
                    }
                    result.room_height = model.room_height;
                    result.room_length = model.room_length;
                    result.room_width = model.room_width;
                    result.room_type = model.room_type;
                    result.room_name = model.room_name;
                    result.modified_by = model.userId;
                    result.modified_on = DateTimeHelper.Now;
                    result.unit_type = model.unit_type;
                    result.unitno = model.unitno;
                    result.area = model.area;
                    result.door_position = model.door_position;
                    result.door_type = model.door_type;
                    result.door_width = model.door_width;
                    result.remarks = model.remarks;
                    result.requested_by = model.requested_by;
                    result.request_approved_by = model.request_approved_by;
                    result.request_ref_id = model.request_ref_id;
                    result.origin_ref_id = model.origin_ref_id;
                    result.origin_ref_description = model.origin_ref_description;
                    result.origin_from = model.origin_from;
                    result.origin_ref_code = model.origin_ref_code;
                }

                return repo.Update(result);
            }
            else
            {


                model.created_by = model.userId;
                model.created_on = DateTimeHelper.Now;
                model.network_status = "P";

                var response = repo.Insert(model);
                if (model.parent_entity_type.ToLower() == "structure")
                    if (model.objIspEntityMap.floor_id != 0 || model.objIspEntityMap.shaft_id != 0)
                    {
                        IspEntityMapping objMapping = new IspEntityMapping();
                        objMapping.id = model.objIspEntityMap.id;
                        objMapping.structure_id = model.objIspEntityMap.structure_id;
                        objMapping.floor_id = model.objIspEntityMap.floor_id ?? 0;
                        objMapping.shaft_id = model.objIspEntityMap.shaft_id ?? 0;
                        objMapping.entity_id = model.system_id;
                        objMapping.entity_type = EntityType.UNIT.ToString();
                        var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    }
                return response;
            }
        }
        public bool DeleteRoomById(int systemId)
        {
            var room = repo.GetById(m => m.system_id == systemId);
            return repo.Delete(room.system_id) > 0;
        }
        public List<RoomDimension> getRoomByFloorId(int structureId, int? floorId)
        {
            try
            {
                return repo.ExecuteProcedure<RoomDimension>("fn_get_room_by_floor_id", new { structureid = structureId, floorid = floorId }).ToList();
            }
            catch { throw; }
            //return repo.GetAll(m => m.floor_id == floorId).ToList();
        }
        public List<StructureElement> getAllParentInFloor(int structureId, int floorId, string parentType)
        {
            try
            {
                return repo.ExecuteProcedure<StructureElement>("fn_isp_get_parent_by_floor", new { p_structure_id = structureId, p_floor_id = floorId, p_parent_entity_type = parentType }, true).ToList();
            }
            catch { throw; }
        }
        public List<DropDownMaster> getPODList(int structureId)
        {
            try
            {
                List<DropDownMaster> objDDL = new List<DropDownMaster>();
                var unitList = repo.ExecuteProcedure<StructureElement>("fn_isp_get_pod_by_structure", new { p_structure_id = structureId }, true).ToList();
                if (unitList.Count > 0)
                { objDDL = unitList.Select(m => new DropDownMaster { dropdown_key = m.network_id, dropdown_value = m.entity_system_id.ToString(), dropdown_type = m.entity_type }).ToList(); }
                return objDDL;
            }
            catch { throw; }
        }
    }
    public class ISPLayerDetails : Repository<layerDetail>
    {
        ISPLayerDetails() { }
        private static ISPLayerDetails objISPLayerDetails = null;
        private static readonly object lockObject = new object();
        public static ISPLayerDetails Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objISPLayerDetails == null)
                    {
                        objISPLayerDetails = new ISPLayerDetails();
                    }
                }
                return objISPLayerDetails;
            }
        }
        public layerDetail getLayerDetails(string ElementType)
        {
            try
            {
                return repo.GetById(m => m.is_isp_layer == true && m.layer_name.Trim() == ElementType.Trim());
            }
            catch
            {
                throw;
            }
        }
    }
    public class DaRoomTemplate : Repository<RoomTemplate>
    {
        DaRoomTemplate() { }
        private static DaRoomTemplate objDaRoomTemplate = null;
        private static readonly object lockObject = new object();
        public static DaRoomTemplate Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDaRoomTemplate == null)
                    {
                        objDaRoomTemplate = new DaRoomTemplate();
                    }
                }
                return objDaRoomTemplate;
            }
        }
        public RoomTemplate getRoomTemplate(int userId)
        {
            var roomTemplate = repo.GetById(m => m.created_by == userId);
            return roomTemplate != null ? roomTemplate : new RoomTemplate();
        }
        public RoomTemplate UpdateUnitTemplate(RoomTemplate UnitTemplateMaster)
        {
            var unitTemplate = repo.GetById(m => m.created_by == UnitTemplateMaster.created_by);
            if (unitTemplate != null)
            {
                unitTemplate.template_name = UnitTemplateMaster.template_name;
                unitTemplate.unit_type = UnitTemplateMaster.unit_type;
                unitTemplate.room_height = UnitTemplateMaster.room_height;
                unitTemplate.room_length = UnitTemplateMaster.room_length;
                unitTemplate.room_width = UnitTemplateMaster.room_width;
                unitTemplate.area = UnitTemplateMaster.area;
                repo.Update(unitTemplate);
            }
            else { return repo.Insert(UnitTemplateMaster); }
            return unitTemplate;
        }
    }
    public class DAHTBInfo : Repository<HTBInfo>
    {
        DAHTBInfo() { }
        private static DAHTBInfo objDAHTBInfo = null;
        private static readonly object lockObject = new object();
        public static DAHTBInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAHTBInfo == null)
                    {
                        objDAHTBInfo = new DAHTBInfo();
                    }
                }
                return objDAHTBInfo;
            }
        }

        public HTBInfo getHTBDetails(int systemId)
        {
            return repo.GetById(m => m.system_id == systemId);

        }
        public HTBInfo SaveHTBDetails(HTBInfo model, int userid)
        {

            string oldPorts = string.Empty;
            int inputPort = 0;
            int outputPort = 0;
            if (model.system_id > 0)
            {
                var result = repo.GetById(model.system_id);
                if (result != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(model.modified_on, result.modified_on, model.modified_by, result.modified_by);
                    if (objPageValidate.message != null)
                    {
                        model.objPM = objPageValidate;
                        return model;
                    }
                    var geomresp = new DAMisc().GetValidatePointGeometry(model.system_id, model.entityType, model.latitude.ToString(), model.longitude.ToString(), model.region_id,model.province_id);
                    if (geomresp.status != "OK")
                    {
                        model.objPM = geomresp;
                        return model;
                    }
                    result.htb_name = model.htb_name;
                    result.modified_by = userid;
                    result.modified_on = DateTimeHelper.Now;
                    result.project_id = model.project_id ?? 0;
                    result.planning_id = model.planning_id ?? 0;
                    result.workorder_id = model.workorder_id ?? 0;
                    result.purpose_id = model.purpose_id ?? 0;
                    result.floor_id = model.floor_id;
                    result.shaft_id = model.shaft_id;
                    result.network_id = model.network_id;
                    result.specification = model.specification;
                    result.category = model.category;
                    result.subcategory1 = model.subcategory1;
                    result.subcategory2 = model.subcategory2;
                    result.subcategory3 = model.subcategory3;
                    result.item_code = model.item_code;
                    result.vendor_id = model.vendor_id;
                    result.type = model.type;
                    result.brand = model.brand;
                    result.model = model.model;
                    result.construction = model.construction;
                    result.activation = model.activation;
                    result.accessibility = model.accessibility;
                    result.parent_system_id = model.parent_system_id;
                    result.parent_network_id = model.parent_network_id;
                    result.parent_entity_type = model.parent_entity_type;
                    result.remarks = model.remarks;
                    result.is_acquire_from = model.is_acquire_from;
                    if (model.no_of_input_port != 0 && model.no_of_output_port != 0 && (result.no_of_input_port != model.no_of_input_port || result.no_of_output_port != model.no_of_output_port))
                    {
                        var portinfo = new DAMisc().isPortConnected(result.system_id, EntityType.HTB.ToString(), result.specification, result.vendor_id, result.item_code);
                        if (portinfo.status)
                        {
                            result.isPortConnected = portinfo.status;
                            result.message = Resources.Helper.MultilingualMessageConvert(portinfo.message); //portinfo.message;
                            return result;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(result.system_id, result.network_id, EntityType.HTB.ToString(), model.no_of_input_port + ":" + model.no_of_output_port);
                        new DAMisc().InsertPortInfo(model.no_of_input_port, model.no_of_output_port, EntityType.HTB.ToString(), result.system_id, result.network_id, userid);
                    }
                    else if (result.no_of_port != model.no_of_port)
                    {
                        var portinfo = new DAMisc().isPortConnected(result.system_id, EntityType.HTB.ToString(), result.specification, result.vendor_id, result.item_code);
                        if (portinfo.status)
                        {
                            result.isPortConnected = portinfo.status;
                            result.message = Resources.Helper.MultilingualMessageConvert(portinfo.message); //portinfo.message;
                            return result;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(result.system_id, result.network_id, EntityType.HTB.ToString(), model.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(model.no_of_port, model.no_of_port, EntityType.HTB.ToString(), result.system_id, result.network_id, userid);
                    }
                    result.longitude = model.longitude;
                    result.latitude = model.latitude;
                    var resp = DAIspEntityMapping.Instance.associateEntityInStructure(model.objIspEntityMap.shaft_id, model.objIspEntityMap.floor_id, model.system_id, EntityType.HTB.ToString(), model.parent_system_id, model.parent_entity_type, model.longitude + " " + model.latitude);
                    if (resp.status)
                    {
                        result.isPortConnected = resp.status;
                        result.message = Resources.Helper.MultilingualMessageConvert(resp.message); //resp.message;
                        return result;
                    }
                    result.no_of_port = model.no_of_port;
                    result.no_of_input_port = model.no_of_input_port;
                    result.no_of_output_port = model.no_of_output_port;
                    result.acquire_from = model.acquire_from;
                    result.ownership_type = model.ownership_type;
                    result.third_party_vendor_id = model.third_party_vendor_id;
                    result.audit_item_master_id = model.audit_item_master_id;
                    result.primary_pod_system_id = model.primary_pod_system_id;
                    result.secondary_pod_system_id = model.secondary_pod_system_id;
                    result.status_remark = model.status_remark;
                    result.is_middleware = model.is_middleware;
                    result.other_info = model.other_info;   //for additional-attributes
                    if (!(string.IsNullOrEmpty(model.unitValue)) && model.unitValue.Contains(":"))
                    {
                        oldPorts = result.no_of_input_port + ":" + result.no_of_output_port;
                        inputPort = Convert.ToInt32(model.unitValue.Split(':')[0]);
                        outputPort = Convert.ToInt32(model.unitValue.Split(':')[1]);
                        result.no_of_input_port = inputPort;
                        result.no_of_output_port = outputPort;
                        model.no_of_input_port = inputPort;
                        model.no_of_output_port = outputPort;

                    }
                }
                result.requested_by = model.requested_by;
                result.request_approved_by = model.request_approved_by;
                result.request_ref_id = model.request_ref_id;
                result.origin_ref_id = model.origin_ref_id;
                result.origin_ref_description = model.origin_ref_description;
                result.origin_from = model.origin_from;
                result.origin_ref_code = model.origin_ref_code;
                result.bom_sub_category=model.bom_sub_category;
                //  result.served_by_ring=model.served_by_ring;
                var response = repo.Update(result);
                DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.HTB.ToString(), response.province_id, 1);
                //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.HTB.ToString(), response.province_id);
                return response;
            }
            else
            {
                // if (model.objIspEntityMap.floor_id > 0 && model.objIspEntityMap.shaft_id > 0)
                //{
                PageMessage objPageValidate = new PageMessage();
                DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                {
                    system_id = model.system_id,
                    entity_type = EntityType.HTB.ToString(),
                    floor_id = model.objIspEntityMap.floor_id ?? 0,
                    shaft_id = model.objIspEntityMap.shaft_id ?? 0,
                    parent_system_id = model.parent_system_id,
                    parent_entity_type = model.parent_entity_type
                }, true);

                if (!string.IsNullOrEmpty(objMessage.message))
                {
                    objPageValidate.status = ResponseStatus.FAILED.ToString();
                    objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                    model.objPM = objPageValidate;
                    return model;
                }
                // }

                model.created_by = userid;
                model.created_on = DateTimeHelper.Now;
                //model.status = "A";
                //model.network_status = "P";
                model.status = String.IsNullOrEmpty(model.status) ? "A" : model.status;
                model.network_status = String.IsNullOrEmpty(model.network_status) ? "P" : model.network_status;
                model.utilization = "L";
                var response = repo.Insert(model);
                // transaction.Commit();

                //new DAMisc().InsertPortInfo(model.no_of_input_port, model.no_of_output_port, EntityType.HTB.ToString(), model.system_id, model.network_id, userid);                
                //if (model.parent_entity_type.ToLower() == "structure")
                //    if (model.objIspEntityMap.floor_id != 0 || model.objIspEntityMap.shaft_id != 0)
                //    {
                //        IspEntityMapping objMapping = new IspEntityMapping();
                //        objMapping.id = model.objIspEntityMap.id;
                //        objMapping.structure_id = model.objIspEntityMap.structure_id != 0 ? model.objIspEntityMap.structure_id : model.parent_system_id;
                //        objMapping.floor_id = model.objIspEntityMap.floor_id ?? 0;
                //        objMapping.shaft_id = model.objIspEntityMap.shaft_id ?? 0;
                //        objMapping.entity_id = model.system_id;
                //        objMapping.entity_type = EntityType.HTB.ToString();
                //        var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                //    }
                DAIspEntityMapping.Instance.associateEntityInStructure(model.objIspEntityMap.shaft_id, model.objIspEntityMap.floor_id, model.system_id, EntityType.HTB.ToString(), model.parent_system_id == 0 ? model.objIspEntityMap.structure_id : model.parent_system_id, string.IsNullOrEmpty(model.parent_entity_type) == true ? EntityType.Structure.ToString() : model.parent_entity_type, model.longitude + " " + model.latitude);
                InputGeom geom = new InputGeom();
                geom.systemId = response.system_id;
                geom.longLat = response.longitude + " " + response.latitude;
                geom.userId = userid;
                geom.ports = response.no_of_port.ToString();
                geom.entityType = EntityType.HTB.ToString();
                geom.commonName = response.network_id;
                geom.geomType = GeometryType.Point.ToString();
                geom.project_id = response.project_id;
                if (model.no_of_input_port != 0 && model.no_of_output_port != 0)
                { geom.ports = model.no_of_input_port + ":" + model.no_of_output_port; }
                else if (model.no_of_port != 0) { geom.ports = model.no_of_port.ToString(); }
                string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.HTB.ToString(), response.province_id, 0);
                //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.HTB.ToString(), response.province_id);
                if (response != null && model.no_of_input_port != 0 && model.no_of_output_port != 0)
                {
                    new DAMisc().InsertPortInfo(model.no_of_input_port, model.no_of_output_port, EntityType.HTB.ToString(), response.system_id, response.network_id, userid);
                }
                else
                {
                    new DAMisc().InsertPortInfo(model.no_of_port, model.no_of_port, EntityType.HTB.ToString(), response.system_id, response.network_id, userid);
                }
                return response;
            }



            //}
            //catch (Exception)
            //{
            //    transaction.Rollback();
            //}
            // }
            //}


        }
        public bool DeleteHTBById(int systemId)
        {
            var htb = repo.GetById(m => m.system_id == systemId);
            return repo.Delete(htb.system_id) > 0;
        }

        #region Additional-Attributes
        public string GetOtherInfoHTB(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
    }
    public class DAHTBTemplate : Repository<HTBTemplate>
    {
        DAHTBTemplate() { }
        private static DAHTBTemplate objDAHTBTemplate = null;
        private static readonly object lockObject = new object();
        public static DAHTBTemplate Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAHTBTemplate == null)
                    {
                        objDAHTBTemplate = new DAHTBTemplate();
                    }
                }
                return objDAHTBTemplate;
            }
        }
        public HTBTemplate getHTBTemplate(int userId)
        {
            var htbTemplate = repo.GetById(m => m.created_by == userId);
            return htbTemplate;
        }
        public HTBTemplate saveHTBTemplate(HTBTemplate ObjHTBTemplateMaster)
        {
            var htbTemplate = repo.GetById(m => m.created_by == ObjHTBTemplateMaster.created_by);
            if (htbTemplate != null)
            {
                htbTemplate.template_name = ObjHTBTemplateMaster.template_name;
                htbTemplate.vendor_id = ObjHTBTemplateMaster.vendor_id;
                htbTemplate.model = ObjHTBTemplateMaster.model;
                htbTemplate.construction = ObjHTBTemplateMaster.construction;
                htbTemplate.accessibility = ObjHTBTemplateMaster.accessibility;
                htbTemplate.activation = ObjHTBTemplateMaster.activation;
                htbTemplate.brand = ObjHTBTemplateMaster.brand;
                htbTemplate.type = ObjHTBTemplateMaster.type;
                htbTemplate.specification = ObjHTBTemplateMaster.specification;
                htbTemplate.modified_by = ObjHTBTemplateMaster.created_by;
                htbTemplate.modified_on = DateTimeHelper.Now;
                htbTemplate.no_of_port = ObjHTBTemplateMaster.no_of_port;
                htbTemplate.no_of_input_port = ObjHTBTemplateMaster.no_of_input_port;
                htbTemplate.no_of_output_port = ObjHTBTemplateMaster.no_of_output_port;
                htbTemplate.item_code = ObjHTBTemplateMaster.item_code;
                htbTemplate.audit_item_master_id = ObjHTBTemplateMaster.audit_item_master_id;
                repo.Update(htbTemplate);

            }
            else
            {
                return repo.Insert(ObjHTBTemplateMaster);
            }
            return htbTemplate;
        }
    }
    public class DAOpticalRepeaterInfo : Repository<OpticalRepeaterInfo>
    {
        DAOpticalRepeaterInfo() { }
        private static DAOpticalRepeaterInfo objDAOpticalRepeaterInfo = null;
        private static readonly object lockObject = new object();
        public static DAOpticalRepeaterInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAOpticalRepeaterInfo == null)
                    {
                        objDAOpticalRepeaterInfo = new DAOpticalRepeaterInfo();
                    }
                }
                return objDAOpticalRepeaterInfo;
            }
        }

        public OpticalRepeaterInfo getOpticalRepeaterDetails(int systemId)
        {
            return repo.GetById(m => m.system_id == systemId);

        }
        public OpticalRepeaterInfo SaveOpticalRepeaterDetails(OpticalRepeaterInfo model, int userid)
        {

            string oldPorts = string.Empty;
            int inputPort = 0;
            int outputPort = 0;
            if (model.system_id > 0)
            {
                var result = repo.GetById(model.system_id);
                if (result != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(model.modified_on, result.modified_on, model.modified_by, result.modified_by);
                    if (objPageValidate.message != null)
                    {
                        model.objPM = objPageValidate;
                        return model;
                    }
                    result.opticalrepeater_name = model.opticalrepeater_name;
                    result.amplifier_type = model.amplifier_type;
                    result.amplifier_model = model.amplifier_model;
                    result.amplifier_wavelength = model.amplifier_wavelength;
                    result.signal_boost_value = model.signal_boost_value;
                    result.noise_figure = model.noise_figure;
                    result.special_gain_flatness = model.special_gain_flatness;
                    result.modified_by = userid;
                    result.modified_on = DateTimeHelper.Now;
                    result.project_id = model.project_id ?? 0;
                    result.planning_id = model.planning_id ?? 0;
                    result.workorder_id = model.workorder_id ?? 0;
                    result.purpose_id = model.purpose_id ?? 0;
                    result.floor_id = model.floor_id;
                    result.shaft_id = model.shaft_id;
                    result.network_id = model.network_id;
                    result.specification = model.specification;
                    result.category = model.category;
                    result.subcategory1 = model.subcategory1;
                    result.subcategory2 = model.subcategory2;
                    result.subcategory3 = model.subcategory3;
                    result.item_code = model.item_code;
                    result.vendor_id = model.vendor_id;
                    result.type = model.type;
                    result.brand = model.brand;
                    result.model = model.model;
                    result.construction = model.construction;
                    result.activation = model.activation;
                    result.accessibility = model.accessibility;
                    result.parent_system_id = model.parent_system_id;
                    result.parent_network_id = model.parent_network_id;
                    result.parent_entity_type = model.parent_entity_type;
                    if (model.no_of_input_port != 0 && model.no_of_output_port != 0 && (result.no_of_input_port != model.no_of_input_port || result.no_of_output_port != model.no_of_output_port))
                    {
                        var portinfo = new DAMisc().isPortConnected(result.system_id, EntityType.OpticalRepeater.ToString(), result.specification, result.vendor_id, result.item_code);
                        if (portinfo.status)
                        {
                            result.isPortConnected = portinfo.status;
                            result.message = Resources.Helper.MultilingualMessageConvert(portinfo.message); //portinfo.message;
                            return result;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(result.system_id, result.network_id, EntityType.OpticalRepeater.ToString(), model.no_of_input_port + ":" + model.no_of_output_port);
                        new DAMisc().InsertPortInfo(model.no_of_input_port, model.no_of_output_port, EntityType.OpticalRepeater.ToString(), result.system_id, result.network_id, userid);
                    }
                    else if (result.no_of_port != model.no_of_port)
                    {
                        var portinfo = new DAMisc().isPortConnected(result.system_id, EntityType.OpticalRepeater.ToString(), result.specification, result.vendor_id, result.item_code);
                        if (portinfo.status)
                        {
                            result.isPortConnected = portinfo.status;
                            result.message = Resources.Helper.MultilingualMessageConvert(portinfo.message); //portinfo.message;
                            return result;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(result.system_id, result.network_id, EntityType.OpticalRepeater.ToString(), model.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(model.no_of_port, model.no_of_port, EntityType.OpticalRepeater.ToString(), result.system_id, result.network_id, userid);
                    }
                    result.longitude = model.longitude;
                    result.latitude = model.latitude;
                    var resp = DAIspEntityMapping.Instance.associateEntityInStructure(model.objIspEntityMap.shaft_id, model.objIspEntityMap.floor_id, model.system_id, EntityType.OpticalRepeater.ToString(), model.parent_system_id, model.parent_entity_type, model.longitude + " " + model.latitude);
                    if (resp.status)
                    {
                        result.isPortConnected = resp.status;
                        result.message = Resources.Helper.MultilingualMessageConvert(resp.message); //resp.message;
                        return result;
                    }
                    result.no_of_port = model.no_of_port;
                    result.no_of_input_port = model.no_of_input_port;
                    result.no_of_output_port = model.no_of_output_port;
                    result.acquire_from = model.acquire_from;
                    result.ownership_type = model.ownership_type;
                    result.third_party_vendor_id = model.third_party_vendor_id;
                    result.audit_item_master_id = model.audit_item_master_id;
                    result.remarks = model.remarks;
                    result.is_acquire_from = model.is_acquire_from;
                    if (!(string.IsNullOrEmpty(model.unitValue)) && model.unitValue.Contains(":"))
                    {
                        oldPorts = result.no_of_input_port + ":" + result.no_of_output_port;
                        inputPort = Convert.ToInt32(model.unitValue.Split(':')[0]);
                        outputPort = Convert.ToInt32(model.unitValue.Split(':')[1]);
                        result.no_of_input_port = inputPort;
                        result.no_of_output_port = outputPort;
                        model.no_of_input_port = inputPort;
                        model.no_of_output_port = outputPort;

                    }
                    result.other_info = model.other_info;   //for additioanl-attributes
                    result.requested_by = model.requested_by;
                    result.request_approved_by = model.request_approved_by;
                    result.request_ref_id = model.request_ref_id;
                    result.origin_ref_id = model.origin_ref_id;
                    result.origin_ref_description = model.origin_ref_description;
                    result.origin_from = model.origin_from;
                    result.origin_ref_code = model.origin_ref_code;
                    result.bom_sub_category = model.bom_sub_category;
                    // result.served_by_ring = model.served_by_ring;
                }
                var response = repo.Update(result);
                DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.OpticalRepeater.ToString(), response.province_id, 1);
                //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.OpticalRepeater.ToString(), response.province_id);
                return response;
            }
            else
            {
                // if (model.objIspEntityMap.floor_id > 0 && model.objIspEntityMap.shaft_id > 0)
                //{
                PageMessage objPageValidate = new PageMessage();
                DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                {
                    system_id = model.system_id,
                    entity_type = EntityType.OpticalRepeater.ToString(),
                    floor_id = model.objIspEntityMap.floor_id ?? 0,
                    shaft_id = model.objIspEntityMap.shaft_id ?? 0,
                    parent_system_id = model.parent_system_id,
                    parent_entity_type = model.parent_entity_type
                }, true);

                if (!string.IsNullOrEmpty(objMessage.message))
                {
                    objPageValidate.status = ResponseStatus.FAILED.ToString();
                    objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                    model.objPM = objPageValidate;
                    return model;
                }
                // }

                model.created_by = userid;
                model.created_on = DateTimeHelper.Now;
                //model.status = "A";
                //model.network_status = "P";
                model.status = String.IsNullOrEmpty(model.status) ? "A" : model.status;
                model.network_status = String.IsNullOrEmpty(model.network_status) ? "P" : model.network_status;
                model.utilization = "L";
                var response = repo.Insert(model);
                // transaction.Commit();

                //new DAMisc().InsertPortInfo(model.no_of_input_port, model.no_of_output_port, EntityType.HTB.ToString(), model.system_id, model.network_id, userid);                
                //if (model.parent_entity_type.ToLower() == "structure")
                //    if (model.objIspEntityMap.floor_id != 0 || model.objIspEntityMap.shaft_id != 0)
                //    {
                //        IspEntityMapping objMapping = new IspEntityMapping();
                //        objMapping.id = model.objIspEntityMap.id;
                //        objMapping.structure_id = model.objIspEntityMap.structure_id != 0 ? model.objIspEntityMap.structure_id : model.parent_system_id;
                //        objMapping.floor_id = model.objIspEntityMap.floor_id ?? 0;
                //        objMapping.shaft_id = model.objIspEntityMap.shaft_id ?? 0;
                //        objMapping.entity_id = model.system_id;
                //        objMapping.entity_type = EntityType.HTB.ToString();
                //        var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                //    }
                DAIspEntityMapping.Instance.associateEntityInStructure(model.objIspEntityMap.shaft_id, model.objIspEntityMap.floor_id, model.system_id, EntityType.OpticalRepeater.ToString(), model.parent_system_id == 0 ? model.objIspEntityMap.structure_id : model.parent_system_id, string.IsNullOrEmpty(model.parent_entity_type) == true ? EntityType.Structure.ToString() : model.parent_entity_type, model.longitude + " " + model.latitude);
                InputGeom geom = new InputGeom();
                geom.systemId = response.system_id;
                geom.longLat = response.longitude + " " + response.latitude;
                geom.userId = userid;
                geom.ports = response.no_of_port.ToString();
                geom.entityType = EntityType.OpticalRepeater.ToString();
                geom.commonName = response.network_id;
                geom.geomType = GeometryType.Point.ToString();
                geom.project_id = response.project_id;
                if (model.no_of_input_port != 0 && model.no_of_output_port != 0)
                { geom.ports = model.no_of_input_port + ":" + model.no_of_output_port; }
                else if (model.no_of_port != 0) { geom.ports = model.no_of_port.ToString(); }
                string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.OpticalRepeater.ToString(), response.province_id, 0);
                //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.OpticalRepeater.ToString(), response.province_id);
                if (response != null && model.no_of_input_port != 0 && model.no_of_output_port != 0)
                {
                    new DAMisc().InsertPortInfo(model.no_of_input_port, model.no_of_output_port, EntityType.OpticalRepeater.ToString(), response.system_id, response.network_id, userid);
                }
                else
                {
                    new DAMisc().InsertPortInfo(model.no_of_port, model.no_of_port, EntityType.OpticalRepeater.ToString(), response.system_id, response.network_id, userid);
                }
                return response;
            }



            //}
            //catch (Exception)
            //{
            //    transaction.Rollback();
            //}
            // }
            //}


        }
        public bool DeleteOpticalRepeaterById(int systemId)
        {
            var OpticalRepeater = repo.GetById(m => m.system_id == systemId);
            return repo.Delete(OpticalRepeater.system_id) > 0;
        }
        #region Additional-Attributes
        public string GetOtherInfoOpticalRepeater(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
    }
    public class DAOpticalRepeaterTemplate : Repository<OpticalRepeaterTemplate>
    {
        DAOpticalRepeaterTemplate() { }
        private static DAOpticalRepeaterTemplate objDAOpticalRepeaterTemplate = null;
        private static readonly object lockObject = new object();
        public static DAOpticalRepeaterTemplate Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAOpticalRepeaterTemplate == null)
                    {
                        objDAOpticalRepeaterTemplate = new DAOpticalRepeaterTemplate();
                    }
                }
                return objDAOpticalRepeaterTemplate;
            }
        }
        public OpticalRepeaterTemplate getOpticalRepeaterTemplate(int userId)
        {
            var OpticalRepeaterTemplate = repo.GetById(m => m.created_by == userId);
            return OpticalRepeaterTemplate;
        }
        public OpticalRepeaterTemplate saveOpticalRepeaterTemplate(OpticalRepeaterTemplate ObjOpticalRepeaterTemplateMaster)
        {
            var OpticalRepeaterTemplate = repo.GetById(m => m.created_by == ObjOpticalRepeaterTemplateMaster.created_by);
            if (OpticalRepeaterTemplate != null)
            {
                OpticalRepeaterTemplate.template_name = ObjOpticalRepeaterTemplateMaster.template_name;
                OpticalRepeaterTemplate.vendor_id = ObjOpticalRepeaterTemplateMaster.vendor_id;
                OpticalRepeaterTemplate.model = ObjOpticalRepeaterTemplateMaster.model;
                OpticalRepeaterTemplate.construction = ObjOpticalRepeaterTemplateMaster.construction;
                OpticalRepeaterTemplate.accessibility = ObjOpticalRepeaterTemplateMaster.accessibility;
                OpticalRepeaterTemplate.activation = ObjOpticalRepeaterTemplateMaster.activation;
                OpticalRepeaterTemplate.brand = ObjOpticalRepeaterTemplateMaster.brand;
                OpticalRepeaterTemplate.type = ObjOpticalRepeaterTemplateMaster.type;
                OpticalRepeaterTemplate.specification = ObjOpticalRepeaterTemplateMaster.specification;
                OpticalRepeaterTemplate.modified_by = ObjOpticalRepeaterTemplateMaster.created_by;
                OpticalRepeaterTemplate.modified_on = DateTimeHelper.Now;
                OpticalRepeaterTemplate.no_of_port = ObjOpticalRepeaterTemplateMaster.no_of_port;
                OpticalRepeaterTemplate.no_of_input_port = ObjOpticalRepeaterTemplateMaster.no_of_input_port;
                OpticalRepeaterTemplate.no_of_output_port = ObjOpticalRepeaterTemplateMaster.no_of_output_port;
                OpticalRepeaterTemplate.item_code = ObjOpticalRepeaterTemplateMaster.item_code;
                OpticalRepeaterTemplate.audit_item_master_id = ObjOpticalRepeaterTemplateMaster.audit_item_master_id;
                repo.Update(OpticalRepeaterTemplate);

            }
            else
            {
                return repo.Insert(ObjOpticalRepeaterTemplateMaster);
            }
            return OpticalRepeaterTemplate;
        }
    }
    public class DAFDBInfo : Repository<FDBInfo>
    {
        DAFDBInfo() { }
        private static DAFDBInfo objDAFDBInfo = null;
        private static readonly object lockObject = new object();
        public static DAFDBInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAFDBInfo == null)
                    {
                        objDAFDBInfo = new DAFDBInfo();
                    }
                }
                return objDAFDBInfo;
            }
        }

        public FDBInfo getFDBDetails(int systemId)
        {
            return repo.GetById(m => m.system_id == systemId);

        }

        public List<FDBInfo> getFDBDetailsbySourceRefId(string sourceRefId)
        {
            return repo.GetAll(m => m.source_ref_id == sourceRefId).ToList();

        }
		public DbMessage getEntityVerificationStatusbySourceRefId(string sourceRefId)
		{
			try
			{
				return repo.ExecuteProcedure<DbMessage>("fn_entity_verification_status", new { p_source_ref_id = sourceRefId }).FirstOrDefault();
			}
			catch { throw; }

		}
		
		public Boolean IsAllSplitterTraceValid(string sourceRefId)
		{
			try
			{
				return repo.ExecuteProcedure<Boolean>("fn_is_all_splitter_trace_valid",
				  new
				  {
					  p_source_ref_id = sourceRefId
				  }).FirstOrDefault();
			}
			catch
			{
				throw;
			}
		}


		public FDBInfo SaveFDBDetails(FDBInfo model)
        {
            if (model.system_id > 0)
            {
                var result = repo.GetById(model.system_id);
                if (result != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(model.modified_on, result.modified_on, model.modified_by, result.modified_by);
                    if (objPageValidate.message != null)
                    {
                        model.objPM = objPageValidate;
                        return model;
                    }
                    var geomresp = new DAMisc().GetValidatePointGeometry(model.system_id, model.entityType, model.latitude.ToString(), model.longitude.ToString(), model.region_id, model.province_id);
                    if (geomresp.status != "OK")
                    {
                        model.objPM = geomresp;
                        return model;
                    }
                    result.fdb_name = model.fdb_name;
                    result.modified_by = model.userId;
                    result.modified_on = DateTimeHelper.Now;
                    result.project_id = model.project_id ?? 0;
                    result.planning_id = model.planning_id ?? 0;
                    result.workorder_id = model.workorder_id ?? 0;
                    result.purpose_id = model.purpose_id ?? 0;
                    result.specification = model.specification;
                    result.vendor_id = model.vendor_id;
                    result.item_code = model.item_code;

                    if (model.no_of_input_port != 0 && model.no_of_output_port != 0 && (result.no_of_input_port != model.no_of_input_port || result.no_of_output_port != model.no_of_output_port))
                    {
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(result.system_id, result.network_id, EntityType.FDB.ToString(), model.no_of_input_port + ":" + model.no_of_output_port);
                        new DAMisc().InsertPortInfo(model.no_of_input_port, model.no_of_output_port, EntityType.FDB.ToString(), result.system_id, result.network_id, model.userId);
                    }
                    else if (result.no_of_port != model.no_of_port)
                    {
                        var response = new DAMisc().isPortConnected(result.system_id, EntityType.FDB.ToString(), result.specification, result.vendor_id, result.item_code);
                        if (response.status)
                        {
                            result.isPortConnected = response.status;
                            result.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                            return result;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(result.system_id, result.network_id, EntityType.FDB.ToString(), model.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(model.no_of_port, model.no_of_port, EntityType.FDB.ToString(), result.system_id, result.network_id, model.userId);
                    }
                    result.no_of_port = model.no_of_port;
                    result.no_of_input_port = model.no_of_input_port;
                    result.no_of_output_port = model.no_of_output_port;
                   
                    result.category = model.category;
                    result.subcategory1 = model.subcategory1;
                    result.subcategory2 = model.subcategory2;
                    result.subcategory3 = model.subcategory3;                  
                    result.ownership_type = model.ownership_type;
                    result.acquire_from = model.acquire_from;
                    result.third_party_vendor_id = model.third_party_vendor_id;
                    result.parent_system_id = model.parent_system_id;
                    result.parent_entity_type = model.parent_entity_type;
                    result.parent_network_id = model.parent_network_id;
                    result.audit_item_master_id = model.audit_item_master_id;
                    result.primary_pod_system_id = model.primary_pod_system_id;
                    result.secondary_pod_system_id = model.secondary_pod_system_id;
                    result.status_remark = model.status_remark;
                    result.remarks = model.remarks;
                    result.is_acquire_from = model.is_acquire_from;
                    result.other_info = model.other_info;   //for additional-attributes
                    result.requested_by = model.requested_by;
                    result.request_approved_by = model.request_approved_by;
                    result.request_ref_id = model.request_ref_id;
                    result.origin_ref_id = model.origin_ref_id;
                    result.origin_ref_description = model.origin_ref_description;
                    result.origin_from = model.origin_from;
                    result.origin_ref_code = model.origin_ref_code;
                    result.bom_sub_category = model.bom_sub_category;
                    result.barcode = model.barcode;
                    //  result.served_by_ring = model.served_by_ring;
                }
                var responseAsso = DAIspEntityMapping.Instance.associateEntityInStructure(model.objIspEntityMap.shaft_id, model.objIspEntityMap.floor_id, model.system_id, EntityType.FDB.ToString(), model.parent_system_id, model.parent_entity_type, model.longitude + " " + model.latitude);
                if (responseAsso.status)
                {
                    result.isPortConnected = responseAsso.status;
                    result.message = responseAsso.message;
                    return result;
                }
                var FDBResp = repo.Update(result);
                DbMessage entityObj =new DAMisc(). updateGeojsonEntityAttribute(FDBResp.system_id, Models.EntityType.FDB.ToString(), FDBResp.province_id, 1);
                //DbMessage geojsonObj = updateGeojsonMetadata(Models.EntityType.FDB.ToString(), FDBResp.province_id);
                return FDBResp;
            }
            else
            {
                if (model.objIspEntityMap.floor_id > 0 && model.objIspEntityMap.shaft_id > 0)
                {
                    PageMessage objPageValidate = new PageMessage();
                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = model.system_id,
                        entity_type = EntityType.FDB.ToString(),
                        floor_id = model.objIspEntityMap.floor_id ?? 0,
                        shaft_id = model.objIspEntityMap.shaft_id ?? 0,
                        parent_entity_type = model.parent_entity_type
                    }, true);

                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message);//objMessage.message;
                        model.objPM = objPageValidate;
                        return model;
                    }
                }
                model.created_by = model.userId;
                model.created_on = DateTimeHelper.Now;
                //model.status = "A";
                //model.network_status = "P";
                model.status = String.IsNullOrEmpty(model.status) ? "A" : model.status;
                model.network_status = String.IsNullOrEmpty(model.network_status) ? "P" : model.network_status;
                model.utilization = "L";
                var response = repo.Insert(model);
                if (response != null && model.no_of_input_port != 0 && model.no_of_output_port != 0)
                {
                    var inputPort = model.no_of_input_port;
                    var outputPort = model.no_of_output_port;
                    new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.FDB.ToString(), response.system_id, response.network_id, model.userId);
                }
                else
                {
                    var inputPort = model.no_of_port;
                    var outputPort = model.no_of_port;
                    new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.FDB.ToString(), response.system_id, response.network_id, model.userId);
                }
                // Save geometry
                InputGeom geom = new InputGeom();
                geom.systemId = response.system_id;
                geom.longLat = response.longitude + " " + response.latitude;
                geom.userId = model.userId;
                geom.entityType = EntityType.FDB.ToString();
                geom.commonName = response.network_id;
                geom.geomType = GeometryType.Point.ToString();
                geom.project_id = response.project_id;
                if (response.no_of_input_port != 0 && response.no_of_output_port != 0)
                { geom.ports = response.no_of_input_port + ":" + response.no_of_output_port; }
                else
                {
                    geom.ports = Convert.ToString(response.no_of_port);
                }
                string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.FDB.ToString(), response.province_id, 0);
                //DbMessage geojsonObj = updateGeojsonMetadata(Models.EntityType.FDB.ToString(), response.province_id);
                //if (model.parent_entity_type.ToLower() == "structure")
                //    if (model.objIspEntityMap.floor_id != 0 || model.objIspEntityMap.shaft_id != 0)
                //    {
                //        IspEntityMapping objMapping = new IspEntityMapping();
                //        objMapping.id = model.objIspEntityMap.id;
                //        objMapping.structure_id = model.objIspEntityMap.structure_id != 0 ? model.objIspEntityMap.structure_id : model.parent_system_id;
                //        objMapping.floor_id = model.objIspEntityMap.floor_id ?? 0;
                //        objMapping.shaft_id = model.objIspEntityMap.shaft_id ?? 0;
                //        objMapping.entity_id = model.system_id;
                //        objMapping.entity_type = EntityType.FDB.ToString();
                //        var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                //    }
                // DAIspEntityMapping.Instance.associateEntityInStructure(model.objIspEntityMap.shaft_id, model.objIspEntityMap.floor_id, model.system_id, EntityType.FDB.ToString(), model.parent_system_id, model.parent_entity_type, model.longitude + " " + model.latitude);
                if (model.pEntityType != null && model.pSystemId != 0 && model.pEntityType.ToUpper() != "STRUCTURE")
                {
                    AssociateEntity objAsso = new AssociateEntity();
                    objAsso.associated_entity_type = EntityType.FDB.ToString();
                    objAsso.associated_system_id = response.system_id;
                    objAsso.associated_network_id = response.network_id;
                    objAsso.entity_network_id = model.pNetworkId;
                    objAsso.entity_system_id = model.pSystemId;
                    objAsso.entity_type = model.pEntityType;
                    objAsso.created_on = DateTimeHelper.Now;
                    objAsso.created_by = model.userId;
                    new DAAssociateEntity().SaveAssociation(objAsso);
                }
                else
                {
                    DAIspEntityMapping.Instance.associateEntityInStructure(model.objIspEntityMap.shaft_id, model.objIspEntityMap.floor_id, model.system_id, EntityType.FDB.ToString(), model.parent_system_id, model.parent_entity_type, model.longitude + " " + model.latitude);
                }
                return response;
            }
        }
        public bool DeleteFDBById(int systemId)
        {
            var htb = repo.GetById(m => m.system_id == systemId);
            return repo.Delete(htb.system_id) > 0;
        }
        public bool VerifiedBarcode(int systemId, string barcode)
        {
            
            var objFdb = repo.GetById(m => m.system_id == systemId);
            if (objFdb != null)
            {
                objFdb.barcode = barcode;
                objFdb.is_barcode_verified = true;
                repo.Update(objFdb);
                return true;
            }
            else
            {
                return false;
            }
        }
		
		public DbMessage GetCreateTicketPermissionByGeom(string ticketType, string entityType, int? systemID)
		{
			try
			{
				return repo.ExecuteProcedure<DbMessage>("fn_get_create_ticket_Permission_by_geom",
                new
				  {
					  p_ticket_type = ticketType,
					  p_entity_type= entityType,
					  p_system_id = systemID

				}).FirstOrDefault();
			}
			catch
			{
				throw;
			}
		}



		public bool UpdateManualBarcode(int systemId)
        {

            var objFdb = repo.GetById(m => m.system_id == systemId);
            if (objFdb != null)
            {
                objFdb.is_manual_barcode = true;
                objFdb.is_barcode_verified = false;
				repo.Update(objFdb);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void FDBWithRiser(StructureMaster structureInfo, int shaftId)
        {
            var settings = new DAFormInputSettings().getformInputSettings(EntityType.Structure.ToString());
            var withRiser = settings.Count > 0 ? settings.Where(m => m.form_feature_name.ToLower() == formFeatureName.with_riser.ToString() && m.form_feature_type.ToLower() == formFeatureType.feature.ToString() && m.is_active == true).FirstOrDefault() : null;
            if (withRiser != null)
            {
                var objItem = new DAItemTemplate().GetTemplateDetail<FDBTemplateMaster>(structureInfo.userid, EntityType.FDB, "");
                int structureId = structureInfo.system_id;
                int regionId = structureInfo.region_id;
                int provinceId = structureInfo.province_id;

                double Latitude = 0;
                double longitude = 0;
                if (!(string.IsNullOrEmpty(structureInfo.geom)))
                {
                    char[] splitter = new char[2] { ',', ' ' };
                    var latLong = structureInfo.geom.Split(splitter);
                    if (latLong.Length > 0)
                    {
                        Latitude = Convert.ToDouble(latLong[1]);
                        longitude = Convert.ToDouble(latLong[0]);
                    }
                }
                else
                {
                    Latitude = structureInfo.latitude;
                    longitude = structureInfo.longitude;
                }
                ISPViewModel objISPViewModel = new ISPViewModel();
                var shaftFloors = DAISP.Instance.getShaftNFloor(structureId);
                if (shaftId > 0)
                {
                    objISPViewModel.ShaftList = shaftFloors.Where(m => m.isshaft == true && m.systemid == shaftId).ToList();
                }
                else
                {
                    objISPViewModel.ShaftList = shaftFloors.Where(m => m.isshaft == true).ToList();
                }

                objISPViewModel.FloorList = shaftFloors.Where(m => m.isshaft == false).ToList();
                foreach (var shaftItem in objISPViewModel.ShaftList)
                {
                    if (shaftItem.withriser == true && shaftItem.systemid > 0)
                    {
                        foreach (var floorItem in objISPViewModel.FloorList)
                        {
                            if (floorItem.systemid > 0)
                            {
                                FDBInfo objFdbInfo = new FDBInfo();
                                FieldInfo[] fields = objItem.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                            .Concat(objItem.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                            .Concat(objItem.GetType().BaseType.BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)).ToArray();

                                foreach (FieldInfo fi in fields)
                                {
                                    fi.SetValue(objFdbInfo, fi.GetValue(objItem));
                                }
                                bool isShaftAvailable = true;
                                bool checkFDB = DAIspEntityMapping.Instance.IsEntityExist(structureInfo.system_id, floorItem.systemid, shaftItem.systemid, EntityType.FDB.ToString());
                                if (shaftItem.is_partial_shaft)
                                {
                                    isShaftAvailable = DAShaftRange.Instance.isShaftAvailable(structureInfo.system_id, shaftItem.systemid, floorItem.systemid);
                                }
                                if (!checkFDB && isShaftAvailable)
                                {
                                    var objISPNetworkCode = new DAMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = structureId });
                                    objFdbInfo.networkIdType = "A";
                                    objFdbInfo.parent_system_id = structureId;
                                    objFdbInfo.parent_entity_type = EntityType.Structure.ToString();
                                    objFdbInfo.objIspEntityMap.floor_id = floorItem.systemid;
                                    objFdbInfo.objIspEntityMap.structure_id = structureId;
                                    objFdbInfo.objIspEntityMap.shaft_id = shaftItem.systemid;
                                    objFdbInfo.fdb_name = objISPNetworkCode.network_code;
                                    objFdbInfo.network_id = objISPNetworkCode.network_code;
                                    objFdbInfo.sequence_id = objISPNetworkCode.sequence_id;
                                    objFdbInfo.network_status = "P";

                                    objFdbInfo.region_id = regionId;
                                    objFdbInfo.province_id = provinceId;
                                    objFdbInfo.latitude = Latitude;
                                    objFdbInfo.longitude = longitude;
                                    objFdbInfo.created_by = structureInfo.userid;
                                    objFdbInfo.created_on = DateTimeHelper.Now;
                                    var response = repo.Insert(objFdbInfo);


                                    // Save geometry
                                    InputGeom geom = new InputGeom();
                                    geom.systemId = response.system_id;
                                    geom.longLat = longitude.ToString() + " " + Latitude.ToString();
                                    geom.userId = structureInfo.userid;
                                    geom.entityType = EntityType.FDB.ToString();
                                    geom.commonName = response.network_id;
                                    geom.geomType = GeometryType.Point.ToString();
                                    if (response.no_of_input_port != 0 && response.no_of_output_port != 0)
                                    { geom.ports = response.no_of_input_port + ":" + response.no_of_output_port; }
                                    else
                                    {
                                        geom.ports = Convert.ToString(response.no_of_port);
                                    }
                                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);

                                    if (objFdbInfo.no_of_port != 0)
                                    {
                                        objFdbInfo.no_of_input_port = objFdbInfo.no_of_port; objFdbInfo.no_of_output_port = objFdbInfo.no_of_port;
                                    }
                                    //DASaveEntityGeometry.Instance.UpdatePortInGeom(objFdbInfo.system_id, objFdbInfo.network_id, EntityType.FDB.ToString(), objFdbInfo.no_of_input_port + ":" + objFdbInfo.no_of_output_port);
                                    new DAMisc().InsertPortInfo(objFdbInfo.no_of_input_port, objFdbInfo.no_of_output_port, EntityType.FDB.ToString(), objFdbInfo.system_id, objFdbInfo.network_id, objFdbInfo.userId);
                                    DAIspEntityMapping.Instance.associateEntityInStructure(objFdbInfo.objIspEntityMap.shaft_id, objFdbInfo.objIspEntityMap.floor_id, objFdbInfo.system_id, EntityType.FDB.ToString(), objFdbInfo.parent_system_id, objFdbInfo.parent_entity_type, objFdbInfo.longitude + " " + objFdbInfo.latitude);
                                    //if (objFdbInfo.parent_entity_type.ToLower() == "structure")
                                    //if (objFdbInfo.objIspEntityMap.floor_id != 0 || objFdbInfo.objIspEntityMap.shaft_id != 0)
                                    //{
                                    //IspEntityMapping objMapping = new IspEntityMapping();
                                    //objMapping.id = objFdbInfo.objIspEntityMap.id;
                                    //objMapping.structure_id = objFdbInfo.objIspEntityMap.structure_id != 0 ? objFdbInfo.objIspEntityMap.structure_id : objFdbInfo.parent_system_id;
                                    //objMapping.floor_id = objFdbInfo.objIspEntityMap.floor_id ?? 0;
                                    //objMapping.shaft_id = objFdbInfo.objIspEntityMap.shaft_id ?? 0;
                                    //objMapping.entity_id = objFdbInfo.system_id;
                                    //objMapping.entity_type = EntityType.FDB.ToString();
                                    //var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                                    //}
                                }
                            }
                        }
                    }
                }
            }
        }
        
        #region Additional-Attributes
        public string GetOtherInfoFDB(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
       
    }
    public class DAFDBTemplate : Repository<FDBTemplate>
    {
        DAFDBTemplate() { }
        private static DAFDBTemplate objDAFDBTemplate = null;
        private static readonly object lockObject = new object();
        public static DAFDBTemplate Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAFDBTemplate == null)
                    {
                        objDAFDBTemplate = new DAFDBTemplate();
                    }
                }
                return objDAFDBTemplate;
            }
        }
        public FDBTemplate getFDBTemplate(int userId)
        {
            var fdbTemplate = repo.GetById(m => m.created_by == userId);
            return fdbTemplate != null ? fdbTemplate : new FDBTemplate();
        }
        public FDBTemplate saveFDBTemplate(FDBTemplate ObjFDBTemplateMaster, int userId)
        {
            var fdbTemplate = repo.GetById(m => m.created_by == userId);
            if (fdbTemplate != null)
            {
                fdbTemplate.template_name = ObjFDBTemplateMaster.template_name;
                fdbTemplate.vendor_id = ObjFDBTemplateMaster.vendor_id;
                fdbTemplate.model = ObjFDBTemplateMaster.model;
                fdbTemplate.construction = ObjFDBTemplateMaster.construction;
                fdbTemplate.accessibility = ObjFDBTemplateMaster.accessibility;
                fdbTemplate.activation = ObjFDBTemplateMaster.activation;
                fdbTemplate.brand = ObjFDBTemplateMaster.brand;
                fdbTemplate.type = ObjFDBTemplateMaster.type;
                fdbTemplate.specification = ObjFDBTemplateMaster.specification;
                fdbTemplate.modified_by = ObjFDBTemplateMaster.created_by;
                fdbTemplate.modified_on = DateTimeHelper.Now;
                fdbTemplate.no_of_port = ObjFDBTemplateMaster.no_of_port;
                fdbTemplate.no_of_input_port = ObjFDBTemplateMaster.no_of_input_port;
                fdbTemplate.no_of_output_port = ObjFDBTemplateMaster.no_of_output_port;
                fdbTemplate.item_code = ObjFDBTemplateMaster.item_code;
                fdbTemplate.category = ObjFDBTemplateMaster.category;
                fdbTemplate.subcategory1 = ObjFDBTemplateMaster.subcategory1;
                fdbTemplate.subcategory2 = ObjFDBTemplateMaster.subcategory2;
                fdbTemplate.subcategory3 = ObjFDBTemplateMaster.subcategory3;
                repo.Update(fdbTemplate);

            }
            else
            {
                ObjFDBTemplateMaster.created_by = userId;
                ObjFDBTemplateMaster.created_on = DateTime.Now;
                return repo.Insert(ObjFDBTemplateMaster);
            }
            return fdbTemplate;
        }
    }
    public class DAISPEntityMapping : Repository<IspEntityMapping>
    {
        DAISPEntityMapping() { }
        private static DAISPEntityMapping objDAISPEntityMapping = null;
        private static readonly object lockObject = new object();
        public static DAISPEntityMapping Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAISPEntityMapping == null)
                    {
                        objDAISPEntityMapping = new DAISPEntityMapping();
                    }
                }
                return objDAISPEntityMapping;
            }
        }
        public IspEntityMapping getMappingByEntityId(int systemId, string entityType)
        {
            return repo.GetById(m => m.entity_id == systemId && m.entity_type == entityType);
        }
    }
}
