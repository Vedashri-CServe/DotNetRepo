using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Entity;

namespace TMS.UserManager.Business
{
    public interface IProcessService
    {
        public ConfigData ConfigData { get; set; }
        public Task<IProcessListRes> GetProcessList(ProcessListFilterVM filter);
        public Task<long> SaveProcess(ProcessVM process);
        public Task<int> DeleteProcess(long processId);
        public Task<ProcessVM> GetProcessById(long processId);

        public Task<SubProcessListVM> GetSubProcessListByProcess(SubProcessListFilterVM filter);

        public Task<IEnumerable<CPAProcessesRes>> GetCPAProcessList(CpaProcessesReq req);

        public Task<bool> SaveCpaProcesses(SaveCpaProcessesReq req);

        #region Subprocess methods
        public Task<SubProcessListVM> GetSubProcessList(SubProcessListFilterVM filter);
        public Task<long> SaveSubProcess(SubProcessVM subProcess);
        public Task<int> DeleteSubProcess(long subProcessId);
        #endregion

        #region Process Master Changes
        public Task<List<DropdownItemVM>> GetProcessMasterList();
        #endregion

        #region Process Details
        public Task<IOnlyProcessListRes> GetOnlyProcessList(ProcessreqVM filter);
        public Task<long> SaveAndEditProcess(SaveAndEditProcessVM req);
        public Task<int> DeleteProcessWithSubProcess(long processId);
        #endregion
    }
}
