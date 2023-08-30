using DocumentFormat.OpenXml.Spreadsheet;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Entity;
using TMS.Helper;

namespace TMS.UserManager.Business
{
    public class AuthBo : IAuthService
    {
        private readonly QueryFactory _dbContext;
        private readonly IUserEmailProvider _emailProvider;
        private readonly IUserSecurityToken _userSecurityToken;
        private readonly IUserHelperService _userHelperService;

        public AuthBo(QueryFactory dbContext, IUserEmailProvider emailProvider, IUserHelperService userHelperService, IUserSecurityToken userSecurityToken)
        {
            _dbContext = dbContext;
            _userSecurityToken = userSecurityToken;
            _userHelperService = userHelperService;
            _emailProvider = emailProvider;
        }

        public async Task<bool> AddLoginLog(AuthLoginLogVM item)
        {
            try
            {
                await _dbContext.Query("user_login_log").InsertAsync(new
                {
                    user_id = item.UserId,
                    source = item.Source,
                    user_agent = item.UserAgent,
                    host_address = item.HostAddress,
                    host_name = item.HostName,
                    created_on = item.LogoutTime ?? DateTime.UtcNow,
                    is_logout = item.IsLogout
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> ValidateToken(ValidateUserTokenVM tokenVM)
        {
            return (await _userSecurityToken.ValidateTokenAsync(tokenVM)).Result;
        }

        public async Task<bool> ResendInvite(ValidateUserTokenVM tokenVM)
        {
            var tokenResult = await _userSecurityToken.ValidateTokenAsync(tokenVM);
            if (tokenResult != null && (tokenResult.Result == -2 || tokenResult.Result == 0))
            {
                var userDetail = await _userHelperService.GetUserById(tokenResult.UserId);
                if (userDetail != null)
                {
                    var userEmailDetail = new List<UserBasicDetailVM>
                    {
                        new UserBasicDetailVM
                        {
                            EmailId = userDetail.EmailId,
                            FirstName = userDetail.FirstName,
                            UserId = userDetail.UserId,
                            IsDeleted = userDetail.IsDeleted,
                            IsEmailVerified = userDetail.IsEmailVerified,
                            LastName = userDetail.LastName,
                            Password = string.Empty,
                            TwoFactorEnabled = userDetail.TwoFactorEnabled
                        }
                    };
                    if (tokenVM.TokenType == (int)UserSecurityTokenType.UserActivation)
                    {
                        await _emailProvider.UserAccountActivation(userEmailDetail);
                        await _userSecurityToken.UpdateTokenStatus(new UpdateTokenStatusVM
                        {
                            Token = tokenVM.Token,
                            Type = tokenVM.TokenType,
                            UserId = tokenResult.UserId
                        });
                        return true;
                    }

                }
            }
            return false;
        }

        public async Task<int> SetPassword(SetPasswordVM password)
        {
            //check token validation
            var tokenRes = await _userSecurityToken.ValidateTokenAsync(new ValidateUserTokenVM
            {
                Token = password.Token,
                TokenType = password.TokenType
            });
            if (tokenRes.Result != 0)
            {
                return tokenRes.Result;
            }
            //genrate password
            var passwordHash = PasswordHash.GenratePasswordHash(password.Password);
            if (string.IsNullOrEmpty(passwordHash))
            {
                return -3;
            }

            //when user activate
            if (password.TokenType == (int)UserSecurityTokenType.UserActivation)
            {
                await _dbContext.Query("user").Where("id", tokenRes.UserId).UpdateAsync(new
                {
                    password_hash = passwordHash,
                    updated_on = DateTime.UtcNow,
                    updated_by = tokenRes.UserId,
                    email_verify = true,
                    is_deleted = false,
                    is_active = true
                });
            }
            // for set password
            else
            {
                //update user password
                await _dbContext.Query("user").Where("id", tokenRes.UserId).UpdateAsync(new
                {
                    password_hash = passwordHash,
                    updated_on = DateTime.UtcNow,
                    updated_by = tokenRes.UserId
                });
            }

            //update token status
            await _userSecurityToken.UpdateTokenStatus(new UpdateTokenStatusVM
            {
                Token = password.Token,
                UserId = tokenRes.UserId,
                Type = password.TokenType
            });
            return 0;
        }
        public async Task<int> ForgetPassword(ForgotPasswordVM forgotPassword)
        {
            var user = await _dbContext.Query("user").
                         Where(new { email_id = forgotPassword.UserEmailId, is_deleted = false }).
                         Select("id As UserId", "email_id as EmailId", "is_active as IsActive", "first_name as FirstName", "last_name as LastName", "email_verify as IsEmailVerified", "is_available as IsAvailable").
                         FirstOrDefaultAsync<UserBasicDetailVM>();

            if(user == null)
            {
                return -1;
            }
            else if (!user.IsActive)
            {
                return -2;
            }
            else if (!user.IsAvailable)
            {
                return -3;
            }
            await _emailProvider.ForgetPassword(new UserBasicDetailVM
            {
                EmailId = user.EmailId,
                FirstName = user.FirstName,
                UserId = user.UserId,
                LastName = user.LastName
            });
            return 1;
        }

        public async Task<int> LogoutUser(AuthLoginLogVM user)
        {
            var res = await AddLoginLog(user);
            if (res)
                return 1;
            return 0;
        }
    }
}
