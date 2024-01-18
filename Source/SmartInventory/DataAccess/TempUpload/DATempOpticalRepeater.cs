using DataAccess.DBHelpers;
using System.Collections.Generic;
using Models;
using Models.TempUpload;

namespace DataAccess.TempUpload
{
    public class DATempOpticalRepeater : Repository<TempOpticalRepeater>
    {
        PostgreSQL postgreSql;
        public DATempOpticalRepeater()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempOpticalRepeater> lstTempOpticalRepeater)
        {
            repo.Insert(lstTempOpticalRepeater);
        }

        public int InsertOpticalRepeaterIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_opticalrepeater", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempOpticalRepeater> GetAll(int uploadId)
        {
            return (List<TempOpticalRepeater>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
