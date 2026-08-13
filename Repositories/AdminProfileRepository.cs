// Repositories/AdminProfileRepository.cs

using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class AdminProfileRepository
        : IAdminProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminProfileRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminProfile?>
            GetByIdentityUserIdAsync(
                string identityUserId)
        {
            return await _context.AdminProfiles
                .FirstOrDefaultAsync(
                    x => x.IdentityUserId == identityUserId);
        }

        public async Task UpdateAsync(
            AdminProfile profile)
        {
            _context.AdminProfiles.Update(profile);

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}