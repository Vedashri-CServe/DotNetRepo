using Dapper;
using Newtonsoft.Json;
using SqlKata.Execution;
using System.Data;
using System.Net.Mail;
using System.Text;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.ExportExcel;
using TMS.Helper.UtilityHelper;

namespace TMS.UserManager.Business
{
    public class ReportBo : IReportService
    {
        private readonly QueryFactory _dbContext;

        private readonly IEmailHelper _emailHelper;
        private readonly IDailyEmailHelper _dailyEmailHelper;
        private readonly IManagementReportEmailHelper _managementReportEmailHelper;

        public ConfigData ConfigData { get; set; }

        public ReportBo(QueryFactory dbContext, IEmailHelper emailHelper, IDailyEmailHelper dailyEmailHelper,IManagementReportEmailHelper managementReportEmailHelper)
        {
            _dbContext = dbContext;
            _emailHelper = emailHelper;
            _dailyEmailHelper = dailyEmailHelper;
            _managementReportEmailHelper = managementReportEmailHelper;
        }

        public async Task<ICPAReportRes> GetCpaReport(CpaReportReq req)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetCpaReport, new
            {
                PageNo = req.PageNo ?? 1,
                PageSize = req.PageSize ?? 10,
                req.GlobalSearch,
                req.SortColumn,
                req.IsDesc,
                req.TypeOfWork,
                req.BillType,
                Clients = req.Clients != null && req.Clients.Count > 0 ? JsonConvert.SerializeObject(req.Clients) : null,
                req.Available,
                CurrentUserId = ConfigData.UserId,
                req.StartDate,
                req.EndDate,
                req.IsDownload,
                Departments = (req.Departments?.Any()).GetValueOrDefault() ? JsonConvert.SerializeObject(req.Departments) : null,
            }, commandType: CommandType.StoredProcedure);

