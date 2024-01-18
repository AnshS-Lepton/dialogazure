using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public sealed class BLIspEntityMapping
    {
        BLIspEntityMapping() { }
        private static BLIspEntityMapping objEntityMap = null;
        private static readonly object lockObject = new object();

        public static BLIspEntityMapping Instance
        {
            get
            {
                lock (lockObject)
                {
                    if(objEntityMap==null)
                    {
                        objEntityMap = new BLIspEntityMapping();
                    }
                }
                return objEntityMap;
            }
        }

        public IspEntityMapping SaveIspEntityMapping(IspEntityMapping objMapping)
        {
            return DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
        } 

        public IspEntityMapping GetIspEntityMapByCustomerId(int customer_system_id, string entity_type)
        {
            return DAIspEntityMapping.Instance.GetIspEntityMapByCustomerId(customer_system_id, entity_type);
        }

        public IspEntityMapping GetIspEntityMapById(int mappingId)
        {
            return DAIspEntityMapping.Instance.GetIspEntityMapById(mappingId);
        }

        public int DeleteIspEntityMap(int mappingId)
        {
            return DAIspEntityMapping.Instance.DeleteIspEntityMap(mappingId);
        }

        public IspEntityMapping GetIspEntityMapByStrucId(int structure_id, int entity_id, string entity_type)
        {
            return DAIspEntityMapping.Instance.GetIspEntityMapByStrucId(structure_id, entity_id, entity_type);
        }

        public IspEntityMapping GetStructureFloorbyEntityId(int entity_id, string entity_type)
        {
            return DAIspEntityMapping.Instance.GetStructureFloorbyEntityId( entity_id, entity_type);
        }
        

        public bool IsShaftAssociated(int shaftId)
        {
            return DAIspEntityMapping.Instance.IsShaftAssociated(shaftId);
        }

        public bool IsFloorAssociated(int floorId)
        {
            return DAIspEntityMapping.Instance.IsFloorAssociated(floorId);
        }
        
    }
}
