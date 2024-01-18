using DataAccess.DBHelpers;
using Models.TempUpload;
using System;
using System.Collections.Generic;
namespace DataAccess
{
    public class DATempFMS : Repository<TempFMS>
    {
        public void Save(List<TempFMS> lstTempFMS)
        {

            repo.Insert(lstTempFMS);
        }

        public int InsertFMSIntoMainTable(Models.UploadSummary summary,int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_fms", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempFMS> GetAll(int uploadId)
        {
            return (List<TempFMS>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
