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
    public class DATempTree : Repository<TempTree>
    {

        PostgreSQL postgreSQL;
        public DATempTree()
        {
            postgreSQL = new PostgreSQL();
            postgreSQL.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempTree> lstTree)
        {
            repo.Insert(lstTree);
        }

        public int InsertTreeIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_tree", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }
        public List<TempTree> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
