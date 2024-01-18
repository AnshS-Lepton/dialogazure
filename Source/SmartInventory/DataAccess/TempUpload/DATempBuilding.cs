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
    public class DATempBuilding : Repository<TempBuilding>
    {
        PostgreSQL postgreSql;
        public DATempBuilding()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempBuilding> lstBuilding)
        {
            repo.Insert(lstBuilding);
        }

        public int InsertBuildingIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_building", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        //public void InsertBuildingIntoMainTable(UploadSummary summary, int batchId)
        //{
        //    string strQuery = string.Format("select * from du_fn_insert_building({0},{1});delete from tbl_du_{2} where upload_id={0} and is_valid=true and batch_id={1}", summary.id, batchId, summary.entity_type);
        //    postgreSql.ExecuteQuery(strQuery);
        //}
        //public void InsertBuildingIntoMainTable(UploadSummary summary)
        //{



        //    string strQuery = string.Format("listen upload_summary;select * from du_fn_insert_building({0},{1});delete from tbl_du_building where is_valid=true and upload_id={0}", summary.id, "'tbl_du_building'");
        //    postgreSql.ExecuteQuery(strQuery);
        //}


        public void Delete(string tempTableName)
        {
            //PostgreSQL postgreSql = new PostgreSQL();
            string strQuery = "delete from " + tempTableName;
            postgreSql.ExecuteQuery(strQuery);
        }

        //public void Validate(UploadSummary summary,string geomtype)
        //{
        //    PostgreSQL postgreSql = new PostgreSQL();
        //    postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        //    string strQuery = string.Format("select * from du_fn_validate_entity({0},'{1}','{2}','{3}')", summary.id, "tbl_du_Building",summary.entity_type, geomtype);
        //    postgreSql.ExecuteQuery(strQuery);
        //}

        public List<TempBuilding> GetAll(int uploadId)
        {
            return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
        }
        public DataTable GetAll(UploadSummary uploadSummary)
        {
            return repo.GetDataTable("select distinct batch_id  from temp_du_building where upload_id=" + uploadSummary.id + " order by batch_id ");
        }
    }
}
