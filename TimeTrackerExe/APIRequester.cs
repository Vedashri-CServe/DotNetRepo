using System.Configuration;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;


namespace TimeTrackerExe
{
    public static class ResponseStatuses
    {
        public const string Success = "Success";
        public const string Failure = "Failure";
    }

    public static class UrlConstants
    {
        public static string Token = ConfigurationManager.ConnectionStrings["UserManagementURl"].ConnectionString +  "auth/token";
        public static string GetWorkPlanList = ConfigurationManager.ConnectionStrings["WorklogsURl"].ConnectionString + "workplan/getworkplanlist";
        public static string GetUserDetail = ConfigurationManager.ConnectionStrings["UserManagementURl"].ConnectionString + "auth/getuserdetail";
        public static string SaveTimeLog = ConfigurationManager.ConnectionStrings["WorklogsURl"].ConnectionString + "task/savetimelog";
        public static string GetWorkPlanComment = ConfigurationManager.ConnectionStrings["WorklogsURl"].ConnectionString + "workplan/getworkplancomment";
        public static string GetCheckList = ConfigurationManager.ConnectionStrings["WorklogsURl"].ConnectionString + "workplan/getchecklist";
        public static string SaveIdleTime = ConfigurationManager.ConnectionStrings["WorklogsURl"].ConnectionString + "task/saveidletime";
        public static string GetLastEventTypeByUserId = ConfigurationManager.ConnectionStrings["WorklogsURl"].ConnectionString + "task/getlasteventtypebyuserid";
        public static string SaveLogoutUser = ConfigurationManager.ConnectionStrings["UserManagementURl"].ConnectionString + "auth/logoutuserandworkplan";
        public static string GetExeSetupLocation = ConfigurationManager.ConnectionStrings["HelpURl"].ConnectionString + "tutorial/getsetup";
    }

    public static class MessageConstants
    {
        public const string LogSynced = "Time logs saved successfully!";
        public const string TaskRunning = "Please pause or stop any running task and try again!";
    }

    public enum RequestType
    {
        POST,
        DELETE,
        GET,
        PUT
    }

    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string source, string toCheck)
        {
            return source?.Equals(toCheck ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }

    public class TMSHttpRequest
    {
        public string RequestType { get; set; }
        public string Url { get; set; }
        public int Timeout { get; set; } = -1;
        public Dictionary<string, string>? Headers;
        public string Body { get; set; }
    }

    public class TMSHttpResponse
    {

        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public Exception Exception { get; set; }
    }

    public class APIRequester
    {

        static async Task<string> Decompress(Stream stream)
        {
            using (var mStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (var reader = new StreamReader(mStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
        private readonly HttpClient _httpClient;
        private readonly int _defaultTimeOut = 6000000;
        public APIRequester()
        {
            var clientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.None };
            _httpClient = new HttpClient(clientHandler);
        }
        public async Task<TMSHttpResponse> ExecuteRequest(TMSHttpRequest request)
        {
            using (var requestMessage = new HttpRequestMessage(new HttpMethod(request.RequestType), request.Url))
            {
                var isRequestCompressed =
                    requestMessage.Headers.TryAddWithoutValidation("Content-Encoding", new List<string>() { "gzip" });

                requestMessage.Content = (isRequestCompressed)
                    ? (HttpContent)new StreamContent(
                        new GZipStream(new MemoryStream(Encoding.UTF8.GetBytes(request.Body)),
                            CompressionLevel.Fastest))
                    : new StringContent(request.Body);
                if (request.Headers != null)
                {
                    foreach (var key in request.Headers.Keys)
                    {
                        var val = request.Headers[key];
                        switch (key.ToLower())
                        {
                            case "authorization":
                                var spl = val.Split(' ');
                                if (spl.Length == 2)
                                {
                                    requestMessage.Headers.Authorization =
                                        new AuthenticationHeaderValue(spl[0], spl[1]);
                                }
                                else
                                {
                                    requestMessage.Headers.TryAddWithoutValidation("Authorization", val);
                                }

                                break;
                            case "connection":
                                requestMessage.Headers.Connection.Add(val);
                                break;
                            case "accept":
                                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(val));
                                break;
                            case "content-type":
                                requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(val);
                                break;
                            default:
                                requestMessage.Headers.TryAddWithoutValidation(key, request.Headers[key]);
                                break;
                        }
                    }
                }

                var isResponseCompressed =
                    requestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", new List<string>() { "gzip" });

                var cancellationToken = new CancellationTokenSource((request.Timeout != -1) ? request.Timeout : _defaultTimeOut);
                try
                {
                    using (var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken.Token))
                    {
                        return new TMSHttpResponse
                        {
                            StatusCode = (int)responseMessage.StatusCode,
                            Headers = (from item in responseMessage.Headers
                                       select new
                                       {
                                           Key = item.Key,
                                           Value = string.Join(" ", item.Value)
                                       }).ToDictionary(i => i.Key, i => i.Value),
                            Body = (isResponseCompressed && responseMessage.Content.Headers.Any(x => x.Key.EqualsIgnoreCase("content-encoding") && x.Value.Any(y => y.EqualsIgnoreCase("gzip"))))
                                ? await (Decompress(await responseMessage.Content.ReadAsStreamAsync()))
                                : await responseMessage.Content.ReadAsStringAsync()
                        };
                    }
                }
                catch (TaskCanceledException e)
                {
                    IdleDataManager.ErrorToFile(e, IdleDataManager.fileType.system_log);
                    return new TMSHttpResponse()
                    {
                        StatusCode = 408,
                        Exception = e
                    };
                }
            }
        }
    }
}
