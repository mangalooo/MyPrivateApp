

namespace MyPrivateApp.Components.Email.Classes
{
    public interface IEmailSender
    {
        Task SendEmailBirthday(string emailTitle, string sendFrom, string subject, string text, string sendFrom2);
        Task SendEmailFreezer(string emailTitle, string sendFrom, string subject, string text, string sendFrom2);
    }
}
