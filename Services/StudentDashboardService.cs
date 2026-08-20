using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using HostelManagementSystem.ViewModels;

namespace HostelManagementSystem.Services;

public class StudentDashboardService : IStudentDashboardService
{
    private readonly IStudentDashboardRepository _repository;

    public StudentDashboardService(
        IStudentDashboardRepository repository)
    {
        _repository = repository;
    }

    // =========================================================
    // DASHBOARD
    // =========================================================

    public async Task<StudentDashboardDto?> GetDashboardAsync(
        string email)
    {
        var student =
            await _repository.GetStudentDashboardAsync(email);

        if (student == null)
            return null;

        return new StudentDashboardDto
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            Gender = student.Gender,
            Department = student.Department,
            ProfilePicture = student.ProfilePicture,

           HostelApplications =
                student.HostelApplications,

            RoomAllocation =
                student.RoomAllocation
        };
    }

    // =========================================================
    // PROFILE
    // =========================================================

    public async Task<EditProfileViewModel?> GetProfileAsync(
        string email)
    {
        var student =
            await _repository.GetStudentDashboardAsync(email);

        if (student == null)
            return null;

        var application = student.HostelApplications?
            .OrderByDescending(a => a.ApplicationDate)
            .FirstOrDefault();

        var payment = application?.Payments?
            .OrderByDescending(p => p.PaymentDate)
            .FirstOrDefault();

        return new EditProfileViewModel
        {
            Id = student.Id,

            Email = student.Email,
            FullName = student.FullName,
            PhoneNumber = student.PhoneNumber,
            Gender = student.Gender,
            Department = student.Department,
            ProfilePicture = student.ProfilePicture,

            ApplicationStatus =
                application?.Status,

            ApplicationDate =
                application?.ApplicationDate,

            HostelType =
                student.RoomAllocation?.HostelRoom?.HostelType
                ?? application?.HostelRoom?.HostelType,

            RoomNumber =
                student.RoomAllocation?.HostelRoom?.RoomNumber
                ?? application?.HostelRoom?.RoomNumber,

            HasAllocation =
                student.RoomAllocation?.IsActive ?? false,

            PaymentStatus =
                payment?.Status,

            PaymentSession =
                payment?.Session,

            HostelApplications =
                student.HostelApplications?
                    .Select(a => new HostelApplicationDto
                    {
                        Id = a.Id,
                        StudentId = a.StudentId,
                        HostelRoomId = a.HostelRoomId,
                        ApplicationDate = a.ApplicationDate,
                        Status = a.Status,

                        HostelRoom =
                            a.HostelRoom == null
                                ? null
                                : new HostelRoomDto
                                {
                                    Id = a.HostelRoom.Id,
                                    RoomNumber =
                                        a.HostelRoom.RoomNumber,
                                    HostelType =
                                        a.HostelRoom.HostelType,
                                    Capacity =
                                        a.HostelRoom.Capacity,
                                    AvailableSpace =
                                        a.HostelRoom.AvailableSpace,
                                    OccupiedSpace =
                                        a.HostelRoom.OccupiedSpace,
                                    IsAvailable =
                                        a.HostelRoom.IsAvailable,
                                    Price =
                                        a.HostelRoom.Price
                                },

                        Payments =
                            a.Payments?
                                .Select(p => new PaymentDto
                                {
                                    Id = p.Id,
                                    HostelApplicationId =
                                        p.HostelApplicationId,
                                    Amount = p.Amount,
                                    Session = p.Session,
                                    Status = p.Status,
                                    PaymentDate =
                                        p.PaymentDate,
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
                student.RoomAllocation == null
                    ? null
                    : new RoomAllocationDto
                    {
                        Id =
                            student.RoomAllocation.Id,

                        StudentId =
                            student.RoomAllocation.StudentId,

                        HostelRoomId =
                            student.RoomAllocation.HostelRoomId,

                        AllocationDate =
                            student.RoomAllocation.AllocationDate,

                        IsActive =
                            student.RoomAllocation.IsActive,

                        StudentName =
                            student.FullName,

                        StudentEmail =
                            student.Email,

                        RoomNumber =
                            student.RoomAllocation
                                .HostelRoom?.RoomNumber
                            ?? string.Empty,

                        HostelType =
                            student.RoomAllocation
                                .HostelRoom?.HostelType
                            ?? string.Empty,

                        HostelRoom =
                            student.RoomAllocation.HostelRoom == null
                                ? null
                                : new HostelRoomDto
                                {
                                    Id =
                                        student.RoomAllocation
                                            .HostelRoom.Id,

                                    RoomNumber =
                                        student.RoomAllocation
                                            .HostelRoom.RoomNumber,

                                    HostelType =
                                        student.RoomAllocation
                                            .HostelRoom.HostelType,

                                    Capacity =
                                        student.RoomAllocation
                                            .HostelRoom.Capacity,

                                    AvailableSpace =
                                        student.RoomAllocation
                                            .HostelRoom.AvailableSpace,

                                    OccupiedSpace =
                                        student.RoomAllocation
                                            .HostelRoom.OccupiedSpace,

                                    IsAvailable =
                                        student.RoomAllocation
                                            .HostelRoom.IsAvailable,

                                    Price =
                                        student.RoomAllocation
                                            .HostelRoom.Price
                                }
                    }
        };
    }

    // =========================================================
    // UPDATE PROFILE
    // =========================================================

    public async Task<bool> UpdateProfileAsync(
        string email,
        EditProfileViewModel model)
    {
        var student =
            await _repository.GetStudentDashboardAsync(email);

        if (student == null)
            return false;

        student.FullName = model.FullName;
        student.PhoneNumber = model.PhoneNumber;
        student.Gender = model.Gender;
        student.Department = model.Department;
        student.ProfilePicture = model.ProfilePicture;

        await _repository.UpdateStudentAsync(student);
        await _repository.SaveChangesAsync();

        return true;
    }
}