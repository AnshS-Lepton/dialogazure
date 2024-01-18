using DataAccess;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLTempONT : BLDataUploader
    {
        DATempONT daTempObject;

        public BLTempONT()
        {
            daTempObject = new DATempONT();
            daTempObject.DataUploaderNotificationHandler += NotifyUpdatedStatus;

        }
        public void Save(List<TempONT> lstONT)
        {
            daTempObject.Save(lstONT);
        }

        public List<ErrorMessage> InsertONTIntoMainTable(UploadSummary summary)
        {
            return ParallelInsert(summary);
        }
        public List<TempONT> GetAll(int uploadId)
        {
            return daTempObject.GetAll(uploadId);
        }

        private List<ErrorMessage> ParallelInsert(UploadSummary summary)
        {
            List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
            List<TempONT> lstTemp = GetAll(summary.id);
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
                        daTempObject.InsertONTIntoMainTable(summary, batchId);
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

        public bool validateShaftFloorinfo(string p_shaft_name, string p_floor_name, string p_parent_network_id)
        {
            return daTempObject.validateShaftFloorinfo( p_shaft_name,  p_floor_name,  p_parent_network_id);
        }
    }
}
