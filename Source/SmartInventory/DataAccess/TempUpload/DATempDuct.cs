using DataAccess.DBHelpers;
using Models.TempUpload;
using System;
using System.Collections.Generic;
namespace DataAccess
{
    public class DATempDuct : Repository<TempDuct>
    {
        PostgreSQL postgreSql;
        public DATempDuct()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;

        }
        public void Save(List<TempDuct> lstDuct)
        {
            repo.Insert(lstDuct);
        }

        public int InsertDuctIntoMainTable(Models.UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_duct", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;

        }


        public List<TempDuct> GetAll(int uploadId)
        {
            return (List<TempDuct>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
