using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Auth;
using TMS.Entity;
using TMS.Helper;
using TMS.UserManager.Business;
using TMS.Helper.BaseHelper;

namespace Worklogs
{
    public class TaskLogFunction : BaseHelper
    {
        private readonly ITimeLogService _TimeLogService;
        public TaskLogFunction(IAccessTokenProvider tokenProvider, ITimeLogService TimeLogService, IConfigurationService configurationService) : base(configurationService, tokenProvider)
        {
            _TimeLogService = TimeLogService;
        }

        [FunctionName(nameof(SaveTimeLog))]
        public async Task<IActionResult> SaveTimeLog(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task/savetimelog")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<List<TimeLogReqVM>>(req.Body);
                        _TimeLogService.ConfigData = config.configData;
                        var response = await _TimeLogService.SaveTimeLog(request);
                        if (response == -1)
                        {
                            return ErrorResponse(MessageHelper.SaveError);
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

        [FunctionName(nameof(StartContinueLogTimer))]
        public async Task<IActionResult> StartContinueLogTimer(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task/startcontinuelog")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<TimeLogReqVM>(req.Body);
                        _TimeLogService.ConfigData = config.configData;
                        request.EventTime = DateTime.UtcNow;  //take event time as current datetime
                        if (request.EventType == 11 || request.EventType == 13)
                        {
                            var saveTimeLog = new List<TimeLogReqVM> { request };

                            var response = await _TimeLogService.SaveTimeLog(saveTimeLog);
                            if (response == -1)
                            {
                                return ErrorResponse(MessageHelper.SaveError);
                            }
                            //update event status in daily work plan table
                            var eventStatus = await _TimeLogService.SaveEventType(request.WorkPlanId, true);
                            if (eventStatus < 0) { return ErrorResponse(MessageHelper.SaveError); }
                            return OkResponse(response);
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

        [FunctionName(nameof(StopWorkLogTimer))]
        public async Task<IActionResult> StopWorkLogTimer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task/stopworklog")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<TimeLogReqVM>(req.Body);
                        _TimeLogService.ConfigData = config.configData;
                        request.EventTime = DateTime.UtcNow; //take event time as current datetime
                        if (request.EventType == 12)
                        {
                            var response = await _TimeLogService.SaveTimeDuration(request);
                            if (response == null)
                            {
                                return ErrorResponse(MessageHelper.SaveError);
                            }
                            // update event status in daily work plan table 
                          //  var eventStatus = await _TimeLogService.SaveEventType(request.WorkPlanId, false);
                          //  if (eventStatus < 0) { return ErrorResponse(MessageHelper.SaveError); }
                            return OkResponse(response);
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

        [FunctionName(nameof(SaveIdleTime))]
        public async Task<IActionResult> SaveIdleTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task/saveidletime")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<IdleTimeVM>(req.Body);
                        _TimeLogService.ConfigData = config.configData;

                        var response = await _TimeLogService.SaveIdleTime(request);
                        if (response < 0)
                        {
                            return ErrorResponse(MessageHelper.SaveError);
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
            catch(Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(GetLastEventTypeByUserId))]
        public async Task<IActionResult> GetLastEventTypeByUserId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task/getlasteventtypebyuserid")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<long>(req.Body);
                        _TimeLogService.ConfigData = config.configData;

                        var response = await _TimeLogService.GetLastBreakTypeByUserId(request);
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
            catch(Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }


        [FunctionName(nameof(RefreshTimeDuration))]
        public async Task<IActionResult> RefreshTimeDuration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task/refreshtimeduration")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<RefreshTimeDuration>(req.Body);
                        request.UserId = config.configData.UserId;
                        request.TimelineDate = DateTime.UtcNow;  //take event time as current datetime
                        _TimeLogService.ConfigData = config.configData;
                        var response = await _TimeLogService.GetRefreshTimeDuration(request);

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
            catch(Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(StartStopUserBreak))]
        public async Task<IActionResult> StartStopUserBreak(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task/startstopuserbreak")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<IdleTimeVM>(req.Body);
                        _TimeLogService.ConfigData = config.configData;
                        request.CreatedOn = DateTime.UtcNow;
                        if (request.EventType == 15 || request.EventType == 16)
                        {
                            request.UserId = config.configData.UserId;
                            if(request.EventType == 15) { request.StartTime = request.CreatedOn; }
                            else request.EndTime = request.CreatedOn;

                            var response = await _TimeLogService.SaveBreakTime(request);
                            if (response < 0)
                            {
                                return ErrorResponse(MessageHelper.SaveError);
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
