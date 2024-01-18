using DataAccess.TempUpload;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
namespace BusinessLogics
{

    public class BLTempCable : BLDataUploader
    {
        DATempCable datempCable;
        public BLTempCable()
        {

            datempCable = new DATempCable();
            datempCable.DataUploaderNotificationHandler += NotifyUpdatedStatus;

        }
        public void Save(List<TempCable> lstCable)
        {
            datempCable.Save(lstCable);
        }

        public List<ErrorMessage> InsertCableIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }
        public List<TempCable> GetAll(int uploadId)
        {
            return datempCable.GetAll(uploadId);
        }
        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempCable> lstTempADB = GetAll(summary.id);
            int totalBatch = lstTempADB.Select(x => x.batch_id).Distinct().Count();
            List<int> lstBatch = Enumerable.Range(1, totalBatch).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            int currentStatus = 0;
            int CurrentValue = 0;
            DateTime dtStarts = DateTimeHelper.Now;
            var exceptions = new ConcurrentQueue<Exception>();
            string EstimatedRemainingTime;
            ParallelLoopResult result = Parallel.ForEach(lstBatch, options, (batchId) =>
            {
                try
                {
                    lock (lockObj)
                    {
                        datempCable.InsertCableIntoMainTable(summary, batchId);
                        Interlocked.Increment(ref currentStatus);
                        CurrentValue = (currentStatus * 100) / totalBatch;
                        EstimatedRemainingTime = CalculateEstimatedRemainingTime(dtStarts, totalBatch, currentStatus);
                        summary.status_message = "Processing..." + CurrentValue + "% Remaining Time: " + EstimatedRemainingTime;
                        if (CurrentValue == 100) { summary.status_message = "Uploaded Successfully"; }
                        UpdateStatus(summary);
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
            return lstErrorMessage;
        }
    }

}
