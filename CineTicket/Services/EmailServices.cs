using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendConfirmationEmail(string toEmail, string seatNumber)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("CineTicket", _config["EmailSettings:SenderEmail"]));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = "Xác nhận đặt vé";

        email.Body = new TextPart("plain")
        {
            Text = $"Bạn đã đặt thành công vé cho ghế {seatNumber}."
        };

        using var smtp = new SmtpClient();
        smtp.Connect(_config["EmailSettings:SMTPServer"], int.Parse(_config["EmailSettings:Port"]), false);
        smtp.Authenticate(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderPassword"]);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
