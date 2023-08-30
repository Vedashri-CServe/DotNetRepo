using Dapper;
using Newtonsoft.Json;
using SqlKata.Execution;
using System.Data;
using TMS.Entity;
using TMS.Helper;
using TMS.Helper.ExportExcel;

namespace TMS.UserManager.Business
{
    public class UserBo : IUserService
    {
        private readonly QueryFactory _dbContext;
        private readonly IUserEmailProvider _emailProvider;
        public ConfigData ConfigData { get; set; }

        public UserBo(QueryFactory dbContext, IUserEmailProvider emailProvider)
        {
            _dbContext = dbContext;
            _emailProvider = emailProvider;
        }

        #region Post Request
        public async Task<long> SaveUser(UserVM user)
        {
            if (user?.OrganizationIds.Count <= 0)
                return -2;
            var OldData = string.Empty;
            var isExistingUser = user.UserId > 0;
            var userPassowrd = string.Empty;
            if (!isExistingUser)
            {
                user.Password = string.Empty;
                user.IsEmailVerified = false;
                user.TwoFactorEnabled = false;
                user.ProfileImage = string.Empty;
                user.IsActive = false;
                user.IsDeleted = false;
            }
            else
            {
                OldData = (await _dbContext.Query("user").
             Where(new { id = user.UserId, is_deleted = false }).
                     Select("email_id").GetAsync<string>()).FirstOrDefault();

            }
            var userId = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_SaveSystemUser, new
            {
                user.UserId,
                OrganizationIds = string.Join(",", user.OrganizationIds),
                user.ParentId,
                user.FirstName,
                user.LastName,
                user.EmailId,
                user.Password,
                user.DepartmentId,
                user.IsEmailVerified,
                RoleTypeIds = string.Join(",", user.RoleTypeIds),
                user.MobileNo,
                user.TwoFactorEnabled,
                user.ProfileImage,
                user.IsActive,
                user.IsDeleted,
                SavedBy = ConfigData.UserId,
                SavedOn = DateTime.UtcNow,
                user.WorkCategory
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            //check sp response
            if (userId != default)
            {
                //User already exists
                if (userId == -1)
                {
                    return -1;
                }
                if (!isExistingUser || (OldData != string.Empty && OldData != user.EmailId))
                {
                    await _emailProvider.UserAccountActivation(new List<UserBasicDetailVM>
                    {
                        new UserBasicDetailVM
                        {
                            EmailId = user.EmailId,
                            FirstName = user.FirstName,
                            UserId = userId,
                            IsActive = user.IsActive,
                            IsDeleted = user.IsDeleted,
                            IsEmailVerified = user.IsEmailVerified,
                            LastName = user.LastName,
                            Password = userPassowrd,
                            TwoFactorEnabled = user.TwoFactorEnabled
                        }
                    });
                }
                return userId;
            }
            return -5;
        }

        public async Task<bool> DeleteUser(long userId)
        {
            if (ConfigData.UserId == userId)
            {
                return false;
            }
            await _dbContext.Query("user").Where(new { id = userId, is_deleted = false }).UpdateAsync(new
            {
                is_deleted = true,
                is_active = false,
                updated_by = ConfigData.UserId,
                updated_on = DateTime.UtcNow
            });
            await _dbContext.Query("user_role_details").Where(new { user_id = userId, is_deleted = false }).UpdateAsync(new
            {
                is_deleted = true,
                updated_by = ConfigData.UserId,
                updated_on = DateTime.UtcNow
            });
            await _dbContext.Query("user_organization_details").Where(new { user_id = userId, is_deleted = false }).UpdateAsync(new
            {
                is_deleted = true,
                updated_by = ConfigData.UserId,
                updated_on = DateTime.UtcNow
            });
            await _dbContext.Query("user_security_code").Where(new { user_id = userId, is_deleted = false }).UpdateAsync(new
            {
                is_deleted = true,
                updated_by = ConfigData.UserId,
                updated_on = DateTime.UtcNow
            });
            return true;
        }

        public async Task<IUserListRes> GetUserList(UserListFilterVM filter)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetUserList, new
            {
                filter.GlobalSearch,
                filter.RoleTypeId,
                filter.DepartmentId,
                PageNo = filter.PageNo == default ? 1 : filter.PageNo,
                PageSize = filter.PageSize == default ? 1000 : filter.PageSize,
                filter.SortColumn,
                filter.IsDesc,
                filter.UserId,
                filter.IsAvailable,
                filter.IsDownload
            }, commandType: CommandType.StoredProcedure);
            var totalCount = (await result.ReadAsync<long>()).FirstOrDefault();
            var data = (await result.ReadAsync<UserItemDataVM>()).ToList();
            if (filter.IsDownload)
            {
                var downloadData = data.Select(x =>
                   new
                   {
                       UserName = string.Concat(x.FirstName?.Trim(), ' ' , x.LastName?.Trim()),
                       Status = x.IsAvailable ? "Active" : "Inactive",
                       UserType = JsonConvert.DeserializeObject<List<DropdownItemVM>>((string)x.Roles).FirstOrDefault().Label,
                       Department = x.DepartmentName,
                       ReportingTo = x.ReportingManager,
                       MobileNumber = x.MobileNo,
                       Email = x.EmailId
                   }).ToList();

                var dataTable = downloadData.ListToDataTable();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);
                var byteArray = ExportExcel.GetExcelFileFormDataSet(dataSet);
                return new UserListDownloadRes()
                {
                    ByteArray = byteArray,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "UserList.xlsx"
                };
            }
            var list = data.Select(item => new UserListItemVM
            {
                DepartmentType = new DropdownItemVM
                {
                    Label = item.DepartmentName,
                    Value = item.DepartmentId
                },
                Roles = JsonConvert.DeserializeObject<List<DropdownItemVM>>(item.Roles) ?? new(),
                Organizations = JsonConvert.DeserializeObject<List<DropdownItemVM>>(item.Organizations) ?? new(),
                EmailId = item.EmailId,
                FirstName = item.FirstName,
                IsActive = item.IsActive,
                LastName = item.LastName,
                MobileNo = item.MobileNo,
                ParentId = item.ParentId,
                UserId = item.UserId,
                IsAvailable = item.IsAvailable,
                WorkCategory = item.WorkCategory,
                ReportingManager= item.ReportingManager,
                ReportingManagerId=item.ReportingManagerId,
            });
            return new UserListNotDownloadRes
            {
                TotalCount = totalCount,
                List = list.ToList()
            };
        }
        #endregion

        #region Get Request
        public async Task<List<DropdownItemVM>> GetUserRoleList()
        {
            var result = await _dbContext.Query("role_type").
                WhereFalse("is_deleted").
                Where("hierarchy_level", ">=", ConfigData.RoleTypeIds).Select
                (
                    "role_name AS Label",
                    "id AS Value"
                ).GetAsync<DropdownItemVM>();
            return result.ToList();
        }

        public async Task<List<DropdownItemVM>> GetDepartmentDropdown()
        {
            return (await _dbContext.Query("user_department").
            Where(new { is_deleted = false }).
                    Select("id as Value", "department_name AS Label").OrderBy("department_name").GetAsync<DropdownItemVM>()).ToList();
        }


        public async Task<bool> InActiveAccount(InActiveAccountVM reqObj)
        {
            //update Daily Work Plan Table
            long Id = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_InActiveAccount, new
            {
                Id = reqObj.Id,
                IsAvailable = reqObj.IsAvailable,
                TableType = reqObj.TableType,
                CurrentUser = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (Id != reqObj.Id)
                return false;
            return true;
        }

        public async Task<List<DropdownItemVM>> GetReportingManagerDropdown()
        {
            var result = (await _dbContext.Connection.QueryAsync<DropdownItemVM>(DBHelper.USP_GetReportingManagerDropdown,
                commandType: CommandType.StoredProcedure)).ToList();
            return result.ToList();
        }

        public async Task<IEnumerable<MenuRespVM>> GetUserMenuList(UserRoleIdVM userRoleId)
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetMenuPermissionList, new
            {
                UserRole = userRoleId.RoleId == null ? ConfigData.RoleTypeIds?.FirstOrDefault() : userRoleId.RoleId,
            }, commandType: CommandType.StoredProcedure);

            var data = (await result.ReadAsync<MenuRespVM>())?.ToList();
            var response = data != null ? FillRecursive(data, null) : Enumerable.Empty<MenuRespVM>();
            return response!;
        }




        public async Task<long> UpdateMenuList(UpdateMenuReqVM updateMenuReq)
        {
            long isUpdate = (_dbContext.Connection.Query<long>(DBHelper.USP_UpdateMenuList, new
            {
                menuId = updateMenuReq.MenuId,
                permissionId = updateMenuReq.PermissionId,
                displayName = updateMenuReq.DisplayName,
                isShow = updateMenuReq.IsShow,
                isAdd = updateMenuReq.IsAdd,
                isUpdate = updateMenuReq.IsUpdate,
                isDelete = updateMenuReq.IsDelete,
                isActive = updateMenuReq.IsActive,
                isDefaultLandingPage = updateMenuReq.IsDefaultLandingPage,
                roleTypeId = updateMenuReq.RoleTypeId,
                parentId = updateMenuReq.ParentId,
                userId = ConfigData.UserId

            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            return isUpdate;
        }
        #endregion


        private static List<MenuRespVM>? FillRecursive(List<MenuRespVM> flatObjects, long? parentId)
        {
            var selectedElements = flatObjects.Where(x => x.ParentId.Equals(parentId));
            var recursiveObjects = selectedElements.Any() ? new List<MenuRespVM>() : null;
            foreach (var item in selectedElements)
            {
                var roleBaseData = flatObjects.Where(x => x.RoleTypeId.Equals(item.RoleTypeId)).ToList();
                recursiveObjects?.Add(new MenuRespVM
                {
                    MenuId = item.MenuId,
                    PermissionId = item.PermissionId,
                    DefaultName = item.DefaultName,
                    DisplayName = item.DisplayName,
                    ParentId = item.ParentId,
                    IsMainMenu = item.IsMainMenu,
                    IsShow = item.IsShow,
                    IsAdd = item.IsAdd,
                    IsUpdate = item.IsUpdate,
                    IsDelete = item.IsDelete,
                    IsActive = item.IsActive,
                    IsDefaultLandingPage = item.IsDefaultLandingPage,
                    RoleTypeId = item.RoleTypeId,
                    RouteLink = item.RouteLink,
                    Icon = item.Icon,
                    TabData = FillRecursive(roleBaseData, item.MenuId),
                });
            }
            return recursiveObjects;
        }

        #region RoleType CRUD
        public async Task<long> SaveRoleTypeForPermission(RoleTypeVM roleObj)
        {
            if (string.IsNullOrEmpty(roleObj?.RoleName?.Trim()))
                return -1;
            else
            {
                var roleId = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_SaveRoleTypeForPermission, new
                {
                    RoleId = roleObj.RoleId,
                    RoleName = roleObj.RoleName.Trim(),
                    UserId = ConfigData.UserId
                }, commandType: CommandType.StoredProcedure)).FirstOrDefault();

                return roleId;
            }

        }
        public async Task<long> DeleteRoleType(UserRoleIdVM roleObj)
        {
            var roleId = (await _dbContext.Connection.QueryAsync<long>(DBHelper.USP_DeleteRoleType, new
            {
                RoleId = roleObj.RoleId,
                UserId = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();

            return roleId;
        }
        public async Task<List<DropdownItemVM>> GetPermissionRoleList()
        {
            var result = await _dbContext.Query("role_type").
                WhereFalse("is_deleted").
                Where("hierarchy_level", ">", ConfigData.RoleTypeIds).Select
                (
                    "role_name AS Label",
                    "id AS Value"
                ).GetAsync<DropdownItemVM>();
            return result.ToList();
        }
        #endregion

        public async Task<IEnumerable<DropdownItemVM>> GetUsersForDropdown()
        {
            var result = await _dbContext.Connection.QueryMultipleAsync(DBHelper.USP_GetUserDropdownForHierarchy, new
            {    
                Userid = ConfigData.UserId
            }, commandType: CommandType.StoredProcedure);

            var userList = await result.ReadAsync<DropdownItemVM>();
            return userList;
        }
    }

}
