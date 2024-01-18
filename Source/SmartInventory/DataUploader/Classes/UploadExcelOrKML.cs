using Models;
using System.Data;
using System.Web;
using BusinessLogics;
using Utility;

namespace DataUploader
{
    public class UploadExcelOrKML
    {
        //private UploadStatus uploadStatus;

        public delegate void NotificationEventHandlerUploadExcelOrKML(dynamic data);

        public event NotificationEventHandlerUploadExcelOrKML UploadExcelOrKMLEvent;
        //BLUploadSummary blUploadSummary;
        BLDataUploader bLDataUploader;
        public UploadExcelOrKML()
        {
            bLDataUploader = new BLDataUploader();
            bLDataUploader.DataUploaderNotifyEventHandler += NotifyUploadStatus;
            //uploadStatus =  UploadStatus.Instance;
            //uploadStatus.UploadStatusEvent += NotifyUploadStatus;
        }

        //private static UploadExcelOrKML objUploadExcelOrKML = null;
        //private static readonly object lockObject = new object();
        //public static UploadExcelOrKML Instance
        //{
        //    get
        //    {
        //        lock (lockObject)
        //        {
        //            if (objUploadExcelOrKML == null)
        //            {
        //                objUploadExcelOrKML = new UploadExcelOrKML();
        //            }
        //        }
        //        return objUploadExcelOrKML;
        //    }
        //}

        public ErrorMessage UploadExcelorKML(UploadSummary summary, HttpPostedFileBase file, string fname, EntityType EnumEntityType, DataTable dataTable)
        {
            try
            {

            //Upload KML
            //if (file.FileName.Contains("kml"))
            //{
            //    IUploadKML _lineuploader = ObjectFactory.GetInstance(EnumEntityType);
            //    summary.status_message = ConstantsKeys.UPLOADING_KML;
            //    summary.execution_type = ConstantsKeys.UPLOAD;
            //    bLDataUploader.UpdateStatus(summary);
            //    return _lineuploader.UploadKML(fname, summary);
            //}
            //else
            //{

                UploadExcel obj = ObjectFactory.GetInstance(EnumEntityType);
                obj.UploadStatusEvent += NotifyUploadStatus;
                CurrentExcelObject currentExcelObj = new CurrentExcelObject(obj);

                //Upload EXCEL files
                //CurrentExcelObject currentExcelObj = new CurrentExcelObject(ObjectFactory.GetInstance(EnumEntityType));
                if (currentExcelObj.IsObjectCreated())
                {
                    summary.status_message = ConstantsKeys.UPLOADING_EXCEL;
                    summary.execution_type = ConstantsKeys.UPLOAD;
                    bLDataUploader.UpdateStatus(summary);
                    ErrorMessage message = currentExcelObj.UploadData(dataTable, summary);
                    return message;
                }
                else
                {
                    return ObjectNotCreatedException();
                } 
                //} 
            }
            catch (System.Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadExcelOrKML", "UploadExcelorKML", ex);
                throw;
            }
        }

        private ErrorMessage ObjectNotCreatedException()
        {
            ErrorMessage statusMessage = new ErrorMessage();
            statusMessage.error_code = "101";
            statusMessage.error_msg = "Operation failed due to enum not defined";
            statusMessage.is_valid = false;
            return statusMessage;
        }

        public void NotifyUploadStatus(dynamic data)
        {
            if (UploadExcelOrKMLEvent != null)
                UploadExcelOrKMLEvent.Invoke(data);
        } 

    }
}
