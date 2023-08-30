namespace TMS.Helper
{
    public static class DBHelper
    {
        public const string USP_SaveSystemUser = "USP_SaveSystemUser";
        public const string USP_GetUserList = "USP_GetUserList";
        public const string USP_SaveOrganizationUser = "USP_SaveOrganizationUser";
        public const string USP_DeleteOrganizationUser = "USP_DeleteOrganizationUser";
        public const string USP_GetOrganizationUserList = "USP_GetOrganizationUserList";
        public const string USP_GetClientByCPA = "USP_getClientByCPA";
        public const string USP_SaveTask = "USP_SaveTask";
        public const string USP_GetCPAList = "USP_GetCPAList";
        public const string USP_GetTaskList = "USP_GetTaskList";
        public const string USP_SaveStatus = "USP_SaveStatus";
        public const string USP_GetStatusList = "USP_GetStatusList";
        public const string USP_SaveProcess = "USP_SaveProcess";
        public const string USP_GetProcessList = "USP_GetProcessList";
        public const string USP_GetLookupValues = "USP_GetLookupValues";
        public const string USP_GetUserDetails = "USP_GetUserDetails";
        public const string USP_GetClientAndTaskAndProcessListByCPA = "USP_GetClientAndTaskAndProcessListByCPA";
        public const string USP_SaveWorkPlan = "USP_SaveWorkPlan";
        public const string USP_GetWorkPlanList = "USP_GetWorkPlanList";
        public const string USP_DeleteWorkPlan = "USP_DeleteWorkPlan";
        public const string USP_SaveCheckList = "USP_SaveCheckList";
        public const string USP_GetCheckList = "USP_GetCheckList";
        public const string USP_DeleteCheckList = "USP_DeleteCheckList";
        public const string USP_WorkPlanComment = "USP_WorkPlanComment";
        public const string USP_GetWorkPlanComment = "USP_GetWorkPlanComment";
        public const string USP_SaveRecurringPlan = "USP_SaveRecurringPlan";
        public const string USP_GetRecurringPlan = "USP_GetRecurringPlan";
        public const string USP_DeleteRecurringPlan = "USP_DeleteRecurringPlan";
        public const string USP_SaveTimeLog = "USP_SaveTimeLog";
        public const string USP_GetTimeLineDetails = "USP_GetTimeLineDetails";
        public const string USP_SaveTimeDuration = "USP_SaveTimeDuration";
        public const string USP_SaveIdleTime = "USP_SaveIdleTime";
        public const string USP_GetIdleDuration = "USP_GetIdleDuration";
        public const string USP_ModifiedReviewLogs = "USP_ModifiedReviewLogs";
        public const string USP_GetReviewerWorkPlanList = "USP_GetReviewerWorkPlanList";
        public const string USP_InActiveAccount = "USP_InActiveAccount";
        public const string USP_GetReportingManagerDropdown = "USP_GetReportingManagerDropdown";
        public const string USP_SaveTaskOrganization = "USP_SaveTaskOrganization";

        public const string USP_SaveEventType = "USP_SaveEventType";
        public const string USP_WorkplanRecurringOperations = "USP_WorkplanRecurringOperations";
        public const string USP_GetWorkplanTotalTime = "USP_GetWorkplanTotalTime";
        public const string USP_SaveBreakTime = "USP_SaveBreakTime";
        public const string USP_GetLastBreakTypeByUserId = "USP_GetLastBreakTypeByUserId";
        public const string USP_GetAllWorplanRecurringByDate = "USP_GetAllWorplanRecurringByDate";
        public const string USP_PauseExistingTasksOnLogout = "USP_PauseExistingTasksOnLogout";

        //Reports
        public const string USP_GetCpaReport = "USP_GetCpaReport";
        public const string USP_GetClientReport = "USP_GetClientReport";
        public const string USP_GetActualPlannedReport = "USP_GetActualPlannedReport";
        public const string USP_GetActivityReport = "USP_GetActivityReport";
        public const string USP_GetOtherReport = "USP_GetUserTimesheetWorkLogsWorkloadReport";
        public const string USP_GetTimesheetReport = "USP_GetTimesheetReport";
        public const string USP_GetKRAReport = "USP_GetKRAReport";
        public const string USP_GetAutoManualReport = "USP_GetAutoManualReport";
        public const string USP_DailyReport = "USP_DailyReport";
        public const string USP_LoginLogoutReport = "USP_LoginLogoutReport";
        public const string USP_AuditReport = "USP_AuditReport";
        public const string USP_TMSManagementReport = "USP_TMSManagementReport";
        //Subprocess
        public const string USP_SaveSubProcess = "USP_SaveSubProcess";
        public const string USP_GetSubProcessList = "USP_GetSubProcessList";
        public const string USP_GetSubProcessListByProcess = "USP_GetSubProcessListByProcess";

        //Role Base Permission 
        public const string USP_GetMenuPermissionList = "USP_GetMenuPermissionList";
        public const string USP_UpdateMenuList = "USP_UpdateMenuList";

        //Add role type
        public const string USP_SaveRoleTypeForPermission = "USP_SaveRoleTypeForPermission";
        public const string USP_DeleteRoleType = "USP_DeleteRoleType";

        public const string USP_GetCPAProcesses = "USP_GetCPAProcesses";
        public const string USP_SaveCPAProcesses = "USP_SaveCPAProcesses";

        //Get User dropdown
        public const string USP_GetUserDropdownForHierarchy = "USP_GetUserDropdownForHierarchy";

        //Process Master 
        public const string USP_GetProcessMasterList = "USP_GetProcessMasterList";

        //Only Process 
        public const string USP_GetOnlyProcessList = "USP_GetOnlyProcessList";
        public const string USP_SaveAndEditProcess = "USP_SaveAndEditProcess";

        // Excel Import
        public const string USP_ImportProject = "USP_ImportProject";
    }
}
