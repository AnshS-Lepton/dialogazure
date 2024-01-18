using DataAccess;
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
    public class BLTempBDB : BLDataUploader
    {
        DATempBDB daTempBDB;
        public BLTempBDB()
        {
            daTempBDB = new DATempBDB();
            daTempBDB.DataUploaderNotificationHandler += NotifyUpdatedStatus;

        }
        public void Save(List<TempBDB> lstBDB)
        {
            daTempBDB.Save(lstBDB);
        }

        public List<ErrorMessage> InsertBDBIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }

        public List<TempBDB> GetAll(int uploadId)
        {
            return daTempBDB.GetAll(uploadId);
        }

        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempBDB> lstTempBDB = GetAll(summary.id);
            int totalBatch = lstTempBDB.Select(x => x.batch_id).Distinct().Count();
            List<int> lstBatch = Enumerable.Range(1, totalBatch).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            int currentStatus = 0;
            int CurrentValue = 0;
            DateTime dtStarts = DateTimeHelper.Now;
            var exceptions = new ConcurrentQueue<Exception>();
            BLTempBDB blTempBDB = new BLTempBDB();
            var daTemplate = new DATempBDB();
            daTemplate.DataUploaderNotificationHandler += NotifyUpdatedStatus;
            string EstimatedRemainingTime;
            ParallelLoopResult result = Parallel.ForEach(lstBatch, options, (batchId) =>
            {
                try
                {
                    lock (lockObj)
                    {
                        daTemplate.InsertBDBIntoMainTable(summary, batchId);
                        Interlocked.Increment(ref currentStatus);
                        CurrentValue = (currentStatus * 100) / totalBatch;
                        EstimatedRemainingTime = CalculateEstimatedRemainingTime(dtStarts, totalBatch, currentStatus);
                        summary.status_message = "Processing..." + CurrentValue + "% Remaining Time: " + EstimatedRemainingTime;
                        //if (CurrentValue == 100)
                        //    summary.status_message = "Finalizing...";
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
