using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Entity
{
    public class CPAReqVM
    {
        public long CPAId { get; set; }
    }

    public class TaskListResp : DropdownItemVM
    {
        public decimal EstimatedDuration { get; set; }
    }

    public class StatusListResp
    {
        public long Id { get; set; }
        public string Color { get; set; }
        public string StatusName { get; set; }
    }


    public class ClientAndTaskListResp
    {
        public List<DropdownItemVM> ClientList { get; set; }
        public List<TaskListResp> TaskList { get; set; }
        public List<StatusListResp> StatusList { get; set; }
        public List<DropdownItemVM> ProcessList { get; set; }
    }

    public class WorkPlanReqVM : ManualWorkPlanVM
    {
        public long WorkPlanId { get; set; }
        public long CPAId { get; set; }
        public long ClientId { get; set; }
        public long? ProcessId { get; set; }
        public long? SubprocessId { get; set; }
        public long? TaskId { get; set; }
        public long StatusId { get; set; }
        public long? Quantity { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? TimelineDate { get; set; }
        public string Description { get; set; }
    }
    public class WorkPlanResultVM
    {
        public long WorkPlanId { get; set; }
    }

    public class ManualWorkPlanVM
    {
        public bool IsManual { get; set; }
        public TimeSpan TotalEstimatedTime { get; set; }
    }
    public class WorkPlanFilterVM : PaginationMetaVM
    {
        public string GlobalSearch { get; set; }
        public long UserId { get; set; }
        public DateTime? TimelineDate { get; set; }

        public List<long>? CPAList { get; set; }
        public List<long>? ClientList { get; set; }
        public List<long>? TaskList { get; set; }
        public List<long>? ProcessList { get; set; }
        public List<long>? SubProcessList { get; set; }
        public List<long>? ReviewerStatus { get; set; }
        public List<long>? WorkPlanStatus { get; set; }
        public bool? IsManual { get; set; }
        public bool IsDownload { get; set; } = false;
    }

    public class WorkPlanRespVM : ApprovalVM
    {
        public long WorkPlanId { get; set; }
        public long CPAId { get; set; }
        public string CPAName { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public long ProcessId { get; set; }
        public string ProcessName { get; set; }
        public long SubprocessId { get; set; }
        public string SubprocessName { get; set; }
        public string ActivityName { get; set; }
        public long TaskId { get; set; }
        public string TaskName { get; set; }
        public decimal EstimatedDuration { get; set; }
        public decimal EstimatedProcessTime { get; set; }
        public long StatusId { get; set; }
        public string StatusName { get; set; }
        public long Quantity { get; set; }
        public decimal TotalTime { get; set; }
        public bool RecurringStatus { get; set; }
        public bool Event { get; set; }
        public int EventType { get; set; }
        public string EventDuration { get; set; }
        public string IsStop { get; set; }
        public string Description { get; set; }
        public bool IsManual { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
    }

    public class WorkPlanListRespVM
    {
        public long WorkPlanId { get; set; }
        public DropdownItemVM CPA { get; set; }
        public DropdownItemVM Client { get; set; }
        public TaskListResp Process { get; set; }
        public DropdownItemVM SubProcess { get; set; }
        public object ActivityName { get; set; }
        public TaskListResp Task { get; set; }
        public DropdownItemVM Status { get; set; }
        public long Quantity { get; set; }
        public bool RecurringStatus { get; set; }
        public bool Event { get; set; }
        public int EventType { get; set; }
        public string EventDuration { get; set; }
        public string IsStop { get; set; }
        public string Description { get; set; }
        public bool IsManual { get; set; } = false;

        public DropdownItemVM ApprovalDetails { get; set; }
        public DropdownItemVM ModifiedHoursDetails { get; set; }
        public string? EmployeeName { get; set; }
        public string? ReviewerName { get; set; }
        public string? RejectedComment { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
    public class WorkPlanListWithCountVM : WorkplanTotalTimeVM, IWorkPlanList
    {
        public bool IsBreak { get; set; }
        public long TotalCount { get; set; }

        public bool IsCreateWorkPlanButtonEnable { get; set; }

        public List<WorkPlanListRespVM> WorkPlanList { get; set; }

    }
    public interface IWorkPlanList { }
    public class WorkPlanListDownloadRes : IWorkPlanList
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
    public class CheckListVM
    {
        public long Id { get; set; }
        public long WorkPlanId { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class CheckListResultVM
    {
        public long checkListId { get; set; }
    }

    public class checkListReqId
    {
        public long WorkPlanId { get; set; }
    }

    public class DeleteCheckList
    {
        public long CheckListId { get; set; }
    }

    public class WorkPlanComment : checkListReqId
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public bool IsComment { get; set; } = true;
    }

    public class CommentRespVM
    {
        public long CommentId { get; set; }
    }

    public class CommnetResponseVM : WorkPlanComment
    {
        public long CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string CommentBy { get; set; }
        public string UserImage { get; set; }
    }

    public class WorkPlanCommentListWithCountVM
    {
        public long TotalCount { get; set; }
        public List<CommnetResponseVM> CommentList { get; set; }
    }

    public class ApprovalVM
    { 
        public long? ApprovalId { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? EmployeeName { get; set; }
        public string? ReviewerName { get; set; }
        public string? RejectedComment { get; set; }
        public long? ModifiedId { get; set; }
        public string? ModifiedHours { get; set; }
        public DateTime? SubmittedDate { get; set; }
    }

    public class SaveRecurringResp
    {
        public long RecurringId { get; set; }
    }
    public class RecurringVM : SaveRecurringResp
    {
        public long WorkPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RecurringCronExp { get; set; }
        public long? CreatedBy { get; set; }
        public string OccurrenceOption { get; set; }
    }

    public class RecurringWithCronExp : RecurringVM
    {
        public CropExpSep CropExpSepobj = new CropExpSep();
    }

    public class CropExpSep
    {
        public string Minutes { get; set; }
        public string Hours { get; set; }
        public string Days { get; set; }
        public string Months { get; set; }
        public string DayOfWeek { get; set; }
    }
    public class ApprovedWorkPlanVM
    {
        public List<StatusDictionary>? SelectedArray { get; set; }
        public string? RejectedComment { get; set; }
    }

    public class StatusDictionary
    {
        public long WorkPlanId { get; set; }
        public long Status { get; set; }
    }
    public class UpdateReviewLogsVM
    {
        public long? ModifiedId { get; set; }
        public long WorkPlanId { get; set; }
        public DateTime ModifiedHours { get; set; }
    }
    public class ReviewerLogsReqVm : PaginationMetaVM
    {
        public string? GlobalSearch { get; set; }
        public List<long>? UserIds { get; set; }
        public DateTime? TimelineDate { get; set; }
        public List<long>? CPAList { get; set; }
        public List<long>? ClientList { get; set; }
        public List<long>? TaskList { get; set; }
        public List<long>? ProcessList { get; set; }
        public List<long>? SubProcessList { get; set; }
        public List<long>? ReviewerStatus { get; set; }
        public List<long>? WorkPlanStatus { get; set; }
        public bool? IsManual { get; set; }
        public bool IsDownload { get; set; } = false;
    }
    public class ReviewerLogsRespVM : WorkPlanRespVM
    {
        public long ApprovalId { get; set; }
        public string ApprovalStatus { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string ReviewerName { get; set; }
        public long ModifiedId { get; set; }
        public string ModifiedHours { get; set; }
        public string RejectedComment { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string DepartmentName { get; set; }
        public string RoleType { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Efficiency { get; set; }
        public string Reason { get; set; }
        
    }

    public interface IReviewerLogList { }
    public class ReviewerLogsListRespVM : DataListVM<TaskRespVM>, IReviewerLogList
    {
        public List<ReviewerLogsRespVM> List { get; set; }
        public long TotalCount { get; set; }
        public string? TotalEventTime { get; set; }
        public long TotalQty { get; set; }
        public string TotalStdHours { get; set; }
        public string TotalModifiedHours { get; set; }

    }
    public class ReviewerLogsDownloadRes : IReviewerLogList
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }

    public class WorkplanTotalTimeVM
    {
        public string TotalEventTime { get; set; }
        public string TotalBreakTime { get; set; }
    }
}
