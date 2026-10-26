using System.Net.Http.Json;
using HostelManagementSystem.Interfaces;

namespace HostelManagementSystem.Implementation;

public class BrevoEmailSender : ICustomEmailSender
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public BrevoEmailSender(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string email,
        string subject,
        string htmlMessage)
    {
        var apiKey = _configuration["BREVO_API_KEY"];
        var senderEmail = _configuration["BREVO_SENDER_EMAIL"];
        var senderName = _configuration["BREVO_SENDER_NAME"]
                         ?? "Hostel Management System";

        if (string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(senderEmail))
        {
            throw new InvalidOperationException(
                "Brevo API key or sender email is not configured.");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.brevo.com/v3/smtp/email");

        request.Headers.Add("api-key", apiKey);

        request.Content = JsonContent.Create(new
        {
            sender = new
            {
                name = senderName,
                email = senderEmail
            },
            to = new[]
            {
                new { email }
            },
            subject,
            htmlContent = htmlMessage
        });

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                $"Brevo email request failed with HTTP " +
                $"{(int)response.StatusCode}: {details}");
        }
    }
}