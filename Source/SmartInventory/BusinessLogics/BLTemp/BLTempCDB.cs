using DataAccess;
using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace BusinessLogics
{
    public class BLTempCDB : BLDataUploader
    {
        DATempCDB datempCDB;
        public BLTempCDB()
        {
            datempCDB = new DATempCDB();
            datempCDB.DataUploaderNotificationHandler += NotifyUpdatedStatus;

        }
        public void Save(List<TempCDB> lstCDB)
        {
            datempCDB.Save(lstCDB);
        }

        public List<ErrorMessage> InsertCDBIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }

        public List<TempCDB> GetAll(int uploadId)
        {
            return datempCDB.GetAll(uploadId);
        }

        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempCDB> lstTempCDB = GetAll(summary.id);
            int totalBatch = lstTempCDB.Select(x => x.batch_id).Distinct().Count();
            List<int> lstBatch = Enumerable.Range(1, totalBatch).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            int currentStatus = 0;
            int CurrentValue = 0;
            DateTime dtStarts = DateTimeHelper.Now;
            var exceptions = new ConcurrentQueue<Exception>();
            var daTemplate = new DATempCDB();
            string EstimatedRemainingTime;
            daTemplate.DataUploaderNotificationHandler += NotifyUpdatedStatus;
            ParallelLoopResult result = Parallel.ForEach(lstBatch, options, (batchId) =>
            {
                try
                {
                    lock (lockObj)
                    {
                        daTemplate.InsertCDBIntoMainTable(summary, batchId);
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
