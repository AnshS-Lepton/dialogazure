using System.Collections.Generic;
using Models;
using DataAccess;
using Models.TempUpload;
namespace BusinessLogics
{
    public class BLTempPoe
    {
        public void Save(List<TempPoe> lstTempPoe)
        {
            new DATempPoe().Save(lstTempPoe);
        }

        public void InsertPOEIntoMainTable(UploadSummary summary)
        {
            new DATempPoe().InsertPOEIntoMainTable(summary);
        }
    }
}
