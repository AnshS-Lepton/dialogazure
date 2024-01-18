using DataAccess.DBHelpers;
using System.Collections.Generic;
using Models;
using Models.TempUpload;
using System;

namespace DataAccess.TempUpload
{
    public class DATempHandhole : Repository<TempHandhole>
    {
        PostgreSQL postgreSql;
        public DATempHandhole()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempHandhole> lstTempHandhole)
        {
            repo.Insert(lstTempHandhole);
        }

        public int InsertHandholeIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_handhole", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempHandhole> GetAll(int uploadId)
        {
            return (List<TempHandhole>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
