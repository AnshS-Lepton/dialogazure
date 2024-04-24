using BusinessLogics;
using Models.TempUpload;
using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;
using BusinessLogics.BLTemp;

namespace DataUploader
{
    public class UploadTempCDBAttributes : UploadExcel
    {
        private readonly TempToMainTable tempToMainTable;
        private readonly BLDataUploader bLDataUploader;
        public UploadTempCDBAttributes()
        {
            bLDataUploader = new BLDataUploader();
            bLDataUploader.DataUploaderNotifyEventHandler += NotifyUpdateStatus;
            tempToMainTable = new TempToMainTable();
        }
        public override ErrorMessage UploadData(DataTable dataTable, UploadSummary summary)
        {
            try
            {
                List<Mapping> lstMapping = bLDataUploader.GetMappingsForCDBCable(EntityType.Cable.ToString());
                ErrorMessage status = DataValidator.ValidateData(summary, dataTable, lstMapping);
                if (status.status == StatusCodes.OK.ToString())
                {
                    List<TempCDBAttributes> lstEntity = DataValidator.ConvertDataTableToListForCDB<TempCDBAttributes>(dataTable, summary);
                    lstEntity.ForEach(m => { m.upload_id = summary.id; m.created_by = summary.user_id; });
                    return ParallelInsert(lstEntity, summary);
                }
                return status;
            }
            catch (Exception ex)
            {
                return tempToMainTable.UpdateStatusAndGetError("UploadTempCDBAttributes", "Upload Data", ex, summary);
            }
        }
        private ErrorMessage ParallelInsert(List<TempCDBAttributes> lstTemp, UploadSummary summary)
        {
            ErrorMessage errorMessage = new ErrorMessage();
            List<List<TempCDBAttributes>> lstChunks = lstTemp.ToChunks(BulkUploadBatchCount).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            int currentStatus = 0;
            int CurrentValue = 0;
            DateTime dtStarts = DateTimeHelper.Now;
            var exceptions = new ConcurrentQueue<Exception>();
            BLTempCDBAttributes blTempcdb = new BLTempCDBAttributes();
            int batchCount = 0;
            string EstimatedRemainingTime;
            ParallelLoopResult result = Parallel.ForEach(lstChunks, options, (item) =>
            {
                try
                {
                    lock (lockObj)
                    {
                        batchCount++;
                        item.ForEach(m => m.batch_id = batchCount);
                        blTempcdb.Save(item);
                        Interlocked.Increment(ref currentStatus);
                        CurrentValue = (currentStatus * 100) / lstChunks.Count;
                        EstimatedRemainingTime = BLDataUploader.CalculateEstimatedRemainingTime(dtStarts, lstChunks.Count, currentStatus);
                        summary.status_message = BLDataUploader.EstimatedRemainingTimeMessage(CurrentValue, EstimatedRemainingTime);
                        if (CurrentValue == 100)
                            summary.status_message = ConstantsKeys.FINALIZING;
                        bLDataUploader.UpdateStatus(summary);
                    }
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }

            });
            if (exceptions.Count > 0)
            {
                return GetAggregateException(exceptions, summary);
            }
            errorMessage.status = StatusCodes.OK.ToString();
            return errorMessage;
        }
    }
}
