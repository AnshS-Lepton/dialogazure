using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using DataAccess.TempUpload;

namespace BusinessLogics.BLTemp
{
    public class BLTempOpticalrepeater : BLDataUploader
    {
        DATempOpticalRepeater daTempObject;
        public BLTempOpticalrepeater()
        {
            daTempObject = new DATempOpticalRepeater();
            daTempObject.DataUploaderNotificationHandler += NotifyUpdatedStatus;
        }
        public void Save(List<TempOpticalRepeater> lstOpticalRepeater)
        {
            daTempObject.Save(lstOpticalRepeater);
        }
        public List<TempOpticalRepeater> GetAll(int UploadId)
        {
            return daTempObject.GetAll(UploadId);
        }
        public List<ErrorMessage> InsertOpticalRepeaterIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);

        }
        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempOpticalRepeater> lstTemp = GetAll(summary.id);
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
                        daTempObject.InsertOpticalRepeaterIntoMainTable(summary, batchId);
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
