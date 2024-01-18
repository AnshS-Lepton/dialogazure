using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
  public  class BLFaultStatusHistory
    {
        BLFaultStatusHistory()
        {

        }
        private static BLFaultStatusHistory objFaultStatus = null;
        private static readonly object lockObject = new object();
        public static BLFaultStatusHistory Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objFaultStatus == null)
                    {
                        objFaultStatus = new BLFaultStatusHistory();
                    }
                }
                return objFaultStatus;
            }
        }
        public FaultStatusHistory SaveFaultStatusHistory(FaultStatusHistory objFaultStatus, int userId)
        {
            return DAFaultStatusHistory.Instance.SaveFaultStatusHistory(objFaultStatus, userId);
        }
        public FaultStatusHistory GetFaultStatusHistoryById(int systemId)
        {
            return DAFaultStatusHistory.Instance.GetFaultStatusHistoryById(systemId);
        }
        public List<FaultStatusHistory> getFaultStatusHistoryList(int fault_system_id)
        {
            return  DAFaultStatusHistory.Instance.getFaultStatusHistoryList(fault_system_id);
        }
        public List<ExportFaultHistory> exportFaultStatusHistoryList(int fault_system_id)
        {
            return DAFaultStatusHistory.Instance.exportFaultStatusHistoryList(fault_system_id);
        }
        public int DeleteStatusHistorybyFaultId(int systemId)
        {
            return DAFaultStatusHistory.Instance.DeleteStatusHistorybyFaultId(systemId);
        }
    }
}
