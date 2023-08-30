using TMS.Entity;

namespace TMS.Helper
{
    public interface IEmailHelper
    {
        public Task<EmailConfigVM> GetEmailConfig();
        public Task<EmailTemplateVM> GetTemplateByType(string templateType);
        public Task SendEmailAsync(SendMailVM request, EmailConfigVM config = null);
    }
}
