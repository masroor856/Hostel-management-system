// Interfaces/IRoomAllocationService.cs

using HostelManagementSystem.DTOs;

namespace HostelManagementSystem.Interfaces
{
    public interface IRoomAllocationService
    {
        Task<IEnumerable<RoomAllocationDto>> GetAllAsync();

        Task<RoomAllocationDto?> GetByIdAsync(int id);

        Task<CreateRoomAllocationDto>
            GetCreateDataAsync();

        Task<bool> CreateAsync(
            CreateRoomAllocationDto dto,
            List<string> errors);

        Task<RoomAllocationDto?> GetEditAsync(int id);

        Task<bool> UpdateAsync(
            int id,
            CreateRoomAllocationDto dto,
            List<string> errors);

        Task<RoomAllocationDto?> GetDeleteAsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}