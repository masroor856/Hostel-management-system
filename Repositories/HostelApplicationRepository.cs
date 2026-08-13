using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class HostelApplicationRepository
        : IHostelApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public HostelApplicationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HostelApplication>> SearchAsync(
            string? search,
            string? status)
        {
            var query = _context.HostelApplications
                .Include(a => a.Student)
                .Include(a => a.HostelRoom)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(a =>
                    a.Student.FullName.Contains(search) ||
                    a.Student.Email.Contains(search) ||
                    a.HostelRoom.RoomNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim();

                query = query.Where(a =>
                    a.Status == status);
            }

            return await query
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<HostelApplication?> GetByIdAsync(int id)
        {
            return await _context.HostelApplications
                .Include(a => a.Student)
                .Include(a => a.HostelRoom)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(
            HostelApplication application)
        {
            await _context.HostelApplications.AddAsync(application);
        }

        public Task UpdateAsync(
            HostelApplication application)
        {
            _context.HostelApplications.Update(application);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(
            HostelApplication application)
        {
            _context.HostelApplications.Remove(application);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Student?> GetStudentByEmailAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Students
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> HasExistingApplicationAsync(
            int studentId)
        {
            return await _context.HostelApplications
                .AnyAsync(a => a.StudentId == studentId);
        }

        public async Task<IEnumerable<HostelRoom>>
            GetAvailableRoomsAsync()
        {
            return await _context.HostelRooms
                .Where(r =>
                    r.IsAvailable &&
                    r.AvailableSpace > 0)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostelRoom>>
            GetRoomsByHostelTypeAsync(
                string hostelType)
        {
            if (string.IsNullOrWhiteSpace(hostelType))
                return Enumerable.Empty<HostelRoom>();

            return await _context.HostelRooms
                .Where(r =>
                    r.HostelType == hostelType &&
                    r.IsAvailable &&
                    r.AvailableSpace > 0)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostelRoom>>
            GetRoomsForStudentAsync(
                int studentId)
        {
            var application = await _context.HostelApplications
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.ApplicationDate)
                .FirstOrDefaultAsync();

            if (application == null)
                return Enumerable.Empty<HostelRoom>();

            return await _context.HostelRooms
                .Where(r =>
                    r.Id == application.HostelRoomId)
                .ToListAsync();
        }
    }
}