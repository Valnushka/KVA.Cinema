namespace KVA.Cinema.Services
{
    using KVA.Cinema.Models.Mail;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web;

    public class EmailSender
    {
        public EmailSettings EmailSettings { get; }

        public IUrlHelper UrlHelper { get; }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public EmailSender(IOptions<EmailSettings> emailSettings,
                           IUrlHelper myProperty,
                           IHttpContextAccessor iHttpContextAccessor,
                           IWebHostEnvironment webHostEnvironment)
        {
            EmailSettings = emailSettings.Value;
            UrlHelper = myProperty;
            HttpContextAccessor = iHttpContextAccessor;
            WebHostEnvironment = webHostEnvironment;
        }

        public Task SendMessageAsync(string email, string subject, string message)
        {
            var client = new SmtpClient
            {
                Host = EmailSettings.Host,
                Port = EmailSettings.Port,
                EnableSsl = EmailSettings.EnableSsl,
                Credentials = new NetworkCredential(EmailSettings.Email, EmailSettings.Password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(EmailSettings.Email),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }

        public async Task SendActivateAccountLinkAsync(string email, string userId, string nickname, string userToken)
        {
            var scheme = HttpContextAccessor.HttpContext.Request.Scheme;
            string userIdEncoded = HttpUtility.UrlEncode(userId);
            string userTokenEncoded = HttpUtility.UrlEncode(userToken);

            string url = UrlHelper.Action("ConfirmEmail", "Users", new { userId = userIdEncoded, userToken = userTokenEncoded }, scheme);

            var model = new ConfirmEmailModel
            {
                UserId = userId,
                Nickname = nickname,
                ConfirmationLink = url
            };

            var messageToSend = await RenderTemplateAsync(Path.Combine(WebHostEnvironment.WebRootPath, "MailTemplates", "ConfirmEmailMessage.html"), model);

            await SendMessageAsync(email, "Confirm your email", messageToSend);
        }

        private async Task<string> RenderTemplateAsync(string templatePath, ConfirmEmailModel model)
        {
            string template = await File.ReadAllTextAsync(Path.Combine(WebHostEnvironment.WebRootPath, templatePath));

            Dictionary<string, string> replacements = model.GetReplacementPairs();

            foreach (var replacement in replacements)
            {
                template = template.Replace(replacement.Key, replacement.Value);
            }

            return template;
        }
    }
}
