using Dapper;
using Newtonsoft.Json;
using SqlKata.Execution;
using System.Data;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.ExportExcel;
using TMS.Helper.ImportExcel;

namespace TMS.UserManager.Business
{
    public class ImportDataBo : IImportService
    {
        private readonly QueryFactory _dbContext;
        public ConfigData ConfigData { get; set; }
        private readonly IProcessService _processService;
        public ImportDataBo(QueryFactory dbContext, IProcessService processService)
        {
            _dbContext = dbContext;
            _processService = processService;
        }


        #region Project
        public async Task<ProjectRespVM> ImportProjectList(List<ProjectReqVM> projectReq)
        {
            List<DropdownItemVM> typeOfWorkList = new List<DropdownItemVM>();
            List<DropdownItemVM> organisationList = new List<DropdownItemVM>();
            List<ImportDataReqVM> ImportProjectsRecords = new List<ImportDataReqVM>();
            List<ProjectReqVM> InvalidRequestList = new List<ProjectReqVM>();

            organisationList = _dbContext.Query("organization").
                                WhereFalse("IsDeleted").
                                Where("OrganizationType", "=", 2).
                                Select("Id as Value", "OrganizationName as Label").
                                Get<DropdownItemVM>().ToList();

            var parentId = _dbContext.Query("lookup_values").WhereFalse("is_deleted").Where("short_description", "=", "TYPEOFWORK").Select("id");

            typeOfWorkList = _dbContext.Query("lookup_values")
                                .WhereFalse("is_deleted").
                                Where("parent_id", "=", parentId).
                                Select("description AS Label", "id AS Value")
                                .Get<DropdownItemVM>().ToList();

            // check invalid request if any 
            InvalidRequestList = ImportsListValidation<ProjectReqVM>(projectReq);

            projectReq = projectReq.Except(InvalidRequestList).ToList();

            foreach (var item in projectReq)
            {
                var TypeOfWork = typeOfWorkList.Find(x => x.Label.Equals(item.TypeOfWork));
                var Organization = organisationList.Find(x => x.Label.Equals(item.ClientName));

                if (TypeOfWork == null || Organization == null)
                {
                    InvalidRequestList.Add(item);
                }
                else
                {
                    ImportDataReqVM insertRecord = new ImportDataReqVM();
                    insertRecord.OrganizationName = item.ProjectName;
                    insertRecord.OrganizationType = Convert.ToInt64(item.OrganizationType);
                    insertRecord.ParentId = Organization.Value;
                    insertRecord.TypeOfWork = TypeOfWork.Value;
                    insertRecord.SOP = item.SOP;
                    //Add Records.
                    ImportProjectsRecords.Add(insertRecord);
                }
            }
            var exist = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_ImportProject, new
            {
                InsertData = JsonConvert.SerializeObject(ImportProjectsRecords),
                UserId = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();

            return new ProjectRespVM()
            {
                Response = InvalidRequestList.Count > 0 ? -1 : exist,
                ProjectReqVMs = InvalidRequestList
            };
        }

        public async Task<ProjectDownloadResp> DownloadInvalidRecodes(List<ProjectReqVM> InvalidData)
        {
            ProjectDownloadResp invalid = new ProjectDownloadResp();

            if (InvalidData.Count > 0)
            {
                var downloadData = InvalidData.Select(x => new
                {
                    ClientName = x.ClientName ?? (object)DBNull.Value,
                    ProjectName = x.ProjectName ?? (object)DBNull.Value,
                    TypeOfWork = x.TypeOfWork ?? (object)DBNull.Value,
                    SOP = x.SOP
                }).ToList();
                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                DateTime exportedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                      TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                Dictionary<string, string> content = new()
                {
                    { "Date",$"{exportedDate :dd-MM-yyyy hh:mm tt}" }
                };
                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet, true, content);

                invalid = new ProjectDownloadResp()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "InvalidRecodes.xlsx"
                };
            }
            return invalid;
        }


        #endregion

