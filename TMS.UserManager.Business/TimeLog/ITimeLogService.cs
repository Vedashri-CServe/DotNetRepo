using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface ITimeLogService
    {
        public ConfigData ConfigData { get; set; }
        public Task<int> SaveTimeLog(List<TimeLogReqVM> logs);
        public Task<ITimeLogList> GetTimeLogList(TimeLogFilterVM filter);
        public Task<TimeLogReqVM> SaveTimeDuration(TimeLogReqVM timeLogs);
        public Task<int> SaveEventType(long WorkPlanId, bool status);
        public Task<int> SaveIdleTime(IdleTimeVM idleTime);
        public Task<TaskLogVM> GetLastEventTypeByUserId(long userId);
        public Task<string> GetIdleDuration(IdleTimeFilterVM filter);
        public Task<TimeLogReqVM> GetRefreshTimeDuration(RefreshTimeDuration filter);
        public Task<int> GetLastBreakTypeByUserId(long userId);
        public Task<int> SaveBreakTime(IdleTimeVM breakTime);
        public Task<long> PauseExistingTasksOnLogout(LogoutUserVM user);

        #region Recurring Workplan methods
        public List<RecurringVM> GetAllWorplanRecurringByDate();
        public long WorkplanRecurringOperations(long RecurringWorkplanId, DateTime TimelineDate);
        #endregion
    }
}
