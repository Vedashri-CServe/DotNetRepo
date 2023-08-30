using TMS.Entity;
using TMS.Helper;

namespace TMS.UserManager.Business
{
    public class UserEmailProvider : IUserEmailProvider
    {
        private readonly IEmailHelper _emailProvider;
        private readonly IUserSecurityToken _userSecurityToken;

        public UserEmailProvider(IEmailHelper emailProvider, IUserSecurityToken userSecurityToken)
        {
            _emailProvider = emailProvider;
            _userSecurityToken = userSecurityToken;
        }

        public async Task UserAccountActivation(List<UserBasicDetailVM> userDetail)
        {
            var template = await _emailProvider.GetTemplateByType("Account Activation");
            if (template != null)
            {

                //get mail config
                var mailConfig = await _emailProvider.GetEmailConfig();
                foreach (var user in userDetail)
                {
                    string mailBody = template.Content;
                    //save token
                    var token = await _userSecurityToken.SaveTokenAsync(user.UserId, UserSecurityTokenType.UserActivation);
                    //check token
                    if (!string.IsNullOrEmpty(token))
                    {
                        mailBody = mailBody.Replace("{!username!}", string.IsNullOrEmpty(user.LastName) ? user.FirstName : user.FirstName + " " + user.LastName);
                        mailBody = mailBody.Replace("{!baseurl!}", mailConfig.SiteBaseUrl);
                        mailBody = mailBody.Replace("{!code!}", token);
                        mailBody = mailBody.Replace("{!type!}", ((int)UserSecurityTokenType.UserActivation).ToString());
                        await _emailProvider.SendEmailAsync(new SendMailVM
                        {
                            Email = user.EmailId,
                            Subject = template.Subject,
                            MailBody = mailBody
                        }, mailConfig);
                    }
                }
            }
        }

        public async Task UserAccountCreated(UserBasicDetailVM userDetail)
        {
            var template = await _emailProvider.GetTemplateByType(EmailTemplateType.AccountCreated);
            if (template != null)
            {
                var mailConfig = await _emailProvider.GetEmailConfig();
                template.Content = template.Content.Replace("{!baseurl!}", mailConfig.SiteBaseUrl.Trim());
                template.Content = template.Content.Replace("{!password!}", userDetail.Password);
                template.Content = template.Content.Replace("{!username!}", string.IsNullOrEmpty(userDetail.LastName) ? userDetail.FirstName : userDetail.FirstName + " " + userDetail.LastName);
                await _emailProvider.SendEmailAsync(new SendMailVM
                {
                    Email = userDetail.EmailId,
                    Subject = template.Subject,
                    MailBody = template.Content
                }, mailConfig);
            }
        }
        public async Task ForgetPassword(UserBasicDetailVM userBasicDetail)
        {
            var template = await _emailProvider.GetTemplateByType("Forget Password");
            if (template != null)
            {
                //get mail config
                var mailConfig = await _emailProvider.GetEmailConfig();
                string mailBody = template.Content;
                //save token
                var token = await _userSecurityToken.SaveTokenAsync(userBasicDetail.UserId, UserSecurityTokenType.ForgotPassword);
                //check token
                if (!string.IsNullOrEmpty(token))
                {
                    mailBody = mailBody.Replace("{!username!}", string.IsNullOrEmpty(userBasicDetail.LastName) ? userBasicDetail.FirstName : userBasicDetail.FirstName + " " + userBasicDetail.LastName);
                    mailBody = mailBody.Replace("{!useremail!}", userBasicDetail.EmailId);
                    mailBody = mailBody.Replace("{!baseurl!}", mailConfig.SiteBaseUrl);
                    mailBody = mailBody.Replace("{!code!}", token);
                    mailBody = mailBody.Replace("{!type!}", ((int)UserSecurityTokenType.ForgotPassword).ToString());
                    await _emailProvider.SendEmailAsync(new SendMailVM
                    {
                        Email = userBasicDetail.EmailId,
                        Subject = template.Subject,
                        MailBody = mailBody
                    }, mailConfig);
                }
            }
        }
    }
}
