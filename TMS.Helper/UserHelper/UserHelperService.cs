using Dapper;
using SqlKata.Execution;
using TMS.Entity;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;

namespace TMS.Helper
{
    public  class UserHelperService : IUserHelperService
    {
        private readonly QueryFactory _dbContext;

        public UserHelperService(QueryFactory dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserVM> GetUser(string? emailId = default, long userId = default)
        {
            if (string.IsNullOrEmpty(emailId) && userId == default) 
            {
                return null;
            }
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetUserDetails, new
            {
                UserId = userId,
                EmailId = emailId
            }, commandType: CommandType.StoredProcedure);

            var userExists = await result.ReadSingleAsync<long>();
            if (userExists < 0) return null; // if user does not exists

            var userDetails = await result.ReadFirstAsync<UserVM>();
            var userOrganizationDetails = await result.ReadAsync<UserOrganizationVM>();
            var userRoleTypeDetails = await result.ReadAsync<UserRoleTypeVM>();
            userDetails.OrganizationIds = userOrganizationDetails.Select(x => x.OrganizationId).ToList();
            userDetails.RoleTypeIds = userRoleTypeDetails.Select(x => x.RoleTypeId).ToList();
            userDetails.RoleName = userRoleTypeDetails.Select(x => x.RoleName).ToList();
            return userDetails;
        }

        public async Task<UserVM> GetUserByUsername(string username)
        {
            return await GetUser(username);
        }

        public async Task<UserVM> GetUserById(long userId)
        {
            return await GetUser(userId: userId);
        }

        public async Task<List<LookupValueVM>> GetLookupValues(string shortDesc)
        {
            return (await _dbContext.Connection.QueryAsync<LookupValueVM>(DBHelper.USP_GetLookupValues, new
            {
                ShortDescription = shortDesc
            }, commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<List<DropdownItemVM>> GetLookupDropdownOpts(string shortDesc)
        {
            var result = await GetLookupValues(shortDesc);
            return result.GroupBy(x => x.ParentId).Where(y => y.Key != default).SelectMany(z => z.Select(n => new DropdownItemVM
            {
                Label = n.Description,
                Value = n.Id
            })).ToList();
        } 
    }
}
