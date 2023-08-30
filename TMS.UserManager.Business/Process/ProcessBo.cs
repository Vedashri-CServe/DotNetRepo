using Dapper;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using SqlKata.Execution;
using System.Data;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.ExportExcel;
using TMS.Helper.UtilityHelper;

namespace TMS.UserManager.Business
{
    public class ProcessBo : IProcessService
    {
        private readonly QueryFactory _dbContext;
        public ConfigData ConfigData { get; set; }

        public ProcessBo(QueryFactory dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> SaveProcess(ProcessVM process)
        {
            process.SubProcessName = process.SubProcessName?.Trim();
            var processId = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_SaveProcess, new
            {
                process.ProcessId,
                process.ProcessParentId,
                process.SubProcessName,
                ActivityName = process?.ActivityName != null ? JsonConvert.SerializeObject(process?.ActivityName) : null,
                process.EstimatedDuration,
                process.IsProductive,
                process.IsBillable,
                SavedBy = ConfigData.UserId,
                SavedOn = DateTime.UtcNow
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response

            return processId;
        }

        public async Task<IProcessListRes> GetProcessList(ProcessListFilterVM filter)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetProcessList, new
            {
                filter.GlobalSearch,
                filter.ProcessName,
                filter.EstimatedDuration,
                PageNo = filter.PageNo == default ? 1 : filter.PageNo,
                PageSize = filter.PageSize == default ? 1000 : filter.PageSize,
                filter.SortColumn,
                filter.IsDesc,
                filter.IsAvailable,
                Userid = filter.UserId,
                filter.IsDownload
            }, commandType: CommandType.StoredProcedure);

            var processData = new ProcessListNotDownloadRes
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<ProcessRespVM>()).OrderByDescending(x => x.ProcessId).ToList()
            };
            if (filter.IsDownload)
            {
                var downloadData = processData.List.Select(x =>
                   new
                   {
                       Process = x.ProcessName,
                       SubProcess = x.SubProcessName,
                       Activity = x.ActivityName != null ? string.Join(",", (JsonConvert.DeserializeObject<List<ActivityName>>((string)x.ActivityName)).Select(x => x.value).ToList()) : string.Empty,
                       EST = TimeSpan.FromMinutes(Convert.ToDouble(x.EstimatedDuration)),
                       ProductiveStatus = x.IsProductive ? "Productive" : "Non-Productive",
                       BillableStatus = x.IsBillable ? "Billable" : "Non-Billable"
                   }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);
                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet);
                return new ProcessListDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "ProcessList.xlsx"
                };
            }
            foreach (var item in processData.List)
            {
                item.ActivityName = item.ActivityName != null ? JsonConvert.DeserializeObject<List<ActivityName>>((string)item.ActivityName) : null;
            }
            return processData;
        }

        public async Task<int> DeleteProcess(long processId)
        {
            try
            {
                //update process Table
                await _dbContext.Query("process").Where(new { id = processId }).UpdateAsync(new
                {
                    is_deleted = true,
                    updated_by = ConfigData.UserId,
                    updated_on = DateTime.UtcNow
                });
                //update process_organization_details Mapping Table
                await _dbContext.Query("process_organization_details").Where(new { process_id = processId, is_deleted = false }).UpdateAsync(new
                {
                    is_deleted = true,
                    updated_by = ConfigData.UserId,
                    updated_on = DateTime.UtcNow
                });
                return 1;

            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        public async Task<ProcessVM> GetProcessById(long processId)
        {
            return await _dbContext.Query("process").Where("id", processId).WhereFalse("is_deleted").
                            Select("id as ProcessId",
                            "process_name as ProcessName",
                            "sub_process_name as SubProcessName",
                            "activity_name as ActivityName",
                            "estimated_duration as EstimatedDuration",
                            "is_productive as IsProductive",
                            "is_billable as IsBillable").
                            FirstOrDefaultAsync<ProcessVM>();
        }

        public async Task<IEnumerable<CPAProcessesRes>> GetCPAProcessList(CpaProcessesReq req)
        {
            var result = await _dbContext.Connection.QueryAsync<CPAProcessesRes>(DBHelper.USP_GetCPAProcesses, new
            {
                req.CPAId
            }, commandType: CommandType.StoredProcedure);
            foreach (var item in result)
            {
                item.ActivityName = item.ActivityName != null ? JsonConvert.DeserializeObject<List<ActivityName>>((string)item.ActivityName) : null;
            }
            return result;
        }

        public async Task<bool> SaveCpaProcesses(SaveCpaProcessesReq req)
        {
            try
            {
                var result = await _dbContext.Connection.QuerySingleOrDefaultAsync<bool>(DBHelper.USP_SaveCPAProcesses, new
                {
                    CurrentUserId = ConfigData.UserId,
                    CPAId = req.CPAId,
                    CPAProcesses = (req?.Data?.Any() ?? false) ? JsonConvert.SerializeObject(req.Data) : null
                }, commandType: CommandType.StoredProcedure);


                return result;
            }
            catch (Exception ex)
            {
                var res = ex;
            }
            return false;
        }

        #region sub-process-methods

        public async Task<long> SaveSubProcess(SubProcessVM subProcess)
        {
            try
            {
                var Id = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_SaveSubProcess, new
                {
                    Id = subProcess.SubprocessId,
                    ProcessId = subProcess.ProcessId,
                    SubprocessName = subProcess.SubprocessName,
                    IsAvailable = subProcess.IsAvailable,
                    IsDeleted = subProcess.IsDeleted,
                    SavedBy = ConfigData.UserId,
                    SavedOn = DateTime.UtcNow
                }, commandType: CommandType.StoredProcedure)).FirstOrDefault();

                return Id;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public async Task<SubProcessListVM> GetSubProcessListByProcess(SubProcessListFilterVM filter)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetSubProcessListByProcess, new
            {
                filter.GlobalSearch,
                filter.SubProcessName,
                filter.ProcessId,
                filter.ClientId,
                filter.IsAvailable,
            }, commandType: CommandType.StoredProcedure);

            var subProcessData = new SubProcessListVM
            {
                List = (await result.ReadAsync<SubProcessResponseVM>()).ToList(),
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
            };
            return subProcessData;
        }

        public async Task<SubProcessListVM> GetSubProcessList(SubProcessListFilterVM filter)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetSubProcessList, new
            {
                filter.GlobalSearch,
                filter.SubProcessName,
                PageNo = filter.PageNo == default ? 1 : filter.PageNo,
                PageSize = filter.PageSize == default ? 1000 : filter.PageSize,
                filter.SortColumn,
                filter.IsDesc,
                ConfigData.UserId,
                filter.ProcessId,
                filter.IsAvailable,
            }, commandType: CommandType.StoredProcedure);

            var subProcessData = new SubProcessListVM
            {
                List = (await result.ReadAsync<SubProcessResponseVM>()).OrderByDescending(x => x.SubprocessId).ToList(),
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
            };
            /* foreach(var subProcess in subProcessData.List)
             {
                 subProcess.ActivityName = subProcess.ActivityName != null ? JsonConvert.DeserializeObject<List<ActivityName>>(Convert.ToString(subProcess.ActivityName)) : new List<ActivityName>();
             }*/
            return subProcessData;
        }

        public async Task<int> DeleteSubProcess(long subProcessId)
        {
            try
            {
                //update process_subprocess_mapping table 
                await _dbContext.Query("process_subprocess_mapping").Where(new { Id = subProcessId }).UpdateAsync(new
                {
                    IsDeleted = true,
                    UpdatedBy = ConfigData.UserId,
                    UpdatedOn = DateTime.UtcNow
                });

                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        #endregion

        #region process-master-methods
        public async Task<List<DropdownItemVM>> GetProcessMasterList()
        {
            var result = await _dbContext.Connection.QueryAsync<DropdownItemVM>(DBHelper.USP_GetProcessMasterList, new { },
                commandType: CommandType.StoredProcedure);

            return result!.ToList();
        }

        #endregion

        #region Get Only Process List
        public async Task<IOnlyProcessListRes> GetOnlyProcessList(ProcessreqVM filter)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetOnlyProcessList, new
            {
                filter.GlobalSearch,
                filter.ProcessName,
                PageNo = filter.PageNo == default ? 1 : filter.PageNo,
                PageSize = filter.PageSize == default ? 1000 : filter.PageSize,
                filter.SortColumn,
                filter.IsDesc,
                filter.IsAvailable,
                filter.IsDownload
            }, commandType: CommandType.StoredProcedure);

            var processData = new OnlyProcessListNotDownloadRes
            {
                TotalCount = (await result.ReadAsync<long>()).FirstOrDefault(),
                List = (await result.ReadAsync<OnlyProcessRespVM>()).ToList()
            };
            if (filter.IsDownload)
            {
                var downloadData = processData.List.Select(x => new
                {
                    Process = x.ProcessName,
                    Active = x.Active == true ? "Active" : "Inactive"
                }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);
                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet);
                return new OnlyProcessListDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "ProcessList.xlsx"
                };
            }
            return processData;
        }
#endregion

        #region Save And Edit Process
        public async Task<long> SaveAndEditProcess(SaveAndEditProcessVM req)
        {
            var processId = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_SaveAndEditProcess, new
            {
                req.ProcessId,
                req.ProcessName,
                SavedBy = ConfigData.UserId,
                SavedOn = DateTime.UtcNow
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response

            return processId;
        }
        #endregion

        #region DELETE Process With Sub Process
        public async Task<int> DeleteProcessWithSubProcess(long processId)
        {
            try
            {
                //Delete SubProcess
                await _dbContext.Query("process").Where(new { parentid = processId, is_deleted = false }).UpdateAsync(new
                {
                    is_deleted = true,
                    updated_by = ConfigData.UserId,
                    updated_on = DateTime.UtcNow
                });

                //Delete Process  
                await _dbContext.Query("process").Where(new { id = processId }).UpdateAsync(new
                {
                    is_deleted = true,
                    updated_by = ConfigData.UserId,
                    updated_on = DateTime.UtcNow
                });
                return 1;

            }
            catch (Exception ex)
            {
                return -1;
            }

        }
        #endregion
    }
}
