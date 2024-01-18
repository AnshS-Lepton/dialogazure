using Models;
using System;

namespace DataUploader
{
    public interface ITempToMainTable
    {
        ErrorMessage UpdateStatusAndGetError(Exception ex, UploadSummary summary);
        ErrorMessage UpdateStatusAndGetError(string controllerName,string actionName, Exception ex, UploadSummary summary);
    }
}
