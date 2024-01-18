
using Models;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using Utility;

namespace DataUploader
{
    public abstract class UploadExcel
    {
        public static int BulkUploadBatchCount = Convert.ToInt32(ConfigurationManager.AppSettings["BulkUploadBatchCount"]);
        public delegate void NotificationEventHandleStatus(dynamic data);
        public event NotificationEventHandleStatus UploadStatusEvent;
        public abstract ErrorMessage UploadData(DataTable dt, UploadSummary summary);
         
        public void NotifyUpdateStatus(dynamic result)
        {
            if (UploadStatusEvent != null)
            {
                UploadStatusEvent.Invoke(result);
            }
        }
        public ErrorMessage GetAggregateException(ConcurrentQueue<Exception> exceptions, UploadSummary summary)
        {
            ErrorMessage errorMessage = new ErrorMessage();
            errorMessage.status = StatusCodes.FAILED.ToString();
            string message = string.Empty;
            var exception = new AggregateException(exceptions);
            foreach (var e in exception.InnerExceptions)
            {
                message += e.GetType().Name + ":" + e.Message + "#";
                ErrorLogHelper.WriteErrorLog(summary.entity_type, summary.entity_type, e, summary);
            }
            errorMessage.error_msg = message;
            return errorMessage;
        }
    }
}