        #region Process
        public async Task<List<ProcessReqVM>> ImportProcessList(List<ProcessReqVM> processReq)
        {
            List<DropdownItemVM> organisationList = new List<DropdownItemVM>();

            organisationList = _dbContext.Query("organization").
                                WhereFalse("IsDeleted").
                                Where("OrganizationType", "=", 2).
                                Select("Id as Value", "OrganizationName as Label").
                                Get<DropdownItemVM>().ToList();

            // check invalid request if any 
            var InvalidRequestList = ImportsListValidation<ProcessReqVM>(processReq);
            processReq = processReq.Except(InvalidRequestList).ToList();

            if (processReq.Count > 0)
            {
                foreach (var item in processReq)
                {
                    var Activity = item.ActivityName.Split("`").
                                    Select(x => x.Trim()).
                                    Where(y => y != string.Empty).ToList();
                    var splitOrganizationList = item.OrganizationName.Split("`").
                                                Select(x => x.Trim()).
                                                Where(y => y != string.Empty).ToList();

                    List<ActivityName> ActivityList = new List<ActivityName>();
                    foreach (var activity in Activity)
                    {
                        var activityJsonObj = new ActivityName { value = activity };
                        ActivityList.Add(activityJsonObj);
                    }
                    string ActivityNames = JsonConvert.SerializeObject(ActivityList);

                    // find distinct organization names
                    organisationList = organisationList.GroupBy(x => new { Label = x.Label })
                                                    .Select(grp => grp.First())
                                                    .ToList();
                    var Organization = organisationList.FindAll(x => splitOrganizationList.Contains(x.Label)).ToList();

                    if (!string.IsNullOrEmpty(ActivityNames) && Organization.Any() && Organization.Count.Equals(splitOrganizationList.Count()))
                    {
                        var process = new
                        {
                            ProcessId = 0,
                            ProcessName = item.ProcessName.Trim(),
                            SubProcessName = item.SubProcessName.Trim(),
                            OrganizationIds = string.Join(",", Organization.Select(x => x.Value).ToList()),
                            ActivityName = ActivityNames,
                            EstimatedDuration = Convert.ToDecimal(item.EstimatedDuration),
                            IsProductive = item.IsProductive,
                            IsBillable = item.IsBillable,
                            SavedBy = ConfigData.UserId,
                            SavedOn = DateTime.UtcNow
                        };
                        long processId = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_SaveProcess,
                            process, commandType: CommandType.StoredProcedure)).FirstOrDefault();

                        if (processId < 0)
                            InvalidRequestList.Add(item);
                    }
                    else
                        InvalidRequestList.Add(item);
                }

            }
            return InvalidRequestList;
        }
        #endregion


        public async Task<List<ImportClientsVM>> ImportClientsExcel(List<ImportClientsVM> req)
        {
            List<ImportClientsVM> invalidRecords = new();
            List<ImportClientsRecordsVM> importClientsRecords = new();

            var typeOfWorkId = _dbContext.Query("lookup_values").WhereFalse("is_deleted").Where("short_description", "=", LookupShortDesc.TYPEOFWORK).Select("id");
            var billingTypeId = _dbContext.Query("lookup_values").WhereFalse("is_deleted").Where("short_description", "=", LookupShortDesc.BILLINGTYPE).Select("id");

            List<DropdownItemVM> typeOfWorkList = (_dbContext.Query("lookup_values")
                .WhereFalse("is_deleted").Where("parent_id", "=", typeOfWorkId).Select(
                "description AS Label",
                "id AS Value").Get<DropdownItemVM>()).ToList();

            List<DropdownItemVM> billingTypeList = (_dbContext.Query("lookup_values").
            WhereFalse("is_deleted").
            Where("parent_id", "=", billingTypeId).Select("description AS Label", "id AS Value").Get<DropdownItemVM>()).ToList();

            // check invalid request if any 
            var invalidRequestList = ImportsListValidation<ImportClientsVM>(req);
            req = req.Except(invalidRequestList).ToList();

            if (req.Count > 0)
            {
                foreach (ImportClientsVM item in req)
                {
                    var typeOfWork = typeOfWorkList.Find(x => x.Label.Equals(item.TypeOfWork));
                    var billingType = billingTypeList.Find(x => x.Label.Equals(item.BillingType));

                    if (typeOfWork != null && billingType != null)
                    {
                        ImportClientsRecordsVM AddValue = new ImportClientsRecordsVM();
                        AddValue.OrganizationName = item.ClientName;
                        AddValue.CompanyName = item.ClientName;
                        AddValue.Email = item.Email;
                        AddValue.MobileNumber = item.MobileNumber;
                        AddValue.Address = item.Address;
                        AddValue.TypeOfWork = typeOfWork.Value;
                        AddValue.BillingType = billingType.Value;
                        AddValue.ContractedHours = item.ContractedHours;
                        AddValue.InternalHours = item.InternalHours;

                        //Add Records.
                        importClientsRecords.Add(AddValue);
                    }
                    else
                    {
                        //Skip records.
                        invalidRecords.Add(item);
                    }
                }
            }
            if (importClientsRecords.Count > 0)
            {
                var tableColumns = new[] {"OrganizationName","CompanyName","WebsiteUrl","ParentId","OrganizationType","EmailId",
                    "MobileNo","Address","TypeOfWork", "BillType","ContractedHours", "InternalHours","IsDeleted","CreatedBy", "CreatedOn","SOP",
                    "IsAvailable"
                };
                var insertRecord = importClientsRecords.Select(x => new object[]
                {
                    x.OrganizationName,
                    x.CompanyName,
                    x.WebsiteUrl,
                    x.ParentId,
                    x.OrganizationType,
                    x.Email,
                    x.MobileNumber,
                    x.Address,
                    x.TypeOfWork,
                    x.BillingType,
                    x.ContractedHours,
                    x.InternalHours,
                    0,
                    ConfigData.UserId,
                    DateTime.UtcNow,
                    0,
                    1
                }).ToArray();
                // check insert record
                if (insertRecord.Length > 0)
                {
                    var data = await _dbContext.Query("organization").InsertAsync(tableColumns, insertRecord);
                }
            }

            return invalidRecords;
        }

        public async Task<List<ImportTaskVM>> ImportTaskExcel(List<ImportTaskVM> req)
        {
            List<ImportTaskVM> invalidRecords = new();

            List<DropdownItemVM> clientsList = (_dbContext.Query("organization")
                .WhereFalse("IsDeleted").Where("OrganizationType", "=", 2).Select(
                "OrganizationName AS Label",
                "Id AS Value").Get<DropdownItemVM>()).ToList();

            // check invalid request if any 
            invalidRecords = ImportsListValidation<ImportTaskVM>(req);
            req = req.Except(invalidRecords).ToList();

            if (req.Count > 0)
            {
                foreach (ImportTaskVM item in req)
                {
                    var Org = item.OrganizationName.Split("`").ToList();
                    var orgExist = clientsList.FindAll(x => Org.Contains(x.Label)).DistinctBy(i => i.Label).ToList();
                    if (Org.Count.Equals(orgExist.Count))
                    {
                        //Add Records.
                        long insertId = _dbContext.Query("task").InsertGetId<long>(
                            new
                            {
                                task_name = item.TaskName,
                                estimated_duration = item.EstimatedDuration,
                                is_productive = item.IsProductive,
                                is_billable = item.IsBillable,
                                is_deleted = false,
                                created_by = ConfigData.UserId,
                                created_on = DateTime.UtcNow
                            });
                        await _dbContext.Connection.QueryAsync<TaskVMResultVM>(DBHelper.USP_SaveTaskOrganization, new
                        {
                            TaskId = insertId,
                            OrganizationIds = string.Join(",", orgExist.Select(x => x.Value)),
                            SavedBy = ConfigData.UserId,
                        }, commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        //skip Recodes.
                        invalidRecords.Add(item);
                    }
                }
            }
            return invalidRecords;
        }

        public async Task<List<ImportUserExcelData>> ImportUserExcel(List<ImportUserExcelData> req)
        {
            List<ImportUserExcelData> invalidRecords = new();
            List<ImportUserExcelData> validRecords = new();

            var userWorkTypeId = _dbContext.Query("lookup_values").WhereFalse("is_deleted").Where("short_description", "=", LookupShortDesc.USERWORK).Select("id");

            List<DropdownItemVM> UserRoleTypeList = (_dbContext.Query("role_type")
               .WhereFalse("is_deleted").Select("role_name AS Label", "id AS Value").Get<DropdownItemVM>()).ToList();

            List<DropdownItemVM> workCategryList = (_dbContext.Query("lookup_values")
                .WhereFalse("is_deleted").Where("parent_id", "=", userWorkTypeId).Select(
                "description AS Label",
                "id AS Value").Get<DropdownItemVM>()).ToList();

            List<DropdownItemVM> departmentList = (_dbContext.Query("user_department")
                .WhereFalse("is_deleted").Select("department_name AS Label", "id AS Value").Get<DropdownItemVM>()).ToList();

            List<DropdownItemVM> reportingManagerList = (await _dbContext.Connection.QueryAsync<DropdownItemVM>(DBHelper.USP_GetReportingManagerDropdown,
                commandType: CommandType.StoredProcedure)).ToList();

            List<DropdownItemVM> clientsList = (_dbContext.Query("organization")
               .WhereFalse("IsDeleted").Where("OrganizationType", "=", 2).Select(
               "OrganizationName AS Label",
               "Id AS Value").Get<DropdownItemVM>()).ToList();

            // check invalid request if any 
            invalidRecords = ImportsListValidation<ImportUserExcelData>(req);
            req = req.Except(invalidRecords).ToList();

            if (req.Any())
            {
                foreach (ImportUserExcelData item in req)
                {
                    var userExist = _dbContext.Query("user")
                  .WhereFalse("is_deleted").Where("email_id", "=", item.Email).Select(
                  "email_id as userExist").Get<string>().FirstOrDefault();

                    if (userExist != item.Email && userExist == null)
                    {
                        var Org = item.OrganizationName.Split("`").ToList();
                        var orgExist = clientsList.FindAll(x => Org.Contains(x.Label)).DistinctBy(i => i.Label).ToList();
                        var userType = UserRoleTypeList.Find(x => x.Label.Equals(item.UserType));
                        var workCategry = workCategryList.Find(x => x.Label.Equals(item.WorkCategory));
                        var department = departmentList.Find(x => x.Label.Equals(item.Department));
                        var reportingManager = reportingManagerList.Find(x => x.Label.Equals(item.ReportingManager));

                        if (userType != null && workCategry != null && department != null && reportingManager != null && Org.Count.Equals(orgExist.Count))
                        {
                            //Add Records.                     
                            var userId = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_SaveSystemUser, new
                            {
                                UserId = 0,
                                OrganizationIds = string.Join(",", orgExist.Select(x => x.Value)),
                                ParentId = reportingManager.Value,
                                FirstName = item.FirstName,
                                LastName = item.LastName,
                                EmailId = item.Email,
                                Password = string.Empty,
                                DepartmentId = department.Value,
                                IsEmailVerified = true,
                                RoleTypeIds = string.Join(",", userType.Value),
                                MobileNo = item.MobileNo,
                                TwoFactorEnabled = true,
                                ProfileImage = string.Empty,
                                IsActive = true,
                                IsDeleted = false,
                                SavedBy = ConfigData.UserId,
                                SavedOn = DateTime.UtcNow,
                                WorkCategory = workCategry.Value
                            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                            if (userId == null)
                                invalidRecords.Add(item);
                        }
                        else
                        {
                            //Skip Records
                            invalidRecords.Add(item);
                        }
                    }
                    else
                    {
                        //Skip Records
                        invalidRecords.Add(item);
                    }
                }
            }
            return invalidRecords;
        }

        #region Validation

        public List<T> ImportsListValidation<T>(List<T> projectReqVM) where T : class
        {
            List<T> InvalidRequestList = new List<T>();
            foreach (var item in projectReqVM)
            {
                if (ImportExcel.IsValid(item))
                    continue;
                else
                    InvalidRequestList.Add(item);
            }
            return InvalidRequestList;
        }
        #endregion
    }
}
