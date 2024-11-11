
using MailKit.Net.Smtp;
using MimeKit;
using static System.Configuration.ConfigurationManager;

namespace MyPrivateApp.Components.Email.Classes
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

        public void SendEmailBirthday(string emailTitle, string sendFrom, string subject, string text, string sendFrom2)
        {
            string mail = config.GetSection("AppSettings")["mail"];
            string emailPassword = config.GetSection("AppSettings")["emailPassword"];
            string connect = config.GetSection("AppSettings")["connect"];
            string port = config.GetSection("AppSettings")["port"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailTitle, sendFrom));
            message.To.Add(new MailboxAddress("", mail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = text
            };

            using var client = new SmtpClient();
            client.Connect(connect, int.Parse(port), false);
            client.Authenticate(sendFrom2, emailPassword);
            client.Send(message);
            client.Disconnect(true);
        }

        public void SendEmailFreezer(string emailTitle, string sendFrom, string subject, string text, string sendFrom2)
        {
            string mail = config.GetSection("AppSettings")["mail"];
            string emailPassword2 = config.GetSection("AppSettings")["emailPassword2"];
            string connect = config.GetSection("AppSettings")["connect"];
            string port = config.GetSection("AppSettings")["port"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailTitle, sendFrom));
            message.To.Add(new MailboxAddress("", mail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = text
            };

            using var client = new SmtpClient();
            client.Connect(connect, int.Parse(port), false);
            client.Authenticate(sendFrom2, emailPassword2);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}