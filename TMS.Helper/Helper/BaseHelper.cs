using Microsoft.AspNetCore.Mvc;
using TMS.Auth;
using TMS.Entity;
using TMS.Helper;

namespace TMS.Helper.BaseHelper
{
    public class BaseHelper
    {
        public readonly IConfigurationService config;
        public readonly IAccessTokenProvider tokenProvider;

        public BaseHelper(IConfigurationService configurationService, IAccessTokenProvider tokenProvider)
        {
            config = configurationService;
            this.tokenProvider = tokenProvider;
        }

        #region Response Utility
        public IActionResult OkResponse(object data = null)
        {
            return new ObjectResult(new ApiResponse { ResponseStatus = "Success", ResponseData = data });
        }
        public IActionResult ExceptionErrorResponse(object data = null)
        {
            return new ObjectResult(new ApiResponse { ResponseStatus = "Failure", ErrorData = new ApiError { ErrorDetail = data } });
        }
        public IActionResult ErrorResponse(string error = "", object data = null)
        {
            return new ObjectResult(new ApiResponse { ResponseStatus = "Failure", Message = error, ErrorData = new ApiError { ErrorDetail = data, Error = error } });
        }
        public IActionResult ErrorWithResponse(ApiError error)
        {
            return new ObjectResult(new ApiResponse { ResponseStatus = "Failure", ErrorData = error });
        }
        public IActionResult InvalidRequest()
        {
            return new ObjectResult(new ApiResponse { ResponseStatus = "Failure", Message = "invalid request!", ErrorData = new ApiError { Error = "invalid request!" } });
        }
        #endregion
    }
}
