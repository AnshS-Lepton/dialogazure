using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models.TempUpload;
using Models;
using System.Data;

namespace DataAccess
{
  public  class DATempLoop: Repository<TempLoop>
    {
        PostgreSQL postgreSql;
        public DATempLoop()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;

        }
        public void Save(List<TempLoop> lstObj)
        {
            repo.Insert(lstObj);
        }

        public int InsertLoopIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_loop", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }
        public List<TempLoop> GetAll(int uploadId)
        {
            return (List<TempLoop>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
