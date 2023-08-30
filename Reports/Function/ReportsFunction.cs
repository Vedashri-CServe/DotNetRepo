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

namespace Reports
{
    public class ReportsFunction : BaseHelper
    {
        private readonly IReportService _reportService;

        public ReportsFunction(IReportService reportService, IConfigurationService configurationService, IAccessTokenProvider tokenProvider) : base(configurationService, tokenProvider)
        {
            _reportService = reportService;
        }

        [FunctionName(nameof(GetCpaReport))]
        public async Task<IActionResult> GetCpaReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/cpaReport")] HttpRequest req,
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
                _reportService.ConfigData = config.configData;
                var response = await _reportService.GetCpaReport(request);

                if (request.IsDownload)
                {
                    var res = response as CPAReportsDownloadRes;
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
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }


        [FunctionName(nameof(GetClientReport))]
        public async Task<IActionResult> GetClientReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/clientReport")] HttpRequest req,
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

                var request = await ModelCast.Request<ClientReportReq>(req.Body);
                _reportService.ConfigData = config.configData;
                var response = await _reportService.GetClientReport(request);
                if (request.IsDownload)
                {
                    var res = response as ClietReportsDownloadRes;
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
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }


        [FunctionName(nameof(GetActualPlannedReport))]
        public async Task<IActionResult> GetActualPlannedReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/actualPlannedReport")] HttpRequest req,
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

                var request = await ModelCast.Request<ActualPlannedReportReq>(req.Body);
                _reportService.ConfigData = config.configData;
                var response = await _reportService.GetActualPlannedReport(request);
                if (request.IsDownload)
                {
                    var res = response as APReportsDownloadRes;
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
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }


        [FunctionName(nameof(GetActivityReport))]
        public async Task<IActionResult> GetActivityReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/activityReport")] HttpRequest req,
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

                var request = await ModelCast.Request<ActivityReportReq>(req.Body);
                _reportService.ConfigData = config.configData;
                var response = await _reportService.GetActivityReport(request);
                if (request.IsDownload)
                {
                    var res = response as ActivityReportsDownloadRes;
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
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }


        /// <summary>
        /// Other Reports contain -> User report, Timesheet report, Worklogs report, Workload report
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(GetOtherReport))]
        public async Task<IActionResult> GetOtherReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/otherReport")] HttpRequest req,
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

                var request = await ModelCast.Request<OtherReportReq>(req.Body);
                _reportService.ConfigData = config.configData;
                var response = await _reportService.GetOtherReport(request);
                
                if (request.IsDownload)
                {
                    var res = response as OtherReportsDownloadRes;
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
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }

        [FunctionName(nameof(GetKRAReport))]
        public async Task<IActionResult> GetKRAReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/kraReport")] HttpRequest req,
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
                        var request = await ModelCast.Request<KRAReqVM>(req.Body);
                        _reportService.ConfigData = config.configData;
                        var response = await _reportService.GetKRAReport(request);
                        if (request.IsDownload)
                        {
                            var res = response as KRAReportsDownloadRes;
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

        [FunctionName(nameof(GetAutoManualReport))]
        public async Task<IActionResult> GetAutoManualReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/autoManualReport")] HttpRequest req,
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
                        var request = await ModelCast.Request<AutoManualReportReq>(req.Body);
                        _reportService.ConfigData = config.configData;
                        var response = await _reportService.GetAutoManualReport(request);
                        if (request.IsDownload)
                        {
                            var res = response as AutoManualReportsDownloadRes;
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


        [FunctionName(nameof(GetLoginLogoutReport))]
        public async Task<IActionResult> GetLoginLogoutReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/loginLogoutReport")] HttpRequest req,
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

                var request = await ModelCast.Request<LoginLogoutReportReq>(req.Body);
                _reportService.ConfigData = config.configData;
                var response = await _reportService.GetLoginLogoutReport(request);

                if (request.IsDownload)
                {
                    var res = response as LoginLogoutReportsDownloadRes;
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
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }

        [FunctionName(nameof(GetAuditReport))]
        public async Task<IActionResult> GetAuditReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/auditReport")] HttpRequest req,
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

                var request = await ModelCast.Request<AuditReportReq>(req.Body);

                // validate date range
                if(request.StartDate != null)
                {
                    var startDate = Convert.ToDateTime(request.StartDate);
                    if(request.EndDate == null)
                        request.EndDate = new System.DateTime(startDate.Year, startDate.Month, System.DateTime.DaysInMonth(startDate.Year, startDate.Month));
                    else
                    {
                        var endDate = Convert.ToDateTime(request.EndDate);
                        var dateSpan = endDate.Month - startDate.Month;
                        if(dateSpan > 3 || dateSpan < 0 ) 
                            return InvalidRequest();
                    }
                }
                _reportService.ConfigData = config.configData;
                var response = await _reportService.GetAuditReport(request);

                if (request.IsDownload)
                {
                    var res = response as AuditReportDownloadRes;
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
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }
    }
}
