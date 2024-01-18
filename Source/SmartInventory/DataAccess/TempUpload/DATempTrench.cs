using DataAccess.DBHelpers;
using Models.TempUpload;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class DATempTrench : Repository<TempTrench>
    {
        public void Save(List<TempTrench> lstTrench)
        {
            repo.Insert(lstTrench);
        }

        public int InsertTrenchIntoMainTable(Models.UploadSummary summary,int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_trench", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;

        }
        public List<TempTrench> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
    }
}
