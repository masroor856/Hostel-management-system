// Controllers/HostelRoomController.cs
using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HostelRoomController : Controller
    {
        private readonly IHostelRoomService _roomService;

        public HostelRoomController(
            IHostelRoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task<IActionResult> Index(
            string? search,
            string? status)
        {
            var rooms = await _roomService.SearchRooms(
                search,
                status);

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(rooms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateHostelRoomDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _roomService.CreateRoom(dto);

            TempData["Success"] =
                "Room created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomService.GetRoomById(id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetRoomById(id);

            if (room == null)
                return NotFound();

            var dto = new UpdateHostelRoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                HostelType = room.HostelType,
                Capacity = room.Capacity
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateHostelRoomDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            await _roomService.UpdateRoom(dto);

            TempData["Success"] =
                "Room updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomService.GetRoomById(id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roomService.DeleteRoom(id);

            TempData["Success"] =
                "Room deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}