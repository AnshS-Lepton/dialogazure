using DataAccess.DBHelpers;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Data;
using WebGrease.Css.Ast.Selectors;

namespace DataAccess
{
    public class DATempADB : Repository<TempADB>
    {
        PostgreSQL postgreSql;
        public DATempADB()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;

        }
        public void Save(List<TempADB> lstADB)
        {
            repo.Insert(lstADB);
        }

        public int InsertADBIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_adb", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;

        }

        public List<TempADB> GetAll(int uploadId)
        {
            return (List<TempADB>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
