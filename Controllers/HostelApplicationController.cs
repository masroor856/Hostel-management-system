// Controllers/HostelApplicationController.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers
{
    [Authorize]
    public class HostelApplicationController : Controller
    {
        private readonly IHostelApplicationService _service;

        public HostelApplicationController(
            IHostelApplicationService service)
        {
            _service = service;
        }

        // =========================
        // ADMIN - INDEX
        // =========================

       [Authorize(Roles = "Admin")]
public async Task<IActionResult> Index(
    string? search,
    string? status)
{
    var applications =
        await _service.SearchAsync(search, status);

    ViewBag.Search = search;
    ViewBag.Status = status;

      
    var data = applications.Select(a => new HostelApplication
    {
        Id = a.Id,
        StudentId = a.StudentId,
        HostelRoomId = a.HostelRoomId.Value,
        ApplicationDate = a.ApplicationDate,
        Status = a.Status,

        Student = a.Student == null
            ? new Student()
            : new Student
            {
                Id = a.Student.Id,
                FullName = a.Student.FullName,
                Email = a.Student.Email,
                PhoneNumber = a.Student.PhoneNumber,
                Gender = a.Student.Gender,
                Department = a.Student.Department,
                ProfilePicture = a.Student.ProfilePicture
            },

        HostelRoom = a.HostelRoom == null
            ? new HostelRoom()
            : new HostelRoom
            {
                Id = a.HostelRoom.Id,
                RoomNumber = a.HostelRoom.RoomNumber,
                HostelType = a.HostelRoom.HostelType,
                Capacity = a.HostelRoom.Capacity,
                OccupiedSpace = a.HostelRoom.OccupiedSpace,
                AvailableSpace = a.HostelRoom.AvailableSpace,
                IsAvailable = a.HostelRoom.IsAvailable,
                Price = a.HostelRoom.Price
            }
    }).ToList();

    return View(data);
}

        // =========================
        // DETAILS
        // =========================

        public async Task<IActionResult> Details(int id)
        {
            var application =
                await _service.GetByIdAsync(id);

            if (application == null)
                return NotFound();

            return View(application);
        }

        // =========================
        // EDIT
        // =========================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var application =
                await _service.GetByIdAsync(id);

            if (application == null)
                return NotFound();

            ViewBag.Rooms =
                await _service.GetAvailableRoomsAsync();

            return View(application);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            HostelApplicationDto model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Rooms =
                    await _service.GetAvailableRoomsAsync();

                return View(model);
            }

            var updated =
                await _service.UpdateAsync(model);

            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var application =
                await _service.GetByIdAsync(id);

            if (application == null)
                return NotFound();

            return View(application);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted =
                await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // APPROVE / REJECT
        // =========================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest();

            var updated =
                await _service.UpdateStatusAsync(id, status);

            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // STUDENT APPLY
        // =========================

        [HttpGet]
        public async Task<IActionResult> Apply()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Home");

            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                return Challenge();

            var student =
                await _service.GetStudentByEmail(email);

            if (student == null)
                return RedirectToAction("Register", "Account");

            ViewBag.Rooms =
                await _service.GetRoomsForStudentAsync(student.Id);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(
            HostelApplicationDto model)
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Home");

            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                return Challenge();

            var student =
                await _service.GetStudentByEmail(email);

            if (student == null)
                return RedirectToAction("Register", "Account");

            var existing =
                await _service.HasExistingApplicationAsync(
                    student.Id);

            if (existing)
            {
                ModelState.AddModelError(
                    "",
                    "You have already applied.");

                ViewBag.Rooms =
                    await _service.GetRoomsForStudentAsync(
                        student.Id);

                return View(model);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Rooms =
                    await _service.GetRoomsForStudentAsync(
                        student.Id);

                return View(model);
            }

            model.StudentId = student.Id;
            model.Status = "Pending";
            model.ApplicationDate = DateTime.Now;

            var created =
                await _service.CreateAsync(model);

            if (!created)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to submit application.");

                ViewBag.Rooms =
                    await _service.GetRoomsForStudentAsync(
                        student.Id);

                return View(model);
            }

            return RedirectToAction(nameof(Confirm));
        }

        // =========================
        // CONFIRM
        // =========================

        public IActionResult Confirm()
        {
            return View();
        }
    }
}