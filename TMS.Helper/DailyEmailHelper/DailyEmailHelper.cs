using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Helper
{
    public class DailyEmailHelper :IDailyEmailHelper
    {
        public string emails { get; }

        public DailyEmailHelper(string emailList)
        {
            emails = emailList;
        }
    }
}
