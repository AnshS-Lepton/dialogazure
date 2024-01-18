using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLRack
    {
        private static BLRack instance = null;
        private static readonly object lockObject = new object();
        public static BLRack Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new BLRack();
                    }
                }
                return instance;
            }
        }
        public RackInfo SaveEntityRack(RackInfo input, int userId)
        {
            return DARack.Instance.SaveEntityRack(input, userId);
        }
        public PageMessage DeleteRackById(int systemId)
        {
            PageMessage result = new PageMessage();
            DbMessage msg = DARack.Instance.DeleteRackById(systemId);
            if (msg.status)
            {
                result.status = ResponseStatus.OK.ToString();
            }
            else
            {
                result.status = ResponseStatus.FAILED.ToString();
                result.message = msg.message;
            }
            return result;
            //return DARack.Instance.DeleteRackById(systemId);
        }

        public List<RackInfo> GetRacksByParentId(int parenteId,string parent_type, int userId)
        {
            return DARack.Instance.GetRackByParentId(parenteId, parent_type, userId);
        }

        public List<EquipmentInfo> GetEquipmentinRack(int parenteId,string parent_type, int userId)
        {
            return DARack.Instance.GetEquipmentinRack(parenteId,parent_type, userId);
        }

        public EquipmentInfo SaveEntityEquipment(EquipmentInfo input, int userId)
        {
            return DAEquipment.Instance.SaveEntityEquipment(input, userId);
        }
        public EquipmentInfo UpdateEquipment(EquipmentInfo input)
        {
            return DAEquipment.Instance.UpdateEquipment(input);
        }
        public PageMessage DeleteEquipmentById(int systemId)
        {
            PageMessage result = new PageMessage();
            DbMessage msg = DAEquipment.Instance.DeleteEquipmentById(systemId);
            if (msg.status)
            {
                result.status = ResponseStatus.OK.ToString();
                result.message = msg.message;
            }
            else
            {
                result.status = ResponseStatus.FAILED.ToString();
                result.message = msg.message;
            }
            return result;
        }
        public DbMessage DeleteChildEquipment(int systemId)
        {
            
            return DAEquipment.Instance.DeleteChildEquipment(systemId);
            
        }

        public List<EquipmentInfo> GetEquipmentByParentId(int parenteId)
        {
            return DAEquipment.Instance.GetEquipmentByParentId(parenteId);
        }

        public DbMessage SaveEquipmentMapping(int id, int modelInfoId, int modelViewId, int userId)
        {
            return DAEquipment.Instance.SaveModelMapping(id, modelInfoId, modelViewId, userId);
        }

        public List<EquipmentInfo> GetEquipmentChildren(int parenteId)
        {
            return DAEquipment.Instance.GetEquipmentChildren(parenteId);
        }

        public List<nodelist> GetEquipmentChildrenList(int system_id)
        {
            return DAEquipment.Instance.GetEquipmentChildrenList(system_id);
        }

        public List<nodelist> GetEquipmentChildrenListCount(int parent_system_id, int port_sequence_id)
        {
            return DAEquipment.Instance.GetEquipmentChildrenListCount(parent_system_id, port_sequence_id);
        }

        public List<PortConnecton> GetPortConnectionDetails(string parent_network_id, int port_sequence_id)
        {
            return DAEquipment.Instance.GetPortConnectionDetails(parent_network_id, port_sequence_id);
        }
        public DbMessage resetConnection()
        {
            return DAEquipment.Instance.resetConnection();
        }
        public List<EquipmentInfo> GetChildModelDetails(int system_id)
        {
            return DAEquipment.Instance.GetChildModelDetails(system_id);
        }

        public bool HasRackChild(int parenteId)
        {
            return DAEquipment.Instance.GetEquipmentByRackId(parenteId).Count > 0;
        }

        public bool SaveEquipmentPosition(EquipmentInfo input, int userId)
        {
            return DAEquipment.Instance.SaveEquipmentPosition(input, userId);
        }

        public bool SaveRackPosition(RackInfo input)
        {
            return DARack.Instance.SaveRackPosition(input);
        }

        public List<ModelConnection> GetModelConnections(int sourceId, string sourceType, int destinationId, string destinationType)
        {
            return DAEquipment.Instance.GetModelConnections(sourceId, sourceType, destinationId, destinationType);
        }
        public DbMessage RenameEquimentChildren(string input)
        {
            return DAEquipment.Instance.RenameEquimentChildren(input);
        }
        public List<EquipmentInfo> GetEquipmentByRackId(int rackId,int parent_id,string parent_type, int userId)
        {
            return DAEquipment.Instance.GetEquipmentByRackId(rackId, parent_id, parent_type, userId);
        }

        public List<FMSMaster> GetUnmappedFMS(int podId,string layer_name,string parent_entity_type)
        {
            return DAEquipment.Instance.GetUnmappedFMS(podId, layer_name, parent_entity_type);
        }
        public List<ISPModelType> GetModelTypes()
        {
            return DAEquipment.Instance.GetModelTypes();
        }
        public List<DropDownMaster> getRackList(int systemId, string entityType)
        {
            return DAEquipment.Instance.getRackList(systemId, entityType);
        }
        public List<DropDownMaster> getCardList(int systemId)
        {
            return DAEquipment.Instance.getCardList(systemId);
        }

        public EquipmentInfo GetEquipmentById(int id)
        {
            return DAEquipment.Instance.GetById(id);
        }
        public int GetPortCountById(int id)
        {
            return DAEquipment.Instance.GetPortCountById(id);
        }
        public List<DropDownMaster> GetEquipmentByParent(int rackId, int parent_id, string parent_type)
        {
            return DAEquipment.Instance.GetEquipmentByParent(rackId, parent_id, parent_type);
        }
        public List<EquipmentInfo> GetByModelInfoId(int modelInfoId)
        {
            return DAEquipment.Instance.GetByModelInfoId(modelInfoId);
        }
        public List<MultipleConnections> getConnectedPorts(int systemId, int portNo)
        {
            return DAEquipment.Instance.getConnectedPorts(systemId, portNo);
        }
        public List<MultipleConnections> getOutConnectedPorts(int systemId, int portNo, int rackId)
        {
            return DAEquipment.Instance.getOutConnectedPorts(systemId, portNo, rackId);
        }
        public List<PortsList> getPortByEquipment(int systemId)
        {
            return DAEquipment.Instance.getPortByEquipment(systemId);
        }

    }

}
