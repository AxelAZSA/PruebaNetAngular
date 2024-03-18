using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace MailService.Service
{
    public class SendMailService : ISendMailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _config;
        public SendMailService(IConfiguration config)
        {
            _config = config;
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config.GetValue<string>("Email:mail"), _config.GetValue<string>("Email:pass")),
                EnableSsl = true
            };
        }

        public void SendEmail(string from, string to, string subject, string body)
        {
            var mailMessage = new MailMessage(from, to, subject, body)
            {
                IsBodyHtml = true
            };
            _smtpClient.Send(mailMessage);
        }
    }
}
