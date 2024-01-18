using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;
using Models.TempUpload;
using System.Data;

namespace DataAccess
{
    public class DATempStructure : Repository<TempStructure>
    {
        PostgreSQL postgreSql;
        public DATempStructure()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempStructure> lstTemp)
        {
            repo.Insert(lstTemp);
        }
        
        public List<TempStructure> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
        public int InsertStructureIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_structure", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }
    }
}
