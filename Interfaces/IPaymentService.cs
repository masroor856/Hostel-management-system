using HostelManagementSystem.DTOs;
using HostelManagementSystem.Models;
namespace HostelManagementSystem.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetPayments(
            string? search);

        Task<IEnumerable<PaymentDto>> GetMyPayments(
            string email);

        Task<PaymentDto?> GetPayment(int id);

        Task<PaymentDto?> GetPaymentByReference(
            string reference);

        Task<IEnumerable<string>> GetPaidSessions(
            int applicationId);

        Task<int?> GetApprovedApplicationId(
            string email);
            Task<Payment> GetPaymentForCreation(int applicationId);

        Task<string> CreatePayment(
    CreatePaymentDto dto,
    string email);

        Task<bool> VerifyPayment(
            string reference);

        Task UpdatePayment(
            UpdatePaymentDto dto);

        Task DeletePayment(int id);

        Task UpdateExpiredPayments();
    }
}