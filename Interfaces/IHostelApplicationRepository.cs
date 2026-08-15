using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IHostelApplicationRepository
    {
        Task<IEnumerable<HostelApplication>> SearchAsync(
            string? search,
            string? status);

        Task<HostelApplication?> GetByIdAsync(int id);

        Task AddAsync(HostelApplication application);

        Task UpdateAsync(HostelApplication application);

        Task DeleteAsync(HostelApplication application);

        Task SaveChangesAsync();

        Task<Student?> GetStudentByEmailAsync(string email);

        Task<Student?> GetStudentByIdAsync(int id);

        Task<bool> HasExistingApplicationAsync(int studentId);

        Task<IEnumerable<HostelRoom>> GetAvailableRoomsAsync();

        Task<IEnumerable<HostelRoom>> GetRoomsByHostelTypeAsync(
            string hostelType);

        Task<IEnumerable<HostelRoom>> GetRoomsForStudentAsync(
            int studentId);

            Task<IEnumerable<HostelRoom>> GetAvailableRoomsForStudentAsync(string email);
    }
}