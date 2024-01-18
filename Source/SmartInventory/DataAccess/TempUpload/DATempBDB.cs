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
    public class DATempBDB : Repository<TempBDB>
    {
        PostgreSQL postgreSQL;
        public DATempBDB()
        {
            postgreSQL = new PostgreSQL();
            postgreSQL.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempBDB> lstBDB)
        {
            repo.Insert(lstBDB);
        }


        public int InsertBDBIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_bdb", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            //var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_bdb_backup_09062021", new { p_uploadid = summary.id, p_batchid = batchId }, false);

            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempBDB> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
