// Interfaces/ICustomEmailSender.cs

namespace HostelManagementSystem.Interfaces;

public interface ICustomEmailSender
{
    Task SendEmailAsync(
        string email,
        string subject,
        string htmlMessage);
}