using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Entity
{
    public class AuthLoginLogVM
    {
        public long UserId { get; set; }
        public string HostName { get; set; }
        public string HostAddress { get; set; }
        public string UserAgent { get; set; }
        public string Source { get; set; }
        public bool IsLogout { get; set; } = false;
        public DateTime? LogoutTime { get; set; }
    }

    public class EmailRequestVM
    {
        public long UserId { get; set; }
        public string ToEmail { get; set; }
    }
}
