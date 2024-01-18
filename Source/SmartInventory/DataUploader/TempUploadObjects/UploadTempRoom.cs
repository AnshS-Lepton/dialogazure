using BusinessLogics;
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
using Models.TempUpload;

namespace DataUploader.TempUploadObjects
{
   internal class UploadTempRoom:UploadExcel
    {
        private readonly TempToMainTable tempToMainTable;
        private readonly BLDataUploader bLDataUploader;

        public UploadTempRoom() 
        {
            bLDataUploader = new BLDataUploader();
            bLDataUploader.DataUploaderNotifyEventHandler += NotifyUpdateStatus;
            tempToMainTable = new TempToMainTable();
        }
        public override ErrorMessage UploadData(DataTable dataTable, UploadSummary summary)
        {
            try 
            {
                ErrorMessage status = DataValidator.ValidateExcel(summary, dataTable);
                if (status.status == StatusCodes.OK.ToString())
                {
                    List<TempRoom> lstEntity = DataValidator.GetObjectList<TempRoom>(dataTable, summary);
                    lstEntity.ForEach(m => { m.upload_id = summary.id; m.created_by = summary.user_id; });
                    return ParallelInsert(BulkUploadBatchCount, lstEntity, summary);
                }
                return status;
            }
            catch (Exception ex)
            {
                return tempToMainTable.UpdateStatusAndGetError("UploadTempRoom", "Upload Data", ex, summary);

            }
        }
        private ErrorMessage ParallelInsert(int TotalBatchCount, List<TempRoom> lstroom, UploadSummary summary)
        {
            ErrorMessage errorMessage = new ErrorMessage();
            List<List<TempRoom>> lstChunks = lstroom.ToChunks(TotalBatchCount).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            int currentStatus = 0;
            int CurrentValue = 0;
            DateTime dtStarts = DateTimeHelper.Now;
            var exceptions = new ConcurrentQueue<Exception>();
            BusinessLogics.BLTemp.BLTempRoom blTempRoom = new BusinessLogics.BLTemp.BLTempRoom();
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
                        blTempRoom.Save(item);
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
