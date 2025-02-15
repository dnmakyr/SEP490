using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using verbum_service_application.Service;
using verbum_service_domain.Models.Mail;

namespace verbum_service_infrastructure.Impl.Service
{
    public class MailServiceImpl : MailService
    {
        private readonly MailSettings mailSettings;
        public MailServiceImpl(IOptions<MailSettings> _mailSettings)
        {
            mailSettings = _mailSettings.Value;
        }
        public async Task<string> SendEmailAsync(string receiver, string subject, string body)
        {
            return await SendMail(new MailContent()
            {
                To = new List<string> { receiver },
                Subject = subject,
                Body = body
            });
        }

        public async Task<string> SendEmailAsync(List<string> receiver, string subject, string body)
        {
            return await SendMail(new MailContent()
            {
                To = receiver,
                Subject = subject,
                Body = body
            });
        }

        private async Task<string> SendMail(MailContent mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.AddRange(mailContent.To.Select(address => MailboxAddress.Parse(address.Trim())));
            email.Subject = mailContent.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            email.Body = builder.ToMessageBody();
            string mess = "empty";

            // dùng SmtpClient của MailKit
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                Console.WriteLine("Connecting to SMTP server...");
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                Console.WriteLine("Sending email...");
                mess = await smtp.SendAsync(email);

                Console.WriteLine("Disconnecting from SMTP server..." + mess);

                smtp.Disconnect(true);
            }
            return mess;
        }
    }
}
