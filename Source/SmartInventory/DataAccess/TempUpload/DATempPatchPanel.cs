using DataAccess.DBHelpers;
using Models.TempUpload;
using System;
using System.Collections.Generic;
namespace DataAccess
{
    public class DATempPatchPanel : Repository<TempPatchPanel>
    {
        public void Save(List<TempPatchPanel> lstTempPatchPanel)
        {

            repo.Insert(lstTempPatchPanel);
        }

        public int InsertPatchPanelIntoMainTable(Models.UploadSummary summary, int batchId)
        {
            var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_PatchPanel", new { p_uploadid = summary.id, p_batchid = batchId }, false);
            return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
        }

        public List<TempPatchPanel> GetAll(int uploadId)
        {
            return (List<TempPatchPanel>)repo.GetAll(x => x.upload_id == uploadId && x.is_valid == true);
        }
    }
}
