using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace CineTicket.Repositories
{
    public class GmailSender : IGmailSender, IEmailSender
    {
        private readonly IConfiguration _configuration;

        public GmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Gọi từ Hangfire hoặc thủ công
        public async Task SendEmail(string to, string subject, string body)
        {
            await SendEmailAsync(to, subject, body);
        }

        // Gọi từ Identity UI
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpSection = _configuration.GetSection("Smtp");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CineTicket", smtpSection["FromEmail"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            // 👉 Nhận nội dung HTML từ bên ngoài
            message.Body = new TextPart("html") { Text = htmlMessage };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpSection["Server"], int.Parse(smtpSection["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpSection["Username"], smtpSection["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
