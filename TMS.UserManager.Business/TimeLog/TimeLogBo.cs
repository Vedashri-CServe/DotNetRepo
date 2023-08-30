using Dapper;
using Newtonsoft.Json;
using SqlKata.Execution;
using System.Data;
using System.Globalization;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.ExportExcel;
using TMS.Helper.UtilityHelper;

namespace TMS.UserManager.Business
{
    public class TimeLogBo : ITimeLogService
    {
        private readonly QueryFactory _dbContext;
        public ConfigData ConfigData { get; set; }
        public TimeLogBo(QueryFactory dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> SaveTimeLog(List<TimeLogReqVM> logs)
        {
            var itemListCols = new[] { "work_plan_id", "event_time", "event_type", "is_deleted", "created_on", "created_by", "event_duration" };
            if (logs != null && logs.Count > 0)
            {
                //construct insert request
                var insertRecord = logs.Select(x => new object[]
                {
                         x.WorkPlanId,
                         x.EventTime,
                         x.EventType,
                         0,
                         DateTime.UtcNow,
                         ConfigData.UserId,
                         x.EventDuration
                }).ToArray();
                // check insert record
                if (insertRecord.Length > 0)
                {
                    await _dbContext.Query("task_log").InsertAsync(itemListCols, insertRecord);
                }
                return 0;
            }
            else
                return -1;


        }

        public async Task<ITimeLogList> GetTimeLogList(TimeLogFilterVM filter)
        {
            TimeLogListVM timeLogsList = new TimeLogListVM();
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetTimeLineDetails, new
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                UserId = filter.UserId,
                CPAList = string.Join(",", filter.CPAList ?? new List<long>()),
                ClientList = string.Join(",", filter.ClientList ?? new List<long>()),
                TaskList = string.Join(",", filter.TaskList ?? new List<long>()),
                ProcessList = string.Join(",", filter.ProcessList ?? new List<long>()),
                SubProcessList = string.Join(",", filter.SubProcessList ?? new List<long>()),
                GlobalSearch = filter.GlobalSearch,
                PageNo = filter.PageNo == default ? 1 : filter.PageNo,
                PageSize = filter.PageSize == default ? 1000 : filter.PageSize,
                IsDownload = filter.IsDownload
            }, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                timeLogsList = new TimeLogListVM()
                {
                    TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                    List = (await result.ReadAsync<TimeLogVM>()).ToList(),
                    TotalEventTime = (await result.ReadSingleAsync<string?>())
                };
            }
            if (filter.IsDownload)
            {
                var downloadData = timeLogsList.List.Select(x => new
                {
                    StartDate = x.TimeLogStartDate ?? (object)DBNull.Value,
                    EndDate = x.TimeLogEndDate ?? (object)DBNull.Value,
                    StartTime = x.StartTime ?? (object)DBNull.Value,
                    EndTime = x.EndTime ?? (object)DBNull.Value,
                    TotalTime = x.TotalTime?.GetTimeSpan() ?? (object)DBNull.Value,
                    ClientName = x.CPAName ?? string.Empty,
                    ProjectName = x.ClientName ?? string.Empty,
                    TaskAndProcess = x.TaskName ?? x.ProcessName ?? string.Empty,
                    Subprocess = x.SubprocessName ?? string.Empty,
                    IsManual = x.IsManual == true ? "Yes" : "No"
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                // add total rows
                DataRow row = dataTable.NewRow();

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                 TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                var filterDate = filter.EndDate == null ? $"{filter.StartDate ?? exportedDate:dd-MM-yyyy}"
                    : $"{filter.StartDate ?? exportedDate:dd-MM-yyyy} to {filter.EndDate ?? exportedDate:dd-MM-yyyy}";
                Dictionary<string, string> content = new()
                    {
                        { "Date", $"{filterDate}" },
                        { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" },
                    { "Total Time",$"{timeLogsList.TotalEventTime :hh:mm tt}" }
                    };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);
                return new TimeLogsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "TimelineList.xlsx"
                };
            }

            foreach (var Process in timeLogsList.List)
            {
                Process.ActivityName = Process.ActivityName != null ? JsonConvert.DeserializeObject<List<ActivityName>>(Convert.ToString(Process.ActivityName)) : new List<ActivityName>();
            }
            return timeLogsList;
        }

