using HostelManagementSystem.DTOs;
using HostelManagementSystem.ViewModels;

namespace HostelManagementSystem.Interfaces;

public interface IStudentDashboardService
{
    Task<StudentDashboardDto?> GetDashboardAsync(string email);

    Task<EditProfileViewModel?> GetProfileAsync(string email);

    Task<bool> UpdateProfileAsync(
        string email,
        EditProfileViewModel model);
}