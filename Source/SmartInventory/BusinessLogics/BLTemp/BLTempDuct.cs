using DataAccess;
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
    public class BLTempDuct : BLDataUploader
    {
        DATempDuct datempDuct;
        public BLTempDuct()
        {
            datempDuct = new DATempDuct();
            datempDuct.DataUploaderNotificationHandler += NotifyUpdatedStatus;
        }
        public void Save(List<TempDuct> lstDuct)
        {
            datempDuct.Save(lstDuct);
        }
        public List<TempDuct> GetAll(int UploadId)
        {
            return datempDuct.GetAll(UploadId);
        }
        public List<ErrorMessage> InsertDuctIntoMainTable(UploadSummary summary)
        {
            //datempDuct.InsertDuctIntoMainTable(summary);
            return ParallelInsert(summary);
        }
        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempDuct> lstTempADB = GetAll(summary.id);
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
                        datempDuct.InsertDuctIntoMainTable(summary, batchId);
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
