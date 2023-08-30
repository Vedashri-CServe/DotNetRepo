namespace TMS.Entity
{
    public class LookupValueVM : DataItemFieldsVM
    {
        public long Id { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
    }
}