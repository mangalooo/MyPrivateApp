

namespace MyPrivateApp.Components.Email.Classes
{
    public interface IEmailSender
    {
        void SendEmailBirthday(string emailTitle, string sendFrom, string subject, string text, string sendFrom2);
        void SendEmailFreezer(string emailTitle, string sendFrom, string subject, string text, string sendFrom2);
    }
}
