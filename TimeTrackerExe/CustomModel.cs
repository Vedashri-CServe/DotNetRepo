using Newtonsoft.Json;
using System.Reflection;
using TMS.Auth;
using TMS.Entity;

namespace TimeTrackerExe
{
    public partial class TokenResponseData
    {
        [JsonProperty("TwoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }

        [JsonProperty("Token")]
        public TokenReponseVM? Token { get; set; }
    }

    public class TypedApiResponse<T> : ApiResponse
    {
        public new T? ResponseData { get; set; }
    }

    public class CommonWorkPlanItemVM
    {
        public long WorkPlanId { get; set; }
        public long CpaId { get; set; }
        public long ClientId { get; set; }
        public long TaskId { get; set; }
        public long Quantity { get; set; }
        public long StatusId { get; set; }
    }

    public class WorkPlanDataItemVM : CommonWorkPlanItemVM
    {
        public Exelocation res = new Exelocation();
        public string CpaName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string EstimatedTime { get; set; } = string.Empty;
        public string TotalTime { get;set; } = string.Empty;
        public IOffsetStopwatch SpentTime { get; set; } = new OffsetStopwatch();
        public TimeSpan SpentTimeSpan => SpentTime.Elapsed;
        public Image ActionBtnLbl => SpentTime.IsRunning ? Image.FromFile(Path.Combine(res.exelocation, "stopvector.png")) :
                                       Image.FromFile(Path.Combine(res.exelocation, "startvector.png"));
        public IOffsetStopwatch BreakTime { get; set; } = new OffsetStopwatch();
        public TimeSpan BreakTimeSpan => BreakTime.Elapsed;
        public Image BreakBtnLbl => BreakTime.IsRunning ? Image.FromFile(Path.Combine(res.exelocation, "stopvector.png")) : Image.FromFile(Path.Combine(res.exelocation, "startvector.png"));
    }

    public class TimeTrackerConfig
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }

    }

    public class WorkplanCommentResponseVM
    {
        public long Id { get; set; }
        public string CreatedOn { get; set; }
        public string CommentBy { get; set; }
        public string Comment { get; set; }
    }

    public class WorkplanChecklistResponseVM
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public bool Checked { get; set; }
    }

    public class Exelocation
    {
        public string exelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
    }
}
