using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAMaintainenceCharges : Repository<EntityMaintainenceCharges>
    {
        private static DAMaintainenceCharges objEntityMaintainenceCharges = null;
        private static readonly object lockObject = new object();
        public static DAMaintainenceCharges Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objEntityMaintainenceCharges == null)
                    {
                        objEntityMaintainenceCharges = new DAMaintainenceCharges();
                    }
                }
                return objEntityMaintainenceCharges;
            }
        }
        public EntityMaintainenceCharges getChargeDetails(int systemId)
        {
            try
            {               
                return repo.GetById(m => m.id == systemId);
            }
            catch { throw; }
        }
        public EntityMaintainenceCharges SaveChargeDetails(EntityMaintainenceCharges objCharges, int userId)
        {
            var chargeDetails = repo.GetById(m => m.id == objCharges.id);
            if (chargeDetails != null)
            {
                chargeDetails.activity_end_date = objCharges.activity_end_date;
                chargeDetails.activity_start_date = objCharges.activity_start_date;
                chargeDetails.charge_category = objCharges.charge_category;
                chargeDetails.remark = objCharges.remark;
                chargeDetails.type_of_activity_charge = objCharges.type_of_activity_charge;
                chargeDetails.modified_by = userId;
                chargeDetails.modified_on = DateTimeHelper.Now;
                chargeDetails.activity_end_date = objCharges.activity_end_date;
                return repo.Update(chargeDetails);

            }
            else
            {
                objCharges.created_by = userId;
                objCharges.created_on = DateTimeHelper.Now;
                return repo.Insert(objCharges);
            }

        }
        public List<EntityMaintainenceCharges> GetEntityMaintainenceChargesRecords(int entityid, string entitytype)
        {
            try
            {

                return repo.ExecuteProcedure<EntityMaintainenceCharges>("fn_get_entity_mainteinance_charges", new { p_entityid = entityid, p_entitytype = entitytype }, true).ToList();

            }
            catch { throw; }
        }


        public List<EMCExport> GetEMChargesDetails(int entityid, string entitytype)
        {
            try
            {

                return repo.ExecuteProcedure<EMCExport>("fn_get_Maintainence_charges_details", new { p_entityid = entityid, p_entitytype = entitytype }, true);

            }
            catch { throw; }
        }
    }
}
