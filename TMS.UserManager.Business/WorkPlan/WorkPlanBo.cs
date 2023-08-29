using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2019.Word.Cid;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public class WorkPlanBo : IWorkPlanService
    {
        private readonly QueryFactory _dbContext;
        private readonly ITimeLogService _timeLogService;
        public ConfigData ConfigData { get; set; }
        public WorkPlanBo(QueryFactory dbContext, ITimeLogService timeLogService)
        {
            _dbContext = dbContext;
            _timeLogService = timeLogService;
        }

        #region POST Method
        public async Task<ClientAndTaskListResp> ClientAndTaskAndProcessListByCPA(long CPAId)
        {
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetClientAndTaskAndProcessListByCPA, new
            {
                CPAId = CPAId,
            }, commandType: CommandType.StoredProcedure));
            var resultList = new ClientAndTaskListResp
            {
                ClientList = (await result.ReadAsync<DropdownItemVM>()).ToList(),
                TaskList = (await result.ReadAsync<TaskListResp>()).ToList(),
                ProcessList = (await result.ReadAsync<DropdownItemVM>()).ToList(),
                StatusList = (await result.ReadAsync<StatusListResp>()).ToList(),
            };

            return resultList;
        }

        public async Task<long> SaveWorkPlan(WorkPlanReqVM objWP)
        {
            var WorkPlanDetail = (await _dbContext.Connection.QueryAsync<WorkPlanResultVM>(DBHelper.USP_SaveWorkPlan, new
            {
                Id = objWP.WorkPlanId,
                CPAId = objWP.CPAId,
                ClientId = objWP.ClientId,
                TaskId = objWP.TaskId,
                ProcessId = objWP.ProcessId,
                SubprocessId = objWP.SubprocessId,
                StatusId = objWP.StatusId,
                QuantityNo = objWP.Quantity,
                CurrentUser = ConfigData.UserId,
                TimelineDate = objWP.TimelineDate == default ? DateTime.UtcNow : objWP.TimelineDate,
                Description = objWP.Description,
                IsManual = objWP.IsManual,
                TotalEstimatedTime = objWP.TotalEstimatedTime

            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (WorkPlanDetail != null)
                return WorkPlanDetail.WorkPlanId;


            //check sp response
            return -5;
        }

        public async Task<IWorkPlanList> GetWorkPlanList(WorkPlanFilterVM filter)
        {
            WorkPlanListWithCountVM reviewerLogsList = new WorkPlanListWithCountVM();
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetWorkPlanList, new
            {
                GlobalSearch = filter.GlobalSearch,
                PageNo = filter.Index,
                PageSize = filter.PageSize,
                UserId = ConfigData.UserId,
                TimelineDate = filter.TimelineDate,
                CPAList = string.Join(",", filter.CPAList ?? new List<long>()),
                ClientList = string.Join(",", filter.ClientList ?? new List<long>()),
                TaskList = string.Join(",", filter.TaskList ?? new List<long>()),
                ProcessList = string.Join(",", filter.ProcessList ?? new List<long>()),
                SubProcessList = string.Join(",", filter.SubProcessList ?? new List<long>()),
                ReviewerStatus = string.Join(",", filter.ReviewerStatus ?? new List<long>()),
                WorkPlanStatus = string.Join(",", filter.WorkPlanStatus ?? new List<long>()),
                IsManual = filter.IsManual,
                IsDownload = filter.IsDownload
            }, commandType: CommandType.StoredProcedure));
            var TotalCount = (await result.ReadAsync<long>()).FirstOrDefault();
            var WorkPlanList = (await result.ReadAsync<WorkPlanRespVM>()).ToList();
            var TotalEventTime = await result.ReadSingleAsync<string?>();
            var TotalBreakTime = await result.ReadSingleAsync<string?>();
            var IsCreateWorkPlanButtonEnable = await result.ReadSingleAsync<long>();
            if (filter.IsDownload)
            {         
                    var downloadData = WorkPlanList.Select(x => new
                    {
                        CreatedDate = x.CreatedDate?.ToString("dd-MM-yyyy") ?? (object)DBNull.Value,
                        ClientName = x.CPAName ?? string.Empty,
                        ProjectName = x.ClientName ?? string.Empty,
                        TaskAndProcess = x.TaskName ?? x.ProcessName ?? string.Empty,
                        Subprocess = x.SubprocessName ?? string.Empty,
                        EstimatedTime = x.EstimatedProcessTime == 0 ? TimeSpan.FromMinutes(Convert.ToDouble(x.EstimatedDuration)) : TimeSpan.FromMinutes(Convert.ToDouble(x.EstimatedProcessTime)),
                        Status = x.StatusName,
                        Quantity = x.Quantity,
                        WorkPlan = x.IsManual == true ? "Manual" : "Auto",
                        StandardTime = TimeSpan.FromMinutes(Convert.ToDouble(x.Quantity * (x.EstimatedProcessTime == 0 ? x.EstimatedDuration : x.EstimatedProcessTime))),
                        TotalTime = x.EventDuration?.GetTimeSpan() ?? (object)DBNull.Value,
                        ReviewerStatus = x.ApprovalStatus ?? string.Empty,
                        Description = x.Description ?? string.Empty,
                    }).ToList();
                    var dataTable = downloadData.ListToDataTable();
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(dataTable);

                    // add total rows
                    DataRow row = dataTable.NewRow();

                    DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                     TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    Dictionary<string, string> content = new()
                    {
                        { "Date", $"{filter.TimelineDate ?? exportedDate:dd-MM-yyyy}" },
                        { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" },
                        { "Total Time",$"{TotalEventTime :hh:mm tt}" }
                    };

                    var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);
                    return new WorkPlanListDownloadRes()
                    {
                        ByteArray = byteArray,
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        FileName = "DailyWorkPlanList.xlsx"
                    };                       
            }
            var isBreak = await _timeLogService.GetLastBreakTypeByUserId(ConfigData.UserId);
            var WorkPlanData = WorkPlanList.Select(item => new WorkPlanListRespVM
            {
                WorkPlanId = item.WorkPlanId,
                CPA = new DropdownItemVM
                {
                    Label = item.CPAName,
                    Value = item.CPAId
                },
                Client = new DropdownItemVM
                {
                    Label = item.ClientName,
                    Value = item.ClientId
                },
                Process = new TaskListResp
                {
                    Label = item.ProcessName,
                    Value = item.ProcessId,
                    EstimatedDuration = item.EstimatedProcessTime
                },
                ActivityName = item.ActivityName != null ? JsonConvert.DeserializeObject<List<ActivityName>>(Convert.ToString(item.ActivityName)) : new List<ActivityName>(),
                SubProcess = new DropdownItemVM
                {
                    Label = item.SubprocessName,
                    Value = item.SubprocessId
                },
                Task = new TaskListResp
                {
                    Label = item.TaskName,
                    Value = item.TaskId,
                    EstimatedDuration = item.EstimatedDuration
                },
                Status = new DropdownItemVM
                {
                    Label = item.StatusName,
                    Value = item.StatusId
                },
                Quantity = item.Quantity,
                RecurringStatus = item.RecurringStatus,
                Event = item.Event,
                EventType = item.EventType,
                EventDuration = item.EventDuration,
                IsStop = item.IsStop,
                IsManual = item.IsManual,
                Description = item.Description,
                CreatedDate = item.CreatedDate,
                ApprovalDetails = new DropdownItemVM
                {
                    Value = Convert.ToInt64(item.ApprovalId),
                    Label = Convert.ToString(item.ApprovalStatus)
                },
                SubmittedDate = item.SubmittedDate,
                ModifiedHoursDetails = new DropdownItemVM
                {
                    Label = Convert.ToString(item.ModifiedHours),
                    Value = Convert.ToInt64(item.ModifiedId)
                },
                EmployeeName = item.EmployeeName,
                ReviewerName = item.ReviewerName,
                RejectedComment = item.RejectedComment,

            });

            return new WorkPlanListWithCountVM
            {
                TotalCount = TotalCount,
                IsBreak = isBreak == 15 ? true : false,
                IsCreateWorkPlanButtonEnable = IsCreateWorkPlanButtonEnable == 0,
                WorkPlanList = WorkPlanData.OrderByDescending(x => x.WorkPlanId).ToList(),
                TotalEventTime = TotalEventTime,
                TotalBreakTime = TotalBreakTime
            };

        }

        public async Task<long> DeleteWorkPlan(long WorkPlanId)
        {
            //update Daily Work Plan Table
            var workPlanDetail = (await _dbContext.Connection.QueryAsync<WorkPlanResultVM>(DBHelper.USP_DeleteWorkPlan, new
            {
                WorkPlanId = WorkPlanId,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (workPlanDetail != null)
                return workPlanDetail.WorkPlanId;

            return -5;
        }

        public async Task<long> SaveCheckList(CheckListVM objCl)
        {
            var checkListDetail = (await _dbContext.Connection.QueryAsync<CheckListResultVM>(DBHelper.USP_SaveCheckList, new
            {
                Id = objCl.Id,
                WorkPlanId = objCl.WorkPlanId,
                Description = objCl.Description,
                IsChecked = objCl.IsChecked,
                IsDeleted = objCl.IsDeleted,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response
            if (checkListDetail != null)
                return checkListDetail.checkListId;

            return -5;
        }

        public async Task<List<CheckListVM>> GetCheckList(checkListReqId filter)
        {
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetCheckList, new
            { WorkPlanId = filter.WorkPlanId }, commandType: CommandType.StoredProcedure));
            var resultList = await result.ReadAsync<CheckListVM>();
            return resultList.OrderByDescending(x => x.Id).ToList();
        }

        public async Task<long> DeleteCheckList(DeleteCheckList objDCL)
        {
            //update Work Plan checkList Table
            var checkListDetail = (await _dbContext.Connection.QueryAsync<DeleteCheckList>(DBHelper.USP_DeleteCheckList, new
            {
                CheckListId = objDCL.CheckListId,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (checkListDetail != null)
                return checkListDetail.CheckListId;

            return -5;
        }

        public async Task<long> AddWorkPlanComment(WorkPlanComment objComment)
        {
            var WorkPlanComment = (await _dbContext.Connection.QueryAsync<CommentRespVM>(DBHelper.USP_WorkPlanComment, new
            {
                WorkPlanId = objComment.WorkPlanId,
                Comment = objComment.Comment,
                CurrentUser = ConfigData.UserId,
                IsComment = objComment.IsComment
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response
            if (WorkPlanComment != null)
                return WorkPlanComment.CommentId;

            return -5;
        }

        public async Task<WorkPlanCommentListWithCountVM> GetComment(checkListReqId filter)
        {
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetWorkPlanComment, new
            {
                WorkPlanId = filter.WorkPlanId
            }, commandType: CommandType.StoredProcedure));
            var commentList = new WorkPlanCommentListWithCountVM
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                CommentList = (await result.ReadAsync<CommnetResponseVM>()).OrderByDescending(x => x.Id).ToList()
            };
            return commentList;
        }

        public async Task<long> SaveRecurringPlan(RecurringVM objRP)
        {
            string RepeatCron = "";
            if (objRP.OccurrenceOption.ToUpper().Equals("DAY"))  // for resolving wrong cron for day
            {
                var res = objRP.RecurringCronExp.Split(" ");
                res[2] = res[2].Replace(res[2], string.Format("*/{0}", res[2]));
                RepeatCron = string.Join(" ", res.ToList());
            }
            var RecurringPlanDetail = (await _dbContext.Connection.QueryAsync<SaveRecurringResp>(DBHelper.USP_SaveRecurringPlan, new
            {
                Id = objRP.RecurringId,
                WorkPlanId = objRP.WorkPlanId,
                StartDate = objRP.StartDate,
                EndDate = objRP.EndDate,
                RepeatCron = RepeatCron == "" ? objRP.RecurringCronExp : RepeatCron,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response
            if (RecurringPlanDetail != null)
                return RecurringPlanDetail.RecurringId;

            return -5;
        }

        public async Task<List<RecurringVM>> GetRecurringDeatials(WorkPlanResultVM filter)
        {
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetRecurringPlan, new
            { WorkPlanId = filter.WorkPlanId }, commandType: CommandType.StoredProcedure));
            var resultList = (await result.ReadAsync<RecurringVM>()).OrderByDescending(x => x.RecurringId).ToList();
            resultList.ForEach(x =>
            {
                if (x.RecurringCronExp.Contains("*/"))
                    x.RecurringCronExp = x.RecurringCronExp.Remove(4, 2);
            });
            return resultList;
        }


        public async Task<long> DeleteRecurringPlan(SaveRecurringResp ReqObj)
        {
            //update Recurring Plan Table
            var workPlanDetail = (await _dbContext.Connection.QueryAsync<SaveRecurringResp>(DBHelper.USP_DeleteRecurringPlan, new
            {
                Id = ReqObj.RecurringId,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (workPlanDetail != null)
                return workPlanDetail.RecurringId;

            return -5;
        }

        public async Task<bool> ApproveWorkPlan(ApprovedWorkPlanVM ReqObj)
        {
            if (ReqObj.SelectedArray?.Count > 0)
            {
                foreach (var item in ReqObj.SelectedArray)
                {
                    if (item.Status == (long)ApprovedStatus.Submitted || item.Status == (long)ApprovedStatus.PartiallySubmitted)
                        await _dbContext.Query("daily_work_plan").Where("Id", item.WorkPlanId).UpdateAsync(new
                        {
                            ApprovedStatus = item.Status,
                            Comment = item.Status == (long)ApprovedStatus.Rejected ? ReqObj.RejectedComment : null,
                            SubmittedDate = DateTime.UtcNow,
                            UpdatedBy = ConfigData.UserId,
                            UpdatedOn = DateTime.UtcNow
                        });
                    else
                        await _dbContext.Query("daily_work_plan").Where("Id", item.WorkPlanId).UpdateAsync(new
                        {
                            ApprovedStatus = item.Status,
                            Comment = item.Status == (long)ApprovedStatus.Rejected ? ReqObj.RejectedComment : null,
                            UpdatedBy = ConfigData.UserId,
                            UpdatedOn = DateTime.UtcNow
                        });
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<long> ModifiedReviewLogs(UpdateReviewLogsVM ReqObj)
        {
            var data = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_ModifiedReviewLogs, new
            {
                id = ReqObj.ModifiedId,
                dwpId = ReqObj.WorkPlanId,
                modifiedHour = ReqObj.ModifiedHours,
                currentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            return data;
        }

        public async Task<IReviewerLogList> GetReviewerLogsList(ReviewerLogsReqVm ReqObj)
        {
            ReviewerLogsListRespVM reviewerLogsList = new ReviewerLogsListRespVM();
            var data = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetReviewerWorkPlanList, new
            {
                PageNo = ReqObj.Index,
                PageSize = ReqObj.PageSize,
                ReportingManagerId = ConfigData.UserId,
                UserIds = string.Join(",", ReqObj.UserIds ?? new List<long>()),
                TimelineDate = ReqObj.TimelineDate,
                CPAList = string.Join(",", ReqObj.CPAList ?? new List<long>()),
                ClientList = string.Join(",", ReqObj.ClientList ?? new List<long>()),
                TaskList = string.Join(",", ReqObj.TaskList ?? new List<long>()),
                ProcessList = string.Join(",", ReqObj.ProcessList ?? new List<long>()),
                SubProcessList = string.Join(",", ReqObj.SubProcessList ?? new List<long>()),
                ReviewerStatus = string.Join(",", ReqObj.ReviewerStatus ?? new List<long>()),
                WorkPlanStatus = string.Join(",", ReqObj.WorkPlanStatus ?? new List<long>()),
                IsManual = ReqObj.IsManual,
                GlobalSearch = ReqObj.GlobalSearch,
                IsDownload = ReqObj.IsDownload
            }, commandType: CommandType.StoredProcedure);

            if (data != null)
            {
                reviewerLogsList = new ReviewerLogsListRespVM()
                {
                    List = (await data.ReadAsync<ReviewerLogsRespVM>()).ToList(),
                    TotalCount = (await data.ReadAsync<long>()).FirstOrDefault(),
                    TotalEventTime = (await data.ReadSingleAsync<string?>()),
                    TotalQty = Convert.ToInt64(await data.ReadSingleAsync<string?>()),
                    TotalStdHours = (await data.ReadSingleAsync<string?>()) ?? string.Empty,
                    TotalModifiedHours = (await data.ReadSingleAsync<string?>()) ?? string.Empty,
                };
            }

            if (ReqObj.IsDownload)
            {
                var downloadData = reviewerLogsList.List.Select(x =>
                   new
                   {
                       EmployeeName = x.EmployeeName ?? string.Empty,
                       Designation = x.RoleType ?? string.Empty,
                       Reviewer = x.ReviewerName ?? string.Empty,
                       StartDateTime = x.StartDateTime.ToString() ?? (object)DBNull.Value,
                       EndDateTime = x.EndDateTime.ToString() ?? (object)DBNull.Value,
                       Client = string.IsNullOrEmpty(x.CPAName) ? "-" : x.CPAName,
                       Project = string.IsNullOrEmpty(x.ClientName) ? "-" : x.ClientName,
                       TaskProcessName = x.TaskName ?? x.ProcessName ?? "-",
                       SubProcess = string.IsNullOrEmpty(x.SubprocessName) ? "-" : x.SubprocessName,
                       IsManual = x.IsManual == true ? "Yes" : "No",
                       Quantity = x.Quantity,
                       StandardHours = TimeSpan.FromMinutes(Convert.ToDouble(x.Quantity * (x.EstimatedProcessTime == 0 ? x.EstimatedDuration : x.EstimatedProcessTime))),
                       TotalHours = x.EventDuration?.GetTimeSpan() ?? (object)DBNull.Value,
                       EditHours = x.ModifiedHours?.GetTimeSpan() ?? (object)DBNull.Value,
                       ReviewerStatus = string.IsNullOrEmpty(x.ApprovalStatus) ? "-" : x.ApprovalStatus,
                       Efficiency = x.Efficiency ?? string.Empty,
                       Description = string.IsNullOrEmpty(x.Description) ? "-" : x.Description,
                       Reason = string.IsNullOrEmpty(x.Reason) ? "-" : x.Reason,
                       CreatedDate = x.CreatedDate ?? (object)DBNull.Value,
                       SubmittedDate = x.SubmittedDate ?? (object)DBNull.Value,
                       EstimatedHours = x.EstimatedProcessTime == 0 ? TimeSpan.FromMinutes(Convert.ToDouble(x.EstimatedDuration)) : TimeSpan.FromMinutes(Convert.ToDouble(x.EstimatedProcessTime)),
                       Status = string.IsNullOrEmpty(x.StatusName) ? "-" : x.StatusName,
                   }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                // add total rows
                DataRow row = dataTable.NewRow();
                row["Quantity"] = reviewerLogsList.TotalQty;
                row["StandardHours"] = reviewerLogsList.TotalStdHours?.GetTimeSpan() ?? (object)DBNull.Value;
                row["TotalHours"] = reviewerLogsList.TotalEventTime?.GetTimeSpan() ?? (object)DBNull.Value;
                row["EditHours"] = reviewerLogsList.TotalModifiedHours?.GetTimeSpan() ?? (object)DBNull.Value;
                dataTable.Rows.Add(row);


                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                 TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                    {
                        { "Date", $"{ReqObj.TimelineDate ?? exportedDate:dd-MM-yyyy}" },
                        { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                    };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);
                return new ReviewerLogsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "ReviewerLogsList.xlsx"
                };
            }
            return reviewerLogsList;

        }

        public async Task<List<DropdownItemVM>> GetEmployeeList()
        {
            return (await _dbContext.Query("user").
            Where(new { parent_id = ConfigData.UserId, is_deleted = false }).
                    Select("id as Value").SelectRaw("CONCAT(first_name,' ',last_name) AS Label").GetAsync<DropdownItemVM>()).OrderBy(x => x.Label).ToList();
        }

        public async Task<List<DropdownItemVM>> GetApprovalStatusList()
        {
            var parentId = _dbContext.Query("lookup_values").WhereFalse("is_deleted").Where("short_description", "=", "APPROVEDSTATUS").Select("id");
            return (await _dbContext.Query("lookup_values").
                        Where("parent_id", "=", parentId).WhereFalse("is_deleted").
                    Select("id as Value", "description as Label").OrderBy("description").GetAsync<DropdownItemVM>()).ToList();
        }

        #endregion
    }
}
