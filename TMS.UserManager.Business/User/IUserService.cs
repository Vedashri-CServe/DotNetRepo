using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IUserService
    {
        public ConfigData ConfigData { get; set; }
        public Task<IUserListRes> GetUserList(UserListFilterVM filter);
        public Task<long> SaveUser(UserVM user);
        public Task<bool> DeleteUser(long userId);
        public Task<List<DropdownItemVM>> GetUserRoleList();
        public Task<List<DropdownItemVM>> GetDepartmentDropdown();
        public Task<bool> InActiveAccount(InActiveAccountVM reqObj);
        public Task<List<DropdownItemVM>> GetReportingManagerDropdown();
        public Task<IEnumerable<MenuRespVM>> GetUserMenuList(UserRoleIdVM userRoleId);
        public Task<long> UpdateMenuList(UpdateMenuReqVM updateManuReq);
        public Task<long> SaveRoleTypeForPermission(RoleTypeVM role);
        public Task<long> DeleteRoleType(UserRoleIdVM roleId);
        public Task<List<DropdownItemVM>> GetPermissionRoleList();

        public Task<IEnumerable<DropdownItemVM>> GetUsersForDropdown();
    }
}
