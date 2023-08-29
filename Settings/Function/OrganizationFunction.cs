using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TMS.Helper;
using TMS.Entity;
using TMS.Auth;
using TMS.UserManager.Business;
using TMS.Helper.BaseHelper;

namespace Settings
{
    public class OrganizationFunction : BaseHelper
    {
        private readonly IOrganizationUserService _OrganizationUserService;
        private readonly IUserHelperService _userService;
        public OrganizationFunction(IAccessTokenProvider tokenProvider, IOrganizationUserService OrgUserService, IConfigurationService configurationService, IUserHelperService userService) : base(configurationService, tokenProvider)
        {
            _OrganizationUserService = OrgUserService;
            _userService = userService;
        }

        [FunctionName(nameof(OrganizationList))]
        public async Task<IActionResult> OrganizationList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/organizationuser/list")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<OrgUserFilterVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        request.UserId = config.configData.UserId;
                       var response = await _OrganizationUserService.GetOrganizationUserList(request);
                        if (request.IsDownload)
                        {
                            var res = response as ClientAndProjecDownloadRes;
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

        [FunctionName(nameof(SaveOrganizationUser))]
        public async Task<IActionResult> SaveOrganizationUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/organizationuser/save")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<OrgUserVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        var response = await _OrganizationUserService.SaveOrganizationUser(request);
                        if (response > 0)
                        {
                            //update user id
                            request.UserId = response;
                            return OkResponse(request);
                        }
                        else if (response == -1)
                        {
                            return ErrorResponse(MessageHelper.ProjectsExists);
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

        [FunctionName(nameof(DeleteOrganizationUser))]
        public async Task<IActionResult> DeleteOrganizationUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/organizationuser/delete")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<SaveOrgResultVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        var response = await _OrganizationUserService.DeleteOrganizationUser(request.UserId);
                        if (response > 0)
                        {
                            return OkResponse(true);
                        }
                        else if (response == -1)
                        {
                            return ErrorResponse(MessageHelper.ActiveCPA);
                        }
                        else
                        {
                            return ErrorResponse(MessageHelper.DeleteError);
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

        [FunctionName(nameof(GetClientByCPA))]
        public async Task<IActionResult> GetClientByCPA([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/organizationuser/getclientbyCPA")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<SaveOrgResultVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        return OkResponse(await _OrganizationUserService.ClientByCPA(request.UserId));
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


        [FunctionName(nameof(GetCPAList))]
        public async Task<IActionResult> GetCPAList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/organizationuser/getcpalist")] HttpRequest req,
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
                        var request = await ModelCast.Request<UserCPAVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        return OkResponse(await _OrganizationUserService.GetCPAList(request));
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

        [FunctionName(nameof(SaveTask))]
        public async Task<IActionResult> SaveTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/savetask")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<TaskVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        var response = await _OrganizationUserService.CreateTask(request);
                        if (response > 0)
                        {
                            //update Task id
                            request.TaskId = response;
                            return OkResponse(request);
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

        [FunctionName(nameof(DeleteTask))]
        public async Task<IActionResult> DeleteTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/deleteTask")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<TaskVMResultVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        var response = await _OrganizationUserService.DeleteTask(request.TaskId);
                        if (response)
                        {
                            return OkResponse(true);
                        }

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

        [FunctionName(nameof(GetTaskList))]
        public async Task<IActionResult> GetTaskList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/gettasklist")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<TaskListFilterVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        request.UserId = config.configData.UserId;
                        var response = await _OrganizationUserService.GetTaskList(request);
                        if (request.IsDownload)
                        {
                            var res = response as TaskListDownloadRes;
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


        [FunctionName(nameof(SaveStatus))]
        public async Task<IActionResult> SaveStatus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/savestatus")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<StatusFilterVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        var response = await _OrganizationUserService.SaveStatus(request);
                        if (response > 0)
                        {
                            //update Task id
                            request.StatusId = response;
                            return OkResponse(request);
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

        [FunctionName(nameof(DeleteStatus))]
        public async Task<IActionResult> DeleteStatus([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/deletestatus")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<StatusVMResultVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        var response = await _OrganizationUserService.DeleteStatus(request.Id);
                        if (response)
                        {
                            return OkResponse(true);
                        }

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

        [FunctionName(nameof(GetStatusList))]
        public async Task<IActionResult> GetStatusList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "setting/getstatuslist")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<PaginationMetaVM>(req.Body);
                        _OrganizationUserService.ConfigData = config.configData;
                        return OkResponse(await _OrganizationUserService.GetStatusList(request));
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


        [FunctionName(nameof(GetBillingTypeList))]
        public async Task<IActionResult> GetBillingTypeList(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "setting/getbillingtypelist")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _OrganizationUserService.ConfigData = config.configData;
                        return OkResponse(await _userService.GetLookupDropdownOpts(LookupShortDesc.BILLINGTYPE));
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

        [FunctionName(nameof(GetTypeOfWorkList))]
        public async Task<IActionResult> GetTypeOfWorkList(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "setting/gettypeofworklist")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _OrganizationUserService.ConfigData = config.configData;
                        return OkResponse(await _userService.GetLookupDropdownOpts(LookupShortDesc.TYPEOFWORK));
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
    }
}
