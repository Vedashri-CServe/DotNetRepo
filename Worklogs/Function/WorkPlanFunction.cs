using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Auth;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.BaseHelper;
using TMS.UserManager.Business;

namespace Worklogs
{
    public class WorkPlanFunction : BaseHelper
    {
        private readonly IWorkPlanService _WorkPlanService;
        private readonly IUserHelperService _userService;
        private readonly ITimeLogService _timeLogService;
        private readonly IProcessService _processService;
        public WorkPlanFunction(IAccessTokenProvider tokenProvider, IWorkPlanService WorkPlanService, IConfigurationService configurationService, IUserHelperService userService, ITimeLogService timeLogService, IProcessService processService) : base(configurationService, tokenProvider)
        {
            _WorkPlanService = WorkPlanService;
            _userService = userService;
            _timeLogService = timeLogService;
            _processService = processService;
        }

        [FunctionName(nameof(GetClientAndTaskAndProcessListByCPA))]
        public async Task<IActionResult> GetClientAndTaskAndProcessListByCPA([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/getclientandtaskandprocesslistByCPA")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<CPAReqVM>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        return OkResponse(await _WorkPlanService.ClientAndTaskAndProcessListByCPA(request.CPAId));
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

        [FunctionName(nameof(GetWorkPlanStatusList))]
        public async Task<IActionResult> GetWorkPlanStatusList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "workplan/getworkplanstatuslist")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _WorkPlanService.ConfigData = config.configData;
                        return OkResponse(await _userService.GetLookupDropdownOpts(LookupShortDesc.WORKPLANSTATUS));
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

        [FunctionName(nameof(SaveWorkPlan))]
        public async Task<IActionResult> SaveWorkPlan(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/saveworkplan")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<WorkPlanReqVM>(req.Body);
                        var isEdit = request.WorkPlanId > 0 ? true : false;
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.SaveWorkPlan(request);
                        if (response > 0)
                        {
                            //update Work Plan id
                            request.WorkPlanId = response;

                            //get process activity list as checklist for workplan
                            if (request.ProcessId != null && request.ProcessId != 0 && isEdit == false)
                            {
                                var process = await _processService.GetProcessById((long)request.SubprocessId);
                                var checklist = JsonConvert.DeserializeObject<List<ActivityName>>(process.ActivityName.ToString());

                                List<CheckListVM> checkLists = new List<CheckListVM>();
                                foreach (var item in checklist)
                                {
                                    var workplanchecklist = new CheckListVM
                                    {
                                        Id = 0,
                                        Description = item.value.ToString(),
                                        IsChecked = false,
                                        IsDeleted = false,
                                        WorkPlanId = request.WorkPlanId
                                    };
                                    var saved = await _WorkPlanService.SaveCheckList(workplanchecklist);

                                    if (saved < 0) { return ErrorResponse(MessageHelper.SaveError); }
                                }
                            }
                            return OkResponse(request);
                        }
                        else if (response == -1)
                        {
                            return ErrorResponse(MessageHelper.DuplicateRecord);
                        }
                        else if(response == -2)
                        {
                            return ErrorResponse(MessageHelper.InvalidTime);
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

        [FunctionName(nameof(GetWorkPlanList))]
        public async Task<IActionResult> GetWorkPlanList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/getworkplanlist")] HttpRequest req,
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
                        var request = await ModelCast.Request<WorkPlanFilterVM>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                       var response = await _WorkPlanService.GetWorkPlanList(request);
                        if (request.IsDownload)
                        {
                            var resp = response as WorkPlanListDownloadRes;
                            return new FileContentResult(resp.ByteArray, resp.ContentType)
                            {
                                FileDownloadName = $"{resp.FileName}",
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

        [FunctionName(nameof(DeleteWorkPlan))]
        public async Task<IActionResult> DeleteWorkPlan([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/deleteworkplan")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<WorkPlanResultVM>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.DeleteWorkPlan(request.WorkPlanId);
                        if (response > 0)
                        {
                            return OkResponse(true);
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

        [FunctionName(nameof(SaveCheckList))]
        public async Task<IActionResult> SaveCheckList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/savechecklist")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<CheckListVM>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.SaveCheckList(request);
                        if (response > 0)
                        {
                            //update Check List id
                            request.Id = response;
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

        [FunctionName(nameof(GetCheckList))]
        public async Task<IActionResult> GetCheckList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/getchecklist")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<checkListReqId>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        return OkResponse(await _WorkPlanService.GetCheckList(request));
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

        [FunctionName(nameof(DeleteCheckListPoint))]
        public async Task<IActionResult> DeleteCheckListPoint([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/deletechecklistpoint")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<DeleteCheckList>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.DeleteCheckList(request);
                        if (response > 0)
                        {
                            return OkResponse(true);
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

        [FunctionName(nameof(WorkPlanComment))]
        public async Task<IActionResult> WorkPlanComment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/workplancomment")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<WorkPlanComment>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.AddWorkPlanComment(request);
                        if (response > 0)
                        {
                            //update Work Plan id
                            request.Id = response;
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

        [FunctionName(nameof(GetWorkPlanComment))]
        public async Task<IActionResult> GetWorkPlanComment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/getworkplancomment")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<checkListReqId>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        return OkResponse(await _WorkPlanService.GetComment(request));
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

        [FunctionName(nameof(SaveRecurringPlan))]
        public async Task<IActionResult> SaveRecurringPlan(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/saverecurringplan")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<RecurringVM>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.SaveRecurringPlan(request);
                        if (response > 0)
                        {
                            //update recurring Plan id
                            request.RecurringId = response;
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

        [FunctionName(nameof(GetWorkPlanRecurring))]
        public async Task<IActionResult> GetWorkPlanRecurring(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/getworkplanrecurring")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<WorkPlanResultVM>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        return OkResponse(await _WorkPlanService.GetRecurringDeatials(request));
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

        [FunctionName(nameof(DeleteRecurringPlan))]
        public async Task<IActionResult> DeleteRecurringPlan([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/deleterecurringplan")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<SaveRecurringResp>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.DeleteRecurringPlan(request);
                        if (response > 0)
                        {
                            return OkResponse(true);
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


        [FunctionName(nameof(GetTimeLogDetailsList))]
        public async Task<IActionResult> GetTimeLogDetailsList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/gettimeloglist")] HttpRequest req, ILogger log)
        {

            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<TimeLogFilterVM>(req.Body);
                        request.UserId = config?.configData?.UserId ?? 0;
                        _timeLogService.ConfigData = config.configData;
                        var resp = await _timeLogService.GetTimeLogList(request);
                        if (request.IsDownload)
                        {
                            var res = resp as TimeLogsDownloadRes;
                            return new FileContentResult(res.ByteArray, res.ContentType)
                            {
                                FileDownloadName = $"{res.FileName}"
                            };
                        }
                        else
                        {
                            return OkResponse(resp);
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


        [FunctionName(nameof(GetIdleDuration))]
        public async Task<IActionResult> GetIdleDuration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/getidleduration")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<IdleTimeFilterVM>(req.Body);
                        _timeLogService.ConfigData = config.configData;

                        var IdleTime = new
                        {
                            IdleTime = await _timeLogService.GetIdleDuration(request)
                        };
                        return OkResponse(IdleTime);
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

        [FunctionName(nameof(ApproveWorkPlan))]
        public async Task<IActionResult> ApproveWorkPlan([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/approveworkplan")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<ApprovedWorkPlanVM>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.ApproveWorkPlan(request);
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

        [FunctionName(nameof(ModifiedReviewLogs))]
        public async Task<IActionResult> ModifiedReviewLogs([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/modifiedreviewlogs")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<UpdateReviewLogsVM>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        return OkResponse(await _WorkPlanService.ModifiedReviewLogs(request));
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

        [FunctionName(nameof(GetReviewerLogsList))]
        public async Task<IActionResult> GetReviewerLogsList([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workplan/getreviewerlogslist")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<ReviewerLogsReqVm>(req.Body);
                        _WorkPlanService.ConfigData = config.configData;
                        var response = await _WorkPlanService.GetReviewerLogsList(request);
                        if (request.IsDownload)
                        {
                            var res = response as ReviewerLogsDownloadRes;
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

        [FunctionName(nameof(GetEmployeeDropdown))]
        public async Task<IActionResult> GetEmployeeDropdown(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "workplan/getemployeedropdown")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _WorkPlanService.ConfigData = config.configData;
                        return OkResponse(await _WorkPlanService.GetEmployeeList());
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


        [FunctionName(nameof(GetApprovalStatusDropdown))]
        public async Task<IActionResult> GetApprovalStatusDropdown(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "workplan/getapprovalstatusdropdown")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        _WorkPlanService.ConfigData = config.configData;
                        return OkResponse(await _WorkPlanService.GetApprovalStatusList());
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
