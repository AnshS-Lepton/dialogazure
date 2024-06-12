using DataAccess;
using Models;
using System.Collections.Generic;


namespace BusinessLogics
{
    public class BLExecution
    {
        private static BLExecution objExecution = null;
        private static readonly object lockObject = new object();
        public static BLExecution Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objExecution == null)
                    {
                        objExecution = new BLExecution();
                    }
                }
                return objExecution;
            }
        }
        public List<TrenchExecution> GetExecutionStatus(int entityid, string entityType)
        {
            return DAExecution.Instance.GetExecutionStatus(entityid, entityType);
        }

        public int DeleteExecutionStatus(int entityid, string entityType)
        {
            return DAExecution.Instance.DeleteExecutionStatus(entityid, entityType);
        }

        
        public void SaveExecutionmethod(List<TrenchExecution> objExecution, int system_id, int userId)
        {
            DAExecution.Instance.SaveExecutionmethod(objExecution, system_id, userId);
        }
    }
}
