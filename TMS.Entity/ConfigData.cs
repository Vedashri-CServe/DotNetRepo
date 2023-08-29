namespace TMS.Entity
{
    public class ConfigData
    {
        public long UserId { get; set; }
        public List<long> OrganizationIds { get; set; } = new List<long>();
        public List<long> RoleTypeIds { get; set; } = new List<long>();
        public long ParentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string MobileNo { get; set; }
        public long DepartmentId { get; set; }
        public string ProfileImage { get; set; }
        public List<MenuRespVM> Menu { get; set; }
        public List<string> RoleName { get; set; }

        public WorkCategory? WorkCategory { get; set; }
    }

    public class MenuVM
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }
        public string Icon { get; set; }
    }
}
