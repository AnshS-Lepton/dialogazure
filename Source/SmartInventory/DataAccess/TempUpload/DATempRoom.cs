using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.TempUpload
{
   public class DATempRoom : Repository<TempRoom>
    {
        PostgreSQL postgreSql;
        public DATempRoom()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempRoom> lstRoom)
        {
            repo.Insert(lstRoom);
        }

        public int InsertRoomIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_room", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        } 

        public void Delete(string tempTableName)
        { 
            string strQuery = "delete from " + tempTableName;
            postgreSql.ExecuteQuery(strQuery);
        } 
        public List<TempRoom> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
        public DataTable GetAll(UploadSummary uploadSummary)
        {
            return repo.GetDataTable("select distinct batch_id  from temp_du_room where upload_id=" + uploadSummary.id + " order by batch_id ");
        }
    }
}
