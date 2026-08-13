// Repositories/PaymentRepository.cs

using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync(
            string? search)
        {
            var query = _context.Payments
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.Student)
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.HostelRoom)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.TransactionReference.Contains(search) ||
                    p.HostelApplication.Student.FullName
                        .Contains(search));
            }

            return await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>>
            GetStudentPaymentsAsync(string email)
        {
            return await _context.Payments
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.Student)
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.HostelRoom)
                .Where(p =>
                    p.HostelApplication.Student.Email == email)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.Student)
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.HostelRoom)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
          
          public async Task<IEnumerable<Payment>>
    GetPaymentsByApplicationIdAsync(int applicationId)
{
    return await _context.Payments
        .Include(p => p.HostelApplication)
            .ThenInclude(a => a.Student)
        .Include(p => p.HostelApplication)
            .ThenInclude(a => a.HostelRoom)
        .Where(p =>
            p.HostelApplicationId == applicationId)
        .OrderByDescending(p => p.PaymentDate)
        .ToListAsync();
}
        public async Task<Payment?> GetByReferenceAsync(
            string reference)
        {
            return await _context.Payments
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.Student)
                .Include(p => p.HostelApplication)
                    .ThenInclude(a => a.HostelRoom)
                .FirstOrDefaultAsync(p =>
                    p.TransactionReference == reference);
        }

        public async Task<Payment?> GetPaidSessionAsync(
            int applicationId,
            string session)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p =>
                    p.HostelApplicationId == applicationId &&
                    p.Session == session &&
                    p.Status == "Paid");
        }

        public async Task<Payment?> GetPendingSessionAsync(
            int applicationId,
            string session)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p =>
                    p.HostelApplicationId == applicationId &&
                    p.Session == session &&
                    p.Status == "Pending");
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var payment =
                await _context.Payments.FindAsync(id);

            if (payment != null)
                _context.Payments.Remove(payment);
        }

        public async Task<IEnumerable<Payment>>
            GetExpiredPendingPaymentsAsync()
        {
            return await _context.Payments
                .Where(p =>
                    p.Status == "Pending" &&
                    p.PaymentDate <=
                    DateTime.UtcNow.AddMinutes(-15))
                .ToListAsync();
        }

        public async Task<HostelApplication?>
            GetApprovedApplicationAsync(string email)
        {
            return await _context.HostelApplications
                .Include(a => a.Student)
                .Include(a => a.HostelRoom)
                .FirstOrDefaultAsync(a =>
                    a.Student != null &&
                    a.Student.Email == email &&
                    a.Status == "Accepted");
        }

        public async Task<HostelApplication?>
            GetApplicationByIdAsync(int id)
        {
            return await _context.HostelApplications
                .Include(a => a.Student)
                .Include(a => a.HostelRoom)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<RoomAllocation?>
            GetActiveAllocationAsync(int studentId)
        {
            return await _context.RoomAllocations
                .FirstOrDefaultAsync(r =>
                    r.StudentId == studentId &&
                    r.IsActive);
        }

        public async Task AddAllocationAsync(
            RoomAllocation allocation)
        {
            await _context.RoomAllocations
                .AddAsync(allocation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}