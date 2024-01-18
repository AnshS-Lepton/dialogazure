using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class DATempPole : Repository<TempPole>
    {
        PostgreSQL postgreSql;
        public DATempPole()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempPole> lstPole)
        {
            repo.Insert(lstPole);
        }

        public int InsertPoleIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_pole", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempPole> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
