// Services/HostelApplicationService.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Services
{
    public class HostelApplicationService : IHostelApplicationService
    {
        private readonly IHostelApplicationRepository _repository;

        public HostelApplicationService(
            IHostelApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<HostelApplicationDto>>
            SearchAsync(
                string? search,
                string? status)
        {
            var applications =
                await _repository.SearchAsync(
                    search,
                    status);

            return applications
                .Select(MapToDto)
                .ToList();
        }

        public async Task<HostelApplicationDto?>
            GetByIdAsync(int id)
        {
            var application =
                await _repository.GetByIdAsync(id);

            return application == null
                ? null
                : MapToDto(application);
        }

        public async Task<bool>
            CreateAsync(
                HostelApplicationDto dto)
        {
            if (!dto.HostelRoomId.HasValue)
                return false;

            var application =
                new HostelApplication
                {
                    StudentId = dto.StudentId,

                    HostelRoomId =
                        dto.HostelRoomId.Value,

                    ApplicationDate =
                        dto.ApplicationDate == default
                            ? DateTime.UtcNow
                            : dto.ApplicationDate,

                    Status =
                        string.IsNullOrWhiteSpace(dto.Status)
                            ? "Pending"
                            : dto.Status
                };

            await _repository.AddAsync(application);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool>
            UpdateAsync(
                HostelApplicationDto dto)
        {
            var application =
                await _repository.GetByIdAsync(dto.Id);

            if (application == null)
                return false;

            if (!dto.HostelRoomId.HasValue)
                return false;

            application.StudentId =
                dto.StudentId;

            application.HostelRoomId =
                dto.HostelRoomId.Value;

            application.ApplicationDate =
                dto.ApplicationDate;

            application.Status =
                string.IsNullOrWhiteSpace(dto.Status)
                    ? "Pending"
                    : dto.Status;

            await _repository.UpdateAsync(application);
            await _repository.SaveChangesAsync();

            return true;
        }

       public async Task<bool> DeleteAsync(int id)
{
    var application = await _repository.GetByIdAsync(id);

    if (application == null)
        return false;

    await _repository.DeleteAsync(application);
    await _repository.SaveChangesAsync();

    return true;
}
        public async Task<bool>
            UpdateStatusAsync(
                int id,
                string status)
        {
            var application =
                await _repository.GetByIdAsync(id);

            if (application == null)
                return false;

            if (string.IsNullOrWhiteSpace(status))
                return false;

            application.Status = status;

            await _repository.UpdateAsync(application);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<StudentDto?>
            GetStudentByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

           var student =
    await _repository.GetStudentByEmailAsync(email);

            if (student == null)
                return null;

            return new StudentDto
            {
                Id = student.Id,

                FullName =
                    student.FullName ?? string.Empty,

                Email =
                    student.Email ?? string.Empty,

                PhoneNumber =
                    student.PhoneNumber ?? string.Empty,

                Gender =
                    student.Gender ?? string.Empty,

                Department =
                    student.Department ?? string.Empty,

                ProfilePicture =
                    student.ProfilePicture
            };
        }

        public async Task<bool>
            HasExistingApplicationAsync(
                int studentId)
        {
            return await _repository
                .HasExistingApplicationAsync(
                    studentId);
        }

        public async Task<IEnumerable<HostelRoomDto>>
            GetAvailableRoomsAsync()
        {
            var rooms =
                await _repository
                    .GetAvailableRoomsAsync();

            return rooms
                .Select(MapRoom)
                .ToList();
        }

        public async Task<IEnumerable<HostelRoomDto>>
            GetRoomsForStudentAsync(
                int studentId)
        {
            var rooms =
                await _repository
                    .GetRoomsForStudentAsync(
                        studentId);

            return rooms
                .Select(MapRoom)
                .ToList();
        }

        private static HostelApplicationDto
            MapToDto(
                HostelApplication application)
        {
            return new HostelApplicationDto
            {
                Id =
                    application.Id,

                StudentId =
                    application.StudentId,

                HostelRoomId =
                    application.HostelRoomId,

                ApplicationDate =
                    application.ApplicationDate,

                Status =
                    application.Status ?? string.Empty,

                Student =
                    application.Student == null
                        ? null
                        : new StudentDto
                        {
                            Id =
                                application.Student.Id,

                            FullName =
                                application.Student.FullName
                                ?? string.Empty,

                            Email =
                                application.Student.Email
                                ?? string.Empty,

                            PhoneNumber =
                                application.Student.PhoneNumber
                                ?? string.Empty,

                            Gender =
                                application.Student.Gender
                                ?? string.Empty,

                            Department =
                                application.Student.Department
                                ?? string.Empty,

                            ProfilePicture =
                                application.Student.ProfilePicture
                        },

                HostelRoom =
                    application.HostelRoom == null
                        ? null
                        : MapRoom(
                            application.HostelRoom)
            };
        }

        private static HostelRoomDto
            MapRoom(
                HostelRoom room)
        {
            return new HostelRoomDto
            {
                Id =
                    room.Id,

                RoomNumber =
                    room.RoomNumber
                    ?? string.Empty,

                HostelType =
                    room.HostelType
                    ?? string.Empty,

                Capacity =
                    room.Capacity,

                AvailableSpace =
                    room.AvailableSpace,

                OccupiedSpace =
                    room.OccupiedSpace,

                IsAvailable =
                    room.IsAvailable,

                Price =
                    room.Price
            };
        }
    }
}