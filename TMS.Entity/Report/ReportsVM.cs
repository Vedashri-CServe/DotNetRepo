using System.Net.Mail;

namespace TMS.Entity
{
    #region CPAReport

    public class CpaReportReq : PageFilterVM
    {
        public long? TypeOfWork { get; set; }
        public long? BillType { get; set; }
        public List<long>? Clients { get; set; }
        public bool? Available { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDownload { get; set; }

        public IEnumerable<long>? Departments { get; set; }
    }

    public class CpaReportData
    {
        public long CPAId { get; set; }

        public string? CPAName { get; set; }

        public string? TypeOfWork { get; set; }

        public string? BillType { get; set; }

        public string? InternalHours { get; set; }

        public string? ContractedHours { get; set; }

        public string? ActualHours { get; set; }

        public string? TotalTimeSpent { get; set; }

        public string? TotalModifiedHours { get; set; }

        public string? Difference { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsAvailable { get; set; }

        public string? ContractedDiff { get; set; }
    }

    public interface ICPAReportRes
    {

    }

    public class CPAReportNotDownloadRes : DataListVM<CpaReportData>, ICPAReportRes
    {

    }

    public class CPAReportsDownloadRes : ICPAReportRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
    #endregion


    #region ClientReport

    public class ClientReportReq : PageFilterVM
    {
        public long? TypeOfWork { get; set; }
        public long? BillType { get; set; }
        public List<long>? Clients { get; set; }
        public List<long>? Projects { get; set; }
        public bool? Available { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDownload { get; set; }
        public IEnumerable<long>? Departments { get; set; }
    }

    public class ClientReportData
    {
        public long ClientId { get; set; }

        public string? ClientName { get; set; }

        public string? ClientTypeOfWork { get; set; }

        public string? CPATypeOfWork { get; set; }

        public string? CPABillType { get; set; }

        public long ParentId { get; set; }

        public string? CPAName { get; set; }

        public string? EstimatedHours { get; set; }

        public string? ActualHours { get; set; }

        public string? TotalTimeSpent { get; set; }

        public string? TotalModifiedHours { get; set; }

        public string? Difference { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool IsAvailable { get; set; }
    }

    public interface IClientReportRes
    {

    }

    public class ClientReportNotDownloadRes : DataListVM<ClientReportData>, IClientReportRes
    {

    }

    public class ClietReportsDownloadRes : IClientReportRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
    #endregion


    #region ActualPlannedReport

    public class ActualPlannedReportReq : PageFilterVM
    {
        public List<long>? Clients { get; set; }
        public List<long>? Projects { get; set; }
        public List<long>? Reporting { get; set; }
        public bool? Available { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsDownload { get; set; }
        public DateTime? EndDate { get; set; }

        public IEnumerable<long>? Users { get; set; }

        public IEnumerable<long>? Departments { get; set; }
    }

    public class ActualPlannedReportData
    {
        public long UserId { get; set; }

        public string? UserName { get; set; }

        public long? DailyWorkPlanId { get; set; }

        public long? TaskId { get; set; }

        public string? TaskName { get; set; }

        public long? SubProcessId { get; set; }

        public string? SubProcessName { get; set; }

        public string? ActivitiesJson { get; set; }

        public string? EstimatedHours { get; set; }

        public DateTime CreatedOn { get; set; }

        public long? CreatedById { get; set; }

        public string? ReportingManager { get; set; }

        public string? TotalTimeSpent { get; set; }

        public double? DifferencePercentage { get; set; }

        public long? ClientId { get; set; }

        public string? ClientName { get; set; }

        public long? CPAId { get; set; }

        public string? CPAName { get; set; }

        public bool IsTask { get; set; }

        public string? TotalModifiedHours { get; set; }
        public bool IsAvailable { get; set; }

        public IEnumerable<ActivityName?>? Activities { get; set; }

        public string? Description { get; set; }

        public string? Comment { get; set; }
        public bool IsManual { get; set; } = false;

        public string? Reason { get; set; }
    }

