using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using TMS.Entity;
using TMS.Auth;
using TMS.Helper;
using TMS.UserManager.Business;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TMS.Helper.BaseHelper;
using Azure;

namespace UserManager
{
    public class SecurityFunction : BaseHelper
    {
        private readonly IUserHelperService _userHelperService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ITimeLogService _timeLogService;

        public SecurityFunction(IAuthService authService, IUserHelperService userHelperService, IConfiguration configuration, IConfigurationService configurationService, IAccessTokenProvider tokenProvider, IUserService userService, ITimeLogService timeLogService) : base(configurationService, tokenProvider)
        {
            _userHelperService = userHelperService;
            _authService = authService;
            _configuration = configuration;
            _userService = userService;
            _timeLogService = timeLogService;
        }

        [FunctionName(nameof(UserAuthentication))]
        public async Task<IActionResult> UserAuthentication(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/token")] HttpRequest req, ILogger log)
        {
            try
            {
                var userCredentials = await ModelCast.Request<UserCredentials>(req.Body);
                if (userCredentials == null)
                {
                    return ErrorResponse(MessageHelper.UsernamePasswordIncorrect);
                }
                var user = await _userHelperService.GetUserByUsername(userCredentials.Username);
                if (user == null)
                {
                    log.LogInformation("user not found");
                    return ErrorResponse(MessageHelper.EmailNotRegistered);
                }
                if (user.IsDeleted)
                {
                    log.LogInformation("user in active");
                    return ErrorResponse(MessageHelper.AccountInactive);
                }
                if (!user.IsEmailVerified)
                {
                    log.LogInformation("user not verified");
                    return ErrorResponse(MessageHelper.AccountNotVerified);
                }
                if (!user.IsAvailable)
                {
                    log.LogInformation("user unavailable");
                    return ErrorResponse(MessageHelper.UserUnavailable);
                }
                var isMatch = PasswordHash.IsPasswordMatchWithHash(userCredentials.Password, user.Password);
                if (isMatch)
                {
                    //TODO: After the match, check for two factor authentication and if it's enabled, send OTP email to user.
                    await _authService.AddLoginLog(new AuthLoginLogVM
                    {
                        HostAddress = req.HttpContext.Connection.RemoteIpAddress.ToString(),
                        UserAgent = req.Headers.UserAgent.ToString(),
                        Source = string.IsNullOrEmpty(req.Headers.UserAgent.ToString()) ? "EXE" : "WEB",
                        UserId = user.UserId,
                        HostName = req.HttpContext.Connection.RemoteIpAddress.ToString()
                    });

                    GenerateJWTToken generateJWTToken = new(_configuration.GetSection("SecretKey").Get<string>());
                    var token = generateJWTToken.IssuingJWT(userCredentials.Username);
                    log.LogInformation("token return");

                    return OkResponse(new
                    {
                        TwoFactorEnabled = user.TwoFactorEnabled,
                        Token = token
                    });

                }
                return ErrorResponse(MessageHelper.UsernamePasswordIncorrect);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(GetUserDetail))]
        public async Task<IActionResult> GetUserDetail(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/getuserdetail")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(req);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        UserRoleIdVM roleId = new UserRoleIdVM();
                        _userService.ConfigData = config.configData;
                        config.configData.Menu = (List<MenuRespVM>)await _userService.GetUserMenuList(roleId);
                        log.LogInformation($"Request received for {result.Username}.");
                        return OkResponse(config.configData);
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

        [FunctionName(nameof(ValidateToken))]
        public async Task<IActionResult> ValidateToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/validatetoken")] ValidateUserTokenVM request, ILogger log)
        {
            try
            {
                if (request == null)
                {
                    return InvalidRequest();
                }
                var response = await _authService.ValidateToken(request);
                if (response == 0)
                {
                    return OkResponse(true);
                }
                else if (response == -1)
                {
                    return ErrorWithResponse(MessageHelper.Invalidlink);
                }
                else if (response == -2)
                {
                    return ErrorResponse(MessageHelper.PasswordResetLinkExpired);
                }
                else
                {
                    return InvalidRequest();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(ResendInvite))]
        public async Task<IActionResult> ResendInvite(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/resendinvite")] ValidateUserTokenVM request, ILogger log)
        {
            try
            {
                if (request == null)
                {
                    return InvalidRequest();
                }
                var response = await _authService.ResendInvite(request);
                if (response)
                {
                    return OkResponse(response);
                }
                else
                {
                    return InvalidRequest();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(SetPassword))]
        public async Task<IActionResult> SetPassword(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/setpassword")] SetPasswordVM request, ILogger log)
        {
            try
            {
                if (request == null)
                {
                    return InvalidRequest();
                }
                var response = await _authService.SetPassword(request);
                if (response == 0)
                {
                    return OkResponse();
                }
                else if (response == -1)
                {
                    return ErrorWithResponse(MessageHelper.Invalidlink);
                }
                else if (response == -2)
                {
                    return ErrorWithResponse(MessageHelper.LinkExpire);
                }
                else
                {
                    return InvalidRequest();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }
        [FunctionName(nameof(ForgetPassword))]
        public async Task<IActionResult> ForgetPassword(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/forgetpassword")] ForgotPasswordVM request, ILogger log)
        {
            try
            {
                if (request == null)
                {
                    return InvalidRequest();
                }
                var response = await _authService.ForgetPassword(request);
                if (response == -1)
                {
                    log.LogInformation("user not exist");
                    return ErrorResponse(MessageHelper.EmailNotRegistered);
                }
                else if (response == -2)
                {
                    log.LogInformation("user not Active");
                    return ErrorResponse(MessageHelper.AccountInactive);
                }
                else if (response == -3)
                {
                    log.LogInformation("user unavailable");
                    return ErrorResponse(MessageHelper.UserUnavailable);
                }
                return OkResponse(response);

            }
            catch (Exception ex)
            {

                log.LogError(ex.ToString());
                return ExceptionErrorResponse();
            }
        }

        [FunctionName(nameof(LogoutUser))]
        public async Task<IActionResult> LogoutUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/logout")] HttpRequest request, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(request);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var req = await ModelCast.Request<Logout>(request.Body);
                        if (req.isLogout)
                        {
                            var logoutUser = new AuthLoginLogVM
                            {
                                HostAddress = request.HttpContext.Connection.RemoteIpAddress.ToString(),
                                UserAgent = request.Headers.UserAgent.ToString(),
                                Source = string.IsNullOrEmpty(request.Headers.UserAgent.ToString()) ? "EXE" : "WEB",
                                UserId = config.configData.UserId,
                                HostName = request.HttpContext.Connection.RemoteIpAddress.ToString(),
                                IsLogout = true
                            };
                            var response = await _authService.LogoutUser(logoutUser);
                            if (response > 0)
                                return OkResponse(response);
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


        [FunctionName(nameof(LogoutUserAndWorkplan))]
        public async Task<IActionResult> LogoutUserAndWorkplan(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/logoutuserandworkplan")] HttpRequest request, ILogger log)
        {
            try
            {
                var result = tokenProvider.ValidateToken(request);
                if (result.IsValid)
                {
                    var status = await config.SetConfigData(result.Username);
                    if (status == 0)
                    {
                        var req = await ModelCast.Request<LogoutUserVM>(request.Body);
                        //pause all existing running tasks
                        var pauseResponse = await _timeLogService.PauseExistingTasksOnLogout(req);

                        // logout use if isLogout = true
                        if (req.isLogout)
                        {
                            var logoutUser = new AuthLoginLogVM
                            {
                                HostAddress = request.HttpContext.Connection.RemoteIpAddress.ToString(),
                                UserAgent = request.Headers.UserAgent.ToString(),
                                Source = string.IsNullOrEmpty(request.Headers.UserAgent.ToString()) ? "EXE" : "WEB",
                                UserId = config.configData.UserId,
                                HostName = request.HttpContext.Connection.RemoteIpAddress.ToString(),
                                IsLogout = true,
                                LogoutTime = req.LogoutTime
                            };


                            var logoutResponse = await _authService.LogoutUser(logoutUser);

                            if (logoutResponse > 0)
                                return OkResponse(logoutResponse);
                            else
                                return ErrorResponse(MessageHelper.SaveError);
                        }

                        if (pauseResponse > 0)
                            return OkResponse(pauseResponse);
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
    }
}
