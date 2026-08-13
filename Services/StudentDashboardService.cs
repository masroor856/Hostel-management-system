// Services/StudentDashboardService.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;

namespace HostelManagementSystem.Services;

public class StudentDashboardService : IStudentDashboardService
{
    private readonly IStudentDashboardRepository _repository;

    public StudentDashboardService(
        IStudentDashboardRepository repository)
    {
        _repository = repository;
    }

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
}