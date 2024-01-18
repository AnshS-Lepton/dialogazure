using DataAccess.DBHelpers;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Linq;
using System.Data;

namespace DataAccess.TempUpload
{
    public class DATempCable : Repository<TempCable>
    {
        public void Save(List<TempCable> lstCable)
        {
            repo.Insert(lstCable);
        }

        public int InsertCableIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_cable", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempCable> GetAll(int UploadId)
        {
            return (List<TempCable>)repo.GetAll(x => x.upload_id == UploadId && x.is_valid == true);
        }
    }
}
