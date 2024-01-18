using Models;
using System.Collections.Generic;
namespace DataUploader
{
    public interface IUploadKML
    {
        ErrorMessage UploadKML(string strXMLPath,UploadSummary summary);
    }
}
