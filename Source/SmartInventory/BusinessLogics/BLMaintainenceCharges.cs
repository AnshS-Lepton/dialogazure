using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BusinessLogics
{
    public class BLMaintainenceCharges
    {
        private static BLMaintainenceCharges objEntityMaintainenceCharges = null;
        private static readonly object lockObject = new object();
        public static BLMaintainenceCharges Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objEntityMaintainenceCharges == null)
                    {
                        objEntityMaintainenceCharges = new BLMaintainenceCharges();
                    }
                }
                return objEntityMaintainenceCharges;
            }
        }
        public List<EntityMaintainenceCharges> GetEntityMaintainenceChargesRecords(int entityid, string entityType)
        {
            return DAMaintainenceCharges.Instance.GetEntityMaintainenceChargesRecords(entityid, entityType);
        }

        

        public List<EMCExport> GetEMChargesDetails(int entityid, string entityType)
        {
            return DAMaintainenceCharges.Instance.GetEMChargesDetails(entityid, entityType);
        }
        public EntityMaintainenceCharges getChargeDetails(int systemId)
        {
            return DAMaintainenceCharges.Instance.getChargeDetails(systemId);
        }
        public EntityMaintainenceCharges SaveChargeDetails(EntityMaintainenceCharges objCharges, int userId)
        {
            return DAMaintainenceCharges.Instance.SaveChargeDetails(objCharges, userId);
        }
    }
}
