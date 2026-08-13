// Interfaces/IReportsRepository.cs

using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IReportsRepository
    {
        Task<int> GetTotalStudentsAsync();

        Task<int> GetTotalRoomsAsync();

        Task<int> GetTotalApplicationsAsync();

        Task<int> GetTotalPaymentsAsync();

        Task<decimal> GetTotalRevenueAsync();

        Task<int> GetAvailableRoomsAsync();

        Task<int> GetOccupiedRoomsAsync();

        Task<IEnumerable<Payment>> GetRecentPaymentsAsync(
            int count);

        Task<IEnumerable<HostelApplication>>
            GetRecentApplicationsAsync(int count);
    }
}