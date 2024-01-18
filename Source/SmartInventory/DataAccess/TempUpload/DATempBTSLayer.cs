using System.Collections.Generic;
using Models.TempUpload;
using DataAccess.DBHelpers;
using Models;
using System;

namespace DataAccess
{
  public  class DATempBTSLayer : Repository<TempBtsLayer>
    {
        public void Save(List<TempBtsLayer> lstBTSLayer)
        {
            repo.Insert(lstBTSLayer);
        }

        public void InsertBTSLayerIntoMainTable(UploadSummary summary)
        {
            
            repo.ExecuteSQLCommand(string.Format("select * from fn_bulkupload_batch_los_bts({0},{1})", summary.id, "'Wireless'"));
        }
    }
}
