// Repositories/StudentDashboardRepository.cs

using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories;

public class StudentDashboardRepository
    : IStudentDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public StudentDashboardRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetStudentDashboardAsync(
        string email)
    {
        return await _context.Students
            .Include(s => s.HostelApplications)
                .ThenInclude(a => a.HostelRoom)
            .Include(s => s.HostelApplications)
                .ThenInclude(a => a.Payments)
            .Include(s => s.RoomAllocation)
                .ThenInclude(r => r.HostelRoom)
            .FirstOrDefaultAsync(s => s.Email == email);
    }
}