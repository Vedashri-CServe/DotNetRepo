using System.ComponentModel;

namespace TMS.Entity
{
    public class ApiResponse
    {
        [Description("Web API Response status. True - Success, False - Error")]
        public string ResponseStatus { get; set; }
        public string Message { get; set; }
        [Description("Web API Object to store response data.")]
        public object ResponseData { get; set; }
        public ApiError ErrorData { get; set; }
    }

    public class ApiError
    {
        public string ErrorCode { get; set; }
        public string Error { get; set; }
        public object ErrorDetail { get; set; }
    }
}