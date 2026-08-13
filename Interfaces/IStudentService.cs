// Interfaces/IStudentService.cs

using HostelManagementSystem.DTOs;

namespace HostelManagementSystem.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllAsync();

        Task<IEnumerable<StudentDto>> SearchAsync(
            string? search);

        Task<StudentDto?> GetByIdAsync(int id);

        Task CreateAsync(
            CreateStudentDto dto);

        Task<bool> UpdateAsync(
            UpdateStudentDto dto);

        Task<bool> DeleteAsync(int id);
    }
}