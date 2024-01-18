using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace BusinessLogics
{
    public class BLSpliceTray
    {
        private static BLSpliceTray objSpliceTray = null;
        private static readonly object lockObject = new object();
        public static BLSpliceTray Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSpliceTray == null)
                    {
                        objSpliceTray = new BLSpliceTray();
                    }
                }
                return objSpliceTray;
            }
        }
        //fn_get_loop_details
        public List<SpliceTrayInfo> GetSpliceTrayInfo(int entityId, string entityType)
        {
            return DASpliceTray.Instance.GetSpliceTrayInfo(entityId, entityType);
        }
        public int GetTrayUsedPort(int systemId)
        { return DASpliceTray.Instance.GetTrayUsedPort(systemId); }
        public void SaveSpliceTary(SpliceTrayInfo objSpliceTrayInfo)
        {
            DASpliceTray.Instance.SaveSpliceTary(objSpliceTrayInfo);
        }
        public int DeleteSpliceTaryInfoById(int system_id)
        {
            return DASpliceTray.Instance.DeleteSpliceTaryInfoById(system_id);
        }
    }
}
