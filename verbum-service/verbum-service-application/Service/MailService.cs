namespace verbum_service_application.Service
{
    public interface MailService
    {
        Task<string> SendEmailAsync(string email, string subject, string body);

        Task<string> SendEmailAsync(List<string> receiver, string subject, string body);

    }
}
