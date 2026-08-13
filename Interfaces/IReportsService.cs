// Interfaces/IReportsService.cs

using HostelManagementSystem.DTOs;

namespace HostelManagementSystem.Interfaces
{
    public interface IReportsService
    {
        Task<ReportsDto> GetReportsAsync();
    }
}