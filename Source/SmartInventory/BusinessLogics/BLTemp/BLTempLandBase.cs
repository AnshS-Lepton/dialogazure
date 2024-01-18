using DataAccess.TempUpload;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogics.BLTemp
{
    public class BLTempLandBase : BLDataUploader
    {
        DATempLandBase daTemplate;
        public BLTempLandBase()
        {
            daTemplate = new DATempLandBase();
            daTemplate.DataUploaderNotificationHandler += NotifyUpdatedStatus;
        }
        public void Save(List<TempLandBase> lstLandBase)
        {
            daTemplate.Save(lstLandBase);
        }

        public List<ErrorMessage> InsertLandBaseIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }
        public List<TempLandBase> GetAll(int uploadId)
        {
            return daTemplate.GetAll(uploadId);
        }
        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempLandBase> lstTemp = GetAll(summary.id);
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
                        daTemplate.InsertLandBaseIntoMainTable(summary, batchId);
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
