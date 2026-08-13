// StudentDashboardController.cs

using HostelManagementSystem.Data;
using HostelManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers;

[Authorize]
public class StudentDashboardController : Controller
{
    private readonly IStudentDashboardService _studentDashboardService;

    public StudentDashboardController(
        IStudentDashboardService studentDashboardService)
    {
        _studentDashboardService = studentDashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var email = User.Identity?.Name;

        if (string.IsNullOrEmpty(email))
            return Challenge();

        var dto =
            await _studentDashboardService.GetDashboardAsync(email);

        if (dto == null)
        {
            TempData["Error"] = "Student record not found.";
            return RedirectToAction("Index", "Home");
        }

        return View(dto);
    }
}