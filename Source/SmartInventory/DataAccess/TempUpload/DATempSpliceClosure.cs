using DataAccess.DBHelpers;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Linq;

namespace DataAccess
{
    public class DATempSpliceClosure : Repository<TempSpliceClosure>
    {
        PostgreSQL postgreSql;
        public void Save(List<TempSpliceClosure> lstSpliceClosure)
        {
            repo.Insert(lstSpliceClosure);
        }
        public DATempSpliceClosure()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;

        }
        public int InsertSpliceClosureIntoMainTable(UploadSummary summary,int batchId)
        {
         
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_spliceclosure", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;

        }

        public List<TempSpliceClosure> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
