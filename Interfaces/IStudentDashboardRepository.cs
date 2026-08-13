// Interfaces/IStudentDashboardRepository.cs

using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces;

public interface IStudentDashboardRepository
{
    Task<Student?> GetStudentDashboardAsync(string email);
}