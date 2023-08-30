using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IImportService
    {
        public ConfigData ConfigData { get; set; }
        public Task<ProjectRespVM> ImportProjectList(List<ProjectReqVM> projectReqVM);
        public Task<ProjectDownloadResp> DownloadInvalidRecodes(List<ProjectReqVM> InvalidData);
        public Task<List<ProcessReqVM>> ImportProcessList(List<ProcessReqVM> projectReqVM);
        public Task<List<ImportClientsVM>> ImportClientsExcel(List<ImportClientsVM> req);
        public Task<List<ImportTaskVM>> ImportTaskExcel(List<ImportTaskVM> req);
        public Task<List<ImportUserExcelData>> ImportUserExcel(List<ImportUserExcelData> req);
    }
}
