using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System.Collections.Generic;

namespace DataAccess.TempUpload
{
    public class DATempRow : Repository<TempRow>
    {
        public void Save(List<TempRow> lstRow)
        {
            repo.Insert(lstRow);
        }

        public int InsertROWIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_row", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempRow> GetAll(int UploadId)
        {
            return (List<TempRow>)repo.GetAll(x => x.upload_id == UploadId && x.is_valid == true);
        }
    }
}
