using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Entity
{
    public class TimeLogVM : ApprovalVM
    {
        public long UserTableId { get; set; }
        public string UserName { get; set; }
        public string CPAName { get; set; }
        public string ClientName { get; set; }
        public long? TaskId { get; set; }
        public string? TaskName { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? TotalTime { get; set; }
        public string? BreakStartTime { get; set; }
        public string? BreakEndTime { get; set; }
        public TimeSpan? BreakTotalTime { get; set; }
        public long? ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public long? SubprocessId { get; set; }
        public string? SubprocessName { get; set; }
        public object ActivityName { get; set; }
        public string? TimeLogStartDate { get; set; }
        public string? TimeLogEndDate { get; set;}
        public bool IsManual { get; set; } = false;

    }

    public interface ITimeLogList { }
    public class TimeLogListVM : DataListVM<TimeLogVM>, ITimeLogList
    {
        public string? TotalEventTime { get; set; }
    }
    public class TimeLogsDownloadRes : ITimeLogList
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
    public class TimeLogFilterVM : PageFilterVM
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? UserId { get; set; }
        public List<long>? CPAList { get; set; }
        public List<long>? ClientList { get; set; }
        public List<long>? TaskList { get; set; }
        public List<long>? ProcessList { get; set; }
        public List<long>? SubProcessList { get; set; }
        public bool IsDownload { get; set; } = false;

    }

    public class RefreshTimeDuration
    {
        public DateTime TimelineDate { get; set; }
        public long? UserId { get; set; }
        public long? WorkPlanId { get; set; }
    }
}
