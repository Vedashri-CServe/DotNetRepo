using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Entity
{
    public class SendMailVM
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string MailBody { get; set; }

        public IEnumerable<Attachment>? Attachments { get; set; }
    }

    public class EmailTemplateVM
    {
        public long Id { get; set; }
        public string TemplateType { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }

    public class EmailConfigVM
    {
        public string SiteBaseUrl { get; set; }
        public string SMTPserver { get; set; }
        public string SMTPPort { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public bool IsSSLEnabled { get; set; }
    }

    public static class EmailTemplateType
    {
        public const string AccountCreated = "Account Created";
    }
}
