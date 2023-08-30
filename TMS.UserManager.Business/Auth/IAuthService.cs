using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IAuthService
    {
        public Task<int> ValidateToken(ValidateUserTokenVM tokenVM);
        public Task<bool> ResendInvite(ValidateUserTokenVM tokenVM);
        public Task<int> SetPassword(SetPasswordVM password);
        public Task<bool> AddLoginLog(AuthLoginLogVM item);
        public Task<int> ForgetPassword(ForgotPasswordVM forgotPassword);
        public Task<int> LogoutUser(AuthLoginLogVM user);
    }
}
