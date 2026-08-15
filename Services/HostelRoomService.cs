// Services/HostelRoomService.cs
using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Services
{
    public class HostelRoomService : IHostelRoomService
    {
        private readonly IHostelRoomRepository _repository;

        public HostelRoomService(IHostelRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<HostelRoom>> GetAllRooms()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<HostelRoom?> GetRoomById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<HostelRoom>> SearchRooms(
            string? search,
            string? status)
        {
            return await _repository.SearchAsync(search, status);
        }

        public async Task CreateRoom(CreateHostelRoomDto dto)
        {
            var room = new HostelRoom
            {
                RoomNumber = dto.RoomNumber,
                HostelType = dto.HostelType,
                Capacity = dto.Capacity,
                Price = dto.Price,
                OccupiedSpace = 0,
                AvailableSpace = dto.Capacity,
                IsAvailable = dto.Capacity > 0
            };

            await _repository.AddAsync(room);
        }

        public async Task UpdateRoom(UpdateHostelRoomDto dto)
        {
            var room = await _repository.GetByIdAsync(dto.Id);

            if (room == null)
                return;

            room.RoomNumber = dto.RoomNumber;
            room.HostelType = dto.HostelType;
            room.Capacity = dto.Capacity;
            room.Price = dto.Price;

            if (room.OccupiedSpace > room.Capacity)
                room.OccupiedSpace = room.Capacity;

            room.AvailableSpace =
                room.Capacity - room.OccupiedSpace;

            room.IsAvailable =
                room.AvailableSpace > 0;

            await _repository.UpdateAsync(room);
        }

        public async Task DeleteRoom(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}