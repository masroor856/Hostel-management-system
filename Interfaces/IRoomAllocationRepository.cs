// Interfaces/IRoomAllocationRepository.cs

using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IRoomAllocationRepository
    {
        Task<IEnumerable<RoomAllocation>> GetAllAsync();

        Task<RoomAllocation?> GetByIdAsync(int id);

        Task<RoomAllocation?> GetActiveByStudentAsync(int studentId);

        Task<Student?> GetStudentByIdAsync(int studentId);

        Task<HostelRoom?> GetRoomByIdAsync(int roomId);

        Task<HostelApplication?> GetAcceptedApplicationAsync(
            int studentId);

        Task<IEnumerable<Student>> GetStudentsAsync();

        Task<IEnumerable<HostelRoom>> GetAvailableRoomsAsync();

        Task<IEnumerable<HostelRoom>> GetAllRoomsAsync();

        Task AddAsync(RoomAllocation allocation);

        Task UpdateAsync(RoomAllocation allocation);

        Task DeleteAsync(RoomAllocation allocation);

        Task SaveChangesAsync();
    }
}