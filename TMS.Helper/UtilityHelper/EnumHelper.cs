using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Helper
{
    public static class LookupShortDesc
    {
        public const string BILLINGTYPE = "BILLINGTYPE";
        public const string TYPEOFWORK = "TYPEOFWORK";
        public const string WORKPLANSTATUS = "WORKPLANSTATUS";
        public const string USERWORK = "USERWORK";
    }
    public enum ApprovedStatus
    {
        Approved = 20,
        Rejected = 21,
        Submitted = 22,
        PartiallySubmitted = 34,
        NotSubmitted = 35
    }

    public enum TableEnum
    {
        CPA = 1,
        Client = 2,
        User = 3,
        Task = 4,
        Process = 5,
        SubProcess = 6,
    }
}
