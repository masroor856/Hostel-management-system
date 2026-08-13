// Controllers/RoomAllocationController.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoomAllocationController : Controller
    {
        private readonly IRoomAllocationService _service;

        public RoomAllocationController(
            IRoomAllocationService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var allocations =
                await _service.GetAllAsync();

            return View(allocations);
        }

        public async Task<IActionResult> Details(
            int id)
        {
            var allocation =
                await _service.GetByIdAsync(id);

            if (allocation == null)
                return NotFound();

            return View(allocation);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCreateData();

            return View(
                new CreateRoomAllocationDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateRoomAllocationDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateData();
                return View(dto);
            }

            var errors = new List<string>();

            var success =
                await _service.CreateAsync(
                    dto,
                    errors);

            if (!success)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(
                        "",
                        error);

                await LoadCreateData();

                return View(dto);
            }

            TempData["Success"] =
                "Room allocated successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(
            int id)
        {
            var allocation =
                await _service.GetEditAsync(id);

            if (allocation == null)
                return NotFound();

            ViewBag.Students =
                await GetStudents();

            ViewBag.Rooms =
                await GetRooms();

            return View(
                new CreateRoomAllocationDto
                {
                    StudentId =
                        allocation.StudentId,

                    HostelRoomId =
                        allocation.HostelRoomId
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            CreateRoomAllocationDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Students =
                    await GetStudents();

                ViewBag.Rooms =
                    await GetRooms();

                return View(dto);
            }

            var errors = new List<string>();

            var success =
                await _service.UpdateAsync(
                    id,
                    dto,
                    errors);

            if (!success)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(
                        "",
                        error);

                ViewBag.Students =
                    await GetStudents();

                ViewBag.Rooms =
                    await GetRooms();

                return View(dto);
            }

            TempData["Success"] =
                "Room allocation updated successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(
            int id)
        {
            var allocation =
                await _service.GetDeleteAsync(id);

            if (allocation == null)
                return NotFound();

            return View(allocation);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            var success =
                await _service.DeleteAsync(id);

            if (!success)
                return NotFound();

            TempData["Success"] =
                "Room allocation deleted successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        private async Task LoadCreateData()
        {
            ViewBag.Students =
                await GetStudents();

            ViewBag.Rooms =
                await GetAvailableRooms();
        }

        private async Task<object>
            GetStudents()
        {
            var repository =
                HttpContext.RequestServices
                    .GetRequiredService<
                        IRoomAllocationRepository>();

            return await repository
                .GetStudentsAsync();
        }

        private async Task<object>
            GetRooms()
        {
            var repository =
                HttpContext.RequestServices
                    .GetRequiredService<
                        IRoomAllocationRepository>();

            return await repository
                .GetAllRoomsAsync();
        }

        private async Task<object>
            GetAvailableRooms()
        {
            var repository =
                HttpContext.RequestServices
                    .GetRequiredService<
                        IRoomAllocationRepository>();

            return await repository
                .GetAvailableRoomsAsync();
        }
    }
}