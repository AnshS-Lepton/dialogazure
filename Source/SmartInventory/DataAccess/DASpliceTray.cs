using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DASpliceTray : Repository<SpliceTrayInfo>
    {
        private static DASpliceTray objSpliceTray = null;
        private static readonly object lockObject = new object();
        public static DASpliceTray Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSpliceTray == null)
                    {
                        objSpliceTray = new DASpliceTray();
                    }
                }
                return objSpliceTray;
            }
        }

        public List<SpliceTrayInfo> GetSpliceTrayInfo(int entityId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<SpliceTrayInfo>("fn_get_splice_tray_Info", new { p_entityId = entityId, p_entityType = entityType}, true);
            }
            catch { throw; }
        }
        public int GetTrayUsedPort(int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<int>("fn_get_tray_used_port", new { p_system_id = systemId}).FirstOrDefault();
            }
            catch { throw; }
        }
        public void SaveSpliceTary(SpliceTrayInfo objSpliceTrayInfo)
        {
            try
            {
                if (objSpliceTrayInfo.system_id > 0)
                {
                    var objTrayInfo = repo.Get(u => u.system_id == objSpliceTrayInfo.system_id);
                    objTrayInfo.network_name = objSpliceTrayInfo.network_name;
                    objTrayInfo.no_of_ports = objSpliceTrayInfo.no_of_ports;
                    objTrayInfo.modified_by = objSpliceTrayInfo.modified_by;
                    objTrayInfo.modified_on = objSpliceTrayInfo.modified_on;
                    repo.Update(objTrayInfo);
                }
                else
                {
                    repo.Insert(objSpliceTrayInfo);
                }
            }
            catch { throw; }
        }
        public int DeleteSpliceTaryInfoById(int system_id)
        {
            int result = 0;          
            try
            {
                result = repo.Delete(system_id);
                return result;
            }
            catch
            {
                throw;
            }           
        }
    }
}
