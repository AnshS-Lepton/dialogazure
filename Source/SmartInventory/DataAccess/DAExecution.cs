using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAExecution : Repository<TrenchExecution>
    {
        private static DAExecution objExecution = null;
        private static readonly object lockObject = new object();
        public static DAExecution Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objExecution == null)
                    {
                        objExecution = new DAExecution();
                    }
                }
                return objExecution;
            }
        }


        public List<TrenchExecution> GetExecutionStatus(int entityid, string entitytype)
        {
            try
            {
                return repo.GetAll(m => m.system_id == entityid && m.entity_type.ToUpper() == entitytype.ToUpper()).ToList();
            }
            catch { throw; }
        }
        public void SaveExecutionmethod(List<TrenchExecution> objExecution, int system_id, int userId)
        {
            try
            {
                objExecution = objExecution.Where(m => m.execution_method != "0").ToList();
                if (objExecution != null)
                {
                    var listExecutionUpdate = objExecution.Where(x => x.id > 0).ToList();
                    var listExecutionInsert = objExecution.Where(x => x.id == 0).ToList();
                    if (listExecutionUpdate.Any())
                    {
                        listExecutionUpdate.ForEach(x => x.modified_on = DateTimeHelper.Now);
                        listExecutionUpdate.ForEach(x => x.modified_by = userId);
                        repo.Update(listExecutionUpdate);
                    }
                    if (listExecutionInsert.Any())
                    {
                        listExecutionInsert.ForEach(x => x.created_on = DateTimeHelper.Now);
                        listExecutionInsert.ForEach(x => x.created_by = userId);
                        repo.Insert(listExecutionInsert);
                    }
                }
            }
            catch { throw; }
        }

    }
}
