// Repositories/ReportsRepository.cs

using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalStudentsAsync()
        {
            return await _context.Students.CountAsync();
        }

        public async Task<int> GetTotalRoomsAsync()
        {
            return await _context.HostelRooms.CountAsync();
        }

        public async Task<int> GetTotalApplicationsAsync()
        {
            return await _context.HostelApplications.CountAsync();
        }

        public async Task<int> GetTotalPaymentsAsync()
        {
            return await _context.Payments.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == "Paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;
        }

        public async Task<int> GetAvailableRoomsAsync()
        {
            return await _context.HostelRooms
                .CountAsync(r => r.IsAvailable);
        }

        public async Task<int> GetOccupiedRoomsAsync()
        {
            return await _context.HostelRooms
                .CountAsync(r => !r.IsAvailable);
        }

        public async Task<IEnumerable<Payment>>
            GetRecentPaymentsAsync(int count)
        {
            return await _context.Payments
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.Student)
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.HostelRoom)
                .OrderByDescending(p => p.PaymentDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostelApplication>>
            GetRecentApplicationsAsync(int count)
        {
            return await _context.HostelApplications
                .Include(a => a.Student)
                .Include(a => a.HostelRoom)
                .OrderByDescending(a => a.ApplicationDate)
                .Take(count)
                .ToListAsync();
        }
    }
}