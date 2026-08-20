// Services/StudentService.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(
            IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            var students =
                await _repository.GetAllAsync();

            return students.Select(MapToDto);
        }

        public async Task<IEnumerable<StudentDto>> SearchAsync(
            string? search)
        {
            var students =
                await _repository.GetAllAsync(search);

            return students.Select(MapToDto);
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var student =
                await _repository.GetByIdAsync(id);

            if (student == null)
                return null;

            return MapToDto(student);
        }

        public async Task CreateAsync(
            CreateStudentDto dto)
        {
            var existing =
                await _repository.GetByEmailAsync(dto.Email);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    "A student with this email already exists.");
            }

            var student = new Student
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Gender = dto.Gender,
                Department = dto.Department
            };

            await _repository.AddAsync(student);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(
            UpdateStudentDto dto)
        {
            var student =
                await _repository.GetByIdAsync(dto.Id);

            if (student == null)
                return false;

            var existing =
                await _repository.GetByEmailAsync(dto.Email);

            if (existing != null &&
                existing.Id != dto.Id)
            {
                throw new InvalidOperationException(
                    "A student with this email already exists.");
            }

            student.FullName = dto.FullName;
            student.Email = dto.Email;
            student.PhoneNumber = dto.PhoneNumber;
            student.Gender = dto.Gender;
            student.Department = dto.Department;

            await _repository.UpdateAsync(student);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student =
                await _repository.GetByIdAsync(id);

            if (student == null)
                return false;

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();

            return true;
        }

       private static StudentDto MapToDto(Student student)
{
    var application = student.HostelApplications?
        .OrderByDescending(a => a.ApplicationDate)
        .FirstOrDefault();

    var allocation = student.RoomAllocation;

    return new StudentDto
    {
        Id = student.Id,
        FullName = student.FullName,
        Email = student.Email,
        PhoneNumber = student.PhoneNumber,
        Gender = student.Gender,
        Department = student.Department,
        ProfilePicture = student.ProfilePicture,

        HostelType =
            allocation?.HostelRoom?.HostelType
            ?? application?.HostelRoom?.HostelType,

        RoomNumber =
            allocation?.HostelRoom?.RoomNumber
            ?? application?.HostelRoom?.RoomNumber,

        ApplicationStatus =
            application?.Status,

        HasAllocation =
            allocation != null &&
            allocation.IsActive,

        HostelApplications =
            student.HostelApplications?
                .OrderByDescending(a => a.ApplicationDate)
                .Select(a => new HostelApplicationDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    HostelRoomId = a.HostelRoomId,
                    ApplicationDate = a.ApplicationDate,
                    Status = a.Status,

                    HostelRoom = a.HostelRoom == null
                        ? null
                        : new HostelRoomDto
                        {
                            Id = a.HostelRoom.Id,
                            RoomNumber = a.HostelRoom.RoomNumber,
                            HostelType = a.HostelRoom.HostelType,
                            Capacity = a.HostelRoom.Capacity,
                            AvailableSpace = a.HostelRoom.AvailableSpace,
                            OccupiedSpace = a.HostelRoom.OccupiedSpace,
                            IsAvailable = a.HostelRoom.IsAvailable,
                            Price = a.HostelRoom.Price
                        },

                    Payments = a.Payments?
                        .OrderByDescending(p => p.PaymentDate)
                        .Select(p => new PaymentDto
                        {
                            Id = p.Id,
                            HostelApplicationId =
                                p.HostelApplicationId,
                            Amount = p.Amount,
                            Session = p.Session,
                            Status = p.Status,
                            PaymentDate = p.PaymentDate,
                            TransactionReference =
                                p.TransactionReference,

                            StudentName =
                                student.FullName,

                            StudentEmail =
                                student.Email,

                            RoomNumber =
                                a.HostelRoom?.RoomNumber
                                ?? string.Empty
                        })
                        .ToList()
                        ?? new List<PaymentDto>()
                })
                .ToList()
            ?? new List<HostelApplicationDto>(),

        RoomAllocation =
            allocation == null
                ? null
                : new RoomAllocationDto
                {
                    Id = allocation.Id,
                    StudentId = allocation.StudentId,
                    HostelRoomId = allocation.HostelRoomId,
                    AllocationDate = allocation.AllocationDate,
                    IsActive = allocation.IsActive,

                    StudentName = student.FullName,
                    StudentEmail = student.Email,

                    RoomNumber =
                        allocation.HostelRoom?.RoomNumber
                        ?? string.Empty,

                    HostelType =
                        allocation.HostelRoom?.HostelType
                        ?? string.Empty,

                    HostelRoom =
                        allocation.HostelRoom == null
                            ? null
                            : new HostelRoomDto
                            {
                                Id = allocation.HostelRoom.Id,
                                RoomNumber =
                                    allocation.HostelRoom.RoomNumber,
                                HostelType =
                                    allocation.HostelRoom.HostelType,
                                Capacity =
                                    allocation.HostelRoom.Capacity,
                                AvailableSpace =
                                    allocation.HostelRoom.AvailableSpace,
                                OccupiedSpace =
                                    allocation.HostelRoom.OccupiedSpace,
                                IsAvailable =
                                    allocation.HostelRoom.IsAvailable,
                                Price =
                                    allocation.HostelRoom.Price
                            }
                }
    };
}
    }
}