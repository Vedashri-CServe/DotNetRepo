using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IUserEmailProvider
    {
        public Task UserAccountActivation(List<UserBasicDetailVM> userDetail);
        public Task UserAccountCreated(UserBasicDetailVM userDetail);
        public Task ForgetPassword(UserBasicDetailVM userDetail);
    }
}
