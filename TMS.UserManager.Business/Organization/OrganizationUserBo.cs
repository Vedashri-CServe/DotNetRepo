using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using SqlKata.Execution;
using System.Data;
using System.Diagnostics;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.ExportExcel;
using TMS.Helper.UtilityHelper;

namespace TMS.UserManager.Business
{
    public class OrganizationUserBo : IOrganizationUserService
    {
        private readonly QueryFactory _dbContext;
        public ConfigData ConfigData { get; set; }
        public OrganizationUserBo(QueryFactory dbContext)
        {
            _dbContext = dbContext;

        }
        #region POST Method

        public async Task<IClientAndProjectRes> GetOrganizationUserList(OrgUserFilterVM filter)
        {
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetOrganizationUserList, new
            {
                PageNo = filter.Index,
                PageSize = filter.PageSize,
                OrganizationType = filter.OrganizationType,
                GlobalSearch = filter.GlobalSearch,
                SortColumn = filter.SortColumn,
                IsDesc = filter.IsDesc,
                TypeOfWork = filter.TypeOfWork,
                BillType = filter.BillingType,
                ContractedHours = filter.ContractedHours,
                IsAvailable = filter.IsAvailable,
                Userid = filter.UserId,
                filter.IsDownload
            }, commandType: CommandType.StoredProcedure));
            var resultList = new ClientAndProjecNotDownloadRes
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<OrgUserVM>()).ToList()
            };
            if (filter.IsDownload)
            {
                if (filter.OrganizationType == 2)
                {
                    var downloadData = resultList.List.Select(x =>
                    new
                    {
                        ClientName = x.CPAName,
                        ContractedHours = x.ContractedHours,
                        InternalHours = x.InternalHours,
                        TypeOfWork = JsonConvert.DeserializeObject<List<DropdownItemVM>>((string)x.TypeOfWorkList).FirstOrDefault().Label,
                        BillingType = JsonConvert.DeserializeObject<List<DropdownItemVM>>((string)x.BillingTypeList).FirstOrDefault().Label,
                        Status = x.IsAvailable ? "Active" : "Inactive",
                        Email = x.Email,
                        MobileNumber = x.MobileNo
                    }).ToList();

                    var dataTable = downloadData.ListToDataTable();
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(dataTable);
                    var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet);
                    return new ClientAndProjecDownloadRes()
                    {
                        ByteArray = byteArray,
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        FileName = "ClientList.xlsx"
                    };
                }
                else
                {
                    var downloadData = resultList.List.Select(x =>
                    new
                    {
                        ClientName = x.CPAName,
                        ProjectName = x.Name,
                        TypeOfWork = JsonConvert.DeserializeObject<List<DropdownItemVM>>((string)x.TypeOfWorkList).FirstOrDefault().Label,
                        Status = x.IsAvailable ? "Active" : "Inactive"
                    }).ToList();

                    var dataTable = downloadData.ListToDataTable();
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(dataTable);
                    var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet);
                    return new ClientAndProjecDownloadRes()
                    {
                        ByteArray = byteArray,
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        FileName = "ProjectList.xlsx"
                    };
                }
            }
            if (filter.OrganizationType == 2)
                resultList.List.ForEach(item => item.BillingTypeList = JsonConvert.DeserializeObject<List<DropdownItemVM>>((string)item.BillingTypeList) ?? new());
            resultList.List.ForEach(item => item.TypeOfWorkList = JsonConvert.DeserializeObject<List<DropdownItemVM>>((string)item.TypeOfWorkList) ?? new());

            return resultList;
        }
        public async Task<long> SaveOrganizationUser(OrgUserVM objCPA)
        {
            var userDetail = (await _dbContext.Connection.QueryAsync<SaveOrgResultVM>(DBHelper.USP_SaveOrganizationUser, new
            {
                Id = objCPA.UserId,
                OrganizationName = objCPA.Name,
                WebsiteUrl = objCPA.WebSiteURl,
                ParentId = objCPA.ParentId,
                OrganizationType = objCPA.OrganizationType,
                Address = objCPA.Address,
                EmailId = objCPA.Email,
                MobileNo = objCPA.MobileNo,
                TypeOfWork = objCPA.TypeOfWork,
                BillType = objCPA.BillingType,
                ContractedHours = objCPA.ContractedHours,
                InternalHours = objCPA.InternalHours,
                IsDeleted = objCPA.IsDeleted,
                CurrentUser = ConfigData.UserId,
                SOP = objCPA.SOP
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response
            if (userDetail != null)
                return userDetail.UserId;

            return -5;
        }

        public async Task<long> DeleteOrganizationUser(long CPAId)
        {
            //update organization Table
            var userDetail = (await _dbContext.Connection.QueryAsync<SaveOrgResultVM>(DBHelper.USP_DeleteOrganizationUser, new
            {
                OrgId = CPAId,
                IsDeleted = true,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (userDetail != null)
                return userDetail.UserId;

            return -5;
        }

        public async Task<OrgUserWithCountVM> ClientByCPA(long CPAId)
        {
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetClientByCPA, new
            { CPAId = CPAId }, commandType: CommandType.StoredProcedure));
            return new OrgUserWithCountVM
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<OrgUserVM>()).ToList()
            };
        }
        #endregion

        public async Task<long> CreateTask(TaskVM objTask)
        {
            var taskDetail = (await _dbContext.Connection.QueryAsync<TaskVMResultVM>(DBHelper.USP_SaveTask, new
            {
                id = objTask.TaskId,
                OrganizationIds = string.Join(",", ((IEnumerable<object>)objTask.OrganizationList).ToList()),
                task_name = objTask.TaskName,
                estimated_duration = objTask.EstimatedDuration,
                Is_productive = objTask.IsProductive,
                Is_billable = objTask.IsBillable,
                is_deleted = objTask.IsDeleted,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response
            if (taskDetail != null)
                return taskDetail.TaskId;

            return -5;
        }

        public async Task<bool> DeleteTask(long TaskId)
        {
            //update Task Table
            await _dbContext.Query("task").Where(new { id = TaskId }).UpdateAsync(new
            {
                is_deleted = true,
                updated_on = DateTime.UtcNow,
                updated_by = ConfigData.UserId
            });

            //update Task Organization Mapping Table
            await _dbContext.Query("task_organization_mapping").Where(new { task_id = TaskId, is_deleted = false }).UpdateAsync(new
            {
                is_deleted = true,
                updated_on = DateTime.UtcNow,
                updated_by = ConfigData.UserId
            });
            return true;
        }

        public async Task<List<GetCpaListVM>> GetCPAList(UserCPAVM reqObj)
        {
            var CPAList = (await _dbContext.Connection.QueryAsync<GetCpaListVM>(DBHelper.USP_GetCPAList, new
            { UserId = ConfigData.UserId, CallByDWp = reqObj.CallByDWP }, commandType: CommandType.StoredProcedure)).ToList();
            return CPAList.ToList();
        }
        public async Task<ITaskListRes> GetTaskList(TaskListFilterVM filter)
        {
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetTaskList, new
            {
                PageNo = filter.PageNo,
                PageSize = filter.PageSize,
                GlobalSearch = filter.GlobalSearch,
                TaskName = filter.TaskName,
                Userid = filter.UserId,
                IsAvailable = filter.IsAvailable,
                filter.IsDownload
            }, commandType: CommandType.StoredProcedure));
            var TaskList = new TaskListNotDownloadRes
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<TaskRespVM>()).OrderByDescending(x => x.TaskId).ToList(),
            };
            if (filter.IsDownload)
            {
                var downloadData = TaskList.List.Select(x =>
                   new
                   {
                       TaskName = x.TaskName,
                       ClientName = string.Join(",", (JsonConvert.DeserializeObject<List<DropdownItemVM>>((string)x.OrganizationList).DistinctBy(i => i.Value)).Select(x => x.Label).ToList()),
                       EstimatedHours = TimeSpan.FromMinutes(Convert.ToDouble(x.EstimatedDuration)),
                       ProductiveStatus = x.IsProductive ? "Productive" : "Non-Productive",
                       BillableStatus = x.IsBillable ? "Billable" : "Non-Billable"
                   }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);
                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet);
                return new TaskListDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "TaskList.xlsx"
                };
            }
            TaskList.List.ForEach(item => item.OrganizationList = ((JsonConvert.DeserializeObject<List<DropdownItemVM>>((string)item.OrganizationList))?.DistinctBy(i => i.Value)) ?? Enumerable.Empty<DropdownItemVM>());         
            return TaskList;
        }

        public async Task<long> SaveStatus(StatusFilterVM objStatus)
        {
            var StatusDetail = (await _dbContext.Connection.QueryAsync<StatusVMResultVM>(DBHelper.USP_SaveStatus, new
            {
                Id = objStatus.StatusId,
                StatusName = objStatus.StatusName,
                Color = objStatus.Color,
                IsDeleted = objStatus.IsDeleted,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response
            if (StatusDetail != null)
                return StatusDetail.Id;

            return -5;
        }

        public async Task<bool> DeleteStatus(long StatusId)
        {
            //update Status Table
            await _dbContext.Query("StatusDetails").Where(new { Id = StatusId }).UpdateAsync(new
            {
                IsDeleted = true,
                UpdatedOn = DateTime.UtcNow,
                UpdatedBy = ConfigData.UserId
            });
            return true;
        }

        public async Task<StatusListWithCountVM> GetStatusList(PaginationMetaVM filter)
        {
            var result = (await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetStatusList, new
            {
                PageNo = filter.Index,
                PageSize = filter.PageSize,
            }, commandType: CommandType.StoredProcedure));
            return new StatusListWithCountVM
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<StatusFilterVM>()).ToList()
            };
        }

    }
}
