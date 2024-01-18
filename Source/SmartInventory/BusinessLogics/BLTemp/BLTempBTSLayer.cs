using System.Collections.Generic;
using Models;
using DataAccess;
using Models.TempUpload;

namespace BusinessLogics
{
    public class BLTempBTSLayer
    {
        public void Save(List<TempBtsLayer> lstBTSLayer)
        {
            new DATempBTSLayer().Save(lstBTSLayer);
        }

        public void InsertBTSLayerIntoMainTable(UploadSummary summary)
        {
            new DATempBTSLayer().InsertBTSLayerIntoMainTable(summary);
        }
    }
}
