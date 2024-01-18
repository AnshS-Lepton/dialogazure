using DataAccess.DBHelpers;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System;

namespace DataAccess
{
    public class DACustomerWireless : Repository<TempWirelessCustomer>
    {
        PostgreSQL postgreSql;
        public DACustomerWireless()
        {
            postgreSql = new PostgreSQL();
            postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public void Save(List<TempWirelessCustomer> lstTempCust)
        {
            repo.Insert(lstTempCust);
        }

        public int InsertCustomerIntoMainTable(UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_customer", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempWirelessCustomer> GetAll(int uploadId)
        {
            return (List<TempWirelessCustomer>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }

}
