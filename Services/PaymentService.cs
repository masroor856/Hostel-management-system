using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;

namespace HostelManagementSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly IPaystackService _paystackService;

        public PaymentService(
            IPaymentRepository repository,
            IPaystackService paystackService)
        {
            _repository = repository;
            _paystackService = paystackService;
        }

        // =========================================================
        // GET ALL PAYMENTS
        // =========================================================

        public async Task<IEnumerable<PaymentDto>> GetPayments(string? search)
        {
            await UpdateExpiredPayments();

            var payments =
                await _repository.GetAllAsync(search);

            return payments
                .Select(Map)
                .ToList();
        }

        // =========================================================
        // GET STUDENT PAYMENTS
        // =========================================================

        public async Task<IEnumerable<PaymentDto>> GetMyPayments(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Enumerable.Empty<PaymentDto>();

            await UpdateExpiredPayments();

            var payments =
                await _repository.GetStudentPaymentsAsync(email);

            return payments
                .Select(Map)
                .ToList();
        }

        // =========================================================
        // GET PAYMENT
        // =========================================================

        public async Task<PaymentDto?> GetPayment(int id)
        {
            await UpdateExpiredPayments();

            var payment =
                await _repository.GetByIdAsync(id);

            if (payment == null)
                return null;

            return Map(payment);
        }

        // =========================================================
        // GET PAYMENT BY REFERENCE
        // =========================================================

        public async Task<PaymentDto?> GetPaymentByReference(
            string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return null;

            var payment =
                await _repository.GetByReferenceAsync(reference);

            if (payment == null)
                return null;

            return Map(payment);
        }

        // =========================================================
        // GET PAID SESSIONS
        // =========================================================

        public async Task<IEnumerable<string>> GetPaidSessions(
            int applicationId)
        {
            var payments =
                await _repository
                    .GetPaymentsByApplicationIdAsync(applicationId);

            return payments
                .Where(p =>
                    p.Status == "Paid" &&
                    !string.IsNullOrWhiteSpace(p.Session))
                .Select(p => p.Session)
                .Distinct()
                .ToList();
        }

        // =========================================================
        // GET APPROVED APPLICATION
        // =========================================================

        public async Task<int?> GetApprovedApplicationId(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var application =
                await _repository
                    .GetApprovedApplicationAsync(email);

            return application?.Id;
        }

        // =========================================================
        // CREATE PAYMENT
        // =========================================================