        public async Task<TimeLogReqVM> SaveTimeDuration(TimeLogReqVM timeLogs)
        {

            if (timeLogs != null)
            {
                var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_SaveTimeDuration, new
                {
                    CreatedBy = ConfigData.UserId,
                    EventType = timeLogs.EventType,
                    WorkPlanId = timeLogs.WorkPlanId,
                    EventTime = DateTime.UtcNow,
                    IsStop = timeLogs.IsStop.ToUpper()
                }, commandType: CommandType.StoredProcedure);

                var insertedTimeLog = (await result.ReadAsync<TimeLogReqVM>()).ToList();
                return insertedTimeLog.FirstOrDefault();
            }
            else
                return null;
        }


        public async Task<int> SaveEventType(long WorkPlanId, bool status)
        {
            if (WorkPlanId != 0)
            {
                var result = await _dbContext.Connection.QueryAsync(DBHelper.USP_SaveEventType, new
                {
                    EventStatus = status,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = ConfigData.UserId,
                    WorkPlanId = WorkPlanId
                }, commandType: CommandType.StoredProcedure);

                var insertedTimeLog = result?.FirstOrDefault().WorkPlanId;
                return (int)insertedTimeLog;
            }
            else
                return -1;
        }


        public async Task<int> SaveIdleTime(IdleTimeVM idleTime)
        {
            if (idleTime != null)
            {
                var result = await _dbContext.Connection.QueryAsync(DBHelper.USP_SaveIdleTime, new
                {
                    UserId = idleTime.UserId,
                    CreatedBy = ConfigData.UserId,
                    StartTime = idleTime.StartTime,
                    EndTime = idleTime.EndTime,
                    IsDeleted = 0,
                    EventType = idleTime.EventType

                }, commandType: CommandType.StoredProcedure);
                return result.Count();
            }
            else
                return -1;
        }

        public async Task<TaskLogVM> GetLastEventTypeByUserId(long userId)
        {
            if (userId > 0)
            {
                var result = await _dbContext.Query("task_log").Where(new
                {
                    created_by = userId,
                }).WhereFalse("is_deleted").WhereRaw("date(event_time) = date(NOW())").OrderByDesc("id").
                Select("id as TaskId", "event_type as EventType", "created_by as CreatedBy", "event_time as EventTime", "event_duration as EventDuration", "work_plan_id as WorkPlanId")
               .FirstOrDefaultAsync<TaskLogVM>();

                return result;
            }
            else
                return null;
        }


        public async Task<string> GetIdleDuration(IdleTimeFilterVM filter)
        {
            var result = await _dbContext.Connection.QueryAsync(DBHelper.USP_GetIdleDuration, new
            {
                TimelineDate = filter.TimelineDate == default ? DateTime.UtcNow : filter.TimelineDate.Date,
                UserId = filter.UserId

            }, commandType: CommandType.StoredProcedure);

            var idleDuration = result?.FirstOrDefault().TotalIdleTime;

            return idleDuration;
        }

