using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Auth
{
    public class TokenValidationVM
    {
        public bool IsValid { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
