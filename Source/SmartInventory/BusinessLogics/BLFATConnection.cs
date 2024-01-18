using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;

namespace BusinessLogics
{
    public class BLFATConnection
    {
        public List<FATDetail> GetConnectionDetails(FATDetailprocess objfilter)
        {
            return new DAFATConnection().GetConnectionDetails(objfilter);
        }

        public FatProcessResult CreateSplicing(FatProcessSummary oFatProcessSummary)
        {
            return new DAFATConnection().CreateSplicing(oFatProcessSummary);
        }
        public FatProcessResult UpdateSplicingStatus(int sub_area_system_id, int user_id, string action_name)
        {
            return new DAFATConnection().UpdateSplicingStatus(sub_area_system_id, user_id, action_name);
        }
        public int GetConnectionCount(int fsa_system_id)
        {
            return new DAFATConnection().GetConnectionCount(fsa_system_id);
        }
        public FatProcessRunningStatus GetSetBKGStatus(int fsa_system_id,  string sAction, string sMessage = "")
        {
            return new DAFATConnection().GetSetBKGStatus(fsa_system_id, sAction, sMessage);
        }
        public bool UpdatePortStatus(int process_id, string action)
        {
            return new DAFATConnection().UpdatePortStatus(process_id, action);
        }

       public bool UpdatSnapCableEndPoint(int process_id)
        {
            return new DAFATConnection().UpdatSnapCableEndPoint(process_id);
        }
    }
}
