// Interfaces/IPaymentRepository.cs

using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync(string? search);

        Task<IEnumerable<Payment>> GetStudentPaymentsAsync(string email);

        Task<Payment?> GetByIdAsync(int id);

        Task<Payment?> GetByReferenceAsync(string reference);

        Task<Payment?> GetPaidSessionAsync(
            int applicationId,
            string session);

        Task<Payment?> GetPendingSessionAsync(
            int applicationId,
            string session);

        Task<HostelApplication?> GetApprovedApplicationAsync(
            string email);
        Task<HostelApplication?> GetApplicationByIdAsync(
             int id);
        Task<IEnumerable<Payment>> GetExpiredPendingPaymentsAsync();

        Task<IEnumerable<Payment>> GetPaymentsByApplicationIdAsync(
            int applicationId);

        Task<RoomAllocation?> GetActiveAllocationAsync(
            int studentId);

        Task AddAsync(Payment payment);

        Task UpdateAsync(Payment payment);

        Task DeleteAsync(int id);

        Task AddAllocationAsync(RoomAllocation allocation);

        Task SaveChangesAsync();
    }
}