public async Task<string> CreatePayment(
    CreatePaymentDto dto,
    string email)
{
    var application =
        await _repository.GetApprovedApplicationAsync(email);

    if (application == null)
        throw new InvalidOperationException(
            "You do not have an approved hostel application yet.");

    if (dto.HostelApplicationId != application.Id)
        throw new UnauthorizedAccessException();

    if (string.IsNullOrWhiteSpace(dto.Session))
        throw new InvalidOperationException(
            "Please select a session.");

    if (application.HostelRoom == null)
        throw new InvalidOperationException(
            "Your application has no hostel room assigned.");

    var paid =
        await _repository.GetPaidSessionAsync(
            application.Id,
            dto.Session);

    if (paid != null)
        throw new InvalidOperationException(
            $"You have already paid for {dto.Session}.");

    var pending =
        await _repository.GetPendingSessionAsync(
            application.Id,
            dto.Session);

    if (pending != null)
        throw new InvalidOperationException(
            $"You already have a pending payment for {dto.Session}.");

    var payment = new Payment
    {
        HostelApplicationId = application.Id,

        Amount = application.HostelRoom.Price,

        Session = dto.Session,

        PaymentDate = DateTime.UtcNow,

        Status = "Pending",

        TransactionReference =
            Guid.NewGuid().ToString("N")
    };

    await _repository.AddAsync(payment);
    await _repository.SaveChangesAsync();

    var paymentUrl =
        await _paystackService.InitializePayment(
            payment,
            application.Student.Email);

    if (string.IsNullOrWhiteSpace(paymentUrl))
    {
        payment.Status = "Failed";

        await _repository.UpdateAsync(payment);
        await _repository.SaveChangesAsync();

        throw new InvalidOperationException(
            "Unable to initialize payment with Paystack.");
    }

    return paymentUrl;
}
        public async Task<Payment> GetPaymentForCreation(
    int applicationId)
{
    var application =
        await _repository.GetApplicationByIdAsync(
            applicationId);

    if (application == null)
        throw new KeyNotFoundException(
            "Hostel application not found.");

    if (application.HostelRoom == null)
        throw new InvalidOperationException(
            "No hostel room is assigned to your application.");

    return new Payment
    {
        HostelApplicationId =
            application.Id,

        Amount =
            application.HostelRoom.Price,

        Session = string.Empty,

        Status = "Pending",

        PaymentDate = DateTime.UtcNow,

        HostelApplication = application
    };
}

        // =========================================================
        // VERIFY PAYMENT
        // =========================================================

        public async Task<bool> VerifyPayment(
            string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return false;

            var payment =
                await _repository
                    .GetByReferenceAsync(reference);

            if (payment == null)
                return false;

            if (payment.Status == "Paid")
                return true;

            var application =
                payment.HostelApplication;

            if (application == null)
                return false;

            if (application.Student == null)
                return false;

            if (application.HostelRoom == null)
                return false;

            var verified =
                await _paystackService
                    .VerifyPayment(reference);

            if (!verified)
            {
                payment.Status = "Failed";

                await _repository.UpdateAsync(payment);
                await _repository.SaveChangesAsync();

                return false;
            }

            payment.Status = "Paid";
            payment.PaymentDate = DateTime.UtcNow;

            var existingAllocation =
                await _repository
                    .GetActiveAllocationAsync(
                        application.StudentId);

            if (existingAllocation == null)
            {
                var room =
                    application.HostelRoom;

                if (room.AvailableSpace <= 0)
                {
                    payment.Status = "Failed";

                    await _repository.UpdateAsync(payment);
                    await _repository.SaveChangesAsync();

                    return false;
                }

                var allocation = new RoomAllocation
                {
                    StudentId =
                        application.StudentId,

                    HostelRoomId =
                        application.HostelRoomId,

                    AllocationDate =
                        DateTime.UtcNow,

                    IsActive = true
                };

                await _repository
                    .AddAllocationAsync(allocation);

                room.OccupiedSpace++;

                room.AvailableSpace =
                    Math.Max(
                        0,
                        room.Capacity -
                        room.OccupiedSpace);

                room.IsAvailable =
                    room.AvailableSpace > 0;
            }

            await _repository.UpdateAsync(payment);
            await _repository.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // UPDATE PAYMENT
        // =========================================================

        public async Task UpdatePayment(
            UpdatePaymentDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var payment =
                await _repository.GetByIdAsync(dto.Id);

            if (payment == null)
                throw new KeyNotFoundException(
                    "Payment not found.");

            payment.Amount =
                dto.Amount;

            payment.Session =
                dto.Session;

            payment.Status =
                dto.Status;

            await _repository.UpdateAsync(payment);
            await _repository.SaveChangesAsync();
        }

        // =========================================================
        // DELETE PAYMENT
        // =========================================================

        public async Task DeletePayment(int id)
        {
            var payment =
                await _repository.GetByIdAsync(id);

            if (payment == null)
                throw new KeyNotFoundException(
                    "Payment not found.");

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }

        // =========================================================
        // EXPIRE OLD PENDING PAYMENTS
        // =========================================================

        public async Task UpdateExpiredPayments()
        {
            var payments =
                await _repository
                    .GetExpiredPendingPaymentsAsync();

            if (payments == null || !payments.Any())
                return;

            foreach (var payment in payments)
            {
                payment.Status = "Failed";
            }

            await _repository.SaveChangesAsync();
        }

        // =========================================================
        // DTO MAPPING
        // =========================================================

        private static PaymentDto Map(Payment p)
{
    return new PaymentDto
    {
        Id = p.Id,

        HostelApplicationId =
            p.HostelApplicationId,

        Amount = p.Amount,

        Session =
            p.Session ?? string.Empty,

        Status =
            p.Status ?? string.Empty,

        PaymentDate =
            p.PaymentDate,

        TransactionReference =
            p.TransactionReference ?? string.Empty,

        StudentName =
            p.HostelApplication?.Student?.FullName
            ?? string.Empty,

        StudentEmail =
            p.HostelApplication?.Student?.Email
            ?? string.Empty,

        RoomNumber =
            p.HostelApplication?.HostelRoom?.RoomNumber
            ?? string.Empty
    };
}
    }
}