using TMS.Entity;
using TMS.Helper;

namespace TMS.UserManager.Business
{
    public class TutorialService : ITutorialService
    {
        public ConfigData ConfigData { get; set; }

        private readonly IAzureBlobHelper _azureBlobHelper;

        public TutorialService(IAzureBlobHelper azureBlobHelper)
        {
            _azureBlobHelper = azureBlobHelper;
        }

        public async Task<IEnumerable<BlobRes>> GetVideoTutorials(bool isOnlyVideos)
        {
            var result = await _azureBlobHelper.ListContainerBlobs(Constants.AZURE_STORAGE_CONTAINER, default);
            return result.Item2.Select(x => new BlobRes()
            {
                FileName = x.Name,
                ContentType = x.Properties.ContentType,
                Url = $"{result.Item1}/{x.Name}"
            }).Where(i => (isOnlyVideos) ? 
            i.ContentType!.StartsWith("video") : 
            (!i.ContentType!.StartsWith("video")));
        }

        public async Task<IEnumerable<BlobRes>> GetVideoTutorials()
        {
            var result = await _azureBlobHelper.ListContainerBlobs(Constants.AZURE_STORAGE_CONTAINER_GENERAL_FILE, default);
            return result.Item2.Select(x => new BlobRes()
            {
                FileName = x.Name,
                ContentType = x.Properties.ContentType,
                Url = $"{result.Item1}/{x.Name}"
            });
        }

        public async Task<DownloadBlobData> DownloadBlob(DownloadBlobReq req)
        {
            return await _azureBlobHelper.DownloadBlob(Constants.AZURE_STORAGE_CONTAINER, req?.FileName ?? string.Empty);
        }

    }
}
