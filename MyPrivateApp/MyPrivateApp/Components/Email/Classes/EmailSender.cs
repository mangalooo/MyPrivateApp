
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MyPrivateApp.Components.Email.Classes
{
    public class EmailSender(IConfiguration config) : IEmailSender
    {
        private readonly IConfiguration _config = config;

        private async Task SendEmail(string emailTitle, string sendFrom, string subject, string text, string sendFrom2, string emailPassword)
        {
            string? mail = _config.GetSection("AppSettings")["mail"];
            string? connect = _config.GetSection("AppSettings")["connect"];
            string? port = _config.GetSection("AppSettings")["port"];

            if (string.IsNullOrEmpty(mail) || string.IsNullOrEmpty(connect) || string.IsNullOrEmpty(port))
                throw new InvalidOperationException("Email configuration settings cannot be null or empty.");

            MimeMessage message = new();
            message.From.Add(new MailboxAddress(emailTitle, sendFrom));
            message.To.Add(new MailboxAddress("", mail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = text
            };

            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true; // Accept all certificates (not recommended for production)
                await client.ConnectAsync(connect, int.Parse(port), SecureSocketOptions.Auto);
                await client.AuthenticateAsync(sendFrom2, emailPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException("Fel när e-post skickades!", ex);
            }
        }

        public async Task SendEmailBirthday(string emailTitle, string sendFrom, string subject, string text, string sendFrom2)
        {
            string? emailPassword = _config.GetSection("AppSettings")["emailPassword"];

            if (string.IsNullOrEmpty(emailPassword))
                throw new InvalidOperationException("E-post lösenord måste finnas!");

            await SendEmail(emailTitle, sendFrom, subject, text, sendFrom2, emailPassword);
        }

        public async Task SendEmailFreezer(string emailTitle, string sendFrom, string subject, string text, string sendFrom2)
        {
            string? emailPassword2 = _config.GetSection("AppSettings")["emailPassword2"];

            if (string.IsNullOrEmpty(emailPassword2))
                throw new InvalidOperationException("E-post lösenord måste finnas!");

            await SendEmail(emailTitle, sendFrom, subject, text, sendFrom2, emailPassword2);
        }
    }
}