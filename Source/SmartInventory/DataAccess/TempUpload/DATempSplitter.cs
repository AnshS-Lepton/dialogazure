using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class DATempSplitter : Repository<TempSplitter>
    {
        PostgreSQL postgreSql;
        public DATempSplitter()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;

        }
        public void Save(List<TempSplitter> lstSplitter)
        {
            repo.Insert(lstSplitter);
        }

        public int InsertSplitterIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_splitter", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;

        }

        public List<TempSplitter> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
