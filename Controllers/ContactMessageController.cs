// Controllers/ContactMessageController.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers
{
    public class ContactMessageController : Controller
    {
        private readonly IContactMessageService
            _messageService;

        public ContactMessageController(
            IContactMessageService messageService)
        {
            _messageService = messageService;
        }

        // ==========================
        // STUDENT SEND MESSAGE
        // ==========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(
            ContactMessageDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] =
                    "Please fill all fields correctly.";

                return RedirectToAction(
                    "Contact",
                    "Home");
            }

            await _messageService.SendAsync(model);

            TempData["Success"] =
                "Your message has been sent successfully.";

            return RedirectToAction(
                "Index",
                "Home");
        }

        // ==========================
        // ADMIN VIEW ALL MESSAGES
        // ==========================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(
            string? search)
        {
            var messages =
                await _messageService
                    .GetMessagesAsync(search);

            ViewBag.Search = search;

            return View(messages);
        }

        // ==========================
        // VIEW MESSAGE
        // ==========================

       [Authorize(Roles = "Admin")]
public async Task<IActionResult> Details(int id)
{
    var message =
        await _messageService.GetByIdAsync(id);

    if (message == null)
        return NotFound();

    if (!message.IsRead)
    {
        await _messageService.MarkReadAsync(id);
        message.IsRead = true;
    }

    return View(message);
}

        // ==========================
        // DELETE MESSAGE
        // ==========================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            Delete(int id)
        {
            var success =
                await _messageService.DeleteAsync(id);

            if (!success)
                return NotFound();

            TempData["Success"] =
                "Message deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // MARK AS UNREAD
        // ==========================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            MarkUnread(int id)
        {
            var success =
                await _messageService
                    .MarkUnreadAsync(id);

            if (!success)
                return NotFound();

            TempData["Success"] =
                "Message marked as unread.";

            return RedirectToAction(nameof(Index));
        }
    }
}