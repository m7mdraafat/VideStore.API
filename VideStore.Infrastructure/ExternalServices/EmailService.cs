using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using VideStore.Application.DTOs;
using VideStore.Application.Interfaces;
using VideStore.Domain.ConfigurationsData;
using VideStore.Infrastructure.Interfaces;

namespace VideStore.Infrastructure.ExternalServices
{
    public class EmailService(IOptions<MailData> options) : IEmailService
    {
        private readonly MailData _mailData = options.Value;
        public async Task SendEmailMessage(EmailResponse email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailData.Email),
                Subject = email.Subject,
            };

            mail.From.Add(new MailboxAddress("Vide-Store", _mailData.Email));
            mail.To.Add(new MailboxAddress("User", email.To));

            var builder = new BodyBuilder
            {
                HtmlBody = email.Body
            };

            mail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            // Connect to the SMTP server
            try
            {
                await smtp.ConnectAsync(_mailData.Host, _mailData.Port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(_mailData.Email, _mailData.Password);
                await smtp.SendAsync(mail);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log the error or rethrow it
                Console.WriteLine($"Failed to send email: {ex.Message}");
                throw;
            }
        }

    }

}
