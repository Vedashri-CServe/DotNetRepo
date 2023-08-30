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

namespace Help
{
    public class TutorialsFunction : BaseHelper
    {

        private readonly ITutorialService _tutorialService;

        public TutorialsFunction(ITutorialService tutorialService, IConfigurationService configurationService, IAccessTokenProvider tokenProvider) : base(configurationService, tokenProvider)
        {
            _tutorialService = tutorialService;
        }

        [FunctionName(nameof(GetAllBlobs))]
        public async Task<IActionResult> GetAllBlobs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tutorial/getAllBlobs")] HttpRequest req,
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
                var d = req.GetQueryParameterDictionary();
                d.TryGetValue("isOnlyVideos", out var isOnlyVideos);
                bool.TryParse(isOnlyVideos, out var yesOnlyVideos);
                _tutorialService.ConfigData = config.configData;
                return OkResponse(await _tutorialService.GetVideoTutorials(yesOnlyVideos));
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }

        [FunctionName(nameof(GetGeneralFile))]
        public async Task<IActionResult> GetGeneralFile(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tutorial/getGeneralFile")] HttpRequest req,
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
                _tutorialService.ConfigData = config.configData;
                return OkResponse(await _tutorialService.GetVideoTutorials());
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }

        [FunctionName(nameof(DownloadBlob))]
        public async Task<IActionResult> DownloadBlob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tutorial/download")] HttpRequest req,
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

                var request = await ModelCast.Request<DownloadBlobReq>(req.Body);
                _tutorialService.ConfigData = config.configData;
                var response = await _tutorialService.DownloadBlob(request);
                return new FileContentResult(response.BlobStream.ToByteArray(), response.ContentType)
                {
                    FileDownloadName = $"{response.FileName}",
                };
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }



        [FunctionName(nameof(GetSetupForExe))]
        public async Task<IActionResult> GetSetupForExe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tutorial/getsetup")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var request = await ModelCast.Request<DownloadExeSetup>(req.Body);
                var config = await _tutorialService.GetVideoTutorials(request.isOnlyVideos);
                return OkResponse(await _tutorialService.GetVideoTutorials(request.isOnlyVideos));
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse(ex);
            }
        }

    }
}
