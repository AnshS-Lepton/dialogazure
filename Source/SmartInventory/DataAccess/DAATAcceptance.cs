using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAATAcceptance : Repository<entityAtAcceptance>
    {
        private static DAATAcceptance objATAcceptance = null;
        private static readonly object lockObject = new object();
        public static DAATAcceptance Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objATAcceptance == null)
                    {
                        objATAcceptance = new DAATAcceptance();
                    }
                }
                return objATAcceptance;
            }
        }


        public List<entityAtAcceptance> GetATStatus(int entityid, string entitytype)
        {
            try
            {
                return repo.GetAll(m => m.system_id == entityid && m.entity_type.ToUpper() == entitytype.ToUpper()).ToList();
            }
            catch { throw; }
        }

        public List<ATExport> GetATDetails(int entityid, string entitytype)
        {
            try
            {

                return repo.ExecuteProcedure<ATExport>("fn_get_at_details", new { p_entityid = entityid, p_entitytype = entitytype }, true);

            }
            catch { throw; }
        }
        public void SaveATAcceptance(List<entityAtAcceptance> objATAcceptance, int system_id, int userId)
        {
            try
            {
                objATAcceptance = objATAcceptance.Where(m => m.status != "0").ToList();
                if (objATAcceptance != null)
                {
                    var listATUpdate = objATAcceptance.Where(x => x.id > 0).ToList();
                    var listATInsert = objATAcceptance.Where(x => x.id == 0).ToList();
                    if (listATUpdate.Any())
                    {
                        listATUpdate.ForEach(x => x.modified_on = DateTimeHelper.Now);
                        listATUpdate.ForEach(x => x.modified_by = userId);
                        repo.Update(listATUpdate);
                    }
                    if (listATInsert.Any())
                    {
                        listATInsert.ForEach(x => x.created_on = DateTimeHelper.Now);
                        listATInsert.ForEach(x => x.created_by = userId);
                        repo.Insert(listATInsert);
                    }
                }
            }
            catch { throw; }
        }
    }
}
