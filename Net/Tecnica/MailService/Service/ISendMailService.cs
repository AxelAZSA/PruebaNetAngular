namespace MailService.Service
{
    public interface ISendMailService
    {
        public void SendEmail(string from, string to, string subject, string body);
    }
}
