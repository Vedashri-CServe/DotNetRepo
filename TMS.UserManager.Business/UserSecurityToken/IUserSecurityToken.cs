using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IUserSecurityToken
    {
        public Task<UserSecurityCodeResultVM> ValidateTokenAsync(ValidateUserTokenVM item, long userId = 0);
        public Task<bool> UpdateTokenStatus(UpdateTokenStatusVM item);
        public Task<string> SaveTokenAsync(long userId, UserSecurityTokenType type);
    }

    public enum UserSecurityTokenType
    {
        ForgotPassword = 1,
        UserActivation = 2,
        ChangeEmailActivation = 3,
        LoginOTP = 4,
        AccountStatus = 5,
    }
}
