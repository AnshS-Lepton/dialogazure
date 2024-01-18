using DataAccess;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System;

namespace BusinessLogics
{
    public class BLTempPOI
    {
        public void Save(List<TempPOI> lstDuct)
        {
            new DATempPOI().Save(lstDuct);
        }
        public void InsertPOIIntoMainTable(UploadSummary summary)
        {
            new DATempPOI().InsertPOIIntoMainTable(summary);
        }
    }
}
