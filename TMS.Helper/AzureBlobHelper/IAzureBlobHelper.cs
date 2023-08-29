using Azure.Storage.Blobs.Models;
using TMS.Entity;

namespace TMS.Helper
{
    public interface IAzureBlobHelper
    {
        Task<Tuple<string, IEnumerable<BlobItem>>> ListContainerBlobs(string containerName, int? segmentSize);

        Task<DownloadBlobData> DownloadBlob(string containerName, string filePath);

        Task<IEnumerable<BlobContainerItem>> ListContainers(int? segmentSize);
    }
}
