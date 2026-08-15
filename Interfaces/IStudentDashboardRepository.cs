using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces;

public interface IStudentDashboardRepository
{
    Task<Student?> GetStudentDashboardAsync(string email);

    Task UpdateStudentAsync(Student student);

    Task SaveChangesAsync();
}