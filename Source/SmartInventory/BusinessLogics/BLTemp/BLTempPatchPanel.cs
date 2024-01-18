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
    public class BLTempPatchPanel : BLDataUploader
    {
        DATempPatchPanel daTempObject;
        public BLTempPatchPanel()
        {
            daTempObject = new DATempPatchPanel();
            daTempObject.DataUploaderNotificationHandler += NotifyUpdatedStatus;

        }
        public void Save(List<TempPatchPanel> lstTempPatchPanel)
        {
            daTempObject.Save(lstTempPatchPanel);
        }

        public List<ErrorMessage> InsertPatchPanelIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }
        public List<TempPatchPanel> GetAll(int UploadId)
        {
            return daTempObject.GetAll(UploadId);
        }
        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempPatchPanel> lstTempADB = GetAll(summary.id);
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
                        daTempObject.InsertPatchPanelIntoMainTable(summary, batchId);
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
