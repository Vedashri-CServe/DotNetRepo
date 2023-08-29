using SqlKata.Execution;
using System.Net.Mail;
using TMS.Entity;

namespace TMS.Helper
{
    public class EmailHelper : IEmailHelper
    {
        private readonly QueryFactory _dbContext;
        public EmailHelper(QueryFactory dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<EmailConfigVM> GetEmailConfig()
        {
            return await _dbContext.Query("site_config").
            Select(
                "site_base_url AS SiteBaseUrl",
                "smtp_server AS SMTPserver",
                "smtp_port AS SMTPPort",
                "smtp_user_name AS SMTPUserName",
                "smtp_password AS SMTPPassword",
                "is_ssl_enabled AS IsSSLEnabled"
            ).FirstOrDefaultAsync<EmailConfigVM>();
        }

        public async Task<EmailTemplateVM> GetTemplateByType(string templateType)
        {
            return await _dbContext.Query("email_template").

                         Where("template_type", templateType).WhereFalse("is_deleted").
                         Select("id AS Id", "template_type AS TemplateType", "subject AS Subject", "content AS Content").
                         FirstOrDefaultAsync<EmailTemplateVM>();
        }

        public async Task SendEmailAsync(SendMailVM request, EmailConfigVM config = null)
        {
            if (config == null)
            {
                config = await GetEmailConfig();
            }

            MailMessage mail = new MailMessage
            {
                From = new MailAddress("noreply@pathquest.com")
                //From = new MailAddress("itrsupport@hls-global.com")
                //From = new MailAddress("noreply@technomark.io")
            };
            mail.To.Add(request.Email);
            mail.Subject = request.Subject;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.Body = request.MailBody;

            if(request.Attachments?.Any() ?? false)
            {
                foreach (var attachment in request.Attachments)
                {
                    mail.Attachments.Add(attachment);
                }
            }

            //SMTPServer
            SmtpClient SmtpServer = new SmtpClient(config.SMTPserver)
            {
                //SMTPPort
                Port = Convert.ToInt16(config.SMTPPort),
                UseDefaultCredentials = false,
                //Set SMTP Username and Password
                Credentials = new System.Net.NetworkCredential(config.SMTPUserName, config.SMTPPassword),
                //EnableSSL
                EnableSsl = config.IsSSLEnabled
            };

            SmtpServer.Send(mail);
        }
    }
}
