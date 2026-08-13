// Repositories/HostelRoomRepository.cs
using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class HostelRoomRepository : IHostelRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public HostelRoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HostelRoom>> GetAllAsync()
        {
            return await _context.HostelRooms
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<HostelRoom?> GetByIdAsync(int id)
        {
            return await _context.HostelRooms
                .Include(r => r.RoomAllocations)
                .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<HostelRoom>> SearchAsync(
            string? search,
            string? status)
        {
            var rooms = _context.HostelRooms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                rooms = rooms.Where(r =>
                    r.RoomNumber.Contains(search) ||
                    r.HostelType.Contains(search));
            }

            if (status == "Available")
            {
                rooms = rooms.Where(r => r.IsAvailable);
            }
            else if (status == "Full")
            {
                rooms = rooms.Where(r => !r.IsAvailable);
            }

            return await rooms
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task AddAsync(HostelRoom room)
        {
            _context.HostelRooms.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HostelRoom room)
        {
            _context.HostelRooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _context.HostelRooms.FindAsync(id);

            if (room == null)
                return;

            _context.HostelRooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }
}