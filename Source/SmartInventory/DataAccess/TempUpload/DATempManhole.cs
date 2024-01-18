using DataAccess.DBHelpers;
using System.Collections.Generic;
using Models;
using Models.TempUpload;
using System;

namespace DataAccess
{
    public class DATempManhole : Repository<TempManhole>
    {
        PostgreSQL postgreSql;
        public DATempManhole()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempManhole> lstTempPoe)
        {
            repo.Insert(lstTempPoe);
        }

        public int InsertManholeIntoMainTable(UploadSummary summary,int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_manhole", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempManhole> GetAll(int uploadId)
        {
            return (List<TempManhole>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
