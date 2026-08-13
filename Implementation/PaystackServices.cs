using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using HostelManagementSystem.Settings;
using Microsoft.Extensions.Options;

namespace HostelManagementSystem.Implementation
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly PaystackSettings _settings;

        public PaystackService(
            HttpClient httpClient,
            IOptions<PaystackSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.SecretKey);
        }

        public async Task<string?> InitializePayment(Payment payment, string email)
        {
            var request = new
            {
                email = email,
                amount = (int)(payment.Amount * 100), // Paystack expects Kobo
                reference = payment.TransactionReference,
               callback_url = "http://localhost:5236/Payment/Verify"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.paystack.co/transaction/initialize",
                content);

          if (!response.IsSuccessStatusCode)
{
    var error = await response.Content.ReadAsStringAsync();
    throw new Exception(error);
}

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .GetProperty("data")
                .GetProperty("authorization_url")
                .GetString();
        }

        public async Task<bool> VerifyPayment(string reference)
        {
            var response = await _httpClient.GetAsync(
                $"https://api.paystack.co/transaction/verify/{reference}");

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var status = document.RootElement
                .GetProperty("data")
                .GetProperty("status")
                .GetString();

            return status == "success";
        }
    }
}