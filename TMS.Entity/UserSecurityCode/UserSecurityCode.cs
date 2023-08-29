using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Entity
{
    public class UserSecurityCode
    {
        public string Token { get; set; }
        public DateTime ExpiryTime { get; set; }
    }

    public class UserSecurityCodeDTO : UserSecurityCode
    {
        public long UserId { get; set; }
    }

    public class UserSecurityCodeResultVM
    {
        public long UserId { get; set; }
        public int Result { get; set; }
    }

    public class UpdateTokenStatusVM
    {
        public long UserId { get; set; }
        public string Token { get; set; }
        public int Type { get; set; }
    }
}
