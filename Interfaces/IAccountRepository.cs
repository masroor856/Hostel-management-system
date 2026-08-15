// Interfaces/IAccountRepository.cs

using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HostelManagementSystem.Interfaces;

public interface IAccountRepository
{
    Task<IdentityUser?> GetUserByEmailAsync(string email);

    Task<IdentityUser?> GetUserByIdAsync(string id);

    Task<Student?> GetStudentByEmailAsync(string email);

    Task AddStudentAsync(Student student);
     Task DeleteStudentAsync(int id);
    Task SaveChangesAsync();
}