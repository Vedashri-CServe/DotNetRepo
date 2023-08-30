using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Helper
{
    public class ManagementEmailHelper : IManagementReportEmailHelper
    {
        public string emails { get; }
        public ManagementEmailHelper(string emailList)
        {
            emails = emailList;
        }

        public string InsertIntoHtmlTemplate<T>(List<T> list)
        {
            StringBuilder sb = new StringBuilder();

            if (list == null) { return sb.ToString(); }
            foreach (var item in list!)
            {
                sb.Append("<tr>");
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    sb.Append("<td align=\"center\" valign=\"middle\" style=\"font-size: 14px;font-weight: 500;color:#232323;border:1px solid #cccccc;\">" + (info.GetValue(item, null) ?? DBNull.Value) + "</td>");
                }
                sb.Append("</tr>");
            }

            return sb.ToString();
        }

    }
}
