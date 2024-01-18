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
    public class DATempWallMount : Repository<TempWallMount>
    {

        PostgreSQL postgreSQL;
        public DATempWallMount()
        {
            postgreSQL = new PostgreSQL();
            postgreSQL.PostgresNotificationEvent += NotifyUpdatedStatus;
        }

        public void Save(List<TempWallMount> lstWallMount)
        {
            repo.Insert(lstWallMount);
        }

        public int InsertWallMountIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_wallmount", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }
        public List<TempWallMount> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }

    }
}
