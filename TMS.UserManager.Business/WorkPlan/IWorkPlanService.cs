using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IWorkPlanService
    {
        public ConfigData ConfigData { get; set; }
        public Task<ClientAndTaskListResp> ClientAndTaskAndProcessListByCPA(long CPAId);
        public Task<long> SaveWorkPlan(WorkPlanReqVM objWP);
        public Task<IWorkPlanList> GetWorkPlanList(WorkPlanFilterVM filter);
        public Task<long> DeleteWorkPlan(long WorkPlanId);
        public Task<long> SaveCheckList(CheckListVM objWP);
        public Task<List<CheckListVM>> GetCheckList(checkListReqId objWP);
        public Task<long> DeleteCheckList(DeleteCheckList objDCL);
        public Task<long> AddWorkPlanComment(WorkPlanComment objComment);
        public Task<WorkPlanCommentListWithCountVM> GetComment(checkListReqId objComment);
        public Task<long> SaveRecurringPlan(RecurringVM objRP);
        public Task<List<RecurringVM>> GetRecurringDeatials(WorkPlanResultVM objWP);
        public Task<long> DeleteRecurringPlan(SaveRecurringResp objDRP);
        public Task<bool> ApproveWorkPlan(ApprovedWorkPlanVM ReqObj);
        public Task<long> ModifiedReviewLogs(UpdateReviewLogsVM ReqObj);
        public Task<IReviewerLogList> GetReviewerLogsList(ReviewerLogsReqVm ReqObj);
        public Task<List<DropdownItemVM>> GetEmployeeList();
        public Task<List<DropdownItemVM>> GetApprovalStatusList();
    }
}
