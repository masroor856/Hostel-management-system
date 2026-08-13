// Controllers/PaymentController.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // =========================
        // ADMIN - ALL PAYMENTS
        // =========================

     [Authorize(Roles = "Admin")]
public async Task<IActionResult> Index(string? search)
{
    var payments =
        await _paymentService.GetPayments(search);

    ViewBag.Search = search;

    var data = payments
        .Select(MapToModel)
        .ToList();

    return View(data);
}

        // =========================
        // STUDENT - MY PAYMENTS
        // =========================

        [Authorize]
        public async Task<IActionResult> MyPayments()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                return Challenge();

            var payments =
                await _paymentService.GetMyPayments(email);

            var data = payments
                .Select(MapToModel)
                .ToList();

            return View(data);
        }

        // =========================
        // DETAILS
        // =========================

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var payment =
                await _paymentService.GetPayment(id);

            if (payment == null)
                return NotFound();

            return View(MapToModel(payment));
        }

        // =========================
        // RECEIPT
        // =========================

        [Authorize]
        public async Task<IActionResult> Receipt(int id)
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                return Challenge();

            var payments =
                await _paymentService.GetMyPayments(email);

            var payment =
                payments.FirstOrDefault(p => p.Id == id);

            if (payment == null)
                return NotFound();

            return View(MapToModel(payment));
        }

        // =========================
        // CREATE
        // =========================

      [Authorize]
[HttpGet]
public async Task<IActionResult> Create()
{
    var email = User.Identity?.Name;

    if (string.IsNullOrWhiteSpace(email))
        return Challenge();

    var applicationId =
        await _paymentService.GetApprovedApplicationId(email);

    if (applicationId == null)
    {
        TempData["Error"] =
            "You do not have an approved hostel application yet.";

        return RedirectToAction(
            "Index",
            "StudentDashboard");
    }

    var paidSessions =
        await _paymentService.GetPaidSessions(
            applicationId.Value);

    ViewBag.PaidSessions =
        paidSessions.ToList();

    var payment =
        await _paymentService.GetPaymentForCreation(
            applicationId.Value);

    if (payment == null)
    {
        TempData["Error"] =
            "Unable to load your hostel payment information.";

        return RedirectToAction(
            "Index",
            "StudentDashboard");
    }

    return View(payment);
}

      [Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(
    CreatePaymentDto dto)
{
    var email = User.Identity?.Name;

    if (string.IsNullOrWhiteSpace(email))
        return Challenge();

    if (!ModelState.IsValid)
    {
        var paidSessions =
            await _paymentService.GetPaidSessions(
                dto.HostelApplicationId);

        ViewBag.PaidSessions =
            paidSessions.ToList();

        var payment =
            await _paymentService.GetPaymentForCreation(
                dto.HostelApplicationId);

        payment.Session = dto.Session;

        return View(payment);
    }

    try
    {
        var paymentUrl =
            await _paymentService.CreatePayment(
                dto,
                email);

        return Redirect(paymentUrl);
    }
    catch (InvalidOperationException ex)
    {
        TempData["Error"] = ex.Message;

        return RedirectToAction(nameof(Create));
    }
    catch (UnauthorizedAccessException)
    {
        TempData["Error"] =
            "You are not authorized to make this payment.";

        return RedirectToAction(nameof(Create));
    }
}
        // =========================
        // VERIFY
        // =========================

        [Authorize]
        public async Task<IActionResult> Verify(
            string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return BadRequest();

            var success =
                await _paymentService
                    .VerifyPayment(reference);

            if (!success)
            {
                TempData["Error"] =
                    "Payment failed or was not completed.";

                return RedirectToAction(
                    "Index",
                    "StudentDashboard");
            }

            TempData["Success"] =
                "Payment verified successfully. Your room has been allocated.";

            return RedirectToAction(
                "Index",
                "StudentDashboard");
        }

        // =========================
        // EDIT
        // =========================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var payment =
                await _paymentService.GetPayment(id);

            if (payment == null)
                return NotFound();

            return View(new UpdatePaymentDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Session = payment.Session,
                Status = payment.Status
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdatePaymentDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            await _paymentService.UpdatePayment(dto);

            TempData["Success"] =
                "Payment updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var payment =
                await _paymentService.GetPayment(id);

            if (payment == null)
                return NotFound();

            return View(MapToModel(payment));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            await _paymentService.DeletePayment(id);

            TempData["Success"] =
                "Payment deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

     private static Payment MapToModel(PaymentDto dto)
{
    var student = new Student
    {
        FullName = dto.StudentName,
        Email = dto.StudentEmail
    };

    var room = new HostelRoom
    {
        RoomNumber = dto.RoomNumber
    };

    var application = new HostelApplication
    {
        Id = dto.HostelApplicationId,
        Student = student,
        HostelRoom = room
    };

    return new Payment
    {
        Id = dto.Id,
        HostelApplicationId = dto.HostelApplicationId,
        Amount = dto.Amount,
        Session = dto.Session,
        Status = dto.Status,
        PaymentDate = dto.PaymentDate,
        TransactionReference = dto.TransactionReference,
        HostelApplication = application
    };
}
        private async Task<string> GetPaymentUrl(
            PaymentDto payment)
        {
            return await Task.FromResult(
                Url.Action(
                    nameof(Verify),
                    "Payment",
                    new
                    {
                        reference =
                            payment.TransactionReference
                    }) ?? "/");
        }
    }
}