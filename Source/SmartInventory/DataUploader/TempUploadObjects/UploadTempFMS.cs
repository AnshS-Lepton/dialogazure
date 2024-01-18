using BusinessLogics;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utility;

namespace DataUploader
{
    public class UploadTempFMS : UploadExcel
    {
        private readonly BLDataUploader bLDataUploader;
        private readonly TempToMainTable tempToMainTable;

        public UploadTempFMS()
        {
            bLDataUploader = new BLDataUploader();
            tempToMainTable = new TempToMainTable();
            bLDataUploader.DataUploaderNotifyEventHandler += NotifyUpdateStatus;
        }
        public override ErrorMessage UploadData(DataTable dataTable, UploadSummary summary)
        {
            try
            {
                ErrorMessage status = DataValidator.ValidateExcel(summary, dataTable);
                if (status.status == StatusCodes.OK.ToString())
                {
                    List<TempFMS> lstEntity = DataValidator.GetObjectList<TempFMS>(dataTable,summary);
                    lstEntity.ForEach(m => { m.upload_id = summary.id; m.created_by = summary.user_id; });
                    return ParallelInsert(BulkUploadBatchCount, lstEntity, summary);
                }
                return status;
            }
            catch (Exception ex)
            {
                return tempToMainTable.UpdateStatusAndGetError("UploadTempADB", "Upload Data", ex, summary);

            }
        }
        private ErrorMessage ParallelInsert(int TotalBatchCount, List<TempFMS> lstFMS, UploadSummary summary)
        {
            ErrorMessage errorMessage = new ErrorMessage();
            List<List<TempFMS>> lstChunks = lstFMS.ToChunks(TotalBatchCount).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            int currentStatus = 0;
            int CurrentValue = 0;
            DateTime dtStarts = DateTimeHelper.Now;
            var exceptions = new ConcurrentQueue<Exception>();
            BLTempFMS blTempPole = new BLTempFMS();
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
                        blTempPole.Save(item);
                        Interlocked.Increment(ref currentStatus);
                        CurrentValue = (currentStatus * 100) / lstChunks.Count;
                        EstimatedRemainingTime = BLDataUploader.CalculateEstimatedRemainingTime(dtStarts, lstChunks.Count, currentStatus);
                        summary.status_message = "Uploading..." + CurrentValue + "% Remaining Time: " + EstimatedRemainingTime;
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
