using BusinessLogics.BLTemp;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utility;
using BusinessLogics;
namespace DataUploader.TempUploadObjects
{

    internal class UploadTempBuilding : UploadExcel
    {
        private readonly ItemSpecification itemSpecification;
        private UploadTempBuilding()
        {
            itemSpecification = new ItemSpecification();
        }

        private static UploadTempBuilding objUploadTempBuilding = null;
        private static readonly object lockObject = new object();
        public static UploadTempBuilding Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objUploadTempBuilding == null)
                    {
                        objUploadTempBuilding = new UploadTempBuilding();
                    }
                }
                return objUploadTempBuilding;
            }
        }

        public override ErrorMessage UploadData(DataTable dataTable, UploadSummary summary)
        {
            try
            {
                ErrorMessage status = DataValidator.ValidateExcel(summary, dataTable);
                if (status.status == StatusCodes.OK.ToString())
                {
                    List<TempBuilding> lstTempBuildingList = DataValidator.GetObjectList<TempBuilding>(dataTable,summary);
                    lstTempBuildingList.ForEach(m => m.upload_id = summary.id);
                    lstTempBuildingList.ForEach(m => m.created_by = summary.user_id);

                    ParallelInsert(BulkUploadBatchCount, lstTempBuildingList, summary);
                }
                return status;
            }
            catch (Exception ex)
            {
               
                ErrorLogHelper.WriteErrorLog("UploadBuildingData()", "Library", ex);
                //return tempToMainTable.UpdateStatusAndGetError("UploadTempBuilding", "Upload Data", ex, summary);
                return null;
            }
        }

        private ErrorMessage ParallelInsert(int TotalBatchCount, List<TempBuilding> lstBuilding, UploadSummary summary)
        {

            //List<List<TempBuilding>> lstChunks = lstBuilding.ToChunks<TempBuilding>(TotalBatchCount).ToList();
            //var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            //object lockObj = new object();
            //int currentStatus = 0;
            //int CurrentValue = 0;
            //DateTime dtStarts = DateTimeHelper.Now;
            //var exceptions = new ConcurrentQueue<Exception>();
            //BusinessLogics.BLTemp.BLTempBuilding blTempBuilding = new BusinessLogics.BLTemp.BLTempBuilding();
            //UploadStatus uploadStatus = UploadStatus.Instance;

            //ParallelLoopResult result = Parallel.ForEach(lstChunks, options, (item) =>
            //{
            //    try
            //    {
            //        //List<TempPole> objListPole = item;
            //        blTempBuilding.Save(item);
            //        lock (lockObj)
            //        {
            //            Interlocked.Increment(ref currentStatus);
            //            CurrentValue = (currentStatus * 100) / lstChunks.Count;
            //            summary.status_message = "Uploading..." + CurrentValue + "%";
            //            new BLDataUploader().UpdateStatus(summary);
            //            //EstimatedRemainingTime = CalculateEstimatedRemainingTime(dtStarts, Totalcount, currentStatus);
            //            //RemainingItems = (Totalcount - currentStatus);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        exceptions.Enqueue(e);
            //    }

            //}); 
            ErrorMessage errorMessage = new ErrorMessage();
            List<List<TempBuilding>> lstChunks = lstBuilding.ToChunks(TotalBatchCount).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            int currentStatus = 0;
            int CurrentValue = 0;
            DateTime dtStarts = DateTimeHelper.Now;
            var exceptions = new ConcurrentQueue<Exception>();
            BusinessLogics.BLTemp.BLTempBuilding blTempBuilding = new BusinessLogics.BLTemp.BLTempBuilding();
            int batchCount = 0;
            string EstimatedRemainingTime;
            ParallelLoopResult result = Parallel.ForEach(lstChunks, options, (item) =>
            {
                try
                {
                    lock (lockObj)
                    {
                        batchCount++;
                        item.ForEach(m => m.batch_id = batchCount);
                        blTempBuilding.Save(item);
                        Interlocked.Increment(ref currentStatus);
                        CurrentValue = (currentStatus * 100) / lstChunks.Count;
                        EstimatedRemainingTime = BLDataUploader.CalculateEstimatedRemainingTime(dtStarts, lstChunks.Count, currentStatus);
                        summary.status_message = "Uploading..." + CurrentValue + "% Remaining Time: " + EstimatedRemainingTime;
                        if (CurrentValue == 100)
                            summary.status_message = ConstantsKeys.FINALIZING;
                        blTempBuilding.UpdateStatus(summary);
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
            errorMessage.status = StatusCodes.OK.ToString();
            return errorMessage;
        }
    }
}
