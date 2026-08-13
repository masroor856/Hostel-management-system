// Interfaces/IHostelRoomRepository.cs
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IHostelRoomRepository
    {
        Task<IEnumerable<HostelRoom>> GetAllAsync();

        Task<HostelRoom?> GetByIdAsync(int id);

        Task<IEnumerable<HostelRoom>> SearchAsync(
            string? search,
            string? status);

        Task AddAsync(HostelRoom room);

        Task UpdateAsync(HostelRoom room);

        Task DeleteAsync(int id);
    }
}