    public interface IAPReportRes
    {

    }

    public class APReportNotDownloadRes : DataListVM<ActualPlannedReportData>, IAPReportRes
    {

    }

    public class APReportsDownloadRes : IAPReportRes
    {

        public byte[]? ByteArray { get; set; }

        public string? ContentType { get; set; }

        public string? FileName { get; set; }

    }
    #endregion


    #region ActivityReport

    public class ActivityReportReq : PageFilterVM
    {
        public List<long>? Users { get; set; }
        public bool? Available { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsDownload { get; set; }
        public DateTime? EndDate { get; set; }
        public List<long>? Departments { get; set; }
    }

    public class ActivityReportData
    {
        public long UserId { get; set; }
        public long ParentId { get; set; }
        public long DepartmentId { get; set; }
        public string? UserName { get; set; }

        public string? TotalHrs { get; set; }

        public string? ProductiveHrs { get; set; }

        public string? UnProductiveHrs { get; set; }

        public string? BillableHrs { get; set; }

        public string? NonBillableHrs { get; set; }
        public string? TotalModifiedHours { get; set; }
        public bool IsAvailable { get; set; }
    }
    public interface IActivityReportRes
    {

    }

    /*public class ActivityReportNotDownloadRes : IActivityReportRes
    {
        public IEnumerable<Tree<ActivityReportData>> List { get; set; }
        public long TotalCount { get; set; }
    }*/

    public class ActivityReportNotDownloadRes : DataListVM<ActivityReportData>, IActivityReportRes
    {

    }
    public class ActivityReportsDownloadRes : IActivityReportRes
    {

        public byte[]? ByteArray { get; set; }

        public string? ContentType { get; set; }

        public string? FileName { get; set; }

    }

    #endregion

    #region Other Reports

    public enum OtherReportIdentifier
    {
        UserReport,
        TimesheetReport,
        WorklogsReport,
        WorkloadReport
    }

    public enum PresentStatus
    {     
        No,  //Absent
        Yes  //Present
    }
    public class OtherReportReq : PageFilterVM
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public OtherReportIdentifier ReportIdentifier { get; set; }

        public List<long>? Users { get; set; }

        public bool IsDownload { get; set; }

        public OtherReportIdentifier OtherReportIdentifier { get; set; }

        public List<long>? Departments { get; set; }
    }

    public class OtherReportData
    {
        public long UserId { get; set; }
        public long ParentId { get; set; }
        public string? UserName { get; set; }
        public string? RoleType { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public string? ReportingManager { get; set; }
        public double? PresentDays { get; set; }
        public string? TotalTimeSpentByUser { get; set; }
        public string? TotalTimeOfUser { get; set; }
        public string? TotalModifiedHoursByUser { get; set; }
        public string? TotalBreakTime { get; set; }
        public string? TotalIdleTime { get; set; }
        public string? TotalRejectedHrsByUser { get; set; }
        public string? TotalTimeExcludingRejectedHrsByUser { get; set; }
        public string? AvgTotalTime { get; set; }
        public string? AvgBreakTime { get; set; }
        public string? AvgIdleTime { get; set; }

        public IEnumerable<DateTimeLog>? DateTimeLogs { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class DateTimeLog
    {
        public long? UserId { get; set; }

        public DateTime? LogDate { get; set; }

        public long? DateOfLogDate { get; set; }
        public long? MonthOfLogDate { get; set; }
        public long? YearOfLogDate { get; set; }

        public string? TotalTimeSpentForDay { get; set; }

        public string? TotalTimeHrsForDay { get; set; }

        public string? TotalTrackedTimeLogHrsForDay { get; set; }

        public string? TotalManualTimeLogHrsForDay { get; set; }

        public string? DayStartTime { get; set; }

        public string? DayEndTime { get; set; }

        public char? AttendanceStatus { get; set; }

        public string? DayBreakTime { get; set; }

        public string? DayIdleTime { get; set; }

        public double? TotalTimeSpentForDayPercent { get; set; }

        public double? TotalManualTimeLogHrsForDayPercent { get; set; }

        public double? TotalTrackedTimeLogHrsForDayPercent { get; set; }

        public double? TotalDayBreakTimePercent { get; set; }

        public double? TotalDayIdleTimePercent { get; set; }

        public IEnumerable<BreakTime> BreakTimes { get; set; }

        public IEnumerable<IdleTime> IdleTimes { get; set; }

        public IEnumerable<LogDetail>? LogDetails { get; set; }

        public IEnumerable<WorkTime> WorkTimes { get; set; }
        public string? TotalModifiedHoursForDay { get; set; }
        public double? TotalModifiedHoursForDayPercent { get; set; }
        public string? TimeSpentWithFormatting { get; set; }
    }

    public class DBWorkTime
    {
        public long? DailyWorkPlanId { get; set; }

        public long? UserId { get; set; }

        public DateTime? EventTime { get; set; }

        public int EventType { get; set; }

        public string? IsStop { get; set; }
    }

    public class WorkTime
    {
        public long? DailyWorkPlanId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool IsStop { get; set; }
    }

    public class DBBreakTime
    {
        public long? UserId { get; set; }

        public DateTime? LogDate { get; set; }

        public DateTime? StartEndDateTime { get; set; }

        public int EventType { get; set; }
    }

    public class BreakTime
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Difference { get; set; }
    }

    public class DBIdleTime
    {
        public long? UserId { get; set; }

        public DateTime? LogDate { get; set; }

        public DateTime? StartEndDateTime { get; set; }

        public int EventType { get; set; }
    }

    public class IdleTime
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Difference { get; set; }

    }

