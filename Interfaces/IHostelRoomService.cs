// Interfaces/IHostelRoomService.cs
using HostelManagementSystem.DTOs;
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IHostelRoomService
    {
        Task<IEnumerable<HostelRoom>> GetAllRooms();

        Task<IEnumerable<HostelRoom>> SearchRooms(
            string? search,
            string? status);

        Task<HostelRoom?> GetRoomById(int id);

        Task CreateRoom(CreateHostelRoomDto dto);

        Task UpdateRoom(UpdateHostelRoomDto dto);

        Task DeleteRoom(int id);
    }
}