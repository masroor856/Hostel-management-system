// Interfaces/IAdminProfileService.cs

using HostelManagementSystem.DTOs;

namespace HostelManagementSystem.Interfaces
{
    public interface IAdminProfileService
    {
        Task<AdminProfileDto?> GetProfileAsync(
            string identityUserId,
            string email);

        Task<AdminProfileDto?> GetEditProfileAsync(
            string identityUserId,
            string email);

        Task<(bool Success, IEnumerable<string> Errors)>
            UpdateProfileAsync(
                string identityUserId,
                AdminProfileDto model);
    }
}