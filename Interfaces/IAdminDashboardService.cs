using HostelManagementSystem.ViewModels;

namespace HostelManagementSystem.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardAsync();
    }
}