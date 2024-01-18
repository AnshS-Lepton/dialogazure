using System.Collections.Generic;
using Models.TempUpload;
using DataAccess.DBHelpers;
using Models;
using System;

namespace DataAccess
{
    public class DATempPOI : Repository<TempPOI>
    {
        public void Save(List<TempPOI> lstDuct)
        {
            repo.Insert(lstDuct);
        }

        public void InsertPOIIntoMainTable(UploadSummary summary)
        {
            repo.ExecuteSQLCommand(string.Format("select * from fn_bulkupload_poi({0},{1})", summary.id, "'Wireless'"));
        }
    }
}
