namespace CineTicket.Repositories
{
    public interface IGmailSender
    {
        Task SendEmail(string to, string subject, string body);
    }
}
