using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;
using System.Data;

namespace BusinessLogics
{
    public class BLFileUploader
    {
        public bool SaveExternalFileDtl(ExternalDataUploader obj,int user_id)
        {
            return new DAExternalFileUploader().SaveExternalFileDtl(obj,user_id);
        }
        public List<ExternalDataFileList> GetExternalDataFileDetails(ExternalDataFilter objExtnlDtaFilter,int user_id)
        {
            return new DAExternalFileUploader().GetExternalDataFileDetails(objExtnlDtaFilter, user_id);
        }
        public ExternalDataUploader getDownloadFileDetails(int fileid)
        {
            return new DAExternalFileUploader().getDownloadFileDetails(fileid);
        }
        public bool getUpdatedFileSize(ExternalDataUploader obj)
        {
            return new DAExternalFileUploader().getUpdatedFileSize(obj);
        }
    }
    public class BLExternalFileUploader
    {
        public int DeleteExtrnlDataFileDtl(int fileid)
        {
            return new DAExternalFileUploader().DeleteExtrnlDataFileDtl(fileid);
        }
    }
}
