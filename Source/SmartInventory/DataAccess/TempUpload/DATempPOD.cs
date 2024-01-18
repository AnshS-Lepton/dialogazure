using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DATempPOD : Repository<TempPOD>
    {
        public void Save(List<TempPOD> objTemp)
        {
            repo.Insert(objTemp);
        }

        public int InsertPopIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_pod", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }
        public List<TempPOD> GetAll(int uploadId)
        {
            return (List<TempPOD>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
