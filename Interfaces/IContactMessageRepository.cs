// Interfaces/IContactMessageRepository.cs

using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IContactMessageRepository
    {
        Task<IEnumerable<ContactMessage>> GetAllAsync(
            string? search);

        Task<ContactMessage?> GetByIdAsync(int id);

        Task AddAsync(ContactMessage message);

        Task DeleteAsync(int id);

        Task SaveChangesAsync();
    }
}