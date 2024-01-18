using Models;

namespace DataUploader
{
    public interface IUploadStatus
    {
        void UpdateStatus(UploadSummary summary);
        int GetSuccessCount(EntityType entityType, UploadSummary summary);
    }
}
