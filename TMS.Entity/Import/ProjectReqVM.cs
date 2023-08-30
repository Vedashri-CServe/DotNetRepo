using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Entity
{
    #region Project
    public class ProjectReqVM
    {
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public long OrganizationType { get; set; } = 3;
        public string TypeOfWork { get; set;}
        public Boolean SOP { get; set;} = false;
    }
    public class ImportDataReqVM
    {
        public string OrganizationName { get; set; }
        public long ParentId { get; set; }
        public long OrganizationType { get; set; } = 3;
        public long TypeOfWork { get; set; }
        public Boolean SOP { get; set; } = false;
    }

    public class ProjectRespVM
    {
        public long Response { get; set; }
        public List<ProjectReqVM> ProjectReqVMs { get; set; }
    }
    public interface IImportReportRes
    {

    }

    public class ProjectDownloadResp : IImportReportRes
    {
        public byte[] ByteArray { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
    #endregion

    #region Process

    public class ProcessReqVM
    {
        public string ProcessName { get; set; }
        public string SubProcessName { get; set; }
        public string ActivityName { get; set; }
        public string OrganizationName { get; set; }
        public decimal? EstimatedDuration { get; set; }
        public bool IsProductive { get; set; }
        public bool IsBillable { get; set; }
    }
  
    #endregion

    #region Clients
    public class ImportClientsVM
    {
        public string ClientName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string TypeOfWork { get; set; }
        public string BillingType { get; set; }
        public long ContractedHours { get; set; }
        public long InternalHours { get; set; }
    }

    public class ImportClientsRecordsVM
    {
        public string OrganizationName { get; set; }
        public string CompanyName { get; set; }
        public string WebsiteUrl { get; set; } = "www.rm1.com";
        public long ParentId { get; set; } = 0;
        public long OrganizationType { get; set; } = 2;
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public long TypeOfWork { get; set; }
        public long BillingType { get; set; }
        public long ContractedHours { get; set; }
        public long InternalHours { get; set; }
    }
    #endregion

    #region Tasks
    public class ImportTaskVM
    {
        public string OrganizationName { get; set; }
        public string TaskName { get; set; }
        public decimal? EstimatedDuration { get; set; }
        public bool IsProductive { get; set; }
        public bool IsBillable { get; set; }

    }
    #endregion

    #region Users
    public class ImportUserExcelData
    {
        public string OrganizationName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string UserType { get; set; }
        public string WorkCategory { get; set; }
        public string Department { get; set; }
        public string ReportingManager { get; set; }

    }
    #endregion
}
