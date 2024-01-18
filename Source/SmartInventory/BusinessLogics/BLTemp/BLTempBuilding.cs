using DataAccess.TempUpload;
using Models.TempUpload;
using System.Collections.Generic;
using Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Configuration;
using System.Data;

namespace BusinessLogics.BLTemp
{
    public class BLTempBuilding : BLDataUploader
    {
        public static int BulkUploadBatchCount = Convert.ToInt32(ConfigurationManager.AppSettings["BulkUploadBatchCount"]);
        //public static int BulkUploadBatchCount = 20;
        //private static BLTempBuilding objBLTempBuilding = null;
        //private static readonly object lockObject = new object();
        private bool isSubscribed = false;
        DATempBuilding daTemplate;
        //public new static BLTempBuilding Instance
        //{
        //    get
        //    {
        //        lock (lockObject)
        //        {
        //            if (objBLTempBuilding == null)
        //            {
        //                objBLTempBuilding = new BLTempBuilding();

        //            }
        //        }
        //        return objBLTempBuilding;
        //    }
        //}

        public BLTempBuilding()
        {
            daTemplate = new DATempBuilding();
            if (!isSubscribed)
            {
                daTemplate.DataUploaderNotificationHandler += NotifyUpdatedStatus;
                isSubscribed = true;
            }
        }
        public void Save(List<TempBuilding> lstBuilding)
        {
            daTemplate.Save(lstBuilding);
        }

        public List<ErrorMessage> InsertBuildingIntoMainTable(UploadSummary summary)
        {
            //var daTemplate = new DATempBuilding();
            //daTemplate.DataUploaderNotificationHandler += NotifyUpdatedStatus;
            //daTemplate.InsertBuildingIntoMainTable(summary);
          return  ParallelInsert(BulkUploadBatchCount, summary);//TODO Later
        }


        public void Delete(string tempTableName)
        {
            daTemplate.Delete(tempTableName);
        }

        //public void ValidateBuilding(UploadSummary summary, string geomtype)
        //{
        //    var daTemplate = new DATempBuilding();
        //    daTemplate.DataUploaderNotificationHandler += NotifyUpdatedStatus;
        //    daTemplate.Validate(summary, geomtype);
        //}
        public List<TempBuilding> GetAll(int uploadId)
        {
            return daTemplate.GetAll(uploadId);
        }
        public DataTable GetAll(UploadSummary summary)
        {
            return daTemplate.GetAll(summary);
        }

        private List<ErrorMessage> ParallelInsert(int TotalBatchCount, UploadSummary summary)
        {

            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempBuilding> lstTemp = GetAll(summary.id);
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
                        daTemplate.InsertBuildingIntoMainTable(summary, batchId);
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
