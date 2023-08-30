using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IOrganizationUserService
    {
        public ConfigData ConfigData { get; set; }
        public Task<long> SaveOrganizationUser(OrgUserVM objCPA);
        public Task<long> DeleteOrganizationUser(long CPAId);
        public Task<IClientAndProjectRes> GetOrganizationUserList(OrgUserFilterVM filter);
        public Task<OrgUserWithCountVM> ClientByCPA(long CPAId);
        public Task<long> CreateTask(TaskVM objTask);
        public Task<bool> DeleteTask(long TaskId);
        public Task<List<GetCpaListVM>> GetCPAList(UserCPAVM repObj);
        public Task<ITaskListRes> GetTaskList(TaskListFilterVM filter);
        public Task<long> SaveStatus(StatusFilterVM objStatus);
        public Task<bool> DeleteStatus(long StatusId);
        public Task<StatusListWithCountVM> GetStatusList(PaginationMetaVM filter);
    }
}
