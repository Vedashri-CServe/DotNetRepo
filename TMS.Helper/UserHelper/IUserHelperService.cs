using TMS.Entity;

namespace TMS.Helper
{
    public interface IUserHelperService
    {
        public Task<UserVM> GetUser(string? emailId = default, long userId = default);
        public Task<UserVM> GetUserByUsername(string username);
        public Task<UserVM> GetUserById(long userId);
        public Task<List<LookupValueVM>> GetLookupValues(string shortDesc);
        public Task<List<DropdownItemVM>> GetLookupDropdownOpts(string shortDesc);
    }
}
