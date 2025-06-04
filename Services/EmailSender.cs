using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Melotrade.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSSL
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSSL { get; set; }
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}