            var resp = new CPAReportNotDownloadRes()
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<CpaReportData>()).ToList()
            };

            if (req.IsDownload)
            {
                var downloadData = resp.List.Select(x => new
                {
                    ClientName = x.CPAName,
                    TypeOfWork = x.TypeOfWork,
                    BillingType = x.BillType,
                    InternalHours = x.InternalHours?.GetTimeSpan(),
                    ContractedHours = x.ContractedHours?.GetTimeSpan(),
                    STDTime = x.ActualHours?.GetTimeSpan(),
                    EditHours = x.TotalModifiedHours?.GetTimeSpan(),
                    TotalTime = x.TotalTimeSpent?.GetTimeSpan(),
                    Difference = x.Difference?.GetTimeSpan(),
                    ContractedDiff = x.ContractedDiff?.GetTimeSpan(),
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);
                
                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
                                        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                    { "Date Range", $"{req.StartDate ?? startDate:dd-MM-yyyy} to {req.EndDate ?? now:dd-MM-yyyy}" },
                    { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);
                return new CPAReportsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "ClientReport.xlsx"
                };
            }
            return resp;
        }

        public async Task<IClientReportRes> GetClientReport(ClientReportReq req)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetClientReport, new
            {
                PageNo = req.PageNo ?? 1,
                PageSize = req.PageSize ?? 10,
                req.GlobalSearch,
                req.SortColumn,
                req.IsDesc,
                req.TypeOfWork,
                req.BillType,
                Clients = req.Clients != null && req.Clients.Count > 0 ? JsonConvert.SerializeObject(req.Clients) : null,
                Projects = req.Projects != null && req.Projects.Count > 0 ? JsonConvert.SerializeObject(req.Projects) : null,
                req.Available,
                CurrentUserId = ConfigData.UserId,
                req.StartDate,
                req.EndDate,
                req.IsDownload,
                Departments = (req.Departments?.Any()).GetValueOrDefault() ? JsonConvert.SerializeObject(req.Departments) : null,
            }, commandType: CommandType.StoredProcedure);

            var resp = new ClientReportNotDownloadRes()
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<ClientReportData>()).ToList()
            };

            if (req.IsDownload)
            {
                var downloadData = resp.List.Select(x => new
                {
                    Projrct = x.ClientName,
                    Client = x.CPAName,
                    TypeOfWork = x.CPATypeOfWork,
                    BiliingType = x.CPABillType,
                    InternalHours = x.EstimatedHours?.GetTimeSpan(),
                    STDTime = x.ActualHours?.GetTimeSpan(),
                    EditHours = x.TotalModifiedHours?.GetTimeSpan(),
                    TotalTime = x.TotalTimeSpent?.GetTimeSpan(),
                    Difference = x.Difference?.GetTimeSpan()
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                       TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                    { "Date Range", $"{req.StartDate ?? startDate:dd-MM-yyyy} to {req.EndDate ?? now:dd-MM-yyyy}" },
                    { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);

                return new ClietReportsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "ProjectReport.xlsx"
                };
            }

            return resp;
        }

        public async Task<IAPReportRes> GetActualPlannedReport(ActualPlannedReportReq req)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetActualPlannedReport, new
            {
                PageNo = req.PageNo ?? 1,
                PageSize = req.PageSize ?? 10,
                req.GlobalSearch,
                req.SortColumn,
                req.IsDesc,
                Clients = req.Clients != null && req.Clients.Count > 0 ? JsonConvert.SerializeObject(req.Clients) : null,
                Projects = req.Projects != null && req.Projects.Count > 0 ? JsonConvert.SerializeObject(req.Projects) : null,
                ReportingTo = req.Reporting != null && req.Reporting.Count > 0 ? JsonConvert.SerializeObject(req.Reporting) : null,
                Users = (req.Users?.Any()).GetValueOrDefault() ? JsonConvert.SerializeObject(req.Users) : null,
                req.Available,
                CurrentUserId = ConfigData.UserId,
                req.StartDate,
                req.EndDate,
                req.IsDownload,
                Departments = (req.Departments?.Any()).GetValueOrDefault() ? JsonConvert.SerializeObject(req.Departments) : null,
            }, commandType: CommandType.StoredProcedure);

            var resp = new APReportNotDownloadRes()
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<ActualPlannedReportData>())?.Select(o => new ActualPlannedReportData()
                {
                    UserId = o.UserId,
                    UserName = o.UserName,
                    DailyWorkPlanId = o.DailyWorkPlanId,
                    TaskId = o.TaskId,
                    TaskName = o.TaskName,
                    SubProcessId = o.SubProcessId,
                    SubProcessName = o.SubProcessName,
                    EstimatedHours = o.EstimatedHours,
                    CreatedOn = o.CreatedOn,
                    CreatedById = o.CreatedById,
                    ReportingManager = o.ReportingManager,
                    TotalTimeSpent = o.TotalTimeSpent,
                    DifferencePercentage = o.DifferencePercentage,
                    ClientId = o.ClientId,
                    ClientName = o.ClientName,
                    CPAId = o.CPAId,
                    CPAName = o.CPAName,
                    IsTask = o.IsTask,
                    TotalModifiedHours = o.TotalModifiedHours,
                    IsAvailable = o.IsAvailable,
                    Activities = !string.IsNullOrEmpty(o.ActivitiesJson) ? JsonConvert.DeserializeObject<List<ActivityName>>(o.ActivitiesJson!) : null,
                    Description = o.Description,
                    Comment = o.Comment,
                    Reason = o.Reason,
                    IsManual = o.IsManual,
                }).ToList()!
            };

            if (req.IsDownload)
            {
                var downloadData = resp.List.Select(x => new
                {
                    ClientName = x.CPAName,
                    Project = x.ClientName,
                    Task = x.TaskName,
                    SubProcess = x.SubProcessName,
                    AssignTo = x.UserName,
                    x.CreatedOn,
                    x.Description,
                    x.Comment,
                    x.Reason,
                    WorkItemType = x.IsManual ? "Manual" : "Auto",
                    ReportingTo = x.ReportingManager,
                    STDTime = x.EstimatedHours?.GetTimeSpan(),
                    TotalTime = x.TotalTimeSpent?.GetTimeSpan(),
                    Difference = x.DifferencePercentage
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                       TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                    { "Date Range", $"{req.StartDate ?? startDate:dd-MM-yyyy} to {req.EndDate ?? now:dd-MM-yyyy}" },
                    { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);

                return new APReportsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "ActualPlannedReport.xlsx"
                };
            }

            return resp;
        }

        public async Task<IActivityReportRes> GetActivityReport(ActivityReportReq req)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetActivityReport, new
            {
                PageNo = req.PageNo ?? 1,
                PageSize = req.PageSize ?? 10,
                req.SortColumn,
                req.IsDesc,
                req.Available,
                req.StartDate,
                req.EndDate,
                CurrentUserId = ConfigData.UserId,
                req.IsDownload,
                UserIdsForFilter = req.Users != null && req.Users.Count > 0 ? JsonConvert.SerializeObject(req.Users) : null,
                Departments = req.Departments != null && req.Departments.Count > 0 ? JsonConvert.SerializeObject(req.Departments) : null,
                req.GlobalSearch
            }, commandType: CommandType.StoredProcedure);

            var resp = new
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<ActivityReportData>()).ToList()
            };

            /*//for hierarchy
            List<Tree<ActivityReportData>>? hierarchyData = new List<Tree<ActivityReportData>>();

            var root = new Tree<ActivityReportData>();
            root.Parent = resp.List?.Find(x => x.UserId == ConfigData.UserId);

            //for super admin
            foreach (var node in resp.List!)
            {
                if (node.UserId != 1) continue;
                node.ParentId = 0;
            }

            var generatedTree = resp.List!.GenerateTree<ActivityReportData, long>(x => x.UserId, y => y.ParentId, ConfigData.UserId);

            if (root.Parent != null)
            {
                root.Children = generatedTree;
                hierarchyData.Add(root);
            }
            else
                hierarchyData = generatedTree.ToList();

            if (!string.IsNullOrEmpty(req.GlobalSearch))
            {
                hierarchyData = TreeBuilder.Descendants(hierarchyData?.FirstOrDefault())?.Where(n => n.Parent.UserName.ToLower().Contains(req.GlobalSearch?.ToLower())).ToList();
            }
            else if (req.Users!.Count() > 0 || req.Departments!.Count() > 0)
            {
                var userfilteredData = new List<Tree<ActivityReportData>>();
                var deptfilteredData = new List<Tree<ActivityReportData>>();
                if (req.Users?.Count() > 0)
                {
                    userfilteredData = TreeBuilder.Descendants(hierarchyData?.FirstOrDefault())?.
                    Where(n => req.Users.Contains(n.Parent.UserId)).ToList();
                }
                if (req.Departments?.Count() > 0)
                {
                    deptfilteredData = TreeBuilder.Descendants(hierarchyData?.FirstOrDefault())?.
                    Where(n => req.Departments.Contains(n.Parent.DepartmentId)).ToList();
                }

                if(userfilteredData.Count > 0)
                {
                    if(deptfilteredData.Count > 0)
                    {
                        hierarchyData = deptfilteredData!.Intersect(userfilteredData!).ToList();
                    }
                    else
                    hierarchyData = userfilteredData.ToList();
                }
                else
                    hierarchyData = deptfilteredData.ToList();
                
            }

            var flatData = TreeBuilder.Flatten(hierarchyData).ToList();
         */
            if (req.IsDownload)
            {
                var downloadData = resp.List!.Select(x => new
                {
                    User = x.UserName,
                    TotalHours = x.TotalHrs?.GetTimeSpan(),
                    Productive = x.ProductiveHrs?.GetTimeSpan(),
                    Unproductive = x.UnProductiveHrs?.GetTimeSpan(),
                    Billable = x.BillableHrs?.GetTimeSpan(),
                    NonBillable = x.NonBillableHrs?.GetTimeSpan()
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                       TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                    { "Date Range", $"{req.StartDate ?? startDate:dd-MM-yyyy} to {req.EndDate ?? now:dd-MM-yyyy}" },
                    { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);

                return new ActivityReportsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "ActivityReport.xlsx"
                };
            }

            /* return new ActivityReportNotDownloadRes()
             {
                 TotalCount = hierarchyData!.Count,
                 List = hierarchyData.ToList()
             };*/
            return new ActivityReportNotDownloadRes()
            {
                TotalCount = resp.List!.Count,
                List = resp.List.ToList()
            };
        }

        public async Task<IOtherReportRes> GetOtherReport(OtherReportReq req)
        {

            long totalRecords = 0;
            List<OtherReportData>? otherReportDatas = new List<OtherReportData>();
            List<DateTimeLog>? dateTimeLogs = new List<DateTimeLog>();
            List<LogDetail>? logDetails = new List<LogDetail>();

            if (req.OtherReportIdentifier == OtherReportIdentifier.UserReport || req.OtherReportIdentifier == OtherReportIdentifier.WorkloadReport)
            {
                var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetOtherReport, new
                {
                    PageNo = req.PageNo ?? 1,
                    PageSize = req.PageSize ?? 10,
                    req.GlobalSearch,
                    req.SortColumn,
                    req.IsDesc,
                    CurrentUserId = ConfigData.UserId,
                    req.StartDate,
                    req.EndDate,
                    UserIdsForFilter = (req.Users?.Any()).GetValueOrDefault() ? JsonConvert.SerializeObject(req.Users) : null,
                    Departments = (req.Departments?.Any()).GetValueOrDefault() ? JsonConvert.SerializeObject(req.Departments) : null,
                    req.IsDownload
                }, commandType: CommandType.StoredProcedure);

                totalRecords = (await result.ReadAsync<long>()).FirstOrDefault();
                otherReportDatas = (await result.ReadAsync<OtherReportData>())?.ToList();
                dateTimeLogs = (await result.ReadAsync<DateTimeLog>())?.ToList();
                logDetails = (await result.ReadAsync<LogDetail>())?.Select(o => new LogDetail()
                {
                    UserId = o.UserId,
                    UserName = o.UserName,
                    LogDate = o.LogDate,
                    DailyWorkPlanId = o.DailyWorkPlanId,
                    TaskId = o.TaskId,
                    TaskName = o.TaskName,
                    EstimatedTime = o.EstimatedTime,
                    Status = o.Status,
                    Quantity = o.Quantity,
                    Description = o.Description,
                    StartTime = o.StartTime,
                    EndTime = o.EndTime,
                    TotalTime = o.TotalTime,
                    TotalTimeSpentOnTask = o.TotalTimeSpentOnTask,
                    TotalManualTimeLogHrs = o.TotalManualTimeLogHrs,
                    TotalTrackedTimeLogHrs = o.TotalTrackedTimeLogHrs,
                    IsRecurring = o.IsRecurring,
                    IsTask = o.IsTask,
                    CPAId = o.CPAId,
                    CPAName = o.CPAName,
                    ClientId = o.ClientId,
                    ClientName = o.ClientName,
                    ModifiedHours = o.ModifiedHours,
                    SubProcessId = o.SubProcessId,
                    SubProcessName = o.SubProcessName,
                    IsTaskAvailable = o.IsTaskAvailable,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    Activities = !string.IsNullOrEmpty(o.ActivitiesJson) ? JsonConvert.DeserializeObject<List<ActivityName>>(o.ActivitiesJson!) : null,
                    IsManual = o.IsManual,
                    ReviewerStatus = o.ReviewerStatus,
                    DepartmentName = o.DepartmentName,
                    RoleType = o.RoleType
                })?.ToList();

            }
            else if (req.OtherReportIdentifier == OtherReportIdentifier.TimesheetReport)
            {
                var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetTimesheetReport, new
                {
                    PageNo = req.PageNo ?? 1,
                    PageSize = req.PageSize ?? 10,
                    req.GlobalSearch,
                    req.SortColumn,
                    req.IsDesc,
                    CurrentUserId = ConfigData.UserId,
                    req.StartDate,
                    req.EndDate,
                    UserIdsForFilter = (req.Users?.Any()).GetValueOrDefault() ? JsonConvert.SerializeObject(req.Users) : null,
                    Departments = (req.Departments?.Any()).GetValueOrDefault() ? JsonConvert.SerializeObject(req.Departments) : null,
                    req.IsDownload
                }, commandType: CommandType.StoredProcedure);

                totalRecords = (await result.ReadAsync<long>()).FirstOrDefault();
                otherReportDatas = (await result.ReadAsync<OtherReportData>())?.ToList();
                dateTimeLogs = (await result.ReadAsync<DateTimeLog>())?.ToList();
                logDetails = (await result.ReadAsync<LogDetail>())?.Select(o => new LogDetail()
                {
                    UserId = o.UserId,
                    UserName = o.UserName,
                    LogDate = o.LogDate,
                    DailyWorkPlanId = o.DailyWorkPlanId,
                    TaskId = o.TaskId,
                    TaskName = o.TaskName,
                    EstimatedTime = o.EstimatedTime,
                    Status = o.Status,
                    Quantity = o.Quantity,
                    Description = o.Description,
                    StartTime = o.StartTime,
                    EndTime = o.EndTime,
                    TotalTime = o.TotalTime,
                    TotalTimeSpentOnTask = o.TotalTimeSpentOnTask,
                    TotalManualTimeLogHrs = o.TotalManualTimeLogHrs,
                    TotalTrackedTimeLogHrs = o.TotalTrackedTimeLogHrs,
                    IsRecurring = o.IsRecurring,
                    IsTask = o.IsTask,
                    CPAId = o.CPAId,
                    CPAName = o.CPAName,
                    ClientId = o.ClientId,
                    ClientName = o.ClientName,
                    ModifiedHours = o.ModifiedHours,
                    SubProcessId = o.SubProcessId,
                    SubProcessName = o.SubProcessName,
                    IsTaskAvailable = o.IsTaskAvailable,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    Activities = !string.IsNullOrEmpty(o.ActivitiesJson) ? JsonConvert.DeserializeObject<List<ActivityName>>(o.ActivitiesJson!) : null,
                    IsManual = o.IsManual,
                    ReviewerStatus = o.ReviewerStatus,
                    DepartmentName = o.DepartmentName,
                    RoleType = o.RoleType
                })?.ToList();
            }
            if (req.IsDownload)
            {
                DataSet dataSet = MakeDataTableForDownload(req.StartDate, req.EndDate, req.OtherReportIdentifier, otherReportDatas, dateTimeLogs, logDetails);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                      TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                    {
                        { "Date Range", $"{req.StartDate ?? startDate:dd-MM-yyyy} to {req.EndDate ?? now:dd-MM-yyyy}" },
                        { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                    };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content, req.OtherReportIdentifier == OtherReportIdentifier.TimesheetReport ? true : false);

                return new OtherReportsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = $"{req.OtherReportIdentifier}.xlsx"
                };
            }
            else
            {
                foreach (var item in dateTimeLogs!)
                {
                    item.LogDetails = logDetails?.Where(i => (i.LogDate?.Equals(item.LogDate)).GetValueOrDefault() && (i.UserId?.Equals(item.UserId)).GetValueOrDefault());
                }
                foreach (var item in otherReportDatas!)
                {
                    item.DateTimeLogs = dateTimeLogs.Where(i => (i.UserId?.Equals(item.UserId)).GetValueOrDefault());
                }
            }

            return new OtherReportNotDownloadRes()
            {
                TotalCount = totalRecords,
                List = otherReportDatas!.ToList()
            };
        }


        public async Task<IKRAReportRes> GetKRAReport(KRAReqVM req)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetKRAReport, new
            {
                PageNo = req.PageNo ?? 1,
                PageSize = req.PageSize ?? 10,
                req.SortColumn,
                req.IsDesc,
                req.GlobalSearch,
                Clients = req.Clients != null && req.Clients.Count > 0 ? JsonConvert.SerializeObject(req.Clients) : null,
                Projects = req.Projects != null && req.Projects.Count > 0 ? JsonConvert.SerializeObject(req.Projects) : null,
                Users = req.Users != null && req.Users.Count > 0 ? JsonConvert.SerializeObject(req.Users) : null,
                req.Available,
                CurrentUserId = ConfigData.UserId,
                req.StartDate,
                req.EndDate,
                req.IsDownload,
                Departments = req.Departments != null && req.Departments.Count > 0 ? JsonConvert.SerializeObject(req.Departments) : null,
            }, commandType: CommandType.StoredProcedure);


            var resp = new KRAReportNotDownloadRes()
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<KRAReportDataVM>())?.Select(o => new KRAReportDataVM()
                {
                    CPAId = o.CPAId,
                    CPAName = o.CPAName,
                    ClientId = o.ClientId,
                    ClientName = o.ClientName,
                    TaskId = o.TaskId,
                    TaskName = o.TaskName,
                    UserId = o.UserId,
                    UserName = o.UserName,
                    EstimatedDuration = o.EstimatedDuration,
                    ActualTime = o.ActualTime,
                    IsTask = o.IsTask,
                    Result = o.Result,
                    EfficiencyPercentage = o.EfficiencyPercentage,
                    TotalNoOfQuantity = o.TotalNoOfQuantity,
                    TotalModifiedHours = o.TotalModifiedHours,
                    SubProcessId = o.SubProcessId,
                    SubProcessName = o.SubProcessName,
                    IsAvailable = o.IsAvailable,
                    Activities = !string.IsNullOrEmpty(o.ActivitiesJson) ? JsonConvert.DeserializeObject<List<ActivityName>>(o.ActivitiesJson!) : null,
                    CreatedOn = o.CreatedOn
                }).ToList()!
            };

            if (req.IsDownload)
            {
                var downloadData = resp.List.Select(x => new
                {

                    Client = x.CPAName,
                    Project = x.ClientName,
                    Task = x.TaskName,
                    SubProcess = x.SubProcessName,
                    UserName = x.UserName,
                    STDTime = x.EstimatedDuration?.GetTimeSpan(),
                    TotalTime = x.ActualTime?.GetTimeSpan(),
                    Quantity = x.TotalNoOfQuantity,
                    EfficiencyPercentage = x.EfficiencyPercentage,
                    Remarks = x.Result
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                      TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                    { "Date Range", $"{req.StartDate ?? startDate:dd-MM-yyyy} to {req.EndDate ?? now:dd-MM-yyyy}" },
                    { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);

                return new KRAReportsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "KRAReport.xlsx"
                };
            }
            return resp;
        }

        public async Task<IAutoManualReportRes> GetAutoManualReport(AutoManualReportReq req)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetAutoManualReport, new
            {
                PageNo = req.PageNo ?? 1,
                PageSize = req.PageSize ?? 10,
                req.SortColumn,
                req.IsDesc,
                req.GlobalSearch,
                CurrentUserId = ConfigData.UserId,
                req.StartDate,
                req.EndDate,
                Departments = req.Departments != null && req.Departments.Count > 0 ? JsonConvert.SerializeObject(req.Departments) : null,
                UserIdsForFilter = req.Users != null && req.Users.Count > 0 ? JsonConvert.SerializeObject(req.Users) : null,
                ReportingUsers = req.ReportingUsers != null && req.ReportingUsers.Count > 0 ? JsonConvert.SerializeObject(req.ReportingUsers) : null,
                req.Available,
                req.IsDownload
            }, commandType: CommandType.StoredProcedure);

            var resp = new AutoManualReportNotDownloadRes()
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<AutoManualReportData>()).ToList()
            };

            if (req.IsDownload)
            {
                var downloadData = resp.List.Select(x => new
                {
                    UserName = x.UserName,
                    Department = x.DepartmentName,
                    ReportingUser = x.ReportingUserName,
                    STDTime = x.TotalEstimatedTime?.GetTimeSpan(),
                    AutoTime = x.TotalTrackedTimeSpent?.GetTimeSpan(),
                    ManualTime = x.TotalManualTimeSpent?.GetTimeSpan(),
                    TotalTime = x.TotalTimeSpent?.GetTimeSpan()
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                      TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                    { "Date Range", $"{req.StartDate ?? startDate:dd-MM-yyyy} to {req.EndDate ?? now:dd-MM-yyyy}" },
                    { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);

                return new AutoManualReportsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "AutoManualReport.xlsx"
                };
            }
            return resp;
        }

        public async Task<IAuditReportRes> GetAuditReport(AuditReportReq req)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_AuditReport, new
            {
                PageNo = req.PageNo ?? 1,
                PageSize = req.PageSize ?? 10,
                req.SortColumn,
                req.IsDesc,
                req.IsDownload,
                req.GlobalSearch,
                CurrentUserId = ConfigData.UserId,
                DepartmentsFilter = req.DepartmentsFilter != null && req.DepartmentsFilter.Count > 0 ? JsonConvert.SerializeObject(req.DepartmentsFilter) : null,
                UserIdsForFilter = req.UserIdsForFilter != null && req.UserIdsForFilter.Count > 0 ? JsonConvert.SerializeObject(req.UserIdsForFilter) : null,
                ClientFilter = req.ClientFilter != null && req.ClientFilter.Count > 0 ? JsonConvert.SerializeObject(req.ClientFilter) : null,
                ProjectFilter = req.ProjectFilter != null && req.ProjectFilter.Count > 0 ? JsonConvert.SerializeObject(req.ProjectFilter) : null,
                req.StartDate,
                req.EndDate,
                
            }, commandType: CommandType.StoredProcedure);

            var resp = new AuditReportNotDownloadRes()
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<AuditReportData>()).ToList()
            };

            if (req.IsDownload)
            {
                var downloadData = resp.List.Select(x => new
                {
                    x.UserName,
                    Department = x.DepartmentName,    
                    x.TaskCreatedDate,
                    x.LoginTime,
                    x.LogoutTime,
                    x.ClientName,
                    x.ProjectName,
                    x.ProcessName,
                    x.SubProcessName,
                    STDTime = x.StandardTime?.GetTimeSpan(), 
                    TotalTime = x.TotalTime?.GetTimeSpan(),
                    BreakTime = x.BreakTime?.GetTimeSpan(),
                    IdleTime = x.IdleTime?.GetTimeSpan()
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new();
                dataSet.Tables.Add(dataTable);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                   { "Date Range", $"{req.StartDate ?? startDate:dd-MM-yyyy} to {req.EndDate ?? now:dd-MM-yyyy}" },
                   { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);
                return new AuditReportDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "AuditReport.xlsx"
                };
            }
            return resp;
        }


        private static DataSet MakeDataTableForDownload(DateTime? startDate, DateTime? endDate, OtherReportIdentifier otherReportIdentifier, IEnumerable<OtherReportData>? otherReportDatas, IEnumerable<DateTimeLog>? dateTimeLogs, IEnumerable<LogDetail>? logDetails)
        {
            DataSet dataSet = new();
            DataTable dt = new("ReportsTable");
            dt.Columns.AddRange(CreateColumnsForOtherReport(otherReportIdentifier).ToArray());

            if (otherReportIdentifier != OtherReportIdentifier.WorkloadReport)
            {
                DateTime date = DateTime.UtcNow;
                if (!startDate.HasValue)
                    startDate = new DateTime(date.Year, date.Month, 1);
                if (!endDate.HasValue)
                    endDate = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);

                if (startDate <= endDate)
                {
                    for (DateTime i = startDate.Value; i <= endDate.Value; i = i.AddDays(1))
                    {
                        DataColumn dateColumn = new()
                        {
                            DataType = typeof(string),
                            ColumnName = i.ToString("dd/MM/yyyy")
                        };
                        dt.Columns.Add(dateColumn);
                    }
                }
            }

            if (otherReportIdentifier == OtherReportIdentifier.WorkloadReport)
            {
                logDetails?.ToList().ForEach(i =>
                {
                    var row = dt.NewRow();
                    row["User Name"] = i.UserName;
                    row["Department"] = i.DepartmentName;
                    row["Designation"] = i.RoleType;
                    row["Date"] = i.LogDate?.Date ?? (object)DBNull.Value;
                    row["Std Time"] = i.TotalTime?.GetTimeSpan() ?? (object)DBNull.Value;
                    row["Total Time"] = i.TotalTimeSpentOnTask?.GetTimeSpan() ?? (object)DBNull.Value;
                    row["WorkItemType"] = i.IsManual ? "Manual" : "Auto";
                    row["Client"] = i.CPAName;
                    row["Project"] = i.ClientName;
                    row["TaskName"] = i.TaskName;
                    row["SubProcessName"] = i.SubProcessName;
                    row["Quantity"] = i.Quantity;
                    row["ReviewerStatus"] = i.ReviewerStatus;
                    dt.Rows.Add(row);
                });
            }
            else
            {
                otherReportDatas?.AsList().ForEach(otherReportData =>
                {
                    var row = dt.NewRow();
                    row["User Name"] = otherReportData.UserName;
                    row["Department"] = otherReportData.DepartmentName;
                    row["Designation"] = otherReportData.RoleType;
                    row["Present Days"] = otherReportData.PresentDays;
                    if (otherReportIdentifier == OtherReportIdentifier.TimesheetReport)
                    {
                        row["Reporting Manager"] = otherReportData.ReportingManager;
                        row["Total Time (Exc Rejected Hours)"] = otherReportData.TotalTimeExcludingRejectedHrsByUser?.GetTimeSpan() ?? (object)DBNull.Value;
                        row["Rejected Hours"] = otherReportData.TotalRejectedHrsByUser?.GetTimeSpan() ?? (object)DBNull.Value;
                        row["Avg. Total Time"] = otherReportData.AvgTotalTime?.GetTimeSpan() ?? (object)DBNull.Value; ;
                        row["Avg. Break Time"] = otherReportData.AvgBreakTime?.GetTimeSpan() ?? (object)DBNull.Value; ;
                        row["Avg. Idle Time"] = otherReportData.AvgIdleTime?.GetTimeSpan() ?? (object)DBNull.Value; 
                    }
                    row["Std Time"] = otherReportData.TotalTimeOfUser?.GetTimeSpan() ?? (object)DBNull.Value;
                    if (otherReportIdentifier == OtherReportIdentifier.UserReport)
                    {
                        row["Total Time"] = otherReportData.TotalTimeSpentByUser?.GetTimeSpan() ?? (object)DBNull.Value;
                        row["Total Break Time"] = otherReportData.TotalBreakTime?.GetTimeSpan() ?? (object)DBNull.Value;
                        row["Total Idle Time"] = otherReportData.TotalIdleTime?.GetTimeSpan() ?? (object)DBNull.Value;
                    }

                    if (otherReportIdentifier != OtherReportIdentifier.WorkloadReport)
                    {
                        dateTimeLogs?.Where(i => (i.UserId?.Equals(otherReportData.UserId)).GetValueOrDefault()).AsList().ForEach(dateTimeLog =>
                        {
                            if (otherReportIdentifier == OtherReportIdentifier.UserReport)
                            {
                                row[dateTimeLog.LogDate!.Value.ToString("dd/MM/yyyy")] = dateTimeLog.AttendanceStatus;
                            }
                            else
                            {
                                row[dateTimeLog.LogDate!.Value.ToString("dd/MM/yyyy")] = dateTimeLog.TimeSpentWithFormatting;
                            }
                        });
                    }
                    dt.Rows.Add(row);
                });
            }
            dataSet.Tables.Add(dt);
            return dataSet;
        }

        private static IEnumerable<DataColumn> CreateColumnsForOtherReport(OtherReportIdentifier otherReportIdentifier)
        {
            List<DataColumn> dataColumns = new();
            DataColumn userNameColumn = new()
            {
                DataType = typeof(string),
                ColumnName = "User Name"
            };
            dataColumns.Add(userNameColumn);
            DataColumn userDepartmentColumn = new()
            {
                DataType = typeof(string),
                ColumnName = "Department"
            };
            dataColumns.Add(userDepartmentColumn);
            DataColumn userDesignationColumn = new()
            {
                DataType = typeof(string),
                ColumnName = "Designation"
            };
            dataColumns.Add(userDesignationColumn);
            switch (otherReportIdentifier)
            {
                case OtherReportIdentifier.UserReport:
                    DataColumn totalTimeColumn = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Total Time"
                    };
                    dataColumns.Add(totalTimeColumn);
                    DataColumn stdTimeColumnUser = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Std Time"
                    };
                    dataColumns.Add(stdTimeColumnUser);
                    DataColumn presentDaysColumn = new()
                    {
                        DataType = typeof(double),
                        ColumnName = "Present Days"
                    };
                    dataColumns.Add(presentDaysColumn);
                    DataColumn totalIdleTime = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Total Idle Time"
                    };
                    dataColumns.Add(totalIdleTime);
                    DataColumn totalBreakTime = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Total Break Time"
                    };
                    dataColumns.Add(totalBreakTime);
                    break;
                case OtherReportIdentifier.TimesheetReport:
                    DataColumn reportingManager = new()
                    {
                        DataType = typeof(string),
                        ColumnName = "Reporting Manager"
                    };
                    dataColumns.Add(reportingManager);
                    DataColumn presentDayColumn = new()
                    {
                        DataType = typeof(double),
                        ColumnName = "Present Days"
                    };
                    dataColumns.Add(presentDayColumn);
                    DataColumn totalTimeExcRejectedHrs = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Total Time (Exc Rejected Hours)"
                    };
                    dataColumns.Add(totalTimeExcRejectedHrs);
                    DataColumn rejectedHrs = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Rejected Hours"
                    };
                    dataColumns.Add(rejectedHrs);
                    DataColumn stdTimeHrs = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Std Time"
                    };
                    dataColumns.Add(stdTimeHrs);
                    DataColumn avgTotalTime = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Avg. Total Time"
                    };
                    dataColumns.Add(avgTotalTime);
                    DataColumn avgBreakTime = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Avg. Break Time"
                    };
                    dataColumns.Add(avgBreakTime);
                    DataColumn avgIdleTime = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Avg. Idle Time"
                    };
                    dataColumns.Add(avgIdleTime);
                    break;
                case OtherReportIdentifier.WorkloadReport:
                default:
                    DataColumn stdTime = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Std Time"
                    };
                    dataColumns.Add(stdTime);
                    DataColumn totalTimeColumnHrs = new()
                    {
                        DataType = typeof(TimeSpan),
                        ColumnName = "Total Time"
                    };
                    dataColumns.Add(totalTimeColumnHrs);
                    DataColumn date = new()
                    {
                        DataType = typeof(DateTime),
                        ColumnName = "Date"
                    };
                    dataColumns.Add(date);
                    DataColumn client = new()
                    {
                        DataType = typeof(string),
                        ColumnName = "Client"
                    };
                    dataColumns.Add(client);
                    DataColumn project = new()
                    {
                        DataType = typeof(string),
                        ColumnName = "Project"
                    };
                    dataColumns.Add(project);
                    DataColumn taskName = new()
                    {
                        DataType = typeof(string),
                        ColumnName = "TaskName"
                    };
                    dataColumns.Add(taskName);
                    DataColumn subProcessName = new()
                    {
                        DataType = typeof(string),
                        ColumnName = "SubProcessName"
                    };
                    dataColumns.Add(subProcessName);
                    DataColumn quantity = new()
                    {
                        DataType = typeof(int),
                        ColumnName = "Quantity"
                    };
                    dataColumns.Add(quantity);
                    DataColumn workItemType = new()
                    {
                        DataType = typeof(string),
                        ColumnName = "WorkItemType"
                    };
                    dataColumns.Add(workItemType);
                    DataColumn reviewerStatus = new()
                    {
                        DataType = typeof(string),
                        ColumnName = "ReviewerStatus"
                    };
                    dataColumns.Add(reviewerStatus);
                    break;
            }
            return dataColumns;
        }

        public async Task SendDailyReportEmail()
        {
            var result = _dbContext.Connection.QueryMultiple(DBHelper.USP_DailyReport, null, commandType: CommandType.StoredProcedure);

            var dailyReportAbsentPresentTable = result?.ReadFirstOrDefaultAsync<DailyReportAbsentPresentTable>()!.Result;
            var dailyReportAbsentPresentExcel = result?.ReadAsync<DailyReportAbsentPresentExcel>()!.Result;

            var atlReportExcelData = result?.ReadAsync<ATLExcelReport>()!.Result;
            var atlReportData = result?.ReadAsync<ATLReport>()!.Result;
            var monthlyReportData = result?.ReadFirstOrDefaultAsync<MonthlyReport>()!.Result;

            //attachment for daily report excel
            var dataTable = dailyReportAbsentPresentExcel!.ToList().ListToDataTable();
            DataSet dataSet = new();
            dataSet.Tables.Add(dataTable);
            var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet);
            Attachment attachment = new(new MemoryStream(byteArray), $"DailyAbsentReport-{DateTime.UtcNow:dd-MM-yyyy}.xlsx");


            //attachment for atl report excel
            var atlExcelData = atlReportExcelData!.ToList().ListToDataTable();
            DataSet atldataSet = new();
            atldataSet.Tables.Add(atlExcelData);
            var atlbyteArray = ExportExcel.GetExcelFileFormDataSet(atldataSet);
            Attachment atlExcelAttachment = new(new MemoryStream(atlbyteArray), $"PendingTaskReport-{DateTime.UtcNow:dd-MM-yyyy}.xlsx");

            var emailTemplate = _emailHelper.GetTemplateByType("DailyReportEmailTemplate").Result;

            StringBuilder sb = new StringBuilder();

            foreach (var atlObj in atlReportData!)
            {
                sb.Append("<tr>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.ReviewerName + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.DepartmentName + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.TotalUserCount + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.TotalTasks + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.TotalActualTime + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.TotalTimeSpent + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.PendingTasksForSubmission + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.PendingStdTime + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.PendingTimeSpent + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.PartiallySubmittedTasks + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.PartiallySubmittedStdTime + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.PartiallySubmittedTimeSpent + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.ApprovedTasks + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.ApprovedStdTime + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.ApprovedTimeSpent + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.RejectedTasks + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.RejectedStdTime + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.RejectedTimeSpent + "</td>");
                sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + atlObj.PendingForReviewTasks + "</td>");
                sb.Append("</tr>");
            }

            sb.ToString();

            if (emailTemplate != null)
            {
                var emailConfig = _emailHelper.GetEmailConfig().Result;
                emailTemplate.Content = emailTemplate.Content.Replace("{!TotalNoOfUsers!}", dailyReportAbsentPresentTable!.TotalNoOfUsers.ToString());
                emailTemplate.Content = emailTemplate.Content.Replace("{!TotalNoOfPresentUsers!}", (dailyReportAbsentPresentTable!.TotalNoOfPresentUsers + dailyReportAbsentPresentTable.TotalNoOfHalfPresentUsers).ToString());
                emailTemplate.Content = emailTemplate.Content.Replace("{!TotalNoOfAbsentUsers!}", dailyReportAbsentPresentTable!.TotalNoOfAbsentUsers.ToString());

                emailTemplate.Content = emailTemplate.Content.Replace("{!ReviewerDetails!}", sb.ToString());

                emailTemplate.Content = emailTemplate.Content.Replace("{!MonthlyTotalTask!}", monthlyReportData!.MonthlyTotalTask.ToString());
                emailTemplate.Content = emailTemplate.Content.Replace("{!MonthlyReviewerPendingTasks!}", monthlyReportData!.MonthlyReviewerPendingTasks.ToString());
                emailTemplate.Content = emailTemplate.Content.Replace("{!MonthlyPartiallySubmittedTasks!}", monthlyReportData!.MonthlyPartiallySubmittedTasks.ToString());
                emailTemplate.Content = emailTemplate.Content.Replace("{!MonthlyApprovedTasks!}", monthlyReportData!.MonthlyApprovedTasks.ToString());
                emailTemplate.Content = emailTemplate.Content.Replace("{!MonthlyRejectedTasks!}", monthlyReportData!.MonthlyRejectedTasks.ToString());

                await _emailHelper.SendEmailAsync(new SendMailVM
                {
                    Email = (_dailyEmailHelper as DailyEmailHelper)!.emails,
                    Subject = emailTemplate!.Subject,
                    MailBody = emailTemplate.Content,
                    Attachments = new List<Attachment>() { attachment, atlExcelAttachment }.AsEnumerable()
                }, emailConfig);
            }

        }

        public async Task<ILoginLogoutReportRes> GetLoginLogoutReport(LoginLogoutReportReq req)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_LoginLogoutReport, new
            {
                PageNo = req.PageNo ?? 1,
                PageSize = req.PageSize ?? 10,
                req.SortColumn,
                req.IsDesc,
                req.GlobalSearch,
                CurrentUserId = ConfigData.UserId,
                DateFilter = req.DateFilter ?? DateTime.UtcNow,
                Departments = req.Departments != null && req.Departments.Count > 0 ? JsonConvert.SerializeObject(req.Departments) : null,
                Users = req.Users != null && req.Users.Count > 0 ? JsonConvert.SerializeObject(req.Users) : null,
                ReportingTo = req.Reporting != null && req.Reporting.Count > 0 ? JsonConvert.SerializeObject(req.Reporting) : null,
                PresentStatusFilter = req.PresentStatus != null && req.PresentStatus.HasValue ? JsonConvert.SerializeObject(req.PresentStatus) : null,
                req.IsAvailable,
                req.IsDownload
            }, commandType: CommandType.StoredProcedure);

            var resp = new LoginLogoutNotDownloadRes()
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<LoginLogoutReportData>()).ToList()
            };

            if (req.IsDownload)
            {
                var downloadData = resp.List.Select(x => new
                {
                    x.UserName,
                    Designation = x.UserType,
                    Department = x.DepartmentName,
                    ReportingUser = x.ReportingTo,
                    x.LoginTime,
                    x.LogoutTime,
                    TotalTime = x.TotalTimeSpentForDay?.GetTimeSpan(),
                    BreakTime = x.BreakTimeForDay?.GetTimeSpan(),
                    IdleTime = x.IdleTimeForDay?.GetTimeSpan(),
                    IsLoggedIn = x.PresentStatus.ToString(),
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new();
                dataSet.Tables.Add(dataTable);

                DateTime now = DateTime.UtcNow;
                var startDate = new DateTime(now.Year, now.Month, 1);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                    { "Date", $"{req.DateFilter ?? now:dd-MM-yyyy}" },
                    { "Exported On",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };

                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);
                return new LoginLogoutReportsDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "LoginLogoutReport.xlsx"
                };
            }
            return resp;
        }

        public async Task SendManagementReportEmail()
        {
            ManagementEmailHelper helper = (_managementReportEmailHelper as ManagementEmailHelper)!;
            var result = _dbContext.Connection.QueryMultiple(DBHelper.USP_TMSManagementReport, null, commandType: CommandType.StoredProcedure);

            // get result from stored procedures into model classes
            var dateRange = result?.ReadAsync<string?>().Result;
            var dayWiseDataTable = result?.ReadAsync<DayWiseDataTable>()!.Result;
            var weeklyDataTable = result?.ReadAsync<WeeklyDataTable>()!.Result;
            var workedLessUserResult = result?.ReadAsync<WorkedLessUserTable>()!.Result;
            var reportingManagerTable = result?.ReadAsync<ReportingManagerTable>()!.Result;
            var usersUnderReportingManagerResult = result?.ReadAsync<UsersUnderReportingManagerExcel>()!.Result;

            // previous date
            var previousDate = DateTime.UtcNow.AddDays(-1);

            // take first ten users for email
            var workedLessUserResultData = workedLessUserResult!.ToList().Take(10);

            // get string format for html tag from lists
            var PrevDayDetails = helper.InsertIntoHtmlTemplate<DayWiseDataTable>(dayWiseDataTable!.ToList());
            var WeekDetails = helper.InsertIntoHtmlTemplate<WeeklyDataTable>(weeklyDataTable!.ToList());
            var UserDetails = helper.InsertIntoHtmlTemplate<WorkedLessUserTable>(workedLessUserResultData!.ToList());
            var ATLDetails = helper.InsertIntoHtmlTemplate<ReportingManagerTable>(reportingManagerTable!.ToList());

            // attachments for user who worked less than std hours
            var workedLessUserExcel = workedLessUserResult!.ToList().ListToDataTable();
            DataSet workedLessUserExcelDataSet = new();
            workedLessUserExcelDataSet.Tables.Add(workedLessUserExcel);
            var userbyteArray = ExportExcel.GetExcelFileFormDataSet(workedLessUserExcelDataSet);
            Attachment userExcelAttachment = new(new MemoryStream(userbyteArray), $"UserWorkedBelowStdHours-{previousDate:yyyy-MM-dd}.xlsx");

            // attachments for users under ATL
            var usersUnderReportingManagerExcel = usersUnderReportingManagerResult!.ToList().ListToDataTable();
            DataSet userUnderReportingManagerDataSet = new();
            userUnderReportingManagerDataSet.Tables.Add(usersUnderReportingManagerExcel);
            var atlbyteArray = ExportExcel.GetExcelFileFormDataSet(userUnderReportingManagerDataSet);
            Attachment atlExcelAttachment = new(new MemoryStream(atlbyteArray), $"UserUnderReportingManager-{previousDate:yyyy-MM-dd}.xlsx");

            // config email template
            var emailTemplate = _emailHelper.GetTemplateByType("ManagementReportTemplate").Result;

            if (emailTemplate != null)
            {
                var emailConfig = _emailHelper.GetEmailConfig().Result;
                emailTemplate.Content = emailTemplate.Content.Replace("{!ReportDate!}", previousDate.ToString("yyyy-MM-dd"));
                emailTemplate.Content = emailTemplate.Content.Replace("{!DateRange!}", dateRange?.ToList().FirstOrDefault());
                emailTemplate.Content = emailTemplate.Content.Replace("{!PrevDayDetails!}", PrevDayDetails);
                emailTemplate.Content = emailTemplate.Content.Replace("{!WeekDetails!}", WeekDetails);
                emailTemplate.Content = emailTemplate.Content.Replace("{!UserDetails!}", UserDetails);
                emailTemplate.Content = emailTemplate.Content.Replace("{!ATLDetails!}", ATLDetails);

                await _emailHelper.SendEmailAsync(new SendMailVM
                {
                    Email = helper!.emails,
                    Subject = emailTemplate!.Subject,
                    MailBody = emailTemplate.Content,
                    Attachments = new List<Attachment>() { userExcelAttachment, atlExcelAttachment }.AsEnumerable()
                }, emailConfig);
            }

        }


    }
}
