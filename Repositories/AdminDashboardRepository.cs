using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var paidPayments = _context.Payments
                .Where(p => p.Status == "Paid");

            return new AdminDashboardViewModel
            {
                // Students
                TotalStudents = await _context.Students.CountAsync(),

                // Applications
                TotalApplications = await _context.HostelApplications.CountAsync(),

                PendingApplications = await _context.HostelApplications
                    .CountAsync(a => a.Status == "Pending"),

                AcceptedApplications = await _context.HostelApplications
                    .CountAsync(a => a.Status == "Accepted"),

                RejectedApplications = await _context.HostelApplications
                    .CountAsync(a => a.Status == "Rejected"),

                // Payments
                PendingPayments = await _context.Payments
                    .CountAsync(p => p.Status == "Pending"),

                TotalPayments = await paidPayments.CountAsync(),

                TotalRevenue = await paidPayments
                    .SumAsync(p => (decimal?)p.Amount) ?? 0,

                // Rooms
                TotalRooms = await _context.HostelRooms.CountAsync(),

                AvailableRooms = await _context.HostelRooms
                    .SumAsync(r => r.AvailableSpace),

                FullRooms = await _context.HostelRooms
                    .CountAsync(r => !r.IsAvailable),

                BoysRooms = await _context.HostelRooms
                    .CountAsync(r => r.HostelType == "Boys"),

                GirlsRooms = await _context.HostelRooms
                    .CountAsync(r => r.HostelType == "Girls"),

                // Beds
                OccupiedBeds = await _context.HostelRooms
                    .SumAsync(r => r.OccupiedSpace),

                TotalBedSpace = await _context.HostelRooms
                    .SumAsync(r => r.Capacity),

                // Messages
                UnreadMessages = await _context.ContactMessages
                    .CountAsync(m => !m.IsRead),

                RecentMessages = await _context.ContactMessages
                    .OrderBy(m => m.IsRead)
                    .ThenByDescending(m => m.DateSent)
                    .Take(5)
                    .ToListAsync(),

                // Recent Applications
                RecentApplications = await _context.HostelApplications
                    .Include(a => a.Student)
                    .Include(a => a.HostelRoom)
                    .OrderByDescending(a => a.ApplicationDate)
                    .Take(5)
                    .ToListAsync(),

                // Recent Payments
                RecentPayments = await paidPayments
                    .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.Student)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .ToListAsync(),

                // Recent Rooms
                RecentRooms = await _context.HostelRooms
                    .OrderBy(r => r.RoomNumber)
                    .Take(5)
                    .ToListAsync()
            };
        }
    }
}