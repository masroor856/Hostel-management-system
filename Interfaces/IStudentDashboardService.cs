// Interfaces/IStudentDashboardService.cs

using HostelManagementSystem.DTOs;

namespace HostelManagementSystem.Interfaces;

public interface IStudentDashboardService
{
    Task<StudentDashboardDto?> GetDashboardAsync(string email);
}