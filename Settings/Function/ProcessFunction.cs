using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using TMS.Auth;
using TMS.Entity;
using TMS.Helper;
using TMS.UserManager.Business;
using TMS.Helper.BaseHelper;

namespace Settings
{
    public class ProcessFunction : BaseHelper
    {
        private readonly IProcessService _processService;

        public ProcessFunction(IConfigurationService configurationService, IAccessTokenProvider tokenProvider, IProcessService processService) : base(configurationService, tokenProvider)
        {
            _processService = processService;
        }

        [FunctionName(nameof(GetProcessList))]
        public async Task<IActionResult> GetProcessList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/list")] HttpRequest req,
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
                        var request = await ModelCast.Request<ProcessListFilterVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        request.UserId = config.configData.UserId;
                        var response = await _processService.GetProcessList(request);
                        if (request.IsDownload)
                        {
                            var res = response as ProcessListDownloadRes;
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

        [FunctionName(nameof(SaveProcess))]
        public async Task<IActionResult> SaveProcess(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/save")] HttpRequest req,
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
                        var request = await ModelCast.Request<ProcessVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        var response = await _processService.SaveProcess(request);
                        if (response > 0)
                        {
                            //update id
                            request.ProcessId = response;
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

        [FunctionName(nameof(DeleteProcess))]
        public async Task<IActionResult> DeleteProcess(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/delete")] HttpRequest req,
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
                        var request = await ModelCast.Request<ProcessIdVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        var response = await _processService.DeleteProcess(request.ProcessId);
                        if (response > 0)
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

        [FunctionName(nameof(GetSubProcessListByProcess))]
        public async Task<IActionResult> GetSubProcessListByProcess(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/getsubprocesslistbyprocess")] HttpRequest req,
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
                        var request = await ModelCast.Request<SubProcessListFilterVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        return OkResponse(await _processService.GetSubProcessListByProcess(request));
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

        [FunctionName(nameof(GetProcessesByCPA))]
        public async Task<IActionResult> GetProcessesByCPA(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/getCpaProcesses")] HttpRequest req,
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
                        var request = await ModelCast.Request<CpaProcessesReq>(req.Body);
                        _processService.ConfigData = config.configData;
                        return OkResponse(await _processService.GetCPAProcessList(request));
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

        [FunctionName(nameof(SaveProcessesByCPA))]
        public async Task<IActionResult> SaveProcessesByCPA(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/saveCpaProcesses")] HttpRequest req,
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
                        var request = await ModelCast.Request<SaveCpaProcessesReq>(req.Body);
                        _processService.ConfigData = config.configData;
                        return OkResponse(await _processService.SaveCpaProcesses(request));
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


        #region sub-process functions

        [FunctionName(nameof(SaveSubProcess))]
        public async Task<IActionResult> SaveSubProcess(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/savesubprocess")] HttpRequest req,
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
                        var request = await ModelCast.Request<SubProcessVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        var response = await _processService.SaveSubProcess(request);

                        if (response < 0)
                        {
                            return ErrorResponse(MessageHelper.SaveError);
                        }

                        return OkResponse(request);
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

        [FunctionName(nameof(GetSubProcessList))]
        public async Task<IActionResult> GetSubProcessList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/getsubprocesslist")] HttpRequest req,
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
                        var request = await ModelCast.Request<SubProcessListFilterVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        return OkResponse(await _processService.GetSubProcessList(request));
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

        [FunctionName(nameof(DeleteSubProcess))]
        public async Task<IActionResult> DeleteSubProcess(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/deletesubprocess")] HttpRequest req,
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
                        var request = await ModelCast.Request<SubProcessIdVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        var response = await _processService.DeleteSubProcess(request.SubprocessId);
                        if (response > 0)
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
        #endregion

        #region process master functions
        [FunctionName(nameof(GetProcessMasterList))]
        public async Task<IActionResult> GetProcessMasterList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "process/masterlist")] HttpRequest req,
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
                        _processService.ConfigData = config.configData;
                        var response = await _processService.GetProcessMasterList();
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
        #endregion

        #region get only process list
        [FunctionName(nameof(GetOnlyProcessList))]
        public async Task<IActionResult> GetOnlyProcessList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/processlist")] HttpRequest req,
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
                        var request = await ModelCast.Request<ProcessreqVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        var response = await _processService.GetOnlyProcessList(request);
                        if (request.IsDownload)
                        {
                            var res = response as OnlyProcessListDownloadRes;
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
        #endregion

        #region Save And Edit Process
        [FunctionName(nameof(SaveAndEditProcess))]
        public async Task<IActionResult> SaveAndEditProcess(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/saveandeditprocess")] HttpRequest req,
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
                        var request = await ModelCast.Request<SaveAndEditProcessVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        var response = await _processService.SaveAndEditProcess(request);
                        if (response > 0)
                        {
                            //update id
                            request.ProcessId = response;
                            return OkResponse(request);
                        }
                        else if (response == -2)
                        {
                            return ErrorResponse(MessageHelper.ProcessExists);
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
        #endregion

        #region Delete Process With SubProcess
        [FunctionName(nameof(DeleteProcessWithSubProcess))]
        public async Task<IActionResult> DeleteProcessWithSubProcess(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "process/deleteprocesswithsubProcess")] HttpRequest req,
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
                        var request = await ModelCast.Request<ProcessIdVM>(req.Body);
                        _processService.ConfigData = config.configData;
                        var response = await _processService.DeleteProcessWithSubProcess(request.ProcessId);
                        if (response > 0)
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
        #endregion

    }
}
