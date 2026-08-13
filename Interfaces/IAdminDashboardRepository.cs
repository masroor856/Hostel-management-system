using HostelManagementSystem.ViewModels;

namespace HostelManagementSystem.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<AdminDashboardViewModel> GetDashboardDataAsync();
    }
}