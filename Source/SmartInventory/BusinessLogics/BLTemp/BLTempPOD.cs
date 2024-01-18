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

    public class BLTempPOD : BLDataUploader
    {
        DATempPOD daTempObject;
        public BLTempPOD()
        {
            daTempObject = new DATempPOD();
            daTempObject.DataUploaderNotificationHandler += NotifyUpdatedStatus;
        }
        public void Save(List<TempPOD> objTemp)
        {
            daTempObject.Save(objTemp);
        }

        public List<ErrorMessage> InsertPopIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }
        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempPOD> lstTemp = GetAll(summary.id);
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
                        daTempObject.InsertPopIntoMainTable(summary, batchId);
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

        private List<TempPOD> GetAll(int id)
        {
            return daTempObject.GetAll(id);
        }
    }
}
