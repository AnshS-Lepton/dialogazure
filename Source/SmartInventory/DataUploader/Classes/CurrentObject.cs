using Models;
using System.Data;
namespace DataUploader
{
    public class CurrentExcelObject
    {
        private readonly UploadExcel _iDataUploader;  
        public CurrentExcelObject(UploadExcel iDataUploader)
        {
            _iDataUploader = iDataUploader;
        }
        public ErrorMessage UploadData(DataTable dt, UploadSummary summary)
        {
            return _iDataUploader.UploadData(dt, summary);

        } 
        public bool IsObjectCreated()
        {
            if (_iDataUploader == null)
                return false;
            else
                return true;

        }
    } 
}
