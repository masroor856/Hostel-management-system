// Repositories/RoomAllocationRepository.cs

using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class RoomAllocationRepository
        : IRoomAllocationRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomAllocationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomAllocation>>
            GetAllAsync()
        {
            return await _context.RoomAllocations
                .Include(r => r.Student)
                .Include(r => r.HostelRoom)
                .OrderByDescending(r => r.AllocationDate)
                .ToListAsync();
        }

        public async Task<RoomAllocation?>
            GetByIdAsync(int id)
        {
            return await _context.RoomAllocations
                .Include(r => r.Student)
                .Include(r => r.HostelRoom)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RoomAllocation?>
            GetActiveByStudentAsync(int studentId)
        {
            return await _context.RoomAllocations
                .FirstOrDefaultAsync(r =>
                    r.StudentId == studentId &&
                    r.IsActive);
        }

        public async Task<Student?>
            GetStudentByIdAsync(int studentId)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task<HostelRoom?>
            GetRoomByIdAsync(int roomId)
        {
            return await _context.HostelRooms
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<HostelApplication?>
            GetAcceptedApplicationAsync(int studentId)
        {
            return await _context.HostelApplications
                .Include(a => a.Student)
                .Include(a => a.HostelRoom)
                .FirstOrDefaultAsync(a =>
                    a.StudentId == studentId &&
                    a.Status == "Accepted");
        }

        public async Task<IEnumerable<Student>>
            GetStudentsAsync()
        {
            return await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostelRoom>>
            GetAvailableRoomsAsync()
        {
            return await _context.HostelRooms
                .Where(r => r.AvailableSpace > 0)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostelRoom>>
            GetAllRoomsAsync()
        {
            return await _context.HostelRooms
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task AddAsync(
            RoomAllocation allocation)
        {
            await _context.RoomAllocations
                .AddAsync(allocation);
        }

        public async Task UpdateAsync(
            RoomAllocation allocation)
        {
            _context.RoomAllocations.Update(allocation);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(
            RoomAllocation allocation)
        {
            _context.RoomAllocations.Remove(allocation);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}