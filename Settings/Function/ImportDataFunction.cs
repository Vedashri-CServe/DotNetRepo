using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Auth;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.BaseHelper;
using TMS.UserManager.Business;

namespace Settings
{
    public class ImportDataFunction : BaseHelper
    {
        private readonly IImportService _ImportService;

        public ImportDataFunction(IAccessTokenProvider tokenProvider, IConfigurationService configurationService, IImportService importService) : base(configurationService, tokenProvider)
        {
            _ImportService = importService;
        }

        [FunctionName(nameof(ProjectsImport))]
        public async Task<IActionResult> ProjectsImport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/projectexcel")] HttpRequest req,
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
                        var request = await ModelCast.Request<List<ProjectReqVM>>(req.Body);
                        _ImportService.ConfigData = config.configData;

                        ProjectRespVM response = await _ImportService.ImportProjectList(request);   
                        
                        if (response.Response == -1)
                        {
                            return ErrorResponse(MessageHelper.InvalidExcelData,response.ProjectReqVMs);
                        }                           
                        else if (response.Response > 0)
                        {
                            return ErrorResponse(MessageHelper.DuplicateValueInExcel);
                        }
                        return OkResponse(true);
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

        [FunctionName(nameof(DownloadInvalidRecodes))]
        public async Task<IActionResult> DownloadInvalidRecodes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/DownloadInvalidRecodes")] HttpRequest req,
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
                        var request = await ModelCast.Request<List<ProjectReqVM>>(req.Body);
                        _ImportService.ConfigData = config.configData;

                        ProjectDownloadResp response = await _ImportService.DownloadInvalidRecodes(request);
                        return new FileContentResult(response.ByteArray, response.ContentType)
                        {
                            FileDownloadName = $"{response.FileName}",
                        };

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

        [FunctionName(nameof(ImportClientsExcel))]
        public async Task<IActionResult> ImportClientsExcel(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/clientexcel")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<List<ImportClientsVM>>(req.Body);
                        _ImportService.ConfigData = config.configData;
                        var response = await _ImportService.ImportClientsExcel(request);
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

        [FunctionName(nameof(ImportTaskExcel))]
        public async Task<IActionResult> ImportTaskExcel(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/taskexcel")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var request = await ModelCast.Request<List<ImportTaskVM>>(req.Body);
                        _ImportService.ConfigData = config.configData;
                        var response = await _ImportService.ImportTaskExcel(request);
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

        [FunctionName(nameof(ImportUserExcel))]
        public async Task<IActionResult> ImportUserExcel(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/userexcel")] HttpRequest req,
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

                var request = await ModelCast.Request<List<ImportUserExcelData>>(req.Body);
                _ImportService.ConfigData = config.configData;
                var response = await _ImportService.ImportUserExcel(request);
                return OkResponse(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }


        [FunctionName(nameof(ProcessImport))]
        public async Task<IActionResult> ProcessImport(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/processexcel")] HttpRequest req,
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
                        var request = await ModelCast.Request<List<ProcessReqVM>>(req.Body);
                        _ImportService.ConfigData = config.configData;

                        var response = await _ImportService.ImportProcessList(request);
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
    }
}
