namespace TMS.Entity
{
    public class DropdownItemVM
    {
        public string Label { get; set; }
        public long Value { get; set; }
    }

    public class DataItemFieldsVM
    {
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class DataListVM<T>
    {
        public List<T> List { get; set; } = new List<T>();
        public long TotalCount { get; set; }
    }

    public class PageFilterVM
    {
        public string GlobalSearch { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public string SortColumn { get; set; }
        public bool IsDesc { get; set; }
    }
}
