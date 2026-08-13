using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountRepository(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IdentityUser?> GetUserByEmailAsync(
        string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityUser?> GetUserByIdAsync(
        string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<Student?> GetStudentByEmailAsync(
        string email)
    {
        return await _context.Students
            .Include(s => s.HostelApplications)
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task AddStudentAsync(Student student)
    {
        await _context.Students.AddAsync(student);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}