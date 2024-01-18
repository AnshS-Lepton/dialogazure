using BusinessLogics;
using Models;
using System;

namespace DataUploader
{
    public class UploadStatus : IUploadStatus
    {

        public delegate void NotificationEventHandlerUploadStatus(dynamic data);

        public event NotificationEventHandlerUploadStatus UploadStatusEvent;
        private static UploadStatus objUploadStatus = null;
        private static readonly object lockObject = new object();
        public static UploadStatus Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objUploadStatus == null)
                    {
                        objUploadStatus = new UploadStatus();
                        BLUploadSummary.Instance.BLUploadSummayEvent += objUploadStatus.NotifyUploadStatus;
                    }
                }
                return objUploadStatus;
            }
        }

        private UploadStatus()
        {

        }

        public void UpdateStatus(UploadSummary summary)
        {

            BLUploadSummary.Instance.UpdateStatus(summary);
        }
        public int GetSuccessCount(EntityType entityType, UploadSummary summary)
        {
            return new BLMisc().GetCount(entityType, summary);
        }
        public void UpdateFailedStatus(UploadSummary summary, ErrorMessage status)
        {
            summary.status = status.status.ToString();// StatusCodes.FAILED.ToString();
            summary.status_message = status.error_msg;
            summary.end_on = DateTime.Now;
            summary.success_record = 0;
            summary.err_description = status.error_msg;
            summary.failed_record = summary.total_record;
            BLUploadSummary.Instance.UpdateStatus(summary);
        }

        public void NotifyUploadStatus(dynamic data)
        {
            if (UploadStatusEvent != null)
                UploadStatusEvent.Invoke(data);
        }
    }
}
