using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLATAcceptance
    {
        private static BLATAcceptance objATAcceptance = null;
        private static readonly object lockObject = new object();
        public static BLATAcceptance Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objATAcceptance == null)
                    {
                        objATAcceptance = new BLATAcceptance();
                    }
                }
                return objATAcceptance;
            }
        }


        public List<entityAtAcceptance> GetATStatus(int entityid, string entityType)
        {
            return DAATAcceptance.Instance.GetATStatus(entityid, entityType);
        }
        public List<ATExport> GetATDetails(int entityid, string entityType)
        {
            return DAATAcceptance.Instance.GetATDetails(entityid, entityType);
        }


        public void SaveATAcceptance(List<entityAtAcceptance> objATAcceptance, int system_id,int userId)
        {
            DAATAcceptance.Instance.SaveATAcceptance(objATAcceptance, system_id, userId);
        }
    }
}
