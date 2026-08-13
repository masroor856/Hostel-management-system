// Interfaces/IStudentRepository.cs

using HostelManagementSystem.Models;

namespace HostelManagementSystem.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync(
            string? search = null);

        Task<Student?> GetByIdAsync(int id);

        Task<Student?> GetByEmailAsync(
            string email);

        Task AddAsync(Student student);

        Task UpdateAsync(Student student);

        Task DeleteAsync(int id);

        Task SaveChangesAsync();
    }
}