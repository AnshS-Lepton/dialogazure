using DataAccess;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Data;


namespace BusinessLogics
{
    public class BLTempWallMount : BLDataUploader
    {
        DATempWallMount dATempWallMount;
        public BLTempWallMount()
        {
            dATempWallMount = new DATempWallMount();
            dATempWallMount.DataUploaderNotificationHandler += NotifyUpdatedStatus;

        }

        public void Save(List<TempWallMount> lstWallMount)
        {
            dATempWallMount.Save(lstWallMount);
        }

        public List<ErrorMessage> InsertWallMountIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }
        public List<TempWallMount> GetAll(int uploadId)
        {
            return dATempWallMount.GetAll(uploadId);
        }
        
        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempWallMount> lstTemp = GetAll(summary.id);
            int totalBatch = lstTemp.Select(x => x.batch_id).Distinct().Count();
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
                        dATempWallMount.InsertWallMountIntoMainTable(summary, batchId);
                        Interlocked.Increment(ref currentStatus);
                        CurrentValue = (currentStatus * 100) / totalBatch;
                        EstimatedRemainingTime = CalculateEstimatedRemainingTime(dtStarts, totalBatch, currentStatus);
                        summary.status_message = "Processing..." + CurrentValue + "% Remaining Time: " + EstimatedRemainingTime;
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
