using DataAccess.DBHelpers;
using Models.TempUpload;
using System;
using System.Collections.Generic;
namespace DataAccess
{
    public class DATempMicroduct : Repository<TempMicroduct>
    {
        PostgreSQL postgreSql;
        public DATempMicroduct()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;

        }
        public void Save(List<TempMicroduct> lstMicroduct)
        {
            repo.Insert(lstMicroduct);
        }

        public int InsertMicroductIntoMainTable(Models.UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_microduct", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;

        }


        public List<TempMicroduct> GetAll(int uploadId)
        {
            return (List<TempMicroduct>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
