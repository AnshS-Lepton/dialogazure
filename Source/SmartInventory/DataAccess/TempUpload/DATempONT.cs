using DataAccess.DBHelpers;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Linq;
using System.Data;

namespace DataAccess
{
    public class DATempONT : Repository<TempONT>
    {
        PostgreSQL postgreSql;
        public DATempONT()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempONT> lstONT)
        {
            repo.Insert(lstONT);
        }

        public int InsertONTIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_ont", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            //var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_ont_Backup_04062021", new { p_uploadid = summary.id, p_batchid = batchId }, false);
             return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }
        public List<TempONT> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }

        public bool validateShaftFloorinfo(string shaft_name, string floor_name, string  parent_network_id)
        {   
            var  result =  repo.ExecuteProcedure<bool>("fn_get_shaft_floor_list", new { p_shaft_name = shaft_name, p_floor_name = floor_name, p_parent_network_id = parent_network_id }, false);
            return Convert.ToBoolean(result[0]) ;
        }
    }
}
