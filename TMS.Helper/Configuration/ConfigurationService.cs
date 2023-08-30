using SqlKata.Execution;
using TMS.Entity;

namespace TMS.Helper
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigData configData { get; set; }
        private readonly QueryFactory _dbContext;
        private readonly IUserHelperService _userService;

        public ConfigurationService(IUserHelperService userervice, QueryFactory dbContext)
        {
            _userService = userervice;
            configData = new ConfigData();
            _dbContext = dbContext;
        }

        public async Task<int> SetConfigData(string username)
        {
            var userDetail = await _userService.GetUserByUsername(username);
            if (userDetail == null)
            {
                //user not found
                return 0;
            }
            else if (userDetail.IsDeleted)
            {
                //user inactive
                return -2;
            }
            configData = new ConfigData
            {
                DepartmentId = userDetail.DepartmentId,
                EmailId = userDetail.EmailId,
                FirstName = userDetail.FirstName,
                IsEmailVerified = userDetail.IsEmailVerified,
                LastName = userDetail.LastName,
                MobileNo = userDetail.MobileNo,
                OrganizationIds = userDetail.OrganizationIds,
                ProfileImage = userDetail.ProfileImage,
                TwoFactorEnabled = userDetail.TwoFactorEnabled,
                UserId = userDetail.UserId,
                RoleTypeIds = userDetail.RoleTypeIds,
                ParentId = userDetail.ParentId,
                RoleName = userDetail.RoleName,
                WorkCategory = userDetail.WorkCategory,
            };
            return 0;
        }
    }
}
