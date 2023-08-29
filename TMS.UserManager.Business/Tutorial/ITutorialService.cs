using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface ITutorialService
    {

        public ConfigData ConfigData { get; set; }

        Task<IEnumerable<BlobRes>> GetVideoTutorials(bool isOnlyVideos);
        Task<IEnumerable<BlobRes>> GetVideoTutorials();

        Task<DownloadBlobData> DownloadBlob(DownloadBlobReq req);

    }
}
