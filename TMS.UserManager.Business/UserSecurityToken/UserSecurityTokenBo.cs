using SqlKata.Execution;
using TMS.Entity;
using TMS.Helper;

namespace TMS.UserManager.Business
{
    public class UserSecurityTokenBo : IUserSecurityToken
    {
        private readonly QueryFactory _dbContext;

        public UserSecurityTokenBo(QueryFactory dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> SaveTokenAsync(long userId, UserSecurityTokenType type)
        {
            var tokenDetail = GetTokenByType(type);
            var query = await _dbContext.Query("user_security_code").InsertGetIdAsync<long>(new
            {
                user_id = userId,
                code = tokenDetail.Token,
                expiry_time = tokenDetail.ExpiryTime,
                code_type = (byte)type,
                is_deleted = false,
                created_on = DateTime.UtcNow
            });
            return tokenDetail.Token;
        }

        private UserSecurityCode GetTokenByType(UserSecurityTokenType type)
        {
            switch (type)
            {
                case UserSecurityTokenType.ForgotPassword:
                case UserSecurityTokenType.UserActivation:
                case UserSecurityTokenType.ChangeEmailActivation:
                    return new UserSecurityCode
                    {
                        Token = Guid.NewGuid().ToString(),
                        ExpiryTime = DateTime.UtcNow.AddDays(1)
                    };
                case UserSecurityTokenType.LoginOTP:
                    return new UserSecurityCode
                    {
                        Token = RandomString.GenerateNumericString(4),
                        ExpiryTime = DateTime.UtcNow.AddMinutes(2)
                    };
                default:
                    return new UserSecurityCode
                    {
                        Token = Guid.NewGuid().ToString(),
                        ExpiryTime = DateTime.UtcNow.AddDays(1)
                    };
            }
        }

        public async Task<UserSecurityCodeResultVM> ValidateTokenAsync(ValidateUserTokenVM tokenVM, long userId = 0)
        {
            var query = _dbContext.Query("user_security_code");
            //when user id pass check specific user
            if (userId > 0)
            {
                query.Where(new 
                {
                    user_id = userId
                });
            }
            var tokenDetail = await query.Where(new 
            {
                code = tokenVM.Token,
                code_type = tokenVM.TokenType
            }).Select("code AS Token", "expiry_time AS ExpiryTime", "user_id AS UserId").FirstOrDefaultAsync<UserSecurityCodeDTO>();

            if (tokenDetail == null)
            {
                //when not found
                return new UserSecurityCodeResultVM
                {
                    Result = -1
                };
            }
            var currentTime = DateTime.UtcNow;
            if (tokenDetail.ExpiryTime >= currentTime)
            {
                return new UserSecurityCodeResultVM
                {
                    Result = 0,
                    UserId = tokenDetail.UserId
                };
            }
            else
            {
                //when token expire
                return new UserSecurityCodeResultVM
                {
                    Result = -2,
                    UserId = tokenDetail.UserId
                };
            }

        }

        public async Task<bool> UpdateTokenStatus(UpdateTokenStatusVM item)
        {
            //update user password
            await _dbContext.Query("user_security_code").Where(new
            {
                user_id = item.UserId,
                code = item.Token,
                code_type = item.Type
            }).UpdateAsync(new
            {
                expiry_time = DateTime.UtcNow,
                is_deleted = true
            });
            return true;
        }
    }
}
