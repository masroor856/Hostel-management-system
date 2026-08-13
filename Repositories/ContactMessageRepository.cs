// Repositories/ContactMessageRepository.cs

using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class ContactMessageRepository
        : IContactMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactMessageRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactMessage>>
            GetAllAsync(string? search)
        {
            var query =
                _context.ContactMessages
                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.FullName.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.Message.Contains(search));
            }

            return await query
                .OrderByDescending(x => x.DateSent)
                .ToListAsync();
        }

        public async Task<ContactMessage?>
            GetByIdAsync(int id)
        {
            return await _context.ContactMessages
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(
            ContactMessage message)
        {
            await _context.ContactMessages
                .AddAsync(message);
        }

        public async Task DeleteAsync(int id)
        {
            var message =
                await _context.ContactMessages
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (message != null)
            {
                _context.ContactMessages.Remove(message);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}