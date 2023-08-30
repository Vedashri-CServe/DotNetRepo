namespace TMS.Entity
{
    public class UserIdVM
    {
        public long UserId { get; set; }
    }

    public class UserBasicDetailVM
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Password { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class UserVM : UserBasicDetailVM
    {
        public List<long> RoleTypeIds { get; set; } = new List<long>();
        public List<long> OrganizationIds { get; set; } = new List<long>();
        public long ParentId { get; set; }
        public string MobileNo { get; set; }
        public long DepartmentId { get; set; }
        public string ProfileImage { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<string> RoleName { get; set; }
        public bool IsAvailable { get; set; }

        public WorkCategory? WorkCategory { get; set; }
    }

    public class UserListFilterVM : PageFilterVM
    {
        public long? RoleTypeId { get; set; }
        public long? DepartmentId { get; set; }
        public long? UserId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsDownload { get; set; }
    }

    public class CommonUserItemDataVM
    {
        public long UserId { get; set; }
        public long ParentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
        public long? WorkCategory { get; set; }
    }

    public class UserItemDataVM : CommonUserItemDataVM
    {
        public string Organizations { get; set; }
        public string Roles { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public long? ReportingManagerId { get; set; }

        public string? ReportingManager { get; set; }
    }

    public class UserListItemVM : CommonUserItemDataVM
    {
        public DropdownItemVM DepartmentType { get; set; }
        public List<DropdownItemVM> Organizations { get; set; } = new List<DropdownItemVM>();
        public List<DropdownItemVM> Roles { get; set; } = new List<DropdownItemVM>();

        public long? ReportingManagerId { get; set; }

        public string? ReportingManager { get; set; }
    }
    public interface IUserListRes
    {

    }
    public class UserListNotDownloadRes : DataListVM<UserListItemVM>, IUserListRes
    {

    }
    public class UserListDownloadRes : IUserListRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }

    public enum RoleTypes
    {
        SuperAdmin = 1,
        Admin = 2
    }

    public class UserOrganizationVM : DataItemFieldsVM
    {
        public long UserOrganizationId { get; set; }
        public long UserId { get; set;}
        public long OrganizationId { get; set; }
    }

    public class UserRoleTypeVM : DataItemFieldsVM
    {
        public long UserRoleTypeId { get; set; }
        public long UserId { get; set; }
        public long RoleTypeId { get; set; }
        public string RoleName { get; set; }
    }

    public class InActiveAccountVM
    {
        public long Id { get; set; }
        public bool IsAvailable { get; set; }
        public long TableType { get; set; }
    }

    public enum WorkCategory
    {
        Tasks = 31,
        Processes = 32,
        Both = 33
    }

    public class MenuRespVM
    {
        public long MenuId { get; set; }
        public long PermissionId { get; set; }
        public string DefaultName { get; set; }
        public string DisplayName { get; set; }
        public long? ParentId { get; set; }
        public bool IsMainMenu { get; set; }
        public bool IsShow { get; set; }
        public bool IsAdd { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefaultLandingPage { get; set; }
        public int RoleTypeId { get; set; }
        public string RouteLink  { get;set;} 
        public string? Icon { get; set; }
        public List<MenuRespVM>? TabData { get; set; } = new List<MenuRespVM>();
    }

    public class UserRoleIdVM
    {
        public long? RoleId { get; set; }
    }

    public class RoleTypeVM
    {
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public class UpdateMenuReqVM
    {
        public long MenuId { get;set; }
        public long PermissionId { get; set; }
        public string DisplayName { get; set; }
        public bool IsShow { get; set; }
        public bool IsAdd { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefaultLandingPage { get; set; }
        public long RoleTypeId { get; set; }
        public long? ParentId { get; set; }
    }
    
}