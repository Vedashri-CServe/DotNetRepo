namespace TMS.Entity
{

    public class BlobRes
    {
        public string? FileName { get; set; }

        public string? ContentType { get; set; }

        public string? Url { get; set; }

    }

    public class DownloadBlobReq
    {
        public string? FileName { get; set; }
    }

    public class DownloadExeSetup
    {
        public bool isOnlyVideos { get; set; } = false;
    }
}