        public async Task<TimeLogReqVM> GetRefreshTimeDuration(RefreshTimeDuration filter)
        {
            if (filter.UserId != null)
            {
                TaskLogVM LastEventType = new TaskLogVM();
                LastEventType = await _dbContext.Query("task_log")
                                              .WhereFalse("is_deleted")
                                              .WhereRaw((filter.WorkPlanId != null) ? (string.Format("(created_by = {0})" +
                                              "AND (work_plan_id = {1})", (long)filter.UserId, filter.WorkPlanId)) :
                                              (string.Format("(created_by = {0})", (long)filter.UserId)))
                                              .OrderByDesc("id")
                                              .Select("id as TaskId", "event_type as EventType", "created_by as CreatedBy", "event_time as EventTime", "event_duration as EventDuration", "work_plan_id as WorkPlanId")
                                              .FirstOrDefaultAsync<TaskLogVM>();

                if (LastEventType != null)
                {
                    switch (LastEventType.EventType)
                    {
                        case 11:
                            var result = filter.TimelineDate.Subtract(LastEventType.EventTime);
                            return new TimeLogReqVM
                            {
                                TaskLogId = LastEventType.TaskId,
                                EventDuration = result.ToString(@"hh\:mm\:ss"),
                                EventType = LastEventType.EventType,
                                WorkPlanId = (long)LastEventType.WorkPlanId
                            };

                        case 12:
                            return new TimeLogReqVM
                            {
                                TaskLogId = LastEventType.TaskId,
                                EventDuration = LastEventType.EventDuration.HasValue ? LastEventType.EventDuration.ToString() : null,
                                EventType = LastEventType.EventType,
                                WorkPlanId = (long)LastEventType.WorkPlanId
                            };

                        case 13:
                            var lastPauseDuration = LastEventType.EventDuration;
                            var lastResumeDuration = filter.TimelineDate.Subtract(LastEventType.EventTime);
                            var result2 = lastResumeDuration.Add((TimeSpan)lastPauseDuration);

                            return new TimeLogReqVM
                            {
                                TaskLogId = LastEventType.TaskId,
                                EventDuration = result2.ToString(@"hh\:mm\:ss"),
                                EventType = LastEventType.EventType,
                                WorkPlanId = (long)LastEventType.WorkPlanId
                            };

                        default:
                            return null;
                    }
                }
            }
            return null;
        }

        public List<RecurringVM> GetAllWorplanRecurringByDate()
        {
            var result = _dbContext.Connection.QueryMultiple(DBHelper.USP_GetAllWorplanRecurringByDate, new
            { }, commandType: CommandType.StoredProcedure);


            var list = result.ReadAsync<RecurringVM>();

            return list.Result.Count() > 0 ? list.Result.ToList() : null;
        }

        public long WorkplanRecurringOperations(long RecurringWorkplanId, DateTime TimelineDate)
        {
            var result = _dbContext.Connection.Query(DBHelper.USP_WorkplanRecurringOperations, new
            {
                TimelineDate = TimelineDate == default ? DateTime.UtcNow : TimelineDate.Date,
                RecurringWorkplanId = RecurringWorkplanId

            }, commandType: CommandType.StoredProcedure);

            var workPlanId = result == null ? -1 : result?.FirstOrDefault().insertedDailyWorkplanId;

            return workPlanId;

        }

        public async Task<int> GetLastBreakTypeByUserId(long userId)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetLastBreakTypeByUserId, new
            {
                UserId = userId
            }, commandType: CommandType.StoredProcedure);

            var list = await result.ReadAsync<IdleTimeVM>();
            if (list != null && list.Count() > 0)
            {
                return Convert.ToInt32(list.FirstOrDefault().EventType);
            }
            return -1;
        }

        public async Task<int> SaveBreakTime(IdleTimeVM breakTime)
        {
            if (breakTime != null)
            {
                var result = await _dbContext.Connection.QueryAsync(DBHelper.USP_SaveBreakTime, new
                {
                    UserId = breakTime.UserId,
                    CreatedBy = ConfigData.UserId,
                    StartTime = breakTime.StartTime,
                    EndTime = breakTime.EndTime,
                    IsDeleted = 0,
                    EventType = breakTime.EventType

                }, commandType: CommandType.StoredProcedure);
                return result.Count();
            }
            else
                return -1;
        }


        public async Task<long> PauseExistingTasksOnLogout(LogoutUserVM user)
        {
            if (user != null)
            {
                try
                {
                    var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_PauseExistingTasksOnLogout, new
                    {
                        UserId = user.UserId,
                        LogoutTime = user.LogoutTime
                    }, commandType: CommandType.StoredProcedure);
                    return await result.ReadSingleAsync<long>();

                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            else
                return -1;
        }
    }
}
