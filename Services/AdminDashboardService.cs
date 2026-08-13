using HostelManagementSystem.Interfaces;
using HostelManagementSystem.ViewModels;

namespace HostelManagementSystem.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _repository;

        public AdminDashboardService(
            IAdminDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdminDashboardViewModel> GetDashboardAsync()
        {
            return await _repository.GetDashboardDataAsync();
        }
    }
}