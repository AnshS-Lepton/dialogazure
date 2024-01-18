using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.TempUpload
{
   public class DATempLandBase : Repository<TempLandBase>
    {
        PostgreSQL postgreSql;
        public DATempLandBase()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempLandBase> lstLandBase)
        {
            repo.Insert(lstLandBase);
        }

        public int InsertLandBaseIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_landbase_uploader_insert", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempLandBase> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