    public class LogDetail
    {
        public long? UserId { get; set; }
        public string? UserName { get; set; }  

        public DateTime? LogDate { get; set; }
        public long? DailyWorkPlanId { get; set; }
        public string? Description { get; set; }
        public long? TaskId { get; set; }
        public string? TaskName { get; set; }
        public string? EstimatedTime { get; set; }
        public string? Status { get; set; }
        public int? Quantity { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? TotalTime { get; set; }
        public string? TotalTimeSpentOnTask { get; set; }
        public string? TotalManualTimeLogHrs { get; set; }
        public string? TotalTrackedTimeLogHrs { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsTask { get; set; }
        public long? CPAId { get; set; }
        public string? CPAName { get; set; }
        public long? ClientId { get; set; }
        public string? ClientName { get; set; }

        public string? ModifiedHours { get; set; }
        public bool IsTaskAvailable { get; set; }

        public long? SubProcessId { get; set; }

        public string? SubProcessName { get; set; }

        public string? ActivitiesJson { get; set; }

        public IEnumerable<ActivityName?>? Activities { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsManual { get; set; } = false;
        public string? ReviewerStatus { get; set; }
        public string? RoleType { get; set; }
        public string? DepartmentName { get; set; }

    }

    public interface IOtherReportRes
    {

    }
    public class Tree<T> where T : class
    {
        public T Parent { get; set; }
        public IEnumerable<Tree<T>> Children { get; set; } = new List<Tree<T>>();
    }

    public class WorkloadReportNotDownloadRes : IOtherReportRes
    {
        public IEnumerable<Tree<OtherReportData>> List { get; set; } 
        public long TotalCount { get; set; }
    }

    public class OtherReportNotDownloadRes : DataListVM<OtherReportData>, IOtherReportRes
    {

    }

    public class OtherReportsDownloadRes : IOtherReportRes
    {

        public byte[]? ByteArray { get; set; }

        public string? ContentType { get; set; }

        public string? FileName { get; set; }

    }

    #endregion

    #region KRA Report

    public class KRAReportDataVM
    {
        public long CPAId { get; set; }
        public string CPAName { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public long TaskId { get; set; }
        public string TaskName { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string EstimatedDuration { get; set; }
        public string ActualTime { get; set; }
        public bool IsTask { get; set; }
        public string Result { get; set; }
        public double? EfficiencyPercentage { get; set; }
        public int TotalNoOfQuantity { get; set; }
        public string? TotalModifiedHours { get; set; }

        public long? SubProcessId { get; set; }

        public string? SubProcessName { get; set; }

        public string? ActivitiesJson { get; set; }

        public IEnumerable<ActivityName?>? Activities { get; set; }
        public bool IsAvailable { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
    public interface IKRAReportRes
    {

    }
    public class KRAReportNotDownloadRes : DataListVM<KRAReportDataVM>, IKRAReportRes
    {

    }
    public class KRAReportsDownloadRes : IKRAReportRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
    public class KRAReqVM : PageFilterVM
    {
        public List<long>? Clients { get; set; }
        public List<long>? Projects { get; set; }
        public List<long>? Users { get; set; }
        public bool? Available { get; set; }
        public DateTime? StartDate { get; set; }

        public bool IsDownload { get; set; }
        public DateTime? EndDate { get; set; }
        public List<long>? Departments { get; set; }
    }



    #endregion

    #region AutoManual Report

    public class AutoManualReportReq : PageFilterVM
    {
        public List<long>? Departments { get; set; }
        public List<long>? Users { get; set; }
        public List<long>? ReportingUsers { get; set; }
        public bool? Available { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsDownload { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class AutoManualReportData
    {
        public long? UserId { get; set; }

        public string? UserName { get; set; }

        public string? DepartmentName { get; set; }

        public long? ReportingUserId { get; set; }

        public string? ReportingUserName { get; set; }

        public string? TotalEstimatedTime { get; set; }

        public string? TotalManualTimeSpent { get; set; }

        public string? TotalTrackedTimeSpent { get; set; }

        public string? TotalModifiedHours { get; set; }

        public string? TotalTimeSpent { get; set; }
    }

    public interface IAutoManualReportRes
    {

    }

    public class AutoManualReportNotDownloadRes : DataListVM<AutoManualReportData>, IAutoManualReportRes
    {

    }

    public class AutoManualReportsDownloadRes : IAutoManualReportRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
    #endregion

    #region Daily Report

    public class DailyReportRes
    {

        public DailyReportAbsentPresentTable? DailyReportAbsentPresentTable { get; set; }

        public Attachment? DailyReportAbsentPresentExcel { get; set; }

    }

    public class DailyReportAbsentPresentTable
    {
        public int? TotalNoOfUsers { get; set; }
        public int? TotalNoOfAbsentUsers { get; set; }
        public int? TotalNoOfHalfPresentUsers { get; set; }

        public int? TotalNoOfPresentUsers { get; set; }
    }

    public class DailyReportAbsentPresentExcel
    {
        public string? UserName { get; set; }
        public string? ReportingManager { get; set; }
    }

    public class ATLReport
    {
        public string? ReviewerName { get; set; }
        public string? DepartmentName { get; set; }
        public long? TotalUserCount { get; set; }
        public long? TotalTasks { get; set; }
        public string? TotalActualTime { get; set; }
        public string? TotalTimeSpent { get; set; }
        public long? PendingTasksForSubmission { get; set; }
        public long? PartiallySubmittedTasks { get; set; }
        public long? ApprovedTasks { get; set; }
        public long? RejectedTasks { get; set; }
        public long? PendingForReviewTasks { get; set; }
        public string? ApprovedTimeSpent { get; set; }
        public string? ApprovedStdTime { get; set; }
        public string? RejectedTimeSpent { get; set; } 
        public string? RejectedStdTime { get; set; }
        public string? PartiallySubmittedTimeSpent { get; set; } 
        public string? PartiallySubmittedStdTime { get; set; }
        public string? PendingTimeSpent { get; set; } 
        public string? PendingStdTime { get; set; }

    }

    public class ATLExcelReport
    {
        public long? ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public string? TaskName { get;set; }
        public string? SubProcessName { get; set; }
        public string? UserName { get; set; }
        public string? PendingTimeSpent { get; set;}
        public string? PendingStdTime { get;set; }
    }
    public class MonthlyReport
    {
        public long? MonthlyTotalTask { get; set; }
        public long? MonthlyReviewerPendingTasks { get; set; }
        public long? MonthlyPartiallySubmittedTasks { get; set; }
        public long? MonthlyApprovedTasks { get; set; }
        public long? MonthlyRejectedTasks { get; set; }
        public long? MonthlyUserPendingTasks { get; set; }
        public long? MonthlyUserCompletedTasks { get; set; }
    }
    #endregion


    #region LoginLogoutReport

    public class LoginLogoutReportReq : PageFilterVM
    {
        public DateTime? DateFilter { get; set; }

        public bool IsDownload { get; set; }

        public bool? IsAvailable { get; set; }

        public List<long>? Users { get; set; }

        public List<long>? Reporting { get; set; }

        public List<long>? Departments { get; set; }
        public PresentStatus? PresentStatus { get; set; }
    }


    public interface ILoginLogoutReportRes
    {

    }

    public class LoginLogoutNotDownloadRes : DataListVM<LoginLogoutReportData>, ILoginLogoutReportRes
    {

    }

    public class LoginLogoutReportsDownloadRes : ILoginLogoutReportRes
    {

        public byte[]? ByteArray { get; set; }

        public string? ContentType { get; set; }

        public string? FileName { get; set; }

    }

    public class LoginLogoutReportData
    {
        public long UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserType { get; set; }

        public string? DepartmentName { get; set; }

        public long ReportingToId { get; set; }

        public string? ReportingTo { get; set; }

        public DateTime? LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public string? TotalTimeSpentForDay { get; set; }

        public string? BreakTimeForDay { get; set; }

        public string? IdleTimeForDay { get; set; }

        public PresentStatus PresentStatus { get; set; }

    }

    #endregion

    #region AuditReport

    public interface IAuditReportRes{}

    public class AuditReportNotDownloadRes : DataListVM<AuditReportData>, IAuditReportRes{}

    public class AuditReportDownloadRes : IAuditReportRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }

    }

    public class AuditReportData
    {
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? TaskCreatedDate { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string? ClientName { get; set; }
        public string? ProjectName { get; set; }
        public string? ProcessName { get; set; }
        public string? SubProcessName { get; set; }
        public string? StandardTime { get; set; }
        public string? TotalTime { get; set; }
        public string? BreakTime { get; set; }
        public string? IdleTime { get; set; }
    }

    public class AuditReportReq : PageFilterVM
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDownload { get; set; }
        public List<long>? UserIdsForFilter { get; set; }
        public List<long>? DepartmentsFilter { get; set; }
        public List<long>? ClientFilter { get; set; }
        public List<long>? ProjectFilter { get; set; }
    }

    #endregion

    #region Management Report

    public class AvgTotalDataTable
    {
        public long? TotalPresentUser { get; set; }
        public string? StdShiftHours { get; set; }
        public string? AvgTotalTimePerUser { get; set;}
        public string? StdBreakTime { get; set; }
        public string? AvgBreakTimePerUser { get; set;}
        public string? AvgIdleTimePerUser { get; set;}

    }

    public class DayWiseDataTable : AvgTotalDataTable 
    {
        public long? TotalUsers { get; set; }
    }

    public class WeeklyDataTable : DayWiseDataTable
    {
        public string? Date { get; set; }
    }

    public class WorkedLessUserTable
    {
        public string? UserName { get; set;}
        public string? Designation { get; set;}
        public string? StdShiftHours { get; set; }
        public string? TotalTime { get; set; }
        public string? StdBreakTime { get; set; }
        public string? BreakTime { get; set; }
        public string? IdleTime { get; set; }
    }

    public class ReportingManagerTable : AvgTotalDataTable
    {
       public string? ReportingManagerUserName { get; set; }

    }

    public class UsersUnderReportingManagerExcel : WorkedLessUserTable
    {
        public string? ReportingManager { get; set; }
    }
    #endregion
}
