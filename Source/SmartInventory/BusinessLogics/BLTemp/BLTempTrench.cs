using DataAccess;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BusinessLogics
{
    public class BLTempTrench : BLDataUploader
    {
        DATempTrench daTempObject;
        public void Save(List<TempTrench> lstTrench)
        {
            daTempObject.Save(lstTrench);
        }
        public BLTempTrench()
        {
            daTempObject = new DATempTrench();
            daTempObject.DataUploaderNotificationHandler += NotifyUpdatedStatus;
        }

        public List<ErrorMessage> InsertTrenchIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }

        public List<TempTrench> GetAll(int uploadId)
        {
            return daTempObject.GetAll(uploadId);
        }
        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempTrench> lstTemp = GetAll(summary.id);
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
                        daTempObject.InsertTrenchIntoMainTable(summary, batchId);
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
