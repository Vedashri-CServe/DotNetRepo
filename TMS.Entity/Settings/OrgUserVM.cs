namespace TMS.Entity
{
    public class OrgUserVM
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string WebSiteURl { get; set; }
        public long ParentId { get; set; }
        public long OrganizationType { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string TypeOfWork { get; set; }
        public string BillingType { get; set; }
        public long ContractedHours { get; set; }
        public long InternalHours { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedBy { get; set; }
        public bool SOP { get; set; }
        public string CPAName { get; set; }
        public object? TypeOfWorkList { get; set; }
        public object? BillingTypeList { get; set; }
        public bool IsAvailable { get; set; }
    }
    public interface IClientAndProjectRes
    {

    }
    public class ClientAndProjecNotDownloadRes : DataListVM<OrgUserVM>, IClientAndProjectRes
    {

    }
    public class ClientAndProjecDownloadRes : IClientAndProjectRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }

    public class SaveOrgResultVM
    {
        public long UserId { get; set; }
    }

    public class OrgUserFilterVM : PaginationVM
    {
        public long OrganizationType { get; set; }
        public string TypeOfWork { get; set; }
        public string BillingType { get; set; }
        public long? ContractedHours { get; set; }
        public string GlobalSearch { get; set; }
        public long? UserId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsDownload { get; set; }
    }

    public class PaginationVM : PaginationMetaVM
    {
        public string SortColumn { get; set; }
        public bool IsDesc { get; set; }
    }

    public class PaginationMetaVM
    {
        public int Index { get; set; }
        public long PageSize { get; set; }
    }

    public class OrgUserWithCountVM
    {
        public long TotalCount { get; set; }
        public List<OrgUserVM> List { get; set; }
    }

    public class StatusFilterVM
    {
        public long StatusId { get; set; }
        public string StatusName { get; set; }
        public string Color { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class StatusVMResultVM
    {
        public long Id { get; set; }
    }

    public class StatusListWithCountVM
    {
        public long TotalCount { get; set; }
        public List<StatusFilterVM> List { get; set; }
    }

    public class UserCPAVM 
    {
        public bool? CallByDWP { get; set; }
    }

}