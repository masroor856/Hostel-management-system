using System.Diagnostics;
using HostelManagementSystem.Data;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // LANDING PAGE
    // ==========================================

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.TotalStudents =
            _context.Students.Count();

        ViewBag.TotalRooms =
            _context.HostelRooms.Count();

        ViewBag.AvailableRooms =
            _context.HostelRooms.Count(r =>
                r.IsAvailable &&
                r.AvailableSpace > 0);

        ViewBag.AllocatedRooms =
            _context.RoomAllocations.Count(r =>
                r.IsActive);

        ViewBag.BoysRooms =
            _context.HostelRooms.Count(r =>
                r.HostelType == "Boys");

        ViewBag.GirlsRooms =
            _context.HostelRooms.Count(r =>
                r.HostelType == "Girls");

        // Available rooms on landing page
        ViewBag.Rooms =
            _context.HostelRooms
                .Where(r =>
                    r.IsAvailable &&
                    r.AvailableSpace > 0)
                .OrderBy(r => r.RoomNumber)
                .Take(6)
                .ToList();

        return View();
    }

    // ==========================================
    // PUBLIC ROOMS PAGE
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Rooms()
    {
        var rooms = await _context.HostelRooms
            .Where(r =>
                r.IsAvailable &&
                r.AvailableSpace > 0)
            .OrderBy(r => r.HostelType)
            .ThenBy(r => r.RoomNumber)
            .ToListAsync();

        return View(rooms);
    }

    // ==========================================
    // PRIVACY
    // ==========================================

    public IActionResult Privacy()
    {
        return View();
    }

    // ==========================================
    // ERROR
    // ==========================================

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId =
                Activity.Current?.Id
                ?? HttpContext.TraceIdentifier
        });
    }
}