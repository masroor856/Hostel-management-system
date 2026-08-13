// Services/RoomAllocationService.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Services
{
    public class RoomAllocationService
        : IRoomAllocationService
    {
        private readonly IRoomAllocationRepository _repository;

        public RoomAllocationService(
            IRoomAllocationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RoomAllocationDto>>
            GetAllAsync()
        {
            var allocations =
                await _repository.GetAllAsync();

            return allocations.Select(Map);
        }

        public async Task<RoomAllocationDto?>
            GetByIdAsync(int id)
        {
            var allocation =
                await _repository.GetByIdAsync(id);

            return allocation == null
                ? null
                : Map(allocation);
        }

        public async Task<CreateRoomAllocationDto>
            GetCreateDataAsync()
        {
            return new CreateRoomAllocationDto();
        }

        public async Task<bool> CreateAsync(
            CreateRoomAllocationDto dto,
            List<string> errors)
        {
            var room =
                await _repository
                    .GetRoomByIdAsync(dto.HostelRoomId);

            if (room == null)
            {
                errors.Add("Room not found.");
                return false;
            }

            if (room.AvailableSpace <= 0)
            {
                errors.Add("Room is full.");
                return false;
            }

            var student =
                await _repository
                    .GetStudentByIdAsync(dto.StudentId);

            if (student == null)
            {
                errors.Add("Student not found.");
                return false;
            }

            var existing =
                await _repository
                    .GetActiveByStudentAsync(dto.StudentId);

            if (existing != null)
            {
                errors.Add(
                    "Student already has a room.");

                return false;
            }

            var application =
                await _repository
                    .GetAcceptedApplicationAsync(
                        dto.StudentId);

            if (application == null)
            {
                errors.Add(
                    "Student has not submitted an accepted hostel application.");

                return false;
            }

            var expectedHostel =
                student.Gender == "Male"
                    ? "Boys"
                    : "Girls";

            if (room.HostelType != expectedHostel)
            {
                errors.Add(
                    $"This student can only be allocated to the {expectedHostel} hostel.");

                return false;
            }

            var allocation = new RoomAllocation
            {
                StudentId = dto.StudentId,
                HostelRoomId = dto.HostelRoomId,
                AllocationDate = DateTime.Now,
                IsActive = true
            };

            room.OccupiedSpace++;

            room.AvailableSpace =
                room.Capacity -
                room.OccupiedSpace;

            room.IsAvailable =
                room.AvailableSpace > 0;

            await _repository.AddAsync(allocation);

            await _repository.UpdateAsync(
                allocation);

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<RoomAllocationDto?>
            GetEditAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(
            int id,
            CreateRoomAllocationDto dto,
            List<string> errors)
        {
            var allocation =
                await _repository.GetByIdAsync(id);

            if (allocation == null)
            {
                errors.Add("Allocation not found.");
                return false;
            }

            var room =
                await _repository
                    .GetRoomByIdAsync(dto.HostelRoomId);

            if (room == null)
            {
                errors.Add("Room not found.");
                return false;
            }

            var student =
                await _repository
                    .GetStudentByIdAsync(dto.StudentId);

            if (student == null)
            {
                errors.Add("Student not found.");
                return false;
            }

            var expectedHostel =
                student.Gender == "Male"
                    ? "Boys"
                    : "Girls";

            if (room.HostelType != expectedHostel)
            {
                errors.Add(
                    $"This student can only be allocated to the {expectedHostel} hostel.");

                return false;
            }

            if (allocation.HostelRoomId !=
                dto.HostelRoomId)
            {
                if (room.AvailableSpace <= 0)
                {
                    errors.Add("Room is full.");
                    return false;
                }

                var oldRoom =
                    await _repository
                        .GetRoomByIdAsync(
                            allocation.HostelRoomId);

                if (oldRoom != null)
                {
                    oldRoom.OccupiedSpace =
                        Math.Max(
                            0,
                            oldRoom.OccupiedSpace - 1);

                    oldRoom.AvailableSpace =
                        oldRoom.Capacity -
                        oldRoom.OccupiedSpace;

                    oldRoom.IsAvailable =
                        oldRoom.AvailableSpace > 0;
                }

                room.OccupiedSpace++;

                room.AvailableSpace =
                    room.Capacity -
                    room.OccupiedSpace;

                room.IsAvailable =
                    room.AvailableSpace > 0;
            }

            allocation.StudentId =
                dto.StudentId;

            allocation.HostelRoomId =
                dto.HostelRoomId;

            await _repository.UpdateAsync(
                allocation);

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<RoomAllocationDto?>
            GetDeleteAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var allocation =
                await _repository.GetByIdAsync(id);

            if (allocation == null)
                return false;

            var room =
                await _repository
                    .GetRoomByIdAsync(
                        allocation.HostelRoomId);

            if (room != null &&
                allocation.IsActive)
            {
                room.OccupiedSpace =
                    Math.Max(
                        0,
                        room.OccupiedSpace - 1);

                room.AvailableSpace =
                    room.Capacity -
                    room.OccupiedSpace;

                room.IsAvailable =
                    room.AvailableSpace > 0;
            }

            await _repository.DeleteAsync(
                allocation);

            await _repository.SaveChangesAsync();

            return true;
        }

        private static RoomAllocationDto Map(
            RoomAllocation allocation)
        {
            return new RoomAllocationDto
            {
                Id = allocation.Id,

                StudentId =
                    allocation.StudentId,

                HostelRoomId =
                    allocation.HostelRoomId,

                AllocationDate =
                    allocation.AllocationDate,

                IsActive =
                    allocation.IsActive,

                StudentName =
                    allocation.Student?.FullName
                    ?? "",

                StudentEmail =
                    allocation.Student?.Email
                    ?? "",

                RoomNumber =
                    allocation.HostelRoom?.RoomNumber
                    ?? "",

                HostelType =
                    allocation.HostelRoom?.HostelType
                    ?? ""
            };
        }
    }
}