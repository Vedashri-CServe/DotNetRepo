using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Entity
{
    public class TaskVM
    {
        public long TaskId { get; set; }
        public object OrganizationList { get; set; }
        public string TaskName { get; set; }
        public decimal EstimatedDuration { get; set; }
        public bool IsProductive { get; set; }
        public bool IsBillable { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedBy { get; set; }
    }

    public class TaskRespVM : TaskVM
    {
        public bool IsAvailable { get; set; }
    }

    public class TaskLogVM
    {
        public long TaskLogId { get; set; }
        public long TaskId { get; set; }
        public DateTime EventTime { get; set; }
        public int EventType { get; set; }
        public TimeSpan? EventDuration { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedBy { get; set; }
        public long? WorkPlanId { get; set; }
    }

    public class TaskVMResultVM
    {
        public long TaskId { get; set; }
    }

    public class TaskListFilterVM
    {
        public string GlobalSearch { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public string TaskName { get; set; }
        public long? UserId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsDownload { get; set; }
    }
    public interface ITaskListRes
    {

    }
    public class TaskListNotDownloadRes : DataListVM<TaskRespVM>, ITaskListRes
    {

    }
    public class TaskListDownloadRes : ITaskListRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }

    public class TimeLogResultVM
    {
        public long TaskLogId { get; set; }
    }

    public class TimeLogReqVM : TimeLogResultVM
    {
        public long WorkPlanId { get; set; }
        public DateTime EventTime { get; set; }
        public int EventType { get; set; }
        public string EventDuration { get; set; }
        public string IsStop { get; set; } = "Pause";
        public bool IsSync { get; set; } = false;
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class IdleTimeVM
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Duration { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public int? EventType { get; set; }
    }

    public class IdleTimeFilterVM : PageFilterVM
    {
        public DateTime TimelineDate { get; set; }
        public long? UserId { get; set; }
        public long? CPAId { get; set; }
        public long? ClientId { get; set; }
    }
    public enum TaskLogEventType
    {
        Start = 11,
        Pause = 12,
        Continue = 13,
        Stop = 14,
        BreakStart = 15,
        BreakEnd = 16,
        IdleStart = 17,
        IdleStop = 18
    }

    public class GetCpaListVM : DropdownItemVM
    {
        public long? CPATypeOfWork { get; set; }
    }
}