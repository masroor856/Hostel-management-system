using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace HostelManagementSystem.Implementation;

public class SmtpEmailSender : ICustomEmailSender
{
    private readonly EmailSettings _settings;

    public SmtpEmailSender(
        IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

   public async Task SendEmailAsync(
    string email,
    string subject,
    string htmlMessage)
{
    using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
{
    EnableSsl = true,
    UseDefaultCredentials = false,
    Credentials = new NetworkCredential(
        _settings.SenderEmail,
        _settings.SenderPassword),
    DeliveryMethod = SmtpDeliveryMethod.Network,
    Timeout = 30000
};

    using var mail = new MailMessage
    {
        From = new MailAddress(
            _settings.SenderEmail,
            _settings.SenderName),

        Subject = subject,
        Body = htmlMessage,
        IsBodyHtml = true
    };
mail.To.Add(email);

try
{
    await client.SendMailAsync(mail);
}
catch (SmtpException ex)
{
    throw new Exception(
        $"SMTP Error: {ex.StatusCode}\n{ex.Message}\n{ex.InnerException?.Message}",
        ex);
}
}
}