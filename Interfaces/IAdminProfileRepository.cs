// Interfaces/IAdminProfileRepository.cs

using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IAdminProfileRepository
    {
        Task<AdminProfile?> GetByIdentityUserIdAsync(
            string identityUserId);

        Task UpdateAsync(AdminProfile profile);

        Task SaveChangesAsync();
    }
}