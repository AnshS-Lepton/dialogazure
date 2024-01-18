using System.Collections.Generic;
using Models.TempUpload;
using DataAccess.DBHelpers;
using Models;
using System;
using System.Linq;

namespace DataAccess
{
    public class DATempSector : Repository<TempSector>
    {
        PostgreSQL postgreSql;
        public DATempSector()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempSector> lst)
        {
            repo.Insert(lst);
        }

        public int InsertSectorIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_sector", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempSector> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
