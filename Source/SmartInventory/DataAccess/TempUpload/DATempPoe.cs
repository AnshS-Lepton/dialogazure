using System.Collections.Generic;
using Models.TempUpload;
using DataAccess.DBHelpers;
namespace DataAccess
{
    public class DATempPoe : Repository<TempPoe>
    {
        public void Save(List<TempPoe> lstTempPoe)
        {
            repo.Insert(lstTempPoe);   
        }

        public void InsertPOEIntoMainTable(Models.UploadSummary summary)
        {
            repo.ExecuteSQLCommand(string.Format("select * from fn_bulkupload_poe({0},{1})", summary.id, "''"));

        }
    }
}
