// Repositories/StudentRepository.cs

using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync(
            string? search = null)
        {
            IQueryable<Student> query =
                _context.Students
                    .Include(s => s.HostelApplications)
                        .ThenInclude(a => a.HostelRoom)
                    .Include(s => s.RoomAllocation)
                        .ThenInclude(r => r.HostelRoom);

           if (!string.IsNullOrWhiteSpace(search))
{
    search = search.Trim();

    query = query.Where(s =>
        EF.Functions.Like(s.FullName, $"%{search}%") ||
        EF.Functions.Like(s.Email, $"%{search}%") ||
        EF.Functions.Like(s.Department, $"%{search}%"));
}

            return await query
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

      public async Task<Student?> GetByIdAsync(int id)
{
    return await _context.Students
        .Include(s => s.HostelApplications)
            .ThenInclude(a => a.HostelRoom)

        .Include(s => s.HostelApplications)
            .ThenInclude(a => a.Payments)

        .Include(s => s.RoomAllocation)
            .ThenInclude(r => r.HostelRoom)

        .FirstOrDefaultAsync(s => s.Id == id);
}
        public async Task<Student?> GetByEmailAsync(
            string email)
        {
            return await _context.Students
                .Include(s => s.HostelApplications)
                    .ThenInclude(a => a.HostelRoom)
                .Include(s => s.RoomAllocation)
                    .ThenInclude(r => r.HostelRoom)
                .FirstOrDefaultAsync(
                    s => s.Email == email);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }

        public Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var student =
                await _context.Students
                    .FirstOrDefaultAsync(s => s.Id == id);

            if (student != null)
            {
                _context.Students.Remove(student);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}