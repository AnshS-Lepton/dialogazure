using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
  public  class BLFault
    {
       public BLFault()
        {

        }
        private static BLFault objFault = null;
        private static readonly object lockObject = new object();
        public static BLFault Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objFault == null)
                    {
                        objFault = new BLFault();
                    }
                }
                return objFault;
            }
        }

        public Fault SaveFault(Fault faultInfo, int userId)
        {
            return DAFault.Instance.SaveFault(faultInfo,userId);
        }
        public Fault GetFaultById(int systemId)
        {
            return DAFault.Instance.GetFaultById(systemId);
        }
        public FaultDetails GetFaultID(string parentCode)
        {
            return DAFault.Instance.GetFaultID(parentCode);
        }
        public List<Fault> GetFaultTypeList(string searchText)
        {
            return DAFault.Instance.GetFaultTypeList(searchText);
        }


    }
}
