using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
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

    public async Task<StudentDashboardDto?> GetDashboardAsync(string email)
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
            HostelApplications = student.HostelApplications,
            RoomAllocation = student.RoomAllocation
        };
    }

    public async Task<EditProfileViewModel?> GetProfileAsync(string email)
    {
        var student =
            await _repository.GetStudentDashboardAsync(email);

        if (student == null)
            return null;

        return new EditProfileViewModel
        {
            Email = student.Email,
            FullName = student.FullName,
            PhoneNumber = student.PhoneNumber,
            Gender = student.Gender,
            Department = student.Department,
            ProfilePicture = student.ProfilePicture
        };
    }

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