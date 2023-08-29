using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TMS.Auth;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.BaseHelper;
using TMS.UserManager.Business;

namespace UserManager
{
    public class UserFunction : BaseHelper
    {
        private readonly IUserService _userService;

        public UserFunction(IConfigurationService configurationService, IAccessTokenProvider tokenProvider, IUserService userService) : base(configurationService, tokenProvider)
        {
            _userService = userService;
        }

        [FunctionName(nameof(GetUserList))]
        public async Task<IActionResult> GetUserList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/list")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<UserListFilterVM>(req.Body);
                        _userService.ConfigData = config.configData;
                        request.UserId = config.configData.UserId;
                        var response = await _userService.GetUserList(request);

                        if (request.IsDownload)
                        {
                            var res = response as UserListDownloadRes;
                            return new FileContentResult(res.ByteArray, res.ContentType)
                            {
                                FileDownloadName = $"{res.FileName}",
                            };
                        }
                        else
                        {
                            return OkResponse(response);
                        }
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(SaveUser))]
        public async Task<IActionResult> SaveUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/save")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<UserVM>(req.Body);
                        _userService.ConfigData = config.configData;
                        var response = await _userService.SaveUser(request);
                        if (response > 0)
                        {
                            //update user id
                            request.UserId = response;
                            return OkResponse(request);
                        }
                        else if (response == -1)
                        {
                            return ErrorResponse(MessageHelper.UserExists);
                        }
                        else if (response == -2)
                        {
                            return ErrorResponse(MessageHelper.SelectCPA);
                        }
                        else
                        {
                            return ErrorResponse(MessageHelper.SaveError);
                        }
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(DeleteUser))]
        public async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/delete")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<UserIdVM>(req.Body);
                        _userService.ConfigData = config.configData;
                        var response = await _userService.DeleteUser(request.UserId);
                        if (response)
                            return OkResponse(true);
                        else
                            return ErrorResponse(MessageHelper.DeleteError);
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(GetUserTypeDropdown))]
        public async Task<IActionResult> GetUserTypeDropdown(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/usertypedropdown")] HttpRequest req,
           ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _userService.ConfigData = config.configData;
                        return OkResponse(await _userService.GetUserRoleList());
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(GetDepartmentDropdown))]
        public async Task<IActionResult> GetDepartmentDropdown(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/departmentdropdown")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _userService.ConfigData = config.configData;
                        return OkResponse(await _userService.GetDepartmentDropdown());
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(InActiveAccount))]
        public async Task<IActionResult> InActiveAccount([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/inactiveaccount")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<InActiveAccountVM>(req.Body);
                        _userService.ConfigData = config.configData;
                        var response = await _userService.InActiveAccount(request);
                        if (!response)
                        {
                            return ErrorWithResponse(MessageHelper.SomethingWentWrong);
                        }
                        return OkResponse(response);
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(GetReportingManagerDropdown))]
        public async Task<IActionResult> GetReportingManagerDropdown(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/getreportingmanagerdropdown")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _userService.ConfigData = config.configData;
                        return OkResponse(await _userService.GetReportingManagerDropdown());
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(GetUserMenuList))]
        public async Task<IActionResult> GetUserMenuList(
     [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/getusermenulist")] HttpRequest req,
     ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<UserRoleIdVM>(req.Body);
                        _userService.ConfigData = config.configData;                          
                        return OkResponse(await _userService.GetUserMenuList(request));
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }
        [FunctionName(nameof(UpdateMenuList))]
        public async Task<IActionResult> UpdateMenuList(
                    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/updatemenulist")] HttpRequest req,
                    ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<UpdateMenuReqVM>(req.Body);
                        _userService.ConfigData = config.configData;
                        long response = await _userService.UpdateMenuList(request);
                        if (response == request.PermissionId)
                            return OkResponse(true);
                        else if (response == -1)
                        {
                            return ErrorResponse(MessageHelper.SelectDefaultLandingPage);
                        }                                  
                        else
                            return ErrorResponse(MessageHelper.UpdatePermissionError);
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(SaveRoleTypeForPermission))]
        public async Task<IActionResult> SaveRoleTypeForPermission(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/saveroletypeforpermission")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<RoleTypeVM>(req.Body);
                        _userService.ConfigData = config.configData;
                        var response = await _userService.SaveRoleTypeForPermission(request);
                        if(response > 0)
                            return OkResponse(response);
                        else if(response == -5)
                            return ErrorResponse(MessageHelper.RoleExists);
                        return ErrorResponse(MessageHelper.SaveError);
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(DeleteRoleType))]
        public async Task<IActionResult> DeleteRoleType(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/deleteroletype")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<UserRoleIdVM>(req.Body);
                        _userService.ConfigData = config.configData;
                        var response = await _userService.DeleteRoleType(request);
                        if (response > 0)
                            return OkResponse(request);
                        else if (response == -5)
                            return ErrorResponse(MessageHelper.UserForRoleExists);
                        else
                            return ErrorResponse(MessageHelper.SaveError);
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(GetPermissionRoleList))]
        public async Task<IActionResult> GetPermissionRoleList(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/getpermissionrolelist")] HttpRequest req,
           ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _userService.ConfigData = config.configData;
                        return OkResponse(await _userService.GetPermissionRoleList());
                    }
                    else
                    {
                        return InvalidRequest();
                    }
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(GetUsersForDropdown))]
        public async Task<IActionResult> GetUsersForDropdown(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/usersForDropdown")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (!result.IsValid)
                {
                    return new UnauthorizedResult();
                }

                var status = await config.SetConfigData(result.Username);
                if (status != 0)
                {
                    return InvalidRequest();
                }

                var request = await ModelCast.Request<CpaReportReq>(req.Body);
                _userService.ConfigData = config.configData;
                return OkResponse(await _userService.GetUsersForDropdown());

            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }
    }
}