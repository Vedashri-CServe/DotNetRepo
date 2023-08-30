using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Auth
{
    public class TokenReponseVM
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
    }
}
