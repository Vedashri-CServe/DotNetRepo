using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TMS.Entity;

namespace TMS.Helper
{
    public class AzureBlobHelper : IAzureBlobHelper
    {

        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobHelper(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Tuple<string, IEnumerable<BlobItem>>> ListContainerBlobs(string containerName, int? segmentSize)
        {
            try
            {
                List<BlobItem> blobItems = new();
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var absoluteUri = blobContainerClient.Uri.AbsoluteUri;
                var resultSegment = blobContainerClient.GetBlobsAsync().AsPages(default, segmentSize);

                await foreach (Page<BlobItem> blobPage in resultSegment)
                {
                    blobItems.AddRange(blobPage.Values);
                }

                return Tuple.Create(absoluteUri, blobItems.AsEnumerable());
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<DownloadBlobData> DownloadBlob(string containerName, string filePath)
        {
            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = blobContainerClient.GetBlobClient(filePath);
                var blobProperties = await blobClient.GetPropertiesAsync();
                return new DownloadBlobData()
                {
                    BlobStream = await blobClient.OpenReadAsync(),
                    ContentType = blobProperties.Value.ContentType,
                    FileName = blobClient.Name
                };
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<BlobContainerItem>> ListContainers(int? segmentSize)
        {
            try
            {
                List<BlobContainerItem> blobContainerItems = new();
                var resultSegment =
                    _blobServiceClient.GetBlobContainersAsync().AsPages(default, segmentSize);

                await foreach (Page<BlobContainerItem> containerPage in resultSegment)
                {
                    blobContainerItems.AddRange(containerPage.Values);
                }
                return blobContainerItems;
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
