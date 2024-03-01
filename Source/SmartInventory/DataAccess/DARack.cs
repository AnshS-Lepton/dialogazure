using DataAccess;
using DataAccess.DBHelpers;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DARack : Repository<RackInfo>
    {
        private static DARack instance = null;
        private static readonly object lockObject = new object();
        public static DARack Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DARack();
                    }
                }
                return instance;
            }
        }
        public RackInfo SaveEntityRack(RackInfo input, int userId)
        {
            try
            {
                var found = repo.Get(x => x.system_id == input.system_id);
                if (found != null)
                {
                    //PageMessage objPageValidate = DAUtility.ValidateModifiedDate(input.modified_on, found.modified_on, input.modified_by, found.modified_by);
                    //if (objPageValidate.message != null)
                    //{
                    //    input.objPM = objPageValidate;
                    //    return input;
                    //}

                    found.rack_name = input.rack_name;
                    found.rack_type = input.rack_type;
                    found.parent_system_id = input.parent_system_id;
                    found.parent_network_id = input.parent_network_id;
                    found.parent_entity_type = input.parent_entity_type;
                    found.pos_x = input.pos_x;
                    found.pos_y = input.pos_y;
                    found.pos_z = input.pos_z;
                    found.modified_by = userId;
                    found.modified_on = DateTimeHelper.Now;
                    found.project_id = input.project_id;
                    found.workorder_id = input.workorder_id;
                    found.planning_id = input.planning_id;
                    found.purpose_id = input.purpose_id;
                    found.audit_item_master_id = input.audit_item_master_id;
                    found.remarks = input.remarks;
                    found.bom_sub_category = input.bom_sub_category;
                    found.gis_design_id = input.gis_design_id;
                    // found.served_by_ring = input.served_by_ring;
                    var result = repo.Update(found);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Rack.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Rack.ToString(), result.province_id);
                    return result;
                }
                else
                {
                   
                    input.created_by = userId;
                    input.created_on = DateTimeHelper.Now;
                    input.status = "A";
                    input.network_status = "P";

                    var resultItem = repo.Insert(input);
                    resultItem.geom = resultItem.longitude + " " + resultItem.latitude;
                    //insert geometery detail....
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom.Replace(",", " ");
                    geom.userId = userId;
                    geom.entityType = EntityType.Rack.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Rack.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Rack.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }

        public DbMessage DeleteRackById(int systemId)
        {
            try
            {
                DbMessage msg = new Models.DbMessage();
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    if (DAEquipment.Instance.GetEquipmentByRackId(systemId).Count > 0) {
                        msg.status = false;
                        msg.message = "Remove all rack content first!";
                        return msg;
                    }
                        //delete rack and its equipments
                        repo.Delete(objSystmId.system_id);
                  
                    msg.status = true;
                    msg.message = "deleted successfully!";
                    return msg;
                }
                else
                {
                    msg.status = false;
                    msg.message = "Entity not found";
                    return msg;
                }


            }
            catch { throw; }
        }

        public List<RackInfo> GetRackByParentId(int parentId,string parent_type, int userId)
        {
            try
            {

                //return repo.GetAll(x => x.parent_system_id == parentId).ToList();
               
                return repo.ExecuteProcedure<RackInfo>("fn_isp_get_room_racks", new { p_parent_id = parentId, p_parent_type = parent_type, p_user_id =userId }, true).ToList();
            }
            catch { throw; }
        }

        public List<EquipmentInfo> GetEquipmentinRack(int parentId, string parent_type, int userId)
        {
            try
            {

                //return repo.GetAll(x => x.parent_system_id == parentId).ToList();

                return repo.ExecuteProcedure<EquipmentInfo>("fn_isp_get_room_equipment", new { p_parent_id = parentId, p_parent_type = parent_type, p_user_id = userId }, true).ToList();
            }
            catch { throw; }
        }

        public bool SaveRackPosition(RackInfo input)
        {
            bool isSaved = false;
            try
            {
                var found = repo.Get(x => x.system_id == input.system_id);
                if (found != null)
                {
                    found.pos_x = input.pos_x;
                    found.pos_y = input.pos_y;
                    found.pos_z = input.pos_z;
                    repo.Update(found);
                    isSaved = true;
                }
            }
            catch { throw; }
            return isSaved;
        }
    }

    public class DAEquipment : Repository<EquipmentInfo>
    {
        private static DAEquipment instance = null;
        private static readonly object lockObject = new object();
        public static DAEquipment Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DAEquipment();
                    }
                }
                return instance;
            }
        }
        public EquipmentInfo SaveEntityEquipment(EquipmentInfo input, int userId)
        {
            try
            {
                var found = repo.Get(x => x.system_id == input.system_id);
                if (found != null)
                {
                    //PageMessage objPageValidate = DAUtility.ValidateModifiedDate(input.modified_on, found.modified_on, input.modified_by, found.modified_by);
                    //if (objPageValidate.message != null)
                    //{
                    //    input.objPM = objPageValidate;
                    //    return input;
                    //}

                    found.model_name = input.model_name;
                    found.parent_system_id = input.parent_system_id;
                    found.parent_network_id = input.parent_network_id;
                    found.parent_entity_type = input.parent_entity_type;
                    found.pos_x = input.pos_x > 0 ? input.pos_x : found.pos_x;
                    found.pos_y = input.pos_y > 0 ? input.pos_y : found.pos_y;
                    found.pos_z = input.pos_z;
                    found.modified_by = userId;
                    found.modified_on = DateTimeHelper.Now;
                    found.equipment_name = input.model_name;
                    found.specification = input.specification;
                    found.vendor_id = input.vendor_id;
                    found.item_code = input.item_code;
                    found.category = input.category;
                    found.subcategory1 = input.subcategory1;
                    found.subcategory2 = input.subcategory2;
                    found.subcategory3 = input.subcategory3;
                    found.no_of_port = input.no_of_port;
                    found.is_multi_connection = input.is_multi_connection;
                    found.project_id = input.project_id;
                    found.workorder_id = input.workorder_id;
                    found.planning_id = input.planning_id;
                    found.purpose_id = input.purpose_id;
                    found.audit_item_master_id = input.audit_item_master_id;
                    found.remarks = input.remarks;
                    found.requested_by = input.requested_by;
                    found.request_approved_by = input.request_approved_by;
                    found.request_ref_id = input.request_ref_id;
                    found.origin_ref_id = input.origin_ref_id;
                    found.origin_ref_description = input.origin_ref_description;
                    found.origin_from = input.origin_from;
                    found.origin_ref_code = input.origin_ref_code;
                    // found.served_by_ring = input.served_by_ring;
                    found.bom_sub_category = input.bom_sub_category;
                    var result = repo.Update(found);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Equipment.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Equipment.ToString(), result.province_id);
                    return result;
                }
                else
                {

                    input.created_by = userId;
                    input.created_on = DateTimeHelper.Now;
                    input.status = "A";
                    input.network_status = "P";
                    input.equipment_name = input.model_name;
                    input.remarks = input.remarks;
                    var resultItem = repo.Insert(input);
                    resultItem.geom = resultItem.longitude + " " + resultItem.latitude;
                    //insert geometery detail....
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom.Replace(",", " ");
                    geom.userId = userId;
                    geom.entityType = EntityType.Equipment.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    if (input.no_of_input_port != 0 && input.no_of_output_port != 0)
                    { geom.ports = input.no_of_input_port + ":" + input.no_of_output_port; }
                    else if (input.no_of_port != 0) { geom.ports = input.no_of_port.ToString(); }
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Equipment.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Equipment.ToString(), resultItem.province_id);

                    return resultItem;
                }
            }
            catch { throw; }
        }



        public EquipmentInfo UpdateEquipment(EquipmentInfo input)
        {
            var found = repo.Get(x => x.system_id == input.system_id);
            if (found != null)
            {
                found.width = input.width;
                found.height = input.height;
                found.length = input.height;
                found.height = input.height;
            }
            return repo.Update(found);
        }

        public bool SaveEquipmentPosition(EquipmentInfo input, int userId) {
            bool isSaved = false;
            try
            {
                var found = repo.Get(x => x.system_id == input.system_id);
                if (found != null)
                {
                    found.pos_x = input.pos_x > 0 ? input.pos_x : found.pos_x;
                    found.pos_y = input.pos_y > 0 ? input.pos_y : found.pos_y;
                    found.pos_z = input.pos_z;
                    found.modified_by = userId;
                    found.modified_on = DateTimeHelper.Now;
                    repo.Update(found);
                    isSaved = true;
                }
            }
            catch { throw; }
            return isSaved;
        }
        public DbMessage DeleteEquipmentById(int systemId)
        {
            try
            {
                //Delete from isp_port_info table
                DbMessage msg = new Models.DbMessage();

               var objSystmId = repo.Get(x => x.system_id == systemId);
                //var children = repo.GetAll(x => x.super_parent == systemId);
                if (objSystmId != null)
                {
                    //Delete all related connection
                    DbMessage connections = isModelConnectionExist(systemId);
                    if (connections!=null && connections.status) {
                        msg.status = false;
                        msg.message = "Splicing existed, remove splicing first!";
                        return msg;
                    }
                    

                    //Delete from isp_port_info and children of equipment
                    DeleteEquipmentChildren(systemId);

                    //Delete FMS Exist

                    //delete equipment and its children
                    // repo.DeleteRange(children.ToList());
                    repo.Delete(objSystmId.system_id);
                    msg.status = true;
                    msg.message = "deleted successfully!";
                    return msg;
                }
                else
                {
                    msg.status = false;
                    msg.message = "Entity not found";
                    return msg;
                }


            }
            catch { throw; }
        }
       
        public List<EquipmentInfo> GetEquipmentByParentId(int parentId)
        {
            try
            {

                return repo.GetAll(x => x.parent_system_id == parentId).ToList();
            }
            catch { throw; }
        }
        public List<EquipmentInfo> GetEquipmentByRackId(int rackId)
        {
            try
            {
                return repo.GetAll(x => x.rack_id == rackId).ToList();
                //return repo.ExecuteProcedure<EquipmentInfo>("fn_isp_get_rack_equipments", new { p_rack_id = rackId, p_user_id = userId }, true).ToList();
            }
            catch { throw; }
        }
        public List<EquipmentInfo> GetEquipmentByRackId(int rackId, int parent_id, string parent_type, int userId)
        {
            try
            {
                //return repo.GetAll(x => x.rack_id == rackId).ToList();
                return repo.ExecuteProcedure<EquipmentInfo>("fn_isp_get_rack_equipments", new { p_rack_id = rackId, p_parent_id=parent_id, p_parent_type= parent_type, p_user_id = userId }, true).ToList();
            }
            catch { throw; }
        }
        public DbMessage SaveModelMapping(int id, int modelInfoId, int modelViewId,int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_isp_save_att_model_mapping", new { p_att_model_id = id, p_model_info_id = modelInfoId, p_model_view_id = modelViewId, p_user_id = userId }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public List<EquipmentInfo> GetEquipmentChildren(int parentId)
        {
            try
            {
                return repo.ExecuteProcedure<EquipmentInfo>("fn_isp_get_att_model_children", new { p_parent_id = parentId}, true).ToList();
            }
            catch { throw; }
        }

        public List<nodelist> GetEquipmentChildrenList(int system_id)
        {
            try
            {
                return repo.ExecuteProcedure<nodelist>("fn_isp_get_child_equipments", new { p_system_id = system_id }, true).ToList();
            }
            catch { throw; }
        }

        public List<nodelist> GetEquipmentChildrenListCount(int parent_system_id,int port_sequence_id)
        {
            try
            {
                return repo.ExecuteProcedure<nodelist>("fn_isp_get_child_equipments_count", 
                    new {
                        p_parent_system_id = parent_system_id,
                        p_port_sequence_id = port_sequence_id
                    }, true).ToList();
            }
            catch { throw; }
        }

        public List<PortConnecton> GetPortConnectionDetails(string parent_network_id, int port_sequence_id)
        {
            try
            {
                return repo.ExecuteProcedure<PortConnecton>("fn_isp_get_port_connction_details",
                    new
                    {
                        p_parent_network_id = parent_network_id,
                        p_port_sequence_id = port_sequence_id
                    }, true).ToList();
            }
            catch { throw; }
        }

        public DbMessage resetConnection()
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_isp_get_reset_all_connection", new {  }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<EquipmentInfo> GetChildModelDetails(int system_id)
        {
            try
            {
                return repo.ExecuteProcedure<EquipmentInfo>("fn_isp_get_child_model_details", new { p_system_id = system_id }, true).ToList();
            }
            catch { throw; }
        }

        public DbMessage DeleteEquipmentChildren(int id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_isp_delete_model_port_info", new { p_att_model_id = id }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage DeleteChildEquipment(int system_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_isp_delete_child_equipment", new { p_system_id = system_id }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<ModelConnection> GetModelConnections(int sourceId,string sourceType,int destinationId, string destinationType)
        {
            try
            {
                return repo.ExecuteProcedure<ModelConnection>("fn_isp_get_model_connection", new { p_source_system_id = sourceId, p_source_entity_type= sourceType, p_destination_system_id = destinationId, p_destination_entity_type = destinationType }, true).ToList();
            }
            catch { throw; }
        }
        public DbMessage isModelConnectionExist(int sourceId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_entity_is_connected", new { p_system_id = sourceId, p_entity_type ="equipment"}).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage RenameEquimentChildren( string mappingData)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_isp_save_att_model_names", new { p_model_children = mappingData }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public List<FMSMaster> GetUnmappedFMS(int podId,string layer_name,string parent_entity_type)
        {
            try
            {
                return repo.ExecuteProcedure<FMSMaster>("fn_isp_get_unmapped_fms", new
                {
                    p_parent_id = podId,
                    p_layer_name = layer_name,
                    p_parent_type = parent_entity_type
                }, true).ToList();
                //return repo.ExecuteProcedure<FMSMaster>("fn_isp_get_unmapped_fms", new
                //{
                //    p_parent_id = podId
                //}, true).ToList();
            }
            catch { throw; }
        }
        public List<ISPModelType> GetModelTypes()
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelType>("fn_isp_get_middleware_model_type", null, true);
                return res;
            }
            catch { throw; }
        }

        public List<DropDownMaster> getRackList(int systemId,string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_isp_get_racks_by_parent", new { p_system_id = systemId, p_entity_type=entityType }, true).ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<DropDownMaster> getCardList(int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_isp_get_cards_by_equipment", new { p_system_id = systemId }, true).ToList();
            }
            catch
            {
                throw;
            }
        }
        public EquipmentInfo GetById(int id)
        {
            try
            {

                return repo.Get(x => x.system_id == id);
            }
            catch { throw; }
        }
        public int GetPortCountById(int id)
        {
            try
            {

                return repo.GetAll(x => x.super_parent == id).Count();
            }
            catch { throw; }
        }
        public List<DropDownMaster> GetEquipmentByParent(int rackId, int parent_id, string parent_type)
        {
            try
            {
                try
                {
                    return repo.ExecuteProcedure<DropDownMaster>("fn_isp_get_equipment_by_parent", new { p_system_id = rackId,p_parent_id = parent_id,p_parent_type = parent_type }, true).ToList();
                }
                catch
                {
                    throw;
                }
            }
            catch { throw; }
        }

        public List<EquipmentInfo> GetByModelInfoId(int modelInfoId)
        {
            try
            {

                return repo.GetAll(x => x.model_info_id == modelInfoId).ToList();
            }
            catch { throw; }
        }

        public List<MultipleConnections> getConnectedPorts(int systemId, int portNo)
        {
            try
            {
                return repo.ExecuteProcedure<MultipleConnections>("fn_isp_get_model_port_details", new
                {
                    p_system_id = systemId,
                    p_port_no = portNo                   
                }, true);
            }
            catch { throw; }
        }
        public List<MultipleConnections> getOutConnectedPorts(int systemId, int portNo,int rackId)
        {
            try
            {
                return repo.ExecuteProcedure<MultipleConnections>("fn_isp_get_model_out_port_details", new
                {
                    p_system_id = systemId,
                    p_port_no = portNo,
                    p_rack_id= rackId
                }, true);
            }
            catch { throw; }
        }
        public List<PortsList> getPortByEquipment(int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<PortsList>("fn_isp_get_ports_by_equipment", new { p_card_system_id = systemId }, true).ToList();
            }
            catch
            {
                throw;
            }
        }
    }

    
}
