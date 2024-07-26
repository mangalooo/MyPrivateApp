using System.Threading.Tasks;

namespace MagnusPrivateApp.Services.EmailServices
{
    public interface IEmailSender
    {
        void SendEmailBirthday(string emailTitle, string sendFrom, string subject, string text, string sendFrom2);
        void SendEmailFreezer(string emailTitle, string sendFrom, string subject, string text, string sendFrom2);
        Task SendEmailAsync(string email, string v1, string v2);
    }
}
