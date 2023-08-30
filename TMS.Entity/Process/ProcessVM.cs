namespace TMS.Entity
{
    public class ProcessIdVM
    {
        public long ProcessId { get; set; }    
    }

    public class ProcessVM : DataItemFieldsVM
    {
        public long ProcessId { get; set; }
        public long ProcessParentId { get; set; }
        public string ProcessName { get; set; }
        public string SubProcessName { get; set; }
        public object? ActivityName { get; set; }
        public decimal EstimatedDuration { get; set; }
        public bool IsProductive { get; set; }
        public bool IsBillable { get; set; }
    }

    public class ProcessRespVM : ProcessVM
    {
        public bool IsAvailable { get; set; }
    }

    public class CPAProcessesRes
    {
        public long ProcessCPAId { get; set; }
        public long ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string SubProcessName { get; set; }
        public object? ActivityName { get; set; }
        public decimal EstimatedDuration { get; set; }
        public int IsProductive { get; set; }
        public int IsBillable { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsCPAmapped { get; set; }
    }

    public interface IProcessListRes
    {

    }
    public class ProcessListNotDownloadRes : DataListVM<ProcessRespVM>, IProcessListRes
    {

    }
    public class ProcessListDownloadRes : IProcessListRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }

    public class ProcessListFilterVM : PageFilterVM
    {
        public string ProcessName { get; set; }
        public decimal? EstimatedDuration { get; set; }
        public long? UserId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsDownload { get; set; }
    }

    public class ActivityName
    {
        public string value { get; set; }
    }

    public class CpaProcessesReq
    {
        public long? CPAId { get; set; }
    }

    public class SaveCpaProcessesReq
    {
        public long CPAId { get; set; }
        public IEnumerable<SaveCpaProcessesReqData> Data { get; set; }
    }

    public class SaveCpaProcessesReqData
    {
        public long? ProcessId { get; set; }

        public bool IsBillable { get; set; }

        public bool IsProductive { get; set; }

        public decimal? EstimatedDuration { get; set; }

    }

    #region SubProcessVM
    public class SubProcessIdVM
    {
        public long SubprocessId { get; set; }
    }
    public class SubProcessVM : DataItemFieldsVM
    {
        public long SubprocessId { get; set; }
        public long ProcessId { get; set; }
        public string SubprocessName { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public class SubProcessListVM : DataListVM<SubProcessResponseVM>
    {

    }

    public class SubProcessListFilterVM : PageFilterVM
    {
        public string SubProcessName { get; set; }
        public long ProcessId { get; set; }
        public long? UserId { get; set; }
        public bool? IsAvailable { get; set; }
        public long ClientId { get; set; }
    }

    public class SubProcessResponseVM 
    {
        public long SubprocessId { get; set; }
        public string SubprocessName { get; set; }
        public bool IsAvailable { get; set; }
        public decimal EstimatedDuration { get; set; }
    }
    #endregion

    public class ProcessreqVM : PageFilterVM
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }  
        public bool? IsAvailable { get; set; }
        public bool IsDownload { get; set; } = false;
    }

    public class OnlyProcessRespVM : DataItemFieldsVM
    {
        public long ProcessId { get; set; }
        public string ProcessName { get; set; }
        public bool Active { get; set; }
    }
    public interface IOnlyProcessListRes
    {

    }
    public class OnlyProcessListNotDownloadRes : DataListVM<OnlyProcessRespVM>, IOnlyProcessListRes
    {

    }
    public class OnlyProcessListDownloadRes : IOnlyProcessListRes
    {
        public byte[]? ByteArray { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }

    public class SaveAndEditProcessVM
    {
        public long ProcessId { get; set; }
        public string ProcessName { get; set; }
    }
}
