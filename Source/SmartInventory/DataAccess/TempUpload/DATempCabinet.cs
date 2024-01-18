using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class DATempCabinet : Repository<TempCabinet>
    {
        PostgreSQL postgreSql;
        public DATempCabinet()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempCabinet> lstCabinet)
        {
            repo.Insert(lstCabinet);
        }

        public int InsertCabinetIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_cabinet", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }
        public List<TempCabinet> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
