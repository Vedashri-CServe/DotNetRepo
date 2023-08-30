using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IReportService
    {
        public ConfigData ConfigData { get; set; }

        public Task<ICPAReportRes> GetCpaReport(CpaReportReq req);

        public Task<IClientReportRes> GetClientReport(ClientReportReq req);

        public Task<IAPReportRes> GetActualPlannedReport(ActualPlannedReportReq req);

        public Task<IActivityReportRes> GetActivityReport(ActivityReportReq req);

        public Task<IOtherReportRes> GetOtherReport(OtherReportReq req);

        public Task<IKRAReportRes> GetKRAReport(KRAReqVM req);

        public Task<IAutoManualReportRes> GetAutoManualReport(AutoManualReportReq req);

        public Task<ILoginLogoutReportRes> GetLoginLogoutReport(LoginLogoutReportReq req);

        public Task<IAuditReportRes> GetAuditReport(AuditReportReq req);

        public Task SendDailyReportEmail();

        public Task SendManagementReportEmail();
    }
}
