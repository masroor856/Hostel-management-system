using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IPaystackService
    {
        Task<string?> InitializePayment(Payment payment, string email);
        Task<bool> VerifyPayment(string reference);
    }
}