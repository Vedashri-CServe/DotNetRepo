namespace TMS.Entity
{
    public class DownloadBlobData
    {
        public Stream? BlobStream { get; set; }

        public string? ContentType { get; set; }

        public string? FileName { get; set; }
    }
}
